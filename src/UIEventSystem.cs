using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using System.Security.Cryptography;

namespace DSP_Battle
{
    public class UIEventSystem
    {
        public static GameObject eventButtonObj = null;
        public static GameObject ESButtonBorderObj;
        public static Image ESButtonImage;
        public static Image ESButtonCircle;
        public static UIButton ESUIButton;
        public static GameObject attentionMarkObj;
        public static Text attentionMarkText;

        public static GameObject eventWindowObj = null;
        public static Text eventWindowTitleText;
        public static Text eventSubTitleText;
        public static Text eventDescText;
        public static Text probabilityTextCursed;
        public static Text probabilityTextLegend;
        public static Text probabilityTextCommon;
        public static Text probabilityTextRare;
        public static Text probabilityTextEpic;
        public static Text probabilityTextReroll;

        public static List<GameObject> decisionButtonObjs = new List<GameObject>();
        public static List<Text> decisionTexts = new List<Text>();
        public static List<UIButton> decisionUIButtons = new List<UIButton>();
        public static Text closeButtonText;

        public static int animationTime = 0;
        public static bool ESButtonHighlighting = true;

        public static Color ButtonWarnColorNorm = new Color(0.7f, 0.2f, 0.2f, 0.6f);
        public static Color ButtonWarnColorHigh = new Color(0.7f, 0.2f, 0.2f, 0.8f);
        public static Color ButtonWarnColorPressed = new Color(0.7f, 0.2f, 0.2f, 0.45f);
        public static Color ButtonEnabledColorNorm = new Color(0.2f, 0.5f, 1f, 0.6f);
        public static Color ButtonEnabledColorHigh = new Color(0.2f, 0.5f, 1f, 0.8f);
        public static Color ButtonEnabledColorPressed = new Color(0.2f, 0.5f, 1f, 0.45f);
        public static Color ButtonDisabledColorNorm = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        public static Color ButtonDisabledColorHigh = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        public static Color ButtonDisabledColorPressed = new Color(0.5f, 0.5f, 0.5f, 0.45f);

        private static Sprite alienmatrix = Resources.Load<Sprite>("Assets/DSPBattle/alienmatrix");
        private static Sprite alienmatrixGray = Resources.Load<Sprite>("Assets/DSPBattle/alienmatrixGray");
        private static Sprite alienmatrixCircle = Resources.Load<Sprite>("Assets/DSPBattle/alienmatrixCircle");

        public static void InitAll()
        {
            // 左上角 事件按钮
            if (eventButtonObj != null)
                return;
            GameObject oriIconWithTips = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/content/icon");
            // Init EventSystemButton
            eventButtonObj = GameObject.Instantiate(oriIconWithTips);
            eventButtonObj.name = "eventButton";
            eventButtonObj.transform.SetParent(UIRelic.relicSlotsWindowObj.transform);
            eventButtonObj.transform.localPosition = new Vector3(50, 0.5f * UIRelic.resolutionY + 40, 0);
            eventButtonObj.transform.localScale = new Vector3(1, 1, 1);
            eventButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            eventButtonObj.SetActive(true);
            ESButtonImage = eventButtonObj.GetComponent<Image>();
            ESButtonImage.sprite = alienmatrix;
            ESUIButton = eventButtonObj.GetComponent<UIButton>();
            ESUIButton.tips.offset = new Vector2(160, 50);
            ESUIButton.tips.delay = 0.05f;
            Button button = eventButtonObj.AddComponent<Button>();
            button.onClick.AddListener(() => { OnEventButtonClick(); });
            attentionMarkObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Assembler Window/produce/circle-back/cnt-text"), eventButtonObj.transform);
            attentionMarkObj.name = "attentionmark";
            attentionMarkObj.transform.localPosition = new Vector3(40, -85, 0);
            attentionMarkObj.transform.localScale = new Vector3(1, 1, 1);
            attentionMarkText = attentionMarkObj.GetComponent<Text>();
            attentionMarkText.text = "!";
            attentionMarkText.fontSize = 25;
            ESButtonBorderObj = new GameObject();
            ESButtonBorderObj.transform.SetParent(eventButtonObj.transform);
            ESButtonBorderObj.name = "border";
            ESButtonBorderObj.transform.localScale = new Vector3(1, 1, 1);
            ESButtonBorderObj.transform.localPosition = new Vector3(0, 0, 0);
            ESButtonCircle = ESButtonBorderObj.AddComponent<Image>();
            ESButtonCircle.sprite = alienmatrixCircle;
            ESButtonCircle.material = attentionMarkText.material;
            if (ESButtonBorderObj.GetComponent<RectTransform>() == null)
            {
                Utils.Log("null\nnull\nnull\nnull");
                ESButtonBorderObj.AddComponent<RectTransform>();
            }
            ESButtonBorderObj.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);

            // 事件消息和选择窗口
            eventWindowObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/"));
            eventWindowObj.name = "EventWindow";
            eventWindowObj.transform.SetParent(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/").transform);
            eventWindowObj.transform.SetAsFirstSibling();
            eventWindowObj.transform.localScale = new Vector3(1, 1, 1);
            eventWindowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 600);
            eventWindowObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            eventWindowObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
            Component.Destroy(eventWindowObj.GetComponent<UIResearchResultWindow>());
            eventWindowObj.SetActive(false);

