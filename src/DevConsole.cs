using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using MoreMegaStructure;
using System.Threading;

namespace DSP_Battle
{
    public class DevConsole
    {
        public static int num = 0;
        public static int password = 1597531;
        public static int shortpassword = 1597531;

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
            if(num>=200000000)
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
        }

        public static void ExecuteCommand(string cmd)
        {
            Print("<color=#ffffff>>>" + cmd + "</color>");
            string[] param = cmd.Split(' '); // 已确定cmd必定不为空
            try
            {
                param[0] = param[0].ToLower();
                switch(param[0])
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
                    case "settime":
                        if (Configs.nextWaveState == 0)
                        {
                            Configs.nextWaveFrameIndex = GameMain.instance.timei - 60 * 60;
                            Print($"Now generate next wave.");
                        }
                        else
                        {
                            Configs.nextWaveFrameIndex = (long)(60 * 60 * Convert.ToDouble(param[1])) + GameMain.instance.timei;
                            Print($"Next wave begins in {param[1]} minute(s).");
                        }
                        break;
                    case "setstar":
                        Configs.nextWaveStarIndex = Convert.ToInt32(param[1]);
                        Print($"Next wave starIndex set to {param[1]}.");
                        break;
                    case "setelite":
                        Configs.nextWaveElite = 1;
                        Print("Next wave is now elite wave.");
                        break;
                    case "nsetelite":
                        Configs.nextWaveElite = 0;
                        Print("Next wave is now normal wave.");
                        break;
                    case "setstg":
                        Configs.nextWaveIntensity = Convert.ToInt32(param[1]);
                        Print($"Next wave intensity is set to {param[1]}.");
                        break;
                    case "seteasy":
                        Configs.difficulty = -1;
                        Print("Easy mode.");
                        break;
                    case "setnorm":
                        Configs.difficulty = 0;
                        Print("Normal mode.");
                        break;
                    case "sethard":
                        Configs.difficulty = 1;
                        Print("Hard mode.");
                        break;
                    case "setshield":
                        int planetId = Convert.ToInt32(param[1]);
                        int result = Convert.ToInt32(param[2]);
                        ShieldGenerator.currentShield.AddOrUpdate(planetId, result, (x, y) => result);
                        Print($"Shield of planet {planetId} is set to {result}.");
                        break;
                    case "setmega":
                        int idx = Convert.ToInt32(param[1]);
                        int type = Convert.ToInt32(param[2]);
                        MoreMegaStructure.MoreMegaStructure.StarMegaStructureType[idx] = type;
                        if(MoreMegaStructure.MoreMegaStructure.curStar!=null)
                            MoreMegaStructure.MoreMegaStructure.RefreshUILabels(MoreMegaStructure.MoreMegaStructure.curStar);
                        if (type == 4)
                            StarAssembly.ResetInGameDataByStarIndex(idx);
                        Print($"Set megastructure type in starIndex {idx} to type {type}.");
                        break;
                    case "setwavenum":
                        int idx2 = Convert.ToInt32(param[1]);
                        int num2 = Convert.ToInt32(param[2]);
                        Configs.wavePerStar[idx2] = num2;
                        Print($"Wave count of starIndex {idx2} is set to {num2}.");
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
                            StarFortress.moduleMaxCount[idx3][2] = Math.Max(StarFortress.moduleMaxCount[idx3][2], (StarFortress.moduleComponentCount[idx3].GetOrAdd(2, 0) + addModule2)/ StarFortress.compoPerModule[2]);
                        }
                        Print($"Set starIndex {idx3} star fortress module{type3} built count to {num3}.");
                        break;
                    case "setrank":
                        Rank.rank = Math.Min(Math.Max(Convert.ToInt32(param[1]), 0), 10);
                        Interlocked.Exchange(ref Rank.exp, 0);
                        Print($"Rank set to {Math.Min(Math.Max(Convert.ToInt32(param[1]), 0), 10)}");
                        break;
                    case "addexp":
                        Rank.AddExp(Convert.ToInt32(param[1]));
                        Print($"Add exp {param[1]}");
                        break;
                    case "newrelic":
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
                        if(Relic.HaveRelic(type5, num5))
                            Relic.relics[type5] = Relic.relics[type5] ^ 1 << num5;
                        UIRelic.RefreshSlotsWindowUI();
                        Print($"Remove relic " + ("遗物名称" + type5.ToString() + "-" + num5.ToString()).Translate().Split('\n')[0]);
                        break;
                    case "kill":
                    case "killall":
                        if (Configs.nextWaveState == 3)
                        {
                            foreach (EnemyShip ship in EnemyShips.ships.Values)
                            {
                                ship.BeAttacked(9999999);
                            }
                        }
                        Print($"Kill all remaining enemies.");
                        break;
                    case "am":
                        GameMain.mainPlayer.TryAddItemToPackage(8032, Convert.ToInt32(param[1]), 0, true);
                        Print($"Add {param[1]} alien matrixes to mecha storage.");
                        break;
                    case "give":
                        GameMain.mainPlayer.TryAddItemToPackage(Convert.ToInt32(param[1]), Convert.ToInt32(param[2]), 0, true);
                        Print($"Add {param[2]} {LDB.items.Select(Convert.ToInt32(param[1]))?.Name.Translate()} to mecha storage.");
                        break;
                    case "cool":
                        if(StarCannon.time < 0)
                            StarCannon.time = 0;
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
        public static int outputClearCount = 0;
        public static int maxOutputClearCount = 25;
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

                GameObject oriInputFieldObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Blueprint Browser/inspector-group/group-1/input-desc-text");
                GameObject inputFieldObj = GameObject.Instantiate(oriInputFieldObj, consoleObj.transform);
                inputFieldObj.name = "inputfield";
                inputFieldObj.GetComponent<UIButton>().tips.tipTitle = "Command";
                inputFieldObj.GetComponent<UIButton>().tips.tipText = consoleHelpTip;
                inputFieldObj.transform.localScale = new Vector3(1, 1, 1);
                inputFieldObj.transform.localPosition = new Vector3(-390, -265);
                inputFieldObj.GetComponent<RectTransform>().sizeDelta = new Vector2(-20, 26);
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

                GameObject outputFieldObj = GameObject.Instantiate(oriInputFieldObj, consoleObj.transform);
                outputFieldObj.name = "outputfield";
                outputFieldObj.GetComponent<UIButton>().tips.delay = 999999;
                outputFieldObj.GetComponent<UIButton>().tips.tipTitle = "Outputs";
                outputFieldObj.GetComponent<UIButton>().tips.tipText = "This console is only for mod devs.";
                outputFieldObj.transform.localScale = new Vector3(1, 1, 1);
                outputFieldObj.transform.localPosition = new Vector3(-390, 265);
                outputFieldObj.GetComponent<RectTransform>().sizeDelta = new Vector2(-20, 500);
                outputFieldObj.transform.Find("value-text").GetComponent<Text>().supportRichText = true;
                outputFieldObj.transform.Find("value-text").GetComponent<Text>().alignment = TextAnchor.LowerLeft;
                outputFieldObj.transform.Find("value-text").GetComponent<Text>().color = new Color(0.1875f,0.8125f,1f);
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
                consoleObj.SetActive(true);
        }

