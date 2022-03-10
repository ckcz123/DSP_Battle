using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using xiaoye97;
using System.IO;
using System.Collections.Concurrent;
using HarmonyLib;

namespace DSP_Battle
{
    class UIBattleStatistics
    {
        static GameObject statPanelLeftTabObj = null; //左切换面板（水平按钮的那个）
        static GameObject battleStatButtonObj = null; //切换到战斗数据统计面板的按钮的obj
        static GameObject battleStatTabObj = null; //主面板

        static GameObject hideButton1;//这六个需要永久隐藏
        static GameObject hideButton2;
        static GameObject hideButton3;
        static GameObject hideGraph1;
        static GameObject hideGraph2;
        static GameObject hideGraph3;

        static Text battleStateButtonText;
        static Text briefTitle;
        static Text bulletsTitle;
        static Text enemyTitle;
        static Text briefLabel;
        static Text briefValue1;
        static Text briefValue2;
        static Text bulletsLabel;
        static Text bulletsValue1;
        static Text bulletsValue2;
        static Text enemyLabel;
        static Text enemyValue1;
        static Text enemyValue2;



        static UIStatisticsWindow UIStatWindowInstance = null; //P键整个面板的GameObject

        static bool isBattleStatTab = false; //当前是否显示战斗简报页面

        public static ConcurrentDictionary<int, int> enemyGen; //敌人生成总量
        public static ConcurrentDictionary<int, int> enemyEliminated; //敌人被消灭数，按类别分类

        public static int totalEliminated; //已消灭的敌人总数
        public static long totalDamage; //已造成的总伤害
        public static long resourceLost; //资源被偷走、销毁量
        public static long buildingLost; //建筑被摧毁量

        public static ConcurrentDictionary<int,long> bulletDamageOutput;//子弹、导弹输出总伤害
        public static ConcurrentDictionary<int,long> bulletDamageHit;//子弹、导弹击中总伤害，由于子弹导弹等有飞行时间，在中飞行时科技升级了，那么单个子弹注册的输出伤害会低于击中伤害，但无所谓，这种情况发生概率或者占比较小，不大会影响数据统计。此外，导弹有aoe，其伤害效率超过100%也是正常的
        public static ConcurrentDictionary<int, int> bulletUse; //每种子弹、导弹的发射量
        public static ConcurrentDictionary<int, int> bulletHit; //每种子弹、导弹击中量