            GameObject eventWindowContentObj = eventWindowObj.transform.Find("content").gameObject;
            eventWindowContentObj.GetComponent<CanvasGroup>().alpha = 1;
            //eventWindowContentObj.GetComponent<CanvasGroup>().blocksRaycasts = true;

            Component.Destroy(eventWindowContentObj.transform.Find("title-text").GetComponent<Localizer>());
            eventWindowTitleText = eventWindowContentObj.transform.Find("title-text").GetComponent<Text>();
            eventWindowTitleText.text = "事件链窗口标题".Translate();
            GameObject.Destroy(eventWindowContentObj.transform.Find("close-button").gameObject);


            GameObject eventSubTitleTextObj = eventWindowContentObj.transform.Find("function-text").gameObject;
            eventSubTitleTextObj.transform.localPosition = new Vector3(0, 230, 0);
            eventSubTitleText = eventSubTitleTextObj.GetComponent<Text>();
            eventSubTitleText.fontSize = 24;
            GameObject eventDescTextObj = eventWindowContentObj.transform.Find("conclusion").gameObject;
            eventDescText = eventDescTextObj.GetComponent<Text>();
            eventDescText.fontSize = 18;
            eventDescTextObj.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 24);

            // 概率文本
            GameObject materialObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/screen/star-cannon-state-text");
            GameObject probabilityObjCursed = GameObject.Instantiate(eventDescTextObj, eventWindowContentObj.transform);
            probabilityObjCursed.name = "prob";
            probabilityObjCursed.transform.localPosition = new Vector3(100, 265, 0);
            probabilityObjCursed.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 24);
            probabilityTextCursed = probabilityObjCursed.GetComponent<Text>();
            probabilityTextCursed.material = materialObj.GetComponent<Text>().material;
            probabilityTextCursed.alignment = TextAnchor.MiddleLeft;
            probabilityTextCursed.fontSize = 16;
            probabilityTextCursed.text = "";
            probabilityTextCursed.supportRichText = true;

            GameObject probabilityObjLegend = GameObject.Instantiate(eventDescTextObj, eventWindowContentObj.transform);
            probabilityObjLegend.name = "prob";
            probabilityObjLegend.transform.localPosition = new Vector3(180, 265, 0);
            probabilityObjLegend.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 24);
            probabilityTextLegend = probabilityObjLegend.GetComponent<Text>();
            probabilityTextLegend.material = materialObj.GetComponent<Text>().material;
            probabilityTextLegend.alignment = TextAnchor.MiddleLeft;
            probabilityTextLegend.fontSize = 16;
            probabilityTextLegend.text = "";
            probabilityTextLegend.supportRichText = true;

            GameObject probabilityObjEpic = GameObject.Instantiate(eventDescTextObj, eventWindowContentObj.transform);
            probabilityObjEpic.name = "prob";
            probabilityObjEpic.transform.localPosition = new Vector3(260, 265, 0);
            probabilityObjEpic.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 24);
            probabilityTextEpic = probabilityObjEpic.GetComponent<Text>();
            probabilityTextEpic.material = materialObj.GetComponent<Text>().material;
            probabilityTextEpic.alignment = TextAnchor.MiddleLeft;
            probabilityTextEpic.fontSize = 16;
            probabilityTextEpic.text = "";
            probabilityTextEpic.supportRichText = true;

            GameObject probabilityObjRare = GameObject.Instantiate(eventDescTextObj, eventWindowContentObj.transform);
            probabilityObjRare.name = "prob";
            probabilityObjRare.transform.localPosition = new Vector3(340, 265, 0);
            probabilityObjRare.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 24);
            probabilityTextRare = probabilityObjRare.GetComponent<Text>();
            probabilityTextRare.material = materialObj.GetComponent<Text>().material;
            probabilityTextRare.alignment = TextAnchor.MiddleLeft;
            probabilityTextRare.fontSize = 16;
            probabilityTextRare.text = "";
            probabilityTextRare.supportRichText = true;

            GameObject probabilityObjCommon = GameObject.Instantiate(eventDescTextObj, eventWindowContentObj.transform);
            probabilityObjCommon.name = "prob";
            probabilityObjCommon.transform.localPosition = new Vector3(420, 265, 0);
            probabilityObjCommon.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 24);
            probabilityTextCommon = probabilityObjCommon.GetComponent<Text>();
            probabilityTextCommon.material = materialObj.GetComponent<Text>().material;
            probabilityTextCommon.alignment = TextAnchor.MiddleLeft;
            probabilityTextCommon.fontSize = 16;
            probabilityTextCommon.text = "";
            probabilityTextCommon.supportRichText = true;

            GameObject probabilityObjReroll = GameObject.Instantiate(eventDescTextObj, eventWindowContentObj.transform);
            probabilityObjReroll.name = "prob";
            probabilityObjReroll.transform.localPosition = new Vector3(500, 265, 0);
            probabilityObjReroll.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 24);
            probabilityTextReroll = probabilityObjReroll.GetComponent<Text>();
            probabilityTextReroll.material = materialObj.GetComponent<Text>().material;
            probabilityTextReroll.alignment = TextAnchor.MiddleLeft;
            probabilityTextReroll.fontSize = 16;
            probabilityTextReroll.text = "";
            probabilityTextReroll.supportRichText = true;

            GameObject oriButtonObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Dyson Sphere Editor/Dyson Editor Control Panel/hierarchy/layers/buttons-group/buttons/add-button");
            decisionButtonObjs = new List<GameObject>();
            for (int i = 0; i < EventProto.maxDecisionCount; i++)
            {
                GameObject buttonObj = GameObject.Instantiate(oriButtonObj);
                buttonObj.name = "decision" + i.ToString();
                buttonObj.transform.SetParent(eventWindowContentObj.transform);
                buttonObj.transform.localPosition = new Vector3(-400, -250 + i * 30, 0);
                buttonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 25);
                Text buttonText = buttonObj.transform.Find("Text").gameObject.GetComponent<Text>();
                GameObject.DestroyImmediate(buttonObj.GetComponent<Button>());
                Button btn = buttonObj.AddComponent<Button>();
                btn.interactable = true;
                string iStr = i.ToString();
                UIButton uibtn = buttonObj.GetComponent<UIButton>();
                uibtn.button = btn;
                Action<int> action = (x) => { EventSystem.Decision(Convert.ToInt32(iStr)); };
                uibtn.onClick += action;
                decisionButtonObjs.Add(buttonObj);
                decisionTexts.Add(buttonText);
                decisionUIButtons.Add(uibtn);
            }

            // 关闭窗口按钮
            GameObject closeButtonObj = GameObject.Instantiate(oriButtonObj);
            closeButtonObj.name = "close";
            closeButtonObj.transform.SetParent(eventWindowContentObj.transform);
            closeButtonObj.transform.localPosition = new Vector3(480, 300, 0);
            closeButtonObj.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            closeButtonText = closeButtonObj.transform.Find("Text").gameObject.GetComponent<Text>();
            closeButtonText.text = "-";
            GameObject.DestroyImmediate(closeButtonObj.GetComponent<Button>());
            Button closeBtn = closeButtonObj.AddComponent<Button>();
            closeBtn.interactable = true;
            closeButtonObj.GetComponent<UIButton>().button = closeBtn;
            Action<int> closeAction = (x) => { OnClose(); };
            closeButtonObj.GetComponent<UIButton>().onClick += closeAction;
        }

        public static void OnEventButtonClick()
        {
            if (eventWindowObj == null)
                return;
            if (eventWindowObj.activeSelf)
                OnClose();
            else
                OnOpen();
        }

        public static void OnOpen()
        {
            if (eventWindowObj == null)
                return;
            if (animationTime != 0)
                return;
            if (EventSystem.recorder == null || EventSystem.recorder.protoId <= 0)
                return;
            eventWindowObj.transform.localScale = new Vector3(1, 1, 1);
            eventWindowObj.transform.localPosition = new Vector3(0, 0, 0);
            eventWindowObj.SetActive(true);
            animationTime = 0;
            closeButtonText.text = "一";
            RefreshAll();
        }

        public static void OnClose(bool closeImmediate = false)
        {
            if (eventWindowObj == null)
                return;
            if (closeImmediate)
            {
                eventWindowObj.SetActive(false);
                animationTime = 0;
            }
            else
            {
                animationTime = -10;
            }
        }

        public static bool EscLogic()
        {
            if (eventWindowObj == null)
                return false;
            if (eventWindowObj.activeSelf)
            {
                bool flag = !VFInput._godModeMechaMove;
                bool flag2 = VFInput.rtsCancel.onDown || VFInput.escKey.onDown || VFInput.escape || VFInput.delayedEscape;
                if (flag && flag2)
                {
                    VFInput.UseEscape();
                    OnClose();
                    return true;
                }
            }
            return false;
        }



        public static void OnUpdate()
        {
            if (eventButtonObj == null)
                return;

            RefreshESButton();
            if (eventWindowObj.activeSelf)
                RefreshESWindow();
        }

        public static void RefreshAll()
        {
            RefreshESButton();
            RefreshESWindow();
        }

        public static void RefreshESButton()
        {
            if (EventSystem.recorder != null && EventSystem.recorder.protoId > 0 && GameMain.instance != null)
            {
                ESButtonImage.sprite = alienmatrix;
                ESButtonHighlighting = false;
                if (EventSystem.protos.ContainsKey(EventSystem.recorder.protoId)) // 至少有一个非结束事件链的decision的所有request被满足时，highlight
                {
                    EventProto proto = EventSystem.protos[EventSystem.recorder.protoId];
                    int[][] decisionReqNeed = proto.decisionRequestNeed;
                    for (int i = 0; i < proto.decisionLen; i++)
                    {
                        bool allSatisfied = true;
                        int[] reqs = decisionReqNeed[i];
                        if (reqs != null && reqs.Length > 0)
                        {
                            for (int j = 0; j < reqs.Length; j++)
                            {
                                int reqIndex = reqs[j];
                                if (reqIndex < EventSystem.recorder.requestCount.Length)
                                {
                                    if (EventSystem.recorder.requestMeet[reqIndex] < EventSystem.recorder.requestCount[reqIndex])
                                    {
                                        allSatisfied = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (allSatisfied)
                        {
                            int[] decisionResults = proto.decisionResultId[i];
                            if (decisionResults == null)
                            {
                                allSatisfied = false;
                                continue;
                            }
                            for (int j = 0; j < decisionResults.Length; j++)
                            {
                                if (decisionResults[j] == -1) // 结束事件链一般都是无需前置条件的，所以不参与事件按钮是否高亮的决定
                                {
                                    allSatisfied = false;
                                    break;
                                }
                            }
                        }
                        if (allSatisfied && (EventSystem.recorder.decodeType == 0 || EventSystem.recorder.decodeTimeSpend >= EventSystem.recorder.decodeTimeNeed))
                        {
                            ESButtonHighlighting = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                ESButtonImage.sprite = alienmatrixGray;
                ESButtonHighlighting = false;
            }
            // 处理有选项可完成时，按钮始终显示以及闪烁动画
            if (ESButtonHighlighting)
            {
                float y = eventButtonObj.transform.localPosition.y;
                float outDis = UIRelic.relicSlotsWindowObj.transform.localPosition.x + 105f + 0.5f * UIRelic.resolutionX;
                eventButtonObj.transform.localPosition = new Vector3(50 + 105 - outDis, y, 0);
                attentionMarkObj.SetActive(true);
                ESButtonBorderObj.SetActive(true);
                int t = (int)(GameMain.instance.timei % 120);
                float alpha = 0.7f + 0.3f * t / 60;
                if (t > 60)
                {
                    alpha = 1f - 0.3f * (t - 60) / 60;
                }
                attentionMarkText.color = new Color(0.14f, 0.94f, 1f, alpha);
                ESButtonCircle.color = new Color(0.14f, 0.94f, 1f, alpha);
            }
            else
            {
                float y = eventButtonObj.transform.localPosition.y;
                eventButtonObj.transform.localPosition = new Vector3(50, y, 0);
                attentionMarkObj.SetActive(false);
                ESButtonBorderObj.SetActive(false);
                //ESButtonCircle.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        public static void RefreshESWindow(bool forceSetButtonColor = false)
        {
            if (eventWindowObj == null)
                return;

            // 处理关闭动画逻辑，用来提示玩家再次打开这个窗口的按钮在哪儿
            if (eventWindowObj.activeSelf && animationTime != 0)
            {
                float timeLeft = Math.Abs(animationTime);
                float scale = 1.0f * (timeLeft - 1) / 10;
                if (timeLeft <= 1) timeLeft = 1;
                scale -= 0.1f;
                eventWindowObj.transform.localScale = new Vector3(scale, scale, scale);
                float xDis = (eventButtonObj.transform.position.x - eventWindowObj.transform.position.x) / (float)Math.Pow(timeLeft, 1);
                float yDis = (eventButtonObj.transform.position.y - eventWindowObj.transform.position.y) / (float)Math.Pow(timeLeft, 1);
                eventWindowObj.transform.position =
                    new Vector3(eventWindowObj.transform.position.x + xDis, eventWindowObj.transform.position.y + yDis, eventWindowObj.transform.position.z);

                if (animationTime < 0)
                {
                    animationTime++;
                    if (animationTime == 0)
                        eventWindowObj.SetActive(false);
                }
                else if (animationTime > 0)
                    animationTime--;
            }
            if (EventSystem.recorder != null && EventSystem.recorder.protoId > 0)
            {
                ref EventRecorder recorder = ref EventSystem.recorder;
                if (recorder.decodeType != 0 && recorder.decodeTimeSpend < recorder.decodeTimeNeed && recorder.decodeTimeNeed != 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        decisionButtonObjs[i].SetActive(false);
                    }
                    eventSubTitleText.text = ($"decodeType{recorder.decodeType}Title").Translate() + "\n" + string.Format("{0:0.0}%", 100.0 * recorder.decodeTimeSpend / recorder.decodeTimeNeed);
                    int tickLeft = recorder.decodeTimeNeed - recorder.decodeTimeSpend;

                    eventDescText.text = "\n\n\n" + "预计剩余解译时间".Translate() + string.Format(" {0:D2}:{1:D2}", tickLeft / 3600, tickLeft % 3600 / 60);
                }
                else
                {
                    eventSubTitleText.text = ($"ept{recorder.protoId}").Translate();
                    eventDescText.text = ($"epd{recorder.protoId}").Translate();
                    EventProto proto = EventSystem.protos[recorder.protoId];
                    int decisionLen = proto.decisionLen;
                    decisionLen = decisionLen > 4 ? 4 : decisionLen;
                    for (int i = 0; i < decisionLen; i++)
                    {
                        decisionButtonObjs[i].SetActive(true);
                        decisionTexts[i].text = ($"epdt{proto.id}-{i}").Translate();
                        string tipTitle = "执行此决定你".Translate();
                        string tipTextNeed = "";
                        bool enabled = true;
                        if (proto.decisionRequestNeed[i].Length > 0)
                        {
                            for (int j = 0; j < proto.decisionRequestNeed[i].Length; j++)
                            {
                                int requestIndex = proto.decisionRequestNeed[i][j];
                                int code = recorder.requestId[requestIndex];
                                if (code == 0) continue;
                                bool finished = recorder.requestMeet[requestIndex] >= recorder.requestCount[requestIndex];
                                if (!finished)
                                    enabled = false;
                                if (code == 9995)
                                    tipTextNeed += "\n    " + "功勋阶级达到".Translate() + $"  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]}";
                                else if (code == 9996)
                                    tipTextNeed += "\n    " + "伊卡洛斯被摧毁次数".Translate() + $"  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]}";
                                else if (code == 9997)
                                    tipTextNeed += "\n    " + "消灭地面黑雾".Translate() + $"  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]}";
                                else if (code == 9998)
                                    tipTextNeed += "\n    " + "消灭太空黑雾".Translate() + $"  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]}";
                                else if (code == 9999)
                                    tipTextNeed += "\n    " + "消灭任意黑雾".Translate() + $"  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]}";
                                else if (code >= 10000 && code < 20000)
                                    tipTextNeed += "\n    " + "提供物品".Translate() + $"{LDB.ItemName(recorder.requestId[requestIndex] - 10000)}  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]}";
                                else if (code >= 20000 && code < 30000)
                                    tipTextNeed += "\n    " + "物品产量".Translate() + $"{LDB.ItemName(recorder.requestId[requestIndex] - 10000)}  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]} /min";
                                else if (code == 30000)
                                    tipTextNeed += "\n    " + "解锁任意科技".Translate() + $"  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]}";
                                else if (code > 30000 && code < 40000)
                                    tipTextNeed += "\n    " + "解锁gm".Translate() + LDB.techs.Select(code - 30000).name + "至等级".Translate() + $"  {recorder.requestMeet[requestIndex]}/{recorder.requestCount[requestIndex]}";
                                else if (code >= 40000 && code < 50000)
                                {
                                    int starId = code - 40000 + 1;
                                    if (recorder.requestCount[requestIndex] == 0)
                                    {
                                        if (recorder.requestMeet[requestIndex] == int.MinValue)
                                            tipTextNeed += "\n    "
                                                + string.Format("消灭恒星系全部地面单位".Translate(), GameMain.galaxy.StarById(starId)?.displayName, finished ? "" : "点击以导航".Translate(), "数量未知gm".Translate());
                                        else
                                            tipTextNeed += "\n    "
                                                + string.Format("消灭恒星系全部地面单位".Translate(), GameMain.galaxy.StarById(starId)?.displayName, finished ? "" : "点击以导航".Translate(), -recorder.requestMeet[requestIndex]);
                                    }
                                    else
                                        tipTextNeed += "\n    "
                                            + string.Format("消灭恒星系地面单位".Translate(), GameMain.galaxy.StarById(starId)?.displayName, recorder.requestMeet[requestIndex], recorder.requestCount[requestIndex], finished ? "" : "点击以导航".Translate());
                                }
                                else if (code >= 50000 && code < 60000)
                                {
                                    int starId = code - 50000 + 1;
                                    if (recorder.requestCount[requestIndex] == 0)
                                    {
                                        tipTextNeed += "\n    "
                                            + string.Format("消灭恒星系全部太空单位".Translate(), GameMain.galaxy.StarById(starId)?.displayName, finished ? "" : "点击以导航".Translate(), -recorder.requestMeet[requestIndex]);
                                    }
                                    else
                                        tipTextNeed += "\n    "
                                            + string.Format("消灭恒星系太空单位".Translate(), GameMain.galaxy.StarById(starId)?.displayName, recorder.requestMeet[requestIndex], recorder.requestCount[requestIndex], finished ? "" : "点击以导航".Translate());
                                }
                                else if (code >= 60000 && code < 70000)
                                {
                                    int starId = code - 60000 + 1;
                                    tipTextNeed += "\n    " + string.Format("提升恒星系威胁等级".Translate(), GameMain.galaxy.StarById(starId)?.displayName, recorder.requestMeet[requestIndex], recorder.requestCount[requestIndex], finished ? "" : "点击以导航".Translate());
                                }
                                else if (code >= 70000 && code < 80000)
                                {
                                    int starId = code - 70000 + 1;
                                    if (recorder.requestMeet[requestIndex] == int.MinValue)
                                        tipTextNeed += "\n    " + string.Format("肃清恒星系".Translate(), GameMain.galaxy.StarById(starId)?.displayName, finished ? "" : "点击以导航".Translate(), "数量未知gm".Translate());
                                    else
                                        tipTextNeed += "\n    " + string.Format("肃清恒星系".Translate(), GameMain.galaxy.StarById(starId)?.displayName, finished ? "" : "点击以导航".Translate(), -recorder.requestMeet[requestIndex]);
                                }
                                else if (code >= 80000 && code < 90000)
                                {
                                    int starId = code - 80000 + 1;
                                    string starName = code == 89999 ? "任意gm".Translate() : GameMain.galaxy.StarById(starId)?.displayName;
                                    tipTextNeed += "\n    " + string.Format("提升巨构能量水平".Translate(), starName, recorder.requestMeet[requestIndex] / 1000, recorder.requestCount[requestIndex] / 1000, finished || code == 89999 ? "" : "点击以导航".Translate());
                                }
                                else if (code >= 90000 && code < 100000)
                                {
                                    int starId = code - 90000 + 1;
                                    string starName = GameMain.galaxy.StarById(starId)?.displayName;
                                    tipTextNeed += "\n    " + string.Format("提升太空黑雾巢穴等级".Translate(), starName, recorder.requestMeet[requestIndex], recorder.requestCount[requestIndex], finished ? "" : "点击以导航".Translate());
                                }
                                else if (code >= 1000000 && code < 2000000)
                                {
                                    EnemyDFHiveSystem[] dfHivesByAstro = GameMain.data.spaceSector.dfHivesByAstro;
                                    EnemyDFHiveSystem hive = dfHivesByAstro[code - 1000000];
                                    if (hive != null)
                                    {
                                        string starName = hive.starData.displayName;
                                        string hiveName = hive.hiveCode;
                                        tipTextNeed += "\n    " + string.Format("消灭太空黑雾巢穴的所有单位".Translate(), starName, hiveName, -recorder.requestMeet[requestIndex], finished ? "" : "点击以导航".Translate());
                                    }
                                }
                                else if (code >= 2000000 && code < 3000000)
                                {
                                    int planetId = code - 2000000;
                                    string planetName = GameMain.galaxy.PlanetById(planetId)?.displayName;
                                    if (planetName != null)
                                    {
                                        if (recorder.requestCount[requestIndex] == 0)
                                        {
                                            if (recorder.requestMeet[requestIndex] == int.MinValue)
                                                tipTextNeed += "\n    " + string.Format("消灭行星全部黑雾单位".Translate(), planetName, finished ? "" : "点击以导航".Translate(), "数量未知gm2".Translate());
                                            else
                                                tipTextNeed += "\n    " + string.Format("消灭行星全部黑雾单位".Translate(), planetName, finished ? "" : "点击以导航".Translate(), -recorder.requestMeet[requestIndex]);

                                        }
                                        else
                                            tipTextNeed += "\n    " + string.Format("消灭行星黑雾单位".Translate(), planetName, recorder.requestMeet[requestIndex], recorder.requestCount[requestIndex], finished ? "" : "点击以导航".Translate());
                                    }
                                }
                                else if (code >= 3000000 && code < 4000000)
                                {
                                    int planetId = code - 3000000;
                                    string planetName = GameMain.galaxy.PlanetById(planetId)?.displayName;
                                    if (planetName != null)
                                    {
                                        if (recorder.requestMeet[requestIndex] == int.MinValue)
                                            tipTextNeed += "\n    " + string.Format("消灭行星全部黑雾基地".Translate(), planetName, finished ? "" : "点击以导航".Translate(), "数量未知gm2".Translate());
                                        else
                                            tipTextNeed += "\n    " + string.Format("消灭行星全部黑雾基地".Translate(), planetName, finished ? "" : "点击以导航".Translate(), -recorder.requestMeet[requestIndex]);
                                    }
                                }
                                else if (code >= 4000000 && code < 5000000)
                                {
                                    int planetId = code - 4000000;
                                    string planetName = GameMain.galaxy.PlanetById(planetId)?.displayName;
                                    if (planetName != null)
                                    {
                                        tipTextNeed += "\n    " + string.Format("到达行星gm".Translate(), planetName, finished ? "已到达gm".Translate() : "点击以导航".Translate());
                                    }
                                }
                            }
                        }
                        if (tipTextNeed.Length > 0)
                            tipTextNeed = "需要gm".Translate() + tipTextNeed;
                        // 
                        string tipTextResult = "";
                        bool containsCancelResult = false;
                        if (proto.decisionResultId[i].Length > 0)
                        {
                            for (int j = 0; j < proto.decisionResultId[i].Length; j++)
                            {
                                int code = proto.decisionResultId[i][j];
                                if (code == -1)
                                {
                                    containsCancelResult = true;
                                    tipTextResult += "\n    " + "这将终止序列".Translate();
                                }
                                else if (code == 0)
                                    tipTextResult += "\n    " + "解译元驱动".Translate();
                                else if (code == 1)
                                {
                                    if (proto.decisionResultCount[i][j] > 0)
                                        tipTextResult += "\n    " + "获得功勋点数".Translate() + proto.decisionResultCount[i][j].ToString();
                                    else if (proto.decisionResultCount[i][j] < 0)
                                        tipTextResult += "\n    " + "失去功勋点数".Translate() + (-proto.decisionResultCount[i][j]).ToString();
                                }
                                else if (code == 2)
                                {
                                    if (proto.decisionResultCount[i][j] > 0)
                                        tipTextResult += "\n    " + "提升功勋阶级".Translate() + proto.decisionResultCount[i][j].ToString();
                                    else if (proto.decisionResultCount[i][j] < 0)
                                        tipTextResult += "\n    " + "降低功勋阶级".Translate() + (-proto.decisionResultCount[i][j]).ToString();
                                }
                                else if (code == 3)
                                    tipTextResult += "\n    " + "推进随机巨构".Translate();
                                else if (code == 4)
                                    tipTextResult += "\n    " + "本次圣物解译普通概率".Translate() + (proto.decisionResultCount[i][j] >= 0 ? "+" : "") + proto.decisionResultCount[i][j].ToString() + "%";
                                else if (code == 5)
                                    tipTextResult += "\n    " + "本次圣物解译稀有概率".Translate() + (proto.decisionResultCount[i][j] >= 0 ? "+" : "") + proto.decisionResultCount[i][j].ToString() + "%";
                                else if (code == 6)
                                    tipTextResult += "\n    " + "本次圣物解译史诗概率".Translate() + (proto.decisionResultCount[i][j] >= 0 ? "+" : "") + proto.decisionResultCount[i][j].ToString() + "%";
                                else if (code == 7)
                                    tipTextResult += "\n    " + "本次圣物解译传说概率".Translate() + (proto.decisionResultCount[i][j] >= 0 ? "+" : "") + proto.decisionResultCount[i][j].ToString() + "%";
                                else if (code == 8)
                                    tipTextResult += "\n    " + "本次圣物解译被诅咒的概率".Translate() + (proto.decisionResultCount[i][j] >= 0 ? "+" : "") + proto.decisionResultCount[i][j].ToString() + "%";
                                else if (code == 9)
                                {
                                    tipTextResult += "\n    " + "免费随机次数".Translate() + (proto.decisionResultCount[i][j] >= 0 ? "+" : "") + proto.decisionResultCount[i][j].ToString();
                                }
                                else if (code >= 20000 && code <= 30000)
                                {
                                    int itemId = code - 20000;
                                    tipTextResult += "\n    " + "获得物品".Translate() + proto.decisionResultCount[i][j].ToString() + LDB.ItemName(itemId);
                                }
                                else if (code >= 100000000)
                                    tipTextResult += "\n    " + "未知后果".Translate();
                            }
                        }
                        if (tipTextResult.Length > 0)
                            tipTextResult = "此选项将导致".Translate() + tipTextResult;
                        if (tipTextNeed.Length + tipTextResult.Length > 0)
                        {
                            decisionUIButtons[i].tips.tipTitle = tipTitle;
                            string mid = tipTextNeed.Length > 0 && tipTextResult.Length > 0 ? "\n\n" : "";
                            decisionUIButtons[i].tips.tipText = tipTextNeed + mid + tipTextResult;
                            if (decisionUIButtons[i].tip != null)
                            {
                                UIButtonTip tip = decisionUIButtons[i].tip as UIButtonTip;
                                if (tip?.titleComp != null && tip?.subTextComp != null)
                                {
                                    tip.titleComp.text = tipTitle;
                                    tip.subTextComp.text = tipTextNeed + mid + tipTextResult;
                                    int pHeight = (int)(tip.subTextComp.preferredHeight / 2f) * 2;
                                    tip.trans.sizeDelta = new Vector2(tip.trans.sizeDelta.x, (float)(pHeight + 38));
                                }
                            }
                            decisionUIButtons[i].tips.delay = 0.1f;
                            decisionUIButtons[i].tips.offset = new Vector2(320, -40);
                            decisionUIButtons[i].tips.width = 400;
                        }
                        else
                        {
                            decisionUIButtons[i].tips.tipTitle = "";
                            decisionUIButtons[i].tips.tipText = "";
                            if (decisionUIButtons[i].tip != null)
                            {
                                UIButtonTip tip = decisionUIButtons[i].tip as UIButtonTip;
                                if (tip?.gameObject != null)
                                {
                                    tip.gameObject.SetActive(false);
                                }
                            }
                        }
                        if (!enabled)
                        {
                            int transLen = decisionUIButtons[i].transitions.Length;
                            for (int j = 0; j < transLen && j < 1; j++)
                            {
                                float oldR = decisionUIButtons[i].transitions[j].normalColor.r;
                                decisionUIButtons[i].transitions[j].normalColor = ButtonDisabledColorNorm;
                                decisionUIButtons[i].transitions[j].mouseoverColor = ButtonDisabledColorHigh;
                                decisionUIButtons[i].transitions[j].pressedColor = ButtonDisabledColorPressed;
                                if (forceSetButtonColor || oldR != decisionUIButtons[i].transitions[j].normalColor.r)
                                {
                                    if (decisionUIButtons[i].isPointerEnter)
                                        decisionButtonObjs[i].GetComponent<Image>().color = decisionUIButtons[i].transitions[0].mouseoverColor;
                                    else
                                        decisionButtonObjs[i].GetComponent<Image>().color = decisionUIButtons[i].transitions[0].normalColor;
                                }
                            }
                        }
                        else if (containsCancelResult)
                        {
                            int transLen = decisionUIButtons[i].transitions.Length;
                            for (int j = 0; j < transLen && j < 1; j++)
                            {
                                float oldR = decisionUIButtons[i].transitions[j].normalColor.r;
                                decisionUIButtons[i].transitions[j].normalColor = ButtonWarnColorNorm;
                                decisionUIButtons[i].transitions[j].mouseoverColor = ButtonWarnColorHigh;
                                decisionUIButtons[i].transitions[j].pressedColor = ButtonWarnColorPressed;
                                if (forceSetButtonColor || oldR != decisionUIButtons[i].transitions[j].normalColor.r)
                                {
                                    if (decisionUIButtons[i].isPointerEnter)
                                        decisionButtonObjs[i].GetComponent<Image>().color = decisionUIButtons[i].transitions[0].mouseoverColor;
                                    else
                                        decisionButtonObjs[i].GetComponent<Image>().color = decisionUIButtons[i].transitions[0].normalColor;
                                }
                            }
                        }
                        else
                        {
                            int transLen = decisionUIButtons[i].transitions.Length;
                            for (int j = 0; j < transLen && j < 1; j++)
                            {
                                float oldR = decisionUIButtons[i].transitions[j].normalColor.r;
                                decisionUIButtons[i].transitions[j].normalColor = ButtonEnabledColorNorm;
                                decisionUIButtons[i].transitions[j].mouseoverColor = ButtonEnabledColorHigh;
                                decisionUIButtons[i].transitions[j].pressedColor = ButtonEnabledColorPressed;
                                if (forceSetButtonColor || oldR != decisionUIButtons[i].transitions[j].normalColor.r)
                                {
                                    if (decisionUIButtons[i].isPointerEnter)
                                        decisionButtonObjs[i].GetComponent<Image>().color = decisionUIButtons[i].transitions[0].mouseoverColor;
                                    else
                                        decisionButtonObjs[i].GetComponent<Image>().color = decisionUIButtons[i].transitions[0].normalColor;
                                }
                            }
                        }
                    }
                    if (decisionLen < 4)
                    {
                        for (int i = decisionLen; i < 4; i++)
                        {
                            decisionButtonObjs[i].SetActive(false);
                        }
                    }
                }

                // 显示预计概率
                int[] preview = new int[] { 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < preview.Length; i++)
                {
                    preview[i] = recorder.modifier[i];
                }
                for (int i = 0; i < 4; i++)
                {
                    EventProto proto = EventSystem.protos[recorder.protoId];
                    if (i >= proto.decisionLen)
                        break;
                    if (decisionUIButtons[i] != null && decisionUIButtons[i].isPointerEnter)
                    {
                        int resultLen = proto.decisionResultId[i].Length;
                        for (int j = 0; j < resultLen; j++)
                        {
                            int code = proto.decisionResultId[i][j];
                            if (code >= 4 && code <= 9) // 说明是改动
                            {
                                int amount = proto.decisionResultCount[i][j];
                                if (code == 4)
                                    preview[3] += amount;
                                else if (code == 5)
                                    preview[2] += amount;
                                else if (code == 6)
                                    preview[1] += amount;
                                else if (code == 7)
                                    preview[0] += amount;
                                else if (code == 8)
                                    preview[4] += amount;
                                else if (code == 9)
                                    preview[5] += amount;
                            }
                        }
                    }
                }
                if (Relic.GetRelicCount() >= 8)
                {
                    probabilityTextCursed.text = "<color=#00c280>◈ --</color>";
                    probabilityTextLegend.text = "<color=#d2853d>◈ --</color>";
                    probabilityTextEpic.text = "<color=#8040d0>◈ --</color>";
                    probabilityTextRare.text = "<color=#2070d0>◈ --</color>";
                    probabilityTextCommon.text = "<color=#30b330>◈ --</color>";
                    probabilityTextReroll.text = $"<color=#00adffa8>↻  {preview[5] + 1}</color>";
                }
                else
                {
                    double rand = Utils.RandDouble();
                    double[] probWeight = Relic.HaveRelic(0, 9) ? Relic.relicTypeProbabilityBuffed : Relic.relicTypeProbability;
                    // relic0-9 五叶草 可以让更高稀有度的遗物刷新概率提高
                    double[] realWeight = new double[] { 0, 0, 0, 0, 0 };
                    for (int type = 0; type < 5; type++)
                    {
                        realWeight[type] = probWeight[type] * (1 + 0.01 * preview[type] + (type == 0 ? SkillPoints.relic0WeightBuff : 0) + (type == 1 ? SkillPoints.relic1WeightBuff : 0));
                        if (realWeight[type] < 0)
                            realWeight[type] = 0;
                    }
                    double[] prob = new double[] { 0, 0, 0, 0, 0 };
                    double weightSum = realWeight.Sum();
                    for (int type = 0; type < 5; type++)
                    {
                        prob[type] = realWeight[type] / weightSum;
                    }

                    //probabilityText.text = string.Format("<color=#00c280>◈ {0:#.0}%</color>     <color=#d2853d>◈ {1:#.0}%</color>     <color=#8040d0>◈ {2:##}%</color>     <color=#2070d0>◈ {3:##}%</color>     <color=#30b330>◈ {4:##}%</color>     <color=#00adffa8>↻ {5:0}</color>", prob[4] * 100.0, prob[0] * 100.0, prob[1] * 100.0, prob[2] * 100.0, prob[3] * 100.0, preview[5] + 1);
                    probabilityTextCursed.text = string.Format("<color=#00c280>◈ {0:#.0}%</color>", prob[4] * 100);
                    probabilityTextLegend.text = string.Format("<color=#d2853d>◈ {0:#.0}%</color>", prob[0] * 100);
                    probabilityTextEpic.text = string.Format("<color=#8040d0>◈ {0:#.0}%</color>", prob[1] * 100);
                    probabilityTextRare.text = string.Format("<color=#2070d0>◈ {0:##}%</color>", prob[2] * 100);
                    probabilityTextCommon.text = string.Format("<color=#30b330>◈ {0:##}%</color>", prob[3] * 100);
                    probabilityTextReroll.text = $"<color=#00adffa8>↻  {preview[5] + 1}</color>";

                }
            }
            closeButtonText.text = "一";
        }

        public static void InitWhenLoad()
        {
            ESUIButton.tips.tipTitle = "打开解译事件链".Translate();
        }

        public static void SetDecisionButtonDisabledColor(int index)
        {

        }
        public static void SetDecisionButtonReadyColor(int index)
        {

        }
        public static void SetDecisionButtonWarnColor(int index)
        {

        }
    }
}
