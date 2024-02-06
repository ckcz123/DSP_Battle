using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    class FastStartOption
    {
        //public static bool fastStart = false;
        //public static GameObject fastStartObj = null;

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        //public static void UIGalaxySelect_OnOpen(ref UIGalaxySelect __instance)
        //{
        //    fastStart = false;
        //    fastStartObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/sandbox-mode"));
        //    fastStartObj.name = "fast-start-mode";
        //    fastStartObj.transform.SetParent(GameObject.Find("UI Root/Overlay Canvas/Galaxy Select").transform, false);
        //    fastStartObj.transform.localPosition = new Vector3(0, 209, 0);
        //    fastStartObj.GetComponent<Text>().text = "快速开局".Translate();
        //    fastStartObj.GetComponentInChildren<Button>().gameObject.SetActive(false);
        //    fastStartObj.GetComponentInChildren<Toggle>().onValueChanged.RemoveAllListeners();
        //    fastStartObj.GetComponentInChildren<Toggle>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) => fastStart = isOn));
        //    fastStartObj.GetComponentInChildren<Toggle>().isOn = false;
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(UIGalaxySelect), "_OnClose")]
        //public static void UIGalaxySelect_OnClose(ref UIGalaxySelect __instance)
        //{
        //    GameObject.Destroy(fastStartObj);
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(GameSave), "LoadCurrentGame")]
        //public static void GameSave_LoadCurrentGame()
        //{
        //    fastStart = false;
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(UIRoot), "OnGameBegin")]
        //public static void UIRoot_OnGameBegin()
        //{
        //    if (!DSPGame.IsMenuDemo && fastStart)
        //    {
        //        DspBattlePlugin.logger.LogInfo("=======================================> FAST START!!!!!!!!!!!!");
        //        Init();
        //    }
        //}

        //private static void Init()
        //{
        //    foreach (TechProto proto in LDB.techs.dataArray)
        //    {
        //        if (proto.ID == 1901 || proto.ID == 1312) continue;
        //        if (!GameMain.data.history.TechUnlocked(proto.ID) && proto.Items.All((e) => e == 6001 || e == 6002 || e < 6000))
        //        {
        //            GameMain.data.history.UnlockTechUnlimited(proto.ID, true);
        //        }
        //    }

        //    // 地面移速科技点满
        //    //for (int techId = 2204; techId <= 2208; techId++)
        //    //{
        //    //    GameMain.data.history.UnlockTechUnlimited(techId, true);
        //    //}

        //    GameMain.data.mainPlayer.TryAddItemToPackage(1131, 1000, 0, false); // 地基

        //    GameMain.data.mainPlayer.TryAddItemToPackage(2001, 280, 0, false); // 一级带
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2002, 600, 0, false); // 二级带
            
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2011, 195, 0, false); // 一级爪
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2013, 195, 0, false); // 二级爪
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2013, 200, 0, false); // 三级爪
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2020, 19, 0, false); // 分流器
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2313, 50, 0, false); // 喷涂机

        //    GameMain.data.mainPlayer.TryAddItemToPackage(2101, 50, 0, false); // 小箱子
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2106, 49, 0, false); // 储液罐
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2107, 50, 0, false); // 配送器
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2103, 50, 0, false); // 小塔
        //    GameMain.data.mainPlayer.TryAddItemToPackage(5003, 200, 0, false); // 配送机
        //    GameMain.data.mainPlayer.TryAddItemToPackage(5001, 1000, 0, false); // 小船

        //    GameMain.data.mainPlayer.TryAddItemToPackage(2201, 199, 0, false); // 电线杆
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2202, 50, 0, false); // 输电塔
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2203, 49, 0, false); // 风电
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2204, 49, 0, false); // 风电
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2205, 149, 0, false); // 太阳能

        //    GameMain.data.mainPlayer.TryAddItemToPackage(2301, 99, 0, false); // 矿机
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2302, 97, 0, false); // 熔炉
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2303, 49, 0, false); // 制造台MK1
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2304, 150, 0, false); // 制造台MK2
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2306, 20, 0, false); // 抽水站
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2307, 20, 0, false); // 抽油机
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2308, 90, 0, false); // 精炼厂
        //    GameMain.data.mainPlayer.TryAddItemToPackage(2309, 90, 0, false); // 化工厂

        //    GameMain.data.mainPlayer.TryAddItemToPackage(2901, 49, 0, false); // 研究站

        //    //GameMain.data.mainPlayer.TryAddItemToPackage(1801, 60, 0, false); // 氢燃料棒
        //}
    }
}
