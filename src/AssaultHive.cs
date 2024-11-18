using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    public class AssaultHive
    {
        public int listIndex; // index of AssaultController.assaultHives;
        public int starIndex;
        public int oriAstroId; // 1000000+byAstro，也是hiveAstroId
        public int orbitIndex; // 0-7, hive's (orbit) index in star system
        public int byAstroIndex; // spaceSector.dfHivesByAstro[] index
        public int strength; // strength

        public EAssaultHiveState state;
        public int time;
        public int timeTillAssault; // totalTime before assault begins
        public int timeTotalInit; // 最初的时间计数
        public int timeDelayedByRelic; // relic 4-7 增加的时间，最大为
        public int level;
        public int oriLevel;

        public int assaultNumTotal; // 初始入侵的舰队数量
        public int inhibitPoints; // 已通过恒星炮压制的点数
        public int inhibitPointsLimit; // 最多可以压制的点数
        public int inhibitPointsTotalInit; // 总强度点
        public bool canFullyStopped; // 是否可以完全阻止
        public bool isSuper; // 超级入侵

        public int enemyKilled;

        public EnemyDFHiveSystem hive;
        public bool isCreated; // 巢穴原本是null或者没有核心，是创建出来的
        public int assembleNum { get { return Math.Min(1440, (int)(assaultNum * 1.5) + 1); } } // 一直制造的舰队数量上限

        public static int timeDelayedMax = 3600 * 20; // 最大推迟的时间


        public int assaultNum
        {
            get
            {
                if (inhibitPoints >= inhibitPointsTotalInit || inhibitPointsTotalInit <= 0)
                    return 0;
                else
                    return Math.Max(1, (int)(assaultNumTotal * (inhibitPointsTotalInit - inhibitPoints) * 1.0 / inhibitPointsTotalInit));
            }
        }

        public AssaultHive(int starIndex, int orbitIndex, int listIndex)
        {
            this.oriAstroId = 1000000 + starIndex * 8 + orbitIndex + 1;
            this.starIndex = starIndex;
            this.orbitIndex = orbitIndex;
            this.listIndex = listIndex;
            byAstroIndex = oriAstroId - 1000000;
            state = EAssaultHiveState.Idle;
            time = 0;
            timeDelayedByRelic = 0;
            inhibitPoints = 0;
            isCreated = false;
            isSuper = false;
            enemyKilled = 0;
            CreateOrGetHive();
        }

        public void CreateOrGetHive()
        {
            hive = GameMain.data.spaceSector.dfHivesByAstro[byAstroIndex];
            if (MP.clientBlocker) return;
            if(hive == null)
            {
                int count = 8;
                while (hive == null && count > 0)
                {
                    GameMain.spaceSector.TryCreateNewHive(GameMain.galaxy.StarById(starIndex + 1));
                    hive = GameMain.data.spaceSector.dfHivesByAstro[byAstroIndex];
                    count--;
                }
            }
            if (hive != null)
            {
                if (!hive.hasCore())
                {
                    isCreated = true;
                    hive.BuildCore();
                }
                if (!hive.realized)
                    hive.Realize();
                oriLevel = hive.evolve.level;
            }
            else
            {
                Debug.Log($"hive is null when init assault hives with {starIndex} - {listIndex}, now removing");
                state = EAssaultHiveState.Remove;
            }
        }

        public void LogicTick()
        {
            if (hive == null)
            {
                if (!MP.clientBlocker)
                {
                    state = EAssaultHiveState.Remove;
                    time = 0;
                }
                else
                {
                    Utils.Log("null hive, but not removing because you are client.");
                }
            }
            else if(!hive.isAlive && state != EAssaultHiveState.End && state != EAssaultHiveState.Remove)
            {
                state = EAssaultHiveState.End;
                time = 120;
            }
            switch (state)
            {
                case EAssaultHiveState.Idle:
                    LogicTickIdle();
                    break;
                case EAssaultHiveState.Expand:
                    LogicTickExpand();
                    break;
                case EAssaultHiveState.Assemble:
                    LogicTickAssemble();
                    break;
                case EAssaultHiveState.Assault:
                    LogicTickAssault();
                    break;
                case EAssaultHiveState.End:
                    LogicTickEnd();
                    break;
                case EAssaultHiveState.Remove:
                    LogicTickRemove();
                    break;
                default:
                    break;
            }

            // Update invincible and modifier array
            // if not removing or assaulting state, add me to the invincibleHives array
            if (state == EAssaultHiveState.Idle || state == EAssaultHiveState.Expand || state == EAssaultHiveState.Assemble)
                AssaultController.invincibleHives[byAstroIndex] = listIndex;
            else
                AssaultController.invincibleHives[byAstroIndex] = -1;

            // if not removing state, and have modifier, add me to the modifierHives array
            if (state == EAssaultHiveState.Assault && AssaultController.modifierEnabled)
                AssaultController.modifierHives[byAstroIndex] = listIndex;
            else
                AssaultController.modifierHives[byAstroIndex] = -1;

            // if should show special alert monitor, add me to the alertHives array
            if (state == EAssaultHiveState.Assemble || state == EAssaultHiveState.Assault)
                AssaultController.alertHives[byAstroIndex] = listIndex;
            else
                AssaultController.alertHives[byAstroIndex] = -1;
        }
        public void LogicTickIdle()
        {
            timeTillAssault--;
            time--;
            if(hive != null)
                hive.evolve.threat = 0;

            if (time <= 0)
            {
                state = EAssaultHiveState.Expand;
                time = 5400;
            }
        }

        public void LogicTickExpand()
        {
            timeTillAssault--;
            time--;
            if (hive != null)
                hive.evolve.threat = 0;

            QuickBuild(5);

            hive.evolve.threat = 0;
            hive.evolve.waveTicks = 0;
            hive.evolve.waveAsmTicks = 0;

            if (time <= 0)
            {
                state = EAssaultHiveState.Assemble;
                time = timeTillAssault;

                // 弹出提示
                if(listIndex == 0)
                {
                    AssaultController.NotifyAssaultDetected();
                }
                if(CheckIfAllHiveReachedState(EAssaultHiveState.Assemble))
                {
                    MP.Sync(EDataType.CallOnAssaultStateSwitch);
                }
            }

        }
        public void LogicTickAssemble()
        {
            if (Relic.playerIdleTime < Relic.playerIdleTimeMax)
            {
                timeTillAssault--;
                time--;
            }
            if (hive != null)
                hive.evolve.threat = 0;
            QuickAssemble();

            hive.evolve.threat = 1;
            hive.evolve.waveTicks = 0;
            hive.evolve.waveAsmTicks = 0;

            if (time <= 0 && !MP.clientBlocker)
            {
                LaunchAssault();
                state = EAssaultHiveState.Assault;
                if (!AssaultController.modifierEnabled && AssaultController.modifier.Sum() > 0) // 虽然modifier可能出现负数，还是可以用Sum()，但是出现负数的时候modifierEnabled已经置为true了，不会影响置true的过程
                    AssaultController.modifierEnabled = true;
                time = 0;

                // 触发修改器 modifier 6 7 8 9，只有第一次切换到入侵状态的巢穴才会触发这个，一旦触发，就会将数值变为负数来防止后续触发，且用于结束之后返还减益的系数，结束之后的返还在AssaultController
                if(AssaultController.modifierEnabled)
                {
                    int value = AssaultController.modifier[6];
                    if(value > 0) 
                    {
                        float realDebuff = value * 1.0f / 100f;
                        if (realDebuff > GameMain.data.history.kineticDamageScale)
                            realDebuff = GameMain.data.history.kineticDamageScale;
                        int realDebuffInt = (int)(realDebuff * 100);
                        realDebuff = realDebuffInt * 1.0f / 100f;
                        GameMain.data.history.kineticDamageScale -= realDebuff;
                        AssaultController.modifier[6] = -realDebuffInt; // 设置为负数
                    }
                    value = AssaultController.modifier[7];
                    if (value > 0)
                    {
                        float realDebuff = value * 1.0f / 100f;
                        if (realDebuff > GameMain.data.history.energyDamageScale)
                            realDebuff = GameMain.data.history.energyDamageScale;
                        int realDebuffInt = (int)(realDebuff * 100);
                        realDebuff = realDebuffInt * 1.0f / 100f;
                        GameMain.data.history.energyDamageScale -= realDebuff;
                        AssaultController.modifier[7] = -realDebuffInt; // 设置为负数
                    }
                    value = AssaultController.modifier[8];
                    if (value > 0)
                    {
                        float realDebuff = value * 1.0f / 100f;
                        if (realDebuff > GameMain.data.history.blastDamageScale)
                            realDebuff = GameMain.data.history.blastDamageScale;
                        int realDebuffInt = (int)(realDebuff * 100);
                        realDebuff = realDebuffInt * 1.0f / 100f;
                        GameMain.data.history.blastDamageScale -= realDebuff;
                        AssaultController.modifier[8] = -realDebuffInt; // 设置为负数
                    }
                    value = AssaultController.modifier[9];
                    if (value > 0)
                    {
                        float realDebuff = value * 1.0f / 100f;
                        if (realDebuff > GameMain.data.history.magneticDamageScale)
                            realDebuff = GameMain.data.history.magneticDamageScale;
                        int realDebuffInt = (int)(realDebuff * 100);
                        realDebuff = realDebuffInt * 1.0f / 100f;
                        GameMain.data.history.magneticDamageScale -= realDebuff;
                        AssaultController.modifier[9] = -realDebuffInt; // 设置为负数
                    }
                }

                if(CheckIfAllHiveReachedState(EAssaultHiveState.Assault))
                {
                    MP.Sync(EDataType.CallOnLaunchAllVoidAssault); // 当所有巢穴都发起攻击后，同步spacesector、assaultController以及gamehistory数据
                }
            }
        }
        public void LogicTickAssault()
        {
            timeTillAssault--;
            time--;

            if (hive == null)
            {
                state = EAssaultHiveState.End;
                time = 120;
            }
            if (MP.clientBlocker) return;

            if(hive.hasIncomingAssaultingUnit)
            {

            }
            else
            {
                state = EAssaultHiveState.End;
                time = 120;
            }

            if (time == 1)
            {
                state = EAssaultHiveState.End;
                time = 120;
            }
            
        }
        public void LogicTickEnd()
        {
            timeTillAssault--;
            time--;
            if (hive != null)
            {
                int slice = 100;
                int len = hive.pbuilders.Length;
                int begin = len / slice * (time % slice);
                int end = len / slice * (time % slice + 1);
                if (end > len)
                    end = len;
                if (time % slice == slice - 1)
                    end = len;
                for (int i = begin; i < end; i++)
                {
                    int piid = hive.pbuilders[i].instId;
                    if(piid > 0)
                        KillHiveStrcuture(piid);
                }
                //for (int i = 0; i < hive.units.buffer.Length; i++)
                //{
                //    int enemyId = hive.units.buffer[i].enemyId;
                //    if(enemyId > 0)
                //    {
                //        CombatStat stat = default(CombatStat);
                //        stat.objectId = enemyId;
                //        GameMain.spaceSector.KillEnemyFinal(enemyId, ref stat);
                //    }
                //    hive.units.Remove(i);
                //}
                //int len2 = hive.forms[0].units.Length;
                //int begin2 = len2 / slice * (time % slice);
                //int end2 = len2 / slice * (time % slice + 1);
                //if(end2 > len2)
                //    end2 = len2;
                //if (time % slice == slice - 1)
                //    end2 = len2;
                //if (hive.forms != null && hive.forms.Length > 2 && hive.forms[0] != null && hive.forms[1] != null && hive.forms[2] != null)
                //{
                //    for (int i = begin2; i < end2; i++)
                //    {
                //        if (hive.forms[0].units[i] == 1)
                //        {
                //            hive.forms[0].RemoveUnit(i);
                //        }
                //        if (hive.forms[1].units.Length > i)
                //        {
                //            if (hive.forms[1].units[i] == 1)
                //                hive.forms[1].RemoveUnit(i);
                //        }
                //        if (hive.forms[2].units.Length > i)
                //        {
                //            if (hive.forms[2].units[i] == 1)
                //                hive.forms[2].RemoveUnit(i);
                //        }
                //    }
                //}
            }
            if (hive.evolve.level > oriLevel)
            {
                hive.evolve.level--;
                hive.evolve.expf = 0;
                hive.evolve.expp = 0;
            }
            if (time <= 0)
            {
                if (hive.evolve.level > oriLevel)
                {
                    hive.evolve.level = oriLevel;
                }
                state = EAssaultHiveState.Remove;
            }
        }
        public void LogicTickRemove()
        {

        }


        void QuickBuild(int speedFactor)
        {
            if (hive == null)
                return;
            //if (GameMain.instance.timei % 4 != listIndex) // 多个巢穴，间隔进行快速建造
            //    return;
            //if (MP.clientBlocker)
            //    return;
            ref AnimData[] ptr2 = ref hive.sector.enemyAnimPool;
            if (hive.realized)
            {
                int cursor2 = hive.cores.cursor;
                int cursor8 = hive.builders.cursor;
                EnemyBuilderComponent[] buffer8 = hive.builders.buffer;
                DFSCoreComponent[] buffer2 = hive.cores.buffer;
                while (speedFactor > 0)
                {
                    speedFactor--;
                    for (int i = 1; i < cursor8; i++)
                    {
                        ref EnemyBuilderComponent ptr4 = ref buffer8[i];
                        if (ptr4.id == i)
                        {
                            int enemyId = ptr4.enemyId;
                            ptr4.energy = ptr4.maxEnergy;
                            ptr4.matter = ptr4.maxMatter;
                            ptr4.LogicTick();
                            if (ptr4.state >= 3)
                            {
                                ptr4.BuildLogic_Space(hive, buffer8, hive.pbuilders);
                            }
                            if (speedFactor == 0)
                                ptr4.RefreshAnimation_Space(hive.pbuilders, ref ptr2[enemyId]);
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        void QuickAssemble()
        {
            //if (MP.clientBlocker)
            //    return;
            EnemyFormation enemyFormation = hive.forms[0];
            if (enemyFormation.vacancyCount > 0 && enemyFormation.unitCount < assembleNum)
            {
                int num5 = enemyFormation.AddUnit();
                if (hive.isLocal && num5 > 0)
                {
                    hive.InitiateUnitDeferred(0, num5,  new UnityEngine.Vector3(0,0,0), new UnityEngine.Quaternion(0, 0, 0, 0), new UnityEngine.Vector3(0,0,0), 0);
                }
            }
        }

        void LaunchAssault()
        {
            if (MP.clientBlocker)
                return;
            if (hive != null)
            {
                hive.hatredAstros.Sort();
                ref HatredTarget ptr = ref hive.hatredAstros.max;
                bool flag2 = false;
                int targetAstroId = 0;
                Vector3 tarPos = Vector3.zero;
                Vector3 maxHatredPos = Vector3.zero;
                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0:
                            ptr = ref hive.hatredAstros.max;
                            break;
                        case 1:
                            ptr = ref hive.hatredAstros.h1;
                            break;
                        case 2:
                            ptr = ref hive.hatredAstros.h2;
                            break;
                        case 3:
                            ptr = ref hive.hatredAstros.h3;
                            break;
                        case 4:
                            ptr = ref hive.hatredAstros.h4;
                            break;
                        case 5:
                            ptr = ref hive.hatredAstros.h5;
                            break;
                        case 6:
                            ptr = ref hive.hatredAstros.h6;
                            break;
                        case 7:
                            ptr = ref hive.hatredAstros.min;
                            break;
                    }
                    if (!ptr.isNull)
                    {
                        int objectId = ptr.objectId;
                        PlanetData planetData = hive.sector.galaxy.PlanetById(objectId);
                        if (planetData != null && planetData.type != EPlanetType.Gas)
                        {
                            PlanetFactory factory = planetData.factory;
                            if (factory != null)
                            {
                                PowerSystem powerSystem = factory.powerSystem;
                                int consumerCursor = powerSystem.consumerCursor;
                                int nodeCursor = powerSystem.nodeCursor;
                                PowerConsumerComponent[] consumerPool = powerSystem.consumerPool;
                                PowerNodeComponent[] nodePool = powerSystem.nodePool;
                                EntityData[] entityPool = factory.entityPool;
                                TurretComponent[] buffer = factory.defenseSystem.turrets.buffer;
                                double num5 = 0.0;
                                Vector3 vector = Vector3.zero;
                                if (hive._assaultPosByQuadrant == null)
                                {
                                    hive._assaultPosByQuadrant = new Vector3[8];
                                }
                                else
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        hive._assaultPosByQuadrant[j] = Vector3.zero;
                                    }
                                }
                                bool flag3 = false;
                                for (int k = 1; k < consumerCursor; k++)
                                {
                                    ref PowerConsumerComponent ptr2 = ref consumerPool[k];
                                    if (ptr2.id == k)
                                    {
                                        double num6 = 0.01;
                                        int networkId = ptr2.networkId;
                                        PowerNetwork powerNetwork = powerSystem.netPool[networkId];
                                        ref Vector3 ptr3 = ref ptr2.plugPos;
                                        if (powerNetwork != null)
                                        {
                                            long num7 = powerNetwork.energyServed / 4L + powerNetwork.energyCapacity / 80L + (long)((double)ptr2.requiredEnergy * powerNetwork.consumerRatio);
                                            num6 += Math.Sqrt((double)num7 / 500000.0);
                                            int turretId = entityPool[ptr2.entityId].turretId;
                                            if (turretId > 0)
                                            {
                                                ref TurretComponent ptr4 = ref buffer[turretId];
                                                if (ptr4.type == ETurretType.Missile)
                                                {
                                                    num6 *= 10.0;
                                                }
                                                else if (ptr4.type == ETurretType.Plasma)
                                                {
                                                    num6 *= 100.0;
                                                }
                                            }
                                            int num8 = ((ptr3.x >= 0f) ? 1 : 0) + ((ptr3.y >= 0f) ? 2 : 0) + ((ptr3.z >= 0f) ? 4 : 0);
                                            hive._assaultPosByQuadrant[num8] += ptr3 * (float)num6;
                                        }
                                        if (num6 > num5)
                                        {
                                            num5 = num6;
                                            vector = ptr3;
                                            flag3 = true;
                                        }
                                    }
                                }
                                for (int l = 1; l < nodeCursor; l++)
                                {
                                    ref PowerNodeComponent ptr5 = ref nodePool[l];
                                    if (ptr5.id == l)
                                    {
                                        double num9 = 0.01;
                                        int networkId2 = ptr5.networkId;
                                        PowerNetwork powerNetwork2 = powerSystem.netPool[networkId2];
                                        ref Vector3 ptr6 = ref ptr5.powerPoint;
                                        if (powerNetwork2 != null)
                                        {
                                            int powerGenId = entityPool[ptr5.entityId].powerGenId;
                                            long num10 = (powerGenId > 0) ? powerSystem.genPool[powerGenId].generateCurrentTick : 0L;
                                            long num11 = (powerNetwork2.energyServed / 4L + powerNetwork2.energyCapacity / 80L + (long)(ptr5.idleEnergyPerTick / 2) + num10 / 20L) / 2L;
                                            num9 += Math.Sqrt((double)num11 / 500000.0);
                                            int num12 = ((ptr6.x >= 0f) ? 1 : 0) + ((ptr6.y >= 0f) ? 2 : 0) + ((ptr6.z >= 0f) ? 4 : 0);
                                            hive._assaultPosByQuadrant[num12] += ptr5.powerPoint * (float)num9;
                                        }
                                        if (num9 > num5)
                                        {
                                            num5 = num9;
                                            vector = ptr5.powerPoint;
                                            flag3 = true;
                                        }
                                    }
                                }
                                if (flag3)
                                {
                                    flag2 = true;
                                    targetAstroId = ptr.objectId;
                                    float num13 = 0f;
                                    for (int m = 0; m < 8; m++)
                                    {
                                        float magnitude = hive._assaultPosByQuadrant[m].magnitude;
                                        if (magnitude > num13)
                                        {
                                            num13 = magnitude;
                                            tarPos = hive._assaultPosByQuadrant[m];
                                        }
                                    }
                                    maxHatredPos = vector;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!flag2 && hive.gameData.localPlanet != null && hive.gameData.localPlanet.type != EPlanetType.Gas && hive.gameData.localPlanet.star == hive.starData)
                {
                    flag2 = true;
                    targetAstroId = hive.gameData.localPlanet.astroId;
                    maxHatredPos = (tarPos = hive.sector.skillSystem.playerSkillTargetL);
                }
                if (flag2)
                {
                    int num2 = 5;
                    int num14 = 100 / (num2 * 4 / 5);
                    if (hive.evolve.waves < 3)
                    {
                        num14 = 1;
                    }
                    int num15 = 100 - num14 * (num2);
                    hive.evolve.threat = 0;
                    hive.LaunchLancerAssault(EAggressiveLevel.Normal, tarPos, maxHatredPos, targetAstroId, assaultNum, num14);
                    hive.evolve.threat = 0;
                    hive.evolve.threatshr = 0;
                    hive.evolve.maxThreat = EvolveData.GetSpaceThreatMaxByWaves(hive.evolve.waves, EAggressiveLevel.Normal);
                    hive.lancerAssaultCountBase += hive.GetLancerAssaultCountIncrement(EAggressiveLevel.Normal);
                    return;
                }
            }
        }


        void KillHiveStrcuture(int pbuilderInstId)
        {
            if (MP.clientBlocker)
                return;
            int id = pbuilderInstId;
            CombatStat stat = default(CombatStat);
            stat.objectId = id;
            GameMain.spaceSector.KillEnemyFinal(id, ref stat);
        }

        public static bool CheckIfAllHiveReachedState(EAssaultHiveState state)
        {
            int length = AssaultController.assaultHives.Count;
            for (int i = length-1; i >= 0; i--)
            {
                if (AssaultController.assaultHives[i] != null)
                {
                    EAssaultHiveState cur = AssaultController.assaultHives[i].state;
                    if(cur < state)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public void Import(BinaryReader r)
        {
            listIndex = r.ReadInt32();
            starIndex = r.ReadInt32();
            oriAstroId = r.ReadInt32();
            orbitIndex = r.ReadInt32();
            byAstroIndex = r.ReadInt32();
            strength = r.ReadInt32();
            state = (EAssaultHiveState)r.ReadInt32();
            time = r.ReadInt32();
            timeTillAssault = r.ReadInt32();
            timeTotalInit = r.ReadInt32();
            timeDelayedByRelic = r.ReadInt32();
            level = r.ReadInt32();
            oriLevel = r.ReadInt32();
            assaultNumTotal = r.ReadInt32();
            inhibitPoints = r.ReadInt32();
            inhibitPointsLimit = r.ReadInt32();
            inhibitPointsTotalInit = r.ReadInt32();
            canFullyStopped = r.ReadBoolean();
            isSuper = r.ReadBoolean();
            enemyKilled = r.ReadInt32();
            isCreated = r.ReadBoolean();
            hive = GameMain.data.spaceSector.GetHiveByAstroId(oriAstroId);
        }

        public void Export(BinaryWriter w)
        {
            w.Write(listIndex);
            w.Write(starIndex);
            w.Write(oriAstroId);
            w.Write(orbitIndex);
            w.Write(byAstroIndex);
            w.Write(strength);
            w.Write((int)state);
            w.Write(time);
            w.Write(timeTillAssault);
            w.Write(timeTotalInit);
            w.Write(timeDelayedByRelic);
            w.Write(level);
            w.Write(oriLevel);
            w.Write(assaultNumTotal);
            w.Write(inhibitPoints);
            w.Write(inhibitPointsLimit);
            w.Write(inhibitPointsTotalInit);
            w.Write(canFullyStopped);
            w.Write(isSuper);
            w.Write(enemyKilled);
            w.Write(isCreated);
        }

        public void IntoOtherSave()
        {
            state = EAssaultHiveState.Remove;
        }

    }

    public static class AssaultHiveCreator
    {

        public static void SetStrength(this AssaultHive _this, int strength)
        {
            _this.strength = strength;
        }
    }

    public enum EAssaultHiveState
    {
        Idle = 0,
        Expand = 1,
        Assemble = 2,
        Assault = 3,
        End = 4,
        Remove = 99
    }
}
