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
    public class UIStarFortress
    {
        public static GameObject StarFortressUIObj = null; // UI总面板
        public static GameObject StarFortressContentObj = null; // 可被隐藏或显示的UI

        public static Text capacityText;
        public static List<Text> moduleCountText;
        public static List<Text> componentCountText;

        public static Text showHideBtnText;
        public static List<Text> tipBtnText;
        public static List<Text> setBtnText;

        public static DysonSphere curDysonSphere = null;

        public static void InitAll()
        {
            if (StarFortressUIObj == null)
            {
                moduleCountText = new List<Text>();
                componentCountText = new List<Text>();
                tipBtnText = new List<Text>();
                setBtnText = new List<Text>();

                // 根节点UI的GameObject
                Transform parentTrans = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy").transform;
                StarFortressUIObj = new GameObject("StarFortress");
                StarFortressUIObj.transform.SetParent(parentTrans);
                StarFortressUIObj.transform.localScale = new Vector3(1, 1, 1);
                StarFortressUIObj.transform.localPosition = new Vector3(350, -30, 0);
                StarFortressUIObj.SetActive(true);
                Transform masterUITrans = StarFortressUIObj.transform;

                // 恒星要塞标题文本
                GameObject TitleTextObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Assembler Window/produce/circle-back/cnt-text"), masterUITrans);
                TitleTextObj.name = "title";
                TitleTextObj.transform.localPosition = new Vector3(0, -20);
                Text titleText = TitleTextObj.GetComponent<Text>();
                titleText.fontSize = 18;
                titleText.alignment = TextAnchor.MiddleLeft;
                titleText.text = "恒星要塞".Translate();

                // 显示、隐藏下面的主要UI部分的按钮
                GameObject addNewLayerButton = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy/layers/buttons-group/buttons/add-button");
                GameObject showHideButtonObj = GameObject.Instantiate(addNewLayerButton, masterUITrans);
                showHideButtonObj.SetActive(true);
                showHideButtonObj.name = "show-hide"; //名字
                showHideButtonObj.transform.localPosition = new Vector3(80, 2, 0); //位置
                showHideButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 24); //按钮大小
                showHideBtnText = showHideButtonObj.transform.Find("Text").gameObject.GetComponent<Text>();
                showHideBtnText.text = "显示/隐藏".Translate();
                Button showHideButton = showHideButtonObj.GetComponent<Button>();
                showHideButton.interactable = true;
                showHideButton.onClick.RemoveAllListeners();
                showHideButton.onClick.AddListener(()=> { ShowHideUI(); });


                // 主体设置部分的GameObject
                StarFortressContentObj = new GameObject("content");
                StarFortressContentObj.transform.SetParent(masterUITrans);
                StarFortressContentObj.transform.localScale = new Vector3(1, 1, 1);
                StarFortressContentObj.transform.localPosition = new Vector3(30, -50, 0);
                StarFortressContentObj.SetActive(true);
                Transform subTrans = StarFortressContentObj.transform;

                // 最大容量文本显示
                GameObject capacityTitleTextObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Assembler Window/produce/circle-back/cnt-text"), subTrans);
                capacityTitleTextObj.name = "capacity-title";
                capacityTitleTextObj.transform.localPosition = new Vector3(0, 0);
                Text capacityTitleText = capacityTitleTextObj.GetComponent<Text>();
                capacityTitleText.fontSize = 16;
                capacityTitleText.alignment = TextAnchor.MiddleLeft;
                capacityTitleText.text = "模块容量".Translate();
                GameObject capacityValueTextObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Assembler Window/produce/circle-back/cnt-text"), subTrans);
                capacityValueTextObj.name = "capacity-value";
                capacityValueTextObj.transform.localPosition = new Vector3(130, 0);
                capacityText = capacityValueTextObj.GetComponent<Text>();
                capacityText.fontSize = 16;
                capacityText.alignment = TextAnchor.MiddleCenter;
                capacityText.text = "0/0";
                CreateTipButton(subTrans, -60, 20, "模块容量", "模块容量说明");

                // 分隔线Obj
                GameObject oriSepLineObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy/swarm/sep-1");

                // 主体要塞设置部分
                // 导弹模块
                GameObject missileModuleObj = new GameObject("missile-module");
                missileModuleObj.transform.SetParent(subTrans);
                missileModuleObj.transform.localScale = new Vector3(1, 1, 1);
                missileModuleObj.transform.localPosition = new Vector3(0, -50, 0);
                missileModuleObj.SetActive(true);
                GameObject sepLineObj = GameObject.Instantiate(oriSepLineObj, missileModuleObj.transform);
                sepLineObj.name = "sep";
                sepLineObj.transform.localScale = new Vector3(1, 1, 1);
                sepLineObj.transform.localPosition = new Vector3(115, 38, 0);
                sepLineObj.GetComponent<RectTransform>().sizeDelta = new Vector2(365, 7);
                CreateText(missileModuleObj.transform, "title", "导弹模块", 0, -0, 16);
                CreateText(missileModuleObj.transform, "compo", "组件需求", 0, -25, 12);
                moduleCountText.Add(CreateText(missileModuleObj.transform,"moduleCount", "0/0", 190, 0, 16, TextAnchor.MiddleCenter));
                componentCountText.Add(CreateText(missileModuleObj.transform, "componentCount", "0/0", 190, -25, 12, TextAnchor.MiddleCenter));
                CreateTipButton(missileModuleObj.transform, -60, 20, "导弹模块", "导弹模块说明");
                CreateSetButtons(missileModuleObj.transform, 0);


                // 光矛模块
                GameObject cannonModuleObj = new GameObject("cannon-module");
                cannonModuleObj.transform.SetParent(subTrans);
                cannonModuleObj.transform.localScale = new Vector3(1, 1, 1);
                cannonModuleObj.transform.localPosition = new Vector3(0, -120, 0);
                cannonModuleObj.SetActive(true);
                GameObject sepLine2Obj = GameObject.Instantiate(oriSepLineObj, cannonModuleObj.transform);
                sepLine2Obj.name = "sep";
                sepLine2Obj.transform.localScale = new Vector3(1, 1, 1);
                sepLine2Obj.transform.localPosition = new Vector3(115, 38, 0);
                sepLine2Obj.GetComponent<RectTransform>().sizeDelta = new Vector2(365, 7);
                CreateText(cannonModuleObj.transform, "title", "光矛模块", 0, -0, 16);
                CreateText(cannonModuleObj.transform, "compo", "组件需求", 0, -25, 12);
                moduleCountText.Add(CreateText(cannonModuleObj.transform, "moduleCount", "0/0", 190, 0, 16, TextAnchor.MiddleCenter));
                componentCountText.Add(CreateText(cannonModuleObj.transform, "componentCount", "0/0", 190, -25, 12, TextAnchor.MiddleCenter));
                CreateTipButton(cannonModuleObj.transform, -60, 20, "光矛模块", "光矛模块说明");
                CreateSetButtons(cannonModuleObj.transform, 1);
            }
        }

        public static void CreateTipButton(Transform parent, float x, float y, string tipTitle, string tipText)
        {
            GameObject addNewLayerButton = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy/layers/buttons-group/buttons/add-button");
            GameObject tipButtonObj = GameObject.Instantiate(addNewLayerButton, parent);
            tipButtonObj.SetActive(true);
            tipButtonObj.name = "tip"; //名字
            tipButtonObj.transform.localPosition = new Vector3(x, y, 0); //位置
            tipButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20); //按钮大小
            tipBtnText.Add(tipButtonObj.transform.Find("Text").gameObject.GetComponent<Text>());
            tipButtonObj.transform.Find("Text").gameObject.GetComponent<Text>().text = "?";
            Button tipButton = tipButtonObj.GetComponent<Button>();
            tipButton.interactable = true;
            tipButton.onClick.RemoveAllListeners();

            tipButtonObj.GetComponent<UIButton>().tips.tipTitle = tipTitle.Translate();
            tipButtonObj.GetComponent<UIButton>().tips.tipText = tipText.Translate();
            tipButtonObj.GetComponent<UIButton>().tips.delay = 0.3f;
            tipButtonObj.GetComponent<UIButton>().tips.width = 280;
            tipButtonObj.GetComponent<UIButton>().tips.offset = new Vector2(140, -50);
        }

        public static Text CreateText(Transform parent, string objname, string defaultText, float x, float y,int fontSize, TextAnchor textAnchor = TextAnchor.MiddleLeft)
        {
            GameObject textObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Assembler Window/produce/circle-back/cnt-text"), parent);
            textObj.name = objname;
            textObj.transform.localPosition = new Vector3(x, y);
            Text txt = textObj.GetComponent<Text>();
            txt.alignment = textAnchor;
            txt.fontSize = fontSize;
            txt.text = defaultText.Translate();
            return txt;
        }

        public static void CreateSetButtons(Transform parent, int index)
        {
            GameObject addNewLayerButton = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy/layers/buttons-group/buttons/add-button");
            List<float> xPosMap = new List<float> { 63, 98, 225, 254 };
            List<int> widthMap = new List<int> { 32, 26, 26, 32 };

            for (int i = 0; i < 4; i++)
            {
                GameObject setButtonObj = GameObject.Instantiate(addNewLayerButton, parent);
                setButtonObj.SetActive(true);
                setButtonObj.name = "button" + i.ToString(); //名字
                setButtonObj.transform.localPosition = new Vector3(xPosMap[i], 20, 0); //位置
                setButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(widthMap[i], 20); //按钮大小
                setBtnText.Add(setButtonObj.transform.Find("Text").gameObject.GetComponent<Text>());
                Button tipButton = setButtonObj.GetComponent<Button>();
                tipButton.interactable = true;
                tipButton.onClick.RemoveAllListeners();
                switch (i)
                {
                    case 0:
                        tipButton.onClick.AddListener(() => { SetModuleNum(index, -100); });
                        break;
                    case 1:
                        tipButton.onClick.AddListener(() => { SetModuleNum(index, -1); });
                        break;
                    case 2:
                        tipButton.onClick.AddListener(() => { SetModuleNum(index, 1); });
                        break;
                    case 3:
                        tipButton.onClick.AddListener(() => { SetModuleNum(index, 100); });
                        break;
                    default:
                        break;
                }
            }
        }

        public static void RefreshAll()
        {
            if (StarFortressUIObj == null) return;
            showHideBtnText.text = "显示/隐藏".Translate();
            for (int i = 0; i < tipBtnText.Count; i++)
            {
                if (tipBtnText[i] != null)
                {
                    tipBtnText[i].text = "?";
                }
            }

            if (StarFortressContentObj.activeSelf) // 避免顶部UI互相遮挡
            {
                UIAlert.ShowAlert(false);
            }
            RefreshSetBtnText();
        }

        public static void RefreshSetBtnText()
        {
            if (StarFortressUIObj == null) return;
            for (int i = 0; i+3 < setBtnText.Count; i=i+4)
            {
                if (DspBattlePlugin.isControlDown)
                {
                    setBtnText[i].text = "-1k";
                    setBtnText[i+1].text = "-10";
                    setBtnText[i+2].text = "+10";
                    setBtnText[i+3].text = "+1k";
                }
                else
                {
                    setBtnText[i].text = "-100";
                    setBtnText[i+1].text = "-1";
                    setBtnText[i+2].text = "+1";
                    setBtnText[i+3].text = "+100";
                }
            }
        }

        public static void ShowHideUI()
        {
            StarFortressContentObj.SetActive(!StarFortressContentObj.activeSelf);
            RefreshAll();
        }

        public static void SetModuleNum(int index, int delta)
        {
            Utils.Log($"index is {index} and delta is {delta}");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDysonEditor), "_OnOpen")]
        public static void SetTextOnOpen(UIDysonEditor __instance)
        {
            if (__instance.selection.viewDysonSphere != null)
            {
                curDysonSphere = __instance.selection.viewDysonSphere;
            }
            RefreshAll();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDysonEditor), "OnViewStarChange")]
        public static void SetTextOnViewStarChange(UIDysonEditor __instance)
        {
            if (__instance.selection.viewDysonSphere != null)
            {
                curDysonSphere = __instance.selection.viewDysonSphere;
            }
            RefreshAll();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDysonEditor), "OnSelectionChange")]
        public static void SetTextOnSelectionChange(UIDysonEditor __instance)
        {
            if (__instance.selection.viewDysonSphere != null)
            {
                curDysonSphere = __instance.selection.viewDysonSphere;
            }
            RefreshAll();
        }
    }
}
