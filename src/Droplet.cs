using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Concurrent;
using System.IO;
using HarmonyLib;
using System.Threading;
using static UnityEngine.EventSystems.EventTrigger;

namespace DSP_Battle
{
    public class Droplets
    {
        //public static GameObject mechaDropletObj = null;
        //public static Text mechaDropletAmountText = null;
        public static long energyConsumptionPerLaunch = 1000000000; //发射水滴耗能
        public static long energyConsumptionPerAttack = 10000000; //水滴攻击耗能，不再使用
        public static long energyConsumptionPerTick = 500000; //水滴每帧耗能
        public static int maxWorkingDroplets = 5;
        public static List<int> warpRushCharge = new List<int> { 0, 0, 0, 0, 0 }; // 足够充能时，如果水滴下一个目标距离超过warpRushDistThr，则消耗充能瞬移过去
        public static int warpRushNeed = 180; // 水滴远距离瞬移所需充能时间（帧数）
        public static int warpRushDistThr = 10000; // 水滴下一个目标距离大于此距离才会触发瞬移
        public static int dropletMaxActiveArea = 20000; // 水滴自动启用的最小敌军距离（游戏默认舰队是20000）
        public static int dropletArrayLength = 25;

        //存档内容
        public static Droplet[] dropletPool = new Droplet[dropletArrayLength];
        public static int bonusDamage = 0;
        public static int bonusDamageLimit = 0;
        public static int activeDropletCount = 0;

        public static void InitAll()
        {
            for (int i = 0; i < dropletArrayLength; i++)
            {
                dropletPool[i] = new Droplet(i);
            }
            bonusDamage = 0;
            bonusDamageLimit = Relic.dropletDamageLimitGrowth;
            warpRushCharge = new List<int>();
            for (int i = 0; i < dropletArrayLength; i++)
            {
                warpRushCharge.Add(0);
            }
            InitUI();
        }

        public static void InitUI()
        {
            //if (mechaDropletObj != null) return;

            //GameObject oriDroneObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Mecha Window/drone");
            //GameObject mechaWindowObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Mecha Window");

            //mechaDropletObj = GameObject.Instantiate(oriDroneObj);
            //mechaDropletObj.name = "droplet";
            //mechaDropletObj.transform.SetParent(mechaWindowObj.transform, false);
            //mechaDropletObj.transform.localPosition = new Vector3(285, 144, 0);
            //Transform lineTrans = mechaDropletObj.transform.Find("line");
            //lineTrans.localPosition = new Vector3(-33,-22,0);
            //lineTrans.rotation = Quaternion.Euler(0,0,30);
            //mechaDropletObj.transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Assets/DSPBattle/dropletW");
            ////mechaDropletObj.transform.Find("icon").localPosition = new Vector3(0, 0, 0);
            //mechaDropletAmountText = mechaDropletObj.transform.Find("cnt-text").GetComponent<Text>();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {
            // 计算当前舰队配置中有多少水滴已经填充，然后设置正确的dropletPool中的水滴状态
            CombatModuleComponent spaceModule = GameMain.mainPlayer?.mecha?.spaceCombatModule;
            int moduleNum = 0;
            if(spaceModule != null)
            {
                if (spaceModule.moduleEnabled)
                {
                    if (spaceModule.moduleFleets == null) return;
                    moduleNum = spaceModule.moduleFleets.Length;
                    for (int i = 0; i < moduleNum; i++)
                    {
                        ref ModuleFleet ptr = ref spaceModule.moduleFleets[i];
                        for (int j = 0; j < 3; j++)
                        {
                            if (ptr.protoId != DropletFleetPatchers.fleetConfigId1 && ptr.protoId != DropletFleetPatchers.fleetConfigId2)
                            {
                                dropletPool[i * 3 + j].SetEmpty();
                            }
                            else if (ptr.fighters == null || j >= ptr.fighters.Length || ptr.fighters[j].count <= 0)
                            {
                                dropletPool[i * 3 + j].SetEmpty();
                            }
                            else if (ptr.fighters != null && j < ptr.fighters.Length && ptr.fleetEnabled)
                            {
                                dropletPool[i * 3 + j].SetActive();
                            }
                            else
                            {
                                dropletPool[i * 3 + j].SetEmpty();
                            }
                        }
                    }
                }
            }
            for (int i = moduleNum * 3; i < dropletArrayLength; i++)
            {
                dropletPool[i].SetEmpty();
            }

            //根据渲染视角设置水滴大小
            if (DysonSphere.renderPlace == ERenderPlace.Universe)
            {
                Droplet.maxPosDelta = 5;
            }
            else
            {
                Droplet.maxPosDelta = 100;
            }

            //计算当前机甲能量可支持的最大运行状态的水滴数
            double mechaCurEnergy = GameMain.mainPlayer.mecha.coreEnergy;
            int maxWorkingDropletsNew = (int)((long)mechaCurEnergy / 100000000);

            //每个水滴update
            for (int i = 0; i < dropletArrayLength; i++)
            {
                if (warpRushCharge[i] < warpRushNeed)
                    warpRushCharge[i] += 1;
                if (dropletPool[i].state >= 2 && dropletPool[i].state <= 3)
                {
                    if (maxWorkingDropletsNew > 0)
                    {
                        dropletPool[i].Update(true);
                        maxWorkingDropletsNew--;
                    }
                    else
                    {
                        dropletPool[i].Update(false);
                    }
                }
                else
                {
                    dropletPool[i].Update(true);
                }
                if (time % 120 == i)
                    dropletPool[i].CheckBullet(); //游荡而又没有实体的子弹强行返回机甲
            }

        }

