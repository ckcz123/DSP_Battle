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

            CountDownRefresh(false, 0, 0, 0, 0, 0, 0, false);
            isActive = false;
            titleObj.SetActive(false);
            statisticObj.SetActive(false);

        }

        /// <summary>
        /// 如果是active状态，则每帧刷新？
        /// </summary>
        /// <param name="framesUntilAttack"></param> 距离下一波还有多少帧
        /// <param name="shipNum"></param> 下一波来袭的敌舰总数
        /// <param name="totalStrength"></param> 下一波来袭的敌舰总强度
        /// <param name="awards"></param> 下一波完成的最大奖励
        /// <param name="showDetails"></param> 是否能够显示下一波来袭的具体细节，例如时间倒数和敌舰信息
        public static void CountDownRefresh(bool Invasion, int framesUntilNextWave, int starIndex, int shipNum, int totalStrength, int awards, int destoryed = 0, bool showDetails = false)
        {

            if (framesUntilNextWave == 3600 || framesUntilNextWave == 600 || framesUntilNextWave == -1)
                ShowAlert();

            if(!Invasion)
            {
                alertMainText.text = "未探测到威胁".Translate();
                stat1label.text = "预估数量".Translate();
                stat2label.text = "预估强度".Translate();
                stat3label.text = "携带资源".Translate();
                stat1value.text = "0";
                stat2value.text = "0";
                stat3value.text = "0";
            }

            if (framesUntilNextWave < 18000)
                showDetails = true;
            if (framesUntilNextWave < 0)
            {
                if ((-framesUntilNextWave) / 60 % 2 != 0)
                    alertMainText.text = txtColorAlert1 + "敌舰正在入侵".Translate() + GameMain.galaxy.stars[starIndex].displayName + "!" + txtColorRight;
                else
                    alertMainText.text = txtColorAlert2 + "敌舰正在入侵".Translate() + GameMain.galaxy.stars[starIndex].displayName + "!" + txtColorRight;

                stat1label.text = "剩余敌舰".Translate();
                stat2label.text = "剩余强度".Translate();
                stat3label.text = "已被摧毁".Translate();
                stat1value.text = shipNum.ToString();
                stat2value.text = totalStrength.ToString();
                stat3value.text = destoryed.ToString();
            }
            else
            {
                int seconds = framesUntilNextWave / 60;
                alertMainText.text = "下一次入侵预计于".Translate() + Sec2StrTime(seconds, showDetails) + "后抵达".Translate() + GameMain.galaxy.stars[starIndex].displayName;
                stat1label.text = "预估数量".Translate();
                stat2label.text = "预估强度".Translate();
                stat3label.text = "携带资源".Translate();
                stat1value.text = RoundByDetail(shipNum, showDetails);
                stat2value.text = RoundByDetail(totalStrength, showDetails);
                stat3value.text = RoundByDetail(awards, showDetails);
            }
        }


        public static void OnActiveChange()
        {
            isActive = !isActive;
            titleObj.SetActive(isActive);
            statisticObj.SetActive(isActive);
        }

        public static void ShowAlert()
        {
            isActive = true;
            titleObj.SetActive(isActive);
            statisticObj.SetActive(isActive);
        }

        public static void ClearAlert()
        {

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
