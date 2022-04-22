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

            rankIconObj = new GameObject();
            rankIconObj.name = "icon";
            rankIconObj.transform.SetParent(rankObj.transform);
            rankIconObj.transform.localPosition = new Vector3(0, 0, 0);
            rankIconObj.transform.localScale = new Vector3(1, 1, 1);
            rankIconObj.AddComponent<Image>();
            rankIconObj.AddComponent<RectTransform>();
            rankIconObj.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            rankIcon = rankIconObj.GetComponent<Image>();
            rankIcon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/rank0");


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
            rankText.text = ("gmRank" + Rank.rank.ToString()).Translate();
            rankIcon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/rank" + Rank.rank.ToString());
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void UIObjGet(ref GameData __instance, long time)
        {
            if (promotionNoticeMainText == null)
                promotionNoticeMainText = GameObject.Find("UI Root/Overlay Canvas/In Game/Top Tips/research-complete/main-text").GetComponent<Text>();
            if (promotionNoticeSubText == null)
                promotionNoticeSubText = GameObject.Find("UI Root/Overlay Canvas/In Game/Top Tips/research-complete/sub-text").GetComponent<Text>();
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
