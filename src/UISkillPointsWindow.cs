using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DSP_Battle
{
    public class UISkillPointsWindow
    {
        public static GameObject spWindowObj = null;
        public static GameObject tipButtonObj;
        public static Text spWindowTitleText;
        public static Text closeButtonText;
        public static Text spText;
        public static Text spDescText;
        public static Text resetCostText;
        public static Text withdrawButtonText;
        public static Image resetButtonImg;
        public static Image confirmButtonImg;
        public static List<GameObject> skillTipButtonObjsL = new List<GameObject>();
        public static List<GameObject> skillTipButtonObjsR = new List<GameObject>();
        public static List<UIButton> skillTipUIButtonsL = new List<UIButton>();
        public static List<UIButton> skillTipUIButtonsR = new List<UIButton>();
        public static List<UIButton> plusUIButtonsL = new List<UIButton>();
        public static List<UIButton> minusUIButtonsL = new List<UIButton>();
        public static List<UIButton> plusUIButtonsR = new List<UIButton>();
        public static List<UIButton> minusUIButtonsR = new List<UIButton>();
        public static List<Text> valueTextsL = new List<Text>();
        public static List<Text> valueTextsR = new List<Text>();

        public static int[] tempLevelAddedL = new int[30];
        public static int[] tempLevelAddedR = new int[30];


        public static Color ButtonEnabledColorNorm = new Color(0.0f, 0.37f, 0.52f, 0.6f);
        public static Color ButtonEnabledColorHigh = new Color(0.0f, 0.37f, 0.52f, 0.8f);
        public static Color ButtonEnabledColorPressed = new Color(0.0f, 0.37f, 0.52f, 0.45f);
        public static Color ButtonDisabledColorNorm = new Color(0.37f, 0.37f, 0.37f, 0.6f);
        public static Color ButtonDisabledColorHigh = new Color(0.37f, 0.37f, 0.37f, 0.7f);
        public static Color ButtonDisabledColorPressed = new Color(0.37f, 0.37f, 0.37f, 0.6f);
        static Color btnDisableColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        static Color btnEnableColor = new Color(0.2f, 0.459f, 0.824f, 1f);

        public static void InitAll()
        {
            if (spWindowObj == null)
            {
                // 事件消息和选择窗口
                spWindowObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/"));
                spWindowObj.name = "SkillPointsWindow";
                spWindowObj.transform.SetParent(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/").transform);
                spWindowObj.transform.SetAsFirstSibling();
                spWindowObj.transform.localScale = new Vector3(1, 1, 1);
                spWindowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 800);
                spWindowObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                spWindowObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                Component.Destroy(spWindowObj.GetComponent<UIResearchResultWindow>());
                spWindowObj.SetActive(false);

                GameObject spWindowContentObj = spWindowObj.transform.Find("content").gameObject;
                spWindowContentObj.GetComponent<CanvasGroup>().alpha = 1;
                //eventWindowContentObj.GetComponent<CanvasGroup>().blocksRaycasts = true;

                Component.Destroy(spWindowContentObj.transform.Find("title-text").GetComponent<Localizer>());
                spWindowTitleText = spWindowContentObj.transform.Find("title-text").GetComponent<Text>();
                spWindowTitleText.text = "技能点标题".Translate();
                GameObject.Destroy(spWindowContentObj.transform.Find("close-button").gameObject);

                // 剩余技能点 以及技能点描述
                GameObject oriSubTitleTextObj = spWindowContentObj.transform.Find("function-text").gameObject;
                oriSubTitleTextObj.name = "skillPoints";
                oriSubTitleTextObj.transform.localPosition = new Vector3(-180, 365, 0);
                spText = oriSubTitleTextObj.GetComponent<Text>();
                spText.fontSize = 24;
                spText.alignment = TextAnchor.MiddleLeft;
                spText.material = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Assembler Window/produce/circle-back/cnt-text").GetComponent<Text>().material;
                spText.color = new Color(0.14f, 0.63f, 1f, 0.84f);
                GameObject oriDescTextObj = spWindowContentObj.transform.Find("conclusion").gameObject;
                //oriDescTextObj.SetActive(false);
                oriDescTextObj.name = "spDesc";
                oriDescTextObj.transform.localPosition = new Vector3(20, 330, 0);
                spDescText = oriDescTextObj.GetComponent<Text>();
                spDescText.fontSize = 14;
                spDescText.alignment = TextAnchor.UpperLeft;
                spDescText.text = "";
                oriDescTextObj.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 24);

                // 问号说明
                GameObject oriTipButtonObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Mecha Window/information/movement-panel/title/tip-button");
                tipButtonObj = GameObject.Instantiate(oriTipButtonObj, spWindowObj.transform);
                tipButtonObj.name = "tip-button";
                tipButtonObj.transform.localPosition = new Vector3(-350, 340, 0);
                tipButtonObj.GetComponent<UIButton>().tips.tipTitle = "技能点".Translate();
                tipButtonObj.GetComponent<UIButton>().tips.tipText = "技能点描述".Translate();
                tipButtonObj.GetComponent<UIButton>().tips.delay = 0.2f;
                tipButtonObj.GetComponent<UIButton>().tips.width = 360;
                tipButtonObj.GetComponent<UIButton>().tips.offset = new Vector2(430, 22);
                tipButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
                tipButtonObj.transform.Find("icon").GetComponent<RectTransform>().sizeDelta = new Vector2(14, 14);
                tipButtonObj.SetActive(true);

                // 关闭窗口按钮
                GameObject oriButtonObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy/layers/buttons-group/buttons/add-button");
                GameObject closeButtonObj = GameObject.Instantiate(oriButtonObj);
                closeButtonObj.name = "close";
                closeButtonObj.transform.SetParent(spWindowContentObj.transform);
                closeButtonObj.transform.localPosition = new Vector3(380, 400, 0);
                closeButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
                closeButtonText = closeButtonObj.transform.Find("Text").gameObject.GetComponent<Text>();
                closeButtonText.text = "-";
                GameObject.DestroyImmediate(closeButtonObj.GetComponent<Button>());
                Button closeBtn = closeButtonObj.AddComponent<Button>();
                closeBtn.interactable = true;
                closeButtonObj.GetComponent<UIButton>().button = closeBtn;
                Action<int> closeAction = (x) => { Hide(); };
                closeButtonObj.GetComponent<UIButton>().onClick += closeAction;

                // 重置按钮和文本
                GameObject resetCostTextObj = GameObject.Instantiate(oriDescTextObj, spWindowObj.transform);
                resetCostTextObj.name = "text-cost";
                resetCostTextObj.transform.localScale = new Vector3(1, 1, 1);
                resetCostTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                resetCostTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                resetCostTextObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
                resetCostTextObj.transform.localPosition = new Vector3(310, 116, 0);
                resetCostText = resetCostTextObj.GetComponent<Text>();
                resetCostText.alignment = TextAnchor.UpperLeft;
                resetCostText.supportRichText = true;
                resetCostText.text = "";


                GameObject oriButtonWOTip = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Station Window/storage-box-0/popup-box/sd-option-button-1");
                if (oriButtonWOTip == null)
                    oriButtonWOTip = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Station Window/Station-scroll(Clone)/Viewport/pane/storage-box-0(Clone)/popup-box/sd-option-button-1");
                GameObject resetButtonObj = GameObject.Instantiate(oriButtonWOTip, spWindowObj.transform);
                resetButtonObj.name = "btn-resetall";
                resetButtonObj.transform.localPosition = new Vector3(325, 350, 0);
                resetButtonObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                resetButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 30);
                resetButtonObj.transform.Find("button-text").GetComponent<Text>().text = "全部重置".Translate();
                resetButtonObj.GetComponent<Button>().onClick.RemoveAllListeners();
                resetButtonObj.GetComponent<Button>().onClick.AddListener(() => { OnResetAllClick(); });
                resetButtonImg = resetButtonObj.GetComponent<Image>();
                resetButtonImg.color = btnEnableColor;

                GameObject matrixIcon = new GameObject("icon-matrix");
                matrixIcon.transform.SetParent(spWindowObj.transform);
                matrixIcon.transform.localScale = Vector3.one;
                matrixIcon.transform.localPosition = new Vector3(290, 318, 0);
                matrixIcon.AddComponent<RectTransform>();
                matrixIcon.AddComponent<Image>();
                matrixIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                matrixIcon.GetComponent<Image>().sprite = LDB.items.Select(6006).iconSprite;
                matrixIcon.SetActive(true);

                // 撤销按钮
                GameObject withdrawButtonObj = GameObject.Instantiate(oriButtonWOTip, spWindowObj.transform);
                withdrawButtonObj.name = "btn-withdraw";
                withdrawButtonObj.transform.localPosition = new Vector3(200, -348, 0);
                withdrawButtonObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                withdrawButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
                withdrawButtonText = withdrawButtonObj.transform.Find("button-text").GetComponent<Text>();
                withdrawButtonText.text = "";
                withdrawButtonObj.transform.Find("button-text").GetComponent<Text>().resizeTextMaxSize = 20;
                withdrawButtonObj.GetComponent<Button>().onClick.RemoveAllListeners();
                withdrawButtonObj.GetComponent<Button>().onClick.AddListener(() => { OnWithdrawAllClick(); });
                withdrawButtonObj.GetComponent<Image>().color = btnEnableColor;

                // 确认按钮
                GameObject confirmButtonObj = GameObject.Instantiate(oriButtonWOTip, spWindowObj.transform);
                confirmButtonObj.name = "btn-confirm";
                confirmButtonObj.transform.localPosition = new Vector3(-200, -348, 0);
                confirmButtonObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                confirmButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
                confirmButtonObj.transform.Find("button-text").GetComponent<Text>().text = "确认分配".Translate();
                confirmButtonObj.transform.Find("button-text").GetComponent<Text>().resizeTextMaxSize = 20;
                confirmButtonObj.GetComponent<Button>().onClick.RemoveAllListeners();
                confirmButtonObj.GetComponent<Button>().onClick.AddListener(() => { OnConfirmAllClick(); });
                confirmButtonImg = confirmButtonObj.GetComponent<Image>();
                confirmButtonImg.color = btnEnableColor;

                InitAssignUI();
            }
        }

        public static void InitAssignUI()
        {
            int leftCount = SkillPoints.skillCountL;
            int rightCount = SkillPoints.skillCountR;
            int lineSpacing = 50;

            GameObject contentObj = new GameObject("assign-content");
            contentObj.transform.SetParent(spWindowObj.transform);
            contentObj.transform.localPosition = new Vector3(0, 0, 0);
            contentObj.transform.localScale = new Vector3(1, 1, 1);
            contentObj.AddComponent<RectTransform>();
            contentObj.GetComponent<RectTransform>().sizeDelta = new Vector2(740, 600);
            contentObj.AddComponent<Image>();
            contentObj.GetComponent<Image>().color = new Color(0.6f, 0.7f, 0.8f, 0.15f);

            GameObject oriTitleTextObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/content/title-text");
            GameObject txtObj = GameObject.Instantiate(oriTitleTextObj, contentObj.transform);
            txtObj.name = "title";
            txtObj.GetComponent<Text>().fontSize = 14;
            Component.DestroyImmediate(txtObj.GetComponent<Localizer>());
            txtObj.SetActive(false);

            GameObject addNewLayerButtonObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy/layers/buttons-group/buttons/add-button");
            GameObject buttonObj = GameObject.Instantiate(addNewLayerButtonObj, contentObj.transform);
            buttonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            buttonObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            buttonObj.SetActive(false);

            for (int i = 0; i < leftCount + rightCount; i++)
            {
                bool isLeft = i < leftCount;
                float x = isLeft ? -210 : 160;
                float y = 260 - (isLeft ? i * lineSpacing : (i - leftCount) * lineSpacing);
                string sign = isLeft ? i.ToString() : (100 + (i - leftCount)).ToString();
                string name = $"skill{(isLeft ? "L" : "R")}{(isLeft ? i : i - leftCount)}";
                int idx = isLeft ? i : i - leftCount;

                GameObject skillObj = new GameObject(name);
                skillObj.transform.SetParent(contentObj.transform);
                skillObj.transform.localScale = new Vector3(1, 1, 1);
                skillObj.transform.localPosition = new Vector3(x, y, 0);
                skillObj.AddComponent<RectTransform>();
                skillObj.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 30);
                skillObj.AddComponent<Image>();
                skillObj.GetComponent<Image>().color = new Color(0, 0, 0, 0);

                // 说明按钮
                GameObject skillTipButtonObj = GameObject.Instantiate(tipButtonObj, skillObj.transform);
                skillTipButtonObj.name = name + "-tip";
                skillTipButtonObj.transform.localPosition = new Vector3(-130, 2, 0);
                skillTipButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(16, 16);
                skillTipButtonObj.transform.Find("icon").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(12, 12);
                skillTipButtonObj.transform.Find("icon").gameObject.name = name + "-tipicon";
                UIButton tipUIBtn = skillTipButtonObj.GetComponent<UIButton>();
                tipUIBtn.tips.tipTitle = (name).Translate();
                tipUIBtn.tips.tipText = (name + "Desc").Translate();
                tipUIBtn.tips.offset = new Vector2(-20, 12);
                tipUIBtn.tips.width = 200;

                // 题目文本以及数值文本
                GameObject titleObj = GameObject.Instantiate(txtObj, skillObj.transform);
                titleObj.transform.localPosition = new Vector3(-20, 20, 0);
                titleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(185, 36);
                titleObj.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                titleObj.GetComponent<Text>().text = name.Translate();
                titleObj.SetActive(true);

                GameObject valueObj = GameObject.Instantiate(txtObj, skillObj.transform);
                valueObj.name = "value";
                valueObj.transform.localPosition = new Vector3(112, 20, 0); // 稍微往右偏移2看起来更在中间
                Text valueText = valueObj.GetComponent<Text>();
                valueText.alignment = TextAnchor.MiddleCenter;
                valueObj.SetActive(true);

                // 分配按钮

                List<float> values = SkillPoints.skillValuesL;
                List<string> suffixes = SkillPoints.skillSuffixesL;
                if (!isLeft)
                {
                    values = SkillPoints.skillValuesR;
                    suffixes = SkillPoints.skillSuffixesR;
                }

                GameObject plusObj = GameObject.Instantiate(buttonObj, skillObj.transform);
                plusObj.name = "assign-" + sign;
                plusObj.transform.localPosition = new Vector3(150, 3, 0);
                plusObj.SetActive(true);
                UIButton plusUIBtn = plusObj.GetComponent<UIButton>();
                plusUIBtn.tips.tipTitle = (values[idx] > 0 ? "+" : "") + values[idx].ToString() + " " + suffixes[idx];
                plusUIBtn.tips.tipText = "按下Shift分配10点说明".Translate();
                plusUIBtn.tips.delay = 0.2f;
                plusUIBtn.tips.offset = new Vector2(150, -22);
                Component.DestroyImmediate(plusObj.GetComponent<Button>());
                Button plusBtn = plusObj.AddComponent<Button>();
                //plusBtn.onClick.RemoveAllListeners();
                plusBtn.onClick.AddListener(() => { OnAssignOneClick(Convert.ToInt32(sign)); });

                GameObject minusObj = GameObject.Instantiate(buttonObj, skillObj.transform);
                minusObj.name = "withdraw-" + sign;
                minusObj.transform.localPosition = new Vector3(70, 3, 0);
                minusObj.SetActive(true);
                UIButton minusUIBtn = minusObj.GetComponent<UIButton>();
                minusUIBtn.tips.tipTitle = "";
                minusUIBtn.tips.tipText = "";
                Component.DestroyImmediate(minusObj.GetComponent<Button>());
                Button minusBtn = minusObj.AddComponent<Button>();
                //minusBtn.onClick.RemoveAllListeners();
                minusBtn.onClick.AddListener(() => { OnWithdrawOneClick(Convert.ToInt32(sign)); });

                if (isLeft)
                {
                    plusUIButtonsL.Add(plusUIBtn);
                    minusUIButtonsL.Add(minusUIBtn);
                    valueTextsL.Add(valueText);
                    skillTipButtonObjsL.Add(skillTipButtonObj);
                    skillTipUIButtonsL.Add(tipUIBtn);
                }
                else
                {
                    plusUIButtonsR.Add(plusUIBtn);
                    minusUIButtonsR.Add(minusUIBtn);
                    valueTextsR.Add(valueText);
                    skillTipButtonObjsR.Add(skillTipButtonObj);
                    skillTipUIButtonsR.Add(tipUIBtn);
                }

            }
        }

        public static void Update()
        {
            if (spWindowObj == null)
                return;
            if (spWindowObj.activeSelf)
            {
                int unassignedPoins = SkillPoints.UnusedPoints() - tempLevelAddedL.Sum() - tempLevelAddedR.Sum();
                closeButtonText.text = "一";
                spText.text = "技能点".Translate() + "  " + unassignedPoins.ToString();
                spDescText.text = "已分配技能点".Translate() + "  " + (SkillPoints.skillLevelL.Sum() + SkillPoints.skillLevelR.Sum() + tempLevelAddedL.Sum() + tempLevelAddedR.Sum()).ToString();

                // 左侧
                for (int i = 0; i < valueTextsL.Count; i++)
                {
                    int level = SkillPoints.skillLevelL[i] + tempLevelAddedL[i];
                    float value = level * SkillPoints.skillValuesL[i];
                    if (i == 4)
                    {
                        if (Rank.rank >= 10)
                            value -= 80;
                        else if (Rank.rank >= 7)
                            value -= 40;
                        else if (Rank.rank >= 4)
                            value -= 20;
                    }
                    valueTextsL[i].text = (value > 0 ? "+" : "") + value.ToString() + " " + SkillPoints.skillSuffixesL[i];
                    int length = plusUIButtonsL[i].transitions.Length;
                    for (int j = 0; j < length && j < 1; j++)
                    {
                        float oldRed = plusUIButtonsL[i].transitions[j].normalColor.r;
                        if (unassignedPoins > 0 && tempLevelAddedL[i] + SkillPoints.skillLevelL[i] < SkillPoints.skillMaxLevelL[i]) // 有剩余点数
                        {
                            plusUIButtonsL[i].transitions[j].normalColor = ButtonEnabledColorNorm;
                            plusUIButtonsL[i].transitions[j].mouseoverColor = ButtonEnabledColorHigh;
                            plusUIButtonsL[i].transitions[j].pressedColor = ButtonEnabledColorPressed;
                        }
                        else
                        {
                            plusUIButtonsL[i].transitions[j].normalColor = ButtonDisabledColorNorm;
                            plusUIButtonsL[i].transitions[j].mouseoverColor = ButtonDisabledColorNorm;
                            plusUIButtonsL[i].transitions[j].pressedColor = ButtonDisabledColorNorm;
                        }
                        // 并立即更改按钮颜色
                        if (oldRed != plusUIButtonsL[i].transitions[j].mouseoverColor.r)
                        {
                            if (plusUIButtonsL[i].isPointerEnter)
                                plusUIButtonsL[i].gameObject.GetComponent<Image>().color = plusUIButtonsL[i].transitions[j].mouseoverColor;
                            else
                                plusUIButtonsL[i].gameObject.GetComponent<Image>().color = plusUIButtonsL[i].transitions[j].normalColor;
                        }

                        float oldRed1 = minusUIButtonsL[i].transitions[j].normalColor.r;
                        if (tempLevelAddedL[i] > 0) // 目前刚分配了点数且还未确定
                        {
                            minusUIButtonsL[i].transitions[j].normalColor = ButtonEnabledColorNorm;
                            minusUIButtonsL[i].transitions[j].mouseoverColor = ButtonEnabledColorHigh;
                            minusUIButtonsL[i].transitions[j].pressedColor = ButtonEnabledColorPressed;
                        }
                        else
                        {
                            minusUIButtonsL[i].transitions[j].normalColor = ButtonDisabledColorNorm;
                            minusUIButtonsL[i].transitions[j].mouseoverColor = ButtonDisabledColorNorm;
                            minusUIButtonsL[i].transitions[j].pressedColor = ButtonDisabledColorNorm;
                        }
                        if (oldRed1 != minusUIButtonsL[i].transitions[j].mouseoverColor.r)
                        {
                            if (minusUIButtonsL[i].isPointerEnter)
                                minusUIButtonsL[i].gameObject.GetComponent<Image>().color = minusUIButtonsL[i].transitions[j].mouseoverColor;
                            else
                                minusUIButtonsL[i].gameObject.GetComponent<Image>().color = minusUIButtonsL[i].transitions[j].normalColor;
                        }
                    }
                }
                // 右侧
                for (int i = 0; i < valueTextsR.Count; i++)
                {
                    int level = SkillPoints.skillLevelR[i] + tempLevelAddedR[i];
                    float value = level * SkillPoints.skillValuesR[i];
                    if (i == 1 && Relic.HaveRelic(2, 12))
                    {
                        value += 10;
                    }
                    valueTextsR[i].text = (value > 0 ? "+" : "") + value.ToString() + " " + SkillPoints.skillSuffixesR[i];
                    int length = plusUIButtonsR[i].transitions.Length;
                    for (int j = 0; j < length && j < 1; j++)
                    {
                        float oldRed = plusUIButtonsR[i].transitions[j].normalColor.r;
                        if (unassignedPoins > 0 && tempLevelAddedR[i] + SkillPoints.skillLevelR[i] < SkillPoints.skillMaxLevelR[i]) // 有剩余点数，且未达到最大等级
                        {
                            plusUIButtonsR[i].transitions[j].normalColor = ButtonEnabledColorNorm;
                            plusUIButtonsR[i].transitions[j].mouseoverColor = ButtonEnabledColorHigh;
                            plusUIButtonsR[i].transitions[j].pressedColor = ButtonEnabledColorPressed;
                        }
                        else
                        {
                            plusUIButtonsR[i].transitions[j].normalColor = ButtonDisabledColorNorm;
                            plusUIButtonsR[i].transitions[j].mouseoverColor = ButtonDisabledColorNorm;
                            plusUIButtonsR[i].transitions[j].pressedColor = ButtonDisabledColorNorm;
                        }
                        // 并立即更改按钮颜色
                        if (oldRed != plusUIButtonsR[i].transitions[j].mouseoverColor.r)
                        {
                            if (plusUIButtonsR[i].isPointerEnter)
                                plusUIButtonsR[i].gameObject.GetComponent<Image>().color = plusUIButtonsR[i].transitions[j].mouseoverColor;
                            else
                                plusUIButtonsR[i].gameObject.GetComponent<Image>().color = plusUIButtonsR[i].transitions[j].normalColor;
                        }

                        float oldRed1 = minusUIButtonsR[i].transitions[j].normalColor.r;
                        if (tempLevelAddedR[i] > 0) // 目前刚分配了点数且还未确定
                        {
                            minusUIButtonsR[i].transitions[j].normalColor = ButtonEnabledColorNorm;
                            minusUIButtonsR[i].transitions[j].mouseoverColor = ButtonEnabledColorHigh;
                            minusUIButtonsR[i].transitions[j].pressedColor = ButtonEnabledColorPressed;
                        }
                        else
                        {
                            minusUIButtonsR[i].transitions[j].normalColor = ButtonDisabledColorNorm;
                            minusUIButtonsR[i].transitions[j].mouseoverColor = ButtonDisabledColorHigh;
                            minusUIButtonsR[i].transitions[j].pressedColor = ButtonDisabledColorPressed;
                        }
                        if (oldRed1 != minusUIButtonsR[i].transitions[j].mouseoverColor.r)
                        {
                            if (minusUIButtonsR[i].isPointerEnter)
                                minusUIButtonsR[i].gameObject.GetComponent<Image>().color = minusUIButtonsR[i].transitions[j].mouseoverColor;
                            else
                                minusUIButtonsR[i].gameObject.GetComponent<Image>().color = minusUIButtonsR[i].transitions[j].normalColor;
                        }
                    }
                }

                // 按钮表现
                int assignedTemp = tempLevelAddedL.Sum() + tempLevelAddedR.Sum();
                if (assignedTemp > 0)
                {
                    withdrawButtonText.text = "撤销分配".Translate();
                    confirmButtonImg.color = btnEnableColor;
                }
                else
                {
                    withdrawButtonText.text = "技能点窗口取消".Translate();
                    confirmButtonImg.color = btnDisableColor;
                }

                ShowHideSkillTipButton();
            }

        }

        public static void RefreshResetButton()
        {
            int need = 0;
            if (CheckCanReset(out need))
            {
                resetButtonImg.color = btnEnableColor;
                resetCostText.text = (-need).ToString();
            }
            else
            {
                resetButtonImg.color = btnDisableColor;
                if (need > 0)
                    resetCostText.text = "<color=#b02020>" + (-need).ToString() + "</color>";
                else
                    resetCostText.text = "0";
            }
        }


        public static void Switch()
        {
            if (spWindowObj == null)
                InitAll();
            else
            {
                if (spWindowObj.activeSelf)
                    Hide();
                else
                    Show();
            }
        }

        public static void Show()
        {
            spWindowObj.SetActive(true);
            for (int i = 0; i < plusUIButtonsL.Count; i++)
            {
                plusUIButtonsL[i].gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = "＋";
                minusUIButtonsL[i].gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = "－";
            }
            for (int i = 0; i < plusUIButtonsR.Count; i++)
            {
                plusUIButtonsR[i].gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = "＋";
                minusUIButtonsR[i].gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = "－";
            }
            Update();
            RefreshResetButton();
        }

        public static void Hide()
        {
            spWindowObj.SetActive(false);
        }

        public static bool EscLogic()
        {
            if (spWindowObj == null)
                return false;
            if (spWindowObj.activeSelf)
            {
                bool flag = !VFInput._godModeMechaMove;
                bool flag2 = VFInput.rtsCancel.onDown || VFInput.escKey.onDown || VFInput.escape || VFInput.delayedEscape;
                if (flag && flag2)
                {
                    VFInput.UseEscape();
                    Hide();
                    return true;
                }
            }
            return false;
        }

        // 随鼠标显示或隐藏问号
        public static void ShowHideSkillTipButton()
        {
            GameObject obj = null;
            Camera cam = Camera.main;
            UnityEngine.EventSystems.EventSystem uiEventSystem = UnityEngine.EventSystems.EventSystem.current;
            if (uiEventSystem != null)
            {
                PointerEventData pointerEventData = new PointerEventData(uiEventSystem);
                pointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                uiEventSystem.RaycastAll(pointerEventData, results);
                if (results.Count > 0)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        string name = results[i].gameObject.name;
                        if (name.Length > 6 && name.Substring(0, 5) == "skill")
                        {
                            obj = results[i].gameObject;
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < skillTipButtonObjsL.Count; i++)
            {
                string descName = "skillL" + i.ToString() + "Desc";
                if (obj != null && obj.name.Split('-')[0] == "skillL" + i.ToString() && descName.Translate() != descName)
                    skillTipButtonObjsL[i].SetActive(true);
                else
                    skillTipButtonObjsL[i].SetActive(false);
            }
            for (int i = 0; i < skillTipButtonObjsR.Count; i++)
            {
                string descName = "skillR" + i.ToString() + "Desc";
                if (obj != null && obj.name.Split('-')[0] == "skillR" + i.ToString() && descName.Translate() != descName)
                    skillTipButtonObjsR[i].SetActive(true);
                else
                    skillTipButtonObjsR[i].SetActive(false);
            }
        }


        public static bool CheckCanReset(out int need)
        {
            need = 0;
            int confirmedAssigned = SkillPoints.skillLevelL.Sum() + SkillPoints.skillLevelR.Sum();
            if (confirmedAssigned <= 0)
                return false;

            need = confirmedAssigned * 10;
            if (confirmedAssigned > 100)
                need += (confirmedAssigned - 100) * 40;
            if (need > 5000)
                need = 5000;

            int itemId = 6006;
            int num = need;
            StorageComponent package = GameMain.mainPlayer.package;
            for (int i = package.size - 1; i >= 0; i--)
            {
                if (package.grids[i].itemId != 0 && (itemId == 0 || package.grids[i].itemId == itemId))
                {
                    itemId = package.grids[i].itemId;
                    if (package.grids[i].count >= num)
                    {
                        return true;
                    }
                    num -= package.grids[i].count;
                }
            }
            return false;
        }

        public static void OnAssignOneClick(int idx)
        {
            bool isLeft = idx < 100;
            idx = idx % 100;
            int unassigned = SkillPoints.UnusedPoints() - tempLevelAddedL.Sum() - tempLevelAddedR.Sum();
            if (unassigned > 0)
            {
                if (isLeft)
                {
                    int remainingLevel = SkillPoints.skillMaxLevelL[idx] - SkillPoints.skillLevelL[idx] - tempLevelAddedL[idx];
                    if (remainingLevel > 0)
                    {
                        int num = 1;
                        if (DspBattlePlugin.isShiftDown)
                        {
                            if (DspBattlePlugin.isControlDown)
                                num = Math.Min(unassigned, remainingLevel);
                            else
                                num = Math.Min(10, Math.Min(unassigned, remainingLevel));
                        }
                        tempLevelAddedL[idx] += num;
                    }
                }
                else
                {
                    int remainingLevel = SkillPoints.skillMaxLevelR[idx] - SkillPoints.skillLevelR[idx] - tempLevelAddedR[idx];
                    if (remainingLevel > 0)
                    {
                        int num = 1;
                        if (DspBattlePlugin.isShiftDown)
                        {
                            if (DspBattlePlugin.isControlDown)
                                num = Math.Min(unassigned, remainingLevel);
                            else
                                num = Math.Min(10, Math.Min(unassigned, remainingLevel));
                        }
                        tempLevelAddedR[idx] += num;
                    }
                }
            }
        }

        public static void OnWithdrawOneClick(int idx)
        {
            bool isLeft = idx < 100;
            idx = idx % 100;
            if (isLeft)
            {
                int tempAssigned = tempLevelAddedL[idx];
                if (DspBattlePlugin.isControlDown && DspBattlePlugin.isShiftDown)
                {
                    tempLevelAddedL[idx] = 0;
                }
                else
                {
                    int num = Math.Min(tempAssigned, DspBattlePlugin.isShiftDown ? 10 : 1);
                    tempLevelAddedL[idx] -= num;
                }
            }
            else
            {
                int tempAssigned = tempLevelAddedR[idx];
                if (DspBattlePlugin.isControlDown && DspBattlePlugin.isShiftDown)
                {
                    tempLevelAddedR[idx] = 0;
                }
                else
                {
                    int num = Math.Min(tempAssigned, DspBattlePlugin.isShiftDown ? 10 : 1);
                    tempLevelAddedR[idx] -= num;
                }
            }
        }

        // 确认本次的分配
        public static void OnConfirmAllClick()
        {
            int assignedTemp = tempLevelAddedL.Sum() + tempLevelAddedR.Sum();
            if (assignedTemp > 0)
            {
                UIMessageBox.Show("分配技能点确认标题".Translate(), String.Format("分配技能点确认警告".Translate()), "否".Translate(), "是".Translate(), 1, new UIMessageBox.Response(() => { }),
                    new UIMessageBox.Response(() =>
                    {
                        SkillPoints.ConfirmAll();
                        RefreshResetButton();
                    }));
            }
        }

        // 撤销本次还未确认的分配
        public static void OnWithdrawAllClick()
        {
            int assignedTemp = tempLevelAddedL.Sum() + tempLevelAddedR.Sum();
            if (assignedTemp > 0)
                ClearTempLevelAdded();
            else
                Hide();
        }

        // 重置所有授权点
        public static void OnResetAllClick()
        {
            int need;
            if (CheckCanReset(out need))
            {
                UIMessageBox.Show("重置技能点确认标题".Translate(), String.Format("重置技能点确认警告".Translate(), need), "否".Translate(), "是".Translate(), 1, new UIMessageBox.Response(() => { }),
                    new UIMessageBox.Response(() =>
                    {
                        int itemId = 6006;
                        int inc = 0;
                        GameMain.mainPlayer.package.TakeTailItems(ref itemId, ref need, out inc, false);
                        SkillPoints.ResetAll();
                        RefreshResetButton();
                    }));
            }
        }

        // 清除暂存的对技能点的分配
        public static void ClearTempLevelAdded()
        {
            if (tempLevelAddedL.Length != SkillPoints.skillCountL)
            {
                tempLevelAddedL = new int[SkillPoints.skillCountL];
            }
            if (tempLevelAddedR.Length != SkillPoints.skillCountR)
            {
                tempLevelAddedR = new int[SkillPoints.skillCountR];
            }
            for (int i = 0; i < tempLevelAddedL.Length; i++)
            {
                tempLevelAddedL[i] = 0;
            }
            for (int i = 0; i < tempLevelAddedR.Length; i++)
            {
                tempLevelAddedR[i] = 0;
            }
        }
    }
}
