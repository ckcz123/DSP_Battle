using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;


namespace DSP_Battle
{
    class RemoveEntities
    {
        public static Dictionary<int, List<Tuple<Vector3, int>>> pendingDestroyedEntities; // planet id; upos; range
        private static bool removingComponets = false;
        public static ConcurrentDictionary<int, object> distroyedStation = new ConcurrentDictionary<int, object>();

        public static void Init()
        {
            distroyedStation = new ConcurrentDictionary<int, object>();
            pendingDestroyedEntities = new Dictionary<int, List<Tuple<Vector3, int>>>();
        }

        public static void Add(EnemyShip ship, StationComponent station)
        {
            PlanetFactory planetFactory = GameMain.galaxy.PlanetById(ship.shipData.planetB).factory;
            Vector3 stationPos = planetFactory.entityPool[station.entityId].pos;
            removingComponets = true;
            UIBattleStatistics.RegisterIntercept(ship, 0);
            RemoveStation(planetFactory, station);
            removingComponets = false;

            if (!pendingDestroyedEntities.ContainsKey(ship.shipData.planetB))
            {
                pendingDestroyedEntities.Add(ship.shipData.planetB, new List<Tuple<Vector3, int>>());
            }
            pendingDestroyedEntities[ship.shipData.planetB].Add(new Tuple<Vector3, int>(stationPos, ship.damageRange));
        }

        public static void CheckPendingDestroyedEntities()
        {
            foreach (var entry in pendingDestroyedEntities)
            {
                removingComponets = true;
                PlanetData planet = GameMain.galaxy.PlanetById(entry.Key);
                PlanetFactory planetFactory = planet.factory;
                bool freePhysics = false;
                int done = 0;
                int idx = 0;
                try
                {
                    for (int i = 0; i < planetFactory.entityPool.Length; ++i)
                    {
                        idx = i;
                        done = 0;
                        if (planetFactory.entityPool[i].isNull ||
                            planetFactory.entityPool[i].beltId != 0 ||
                            planetFactory.entityPool[i].inserterId != 0 ||
                            !entry.Value.Any((e) => (planetFactory.entityPool[i].pos - e.Item1).magnitude <= e.Item2))
                        {
                            continue;
                        }

                        if (planet.physics == null)
                        {
                            freePhysics = true;
                            planet.physics = new PlanetPhysics(planet);
                            planet.physics.Init();
                        }

                        if (planetFactory.entityPool[i].stationId > 0)
                        {
                            RemoveStation(planetFactory, planetFactory.transport.stationPool[planetFactory.entityPool[i].stationId]);
                        }
                        else if (Configs.difficulty == 0 || (Configs.difficulty == -1 && planetFactory.entityPool[i].powerNodeId > 0))
                        {
                            UIBattleStatistics.RegisterOtherBuildingLost();
                            EntityToPrebuild(planetFactory, i);
                        }
                        else if (Configs.difficulty == 1)
                        {
                            UIBattleStatistics.RegisterOtherBuildingLost();
                            RemoveEntity(planetFactory, i);
                        }
                    }
                }
                catch (Exception) { Debug.Log($"removing err in i = {idx} with done {done}"); }
                finally { removingComponets = false; }
                if (freePhysics)
                {
                    planet.physics.Free();
                    planet.physics = null;
                }
            }
            pendingDestroyedEntities.Clear();
        }

        private static void RemoveStation(PlanetFactory factory, StationComponent station)
        {
            if (!EnemyShips.ValidStellarStation(station) || !removingComponets) return;

            for (int slot = 0; slot < station.storage.Length; slot++) //资源损失
            {
                if (Relic.HaveRelic(2, 2)) // relic2-2 极限一换一加经验
                {
                    Rank.AddExp(station.storage[slot].count / 100);
                }
                UIBattleStatistics.RegisterResourceLost(station.storage[slot].count);
            }
            if (Relic.HaveRelic(2, 2) && Configs.difficulty >= 0) // relic2-2 极限一换一加经验 物流塔被毁 简单模式无效因为没被毁
            {
                Rank.AddExp(200);
            }
            UIBattleStatistics.RegisterResourceLost(station.warperCount + station.idleShipCount + station.workShipCount + station.idleDroneCount + station.workDroneCount);
            distroyedStation[station.gid] = 0;

            // 破坏资源但不损毁物流塔
            for (var i = 0; i < station.storage.Length; ++i)
            {
                station.storage[i].count = 0;
                station.storage[i].inc = 0;
                station.storage[i].localOrder = 0;
                station.storage[i].remoteOrder = 0;
            }

            //station.idleDroneCount = 0;
            //station.workDroneCount = 0;
            //Array.Clear(station.workDroneDatas, 0, station.workDroneDatas.Length);
            //Array.Clear(station.workDroneOrders, 0, station.workDroneOrders.Length);
            //station.idleShipCount = 0;
            //station.workShipCount = 0;
            //station.workShipIndices = 0;
            //Array.Clear(station.workShipDatas, 0, station.workShipDatas.Length);
            //Array.Clear(station.workShipOrders, 0, station.workShipOrders.Length);
            //station.energy = 0;
            //station.energyPerTick = 0;
            //station.warperCount = 0;

            //factory.transport.RefreshStationTraffic(station.id);
            //GameMain.data.galacticTransport.RefreshTraffic(station.gid);

            bool freePhysics = false;
            if (factory.planet.physics == null)
            {
                freePhysics = true;
                factory.planet.physics = new PlanetPhysics(factory.planet);
                factory.planet.physics.Init();
            }

            if (Configs.difficulty == 0)
            {
                UIBattleStatistics.RegisterStationLost();
                EntityToPrebuild(factory, station.entityId);
            }
            else if (Configs.difficulty == 1)
            {
                UIBattleStatistics.RegisterStationLost();
                RemoveEntity(factory, station.entityId);
            }
            if (freePhysics)
            {
                factory.planet.physics.Free();
                factory.planet.physics = null;
            }
        }


