using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MoreMegaStructure;
using HarmonyLib;

namespace DSP_Battle
{
    class StarCannon
    {

        public static GameObject fireButtonObj = null;
        public static Button fireButton = null;
        public static Text fireButtonText = null;
        public static Image fireButtonImage = null;

        static Color cannonDisableColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        static Color cannonChargingColor = new Color(0.42f, 0.2f, 0.2f, 1f);
        static Color cannonReadyColor = new Color(0f, 0.499f, 0.824f, 1f);
        static Color cannonAimingColor = new Color(0.973f, 0.359f, 0.170f, 1f);
        static Color cannonFiringColor = new Color(1f, 0.16f, 0.16f, 1f);

        //下面属性可能根据戴森球等级有变化，但并不需要存档        
        public static int starCannonLevel = 1; //恒星炮建造的所属阶段（等级），即完成度
        public static int damagePerTick = 4000; //每tick伤害
        public static double maxRange = 10.0; //恒星炮最大开火距离，以光年计，1ly = 60AU = 60 * 40000m。
        public static int warmTimeNeed = 600; //阶段2预热加速旋转需要的tick时间
        public static int cooldownTimeNeed = 600; //阶段5冷却需要的tick时间
        public static int chargingTimeNeed = 75 * 3600; //阶段-1的重新充能需要的tick时间
        public static float reAimAngularSpeed = 50f; //连续瞄准时，所有层以同一个速度旋转瞄准到下一个虫洞
        public static int maxAimCount = 100; //连续瞄准次数上限
        public static List<double> layerRotateSpeed; //不需要存档，每次随机生成即可

        //需要存档的量
        public static bool Fire = false;
        public static int time = 0; //恒星炮工作流程计时，均以tick记。负值代表冷却/充能过程
        public static int fireStage = 0; //恒星炮开火阶段。1=瞄准；2=预热旋转且瞄准锁定；3=开火；4=刚消灭一个虫洞、准备继续连续瞄准（此阶段只有一帧）；5=连续开火的正在瞄准新虫洞；-2=将各层角度还原到随机的其他角度并减慢旋转速度，冷却中；-1=重新充能中；0=充能完毕、待命、可开火。
        public static int endAimTime = 999; //最慢的轨道所需的瞄准时间，也就是阶段1的总时间
        public static VectorLF3 targetUPos = new VectorLF3(30000, 40000, -50000);
        public static float rotateSpeedScale = 1;

        //每帧更新不需要存档
        public static DysonSwarm targetSwarm = null; //除了要在恒星炮的星系上发射“太阳帆束”来体现动画效果，还要在受攻击恒星上发射，使在观看目标点时也能够渲染，所以需要受击目标所在恒星系的index
        public static int starCannonStarIndex = -1; //恒星炮所在恒星的index，每帧更新


        public static void InitAll()
        {
            layerRotateSpeed = new List<double>();
            for (int i = 0; i < 22; i++)
            {
                double speed = DspBattlePlugin.randSeed.NextDouble() - 0.5;
                if (speed < 0.2 && speed > -0.2)
                    speed *= 2;
                layerRotateSpeed.Add(speed);
            }
            targetUPos = new VectorLF3(0,0,0);
            targetSwarm = null;
            starCannonStarIndex = -1;
            starCannonLevel = 0;
            time = 0;
            fireStage = 0;
            InitUI();
        }
        public static void InitUI()
        {
            if (fireButtonObj != null) return;

            GameObject alertUIObj = GameObject.Find("UI Root/Overlay Canvas/In Game/AlertUI");
            GameObject oriButton = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Station Window/storage-box-0/popup-box/sd-option-button-1");

            if (alertUIObj == null || oriButton == null) return;

            fireButtonObj = GameObject.Instantiate(oriButton);
            fireButtonObj.name = "FireButton";
            fireButtonObj.transform.SetParent(alertUIObj.transform, false);
            fireButtonObj.transform.localPosition = new Vector3(-80, -110, 0);
            fireButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 40);

