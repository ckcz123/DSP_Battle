using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DSP_Battle
{
    class Cannon
    {
        //需要存读档
        // 这里应该是HashSet，为了线程安全还是用ConcurrentDictionary
        public static List<ConcurrentDictionary<int, int>> sailBulletsIndex; //记录应该变成太阳帆的子弹，原本是记录攻击用子弹，但是总有漏网之鱼变成太阳帆，找不到原因，所以反过来记录应该变成太阳帆的子弹，这可能导致0.1%（目测，或许远低于此）的太阳帆无法生成
        public static List<ConcurrentDictionary<int, int>> bulletTargets; //记录子弹的目标
        public static List<ConcurrentDictionary<int, int>> bulletIds; //记录子弹Id，原本是canDoDamage记录子弹还能造成多少伤害
        //

        public static bool doTrack = true;
        public static System.Random rand = new System.Random();

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
                            if (!sailBulletsIndex[starIndex].ContainsKey(j)) //只有对应swarm的对应位置的bullet不是之前存下来的solarsail的Bullet的时候才改变目标终点
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

            }
            catch (Exception)
            {
            }
        }

        public static void ReInitAll()
        {
            try
            {
                sailBulletsIndex = new List<ConcurrentDictionary<int, int>>();
                bulletTargets = new List<ConcurrentDictionary<int, int>>();
                bulletIds = new List<ConcurrentDictionary<int, int>>();

                for (int i = 0; i < GameMain.galaxy.starCount; i++)
                {
                    sailBulletsIndex.Add(new ConcurrentDictionary<int, int>());
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
        public static bool EjectorPatch(ref EjectorComponent __instance, float power, DysonSwarm swarm, AstroPose[] astroPoses, AnimData[] animPool, int[] consumeRegister, ref uint __result)
        {
            int planetId = __instance.planetId;
            int starIndex = planetId / 100 - 1;
            PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
            int gmProtoId = factory.entityPool[__instance.entityId].protoId;

            //if (gmProtoId != 9801) return true; // 暂时不要取消注释，下面已经做了普通弹射器的适配。
            bool cannon = isCannon(gmProtoId);

            //子弹需求循环
            if (cannon && __instance.bulletCount == 0 && gmProtoId != 8014 && GameMain.instance.timei % 60 == 0)
            {
                __instance.bulletId = nextBulletId(__instance.bulletId);
            }
            else if (gmProtoId == 8014)
            {
                __instance.bulletId = 8007;
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

            //下面是因为 炮需要用orbitId记录索敌模式，而orbitId有可能超出已设定的轨道数，为了避免溢出，炮的orbitalId在参与计算时需要独立指定为1。
            //后续所有的__instance.orbitId都被替换为此
            int calcOrbitId = __instance.orbitId;
            if (cannon)
            {
                if (calcOrbitId <= 0 || calcOrbitId > 4) calcOrbitId = 1;
            }
            else
            {
                if (calcOrbitId < 0 || calcOrbitId >= swarm.orbitCursor || swarm.orbits[calcOrbitId].id != calcOrbitId || !swarm.orbits[calcOrbitId].enabled)
                {
                    calcOrbitId = 0;
                    //如果是原本的弹射器，则修改也得同步到真正的orbitId上
                    __instance.orbitId = calcOrbitId;
                }

            }

            float num2 = (float)Cargo.accTableMilli[__instance.incLevel];

            if (cannon)
                num2 = (float)Cargo.incTableMilli[__instance.incLevel];

            int num3 = (int)(power * 10000f * (1f + num2) + 0.1f);
            if (calcOrbitId == 0)
            {
                if (__instance.direction == 1)
                {
                    __instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
                    __instance.direction = -1;
                }
                if (__instance.direction == -1)
                {
                    __instance.time -= num3;
                    if (__instance.time <= 0)
                    {
                        __instance.time = 0;
                        __instance.direction = 0;
                    }
                }
                if (power >= 0.1f)
                {
                    __instance.localDir.x = __instance.localDir.x * 0.9f;
                    __instance.localDir.y = __instance.localDir.y * 0.9f;
                    __instance.localDir.z = __instance.localDir.z * 0.9f + 0.1f;
                    __result = 1U;
                    return false;
                }
                __result = 0U;
                return false;
            }
            else
            {
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
                uint result = 0U;
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
                int loopNum = 1;
                double cannonSpeedScale = 1;
                if (gmProtoId == 8012)
                    cannonSpeedScale = 2;
                EnemyShip curTarget = null;

                if (cannon)
                {
                    loopNum = sortedShips.Count;
                    if (__instance.bulletId == 8001)
                    {
                        maxtDivisor = Configs.bullet1Speed * cannonSpeedScale;
                        damage = (int)Configs.bullet1Atk; //只有这个子弹能够因为引力弹射器而强化伤害
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
                }



                //不该参与循环的部分，换到循环前了

                bool flag2 = __instance.bulletCount > 0;
                if (gmProtoId == 8014) //脉冲炮不需要子弹
                    flag2 = true;
                VectorLF3 vectorLF2 = VectorLF3.zero;

                for (int gm = 0; gm < loopNum; gm++)
                {

                    //新增的，每次循环开始必须重置
                    __instance.targetState = EjectorComponent.ETargetState.OK;
                    flag = true;
                    flag2 = __instance.bulletCount > 0;
                    if (gmProtoId == 8014) //脉冲炮不需要子弹
                        flag2 = true;

                    int shipIdx = 0;//ship总表中的唯一标识：index
                    if (cannon)
                    {
                        vectorLF2 = sortedShips[gm].uPos;
                        shipIdx = sortedShips[gm].shipIndex;
                        if (!EnemyShips.ships.ContainsKey(shipIdx)) continue;
                    }
                    else
                    {
                        vectorLF2 = uPos + VectorLF3.Cross(swarm.orbits[calcOrbitId].up, b).normalized * (double)swarm.orbits[calcOrbitId].radius;
                    }
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
                    if (cannon && EnemyShips.ships.ContainsKey(shipIdx) && EnemyShips.ships[shipIdx].state == EnemyShip.State.active && __instance.targetState != EjectorComponent.ETargetState.Blocked && __instance.targetState != EjectorComponent.ETargetState.AngleLimit)
                    {
                        curTarget = EnemyShips.ships[shipIdx]; //设定目标
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
                if (curTarget == null && cannon)
                {
                    flag = false; //本身是由于俯仰限制或路径被阻挡的判断，现在找不到目标而不打炮也算做里面
                }
                else if (curTarget != null && curTarget.hp <= 0 && cannon)
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

                        //下面是添加子弹
                        int bulletIndex = swarm.AddBullet(new SailBullet
                        {
                            maxt = (float)(__instance.targetDist / maxtDivisor),
                            lBegin = vector,
                            uEndVel = VectorLF3.Cross(vectorLF2 - uPos, swarm.orbits[calcOrbitId].up).normalized * Math.Sqrt((double)(swarm.dysonSphere.gravity / swarm.orbits[calcOrbitId].radius)), //至少影响着形成的太阳帆的初速度方向
                            uBegin = uBeginChange,
                            uEnd = vectorLF2
                        }, calcOrbitId);


                        //将添加的用于攻击的子弹的index存储，便于后续更新其弹道，又能防止影响正常的太阳帆
                        if (!cannon && !sailBulletsIndex[swarm.starData.index].ContainsKey(bulletIndex))
                        {
                            sailBulletsIndex[swarm.starData.index].AddOrUpdate(bulletIndex, 0, (x, y) => 0);
                        }
                        //如果是炮，设定子弹目标以及伤害，并注册伤害
                        else if (cannon)
                        {
                            try
                            {
                                swarm.bulletPool[bulletIndex].state = 0; //设置成0，该子弹将不会生成太阳帆
                            }
                            catch (Exception)
                            {
                                DspBattlePlugin.logger.LogInfo("bullet info1 set error.");
                            }

                            UIBattleStatistics.RegisterShootOrLaunch(__instance.bulletId, damage);
                            bulletTargets[swarm.starData.index].AddOrUpdate(bulletIndex, curTarget.shipIndex, (x, y) => curTarget.shipIndex);

                            //Main.logger.LogInfo("bullet info2 set error.");


                            try
                            {
                                int bulletId = __instance.bulletId;
                                bulletIds[swarm.starData.index].AddOrUpdate(bulletIndex, bulletId, (x, y) => bulletId);
                                // bulletIds[swarm.starData.index][bulletIndex] = 1;//后续可以根据子弹类型/炮类型设定不同数值
                            }
                            catch (Exception)
                            {
                                DspBattlePlugin.logger.LogInfo("bullet info3 set error.");
                            }

                        }
                        if (__instance.bulletCount != 0)
                        {
                            __instance.bulletInc -= __instance.bulletInc / __instance.bulletCount;
                        }
                        __instance.bulletCount--;
                        if (__instance.bulletCount <= 0)
                        {
                            __instance.bulletInc = 0;
                            __instance.bulletCount = 0;
                        }
                        lock (consumeRegister)
                        {
                            consumeRegister[__instance.bulletId]++;
                        }
                        __instance.time = __instance.coldSpend;
                        __instance.direction = -1;

                        //if (gmProtoId == 8014) //激光炮为了视觉效果，取消冷却阶段每帧都发射（不能简单地将charge和cold的spend设置为0，因为会出现除以0的错误）
                        //    __instance.direction = 1;

                    }
                }
                else if (!cannon)
                {
                    return true; //如果不是炮，又没发射，就返回。这是为了防止return false影响潜在的其他mod的prepatch。 有必要吗？？？？
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

                __result = result;
                return false;
            }
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
            int starIndex = __instance.starData.index;

            foreach (var i in bulletTargets[starIndex].Keys)
            {
                if (__instance.bulletPool.Length > i && __instance.bulletPool[i].id == i && !sailBulletsIndex[starIndex].ContainsKey(i)) //后面的判断条件就是说只对攻击用的子弹生效，不对正常的太阳帆操作
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
                            switch (bulletId)
                            {
                                case 8001: damage = Configs.bullet1Atk; break;
                                case 8002: damage = Configs.bullet2Atk; break;
                                case 8003: damage = Configs.bullet3Atk; break;
                                case 8007: damage = Configs.bullet4Atk; break;
                                default:
                                    break;
                            }

                            int realDamage = EnemyShips.ships[bulletTargets[starIndex][i]].BeAttacked(damage); //击中造成伤害  //如果在RemoveBullet的postpatch写这个，可以不用每帧循环检测，但是伤害将在爆炸动画后结算，感觉不太合理
                            UIBattleStatistics.RegisterHit(bulletId, realDamage);
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
            if (sailBulletsIndex[__instance.starData.index].ContainsKey(id))
            {
                int v;
                sailBulletsIndex[__instance.starData.index].TryRemove(id, out v); //删除，i不再被记为太阳帆子弹。子弹实体会在后续自动被游戏原本逻辑移除
            }
            if (bulletTargets[__instance.starData.index].ContainsKey(id))
            {
                int v;
                bulletTargets[__instance.starData.index].TryRemove(id, out v);
            }

        }

        public static void Export(BinaryWriter w)
        {
            w.Write(sailBulletsIndex.Count);
            for (int i1 = 0; i1 < sailBulletsIndex.Count; i1++)
            {
                w.Write(sailBulletsIndex[i1].Count);
                foreach (var item in sailBulletsIndex[i1].Keys)
                {
                    w.Write(item);
                }
            }
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
            int total1 = r.ReadInt32();
            for (int c1 = 0; c1 < total1 - sailBulletsIndex.Count; c1++)
            {
                sailBulletsIndex.Add(new ConcurrentDictionary<int, int>());
            }
            for (int i1 = 0; i1 < total1; i1++)
            {
                int num1 = r.ReadInt32();
                for (int j1 = 0; j1 < num1; j1++)
                {
                    sailBulletsIndex[i1].TryAdd(r.ReadInt32(), 0);
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
