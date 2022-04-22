using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.IO;

namespace DSP_Battle
{
    public class UIRank
    {
        public static GameObject rankObj = null;
        public static GameObject rankTextObj;
        public static GameObject rankIconObj;
        public static Image rankIcon;
        public static Text rankText;
        static string color1Left = "<color=#c2853d>";
        static string colorRight = "</color>";
        public static Text promotionNoticeMainText = null;
        public static Text promotionNoticeSubText = null;
        public static UIButton uiBtn = null;
        public static Text tipTxtTitle = null;
        public static Text tipTxtContent = null;

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
            rankObj.transform.localPosition = new Vector3(DSPGame.globalOption.resolution.width / 2 - 80, DSPGame.globalOption.resolution.height / 2 - 120);
            rankObj.transform.SetAsFirstSibling();
            rankObj.transform.localScale = new Vector3(1, 1, 1);

            GameObject oriIconWithTips = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/content/icon");
            rankIconObj = GameObject.Instantiate(oriIconWithTips);
            rankIconObj.name = "icon";
            rankIconObj.transform.SetParent(rankObj.transform);
            rankIconObj.transform.localPosition = new Vector3(0, 40, 0);
            rankIconObj.transform.localScale = new Vector3(1, 1, 1);
            rankIcon = rankIconObj.GetComponent<Image>();
            rankIcon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/rank0");
            uiBtn = rankIconObj.GetComponent<UIButton>();

            rankTextObj = GameObject.Instantiate(oriTextObj);
            rankTextObj.name = "text";
            rankTextObj.transform.SetParent(rankObj.transform);
            rankTextObj.transform.localPosition = new Vector3(0, -40, 0);
            rankTextObj.AddComponent<Text>();
            rankTextObj.transform.localScale = new Vector3(1, 1, 1);
            rankText = rankTextObj.GetComponent<Text>();
            rankText.lineSpacing = 0.75f;
            rankText.fontSize = 14;
            rankText.color = new Color(1, 1, 1, 0.5f);
            rankText.alignment = TextAnchor.UpperCenter;
            rankText.supportRichText = true;
            rankText.text = ("gmRank" + Rank.rank.ToString()).Translate();// + "\n" + exp.ToString() + "/" + Configs.expToNextRank[rank];
            if (Rank.rank >= 0 && Rank.rank <= 10) ForceRefreshAll();
        }

        public static void ForceRefreshAll()
        {
            if (rankObj == null) return;
            rankIconObj.SetActive(true);
            rankText.text = ("gmRank" + Rank.rank.ToString()).Translate();
            rankIcon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/rank" + Rank.rank.ToString());
            if (uiBtn?.tip!=null)
            {
                tipTxtContent = uiBtn.tip.GetComponent<UIButtonTip>().subTextComp;
                tipTxtTitle =uiBtn.tip.GetComponent<UIButtonTip>().titleComp;
                tipTxtTitle.supportRichText = true;
            }

            uiBtn.tips.delay = 0.1f;
            uiBtn.tips.offset = new Vector2(-150, 40);
            if (uiBtn.tipShowing)
            {
                uiBtn.OnPointerExit(null);
                uiBtn.OnPointerEnter(null);
                uiBtn.enterTime = 1;
            }
        }

        public static string GetRankInfoText()
        {
            int rank = Rank.rank;
            string res = "";
            if (rank > 0)
            {
                res = "<color=#61d8ffb4>";
                if (rank >= 1)
                    res += "-  " + "gmRankReward1".Translate() + "\n";
                if (rank >= 7)
                    res += "-  " + "gmRankReward7".Translate() + "\n";
                else if (rank >= 3)
                    res += "-  " + "gmRankReward3".Translate() + "\n";
                if (rank >= 5)
                    res += "-  " + "gmRankReward5".Translate() + "\n";
                if (rank >= 2)
                    res += "-  " + "gmRankReward2".Translate() + (rank / 2 * 2).ToString() + "0%\n";
                if (rank >= 9)
                    res += "-  " + "gmRankReward9".Translate() + "\n";
                if (rank == 10)
                    res += "-  " + "gmRankReward10".Translate();
                res += "</color>";
            }
            int nextRank = rank + 1;
            if (rank > 0 && rank < 10) 
                res += "\n";
            if (rank<10)
            {
                res += "下一功勋等级解锁".Translate() + "\n<color=#61d8ffb4>-  ";
                if (nextRank % 2 == 0)
                    res += "gmRankReward2".Translate() + "20%";
                else
                    res += ("gmRankReward" + nextRank.ToString()).Translate();
                if (nextRank == 10)
                    res += "\n-  " + "gmRankReward10".Translate();
                res += "</color>";
            }

            return res;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void UIRankGameTick(ref GameData __instance, long time)
        {
            if (promotionNoticeMainText == null)
                promotionNoticeMainText = GameObject.Find("UI Root/Overlay Canvas/In Game/Top Tips/research-complete/main-text").GetComponent<Text>();
            if (promotionNoticeSubText == null)
                promotionNoticeSubText = GameObject.Find("UI Root/Overlay Canvas/In Game/Top Tips/research-complete/sub-text").GetComponent<Text>();

            uiBtn.tips.tipTitle = ("gmRank" + Rank.rank.ToString()).Translate();
            if (Rank.rank < 10 && Rank.rank >= 0)
            {
                uiBtn.tips.tipTitle += "  [" + "功勋点数".Translate() + " " + Rank.exp.ToString() + " / " + Configs.expToNextRank[Rank.rank].ToString() + "]";
            }
            uiBtn.tips.tipText = GetRankInfoText();
            if (uiBtn.tipShowing && tipTxtTitle != null && tipTxtContent != null)
            {
                tipTxtTitle.text = uiBtn.tips.tipTitle;
                tipTxtContent.text = uiBtn.tips.tipText;
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
            VFAudio.Create("mission-accomplished", null, Vector3.zero, true, 0, -1, -1L);

            UIDialogPatch.ShowUIDialog(("gmRankNoColor" + Rank.rank.ToString()).Translate(), ("gmRankUnlockText" + Rank.rank.ToString()).Translate(), 1, rankIcon.sprite);
        }
        
    }
}
