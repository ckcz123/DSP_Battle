using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DSP_Battle
{
    // 因为暂停此功能，已阻止的方法包括public static void GameData_GameTick(long time);public static void StarFortressGameTick(ref DysonSphere __instance, long gameTick);
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
        public static List<List<int>> moduleBuiltCount = new List<List<int>>(); // 每秒刷新，记录战斗星系的恒星要塞各模块已建成的数量
        public static double cannonChargeProgress = 0; // 战斗所在星系的光矛充能，不进入存档

        public static int energyPerModule = 1000000;
        public static List<int> compoPerModule = new List<int> { 20, 200, 200, 200 }; // 测试前后务必修改
        public static int lightSpearDamage = 20000;
        public static int RefreshDataCountDown = 120; // 每次载入游戏时，前两秒不刷新数据
        public static List<int> randInc = new List<int> { -9, -7, -3, -1, 1, 3, 7, 9 };
        public static int randIncLength = 8;
        public static Dictionary<int, List<int>> enemyPoolByStarIndex = new Dictionary<int, List<int>>();


        public static void InitAll()
        {
            StarFortressSilo.InitAll();
            UIStarFortress.InitAll();
            moduleCapacity = new List<int>();
            moduleComponentCount = new List<ConcurrentDictionary<int, int>>();
            moduleMaxCount = new List<List<int>>();
            moduleComponentInProgress = new List<ConcurrentDictionary<int, int>>();
            rocketsRequireMap = new List<List<bool>>();
            moduleBuiltCount = new List<List<int>>();
            enemyPoolByStarIndex = new Dictionary<int, List<int>>();
            cannonChargeProgress = 0;
            for (int i = 0; i < 1024; i++)
            {
                moduleCapacity.Add(0);
                moduleComponentCount.Add(new ConcurrentDictionary<int, int>());
                moduleMaxCount.Add(new List<int> { 0, 0, 0, 0 });
                moduleComponentInProgress.Add(new ConcurrentDictionary<int, int>());
                moduleBuiltCount.Add(new List<int> { 0, 0, 0, 0 });
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
            RefreshDataCountDown = 120;
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
        public static void ConstructStarFortPoint(int starIndex, int rocketProtoId, int count = 1)
        {
            int index = rocketProtoId - 8037;
            index = Math.Min(Math.Max(0, index), 2);
            moduleComponentCount[starIndex].AddOrUpdate(index, 1, (x, y) => y + count);
            if (count == 1)
                moduleComponentInProgress[starIndex].AddOrUpdate(index, 0, (x, y) => Math.Max(0, y - 1));
        }

        // 游戏每帧调用，逐步刷新全星系的是否需要火箭
        public static void RecalcRocketNeed(int begin, int end)
        {
            end = Math.Min(end, GameMain.galaxy.starCount);
            if (end <= begin) return;
            if (begin >= GameMain.galaxy.starCount) return;

            for (int starIndex = begin; starIndex < end; starIndex++)
            {
                for (int i = 0; i < rocketsRequireMap[starIndex].Count; i++)
                {
                    rocketsRequireMap[starIndex][i] = moduleComponentCount[starIndex][i] + moduleComponentInProgress[starIndex][i] < moduleMaxCount[starIndex][i] * compoPerModule[i];
                }
            }
        }

        // 按星系储存可被选定为目标的敌军单位，每秒刷新，载入时刷新
        public static void RefreshEnemyPool()
        {
            enemyPoolByStarIndex.Clear();
            SpaceSector sector = GameMain.data.spaceSector;
            EnemyDFHiveSystem[] hives = sector.dfHivesByAstro;
            EnemyData[] oriPool = sector.enemyPool;
            for (int i = 0; i < sector.enemyCursor; i++)
            {
                ref EnemyData e = ref oriPool[i];
                if (e.id > 0 && e.unitId > 0)
                {
                    EnemyDFHiveSystem hive = hives[e.originAstroId - 1000000];
                    int starIndex = hive?.starData?.index ?? -1;
                    if (starIndex >= 0)
                    {
                        EnemyUnitComponent[] units = hive.units.buffer;
                        ref EnemyUnitComponent u = ref units[e.unitId];

                        if (true || u.behavior == EEnemyBehavior.ApproachTarget || u.behavior == EEnemyBehavior.OrbitTarget || u.behavior == EEnemyBehavior.Engage || u.behavior == EEnemyBehavior.SeekTarget)
                        {
                            if (enemyPoolByStarIndex.ContainsKey(starIndex))
                                enemyPoolByStarIndex[starIndex].Add(e.id);
                            else
                                enemyPoolByStarIndex.Add(starIndex, new List<int>() { e.id });
                        }
                    }
                }
            }
        }

        // 不被多线程调用，交互时可能调用，或每多少帧的时候调用
        public static void ReCalcData(ref DysonSphere sphere)
        {
            if (sphere == null) return;
            if (RefreshDataCountDown > 0) return;
            int starIndex = sphere.starData.index;
            moduleCapacity[starIndex] = (int)(sphere.energyGenCurrentTick_Layers / energyPerModule);
            if (moduleCapacity[starIndex] < 10) // 恒星要塞需要一个最小巨构能量水平才能开启
                moduleCapacity[starIndex] = 0;
            else
            {
                moduleCapacity[starIndex] += moduleBuiltCount[starIndex][2];
            }

            // 如果拆除戴森球壳面导致容量下降，需要执行对模块的拆除
            int overflow = -CapacityRemaining(starIndex);
            if (overflow > 0)
            {
                int destructCount = Math.Max(1, overflow / 100);
                float cannonRatio = moduleMaxCount[starIndex][1] * 1.0f / (moduleMaxCount[starIndex][0] + moduleMaxCount[starIndex][1]);
                int destructCannonModule = (int)(destructCount * cannonRatio);
                int destructMissileModule = destructCount - destructCannonModule;
                moduleMaxCount[starIndex][0] = Math.Max(0, moduleMaxCount[starIndex][0] - destructMissileModule);
                moduleMaxCount[starIndex][1] = Math.Max(0, moduleMaxCount[starIndex][1] - destructCannonModule);
            }

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
        public static void CalcModuleBuilt(int starIndex)
        {
            if (starIndex < 0 || starIndex >= moduleMaxCount.Count) return;
            for (int i = 0; i < 4; i++)
            {
                moduleBuiltCount[starIndex][i] = moduleComponentCount[starIndex][i] / compoPerModule[i];
            }
        }

        // 容量剩余
        public static int CapacityRemaining(int starIndex)
        {
            int sum = moduleMaxCount[starIndex][0] + moduleMaxCount[starIndex][1];
            return moduleCapacity[starIndex] - sum;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(long time)
        {
            return;

            if (RefreshDataCountDown > 0)
                RefreshDataCountDown -= 1;
            int starCount = GameMain.galaxy.starCount;
            int starsPerFrame = Math.Max(1, starCount / 60);
            int f = (int)(time % 60);
            int end = Math.Min((f + 1) * starsPerFrame, starCount);
            if (f == 59) end = starCount;
            for (int i = f * starsPerFrame; i < end; i++)
            {
                if (GameMain.data.dysonSpheres != null && i < GameMain.data.dysonSpheres.Length)
                {
                    CalcModuleBuilt(i);
                    DysonSphere sphere = GameMain.data.dysonSpheres[i];
                    ReCalcData(ref sphere);
                }
            }
            RecalcRocketNeed(f * starsPerFrame, end);
            if (time % 60 == 18)
                RefreshEnemyPool();

            if (UIStarFortress.curDysonSphere == null) return;
            if (time % 60 == 46)
            {
                UIStarFortress.RefreshAll();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSphere), "GameTick")]
        public static void StarFortressGameTick(ref DysonSphere __instance, long gameTick)
        {
            return;
            int starIndex = __instance.starData.index;

            int cannonModuleCount = moduleBuiltCount[starIndex][1];
            if (cannonChargeProgress >= 6000 && cannonModuleCount > 0) // 光矛开火 6000
            {
                FireCannon(ref __instance.swarm, 4);
                cannonChargeProgress %= 6000;
            }
            cannonChargeProgress += 1000.0 * (cannonModuleCount) / (299.0 + cannonModuleCount); // 充能速度有一个上限，就是1000/帧，也就是说发射速度有每秒10次的上限（因为充能满需要6000）

            // 发射导弹的速度暂定为：每个导弹模块提供1导弹/10s的发射速度
            int launchCheck = 60;
            int divisor = 100; // 这是由导弹模块的射速决定的
            //if (UIBattleStatistics.battleTime > 5400) divisor = 1000;
            if (gameTick % launchCheck == 0) // 最快也是每秒才会发射一次（发射数量为模块数的二十分之一），因此每秒可能不发射或发射多个导弹。// 已移除：如果战斗已超过90s，射速降低至1%
            {
                int launchCount = 0; // 计算后得到的发射数量
                int missileModuleCount = moduleBuiltCount[starIndex][0];
                if (missileModuleCount >= divisor)
                {
                    int over = missileModuleCount % divisor;
                    launchCount = missileModuleCount / divisor;
                    if (gameTick % (divisor * launchCheck) / 60 < over)
                        launchCount += 1;
                }
                else if (missileModuleCount > 0)
                {
                    if (gameTick % (divisor * launchCheck) / 60 < missileModuleCount) // 不能超过每秒发射一个的速度的时候，则每10s的前第整n秒发射一发
                    {
                        launchCount = 1;
                    }
                }
                System.Random rand = new System.Random();
                //launchCount = launchCount > 50 ? 50 : launchCount;
                for (int i = 0; i < launchCount; i++)
                {
                    DysonNode node = null;
                    int beginLayerIndex = rand.Next(1, 10);
                    int inc = randInc[rand.Next(0, randInc.Count)];
                    // 寻找第一个壳面
                    for (int layerIndex = (beginLayerIndex + inc + 10) % 10; layerIndex < 10; layerIndex = (layerIndex + inc + 10) % 10)
                    {
                        if (__instance.layersIdBased.Length > layerIndex && __instance.layersIdBased[layerIndex] != null && __instance.layersIdBased[layerIndex].nodeCount > 0)
                        {
                            DysonSphereLayer layer = __instance.layersIdBased[layerIndex];
                            bool found = false; // 寻找到可用的发射node之后，发射导弹，一直break到外面
                            int beginNodeIndex = rand.Next(0, Math.Max(1, layer.nodeCursor));
                            int nodeInc = layer.nodeCursor % inc == 0 ? 1 : inc + layer.nodeCursor;
                            nodeInc = nodeInc <= 0 ? 1 : nodeInc;
                            for (int nodeIndex = (beginNodeIndex + nodeInc) % layer.nodeCursor; nodeIndex < layer.nodeCursor && nodeIndex < layer.nodeCursor; nodeIndex = (nodeIndex + nodeInc) % layer.nodeCursor)
                            {
                                if (layer.nodePool[nodeIndex] != null)
                                {
                                    found = true;
                                    LauchMissile(starIndex, layer, layer.nodePool[nodeIndex]);
                                    break;
                                }
                                if (nodeIndex == beginNodeIndex) break;
                            }
                            if (found)
                                break;
                            for (int nodeIndex = 0; nodeIndex < beginNodeIndex; nodeIndex++)
                            {
                                if (layer.nodePool[nodeIndex] != null)
                                {
                                    found = true;
                                    LauchMissile(starIndex, layer, layer.nodePool[nodeIndex]);
                                    break;
                                }
                            }
                            if (found)
                                break;
                        }

                        if (layerIndex == beginLayerIndex) break;
                    }
                }
            }

        }

        /// <summary>
        /// 用于在战斗时屏蔽一些不重要的log
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
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

        public static void LauchMissile(int starIndex, DysonSphereLayer layer, DysonNode node)
        {
            if (node == null) return;

            if (enemyPoolByStarIndex.ContainsKey(starIndex) && enemyPoolByStarIndex[starIndex].Count > 0)
            {
                int total = enemyPoolByStarIndex[starIndex].Count;
                int begin = Utils.RandInt(0, total);
                int i = begin;
                SpaceSector sector = GameMain.data.spaceSector;

                while (true)
                {
                    int id = enemyPoolByStarIndex[starIndex][i];
                    if (id < sector.enemyCursor)
                    {
                        ref EnemyData e = ref sector.enemyPool[id];
                        if (e.id == id)
                        {
                            StarData star = layer.starData;
                            Vector3 nodeUPos = layer.NodeUPos(node);
                            Vector3 starUPos = star.uPosition;
                            SkillSystem skillSystem = sector.skillSystem;
                            ShootMissile(ref skillSystem, starIndex, ref e, 100000, nodeUPos, starUPos);
                            return;
                        }
                    }
                    i = (i + 1) % total;
                    if (i == begin) break;
                }


            }

        }

        public static void FireCannon(ref DysonSwarm swarm, int loop = 100)
        {
            int starIndex = swarm.starData.index;
            if (enemyPoolByStarIndex.ContainsKey(starIndex) && enemyPoolByStarIndex[starIndex].Count > 0)
            {
                int total = enemyPoolByStarIndex[starIndex].Count;
                int begin = Utils.RandInt(0, total);
                int i = begin;
                SpaceSector sector = GameMain.data.spaceSector;

                while (true)
                {
                    int id = enemyPoolByStarIndex[starIndex][i];
                    if (id < sector.enemyCursor)
                    {
                        ref EnemyData e = ref sector.enemyPool[id];
                        if (e.id == id)
                        {

                            ShootLaser(swarm.starData.uPosition, e.pos, e.id, 100000, 35);
                            return;
                        }
                    }
                    i = (i + 1) % total;
                    if (i == begin) break;
                }
            }
        }

        public static void ShootMissile(ref SkillSystem skillSystem, int starIndex, ref EnemyData enemy, int damage, Vector3 beginUPos, Vector3 starUPos)
        {
            ref GeneralMissile ptr = ref skillSystem.mechaMissiles.Add();
            //ref GeneralMissile ptr = ref this.skillSystem.mechaMissiles.Add();
            ptr.nearAstroId = GameMain.galaxy.stars[starIndex].astroId;
            ptr.caster.astroId = 0;
            ptr.casterVel = new Vector3(0, 0, 0);
            ItemProto activeAmmoProto = LDB.items.Select(1611);
            ptr.modelIndex = ((activeAmmoProto != null) ? activeAmmoProto.ModelIndex : 432);
            PrefabDesc prefabDesc = SpaceSector.PrefabDescByModelIndex[ptr.modelIndex];
            ptr.uPos = beginUPos;
            ptr.uRot = Quaternion.LookRotation(beginUPos - starUPos);
            ptr.uVel = (beginUPos - starUPos).normalized * 1000;
            ptr.moveAcc = 10 * prefabDesc.AmmoMoveAcc;
            ptr.turnAcc = prefabDesc.AmmoTurnAcc;
            ptr.damage = damage;
            ptr.mask = ETargetTypeMask.Enemy;
            ptr.life = 1;
            ptr.target.type = ETargetType.Enemy;
            ptr.target.id = enemy.id;
            ptr.target.astroId = enemy.originAstroId;
            ptr.caster.type = ETargetType.None;
            ptr.caster.id = 0;
            ref CombatStat combatStat = ref skillSystem.GetCombatStat(ptr.target);
            ptr.damageIncoming = skillSystem.CalculateDamageIncoming(ref ptr.target, ptr.damage, 1);
            combatStat.hpIncoming -= ptr.damageIncoming;
            ptr.targetCombatStatId = combatStat.id;

        }

        public static void RewriteMesh()
        {
            //GameMain.data.spaceSector.skillSystem.mechaMissileRenderer.instMesh = LDB.models.modelArray[75].prefabDesc.lodMeshes[0];
            //ref var _this = ref GameMain.data.spaceSector.skillSystem.mechaMissileRenderer;

            //Material[] materials = LDB.models.modelArray[75].prefabDesc.materials;
            //for (int i = 0; i < _this.instMats.Length; i++)
            //{
            //    int num = (materials != null) ? materials.Length : 0;
            //    _this.instMats[i] = ((i < num) ? UnityEngine.Object.Instantiate<Material>(materials[i]) : null);
            //}

            //_this.argArr = new uint[_this.instMats.Length * 5];
            //for (int j = 0; j < _this.instMats.Length; j++)
            //{
            //    _this.argArr[j * 5] = _this.instMesh.GetIndexCount(j);
            //    _this.argArr[1 + j * 5] = 0u;
            //    _this.argArr[2 + j * 5] = _this.instMesh.GetIndexStart(j);
            //    _this.argArr[3 + j * 5] = _this.instMesh.GetBaseVertex(j);
            //    _this.argArr[4 + j * 5] = 0u;
            //}
            //_this.argBuffer = new ComputeBuffer(_this.argArr.Length, 4, ComputeBufferType.DrawIndirect);
            //Utils.Log("rewrite mesh done");
        }



        public static void ShootLaser(VectorLF3 uBegin, VectorLF3 uEnd, int targetId, int damage, int life)
        {
            SpaceSector spaceSector = GameMain.data.spaceSector;
            if (targetId == 0)
                return;
            if (targetId >= spaceSector.enemyPool.Length || targetId <= 0)
                return;
            ref EnemyData ptr0 = ref spaceSector.enemyPool[targetId];
            if (ptr0.id <= 0)
                return;

            ref SpaceLaserOneShot ptr = ref spaceSector.skillSystem.lancerLaserOneShots.Add();
            ptr.astroId = 0;
            ptr.hitIndex = 32;
            ptr.target.type = ETargetType.Enemy;
            ptr.target.id = ptr0.id;
            ptr.target.astroId = ptr0.originAstroId;
            ptr.caster.type = ETargetType.None;
            ptr.caster.id = 0;
            ptr.beginPosU = uBegin;
            ptr.endPosU = uEnd;
            ptr.endVelU = Vector3.zero;
            ptr.muzzleOffset = Vector3.zero;
            ptr.damage = damage;
            ptr.life = life;
            ptr.extendedDistWhenMiss = 0;
            ptr.mask = ETargetTypeMask.Enemy;
        }


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

            StarFortressSilo.Export(w);
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
                        moduleComponentCount[i].AddOrUpdate(key, value, (x, y) => value); // 这里不用tryadd是因为InitAll里面对每个key（只有0123）都进行过了add
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
                    }
                }
                remindPlayerWhenDestruction = r.ReadInt32();
            }

            StarFortressSilo.Import(r);
            RefreshEnemyPool();
            RewriteMesh();
        }

        public static void IntoOtherSave()
        {
            InitAll();

            StarFortressSilo.IntoOtherSave();
            RefreshEnemyPool();
        }
    }
}
