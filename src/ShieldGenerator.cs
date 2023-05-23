using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace DSP_Battle
{
    public class ShieldGenerator
    {
        //需要存档
        public static ConcurrentDictionary<int, int> currentShield = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, int> maxShieldCapacity = new ConcurrentDictionary<int, int>();

        //无需存档，每帧清零并重新计算
        public static ConcurrentDictionary<int, int> calcShieldCapacity = new ConcurrentDictionary<int, int>();
        public static ConcurrentDictionary<int, double> calcShieldInc = new ConcurrentDictionary<int, double>();
        public static ConcurrentDictionary<int, int> calcShieldGenCount = new ConcurrentDictionary<int, int>();


        public static int curShieldIncUI = 0; //由于每帧会清零calc的三个Dictionary，UI无法从中获取数值并更新显示，因此在清零前将必要信息暂时保存在这两个int中，供UI刷新显示数值使用
        public static int curShieldGenCntUI = 0;

        public static GameObject modeButtonsLeftBarObj = null;
        public static GameObject switchButton1Obj = null;
        public static GameObject switchButton2Obj = null;
        public static GameObject switchButton3Obj = null;
        //public static GameObject arrow1Obj = null;
        //public static GameObject arrow2Obj = null;
        //public static GameObject emptyItemObj = null;
        //public static GameObject fullItemObj = null;
        public static GameObject emptyItemCntTextObj = null;
        public static GameObject fullItemCntTextObj = null;
        public static Image circleBack = null;
        public static Image circlePowerIcon = null;
        public static Image circleFg = null;
        public static Image circleFg2 = null;
        public static Image circleFg3 = null;
        public static Text switchButton2Text = null;
        public static Text switchButton3Text = null;
        public static Text powerNetworkTitle = null;
        public static Text powerNetworkTitle2 = null;
        public static Text powerNetworkLabel1 = null;
        public static Text powerNetworkLabel2 = null;
        public static Text powerNetworkLabel3 = null;
        public static Text powerNetworkLabel4 = null;
        public static Text powerNetworkLabel5 = null;
        public static Text powerNetworkValue1 = null;
        public static Text powerNetworkValue2 = null;
        public static Text powerNetworkValue3 = null;
        public static Text powerNetworkValue4 = null;
        public static Text powerNetworkValue5 = null;

        public static GameObject powerCircleLegend3Obj = null;
        public static GameObject powerCircleLegend4Obj = null;
        public static GameObject powerCircleAccInObj = null;
        public static GameObject powerCircleAccOutObj = null;
        public static Image powerCircleGen = null;
        public static Image powerCircleCons = null;
        public static Image powerCircleLegend2Bar = null;
        public static Text powerCircleCenterValue = null;
        public static Text powerCircleCenterLabel = null;
        public static Text powerCircleLegend1Label = null;
        public static Text powerCircleLegend2Label = null;


        public static Color oriBack = new Color(0, 0, 0, 0.1f);
        public static Color oriFg = new Color(0.9906f, 0.5897f, 0.3691f, 0.8549f);
        public static Color oriFg2 = new Color(0.9906f, 0.5897f, 0.3691f, 0.1255f);
        public static Color oriFg3 = new Color(0.9906f, 0.5897f, 0.3691f, 0.5098f);
        public static Color oriConsumeCircle = new Color(0.9906f, 0.5897f, 0.3691f, 0.4706f);
        public static Color oriActiveHighlight = new Color(0.9922f, 0.5882f, 0.3686f, 0.4235f);

        public static Color shieldBack = new Color(0f, 0.414f, 0.8548f, 0.1f);
        public static Color shieldFg = new Color(0f, 0.7358f, 0.8868f, 0.8549f);
        public static Color shieldFg2 = new Color(0f, 0.6097f, 0.9291f, 0.1255f);
        public static Color shieldFg3 = new Color(0f, 0.6097f, 0.9291f, 0.5098f);
        public static Color shieldCurCircle = new Color(0, 0.369f, 1f, 1f);
        public static Color shieldActiveHighlight = new Color(0.1312f, 0.6682f, 0.94f, 0.4235f);

        public static bool isShieldUI = false;
        public static int shieldUIPlanetId = 0;

        public static void InitAll()
        {
            currentShield = new ConcurrentDictionary<int, int>();
            maxShieldCapacity = new ConcurrentDictionary<int, int>();
            calcShieldCapacity = new ConcurrentDictionary<int, int>();
            calcShieldInc = new ConcurrentDictionary<int, double>();
            calcShieldGenCount = new ConcurrentDictionary<int, int>();
            shieldUIPlanetId = 0;
            curShieldIncUI = 0;
            curShieldGenCntUI = 0;
            InitUI();
        }

        public static void InitUI()
        {
            if (modeButtonsLeftBarObj != null)
                return;
            modeButtonsLeftBarObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/tap");
            //arrow1Obj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/transiton-0");
            //arrow2Obj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/transiton-1");
            //emptyItemObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/empty");
            //fullItemObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/full");
            emptyItemCntTextObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/empty/empty-icon/cnt-text");
            fullItemCntTextObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/full/full-icon/cnt-text");

            switchButton1Obj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/switch-button-1");
            switchButton2Obj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/switch-button-2");
            switchButton3Obj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/switch-button-3");
            switchButton2Text = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/switch-button-2/button-text").GetComponent<Text>();
            switchButton3Text = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/switch-button-3/button-text").GetComponent<Text>();

            circleBack = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/power-state/circle-back").GetComponent<Image>();
            circlePowerIcon = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/power-state/circle-back/power-icon").GetComponent<Image>();
            circleFg = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/power-state/circle-back/circle-fg").GetComponent<Image>();
            circleFg2 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/power-state/circle-back/circle-fg-2").GetComponent<Image>();
            circleFg3 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/exchanger-desc/power-state/circle-back/circle-fg-3").GetComponent<Image>();

            powerNetworkTitle = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/title-text").GetComponent<Text>();
            powerNetworkTitle2 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/label-0").GetComponent<Text>();
            powerNetworkLabel1 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/label-1").GetComponent<Text>();
            powerNetworkLabel2 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/label-2").GetComponent<Text>();
            powerNetworkLabel3 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/label-3").GetComponent<Text>();
            powerNetworkLabel4 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/label-4").GetComponent<Text>();
            powerNetworkLabel5 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/label-5").GetComponent<Text>();
            powerNetworkValue1 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/value-1").GetComponent<Text>();
            powerNetworkValue2 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/value-2").GetComponent<Text>();
            powerNetworkValue3 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/value-3").GetComponent<Text>();
            powerNetworkValue4 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/value-4").GetComponent<Text>();
            powerNetworkValue5 = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/value-5").GetComponent<Text>();

            powerCircleLegend3Obj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/gen-circle/legend-3");
            powerCircleLegend4Obj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/gen-circle/legend-4");
            powerCircleAccInObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/acc-in-circle");
            powerCircleAccOutObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/acc-out-circle");
            powerCircleGen = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/gen-circle").GetComponent<Image>();
            powerCircleCons = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/cons-circle").GetComponent<Image>();
            powerCircleLegend2Bar = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/gen-circle/legend-2").GetComponent<Image>();
            powerCircleCenterValue = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/cons-circle/value-c").GetComponent<Text>();
            powerCircleCenterLabel = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/cons-circle/label-c").GetComponent<Text>();
            powerCircleLegend1Label = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/gen-circle/legend-1/label").GetComponent<Text>();
            powerCircleLegend2Label = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Exchanger Window/power-network-desc/gen-circle/legend-2/label").GetComponent<Text>();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "NewExchangerComponent")]
        public static void MarkShieldGenByPlanetId(ref PowerSystem __instance, int __result)
        {
            if (__instance.excPool[__result].emptyId == 9999)
                __instance.excPool[__result].fullId = __instance.planet.id; //将planetId存储在fullId里，而emptyId则是9999（在modelProto里已经设定好）来代表这个是护盾产生器
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerExchangerComponent), "InputUpdate")]
        public static void ShieldGenPowerUpdatePatch1(ref PowerExchangerComponent __instance)
        {
            if (__instance.emptyId != 9999)
                return;
            if (Configs.nextWaveStarIndex == __instance.fullId / 100 - 1 && Configs.nextWaveState == 3) //战斗中护盾回复动画停止
            {
                __instance.currPoolEnergy = __instance.currPoolEnergy - __instance.currEnergyPerTick;
            }
            if (__instance.currPoolEnergy >= __instance.maxPoolEnergy || __instance.currPoolEnergy <= 0)
                __instance.currPoolEnergy = __instance.currEnergyPerTick;
            if (GameMain.instance.timei % 30 != 1) return;

            if (__instance.currPoolEnergy >= __instance.maxPoolEnergy - __instance.energyPerTick)
                __instance.currPoolEnergy = 0;

            //提供shieldCapacity
            int planetId = __instance.fullId;
            int capacityProvided = 0;
            int beforeCapacity = calcShieldCapacity.GetOrAdd(planetId, 0);
            for (int i = 0; i < Configs.capacityPerGenerator.Count; i++)
            {
                if (beforeCapacity >= Configs.capacityPerGenerator[i].Item1)
                    capacityProvided = Configs.capacityPerGenerator[i].Item2;
                else
                    break;
            }
            float energyFactor = __instance.currEnergyPerTick * 1.0f / __instance.energyPerTick;
            capacityProvided = (int)(capacityProvided * (energyFactor < 0.5 ? energyFactor * 2 : 1)); //电网供给少于50%时，每个建筑贡献的护盾容量也会减少，但只要高于50%就可以则贡献（被总容量削减后的）全额容量
            calcShieldCapacity.AddOrUpdate(planetId, capacityProvided, (x, y) => y + capacityProvided);
            maxShieldCapacity.AddOrUpdate(planetId, capacityProvided, (x, y) => y);

            //计算护盾产生器总量
            int existingCount = calcShieldGenCount.AddOrUpdate(planetId, 1, (x, y) => y + 1);

            //护盾回复
            int maxCap = maxShieldCapacity.GetOrAdd(planetId, 0);
            double gen = Configs.shieldGenPerTick * (__instance.currEnergyPerTick * 1.0 / __instance.energyPerTick); //电网供给只要不是100%就会按比例缩减护盾恢复速度
            
            // 护盾提供回复量： 1, 1/2, 1/2, 1/3, 1/3, 1/3, 1/4, 1/4, 1/4, 1/4, 1/5, ...
            for (int j = 1; ; j++)
            {
                if (j * (j + 1)  / 2 >= existingCount)
                {
                    gen /= j;
                    break;
                }
            }

            if (Configs.nextWaveStarIndex == __instance.fullId / 100 - 1 && Configs.nextWaveState == 3) //战斗中不回复护盾
            { 
                gen = 0;
            }
            if (Relic.HaveRelic(2, 1)) gen = gen * 1.5;
            if (Relic.HaveRelic(4, 5)) gen = gen * 0.7;
            calcShieldInc.AddOrUpdate(planetId, gen, (x, y) => y + gen);

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerExchangerComponent), "OutputUpdate")]
        public static bool ShieldGenPowerUpdatePatch2(ref PowerExchangerComponent __instance)
        {
            if (__instance.emptyId == 9999)
                return false;
            else
                return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerExchangerComponent), "StateUpdate")]
        public static bool ShieldGenPowerUpdatePatch0(ref PowerExchangerComponent __instance)
        {
            if (__instance.emptyId != 9999)
                return true;

            if (__instance.targetState < 0) //不允许有>0的targetState
                __instance.targetState = 1;
            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RefreshMaxShieldCapacity(long time)
        {
            // 半秒更新一次！
            if (GameMain.instance.timei % 30 != 1)
            {
                calcShieldCapacity.Clear();
                calcShieldInc.Clear();
                calcShieldGenCount.Clear();
                return;
            }

            //将这一帧刚算好的最大护盾承载量储存，由于计算calcShieldCapacity的过程中必定伴随着设置maxShieldCapacity，所以maxShieldCapacity一定包含了calc的所有键值对，不会漏掉。反之则不然。
            foreach (var item in maxShieldCapacity)
            {
                int planetId = item.Key;
                if (Relic.HaveRelic(2, 10) && Relic.starsWithMegaStructure.Contains(planetId / 100 - 1)) // relic2-10 矩阵充能提升护盾上限
                {
                    int shieldCap = calcShieldCapacity.GetOrAdd(planetId, 0);
                    int starIndex = planetId / 100 - 1;
                    if (starIndex > 0 && starIndex < GameMain.data.dysonSpheres.Length && GameMain.data.dysonSpheres[starIndex] != null)
                    {
                        double bonus = Math.Min(0.5, GameMain.data.dysonSpheres[starIndex].energyGenCurrentTick_Layers * 1.0 / 2000000000); // 60GW达到加成上限：50%
                        shieldCap = (int)(shieldCap * (1 + bonus));
                        if (Configs.developerMode && shieldCap <= 1000) shieldCap = 1000000000;
                    }
                    maxShieldCapacity.AddOrUpdate(planetId, item.Value, (x, y) => shieldCap);
                }
                else
                    maxShieldCapacity.AddOrUpdate(planetId, item.Value, (x, y) => calcShieldCapacity.GetOrAdd(planetId, 0));
                //护盾值回复，或如果当前护盾值超出上限，则护盾值下降
                double inc = 0;
                if(calcShieldInc.TryGetValue(planetId, out inc) && currentShield.GetOrAdd(planetId,0) < item.Value)
                {
                    inc *= 30;
                    if (Relic.HaveRelic(2, 0)) // relic2-0提高充能速度
                        inc *= 1.5;
                    if (Configs.developerMode) inc *= 1000;
                    currentShield.AddOrUpdate(planetId, (int) inc, (x, y) => y + (int) inc);
                    if (currentShield[planetId] > item.Value)
                        currentShield.AddOrUpdate(planetId, item.Value, (x, y) => item.Value);
                }
            }

            if(shieldUIPlanetId!=0)
            {
                curShieldIncUI = (int)(calcShieldInc.GetOrAdd(shieldUIPlanetId, 0) * (Relic.HaveRelic(2, 0) ? 1.5 : 1.0)); // relic2-0提高充能速度 面板ui显示
                curShieldGenCntUI = calcShieldGenCount.GetOrAdd(shieldUIPlanetId, 0);
            }

            //还原calc
            calcShieldCapacity.Clear();
            calcShieldInc.Clear();
            calcShieldGenCount.Clear();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIPowerExchangerWindow), "_OnOpen")]
        public static void UIOpenPatch(ref UIPowerExchangerWindow __instance)
        {

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIPowerExchangerWindow), "OnExchangerIdChange")]
        public static void UIChangePatch(ref UIPowerExchangerWindow __instance)
        {
            if (!__instance.active) return;
            if (__instance == null) return;
            if (__instance.exchangerId <= 0) return;
            PowerExchangerComponent powerExchangerComponent = __instance.powerSystem.excPool[__instance.exchangerId];
            if(powerExchangerComponent.emptyId == 9999)
            {
                isShieldUI = true;
                
            }
            else
            {
                isShieldUI = false;
            }

            int planetId = __instance.factory.planetId;
            shieldUIPlanetId = planetId;
            RefreshUIOnce(planetId);
            RefreshOtherShieldUI(planetId);
            RefreshPowerNetworkUI(planetId);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIPowerExchangerWindow), "_OnUpdate")]
        public static void UIUpdatePatch(ref UIPowerExchangerWindow __instance)
        {
            if (__instance == null || __instance.powerSystem == null || __instance.powerSystem.excPool == null) return;
            PowerExchangerComponent powerExchangerComponent = __instance.powerSystem.excPool[__instance.exchangerId];
            if (powerExchangerComponent.emptyId != 9999) return;

            __instance.emptyUIButton.tips.itemId = 0;
            __instance.fullUIButton.tips.itemId = 0;
            __instance.emptyIcon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/elect0");
            __instance.fullIcon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/shield3");

            //if (powerExchangerComponent.targetState == 1 && isShieldUI)
            //{
            //    modeButtonsLeftBarObj.GetComponent<Image>().color = shieldActiveHighlight;
            //    switchButton3Obj.GetComponent<Image>().color = shieldActiveHighlight;
            //}

            int planetId = __instance.factory.planetId;
            shieldUIPlanetId = planetId;
            RefreshOtherShieldUI(planetId);
            RefreshPowerNetworkUI(planetId);
        }

        public static void RefreshUIOnce(int planetId)
        {
            if (modeButtonsLeftBarObj == null)
                return;

            if(isShieldUI)
            {
                modeButtonsLeftBarObj.GetComponent<RectTransform>().sizeDelta = new Vector2(3, 68);
                modeButtonsLeftBarObj.transform.localPosition = new Vector3(23, -57, 0); // 23 -74 0 from 23 -40 0
                switchButton2Obj.transform.localPosition = new Vector3(26, -57, 0);
                switchButton3Obj.transform.localPosition = new Vector3(26, -91, 0);
                switchButton1Obj.SetActive(false);
                emptyItemCntTextObj.SetActive(false);
                fullItemCntTextObj.SetActive(false);
                //switchButton2Text.text = "待机gm".Translate();
                switchButton3Text.text = "启动gm".Translate();
                circleBack.color = shieldBack;
                circleFg.color = shieldFg;
                circleFg2.color = shieldFg2;
                circleFg3.color = shieldFg3;
                circlePowerIcon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/shield3");
                circleFg3.sprite = Resources.Load<Sprite>("Assets/DSPBattle/shield3");
                powerCircleLegend3Obj.SetActive(false);
                powerCircleLegend4Obj.SetActive(false);
                powerCircleAccInObj.SetActive(false);
                powerCircleAccOutObj.SetActive(false);
                powerCircleCons.color = shieldCurCircle;
                powerCircleLegend2Bar.color = shieldCurCircle;
                switchButton2Obj.GetComponent<UIButton>().tips.tipText = "护盾生成器待机提示";
                switchButton3Obj.GetComponent<UIButton>().tips.tipTitle = "启动gm";
                switchButton3Obj.GetComponent<UIButton>().tips.tipText = "护盾生成器启动提示";
            }
            else
            {
                modeButtonsLeftBarObj.GetComponent<RectTransform>().sizeDelta = new Vector2(3, 102);
                modeButtonsLeftBarObj.transform.localPosition = new Vector3(23, -40, 0);
                switchButton2Obj.transform.localPosition = new Vector3(26, -74, 0);
                switchButton3Obj.transform.localPosition = new Vector3(26, -108, 0);
                switchButton1Obj.SetActive(true);
                emptyItemCntTextObj.SetActive(true);
                fullItemCntTextObj.SetActive(true);
                //switchButton2Text.text = "待机".Translate();
                switchButton3Text.text = "充电".Translate();
                circleBack.color = oriBack;
                circleFg.color = oriFg;
                circleFg2.color = oriFg2;
                circleFg3.color = oriFg3;
                circlePowerIcon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/elect0");
                circleFg3.sprite = Resources.Load<Sprite>("Assets/DSPBattle/elect0");
                powerCircleLegend3Obj.SetActive(true);
                powerCircleLegend4Obj.SetActive(true);
                powerCircleAccInObj.SetActive(true); 
                powerCircleAccOutObj.SetActive(true);
                powerCircleCons.color = oriConsumeCircle;
                powerCircleLegend2Bar.color = oriConsumeCircle;
                switchButton2Obj.GetComponent<UIButton>().tips.tipText = "能量枢纽待机提示";
                switchButton3Obj.GetComponent<UIButton>().tips.tipTitle = "充电";
                switchButton3Obj.GetComponent<UIButton>().tips.tipText = "能量枢纽充电提示";

            }
        }

        public static void RefreshOtherShieldUI(int planetId)
        {
        }

        public static void RefreshPowerNetworkUI(int planetId)
        {
            if (isShieldUI)
            {
                powerNetworkTitle.text = "行星护盾".Translate();
                powerNetworkTitle2.text = GameMain.galaxy.stars[planetId / 100 - 1].planets[planetId % 100 - 1].displayName;
                powerCircleLegend1Label.text = "护盾容量短".Translate();
                powerCircleLegend2Label.text = "当前护盾".Translate();
                float chargePercent = 0;
                if(maxShieldCapacity.GetOrAdd(planetId,0) > 0)
                {
                    chargePercent = currentShield.GetOrAdd(planetId, 0) * 1.0f / maxShieldCapacity[planetId];
                }
                powerCircleCenterValue.text = chargePercent.ToString("0.0%");
                powerCircleGen.fillAmount = 1.0f;
                powerCircleCons.fillAmount = chargePercent;


                powerNetworkLabel1.text = "护盾容量".Translate();
                powerNetworkLabel2.text = "当前护盾".Translate();
                powerNetworkLabel3.text = "护盾恢复".Translate();
                powerNetworkLabel4.text = "护盾生成器总数".Translate();
                powerNetworkLabel5.text = "完全充能时间".Translate();
                powerCircleCenterLabel.text = "充能gm".Translate();

                powerNetworkValue1.text = maxShieldCapacity[planetId].ToString("N0");
                powerNetworkValue2.text = currentShield.GetOrAdd(planetId, 0).ToString("N0");
                powerNetworkValue3.text = (curShieldIncUI * 60).ToString("N0") + " /s";
                powerNetworkValue4.text = curShieldGenCntUI.ToString("N0");
                if(currentShield[planetId] < maxShieldCapacity[planetId] && curShieldIncUI > 0)
                {
                    int sec = (int)((maxShieldCapacity[planetId] - currentShield[planetId]) * 1.0 / curShieldIncUI / 60);
                    powerNetworkValue5.text = $"{sec/3600:00}:{sec%3600/60:00}:{sec%60:00}";
                }
                else
                {
                    powerNetworkValue5.text = "-";
                }

            }
            else
            {

                powerNetworkLabel1.text = "发电性能".Translate();
                powerNetworkLabel2.text = "耗电需求gm".Translate();
                powerNetworkLabel3.text = "实际发电".Translate();
                powerNetworkLabel4.text = "蓄电池充电".Translate();
                powerNetworkLabel5.text = "总蓄电量".Translate();
                powerCircleCenterLabel.text = "供电率".Translate();

                powerCircleLegend1Label.text = "发电性能短gm".Translate();
                powerCircleLegend2Label.text = "耗电需求短gm".Translate();
            }
        }

        //发电性能 耗电需求 实际发电    蓄电池充电 总蓄电量
        //护盾容量 当前护盾 充能速度    护盾发生器总数 完全充能耗时

        public static void Export(BinaryWriter w)
        {
            w.Write(currentShield.Count);
            foreach (var item in currentShield)
            {
                w.Write(item.Key);
                w.Write(item.Value);
            }
            w.Write(maxShieldCapacity.Count);
            foreach (var item in maxShieldCapacity)
            {
                w.Write(item.Key);
                w.Write(item.Value);
            }
        }
        public static void Import(BinaryReader r)
        {
            InitAll();
            if(Configs.versionWhenImporting >= 30220325)
            {
                int num1 = r.ReadInt32();
                for (int i = 0; i < num1; i++)
                {
                    int k = r.ReadInt32();
                    int v = r.ReadInt32();
                    currentShield.AddOrUpdate(k, v, (x, y) => v);
                }
                int num2 = r.ReadInt32();
                for (int i = 0; i < num2; i++)
                {
                    int k = r.ReadInt32();
                    int v = r.ReadInt32();
                    maxShieldCapacity.AddOrUpdate(k, v, (x, y) => v);
                }
            }
        }
        public static void IntoOtherSave()
        {
            InitAll();
        }
    }
}