        public static int RefreshDropletNum()
        {
            return 0;
        }

        public static bool TryConsumeMechaEnergy(double energy)
        {
            if (Relic.HaveRelic(1, 4)) energy *= 0.5; // relic1-4 水滴减耗
            if (Relic.HaveRelic(2, 6)) energy *= 0.6; // relic2-6 水滴减耗
            if (GameMain.mainPlayer.mecha.coreEnergy >= energy)
            {
                GameMain.mainPlayer.mecha.coreEnergy -= energy;
                GameMain.mainPlayer.mecha.MarkEnergyChange(13, -energy);
                return true;
            }
            else
                return false;
        }

        public static void ForceConsumeMechaEnergy(double energy)
        {
            if (Relic.HaveRelic(1, 4)) energy *= 0.5; // relic1-4 水滴减耗
            if (Relic.HaveRelic(2, 6)) energy *= 0.6; // relic2-6 水滴减耗
            double curEnergy = GameMain.mainPlayer.mecha.coreEnergy;
            energy = energy < curEnergy ? energy : curEnergy;
            GameMain.mainPlayer.mecha.coreEnergy -= energy;
            GameMain.mainPlayer.mecha.MarkEnergyChange(13, -energy);
        }

        public static void DamageGrow()
        {
            if (bonusDamage >= bonusDamageLimit)
            {
                int inc;
                if (GameMain.mainPlayer.package.TakeItem(9511, 1, out inc) > 0) //拿到了
                {
                    Interlocked.Add(ref bonusDamageLimit, Relic.dropletDamageLimitGrowth);
                }
            }
            if (bonusDamage < bonusDamageLimit)
            {
                Interlocked.Exchange(ref bonusDamage, Math.Min(bonusDamageLimit, bonusDamage + Relic.dropletDamageGrowth));
                int slotNum = 0;
                for (int rnum = 0; rnum < 10; rnum++)
                {
                    if (Relic.HaveRelic(0, rnum))
                        slotNum++;
                }
                if (slotNum >= 8) return;
                UIRelic.relicSlotUIBtns[slotNum].tips.tipText = "遗物描述0-10".Translate() + "\n" + "relicTipText0-10".Translate() + "\n\n<color=#61d8ffb4>" + "当前加成gm".Translate() + "  " + Droplets.bonusDamage + " / " + Droplets.bonusDamageLimit + "</color>";
                if (UIRelic.relicSlotUIBtns[slotNum].tipShowing)
                {
                    UIRelic.relicSlotUIBtns[slotNum].OnPointerExit(null);
                    UIRelic.relicSlotUIBtns[slotNum].OnPointerEnter(null);
                    UIRelic.relicSlotUIBtns[slotNum].enterTime = 1;
                }
                try
                {
                    int width = (int)Math.Log10(bonusDamage) * 12 + 200;
                    Utils.UIItemUp(8035, Relic.dropletDamageGrowth, width, bonusDamage);
                }
                catch (Exception)
                { }
            }
        }

        public static void Export(BinaryWriter w) 
        {
            for (int i = 0; i < dropletArrayLength; i++)
            {
                dropletPool[i].Export(w);
            }
            w.Write(bonusDamage);
            w.Write(bonusDamageLimit);
        }
        public static void Import(BinaryReader r) 
        {
            InitAll();
            if(Configs.versionWhenImporting >= 30220328)
            {
                for (int i = 0; i < dropletArrayLength; i++)
                {
                    dropletPool[i].Import(r);
                }
            }
            if (Configs.versionWhenImporting >= 30221118)
            {
                bonusDamage = r.ReadInt32();
                bonusDamageLimit = r.ReadInt32();
            }
            else
            {
                bonusDamage = 0;
                bonusDamageLimit = Relic.dropletDamageLimitGrowth;
            }
        }
        public static void IntoOtherSave() 
        {
            InitAll();
        }
    }

    public class Droplet
    {
        public static int exceedDis = 3000; //水滴撞击敌舰后保持原速度方向飞行的距离，之后才会瞬间转弯
        public static int bulletCnt = 25;
        public static int maxPosDelta = 100; //会根据是否达开星图设置为100或5，在Droplets里更新。目的是不打开星图时水滴比较细，打开星图后为了能清楚地看到设置得很大

