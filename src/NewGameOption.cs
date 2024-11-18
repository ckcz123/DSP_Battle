using BepInEx;
using HarmonyLib;
using rail;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    class NewGameOption
    {
        public static bool fastStart = false;
        public static GameObject fastStartObj = null;
        public static GameObject voidInvasionToggleObj = null;

        public static  GameObject oriPropertyMultiplierObj;
        public static GameObject oriSeedKeyObj;

        public static Image voidInvasionLogo;
        public static Toggle voidInvasionToggle;
        public static UIToggle voidInvasionUITg;
        public static Text voidInvasionTitleText;

        public static float propertyObjX = 70;
        public static float propertyObjY0 = -325;
        public static float propertyObjY1 = -361;
        public static float seedKeyObjX = 0;
        public static float seedKeyObjY0 = -351;
        public static float seedKeyObjY1 = -387;

        public static Toggle DFToggle;
        public static int voidInvasionEnabledCache = 1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        public static void UIGalaxySelect_OnOpen(ref UIGalaxySelect __instance)
        {
            fastStart = false;


            GameObject oriSettingObj = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/sandbox-mode");

            oriPropertyMultiplierObj = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/property-multiplier");
            oriSeedKeyObj = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/seed-key");

            if (fastStartObj == null)
            {
                if (oriSettingObj == null)
                    return;
                fastStartObj = GameObject.Instantiate(oriSettingObj);
                fastStartObj.name = "fast-start-mode";
                fastStartObj.transform.SetParent(GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/").transform, false);
                fastStartObj.transform.localPosition = new Vector3(0, -244, 0);
                fastStartObj.GetComponent<Text>().text = "快速开局".Translate();
                fastStartObj.GetComponentInChildren<UIButton>().tips.tipTitle = "快速开局".Translate();
                fastStartObj.GetComponentInChildren<UIButton>().tips.tipText = "快速开局提示".Translate();
                fastStartObj.GetComponentInChildren<Toggle>().onValueChanged.RemoveAllListeners();
                fastStartObj.GetComponentInChildren<Toggle>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) => fastStart = isOn));
                fastStartObj.GetComponentInChildren<Toggle>().isOn = false;

                if (MoreMegaStructure.MoreMegaStructure.GenesisCompatibility)
                {
                    fastStartObj.SetActive(false);
                    fastStart = false;
                }
                else
                {
                    GameObject oriDFToggleObj = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/DF-toggle");
                    oriDFToggleObj.transform.localPosition = new Vector3(0, -280, 0);
                    oriPropertyMultiplierObj.transform.localPosition = new Vector3(70, -325, 0);
                    oriSeedKeyObj.transform.localPosition = new Vector3(0, -351, 0);
                }
            }
            if (Configs.enableVoidInvasionUpdate)
            {
                if (voidInvasionToggleObj == null)
                {
                    GameObject oriToggleWithIcon = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/DF-toggle/check-box");
                    DFToggle = oriToggleWithIcon.GetComponent<Toggle>();

                    voidInvasionToggleObj = GameObject.Instantiate(oriSettingObj);
                    voidInvasionToggleObj.name = "void-invasion-toggle";
                    voidInvasionToggleObj.transform.SetParent(GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/setting-group/").transform, false);
                    //GameObject.DestroyImmediate(voidInvasionToggleObj.transform.Find("CheckBox"));
                    voidInvasionToggleObj.transform.Find("CheckBox").gameObject.SetActive(false);
                    voidInvasionToggleObj.transform.localScale = Vector3.one;
                    voidInvasionToggleObj.transform.localPosition = new Vector3(0, -316, 0);

                    GameObject VICheckBoxObj = GameObject.Instantiate(oriToggleWithIcon, voidInvasionToggleObj.transform);
                    VICheckBoxObj.transform.localScale = Vector3.one;
                    VICheckBoxObj.transform.localPosition = new Vector3(-40, -15, 0);
                    GameObject toggleIconObj = VICheckBoxObj.transform.Find("df-icon").gameObject;
                    toggleIconObj.name = "vi-icon";
                    voidInvasionLogo = toggleIconObj.GetComponent<Image>();
                    voidInvasionLogo.sprite = Resources.Load<Sprite>("Assets/DSPBattle/techMD");

                    voidInvasionTitleText = voidInvasionToggleObj.GetComponent<Text>();
                    voidInvasionTitleText.text = "虚空入侵".Translate();
                    voidInvasionToggleObj.GetComponentInChildren<UIButton>().tips.tipTitle = "虚空入侵".Translate();
                    voidInvasionToggleObj.GetComponentInChildren<UIButton>().tips.tipText = "虚空入侵提示".Translate();
                    voidInvasionToggle = voidInvasionToggleObj.GetComponentInChildren<Toggle>();
                    voidInvasionToggle.onValueChanged.RemoveAllListeners();
                    voidInvasionToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) => voidInvasionEnabledCache = isOn ? 1 : -1));
                    voidInvasionToggleObj.GetComponentInChildren<Toggle>().isOn = true;
                    voidInvasionUITg = voidInvasionToggleObj.GetComponentInChildren<UIToggle>();

                    DFToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) => voidInvasionToggle.isOn = isOn));
                }

                voidInvasionEnabledCache = voidInvasionToggle.isOn ? 1 : -1;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnClose")]
        public static void UIGalaxySelect_OnClose(ref UIGalaxySelect __instance)
        {
            if(fastStartObj != null)
                GameObject.Destroy(fastStartObj);
            if (voidInvasionToggleObj != null)
                GameObject.Destroy(voidInvasionToggleObj);    
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnUpdate")]
        public static void UIGalaxySelect_OnUpdate(ref UIGalaxySelect __instance)
        {
            if (fastStartObj != null)
                fastStartObj.GetComponent<Text>().text = "快速开局".Translate();
            if (Configs.enableVoidInvasionUpdate)
            {
                if (__instance.gameDesc.isCombatMode)
                {
                    oriPropertyMultiplierObj.transform.localPosition = new Vector3(propertyObjX, propertyObjY1, 0);
                    oriSeedKeyObj.transform.localPosition = new Vector3(seedKeyObjX, seedKeyObjY1, 0);

                    voidInvasionToggleObj.SetActive(true);
                    voidInvasionTitleText.text = "虚空入侵".Translate();
                    voidInvasionLogo.color = new Color(1f, 1f, 1f, (voidInvasionEnabledCache == 1 ? 0.86f : 0.06f) + (voidInvasionUITg.isMouseEnter ? 0.14f : 0f));
                    if (voidInvasionToggleObj.transform.localPosition.y != -316)
                    {
                        voidInvasionToggleObj.transform.localPosition = new Vector3(0, -316, 0);
                    }
                }
                else
                {
                    oriPropertyMultiplierObj.transform.localPosition = new Vector3(propertyObjX, propertyObjY0, 0);
                    oriSeedKeyObj.transform.localPosition = new Vector3(seedKeyObjX, seedKeyObjY0, 0);
                    voidInvasionToggleObj.SetActive(false);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameSave), "LoadCurrentGame")]
        public static void GameSave_LoadCurrentGame()
        {
            fastStart = false;
            voidInvasionEnabledCache = 0; // 读取游戏时阻止cache改变虚空入侵的开关，因为只有1或-1的时候才会更改虚空入侵的开关设定
        }   

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIRoot), "OnGameBegin")]
        public static void UIRoot_OnGameBegin()
        {
            if (!DSPGame.IsMenuDemo && fastStart)
            {
                DspBattlePlugin.logger.LogInfo("=======================================> FAST START");
                Init();
            }
            if (voidInvasionEnabledCache == 1)
                AssaultController.voidInvasionEnabled = true;
            else if (voidInvasionEnabledCache == -1)
                AssaultController.voidInvasionEnabled = false;
            UIEscMenuPatch.Init();
        }

        private static void Init()
        {
            foreach (TechProto proto in LDB.techs.dataArray)
            {
                if (proto.ID == 1901 || proto.ID == 1312) continue;
                if (!GameMain.data.history.TechUnlocked(proto.ID) && proto.Items.All((e) => e == 6001 || e == 6002 || e < 5200))
                {
                    GameMain.data.history.UnlockTechUnlimited(proto.ID, true);
                }
                if (!GameMain.data.history.TechUnlocked(2303)) // 额外解锁一行物品栏
                {
                    GameMain.data.history.UnlockTechUnlimited(2303, true);
                }
            }

            // 地面移速科技点满
            //for (int techId = 2204; techId <= 2208; techId++)
            //{
            //    GameMain.data.history.UnlockTechUnlimited(techId, true);
            //}

            GameMain.data.mainPlayer.TryAddItemToPackage(1131, 1000, 0, false); // 地基

            GameMain.data.mainPlayer.TryAddItemToPackage(2001, 280, 0, false); // 一级带
            GameMain.data.mainPlayer.TryAddItemToPackage(2002, 600, 0, false); // 二级带

            GameMain.data.mainPlayer.TryAddItemToPackage(2011, 195, 0, false); // 一级爪
            GameMain.data.mainPlayer.TryAddItemToPackage(2013, 195, 0, false); // 二级爪
            GameMain.data.mainPlayer.TryAddItemToPackage(2013, 200, 0, false); // 三级爪
            GameMain.data.mainPlayer.TryAddItemToPackage(2020, 19, 0, false); // 分流器
            GameMain.data.mainPlayer.TryAddItemToPackage(2313, 50, 0, false); // 喷涂机

            GameMain.data.mainPlayer.TryAddItemToPackage(2101, 50, 0, false); // 小箱子
            GameMain.data.mainPlayer.TryAddItemToPackage(2106, 49, 0, false); // 储液罐
            GameMain.data.mainPlayer.TryAddItemToPackage(2107, 50, 0, false); // 配送器
            GameMain.data.mainPlayer.TryAddItemToPackage(2103, 50, 0, false); // 小塔
            GameMain.data.mainPlayer.TryAddItemToPackage(5003, 200, 0, false); // 配送机
            GameMain.data.mainPlayer.TryAddItemToPackage(5001, 1000, 0, false); // 小船

            GameMain.data.mainPlayer.TryAddItemToPackage(2201, 199, 0, false); // 电线杆
            GameMain.data.mainPlayer.TryAddItemToPackage(2202, 50, 0, false); // 输电塔
            GameMain.data.mainPlayer.TryAddItemToPackage(2203, 49, 0, false); // 风电
            GameMain.data.mainPlayer.TryAddItemToPackage(2204, 49, 0, false); // 风电
            GameMain.data.mainPlayer.TryAddItemToPackage(2205, 149, 0, false); // 太阳能

            GameMain.data.mainPlayer.TryAddItemToPackage(2301, 99, 0, false); // 矿机
            GameMain.data.mainPlayer.TryAddItemToPackage(2302, 97, 0, false); // 熔炉
            GameMain.data.mainPlayer.TryAddItemToPackage(2303, 49, 0, false); // 制造台MK1
            GameMain.data.mainPlayer.TryAddItemToPackage(2304, 150, 0, false); // 制造台MK2
            GameMain.data.mainPlayer.TryAddItemToPackage(2306, 20, 0, false); // 抽水站
            GameMain.data.mainPlayer.TryAddItemToPackage(2307, 20, 0, false); // 抽油机
            GameMain.data.mainPlayer.TryAddItemToPackage(2308, 90, 0, false); // 精炼厂
            GameMain.data.mainPlayer.TryAddItemToPackage(2309, 90, 0, false); // 化工厂

            GameMain.data.mainPlayer.TryAddItemToPackage(2901, 49, 0, false); // 研究站

            if(GameMain.data.gameDesc.isCombatMode)
            {
                GameMain.data.mainPlayer.TryAddItemToPackage(3001, 50, 0, false); // 机枪塔
                GameMain.data.mainPlayer.TryAddItemToPackage(3002, 50, 0, false); // 激光塔
                GameMain.data.mainPlayer.TryAddItemToPackage(3003, 50, 0, false); // 加农炮
                GameMain.data.mainPlayer.TryAddItemToPackage(3005, 50, 0, false); // 导弹塔
                GameMain.data.mainPlayer.TryAddItemToPackage(3007, 20, 0, false); // 信号塔
                GameMain.data.mainPlayer.TryAddItemToPackage(3009, 5, 0, false); // 战场分析基站

                GameMain.data.mainPlayer.TryAddItemToPackage(1601, 90, 0, false); // 子弹
                GameMain.data.mainPlayer.TryAddItemToPackage(1604, 100, 0, false); // 炮弹
                GameMain.data.mainPlayer.TryAddItemToPackage(1609, 100, 0, false); // 导弹
            }

            //GameMain.data.mainPlayer.TryAddItemToPackage(1801, 60, 0, false); // 氢燃料棒
        }
    }
}