            fireButton = fireButtonObj.GetComponent<Button>();
            fireButton.onClick.RemoveAllListeners();
            fireButton.onClick.AddListener(() => { OnFireButtonClick(); });
            fireButtonText = fireButtonObj.transform.Find("button-text").GetComponent<Text>();
            fireButtonText.text = "恒星炮开火".Translate();
            fireButtonText.fontSize = 18;
            //fireButtonText.resizeTextMinSize = 16;
            //fireButtonText.resizeTextMaxSize = 18;
            fireButtonText.resizeTextForBestFit = false;
            fireButtonImage = fireButtonObj.GetComponent<Image>();
            fireButtonImage.color = cannonReadyColor;

        }
        public static void OnFireButtonClick()
        {
            if(starCannonStarIndex<0)
            {
                UIRealtimeTip.Popup("没有规划的恒星炮！".Translate());
                return;
            }
            if(starCannonLevel <= 0)
            {
                UIRealtimeTip.Popup("恒星炮需要至少修建至第一阶段才能够开火！".Translate());
                return;
            }
            switch (fireStage)
            {
                case 1: //原本计划是可以在瞄准初期取消开火，但每次瞄准的时间不一样，可供取消的时间窗口也不一样，不如不加这个功能
                    //UIRealtimeTip.Popup("已取消开火！".Translate());
                    //fireStage = 100; //将取消开火
                    //return;
                case 2:
                case 3:
                case 4:
                case 5:
                    UIRealtimeTip.Popup("恒星炮已经启动".Translate());
                    return;
                case -2:
                    UIRealtimeTip.Popup("恒星炮冷却中！".Translate());
                    return;
                case -1:
                    UIRealtimeTip.Popup("恒星炮充能中！".Translate());
                    return;
            }
            int tarSet = TrySetNextTarget();
            switch (tarSet) //如果能按下按钮，则不会出现 case -3。
            {
                case -1:
                    UIRealtimeTip.Popup("没有目标！".Translate());
                    return;
                case -2:
                    UIRealtimeTip.Popup("超出射程！".Translate());
                    return;
                case 1:
                    StartAiming();
                    UIRealtimeTip.Popup("恒星炮已启动".Translate());
                    return;
            }
            
            //UIRealtimeTip.Popup("nope!");
        }


        public static void UpdateStarCannonLevel(DysonSphere sphere)
        {
            starCannonLevel = 1;
        }

        public static void RefreshFireButtonUI()
        {
            if (fireButton == null || fireButtonImage == null || fireButtonText == null) return;
            if(starCannonStarIndex < 0)
            {
                fireButtonImage.color = cannonDisableColor;
                fireButtonText.text = "恒星炮未规划".Translate();
                fireButtonText.fontSize = 18;
                return;
            }
            else if(starCannonLevel <= 0)
            {
                fireButtonImage.color = cannonDisableColor;
                fireButtonText.text = "恒星炮建设中".Translate();
                fireButtonText.fontSize = 18;
                return;
            }
            switch (fireStage)
            {
                case -2:
                    fireButtonImage.color = cannonChargingColor;
                    fireButtonText.text = "  " + "恒星炮冷却中".Translate() + "   " + $"{(-chargingTimeNeed - time) / 60 / 60:00}:{(-chargingTimeNeed - time) / 60 % 60:00}" + "  ";
                    fireButtonText.fontSize = 16;
                    break;
                case -1:
                    fireButtonImage.color = cannonChargingColor;
                    fireButtonText.text = "  " + "恒星炮充能中".Translate() + "   " + $"{(-time) / 60 / 60:00}:{(-time) / 60 % 60:00}" + "  ";
                    fireButtonText.fontSize = 16;
                    break;
                case 0:
                    fireButtonImage.color = cannonReadyColor;
                    fireButtonText.text = "恒星炮开火".Translate();
                    fireButtonText.fontSize = 18;
                    break;
                case 1:
                case 2:
                    fireButtonImage.color = cannonAimingColor;
                    fireButtonText.text = "正在瞄准".Translate();
                    fireButtonText.fontSize = 18;
                    break;
                case 3:
                case 4:
                case 5:
                    fireButtonImage.color = cannonFiringColor;
                    fireButtonText.text = "正在开火".Translate();
                    fireButtonText.fontSize = 18;
                    break;
            }
        }