        private static void RemoveEntity(PlanetFactory factory, int entityId)
        {
            if (!removingComponets)
            {
                return;
            }
            try
            {
                if (entityId < 0 || entityId >= factory.entityPool.Length || factory.entityPool[entityId].isNull) return;
                int labId = factory.entityPool[entityId].labId;
                if (labId != 0 && labId < factory.factorySystem.labPool.Length && factory.factorySystem.labPool[labId].id != 0)
                {
                    labId = factory.factorySystem.labPool[labId].nextLabId;
                    if (labId != 0 && labId < factory.factorySystem.labPool.Length && factory.factorySystem.labPool[labId].id != 0)
                    {
                        RemoveEntity(factory, factory.factorySystem.labPool[labId].entityId);
                    }
                }

                int storageId = factory.entityPool[entityId].storageId;
                if (storageId != 0 && storageId < factory.factoryStorage.storagePool.Length && factory.factoryStorage.storagePool[storageId].id != 0)
                {
                    StorageComponent nextStorage = factory.factoryStorage.storagePool[storageId].nextStorage;
                    if (nextStorage != null)
                    {
                        RemoveEntity(factory, nextStorage.entityId);
                    }
                }

                factory.RemoveEntityWithComponents(entityId);
            }
            catch
            {
            }
        }

        // Source: https://gist.github.com/starfi5h/2d52f7959892467ae46599317ce84f63
        private static void EntityToPrebuild(PlanetFactory factory, int entityId)
        {
            if (!removingComponets) return;
            try
            {
                if (entityId < 0 || entityId >= factory.entityPool.Length || factory.entityPool[entityId].isNull) return;
                if (factory.entityPool[entityId].beltId > 0 || factory.entityPool[entityId].inserterId > 0 || factory.entityPool[entityId].minerId > 0) return;

                PrebuildData prebuildData = default(PrebuildData);
                prebuildData.protoId = factory.entityPool[entityId].protoId;
                prebuildData.modelIndex = factory.entityPool[entityId].modelIndex;
                prebuildData.pos = factory.entityPool[entityId].pos;
                prebuildData.rot = factory.entityPool[entityId].rot;
                prebuildData.itemRequired = 1;

                BuildingParameters parameters = default(BuildingParameters);
                parameters.CopyFromFactoryObject(entityId, factory);
                parameters.itemId = prebuildData.protoId;
                /*
                if (parameters.parameters != null)
                {
                    prebuildData.InitParametersArray(parameters.parameters.Length);
                    Array.Copy(parameters.parameters, prebuildData.parameters, prebuildData.parameters.Length);
                } */

                bool[] isOutput = new bool[16];
                int[] otherObjId = new int[16];
                int[] otherSlot = new int[16];
                for (int i = 0; i < 16; ++i)
                {
                    factory.ReadObjectConn(entityId, i, out isOutput[i], out otherObjId[i], out otherSlot[i]);
                }

                try
                {
                    factory.RemoveEntityWithComponents(entityId);
                }
                catch (Exception) { }

                int objId = -PlanetFactory_AddPrebuildDataWithComponents(factory, prebuildData);
                for (int i = 0; i < 16; ++i)
                {
                    factory.WriteObjectConn(objId, i, isOutput[i], otherObjId[i], otherSlot[i]);
                }
                parameters.PasteToFactoryObject(objId, factory);
                parameters.ToParamsArray(ref factory.prebuildPool[-objId].parameters, ref factory.prebuildPool[-objId].paramCount);
            }
            catch (Exception e) { DspBattlePlugin.logger.LogError(e); }
        }

