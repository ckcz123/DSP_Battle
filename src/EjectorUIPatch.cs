using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    class EjectorUIPatch
    {
        public static int curEjectorPlanetId = -1;
        public static int curEjectorEntityId = -1;
        public static int curEjectorPoolId = -1;
        public static bool needToRefreshTarget = false;
        public static bool curEjectorIsCannon = false;
        public static EnemyShip curTarget = null;

        public static Text SolarSailAmoutLabel = null; //原本显示的是太阳帆总数的标题，现在要改成“剩余生命值”之类的
        public static Text EjectCycleLabel = null; //原本是显示弹射周期的标题，现在要改成“射速”字样

        public static GameObject orbitalPickerObj = null;
        public static GameObject remainEnemyShipsLabelObj = null;
        public static GameObject remainEnemyShipsValueObj = null;
        public static Text remainEnemyShipsLabel = null;
        public static Text remainEnemyShipsValue = null;

        public static GameObject setAimingModeLabelObj;
        public static Text setAimingModeLabel;
        public static GameObject setModeButton1Obj;
        public static Button setModeButton1;
        public static GameObject setModeButton2Obj;
        public static Button setModeButton2;
        public static GameObject setModeButton3Obj;
        public static Button setModeButton3;
        public static GameObject setModeButton4Obj;
        public static Button setModeButton4;

        public static void InitGameObjects()
        {
            try
            {
                GameObject SolarSailAmoutLabelObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/detail-texts/label (5)");
                SolarSailAmoutLabel = SolarSailAmoutLabelObj.GetComponent<Text>();
                GameObject EjectCycleLabelObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/detail-texts/label (4)");
                EjectCycleLabel = EjectCycleLabelObj.GetComponent<Text>();

                //下面初始化一组显示剩余敌机总数的UI
                GameObject parent = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window");
                GameObject EjectCycleValueObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/detail-texts/value (4)");
                remainEnemyShipsLabelObj = GameObject.Instantiate(EjectCycleLabelObj);
                remainEnemyShipsLabelObj.name = "remain-enemy-label";
                remainEnemyShipsLabelObj.transform.SetParent(parent.transform, false);
                remainEnemyShipsLabelObj.transform.localPosition = new Vector3(-170, -113, 0);
                remainEnemyShipsLabel = remainEnemyShipsLabelObj.GetComponent<Text>();

                remainEnemyShipsValueObj = GameObject.Instantiate(EjectCycleValueObj);
                remainEnemyShipsValueObj.name = "remain-enemy-value";
                remainEnemyShipsValueObj.transform.SetParent(parent.transform, false);
                remainEnemyShipsValueObj.transform.localPosition = new Vector3(-30, -113, 0);
                remainEnemyShipsValue = remainEnemyShipsValueObj.GetComponent<Text>();


                orbitalPickerObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/orbit-picker");

                //下面初始化选择攻击目标逻辑的UI
                GameObject oriTitleLabelObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/orbit-picker/title");
                setAimingModeLabelObj = GameObject.Instantiate(oriTitleLabelObj);
                setAimingModeLabelObj.name = "setModeLabel";
                setAimingModeLabelObj.transform.SetParent(parent.transform, false);
                setAimingModeLabelObj.transform.localPosition = new Vector3(-170, 30, 0);
                setAimingModeLabel = setAimingModeLabelObj.GetComponent<Text>();


                GameObject oriOrbitalSelectButtonObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Ejector Window/orbit-picker/button (1)");
                setModeButton1Obj = GameObject.Instantiate(oriOrbitalSelectButtonObj);
                setModeButton1Obj.name = "setMode1";
                setModeButton1Obj.transform.SetParent(parent.transform, false);
                setModeButton1Obj.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 20);
                setModeButton1Obj.transform.localPosition = new Vector3(-170, 0, 0);
                setModeButton1 = setModeButton1Obj.GetComponent<Button>();

                setModeButton2Obj = GameObject.Instantiate(oriOrbitalSelectButtonObj);
                setModeButton2Obj.name = "setMode2";
                setModeButton2Obj.transform.SetParent(parent.transform, false);
                setModeButton2Obj.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 20);
                setModeButton2Obj.transform.localPosition = new Vector3(-170, -30, 0);
                setModeButton2 = setModeButton2Obj.GetComponent<Button>();

                setModeButton3Obj = GameObject.Instantiate(oriOrbitalSelectButtonObj);
                setModeButton3Obj.name = "setMode3";
                setModeButton3Obj.transform.SetParent(parent.transform, false);
                setModeButton3Obj.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 20);
                setModeButton3Obj.transform.localPosition = new Vector3(-170, -60, 0);
                setModeButton3 = setModeButton3Obj.GetComponent<Button>();

                setModeButton4Obj = GameObject.Instantiate(oriOrbitalSelectButtonObj);
                setModeButton4Obj.name = "setMode4";
                setModeButton4Obj.transform.SetParent(parent.transform, false);
                setModeButton4Obj.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 20);
                setModeButton4Obj.transform.localPosition = new Vector3(-170, -90, 0);
                setModeButton4 = setModeButton4Obj.GetComponent<Button>();

                setModeButton1.onClick.RemoveAllListeners();
                setModeButton1.onClick.AddListener(() => { SetAimingMode(1); });
                setModeButton2.onClick.RemoveAllListeners();
                setModeButton2.onClick.AddListener(() => { SetAimingMode(2); });
                setModeButton3.onClick.RemoveAllListeners();
                setModeButton3.onClick.AddListener(() => { SetAimingMode(3); });
                setModeButton4.onClick.RemoveAllListeners();
                setModeButton4.onClick.AddListener(() => { SetAimingMode(4); });

            }
            catch (Exception)
            {
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEjectorWindow), "OnEjectorIdChange")]
        public static void OnEjectorIdChangePostPatch(UIEjectorWindow __instance)
        {
            ShareUICurEjectorInfos(__instance);
        }


        public static void ShareUICurEjectorInfos(UIEjectorWindow __instance)
        {
            if (SolarSailAmoutLabel == null)
            {
                InitGameObjects();
            }
            try
            {
                curEjectorIsCannon = true;
                EjectorComponent ejectorComponent = __instance.factorySystem.ejectorPool[__instance.ejectorId];
                int gmProtoId = __instance.factory.entityPool[ejectorComponent.entityId].protoId;
                if (gmProtoId == 2311) //原版弹射器不进行修改
                {
                    curTarget = null;
                    curEjectorIsCannon = false;
                }
                curEjectorPlanetId = ejectorComponent.planetId;
                curEjectorPoolId = __instance.ejectorId;
                curEjectorEntityId = ejectorComponent.entityId; //二者均相符时，代表是同一个建筑
                needToRefreshTarget = true;//在ejector选择目标时，如果needToRefreshTarget，则将选择的目标刷新传递过来，供UI显示所需属性，同时将此项设置为false，不重复刷新
                RefreshEjectorUIOnce();

            }
            catch (Exception)
            {
                curEjectorIsCannon = false;
            }

        }


        public static void RefreshEjectorUIOnce()
        {
            if (curEjectorIsCannon)
            {
                int orbitId = GameMain.data.galaxy.PlanetById(curEjectorPlanetId).factory.factorySystem.ejectorPool[curEjectorPoolId].orbitId;
                orbitalPickerObj.SetActive(false);
                setAimingModeLabelObj.SetActive(true);
                setModeButton1Obj.SetActive(true);
                setModeButton1Obj.GetComponent<UIButton>().highlighted = orbitId == 1;
                setModeButton2Obj.SetActive(true);
                setModeButton2Obj.GetComponent<UIButton>().highlighted = orbitId == 2;
                setModeButton3Obj.SetActive(true);
                setModeButton3Obj.GetComponent<UIButton>().highlighted = orbitId == 3;
                setModeButton4Obj.SetActive(true);
                setModeButton4Obj.GetComponent<UIButton>().highlighted = orbitId == 4;
                remainEnemyShipsValueObj.SetActive(true);
                remainEnemyShipsLabelObj.SetActive(true);
                EjectCycleLabel.text = "射速".Translate();
                remainEnemyShipsLabel.text = "剩余敌人".Translate();
                setAimingModeLabel.text = "设定索敌最高优先级".Translate();
                setModeButton1Obj.transform.Find("Text").GetComponent<Text>().text = "最接近物流塔".Translate();
                setModeButton2Obj.transform.Find("Text").GetComponent<Text>().text = "最大威胁".Translate();
                setModeButton3Obj.transform.Find("Text").GetComponent<Text>().text = "距自己最近".Translate();
                setModeButton4Obj.transform.Find("Text").GetComponent<Text>().text = "最低生命".Translate();

            }
            else
            {
                orbitalPickerObj.SetActive(true);
                setAimingModeLabelObj.SetActive(false);
                setModeButton1Obj.SetActive(false);
                setModeButton2Obj.SetActive(false);
                setModeButton3Obj.SetActive(false);
                setModeButton4Obj.SetActive(false);
                remainEnemyShipsValueObj.SetActive(false);
                remainEnemyShipsLabelObj.SetActive(false);
                SolarSailAmoutLabel.text = "太阳帆总数".Translate();
                EjectCycleLabel.text = "弹射周期".Translate();

            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEjectorWindow), "_OnUpdate")]
        public static void DisplayTargetShipHp(ref UIEjectorWindow __instance)
        {
            if (curEjectorIsCannon)
            {

                EjectorComponent ejectorComponent = __instance.factorySystem.ejectorPool[__instance.ejectorId];
                PowerConsumerComponent powerConsumerComponent = __instance.powerSystem.consumerPool[ejectorComponent.pcId];
                int networkId = powerConsumerComponent.networkId;
                PowerNetwork powerNetwork = __instance.powerSystem.netPool[networkId];
                float num = (powerNetwork != null && networkId > 0) ? ((float)powerNetwork.consumerRatio) : 0f;

                if (ejectorComponent.bulletId == 8007) //脉冲炮的子弹，即使是0也不显示缺少子弹
                {
                    if (num < 0.1f)
                    {
                        __instance.stateText.text = "电力不足".Translate();
                        __instance.stateText.color = __instance.powerOffColor;
                        __instance.valueText1.text = "-";
                        __instance.valueText2.text = "-";
                        __instance.valueText2.color = __instance.idleColor;
                        __instance.valueText3.color = __instance.idleColor;
                        __instance.chargeFg.color = __instance.powerOffColor * 0.5f;
                    }
                    else if (ejectorComponent.targetState == EjectorComponent.ETargetState.None)
                    {
                        __instance.stateText.text = "待机".Translate();
                        __instance.stateText.color = __instance.workStoppedColor;
                        __instance.valueText1.text = "-";
                        __instance.valueText2.text = "-";
                        __instance.valueText2.color = __instance.idleColor;
                        __instance.valueText3.color = __instance.idleColor;
                    }
                    else if (ejectorComponent.targetState == EjectorComponent.ETargetState.Blocked)
                    {
                        __instance.stateText.text = "路径被遮挡".Translate();
                        __instance.stateText.color = __instance.workStoppedColor;
                        __instance.valueText1.text = (ejectorComponent.targetDist / 40000.0).ToString("0.00 AU");
                        __instance.valueText2.text = "路径被遮挡".Translate();
                        __instance.valueText2.color = __instance.workStoppedColor;
                        __instance.valueText3.color = __instance.idleColor;
                    }
                    else if (ejectorComponent.targetState == EjectorComponent.ETargetState.AngleLimit)
                    {
                        __instance.stateText.text = "俯仰限制".Translate();
                        __instance.stateText.color = __instance.workStoppedColor;
                        __instance.valueText1.text = (ejectorComponent.targetDist / 40000.0).ToString("0.00 AU");
                        __instance.valueText2.text = "俯仰限制".Translate();
                        __instance.valueText2.color = __instance.workStoppedColor;
                        __instance.valueText3.color = __instance.workStoppedColor;
                    }
                    else if (ejectorComponent.direction != 0)
                    {
                        if (num == 1f)
                        {
                            __instance.stateText.text = "开火中gm".Translate(); //此处原本是充能中和冷却中
                            __instance.stateText.color = __instance.workNormalColor;
                        }
                        else
                        {
                            __instance.stateText.text = "电力不足".Translate();
                            __instance.stateText.color = __instance.powerLowColor;
                        }
                        __instance.valueText1.text = (ejectorComponent.targetDist / 40000.0).ToString("0.00 AU");
                        __instance.valueText2.text = "可弹射".Translate();
                        __instance.valueText2.color = __instance.idleColor;
                        __instance.valueText3.color = __instance.idleColor;
                    }
                    else
                    {
                        __instance.stateText.text = "待机".Translate();
                        __instance.stateText.color = __instance.idleColor;
                        __instance.valueText1.text = "-";
                        __instance.valueText2.text = "-";
                        __instance.valueText2.color = __instance.idleColor;
                        __instance.valueText3.color = __instance.idleColor;
                    }
                }


                if (curTarget != null && curTarget.state == EnemyShip.State.active)// && EnemyShips.ships.ContainsKey(curTarget.shipIndex)
                {
                    SolarSailAmoutLabel.text = "目标生命值".Translate();
                    __instance.valueText6.text = curTarget.hp.ToString();
                }
                else
                {
                    curTarget = null;
                    needToRefreshTarget = true;
                    SolarSailAmoutLabel.text = "无攻击目标".Translate();
                    __instance.valueText6.text = "-";
                }
                remainEnemyShipsValue.text = EnemyShips.ships.Count.ToString();
                //Main.logger.LogInfo($"cur orbit id is {curEjector.orbitId}");

                float num2 = 60f / (float)(ejectorComponent.chargeSpend + ejectorComponent.coldSpend) * 600000f;
                num2 *= (float)(Cargo.incTableMilli[ejectorComponent.incLevel] + 1.0);
                __instance.valueText5.text = num2.ToString("0.0") + "每分钟".Translate();

            }
            else
            {
            }
        }

        public static void SetAimingMode(int modenum)
        {
            if (curEjectorIsCannon)
            {
                try
                {
                    GameMain.data.galaxy.PlanetById(curEjectorPlanetId).factory.factorySystem.ejectorPool[curEjectorPoolId].SetOrbit(modenum);
                    RefreshEjectorUIOnce();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