        public static int TrySetNextTarget()
        {
            if (Configs.nextWaveState < 2)
            {
                return -1; //没有目标
            }
            else if(starCannonStarIndex < 0)
            {
                return -9; //其他错误
            }
            else
            {
                double distance = (GameMain.galaxy.stars[starCannonStarIndex].uPosition - GameMain.galaxy.stars[Configs.nextWaveStarIndex].uPosition).magnitude / 40000.0 / 60.0;
                if (distance > maxRange)
                {
                    return -2; //射程不足
                }

                int alreadyDestoryCount = WormholeProperties.initialWormholeCount - Configs.nextWaveWormCount;
                if (alreadyDestoryCount >= maxAimCount)
                {
                    return -3; //达到连续开火次数上限
                }

                if (Configs.nextWaveStarIndex < 0 || GameMain.data.dysonSpheres.Length <= Configs.nextWaveStarIndex) return -9;
                DysonSphere targetSphere = GameMain.data.dysonSpheres[Configs.nextWaveStarIndex];
                if (targetSphere != null && targetSphere.swarm != null)
                    targetSwarm = targetSphere.swarm;
                else
                    targetSwarm = null;

                targetUPos = WormholeUIPatch.starData[Configs.nextWaveWormCount - 1].uPosition;

                return 1;
            }
        }

        /// <summary>
        /// 开始瞄准与开火进程
        /// </summary>
        public static void StartAiming()
        {
            //if (fireStage != 0) return; //恒星炮不在待命状态，不允许进入瞄准状态 
            
            
            time = 0;
            endAimTime = 999;
            fireStage = 1; //进入瞄准阶段
            rotateSpeedScale = 1;
            layerRotateSpeed = new List<double>();
            for (int i = 0; i < 22; i++)
            {
                double speed = DspBattlePlugin.randSeed.NextDouble() - 0.5;
                if (speed < 0.2 && speed > -0.2)
                    speed *= 2;
                layerRotateSpeed.Add(speed);
            }
        }


