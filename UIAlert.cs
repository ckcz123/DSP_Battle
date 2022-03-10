using HarmonyLib;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    class UIAlert
    {
        //存档内容
        public static bool isActive = false;

        public static long totalDistance = 1;
        public static long totalStrength = 1;
        public static float elimPointRatio = 1.0f;

        public static int lastState = 0;

        public static GameObject titleObj = null;
        public static GameObject statisticObj = null;
        public static GameObject titleLeftBar = null;
        public static GameObject titleRightBar = null;
        public static GameObject eliminateProgressBar = null;
        public static GameObject invasionProgressBar = null;
        public static Text alertMainText;
        public static Text stat1label;
        public static Text stat1value;
        public static Text stat2label;
        public static Text stat2value;
        public static Text stat3label;
        public static Text stat3value;
        public static Text helpInfo;
        public static RectTransform elimProgRT;
        public static RectTransform invaProgRT;

        public static string txtColorWarn1 = "<color=#ffa800>";
        public static string txtColorWarn2 = "<color=#ff7000>";
        public static string txtColorAlert1 = "<color=#e30000>";
        public static string txtColorAlert2 = "<color=#a10000>";
        public static string txtColorRight = "</color>";

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIRoot), "OnGameLoadStart")]
        public static void UIRoot_OnGameLoadStart()
        {
            if (!DSPGame.IsMenuDemo)
            {
                if (titleObj != null)
                {
                    titleObj.SetActive(false);
                    statisticObj.SetActive(false);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIRoot), "OnGameBegin")]
        public static void UIRoot_OnGameBegin()
        {
            if (!DSPGame.IsMenuDemo)
            {
                if (titleObj != null)
                {
                    titleObj.SetActive(isActive);
                    statisticObj.SetActive(isActive);
                    RefreshUIAlert(GameMain.instance.timei, true);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameSave), "LoadCurrentGame")]
        public static void InitUIAlert()
        {
            if (titleObj != null)
                return;
            GameObject overlayCanvas = GameObject.Find("UI Root/Overlay Canvas");

            GameObject oriTitleObj = GameObject.Find("UI Root/Overlay Canvas/Milky Way UI/milky-way-screen-ui/top-title");
            titleObj = GameObject.Instantiate(oriTitleObj);
            titleObj.name = "battle-alert-title";
            titleObj.transform.SetParent(overlayCanvas.transform, false);
            titleObj.transform.localPosition = new Vector3(0, 480, 0);
            titleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 70);

            GameObject oriStatisticObj = GameObject.Find("UI Root/Overlay Canvas/Milky Way UI/milky-way-screen-ui/statistics");
            statisticObj = GameObject.Instantiate(oriStatisticObj);
            statisticObj.name = "battle-alert-stat";
            statisticObj.transform.SetParent(overlayCanvas.transform, false);
            statisticObj.transform.localPosition = new Vector3(0, 330, 0);

            titleLeftBar = titleObj.transform.Find("left").gameObject;
            titleRightBar = titleObj.transform.Find("right").gameObject;
            titleLeftBar.GetComponent<RectTransform>().sizeDelta = new Vector2(472, 12);
            titleRightBar.GetComponent<RectTransform>().sizeDelta = new Vector2(472, 12);

            eliminateProgressBar = GameObject.Instantiate(titleLeftBar);
            invasionProgressBar = GameObject.Instantiate(titleRightBar);
            eliminateProgressBar.transform.SetParent(titleObj.transform, false);
            invasionProgressBar.transform.SetParent(titleObj.transform, false);
            eliminateProgressBar.transform.localPosition = new Vector3(-500, -50, 0);
            invasionProgressBar.transform.localPosition = new Vector3(500, -50, 0);
            eliminateProgressBar.GetComponent<Image>().color = new Color(0.26f, 1f, 0.46f, 1f);
            invasionProgressBar.GetComponent<Image>().color = new Color(1f, 0.08f, 0.08f, 1f);
            elimProgRT = eliminateProgressBar.GetComponent<RectTransform>();
            invaProgRT = invasionProgressBar.GetComponent<RectTransform>();
            elimProgRT.sizeDelta = new Vector2(0, 12);
            invaProgRT.sizeDelta = new Vector2(0, 12);
            elimProgRT.localScale = new Vector3(1, 2, 1);
            invaProgRT.localScale = new Vector3(1, 2, 1);


            alertMainText = titleObj.GetComponent<Text>();
            alertMainText.supportRichText = true;
            Transform sons = statisticObj.transform.Find("desc-mask/desc");
            stat1label = sons.Find("dyson-cnt-label").GetComponent<Text>();
            stat1value = sons.Find("dyson-cnt-text").GetComponent<Text>();
            stat2label = sons.Find("dyson-gen-label").GetComponent<Text>();
            stat2value = sons.Find("dyson-gen-text").GetComponent<Text>();
            stat3label = sons.Find("sail-cnt-label").GetComponent<Text>();
            stat3value = sons.Find("sail-cnt-text").GetComponent<Text>();

            GameObject myStat1LabelObj = sons.Find("dyson-cnt-label").gameObject;
            GameObject addHelpObj = GameObject.Instantiate(myStat1LabelObj);
            addHelpObj.transform.SetParent(titleObj.transform, false);
            addHelpObj.transform.localPosition = new Vector3(500, -60, 0);
            addHelpObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 70);
            helpInfo = addHelpObj.GetComponent<Text>();

            isActive = false;
            titleObj.SetActive(false);
            statisticObj.SetActive(false);

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {
            RefreshUIAlert(time, false);
            RefreshBattleProgress(time);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameMain), "Pause")]
        public static void OnPaused()
        {
            titleObj.SetActive(false);
            statisticObj.SetActive(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameMain), "Resume")]
        public static void OnResumed()
        {
            titleObj.SetActive(isActive);
            statisticObj.SetActive(isActive);
            RefreshUIAlert(GameMain.instance.timei, true);
        }

        public static void RefreshUIAlert(long time, bool forceRefresh = false)
        {
            
            if (DSPGame.IsMenuDemo)
            {
                ShowAlert(false);
                return;
            }
            if (time % 30 != 1 && !forceRefresh) return;
            helpInfo.text = "按下退格键开启或关闭此窗口，按下减号键使敌军进攻时间提前1分钟。".Translate();
            if (Configs.nextWaveState == 0 && lastState != 0) //刚刚打完一架，关闭警告
            {
                ShowAlert(false);
            }
            lastState = Configs.nextWaveState;

            if (Configs.nextWaveState == 0 || Configs.nextWaveFrameIndex < 0 || Configs.nextWaveStarIndex < 0)
            {
                alertMainText.text = "未探测到威胁".Translate();
                stat1label.text = "预估数量".Translate();
                stat2label.text = "预估强度".Translate();
                stat3label.text = "携带资源".Translate();
                stat1value.text = "-";
                stat2value.text = "-";
                stat3value.text = "-";
                return;
            }

            long framesUntilNextWave = Configs.nextWaveFrameIndex - time;
            bool showDetails = framesUntilNextWave < 18000;

            if (framesUntilNextWave == 60 * 60 * 30 || framesUntilNextWave == 60 * 60 * 60 || framesUntilNextWave == 36000) ShowAlert(true);


            if (framesUntilNextWave < 0)
            {
                if ((-framesUntilNextWave) / 60 % 2 != 0)
                    alertMainText.text = txtColorAlert1 + "敌舰正在入侵".Translate() + GameMain.galaxy.stars[Configs.nextWaveStarIndex].displayName + "!" + txtColorRight;
                else
                    alertMainText.text = txtColorAlert2 + "敌舰正在入侵".Translate() + GameMain.galaxy.stars[Configs.nextWaveStarIndex].displayName + "!" + txtColorRight;

                stat1label.text = "剩余敌舰".Translate();
                stat2label.text = "剩余强度".Translate();
                stat3label.text = "已被摧毁".Translate();
                stat1value.text = EnemyShips.ships.Count.ToString();
                stat2value.text = EnemyShips.ships.Values.ToList().Sum(e => e.intensity).ToString();
                stat3value.text = (Configs.nextWaveEnemy.Sum() - EnemyShips.ships.Count).ToString();
            }
            else
            {
                int seconds = (int)framesUntilNextWave / 60;
                alertMainText.text = "下一次入侵预计于".Translate() + Sec2StrTime(seconds, showDetails) + "后抵达".Translate() + GameMain.galaxy.stars[Configs.nextWaveStarIndex].displayName;
                stat1label.text = "预估数量".Translate();
                stat2label.text = "预估强度".Translate();
                stat3label.text = "虫洞数量".Translate();
                stat1value.text = Configs.nextWaveEnemy.Sum().ToString();
                stat2value.text = Configs.nextWaveIntensity.ToString();
                stat3value.text = Configs.nextWaveWormCount.ToString();
            }
        }

        public static void RefreshBattleProgress(long time)
        {
            int curState = Configs.nextWaveState;
            try
            {

                if ((lastState != 3 && curState == 3) || (curState == 3 && totalDistance == 1 && totalStrength == 1))
                {
                    totalStrength = 0;
                    foreach (var shipIndex in EnemyShips.ships.Keys)
                    {
                        var ship = EnemyShips.ships[shipIndex];
                        PlanetFactory planetFactory = GameMain.galaxy.PlanetById(ship.shipData.planetB).factory;
                        Vector3 stationPos = planetFactory.entityPool[ship.targetStation.entityId].pos;
                        int planetId = planetFactory.planetId;
                        AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                        VectorLF3 stationUpos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, stationPos);

                        totalDistance += (long)(stationUpos - ship.uPos).magnitude;
                        totalStrength += ship.hp;
                    }
                }
                if (curState != 3 || totalStrength < 1) totalStrength = 1;
                if (curState != 3 || totalDistance <= 0) totalDistance = 1;
                if (lastState == 3 && curState != 3)
                {
                    totalDistance = 1;
                    totalStrength = 1;
                    elimPointRatio = 1.0f;
                    elimProgRT.sizeDelta = new Vector2(0, 12);
                    invaProgRT.sizeDelta = new Vector2(0, 12);
                }
                if (curState == 3) //要刷新进度条
                {
                    long curTotalDistance = 0;
                    long curTotalStrength = 0;
                    foreach (var shipIndex in EnemyShips.ships.Keys)
                    {
                        var ship = EnemyShips.ships[shipIndex];
                        PlanetFactory planetFactory = GameMain.galaxy.PlanetById(ship.shipData.planetB).factory;
                        Vector3 stationPos = planetFactory.entityPool[ship.targetStation.entityId].pos;
                        int planetId = planetFactory.planetId;
                        AstroPose[] astroPoses = GameMain.galaxy.astroPoses;
                        VectorLF3 stationUpos = astroPoses[planetId].uPos + Maths.QRotateLF(astroPoses[planetId].uRot, stationPos);

                        curTotalDistance += (long)(stationUpos - ship.uPos).magnitude;
                        curTotalStrength += ship.hp;
                    }
                    double elimPoint = (totalStrength - curTotalStrength) * 1.0 / totalStrength;
                    double invaPoint = Mathf.Min((float)((totalDistance - curTotalDistance) * 1.0 / totalDistance), (float)(1-elimPoint));
                    if (invaPoint < 0) invaPoint = 0;

                    double totalPoint = elimPoint + invaPoint;
                    if (totalPoint <= 0)
                    {
                        elimProgRT.sizeDelta = new Vector2(500, 12);
                        invaProgRT.sizeDelta = new Vector2(500, 12);
                    }
                    else
                    {
                        float leftProp = (float)(elimPoint / totalPoint);
                        float deRatio = 1.0f;
                        if(elimPoint > 0.99)//快消灭干净了，让绿条多填充，弥补一半之前建筑炸掉的比例减成
                        {
                            deRatio = 0.5f / elimPointRatio;
                        }
                        elimProgRT.sizeDelta = new Vector2(1000 * leftProp * elimPointRatio * deRatio + 2, 12);
                        invaProgRT.sizeDelta = new Vector2(1000 * (1 - leftProp * elimPointRatio * deRatio) + 2, 12);
                    }
                }
            }
            catch (System.Exception)
            {

            }

            lastState = curState;
        }
        public static void OnActiveChange()
        {
            isActive = !isActive;
            titleObj.SetActive(isActive);
            statisticObj.SetActive(isActive);
            RefreshUIAlert(GameMain.instance.timei, true);
        }

        public static void ShowAlert(bool active)
        {
            if (isActive == active) return;
            isActive = active;
            titleObj.SetActive(isActive);
            statisticObj.SetActive(isActive);
            RefreshUIAlert(GameMain.instance.timei, true);
        }

        static string Sec2StrTime(int sec, bool showDetails)
        {
            string res = "";
            string left = "";
            string right = "";
            if (sec > 3600)
            {
                res += (sec / 3600).ToString() + "小时gm".Translate();
                if (!showDetails)
                {
                    return "约gm".Translate() + res;
                }
            }
            if (sec > 60)
            {
                res += ((sec % 3600) / 60).ToString() + "分gm".Translate();
                if (sec < 300)
                {
                    left = txtColorWarn1;
                    right = txtColorRight;
                }
                if (!showDetails)
                {
                    return "约gm".Translate() + left + res + right;
                }
            }
            if (sec == 60)
                res += "60" + "秒gm".Translate();
            else
                res += (sec % 60).ToString() + "秒gm".Translate();
            if (sec <= 60)
            {
                left = txtColorAlert1;
                right = txtColorRight;
            }
            return left + res + right;
        }

        public static void Export(BinaryWriter w)
        {
            w.Write(isActive);
        }

        public static void Import(BinaryReader r)
        {
            totalStrength = 1;
            totalDistance = 1;
            elimPointRatio = 1.0f;
            elimProgRT.sizeDelta = new Vector2(0, 12);
            invaProgRT.sizeDelta = new Vector2(0, 12);
            isActive = r.ReadBoolean();
        }

        public static void IntoOtherSave()
        {
            totalStrength = 1;
            totalDistance = 1;
            elimPointRatio = 1.0f;
            elimProgRT.sizeDelta = new Vector2(0, 12);
            invaProgRT.sizeDelta = new Vector2(0, 12);
            isActive = false;
        }
    }
}
