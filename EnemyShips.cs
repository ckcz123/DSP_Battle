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

        public static int timeDelay = 0;

        public static void Init()
        {
            ships = new ConcurrentDictionary<int, EnemyShip>();
            timeDelay = 0;
            SortShips();
        }

        public static void Create(int starIndex, int wormholeIndex, int enemyId, int countDown)
        {
            int nextGid = gidRandom.Next(1 << 27, 1 << 29);
            while (ships.ContainsKey(nextGid)) nextGid = gidRandom.Next(1 << 27, 1 << 29);

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

        private static bool ValidStellarStation(StationComponent s)
        {
            return s != null && s.id != 0 && s.gid != 0 && s.isStellar && !s.isCollector;
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
                gid = stations[gidRandom.Next(0, stations.Length)].gid;
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
                            if (planetFactory.entityPool[i].beltId != 0 && gidRandom.NextDouble() < 0.5) continue;

                            RemoveEntity(planetFactory, i);
                        }
                    }
                    UIAlert.elimPointRatio *= 0.5f;
                    removingComponets = false;
                    timeDelay += 5 * 3600;
                    if (timeDelay > 30 * 3600) timeDelay = 30 * 3600;
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
                ship.Update(time);
                if (ship.state == EnemyShip.State.landed) OnShipLanded(ship);
                if (ship.state != EnemyShip.State.active && ship.state != EnemyShip.State.uninitialized) RemoveShip(ship);
            });

            // time is the frame since start
            if (time % 60 == 1)
            {
                SortShips();
            }

            switch (Configs.nextWaveState)
            {
                case 0: UpdateWaveStage0(time); break;
                case 1: UpdateWaveStage1(time); break;
                case 2: UpdateWaveStage2(time); break;
                case 3: UpdateWaveStage3(time); break;
            }
        }

        private static void UpdateWaveStage0(long time)
        {
            if (time % 1800 != 1) return;
            DspBattlePlugin.logger.LogInfo("=====> Initializing next wave");
            StationComponent[] stations = GameMain.data.galacticTransport.stationPool.Where(ValidStellarStation).ToArray();
            if (stations.Length == 0) return;
            int starId = stations[gidRandom.Next(0, stations.Length)].planetId / 100 - 1;

            // Gen next wave
            int deltaFrames = (Configs.coldTime[Math.Min(Configs.coldTime.Length - 1, Configs.totalWave)] + 1) * 3600;
            Configs.nextWaveFrameIndex = time + deltaFrames + timeDelay;
            timeDelay = 0;
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
                double v = gidRandom.NextDouble() / 2 + 0.25;
                Configs.nextWaveEnemy[i] = (int)(intensity * v / Configs.enemyIntensity[i]);
                intensity -= Configs.nextWaveEnemy[i] * Configs.enemyIntensity[i];
            }
            Configs.nextWaveEnemy[0] = intensity / Configs.enemyIntensity[0];
            Configs.nextWaveWormCount = gidRandom.Next(Math.Min(Configs.nextWaveIntensity / 100, 40), Math.Min(80, Configs.nextWaveEnemy.Sum())) + 1;

            UIRealtimeTip.Popup("下一波进攻即将到来！".Translate());
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
                    if (stations.Length != 0) planetId = stations[gidRandom.Next(0, stations.Length)].planetId;

                    Wormhole wormhole = new Wormhole(planetId, UnityEngine.Random.onUnitSphere);
                    VectorLF3 pos = wormhole.uPos;

                    if ((star.uPosition - pos).magnitude < Configs.wormholeRange + star.radius - 10) continue;
                    if (star.planets.Any(planetData => planetData.type != EPlanetType.Gas && (planetData.uPosition - pos).magnitude < Configs.wormholeRange + planetData.radius - 10)) continue;

                    Configs.nextWaveWormholes[i] = wormhole;
                    break;
                }
                UIRealtimeTip.Popup(string.Format("虫洞 {0} 已生成！".Translate(), i + 1));
            }

            Configs.nextWaveState = 2;
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
                    Create(Configs.nextWaveStarIndex, u % Configs.nextWaveWormCount, i, gidRandom.Next(0, Math.Min(u + 1, 30)));
                    u++;
                }
            }

            Configs.nextWaveState = 3;
        }

        private static void UpdateWaveStage3(long time)
        {
            if (ships.Count == 0)
            {
                Configs.wavePerStar[Configs.nextWaveStarIndex]++;
                Configs.nextWaveState = 0;
                Configs.nextWaveFrameIndex = -1;
                Configs.nextWaveStarIndex = 0;
                Configs.nextWaveWormCount = 0;
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
            if (ships.Count == 0) return;
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
                if (ship.distanceToTarget > 250)
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