        public static void Hide()
        {
            if (consoleObj != null)
                consoleObj.SetActive(false);
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
                "<color=#ffffff>cur</color> 输出伊卡洛斯所在星系的starId和starIndex，并输入伊卡洛斯所在行星的planetId" + "\n" +
                "<color=#ffffff>settime [param1]</color> 设定下次进攻时间为[param1]分钟后开始，若尚未有下次进攻，则立刻生成下次进攻但保留默认进攻的预留时间" + "\n" +
                "<color=#ffffff>setstar [param1]</color> 更改下次进攻的恒星系为[param1]，若尚未有下次进攻则命令无效" + "\n" +
                "<color=#ffffff>setelite</color>或<color=#ffffff>nsetelite</color> 设定下次进攻为或不为精英波次" + "\n" +
                "<color=#ffffff>setstg [param1]</color> 设定下次进攻强度为[param1]" + "\n" +
                "<color=#ffffff>seteasy</color>或<color=#ffffff>setnorm</color>或<color=#ffffff>sethard</color> 设定游戏难度为简单、普通或困难" + "\n" +
                "<color=#ffffff>setshield [param1] [param2]</color> 将planetId为[param1]的星球的护盾值设置为[param2]" + "\n" +
                "<color=#ffffff>setmega [param1] [param2]</color> 立刻将星系index为[param1]的巨构类型设置为[param2]" + "\n" +
                "<color=#ffffff>setwavenum [param1] [param2]</color> 立刻将星系index为[param1]的恒星进攻波次计数设定为非负整数[param2]" + "\n" +
                "<color=#ffffff>setsf [param1] [param2] [param3]</color>  立刻将星系index为[param1]的恒星要塞第[param2]个模块已建成数量设置为[param3]" + "\n" +
                "<color=#ffffff>setrank [param1]</color> 将功勋等级设置为[param1]，改变等级后还会使经验降低至0" + "\n" +
                "<color=#ffffff>addexp [param1]</color> 增加[param1]经验，可升级，也可为负但不会降级" + "\n" +
                "<color=#ffffff>newrelic</color> 立刻随机并打开选择圣物窗口" + "\n" +
                "<color=#ffffff>addrelic [param1] [param2]</color> 立刻获得第[param1]类型第[param2]号圣物" + "\n" +
                "<color=#ffffff>rmrelic [param1] [param2]</color> 立刻删除第[param1]类型第[param2]号圣物（如果已经拥有）" + "\n" +
                "<color=#ffffff>kill</color> 如果在战斗中，立刻击杀所有敌舰" + "\n" +
                "<color=#ffffff>am [param1]</color> 立刻给予[param1]个异星矩阵" + "\n" +
                "<color=#ffffff>give [param1] [param2]</color> 立刻给予[param2]个itemId为[param1]的物品" + "\n" +
                "<color=#ffffff>cool</color> 恒星炮立即冷却完毕" + "\n" +
                "---------------------- help ----------------------";
            Print(allCmds,22, false); // 这个forceLineCount传值取决于allCmds的行数
        }

        public static void ClearOutputField()
        {
            consoleOutputField.text = "";
            outputClearCount = 0;
        }
    }
}
