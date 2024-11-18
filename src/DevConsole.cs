using CommonAPI.Systems;
using MoreMegaStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    public class StrNode
    {
        public string cmd;
        public StrNode prev;
        public StrNode next;
        public StrNode(string cmd)
        {
            this.cmd = cmd;
            this.prev = null;
            this.next = null;
        }
        public StrNode(string cmd, StrNode prev)
        {
            this.cmd = cmd;
            this.prev = prev;
            this.next = null;
        }
    }

    public class DevConsole
    {
        public static int num = 0;
        public static int password = 1597531;
        public static int shortpassword = 1597531;

        public static StrNode cur = null;
        public static StrNode last = null;
        public static StrNode root = null;
        public static int lineCount = 0;
        public static int maxLineCount = 100;

        public static void InitAll()
        {
            UIDevConsole.InitAll();
            InitData();
        }

        public static void InitData()
        {

        }

        public static void Update()
        {
            if (num >= 200000000)
                num = 0;
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(KeyCode.Keypad0 + i))
                {
                    num *= 10;
                    num += i;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (num == password || num == shortpassword)
                {
                    UIDevConsole.Show();
                    if (shortpassword != 123) // 第一次打开自动输出所有命令说明
                        UIDevConsole.ShowAllCommands();
                    shortpassword = 123;
                }
                num = 0;
            }
        }

        public static void OnInputFieldValueChange(string cmd)
        {
            if (cmd.Length <= 0) return;

            char last = cmd.Last<char>();
            if (last == '\n')
            {
                cmd = cmd.Trim('\n').Trim(' ');
                if (cmd.Length > 0)
                {
                    ExecuteCommand(cmd);
                }
                UIDevConsole.ClearInputField();
            }
            else
            {
                string final = "";
                bool execute = false;
                foreach (var item in cmd)
                {
                    if (item == '\n')
                        execute = true;
                    else
                        final += item;
                }
                if (execute)
                {
                    ExecuteCommand(final);
                    UIDevConsole.ClearInputField();
                }
            }
        }

        public static void ExecuteCommand(string cmd)
        {
            RegNewLine(cmd);
            Print("<color=#ffffff>>>" + cmd + "</color>");
            string[] param = cmd.Split(' '); // 已确定cmd必定不为空
            try
            {
                param[0] = param[0].ToLower();
                switch (param[0])
                {
                    case "h":
                    case "help":
                        UIDevConsole.ShowAllCommands(1);
                        break;
                    case "h2":
                    case "help2":
                        UIDevConsole.ShowAllCommands(2);
                        break;
                    case "c":
                    case "clr":
                    case "clear":
                        UIDevConsole.ClearOutputField();
                        break;
                    case "cur":
                        int sidx = GameMain.data.localStar != null ? GameMain.data.localStar.index : -1;
                        int pid = GameMain.data.localPlanet != null ? GameMain.data.localPlanet.id : -1;
                        Print($"StarIndex = {sidx}, StarId = {(sidx >= 0 ? sidx + 1 : sidx)}, PlanetId = {pid}");
                        break;
                    case "setmega":
                        int idx = Convert.ToInt32(param[1]);
                        int type = Convert.ToInt32(param[2]);
                        MoreMegaStructure.MoreMegaStructure.StarMegaStructureType[idx] = type;
                        if (MoreMegaStructure.MoreMegaStructure.curStar != null)
                            MoreMegaStructure.MoreMegaStructure.RefreshUILabels(MoreMegaStructure.MoreMegaStructure.curStar);
                        if (type == 4)
                            StarAssembly.ResetInGameDataByStarIndex(idx);
                        Print($"Set megastructure type in starIndex {idx} to type {type}.");
                        break;
                    case "setsf":
                        int idx3 = Convert.ToInt32(param[1]);
                        int type3 = Convert.ToInt32(param[2]);
                        //if (type3 < 0 || type3 > 3) type3 = 2;
                        int num3 = Convert.ToInt32(param[3]);
                        int cnum3 = num3 * StarFortress.compoPerModule[type3]; // 这里会因为越界或者负数的索引抛出异常，防止后面的字典加入无效的数据
                        StarFortress.moduleComponentCount[idx3].AddOrUpdate(type3, cnum3, (x, y) => cnum3);
                        StarFortress.moduleMaxCount[idx3][type3] = Math.Max(StarFortress.moduleMaxCount[idx3][type3], num3);
                        if (StarFortress.CapacityRemaining(idx3) < 0)
                        {
                            int addModule2 = -StarFortress.CapacityRemaining(idx3);
                            StarFortress.moduleComponentCount[idx3].AddOrUpdate(2, addModule2 * StarFortress.compoPerModule[2], (x, y) => y + addModule2 * StarFortress.compoPerModule[2]);
                            StarFortress.moduleMaxCount[idx3][2] = Math.Max(StarFortress.moduleMaxCount[idx3][2], (StarFortress.moduleComponentCount[idx3].GetOrAdd(2, 0) + addModule2) / StarFortress.compoPerModule[2]);
                        }
                        Print($"Set starIndex {idx3} star fortress module{type3} built count to {num3}.");
                        break;
                    case "setrank":
                        int target = Math.Min(Math.Max(Convert.ToInt32(param[1]), 0), 10);
                        if (target > Rank.rank)
                        {
                            for (int i = Rank.rank; i < target; i++)
                            {
                                Rank.AddExp(Configs.expToNextRank[i]);
                            }
                        }
                        else
                        {
                            while (Rank.rank > target)
                            {
                                Rank.DownGrade();
                            }
                        }
                        Print($"Rank set to {Math.Min(Math.Max(Convert.ToInt32(param[1]), 0), 10)}");
                        break;
                    case "addexp":
                        Rank.AddExp(Convert.ToInt32(param[1]));
                        Print($"Add exp {param[1]}");
                        break;
                    case "newrelic":
                        if (param.Length > 1)
                        {
                            UIRelic.forceType = Convert.ToInt32(param[1]);
                            if (param.Length > 2)
                            {
                                UIRelic.forceNum = Convert.ToInt32(param[2]);
                            }
                        }
                        Relic.PrepareNewRelic();
                        Print($"Prepare new relics.");
                        break;
                    case "addrelic":
                        int type4 = Convert.ToInt32(param[1]);
                        int num4 = Convert.ToInt32(param[2]);
                        Relic.AddRelic(type4, num4);
                        UIRelic.RefreshSlotsWindowUI();
                        Print($"Add relic " + ("遗物名称" + type4.ToString() + "-" + num4.ToString()).Translate().Split('\n')[0]);
                        break;
                    case "rmrelic":
                        int type5 = Convert.ToInt32(param[1]);
                        int num5 = Convert.ToInt32(param[2]);
                        if (Relic.HaveRelic(type5, num5))
                            Relic.RemoveRelic(type5, num5);
                        UIRelic.RefreshSlotsWindowUI();
                        Print($"Remove relic " + ("遗物名称" + type5.ToString() + "-" + num5.ToString()).Translate().Split('\n')[0]);
                        break;
                    case "lsrelic":
                        string allRelicNames = "";
                        for (int i = 0; i < 5; i++)
                        {
                            if (i == 0)
                                allRelicNames += "<color=#ffa03d>";
                            else if (i == 1)
                                allRelicNames += "<color=#d060ff>";
                            else if (i == 2)
                                allRelicNames += "<color=#20c0ff>";
                            else if (i == 3)
                                allRelicNames += "<color=#30ff30>";
                            else if (i == 4)
                                allRelicNames += "<color=#00c560>";
                            for (int j = 0; j < Relic.relicNumByType[i]; j++)
                            {
                                allRelicNames += $"{i}-{j}:{($"遗物名称{i}-{j}").Translate().Split('\n')[0]}    ";
                            }
                            allRelicNames += "</color>";
                            if (i < 4)
                                allRelicNames += "\n";
                        }
                        Print(allRelicNames, 10);
                        break;
                    case "give":
                        GameMain.mainPlayer.TryAddItemToPackage(Convert.ToInt32(param[1]), Convert.ToInt32(param[2]), 0, true);
                        Print($"Add {param[2]} {LDB.items.Select(Convert.ToInt32(param[1]))?.Name.Translate()} to mecha storage.");
                        break;
                    case "cool":
                        if (MoreMegaStructure.StarCannon.time < 0)
                            MoreMegaStructure.StarCannon.time = 0;
                        Print($"Star cannon cool down.");
                        break;
                    case "dev":
                        Configs.developerMode = true;
                        Print($"Developer Mode True.");
                        break;
                    case "ndev":
                        Configs.developerMode = false;
                        Print($"Developer Mode False.");
                        break;
                    case "g":
                        Relic.relic0_2Charge = Convert.ToInt32(param[1]);
                        Relic.relic0_2CanActivate = 1;
                        UIRelic.RefreshTearOfGoddessSlotTips();
                        break;
                    case "es":
                        if (param.Length == 1)
                        {
                            EventSystem.InitNewEvent();
                            Print($"Init new event.");
                        }
                        else
                        {
                            EventSystem.SetEvent(Convert.ToInt32(param[1]));
                            Print($"Set event id to {param[1]}.");
                        }
                        break;
                    case "est":
                        EventSystem.TransferTo(Convert.ToInt32(param[1]));
                        Print($"Transfer event to {param[1]}.");
                        break;
                    case "esf":
                        EventSystem.recorder.decodeTimeSpend = EventSystem.recorder.decodeTimeNeed;
                        Print($"Event count down finished.");
                        break;
                    case "ap":
                        int apAdd = Convert.ToInt32(param[1]);
                        SkillPoints.totalPoints += apAdd;
                        Print($"Authorization points {(apAdd >= 0 ? "+" : "")}{apAdd}");
                        break;
                    case "t":
                        Utils.beginBasic = Convert.ToDouble(param[1]);
                        Utils.minBasic = Convert.ToDouble(param[2]);
                        Utils.dec = Convert.ToDouble(param[3]);
                        Print($"ok.");
                        break;
                    case "probtick":
                        EventSystem.tickFromLastRelic = Convert.ToInt32(param[1]);
                        Print($"ok.");
                        break;
                    case "voidon":
                        AssaultController.voidInvasionEnabled = true;
                        Print("Void assault enabled");
                        break;
                    case "voidoff":
                        AssaultController.voidInvasionEnabled = false;
                        Print("Void assault disabled.");
                        break;


                    //  
                    case "scan":

                        List<int> res = new List<int>();
                        string resStr = "";
                        if (param.Length == 1)// || param[1] == "hive" || param[1] == "h")
                        {
                            Print("Found hives in players star system");
                            int starIndex = GameMain.data.localStar != null ? GameMain.data.localStar.index : -1;
                            SpaceSector sector = GameMain.spaceSector;
                            EnemyDFHiveSystem[] dfHivesByAstro = GameMain.data.spaceSector.dfHivesByAstro;
                            for (int i = 1; i < sector.enemyCursor; i++)
                            {
                                int oriAstroId = sector.enemyPool[i].originAstroId;
                                int index = oriAstroId - 1000000;
                                if (index >= 0 && index < dfHivesByAstro.Length)
                                {
                                    if (!res.Contains(oriAstroId) && dfHivesByAstro[oriAstroId - 1000000]?.starData?.index == starIndex)
                                        res.Add(oriAstroId);
                                }
                            }
                        }
                        if (res.Count > 0)
                        {
                            foreach (int r in res)
                            {
                                resStr += r.ToString() + "   ";
                            }
                        }
                        else
                        {
                            resStr += "Nothing";
                        }
                        Print(resStr);
                        break;
                    case "qhive":
                    case "qthive":
                    case "qbhive":
                        AssaultController.quickTickHive = Convert.ToInt32(param[1]);
                        int factor = 10;
                        if (param.Length > 2)
                            factor = Convert.ToInt32(param[2]);
                        AssaultController.quickTickFactor = factor;
                        Print($"set hive {param[1]} to quick tick mode to {factor * 60}x speed");
                        break;
                    case "mkhive":
                        EnemyDFHiveSystem hive = null;
                        if (param.Length > 2)
                        {
                            AssaultHive ah = new AssaultHive(Convert.ToInt32(param[1]), Convert.ToInt32(param[2]), 0);
                            hive = ah.hive;
                        }
                        if (hive != null)
                        {
                            Print($"Created hive in star system {hive.starData.index}");
                        }
                        else
                            Print("Failed to create hive.");
                        break;
                    case "newhive":
                        EnemyDFHiveSystem hive2 = null;
                        if(param.Length > 1)
                            hive2 = AssaultController.TryCreateNewHiveAndCore(Convert.ToInt32(param[1]));
                        else if(GameMain.data.localStar != null)
                            hive2 = AssaultController.TryCreateNewHiveAndCore(GameMain.data.localStar.index);
                        if (hive2 != null)
                        {
                            Print($"Created hive in star system {hive2.starData.index}");
                        }
                        else
                            Print("Failed to create hive.");
                        break;
                    case "checkhive":
                        int starIndex2 = -1;
                        if (param.Length > 1)
                            starIndex2 =Convert.ToInt32(param[1]);
                        else if (GameMain.data.localStar != null)
                            starIndex2 = GameMain.data.localStar.index;
                        if(starIndex2 >= 0)
                        {
                            for (int i = 0; i < 9; i++)
                            {
                                EnemyDFHiveSystem hive3 = GameMain.spaceSector.dfHivesByAstro[starIndex2 * 8 + i];
                                if(hive3 != null)
                                {
                                    Print($"i = {i}, star is {hive3.starData.index}");
                                }
                            }
                            AssaultController.CheckHiveStatus(starIndex2);
                        }
                        break;
                    case "buildhive":
                        AssaultController.BuildHiveAlreadyInited(Convert.ToInt32(param[1]));
                        break;
                    case "tlvl":
                        AssaultController.testLvlSet = Convert.ToInt32(param[1]);
                        Print($"set lvl to {param[1]} once");
                        break;
                    case "god":
                        DspBattlePlugin.playerInvincible = true;
                        Print($"god mode on");
                        break;
                    case "ngod":
                        DspBattlePlugin.playerInvincible = false;
                        Print($"god mode off");
                        break;
                    case "mpstat":
                        Print($"MP.NebulaEnabled={MP.NebulaEnabled}, MP.clientBlocker={MP.clientBlocker}");
                        break;
                    default:
                        Print($"未知的命令：{param[0]}，输入 \"help\" 查看所有命令说明。", 1, true);
                        break;
                }
            }
            catch (Exception)
            {
                Print($"命令 \"{param[0]}\" 具有非法参数，请检查参数数量、参数数值可能导致的数组越界问题。输入 \"help\" 查看所有命令说明。", 1, true);
            }

        }

        public static void Print(string msg, int forceLineCount = 1, bool err = false)
        {
            UIDevConsole.Print(msg, forceLineCount, err);
        }
        public static void RegNewLine(string command)
        {
            if (root == null)
            {
                root = new StrNode(command);
                last = root;
                lineCount = 1;
            }
            else if (last == null)
            {
                Utils.Log("Dev console err with lastNode is null. Now ReInit");
                root = new StrNode(command);
                last = root;
                lineCount = 1;
            }
            else
            {
                last.next = new StrNode(command, last);
                last = last.next;
                lineCount++;
            }
            cur = null;

            if (lineCount > maxLineCount)
            {
                root = root.next;
                root.prev = null;
                lineCount--;
                GC.Collect();
            }
        }

        public static void PrevCommand()
        {
            if (cur == null)
            {
                if (last == null)
                    return;
                cur = last;
            }
            else if (UIDevConsole.consoleInputField.text == "")
            {
                // 不做任何事
            }
            else if (cur.prev != null)
                cur = cur.prev;
            else
                return;

            UIDevConsole.consoleInputField.text = cur.cmd;
            UIDevConsole.consoleInputField.caretPosition = UIDevConsole.consoleInputField.text.Length;
        }

        public static void NextCommand()
        {
            if (cur == null)
                return;
            else if (cur.next == null)
            {
                if (UIDevConsole.consoleInputField.text == cur.cmd)
                {
                    UIDevConsole.consoleInputField.text = "";
                    UIDevConsole.consoleInputField.caretPosition = UIDevConsole.consoleInputField.text.Length;
                }
                return;
            }
            cur = cur.next;

            UIDevConsole.consoleInputField.text = cur.cmd;
            UIDevConsole.consoleInputField.caretPosition = UIDevConsole.consoleInputField.text.Length;
        }

        public static void Export(BinaryWriter w)
        {
        }

        public static void Import(BinaryReader r)
        {
            InitAll();
        }

        public static void IntoOtherSave()
        {
            InitAll();
        }
    }

    public class UIDevConsole
    {
        public static GameObject consoleObj = null;
        public static string consoleHelpTip = "Use command \"help\" to view all commands.";
        public static InputField consoleInputField = null;
        public static InputField consoleOutputField = null;
        public static GameObject outputFieldObj = null;
        public static int outputClearCount = 0;
        public static int maxOutputClearCount = 23;
        public static void InitAll()
        {
            if (consoleObj == null)
            {
                GameObject oriBlueprintPanelObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Blueprint Browser");
                //consoleObj = GameObject.Instantiate(GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Research Result Window/"));
                GameObject parentObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows");
                //consoleObj = new GameObject();
                consoleObj = GameObject.Instantiate(oriBlueprintPanelObj) as GameObject;
                consoleObj.name = "tcfv-dev-console";

                GameObject.Destroy(consoleObj.transform.Find("view-group").gameObject);
                GameObject.Destroy(consoleObj.transform.Find("inspector-group-bg").gameObject);
                GameObject.Destroy(consoleObj.transform.Find("inspector-group").gameObject);
                GameObject.Destroy(consoleObj.transform.Find("folder-info-group").gameObject);
                GameObject.Destroy(consoleObj.transform.Find("title-group").gameObject);
                GameObject.Destroy(consoleObj.transform.Find("panel-bg/title-text").gameObject);
                GameObject.Destroy(consoleObj.transform.Find("panel-bg/x").gameObject);

                consoleObj.transform.SetParent(parentObj.transform);
                consoleObj.transform.localScale = new Vector3(1, 1, 1);
                consoleObj.transform.localPosition = new Vector3(0, 0);
                consoleObj.AddComponent<Image>();
                consoleObj.GetComponent<Image>().color = new Color(0f, 0.096f, 0.32f, 0.8f);
                consoleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 600);
                //consoleObj.AddComponent<UIWindowDrag>();
                consoleObj.GetComponent<UIWindowDrag>().refTrans = consoleObj.GetComponent<RectTransform>();
                consoleObj.GetComponent<UIWindowDrag>().dragTrans = consoleObj.GetComponent<RectTransform>();

                GameObject inputBgObj = new GameObject();
                inputBgObj.name = "input-bg";
                inputBgObj.transform.SetParent(consoleObj.transform);
                inputBgObj.AddComponent<Image>();
                inputBgObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.15f);
                inputBgObj.GetComponent<RectTransform>().sizeDelta = new Vector2(780, 27);
                inputBgObj.transform.localScale = new Vector3(1, 1, 1);
                inputBgObj.transform.localPosition = new Vector3(0, -278, 0);

                GameObject oriInputFieldObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Blueprint Browser/inspector-group/Scroll View/Viewport/Content/group-1/input-short-text");
                bool BPTEnabled = false;
                if (oriInputFieldObj == null)
                {
                    BPTEnabled = true;
                    oriInputFieldObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Blueprint Browser/inspector-group/BP-panel-scroll(Clone)/Viewport/pane/group-1/input-desc-text");
                    maxOutputClearCount = 25;
                }
                GameObject inputFieldObj = GameObject.Instantiate(oriInputFieldObj, consoleObj.transform);
                inputFieldObj.name = "inputfield";
                inputFieldObj.GetComponent<UIButton>().tips.tipTitle = "Command";
                inputFieldObj.GetComponent<UIButton>().tips.tipText = consoleHelpTip;
                inputFieldObj.transform.localScale = new Vector3(1, 1, 1);
                inputFieldObj.transform.localPosition = new Vector3(-390, -265);
                inputFieldObj.GetComponent<RectTransform>().sizeDelta = new Vector2(780, 26);
                if(BPTEnabled)
                    inputFieldObj.GetComponent<RectTransform>().sizeDelta = new Vector2(-20, 26);
                inputFieldObj.GetComponent<InputField>().lineType = InputField.LineType.MultiLineNewline;
                inputFieldObj.transform.Find("value-text").GetComponent<Text>().color = Color.white;
                //inputFieldObj.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                consoleInputField = inputFieldObj.GetComponent<InputField>();
                consoleInputField.onValueChange.RemoveAllListeners();
                consoleInputField.onValueChange.AddListener((x) => { DevConsole.OnInputFieldValueChange(x); });

                GameObject oriCloseXMarkObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Blueprint Browser/panel-bg/x");
                GameObject XObj = GameObject.Instantiate(oriCloseXMarkObj, consoleObj.transform);
                XObj.name = "x";
                XObj.GetComponent<Button>().onClick.RemoveAllListeners();
                XObj.GetComponent<Button>().onClick.AddListener(() => { Hide(); });
                XObj.transform.localScale = new Vector3(1, 1, 1);
                XObj.transform.localPosition = new Vector3(390, 290);

                outputFieldObj = GameObject.Instantiate(oriInputFieldObj, consoleObj.transform);
                outputFieldObj.name = "outputfield";
                outputFieldObj.GetComponent<UIButton>().tips.delay = 999999;
                outputFieldObj.GetComponent<UIButton>().tips.tipTitle = "Outputs";
                outputFieldObj.GetComponent<UIButton>().tips.tipText = "This console is only for mod devs.";
                outputFieldObj.transform.localScale = new Vector3(1, 1, 1);
                outputFieldObj.transform.localPosition = new Vector3(-390, 265);
                outputFieldObj.GetComponent<RectTransform>().sizeDelta = new Vector2(780, 500);
                if (BPTEnabled)
                    outputFieldObj.GetComponent<RectTransform>().sizeDelta = new Vector2(-20, 500);
                outputFieldObj.GetComponent<InputField>().lineType = InputField.LineType.MultiLineNewline;
                outputFieldObj.transform.Find("value-text").GetComponent<Text>().supportRichText = true;
                outputFieldObj.transform.Find("value-text").GetComponent<Text>().alignment = TextAnchor.LowerLeft;
                outputFieldObj.transform.Find("value-text").GetComponent<Text>().color = new Color(0.1875f, 0.8125f, 1f);
                consoleOutputField = outputFieldObj.GetComponent<InputField>();
                consoleOutputField.onValueChange.RemoveAllListeners();
                consoleOutputField.interactable = false;
                consoleOutputField.characterLimit = 5000;

                outputClearCount = 0;
            }

            Hide();
        }

        public static void Show()
        {
            if (consoleObj != null)
            {
                consoleObj.SetActive(true);
                consoleInputField.ActivateInputField();
                outputFieldObj.SetActive(true);
            }
        }

        public static void Hide()
        {
            if (consoleObj != null)
            {
                consoleObj.SetActive(false);
            }
        }

        public static bool EscLogic()
        {
            if (consoleObj == null)
                return false;
            if (consoleObj.activeSelf)
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
        public static void ClearInputField()
        {
            consoleInputField.text = "";
        }

        public static void Print(string msg, int forceLineCount = 1, bool err = false)
        {
            outputClearCount += forceLineCount;
            if (outputClearCount > maxOutputClearCount)
            {
                string[] old = consoleOutputField.text.Trim('\n').Split('\n');
                if (old.Length > 1)
                {
                    string newout = "";
                    int begin = Math.Min(outputClearCount - maxOutputClearCount, old.Length);
                    for (int i = begin; i < old.Length; i++)
                    {
                        newout += old[i] + "\n";
                    }
                    consoleOutputField.text = newout;
                    outputClearCount = maxOutputClearCount;
                }
                else
                {
                    ClearOutputField();
                }
            }
            if (err)
            {
                consoleOutputField.text = consoleOutputField.text + "<color=#ff2020>" + msg + "</color>\n";
            }
            else
            {
                consoleOutputField.text = consoleOutputField.text + msg + "\n";
            }
        }

        public static void ShowAllCommands(int page = 1)
        {
            string allCmds = "---------------------- help ----------------------" + "\n" +
                "<color=#ffffff>h</color>或<color=#ffffff>help</color> 输出所有可用命令" + "\n" +
                "<color=#ffffff>c</color>或<color=#ffffff>clear</color> 清空输出缓存" + "\n" +
                "<color=#ffffff>cur</color> 输出伊卡洛斯所在星系的starId和starIndex，并输出伊卡洛斯所在行星的planetId" + "\n" +
                "<color=#ffffff>setmega [param1] [param2]</color> 立刻将星系index为[param1]的巨构类型设置为[param2]" + "\n" +
                //"<color=#ffffff>setsf [param1] [param2] [param3]</color>  立刻将星系index为[param1]的恒星要塞第[param2]个模块已建成数量设置为[param3]" + "\n" +
                "<color=#ffffff>setrank [param1]</color> 将功勋等级设置为[param1]，改变等级后还会使经验降低至0" + "\n" +
                "<color=#ffffff>addexp [param1]</color> 增加[param1]经验，可升级，也可为负但不会降级" + "\n" +
                "<color=#ffffff>newrelic ([param1]) ([param2])</color> 立刻随机并打开选择元驱动窗口，可选参数可强制在中间刷新某个特定稀有度，或特定元驱动" + "\n" +
                "<color=#ffffff>addrelic [param1] [param2]</color> 立刻获得第[param1]类型第[param2]号元驱动" + "\n" +
                "<color=#ffffff>rmrelic [param1] [param2]</color> 立刻删除第[param1]类型第[param2]号元驱动（如果已经拥有）" + "\n" +
                "<color=#ffffff>lsrelic</color> 展示所有元驱动的名称" + "\n" +
                "<color=#ffffff>give [param1] [param2]</color> 立刻给予[param2]个itemId为[param1]的物品" + "\n" +
                "<color=#ffffff>cool</color> 恒星炮立即冷却完毕" + "\n" +
                "<color=#ffffff>es [param1]</color> 设定当前事件链为[param1]，不提供参数则初始化合法的事件" + "\n" +
                "<color=#ffffff>est [param1]</color> 当前事件链转移至[param1]" + "\n" +
                "<color=#ffffff>ap [param1]</color> 授权点增加[param1]" + "\n" +
                "<color=#ffffff>voidon</color> 开启虚空入侵" + "\n" +
                "<color=#ffffff>voidoff</color> 禁止虚空入侵" + "\n" +
                "<color=#ffffff>mpstat</color> 查看联机相关参数" + "\n" +
                "---------------------- help ----------------------";
            Print(allCmds, 20, false); // 这个forceLineCount传值取决于allCmds的行数
        }

        public static void ClearOutputField()
        {
            consoleOutputField.text = "";
            outputClearCount = 0;
        }
    }
}
