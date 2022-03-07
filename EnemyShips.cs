using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DSP_Battle
{
    public class EnemyShips
    {

        public static ConcurrentDictionary<int, EnemyShip> ships;

        public static System.Random gidRandom = new System.Random();
        public static List<List<EnemyShip>> minTargetDisSortedShips;
        public static List<Dictionary<int, List<EnemyShip>>> minPlanetDisSortedShips;
        public static List<List<EnemyShip>> maxThreatSortedShips;
        public static List<List<EnemyShip>> minHpSortedShips;

        public static bool shouldDistroy = true;
        private static bool removingComponets = false;

        public static void Init()
        {
            ships = new ConcurrentDictionary<int, EnemyShip>();
            SortShips();
        }

        public static void Create(int starIndex, VectorLF3 initPos, int enemyId)
        {
            int nextGid = gidRandom.Next(1 << 27, 1 << 29);
            while (ships.ContainsKey(nextGid)) nextGid = gidRandom.Next(1 << 27, 1 << 29);

            int stationId = FindNearestStation(GameMain.galaxy.stars[starIndex], initPos);
            if (stationId < 0) return;

            EnemyShip enemyShip = new EnemyShip(
                nextGid,
                stationId,
                initPos,
                enemyId);
            DspBattlePlugin.logger.LogInfo("=========> Init ship " + nextGid + " at station " + enemyShip.shipData.otherGId);

            ships.TryAdd(nextGid, enemyShip);
        }

        public static int FindNearestStation(StarData starData, VectorLF3 pos)
        {
            StationComponent[] stations = GameMain.data.galacticTransport.stationPool;
            AstroPose[] astroPoses = GameMain.data.galaxy.astroPoses;

            int index = -1;
            double distance = 1e99;
            for (int i = 0; i < stations.Length; ++i)
            {
                if (stations[i] != null && stations[i].id != 0 && stations[i].gid != 0 && stations[i].isStellar &&
                    !stations[i].isCollector && !stations[i].isVeinCollector && stations[i].planetId / 100 - 1 == starData.index)
                {
                    AstroPose astroPose = astroPoses[stations[i].planetId];
                    VectorLF3 stationPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, stations[i].shipDockPos + stations[i].shipDockPos.normalized * 25f);

                    double dis = (pos - stationPos).magnitude;
                    if (dis < distance)
                    {
                        distance = dis;
                        index = i;
                    }
                }
            }
            return index;
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
            var sortedShips = new List<List<EnemyShip>>(100);
            for (var i = 0; i < 100; ++i) sortedShips.Add(new List<EnemyShip>());

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
            var sortedShips = new List<Dictionary<int, List<EnemyShip>>>(100);
            for (var i = 0; i < 100; ++i)
            {
                var dictionary = new Dictionary<int, List<EnemyShip>>();
                for (var j = 0; j < 10; ++j)
                {
                    dictionary.Add(100 * (i + 1) + j, new List<EnemyShip>());
                }
                sortedShips.Add(dictionary);
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
        }

        public static void SortShipsMaxThreat()
        {
            var sortedShips = new List<List<EnemyShip>>(100);
            for (var i = 0; i < 100; ++i) sortedShips.Add(new List<EnemyShip>());

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
            var sortedShips = new List<List<EnemyShip>>(100);
            for (var i = 0; i < 100; ++i) sortedShips.Add(new List<EnemyShip>());

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
                if (station != null && station.entityId > 0)
                {
                    PlanetFactory planetFactory = GameMain.galaxy.PlanetById(ship.shipData.planetB).factory;
                    removingComponets = true;
                    Vector3 stationPos = planetFactory.entityPool[station.entityId].pos;
                    RemoveEntity(planetFactory, station.entityId);
                    // Find all entities in damageRange
                    for (int i = 0; i < planetFactory.entityPool.Length; ++i)
                    {
                        if (planetFactory.entityPool[i].notNull && (planetFactory.entityPool[i].pos - stationPos).magnitude <= ship.damageRange)
                        {
                            RemoveEntity(planetFactory, i);
                        }
                    }
                    removingComponets = false;
                }
            }

            ship.shipData.inc--;
            if (ship.shipData.inc > 0)
            {
                ship.state = EnemyShip.State.active;
            }


        }
        private static void RemoveEntity(PlanetFactory factory, int entityId)
        {
            try
            {
                if (entityId >= factory.entityPool.Length || factory.entityPool[entityId].isNull) return;

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
                ship.Update();
                if (ship.state == EnemyShip.State.landed) OnShipLanded(ship);
                if (ship.state != EnemyShip.State.active) RemoveShip(ship);
            });

            // time is the frame since start
            if (time % 60 == 1)
            {
                SortShips();
            }

            UpdateWaveState(time);
        }

        private static void UpdateWaveState(long time)
        {

            switch (Configs.nextWaveState)
            {
                case 0:
                    if (time % 1800 != 1) break;
                    DspBattlePlugin.logger.LogInfo("=====> Initializing next wave");
                    StationComponent[] stations = GameMain.data.galacticTransport.stationPool.Where(e => e != null && e.isStellar && e.gid != 0 && e.id != 0).ToArray();
                    if (stations.Length == 0) break;
                    int planetId = stations[gidRandom.Next(0, stations.Length)].planetId;
                    int starId = planetId / 100 - 1;

                    // Gen next wave
                    int deltaFrames = (Configs.coldTime[Math.Min(Configs.coldTime.Length - 1, Configs.totalWave)] + 1) * 3600;
                    Configs.nextWaveFrameIndex = time + deltaFrames;
                    DspBattlePlugin.logger.LogInfo("=====> DeltaFrames: " + deltaFrames);
                    Configs.nextWaveIntensity = Configs.intensity[Math.Min(Configs.intensity.Length - 1, Configs.wavePerStar[starId])];
                    Configs.nextWavePlanetId = planetId;
                    Configs.nextWaveState = 1;

                    DspBattlePlugin.logger.LogInfo("=====> nextWaveFrameIndex: " + Configs.nextWaveFrameIndex);
                    DspBattlePlugin.logger.LogInfo("=====> nextWaveIntensity: " + Configs.nextWaveIntensity);
                    DspBattlePlugin.logger.LogInfo("=====> nextWavePlanetId: " + Configs.nextWavePlanetId);

                    int intensity = Configs.nextWaveIntensity;
                    for (int i = 4; i >= 1; --i)
                    {
                        double v = gidRandom.NextDouble() / 2 + 0.25;
                        Configs.nextWaveEnemy[i] = (int)(intensity * v / Configs.enemyIntensity[i]);
                        intensity -= Configs.nextWaveEnemy[i] * Configs.enemyIntensity[i];
                    }
                    Configs.nextWaveEnemy[0] = intensity / Configs.enemyIntensity[0];
                    Configs.nextWaveWormCount = gidRandom.Next(0, Math.Min(20, Configs.nextWaveEnemy.Sum())) + 1;

                    DspBattlePlugin.logger.LogInfo("=====> nextWaveWormCount: " + Configs.nextWaveWormCount);
                    DspBattlePlugin.logger.LogInfo("=====> nextWaveWormEnemy: " + Configs.nextWaveEnemy.Select(e => e + "").Join(null, ","));

                    UIRealtimeTip.Popup("下一波进攻即将到来！".Translate());
                    UIAlert.ShowAlert(true);
                    break;
                case 1:
                    if (time < Configs.nextWaveFrameIndex - 3600 * 5) break;

                    PlanetData planet = GameMain.galaxy.PlanetById(Configs.nextWavePlanetId);
                    StarData star = planet.star;

                    for (int i = 0; i < Configs.nextWaveWormCount; ++i)
                    {
                        while (true)
                        {
                            int angle1 = gidRandom.Next(0, 360);
                            int angle2 = gidRandom.Next(0, 360);
                            VectorLF3 pos = planet.uPosition + new VectorLF3(
                                (Configs.wormholeRange + planet.radius) * Math.Cos(angle1 * Math.PI / 360) * Math.Cos(angle2 * Math.PI / 360), // rcosAcosB
                                (Configs.wormholeRange + planet.radius) * Math.Cos(angle1 * Math.PI / 360) * Math.Sin(angle2 * Math.PI / 360), // rcosAsinB
                                (Configs.wormholeRange + planet.radius) * Math.Sin(angle1 * Math.PI / 360) // rsinA
                            );
                            if ((star.uPosition - pos).magnitude < Configs.wormholeRange + planet.radius - 10) continue;
                            bool valid = true;
                            foreach (PlanetData planetData in star.planets)
                            {
                                if ((planetData.uPosition - pos).magnitude < Configs.wormholeRange + planetData.radius - 10) valid = false;
                            }
                            if (valid)
                            {
                                Configs.nextWaveAngle1[i] = angle1;
                                Configs.nextWaveAngle2[i] = angle2;
                                break;
                            }
                        }
                    }

                    Configs.nextWaveState = 2;
                    UIAlert.ShowAlert(true);
                    UIRealtimeTip.Popup("虫洞已生成！".Translate());
                    break;
                case 2:
                    if (time >= Configs.nextWaveFrameIndex)
                    {
                        PlanetData planetData = GameMain.galaxy.PlanetById(Configs.nextWavePlanetId);
                        int u = 0;
                        for (int i = 0; i <= 4; ++i)
                        {
                            for (int j = 0; j < Configs.nextWaveEnemy[i]; ++j)
                            {
                                int angle1 = Configs.nextWaveAngle1[u % Configs.nextWaveWormCount];
                                int angle2 = Configs.nextWaveAngle2[u % Configs.nextWaveWormCount];
                                Create(Configs.nextWavePlanetId / 100 - 1,
                                    planetData.uPosition
                                    + new VectorLF3(
                                         (Configs.wormholeRange + planetData.radius) * Math.Cos(angle1 * Math.PI / 360) * Math.Cos(angle2 * Math.PI / 360), // rcosAcosB
                                         (Configs.wormholeRange + planetData.radius) * Math.Cos(angle1 * Math.PI / 360) * Math.Sin(angle2 * Math.PI / 360), // rcosAsinB
                                         (Configs.wormholeRange + planetData.radius) * Math.Sin(angle1 * Math.PI / 360) // rsinA
                                    ), i);
                                u++;
                            }
                        }

                        Configs.nextWaveState = 3;
                    }
                    break;
                case 3:
                    if (ships.Count == 0)
                    {
                        Configs.wavePerStar[Configs.nextWavePlanetId / 100 - 1]++;
                        Configs.nextWaveState = 0;
                        Configs.nextWaveFrameIndex = -1;
                        Configs.nextWavePlanetId = -1;
                    }
                    break;

            }

        }

        public static void RemoveShip(EnemyShip ship)
        {
            ships.TryRemove(ship.shipIndex, out EnemyShip _);
            minTargetDisSortedShips[ship.starIndex].Remove(ship);
            minHpSortedShips[ship.starIndex].Remove(ship);
            foreach (var entry in minPlanetDisSortedShips[ship.starIndex])
            {
                entry.Value.Remove(ship);
            }
        }

        private static Dictionary<LogisticShipRenderer, ComputeBuffer> logisticShipRendererComputeBuffer = new Dictionary<LogisticShipRenderer, ComputeBuffer>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogisticShipRenderer), "Update")]
        public static void LogisticShipRenderer_Update(ref LogisticShipRenderer __instance)
        {
            if (ships.Count == 0) return;
            if (__instance.transport == null) return;
            while (__instance.capacity < __instance.shipCount + ships.Count)
            {
                __instance.Expand2x();
            }

            foreach (var ship in ships.Values)
            {
                __instance.shipsArr[__instance.shipCount++] = ship.renderingData;
            }

            if (!logisticShipRendererComputeBuffer.ContainsKey(__instance))
            {
                logisticShipRendererComputeBuffer.Add(__instance, AccessTools.FieldRefAccess<LogisticShipRenderer, ComputeBuffer>(__instance, "shipsBuffer"));
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
        [HarmonyPatch(typeof(LogisticShipUIRenderer), "SetCapacity")
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
        [HarmonyPatch(typeof(Player), "TryAddItemToPackage")]
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
        public static bool UIItemup_Up(int itemId, int upCount)
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
            SortShips();
        }

        public static void IntoOtherSave()
        {


            ships.Clear();
            SortShips();

        }
    }
}
