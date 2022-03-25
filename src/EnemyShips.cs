using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace DSP_Battle
{
    public class EnemyShips
    {

        public static ConcurrentDictionary<int, EnemyShip> ships;

        public static System.Random random = new System.Random();
        public static List<List<EnemyShip>> minTargetDisSortedShips;
        public static List<Dictionary<int, List<EnemyShip>>> minPlanetDisSortedShips;
        public static List<Dictionary<int, List<EnemyShip>>> targetPlanetShips;
        public static List<List<EnemyShip>> maxThreatSortedShips;
        public static List<List<EnemyShip>> minHpSortedShips;
        public static Dictionary<int, List<Tuple<Vector3, int>>> pendingDestroyedEntities; // planet id; upos; range

        public static bool shouldDistroy = true;
        private static bool removingComponets = false;
        public static ConcurrentDictionary<int, object> distroyedStation = new ConcurrentDictionary<int, object>();

        public static void Init()
        {
            ships = new ConcurrentDictionary<int, EnemyShip>();
            distroyedStation = new ConcurrentDictionary<int, object>();
            pendingDestroyedEntities = new Dictionary<int, List<Tuple<Vector3, int>>>();
            SortShips();
        }

        public static void Create(int starIndex, int wormholeIndex, int enemyId, int countDown)
        {
            int nextGid = random.Next(1 << 27, 1 << 29);
            while (ships.ContainsKey(nextGid)) nextGid = random.Next(1 << 27, 1 << 29);

            int stationId = FindNearestPlanetStation(GameMain.galaxy.stars[starIndex], Configs.nextWaveWormholes[wormholeIndex].uPos);
            if (stationId < 0) return;

            EnemyShip enemyShip = new EnemyShip(
                nextGid,
                stationId,
                wormholeIndex,
                enemyId,
                countDown);
            DspBattlePlugin.logger.LogInfo("=========> Init ship " + nextGid + " at station " + enemyShip.shipData.otherGId);

            ships.TryAdd(nextGid, enemyShip);
        }

        public static bool ValidStellarStation(StationComponent s)
        {
            return s != null && s.id != 0 && s.gid != 0 && s.isStellar && !s.isCollector && !distroyedStation.ContainsKey(s.gid);
        }

        public static int FindNearestPlanetStation(StarData starData, VectorLF3 pos)
        {
            int gid = -1;
            double distance = 1e99;

            foreach (var planet in starData.planets)
            {
                double dis = (planet.uPosition - pos).magnitude;
                if (dis > distance || planet.factory?.transport?.stationPool == null) continue;
                StationComponent[] stations = planet.factory.transport.stationPool.Where(ValidStellarStation).ToArray();
                if (stations.Length == 0) continue;
                gid = stations[random.Next(0, stations.Length)].gid;
                distance = dis;
            }
            return gid;
        }

        public static List<int> FindShipsInRange(VectorLF3 pos, double range)
        {
            List<int> list = new List<int>();
            foreach (var ship in ships.Values)
            {
                if (ship.state == EnemyShip.State.active && ship.distanceTo(pos) <= range)
                {
                    list.Add(ship.shipIndex);
                }
            }
            return list;
        }

        public static void SortShips()
        {
            SortShipsMinTargetDis();
            SortShipsMinPlanetDis();
            SortShipsMaxThreat();
            SortShipsMinHp();
        }

        public static List<EnemyShip> sortedShips(int strategy, int starIndex, int planetId)
        {
            // 最接近物流塔
            if (strategy == 1) return minTargetDisSortedShips[starIndex];
            // 最大威胁
            if (strategy == 2) return maxThreatSortedShips[starIndex];
            // 距自己最近
            if (strategy == 3) return minPlanetDisSortedShips[starIndex][planetId];
            // 最低生命
            if (strategy == 4) return minHpSortedShips[starIndex];
            return new List<EnemyShip>();
        }

        public static void SortShipsMinTargetDis()
        {
            var sortedShips = new List<List<EnemyShip>>(Configs.starCount);
            for (var i = 0; i < Configs.starCount; ++i) sortedShips.Add(new List<EnemyShip>());

            Dictionary<int, double> distances = new Dictionary<int, double>();
            foreach (EnemyShip ship in ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                distances.Add(ship.shipIndex, ship.distanceToTarget);
                sortedShips[ship.starIndex].Add(ship);
            }
            foreach (var shipArr in sortedShips)
            {
                shipArr.Sort((v1, v2) => Math.Sign(distances[v1.shipIndex] - distances[v2.shipIndex]));
            }
            minTargetDisSortedShips = sortedShips;
        }

        public static void SortShipsMinPlanetDis()
        {
            var sortedShips = new List<Dictionary<int, List<EnemyShip>>>(Configs.starCount);
            var targetShips = new List<Dictionary<int, List<EnemyShip>>>(Configs.starCount);
            for (var i = 0; i < Configs.starCount; ++i)
            {
                var dictionary = new Dictionary<int, List<EnemyShip>>();
                var dictionary2 = new Dictionary<int, List<EnemyShip>>();
                for (var j = 0; j < 10; ++j)
                {
                    dictionary.Add(100 * (i + 1) + j, new List<EnemyShip>());
                    dictionary2.Add(100 * (i + 1) + j, new List<EnemyShip>());
                }
                sortedShips.Add(dictionary);
                targetShips.Add(dictionary2);
            }

            Dictionary<int, Dictionary<int, double>> distances = new Dictionary<int, Dictionary<int, double>>();
            foreach (EnemyShip ship in ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                int starIndex = ship.starIndex;
                StarData starData = GameMain.galaxy.stars[starIndex];
                foreach (PlanetData planet in starData.planets)
                {
                    int planetId = planet.id;
                    if (!distances.ContainsKey(planetId)) distances.Add(planetId, new Dictionary<int, double>());
                    distances[planetId].Add(ship.shipIndex, ship.distanceTo(planet.uPosition));
                    sortedShips[starIndex][planetId].Add(ship);
                }
                if (targetShips[starIndex].ContainsKey(ship.shipData.planetB))
                    targetShips[starIndex][ship.shipData.planetB].Add(ship);
            }

            foreach (var one in sortedShips)
            {
                foreach (var entry in one)
                {
                    int planetId = entry.Key;
                    entry.Value.Sort((v1, v2) => Math.Sign(distances[planetId][v1.shipIndex] - distances[planetId][v2.shipIndex]));
                }
            }
            minPlanetDisSortedShips = sortedShips;
            targetPlanetShips = targetShips;
        }

        public static void SortShipsMaxThreat()
        {
            var sortedShips = new List<List<EnemyShip>>(Configs.starCount);
            for (var i = 0; i < Configs.starCount; ++i) sortedShips.Add(new List<EnemyShip>());

            Dictionary<int, double> distances = new Dictionary<int, double>();
            foreach (EnemyShip ship in ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                distances.Add(ship.shipIndex, ship.threat);
                sortedShips[ship.starIndex].Add(ship);
            }
            foreach (var shipArr in sortedShips)
            {
                shipArr.Sort((v1, v2) => Math.Sign(distances[v2.shipIndex] - distances[v1.shipIndex]));
            }
            maxThreatSortedShips = sortedShips;
        }

        public static void SortShipsMinHp()
        {
            var sortedShips = new List<List<EnemyShip>>(Configs.starCount);
            for (var i = 0; i < Configs.starCount; ++i) sortedShips.Add(new List<EnemyShip>());

            foreach (EnemyShip ship in ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                sortedShips[ship.starIndex].Add(ship);
            }
            foreach (var shipArr in sortedShips)
            {
                shipArr.Sort((v1, v2) => Math.Sign(v1.hp - v2.hp));
            }
            minHpSortedShips = sortedShips;
        }

        public static void OnShipLanded(EnemyShip ship)
        {
            DspBattlePlugin.logger.LogInfo("=========> Ship " + ship.shipIndex + " landed at station " + ship.shipData.otherGId);

            if (shouldDistroy)
            {
                StationComponent station = ship.targetStation;
                if (ValidStellarStation(station))
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

                    UIAlert.elimPointRatio *= 0.5f;
                    Configs.nextWaveDelay += 5 * 3600;
                    if (Configs.nextWaveDelay > 30 * 3600) Configs.nextWaveDelay = 30 * 3600;
                }

            }

            ship.shipData.inc--;
            if (ship.shipData.inc > 0)
            {
                ship.state = EnemyShip.State.active;
            }
        }

        private static void CheckPendingDestroyedEntities()
        {
            foreach (var entry in pendingDestroyedEntities)
            {
                removingComponets = true;
                PlanetData planet = GameMain.galaxy.PlanetById(entry.Key);
                PlanetFactory planetFactory = planet.factory;
                bool freePhysics = false;
                try
                {
                    for (int i = 0; i < planetFactory.entityPool.Length; ++i)
                    {
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
                catch (Exception) { }
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
            if (!ValidStellarStation(station) || !removingComponets) return;

            for (int slot = 0; slot < station.storage.Length; slot++) //资源损失
            {
                UIBattleStatistics.RegisterResourceLost(station.storage[slot].count);
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

            station.idleDroneCount = 0;
            station.workDroneCount = 0;
            Array.Clear(station.workDroneDatas, 0, station.workDroneDatas.Length);
            Array.Clear(station.workDroneOrders, 0, station.workDroneOrders.Length);
            station.idleShipCount = 0;
            station.workShipCount = 0;
            station.workShipIndices = 0;
            Array.Clear(station.workShipDatas, 0, station.workShipDatas.Length);
            Array.Clear(station.workShipOrders, 0, station.workShipOrders.Length);
            station.energy = 0;
            station.energyPerTick = 0;
            station.warperCount = 0;

            factory.transport.RefreshTraffic(station.id);
            GameMain.data.galacticTransport.RefreshTraffic(station.gid);

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
            if (!removingComponets) return;
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
                if (factory.entityPool[entityId].beltId > 0 || factory.entityPool[entityId].inserterId > 0) return;

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
                for (int i =0; i < 16; ++i)
                {
                    factory.WriteObjectConn(objId, i, isOutput[i], otherObjId[i], otherSlot[i]);
                }
                parameters.PasteToFactoryObject(objId, factory);
                parameters.ToParamsArray(ref factory.prebuildPool[-objId].parameters, ref factory.prebuildPool[-objId].paramCount);
            }
            catch(Exception e) { DspBattlePlugin.logger.LogError(e); }
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

        public static void OnShipDistroyed(EnemyShip ship)
        {
            RemoveShip(ship);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {
            if (DSPGame.IsMenuDemo) return;

            List<EnemyShip> list = ships.Values.ToList();

            list.Do(ship =>
            {
                ship.Update(time);
                if (ship.state == EnemyShip.State.landed) OnShipLanded(ship);
                if (ship.state != EnemyShip.State.active && ship.state != EnemyShip.State.uninitialized) RemoveShip(ship);
            });

            if (time % 20 == 1) CheckPendingDestroyedEntities();

            // time is the frame since start
            if (time % 30 == 1)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((e) => SortShips()));
            }

            switch (Configs.nextWaveState)
            {
                case 0: UpdateWaveStage0(time); break;
                case 1: UpdateWaveStage1(time); break;
                case 2: UpdateWaveStage2(time); break;
                case 3: UpdateWaveStage3(time); break;
            }

            if (time >= Configs.extraSpeedFrame && Configs.extraSpeedEnabled)
            {
                Configs.extraSpeedEnabled = false;
                Configs.extraSpeedFrame = -1;
                GameMain.history.miningSpeedScale /= 2;
                GameMain.history.techSpeed /= 2;
                GameMain.history.logisticDroneSpeedScale /= 1.5f;
                GameMain.history.logisticShipSpeedScale /= 1.5f;
            }
        }

        private static void UpdateWaveStage0(long time)
        {
            if (time % 1800 != 1 || time < Configs.nextWaveFrameIndex + 60 * 60 * 2) return;
            DspBattlePlugin.logger.LogInfo("=====> Initializing next wave");
            StationComponent[] stations = GameMain.data.galacticTransport.stationPool.Where(ValidStellarStation).ToArray();
            if (stations.Length == 0) return;
            int starId = stations[random.Next(0, stations.Length)].planetId / 100 - 1;

            // Gen next wave
            int deltaFrames = (Configs.coldTime[Math.Min(Configs.coldTime.Length - 1, Configs.totalWave)] + 1) * 3600;
            Configs.nextWaveFrameIndex = time + deltaFrames + Configs.nextWaveDelay;
            Configs.nextWaveDelay = 0;
            Configs.nextWaveIntensity = Configs.intensity[Math.Min(Configs.intensity.Length - 1, Configs.wavePerStar[starId])];
            // Extra intensity
            long cube = (long)(GameMain.history.universeMatrixPointUploaded * 0.0002777777777777778);
            if (cube > 100000) Configs.nextWaveIntensity += (int)((cube - 100000) / 1000);

            ulong energy = 0;
            DysonSphere[] dysonSpheres = GameMain.data.dysonSpheres;
            int num3 = dysonSpheres.Length;
            for (int i = 0; i < num3; i++)
            {
                if (dysonSpheres[i] != null)
                {
                    energy += (ulong)dysonSpheres[i].energyGenCurrentTick;
                }
            }
            energy *= 60;
            energy /= (1024 * 1024 * 1024L);

            if (energy > 300) // 300G
                Configs.nextWaveIntensity += (int)(energy - 300) * 5;
            if (Configs.nextWaveIntensity > 30000) Configs.nextWaveIntensity = 30000;

            Configs.nextWaveStarIndex = starId;
            Configs.nextWaveState = 1;

            int intensity = Configs.nextWaveIntensity;
            for (int i = 4; i >= 1; --i)
            {
                double v = random.NextDouble() / 2 + 0.25;
                Configs.nextWaveEnemy[i] = (int)(intensity * v / Configs.enemyIntensity[i]);
                intensity -= Configs.nextWaveEnemy[i] * Configs.enemyIntensity[i];
            }
            Configs.nextWaveEnemy[0] = intensity / Configs.enemyIntensity[0];
            Configs.nextWaveWormCount = random.Next(Math.Min(Configs.nextWaveIntensity / 100, 40), Math.Min(80, Configs.nextWaveEnemy.Sum())) + 1;

            UIDialogPatch.ShowUIDialog("下一波攻击即将到来！".Translate(),
                string.Format("做好防御提示".Translate(), GameMain.galaxy.stars[Configs.nextWaveStarIndex].displayName));

            UIAlert.ShowAlert(true);
        }

        private static void UpdateWaveStage1(long time)
        {
            if (time < Configs.nextWaveFrameIndex - 3600 * 5) return;
            StarData star = GameMain.galaxy.stars[Configs.nextWaveStarIndex];

            StationComponent[] stations = GameMain.data.galacticTransport.stationPool.Where(e => e != null && e.isStellar && !e.isCollector && e.gid != 0 && e.id != 0 && e.planetId / 100 - 1 == Configs.nextWaveStarIndex).ToArray();

            for (int i = 0; i < Configs.nextWaveWormCount; ++i)
            {
                while (true)
                {
                    int planetId = 100 * (Configs.nextWaveStarIndex + 1) + 1;
                    if (stations.Length != 0) planetId = stations[random.Next(0, stations.Length)].planetId;

                    Wormhole wormhole = new Wormhole(planetId, UnityEngine.Random.onUnitSphere);
                    VectorLF3 pos = wormhole.uPos;

                    if ((star.uPosition - pos).magnitude < Configs.wormholeRange + star.radius - 10) continue;
                    if (star.planets.Any(planetData => planetData.type != EPlanetType.Gas && (planetData.uPosition - pos).magnitude < Configs.wormholeRange + planetData.radius - 10)) continue;

                    Configs.nextWaveWormholes[i] = wormhole;
                    break;
                }
            }

            Configs.nextWaveState = 2;


            UIDialogPatch.ShowUIDialog("虫洞已生成！".Translate(),
                string.Format("虫洞生成提示".Translate(), GameMain.galaxy.stars[Configs.nextWaveStarIndex].displayName));

            UIAlert.ShowAlert(true);
        }

        private static void UpdateWaveStage2(long time)
        {
            if (time < Configs.nextWaveFrameIndex) return;
            int u = 0;
            for (int i = 0; i <= 4; ++i)
            {
                for (int j = 0; j < Configs.nextWaveEnemy[i]; ++j)
                {
                    Create(Configs.nextWaveStarIndex, u % Configs.nextWaveWormCount, i, random.Next(0, Math.Min(u + 1, 30)));
                    u++;
                }
            }

            Configs.nextWaveState = 3;
        }

        private static void UpdateWaveStage3(long time)
        {
            UIBattleStatistics.RegisterBattleTime(time);
            if (ships.Count == 0)
            {
                Configs.wavePerStar[Configs.nextWaveStarIndex]++;
                Configs.nextWaveState = 0;
                Configs.nextWaveStarIndex = 0;
                Configs.nextWaveWormCount = 0;
                Configs.nextWaveFrameIndex = time;
                distroyedStation.Clear();

                long rewardBase = 5 * 60 * 60;
                if (Configs.difficulty == -1) rewardBase = rewardBase * 3 / 4;
                if (Configs.difficulty == 1) rewardBase *= 2;

                long extraSpeedFrame = UIBattleStatistics.totalEnemyEliminated * rewardBase / UIBattleStatistics.totalEnemyGen;
                Configs.extraSpeedFrame = time + extraSpeedFrame;
                Configs.extraSpeedEnabled = true;
                GameMain.history.miningSpeedScale *= 2;
                GameMain.history.techSpeed *= 2;
                GameMain.history.logisticDroneSpeedScale *= 1.5f;
                GameMain.history.logisticShipSpeedScale *= 1.5f;

                UIDialogPatch.ShowUIDialog("战斗已结束！".Translate(),
                    "战斗时间".Translate() + ": " + string.Format("{0:00}:{1:00}", new object[] { UIBattleStatistics.battleTime / 60 / 60, UIBattleStatistics.battleTime / 60 % 60 }) + "; " +
                    "歼灭敌人".Translate() + ": " + UIBattleStatistics.totalEnemyEliminated.ToString("N0") + "; " +
                    "输出伤害".Translate() + ": " + UIBattleStatistics.totalDamage.ToString("N0") + "; " +
                    "损失物流塔".Translate() + ": " + UIBattleStatistics.stationLost.ToString("N0") + "; " +
                    "损失其他建筑".Translate() + ": " + UIBattleStatistics.othersLost.ToString("N0") + "; " +
                    "损失资源".Translate() + ": " + UIBattleStatistics.resourceLost.ToString("N0") + "." +
                    "\n\n<color=#c2853d>" + string.Format("奖励提示".Translate(), extraSpeedFrame / 60) + "</color>\n\n" +
                    "查看更多战斗信息".Translate()
                    );

            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameHistoryData), "UnlockTechFunction")]
        public static void GameHistoryData_UnlockTechFunction(ref GameHistoryData __instance, int func, double value, int level)
        {
            int num = (int)((value > 0.0) ? (value + 0.5) : (value - 0.5));
            if (Configs.extraSpeedEnabled) {
                switch (func)
                {
                    case 15: __instance.logisticDroneSpeedScale += (float)value / 2; break;
                    case 16: __instance.logisticShipSpeedScale += (float)value / 2; break;
                    case 21: __instance.miningSpeedScale += (float)value; break;
                    case 22: __instance.techSpeed += num; break;
                }
            }
        }

        public static void RemoveShip(EnemyShip ship)
        {
            ships.TryRemove(ship.shipIndex, out EnemyShip _);
        }

        private static Dictionary<LogisticShipRenderer, ComputeBuffer> logisticShipRendererComputeBuffer = new Dictionary<LogisticShipRenderer, ComputeBuffer>();
        private static bool logisticShipRendererExpanded = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogisticShipRenderer), "SetCapacity")]
        public static void LogisticShipRenderer_SetCapacity()
        {
            logisticShipRendererExpanded = true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogisticShipRenderer), "Update")]
        public static void LogisticShipRenderer_Update(ref LogisticShipRenderer __instance)
        {
            if (ships.Count == 0 || UIRoot.instance.uiGame.starmap.active) return;
            if (__instance.transport == null) return;
            while (__instance.capacity < __instance.shipCount + ships.Count)
            {
                __instance.Expand2x();
            }

            foreach (var ship in ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;

                __instance.shipsArr[__instance.shipCount++] = ship.renderingData;
                //if (ship.shipData.stage != 1)
                //    continue;
                if (ship.distanceToTarget > 250 || ship.shipData.planetB != GameMain.localPlanet?.id)
                    continue;
                //飞船发射子弹轰击
                try
                {
                    PlanetFactory planetFactory = GameMain.galaxy.PlanetById(ship.shipData.planetB).factory;
                    Vector3 stationPos = planetFactory.entityPool[ship.targetStation.entityId].pos;
                    int starIndex = planetFactory.planetId / 100 - 1;
                    if (GameMain.data.dysonSpheres.Length > starIndex && GameMain.data.dysonSpheres[starIndex] != null && GameMain.data.dysonSpheres[starIndex].swarm != null && (GameMain.instance.timei + ship.shipIndex) % 20 == 0) 
                    {
                        int planetId = planetFactory.planetId;
                        DysonSwarm swarm = GameMain.data.dysonSpheres[starIndex].swarm;
                        AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                        VectorLF3 stationUpos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, stationPos);
                        VectorLF3 shipLocalPos = Maths.QInvRotateLF(astroPoses[planetId].uRot, ship.uPos - astroPoses[planetId].uPos);
                        //我不会再星球表面生成目标点，就用下面这种近似方法
                        VectorLF3 direc2Center = astroPoses[planetId].uPos - stationUpos; //物流塔到星球球心连线
                        VectorLF3 vert = new VectorLF3(0, 0, 1);
                        if (direc2Center.z != 0)
                        {
                            double randX = DspBattlePlugin.randSeed.NextDouble() - 0.5;
                            double randY = DspBattlePlugin.randSeed.NextDouble() - 0.5;
                            vert = new VectorLF3(randX, randY, ((-direc2Center.x) * randX - direc2Center.y * randY) / direc2Center.z); //这是一个与星球表面相切的随机方向，模拟地表
                        }

                        int bulletIndex = swarm.AddBullet(new SailBullet
                        {
                            maxt = 0.08f,
                            lBegin = shipLocalPos,
                            uEndVel = stationUpos - ship.uPos,
                            uBegin = ship.uPos,
                            uEnd = stationUpos + vert.normalized * ship.damageRange * DspBattlePlugin.randSeed.NextDouble() + direc2Center.normalized * DspBattlePlugin.randSeed.NextDouble() * ship.damageRange * 0.4
                        }, 1);

                        swarm.bulletPool[bulletIndex].state = 0;

                    }
                }
                catch (Exception)
                {
                }
                
            }

            if (!logisticShipRendererComputeBuffer.ContainsKey(__instance) || logisticShipRendererExpanded)
            {
                logisticShipRendererComputeBuffer[__instance] = AccessTools.FieldRefAccess<LogisticShipRenderer, ComputeBuffer>(__instance, "shipsBuffer");
                logisticShipRendererExpanded = false;
            }

            if (logisticShipRendererComputeBuffer[__instance] != null)
            {
                logisticShipRendererComputeBuffer[__instance].SetData(__instance.shipsArr, 0, 0, __instance.shipCount);
            }
        }

        private static Dictionary<LogisticShipUIRenderer, UIStarmap> logisticShipUIRendererUIStarmap = new Dictionary<LogisticShipUIRenderer, UIStarmap>();
        private static Dictionary<LogisticShipUIRenderer, ComputeBuffer> logisticShipUIRendererComputeBuffer = new Dictionary<LogisticShipUIRenderer, ComputeBuffer>();
        private static Dictionary<LogisticShipUIRenderer, ShipUIRenderingData[]> logisticShipUIRendererShipUIRenderingData = new Dictionary<LogisticShipUIRenderer, ShipUIRenderingData[]>();
        private static FieldInfo logisticShipUIRendererShipCount = null;

        private static bool logisticShipUIRendererExpanded = false;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogisticShipUIRenderer), "SetCapacity")]
        public static void LogisticShipUIRenderer_SetCapacity()
        {
            logisticShipUIRendererExpanded = true;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogisticShipUIRenderer), "Update")]
        public static void LogisticShipUIRenderer_Update(ref LogisticShipUIRenderer __instance)
        {
            if (ships.Count == 0) return;

            if (!logisticShipUIRendererUIStarmap.ContainsKey(__instance))
            {
                logisticShipUIRendererUIStarmap.Add(__instance, AccessTools.FieldRefAccess<LogisticShipUIRenderer, UIStarmap>(__instance, "uiStarmap"));
                logisticShipUIRendererShipCount = AccessTools.Field(typeof(LogisticShipUIRenderer), "shipCount");
            }

            UIStarmap uiStarMap = logisticShipUIRendererUIStarmap[__instance];

            if (__instance.transport == null || uiStarMap == null || !uiStarMap.active) return;

            int shipCount = (int)logisticShipUIRendererShipCount.GetValue(__instance);

            while (__instance.capacity < shipCount + ships.Count)
            {
                __instance.Expand2x();
            }

            if (!logisticShipUIRendererComputeBuffer.ContainsKey(__instance) || logisticShipUIRendererExpanded)
            {
                logisticShipUIRendererComputeBuffer[__instance] = AccessTools.FieldRefAccess<LogisticShipUIRenderer, ComputeBuffer>(__instance, "shipsBuffer");
                logisticShipUIRendererShipUIRenderingData[__instance] = AccessTools.FieldRefAccess<LogisticShipUIRenderer, ShipUIRenderingData[]>(__instance, "shipsArr");
                logisticShipUIRendererExpanded = false;
            }

            ComputeBuffer shipsBuffer = logisticShipUIRendererComputeBuffer[__instance];
            ShipUIRenderingData[] shipsArr = logisticShipUIRendererShipUIRenderingData[__instance];

            foreach (var ship in ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                shipsArr[shipCount] = ship.renderingUIData;
                shipsArr[shipCount].rpos = (shipsArr[shipCount].upos - uiStarMap.viewTargetUPos) * 0.00025;
                shipCount++;
            }

            logisticShipUIRendererShipCount.SetValue(__instance, shipCount);

            if (shipsBuffer != null)
            {
                shipsBuffer.SetData(shipsArr, 0, 0, shipCount);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "OnDraw")]
        public static void GameData_OnDraw(ref GameData __instance, int frame)
        {
            if (__instance.galacticTransport != null && ships.Count != 0)
            {
                __instance.galacticTransport.shipRenderer.Update();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "TryAddItemToPackage", new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(int)})]
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

        public static void Export(BinaryWriter w)
        {
            w.Write(ships.Count);
            ships.Values.Do(ship => ship.Export(w));

        }

        public static void Import(BinaryReader r)
        {
            int cnt = r.ReadInt32();
            ships.Clear();
            for (var i = 0; i < cnt; ++i)
            {
                EnemyShip ship = new EnemyShip(r);
                ships.TryAdd(ship.shipIndex, ship);
            }
            distroyedStation.Clear();
            SortShips();
        }

        public static void IntoOtherSave()
        {
            ships.Clear();
            distroyedStation.Clear();
            SortShips();
        }
    }
}
