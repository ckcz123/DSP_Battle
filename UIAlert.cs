using System;
using System.Collections.Generic;
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
            if (DSPGame.IsMenuDemo) return;
            if (Configs.nextWaveState == 0 || Configs.nextWaveFrameIndex < 0 || Configs.nextWavePlanetId < 0)
            {
                ShowAlert(false);
                return;
            }

            long framesUntilNextWave = Configs.nextWaveFrameIndex - time;
            bool showDetails = framesUntilNextWave < 18000;

            if (showDetails || framesUntilNextWave == 60 * 60 * 30 || framesUntilNextWave == 60 * 60 * 60 || framesUntilNextWave == 36000) ShowAlert(true);

            if (time % 30 != 1) return;

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
                int seconds = (int) framesUntilNextWave / 60;
                alertMainText.text = "下一次入侵预计于".Translate() + Sec2StrTime(seconds, showDetails) + "后抵达".Translate() + GameMain.galaxy.stars[Configs.nextWavePlanetId / 100 - 1].displayName;
                stat1label.text = "预估数量".Translate();
                stat2label.text = "预估强度".Translate();
                stat3label.text = "虫洞数量".Translate();
                stat1value.text = RoundByDetail(Configs.nextWaveEnemy.Sum(), showDetails);
                stat2value.text = RoundByDetail(Configs.nextWaveIntensity, showDetails);
                stat3value.text = Configs.nextWaveWormCount.ToString();
            }
        }



        public static void OnActiveChange()
        {
            isActive = !isActive;
            titleObj.SetActive(isActive);
            statisticObj.SetActive(isActive);
        }

        public static void ShowAlert(bool active)
        {
            if (isActive == active) return;
            isActive = active;
            titleObj.SetActive(isActive);
            statisticObj.SetActive(isActive);
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

        static string RoundByDetail(int num, bool showDetails)
        {
            if (showDetails)
                return num.ToString();
            else
            {
                if (num > 1000000)
                    return "约".Translate() + (num / 1000000).ToString() + "M";
                else if (num > 1000)
                    return "约".Translate() + (num / 1000).ToString() + "k";
                else if (num > 100)
                    return "约".Translate() + (num / 100 * 100).ToString();
                else if (num > 10)
                    return "约".Translate() + (num / 10 * 10).ToString();
                else
                    return "约".Translate() + "10";
            }
        }
    }
}
