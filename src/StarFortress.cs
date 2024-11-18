using BepInEx;
using BepInEx.Logging;
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
        public static List<ConcurrentDictionary<int, int>> moduleComponentInProgress; // 已发射了组件运载火箭但还未飞到节点内部的
        public static List<List<int>> moduleMaxCount; // 存储规划的模块数
        public static int remindPlayerWhenDestruction = 1; // 减少模块数量时，减少后的上限少于已经建造的数量时，就拆除，是否需要提醒玩家
        // 下面不进入存档
        static int lockLoop = 0; // 由于光矛伤害改为即时命中，此项功能已失去实际意义。为减少射击弹道飞行过程中重复锁定同一个敌人导致伤害溢出的浪费，恒星要塞的炮会依次序攻击队列中第lockLoop序号的敌人，且每次攻击后此值+1（对一个循环上限取余，循环上线取决于射击频率，原则上射击频率越快循环上限越大，循环上限loop通过FireCannon函数传入）
        static List<List<bool>> rocketsRequireMap; // 每帧刷新一部分，每秒进行一次完整刷新，记录是否需要发射恒星要塞组件火箭
        static List<int> battleStarModuleBuiltCount = new List<int> { 0, 0, 0, 0 }; // 每秒刷新，记录战斗星系的恒星要塞各模块已建成的数量
        public static double cannonChargeProgress = 0; // 战斗所在星系的光矛充能，不进入存档

        public static int energyPerModule = 1000000;
        public static List<int> compoPerModule = new List<int> { 20, 200, 200, 200 }; // 测试前后务必修改
        public static int lightSpearDamage = 20000;
        public static int RefreshDataCountDown = 120; // 每次载入游戏时，前两秒不刷新数据
        public static List<int> randInc = new List<int> { -9, -7, -3, -1, 1, 3, 7, 9 };
        public static int randIncLength = 8;

        public static void InitAll()
        {
        //    StarFortressSilo.InitAll();
        //    UIStarFortress.InitAll();
        //    moduleCapacity = new List<int>();
        //    moduleComponentCount = new List<ConcurrentDictionary<int, int>>();
        //    moduleMaxCount = new List<List<int>>();
        //    moduleComponentInProgress = new List<ConcurrentDictionary<int, int>>();
        //    rocketsRequireMap = new List<List<bool>>();
        //    battleStarModuleBuiltCount = new List<int> { 0, 0, 0, 0 };
        //    cannonChargeProgress = 0;
        //    for (int i = 0; i < 1024; i++)
        //    {
        //        moduleCapacity.Add(0);
        //        moduleComponentCount.Add(new ConcurrentDictionary<int, int>());
        //        moduleMaxCount.Add(new List<int> { 0, 0, 0, 0 });
        //        moduleComponentInProgress.Add(new ConcurrentDictionary<int, int>());
        //        rocketsRequireMap.Add(new List<bool> { false, false, false, false });
        //        moduleComponentCount[i].AddOrUpdate(0, 0, (x, y) => 0);
        //        moduleComponentCount[i].AddOrUpdate(1, 0, (x, y) => 0);
        //        moduleComponentCount[i].AddOrUpdate(2, 0, (x, y) => 0);
        //        moduleComponentCount[i].AddOrUpdate(3, 0, (x, y) => 0);
        //        moduleComponentInProgress[i].AddOrUpdate(0, 0, (x, y) => 0);
        //        moduleComponentInProgress[i].AddOrUpdate(1, 0, (x, y) => 0);
        //        moduleComponentInProgress[i].AddOrUpdate(2, 0, (x, y) => 0);
        //        moduleComponentInProgress[i].AddOrUpdate(3, 0, (x, y) => 0);
        //    }
        //    remindPlayerWhenDestruction = 1;
        //    RefreshDataCountDown = 120;
        }

        //// 由Silo调用
        //public static bool NeedRocket(DysonSphere sphere, int rocketId)
        //{
        //    if (sphere == null) return false;
        //    int starIndex = sphere.starData.index;
        //    int index = rocketId - 8037;
        //    index = Math.Min(Math.Max(0, index), 2);
        //    return rocketsRequireMap[starIndex][index];
        //}

        //// 可能被多线程调用
        //public static void ConstructStarFortPoint(int starIndex, int rocketProtoId, int count = 1)
        //{
        //    int index = rocketProtoId - 8037;
        //    index = Math.Min(Math.Max(0, index), 2);
        //    moduleComponentCount[starIndex].AddOrUpdate(index, 1, (x, y) => y + count);
        //    if(count == 1)
        //        moduleComponentInProgress[starIndex].AddOrUpdate(index,0, (x, y) => Math.Max(0, y - 1));
        //}

        //// 游戏每帧调用，逐步刷新全星系的是否需要火箭
        //public static void RecalcRocketNeed(int begin, int end)
        //{
        //    end = Math.Min(end, GameMain.galaxy.starCount);
        //    if (end <= begin) return;
        //    if (begin >= GameMain.galaxy.starCount) return;

        //    for (int starIndex = begin; starIndex < end; starIndex++)
        //    {
        //        var alreadyBuilt = CalcModuleBuilt(starIndex);
        //        for (int i = 0; i < rocketsRequireMap[starIndex].Count; i++)
        //        {
        //            rocketsRequireMap[starIndex][i] = moduleComponentCount[starIndex][i] + moduleComponentInProgress[starIndex][i] < moduleMaxCount[starIndex][i] * compoPerModule[i];
        //        }
        //    }
        //}

        //// 不被多线程调用，交互时可能调用，或每多少帧的时候调用
        //public static void ReCalcData(ref DysonSphere sphere)
        //{
        //    if (sphere == null) return;
        //    if (RefreshDataCountDown > 0) return;
        //    int starIndex = sphere.starData.index;
        //    moduleCapacity[starIndex] = (int)(sphere.energyGenCurrentTick_Layers / energyPerModule);
        //    if (moduleCapacity[starIndex] < 10) // 恒星要塞需要一个最小巨构能量水平才能开启
        //        moduleCapacity[starIndex] = 0;
        //    else 
        //    {
        //        moduleCapacity[starIndex] += CalcModuleBuilt(starIndex)[2];
        //    }

        //    // 如果拆除戴森球壳面导致容量下降，需要执行对模块的拆除
        //    int overflow = -CapacityRemaining(starIndex);
        //    if (overflow > 0)
        //    {
        //        int destructCount = Math.Max(1, overflow / 100);
        //        float cannonRatio = moduleMaxCount[starIndex][1] * 1.0f / (moduleMaxCount[starIndex][0] + moduleMaxCount[starIndex][1]);
        //        int destructCannonModule = (int)(destructCount * cannonRatio);
        //        int destructMissileModule = destructCount - destructCannonModule;
        //        moduleMaxCount[starIndex][0] = Math.Max(0, moduleMaxCount[starIndex][0] - destructMissileModule);
        //        moduleMaxCount[starIndex][1] = Math.Max(0, moduleMaxCount[starIndex][1] - destructCannonModule);
        //    }

        //    // 计算已建成的是否超过上限
        //    for (int i = 0; i < 4; i++)
        //    {
        //        moduleComponentCount[starIndex].AddOrUpdate(i, 0, (x, y) => Math.Min(moduleMaxCount[starIndex][i] * compoPerModule[i], y));
        //    }

        //}

        ///// <summary>
        ///// 已经建造完成的模块数
        ///// </summary>
        ///// <param name="starIndex"></param>
        ///// <returns></returns>
        //public static List<int> CalcModuleBuilt(int starIndex) 
        //{
        //    List<int> res = new List<int> { 0, 0, 0, 0 };
        //    if (starIndex < 0 || starIndex >= moduleMaxCount.Count) return res;
        //    for (int i = 0; i < 4; i++)
        //    {
        //        res[i] = moduleComponentCount[starIndex][i] / compoPerModule[i];
        //    }
        //    return res;
        //}

        //// 容量剩余
        //public static int CapacityRemaining(int starIndex)
        //{
        //    int sum = moduleMaxCount[starIndex][0]+moduleMaxCount[starIndex][1];
        //    return moduleCapacity[starIndex] - sum;
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(GameData), "GameTick")]
        //public static void GameData_GameTick(long time)
        //{
        //    if (RefreshDataCountDown > 0)
        //        RefreshDataCountDown -= 1;
        //    int starCount = GameMain.galaxy.starCount;
        //    int starsPerFrame = Math.Max(1, starCount / 60);
        //    int f = (int)(time % 60);
        //    int end = Math.Min((f + 1) * starsPerFrame, starCount);
        //    if (f == 59) end = starCount;
        //    for (int i = f * starsPerFrame; i < end; i++)
        //    {
        //        if (GameMain.data.dysonSpheres != null && i < GameMain.data.dysonSpheres.Length)
        //        {
        //            DysonSphere sphere = GameMain.data.dysonSpheres[i];
        //            ReCalcData(ref sphere);
        //        }
        //    }
        //    RecalcRocketNeed(f * starsPerFrame, end);

        //    if (time % 60 == 45) // 为了战斗时防止每帧都重新计算，仅每秒计算后存储
        //    {
        //        List<int> built = CalcModuleBuilt(Configs.nextWaveStarIndex);
        //        battleStarModuleBuiltCount[0] = built[0];
        //        battleStarModuleBuiltCount[1] = built[1];
        //        battleStarModuleBuiltCount[2] = built[2];
        //        battleStarModuleBuiltCount[3] = built[3];
        //    }

        //    if (UIStarFortress.curDysonSphere == null) return;
        //    if (time % 60 == 46)
        //    {
        //        UIStarFortress.RefreshAll();
        //    }
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(DysonSphere), "GameTick")]
        //public static void StarFortressGameTick(ref DysonSphere __instance, long gameTick)
        //{
        //    int starIndex = __instance.starData.index;

        //    if (Configs.nextWaveState == 3  && starIndex == Configs.nextWaveStarIndex)
        //    {
        //        if (cannonChargeProgress >= 6000 && battleStarModuleBuiltCount[1] > 0 ) // 光矛开火 6000
        //        {
        //            FireCannon(ref __instance.swarm,4);
        //            cannonChargeProgress %= 6000;
        //        }
        //        int cannonModuleCount = battleStarModuleBuiltCount[1];
        //        cannonChargeProgress += 1000.0 * (cannonModuleCount)/(299.0+cannonModuleCount); // 充能速度有一个上限，就是1000/帧，也就是说发射速度有每秒10次的上限（因为充能满需要6000）


        //        // 发射导弹的速度暂定为：每个导弹模块提供1导弹/10s的发射速度
        //        int launchCheck = 60;
        //        int divisor = 100; // 这是由导弹模块的射速决定的
        //        //if (UIBattleStatistics.battleTime > 5400) divisor = 1000;
        //        if (gameTick % launchCheck == 0) // 最快也是每秒才会发射一次（发射数量为模块数的二十分之一），因此每秒可能不发射或发射多个导弹。// 已移除：如果战斗已超过90s，射速降低至1%
        //        {
        //            int launchCount = 0; // 计算后得到的发射数量
        //            int missileModuleCount = battleStarModuleBuiltCount[0];
        //            if (missileModuleCount >= divisor)
        //            {
        //                int over = missileModuleCount % divisor;
        //                launchCount = missileModuleCount / divisor;
        //                if (gameTick % (divisor * launchCheck) / 60 < over)
        //                    launchCount += 1;
        //            }
        //            else if (missileModuleCount > 0)
        //            {
        //                if (gameTick % (divisor * launchCheck) / 60 < missileModuleCount) // 不能超过每秒发射一个的速度的时候，则每10s的前第整n秒发射一发
        //                {
        //                    launchCount = 1;
        //                }
        //            }
        //            System.Random rand = new System.Random();
        //            //launchCount = launchCount > 50 ? 50 : launchCount;
        //            for (int i = 0; i < launchCount; i++)
        //            {
        //                DysonNode node = null;
        //                int beginLayerIndex = rand.Next(1, 10);
        //                int inc = randInc[rand.Next(0,randInc.Count)];
        //                // 寻找第一个壳面
        //                for (int layerIndex = (beginLayerIndex + inc + 10) % 10; layerIndex < 10; layerIndex = (layerIndex + inc + 10) % 10)
        //                {
        //                    if (__instance.layersIdBased.Length > layerIndex && __instance.layersIdBased[layerIndex] != null && __instance.layersIdBased[layerIndex].nodeCount > 0)
        //                    {
        //                        DysonSphereLayer layer = __instance.layersIdBased[layerIndex];
        //                        bool found = false; // 寻找到可用的发射node之后，发射导弹，一直break到外面
        //                        int beginNodeIndex = rand.Next(0, Math.Max(1, layer.nodeCursor));
        //                        int nodeInc = layer.nodeCursor % inc == 0 ? 1 : inc + layer.nodeCursor;
        //                        nodeInc = nodeInc <= 0 ? 1 : nodeInc;
        //                        for (int nodeIndex = (beginNodeIndex + nodeInc) % layer.nodeCursor; nodeIndex < layer.nodeCursor && nodeIndex < layer.nodeCursor; nodeIndex = (nodeIndex + nodeInc) % layer.nodeCursor)
        //                        {
        //                            if (layer.nodePool[nodeIndex] != null)
        //                            {
        //                                found = true;
        //                                LauchMissile(layer, layer.nodePool[nodeIndex]);
        //                                break;
        //                            }
        //                            if (nodeIndex == beginNodeIndex) break;
        //                        }
        //                        if (found)
        //                            break;
        //                        for (int nodeIndex = 0; nodeIndex < beginNodeIndex; nodeIndex++)
        //                        {
        //                            if (layer.nodePool[nodeIndex] != null)
        //                            {
        //                                found = true;
        //                                LauchMissile(layer, layer.nodePool[nodeIndex]);
        //                                break;
        //                            }
        //                        }
        //                        if (found)
        //                            break;
        //                    }

        //                    if (layerIndex == beginLayerIndex) break;
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 用于在战斗时屏蔽一些不重要的log
        ///// </summary>
        ///// <param name="__instance"></param>
        ///// <param name="sender"></param>
        ///// <param name="eventArgs"></param>
        ///// <returns></returns>
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(BepInEx.Logging.ConsoleLogListener), "LogEvent")]
        //public static bool BepInExLogEventPatch(ref BepInEx.Logging.ConsoleLogListener __instance, object sender, LogEventArgs eventArgs)
        //{
        //    if ((eventArgs.Level & (LogLevel.Error | LogLevel.Warning)) != LogLevel.None && Configs.nextWaveState == 3)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static void LauchMissile(DysonSphereLayer layer, DysonNode node)
        //{
        //    if (node == null) return;
        //    StarData star = layer.starData;
        //    int starIndex = star.index;
        //    Vector3 nodeUPos = layer.NodeUPos(node);
        //    Vector3 starUPos = star.uPosition;
        //    int targetIndex = MissileSilo.FindTarget(starIndex, star.id * 100 + 1);
        //    if (targetIndex < 0) return;

        //    DysonRocket dysonRocket = default(DysonRocket);
        //    dysonRocket.planetId = star.id * 100 + 1;
        //    dysonRocket.uPos = nodeUPos;
        //    dysonRocket.uRot = Quaternion.LookRotation(nodeUPos-starUPos, new Vector3(0,1,0));
        //    dysonRocket.uVel = dysonRocket.uRot * Vector3.forward;
        //    dysonRocket.uSpeed = 0f;
        //    dysonRocket.launch = (nodeUPos-starUPos).normalized;
        //    //sphere.AddDysonRocket(dysonRocket, autoDysonNode); //原本
        //    int rocketIndex = MissileSilo.AddDysonRockedGniMaerd(ref layer.dysonSphere, ref dysonRocket, null); //这是添加了一个目标戴森球节点为null的火箭，因此被判定为导弹

        //    MissileSilo.MissileTargets[starIndex][rocketIndex] = targetIndex;
        //    MissileSilo.missileProtoIds[starIndex][rocketIndex] = 8008; // 虽然伤害是按照反物质导弹的但是序号是8008不计入导弹统计
        //    //int damage = 0;
        //    //if (__instance.bulletId == 8004) damage = Configs.missile1Atk;
        //    //else if (__instance.bulletId == 8005) damage = Configs.missile2Atk;
        //    //else if (__instance.bulletId == 8006) damage = Configs.missile3Atk;
        //    ////注册导弹
        //    //UIBattleStatistics.RegisterShootOrLaunch(__instance.bulletId, damage);
        //}

        //public static void FireCannon(ref DysonSwarm swarm, int loop = 100)
        //{
        //    lockLoop = (lockLoop + 1) % loop;
        //    int starIndex = Configs.nextWaveStarIndex;
        //    StarData star = swarm.starData;

        //    try // 仅在快结束战斗时可能会越界报错，推测是sorted的问题。
        //    {
        //        List<EnemyShip> sortedShips = EnemyShips.sortedShips(1, starIndex, starIndex * 100 + 101);
        //        int targetIndex = MissileSilo.FindTarget(starIndex, starIndex * 100 + 101);
        //        int bulletIndex;
        //        if (targetIndex < 0) return;
        //        EnemyShip enemyShip = sortedShips[Math.Min(lockLoop, sortedShips.Count)];
        //        Vector3 targetUPos = star.uPosition;
        //        if (enemyShip != null && enemyShip.state == EnemyShip.State.active)
        //            targetUPos = enemyShip.uPos;
        //        else
        //            return;
        //        float t = (float)((VectorLF3)targetUPos - star.uPosition).magnitude / 250000f;
        //        Vector3 finalUPos = star.uPosition + ((VectorLF3)targetUPos - star.uPosition).normalized * 400000;
        //        //下面是添加子弹，并且伤害立刻结算
        //        for (int i = 0; i < 100; i++)
        //        {
        //            VectorLF3 randDelta = Utils.RandPosDelta() * 5;
        //            bulletIndex = swarm.AddBullet(new SailBullet
        //            {
        //                maxt = 0.2f + i>10?0.1f:0.0f,
        //                lBegin = star.uPosition,
        //                uEndVel = targetUPos, //至少影响着形成的太阳帆的初速度方向
        //                uBegin = star.uPosition + randDelta,
        //                uEnd = ((VectorLF3)finalUPos-star.uPosition)*(100.0-i)/100.0 + star.uPosition + randDelta
        //            }, 2);

        //            try
        //            {
        //                if (bulletIndex != -1)
        //                    swarm.bulletPool[bulletIndex].state = 0; //设置成0，该子弹将不会生成太阳帆
        //            }
        //            catch (Exception)
        //            {
        //                DspBattlePlugin.logger.LogInfo("bullet info1 set error.");
        //            }
        //        }
        //        // 立即结算伤害
        //        int damage = Configs.lightSpearAtk;
        //        int realDamage = enemyShip.BeAttacked(damage, DamageType.laser); //击中造成伤害
        //        if (realDamage > 0) // 被闪避了则不算击中
        //            UIBattleStatistics.RegisterHit(8009, realDamage, 1);
        //        if (Relic.HaveRelic(3, 7)) // relic3-7 虚空折射 子弹命中时对一个随机敌人造成20%额外伤害
        //        {
        //            int refDmg = Relic.BonusDamage(damage, 0.2) - 50000;
        //            int randNum = -1;
        //            if (EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex].Count > 0)
        //                randNum = Utils.RandInt(0, EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex].Count);
        //            if (randNum >= 0 && EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex][randNum] != null && EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex][randNum].state == EnemyShip.State.active)
        //            {
        //                int realRefDmg = EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex][randNum].BeAttacked(refDmg, DamageType.laser);
        //                UIBattleStatistics.RegisterHit(8009, realRefDmg, 0);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}


        public static void Export(BinaryWriter w)
        {
            w.Write(moduleCapacity.Count);
            for (int i = 0; i < moduleCapacity.Count; i++)
            {
                w.Write(moduleCapacity[i]);
            }
            w.Write(moduleComponentCount.Count);
            for (int i = 0; i < moduleComponentCount.Count; i++)
            {
                w.Write(moduleComponentCount[i].Count);
                foreach (var item in moduleComponentCount[i])
                {
                    w.Write(item.Key);
                    w.Write(item.Value);
                }
            }
            w.Write(moduleComponentInProgress.Count);
            for (int i = 0; i < moduleComponentInProgress.Count; i++)
            {
                w.Write(moduleComponentInProgress[i].Count);
                foreach (var item in moduleComponentInProgress[i])
                {
                    w.Write(item.Key);
                    w.Write(item.Value);
                }
            }
            w.Write(moduleMaxCount.Count);
            for (int i = 0; i < moduleMaxCount.Count; i++)
            {
                w.Write(moduleMaxCount[i].Count);
                for (int j = 0; j < moduleMaxCount[i].Count; j++)
                {
                    w.Write(moduleMaxCount[i][j]);
                }
            }
            w.Write(remindPlayerWhenDestruction);

            //StarFortressSilo.Export(w);
        }

        public static void Import(BinaryReader r)
        {
            InitAll();
            if (Configs.versionWhenImporting >= 30230319)
            {
                int total1 = r.ReadInt32();
                for (int i = 0; i < total1; i++)
                {
                    moduleCapacity[i] = r.ReadInt32();
                }
                int total2 = r.ReadInt32();
                for (int i = 0; i < total2; i++)
                {
                    int total2_1 = r.ReadInt32();
                    for (int j = 0; j < total2_1; j++)
                    {
                        int key = r.ReadInt32();
                        int value = r.ReadInt32();
                        moduleComponentCount[i].AddOrUpdate(key, value, (x,y)=>value); // 这里不用tryadd是因为InitAll里面对每个key（只有0123）都进行过了add
                    }
                }
                int total3 = r.ReadInt32();
                for (int i = 0; i < total3; i++)
                {
                    int total3_1 = r.ReadInt32();
                    for (int j = 0; j < total3_1; j++)
                    {
                        int key = r.ReadInt32();
                        int value = r.ReadInt32();
                        moduleComponentInProgress[i].AddOrUpdate(key, value, (x, y) => value);
                    }
                }
                int total4 = r.ReadInt32();
                for (int i = 0; i < total4; i++)
                {
                    int total4_1 = r.ReadInt32();
                    for (int j = 0; j < total4_1; j++)
                    {
                        moduleMaxCount[i][j] = r.ReadInt32();
                        if (j==0 && Configs.versionWhenImporting < 30230414) // 更新之后模块占用*5
                        {
                            moduleMaxCount[i][j] *= 5;
                        }
                    }
                }
                remindPlayerWhenDestruction = r.ReadInt32();
            }

            //StarFortressSilo.Import(r);
        }

        public static void IntoOtherSave()
        {
            InitAll();

            //StarFortressSilo.IntoOtherSave();
        }
    }
}
