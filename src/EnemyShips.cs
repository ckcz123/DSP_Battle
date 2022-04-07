using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

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
        //public static string meshDir = "entities/models/space capsule/space-capsule"; //是十字外面一个圈，很小
        public static bool shouldDistroy = true;

        public static void Init()
        {
            ships = new ConcurrentDictionary<int, EnemyShip>();
            RemoveEntities.Init();
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
            return s != null && s.id != 0 && s.gid != 0 && s.isStellar && !s.isCollector && !RemoveEntities.distroyedStation.ContainsKey(s.gid);
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
                    RemoveEntities.Add(ship, station);

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

            if (time % 20 == 1) RemoveEntities.CheckPendingDestroyedEntities();

            // time is the frame since start
            if (time % 30 == 1)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((e) => SortShips()));
            }

            WaveStages.Update(time);

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
                        DysonSwarm swarm = RendererSphere.enemySpheres[starIndex].swarm;
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

        static List<LogisticShipUIRenderer> enemyShipsUIRenderers = new List<LogisticShipUIRenderer>(); //List是为了后续还能支持不同的敌舰大小或其他模型，目前只用第[0]个测试

        public static void InitRenderers()
        {
            enemyShipsUIRenderers = new List<LogisticShipUIRenderer>();
            for (int i = 0; i < 6; i++)
            {
                enemyShipsUIRenderers.Add(new LogisticShipUIRenderer(GameMain.data.galacticTransport));
            }
            enemyShipsUIRenderers[0].shipMesh = Resources.Load<Mesh>("test/tgs demo/enemy-base");
            Mesh mesh = enemyShipsUIRenderers[0].shipMesh;
            var oriVerts = mesh.vertices;
            for (int i = 0; i < oriVerts.Length; i++)
            {
                Vector3 vert = oriVerts[i];
                vert.x *= 2;
                vert.y *= 2;
                vert.z *= 2;
                oriVerts[i] = vert;
            }
            mesh.vertices = oriVerts;
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogisticShipUIRenderer), "Update")]
        public static void LogisticShipUIRenderer_Update(ref LogisticShipUIRenderer __instance)
        {
            if (ships.Count == 0) return;
            UIStarmap uiStarMap = enemyShipsUIRenderers[0].uiStarmap;

            if (__instance.transport == null || uiStarMap == null || !uiStarMap.active) return;

            enemyShipsUIRenderers[0].shipCount = 0; //我试了不归零貌似也不行
            int shipCount = enemyShipsUIRenderers[0].shipCount;

            while (enemyShipsUIRenderers[0].capacity < shipCount + ships.Count)
            {
                enemyShipsUIRenderers[0].Expand2x();
            }

            ComputeBuffer shipsBuffer = enemyShipsUIRenderers[0].shipsBuffer;
            ShipUIRenderingData[] shipsArr = enemyShipsUIRenderers[0].shipsArr;

            foreach (var ship in ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                shipsArr[shipCount] = ship.renderingUIData;
                shipsArr[shipCount].rpos = (shipsArr[shipCount].upos - uiStarMap.viewTargetUPos) * 0.00025;
                shipCount++;
                if (shipCount == 1)
                    Utils.Log($"shipsarr pos =  {shipsArr[shipCount].upos} and real pos = {ship.shipData.uPos}");
            }
            enemyShipsUIRenderers[0].shipCount = shipCount;

            if (shipsBuffer != null)
            {
                shipsBuffer.SetData(shipsArr, 0, 0, shipCount);
            }
            enemyShipsUIRenderers[0].shipsArr = shipsArr;
            enemyShipsUIRenderers[0].shipsBuffer = shipsBuffer;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "OnDraw")]
        public static void GameData_OnDraw(ref GameData __instance, int frame)
        {
            if (__instance.galacticTransport != null && ships.Count != 0)
            {
                __instance.galacticTransport.shipRenderer.Update();
                enemyShipsUIRenderers[0].Draw();
            }
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
            RemoveEntities.Init();
            SortShips();
        }

        public static void IntoOtherSave()
        {
            ships.Clear();
            RemoveEntities.Init();
            SortShips();
        }
    }
}
