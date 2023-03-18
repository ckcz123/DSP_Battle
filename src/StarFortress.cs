using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    public class StarFortress
    {
        // 下面进入存档
        public static List<int> moduleCapacity;
        public static List<ConcurrentDictionary<int, int>> moduleComponentCount;// 存储已完成的组件数，除以每模块的组件需求数量就是已经完成的模块数
        public static List<List<int>> moduleMaxCount; // 存储规划的模块数
        public static List<ConcurrentDictionary<int, int>> moduleComponentInProgress; // 已发射了组件运载火箭但还未建成的
        public static int remindPlayerWhenDestruction = 1; // 减少模块数量时，减少后的上限少于已经建造的数量时，就拆除，是否需要提醒玩家
        // 下面不进入存档
        static int lockLoop = 0; // 为减少射击弹道飞行过程中重复锁定同一个敌人导致伤害溢出的浪费，恒星要塞的炮会依次序攻击队列中第lockLoop序号的敌人，且每次攻击后此值+1（对一个循环上限取余，循环上线取决于射击频率，原则上射击频率越快循环上限越大，循环上限loop通过FireCannon函数传入）
        static List<List<bool>> rocketsRequireMap; // 每帧刷新一部分，每秒进行一次完整刷新，记录是否需要发射恒星要塞组件火箭
        static List<int> battleStarModuleBuiltCount = new List<int> { 0, 0, 0, 0 }; // 每秒刷新，记录战斗星系的恒星要塞各模块已建成的数量
        public static int cannonChargeProgress = 0; // 战斗所在星系的光矛充能，不进入存档

        public static int energyPerModule = 1000000;
        public static List<int> compoPerModule = new List<int> { 3, 4, 4, 4 }; // 测试前后务必修改

        public static void InitAll()
        {
            StarFortressSilo.InitAll();
            UIStarFortress.InitAll();
            moduleCapacity = new List<int>();
            moduleComponentCount = new List<ConcurrentDictionary<int, int>>();
            moduleMaxCount = new List<List<int>>();
            moduleComponentInProgress = new List<ConcurrentDictionary<int, int>>();
            rocketsRequireMap = new List<List<bool>>();
            battleStarModuleBuiltCount = new List<int> { 0, 0, 0, 0 };
            cannonChargeProgress = 0;
            for (int i = 0; i < 1024; i++)
            {
                moduleCapacity.Add(0);
                moduleComponentCount.Add(new ConcurrentDictionary<int, int>());
                moduleMaxCount.Add(new List<int> { 0, 0, 0, 0 });
                moduleComponentInProgress.Add(new ConcurrentDictionary<int, int>());
                rocketsRequireMap.Add(new List<bool> { false, false, false, false });
                moduleComponentCount[i].AddOrUpdate(0, 0, (x, y) => 0);
                moduleComponentCount[i].AddOrUpdate(1, 0, (x, y) => 0);
                moduleComponentCount[i].AddOrUpdate(2, 0, (x, y) => 0);
                moduleComponentCount[i].AddOrUpdate(3, 0, (x, y) => 0);
                moduleComponentInProgress[i].AddOrUpdate(0, 0, (x, y) => 0);
                moduleComponentInProgress[i].AddOrUpdate(1, 0, (x, y) => 0);
                moduleComponentInProgress[i].AddOrUpdate(2, 0, (x, y) => 0);
                moduleComponentInProgress[i].AddOrUpdate(3, 0, (x, y) => 0);
            }
            remindPlayerWhenDestruction = 1;
        }

        // 由Silo调用
        public static bool NeedRocket(DysonSphere sphere, int rocketId)
        {
            if (sphere == null) return false;
            int starIndex = sphere.starData.index;
            int index = rocketId - 8037;
            index = Math.Min(Math.Max(0, index), 2);
            return rocketsRequireMap[starIndex][index];
        }

        // 可能被多线程调用
        public static void ConstructStarFortPoint(int starIndex, int rocketProtoId)
        {
            int index = rocketProtoId - 8037;
            index = Math.Min(Math.Max(0, index), 2);
            moduleComponentCount[starIndex].AddOrUpdate(index, 1, (x, y) => y + 1);
            moduleComponentInProgress[starIndex].AddOrUpdate(index,0, (x, y) => Math.Max(0, y - 1));
        }

        // 游戏每帧调用，逐步刷新全星系的是否需要火箭
        public static void RecalcRocketNeed(int begin, int end)
        {
            end = Math.Min(end, GameMain.galaxy.starCount);
            if (end <= begin) return;
            if (begin >= GameMain.galaxy.starCount) return;

            for (int starIndex = begin; starIndex < end; starIndex++)
            {
                var alreadyBuilt = CalcModuleBuilt(starIndex);
                for (int i = 0; i < rocketsRequireMap[starIndex].Count; i++)
                {
                    rocketsRequireMap[starIndex][i] = moduleComponentCount[starIndex][i] + moduleComponentInProgress[starIndex][i] < moduleMaxCount[starIndex][i] * compoPerModule[i];
                }
            }
        }

        // 不被多线程调用，交互时可能调用，或每多少帧的时候调用
        public static void ReCalcData(ref DysonSphere sphere)
        {
            if (sphere == null) return;
            int starIndex = sphere.starData.index;
            moduleCapacity[starIndex] = (int)(sphere.energyGenCurrentTick_Layers / energyPerModule);
            if (moduleCapacity[starIndex] < 10) // 恒星要塞需要一个最小巨构能量水平才能开启
                moduleCapacity[starIndex] = 0;
            else 
            {
                moduleCapacity[starIndex] += CalcModuleBuilt(starIndex)[2];
            }

            // 如果拆除戴森球壳面导致容量下降，需要执行对模块的拆除

            // 计算已建成的是否超过上限
            for (int i = 0; i < 4; i++)
            {
                moduleComponentCount[starIndex].AddOrUpdate(i, 0, (x, y) => Math.Min(moduleMaxCount[starIndex][i] * compoPerModule[i], y));
            }

        }

        /// <summary>
        /// 已经建造完成的模块数
        /// </summary>
        /// <param name="starIndex"></param>
        /// <returns></returns>
        public static List<int> CalcModuleBuilt(int starIndex) 
        {
            List<int> res = new List<int> { 0, 0, 0, 0 };
            if (starIndex < 0 || starIndex >= moduleMaxCount.Count) return res;
            for (int i = 0; i < 4; i++)
            {
                res[i] = Math.Min(moduleMaxCount[starIndex][i], moduleComponentCount[starIndex][i] / compoPerModule[i]);
            }
            return res;
        }

        // 容量剩余
        public static int CapacityRemaining(int starIndex)
        {
            int sum = moduleMaxCount[starIndex][0]+moduleMaxCount[starIndex][1];
            return moduleCapacity[starIndex] - sum;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(long time)
        {
            int starCount = GameMain.galaxy.starCount;
            int starsPerFrame = Math.Max(1, starCount / 60);
            int f = (int)(time % 60);
            RecalcRocketNeed(f * starsPerFrame, Math.Max(starCount, (f + 1) * starsPerFrame));

            if (UIStarFortress.curDysonSphere == null) return;
            if (time % 60 == 45)
            {
                ReCalcData(ref UIStarFortress.curDysonSphere);
                UIStarFortress.RefreshAll();
            }
            else if (time % 60 == 46) // 为了战斗时防止每帧都重新计算，仅每秒计算后存储
            {
                List<int> built = CalcModuleBuilt(Configs.nextWaveStarIndex);
                battleStarModuleBuiltCount[0] = built[0];
                battleStarModuleBuiltCount[1] = built[1];
                battleStarModuleBuiltCount[2] = built[2];
                battleStarModuleBuiltCount[3] = built[3];
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSphere), "GameTick")]
        public static void StarFortressGameTick(ref DysonSphere __instance, long gameTick)
        {
            int starIndex = __instance.starData.index;

            if (Configs.nextWaveState == 3  && starIndex == Configs.nextWaveStarIndex)
            {
                if (cannonChargeProgress >= 600 && battleStarModuleBuiltCount[1] > 0) // 光矛开火
                {
                    int fireTimes = cannonChargeProgress / 600;
                    for (int i = 0; i < fireTimes; i++)
                    {
                        FireCannon(ref __instance.swarm);
                    }
                    cannonChargeProgress %= 600;
                }
                cannonChargeProgress += battleStarModuleBuiltCount[1];


                // 发射导弹的速度暂定为：每个导弹模块提供1导弹/10s的发射速度
                if (gameTick % 60 == 0) // 最快也是每秒才会发射一次，依此可以不发射或发射多个导弹
                {
                    int launchCount = 0; // 计算后得到的发射数量
                    int missileModuleCount = battleStarModuleBuiltCount[0];
                    if (missileModuleCount >= 10)
                    {
                        int over = missileModuleCount % 10;
                        launchCount = missileModuleCount / 10;
                        if (gameTick % 600 < 60 * over)
                            launchCount += 1;
                    }
                    else if (missileModuleCount > 0)
                    {
                        if (gameTick % 600 < 60 * missileModuleCount) // 不能超过每秒发射一个的速度的时候，则每10s的前第整n秒发射一发
                        {
                            launchCount = 1;
                        }
                    }
                    System.Random rand = new System.Random();

                    for (int i = 0; i < launchCount; i++)
                    {
                        DysonNode node = null;
                        int beginLayerIndex = rand.Next(1, 10);
                        // 寻找第一个壳面
                        for (int layerIndex = beginLayerIndex; layerIndex < 10; layerIndex = (layerIndex + 1) % 10)
                        {
                            if (__instance.layersIdBased.Length > layerIndex && __instance.layersIdBased[layerIndex] != null && __instance.layersIdBased[layerIndex].nodeCount > 0)
                            {
                                DysonSphereLayer layer = __instance.layersIdBased[layerIndex];
                                bool found = false; // 寻找到可用的发射node之后，发射导弹，一直break到外面
                                int beginNodeIndex = rand.Next(0, Math.Max(1, layer.nodePool.Length));
                                for (int nodeIndex = beginNodeIndex; nodeIndex < layer.nodeCursor && nodeIndex < layer.nodeCursor; nodeIndex++)
                                {
                                    if (layer.nodePool[nodeIndex] != null)
                                    {
                                        found = true;
                                        LauchMissile(layer, layer.nodePool[nodeIndex]);
                                        break;
                                    }
                                }
                                if (found)
                                    break;
                                for (int nodeIndex = 0; nodeIndex < beginNodeIndex; nodeIndex++)
                                {
                                    if (layer.nodePool[nodeIndex] != null)
                                    {
                                        found = true;
                                        LauchMissile(layer, layer.nodePool[nodeIndex]);
                                        break;
                                    }
                                }
                                if (found)
                                    break;
                            }

                            if (layerIndex == beginLayerIndex - 1) break;
                        }
                    }
                }
            }
        }

        public static void LauchMissile(DysonSphereLayer layer, DysonNode node)
        {
            if (node == null) return;
            StarData star = layer.starData;
            int starIndex = star.index;
            Vector3 nodeUPos = layer.NodeUPos(node);
            Vector3 starUPos = star.uPosition;
            int targetIndex = MissileSilo.FindTarget(starIndex, star.id * 100 + 1);

            DysonRocket dysonRocket = default(DysonRocket);
            dysonRocket.planetId = star.id * 100 + 1;
            dysonRocket.uPos = nodeUPos;
            dysonRocket.uRot = Quaternion.LookRotation(nodeUPos-starUPos, new Vector3(0,1,0));
            dysonRocket.uVel = dysonRocket.uRot * Vector3.forward;
            dysonRocket.uSpeed = 0f;
            dysonRocket.launch = (nodeUPos-starUPos).normalized;
            //sphere.AddDysonRocket(dysonRocket, autoDysonNode); //原本
            int rocketIndex = MissileSilo.AddDysonRockedGniMaerd(ref layer.dysonSphere, ref dysonRocket, null); //这是添加了一个目标戴森球节点为null的火箭，因此被判定为导弹

            MissileSilo.MissileTargets[starIndex][rocketIndex] = targetIndex;
            MissileSilo.missileProtoIds[starIndex][rocketIndex] = 8008; // 虽然伤害是按照反物质导弹的但是序号是8008不计入导弹统计
            //int damage = 0;
            //if (__instance.bulletId == 8004) damage = Configs.missile1Atk;
            //else if (__instance.bulletId == 8005) damage = Configs.missile2Atk;
            //else if (__instance.bulletId == 8006) damage = Configs.missile3Atk;
            ////注册导弹
            //UIBattleStatistics.RegisterShootOrLaunch(__instance.bulletId, damage);
        }

        public static void FireCannon(ref DysonSwarm swarm, int loop = 20)
        {
            lockLoop = (lockLoop + 1) % loop;
            int starIndex = Configs.nextWaveStarIndex;
            StarData star = swarm.starData;

            try // 仅在快结束战斗时可能会越界报错，推测是sorted的问题。
            {
                List<EnemyShip> sortedShips = EnemyShips.sortedShips(1, starIndex, starIndex * 100 + 101);
                int targetIndex = MissileSilo.FindTarget(starIndex, starIndex * 100 + 101);
                int bulletIndex;
                if (targetIndex < 0) return;
                EnemyShip enemyShip = sortedShips[Math.Min(lockLoop, sortedShips.Count)];
                Vector3 targetUPos = star.uPosition;
                if (enemyShip != null && enemyShip.state == EnemyShip.State.active)
                    targetUPos = enemyShip.uPos;
                else
                    return;
                float t = (float)((VectorLF3)targetUPos - star.uPosition).magnitude / 250000f;

                //下面是添加子弹
                for (int i = 0; i < 10; i++)
                {
                    VectorLF3 randDelta = Utils.RandPosDelta();
                    bulletIndex = swarm.AddBullet(new SailBullet
                    {
                        maxt = t + i * 0.01f,
                        lBegin = star.uPosition,
                        uEndVel = targetUPos, //至少影响着形成的太阳帆的初速度方向
                        uBegin = star.uPosition + randDelta,
                        uEnd = (VectorLF3)targetUPos + randDelta
                    }, 2);

                    try
                    {
                        if (bulletIndex != -1)
                            swarm.bulletPool[bulletIndex].state = 0; //设置成0，该子弹将不会生成太阳帆
                    }
                    catch (Exception)
                    {
                        DspBattlePlugin.logger.LogInfo("bullet info1 set error.");
                    }

                    if (bulletIndex != -1)
                        Cannon.bulletTargets[swarm.starData.index].AddOrUpdate(bulletIndex, enemyShip.shipIndex, (x, y) => enemyShip.shipIndex);

                    //Main.logger.LogInfo("bullet info2 set error.");


                    try
                    {
                        int bulletId = 8009;
                        if (bulletIndex != -1)
                            Cannon.bulletIds[swarm.starData.index].AddOrUpdate(bulletIndex, bulletId, (x, y) => bulletId);
                        // bulletIds[swarm.starData.index][bulletIndex] = 1;//后续可以根据子弹类型/炮类型设定不同数值
                    }
                    catch (Exception)
                    {
                        DspBattlePlugin.logger.LogInfo("bullet info8009 set error.");
                    }
                }
            }
            catch (Exception)
            {

            }
        }


        public static void Export(BinaryWriter w)
        {
            
        }

        public static void Import(BinaryReader r)
        {
            InitAll();
        }

        public static void IntoOtherSave()
        {
            InitAll();
        }
    }
}
