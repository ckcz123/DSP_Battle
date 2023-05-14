using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    class MissileSilo
    {
        public static List<ConcurrentDictionary<int, int>> MissileTargets; //记录导弹的目标
        public static List<ConcurrentDictionary<int, int>> missileProtoIds; //记录导弹还能造成多少伤害


        private static MethodInfo methodInfo;

        //以下数值尽量不要改动
        //public static double distIntoTrackStage2;

        public static void ReInitAll()
        {
            MissileTargets = new List<ConcurrentDictionary<int, int>>();
            missileProtoIds = new List<ConcurrentDictionary<int, int>>();
            for (int i = 0; i < GameMain.galaxy.starCount; i++)
            {
                MissileTargets.Add(new ConcurrentDictionary<int, int>());
                missileProtoIds.Add(new ConcurrentDictionary<int, int>());
            }
            methodInfo = null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SiloComponent), "InternalUpdate")]
        public static bool SiloPatch(ref SiloComponent __instance, float power, DysonSphere sphere, AnimData[] animPool, int[] consumeRegister, ref uint __result)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;

            if(gmProtoId == 2312 && MoreMegaStructure.MoreMegaStructure.StarMegaStructureType[starIndex] == 6 && StarCannon.fireStage>0) //如果恒星炮正在开火，停止发射火箭
            {
                if (__instance.needs != null && __instance.needs.Length > 0)
                {
                    __instance.needs[0] = ((__instance.bulletCount >= 20) ? 0 : __instance.bulletId);
                }
                __result = 0;
                return false;
            }

            if (gmProtoId == 2312) // 恒星要塞组件使用原本的发射井发射
            {
                if (GameMain.instance.timei % 60 == 1 && __instance.bulletCount <= 0)
                {
                    __instance.bulletId = StarFortressSilo.nextBulletId(starIndex, __instance.bulletId);
                }
                if (__instance.bulletId < 8037 || __instance.bulletId > 8039)
                    return true; //原始发射井返回原函数
                else
                    return StarFortressSilo.SiloSubPatch(ref __instance, power, sphere, animPool, consumeRegister, ref __result);
            }
            if (gmProtoId == 8036) return StarFortressSilo.SiloSubPatch(ref __instance, power, sphere, animPool, consumeRegister, ref __result);

            if (Configs.developerMode)
            {
                __instance.bulletId = 8006;
                __instance.bulletCount = 99;
            }
            if (GameMain.instance.timei % 60 == 0 && __instance.bulletCount == 0)
            {
                __instance.bulletId = nextBulletId(__instance.bulletId);
            }

            if (__instance.needs == null)
            {
                __instance.needs = new int[6];
            }
            __instance.needs[0] = ((__instance.bulletCount >= 20) ? 0 : __instance.bulletId);
            if (__instance.fired && __instance.direction != -1)
            {
                __instance.fired = false;
            }
            if (__instance.direction == 1)
            {
                animPool[__instance.entityId].time = (float)__instance.time / (float)__instance.chargeSpend;
            }
            else if (__instance.direction == -1)
            {
                animPool[__instance.entityId].time = -(float)__instance.time / (float)__instance.coldSpend;
            }
            animPool[__instance.entityId].power = power;
            float num = (float)Cargo.accTableMilli[__instance.incLevel];
            int num2 = (int)(power * 10000f * (1f + num) + 0.1f);
            Mutex dysonSphere_mx = sphere.dysonSphere_mx;
            uint result;
            lock (dysonSphere_mx)
            {
                //下面设定目标，发射时是选择最近目标；如果目标丢失则再随机选择目标
                int targetIndex = 0;
                if (Configs.nextWaveState == 3)
                {
                    targetIndex = FindTarget(starIndex, planetId);
                }

                __instance.hasNode = (sphere.GetAutoNodeCount() > 0);
                if (targetIndex <= 0)  //if (!__instance.hasNode) 原本是没有节点，因此不发射
                {
                    __instance.autoIndex = 0;
                    if (__instance.direction == 1)
                    {
                        __instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
                        __instance.direction = -1;
                    }
                    if (__instance.direction == -1)
                    {
                        __instance.time -= num2;
                        if (__instance.time <= 0)
                        {
                            __instance.time = 0;
                            __instance.direction = 0;
                        }
                    }
                    if (power >= 0.1f)
                    {
                        result = 1U;
                    }
                    else
                    {
                        result = 0U;
                    }
                }
                else if (power < 0.1f)
                {
                    if (__instance.direction == 1)
                    {
                        __instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
                        __instance.direction = -1;
                    }
                    result = 0U;
                }
                else
                {
                    uint num3 = 0U;
                    bool flag2;
                    num3 = ((flag2 = (__instance.bulletCount > 0)) ? 3U : 2U);
                    if (__instance.direction == 1)
                    {
                        if (!flag2)
                        {
                            __instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
                            __instance.direction = -1;
                        }
                    }
                    else if (__instance.direction == 0 && flag2)
                    {
                        __instance.direction = 1;
                    }
                    if (__instance.direction == 1)
                    {
                        __instance.time += num2;
                        if (__instance.time >= __instance.chargeSpend)
                        {
                            AstroData[] astroPoses = sphere.starData.galaxy.astrosData;
                            __instance.fired = true;
                            //DysonNode autoDysonNode = sphere.GetAutoDysonNode(__instance.autoIndex + __instance.id); //原本获取目标节点，现在已不需要
                            DysonRocket dysonRocket = default(DysonRocket);
                            dysonRocket.planetId = __instance.planetId;
                            dysonRocket.uPos = astroPoses[__instance.planetId].uPos + Maths.QRotateLF(astroPoses[__instance.planetId].uRot, __instance.localPos + __instance.localPos.normalized * 6.1f);
                            dysonRocket.uRot = astroPoses[__instance.planetId].uRot * __instance.localRot * Quaternion.Euler(-90f, 0f, 0f);
                            dysonRocket.uVel = dysonRocket.uRot * Vector3.forward;
                            dysonRocket.uSpeed = 0f;
                            dysonRocket.launch = __instance.localPos.normalized;
                            //sphere.AddDysonRocket(dysonRocket, autoDysonNode); //原本
                            int rocketIndex = AddDysonRockedGniMaerd(ref sphere, ref dysonRocket, null); //这是添加了一个目标戴森球节点为null的火箭，因此被判定为导弹

                            MissileTargets[starIndex][rocketIndex] = targetIndex;
                            missileProtoIds[starIndex][rocketIndex] = __instance.bulletId;
                            int damage = 0;
                            if (__instance.bulletId == 8004) damage = Configs.missile1Atk;
                            else if (__instance.bulletId == 8005) damage = Configs.missile2Atk;
                            else if (__instance.bulletId == 8006) damage = Configs.missile3Atk;
                            //注册导弹
                            UIBattleStatistics.RegisterShootOrLaunch(__instance.bulletId, damage);

                            __instance.autoIndex++;
                            __instance.bulletInc -= __instance.bulletInc / __instance.bulletCount;
                            if(!Relic.HaveRelic(1,5))
                                __instance.bulletCount--;
                            if (__instance.bulletCount == 0)
                            {
                                __instance.bulletInc = 0;
                            }
                            lock (consumeRegister)
                            {
                                consumeRegister[__instance.bulletId]++;
                            }
                            __instance.time = __instance.coldSpend;
                            __instance.direction = -1;
                        }
                    }
                    else if (__instance.direction == -1)
                    {
                        __instance.time -= num2;
                        if (__instance.time <= 0)
                        {
                            __instance.time = 0;
                            __instance.direction = (flag2 ? 1 : 0);
                        }
                    }
                    else
                    {
                        __instance.time = 0;
                    }
                    result = num3;
                }
            }
            __result = result;
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSphere), "RocketGameTick", new Type[] { })]
        public static bool RocketGameTickNoThreadPatch(ref DysonSphere __instance)
        {
            AstroData[] astroPoses = __instance.starData.galaxy.astrosData;
            double num = 0.016666666666666666;
            float num2 = Mathf.Max(1f, (float)Math.Pow((double)__instance.defOrbitRadius / 40000.0 * 4.0, 0.4));
            float num3 = 7.5f;
            float num4 = 18f * num2;
            float num5 = 2800f * num2;
            bool relic1_1Activated = Relic.HaveRelic(1, 1); // relic 是否拥有
            bool relic1_11Activated = Relic.HaveRelic(1, 11);
            bool relic2_7Activated = Relic.HaveRelic(2, 7);
            bool relic2_15Activated = Relic.HaveRelic(2, 15);
            for (int i = 1; i < __instance.rocketCursor; i++)
            {
                if (__instance.rocketPool[i].id == i)
                {
                    DysonRocket dysonRocket = __instance.rocketPool[i];

                    bool isMissile = dysonRocket.node == null;//只有null是导弹，其他的是正常的戴森火箭
                    int starIndex = __instance.starData.index;
                    bool forceDisplacement = false;

                    if (isMissile && missileProtoIds[starIndex].ContainsKey(i))
                    {
                        if (Configs.nextWaveState != 3)
                        {
                            __instance.RemoveDysonRocket(i);
                            continue;
                        }

                        int missileId = missileProtoIds[starIndex][i];
                        float missileMaxSpeed = (float)Configs.missile1Speed;
                        int damage = Configs.missile1Atk;
                        int dmgRange = Configs.missile1Range;
                        if (missileId == 8005 || missileId == 8008)
                        {
                            missileMaxSpeed = (float)Configs.missile2Speed;
                            damage = Configs.missile2Atk;
                            dmgRange = Configs.missile2Range;
                        }
                        else if (missileId == 8006)
                        {
                            missileMaxSpeed = (float)Configs.missile3Speed;
                            damage = Configs.missile3Atk;
                            dmgRange = Configs.missile3Range;
                            forceDisplacement = true;
                        }
                        float missileSpeedUp = (float)missileMaxSpeed / 200f;


                        //DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[dysonRocket.node.layerId];
                        AstroData astroPose = astroPoses[dysonRocket.planetId];
                        VectorLF3 vectorLF = astroPose.uPos - dysonRocket.uPos;
                        double num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
                        if (dysonRocket.t <= 0f) //如果离地面很近，是个加速从地面直线向上冲的过程
                        {
                            if (num8 < 200.0) //如果离地面很近，是个加速从地面直线向上冲的过程
                            {
                                float num9 = (float)num8 / 200f;
                                if (num9 < 0f)
                                {
                                    num9 = 0f;
                                }
                                float num10 = num9 * num9 * 600f + 15f;
                                dysonRocket.uSpeed = dysonRocket.uSpeed * 0.9f + num10 * 0.1f;
                                dysonRocket.t = (num9 - 1f) * 1.2f;
                                if (dysonRocket.t < -1f)
                                {
                                    dysonRocket.t = -1f;
                                }
                            }
                            else //离地面超过200
                            {

                                VectorLF3 vectorLF2 = dysonRocket.uPos;
                                //根据是导弹还是火箭确定
                                if (EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]) && EnemyShips.ships[MissileTargets[starIndex][i]].state == EnemyShip.State.active)//如果以前的目标敌人还存在
                                {
                                    vectorLF2 = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
                                }
                                else
                                {
                                    int newTargetId = FindTarget(starIndex, dysonRocket.planetId);
                                    if (newTargetId > 0)
                                    {
                                        MissileTargets[starIndex][i] = newTargetId;
                                        vectorLF2 = EnemyShips.ships[newTargetId].uPos - dysonRocket.uPos;
                                        dysonRocket.t = 0; //让其回到第一阶段，允许避障
                                    }
                                    else
                                    {
                                        missileProtoIds[starIndex][i] = 0;
                                        __instance.RemoveDysonRocket(i);
                                        goto IL_BDF;
                                    }
                                }

                                //根据距离地表的距离设置速度，被我改成一直加速了
                                double num11 = Math.Sqrt(vectorLF2.x * vectorLF2.x + vectorLF2.y * vectorLF2.y + vectorLF2.z * vectorLF2.z);
                                if (num11 < missileMaxSpeed * 3) //如果距离目标足够近，则进入下一阶段
                                {
                                    dysonRocket.t = 0.0001f;
                                }
                                else
                                {
                                    dysonRocket.t = 0f;
                                }
                                double num12 = num11 / ((double)dysonRocket.uSpeed + 0.1) * 0.382;
                                double num13 = num11 / (double)num5;
                                float num14 = (float)((double)dysonRocket.uSpeed * num12) + 150f; //这里对于导弹，num14是没有用的
                                if (num14 > num5)
                                {
                                    num14 = num5;
                                }
                                if (dysonRocket.uSpeed < missileMaxSpeed)
                                {
                                    dysonRocket.uSpeed += missileSpeedUp;
                                }
                                //else if (dysonRocket.uSpeed > num14 + num4)
                                //{
                                //	dysonRocket.uSpeed -= num4;
                                //}
                                else
                                {
                                    dysonRocket.uSpeed = missileMaxSpeed;
                                }

                                //下面难道是躲避巨星？
                                int num15 = -1;
                                double rhs = 0.0;
                                double num16 = 1E+40;
                                int num17 = dysonRocket.planetId / 100 * 100;
                                for (int j = num17; j < num17 + 10; j++)
                                {
                                    float uRadius = astroPoses[j].uRadius;
                                    if (uRadius >= 1f)
                                    {
                                        VectorLF3 vectorLF3 = dysonRocket.uPos - astroPoses[j].uPos;
                                        double num18 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
                                        double num19 = -((double)dysonRocket.uVel.x * vectorLF3.x + (double)dysonRocket.uVel.y * vectorLF3.y + (double)dysonRocket.uVel.z * vectorLF3.z);
                                        if ((num19 > 0.0 || num18 < (double)(uRadius * uRadius * 7f)) && num18 < num16)
                                        {
                                            rhs = ((num19 < 0.0) ? 0.0 : num19);
                                            num15 = j;
                                            num16 = num18;
                                        }
                                    }
                                }
                                VectorLF3 rhs2 = VectorLF3.zero;
                                float num20 = 0f;
                                if (num15 > 0)
                                {
                                    float num21 = astroPoses[num15].uRadius;
                                    bool flag = num15 % 100 == 0;
                                    if (flag)
                                    {
                                        num21 = 1000 - 400f; //dysonSphereLayer.orbitRadius - 400f
                                    }
                                    double num22 = 1.25;
                                    VectorLF3 vectorLF4 = dysonRocket.uPos + (VectorLF3)dysonRocket.uVel * rhs - astroPoses[num15].uPos;
                                    double num23 = vectorLF4.magnitude / (double)num21;
                                    if (num23 < num22)
                                    {
                                        double num24 = Math.Sqrt(num16) - (double)num21 * 0.82;
                                        if (num24 < 1.0)
                                        {
                                            num24 = 1.0;
                                        }
                                        double num25 = (num23 - 1.0) / (num22 - 1.0);
                                        if (num25 < 0.0)
                                        {
                                            num25 = 0.0;
                                        }
                                        num25 = 1.0 - num25 * num25;
                                        double num26 = (double)(dysonRocket.uSpeed - 6f) / num24 * 2.5 - 0.01;
                                        if (num26 > 1.5)
                                        {
                                            num26 = 1.5;
                                        }
                                        else if (num26 < 0.0)
                                        {
                                            num26 = 0.0;
                                        }
                                        num26 = num26 * num26 * num25;
                                        num20 = (float)(flag ? 0.0 : (num26 * 0.5));
                                        rhs2 = vectorLF4.normalized * num26 * 2.0;
                                    }
                                }
                                float num27 = 1f / (float)num13 - 0.05f;
                                num27 += num20;
                                float t = Mathf.Lerp(0.005f, 0.08f, num27);
                                dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF2.normalized + rhs2, t).normalized;
                                Quaternion b;
                                if (num11 < 350.0)
                                {
                                    float t2 = ((float)num11 - 50f) / 300f;
                                    b = Quaternion.Slerp(Quaternion.LookRotation(vectorLF2), Quaternion.LookRotation(dysonRocket.uVel), t2);//b = Quaternion.Slerp(dysonSphereLayer.NodeURot(dysonRocket.node), Quaternion.LookRotation(dysonRocket.uVel), t2);
                                }
                                else
                                {
                                    b = Quaternion.LookRotation(dysonRocket.uVel);
                                }
                                if (vectorLF2.magnitude < missileMaxSpeed * 0.5) //如果离得很近，则增大转弯速度
                                {
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.4f);
                                }
                                else if (vectorLF2.magnitude < missileMaxSpeed)
                                {
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.3f);
                                }
                                else
                                {
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.2f);
                                }

                            }
                        }
                        else
                        {


                            VectorLF3 vectorLF5 = dysonRocket.uPos;
                            //之前的目标是否还存活
                            if (EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]) && EnemyShips.ships[MissileTargets[starIndex][i]].state == EnemyShip.State.active)//如果以前的目标敌人还存在
                            {
                                vectorLF5 = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
                            }
                            else
                            {
                                int newTargetId = FindTarget(starIndex, dysonRocket.planetId);
                                if (newTargetId > 0)
                                {
                                    MissileTargets[starIndex][i] = newTargetId;
                                    vectorLF5 = EnemyShips.ships[newTargetId].uPos - dysonRocket.uPos;
                                    dysonRocket.t = 0; //让其回到第一阶段，允许避障
                                }
                                else
                                {
                                    missileProtoIds[starIndex][i] = 0;
                                    __instance.RemoveDysonRocket(i);
                                    goto IL_BDF;
                                }
                            }

                            double num28 = Math.Sqrt(vectorLF5.x * vectorLF5.x + vectorLF5.y * vectorLF5.y + vectorLF5.z * vectorLF5.z);
                            if (num28 < dmgRange * 0.5 && num28 < 400)
                            {
                                try
                                {
                                    //借助太阳帆弹射的效果触发爆炸动画
                                    int bulletIndex0 = __instance.swarm.AddBullet(new SailBullet
                                    {
                                        maxt = 0.000f,
                                        lBegin = dysonRocket.uPos,
                                        uEndVel = dysonRocket.uPos,
                                        uBegin = dysonRocket.uPos,
                                        uEnd = vectorLF5 + dysonRocket.uPos
                                    }, 1);

                                    __instance.swarm.bulletPool[bulletIndex0].state = 0;


                                    //持续爆炸，以及根据子弹爆炸范围决定爆炸效果
                                    for (int s = 0; s < 10; s++)
                                    {
                                        for (int ss = 0; ss < (int)Math.Sqrt(dmgRange) - 15; ss++)
                                        {
                                            VectorLF3 explodePoint = dysonRocket.uPos;
                                            int bulletIndex = __instance.swarm.AddBullet(new SailBullet
                                            {
                                                maxt = 0.016666667f * s,
                                                lBegin = dysonRocket.uPos,
                                                uEndVel = dysonRocket.uPos,
                                                uBegin = explodePoint,
                                                uEnd = explodePoint
                                            }, 1);

                                            __instance.swarm.bulletPool[bulletIndex].state = 0;

                                        }
                                    }
                                }
                                catch (Exception)
                                { }
                                

                                //范围伤害和强制位移
                                var shipsHit = EnemyShips.FindShipsInRange(dysonRocket.uPos, dmgRange);
                                if (shipsHit.Count > 0) UIBattleStatistics.RegisterHit(missileId, 0, 1); //首先注册一下该导弹击中，但不注册伤害
                                EnemyShip target = null;
                                if (!EnemyShips.ships.TryGetValue(MissileTargets[starIndex][i], out target)) target = null;

                                if (target != null && (target.uPos - GameMain.galaxy.PlanetById(target.shipData.planetB).uPosition).magnitude < 2000) //如果离地表过近，则不造成范围伤害，否则在护盾强大时过于imba。目前单线程没有这个修正，感觉可以先忽略
                                {
                                    double bonus = 0;
                                    if (relic2_7Activated) bonus += 1;
                                    if (relic2_15Activated && missileId == 8006) bonus += 9;
                                    damage = Relic.BonusDamage(damage, bonus);
                                    UIBattleStatistics.RegisterHit(missileId, target.BeAttacked(damage, DamageType.missileMain), 0);
                                }
                                else
                                {
                                    foreach (var item in shipsHit)
                                    {
                                        if (EnemyShips.ships.ContainsKey(item))
                                        {
                                            double distance = (dysonRocket.uPos - EnemyShips.ships[item].uPos).magnitude;
                                            int aoeDamage = damage;
                                            int realDamage = 0;
                                            if (distance > dmgRange * 0.5 && !relic1_1Activated)
                                            {
                                                aoeDamage = (int)(damage * (1.0 - (2 * distance - dmgRange) / dmgRange));
                                            }
                                            if (EnemyShips.ships[item] == target) // 对首要目标造成额外伤害
                                            {
                                                double bonus = 0;
                                                if (relic2_7Activated) bonus += 1;
                                                if (relic2_15Activated && missileId == 8006) bonus += 9;
                                                aoeDamage = Relic.BonusDamage(aoeDamage, bonus);
                                                realDamage = EnemyShips.ships[item].BeAttacked(aoeDamage, DamageType.missileMain);
                                            }
                                            else
                                            {
                                                realDamage = EnemyShips.ships[item].BeAttacked(aoeDamage, DamageType.missileAoe);
                                            }
                                            UIBattleStatistics.RegisterHit(missileId, realDamage, 0); //每个目标不再注册新的击中数量，只注册伤害
                                            //引力导弹的强制位移
                                            if (forceDisplacement)
                                            {
                                                EnemyShip hitShip = EnemyShips.ships[item];
                                                if (relic1_11Activated) // relic1-11 冰封陵墓 冻结效果
                                                {
                                                    if (relic1_1Activated) // relic1-1无视范围衰减
                                                        hitShip.InitForceDisplacement(hitShip.uPos, 180, 1);
                                                    else
                                                        hitShip.InitForceDisplacement(hitShip.uPos, (int)(180 * (1.0 - (2 * distance - dmgRange) / dmgRange)), 1);
                                                }
                                                else
                                                {
                                                    hitShip.InitForceDisplacement(dysonRocket.uPos);
                                                }
                                            }
                                        }
                                    }
                                }
                                missileProtoIds[starIndex][i] = 0;
                                __instance.RemoveDysonRocket(i);
                                goto IL_BDF;
                            }
                            float num29 = (float)(num28 * 0.75 + 15.0);
                            if (num29 > num5)
                            {
                                num29 = num5;
                            }
                            if (dysonRocket.uSpeed <= missileMaxSpeed)
                            {
                                //离目标过远且没到满速度 或 速度少于1/2的最大速度 或 速度少于5000都会加速；而如果离目标过近且速度超过1/2最大速度，或离得过近的同时速度超过了10000，会减速
                                if (vectorLF5.magnitude > Math.Min(missileMaxSpeed, 30000))
                                    dysonRocket.uSpeed += missileSpeedUp;
                                else
                                {
                                    if (dysonRocket.uSpeed > Math.Min(Math.Max(missileMaxSpeed * 0.5, 5000), 10000))
                                        dysonRocket.uSpeed -= missileSpeedUp;
                                    else if (dysonRocket.uSpeed < Math.Min(Math.Max(missileMaxSpeed * 0.5, 5000), 10000))
                                        dysonRocket.uSpeed += missileSpeedUp;
                                }
                            }
                            else
                            {
                                dysonRocket.uSpeed = missileMaxSpeed;
                            }
                            dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF5.normalized, 0.1f);
                            //dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.2f);
                            if (vectorLF5.magnitude < Configs.missile1Speed * 0.5f) //如果离得很近，则增大转弯速度
                            {
                                if (Quaternion.Angle(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5)) < 60)
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 1f);
                                else
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.6f);
                            }
                            else if (vectorLF5.magnitude < missileMaxSpeed)
                            {
                                dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.5f);
                            }
                            else
                            {
                                dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.4f);
                            }

                            dysonRocket.t = (350f - (float)num28) / 330f;
                            if (dysonRocket.t > 1f)
                            {
                                dysonRocket.t = 1f;
                            }
                            else if (dysonRocket.t < 0.0001f)
                            {
                                dysonRocket.t = 0.0001f;
                            }
                        }


                        VectorLF3 vectorLF6 = Vector3.zero;
                        double num30 = (double)(2f - (float)num8 / 200f);
                        if (num30 > 1.0)
                        {
                            num30 = 1.0;
                        }
                        else if (num30 < 0.0)
                        {
                            num30 = 0.0;
                        }
                        if (num30 > 0.0)
                        {
                            VectorLF3 v = dysonRocket.uPos - astroPose.uPos;
                            VectorLF3 v2 = Maths.QInvRotateLF(astroPose.uRot, v);
                            VectorLF3 lhs = Maths.QRotateLF(astroPose.uRotNext, v2) + astroPose.uPosNext;
                            Quaternion rhs3 = Quaternion.Inverse(astroPose.uRot) * dysonRocket.uRot;
                            Quaternion b2 = astroPose.uRotNext * rhs3;
                            num30 = (3.0 - num30 - num30) * num30 * num30;
                            vectorLF6 = (lhs - dysonRocket.uPos) * num30;
                            dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b2, (float)num30);
                        }

                        double num33 = (double)dysonRocket.uSpeed * num;
                        //下面很关键，如果导弹离目标过于近，则无视旋转直接直线飞过去
                        if (MissileTargets[starIndex].ContainsKey(i) && EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))
                        {
                            VectorLF3 toTarget = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
                            double distance = toTarget.magnitude;
                            if (distance < num33) //距离小于一帧的量
                            {
                                dysonRocket.uPos.x += toTarget.x;
                                dysonRocket.uPos.y += toTarget.y;
                                dysonRocket.uPos.z += toTarget.z;
                            }
                            else if (distance < missileMaxSpeed * 0.4) //直线过去
                            {
                                dysonRocket.uPos = dysonRocket.uPos + toTarget.normalized * num33;
                            }
                            else //这是原始规则
                            {
                                dysonRocket.uPos.x = dysonRocket.uPos.x + (double)dysonRocket.uVel.x * num33 + vectorLF6.x;
                                dysonRocket.uPos.y = dysonRocket.uPos.y + (double)dysonRocket.uVel.y * num33 + vectorLF6.y;
                                dysonRocket.uPos.z = dysonRocket.uPos.z + (double)dysonRocket.uVel.z * num33 + vectorLF6.z;
                            }

                        }
                        vectorLF = astroPose.uPos - dysonRocket.uPos;
                        num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
                        if (num8 < 180.0)
                        {
                            dysonRocket.uPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, (VectorLF3)dysonRocket.launch * ((double)astroPose.uRadius + num8));
                            dysonRocket.uRot = astroPose.uRot * Quaternion.LookRotation(dysonRocket.launch);
                        }
                        __instance.rocketPool[i] = dysonRocket;

                    }
                    else if (isMissile)
                    {
                        __instance.RemoveDysonRocket(i);
                    }
                    else//普通火箭不要动！！！！！！！！！！！但是现在加入了对恒星要塞火箭的特殊处理，他也要飞入节点但是他不能实际构建node的点数
                    {

                        DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[dysonRocket.node.layerId];
                        AstroData astroPose = astroPoses[dysonRocket.planetId];
                        VectorLF3 vectorLF = astroPose.uPos - dysonRocket.uPos;
                        double num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
                        if (dysonRocket.t <= 0f) //如果离地面很近，是个加速从地面直线向上冲的过程
                        {
                            if (num8 < 200.0) //如果离地面很近，是个加速从地面直线向上冲的过程
                            {
                                float num9 = (float)num8 / 200f;
                                if (num9 < 0f)
                                {
                                    num9 = 0f;
                                }
                                float num10 = num9 * num9 * 600f + 15f;
                                dysonRocket.uSpeed = dysonRocket.uSpeed * 0.9f + num10 * 0.1f;
                                dysonRocket.t = (num9 - 1f) * 1.2f;
                                if (dysonRocket.t < -1f)
                                {
                                    dysonRocket.t = -1f;
                                }
                            }
                            else //离地面超过200
                            {
                                dysonSphereLayer.NodeEnterUPos(dysonRocket.node, out VectorLF3 vectorLF2);
                                vectorLF2 -= dysonRocket.uPos;

                                double num11 = Math.Sqrt(vectorLF2.x * vectorLF2.x + vectorLF2.y * vectorLF2.y + vectorLF2.z * vectorLF2.z);
                                if (num11 < 50.0)
                                {
                                    dysonRocket.t = 0.0001f;
                                }
                                else
                                {
                                    dysonRocket.t = 0f;
                                }
                                double num12 = num11 / ((double)dysonRocket.uSpeed + 0.1) * 0.382;
                                double num13 = num11 / (double)num5;
                                float num14 = (float)((double)dysonRocket.uSpeed * num12) + 150f;
                                if (num14 > num5)
                                {
                                    num14 = num5;
                                }
                                if (dysonRocket.uSpeed < num14 - num3)
                                {
                                    dysonRocket.uSpeed += num3;
                                }
                                else if (dysonRocket.uSpeed > num14 + num4)
                                {
                                    dysonRocket.uSpeed -= num4;
                                }
                                else
                                {
                                    dysonRocket.uSpeed = num14;
                                }
                                int num15 = -1;
                                double rhs = 0.0;
                                double num16 = 1E+40;
                                int num17 = dysonRocket.planetId / 100 * 100;
                                for (int j = num17; j < num17 + 10; j++)
                                {
                                    float uRadius = astroPoses[j].uRadius;
                                    if (uRadius >= 1f)
                                    {
                                        VectorLF3 vectorLF3 = dysonRocket.uPos - astroPoses[j].uPos;
                                        double num18 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
                                        double num19 = -((double)dysonRocket.uVel.x * vectorLF3.x + (double)dysonRocket.uVel.y * vectorLF3.y + (double)dysonRocket.uVel.z * vectorLF3.z);
                                        if ((num19 > 0.0 || num18 < (double)(uRadius * uRadius * 7f)) && num18 < num16)
                                        {
                                            rhs = ((num19 < 0.0) ? 0.0 : num19);
                                            num15 = j;
                                            num16 = num18;
                                        }
                                    }
                                }
                                VectorLF3 rhs2 = VectorLF3.zero;
                                float num20 = 0f;
                                if (num15 > 0)
                                {
                                    float num21 = astroPoses[num15].uRadius;
                                    bool flag = num15 % 100 == 0;
                                    if (flag)
                                    {
                                        num21 = dysonSphereLayer.orbitRadius - 400f;
                                    }
                                    double num22 = 1.25;
                                    VectorLF3 vectorLF4 = dysonRocket.uPos + (VectorLF3)dysonRocket.uVel * rhs - astroPoses[num15].uPos;
                                    double num23 = vectorLF4.magnitude / (double)num21;
                                    if (num23 < num22)
                                    {
                                        double num24 = Math.Sqrt(num16) - (double)num21 * 0.82;
                                        if (num24 < 1.0)
                                        {
                                            num24 = 1.0;
                                        }
                                        double num25 = (num23 - 1.0) / (num22 - 1.0);
                                        if (num25 < 0.0)
                                        {
                                            num25 = 0.0;
                                        }
                                        num25 = 1.0 - num25 * num25;
                                        double num26 = (double)(dysonRocket.uSpeed - 6f) / num24 * 2.5 - 0.01;
                                        if (num26 > 1.5)
                                        {
                                            num26 = 1.5;
                                        }
                                        else if (num26 < 0.0)
                                        {
                                            num26 = 0.0;
                                        }
                                        num26 = num26 * num26 * num25;
                                        num20 = (float)(flag ? 0.0 : (num26 * 0.5));
                                        rhs2 = vectorLF4.normalized * num26 * 2.0;
                                    }
                                }
                                float num27 = 1f / (float)num13 - 0.05f;
                                num27 += num20;
                                float t = Mathf.Lerp(0.005f, 0.08f, num27);
                                dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF2.normalized + rhs2, t).normalized;
                                Quaternion b;
                                if (num11 < 350.0)
                                {
                                    float t2 = ((float)num11 - 50f) / 300f;
                                    b = Quaternion.Slerp(dysonSphereLayer.NodeURot(dysonRocket.node), Quaternion.LookRotation(dysonRocket.uVel), t2);
                                }
                                else
                                {
                                    b = Quaternion.LookRotation(dysonRocket.uVel);
                                }
                                dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.2f);
                            }
                        }
                        else
                        {
                            dysonSphereLayer.NodeSlotUPos(dysonRocket.node, out VectorLF3 vectorLF5);
                            vectorLF5 -= dysonRocket.uPos;
                            double num28 = Math.Sqrt(vectorLF5.x * vectorLF5.x + vectorLF5.y * vectorLF5.y + vectorLF5.z * vectorLF5.z);
                            if (num28 < 2.0)
                            {
                                // 下面这个判断是为了让构建恒星要塞的火箭不建造巨构的框架节点，而是构建恒星要塞
                                if (!(StarFortressSilo.starFortressRocketProtoIds[__instance.starData.index].ContainsKey(dysonRocket.id) && StarFortressSilo.starFortressRocketProtoIds[__instance.starData.index][dysonRocket.id] > 0))
                                    __instance.ConstructSp(dysonRocket.node);
                                else
                                {
                                    StarFortress.ConstructStarFortPoint(__instance.starData.index, StarFortressSilo.starFortressRocketProtoIds[__instance.starData.index][dysonRocket.id]);
                                    StarFortressSilo.starFortressRocketProtoIds[__instance.starData.index][dysonRocket.id] = 0;
                                }
                                __instance.RemoveDysonRocket(i);
                                goto IL_BDF;
                            }
                            float num29 = (float)(num28 * 0.75 + 15.0);
                            if (num29 > num5)
                            {
                                num29 = num5;
                            }
                            if (dysonRocket.uSpeed < num29 - num3)
                            {
                                dysonRocket.uSpeed += num3;
                            }
                            else if (dysonRocket.uSpeed > num29 + num4)
                            {
                                dysonRocket.uSpeed -= num4;
                            }
                            else
                            {
                                dysonRocket.uSpeed = num29;
                            }
                            dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF5.normalized, 0.1f);
                            dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, dysonSphereLayer.NodeURot(dysonRocket.node), 0.2f);
                            dysonRocket.t = (350f - (float)num28) / 330f;
                            if (dysonRocket.t > 1f)
                            {
                                dysonRocket.t = 1f;
                            }
                            else if (dysonRocket.t < 0.0001f)
                            {
                                dysonRocket.t = 0.0001f;
                            }
                        }
                        VectorLF3 vectorLF6 = Vector3.zero;
                        bool flag2 = false;
                        double num30 = (double)(2f - (float)num8 / 200f);
                        if (num30 > 1.0)
                        {
                            num30 = 1.0;
                        }
                        else if (num30 < 0.0)
                        {
                            num30 = 0.0;
                        }
                        if (num30 > 0.0)
                        {
                            VectorLF3 v = dysonRocket.uPos - astroPose.uPos;
                            VectorLF3 v2 = Maths.QInvRotateLF(astroPose.uRot, v);
                            VectorLF3 lhs = Maths.QRotateLF(astroPose.uRotNext, v2) + astroPose.uPosNext;
                            Quaternion rhs3 = Quaternion.Inverse(astroPose.uRot) * dysonRocket.uRot;
                            Quaternion b2 = astroPose.uRotNext * rhs3;
                            num30 = (3.0 - num30 - num30) * num30 * num30;
                            vectorLF6 = (lhs - dysonRocket.uPos) * num30;
                            dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b2, (float)num30);
                            flag2 = true;
                        }
                        if (!flag2)
                        {
                            VectorLF3 vectorLF7 = dysonRocket.uPos - __instance.starData.uPosition;
                            double num31 = Math.Abs(Math.Sqrt(vectorLF7.x * vectorLF7.x + vectorLF7.y * vectorLF7.y + vectorLF7.z * vectorLF7.z) - (double)dysonSphereLayer.orbitRadius);
                            double num32 = 1.5 - (double)((float)num31 / 1800f);
                            if (num32 > 1.0)
                            {
                                num32 = 1.0;
                            }
                            else if (num32 < 0.0)
                            {
                                num32 = 0.0;
                            }
                            if (num32 > 0.0)
                            {
                                VectorLF3 v3 = Maths.QInvRotateLF(dysonSphereLayer.currentRotation, vectorLF7);
                                VectorLF3 lhs2 = Maths.QRotateLF(dysonSphereLayer.nextRotation, v3) + __instance.starData.uPosition;
                                Quaternion rhs4 = Quaternion.Inverse(dysonSphereLayer.currentRotation) * dysonRocket.uRot;
                                Quaternion b3 = dysonSphereLayer.nextRotation * rhs4;
                                num32 = (3.0 - num32 - num32) * num32 * num32;
                                vectorLF6 = (lhs2 - dysonRocket.uPos) * num32;
                                dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b3, (float)num32);
                            }
                        }
                        double num33 = (double)dysonRocket.uSpeed * num;
                        dysonRocket.uPos.x = dysonRocket.uPos.x + (double)dysonRocket.uVel.x * num33 + vectorLF6.x;
                        dysonRocket.uPos.y = dysonRocket.uPos.y + (double)dysonRocket.uVel.y * num33 + vectorLF6.y;
                        dysonRocket.uPos.z = dysonRocket.uPos.z + (double)dysonRocket.uVel.z * num33 + vectorLF6.z;
                        vectorLF = astroPose.uPos - dysonRocket.uPos;
                        num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
                        if (num8 < 180.0)
                        {
                            dysonRocket.uPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, (VectorLF3)dysonRocket.launch * ((double)astroPose.uRadius + num8));
                            dysonRocket.uRot = astroPose.uRot * Quaternion.LookRotation(dysonRocket.launch);
                        }
                        __instance.rocketPool[i] = dysonRocket;

                    }
                }
            IL_BDF:;
            }

            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSphere), "RocketGameTick", new Type[] { typeof(int), typeof(int), typeof(int) })]
        public static bool RocketGameTickThreadPatch(ref DysonSphere __instance, int _usedThreadCnt, int _curThreadIdx, int _minimumMissionCnt)
        {
            AstroData[] astroPoses = __instance.starData.galaxy.astrosData;
            double num = 0.016666666666666666;
            float num2 = Mathf.Max(1f, (float)Math.Pow((double)__instance.defOrbitRadius / 40000.0 * 4.0, 0.4));
            float num3 = 7.5f;
            float num4 = 18f * num2;
            float num5 = 2800f * num2;
            int num6;
            int num7;
            bool relic1_1Activated = Relic.HaveRelic(1, 1); // relic 是否拥有
            bool relic1_11Activated = Relic.HaveRelic(1, 11);
            bool relic2_7Activated = Relic.HaveRelic(2, 7);
            bool relic2_15Activated = Relic.HaveRelic(2, 15);
            if (!WorkerThreadExecutor.CalculateMissionIndex(1, __instance.rocketCursor - 1, _usedThreadCnt, _curThreadIdx, _minimumMissionCnt, out num6, out num7))
            {
                return false;
            }
            for (int i = num6; i < num7; i++)
            {
                if (__instance.rocketPool[i].id == i)
                {
                    DysonRocket dysonRocket = __instance.rocketPool[i];

                    bool isMissile = dysonRocket.node == null;//只有null是导弹，其他的是正常的戴森火箭
                    int starIndex = __instance.starData.index;
                    bool forceDisplacement = false;

                    if (isMissile && missileProtoIds[starIndex].ContainsKey(i))
                    {

                        if (Configs.nextWaveState != 3)
                        {
                            __instance.RemoveDysonRocket(i);
                            continue;
                        }

                        int missileId = missileProtoIds[starIndex][i];
                        float missileMaxSpeed = (float)Configs.missile1Speed;
                        int damage = Configs.missile1Atk;
                        int dmgRange = Configs.missile1Range;
                        if (missileId == 8005 || missileId == 8008)
                        {
                            missileMaxSpeed = (float)Configs.missile2Speed;
                            damage = Configs.missile2Atk;
                            dmgRange = Configs.missile2Range;
                        }
                        else if (missileId == 8006)
                        {
                            missileMaxSpeed = (float)Configs.missile3Speed;
                            damage = Configs.missile3Atk;
                            dmgRange = Configs.missile3Range;
                            forceDisplacement = true;
                        }
                        float missileSpeedUp = (float)missileMaxSpeed / 200f;

                        //DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[dysonRocket.node.layerId];
                        AstroData astroPose = astroPoses[dysonRocket.planetId];
                        VectorLF3 vectorLF = astroPose.uPos - dysonRocket.uPos;
                        double num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
                        if (dysonRocket.t <= 0f) //如果离地面很近，是个加速从地面直线向上冲的过程
                        {
                            if (num8 < 200.0) //如果离地面很近，是个加速从地面直线向上冲的过程
                            {
                                float num9 = (float)num8 / 200f;
                                if (num9 < 0f)
                                {
                                    num9 = 0f;
                                }
                                float num10 = num9 * num9 * 600f + 15f;
                                dysonRocket.uSpeed = dysonRocket.uSpeed * 0.9f + num10 * 0.1f;
                                dysonRocket.t = (num9 - 1f) * 1.2f;
                                if (dysonRocket.t < -1f)
                                {
                                    dysonRocket.t = -1f;
                                }
                            }
                            else //离地面超过或等于200
                            {

                                VectorLF3 vectorLF2 = dysonRocket.uPos; //这个值下面立刻会修改

                                bool needFindNewTarget = true;
                                if (EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))//如果以前的目标敌人还存在
                                {
                                    EnemyShip ship = null;
                                    if (EnemyShips.ships.TryGetValue(MissileTargets[starIndex][i], out ship) && ship.state == EnemyShip.State.active)
                                    {
                                        needFindNewTarget = false;
                                        vectorLF2 = ship.uPos - dysonRocket.uPos;
                                    }
                                }

                                if(needFindNewTarget)
                                {
                                    int newTargetId = FindTarget(starIndex, dysonRocket.planetId, i); // 这个i是种子偏移
                                    if (newTargetId > 0)
                                    {
                                        MissileTargets[starIndex][i] = newTargetId;
                                        EnemyShip ship = null;
                                        if (EnemyShips.ships.TryGetValue(newTargetId, out ship))
                                            vectorLF2 = ship.uPos - dysonRocket.uPos;
                                        dysonRocket.t = 0; //让其回到第一阶段，允许避障
                                    }
                                    else
                                    {
                                        __instance.RemoveDysonRocket(i);
                                        goto IL_BDF;
                                    }
                                }

                                //设置速度
                                double num11 = Math.Sqrt(vectorLF2.x * vectorLF2.x + vectorLF2.y * vectorLF2.y + vectorLF2.z * vectorLF2.z);
                                if (num11 < missileMaxSpeed * 3) //如果与目标足够近，进入下一阶段
                                {
                                    dysonRocket.t = 0.0001f;
                                }
                                else
                                {
                                    dysonRocket.t = 0f;
                                }
                                double num12 = num11 / ((double)dysonRocket.uSpeed + 0.1) * 0.382;
                                double num13 = num11 / (double)num5;
                                float num14 = (float)((double)dysonRocket.uSpeed * num12) + 150f; //这里对于导弹，num14是没有用的
                                if (num14 > num5)
                                {
                                    num14 = num5;
                                }
                                if (dysonRocket.uSpeed < missileMaxSpeed)
                                {
                                    dysonRocket.uSpeed += missileSpeedUp;
                                }
                                //else if (dysonRocket.uSpeed > num14 + num4)
                                //{
                                //	dysonRocket.uSpeed -= num4;
                                //}
                                else
                                {
                                    dysonRocket.uSpeed = missileMaxSpeed;
                                }

                                //下面难道是躲避巨星？
                                int num15 = -1;
                                double rhs = 0.0;
                                double num16 = 1E+40;
                                int num17 = dysonRocket.planetId / 100 * 100;
                                for (int j = num17; j < num17 + 10; j++)
                                {
                                    float uRadius = astroPoses[j].uRadius;
                                    if (uRadius >= 1f)
                                    {
                                        VectorLF3 vectorLF3 = dysonRocket.uPos - astroPoses[j].uPos;
                                        double num18 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
                                        double num19 = -((double)dysonRocket.uVel.x * vectorLF3.x + (double)dysonRocket.uVel.y * vectorLF3.y + (double)dysonRocket.uVel.z * vectorLF3.z);
                                        if ((num19 > 0.0 || num18 < (double)(uRadius * uRadius * 7f)) && num18 < num16)
                                        {
                                            rhs = ((num19 < 0.0) ? 0.0 : num19);
                                            num15 = j;
                                            num16 = num18;
                                        }
                                    }
                                }
                                VectorLF3 rhs2 = VectorLF3.zero;
                                float num20 = 0f;
                                if (num15 > 0)
                                {
                                    float num21 = astroPoses[num15].uRadius;
                                    bool flag = num15 % 100 == 0;
                                    if (flag)
                                    {
                                        num21 = 1000 - 400f; //dysonSphereLayer.orbitRadius - 400f
                                    }
                                    double num22 = 1.25;
                                    VectorLF3 vectorLF4 = dysonRocket.uPos + (VectorLF3)dysonRocket.uVel * rhs - astroPoses[num15].uPos;
                                    double num23 = vectorLF4.magnitude / (double)num21;
                                    if (num23 < num22)
                                    {
                                        double num24 = Math.Sqrt(num16) - (double)num21 * 0.82;
                                        if (num24 < 1.0)
                                        {
                                            num24 = 1.0;
                                        }
                                        double num25 = (num23 - 1.0) / (num22 - 1.0);
                                        if (num25 < 0.0)
                                        {
                                            num25 = 0.0;
                                        }
                                        num25 = 1.0 - num25 * num25;
                                        double num26 = (double)(dysonRocket.uSpeed - 6f) / num24 * 2.5 - 0.01;
                                        if (num26 > 1.5)
                                        {
                                            num26 = 1.5;
                                        }
                                        else if (num26 < 0.0)
                                        {
                                            num26 = 0.0;
                                        }
                                        num26 = num26 * num26 * num25;
                                        num20 = (float)(flag ? 0.0 : (num26 * 0.5));
                                        rhs2 = vectorLF4.normalized * num26 * 2.0;
                                    }
                                }
                                float num27 = 1f / (float)num13 - 0.05f;
                                num27 += num20;
                                float t = Mathf.Lerp(0.005f, 0.08f, num27);
                                dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF2.normalized + rhs2, t).normalized;
                                Quaternion b;
                                if (num11 < 350.0)
                                {
                                    float t2 = ((float)num11 - 50f) / 300f;
                                    b = Quaternion.Slerp(Quaternion.LookRotation(vectorLF2), Quaternion.LookRotation(dysonRocket.uVel), t2);//b = Quaternion.Slerp(dysonSphereLayer.NodeURot(dysonRocket.node), Quaternion.LookRotation(dysonRocket.uVel), t2);
                                }
                                else
                                {
                                    b = Quaternion.LookRotation(dysonRocket.uVel);
                                }
                                if (vectorLF2.magnitude < missileMaxSpeed * 0.5) //如果离得很近，则增大转弯速度
                                {
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.4f);
                                }
                                else if (vectorLF2.magnitude < missileMaxSpeed)
                                {
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.3f);
                                }
                                else
                                {
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.2f);
                                }

                            }
                        }
                        else //远距离
                        {

                            VectorLF3 vectorLF5 = dysonRocket.uPos;
                            bool needFindNewTarget = true;
                            //之前的目标是否还存活
                            if (EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))//如果以前的目标敌人还存在
                            {
                                EnemyShip ship = null;
                                if (EnemyShips.ships.TryGetValue(MissileTargets[starIndex][i], out ship) && ship.state == EnemyShip.State.active)
                                {
                                    needFindNewTarget = false;
                                    vectorLF5 = ship.uPos - dysonRocket.uPos;
                                }
                            }
                            if(needFindNewTarget)
                            {
                                int newTargetId = FindTarget(starIndex, dysonRocket.planetId, i);
                                if (newTargetId > 0)
                                {
                                    MissileTargets[starIndex][i] = newTargetId;
                                    EnemyShip ship = null;
                                    if (EnemyShips.ships.TryGetValue(newTargetId, out ship))
                                        vectorLF5 = ship.uPos - dysonRocket.uPos;
                                    dysonRocket.t = 0; //让其回到第一阶段，允许避障
                                }
                                else
                                {
                                    __instance.RemoveDysonRocket(i);
                                    goto IL_BDF;
                                }
                            }



                            double num28 = Math.Sqrt(vectorLF5.x * vectorLF5.x + vectorLF5.y * vectorLF5.y + vectorLF5.z * vectorLF5.z);
                            if (num28 < dmgRange * 0.5 && num28 < 400)
                            {
                                try
                                {
                                    //借助太阳帆弹射的效果触发爆炸动画
                                    int bulletIndex0 = __instance.swarm.AddBullet(new SailBullet
                                    {
                                        maxt = 0.000f,
                                        lBegin = dysonRocket.uPos,
                                        uEndVel = dysonRocket.uPos,
                                        uBegin = dysonRocket.uPos,
                                        uEnd = vectorLF5 + dysonRocket.uPos
                                    }, 1);

                                    __instance.swarm.bulletPool[bulletIndex0].state = 0;

                                    //持续爆炸，以及根据子弹爆炸范围决定爆炸效果
                                    for (int s = 0; s < 10; s++)
                                    {
                                        for (int ss = 0; ss < (int)Math.Sqrt(dmgRange) - 15; ss++)
                                        {
                                            VectorLF3 explodePoint = dysonRocket.uPos;
                                            int bulletIndex = __instance.swarm.AddBullet(new SailBullet
                                            {
                                                maxt = 0.016666667f * s,
                                                lBegin = dysonRocket.uPos,
                                                uEndVel = dysonRocket.uPos,
                                                uBegin = explodePoint,
                                                uEnd = explodePoint
                                            }, 1);

                                            __instance.swarm.bulletPool[bulletIndex].state = 0;

                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                                //范围伤害和强制位移
                                var shipsHit = EnemyShips.FindShipsInRange(dysonRocket.uPos, dmgRange);
                                if (shipsHit.Count > 0) UIBattleStatistics.RegisterHit(missileId, 0, 1); //首先注册一下该导弹击中，但不注册伤害
                                EnemyShip target = null;
                                if (!EnemyShips.ships.TryGetValue(MissileTargets[starIndex][i], out target)) target = null;

                                if (target != null && (target.uPos - GameMain.galaxy.PlanetById(target.shipData.planetB).uPosition).magnitude < 2000) //如果离地表过近，则不造成范围伤害，否则在护盾强大时过于imba。目前单线程没有这个修正，感觉可以先忽略
                                {
                                    double bonus = 0;
                                    if (relic2_7Activated) bonus += 1;
                                    if (relic2_15Activated && missileId == 8006) bonus += 9;
                                    damage = Relic.BonusDamage(damage, bonus);
                                    UIBattleStatistics.RegisterHit(missileId, target.BeAttacked(damage, DamageType.missileMain), 0);
                                }
                                else
                                {
                                    foreach (var item in shipsHit)
                                    {
                                        if (EnemyShips.ships.ContainsKey(item))
                                        {
                                            EnemyShip hitShip = null;
                                            if (EnemyShips.ships.TryGetValue(item, out hitShip))
                                            {
                                                double distance = (dysonRocket.uPos - hitShip.uPos).magnitude;
                                                int aoeDamage = damage;
                                                int realDamage = 0;
                                                if (distance > dmgRange * 0.5 && !relic1_1Activated) // relic1-1虚空爆发 范围效果不衰减
                                                {
                                                    aoeDamage = (int)(damage * (1.0 - (2 * distance - dmgRange) / dmgRange));
                                                }
                                                if (hitShip == target) // 对首要目标造成额外伤害
                                                {
                                                    double bonus = 0;
                                                    if (relic2_7Activated) bonus += 1;
                                                    if (relic2_15Activated && missileId == 8006) bonus += 9;
                                                    aoeDamage = Relic.BonusDamage(aoeDamage, bonus);
                                                    realDamage = hitShip.BeAttacked(aoeDamage, DamageType.missileMain);
                                                }
                                                else
                                                {
                                                    realDamage = hitShip.BeAttacked(aoeDamage, DamageType.missileAoe);
                                                }
                                                UIBattleStatistics.RegisterHit(missileId, realDamage, 0); //每个目标不再注册新的击中数量，只注册伤害
                                                //引力导弹的强制位移
                                                if (forceDisplacement)
                                                {
                                                    if (relic1_11Activated) // relic1-11 冰封陵墓 冻结效果
                                                    {
                                                        if(relic1_1Activated) // relic1-1无视范围衰减
                                                            hitShip.InitForceDisplacement(hitShip.uPos,180,1);
                                                        else
                                                            hitShip.InitForceDisplacement(hitShip.uPos, (int)(180 * (1.0 - (2 * distance - dmgRange) / dmgRange)), 1);
                                                    }
                                                    else 
                                                    {
                                                        hitShip.InitForceDisplacement(dysonRocket.uPos);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                                missileProtoIds[starIndex][i] = 0;
                                __instance.RemoveDysonRocket(i);
                                goto IL_BDF;
                            }
                            float num29 = (float)(num28 * 0.75 + 15.0);
                            if (num29 > num5)
                            {
                                num29 = num5;
                            }
                            if (dysonRocket.uSpeed <= missileMaxSpeed)
                            {
                                //离目标过远且没到满速度 或 速度少于1/2的最大速度 或 速度少于5000都会加速；而如果离目标过近且速度超过1/2最大速度，或离得过近的同时速度超过了10000，会减速
                                if (vectorLF5.magnitude > Math.Min(missileMaxSpeed, 30000))
                                    dysonRocket.uSpeed += missileSpeedUp;
                                else
                                {
                                    if (dysonRocket.uSpeed > Math.Min(Math.Max(missileMaxSpeed * 0.5, 5000), 10000))
                                        dysonRocket.uSpeed -= missileSpeedUp;
                                    else if (dysonRocket.uSpeed < Math.Min(Math.Max(missileMaxSpeed * 0.5, 5000), 10000))
                                        dysonRocket.uSpeed += missileSpeedUp;
                                }
                            }
                            else
                            {
                                dysonRocket.uSpeed = missileMaxSpeed;
                            }
                            dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF5.normalized, 0.1f);
                            //dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.2f);
                            if (vectorLF5.magnitude < missileMaxSpeed * 0.5f) //如果离得很近，则增大转弯速度
                            {
                                if (Quaternion.Angle(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5)) < 60)
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 1f);
                                else
                                    dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.6f);
                            }
                            else if (vectorLF5.magnitude < missileMaxSpeed)
                            {
                                dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.5f);
                            }
                            else
                            {
                                dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.4f);
                            }
                            dysonRocket.t = (350f - (float)num28) / 330f;
                            if (dysonRocket.t > 1f)
                            {
                                dysonRocket.t = 1f;
                            }
                            else if (dysonRocket.t < 0.0001f)
                            {
                                dysonRocket.t = 0.0001f;
                            }
                        }


                        VectorLF3 vectorLF6 = Vector3.zero;
                        double num30 = (double)(2f - (float)num8 / 200f);
                        if (num30 > 1.0)
                        {
                            num30 = 1.0;
                        }
                        else if (num30 < 0.0)
                        {
                            num30 = 0.0;
                        }
                        if (num30 > 0.0)
                        {
                            VectorLF3 v = dysonRocket.uPos - astroPose.uPos;
                            VectorLF3 v2 = Maths.QInvRotateLF(astroPose.uRot, v);
                            VectorLF3 lhs = Maths.QRotateLF(astroPose.uRotNext, v2) + astroPose.uPosNext;
                            Quaternion rhs3 = Quaternion.Inverse(astroPose.uRot) * dysonRocket.uRot;
                            Quaternion b2 = astroPose.uRotNext * rhs3;
                            num30 = (3.0 - num30 - num30) * num30 * num30;
                            vectorLF6 = (lhs - dysonRocket.uPos) * num30;
                            dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b2, (float)num30);
                        }
                        double num33 = (double)dysonRocket.uSpeed * num; //这里是乘了1/60
                        VectorLF3 distanceCheck = new VectorLF3(999, 999, 999);
                        //下面很关键，如果导弹离目标过于近，则无视旋转直接直线飞过去
                        if (MissileTargets[starIndex].ContainsKey(i) && EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))
                        {
                            EnemyShip ship = null;
                            VectorLF3 toTarget = new VectorLF3(0, 0, 0);
                            if (EnemyShips.ships.TryGetValue(MissileTargets[starIndex][i], out ship))
                            {
                                toTarget = ship.uPos - dysonRocket.uPos;

                            }
                            double distance = toTarget.magnitude;
                            if (distance < num33) //距离小于一帧的量
                            {
                                dysonRocket.uPos.x += toTarget.x;
                                dysonRocket.uPos.y += toTarget.y;
                                dysonRocket.uPos.z += toTarget.z;
                            }
                            else if (distance < missileMaxSpeed * 0.4) //直线过去
                            {
                                dysonRocket.uPos = dysonRocket.uPos + toTarget.normalized * num33;
                            }
                            else //这是原始规则
                            {
                                dysonRocket.uPos.x = dysonRocket.uPos.x + (double)dysonRocket.uVel.x * num33 + vectorLF6.x;
                                dysonRocket.uPos.y = dysonRocket.uPos.y + (double)dysonRocket.uVel.y * num33 + vectorLF6.y;
                                dysonRocket.uPos.z = dysonRocket.uPos.z + (double)dysonRocket.uVel.z * num33 + vectorLF6.z;
                            }

                        }
                        vectorLF = astroPose.uPos - dysonRocket.uPos;
                        num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
                        if (num8 < 180.0)
                        {
                            dysonRocket.uPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, (VectorLF3)dysonRocket.launch * ((double)astroPose.uRadius + num8));
                            dysonRocket.uRot = astroPose.uRot * Quaternion.LookRotation(dysonRocket.launch);
                        }
                        __instance.rocketPool[i] = dysonRocket;

                        //Main.logger.LogInfo("Missile track error, might caused by multi-thread.");
                        //__instance.RemoveDysonRocket(i);

                    }
                    else if (isMissile)
                    {
                        __instance.RemoveDysonRocket(i);
                    }
                    else //普通火箭不要管！！！！！！！！！！！！！！！！！！！！！！！！！！！但是现在加入了对恒星要塞火箭的特殊处理，他也要飞入节点但是他不能实际构建node的点数
                    {

                        DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[dysonRocket.node.layerId];
                        AstroData astroPose = astroPoses[dysonRocket.planetId];
                        VectorLF3 vectorLF = astroPose.uPos - dysonRocket.uPos;
                        double num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
                        if (dysonRocket.t <= 0f) //如果离地面很近，是个加速从地面直线向上冲的过程
                        {
                            if (num8 < 200.0) //如果离地面很近，是个加速从地面直线向上冲的过程
                            {
                                float num9 = (float)num8 / 200f;
                                if (num9 < 0f)
                                {
                                    num9 = 0f;
                                }
                                float num10 = num9 * num9 * 600f + 15f;
                                dysonRocket.uSpeed = dysonRocket.uSpeed * 0.9f + num10 * 0.1f;
                                dysonRocket.t = (num9 - 1f) * 1.2f;
                                if (dysonRocket.t < -1f)
                                {
                                    dysonRocket.t = -1f;
                                }
                            }
                            else //离地面超过200
                            {
                                dysonSphereLayer.NodeEnterUPos(dysonRocket.node, out VectorLF3 vectorLF2);
                                vectorLF2 -= dysonRocket.uPos;

                                double num11 = Math.Sqrt(vectorLF2.x * vectorLF2.x + vectorLF2.y * vectorLF2.y + vectorLF2.z * vectorLF2.z);
                                if (num11 < 50.0)
                                {
                                    dysonRocket.t = 0.0001f;
                                }
                                else
                                {
                                    dysonRocket.t = 0f;
                                }
                                double num12 = num11 / ((double)dysonRocket.uSpeed + 0.1) * 0.382;
                                double num13 = num11 / (double)num5;
                                float num14 = (float)((double)dysonRocket.uSpeed * num12) + 150f;
                                if (num14 > num5)
                                {
                                    num14 = num5;
                                }
                                if (dysonRocket.uSpeed < num14 - num3)
                                {
                                    dysonRocket.uSpeed += num3;
                                }
                                else if (dysonRocket.uSpeed > num14 + num4)
                                {
                                    dysonRocket.uSpeed -= num4;
                                }
                                else
                                {
                                    dysonRocket.uSpeed = num14;
                                }
                                int num15 = -1;
                                double rhs = 0.0;
                                double num16 = 1E+40;
                                int num17 = dysonRocket.planetId / 100 * 100;
                                for (int j = num17; j < num17 + 10; j++)
                                {
                                    float uRadius = astroPoses[j].uRadius;
                                    if (uRadius >= 1f)
                                    {
                                        VectorLF3 vectorLF3 = dysonRocket.uPos - astroPoses[j].uPos;
                                        double num18 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
                                        double num19 = -((double)dysonRocket.uVel.x * vectorLF3.x + (double)dysonRocket.uVel.y * vectorLF3.y + (double)dysonRocket.uVel.z * vectorLF3.z);
                                        if ((num19 > 0.0 || num18 < (double)(uRadius * uRadius * 7f)) && num18 < num16)
                                        {
                                            rhs = ((num19 < 0.0) ? 0.0 : num19);
                                            num15 = j;
                                            num16 = num18;
                                        }
                                    }
                                }
                                VectorLF3 rhs2 = VectorLF3.zero;
                                float num20 = 0f;
                                if (num15 > 0)
                                {
                                    float num21 = astroPoses[num15].uRadius;
                                    bool flag = num15 % 100 == 0;
                                    if (flag)
                                    {
                                        num21 = dysonSphereLayer.orbitRadius - 400f;
                                    }
                                    double num22 = 1.25;
                                    VectorLF3 vectorLF4 = dysonRocket.uPos + (VectorLF3)dysonRocket.uVel * rhs - astroPoses[num15].uPos;
                                    double num23 = vectorLF4.magnitude / (double)num21;
                                    if (num23 < num22)
                                    {
                                        double num24 = Math.Sqrt(num16) - (double)num21 * 0.82;
                                        if (num24 < 1.0)
                                        {
                                            num24 = 1.0;
                                        }
                                        double num25 = (num23 - 1.0) / (num22 - 1.0);
                                        if (num25 < 0.0)
                                        {
                                            num25 = 0.0;
                                        }
                                        num25 = 1.0 - num25 * num25;
                                        double num26 = (double)(dysonRocket.uSpeed - 6f) / num24 * 2.5 - 0.01;
                                        if (num26 > 1.5)
                                        {
                                            num26 = 1.5;
                                        }
                                        else if (num26 < 0.0)
                                        {
                                            num26 = 0.0;
                                        }
                                        num26 = num26 * num26 * num25;
                                        num20 = (float)(flag ? 0.0 : (num26 * 0.5));
                                        rhs2 = vectorLF4.normalized * num26 * 2.0;
                                    }
                                }
                                float num27 = 1f / (float)num13 - 0.05f;
                                num27 += num20;
                                float t = Mathf.Lerp(0.005f, 0.08f, num27);
                                dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF2.normalized + rhs2, t).normalized;
                                Quaternion b;
                                if (num11 < 350.0)
                                {
                                    float t2 = ((float)num11 - 50f) / 300f;
                                    b = Quaternion.Slerp(dysonSphereLayer.NodeURot(dysonRocket.node), Quaternion.LookRotation(dysonRocket.uVel), t2);
                                }
                                else
                                {
                                    b = Quaternion.LookRotation(dysonRocket.uVel);
                                }
                                dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.2f);
                            }
                        }
                        else
                        {
                            dysonSphereLayer.NodeSlotUPos(dysonRocket.node, out VectorLF3 vectorLF5);
                            vectorLF5 -= dysonRocket.uPos;
                            double num28 = Math.Sqrt(vectorLF5.x * vectorLF5.x + vectorLF5.y * vectorLF5.y + vectorLF5.z * vectorLF5.z);
                            if (num28 < 2.0)
                            {
                                // 下面这个判断是为了让构建恒星要塞的火箭不建造巨构的框架节点，而是构建恒星要塞
                                if (!(StarFortressSilo.starFortressRocketProtoIds[__instance.starData.index].ContainsKey(dysonRocket.id) && StarFortressSilo.starFortressRocketProtoIds[__instance.starData.index][dysonRocket.id] > 0))
                                    __instance.ConstructSp(dysonRocket.node);
                                else
                                {
                                    StarFortress.ConstructStarFortPoint(__instance.starData.index, StarFortressSilo.starFortressRocketProtoIds[__instance.starData.index][dysonRocket.id]);
                                    StarFortressSilo.starFortressRocketProtoIds[__instance.starData.index][dysonRocket.id] = 0;
                                }
                                __instance.RemoveDysonRocket(i);
                                goto IL_BDF;
                            }
                            float num29 = (float)(num28 * 0.75 + 15.0);
                            if (num29 > num5)
                            {
                                num29 = num5;
                            }
                            if (dysonRocket.uSpeed < num29 - num3)
                            {
                                dysonRocket.uSpeed += num3;
                            }
                            else if (dysonRocket.uSpeed > num29 + num4)
                            {
                                dysonRocket.uSpeed -= num4;
                            }
                            else
                            {
                                dysonRocket.uSpeed = num29;
                            }
                            dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF5.normalized, 0.1f);
                            dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, dysonSphereLayer.NodeURot(dysonRocket.node), 0.2f);
                            dysonRocket.t = (350f - (float)num28) / 330f;
                            if (dysonRocket.t > 1f)
                            {
                                dysonRocket.t = 1f;
                            }
                            else if (dysonRocket.t < 0.0001f)
                            {
                                dysonRocket.t = 0.0001f;
                            }
                        }
                        VectorLF3 vectorLF6 = Vector3.zero;
                        bool flag2 = false;
                        double num30 = (double)(2f - (float)num8 / 200f);
                        if (num30 > 1.0)
                        {
                            num30 = 1.0;
                        }
                        else if (num30 < 0.0)
                        {
                            num30 = 0.0;
                        }
                        if (num30 > 0.0)
                        {
                            VectorLF3 v = dysonRocket.uPos - astroPose.uPos;
                            VectorLF3 v2 = Maths.QInvRotateLF(astroPose.uRot, v);
                            VectorLF3 lhs = Maths.QRotateLF(astroPose.uRotNext, v2) + astroPose.uPosNext;
                            Quaternion rhs3 = Quaternion.Inverse(astroPose.uRot) * dysonRocket.uRot;
                            Quaternion b2 = astroPose.uRotNext * rhs3;
                            num30 = (3.0 - num30 - num30) * num30 * num30;
                            vectorLF6 = (lhs - dysonRocket.uPos) * num30;
                            dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b2, (float)num30);
                            flag2 = true;
                        }
                        if (!flag2)
                        {
                            VectorLF3 vectorLF7 = dysonRocket.uPos - __instance.starData.uPosition;
                            double num31 = Math.Abs(Math.Sqrt(vectorLF7.x * vectorLF7.x + vectorLF7.y * vectorLF7.y + vectorLF7.z * vectorLF7.z) - (double)dysonSphereLayer.orbitRadius);
                            double num32 = 1.5 - (double)((float)num31 / 1800f);
                            if (num32 > 1.0)
                            {
                                num32 = 1.0;
                            }
                            else if (num32 < 0.0)
                            {
                                num32 = 0.0;
                            }
                            if (num32 > 0.0)
                            {
                                VectorLF3 v3 = Maths.QInvRotateLF(dysonSphereLayer.currentRotation, vectorLF7);
                                VectorLF3 lhs2 = Maths.QRotateLF(dysonSphereLayer.nextRotation, v3) + __instance.starData.uPosition;
                                Quaternion rhs4 = Quaternion.Inverse(dysonSphereLayer.currentRotation) * dysonRocket.uRot;
                                Quaternion b3 = dysonSphereLayer.nextRotation * rhs4;
                                num32 = (3.0 - num32 - num32) * num32 * num32;
                                vectorLF6 = (lhs2 - dysonRocket.uPos) * num32;
                                dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b3, (float)num32);
                            }
                        }
                        double num33 = (double)dysonRocket.uSpeed * num;
                        dysonRocket.uPos.x = dysonRocket.uPos.x + (double)dysonRocket.uVel.x * num33 + vectorLF6.x;
                        dysonRocket.uPos.y = dysonRocket.uPos.y + (double)dysonRocket.uVel.y * num33 + vectorLF6.y;
                        dysonRocket.uPos.z = dysonRocket.uPos.z + (double)dysonRocket.uVel.z * num33 + vectorLF6.z;
                        vectorLF = astroPose.uPos - dysonRocket.uPos;
                        num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
                        if (num8 < 180.0)
                        {
                            dysonRocket.uPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, (VectorLF3)dysonRocket.launch * ((double)astroPose.uRadius + num8));
                            dysonRocket.uRot = astroPose.uRot * Quaternion.LookRotation(dysonRocket.launch);
                        }
                        __instance.rocketPool[i] = dysonRocket;

                    }
                }
            IL_BDF:;
            }

            return false;
        }

        public static int AddDysonRockedGniMaerd(ref DysonSphere _this, ref DysonRocket rocket, DysonNode node = null)
        {
            DysonRocket[] obj = _this.rocketPool;
            int num;
            lock (obj)
            {
                //rocket.GiveRefNode(_this, node);
                rocket.nodeLayerId = 0;
                rocket.nodeId = 0;
                rocket.node = null;
                if (rocket.node == null)
                {
                    int num2;
                    if (_this.rocketRecycleCursor > 0)
                    {
                        int[] array = _this.rocketRecycle;
                        num = _this.rocketRecycleCursor - 1;
                        _this.rocketRecycleCursor = num;
                        num2 = array[num];
                    }
                    else
                    {
                        num = _this.rocketCursor;
                        _this.rocketCursor = num + 1;
                        num2 = num;
                        if (num2 == _this.rocketCapacity)
                        {
                            if (methodInfo == null) methodInfo = AccessTools.Method(typeof(DysonSphere), "SetRocketCapacity");
                            methodInfo.Invoke(_this, new object[] { _this.rocketCapacity * 2 });
                        }
                    }
                    _this.rocketPool[num2] = rocket;
                    _this.rocketPool[num2].id = num2;
                    _this.rocketPool[num2].t = -1f;
                    //_this.OrderConstructSp(node);必须删除！！！
                    num = num2;
                }
                else
                {
                    num = 0;
                }
            }
            return num;
        }

        private static int nextBulletId(int id)
        {
            return (id - 8003) % 3 + 8004;
        }

        private static int nextStarFortRocketId(int id)
        {
            return 1101;
        }
        public static int FindTarget(int starIndex, int planetId, int randSeed = 0, bool showLog = true)
        {
            int index;
            if (Utils.RandDouble() < 0.5)
            {
                index = FindNearestTarget(EnemyShips.minPlanetDisSortedShips[starIndex][planetId]);
                if (index > 0)
                {
                    return index;
                }
            }

            index = FindRandTarget(EnemyShips.minPlanetDisSortedShips[starIndex][planetId], randSeed);
            if (index > 0)
            {
                return index;
            }
            return FindRandTarget(EnemyShips.minTargetDisSortedShips[starIndex], randSeed);
        }

        private static int FindNearestTarget(List<EnemyShip> ships)
        {
            foreach (var ship in ships)
            {
                if (ship.state == EnemyShip.State.active && EnemyShips.ships.ContainsKey(ship.shipIndex))
                    return ship.shipIndex;
            }
            return -1;
        }

        private static int FindRandTarget(List<EnemyShip> ships, int randSeed = 0)
        {
            if (ships.Count == 0) return -1;
            return ships[Utils.RandInt(0, ships.Count)].shipIndex;
        }


        public static void Export(BinaryWriter w)
        {
            w.Write(MissileTargets.Count);
            for (int i1 = 0; i1 < MissileTargets.Count; i1++)
            {
                w.Write(MissileTargets[i1].Count);
                foreach (var item in MissileTargets[i1])
                {
                    w.Write(item.Key);
                    w.Write(item.Value);
                }
            }
            w.Write(missileProtoIds.Count);
            for (int i2 = 0; i2 < missileProtoIds.Count; i2++)
            {
                w.Write(missileProtoIds[i2].Count);
                foreach (var item in missileProtoIds[i2])
                {
                    w.Write(item.Key);
                    w.Write(item.Value);
                }
            }
        }

        public static void Import(BinaryReader r)
        {
            ReInitAll();
            int total1 = r.ReadInt32();
            for (int c1 = 0; c1 < total1 - MissileTargets.Count; c1++)
            {
                MissileTargets.Add(new ConcurrentDictionary<int, int>());
            }
            for (int i1 = 0; i1 < total1; i1++)
            {
                int num1 = r.ReadInt32();
                for (int j1 = 0; j1 < num1; j1++)
                {
                    MissileTargets[i1].TryAdd(r.ReadInt32(), r.ReadInt32());
                }
            }

            int total2 = r.ReadInt32();
            for (int c2 = 0; c2 < total2 - missileProtoIds.Count; c2++)
            {
                missileProtoIds.Add(new ConcurrentDictionary<int, int>());
            }
            for (int i2 = 0; i2 < total2; i2++)
            {
                int num2 = r.ReadInt32();
                for (int j2 = 0; j2 < num2; j2++)
                {
                    missileProtoIds[i2].TryAdd(r.ReadInt32(), r.ReadInt32());
                }
            }
        }

        public static void IntoOtherSave()
        {
            ReInitAll();
        }

        private static Text siloDetailText;
        private static Text siloPickerTitle;
        private static Text siloEditButtonText;
        private static string originSiloDetailLabel;
        private static string originSiloPickerTitle;
        private static string originSiloPickerButtonText;
        private static GameObject productIconObj;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UISiloWindow), "OnSiloIdChange")]
        public static void UISiloWindow_OnSiloIdChange(ref UISiloWindow __instance)
        {
            if (siloDetailText == null)
            {
                siloDetailText = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Silo Window/detail-texts/label (3)").GetComponent<Text>();
                siloPickerTitle = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Silo Window/node-picker/title").GetComponent<Text>();
                siloEditButtonText = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Silo Window/node-picker/button (Edit)/Text").GetComponent<Text>();
                productIconObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Silo Window/product");

                originSiloDetailLabel = siloDetailText.text;
                originSiloPickerTitle = siloPickerTitle.text;
                originSiloPickerButtonText = siloEditButtonText.text;
            }

            try
            {
                SiloComponent siloComponent = __instance.factorySystem.siloPool[__instance.siloId];
                int gmProtoId = __instance.factory.entityPool[siloComponent.entityId].protoId;

                if (gmProtoId == 2312 ||  gmProtoId == 8036)
                {
                    siloDetailText.text = originSiloDetailLabel;
                    siloPickerTitle.text = originSiloPickerTitle;
                    siloEditButtonText.text = originSiloPickerButtonText;
                    productIconObj.SetActive(true);
                }
                else
                {
                    siloDetailText.text = "剩余敌人".Translate();
                    siloPickerTitle.text = "火箭模式提示".Translate();
                    siloEditButtonText.text = "打开统计面板".Translate();
                    productIconObj.SetActive(false);
                }
            }
            catch (Exception) { }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UISiloWindow), "_OnUpdate")]
        public static void UISiloWindow_OnUpdate(ref UISiloWindow __instance)
        {
            if (__instance == null || __instance.factorySystem == null || __instance.factorySystem.siloPool == null) return;
            SiloComponent siloComponent = __instance.factorySystem.siloPool[__instance.siloId];
            int gmProtoId = __instance.factory.entityPool[siloComponent.entityId].protoId;
            if (gmProtoId != 2312 && gmProtoId != 8036)
            {
                __instance.value3Text.text = EnemyShips.ships.Count.ToString();
            }
            else if (siloComponent.bulletId > 8036 && siloComponent.bulletId <8040) // 说明是恒星要塞火箭的发射台
            {
                int index = siloComponent.bulletId - 8037;
                int starIndex = __instance.factory.planet.star.index;
                __instance.value3Text.text = StarFortress.moduleComponentCount[starIndex][index] + " / " + StarFortress.moduleMaxCount[starIndex][index]*StarFortress.compoPerModule[index];
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISiloWindow), "OnEditNodeClick")]
        public static bool UISiloWindow_OnEditNodeClick(ref UISiloWindow __instance)
        {
            SiloComponent siloComponent = __instance.factorySystem.siloPool[__instance.siloId];
            int gmProtoId = __instance.factory.entityPool[siloComponent.entityId].protoId;
            if (gmProtoId != 2312 && gmProtoId != 8036)
            {
                UIRoot.instance.uiGame.OpenProductionWindow();
                UIBattleStatistics.OnClickBattleStatButton();
                return false;
            }
            return true;
        }

    }
}
