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
        static Text ammoLabel;
        static Text ammoValue1;
        static Text ammoValue2;
        static Text enemyLabel;
        static Text enemyValue1;
        static Text enemyValue2;



        static UIStatisticsWindow UIStatWindowInstance = null; //P键整个面板的GameObject

        static bool isBattleStatTab = false; //当前是否显示战斗简报页面

        public static ConcurrentDictionary<int, int> enemyGen; //敌人生成总量
        public static ConcurrentDictionary<int, int> enemyEliminated; //敌人被消灭数，按类别分类

        //简报第一栏显示
        public static int totalEnemyEliminated; //已消灭的敌人总数
        public static long totalDamage; //已造成的总伤害
        public static long resourceLost; //资源被偷走、销毁量
        public static long buildingLost; //建筑被摧毁量

        //除上面的之外其他总和数值
        public static int totalEnemyGen; //生成的敌人总数
        public static long totalAmmoDamageOut, totalAmmoDamageHit, bAmmoDamageOut, bAmmoDamageHit, mAmmoDamageOut, mAmmoDamageHit; //伤害输出和命中总和记录。此处Ammo泛指子弹和导弹，前缀b和m指代子弹和导弹
        public static int totalAmmoUse, totalAmmoHit, bAmmoUse, bAmmoHit, mAmmoUse, mAmmoHit; //数量发射和命中总和记录
        public static ConcurrentDictionary<int,long> ammoDamageOutput; //每种子弹、导弹输出总伤害
        public static ConcurrentDictionary<int,long> ammoDamageHit; //每种子弹、导弹击中总伤害，由于子弹导弹等有飞行时间，在中飞行时科技升级了，那么单个子弹注册的输出伤害会低于击中伤害，但无所谓，这种情况发生概率或者占比较小，不大会影响数据统计。此外，导弹有aoe，其伤害效率超过100%也是正常的
        public static ConcurrentDictionary<int, int> ammoUse; //每种子弹、导弹的发射量
        public static ConcurrentDictionary<int, int> ammoHit; //每种子弹、导弹击中量

        public static void InitAll()
        {
            ReInitBattleStat();
            isBattleStatTab = false;
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
            //需要改大小
            battleStatTabObj.transform.Find("cpu-panel/Scroll View").GetComponent<RectTransform>().sizeDelta = new Vector2(330,550);
            battleStatTabObj.transform.Find("gpu-panel/Scroll View").GetComponent<RectTransform>().sizeDelta = new Vector2(330, 550);
            battleStatTabObj.transform.Find("data-panel/Scroll View").GetComponent<RectTransform>().sizeDelta = new Vector2(330, 550);


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
            ammoLabel = battleStatTabObj.transform.Find("gpu-panel/Scroll View/Viewport/Content/label").GetComponent<Text>();
            ammoValue1 = battleStatTabObj.transform.Find("gpu-panel/Scroll View/Viewport/Content/value-1").GetComponent<Text>();
            ammoValue2 = battleStatTabObj.transform.Find("gpu-panel/Scroll View/Viewport/Content/value-2").GetComponent<Text>();
            enemyTitle = battleStatTabObj.transform.Find("data-panel/title-text").GetComponent<Text>();
            enemyLabel = battleStatTabObj.transform.Find("data-panel/Scroll View/Viewport/Content/label").GetComponent<Text>();
            enemyValue1 = battleStatTabObj.transform.Find("data-panel/Scroll View/Viewport/Content/value-1").GetComponent<Text>();
            enemyValue2 = battleStatTabObj.transform.Find("data-panel/Scroll View/Viewport/Content/value-2").GetComponent<Text>();

            battleStatTabObj.transform.Find("gpu-panel/Scroll View/Viewport/Content/value-1").localPosition = new Vector3(80, -4, 0); //重新设置一下位置，稍微右移防止数据重叠

        }

        public static void ReInitBattleStat()
        {
            try
            {
                resourceLost = 0;
                buildingLost = 0;
                totalDamage = 0;
                totalEnemyEliminated = 0;
                totalEnemyGen = 0;
                totalAmmoDamageHit = 0;
                totalAmmoDamageOut = 0;
                totalAmmoHit = 0;
                totalAmmoUse = 0;
                bAmmoDamageHit = 0;
                bAmmoDamageOut = 0;
                bAmmoUse = 0;
                bAmmoHit = 0;
                mAmmoDamageHit = 0;
                mAmmoDamageOut = 0;
                mAmmoUse = 0;
                mAmmoHit = 0;
                enemyGen = new ConcurrentDictionary<int, int>();
                enemyEliminated = new ConcurrentDictionary<int, int>();
                ammoDamageOutput = new ConcurrentDictionary<int, long>();
                ammoDamageHit = new ConcurrentDictionary<int, long>();
                ammoUse = new ConcurrentDictionary<int, int>();
                ammoHit = new ConcurrentDictionary<int, int>();
                //因为后面需要直接用比较方便，所以直接初始化了，后面就不用判断了
                for (int i = 8001; i < 8008; i++)
                {
                    ammoDamageOutput.AddOrUpdate(i, 0, (x, y) => 0);
                    ammoDamageHit.AddOrUpdate(i, 0, (x, y) => 0);
                    ammoUse.AddOrUpdate(i, 0, (x, y) => 0);
                    ammoHit.AddOrUpdate(i, 0, (x, y) => 0);
                }
                for (int i = 0; i < 30; i++)
                {
                    enemyGen.AddOrUpdate(i, 0, (x, y) => 0);
                    enemyEliminated.AddOrUpdate(i, 0, (x, y) => 0);
                }
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
                totalEnemyGen = EnemyShips.ships.Count;
            }
            catch (Exception)
            {
            }
        }

        //飞船被击毁
        public static void RegisterEliminate(int shipType, int elminateNum = 1) 
        {
            try 
            {
                totalEnemyEliminated += elminateNum;
                enemyEliminated.AddOrUpdate(shipType, elminateNum, (x, y) => y + elminateNum); 
            }
            catch (Exception) { }
        }

        //资源被掠夺
        public static void RegisterResourceLost(int lostnum)
        {
            resourceLost += lostnum;
        }

        //建筑被摧毁
        public static void RegisterBuildingLost(int lostnum = 1)
        {
            buildingLost += lostnum;
        }

        //子弹或者火箭发射
        public static void RegisterShootOrLaunch(int bulletId, int damage, int num = 1) 
        {
            try
            { 
                switch (bulletId)
                {
                    case 8001:
                    case 8002:
                    case 8003:
                        bAmmoUse += num;
                        bAmmoDamageOut += damage;
                        break;
                    case 8007: //总和统计时，不计算激光的子弹用量，只计算伤害，下同
                        bAmmoDamageOut += damage;
                        break;
                    case 8004:
                    case 8005:
                    case 8006:
                        mAmmoUse += num;
                        mAmmoDamageOut += damage;
                        break;
                    default:
                        break;
                }
                if (bulletId != 8007) 
                    totalAmmoUse += num;
                totalAmmoDamageOut += damage;
                ammoUse.AddOrUpdate(bulletId, num, (x, y) => y + num);
                ammoDamageOutput.AddOrUpdate(bulletId, damage, (x, y) => y + damage);
            }
            catch (Exception) { }
        }

        //子弹或者火箭命中
        public static void RegisterHit(int bulletId, int damage, int num = 1) 
        {
            try
            {
                totalDamage += damage;
                switch (bulletId)
                {
                    case 8001:
                    case 8002:
                    case 8003:
                        bAmmoHit += num;
                        bAmmoDamageHit += damage;
                        break;
                    case 8007:
                        bAmmoDamageHit += damage;
                        break;
                    case 8004:
                    case 8005:
                    case 8006:
                        mAmmoHit += num;
                        mAmmoDamageHit += damage;
                        break;
                    default:
                        break;
                }
                if(bulletId != 8007) //不计算激光的子弹用量
                    totalAmmoHit += num;
                totalAmmoDamageHit += damage;
                ammoHit.AddOrUpdate(bulletId, num, (x, y) => y + num);
                ammoDamageHit.AddOrUpdate(bulletId, damage, (x, y) => y + damage);
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
            UIAlert.ShowAlert(false); //关闭顶部警告避免遮挡？
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

                List<double> ammoAmoutProps = new List<double> { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 }; //数量的效率,1=8001,...,7=8007，0是无效的
                List<double> ammoDamageProps = new List<double> { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 }; //伤害的效率，同上，0是无效的
                List<double> enemyEliminatedProps = new List<double>(); //敌人消灭比例，0代表总和

                for (int i = 8001; i < 8008; i++)
                {
                    if (ammoUse[i] > 0) ammoAmoutProps[i - 8000] = ammoHit[i] * 1.0 / ammoUse[i];
                    if (ammoDamageOutput[i] > 0) ammoDamageProps[i - 8000] = ammoDamageHit[i] * 1.0 / ammoDamageOutput[i];
                }

                double totalAmoutProp = 1.0, totalDamageProp = 1.0, bAmoutProp = 1.0, bDamageProp = 1.0, mAmoutProp = 1.0, mDamageProp = 1.0;
                if (totalAmmoUse > 0) totalAmoutProp = totalAmmoHit * 1.0 / totalAmmoUse;
                if (bAmmoUse > 0) bAmoutProp = bAmmoHit * 1.0 / bAmmoUse;
                if (mAmmoUse > 0) mAmoutProp = mAmmoHit * 1.0 / mAmmoUse;
                if (totalAmmoDamageOut > 0) totalDamageProp = totalAmmoDamageHit * 1.0 / totalAmmoDamageOut;
                if (bAmmoDamageOut > 0) bDamageProp = bAmmoDamageHit * 1.0 / bAmmoDamageOut;
                if (mAmmoDamageOut > 0) mDamageProp = mAmmoDamageHit * 1.0 / mAmmoDamageOut;

                
                for (int i = 0; i < 30; i++)
                {
                    enemyEliminatedProps.Add(0);
                    if (enemyGen.ContainsKey(i) && enemyGen[i] > 0)
                        enemyEliminatedProps[i] = enemyEliminated[i] * 1.0 / enemyGen[i];
                    else
                        enemyEliminatedProps[i] = 0;
                }
                if (totalEnemyGen > 0)
                    enemyEliminatedProps[0] = totalEnemyEliminated * 1.0 / totalEnemyGen;


                briefLabel.text = "歼灭敌舰".Translate() + "\n" + "输出伤害".Translate() + "\n" + "损失建筑".Translate() + "\n" + "损失资源".Translate();
                briefValue1.text = "";
                briefValue2.text = totalEnemyEliminated.ToString("N0") + "\n" + totalDamage.ToString("N0") + "\n" + buildingLost.ToString("N0") + "\n" + resourceLost.ToString("N0");

                ammoLabel.text = "\n" + 
                    "数量总计".Translate() + "\n" + "伤害总计".Translate() + "\n" +
                    "子弹数量".Translate() + "\n" + "  > " + "子弹1".Translate() + "\n" + "  > " + "子弹2".Translate() + "\n" + "  > " + "子弹3".Translate() + "\n" + "  > " + "脉冲".Translate() + "\n" +
                    "导弹数量".Translate() + "\n" + "  > " + "导弹1".Translate() + "\n" + "  > " + "导弹2".Translate() + "\n" + "  > " + "导弹3".Translate() + "\n" +
                    "子弹伤害".Translate() + "\n" + "  > " + "子弹1".Translate() + "\n" + "  > " + "子弹2".Translate() + "\n" + "  > " + "子弹3".Translate() + "\n" + "  > " + "脉冲".Translate() + "\n" +
                    "导弹伤害".Translate() + "\n" + "  > " + "导弹1".Translate() + "\n" + "  > " + "导弹2".Translate() + "\n" + "  > " + "导弹3".Translate() + "\n";

                ammoValue1.text = "击中".Translate() + "/" + "发射".Translate() + "\n" +
                    $"{totalAmmoHit}/{totalAmmoUse}\n{totalAmmoDamageHit}/{totalAmmoDamageOut}\n" +
                    $"{bAmmoHit}/{bAmmoUse}\n{ammoHit[8001]}/{ammoUse[8001]}\n {ammoHit[8002]}/{ammoUse[8002]}\n {ammoHit[8003]}/{ammoUse[8003]}\n {ammoHit[8007]}/{ammoUse[8007]}\n" + 
                    $"{mAmmoHit}/{mAmmoUse}\n{ammoHit[8004]}/{ammoUse[8004]}\n {ammoHit[8005]}/{ammoUse[8005]}\n {ammoHit[8006]}/{ammoUse[8006]}\n" +
                    $"{bAmmoDamageHit}/{bAmmoDamageOut}\n{ammoDamageHit[8001]}/{ammoDamageOutput[8001]}\n {ammoDamageHit[8002]}/{ammoDamageOutput[8002]}\n {ammoDamageHit[8003]}/{ammoDamageOutput[8003]}\n {ammoDamageHit[8007]}/{ammoDamageOutput[8007]}\n" +
                    $"{mAmmoDamageHit}/{mAmmoDamageOut}\n{ammoDamageHit[8004]}/{ammoDamageOutput[8004]}\n {ammoDamageHit[8005]}/{ammoDamageOutput[8005]}\n {ammoDamageHit[8006]}/{ammoDamageOutput[8006]}\n";

                ammoValue2.text = "效率gm".Translate() + "\n" +
                    totalAmoutProp.ToString("0.00%")+ "\n" + totalDamageProp.ToString("0.00%") + "\n" +
                    $"{bAmoutProp:0.00%}\n{ammoAmoutProps[1]:0.00%}\n{ammoAmoutProps[2]:0.00%}\n{ammoAmoutProps[3]:0.00%}\n{ammoAmoutProps[7]:0.00%}\n" + 
                    $"{mAmoutProp:0.00%}\n{ammoAmoutProps[4]:0.00%}\n{ammoAmoutProps[5]:0.00%}\n{ammoAmoutProps[6]:0.00%}\n" +
                    $"{bDamageProp:0.00%}\n{ammoDamageProps[1]:0.00%}\n{ammoDamageProps[2]:0.00%}\n{ammoDamageProps[3]:0.00%}\n{ammoDamageProps[7]:0.00%}\n" +
                    $"{mDamageProp:0.00%}\n{ammoDamageProps[4]:0.00%}\n{ammoDamageProps[5]:0.00%}\n{ammoDamageProps[6]:0.00%}\n";

                enemyLabel.text = "\n" +
                    "总计".Translate() + "\n" + "  > " + "护卫舰".Translate() + "\n" + "  > " + "驱逐舰".Translate() + "\n" + "  > " + "巡洋舰".Translate() + "\n" + "  > " + "战列舰".Translate() + "\n" + "  > " + "泰坦".Translate();
                enemyValue1.text = "已歼灭".Translate() + "/" + "已产生".Translate() + "\n" +
                    $"{totalEnemyEliminated}/{totalEnemyGen}\n{enemyEliminated[1]}/{enemyGen[1]}\n{enemyEliminated[4]}/{enemyGen[4]}\n{enemyEliminated[8]}/{enemyGen[8]}\n{enemyEliminated[9]}/{enemyGen[9]}\n{enemyEliminated[15]}/{enemyGen[15]}";
                enemyValue2.text = "歼灭".Translate() + "\n" +
                    $"{enemyEliminatedProps[0]:0.00%}\n{enemyEliminatedProps[1]:0.00%}\n{enemyEliminatedProps[4]:0.00%}\n{enemyEliminatedProps[8]:0.00%}\n{enemyEliminatedProps[9]:0.00%}\n{enemyEliminatedProps[15]:0.00%}\n";
            }
        }

    }
}