        public static int PlanetFactory_AddPrebuildDataWithComponents(PlanetFactory factory, PrebuildData prebuild)
        {
            ItemProto itemProto = LDB.items.Select((int)prebuild.protoId);
            if (itemProto == null || !itemProto.IsEntity)
            {
                return 0;
            }
            int num = factory.AddPrebuildData(prebuild);
            PrefabDesc prefabDesc = itemProto.prefabDesc;
            ModelProto modelProto = LDB.models.Select((int)prebuild.modelIndex);
            if (modelProto != null)
            {
                prefabDesc = modelProto.prefabDesc;
            }
            if (prefabDesc == null)
            {
                return num;
            }
            if (prebuild.itemRequired > 0)
            {
                factory.AddPrebuildWarning(num);
            }
            factory.prebuildPool[num].modelIndex = (short)prefabDesc.modelIndex;
            factory.prebuildPool[num].modelId =
                factory == GameMain.gpuiManager.activeFactory ?
                GameMain.gpuiManager.AddPrebuildModel((int)factory.prebuildPool[num].modelIndex, num, factory.prebuildPool[num].pos, factory.prebuildPool[num].rot, true)
                : 0;

            if (prefabDesc.colliders != null && prefabDesc.colliders.Length != 0)
            {

                for (int i = 0; i < prefabDesc.colliders.Length; i++)
                {
                    try
                    {
                        if (prefabDesc.isInserter)
                        {
                            ColliderData colliderData = prefabDesc.colliders[i];
                            Vector3 wpos = Vector3.Lerp(factory.prebuildPool[num].pos, factory.prebuildPool[num].pos2, 0.5f);
                            Quaternion wrot = Quaternion.LookRotation(factory.prebuildPool[num].pos2 - factory.prebuildPool[num].pos, wpos.normalized);
                            colliderData.ext = new Vector3(colliderData.ext.x, colliderData.ext.y, Vector3.Distance(factory.prebuildPool[num].pos2, factory.prebuildPool[num].pos) * 0.5f + colliderData.ext.z);
                            factory.prebuildPool[num].colliderId = factory.planet.physics.AddColliderData(colliderData.BindToObject(num, factory.prebuildPool[num].colliderId, EObjectType.Prebuild, wpos, wrot));
                        }
                        else
                        {
                            factory.prebuildPool[num].colliderId = factory.planet.physics.AddColliderData(prefabDesc.colliders[i].BindToObject(num, factory.prebuildPool[num].colliderId, EObjectType.Prebuild, factory.prebuildPool[num].pos, factory.prebuildPool[num].rot));
                        }
                    }
                    catch (Exception) { }
                }
            }
            return num;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "ApplyEntityDisconnection")]
        public static bool PlanetFactory_ApplyEntityDisconnection(ref PlanetFactory __instance, int otherEntityId, int removingEntityId, int otherSlotId, int removingSlotId)
        {
            if (__instance.planet == GameMain.localPlanet || otherEntityId == 0) return true;
            int inserterId = __instance.entityPool[otherEntityId].inserterId;
            if (inserterId > 0)
            {
                int modelId = __instance.entityPool[otherEntityId].modelId;
                __instance.entityPool[otherEntityId].modelId = 0;
                if (__instance.factorySystem.inserterPool[inserterId].insertTarget == removingEntityId)
                {
                    __instance.factorySystem.SetInserterInsertTarget(inserterId, 0, 0);
                }
                if (__instance.factorySystem.inserterPool[inserterId].pickTarget == removingEntityId)
                {
                    __instance.factorySystem.SetInserterPickTarget(inserterId, 0, 0);
                }
                __instance.entityPool[otherEntityId].modelId = modelId;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), "RemoveEntityWithComponents")]
        public static bool PlanetFactory_RemoveEntityWithComponents(ref PlanetFactory __instance, int id)
        {
            if (__instance.planet == GameMain.localPlanet) return true;
            if (id != 0 && __instance.entityPool[id].id != 0 && __instance.entityPool[id].colliderId != 0)
            {
                if (__instance.planet.physics != null)
                {
                    try
                    {
                        __instance.planet.physics.RemoveLinkedColliderData(__instance.entityPool[id].colliderId);
                    }
                    catch (Exception e) { }
                }
                __instance.entityPool[id].colliderId = 0;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "TryAddItemToPackage", new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(int) })]
        public static bool Player_TryAddItemToPackage(ref Player __instance, ref int __result, int itemId, int count, int inc, bool throwTrash, int objId = 0)
        {
            if (removingComponets)
            {
                __result = count;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIItemup), "Up")]
        public static bool UIItemup_Up()
        {
            return !removingComponets;
        }

    }
}
