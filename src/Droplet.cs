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
        public int state = -1; //-1空-根本没有水滴，0有水滴-正在机甲中待命，1攻击-飞向目标，2攻击-已越过目标准备折返，3返航回机甲
        int swarmIndex = -1;
        int[] bulletIds = new int[25];
        int targetShipIndex = -1;

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
            if (state != 0 && state != 3)
                return false;

            if (swarmIndex == starIndex || state == 0) 
            {
                if (FindNextTarget())
                {
                    swarmIndex = starIndex;
                    if (state == 0) //如果是在机甲中待机的状态，需要创建全新的子弹实体来显示
                    {
                        if (CreateBulltes())
                            state = 1;
                        else //没创建成功，很可能是因为swarm为null
                            return false;
                    }
                    else
                    {
                        state = 1;
                    }
                    return true;
                }
            }
            return false;
        }

        bool CreateBulltes()
        {
            DysonSwarm swarm = GameMain.data.dysonSpheres[swarmIndex]?.swarm;
            if (swarm == null || !EnemyShips.ships.ContainsKey(targetShipIndex)) return false;

            VectorLF3 beginUPos = GameMain.mainPlayer.uPosition;
            if(GameMain.localPlanet!=null)
            {
                int planetId = GameMain.localPlanet.id;
                AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                beginUPos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, GameMain.mainPlayer.position);
            }
            VectorLF3 endUPos = (EnemyShips.ships[targetShipIndex].uPos - beginUPos).normalized * exceedDis + EnemyShips.ships[targetShipIndex].uPos;

            for (int i = 0; i < 25; i++)
            {
                bulletIds[i] = swarm.AddBullet(new SailBullet
                {
                    maxt = (float)((endUPos-beginUPos).magnitude / Configs.dropletSpd),
                    lBegin = new Vector3(0, 0, 0),
                    uEndVel = new Vector3(1, 1, 1),
                    uBegin = beginUPos + Utils.RandPosDelta(),
                    uEnd = endUPos + Utils.RandPosDelta()
                }, 1);
                swarm.bulletPool[bulletIds[i]].state = 0;
            }
            return true;
        }

        bool FindNextTarget(int strategy=1, int planetIdDelta = 101)
        {
            if(Configs.nextWaveState ==3 && GameMain.localStar!=null && GameMain.localStar.index == Configs.nextWaveStarIndex)
            {
                List<EnemyShip> targets = EnemyShips.sortedShips(strategy, Configs.nextWaveStarIndex, Configs.nextWaveStarIndex * 100 + planetIdDelta);
                if(targets.Count>0)
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if(targets[i].state == EnemyShip.State.active)
                        {
                            targetShipIndex = targets[i].shipIndex;
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
            if (state == 1) //追敌中
            {
                //如果远目标不存在了，尝试寻找新目标，如果找不到目标，设定为回机甲状态（3）
                if (!EnemyShips.ships.ContainsKey(targetShipIndex) || EnemyShips.ships[targetShipIndex].state != EnemyShip.State.active)
                {
                    if (!FindNextTarget())
                        state = 3;
                }
            }









            int playerStarIndex = -1;
            if (GameMain.localStar != null)
                playerStarIndex = GameMain.localStar.index;



        }

        public void Export(BinaryWriter w)
        {

        }
        public void Import(BinaryReader r)
        {

        }
    }
}
