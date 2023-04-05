using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace DSP_Battle
{
    class Cannon
    {
        //需要存读档
        // 这里应该是HashSet，为了线程安全还是用ConcurrentDictionary
        public static List<ConcurrentDictionary<int, int>> bulletTargets; //记录子弹的目标
        public static List<ConcurrentDictionary<int, int>> bulletIds; //记录子弹Id，原本是canDoDamage记录子弹还能造成多少伤害
        //

        public static bool doTrack = true;
        public static System.Random rand = new System.Random();
        public static int indexBegins = 0; // 寻敌遍历时开始的index，每帧寻敌最多遍历3次，每次重新排序则置0

        public static ConcurrentDictionary<int, ConcurrentDictionary<int, int>> cannonTargets = new ConcurrentDictionary<int, ConcurrentDictionary<int, int>>(); //cannonTargets[planetId][entityId] = 目标的shipIndex

        /// <summary>
        /// 每帧调用刷新子弹终点
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void BulletTrack()
        {
            if (!doTrack) return;
            try
            {
                for (int i = 0; i < GameMain.data.dysonSpheres.Length; i++)
                {
                    DysonSphere sphere = GameMain.data.dysonSpheres[i];
                    if (sphere == null)
                    {
                        continue;
                    }
                    DysonSwarm swarm = GameMain.data.dysonSpheres[i].swarm;
                    int starIndex = GameMain.data.dysonSpheres[i].starData.index;
                    if (swarm != null)
                    {
                        if (bulletTargets[starIndex] == null)
                        {
                            continue;
                        }
                        foreach (var j in bulletTargets[starIndex].Keys)
                        {
                            int targetShipIndex = bulletTargets[starIndex][j];
                            if (EnemyShips.ships.ContainsKey(targetShipIndex) && EnemyShips.ships[targetShipIndex].state == EnemyShip.State.active)
                            {
                                swarm.bulletPool[j].uEnd = EnemyShips.ships[targetShipIndex].uPos;
                            }
                        }
                    }

                }

            }
            catch (Exception)
            {
            }
        }

        public static void ReInitAll()
        {
            try
            {
                bulletTargets = new List<ConcurrentDictionary<int, int>>();
                bulletIds = new List<ConcurrentDictionary<int, int>>();
                cannonTargets = new ConcurrentDictionary<int, ConcurrentDictionary<int, int>>();

                for (int i = 0; i < GameMain.galaxy.starCount; i++)
                {
                    bulletTargets.Add(new ConcurrentDictionary<int, int>());
                    bulletIds.Add(new ConcurrentDictionary<int, int>());
                }

            }
            catch (Exception)
            {
                DspBattlePlugin.logger.LogWarning("Cannon ReInit ERROR");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static bool EjectorPatch(ref EjectorComponent __instance, float power, DysonSwarm swarm, AstroData[] astroPoses, AnimData[] animPool, int[] consumeRegister, ref uint __result)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;

            bool cannon = isCannon(gmProtoId);

            if (!cannon) return true; // 普通弹射器直接执行原函数

            //子弹需求循环
            if (__instance.bulletCount == 0 && gmProtoId != 8014 && GameMain.instance.timei % 60 == 0)
            {
                __instance.bulletId = nextBulletId(__instance.bulletId);
            }
            else if (gmProtoId == 8014)
            {
                __instance.bulletId = 8007;
                __instance.bulletCount = 0;
            }

            if (__instance.needs == null)
            {
                __instance.needs = new int[6];
            }
            __instance.needs[0] = ((__instance.bulletCount >= 20) ? 0 : __instance.bulletId);
            animPool[__instance.entityId].prepare_length = __instance.localDir.x;
            animPool[__instance.entityId].working_length = __instance.localDir.y;
            animPool[__instance.entityId].power = __instance.localDir.z;
            __instance.targetState = EjectorComponent.ETargetState.None;
            if (__instance.fired)
            {
                int num = __instance.entityId;
                animPool[num].time = animPool[num].time + 0.016666668f;
                if (animPool[__instance.entityId].time >= 11f)
                {
                    __instance.fired = false;
                    animPool[__instance.entityId].time = 0f;
                }
            }
            else if (__instance.direction > 0)
            {
                animPool[__instance.entityId].time = (float)__instance.time / (float)__instance.chargeSpend;
            }
            else if (__instance.direction < 0)
            {
                animPool[__instance.entityId].time = -(float)__instance.time / (float)__instance.coldSpend;
            }
            else
            {
                animPool[__instance.entityId].time = 0f;
            }

            if (Configs.nextWaveState == 1 || Configs.nextWaveState == 2) return false; // 非战斗下就不索敌，且不执行原本的函数

            //下面是因为 炮需要用orbitId记录索敌模式，而orbitId有可能超出已设定的轨道数，为了避免溢出，炮的orbitalId在参与计算时需要独立指定为1。
            //后续所有的__instance.orbitId都被替换为此
            if (__instance.orbitId <= 0 || __instance.orbitId > 4)
            {
                __instance.orbitId = 1;
            }

            if (power < 0.1f)
            {
                if (EjectorUIPatch.needToRefreshTarget) //如果需要刷新目标，又没电。必须告诉UI面板没有目标，而不能维持前一个选择的弹射器的状况
                {
                    if (EjectorUIPatch.curEjectorPlanetId == __instance.planetId && EjectorUIPatch.curEjectorEntityId == __instance.entityId)
                    {
                        EjectorUIPatch.curTarget = null;
                    }
                }

                if (__instance.direction == 1)
                {
                    __instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
                    __instance.direction = -1;
                }
                __result = 0U;
                return false;
            }
            __result = CannonFire(ref __instance, power, swarm, astroPoses, animPool, consumeRegister, gmProtoId);
            return false;
        }

        private static uint CannonFire(ref EjectorComponent __instance, float power, DysonSwarm swarm, AstroData[] astroPoses, AnimData[] animPool, int[] consumeRegister, int gmProtoId)
        {
            int planetId = __instance.planetId;
            int entityId = __instance.entityId;
            int starIndex = planetId / 100 - 1;
            int calcOrbitId = __instance.orbitId;
            uint result = 0;

            float num2 = (float)Cargo.incTableMilli[__instance.incLevel];
            int num3 = (int)(power * 10000f * (1f + num2) + 0.1f);

            bool relic0_6Activated = Relic.HaveRelic(0, 6) && __instance.bulletId == 8001; // relic0-6 京级巨炮如果激活并且当前子弹确实是穿甲磁轨弹
            if (relic0_6Activated) num3 = (int)(num3 * 0.1);

            __instance.targetState = EjectorComponent.ETargetState.OK;
            bool flag = true;
            int num4 = __instance.planetId / 100 * 100;
            float num5 = __instance.localAlt + __instance.pivotY + (__instance.muzzleY - __instance.pivotY) / Mathf.Max(0.1f, Mathf.Sqrt(1f - __instance.localDir.y * __instance.localDir.y));
            Vector3 vector = new Vector3(__instance.localPosN.x * num5, __instance.localPosN.y * num5, __instance.localPosN.z * num5);
            VectorLF3 vectorLF = astroPoses[__instance.planetId].uPos + Maths.QRotateLF(astroPoses[__instance.planetId].uRot, vector);
            Quaternion q = astroPoses[__instance.planetId].uRot * __instance.localRot;
            VectorLF3 uPos = astroPoses[num4].uPos;
            VectorLF3 b = uPos - vectorLF;

            List<EnemyShip> sortedShips = EnemyShips.sortedShips(calcOrbitId, starIndex, __instance.planetId);

            //下面的参数根据是否是炮还是太阳帆的弹射器有不同的修改
            double maxtDivisor = 5000.0; //决定子弹速度
            int damage = 0;
            int loopNum = sortedShips.Count;
            double cannonSpeedScale = 1;
            if (gmProtoId == 8012)
                cannonSpeedScale = 2;
            EnemyShip curTarget = null;

            if (__instance.bulletId == 8001)
            {
                maxtDivisor = relic0_6Activated ? Configs.bullet4Speed : Configs.bullet1Speed * cannonSpeedScale; // relic0-6京级巨炮 还会大大加速此子弹速度
                damage = (int)Configs.bullet1Atk; //只有这个子弹能够因为引力弹射器而强化伤害。这个强化是不是取消了？
                //if (relic0_6Activated) // relic0-6 京级巨炮效果，由于这个伤害只在统计中计算为发射伤害，实际造成上海市还要再重新计算，因此这里不计算了，统计中记为发射了基础的伤害
                //    damage = Relic.BonusDamage(damage, 500); 
            }
            else if (__instance.bulletId == 8002)
            {
                maxtDivisor = Configs.bullet2Speed * cannonSpeedScale;
                damage = Configs.bullet2Atk;
            }
            else if (__instance.bulletId == 8003)
            {
                maxtDivisor = Configs.bullet3Speed * cannonSpeedScale;
                damage = Configs.bullet3Atk;
            }
            else if (__instance.bulletId == 8007)
            {
                maxtDivisor = Configs.bullet4Speed; //没有速度加成
                damage = Configs.bullet4Atk;
            }

            //不该参与循环的部分，换到循环前了

            bool flag2 = __instance.bulletCount > 0;
            if (gmProtoId == 8014) //脉冲炮不需要子弹
                flag2 = true;
            VectorLF3 vectorLF2 = VectorLF3.zero;

            int begins = indexBegins;
            if (begins >= loopNum)
            {
                Interlocked.Exchange(ref indexBegins, 0);
                begins = 0;
            }

            bool needFindNewTarget = true;
            EnemyShip lastTargetShip = null;
            if (cannonTargets.ContainsKey(planetId))
            {
                if (cannonTargets[planetId].ContainsKey(entityId))
                {
                    int lastTargetShipIndex = cannonTargets[planetId][entityId];
                    if (EnemyShips.ships.ContainsKey(lastTargetShipIndex) && EnemyShips.ships[lastTargetShipIndex].state == EnemyShip.State.active)
                    {
                        lastTargetShip = EnemyShips.ships[lastTargetShipIndex];
                        needFindNewTarget = false; // 老目标存在的话，在下面的循环中首先判断老目标是否合法（不被阻挡、俯仰角合适等）
                    }
                }
            }
            for (int gm = begins; gm < loopNum && gm < begins+3; gm++)
            {
                if (!needFindNewTarget && gm > begins) // 说明上一个循环判定了原目标，且原目标无法作为合法目标，因此重新开始判定目标
                {
                    needFindNewTarget = true;
                    gm = begins;
                }

                //新增的，每次循环开始必须重置
                __instance.targetState = EjectorComponent.ETargetState.OK;
                flag = true;
                flag2 = __instance.bulletCount > 0;
                if (gmProtoId == 8014) //脉冲炮不需要子弹
                    flag2 = true;
                else if (relic0_6Activated) // relic0-6 京级巨炮效果 每次消耗五发弹药
                    flag2 = __instance.bulletCount > 4;

                int shipIdx = 0;//ship总表中的唯一标识：index
                EnemyShip targetShip = sortedShips[gm];
                if (!needFindNewTarget) // 如果原本的目标存在，则先判断原本的目标，此时在这个循环中，gm=begins的ship并没有真的被计算俯仰角等合法性判断，因此假若原本的目标失效，进入了下个循环后要根据情况重置gm=begins（见循环节开头）
                {
                    targetShip = lastTargetShip;
                }
                vectorLF2 = targetShip.uPos;
                shipIdx = targetShip.shipIndex;
                
                if(needFindNewTarget && (!EnemyShips.ships.ContainsKey(shipIdx) || targetShip.state != EnemyShip.State.active)) continue;


                VectorLF3 vectorLF3 = vectorLF2 - vectorLF;
                __instance.targetDist = vectorLF3.magnitude;
                vectorLF3.x /= __instance.targetDist;
                vectorLF3.y /= __instance.targetDist;
                vectorLF3.z /= __instance.targetDist;
                Vector3 vector2 = Maths.QInvRotate(q, vectorLF3);
                __instance.localDir.x = __instance.localDir.x * 0.9f + vector2.x * 0.1f;
                __instance.localDir.y = __instance.localDir.y * 0.9f + vector2.y * 0.1f;
                __instance.localDir.z = __instance.localDir.z * 0.9f + vector2.z * 0.1f;
                if ((double)vector2.y < 0.08715574 || vector2.y > 0.8660254f)
                {
                    __instance.targetState = EjectorComponent.ETargetState.AngleLimit;
                    flag = false;
                }
                if (flag2 && flag)
                {
                    for (int i = num4 + 1; i <= __instance.planetId + 2; i++)
                    {
                        if (i != __instance.planetId)
                        {
                            double num6 = (double)astroPoses[i].uRadius;
                            if (num6 > 1.0)
                            {
                                VectorLF3 vectorLF4 = astroPoses[i].uPos - vectorLF;
                                double num7 = vectorLF4.x * vectorLF4.x + vectorLF4.y * vectorLF4.y + vectorLF4.z * vectorLF4.z;
                                double num8 = vectorLF4.x * vectorLF3.x + vectorLF4.y * vectorLF3.y + vectorLF4.z * vectorLF3.z;
                                if (num8 > 0.0)
                                {
                                    double num9 = num7 - num8 * num8;
                                    num6 += 120.0;
                                    if (num9 < num6 * num6)
                                    {
                                        flag = false;
                                        __instance.targetState = EjectorComponent.ETargetState.Blocked;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (EnemyShips.ships.ContainsKey(shipIdx) && EnemyShips.ships[shipIdx].state == EnemyShip.State.active && __instance.targetState != EjectorComponent.ETargetState.Blocked && __instance.targetState != EjectorComponent.ETargetState.AngleLimit)
                {
                    curTarget = EnemyShips.ships[shipIdx]; //设定目标
                    if (!cannonTargets.ContainsKey(planetId))
                    {
                        cannonTargets.TryAdd(planetId, new ConcurrentDictionary<int, int>());
                        cannonTargets[planetId].TryAdd(entityId, shipIdx);
                    }
                    else
                    {
                        cannonTargets[planetId].AddOrUpdate(entityId, shipIdx, (x, y) => shipIdx);
                    }
                    if (EjectorUIPatch.needToRefreshTarget) //如果需要刷新目标
                    {
                        if (EjectorUIPatch.curEjectorPlanetId == __instance.planetId && EjectorUIPatch.curEjectorEntityId == __instance.entityId)
                        {
                            EjectorUIPatch.curTarget = curTarget;
                        }
                    }
                    break;
                }
            }
            //如果没有船/船没血了，就不打炮了
            if (curTarget == null)
            {
                Interlocked.Add(ref indexBegins, 3);
                flag = false; //本身是由于俯仰限制或路径被阻挡的判断，现在找不到目标而不打炮也算做里面
            }
            else if (curTarget != null && curTarget.hp <= 0)
            {
                flag = false;
            }
            else if (curTarget.state != EnemyShip.State.active)
            {
                flag = false;
            }

            bool flag3 = flag && flag2;
            result = (flag2 ? (flag ? 4U : 3U) : 2U);
            if (__instance.direction == 1)
            {
                if (!flag3)
                {
                    __instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
                    __instance.direction = -1;
                }
            }
            else if (__instance.direction == 0 && flag3)
            {
                __instance.direction = 1;
            }


            if (__instance.direction == 1)
            {
                __instance.time += num3;
                if (__instance.time >= __instance.chargeSpend)
                {
                    __instance.fired = true;
                    animPool[__instance.entityId].time = 10f;
                    VectorLF3 uBeginChange = vectorLF;
                    int bulletCost = relic0_6Activated ? 5 : 1;

                    int bulletIndex = -1;

                    if (gmProtoId != 8014 || GameMain.instance.timei % 5 == 1) // 相位炮五帧才发一个，但是伤害x5
                    {
                        //下面是添加子弹
                        bulletIndex = swarm.AddBullet(new SailBullet
                        {
                            maxt = (float)(__instance.targetDist / maxtDivisor),
                            lBegin = vector,
                            uEndVel = VectorLF3.Cross(vectorLF2 - uPos, swarm.orbits[calcOrbitId].up).normalized * Math.Sqrt((double)(swarm.dysonSphere.gravity / swarm.orbits[calcOrbitId].radius)), //至少影响着形成的太阳帆的初速度方向
                            uBegin = uBeginChange,
                            uEnd = vectorLF2
                        }, calcOrbitId);
                    }

                    //设定子弹目标以及伤害，并注册伤害
                    try
                    {
                        if (bulletIndex != -1)
                            swarm.bulletPool[bulletIndex].state = 0; //设置成0，该子弹将不会生成太阳帆
                    }
                    catch (Exception)
                    {
                        DspBattlePlugin.logger.LogInfo("bullet info1 set error.");
                    }
                    UIBattleStatistics.RegisterShootOrLaunch(__instance.bulletId, damage, bulletCost);

                    if (bulletIndex != -1)
                        bulletTargets[swarm.starData.index].AddOrUpdate(bulletIndex, curTarget.shipIndex, (x, y) => curTarget.shipIndex);

                    //Main.logger.LogInfo("bullet info2 set error.");


                    try
                    {
                        int bulletId = __instance.bulletId;
                        if (bulletIndex != -1)
                            bulletIds[swarm.starData.index].AddOrUpdate(bulletIndex, bulletId, (x, y) => bulletId);
                        // bulletIds[swarm.starData.index][bulletIndex] = 1;//后续可以根据子弹类型/炮类型设定不同数值
                    }
                    catch (Exception)
                    {
                        DspBattlePlugin.logger.LogInfo("bullet info3 set error.");
                    }
                    if (__instance.bulletCount != 0)
                    {
                        __instance.bulletInc -= bulletCost * __instance.bulletInc / __instance.bulletCount;
                    }
                    __instance.bulletCount -= bulletCost;
                    if(gmProtoId==8012 && Relic.HaveRelic(2,3) && Relic.Verify(0.75)) // relic2-3 回声 概率回填弹药
                        __instance.bulletCount += 1;
                    if (__instance.bulletCount <= 0)
                    {
                        __instance.bulletInc = 0;
                        __instance.bulletCount = 0;
                    }
                    lock (consumeRegister)
                    {
                        consumeRegister[__instance.bulletId] += bulletCost;
                    }
                    __instance.time = __instance.coldSpend;
                    __instance.direction = -1;

                    //if (gmProtoId == 8014) //激光炮为了视觉效果，取消冷却阶段每帧都发射（不能简单地将charge和cold的spend设置为0，因为会出现除以0的错误）
                    //    __instance.direction = 1;

                }
            }

            else if (__instance.direction == -1)
            {

                __instance.time -= num3;
                if (__instance.time <= 0)
                {
                    __instance.time = 0;
                    __instance.direction = (flag3 ? 1 : 0);
                }
            }
            else
            {
                __instance.time = 0;

            }

            return result;
        }

        private static bool isCannon(int protoId)
        {
            return protoId != 2311;
        }

        private static int nextBulletId(int id)
        {
            return (id - 8000) % 3 + 8001;
        }

        /// <summary>
        /// 这个函数用于从GameTick手中截胡攻击用的子弹，不让它创建太阳帆（而非以往的创建太阳帆后删除），这样可以保证正常的太阳帆发射行为不受干扰，但是，缺点是会提前一帧触发某些效果，但我觉得一帧无所谓。
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSwarm), "GameTick")]
        public static void EarlyCalcBulletState(ref DysonSwarm __instance)
        {
            if (__instance.dysonSphere.layerCount < 0) return; //专门用于渲染的swarm设置成layerCount=-1，不受这个patch影响
            int starIndex = __instance.starData.index;

            // relic0-6 京级巨炮效果
            int bullet1DamageWithRelic = Relic.HaveRelic(0, 6) ? Relic.BonusDamage(Configs.bullet1Atk, 200) : Configs.bullet1Atk;
            int bullet1Count = Relic.HaveRelic(0, 6) ? 5 : 1; // 实际消耗过的穿甲弹数量

            foreach (var i in bulletTargets[starIndex].Keys)
            {
                if (__instance.bulletPool.Length > i && __instance.bulletPool[i].id == i) //后面的判断条件就是说只对攻击用的子弹生效，不对正常的太阳帆操作
                {
                    //SailBullet[] array = __instance.bulletPool;
                    //int num3 = i;
                    //此处原本函数有array[num3].t = array[num3].t + num;其中num是1/60，但是这是postfix，已经执行过t增加了，所以不能再加一次，否则会使子弹变成二倍速

                    //为什么下面的if注释掉了呢？我发现在创建子弹时就设置成state=0貌似也没什么影响。。那既然state=0不会创建太阳帆，那就一开始设置吧，不要每帧检测了。
                    //好吧我又加回来了，因为读档后会出现莫名其妙的太阳帆，有了这个就不出来了
                    if (__instance.bulletPool[i].t >= __instance.bulletPool[i].maxt - 0.02f) //
                    {
                        __instance.bulletPool[i].state = 0; //这就阻止了后续创建太阳帆的可能，但也可能带来其他影响，但无所谓，只有最多2帧的异常帧

                    }
                    if (__instance.bulletPool[i].t >= __instance.bulletPool[i].maxt && bulletTargets[starIndex].ContainsKey(i) && bulletIds[starIndex].ContainsKey(i))
                    {
                        if (EnemyShips.ships.ContainsKey(bulletTargets[starIndex][i]) && bulletIds[starIndex][i] != 0)
                        {
                            int damage = 0;
                            int bulletId = bulletIds[starIndex][i];
                            int bulletCount = 1;
                            DamageType dmgType = DamageType.bullet;
                            switch (bulletId)
                            {
                                case 8001: damage = bullet1DamageWithRelic; bulletCount = bullet1Count; break;
                                case 8002: damage = Configs.bullet2Atk; break;
                                case 8003: damage = Configs.bullet3Atk; break;
                                case 8007: damage = (int)(Configs.bullet4Atk * 5 * Math.Max(0.2,(1 - Configs.laserDamageReducePerAU * __instance.bulletPool[i].maxt * Configs.bullet4Speed / 40000))); dmgType = DamageType.laser; break;
                                case 8009: damage = 5000; dmgType = DamageType.laser; break;
                                default:
                                    break;
                            }

                            int realDamage = EnemyShips.ships[bulletTargets[starIndex][i]].BeAttacked(damage, dmgType); //击中造成伤害  //如果在RemoveBullet的postpatch写这个，可以不用每帧循环检测，但是伤害将在爆炸动画后结算，感觉不太合理
                            if(realDamage > 0) // 被闪避了则不算击中
                                UIBattleStatistics.RegisterHit(bulletId, realDamage, bulletCount);
                            if (Relic.HaveRelic(3, 7)) // relic3-7 虚空折射 子弹命中时对一个随机敌人造成20%额外伤害
                            {
                                int refDmg = Relic.BonusDamage(damage, 0.2) - damage;
                                int randNum = -1;
                                if (EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex].Count > 0)
                                    randNum = Utils.RandInt(0, EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex].Count);
                                if (randNum >= 0 && EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex][randNum] != null && EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex][randNum].state == EnemyShip.State.active)
                                {
                                    int realRefDmg = EnemyShips.minTargetDisSortedShips[Configs.nextWaveStarIndex][randNum].BeAttacked(refDmg, dmgType);
                                    UIBattleStatistics.RegisterHit(bulletId, realRefDmg, 0);
                                }
                            }
                            if (bulletId == 8007)
                            {
                                for (int j = 1; j <= 4; ++j) UIBattleStatistics.RegisterHit(bulletId, 0);
                            }
                        }
                        int v;
                        bulletIds[starIndex].TryRemove(i, out v); //该子弹已造成过伤害，或者因为飞船已经不存在了，这两种情况都要将子弹的未来还可造成的伤害设置成0
                    }
                }

            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSwarm), "RemoveBullet")]
        public static void RemoveBulletThenRemoveSailMark(DysonSwarm __instance, int id)
        {
            if (__instance.dysonSphere.layerCount < 0) return; //专门用于渲染的swarm设置成layerCount=-1，不受这个patch影响
            if (bulletTargets[__instance.starData.index].ContainsKey(id))
            {
                int v;
                bulletTargets[__instance.starData.index].TryRemove(id, out v);
            }

        }


        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(PlanetFactory), "UpgradeEntityWithComponents")]
        //public static void UpgradePostPatch()
        //{
        //}

        public static void Export(BinaryWriter w)
        {
            w.Write(bulletTargets.Count);
            for (int i2 = 0; i2 < bulletTargets.Count; i2++)
            {
                w.Write(bulletTargets[i2].Count);
                foreach (var item in bulletTargets[i2])
                {
                    w.Write(item.Key);
                    w.Write(item.Value);
                }
            }
            w.Write(bulletIds.Count);
            for (int i3 = 0; i3 < bulletIds.Count; i3++)
            {
                w.Write(bulletIds[i3].Count);
                foreach (var item in bulletIds[i3])
                {
                    w.Write(item.Key);
                    w.Write(item.Value);
                }
            }

        }

        public static void Import(BinaryReader r)
        {
            ReInitAll();
            if (Configs.versionWhenImporting < 30221024)
            {
                int total1 = r.ReadInt32();
                for (int i1 = 0; i1 < total1; i1++)
                {
                    int num1 = r.ReadInt32();
                    for (int j1 = 0; j1 < num1; j1++)
                    {
                        r.ReadInt32();
                    }
                }
            }

            int total2 = r.ReadInt32();
            for (int c2 = 0; c2 < total2 - bulletTargets.Count; c2++)
            {
                bulletTargets.Add(new ConcurrentDictionary<int, int>());
            }
            for (int i2 = 0; i2 < total2; i2++)
            {
                int num2 = r.ReadInt32();
                for (int j2 = 0; j2 < num2; j2++)
                {
                    bulletTargets[i2].TryAdd(r.ReadInt32(), r.ReadInt32());
                }
            }

            int total3 = r.ReadInt32();
            for (int c3 = 0; c3 < total3 - bulletIds.Count; c3++)
            {
                bulletIds.Add(new ConcurrentDictionary<int, int>());
            }
            for (int i3 = 0; i3 < total3; i3++)
            {
                int num3 = r.ReadInt32();
                for (int j3 = 0; j3 < num3; j3++)
                {
                    bulletIds[i3].TryAdd(r.ReadInt32(), r.ReadInt32());
                }
            }
        }

        public static void IntoOtherSave()
        {
            ReInitAll();
        }
    }


}
