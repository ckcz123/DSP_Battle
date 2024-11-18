using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    public class UIRank
    {
        public static GameObject rankObj = null;
        public static GameObject rankTextObj;
        public static GameObject rankIconObj;
        public static GameObject rankExpTextObj;
        public static GameObject rankExpBarObj;
        public static GameObject frontBarObj;
        public static GameObject backBarObj;
        public static GameObject attentionMarkObj;
        public static Image rankIcon;
        public static Image expBarImg;
        public static Text rankText;
        public static Text expText;
        public static Text attentionMarkText;
        static string color1Left = "<color=#c2853d>";
        static string colorRight = "</color>";
        public static Text promotionNoticeMainText = null;
        public static Text promotionNoticeSubText = null;
        public static UIButton uiBtn = null;
        public static Text tipTxtTitle = null;
        public static Color rank03Color = new Color(0.38f, 0.847f, 1f, 0.7f);
        public static Color rank46Color = new Color(0.823f, 0.22f, 1f, 0.7f);
        public static Color rank79Color = new Color(0.99f, 0.588f, 0.125f, 0.753f);

        public static Sprite[] rank = new Sprite[12];

        public static void InitUI()
        {
            if (rankObj != null)
            {
                ForceRefreshAll();
                return;
            }
            GameObject oriTextObj = GameObject.Find("UI Root/Overlay Canvas/Milky Way UI/milky-way-screen-ui/statistics/desc-mask/desc/dyson-gen-text");
            if (oriTextObj == null)
            {
                rankObj = null;
                return;
            }
            rankObj = new GameObject();
            GameObject inGameObj = GameObject.Find("UI Root/Overlay Canvas/In Game");

            rankObj.name = "BattleRank";
            rankObj.transform.SetParent(inGameObj.transform);

            rankObj.transform.localPosition = new Vector3(DSPGame.globalOption.resolution.width * 1.0f * DSPGame.globalOption.uiLayoutHeight / DSPGame.globalOption.resolution.height / 2 - 80, DSPGame.globalOption.uiLayoutHeight / 2 - 120);
            rankObj.transform.SetAsFirstSibling();
            rankObj.transform.localScale = new Vector3(1, 1, 1);

            GameObject oriIconWithTips = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/content/icon");
            rankIconObj = GameObject.Instantiate(oriIconWithTips);
            rankIconObj.name = "icon";
            rankIconObj.transform.SetParent(rankObj.transform);
            rankIconObj.transform.localPosition = new Vector3(0, 40, 0);
            rankIconObj.transform.localScale = new Vector3(1, 1, 1);
            rankIcon = rankIconObj.GetComponent<Image>();
            if (rank[0] == null) {
                rank[0] = Resources.Load<Sprite>("Assets/DSPBattle/rank0");
            }
            rankIcon.sprite = rank[0];
            uiBtn = rankIconObj.GetComponent<UIButton>();
            Button rankClickButton = rankIconObj.AddComponent<Button>();
            rankClickButton.onClick.RemoveAllListeners();
            rankClickButton.onClick.AddListener(() => { UISkillPointsWindow.Switch(); });

            rankTextObj = GameObject.Instantiate(oriTextObj);
            rankTextObj.name = "text-title";
            rankTextObj.transform.SetParent(rankObj.transform);
            rankTextObj.transform.localPosition = new Vector3(0, -40, 0);
            rankTextObj.transform.localScale = new Vector3(1, 1, 1);
            rankText = rankTextObj.GetComponent<Text>();
            rankText.lineSpacing = 0.75f;
            rankText.fontSize = 14;
            rankText.color = new Color(1, 1, 1, 0.5f);
            rankText.alignment = TextAnchor.UpperCenter;
            rankText.supportRichText = true;
            rankText.text = ("gmRank" + Rank.rank.ToString()).Translate();// + "\n" + exp.ToString() + "/" + Configs.expToNextRank[rank];

            rankExpBarObj = new GameObject();
            rankExpBarObj.transform.SetParent(rankObj.transform);
            rankExpBarObj.name = "expbar";
            rankExpBarObj.transform.localScale = new Vector3(1, 1, 1);
            rankExpBarObj.transform.localPosition = new Vector3(0, 0, 0);
            frontBarObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/inspector/sphere-group/sail-stat/bar-group/bar-blue"), rankExpBarObj.transform);
            frontBarObj.name = "bar-front";
            frontBarObj.transform.localPosition = new Vector3(0, -62, 0);
            frontBarObj.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 2);
            frontBarObj.GetComponent<Image>().fillAmount = 1f;
            expBarImg = frontBarObj.GetComponent<Image>();
            expBarImg.fillAmount = 0f;
            backBarObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/inspector/sphere-group/sail-stat/bar-group/bar-bg"), rankExpBarObj.transform);
            backBarObj.name = "bar-bg";
            backBarObj.transform.localPosition = new Vector3(0, -62, 0);
            backBarObj.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 2);
            backBarObj.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1f);

            rankExpTextObj = GameObject.Instantiate(oriTextObj);
            rankExpTextObj.name = "text-exp";
            rankExpTextObj.transform.SetParent(rankObj.transform);
            rankExpTextObj.transform.localPosition = new Vector3(0, -62, 0);
            rankExpTextObj.transform.localScale = new Vector3(1, 1, 1);
            expText = rankExpTextObj.GetComponent<Text>();
            expText.lineSpacing = 0.75f;
            expText.fontSize = 11;
            expText.color = new Color(1, 1, 1, 0.5f);
            expText.alignment = TextAnchor.UpperCenter;
            expText.supportRichText = true;
            expText.text = "";

            attentionMarkObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Assembler Window/produce/circle-back/cnt-text"), rankObj.transform);
            attentionMarkObj.name = "attentionmark";
            attentionMarkObj.transform.localPosition = new Vector3(46f, 22f, 0);
            attentionMarkObj.transform.localScale = new Vector3(1, 1, 1);
            attentionMarkText = attentionMarkObj.GetComponent<Text>();
            attentionMarkText.text = "□";
            attentionMarkText.alignment = TextAnchor.MiddleRight;
            attentionMarkText.fontSize = 30;


            if (Rank.rank >= 0 && Rank.rank <= 10) ForceRefreshAll();
        }


        /// <summary>
        /// 在创建tip后，将需要设置的title设置为支持richtext
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIButtonTip), "Create")]
        public static void UIButtonTipTitleSetSupportRichText(string title, ref UIButtonTip __result)
        {
            if (title != null && title.Length > 0 && title[0] == '<') //对于<color=#>形式的标题，修改标题支持richtext
            {
                __result.titleComp.supportRichText = true;
            }
            //if (uiBtn?.tip != null)
            //{
            //    try
            //    {
            //        tipTxtTitle = uiBtn.tip.GetComponent<UIButtonTip>().titleComp;
            //        tipTxtTitle.supportRichText = true;
            //    }
            //    catch (Exception) { }
            //}
        }

        /// <summary>
        /// 不需要每帧刷新的，但是鼠标移入、升级、读存档等时候需要刷新的
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIButton), "OnPointerEnter")]
        public static void ForceRefreshAll()
        {
            if (rankObj == null) return;
            rankIconObj.SetActive(true);
            rankText.text = ("gmRank" + Rank.rank.ToString()).Translate();
            if (rank[Rank.rank] == null) {
                rank[Rank.rank] = Resources.Load<Sprite>($"Assets/DSPBattle/rank{Rank.rank}");
            }
            rankIcon.sprite = rank[Rank.rank];
            uiBtn.tips.width = 250;
            uiBtn.tips.delay = 0.1f;
            uiBtn.tips.offset = new Vector2(-190, 60);
            RefreshTextAndMark();

            if (uiBtn.tipShowing)
            {
                uiBtn.OnPointerExit(null);
                uiBtn.OnPointerEnter(null);
                uiBtn.enterTime = 1;
            }

            if (Rank.rank > 10)
            {
                rankExpBarObj.SetActive(true);
                rankExpTextObj.SetActive(true);
            }
            else
            {
                rankExpBarObj.SetActive(true);
                rankExpTextObj.SetActive(true);
                if (Rank.rank > 6)
                {
                    expBarImg.color = rank79Color;
                    expText.color = rank79Color;
                }
                else if (Rank.rank > 3)
                {
                    expBarImg.color = rank46Color;
                    expText.color = rank46Color;
                }
                else
                {
                    expBarImg.color = rank03Color;
                    expText.color = rank03Color;
                }
            }
        }

        public static string GetRankInfoText()
        {
            int rank = Rank.rank;
            string res = "";

            if (SkillPoints.UnusedPoints() > 0 && !SkillPoints.isFullLevel)
            {
                if (UISkillPointsWindow.tempLevelAddedL.Sum() + UISkillPointsWindow.tempLevelAddedR.Sum() > 0)
                    res = "剩余技能点待确认".Translate() + "\n";
                else
                    res = string.Format("剩余技能点".Translate(), SkillPoints.UnusedPoints()) + "\n";
            }

            res += "<color=#61d8ffb4>";
            if (Relic.resurrectCoinCount > 0)
                res += "-  " + "剩余复活币".Translate() + $"  {Relic.resurrectCoinCount}\n";
            if (rank >= 1)
                res += "-  " + "gmRankReward1".Translate() + "\n";
            if (rank >= 2)
                res += "-  " + "gmRankReward2".Translate() + "\n";
            if (rank >= 3)
                res += "-  " + "gmRankReward3".Translate() + "\n";
            if (rank >= 4)
            {
                if (rank >= 10)
                    res += "-  " + "gmRankReward10".Translate() + "\n";
                else if (rank >= 7)
                    res += "-  " + "gmRankReward7".Translate() + "\n";
                else
                    res += "-  " + "gmRankReward4".Translate() + "\n";
            }
            if (rank >= 5)
                res += "-  " + "gmRankReward5".Translate() + "\n";
            if (rank >= 6)
                res += "-  " + "gmRankReward6".Translate() + "\n";
            if (rank >= 8)
                res += "-  " + "gmRankReward8".Translate() + "\n";
            if (rank >= 9)
                res += "-  " + "gmRankReward9".Translate();
            if (Relic.trueDamageActive > 0)
                res += "\n- " + "真实伤害已启用".Translate();
            res += "</color>";

            int nextRank = rank + 1;
            if (rank > 0 && rank < 10)
                res += "\n";
            if (rank < 10)
            {
                res += "下一功勋等级解锁".Translate() + "\n<color=#61d8ffb4>-  ";
                res += $"gmRankUnlockText{nextRank}".Translate();
                res += "</color>";
            }

            return res;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void UIRankGameTick(ref GameData __instance, long time)
        {
            if (Rank.rank >= 0 && Rank.rank <= 10)
            {
                float expProp = Rank.exp * 1.0f / Configs.expToNextRank[Rank.rank];
                expBarImg.fillAmount = expProp < 1 ? expProp : 1;
                if (uiBtn.tip != null && uiBtn.tipShowing)
                {
                    expText.text = Rank.exp.ToString() + " / " + Configs.expToNextRank[Rank.rank].ToString();
                }
                else
                {
                    expText.text = "";
                }
            }

            if (promotionNoticeMainText == null)
                promotionNoticeMainText = GameObject.Find("UI Root/Overlay Canvas/In Game/Top Tips/research-complete/main-text").GetComponent<Text>();
            if (promotionNoticeSubText == null)
                promotionNoticeSubText = GameObject.Find("UI Root/Overlay Canvas/In Game/Top Tips/research-complete/sub-text").GetComponent<Text>();

            if (time % 120 == 22)
            {
                int fullLevel = 0;
                for (int i = 0; i < SkillPoints.skillCountL; i++)
                {
                    fullLevel += SkillPoints.skillMaxLevelL[i];
                    fullLevel -= SkillPoints.skillLevelL[i];
                }
                for (int i = 0; i < SkillPoints.skillCountR; i++)
                {
                    fullLevel += SkillPoints.skillMaxLevelR[i];
                    fullLevel -= SkillPoints.skillLevelR[i];
                }
                if (fullLevel <= 0)
                    SkillPoints.isFullLevel = true;
                else
                    SkillPoints.isFullLevel = false;
            }

            RefreshTextAndMark();
        }

        public static void RefreshTextAndMark()
        {
            //刷新
            uiBtn.tips.tipTitle = ("gmRank" + Rank.rank.ToString()).Translate();
            uiBtn.tips.tipText = GetRankInfoText();

            // 有未使用的授权点时，且还能使用时，闪烁感叹号
            if (SkillPoints.UnusedPoints() > 0 && !SkillPoints.isFullLevel)
            {
                attentionMarkObj.SetActive(true);
                //if (Rank.rank <= 8)
                //{
                //    // attentionMarkObj.transform.localPosition = new Vector3(49.5f, -75, 0); // -75
                //}
                //else
                //{
                //    // attentionMarkObj.transform.localPosition = new Vector3(49.5f, -78, 0); // -78
                //}
                int t = (int)(GameMain.instance.timei % 120);
                float alpha = 0.65f + 0.2f * t / 60;
                if (t > 60)
                {
                    alpha = 0.85f - 0.2f * (t - 60) / 60;
                }
                attentionMarkText.color = new Color(1f, 0.62f, 0.14f, alpha);
                attentionMarkText.text = SkillPoints.UnusedPoints().ToString(); 
            }
            else
            {
                attentionMarkObj.SetActive(false);
                //ESButtonCircle.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        public static void UIPromotionNotify()
        {
            UIGeneralTips gTips = UIRoot.instance.uiGame.generalTips;
            promotionNoticeMainText.text = ("gmRankNoColor" + Rank.rank.ToString()).Translate();
            //promotionNoticeSubText.text = "功勋阶级".Translate();
            gTips.researchCompleteTip.gameObject.SetActive(true);
            gTips.researchCompleteTip.Stop();
            gTips.researchCompleteTip.Play();
            promotionNoticeSubText.text = "功勋阶级".Translate();
            VFAudio.Create("research-complete", null, Vector3.zero, true, 0, -1, -1L);

            UIDialogPatch.ShowUIDialog(("gmRankNoColor" + Rank.rank.ToString()).Translate(), ("gmRankUnlockText" + Rank.rank.ToString()).Translate(), 1, rankIcon.sprite);
        }
        public static void UIPointsGainNotify()
        {
            VFAudio.Create("research-complete", null, Vector3.zero, true, 0, -1, -1L);
        }
    }
}