        public int state = -1; //-1空-根本没有水滴，0有水滴-正在机甲中待命，1刚刚从机甲出来-慢，2攻击-飞向目标或在太空中待命，3攻击-已越过目标准备折返，4强制返航,5马上要回到机甲-慢
        public int forceLaunchState = 0; // 通过单击或者右击FleetEntry控制。0未调遣，1强制唤出。0状态会在敌舰过近时（此时无法通过右键FleetEntry主动收回，除非disable那个启用的复选框）自动launch并且消灭该星系全部敌人后回到机甲。1会主动攻击本星系敌人，消灭全部之后会在轨道待命，不会回到机甲。
        int dropletIndex = 0;
        int swarmIndex = -1;
        int[] bulletIds = new int[bulletCnt];
        int targetEnemyId = -1;
        int randSeed = 0; //决定水滴在撞向敌舰时的一个随机的小偏移，使其往返攻击时不是在两个点之间来回飞，而是有一些随机的角度变化，每次find敌人和从2阶段回到1阶段都会改变一次randSeed

        public Droplet(int idx)
        {
            state = -1;
            forceLaunchState = 0;
            dropletIndex = idx;
            swarmIndex = -1;
            bulletIds = new int[bulletCnt];
            for (int i = 0; i < bulletIds.Length; i++)
            {
                bulletIds[i] = 0;
            }
            targetEnemyId = -1;
            randSeed = Utils.RandNext();
        }

        public void SetEmpty()
        {
            state = -1;
            TryRemoveOtherBullets(0);
            swarmIndex = -1;
            targetEnemyId = -1;
        }

        public void SetActive()
        {
            if (state < 0)
            {
                state = 0;
                forceLaunchState = 0;
                swarmIndex = -1;
                bulletIds = new int[bulletCnt];
                for (int i = 0; i < bulletIds.Length; i++)
                {
                    bulletIds[i] = 0;
                }
                targetEnemyId = -1;
                randSeed = Utils.RandNext();
            }
        }

        public void SetStandby()
        {
            if (state >= 0)
            {
                state = 0;
                TryRemoveOtherBullets(0);
                swarmIndex = -1;
                targetEnemyId = -1;
            }
        }

