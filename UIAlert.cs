using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    class UIAlert
    {
        public static bool isActive = false;

        public static int lastState = 0;

        public static GameObject titleObj = null;
        public static GameObject statisticObj = null;
        public static GameObject titleLeftBar = null;
        public static GameObject titleRightBar = null;
        public static Text alertMainText;
        public static Text stat1label;
        public static Text stat1value;
        public static Text stat2label;
        public static Text stat2value;
        public static Text stat3label;
        public static Text stat3value;

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

            alertMainText = titleObj.GetComponent<Text>();
            alertMainText.supportRichText = true;
            Transform sons = statisticObj.transform.Find("desc-mask/desc");
            stat1label = sons.Find("dyson-cnt-label").GetComponent<Text>();
            stat1value = sons.Find("dyson-cnt-text").GetComponent<Text>();
            stat2label = sons.Find("dyson-gen-label").GetComponent<Text>();
            stat2value = sons.Find("dyson-gen-text").GetComponent<Text>();
            stat3label = sons.Find("sail-cnt-label").GetComponent<Text>();
            stat3value = sons.Find("sail-cnt-text").GetComponent<Text>();

            isActive = false;
            titleObj.SetActive(false);
            statisticObj.SetActive(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {
            RefreshUIAlert(time, false);
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

            if(Configs.nextWaveState == 0 && lastState != 0) //刚刚打完一架，关闭警告
            {
                ShowAlert(false);
            }
            lastState = Configs.nextWaveState;

            if (Configs.nextWaveState == 0 || Configs.nextWaveFrameIndex < 0 || Configs.nextWavePlanetId < 0)
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

            if (showDetails || framesUntilNextWave == 60 * 60 * 30 || framesUntilNextWave == 60 * 60 * 60 || framesUntilNextWave == 36000) ShowAlert(true);


            if (framesUntilNextWave < 0)
            {
                if ((-framesUntilNextWave) / 60 % 2 != 0)
                    alertMainText.text = txtColorAlert1 + "敌舰正在入侵".Translate() + GameMain.galaxy.stars[Configs.nextWavePlanetId / 100 - 1].displayName + "!" + txtColorRight;
                else
                    alertMainText.text = txtColorAlert2 + "敌舰正在入侵".Translate() + GameMain.galaxy.stars[Configs.nextWavePlanetId / 100 - 1].displayName + "!" + txtColorRight;

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
                alertMainText.text = "下一次入侵预计于".Translate() + Sec2StrTime(seconds, showDetails) + "后抵达".Translate() + GameMain.galaxy.stars[Configs.nextWavePlanetId / 100 - 1].displayName;
                stat1label.text = "预估数量".Translate();
                stat2label.text = "预估强度".Translate();
                stat3label.text = "虫洞数量".Translate();
                stat1value.text = Configs.nextWaveEnemy.Sum().ToString();
                stat2value.text = Configs.nextWaveIntensity.ToString();
                stat3value.text = Configs.nextWaveWormCount.ToString();
            }
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
                res += (sec / 3600).ToString() + "小时".Translate();
                if(!showDetails)
                {
                    return "约".Translate() + res;
                }
            }
            if(sec > 60)
            {
                res += ((sec % 3600) / 60).ToString() + "分".Translate();
                if(sec < 300)
                {
                    left = txtColorWarn1;
                    right = txtColorRight;
                }
                if(!showDetails)
                {
                    return "约".Translate() + left + res + right;
                }
            }
            if (sec == 60)
                res += "60" + "秒".Translate();
            else
                res += (sec % 60).ToString() + "秒".Translate();
            if(sec <= 60)
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
            isActive = r.ReadBoolean();
        }

        public static void IntoOtherSave()
        {
            isActive = false;
        }
    }
}
