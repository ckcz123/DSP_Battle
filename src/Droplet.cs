﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Concurrent;
using System.IO;
using HarmonyLib;

namespace DSP_Battle
{
    public class Droplets
    {
        public static GameObject mechaDropletObj = null;
        public static Text mechaDropletAmountText = null;

        //存档内容
        public static Droplet[] dropletPool = new Droplet[5];
        public static int maxDroplet = 2;



        public static void InitAll()
        {
            maxDroplet = 2;
            for (int i = 0; i < 5; i++)
            {
                dropletPool[i] = new Droplet();
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
            mechaDropletObj.transform.Find("icon").localPosition = new Vector3(1, 0, 0);
            mechaDropletAmountText = mechaDropletObj.transform.Find("cnt-text").GetComponent<Text>();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {
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
            if(Configs.nextWaveState == 3 && GameMain.localStar!=null && GameMain.localStar.index == Configs.nextWaveStarIndex && time % 60 == 6)
            {
                int starIndex = GameMain.localStar.index;
                for (int i = 0; i < maxDroplet; i++)
                {
                    if (dropletPool[i].Launch(starIndex)) break;
                }
            }

            //每个水滴update
            for (int i = 0; i < maxDroplet; i++)
            {
                dropletPool[i].Update();
                if (time % 120 == 0)
                    dropletPool[i].CheckBullet(); //游荡而又没有实体的子弹强行返回机甲
            }
            Utils.Log("");

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
            mechaDropletAmountText.text = $"{workingDroplectCnt}+{loadedDropletCnt - workingDroplectCnt} / {maxDroplet}";
            return loadedDropletCnt;
        }

        public static void UpdateMechaUI()
        {

        }

        public static void Export(BinaryWriter w) 
        { 

        }
        public static void Import(BinaryReader r) 
        {
            InitAll();
            if(Configs.versionWhenImporting >= 30220328)
            {

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
        int swarmIndex = -1;
        int[] bulletIds = new int[25];
        int targetShipIndex = -1;
        int randSeed = 0; //决定水滴在撞向敌舰时的一个随机的小偏移，使其往返攻击时不是在两个点之间来回飞，而是有一些随机的角度变化，每次find敌人和从2阶段回到1阶段都会改变一次randSeed

        public static int exceedDis = 2000; //水滴撞击敌舰后保持原速度方向飞行的距离，之后才会瞬间转弯


        public Droplet()
        {
            state = -1;
            swarmIndex = -1;
            bulletIds = new int[25];
            targetShipIndex = -1;
        }

        public bool Launch(int starIndex)
        {
            if (state != 0)
                return false;

            if (swarmIndex == starIndex || state == 0) 
            {
                if (FindNextTarget())
                {
                    swarmIndex = starIndex;

                    if (CreateBulltes())
                    {
                        state = 1;//起飞
                        return true;
                    }
                    else //没创建成功，很可能是因为swarm为null
                        return false;
                }
            }
            return false;
        }

        bool CreateBulltes()
        {
            DysonSwarm swarm = GameMain.data.dysonSpheres[swarmIndex]?.swarm;
            if (swarm == null || !EnemyShips.ships.ContainsKey(targetShipIndex)) return false;

            VectorLF3 beginUPos = GameMain.mainPlayer.uPosition;
            VectorLF3 endUPos = beginUPos + (EnemyShips.ships[targetShipIndex].uPos - beginUPos).normalized * 500;
            Vector3 beginLPos = new Vector3(0, 0, 0);
            if (GameMain.localPlanet!=null)
            {
                int planetId = GameMain.localPlanet.id;
                AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                beginUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
                endUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position * 2); //飞出地表的方向
                beginLPos = GameMain.mainPlayer.position;
            }

            float newMaxt = (float)((endUPos - beginUPos).magnitude / Configs.dropletSpd * 200);
            for (int i = 0; i < 25; i++)
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

        bool FindNextTarget(int strategy=1, int planetIdDelta = 101)
        {
            if(Configs.nextWaveState == 3 && GameMain.localStar!=null && GameMain.localStar.index == Configs.nextWaveStarIndex)
            {
                List<EnemyShip> targets = EnemyShips.sortedShips(strategy, Configs.nextWaveStarIndex, Configs.nextWaveStarIndex * 100 + planetIdDelta);
                if(targets.Count>0)
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if(targets[i].state == EnemyShip.State.active)
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

        public void Update()
        {
            if (state <= 0) return;
            if(swarmIndex < 0)
            {
                state = 0;
                return;
            }
            DysonSwarm swarm = GameMain.data.dysonSpheres[swarmIndex]?.swarm;
            if(swarm == null)
            {
                state = 0;
                return;
            }
            if(swarm.bulletPool.Length <= bulletIds[0])
            {
                state = 0;
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
                        VectorLF3 uBegin = swarm.bulletPool[bulletIds[0]].uBegin;
                        VectorLF3 uEnd = swarm.bulletPool[bulletIds[0]].uEnd;
                        uBegin = (uEnd - uBegin) * (lastT / lastMaxt) + uBegin;
                        uEnd = GameMain.mainPlayer.uPosition;
                        if (GameMain.localPlanet != null) //如果玩家在星球上，水滴则不是直线往玩家身上飞，而是飞到玩家头顶星球上空，然后再飞回玩家（这是在state=5阶段）
                        {
                            int planetId = GameMain.localPlanet.id;
                            AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                            uEnd = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
                        }
                        RetargetAllBullet(uEnd, 1, 0, 0, Configs.dropletSpd / 200.0);
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
                    if (!FindNextTarget())
                    {
                        state = 4;
                        VectorLF3 uBegin = swarm.bulletPool[bulletIds[0]].uBegin;
                        VectorLF3 uEnd = swarm.bulletPool[bulletIds[0]].uEnd;
                        uBegin = (uEnd - uBegin) * (lastT / lastMaxt) + uBegin;
                        uEnd = GameMain.mainPlayer.uPosition;
                        if (GameMain.localPlanet!=null) //如果玩家在星球上，水滴则不是直线往玩家身上飞，而是飞到玩家头顶星球上空，然后再飞回玩家（这是在state=5阶段）
                        {
                            int planetId = GameMain.localPlanet.id;
                            AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                            uEnd = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position * 2); 
                        }
                        //float newMaxt = (float)((uEnd - uBegin).magnitude / Configs.dropletSpd);
                        //for (int i = 0; i < bulletIds.Length; i++)
                        //{
                        //    if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                        //    swarm.bulletPool[bulletIds[i]].t = 0;
                        //    swarm.bulletPool[bulletIds[i]].maxt = newMaxt;
                        //    swarm.bulletPool[bulletIds[i]].uBegin = uBegin + Utils.RandPosDelta(randSeed + i + 100) * 5;
                        //    swarm.bulletPool[bulletIds[i]].uEnd = uEnd + Utils.RandPosDelta(randSeed + i + 100) * 5;
                        //}
                        RetargetAllBullet(uEnd, 25, 5, 5, Configs.dropletSpd);
                    }
                }
                else //目标存在
                {
                    VectorLF3 enemyUPos = EnemyShips.ships[targetShipIndex].uPos + Utils.RandPosDelta(randSeed) * 500f;
                    VectorLF3 uBegin = swarm.bulletPool[bulletIds[0]].uBegin;
                    VectorLF3 uEnd = (enemyUPos - uBegin).normalized * exceedDis + enemyUPos;
                    uBegin = (uEnd - uBegin) * (lastT / lastMaxt) + uBegin;
                    float newMaxt = (float)((uEnd - uBegin).magnitude / Configs.dropletSpd);
                    ////将属性值应用到组成水滴的子弹上
                    //for (int i = 0; i < bulletIds.Length; i++)
                    //{
                    //    if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                    //    swarm.bulletPool[bulletIds[i]].t = 0;
                    //    swarm.bulletPool[bulletIds[i]].maxt = newMaxt;
                    //    swarm.bulletPool[bulletIds[i]].uBegin = uBegin + Utils.RandPosDelta(randSeed + i + 100) * 5;
                    //    swarm.bulletPool[bulletIds[i]].uEnd = uEnd + Utils.RandPosDelta(randSeed + i + 100) * 5;
                    //}
                    RetargetAllBullet(uEnd, 25, 5, 5, Configs.dropletSpd);
                    //判断击中，如果距离过近
                    if ((uBegin - enemyUPos).magnitude < 500 || newMaxt <= exceedDis * 1.0 / Configs.dropletSpd + 0.035f)
                    {
                        UIBattleStatistics.RegisterDropletAttack(EnemyShips.ships[targetShipIndex].BeAttacked(Configs.dropletAtk));
                        state = 3; //击中后继续冲过目标，准备转向的阶段
                    }
                }
            }
            else if (state == 3) //刚刚击中敌船，正准备转向
            {
                float lastT = swarm.bulletPool[bulletIds[0]].t;
                float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
                VectorLF3 uBegin = swarm.bulletPool[bulletIds[0]].uBegin;
                if (lastMaxt - lastT <= 0.035) //到头了，执行转向/重新索敌
                {
                    bool continueAttack = false;
                    if (EnemyShips.ships.ContainsKey(targetShipIndex) && EnemyShips.ships[targetShipIndex].state == EnemyShip.State.active)
                        continueAttack = true;
                    else if (FindNextTarget())
                        continueAttack = true;

                    if (continueAttack)
                    {
                        randSeed = Utils.RandNext(); //改变索敌定位时的随机偏移种子
                        state = 2; //回到追敌攻击状态
                        VectorLF3 uEnd = swarm.bulletPool[bulletIds[0]].uEnd;
                        VectorLF3 enemyUPos = EnemyShips.ships[targetShipIndex].uPos + Utils.RandPosDelta(randSeed) * 500f;
                        uBegin = (uEnd - uBegin) * (lastT / lastMaxt) + uBegin;
                        uEnd = (enemyUPos - uBegin).normalized * exceedDis + enemyUPos;
                        RetargetAllBullet(uEnd, 25, 5, 5, Configs.dropletSpd);
                    }
                    else
                    {
                        state = 4;
                    }
                }
            }
            else if (state == 4) //正在回到机甲的路程中，但还不够近
            {
                int mechaStarIndex = -1;

                VectorLF3 mechaUPos = GameMain.mainPlayer.uPosition;

                if (GameMain.localStar != null)
                    mechaStarIndex = GameMain.localStar.index;

                if (GameMain.localPlanet != null)
                {
                    int planetId = GameMain.localPlanet.id;
                    AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                    mechaUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
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
                    VectorLF3 uBegin = swarm.bulletPool[bulletIds[0]].uBegin;
                    VectorLF3 uEnd = swarm.bulletPool[bulletIds[0]].uEnd;
                    uBegin = (uEnd - uBegin) * (lastT / lastMaxt) + uBegin;
                    uEnd = (mechaUPos - uBegin).normalized * (Configs.dropletSpd / 20f) + mechaUPos;
                    if (GameMain.localPlanet != null) //如果玩家在星球上，水滴则不是直线往玩家身上飞，而是飞到玩家头顶星球上空，然后再飞回玩家（这是在state=5阶段）
                    {
                        int planetId = GameMain.localPlanet.id;
                        AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                        uEnd = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position * 2);
                    }
                    //float newMaxt = (float)((uEnd - uBegin).magnitude / Configs.dropletSpd);

                    if (lastMaxt <= 0.05 || (uBegin - mechaUPos).magnitude < Configs.dropletSpd / 20f) //已经到机甲上方或者接近机甲
                    {
                        state = 5;
                        //newMaxt = (float)((uEnd - uBegin).magnitude / Configs.dropletSpd * 200); //设定一个很慢的速度回到机甲
                    }
                    
                    if(state==5)
                    {
                        TryRemoveOtherBullets();
                        RetargetAllBullet(uEnd, 1, 0, 0, Configs.dropletSpd / 200.0, true);
                    }
                    else
                    {
                        RetargetAllBullet(uEnd, 25, 5, 5, Configs.dropletSpd);
                    }

                }


            }
            else if(state == 5) //回到机甲阶段
            {
                VectorLF3 mechaUPos = GameMain.mainPlayer.uPosition;

                if (GameMain.localPlanet != null)
                {
                    int planetId = GameMain.localPlanet.id;
                    AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                    mechaUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
                }

                float lastT = swarm.bulletPool[bulletIds[0]].t;
                float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;

                if (lastMaxt - lastT <= 0.03) //足够近，则回到机甲
                {
                    state = 0;
                    swarmIndex = -1;
                    TryRemoveOtherBullets(0);
                }
                else //否则持续更新目标点为机甲位置
                {
                    for (int i = 0; i < 1; i++)
                    {
                        if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                        swarm.bulletPool[bulletIds[i]].uEnd = mechaUPos;
                    }

                }
            }


        }


        void RetargetAllBullet(VectorLF3 newUEnd, int bulletNum, int randomBeginRatio, int randomEndRatio, double speed, bool debug=false) //如果只更新uEnd，不更新maxt等其他信息，则不要使用这个函数
        {
            DysonSwarm swarm = null;
            if (GameMain.data.dysonSpheres[swarmIndex] != null)
                swarm = GameMain.data.dysonSpheres[swarmIndex].swarm;
            if (swarm == null) return;
            if (swarm.bulletPool.Length <= bulletIds[0])
            {
                state = 0;
                return;
            }

            float lastT = swarm.bulletPool[bulletIds[0]].t;
            float lastMaxt = swarm.bulletPool[bulletIds[0]].maxt;
            VectorLF3 uBegin = swarm.bulletPool[bulletIds[0]].uBegin;
            VectorLF3 uEnd = swarm.bulletPool[bulletIds[0]].uEnd;
            if (debug) Utils.Log($"lastT = {lastT}, lastmaxt={lastMaxt}, then {uEnd}-{uBegin} / {lastT / lastMaxt}");
            uBegin = (uEnd - uBegin) * lastT / lastMaxt + uBegin;
            uEnd = newUEnd;

            float newMaxt = 1;
            if (speed > 0)
                newMaxt = (float)((uEnd - uBegin).magnitude / speed);
            if (newMaxt <= 0.017f)
                newMaxt = 0.017f;

            if (debug) Utils.Log($"magnitude = {uEnd}-{uBegin} then divide {speed} is {newMaxt}");
            if (bulletNum > bulletIds.Length)
                bulletNum = bulletIds.Length;

            swarm.bulletPool[bulletIds[0]].t = 0;
            swarm.bulletPool[bulletIds[0]].maxt = newMaxt;
            swarm.bulletPool[bulletIds[0]].uBegin = uBegin;
            swarm.bulletPool[bulletIds[0]].uEnd = uEnd;

            for (int i = 1; i < bulletNum; i++)
            {
                if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                swarm.bulletPool[bulletIds[i]].t = 0;
                swarm.bulletPool[bulletIds[i]].maxt = newMaxt;
                swarm.bulletPool[bulletIds[i]].uBegin = uBegin + Utils.RandPosDelta(randSeed + i + 100) * randomBeginRatio;
                swarm.bulletPool[bulletIds[i]].uEnd = uEnd + Utils.RandPosDelta(randSeed + i + 100) * randomEndRatio;
            }

        }

        void TryRemoveOtherBullets(int beginIndex=1)
        {
            DysonSwarm swarm = null;
            if (GameMain.data.dysonSpheres[swarmIndex] != null)
                swarm = GameMain.data.dysonSpheres[swarmIndex].swarm;
            if (swarm == null) return;
            for (int i = beginIndex; i < bulletIds.Length; i++)
            {
                if (swarm.bulletPool.Length <= bulletIds[i]) continue;
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
            DysonSwarm swarm = null;
            if (GameMain.data.dysonSpheres[swarmIndex] != null)
                swarm = GameMain.data.dysonSpheres[swarmIndex].swarm;
            if (swarm == null) return;
            if (state < 0) return;
            for (int i = 0; i < bulletIds.Length; i++)
            {
                if (swarm.bulletPool.Length <= bulletIds[i]) continue;
                if (swarm.bulletPool[bulletIds[i]].id > 0) return;
            }
            state = 0;
        }

        public void Export(BinaryWriter w)
        {

        }
        public void Import(BinaryReader r)
        {

        }
    }
}