        public static void StopFiring(ref DysonSphere sphere)
        {
            //每层旋转到随机位置
            for (int i = 0; i < sphere.layersIdBased.Length; i++)
            {
                if (sphere.layersIdBased[i] != null)
                {
                    DysonSphereLayer layer = sphere.layersIdBased[i];
                    Quaternion randRotation = Quaternion.LookRotation( new VectorLF3(DspBattlePlugin.randSeed.NextDouble() - 0.5f, DspBattlePlugin.randSeed.NextDouble() - 0.5f, DspBattlePlugin.randSeed.NextDouble() - 0.5f));
                    layer.orbitAngularSpeed *= 5.0f; //加快轨道旋转速度，只在下面计算时用到一次，之后可以立刻还原
                    layer.InitOrbitRotation(layer.orbitRotation, randRotation); //每个戴森壳层开始随机旋转到任意方向，这个随机也不是平均分布的……
                    layer.orbitAngularSpeed /= 5.0f; //轨道旋转速度还原
                }
            }

            if (fireStage == 1) //代表刚瞄准还没预热就停止“开火”，因此不需要重新充能
            {
                fireStage = 0;
                return;
            }

            //如果至少进入过预热阶段（fireStage=2的阶段），则必须经过完整的冷却和再充能过程，才能再次瞄准、开火
            fireStage = -2;
            time = -cooldownTimeNeed - chargingTimeNeed;
            
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSphere), "GameTick")]
        public static void DysonSphereStarCannonFire(ref DysonSphere __instance, long gameTick)
        {
            int starIndex = __instance.starData.index;
            if (MoreMegaStructure.MoreMegaStructure.StarMegaStructureType[starIndex] != 6) //Type==6为恒星炮
                return;

            if (__instance == null || __instance.layersIdBased == null)
                return;

            if (__instance.energyGenCurrentTick < 10000)
                return;

            UpdateStarCannonLevel(__instance); //根据恒星炮的能量输出和层级等属性，确定恒星炮的等级
            starCannonStarIndex = starIndex;


            if ( (Configs.nextWaveState != 2 && Configs.nextWaveState != 3 && fireStage > 0) || fireStage >= 9 || starCannonLevel <= 0)
            {
                StopFiring(ref __instance);
                return;
            }

            //每帧更新目标虫洞的坐标，目前只打列表中的最后一个
            if(Configs.nextWaveWormCount > 0 && Configs.nextWaveWormCount < WormholeUIPatch.starData.Length)
                targetUPos = WormholeUIPatch.starData[Configs.nextWaveWormCount - 1].uPosition;

            if (fireStage != 0 && fireStage != -1) //每个壳层都需要做的
            {
                VectorLF3 direction = targetUPos - __instance.starData.uPosition;
                VectorLF3 vert = new VectorLF3(0, 0, 1);
                if (direction.z != 0)
                    vert = new VectorLF3(1, 1, (-direction.x - direction.y) / direction.z);
                Quaternion final = Quaternion.LookRotation(vert, direction);

                for (int i = 0; i < __instance.layersIdBased.Length; i++)
                {
                    if (__instance.layersIdBased[i] != null)
                    {
                        DysonSphereLayer layer = __instance.layersIdBased[i];

                        if (fireStage == 1 && time <= 1) //原本第二个条件是是time==0，但可能会出现不进行瞄准动画的问题，因此改成了<=1，大不了瞄准两次
                        {
                            layer.orbitAngularSpeed *= 10.0f; //加快轨道旋转速度，只在下面计算时用到一次，之后可以立刻还原

                            layer.InitOrbitRotation(layer.orbitRotation, final); //每个戴森壳层开始轨道旋转、对齐瞄准
                            float aimTimeNeed = Quaternion.Angle(layer.orbitRotation, final) / layer.orbitAngularSpeed * 60f;
                            endAimTime = Mathf.Max(endAimTime, (int)aimTimeNeed); //保存瞄准完成所需的最大时间

                            layer.orbitAngularSpeed /= 10.0f; //轨道旋转速度还原
                        }

                        //旋转
                        if (fireStage >= 2 || fireStage == -2)
                        {
                            layer.currentAngle += rotateSpeedScale * (float)layerRotateSpeed[i];
                        }

                        //目标锁定和旋转速度设置
                        if (fireStage > 0 && fireStage <=3) //如果不是连续开火的瞄准阶段，也不是停火后的冷却阶段
                        {
                            layer.orbitRotation = final; //瞄准方向锁定在目标上
                        }

                        if (fireStage >= 2)//加速旋转
                        {
                            if (rotateSpeedScale < 15f)
                                rotateSpeedScale += 0.01f;
                        }
                        else if (fireStage < 0) //冷却、停止开火阶段，减速旋转
                        {
                            if (rotateSpeedScale > 0.5f)
                                rotateSpeedScale -= 0.02f;
                        }
                        
                        //连续开火的再瞄准开始
                        if(fireStage == 4)
                        {
                            float oriAngularSpeed = layer.orbitAngularSpeed;
                            layer.orbitAngularSpeed = reAimAngularSpeed; //所有轨道在连续瞄准期间保持同样的瞄准速度

                            layer.InitOrbitRotation(layer.orbitRotation, final); //每个戴森壳层开始轨道旋转、对齐瞄准
                            float aimTimeNeed = Quaternion.Angle(layer.orbitRotation, final) / layer.orbitAngularSpeed * 60f;
                            endAimTime = Mathf.Max(endAimTime, (int)aimTimeNeed); //保存瞄准完成所需的最大时间

                            layer.orbitAngularSpeed = oriAngularSpeed; //轨道旋转速度还原

                            endAimTime = (int)aimTimeNeed;
                            time = 0;
                        }

                    }
                }
            }
            if (fireStage == 3) //开火阶段
            {
                //造成伤害（瞬间造成，无视弹道速度）
                int hitResult = WormholeProperties.TryTakeDamage(damagePerTick);
                if(hitResult == 1)//代表摧毁了一个虫洞
                {
                    int result = TrySetNextTarget();
                    if (result == 1) //找到了下一个目标，则进入连续开火的瞄准阶段
                    {
                        fireStage = 4;
                    }
                    else //超过了连续开火次数上限，则停止开火进入冷却和在充能阶段
                    {
                        StopFiring(ref __instance);
                    }
                    return;
                }

                //激光效果
                for (int i = 0; i < 20; i++)
                {
                    int bulletIndex = __instance.swarm.AddBullet(new SailBullet
                    {
                        maxt = 0.3f,
                        lBegin = __instance.starData.uPosition,
                        uEndVel = targetUPos,
                        uBegin = __instance.starData.uPosition + new VectorLF3(DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20),
                        uEnd = targetUPos + new VectorLF3(DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20)
                    }, 0);
                    __instance.swarm.bulletPool[bulletIndex].state = 0;
                }
                //如果不在同星系，则本星系内光会很细，增加一段短光
                if (targetSwarm == null || targetSwarm.starData.index != __instance.starData.index)
                {
                    int nearPoint = 400000;
                    if (__instance.starData.type == EStarType.GiantStar)
                        nearPoint = 1000000;
                    for (int i = 0; i < 20; i++)
                    {
                        int bulletIndex = __instance.swarm.AddBullet(new SailBullet
                        {
                            maxt = 0.3f,
                            lBegin = __instance.starData.uPosition,
                            uEndVel = targetUPos,
                            uBegin = __instance.starData.uPosition + new VectorLF3(DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20),
                            uEnd = (targetUPos - __instance.starData.uPosition).normalized * nearPoint + __instance.starData.uPosition + new VectorLF3(DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20)
                        }, 0);
                        __instance.swarm.bulletPool[bulletIndex].state = 0;
                    }
                }
                //如果不在同星系，接收星系需要同样生成光束（由于发射星系的光束不会在观察目标星系时渲染）
                if (targetSwarm != null && targetSwarm.starData.index != __instance.starData.index)
                {
                    //无需改变生成点和终点
                    for (int i = 0; i < 20; i++)
                    {
                        int bulletIndex = targetSwarm.AddBullet(new SailBullet
                        {
                            maxt = 0.3f,
                            lBegin = __instance.starData.uPosition,
                            uEndVel = targetUPos,
                            uBegin = __instance.starData.uPosition + new VectorLF3(DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20),
                            uEnd = targetUPos + new VectorLF3(DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20, DspBattlePlugin.randSeed.NextDouble() * 20)
                        }, 0);
                        targetSwarm.bulletPool[bulletIndex].state = 0;
                    }
                }
            }

            //结算阶段
            time += 1;

            if (fireStage == 1 && time >= endAimTime - 100) //瞄准完成，进入预热（加速旋转）阶段
            {
                fireStage = 2;
            }
            else if (fireStage == 2 && time >= endAimTime + warmTimeNeed) //开火
            {
                fireStage = 3;
            }
            else if (fireStage == 4)
            {
                fireStage = 5;
            }
            else if (fireStage == 5 && time >= endAimTime)//连续瞄准完成，继续开火
            {
                fireStage = 3;
            }
            else if (fireStage == -2 && time >= -chargingTimeNeed)
            {
                fireStage = -1;
            }
            else if (fireStage == -1 && time >= 0)
            {
                fireStage = 0;
                time = 0;
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RefreshAllWhenGameTick()
        {
            for (int i = 0; i < 200; i++) //其实能支持1000个星系
            {
                if(MoreMegaStructure.MoreMegaStructure.StarMegaStructureType[i] == 6)
                {
                    starCannonStarIndex = i;
                    RefreshFireButtonUI();
                    return;
                }
            }
            starCannonStarIndex = -1;
            RefreshFireButtonUI();
        }

      



        public static void Export(BinaryWriter w)
        {

        }

        public static void Import(BinaryReader r)
        {
            InitAll();
            if (Configs.versionWhenImporting >= 20220321)
            {

            }
            else
            {

            }
        }

        public static void IntoOtherSave()
        {
            InitAll();
        }
    }
}