        public static void InitAll()
        {
            ReInitBattleStat();
            if (statPanelLeftTabObj != null)
                return;
            
            //按钮
            statPanelLeftTabObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Statistics Window/panel-bg/horizontal-tab");
            statPanelLeftTabObj.GetComponent<RectTransform>().sizeDelta = new Vector2(105, 416);
            GameObject oriAchButtonObj = statPanelLeftTabObj.transform.Find("achievement-btn").gameObject;
            battleStatButtonObj = GameObject.Instantiate(oriAchButtonObj);
            battleStatButtonObj.name = "battle-btn";
            battleStatButtonObj.transform.SetParent(statPanelLeftTabObj.transform, false);
            battleStatButtonObj.transform.localPosition = new Vector3(-52, -364, 0);

            battleStatButtonObj.GetComponent<Button>().onClick.RemoveAllListeners();
            battleStatButtonObj.GetComponent<Button>().onClick.AddListener(() => { OnClickBattleStatButton(); });

            battleStateButtonText = battleStatButtonObj.transform.Find("text").GetComponent<Text>();
            battleStateButtonText.text = "战斗简报".Translate();

            //面板
            GameObject oriPerformanceObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Statistics Window/performance-bg");
            battleStatTabObj = GameObject.Instantiate(oriPerformanceObj);
            battleStatTabObj.name = "battle-bg";
            battleStatTabObj.transform.SetParent(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Statistics Window").transform, false);
            battleStatTabObj.SetActive(false);


            //需要改位置
            battleStatTabObj.transform.Find("cpu-panel/title-text").localPosition = new Vector3(183, 350, 0);
            battleStatTabObj.transform.Find("gpu-panel/title-text").localPosition = new Vector3(183, 350, 0);
            battleStatTabObj.transform.Find("data-panel/title-text").localPosition = new Vector3(183, 350, 0);
            battleStatTabObj.transform.Find("cpu-panel/Scroll View").localPosition = new Vector3(20, 270, 0);
            battleStatTabObj.transform.Find("gpu-panel/Scroll View").localPosition = new Vector3(20, 270, 0);
            battleStatTabObj.transform.Find("data-panel/Scroll View").localPosition = new Vector3(20, 270, 0);


            battleStatTabObj.transform.Find("cpu-panel/title-text").GetComponent<Text>().text = "战况概览".Translate();
            battleStatTabObj.transform.Find("gpu-panel/title-text").GetComponent<Text>().text = "弹药概览".Translate();
            battleStatTabObj.transform.Find("data-panel/title-text").GetComponent<Text>().text = "敌舰".Translate();

            //需要永久隐藏
            hideButton1 = battleStatTabObj.transform.Find("cpu-panel/active-button").gameObject;
            hideButton2 = battleStatTabObj.transform.Find("gpu-panel/active-button").gameObject;
            hideButton3 = battleStatTabObj.transform.Find("data-panel/active-button").gameObject;
            hideGraph1 = battleStatTabObj.transform.Find("cpu-panel/sector-graph").gameObject;
            hideGraph2 = battleStatTabObj.transform.Find("gpu-panel/sector-graph").gameObject;
            hideGraph3 = battleStatTabObj.transform.Find("data-panel/sector-graph").gameObject;

            //需要实时刷新
            briefTitle = battleStatTabObj.transform.Find("cpu-panel/title-text").GetComponent<Text>();
            briefLabel = battleStatTabObj.transform.Find("cpu-panel/Scroll View/Viewport/Content/label").GetComponent<Text>();
            briefValue1 = battleStatTabObj.transform.Find("cpu-panel/Scroll View/Viewport/Content/value-1").GetComponent<Text>();
            briefValue2 = battleStatTabObj.transform.Find("cpu-panel/Scroll View/Viewport/Content/value-2").GetComponent<Text>();
            bulletsTitle = battleStatTabObj.transform.Find("gpu-panel/title-text").GetComponent<Text>();
            bulletsLabel = battleStatTabObj.transform.Find("gpu-panel/Scroll View/Viewport/Content/label").GetComponent<Text>();
            bulletsValue1 = battleStatTabObj.transform.Find("gpu-panel/Scroll View/Viewport/Content/value-1").GetComponent<Text>();
            bulletsValue2 = battleStatTabObj.transform.Find("gpu-panel/Scroll View/Viewport/Content/value-2").GetComponent<Text>();
            enemyTitle = battleStatTabObj.transform.Find("data-panel/title-text").GetComponent<Text>();
            enemyLabel = battleStatTabObj.transform.Find("data-panel/Scroll View/Viewport/Content/label").GetComponent<Text>();
            enemyValue1 = battleStatTabObj.transform.Find("data-panel/Scroll View/Viewport/Content/value-1").GetComponent<Text>();
            enemyValue2 = battleStatTabObj.transform.Find("data-panel/Scroll View/Viewport/Content/value-2").GetComponent<Text>();

        }

        public static void ReInitBattleStat()
        {
            try
            {
                resourceLost = 0;
                buildingLost = 0;
                totalDamage = 0;
                totalEliminated = 0;
                enemyGen = new ConcurrentDictionary<int, int>();
                enemyEliminated = new ConcurrentDictionary<int, int>();
                bulletDamageOutput = new ConcurrentDictionary<int, long>();
                bulletDamageHit = new ConcurrentDictionary<int, long>();
                bulletUse = new ConcurrentDictionary<int, int>();
                bulletHit = new ConcurrentDictionary<int, int>();
            }
            catch (Exception)
            {
                DspBattlePlugin.logger.LogWarning("ReInitBattleState Error.");
            }
        }

        //初始化敌人总生成量，仅由UIAlert在刚进入战斗状态时调用，如果是刚载入游戏且正处于战斗状态，则按当前重新计算
        public static void RegisterEnemyGen()
        {
            try
            {
                ReInitBattleStat();
                foreach (var item in EnemyShips.ships.Values)
                {
                    enemyGen.AddOrUpdate(item.intensity, 1, (x, y) => y + 1);
                }
            }
            catch (Exception)
            {
            }
        }

        //飞船被击毁
        public static void RegisterEliminate(int shipType, int destoryedNum = 1) 
        {
            try { enemyEliminated.AddOrUpdate(shipType, destoryedNum, (x, y) => destoryedNum); }
            catch (Exception) { }
        }

        //资源被掠夺
        public static void RegisterResourceLost(int lostnum)
        {
            resourceLost += lostnum;
        }

        //建筑被摧毁
        public static void RegisterBuildingLost(int lostnum)
        {
            buildingLost += lostnum;
        }

        //子弹或者火箭发射
        public static void RegisterShootOrLaunch(int bulletId, int damage, int num = 1) 
        {
            try
            { 
                bulletUse.AddOrUpdate(bulletId, num, (x, y) => y+num);
                bulletDamageOutput.AddOrUpdate(bulletId, damage, (x, y) => y + damage);
            }
            catch (Exception) { }
        }

        //子弹或者火箭命中
        public static void RegisterDamageHit(int bulletId, int damage, int num = 1) 
        {
            try
            {
                totalDamage += damage;
                bulletHit.AddOrUpdate(bulletId, num, (x, y) => y + num);
                bulletDamageHit.AddOrUpdate(bulletId, damage, (x, y) => y + damage);
            }
            catch (Exception) { }
        }

        public static void OnClickBattleStatButton()
        {
            if (UIStatWindowInstance == null)
                return;
            isBattleStatTab = true;
            //UIStatWindowInstance.OnTabButtonClick(99); //这个目前绝对不要用来替代下面那一堆，会显示不了！！！不知道为什么。
            UIStatWindowInstance.isMilestoneTab = false;
            UIStatWindowInstance.isProductionTab = false;
            UIStatWindowInstance.isPowerTab = false;
            UIStatWindowInstance.isResearchTab = false;
            UIStatWindowInstance.isDysonTab = false;
            UIStatWindowInstance.isAchievementTab = false;
            UIStatWindowInstance.isPerformanceTab = false;
            UIStatWindowInstance.tabIndex = 99;
            UIStatWindowInstance.RefreshAll();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStatisticsWindow), "_OnOpen")]
        public static void UIStateWinOpenPatch(ref UIStatisticsWindow __instance)
        {
            UIStatWindowInstance = __instance;
            //UIAlert.ShowAlert(false); //关闭顶部警告避免遮挡？
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStatisticsWindow), "OnTabButtonClick")]
        public static void OtherButtonClickPatch()
        {
            isBattleStatTab = false;
            if(UIStatWindowInstance != null) UIStatWindowInstance.RefreshAll();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStatisticsWindow), "RefreshAll")]
        public static void UIStateWinRefreshPatch(ref UIStatisticsWindow __instance)
        {
            UIStatWindowInstance = __instance;
            battleStatTabObj.SetActive(isBattleStatTab);
            battleStatButtonObj.GetComponent<UIButton>().highlighted = isBattleStatTab;
            battleStateButtonText.text = "战斗简报".Translate();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStatisticsWindow), "_OnUpdate")]
        public static void OnUpdatePatch(ref UIStatisticsWindow __instance)
        {
            UIStatWindowInstance = __instance;
            if (isBattleStatTab)
            {
                hideButton1.SetActive(false);
                hideButton2.SetActive(false);
                hideButton3.SetActive(false);
                hideGraph1.SetActive(false);
                hideGraph2.SetActive(false);
                hideGraph3.SetActive(false);

                briefLabel.text = "歼灭敌舰".Translate() + "\n" + "输出伤害".Translate() + "\n" + "损失建筑".Translate() + "\n" + "损失资源".Translate();
                briefValue1.text = "";
                briefValue2.text = totalEliminated.ToString("###,###") + "\n" + totalDamage.ToString("###,###") + "\n" + buildingLost.ToString("###,###") + "\n" + resourceLost.ToString("###,###");

                bulletsLabel.text = "\n" + "数量总计".Translate() + "\n" + "伤害总计".Translate() + "\n";
            }
        }

    }
}
