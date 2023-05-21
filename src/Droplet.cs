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
        public static GameObject mechaDropletObj = null;
        public static Text mechaDropletAmountText = null;
        public static long energyConsumptionPerLaunch = 1000000000; //发射水滴耗能
        public static long energyConsumptionPerAttack = 10000000; //水滴攻击耗能，不再使用
        public static long energyConsumptionPerTick = 500000; //水滴每帧耗能
        public const int dropletsUpperLimit = 20; // 游戏支持的最大水滴数量
        public static int maxWorkingDroplets = dropletsUpperLimit; // 受限于机甲当前能量，最大可支持的同时工作的水滴数量
        public static List<int> warpRushCharge = new List<int> { 0, 0, 0, 0, 0 }; // 足够充能时，如果水滴下一个目标距离超过warpRushDistThr，则消耗充能瞬移过去
        public static int warpRushNeed = 180; // 水滴远距离瞬移所需充能时间（帧数）
        public static int warpRushDistThr = 10000; // 水滴下一个目标距离大于此距离才会触发瞬移

        //存档内容
        public static Droplet[] dropletPool = new Droplet[21];
        public static int maxDroplet = 2;
        public static int bonusDamage = 0;
        public static int bonusDamageLimit = 0;

        public static void InitAll()
        {
            maxDroplet = 2;
            for (int i = 0; i < dropletsUpperLimit; i++)
            {
                dropletPool[i] = new Droplet(i);
            }
            bonusDamage = 0;
            bonusDamageLimit = Relic.dropletDamageLimitGrowth;
            warpRushCharge = new List<int>();
            for (int i = 0; i < dropletsUpperLimit; i++)
            {
                warpRushCharge.Add(0);
            }
            InitUI();
        }

        public static void InitUI()
        {
            if (mechaDropletObj != null) return;

            GameObject oriDroneObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Mecha Window/drone");
            GameObject mechaWindowObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Mecha Window");

            mechaDropletObj = GameObject.Instantiate(oriDroneObj);
            mechaDropletObj.name = "droplet";
            mechaDropletObj.transform.SetParent(mechaWindowObj.transform, false);
            mechaDropletObj.transform.localPosition = new Vector3(285, 144, 0);
            Transform lineTrans = mechaDropletObj.transform.Find("line");
            lineTrans.localPosition = new Vector3(-33,-22,0);
            lineTrans.rotation = Quaternion.Euler(0,0,30);
            mechaDropletObj.transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Assets/DSPBattle/dropletW");
            //mechaDropletObj.transform.Find("icon").localPosition = new Vector3(0, 0, 0);
            mechaDropletAmountText = mechaDropletObj.transform.Find("cnt-text").GetComponent<Text>();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {
            //根据科技等级确定上限
            if (GameMain.history.techStates.ContainsKey(4929) && GameMain.history.techStates[4929].unlocked) { maxDroplet = 5; }
            else if (GameMain.history.techStates.ContainsKey(4928) && GameMain.history.techStates[4928].unlocked) { maxDroplet = 4; }
            else if (GameMain.history.techStates.ContainsKey(4927) && GameMain.history.techStates[4927].unlocked) { maxDroplet = 3; }
            else { maxDroplet = 2; }
            if (Relic.HaveRelic(4, 1)) 
                maxDroplet += 15;
            if(DysonSphere.renderPlace == ERenderPlace.Universe)
            {
                Droplet.maxPosDelta = 5;
            }
            else
            {
                Droplet.maxPosDelta = 100;
            }

            if (maxDroplet > dropletPool.Length) maxDroplet = dropletPool.Length;

            int loadedDropletCnt = RefreshDropletNum();
            if(loadedDropletCnt < maxDroplet) //如果当前可控水滴的数量还没满，则从机甲物品栏里面尝试拿一个水滴放入pool，使其可用
            {
                int inc = 0;
                if(GameMain.mainPlayer.package.TakeItem(9511, 1, out inc) > 0) //拿到了
                {
                    dropletPool[loadedDropletCnt].state = 0;
                }
            }

            //如果在战斗状态，且机甲在相同星系，每秒发射一个水滴
            long energyCons = energyConsumptionPerLaunch;
            if (Relic.HaveRelic(1, 4)) energyCons = (long)(energyCons * 0.5); // relic1-4 水滴减耗
            if (Relic.HaveRelic(2, 6)) energyCons = (long)(energyCons * 0.6); // relic2-6 水滴减耗
            if (Configs.nextWaveState == 3 && GameMain.localStar!=null && GameMain.localStar.index == Configs.nextWaveStarIndex && time % 60 == 6)
            {
                int starIndex = GameMain.localStar.index;
                for (int i = 0; i < maxDroplet; i++)
                {
                    if (GameMain.mainPlayer.mecha.coreEnergy > energyCons * 2 && dropletPool[i].Launch(starIndex))
                    {
                        TryConsumeMechaEnergy(energyCons);
                        break; 
                    }
                }
            }

            //计算当前机甲能量可支持的最大运行状态的水滴数
            double mechaCurEnergy = GameMain.mainPlayer.mecha.coreEnergy;
            int maxWorkingDropletsNew = (int)((long)mechaCurEnergy / 100000000);
            if (maxWorkingDropletsNew > maxWorkingDroplets + 1)
                maxWorkingDroplets = maxWorkingDropletsNew - 1;
            if (maxWorkingDropletsNew < maxWorkingDroplets)
                maxWorkingDroplets = maxWorkingDropletsNew;
            maxWorkingDropletsNew = maxWorkingDroplets;

            //每个水滴update
            for (int i = 0; i < maxDroplet; i++)
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
                if (time % 120 == 0)
                    dropletPool[i].CheckBullet(); //游荡而又没有实体的子弹强行返回机甲
            }

        }

        public static int RefreshDropletNum()
        {
            int loadedDropletCnt = 0;
            int workingDroplectCnt = 0;
            for (int i = 0; i < maxDroplet; i++)
            {
                if (dropletPool[i].state > -1)
                    loadedDropletCnt++;
                if (dropletPool[i].state > 0)
                    workingDroplectCnt++;
            }
            for (int i = maxDroplet; i < 5; i++)
            {
                dropletPool[i].state = -1;
            }
            mechaDropletAmountText.text = $"{loadedDropletCnt - workingDroplectCnt} / {loadedDropletCnt} [max:{maxDroplet}]";
            return loadedDropletCnt;
        }

        public static bool TryConsumeMechaEnergy(double energy)
        {
            if (Relic.HaveRelic(1, 4)) energy *= 0.5; // relic1-4 水滴减耗
            if (Relic.HaveRelic(2, 6)) energy *= 0.6; // relic2-6 水滴减耗
            if (GameMain.mainPlayer.mecha.coreEnergy >= energy)
            {
                GameMain.mainPlayer.mecha.coreEnergy -= energy;
                GameMain.mainPlayer.mecha.MarkEnergyChange(8, -energy);
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
            GameMain.mainPlayer.mecha.MarkEnergyChange(8, -energy);
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
            for (int i = 0; i < dropletsUpperLimit; i++)
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
                for (int i = 0; i < 5; i++)
                {
                    dropletPool[i].Import(r);
                }
                if (Configs.versionWhenImporting >= 30230519)
                {
                    for (int i = 5; i < dropletsUpperLimit; i++)
                    {
                        dropletPool[i].Import(r);
                    }
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
        public int state = -1; //-1空-根本没有水滴，0有水滴-正在机甲中待命，1刚刚从机甲出来-慢，2攻击-飞向目标，3攻击-已越过目标准备折返，4返航回机甲,5马上要回到机甲-慢
        int dropletIndex = 0;
        int swarmIndex = -1;
        int[] bulletIds = new int[bulletCnt];
        int targetShipIndex = -1;
        int randSeed = 0; //决定水滴在撞向敌舰时的一个随机的小偏移，使其往返攻击时不是在两个点之间来回飞，而是有一些随机的角度变化，每次find敌人和从2阶段回到1阶段都会改变一次randSeed

        public static int exceedDis = 3000; //水滴撞击敌舰后保持原速度方向飞行的距离，之后才会瞬间转弯
        public static int bulletCnt = 25;
        public static int maxPosDelta = 100; //会根据是否达开星图设置为100或5，在Droplets里更新。目的是不打开星图时水滴比较细，打开星图后为了能清楚地看到设置得很大

        public Droplet(int idx)
        {
            state = -1;
            dropletIndex = idx;
            swarmIndex = -1;
            bulletIds = new int[bulletCnt];
            for (int i = 0; i < bulletIds.Length; i++)
            {
                bulletIds[i] = 0;
            }
            targetShipIndex = -1;
            randSeed = Utils.RandNext();
        }

        public bool Launch(int starIndex)
        {
            if (state != 0)
                return false;

            if (swarmIndex == starIndex || state == 0) 
            {
                swarmIndex = starIndex;
                if (FindNextTarget())
                {
                    if (CreateBulltes())
                    {
                        state = 1;//起飞
                        return true;
                    }
                    else //没创建成功，很可能是因为swarm为null
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        bool CreateBulltes()
        {
            if (swarmIndex < 0) return false;
            DysonSwarm swarm = GetSwarm();
            if (swarm == null || !EnemyShips.ships.ContainsKey(targetShipIndex)) return false;

            VectorLF3 beginUPos = GameMain.mainPlayer.uPosition;
            VectorLF3 endUPos = beginUPos + (EnemyShips.ships[targetShipIndex].uPos - beginUPos).normalized * 300;
            Vector3 beginLPos = new Vector3(0, 0, 0);
            if (GameMain.localPlanet!=null)
            {
                int planetId = GameMain.localPlanet.id;
                AstroData[] astroPoses = GameMain.galaxy.astrosData;
                beginUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
                endUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position + GameMain.mainPlayer.position.normalized*300); //飞出地表的方向
                beginLPos = GameMain.mainPlayer.position;
            }

            float newMaxt = (float)((endUPos - beginUPos).magnitude / Configs.dropletSpd * 200);
            for (int i = 0; i < bulletIds.Length; i++)
            {
                if (i > 0) //起飞阶段只渲染一个
                {
                    beginUPos = new VectorLF3(0, 0, 0);
                    endUPos = new VectorLF3(1, 2, 3);
                    beginLPos = new Vector3(9999, 9998, 9997);
                }
                bulletIds[i] = swarm.AddBullet(new SailBullet
                {
                    maxt = newMaxt,
                    lBegin = beginLPos,
                    uEndVel = new Vector3(1, 1, 1),
                    uBegin = beginUPos, //起飞过程不加random
                    uEnd = endUPos
                }, 1);
                swarm.bulletPool[bulletIds[i]].state = 0;
            }
            return true;
        }

        bool FindNextNearTarget(double searchRangeRatio = 0.2, int backupStrategy = 1, int planetIdDelta = 1)
        {
            if (Configs.nextWaveState == 3 && swarmIndex == Configs.nextWaveStarIndex)
            {
                VectorLF3 curUPos = GetCurrentUPos();
                List<int> enemiesNear = EnemyShips.FindShipsInRange(curUPos, Configs.dropletSpd * searchRangeRatio);
                if (enemiesNear.Count <= 0) enemiesNear = EnemyShips.FindShipsInRange(curUPos, Configs.dropletSpd * searchRangeRatio * 10); //如果没找到，扩大寻找范围
                if (enemiesNear.Count <= 0) return FindNextTarget(); //还没找到就按其他规则寻敌

                int tarIdx = Utils.RandInt(0, enemiesNear.Count);
                if(tarIdx == targetShipIndex && enemiesNear.Count > 1) //新目标如果和上一个目标一样，并且有其他可选目标，则尽量选择不同的目标
                {
                    if(tarIdx > 0)
                    {
                        tarIdx = Utils.RandInt(0, tarIdx);
                    }
                    else
                    {
                        tarIdx = Utils.RandInt(tarIdx + 1, enemiesNear.Count);
                    }
                }
                if (EnemyShips.ships[enemiesNear[tarIdx]].state == EnemyShip.State.active)
                {
                    targetShipIndex = enemiesNear[tarIdx];
                    return true;
                }
                else
                {
                    return FindNextTarget();
                }
            }
            return false;
        }
        bool FindNextTarget(int strategy=1, int planetIdDelta = 1)
        {
            if(Configs.nextWaveState == 3 && swarmIndex == Configs.nextWaveStarIndex)
            {
                List<EnemyShip> targets = EnemyShips.sortedShips(strategy, Configs.nextWaveStarIndex, (Configs.nextWaveStarIndex+1) * 100 + planetIdDelta);
                if(targets.Count>0)
                {
                    int begin = randSeed % 5;
                    begin = begin % targets.Count;

                    for (int i = begin; i < targets.Count; i++)
                    {
                        if(targets[i].state == EnemyShip.State.active)
                        {
                            targetShipIndex = targets[i].shipIndex;
                            randSeed = Utils.RandNext();
                            return true;
                        }
                    }
                    for (int i = 0; i < begin; i++)
                    {
                        if (targets.Count <= i) break;
                        if (targets[i].state == EnemyShip.State.active)
                        {
                            targetShipIndex = targets[i].shipIndex;
                            randSeed = Utils.RandNext();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Update(bool working = true)
        {
            if (state <= 0) return;
            if(swarmIndex < 0)
            {
                state = 0;
                return;
            }
            DysonSwarm swarm = GetSwarm();

            if (swarm == null)
            {
                state = 0;
                return;
            }
            if(swarm.bulletPool.Length <= bulletIds[0])
            {
                state = 0;
                return;
            }

            if(GameMain.localStar == null || GameMain.localStar.index!=swarmIndex)//机甲所在星系和水滴不一样，让水滴返回
            {
                state = 0;
                TryRemoveOtherBullets(0);
                return;
            }

            if(state >= 2 && state <= 3 && working)//只有不是飞出、返航过程，才会消耗能量
            {
                Droplets.ForceConsumeMechaEnergy(Droplets.energyConsumptionPerTick);
            }

            if(state >= 2 && state <= 3 && !working && Configs.nextWaveState==3) //如果因为机甲能量水平不够，水滴会停在原地，但是如果战斗状态结束了，那么水滴会无视能量限制继续正常Update（正常Update会在战斗结束后让水滴立刻进入4阶段回机甲）
            {
                float tickT = 0.016666668f;
                swarm.bulletPool[bulletIds[0]].t -= tickT;
                VectorLF3 lastUPos = GetCurrentUPos();
                for (int i = 0; i < bulletIds.Length; i++)
                {
                    if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                    swarm.bulletPool[bulletIds[i]].uBegin = lastUPos;
                    swarm.bulletPool[bulletIds[i]].t = 0;
                }
                return;
            }

            if (state == 1) //刚起飞
            {
                float lastT = swarm.bulletPool[bulletIds[0]].t;
                float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
                //如果原目标不存在了，尝试寻找新目标，如果找不到目标，设定为极接近机甲的回到机甲状态（5）
                if (!EnemyShips.ships.ContainsKey(targetShipIndex) || EnemyShips.ships[targetShipIndex].state != EnemyShip.State.active)
                {
                    if (!FindNextTarget())
                    {
                        state = 5;
                        //刷新当前位置，设t=0，重新计算终点和maxt
                        VectorLF3 newBegin = GetCurrentUPos();
                        VectorLF3 newEnd = GameMain.mainPlayer.uPosition;
                        if (GameMain.localPlanet != null)
                        {
                            int planetId = GameMain.localPlanet.id;
                            AstroData[] astroPoses = GameMain.galaxy.astrosData;
                            newEnd = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
                        }
                        RetargetAllBullet(newBegin, newEnd, 1, 0, 0, Configs.dropletSpd / 200.0);
                        //TryRemoveOtherBullets();
                    }
                }
                else if (lastMaxt - lastT <= 0.035f) //进入太空索敌阶段
                {
                    state = 2; //不需要在此刷新当前位置，因为state2每帧开头都刷新
                }
                
            }
            else if (state == 2) //追敌中
            {

                float lastT = swarm.bulletPool[bulletIds[0]].t;
                float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;

                //如果原目标不存在了，尝试寻找新目标，如果找不到目标，设定为回机甲状态（4）
                if (!EnemyShips.ships.ContainsKey(targetShipIndex) || EnemyShips.ships[targetShipIndex].state != EnemyShip.State.active)
                {
                    if (!FindNextNearTarget())
                    {
                        state = 4;
                        VectorLF3 newBegin = GetCurrentUPos();
                        VectorLF3 newEnd = GameMain.mainPlayer.uPosition;
                        if (GameMain.localPlanet!=null) //如果玩家在星球上，水滴则不是直线往玩家身上飞，而是飞到玩家头顶星球上空，然后再飞回玩家（这是在state=5阶段）
                        {
                            int planetId = GameMain.localPlanet.id;
                            AstroData[] astroPoses = GameMain.galaxy.astrosData;
                            newEnd = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position * 2); 
                        }
                        RetargetAllBullet(newBegin, newEnd, bulletIds.Length, maxPosDelta, maxPosDelta, Configs.dropletSpd);
                    }
                }
                else //目标存在
                {
                    VectorLF3 enemyUPos = EnemyShips.ships[targetShipIndex].uPos + Utils.RandPosDelta(randSeed) * 200f;
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
                    RetargetAllBullet(newBegin, newEnd, bulletIds.Length, maxPosDelta, maxPosDelta, realSpd);
                    //判断击中，如果距离过近
                    if ((newBegin - enemyUPos).magnitude < 500 || newMaxt <= exceedDis * 1.0 / Configs.dropletSpd + 0.035f)
                    {
                        int damage = Configs.dropletAtk;
                        if (Rank.rank >= 10) damage = 5* Configs.dropletAtk;
                        if (Relic.HaveRelic(0, 10))
                        {
                            damage = damage + (Relic.BonusDamage(Droplets.bonusDamage, 1) - Droplets.bonusDamage);
                            UIBattleStatistics.RegisterDropletAttack(EnemyShips.ships[targetShipIndex].BeAttacked(damage, DamageType.droplet, true));
                        }
                        else
                        {
                            UIBattleStatistics.RegisterDropletAttack(EnemyShips.ships[targetShipIndex].BeAttacked(damage, DamageType.droplet));
                        }
                        state = 3; //击中后继续冲过目标，准备转向的阶段
                    }
                }
            }
            else if (state == 3) //刚刚击中敌船，正准备转向
            {
                float lastT = swarm.bulletPool[bulletIds[0]].t;
                float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
                VectorLF3 newBegin = GetCurrentUPos();
                if (lastMaxt - lastT <= 0.035) //到头了，执行转向/重新索敌
                {
                    bool continueAttack = false;
                    if (EnemyShips.ships.ContainsKey(targetShipIndex) && EnemyShips.ships[targetShipIndex].state == EnemyShip.State.active)
                        continueAttack = true;
                    else if (FindNextTarget())
                        continueAttack = true;

                    if (continueAttack)
                    {
                        FindNextNearTarget(); //寻找新的较近的敌人
                        randSeed = Utils.RandNext(); //改变索敌定位时的随机偏移种子
                        state = 2; //回到追敌攻击状态
                        VectorLF3 uEnd = swarm.bulletPool[bulletIds[0]].uEnd;
                        VectorLF3 enemyUPos = EnemyShips.ships[targetShipIndex].uPos + Utils.RandPosDelta(randSeed) * 200f;
                        uEnd = (enemyUPos - newBegin).normalized * exceedDis + enemyUPos;
                        RetargetAllBullet(newBegin, uEnd, bulletIds.Length, maxPosDelta, maxPosDelta, Configs.dropletSpd);
                    }
                    else
                    {
                        state = 4;
                        VectorLF3 newEnd = GameMain.mainPlayer.uPosition;
                        if (GameMain.localPlanet != null) //如果玩家在星球上，水滴则不是直线往玩家身上飞，而是飞到玩家头顶星球上空，然后再飞回玩家（这是在state=5阶段）
                        {
                            int planetId = GameMain.localPlanet.id;
                            AstroData[] astroPoses = GameMain.galaxy.astrosData;
                            newEnd = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position * 2);
                        }
                        RetargetAllBullet(newBegin, newEnd, bulletIds.Length, maxPosDelta, maxPosDelta, Configs.dropletSpd);
                    }
                }
            }
            else if (state == 4) //正在回到机甲的路程中，但还不够近
            {
                int mechaStarIndex = -1;

                VectorLF3 mechaUPos = GameMain.mainPlayer.uPosition;
                VectorLF3 mechaUPos2 = GameMain.mainPlayer.uPosition;

                if (GameMain.localStar != null)
                    mechaStarIndex = GameMain.localStar.index;

                if (GameMain.localPlanet != null)
                {
                    int planetId = GameMain.localPlanet.id;
                    AstroData[] astroPoses = GameMain.galaxy.astrosData;
                    mechaUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
                    mechaUPos2 = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position * 2);
                }

                //如果水滴已经处在返回状态但是和机甲不在同一个星系，直接传送回机甲。但是攻击状态（1和2）的水滴不会瞬移回机甲
                if (mechaStarIndex >= 0 && swarmIndex != mechaStarIndex)
                {
                    for (int i = 0; i < bulletIds.Length; i++)
                    {
                        if (swarm.bulletPool.Length <= bulletIds[i])
                            continue;
                        swarm.RemoveBullet(bulletIds[i]);
                    }
                    swarmIndex = -1;
                    state = 0;
                }
                else
                {
                    float lastT = swarm.bulletPool[bulletIds[0]].t;
                    float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
                    VectorLF3 newBegin = GetCurrentUPos();
                    VectorLF3 newEnd = mechaUPos2;
                    //float newMaxt = (float)((uEnd - uBegin).magnitude / Configs.dropletSpd);

                    if (lastMaxt - lastT <= 0.05 || (newBegin - newEnd).magnitude < Configs.dropletSpd / 20f) //已经到机甲上方或者接近机甲
                    {
                        state = 5;
                    }
                    
                    if(state==5)
                    {
                        TryRemoveOtherBullets();
                        if (GameMain.localPlanet != null) newBegin = mechaUPos2; //如果是在星球上，则从上空(也就是mechUPos2)飞回来，否则从阶段拐点的原位置(也就是GetCurrentUPos())直线向机甲飞回来
                        //RetargetAllBullet(newBegin, mechaUPos, 1, 0, 0, Configs.dropletSpd / 200.0);
                        float tickT = 0.016666668f;
                        swarm.bulletPool[bulletIds[0]].maxt = (float)((newBegin - mechaUPos).magnitude / (Configs.dropletSpd / 200.0));
                        swarm.bulletPool[bulletIds[0]].t = swarm.bulletPool[bulletIds[0]].maxt - tickT * 3;
                        swarm.bulletPool[bulletIds[0]].uEnd = newBegin;
                        swarm.bulletPool[bulletIds[0]].uBegin = mechaUPos;
                        swarm.bulletPool[bulletIds[0]].lBegin = GameMain.mainPlayer.position;
                    }
                    else
                    {
                        RetargetAllBullet(newBegin, mechaUPos2, bulletIds.Length, maxPosDelta, maxPosDelta, Configs.dropletSpd);
                    }

                }


            }
            else if(state == 5) //回到机甲阶段
            {
                VectorLF3 mechaUPos = GameMain.mainPlayer.uPosition;

                float tickT = 0.016666668f;

                if (GameMain.localPlanet != null)
                {
                    int planetId = GameMain.localPlanet.id;
                    AstroData[] astroPoses = GameMain.galaxy.astrosData;
                    mechaUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
                }
                swarm.bulletPool[bulletIds[0]].t -= tickT * 2;
                float lastT = swarm.bulletPool[bulletIds[0]].t;
                //float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;

                if (lastT <= 0.03) //足够近，则回到机甲
                {
                    state = 0;
                    TryRemoveOtherBullets(0);
                    swarmIndex = -1;
                }
                else //否则持续更新目标点为机甲位置
                {
                    for (int i = 0; i < 1; i++)
                    {
                        if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                        swarm.bulletPool[bulletIds[i]].uBegin = mechaUPos;
                        swarm.bulletPool[bulletIds[0]].lBegin = GameMain.mainPlayer.position;
                    }

                }
            }


        }

        VectorLF3 GetCurrentUPos()
        {
            VectorLF3 uPos000 = new VectorLF3(0, 0, 0);
            DysonSwarm swarm = GetSwarm();

            if (swarm == null) return uPos000;
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
                swarm.bulletPool[bulletIds[i]].uBegin = newUBegin + Utils.RandPosDelta(randSeed + i + 100) * randomBeginRatio; //uBegin + Utils.RandPosDelta(randSeed + i + 100) * randomBeginRatio;
                swarm.bulletPool[bulletIds[i]].uEnd = uEnd + Utils.RandPosDelta(randSeed + i + 100) * randomEndRatio;
            }

        }

        void TryRemoveOtherBullets(int beginIndex=1)
        {
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
                //return GameMain.data.dysonSpheres[swarmIndex]?.swarm;
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
            w.Write(swarmIndex);
            for (int i = 0; i < 25; i++)
            {
                w.Write(bulletIds[i]);
            }
            w.Write(targetShipIndex);
            w.Write(randSeed);
        }
        public void Import(BinaryReader r)
        {
            state = r.ReadInt32();
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
            targetShipIndex = r.ReadInt32();
            randSeed = r.ReadInt32();
        }
    }
}