        public bool Launch(int starIndex)
        {
            if (state != 0)
                return false;
            if (swarmIndex == starIndex || state == 0)
            {
                swarmIndex = starIndex;
                if (SearchNextNearestTarget())
                {
                    if (CreateBulltes())
                    {
                        state = 2;//直接创建在太空
                        return true;
                    }
                    else //没创建成功，可能是因为swarm为null
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool ForceLaunch(int starIndex)
        {
            //if(starIndex >= 0)
            //{
            //    swarmIndex = starIndex;
            //    return Launch(starIndex);
            //}
            return false;
        }

        public void Retreat()
        {
            if (!DiscoverSpaceEnemy() && state >= 2)
            {
                state = 4;
            }
        }

        bool CreateBulltes()
        {
            if (swarmIndex < 0) return false;
            DysonSwarm swarm = GetSwarm();
            if (swarm == null) return false;

            VectorLF3 beginUPos = GetIdleUPos();
            VectorLF3 endUPos = beginUPos + (GameMain.localPlanet == null ? (VectorLF3)(GameMain.mainPlayer.uRotation * new VectorLF3(0, 0, 300)) : new VectorLF3(0, 0, 300));
            bool validTarget = false;
            if(CheckOrSearchTarget() || forceLaunchState > 0)
            {
                if (targetEnemyId > 0 && targetEnemyId < GameMain.data.spaceSector.enemyPool.Length)
                {
                    validTarget = GetEnemyUPos(ref endUPos);
                    if (validTarget)
                    {
                        endUPos = (endUPos - beginUPos).normalized * exceedDis * RandExceedDisRatio() + endUPos;
                    }
                }
            }
            if (validTarget)
            {
                Vector3 beginLPos = new Vector3(0, 0, 0);
                float newMaxt = (float)((endUPos - beginUPos).magnitude / Configs.dropletSpd * 200);
                if (newMaxt < 0.036f) newMaxt = 0.036f;
                for (int i = 0; i < bulletIds.Length; i++)
                {
                    if (false) //无敌人的悬停状态只渲染一个
                    {
                        beginUPos = new VectorLF3(0, 0, 0);
                        endUPos = new VectorLF3(1, 2, 3);
                        beginLPos = new Vector3(9999, 9998, 9997);
                    }
                    bulletIds[i] = swarm.AddBullet(new SailBullet
                    {
                        maxt = newMaxt,
                        lBegin = beginLPos,
                        uEndVel = new Vector3(0, 0, 0),
                        uBegin = beginUPos,
                        uEnd = endUPos
                    }, 1);
                    swarm.bulletPool[bulletIds[i]].state = 0;
                }
            }
            return validTarget;
        }

        /// <summary>
        /// 搜索最近的敌军单位（火种除外）
        /// </summary>
        /// <returns></returns>
        bool SearchNextNearestTarget(bool nearMecha = false)
        {
            SpaceSector sector = GameMain.data.spaceSector;
            EnemyData[] enemyPool = sector.enemyPool;
            int enemyCursor = sector.enemyCursor;
            EnemyDFHiveSystem[] dfHivesByAstro = sector.dfHivesByAstro;
            Vector3 currentUPos = GetCurrentUPos();
            if (nearMecha)
                currentUPos = GameMain.mainPlayer.uPosition;
            float defaultCheckDistance2 = 60000f * 60000f;
            if (forceLaunchState > 0)// 手动launch的水滴寻敌距离是无限的，否则只有以水滴本身为中心的1.5AU半径
            {
                defaultCheckDistance2 = float.MaxValue;
            }
            int foundCount = 0;
            int[] targetIds = { -1, -1, -1 };
            float[] targetDistances = { defaultCheckDistance2, defaultCheckDistance2, defaultCheckDistance2 };
            int isUnitTargetId = -1;
            float isUnitTargetDistance = defaultCheckDistance2;
            int poolLen = targetIds.Length;
            float checkDistance2 = targetDistances[poolLen - 1];
            VectorLF3 zero = VectorLF3.zero;
            for (int m = 0; m < enemyCursor; m++)
            {
                ref EnemyData ptr = ref enemyPool[m];
                if (ptr.id != 0 && (ptr.dfRelayId == 0 || (bool)GameMain.data.mainPlayer?.mecha?.spaceCombatModule?.attackRelay))
                {
                    if (ptr.dfTinderId != 0) // 水滴不攻击火种
                    {
                        continue;
                    }
                    else
                    {
                        if (GameMain.localStar == null)
                            continue;
                        EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                        if (enemyDFHiveSystem == null)
                            continue;
                        else if (enemyDFHiveSystem.starData.index != swarmIndex)
                            continue;
                    }
                    sector.TransformFromAstro_ref(ptr.astroId, out zero, ref ptr.pos); 
                    checkDistance2 = targetDistances[poolLen - 1];
                    if (ptr.unitId > 0)
                        checkDistance2 = isUnitTargetDistance;
                    float x = (float)(zero.x - currentUPos.x);
                    float x2 = x * x;
                    if (x2 <= checkDistance2)
                    {
                        float y = (float)(zero.y - currentUPos.y);
                        float y2 = y * y;
                        if (y2 <= checkDistance2)
                        {
                            float z = (float)(zero.z - currentUPos.z);
                            float z2 = z * z;
                            if (z2 <= checkDistance2)
                            {
                                //bool isUnit = ptr.unitId > 0;
                                float distance2 = x2 + y2 + z2;
                                if (distance2 <= checkDistance2) 
                                {
                                    if (ptr.unitId > 0)
                                    {
                                        isUnitTargetId = ptr.id;
                                        isUnitTargetDistance = distance2;
                                    }
                                    else
                                    {
                                        for (int i = poolLen - 2; i >= 0; i--)
                                        {
                                            if (targetDistances[i] < distance2)
                                            {
                                                targetIds[i + 1] = ptr.id;
                                                targetDistances[i + 1] = distance2;
                                            }
                                            else
                                            {
                                                targetIds[i + 1] = targetIds[i];
                                                targetDistances[i + 1] = targetDistances[i];
                                                if (i == 0)
                                                {
                                                    targetIds[i] = ptr.id;
                                                    targetDistances[i] = distance2;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < poolLen; i++)
            {
                if (targetIds[i] > 0)
                    foundCount++;
            }
            if (isUnitTargetId > 0 && Utils.RandDouble() < 0.8) // 敌舰优先
                targetEnemyId = isUnitTargetId;
            else if (foundCount > 0)
                targetEnemyId = targetIds[Utils.RandInt(0, foundCount)];
            else
                targetEnemyId = isUnitTargetId;
            return foundCount > 0 || isUnitTargetId > 0;
        }
        

        /// <summary>
        /// 判定当前目标是否存在且合法，如果不是，寻找并锁定下一个最近的合法目标，如果找不到，返回false
        /// </summary>
        /// <returns></returns>
        bool CheckOrSearchTarget()
        {
            if (targetEnemyId > 0)
            {
                SpaceSector sector = GameMain.data.spaceSector;
                if(sector.enemyPool.Length > targetEnemyId && sector.enemyCursor > targetEnemyId)
                {
                    EnemyData ptr = sector.enemyPool[targetEnemyId];
                    if (ptr.id > 0 && ptr.id == targetEnemyId)
                    {
                        EnemyDFHiveSystem[] dfHivesByAstro = sector.dfHivesByAstro;
                        int hiveAstroId = ptr.originAstroId - 1000000;
                        if (hiveAstroId >= 0 && hiveAstroId < dfHivesByAstro.Length)
                        {
                            EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                            if (enemyDFHiveSystem.starData?.index == swarmIndex)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return SearchNextNearestTarget();
        }

        public bool DiscoverSpaceEnemy()
        {
            VectorLF3 centerPosU = GameMain.mainPlayer.uPosition;
            bool checkAssaultLancerOnly = GameMain.localPlanet != null;
            SpaceSector sector = GameMain.data.spaceSector;
            EnemyData[] enemyPool = sector.enemyPool;
            int enemyCursor = sector.enemyCursor;
            EnemyDFHiveSystem[] dfHivesByAstro = sector.dfHivesByAstro;
            float sensorRange = 40000f * 40000f;
            if(LDB.fleets.Select(3) != null)
            {
                var fleetProto = LDB.fleets.Select(3);
                sensorRange = (float)Math.Max(sensorRange, (double)(fleetProto.prefabDesc?.fleetMaxActiveArea * fleetProto.prefabDesc?.fleetMaxActiveArea * 4));
            }
            VectorLF3 zero = VectorLF3.zero;
            for (int i = 0; i < enemyCursor; i++)
            {
                ref EnemyData ptr = ref enemyPool[i];
                if (ptr.id != 0 && ptr.dfRelayId == 0)
                {
                    if (checkAssaultLancerOnly)
                    {
                        bool flag = false;
                        if (ptr.unitId > 0 && ptr.protoId == 8113)
                        {
                            EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                            if (enemyDFHiveSystem != null)
                            {
                                ref EnemyUnitComponent ptr2 = ref enemyDFHiveSystem.units.buffer[ptr.unitId];
                                if (ptr2.id == ptr.unitId && ptr2.assaults.count > 0)
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (!flag)
                        {
                            continue;
                        }
                    }
                    if (ptr.dfTinderId == 0)
                    {
                        if (GameMain.localStar == null)
                        {
                            break;
                        }
                        EnemyDFHiveSystem enemyDFHiveSystem2 = dfHivesByAstro[ptr.originAstroId - 1000000];
                        if (enemyDFHiveSystem2 != null && enemyDFHiveSystem2.starData != GameMain.localStar)
                        {
                            continue;
                        }
                    }
                    sector.TransformFromAstro_ref(ptr.astroId, out zero, ref ptr.pos);
                    float num2 = (float)(zero.x - (double)centerPosU.x);
                    float num3 = num2 * num2;
                    if (num3 <= sensorRange)
                    {
                        float num4 = (float)(zero.y - (double)centerPosU.y);
                        float num5 = num4 * num4;
                        if (num5 <= sensorRange)
                        {
                            float num6 = (float)(zero.z - (double)centerPosU.z);
                            float num7 = num6 * num6;
                            if (num7 <= sensorRange && num3 + num5 + num7 < sensorRange)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void Update(bool working = true)
        {
            if (state < 0) return;
            if (swarmIndex < 0)
            {
                swarmIndex = GameMain.localStar != null ? GameMain.localStar.index : -1;
            }
            if(GameMain.localStar == null)
            {
                SetStandby();
                return;
            }
            else if(GameMain.localStar.index != swarmIndex)
            {
                SetStandby();
                return;
            }
            DysonSwarm swarm = GetSwarm();
            if (swarm == null)
            {
                state = 0;
                return;
            }
            int playerSwarmIndex = GameMain.localStar != null ? GameMain.localStar.index : -1;
            //Utils.Log($"playerindex {playerSwarmIndex}/swarm{swarmIndex}, and state = {state}, forceL={forceLaunchState}, discover?{DiscoverSpaceEnemy()}", 60);
            if (state == 0 && (DiscoverSpaceEnemy() || forceLaunchState > 0) && playerSwarmIndex >= 0)
            {
                Launch(playerSwarmIndex);
            }
            if (swarm.bulletPool.Length <= bulletIds[0])
            {
                state = 0;
                return;
            }

            if (GameMain.localStar == null || GameMain.localStar.index != swarmIndex)//机甲所在星系和水滴不一样，让水滴返回
            {
                SetStandby();
                return;
            }

            bool validTargetEnemy = false;
            if (state > 0)
            {
                validTargetEnemy = CheckOrSearchTarget();
            }
            if (state >= 2 && state <= 3 && working) //只有不是飞出、返航过程，才会消耗能量
            {
                if(validTargetEnemy)
                    Droplets.ForceConsumeMechaEnergy(Droplets.energyConsumptionPerTick);
                else
                    Droplets.ForceConsumeMechaEnergy(Droplets.energyConsumptionPerTick / 100);
            }

            if (state >= 2 && state <= 3 && !working) //如果因为机甲能量水平不够，水滴会停在原地，但是如果战斗状态结束了，那么水滴会无视能量限制继续正常Update（正常Update会在战斗结束后让水滴立刻进入4阶段回机甲）
            {
                float tickT = 0.016666668f;
                swarm.bulletPool[bulletIds[0]].t -= tickT;
                VectorLF3 lastUPos = GetCurrentUPos();
                HoverHere(lastUPos);
                return;
            }

            //if (state == 1) //刚起飞
            //{
            //    float lastT = swarm.bulletPool[bulletIds[0]].t;
            //    float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
            //    //如果原目标不存在了，尝试寻找新目标，如果找不到目标，设定为极接近机甲的回到机甲状态（5）
            //    if (!EnemyShips.ships.ContainsKey(targetShipIndex) || EnemyShips.ships[targetShipIndex].state != EnemyShip.State.active)
            //    {
            //        if (!FindNextTarget())
            //        {
            //            state = 5;
            //            //刷新当前位置，设t=0，重新计算终点和maxt
            //            VectorLF3 newBegin = GetCurrentUPos();
            //            VectorLF3 newEnd = GameMain.mainPlayer.uPosition;
            //            if (GameMain.localPlanet != null)
            //            {
            //                int planetId = GameMain.localPlanet.id;
            //                AstroData[] astroPoses = GameMain.galaxy.astrosData;
            //                newEnd = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
            //            }
            //            RetargetAllBullet(newBegin, newEnd, 1, 0, 0, Configs.dropletSpd / 200.0);
            //            //TryRemoveOtherBullets();
            //        }
            //    }
            //    else if (lastMaxt - lastT <= 0.035f) //进入太空索敌阶段
            //    {
            //        state = 2; //不需要在此刷新当前位置，因为state2每帧开头都刷新
            //    }
            //}
            if (state == 2 || state == 4) //追敌中或强制返航途中
            {
                float lastT = swarm.bulletPool[bulletIds[0]].t;
                float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
                //如果原目标不存在了（或被强制召回（前提是机甲附近没有目标）的状态state==4），回机甲附近，然后收回太空中的水滴
                if (!validTargetEnemy || state == 4)
                {
                    if (state != 4)
                        state = 2;
                    VectorLF3 newBegin = GetCurrentUPos();
                    VectorLF3 newEnd = GetIdleUPos();
                    if ((newEnd - newBegin).magnitude <= Configs.dropletSpd * 0.05f || lastMaxt - lastT <= 0.05f)
                    {
                        SetStandby();
                    }
                    else
                    {
                        RetargetAllBullet(newBegin, newEnd, bulletIds.Length, maxPosDelta, maxPosDelta, Configs.dropletSpd);
                    }

                }
                else //目标存在
                {
                    VectorLF3 enemyUPos = VectorLF3.zero;
                    GetEnemyUPos(ref enemyUPos);
                    VectorLF3 newBegin = GetCurrentUPos();
                    VectorLF3 newEnd = (enemyUPos - newBegin).normalized * exceedDis + enemyUPos;
                    float newMaxt = (float)((newEnd - newBegin).magnitude / Configs.dropletSpd);
                    double realSpd = Configs.dropletSpd;
                    if (Rank.rank >= 8 || Configs.developerMode) // 水滴快速接近
                    {
                        double warpRushDist = (enemyUPos - newBegin).magnitude - exceedDis;
                        if (warpRushDist > Droplets.warpRushDistThr && Droplets.warpRushCharge[dropletIndex] >= Droplets.warpRushNeed)
                        {
                            Droplets.warpRushCharge[dropletIndex] = -5;
                            realSpd = 12 * warpRushDist;
                        }
                        else if (Droplets.warpRushCharge[dropletIndex] < 0)
                        {
                            int phase = -Droplets.warpRushCharge[dropletIndex];
                            realSpd = 60 / phase * warpRushDist;
                        }
                    }
                    //判断击中，如果距离过近
                    if ((newBegin - enemyUPos).magnitude < 500 || newMaxt <= exceedDis * 1.0 / Configs.dropletSpd + 0.035f)
                    {
                        int damage = Configs.dropletAtk;
                        if (Rank.rank >= 10) damage = 5 * Configs.dropletAtk;
                        if (Relic.HaveRelic(0, 10))
                            damage = damage + (Relic.BonusDamage(Droplets.bonusDamage, 1) - Droplets.bonusDamage);
                        Attack(damage); 
                        newEnd = (enemyUPos - newBegin).normalized * exceedDis * RandExceedDisRatio() + enemyUPos;
                        state = 3; //击中后继续冲过目标，准备转向的阶段
                    }
                    RetargetAllBullet(newBegin, newEnd, bulletIds.Length, maxPosDelta, maxPosDelta, realSpd);
                }
            }
            else if (state == 3) //刚刚击中敌船，正准备转向
            {
                float lastT = swarm.bulletPool[bulletIds[0]].t;
                float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
                VectorLF3 newBegin = GetCurrentUPos();
                if (lastMaxt - lastT <= 0.035) //到头了，执行转向/重新索敌
                {
                    if (CheckOrSearchTarget())
                    {
                        state = 2; //回到追敌攻击状态
                        VectorLF3 uEnd = swarm.bulletPool[bulletIds[0]].uEnd;
                        VectorLF3 enemyUPos = uEnd;
                        GetEnemyUPos(ref enemyUPos);
                        uEnd = (enemyUPos - newBegin).normalized * exceedDis + enemyUPos;
                        RetargetAllBullet(newBegin, uEnd, bulletIds.Length, maxPosDelta, maxPosDelta, Configs.dropletSpd);
                    }
                    else
                    {
                        state = 2;
                        VectorLF3 newEnd = GetIdleUPos();
                        RetargetAllBullet(newBegin, newEnd, bulletIds.Length, maxPosDelta, maxPosDelta, Configs.dropletSpd);
                    }
                }
            }
        }


        int Attack(int damage)
        {
            SkillTarget target;
            SkillTarget caster;
            SpaceSector sector = GameMain.data.spaceSector;
            if (targetEnemyId > 0 && targetEnemyId < sector.enemyPool.Length)
            {
                ref EnemyData ptr = ref sector.enemyPool[targetEnemyId];
                target.id = ptr.id;
                target.astroId = ptr.originAstroId;
                target.type = ETargetType.Enemy;
                caster.id = 1;
                caster.type = ETargetType.Player;
                caster.astroId = 0;
                sector.skillSystem.DamageObject(damage, 1,ref target,ref caster);
            }
            return damage;
        }

        VectorLF3 GetCurrentUPos()
        {
            VectorLF3 uPos000 = GetIdleUPos();
            DysonSwarm swarm = GetSwarm();

            if (state <=0 || swarm == null) return uPos000;
            if (swarm.bulletPool.Length <= bulletIds[0])
            {
                state = 0;
                return uPos000;
            }


            float lastT = swarm.bulletPool[bulletIds[0]].t;
            float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
            VectorLF3 uBegin = swarm.bulletPool[bulletIds[0]].uBegin;
            VectorLF3 uEnd = swarm.bulletPool[bulletIds[0]].uEnd;
            uBegin = (uEnd - uBegin) * lastT / lastMaxt + uBegin;
            return uBegin;
        }

        /// <summary>
        /// 返回水滴在找不到目标，但是被要求在太空中待命时的悬停位置
        /// </summary>
        /// <returns></returns>
        VectorLF3 GetIdleUPos()
        {
            VectorLF3 endPos = new VectorLF3(0, 0, 0);

            int group = dropletIndex / 3 % 8;
            int secondary = dropletIndex % 3;
            if (GameMain.localPlanet != null)
            {
                //int planetId = GameMain.localPlanet.id;
                //AstroData[] astroPoses = GameMain.galaxy.astrosData;
                float distance2Center = GameMain.localPlanet.realRadius + 100;
                endPos = GameMain.localPlanet.uPosition + new VectorLF3((group & 4) > 0 ? 1 : -1, (group & 2) > 0 ? 1 : -1, (group & 1) > 0 ? 1 : -1) * distance2Center;
                endPos += new VectorLF3((secondary - 1) * 50, 0, 0);
            }
            else
            {
                float distance2Center = 0;
                endPos = GameMain.mainPlayer.uPosition + new VectorLF3((group & 4) > 0 ? 1 : -1, (group & 2) > 0 ? 1 : -1, (group & 1) > 0 ? 1 : -1) * distance2Center;
                endPos += (VectorLF3)(GameMain.mainPlayer.uRotation * new VectorLF3((secondary - 1) * 0, 0, 0));
            }

            return endPos;
        }

        bool GetEnemyUPos(ref VectorLF3 enemyUPos, bool addRandomPos = true)
        {
            SpaceSector sector = GameMain.data.spaceSector;
            if (targetEnemyId > 0 && targetEnemyId < sector.enemyPool.Length && targetEnemyId < sector.enemyCursor)
            {
                ref EnemyData ptr = ref sector.enemyPool[targetEnemyId];
                SkillTarget target = default(SkillTarget);
                target.id = ptr.id;
                target.type = ETargetType.Enemy;
                target.astroId = ptr.originAstroId;
                Vector3 vec;
                sector.skillSystem.GetObjectUPositionAndVelocity(ref target, out enemyUPos, out vec);
                enemyUPos += (VectorLF3)vec * 0.016666667f;
                if(addRandomPos)
                    enemyUPos += Utils.RandPosDelta(ref randSeed) * 200;
                return true;
            }
            return false;
        }

        public double RandExceedDisRatio()
        {
            return 1 + 0.4 * (Utils.RandDouble() - 0.5);
        }

        /// <summary>
        /// 让水滴悬停在此处，极大减小拖尾长度
        /// </summary>
        void HoverHere(VectorLF3 upos)
        {
            DysonSwarm swarm = GetSwarm();
            if (swarm != null)
            {
                for (int i = 0; i < bulletIds.Length; i++)
                {
                    if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                    swarm.bulletPool[bulletIds[i]].uBegin = upos;
                    swarm.bulletPool[bulletIds[i]].t = 0;
                }
            }
        }

        void RetargetAllBullet(VectorLF3 newUBegin, VectorLF3 newUEnd, int bulletNum, int randomBeginRatio, int randomEndRatio, double speed) //如果只更新uEnd，不更新maxt等其他信息，则不要使用这个函数
        {
            if (swarmIndex < 0) return;
            DysonSwarm swarm = GetSwarm();

            if (swarm == null) return;
            if (swarm.bulletPool.Length <= bulletIds[0])
            {
                state = 0;
                return;
            }

            float originalT = swarm.bulletPool[bulletIds[0]].t;
            int tailT = speed > Droplets.warpRushDistThr * 5 ? 1 : 4;
            float newBeginT = originalT < 0.0166667f * tailT ? originalT : 0.0166667f * tailT;

            VectorLF3 uBegin = newUBegin + (newUBegin-newUEnd).normalized * speed * newBeginT;
            VectorLF3 uEnd = newUEnd;

            float newMaxt = 1;
            if (speed > 0)
                newMaxt = (float)((uEnd - newUBegin).magnitude / speed);
            if (newMaxt <= 0.017f)
                newMaxt = 0.017f;

            if (bulletNum > bulletIds.Length)
                bulletNum = bulletIds.Length;


            swarm.bulletPool[bulletIds[0]].t = newBeginT;
            swarm.bulletPool[bulletIds[0]].maxt = newMaxt + newBeginT;
            swarm.bulletPool[bulletIds[0]].uBegin = uBegin;
            swarm.bulletPool[bulletIds[0]].uEnd = uEnd;

            for (int i = 1; i < bulletNum; i++)
            {
                if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                swarm.bulletPool[bulletIds[i]].t = newBeginT;//0.0166667f * 4;
                swarm.bulletPool[bulletIds[i]].maxt = newMaxt + newBeginT;
                swarm.bulletPool[bulletIds[i]].uBegin = newUBegin + Utils.RandPosDelta(ref randSeed) * randomBeginRatio; //uBegin + Utils.RandPosDelta(randSeed + i + 100) * randomBeginRatio;
                swarm.bulletPool[bulletIds[i]].uEnd = uEnd + Utils.RandPosDelta(ref randSeed) * randomEndRatio;
            }

        }

        public void TryRemoveOtherBullets(int beginIndex=1)
        {
            state = state >= 0 ? 0 : state;
            if (swarmIndex < 0) return;
            DysonSwarm swarm = GetSwarm();

            if (swarm == null) return;
            for (int i = beginIndex; i < bulletIds.Length; i++)
            {
                if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                swarm.bulletPool[bulletIds[i]].uBegin = new VectorLF3(0, 0, 0);
                swarm.bulletPool[bulletIds[i]].uEnd = new VectorLF3(1, 2, 3);
                swarm.RemoveBullet(bulletIds[i]);
            }
        }

        public void CheckBullet()
        {
            if (state <= 0) return;
            if (swarmIndex < 0)
            {
                state = 0;
                return;
            }
            DysonSwarm swarm = GetSwarm();

            if (swarm == null) return;
            if (state < 0) return;
            for (int i = 0; i < bulletIds.Length; i++)
            {
                if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                if (swarm.bulletPool[bulletIds[i]].id > 0) return;
            }
            state = 0;
        }

        DysonSwarm GetSwarm()
        {
            if (swarmIndex >= 0 && swarmIndex < GameMain.galaxy.starCount)
            {
                return RendererSphere.dropletSpheres[swarmIndex]?.swarm;
            }
            else
            {
                return null;
            }
        }

        public void Export(BinaryWriter w)
        {
            w.Write(state);
            w.Write(forceLaunchState);
            w.Write(swarmIndex);
            for (int i = 0; i < 25; i++)
            {
                w.Write(bulletIds[i]);
            }
            w.Write(targetEnemyId);
            w.Write(randSeed);
        }
        public void Import(BinaryReader r)
        {
            state = r.ReadInt32();
            forceLaunchState = r.ReadInt32();
            swarmIndex = r.ReadInt32();
            int savedCnt = 25;
            if (Configs.versionWhenImporting >= 30220417)
            {
                savedCnt = bulletIds.Length;
            }
            else if (state > 0 && bulletIds.Length > 25) //版本更迭后的存档子弹数量存储不足，强制水滴回到机甲重新创建足够数量的子弹
            {
                state = 0;
            }
            for (int i = 0; i < savedCnt; i++)
            {
                bulletIds[i] = r.ReadInt32();
            }
            targetEnemyId = r.ReadInt32();
            randSeed = r.ReadInt32();
        }
    }
    public enum EDropletState
    {
        Empty = -2,
        Inactive = -1,
        Standby = 0,
        OrbitIdle = 1,
        FollowIdle = 2,
    }
}
