using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace DSP_Battle
{
    public class UIRelic
    {
        static int resolutionX = 1920;
        static int resolutionY = 1080;

        // 以下为选择Relic的窗口
        static int closingMaxCountDown = 30;
        static int openingMaxCountDown = 30;
        static int closingCountDown = -1;
        static int openingCountDown = -1;
        static int selectedRelicInUI = 0; // 本次选择了在UI上的左中右(123)哪个遗物

        static GameObject relicSelectionWindowObj = null;
        static GameObject relicSelectionContentObj;
        static GameObject relic1NameObj;
        static GameObject relic1DescObj;
        static GameObject relic1IconObj;
        static GameObject relic2NameObj;
        static GameObject relic2DescObj;
        static GameObject relic2IconObj;
        static GameObject relic3NameObj;
        static GameObject relic3DescObj;
        static GameObject relic3IconObj;
        static GameObject relic1SelectButtonObj;
        static GameObject relic2SelectButtonObj;
        static GameObject relic3SelectButtonObj;
        static GameObject reRollBtnObj;
        static GameObject abortBtnObj;
        static GameObject matrixIcon;
        static GameObject matrixIcon2;
        static Text relic1Name;
        static Text relic1Desc;
        static Image relic1Icon;
        static Image relic1BtnImg;
        static Image relic1Frame;
        static Text relic2Name;
        static Text relic2Desc;
        static Image relic2Icon;
        static Image relic2BtnImg;
        static Image relic2Frame;
        static Text relic3Name;
        static Text relic3Desc;
        static Image relic3Icon;
        static Image relic3BtnImg;
        static Image relic3Frame;
        static UIButton relic1UIBtn;
        static UIButton relic2UIBtn;
        static UIButton relic3UIBtn;

        public static Text relicSelectWindowTitle;
        public static Text relicSelectionNoticeText;
        public static Text rollCostText;
        public static Text rollBtnText;
        public static Text abortBtnText;
        public static Image rollBtnImg;


        //static string colorLegendLeft = "<color=#d2853d>";
        //static string colorEpicLeft = "<color=#9040d0>";
        //static string colorRareLeft = "<color=#2080d0>";
        //static string colorCommonLeft = "<color=#30b530>";
        //static string colorRight = "</color>";
        static Color colorBtnLegend = new Color(0.827f, 0.603f, 0.15f, 0.162f);
        static Color colorBtnEpic = new Color(0.66f, 0f, 0.875f, 0.115f);
        static Color colorBtnRare = new Color(0.183f, 0.426f, 0.811f, 0.178f);
        static Color colorBtnCommon = new Color(0.5f, 0.943f, 0.3f, 0.057f);
        static Color colorBtnCursed0 = new Color(0f, 0.04f, 0.1f, 1f);
        static Color colorBtnCursed2 = new Color(0.82f, 0.24f, 0.20f, 0.94f);
        static Color colorBtnCursed3 = new Color(0.2f, 0.1f, 0.82f, 0.94f);
        static Color colorBtnCursed = new Color(0f, 1f, 0.86f, 0.076f);
        static Color colorBtnLegendH = new Color(0.793f, 0.569f, 0.104f, 0.259f);
        static Color colorBtnEpicH = new Color(0.66f, 0.1f, 0.875f, 0.188f);
        static Color colorBtnRareH = new Color(0.183f, 0.426f, 0.811f, 0.378f);
        static Color colorBtnCommonH = new Color(0.5f, 0.943f, 0.3f, 0.097f);
        static Color colorBtnCursedH = new Color(0f, 1f, 0.86f, 0.116f);
        static Color colorBtnLegendP = new Color(0.827f, 0.603f, 0.15f, 0.091f);
        static Color colorBtnEpicP = new Color(0.66f, 0f, 0.875f, 0.068f);
        static Color colorBtnRareP = new Color(0.183f, 0.426f, 0.811f, 0.098f);
        static Color colorBtnCommonP = new Color(0.5f, 0.943f, 0.3f, 0.04f);
        static Color colorBtnCursedP = new Color(0f, 1f, 0.86f, 0.036f);
        static Color colorBtnDelete = new Color(0.5f, 0.19f, 0.1f, 0.2f);
        static Color colorBtnDeleteH = new Color(0.5f, 0.19f, 0.1f, 0.5f);
        static Color colorBtnDeleteP = new Color(0.5f, 0.19f, 0.1f, 0.1f);
        static Color colorTextLegend = new Color(0.82f, 0.52f, 0.238f, 1f);
        static Color colorTextEpic = new Color(0.563f, 0.25f, 0.813f, 1f);
        static Color colorTextRare = new Color(0.125f, 0.5f, 0.813f, 1f);
        static Color colorTextCommon = new Color(0.188f, 0.609f, 0.188f, 1f);
        static Color colorTextCursed = new Color(0.8f, 0.4f, 0.7f, 1f);
        static Color colorTextDelete = new Color(0.5f, 0.5f, 0.5f, 1f);
        static Color btnDisableColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        static Color btnAbleColor = new Color(0f, 0.499f, 0.824f, 1f);
        public static void InitAll()
        {
            closingCountDown = -1;
            openingCountDown = -1;
            selectedRelicInUI = -1;
            resolutionX = DSPGame.globalOption.resolution.width * DSPGame.globalOption.uiLayoutHeight / DSPGame.globalOption.resolution.height;
            resolutionY = DSPGame.globalOption.uiLayoutHeight;
            InitRelicSelectionWindowUI();
            InitRelicSlotsWindowUI();
            relicSelectionWindowObj.SetActive(false);
        }



        public static void SelectionWindowAnimationUpdate()
        {
            if (relicSelectionWindowObj == null) return;
            // 以下是开关窗口的动画逻辑
            float r = -1;
            if (closingCountDown > 0)
            {
                r = 1.0f * closingCountDown * closingCountDown / closingMaxCountDown / closingMaxCountDown;
            }
            else if (openingCountDown > 0)
            {
                r = 1.01f - 1.0f * openingCountDown * openingCountDown / openingMaxCountDown / openingMaxCountDown;
            }
            if (r > -1)
            {
                relicSelectionWindowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(r * 1000, 600);
                Color tColor = new Color(0.588f, 0.588f, 0.588f, r);
                Color iColor = new Color(1f, 1f, 1f, r);
                if (selectedRelicInUI == 0)
                {
                    relic1BtnImg.color = new Color(relic1BtnImg.color.r, relic1BtnImg.color.g, relic1BtnImg.color.b, relic1BtnImg.color.a * r);
                    relic1UIBtn.transitions[0].target.color = relic1BtnImg.color;
                    relic1Name.color = new Color(relic1Name.color.r, relic1Name.color.g, relic1Name.color.b, relic1Name.color.a * r);
                    relic1Desc.color = tColor;
                    relic1Icon.color = iColor;
                    relic1Frame.color = iColor;
                }
                else if (selectedRelicInUI == 1)
                {
                    relic2BtnImg.color = new Color(relic2BtnImg.color.r, relic2BtnImg.color.g, relic2BtnImg.color.b, relic2BtnImg.color.a * r);
                    relic2UIBtn.transitions[0].target.color = relic2BtnImg.color;
                    relic2Name.color = new Color(relic2Name.color.r, relic2Name.color.g, relic2Name.color.b, relic2Name.color.a * r);
                    relic2Desc.color = tColor;
                    relic2Icon.color = iColor;
                    relic2Frame.color = iColor;
                }
                else if (selectedRelicInUI == 2)
                {
                    relic3BtnImg.color = new Color(relic3BtnImg.color.r, relic3BtnImg.color.g, relic3BtnImg.color.b, relic3BtnImg.color.a * r);
                    relic3UIBtn.transitions[0].target.color = relic3BtnImg.color;
                    relic3Name.color = new Color(relic3Name.color.r, relic3Name.color.g, relic3Name.color.b, relic3Name.color.a * r);
                    relic3Desc.color = tColor;
                    relic3Icon.color = iColor;
                    relic3Frame.color = iColor;
                }

                if (closingCountDown >= 0)
                {
                    closingCountDown--;
                    if (closingCountDown <= 0)
                    {
                        closingCountDown = -1; 
                        relicSelectionWindowObj.SetActive(false);
                    }
                }
                if (openingCountDown >= 0)
                {
                    openingCountDown--;
                    if (openingCountDown <= 0)
                    {
                        openingCountDown = -1; // 必须，否则卡在0造成无法与按钮互动
                        ShowAllInSelectionWindow();
                    }
                }
            }
        }

        static void InitRelicSelectionWindowUI()
        {
            if (relicSelectionWindowObj == null)
            {
                relicSelectionWindowObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/"));
                relicSelectionWindowObj.name = "RelicSelection";
                relicSelectionWindowObj.transform.SetParent(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/").transform);
                relicSelectionWindowObj.transform.SetAsFirstSibling();
                relicSelectionWindowObj.transform.localScale = new Vector3(1, 1, 1);
                relicSelectionWindowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 600);
                relicSelectionWindowObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                relicSelectionWindowObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                Component.Destroy(relicSelectionWindowObj.GetComponent<UIResearchResultWindow>());

                relicSelectionContentObj = relicSelectionWindowObj.transform.Find("content").gameObject;
                relicSelectionContentObj.GetComponent<CanvasGroup>().alpha = 1;
                relicSelectionContentObj.GetComponent<CanvasGroup>().blocksRaycasts = false; // 关键！这样才能使得遗物图标icon不会挡住鼠标按下下层的按钮。

                Component.Destroy(relicSelectionContentObj.transform.Find("title-text").GetComponent<Localizer>());
                relicSelectWindowTitle = relicSelectionContentObj.transform.Find("title-text").GetComponent<Text>();
                relicSelectWindowTitle.text = "发现异星圣物".Translate();
                GameObject.Destroy(relicSelectionContentObj.transform.Find("close-button").gameObject);

                relic1NameObj = relicSelectionContentObj.transform.Find("function-text").gameObject;
                relic1DescObj = relicSelectionContentObj.transform.Find("conclusion").gameObject;
                relic1DescObj.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 24);
                relic1IconObj = relicSelectionContentObj.transform.Find("icon").gameObject;
                relic1NameObj.transform.localPosition = new Vector3(-300, 180);
                relic1DescObj.transform.localPosition = new Vector3(-300, -80);
                relic1IconObj.transform.localPosition = new Vector3(-300, 95);
                relic1IconObj.GetComponent<RectTransform>().sizeDelta = new Vector3(120, 120);
                relic1NameObj.name = "name1";
                relic1DescObj.name = "desc1";
                relic1IconObj.name = "icon1";
                relic1Name = relic1NameObj.GetComponent<Text>();
                relic1Desc = relic1DescObj.GetComponent<Text>();
                relic1Icon = relic1IconObj.GetComponent<Image>();
                relic1IconObj.SetActive(true);
                relic1Desc.fontSize = 16;
                relic1Desc.alignment = TextAnchor.MiddleCenter;
                relic1Name.fontSize = 25;
                relic1Name.supportRichText = true;
                relic1Name.material = GameObject.Find("UI Root/Overlay Canvas/In Game/AlertUI/battle-alert-title/").GetComponent<Text>().material;
                GameObject relic1FrameObj = GameObject.Instantiate(relic1IconObj);
                relic1FrameObj.name = "r1BtnFrame";
                relic1FrameObj.transform.SetParent(relicSelectionContentObj.transform);
                relic1FrameObj.transform.localPosition = new Vector3(-300, 250);
                relic1FrameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 500);
                relic1FrameObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                relic1Frame = relic1FrameObj.GetComponent<Image>();
                relic1Frame.sprite = Resources.Load<Sprite>("Assets/DSPBattle/flagCursedBig");

                relic2NameObj = GameObject.Instantiate(relic1NameObj, relicSelectionContentObj.transform);
                relic2DescObj = GameObject.Instantiate(relic1DescObj, relicSelectionContentObj.transform);
                relic2NameObj.transform.localScale = new Vector3(1, 1, 1);
                relic2DescObj.transform.localScale = new Vector3(1, 1, 1);
                relic2IconObj = relicSelectionContentObj.transform.Find("icon").gameObject;
                relic2NameObj.transform.localPosition = new Vector3(0, 180);
                relic2DescObj.transform.localPosition = new Vector3(0, -80);
                relic2IconObj.transform.localPosition = new Vector3(0, 95);
                relic2IconObj.GetComponent<RectTransform>().sizeDelta = new Vector3(120, 120);
                relic2NameObj.name = "name2";
                relic2DescObj.name = "desc2";
                relic2IconObj.name = "icon2";
                relic2Name = relic2NameObj.GetComponent<Text>();
                relic2Desc = relic2DescObj.GetComponent<Text>();
                relic2Icon = relic2IconObj.GetComponent<Image>();
                relic2IconObj.SetActive(true);
                GameObject relic2FrameObj = GameObject.Instantiate(relic1IconObj);
                relic2FrameObj.name = "r2BtnFrame";
                relic2FrameObj.transform.SetParent(relicSelectionContentObj.transform);
                relic2FrameObj.transform.localPosition = new Vector3(0, 250);
                relic2FrameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 500);
                relic2FrameObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                relic2Frame = relic2FrameObj.GetComponent<Image>();
                relic2Frame.sprite = Resources.Load<Sprite>("Assets/DSPBattle/flagCursedBig");

                relic3NameObj = GameObject.Instantiate(relic1NameObj, relicSelectionContentObj.transform);
                relic3DescObj = GameObject.Instantiate(relic1DescObj, relicSelectionContentObj.transform);
                relic3NameObj.transform.localScale = new Vector3(1, 1, 1);
                relic3DescObj.transform.localScale = new Vector3(1, 1, 1);
                relic3IconObj = relicSelectionContentObj.transform.Find("icon").gameObject;
                relic3NameObj.transform.localPosition = new Vector3(300, 180);
                relic3DescObj.transform.localPosition = new Vector3(300, -80);
                relic3IconObj.transform.localPosition = new Vector3(300, 95);
                relic3IconObj.GetComponent<RectTransform>().sizeDelta = new Vector3(120, 120);
                relic3NameObj.name = "name3";
                relic3DescObj.name = "desc3";
                relic3IconObj.name = "icon3";
                relic3Name = relic3NameObj.GetComponent<Text>();
                relic3Desc = relic3DescObj.GetComponent<Text>();
                relic3Icon = relic3IconObj.GetComponent<Image>();
                relic3IconObj.SetActive(true);
                GameObject relic3FrameObj = GameObject.Instantiate(relic1IconObj);
                relic3FrameObj.name = "r1BtnFrame";
                relic3FrameObj.transform.SetParent(relicSelectionContentObj.transform);
                relic3FrameObj.transform.localPosition = new Vector3(300, 250);
                relic3FrameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 500);
                relic3FrameObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                relic3Frame = relic3FrameObj.GetComponent<Image>();
                relic3Frame.sprite = Resources.Load<Sprite>("Assets/DSPBattle/flagCursedBig");

                //GameObject oriButton = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Station Window/storage-box-0/popup-box/sd-option-button-1");
                GameObject oriButtonWOTip = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Station Window/storage-box-0/popup-box/sd-option-button-1");
                GameObject oriButton = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy/layers/buttons-group/buttons/add-button");
                relic1SelectButtonObj = GameObject.Instantiate(oriButton, relicSelectionWindowObj.transform);
                relic1SelectButtonObj.name = "btn1";
                relic1SelectButtonObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                relic1SelectButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 450);
                relic1SelectButtonObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                relic1SelectButtonObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                relic1SelectButtonObj.transform.localPosition = new Vector3(-300, 0);
                relic1SelectButtonObj.GetComponent<Button>().onClick.RemoveAllListeners();
                relic1SelectButtonObj.GetComponent<Button>().onClick.AddListener(() => { SelectNewRelic(0); });
                relic1SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
                relic1BtnImg = relic1SelectButtonObj.GetComponent<Image>();
                relic1BtnImg.color = colorBtnLegend;
                relic1UIBtn = relic1SelectButtonObj.GetComponent<UIButton>();
                relic1UIBtn.tips.delay = 0;
                relic1UIBtn.tips.offset = new Vector2(-250, 0);
                relic1UIBtn.tips.width = 300;

                relic2SelectButtonObj = GameObject.Instantiate(oriButton, relicSelectionWindowObj.transform);
                relic2SelectButtonObj.name = "btn2";
                relic2SelectButtonObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                relic2SelectButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 450);
                relic2SelectButtonObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                relic2SelectButtonObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                relic2SelectButtonObj.transform.localPosition = new Vector3(0, 0);
                relic2SelectButtonObj.GetComponent<Button>().onClick.RemoveAllListeners();
                relic2SelectButtonObj.GetComponent<Button>().onClick.AddListener(() => { SelectNewRelic(1); });
                relic2SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
                relic2BtnImg = relic2SelectButtonObj.GetComponent<Image>();
                relic2BtnImg.color = colorBtnEpic;
                relic2UIBtn = relic2SelectButtonObj.GetComponent<UIButton>();
                relic2UIBtn.tips.delay = 0;
                relic2UIBtn.tips.offset = new Vector2(250, 0);
                relic2UIBtn.tips.width = 300;

                relic3SelectButtonObj = GameObject.Instantiate(oriButton, relicSelectionWindowObj.transform);
                relic3SelectButtonObj.name = "btn3";
                relic3SelectButtonObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                relic3SelectButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 450);
                relic3SelectButtonObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                relic3SelectButtonObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                relic3SelectButtonObj.transform.localPosition = new Vector3(300, 0);
                relic3SelectButtonObj.GetComponent<Button>().onClick.RemoveAllListeners();
                relic3SelectButtonObj.GetComponent<Button>().onClick.AddListener(() => { SelectNewRelic(2); });
                relic3SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
                relic3BtnImg = relic3SelectButtonObj.GetComponent<Image>();
                relic3BtnImg.color = colorBtnRare;
                relic3UIBtn = relic3SelectButtonObj.GetComponent<UIButton>();
                relic3UIBtn.tips.delay = 0;
                relic3UIBtn.tips.offset = new Vector2(250, 0);
                relic3UIBtn.tips.width = 300;

                // 提示文本
                GameObject noticTextObj = GameObject.Instantiate(relic1DescObj, relicSelectionWindowObj.transform);
                noticTextObj.name = "notice";
                noticTextObj.transform.localScale = new Vector3(1, 1, 1);
                noticTextObj.transform.localPosition = new Vector3(-450, 108, 0);
                noticTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                noticTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                noticTextObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
                noticTextObj.transform.localPosition = new Vector3(-450, 110, 0);
                relicSelectionNoticeText = noticTextObj.GetComponent<Text>();
                relicSelectionNoticeText.alignment = TextAnchor.UpperLeft;
                relicSelectionNoticeText.horizontalOverflow = HorizontalWrapMode.Overflow;
                relicSelectionNoticeText.text = "解译异星圣物提示".Translate();
                relicSelectionNoticeText.fontSize = 14;

                GameObject rollCostTextObj = GameObject.Instantiate(relic1DescObj, relicSelectionWindowObj.transform);
                rollCostTextObj.name = "rollcost";
                rollCostTextObj.transform.localScale = new Vector3(1, 1, 1);
                rollCostTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                rollCostTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                rollCostTextObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
                rollCostTextObj.transform.localPosition = new Vector3(445, 99, 0);
                rollCostText = rollCostTextObj.GetComponent<Text>();
                rollCostText.alignment = TextAnchor.UpperLeft;
                rollCostText.supportRichText = true;
                rollCostText.text = "免费".Translate();

                reRollBtnObj = GameObject.Instantiate(oriButtonWOTip, relicSelectionWindowObj.transform);
                reRollBtnObj.name = "btn-roll";
                reRollBtnObj.transform.localPosition = new Vector3(380, 250, 0);
                reRollBtnObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                reRollBtnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 30);
                rollBtnText = reRollBtnObj.transform.Find("button-text").GetComponent<Text>();
                rollBtnText.text = "重新随机".Translate();
                reRollBtnObj.GetComponent<Button>().onClick.RemoveAllListeners();
                reRollBtnObj.GetComponent<Button>().onClick.AddListener(() => { RollNewAlternateRelics(); });
                rollBtnImg = reRollBtnObj.GetComponent<Image>();
                rollBtnImg.color = btnAbleColor;

                matrixIcon = relicSelectionContentObj.transform.Find("icon").gameObject;
                matrixIcon.name = "icon-matrix-cost";
                matrixIcon.transform.localPosition = new Vector3(430, 263);
                matrixIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                matrixIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Assets/DSPBattle/alienmatrix");
                matrixIcon.SetActive(true);

                //放弃解译按钮
                abortBtnObj = GameObject.Instantiate(oriButtonWOTip, relicSelectionWindowObj.transform);
                abortBtnObj.name = "btn-abort";
                abortBtnObj.transform.localPosition = new Vector3(0, -270, 0);
                abortBtnObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                abortBtnObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
                abortBtnText = abortBtnObj.transform.Find("button-text").GetComponent<Text>();
                abortBtnText.text = "放弃解译".Translate() + Relic.AbortReward.ToString();
                abortBtnObj.transform.Find("button-text").GetComponent<Text>().resizeTextMaxSize = 20;
                abortBtnObj.GetComponent<Button>().onClick.RemoveAllListeners();
                abortBtnObj.GetComponent<Button>().onClick.AddListener(() => { AbortSelection(); });
                abortBtnObj.GetComponent<Image>().color = btnAbleColor;

                matrixIcon2 = relicSelectionContentObj.transform.Find("icon").gameObject;
                matrixIcon2.name = "icon-matrix-abort";
                matrixIcon2.transform.localPosition = new Vector3(23, -249);
                matrixIcon2.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                matrixIcon2.GetComponent<Image>().sprite = Resources.Load<Sprite>("Assets/DSPBattle/alienmatrix");
                matrixIcon2.SetActive(true);

                relicSelectionContentObj.transform.SetAsLastSibling();
                relicSelectionWindowObj.SetActive(false);
            }
        }


        // 准备打开选择遗物窗口的动画
        public static void OpenSelectionWindow()
        {
            selectedRelicInUI = -1; 
            CloseSelectionWindow(); // 先执行立刻全关（立刻体现在closingCountDown在之后手动置为-1）
            closingCountDown = -1;
            closingCountDown = -1; // 取消任何正在关闭的动画
            openingCountDown = openingMaxCountDown; // 如果有打开动画则是=openingMaxCountDown以此开始倒数
            relicSelectionWindowObj.SetActive(true);
            relic1SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
            relic2SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
            relic3SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
        }

        // 准备进行关闭选择遗物窗口的动画
        public static void CloseSelectionWindow()
        {
            closingCountDown = closingMaxCountDown;
            Color zero = new Color(0, 0, 0, 0);
            if (selectedRelicInUI != 0)
            {
                relic1Icon.color = zero;
                relic1Name.color = zero;
                relic1Desc.color = zero;
                relic1BtnImg.color = zero;
                relic1Frame.color = zero;
                relic1UIBtn.transitions[0].target.color = zero;
                relic1UIBtn.transitions[0].normalColor = zero;
                relic1UIBtn.transitions[0].mouseoverColor = zero;
                relic1UIBtn.transitions[0].pressedColor = zero;
            }
            if (selectedRelicInUI != 1)
            {
                relic2Icon.color = zero;
                relic2Name.color = zero;
                relic2Desc.color = zero;
                relic2BtnImg.color = zero;
                relic2Frame.color = zero;
                relic2UIBtn.transitions[0].target.color = zero;
                relic2UIBtn.transitions[0].normalColor = zero;
                relic2UIBtn.transitions[0].mouseoverColor = zero;
                relic2UIBtn.transitions[0].pressedColor = zero;
            }
            if (selectedRelicInUI != 2)
            {
                relic3Icon.color = zero;
                relic3Name.color = zero;
                relic3Desc.color = zero;
                relic3BtnImg.color = zero;
                relic3Frame.color = zero;
                relic3UIBtn.transitions[0].target.color = zero;
                relic3UIBtn.transitions[0].normalColor = zero;
                relic3UIBtn.transitions[0].mouseoverColor = zero;
                relic3UIBtn.transitions[0].pressedColor = zero;
            }
            relicSelectionNoticeText.text = "";
            rollCostText.text = "";
            relicSelectWindowTitle.text = "";
            reRollBtnObj.SetActive(false);
            abortBtnObj.SetActive(false);
            matrixIcon.SetActive(false);
            matrixIcon2.SetActive(false);
        }

        // 立即开启显示选择遗物窗口里的所有元素，同时进行重随
        public static void ShowAllInSelectionWindow()
        {
            reRollBtnObj.SetActive(true);
            abortBtnObj.SetActive(true);
            matrixIcon.SetActive(true);
            matrixIcon2.SetActive(true);

            Color norm = new Color(0.588f, 0.588f, 0.588f, 1);
            Color full = new Color(1, 1, 1, 1);
            relic1Icon.color = full;
            relic1Name.color = norm;
            relic1Desc.color = norm;
            relic2Icon.color = full;
            relic2Name.color = norm;
            relic2Desc.color = norm;
            relic3Icon.color = full;
            relic3Name.color = norm;
            relic3Desc.color = norm;
            relicSelectionWindowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 600);

            RollNewAlternateRelics(); // 重随并刷新
        }


        // 刷新选择窗口的显示
        public static void RefreshSelectionWindowUI()
        {
            relicSelectWindowTitle.text = "发现异星圣物".Translate();
            relicSelectionNoticeText.text = "解译异星圣物提示".Translate();
            rollBtnText.text = "重新随机".Translate();
            abortBtnText.text = "放弃解译".Translate() + "500";
            Color white = new Color(1, 1, 1);

            relic1SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
            relic2SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
            relic3SelectButtonObj.transform.Find("Text").GetComponent<Text>().text = "".Translate();
            relic1UIBtn.tips.tipTitle = "";
            relic1UIBtn.tips.tipText = "";
            relic2UIBtn.tips.tipTitle = "";
            relic2UIBtn.tips.tipText = "";
            relic3UIBtn.tips.tipTitle = "";
            relic3UIBtn.tips.tipText = "";

            relic1Frame.color = new Color(0, 0, 0, 0);
            relic2Frame.color = new Color(0, 0, 0, 0);
            relic3Frame.color = new Color(0, 0, 0, 0);

            // 如果是删除遗物的按钮
            if (Relic.alternateRelics[0] == 999)
            {
                relic1Name.color = colorTextDelete;
                relic1BtnImg.color = colorBtnDelete;
                relic1UIBtn.transitions[0].normalColor = colorBtnDelete;
                relic1UIBtn.transitions[0].mouseoverColor = colorBtnDeleteH;
                relic1UIBtn.transitions[0].pressedColor = colorBtnDeleteP;
                relic1Name.text = "删除遗物名称".Translate();
                relic1Desc.text = "删除遗物描述".Translate();
                relic1Icon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/rNULL");
                relic2Name.color = colorTextDelete;
                relic2BtnImg.color = colorBtnDelete;
                relic2UIBtn.transitions[0].normalColor = colorBtnDelete;
                relic2UIBtn.transitions[0].mouseoverColor = colorBtnDeleteH;
                relic2UIBtn.transitions[0].pressedColor = colorBtnDeleteP;
                relic2Name.text = "删除遗物名称".Translate();
                relic2Desc.text = "删除遗物描述".Translate();
                relic2Icon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/rNULL");
                relic3Name.color = colorTextDelete;
                relic3BtnImg.color = colorBtnDelete;
                relic3UIBtn.transitions[0].normalColor = colorBtnDelete;
                relic3UIBtn.transitions[0].mouseoverColor = colorBtnDeleteH;
                relic3UIBtn.transitions[0].pressedColor = colorBtnDeleteP;
                relic3Name.text = "删除遗物名称".Translate();
                relic3Desc.text = "删除遗物描述".Translate();
                relic3Icon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/rNULL");
            }
            else
            {
                // Relic1
                bool isRemove = Relic.alternateRelics[0] < 0;
                int r1type = Relic.alternateRelics[0] / 100;
                int r1num = Relic.alternateRelics[0] % 100;
                if (isRemove)
                { 
                    r1type = (-1 * (Relic.alternateRelics[0] + 1)) / 100; 
                    r1num = (-1 * (Relic.alternateRelics[0] + 1)) % 100;
                }
                if (r1type == 0)
                {
                    relic1Name.color = colorTextLegend;
                    relic1BtnImg.color = colorBtnLegend;
                    relic1UIBtn.transitions[0].normalColor = colorBtnLegend;
                    relic1UIBtn.transitions[0].mouseoverColor = colorBtnLegendH;
                    relic1UIBtn.transitions[0].pressedColor = colorBtnLegendP;
                }
                else if (r1type == 1)
                {
                    relic1Name.color = colorTextEpic;
                    relic1BtnImg.color = colorBtnEpic;
                    relic1UIBtn.transitions[0].normalColor = colorBtnEpic;
                    relic1UIBtn.transitions[0].mouseoverColor = colorBtnEpicH;
                    relic1UIBtn.transitions[0].pressedColor = colorBtnEpicP;
                }
                else if (r1type == 2)
                {
                    relic1Name.color = colorTextRare;
                    relic1BtnImg.color = colorBtnRare;
                    relic1UIBtn.transitions[0].normalColor = colorBtnRare;
                    relic1UIBtn.transitions[0].mouseoverColor = colorBtnRareH;
                    relic1UIBtn.transitions[0].pressedColor = colorBtnRareP;
                }
                else if (r1type == 3)
                {
                    relic1Name.color = colorTextCommon;
                    relic1BtnImg.color = colorBtnCommon;
                    relic1UIBtn.transitions[0].normalColor = colorBtnCommon;
                    relic1UIBtn.transitions[0].mouseoverColor = colorBtnCommonH;
                    relic1UIBtn.transitions[0].pressedColor = colorBtnCommonP;
                }
                else
                {
                    relic1Name.color = colorTextCursed;
                    relic1BtnImg.color = colorBtnCursed;
                    relic1UIBtn.transitions[0].normalColor = colorBtnCursed;
                    relic1UIBtn.transitions[0].mouseoverColor = colorBtnCursedH;
                    relic1UIBtn.transitions[0].pressedColor = colorBtnCursedP;
                    relic1Frame.color = new Color(1, 1, 1, 1);
                }
                if (isRemove)
                {
                    relic1Name.text = "移除遗物".Translate() + "  " + ("遗物名称" + r1type.ToString() + "-" + r1num.ToString()).Translate();
                    relic1Name.color = colorTextDelete;
                    relic1BtnImg.color = colorBtnDelete;
                    relic1UIBtn.transitions[0].normalColor = colorBtnDelete;
                    relic1UIBtn.transitions[0].mouseoverColor = colorBtnDeleteH;
                    relic1UIBtn.transitions[0].pressedColor = colorBtnDeleteP;
                }
                else
                {
                    relic1Name.text = ("遗物名称" + r1type.ToString() + "-" + r1num.ToString()).Translate();
                }
                relic1Desc.text = ("遗物描述" + r1type.ToString() + "-" + r1num.ToString()).Translate();
                if (r1type == 4) relic1Desc.text += "\n" + "负面效果警告".Translate();
                relic1Icon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/r" + r1type.ToString() + "-" + r1num.ToString());
                AddTipText(r1type, r1num, relic1UIBtn);

                // Relic2
                isRemove = Relic.alternateRelics[1] < 0;
                int r2type = Relic.alternateRelics[1] / 100;
                int r2num = Relic.alternateRelics[1] % 100;
                if (isRemove)
                {
                    r2type = (-1 * (Relic.alternateRelics[1] + 1)) / 100;
                    r2num = (-1 * (Relic.alternateRelics[1] + 1)) % 100;
                }
                if (r2type == 0)
                {
                    relic2Name.color = colorTextLegend;
                    relic2BtnImg.color = colorBtnLegend;
                    relic2UIBtn.transitions[0].normalColor = colorBtnLegend;
                    relic2UIBtn.transitions[0].mouseoverColor = colorBtnLegendH;
                    relic2UIBtn.transitions[0].pressedColor = colorBtnLegendP;
                }
                else if (r2type == 1)
                {
                    relic2Name.color = colorTextEpic;
                    relic2BtnImg.color = colorBtnEpic;
                    relic2UIBtn.transitions[0].normalColor = colorBtnEpic;
                    relic2UIBtn.transitions[0].mouseoverColor = colorBtnEpicH;
                    relic2UIBtn.transitions[0].pressedColor = colorBtnEpicP;
                }
                else if (r2type == 2)
                {
                    relic2Name.color = colorTextRare;
                    relic2BtnImg.color = colorBtnRare;
                    relic2UIBtn.transitions[0].normalColor = colorBtnRare;
                    relic2UIBtn.transitions[0].mouseoverColor = colorBtnRareH;
                    relic2UIBtn.transitions[0].pressedColor = colorBtnRareP;
                }
                else if(r2type == 3)
                {
                    relic2Name.color = colorTextCommon;
                    relic2BtnImg.color = colorBtnCommon;
                    relic2UIBtn.transitions[0].normalColor = colorBtnCommon;
                    relic2UIBtn.transitions[0].mouseoverColor = colorBtnCommonH;
                    relic2UIBtn.transitions[0].pressedColor = colorBtnCommonP;
                }
                else
                {
                    relic2Name.color = colorTextCursed;
                    relic2BtnImg.color = colorBtnCursed;
                    relic2UIBtn.transitions[0].normalColor = colorBtnCursed;
                    relic2UIBtn.transitions[0].mouseoverColor = colorBtnCursedH;
                    relic2UIBtn.transitions[0].pressedColor = colorBtnCursedP;
                    relic2Frame.color = new Color(1, 1, 1, 1);
                }
                if (isRemove)
                {
                    relic2Name.text = "移除遗物".Translate() + "  " + ("遗物名称" + r2type.ToString() + "-" + r2num.ToString()).Translate();
                    relic2Name.color = colorTextDelete;
                    relic2BtnImg.color = colorBtnDelete;
                    relic2UIBtn.transitions[0].normalColor = colorBtnDelete;
                    relic2UIBtn.transitions[0].mouseoverColor = colorBtnDeleteH;
                    relic2UIBtn.transitions[0].pressedColor = colorBtnDeleteP;
                }
                else
                {
                    relic2Name.text = ("遗物名称" + r2type.ToString() + "-" + r2num.ToString()).Translate();
                }
                relic2Desc.text = ("遗物描述" + r2type.ToString() + "-" + r2num.ToString()).Translate();
                if (r2type == 4) relic2Desc.text += "\n" + "负面效果警告".Translate();
                relic2Icon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/r" + r2type.ToString() + "-" + r2num.ToString());
                AddTipText(r2type, r2num, relic2UIBtn);

                // Relic3
                isRemove = Relic.alternateRelics[2] < 0;
                int r3type = Relic.alternateRelics[2] / 100;
                int r3num = Relic.alternateRelics[2] % 100; 
                if (isRemove)
                {
                    r3type = (-1 * (Relic.alternateRelics[2] + 1)) / 100;
                    r3num = (-1 * (Relic.alternateRelics[2] + 1)) % 100;
                }
                if (r3type == 0)
                {
                    relic3Name.color = colorTextLegend;
                    relic3BtnImg.color = colorBtnLegend;
                    relic3UIBtn.transitions[0].normalColor = colorBtnLegend;
                    relic3UIBtn.transitions[0].mouseoverColor = colorBtnLegendH;
                    relic3UIBtn.transitions[0].pressedColor = colorBtnLegendP;
                }
                else if (r3type == 1)
                {
                    relic3Name.color = colorTextEpic;
                    relic3BtnImg.color = colorBtnEpic;
                    relic3UIBtn.transitions[0].normalColor = colorBtnEpic;
                    relic3UIBtn.transitions[0].mouseoverColor = colorBtnEpicH;
                    relic3UIBtn.transitions[0].pressedColor = colorBtnEpicP;
                }
                else if (r3type == 2)
                {
                    relic3Name.color = colorTextRare;
                    relic3BtnImg.color = colorBtnRare;
                    relic3UIBtn.transitions[0].normalColor = colorBtnRare;
                    relic3UIBtn.transitions[0].mouseoverColor = colorBtnRareH;
                    relic3UIBtn.transitions[0].pressedColor = colorBtnRareP;
                }
                else if (r3type == 3)
                {
                    relic3Name.color = colorTextCommon;
                    relic3BtnImg.color = colorBtnCommon;
                    relic3UIBtn.transitions[0].normalColor = colorBtnCommon;
                    relic3UIBtn.transitions[0].mouseoverColor = colorBtnCommonH;
                    relic3UIBtn.transitions[0].pressedColor = colorBtnCommonP;
                }
                else
                {
                    relic3Name.color = colorTextCursed;
                    relic3BtnImg.color = colorBtnCursed;
                    relic3UIBtn.transitions[0].normalColor = colorBtnCursed;
                    relic3UIBtn.transitions[0].mouseoverColor = colorBtnCursedH;
                    relic3UIBtn.transitions[0].pressedColor = colorBtnCursedP;
                    relic3Frame.color = new Color(1, 1, 1, 1);
                }
                if (isRemove)
                {
                    relic3Name.text = "移除遗物".Translate() + "  " + ("遗物名称" + r3type.ToString() + "-" + r3num.ToString()).Translate();
                    relic3Name.color = colorTextDelete;
                    relic3BtnImg.color = colorBtnDelete;
                    relic3UIBtn.transitions[0].normalColor = colorBtnDelete;
                    relic3UIBtn.transitions[0].mouseoverColor = colorBtnDeleteH;
                    relic3UIBtn.transitions[0].pressedColor = colorBtnDeleteP;
                }
                else
                {
                    relic3Name.text = ("遗物名称" + r3type.ToString() + "-" + r3num.ToString()).Translate();
                }
                relic3Desc.text = ("遗物描述" + r3type.ToString() + "-" + r3num.ToString()).Translate();
                if (r3type == 4) relic3Desc.text += "\n" + "负面效果警告".Translate();
                relic3Icon.sprite = Resources.Load<Sprite>("Assets/DSPBattle/r" + r3type.ToString() + "-" + r3num.ToString());
                AddTipText(r3type, r3num, relic3UIBtn);
            }

            if (CheckEnoughMatrixToRoll())
            {
                if (Relic.rollCount <= 0)
                    rollCostText.text = "免费".Translate();
                else
                {
                    int need = Relic.basicMatrixCost << Relic.rollCount;
                    rollCostText.text = "-" + need.ToString();
                }
                rollBtnImg.color = btnAbleColor;
            }
            else
            {
                int need = Relic.basicMatrixCost << Relic.rollCount;
                rollCostText.text = "<color=#b02020>-" + need.ToString() + "</color>";
                rollBtnImg.color = btnDisableColor;
            }

            relicSelectionWindowObj.SetActive(true); // 显示窗口
        }


        public static void AddTipText(int type, int num, UIButton uibt, bool isLeftSlot = false)
        {
            if ((type == 0 && num == 2 && !(Relic.HaveRelic(0, 2) && Relic.relic0_2Version == 0)) || (type == 0 && num == 7) || (type == 0 && num == 10))
            {
                if (uibt.tips.tipTitle.Length == 0)
                    uibt.tips.tipTitle = ($"relicTipTitle{type}-{num}").Translate();
                uibt.tips.tipText += "\n" + ($"relicTipText{type}-{num}").Translate();
            }
            if (type == 4)
            {
                if (uibt.tips.tipTitle.Length == 0)
                    uibt.tips.tipTitle = ($"诅咒").Translate();
                if(isLeftSlot)
                    uibt.tips.tipText += "\n" + "诅咒描述短".Translate() + ($"relicTipText{type}-{num}").Translate(); // 受诅咒的圣物全都具有后面这项TipText，因为都有负面效果
                else
                    uibt.tips.tipText += "诅咒描述".Translate() + ($"relicTipText{type}-{num}").Translate(); // 受诅咒的圣物全都具有后面这项TipText，因为都有负面效果
            }
            if ((type == 4 && num == 6)) // relic4-6 符文之书额外显示记录了哪些relic
            {
                uibt.tips.tipText += "\n\n" + "已记载".Translate() + "  ";
                foreach (var item in Relic.recordRelics)
                {
                    int rType = item / 100;
                    int rNum = item % 100;
                    uibt.tips.tipText += $"遗物名称带颜色{rType}-{rNum}".Translate().Split('[')[0] + "</color>";
                }
                foreach (var item in Relic.recordRelics)
                {
                    int rType = item / 100;
                    int rNum = item % 100;
                    AddTipVarData(rType, rNum, uibt);
                }
            }
        }

        public static void AddTipVarData(int type, int num, UIButton uibt)
        {
            if (type == 0 && num == 2 && Relic.relic0_2Version == 1)
            {
                uibt.tips.tipText = uibt.tips.tipText + "\n\n<color=#61d8ffb4>" + "已充能gm".Translate() + "  " + Relic.relic0_2Charge + " / " + Relic.relic0_2MaxCharge + "</color>";   
            }
            if (type == 0 && num == 10)
                uibt.tips.tipText = uibt.tips.tipText + "\n\n<color=#61d8ffb4>" + "当前加成gm".Translate() + "  " + Droplets.bonusDamage + " / " + Droplets.bonusDamageLimit + "</color>";
        }

        // 检查背包里的矩阵是否足够随机，现在不打算每帧检查来刷新按钮和文本的显示以防突然增加矩阵，可能性不大，即使存在这种可能也不影响实际按下按钮触发功能，只是显示灰色按钮这样
        public static bool CheckEnoughMatrixToRoll()
        {
            if (Relic.rollCount <= 0)
                return true;

            int need = Relic.basicMatrixCost << Relic.rollCount;
            StorageComponent package = GameMain.mainPlayer.package;
            int num = need;
            int itemId = 8032;

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

       

        // 随机3个新的relic 并刷新显示
        public static void RollNewAlternateRelics()
        {
            if (!CheckEnoughMatrixToRoll()) return;
            Relic.alternateRelics[0] = -1; // 三个备选遗物
            Relic.alternateRelics[1] = -1;
            Relic.alternateRelics[2] = -1;
            if (Relic.rollCount > 0)
            {
                int need = Relic.basicMatrixCost << Relic.rollCount;
                int itemId = 8032;
                int inc = 0;
                GameMain.mainPlayer.package.TakeTailItems(ref itemId, ref need, out inc, false);
            }
            if (Relic.GetRelicCount() >= 8) // 如果遗物已满，则刷新的都是删除遗物的按钮
            {
                List<int> relicAlreadyHave = new List<int>();
                for (int type = 0; type < 4; type++)
                {
                    for (int num = 0; num < Relic.relicNumByType[type]; num++)
                    {
                        if (Relic.HaveRelic(type, num) && !Relic.isRecorded(type, num))
                            relicAlreadyHave.Add(100 * type + num);
                    }
                }
                int i = 0;
                while(i < 3 && relicAlreadyHave.Count>0)
                {
                    int relicId = relicAlreadyHave[Utils.RandInt(0, relicAlreadyHave.Count)];
                    Relic.alternateRelics[i] = -1 * relicId - 1; // 遗物id的相反数-1设定为移除遗物
                    relicAlreadyHave.Remove(relicId);
                    i++;
                }
                if (Relic.alternateRelics[2] == -1 && !Relic.HaveRelic(0, 0)) // 说明可以移除的数量不足3个，第三个格子是默认的-1
                {
                    Relic.alternateRelics[2] = 305; // 复活币
                }
            }
            else
            {
                // 重随三个遗物
                for (int i = 0; i < 3; i++)
                {
                    double rand = Utils.RandDouble();
                    double[] prob = Relic.HaveRelic(0, 9) ? Relic.relicTypeProbability : Relic.relicTypeProbabilityBuffed;
                    // relic0-9 五叶草 可以让更高稀有度的遗物刷新概率提高
                    for (int type = 0; type < 5; type++)
                    {
                        if (rand <= prob[type] || (i == 0 && type == 2 && rand < Relic.firstRelicIsRare) || (i==1 && type == 0 && Relic.HaveRelic(4,1) && Relic.rollCount == -1)) // 后面的判别条件是，第一个遗物至少是稀有以上的概率为独立的较大的一个概率，第三个判别条件是relic 4-1的效果 第一次必在中间位置刷一个传说
                        {
                            List<int> relicNotHave = new List<int>();
                            for (int num = 0; num < Relic.relicNumByType[type]; num++)
                            {
                                if (Configs.developerMode) relicNotHave.Add(num);
                                if (!Relic.HaveRelic(type, num) && !Relic.alternateRelics.Contains(type * 100 + num)) relicNotHave.Add(num); 
                            }
                            if (relicNotHave.Count > 0)
                            {
                                Relic.alternateRelics[i] = type * 100 + relicNotHave[Utils.RandInt(0, relicNotHave.Count)];
                                break;
                            }
                            else if (type == 4)
                            {
                                Relic.alternateRelics[i] = 305; // 如果是最后一个循环但是没有该稀有度可选的圣物了，则这个位置设定为可选择的复活币
                            }
                        }
                    }
                    // 有概率在最后一个格子设定为删除一个现有遗物，如果最后一个格子随机到了传说，避免将其改为移除遗物
                    if (i == 2 && Utils.RandDouble() < Relic.relicRemoveProbabilityByRelicCount[Relic.GetRelicCount()] && Relic.alternateRelics[2] >= 100)
                    {
                        List<int> relicAlreadyHave = new List<int>();
                        for (int type = 0; type < 4; type++)
                        {
                            for (int num = 0; num < Relic.relicNumByType[type]; num++)
                            {
                                if (Relic.HaveRelic(type, num) && !Relic.isRecorded(type, num))
                                    relicAlreadyHave.Add(100 * type + num);
                            }
                        }
                        if (relicAlreadyHave.Count > 0)
                        {
                            Relic.alternateRelics[2] = -1 * relicAlreadyHave[Utils.RandInt(0, relicAlreadyHave.Count)] - 1;
                        }
                    }
                }
            }
            Relic.rollCount = Math.Min(Relic.rollCount+1, 8); // 记录本次重随次数，重随消耗有上限
            if (Configs.developerMode)
            {
                //Relic.alternateRelics[0] = 7;
                //Relic.alternateRelics[1] = 2;
                //Relic.alternateRelics[2] = 10;
            }
            RefreshSelectionWindowUI();
        }

        public static void SelectNewRelic(int selection)
        {
            if (closingCountDown >= 0 || openingCountDown >= 0) return; // 打开或关闭窗口过程中禁止互动
            if (Relic.canSelectNewRelic)
            {
                selectedRelicInUI = selection;
                Relic.canSelectNewRelic = false;
                if (Relic.alternateRelics[selection] >= 0)
                {
                    int type = Relic.alternateRelics[selection] / 100;
                    int num = Relic.alternateRelics[selection] % 100;
                    Relic.AddRelic(type, num);
                    CloseSelectionWindow();
                    RefreshSlotsWindowUI();
                }
                else
                {
                    int type = -(Relic.alternateRelics[selection]+1) / 100;
                    int num = -(Relic.alternateRelics[selection]+1) % 100;
                    Relic.AskRemoveRelic(type, num);
                }
            }
        }

        public static void AbortSelection()
        {
            if (closingCountDown >= 0 || openingCountDown >= 0) return; // 打开或关闭窗口过程中禁止互动
            Relic.canSelectNewRelic = false;
            selectedRelicInUI = -1;
            int addCount = Relic.AbortReward;
            int matrixId = 8032;
            GameMain.mainPlayer.TryAddItemToPackage(matrixId, addCount, 0, true);
            Utils.UIItemUp(matrixId, addCount, 180);
            UIBattleStatistics.RegisterAlienMatrixGain(addCount);

            CloseSelectionWindow();
            RefreshSlotsWindowUI();
        }

        
        //  以下为左侧Relic 预览窗口
        static GameObject relicSlotsWindowObj = null;
        public static List<GameObject> relicSlotObjs = new List<GameObject>();
        public static List<Image> relicSlotImgs = new List<Image>();
        public static List<UIButton> relicSlotUIBtns = new List<UIButton>();
        static float targetX = -1065;
        static float curX = -1065;
        

        public static void InitRelicSlotsWindowUI()
        {
            if (relicSlotsWindowObj == null)
            {
                float slotDis = 90; // 每个遗物slot的竖向间隔（顶距离顶）

                Transform parentTrans = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows").transform;
                relicSlotsWindowObj = new GameObject("RelicPanel");
                relicSlotsWindowObj.transform.SetParent(parentTrans); 
                relicSlotsWindowObj.transform.SetAsFirstSibling(); // 置于底层
                GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Mini Lab Panel").transform.SetAsFirstSibling(); //否则UI Root/Overlay Canvas/In Game/Windows/Mini Lab Panel/这个会挡住relic
                relicSlotsWindowObj.AddComponent<RectTransform>();
                RectTransform rt = relicSlotsWindowObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 0);
                rt.pivot = new Vector2(0, 0);
                rt.sizeDelta = new Vector2(0, 0);
                relicSlotsWindowObj.transform.localPosition = new Vector3(-0.5f*resolutionX - 105, -50, 0);
                relicSlotsWindowObj.transform.localScale = new Vector3(1, 1, 1);
                relicSlotsWindowObj.SetActive(true);
                GameObject rightBarObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/inspector/sphere-group/sail-stat/bar-group/bar-orange"), relicSlotsWindowObj.transform);
                rightBarObj.name = "right-bar";
                rightBarObj.transform.localPosition = new Vector3(105, 0.5f * resolutionY - 40, 0);
                rightBarObj.transform.localScale = new Vector3(1, 1, 1);
                rightBarObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                rightBarObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                rightBarObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                rightBarObj.GetComponent<Image>().fillAmount = 1;
                rightBarObj.GetComponent<RectTransform>().sizeDelta = new Vector2(8, slotDis*Relic.relicHoldMax);

                // 创建relic的slot
                GameObject oriIconWithTips = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/content/icon");
                for (int i = 0; i < Relic.relicHoldMax; i++)
                {
                    GameObject iconObj = GameObject.Instantiate(oriIconWithTips);
                    iconObj.name = "slot" + i.ToString();
                    iconObj.transform.SetParent(relicSlotsWindowObj.transform);
                    iconObj.transform.localPosition = new Vector3(50, 0.5f * resolutionY - 0.5f*slotDis - slotDis * i, 0);
                    iconObj.transform.localScale = new Vector3(1, 1, 1);
                    iconObj.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                    relicSlotObjs.Add(iconObj);
                    relicSlotImgs.Add(iconObj.GetComponent<Image>());
                    relicSlotImgs[i].sprite = Resources.Load<Sprite>("Assets/DSPBattle/alienmeta"); // 载入空遗物的图片
                    relicSlotUIBtns.Add(iconObj.GetComponent<UIButton>());
                    iconObj.SetActive(true);
                }
            }
            else 
            {
                HideSlots();
                RefreshSlotsWindowUI();
            }
            

        }

        public static void RefreshSlotsWindowUI(bool onlyVarTips = false)
        {
            int slotNum = 0;
            for (int type = 4; type < 5; type = (type + 1) % 5)
            {
                for (int num = 0; num < Relic.relicNumByType[type]; num++)
                {
                    if (Relic.HaveRelic(type, num) && !Relic.isRecorded(type, num))
                    {
                        if (onlyVarTips)
                        {
                            if (!(type == 0 && num == 2) && !(type == 0 && num == 10) && !(type == 4 && num == 6 && (Relic.recordRelics.Contains(2) || Relic.recordRelics.Contains(10))))
                            {
                                slotNum++;
                                continue;
                            }
                        }
                        if (slotNum < relicSlotImgs.Count)
                        {
                            relicSlotImgs[slotNum].sprite = Resources.Load<Sprite>("Assets/DSPBattle/r" + type.ToString() + "-" + num.ToString());
                            relicSlotUIBtns[slotNum].tips.tipTitle = ("遗物名称带颜色" + type.ToString() + "-" + num.ToString()).Translate();
                            relicSlotUIBtns[slotNum].tips.tipText = ("遗物描述" + type.ToString() + "-" + num.ToString()).Translate();
                            if (type == 0 && num == 2 && Relic.relic0_2Version == 0) // 老版女神泪
                            {
                                relicSlotUIBtns[slotNum].tips.tipText = ("遗物描述" + type.ToString() + "-" + num.ToString() + "old").Translate();
                            }
                            AddTipText(type, num, relicSlotUIBtns[slotNum], true); // 对于一些原本描述较短的，还要将更详细的描述加入
                            AddTipVarData(type, num, relicSlotUIBtns[slotNum]); // 对于部分需要展示实时数据的，还需要加入数据

                            relicSlotUIBtns[slotNum].tips.offset = new Vector2(160, 70);
                            relicSlotUIBtns[slotNum].tips.width = 300;
                            relicSlotUIBtns[slotNum].tips.delay = 0.05f;
                            UIButtonTip uibtnt = relicSlotUIBtns[slotNum].tip as UIButtonTip;
                            if (uibtnt != null) uibtnt.titleComp.supportRichText = true;

                            if (onlyVarTips)
                            {
                                if (UIRelic.relicSlotUIBtns[slotNum].tipShowing)
                                {
                                    UIRelic.relicSlotUIBtns[slotNum].OnPointerExit(null);
                                    UIRelic.relicSlotUIBtns[slotNum].OnPointerEnter(null);
                                    UIRelic.relicSlotUIBtns[slotNum].enterTime = 1;
                                }
                            }

                            slotNum++;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                if (type == 3) break;
            }
            for (; slotNum < relicSlotImgs.Count ; slotNum++)
            {
                relicSlotImgs[slotNum].sprite = Resources.Load<Sprite>("Assets/DSPBattle/rEmpty");
                relicSlotUIBtns[slotNum].tips.tipTitle = ("未获取遗物标题").Translate();
                relicSlotUIBtns[slotNum].tips.tipText = ("未获取遗物描述").Translate();
                relicSlotUIBtns[slotNum].tips.offset = new Vector2(160, 70);
                relicSlotUIBtns[slotNum].tips.delay = 0.05f;
            }
        }

        // 击杀时刷新左侧数据
        public static void RefreshTearOfGoddessSlotTips()
        {
            RefreshSlotsWindowUI(true);
            //int slotNum = 0;
            //for (int rnum = 0; rnum < 2; rnum++)
            //{
            //    if (Relic.HaveRelic(0, rnum))
            //        slotNum++;
            //}
            //if (slotNum >= 8) return;
            //relicSlotUIBtns[slotNum].tips.tipText = "遗物描述0-2".Translate() +"\n" + "relicTipText0-2".Translate() + "\n\n<color=#61d8ffb4>" + "已充能gm".Translate() + "  " + Relic.relic0_2Charge + " / " + Relic.relic0_2MaxCharge + "</color>";
        }


        public static void CheckRelicSlotsWindowShowByMouse()
        {
            Vector3 mouseUIPos = Input.mousePosition;
            if (mouseUIPos.x <= curX + 0.5f * resolutionX + 105 + 8)
            {
                ShowSlots();
            }
            else if(targetX != -0.5f * resolutionX - 105 && !Relic.canSelectNewRelic) // 第二个判据是：正在选择relic时不允许隐藏
            {
                HideSlots();
            }
        }

        public static void SlotWindowAnimationUpdate()
        {
            if (curX != targetX)
            {
                float distance = Math.Abs(targetX - curX);
                float move = Math.Max(distance * 0.2f, 0.5f);
                if (move > distance)
                {
                    relicSlotsWindowObj.transform.localPosition = new Vector3(targetX, -50, 0);
                }
                else 
                {
                    move *= distance / (targetX - curX);
                    relicSlotsWindowObj.transform.localPosition = new Vector3(curX + move, -50, 0);
                }
                curX = relicSlotsWindowObj.transform.localPosition.x;
            }
        }
        
        public static void ShowSlots()
        {
            if (relicSlotsWindowObj != null)
            {
                curX = relicSlotsWindowObj.transform.localPosition.x;
                targetX = -0.5f * resolutionX;
            }
        }

        public static void HideSlots()
        {
            if (relicSlotsWindowObj != null)
            {
                curX = relicSlotsWindowObj.transform.localPosition.x;
                targetX = -0.5f * resolutionX - 105;
            }
        }
    }
}
