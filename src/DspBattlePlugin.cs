using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CommonAPI;
using CommonAPI.Systems;
using CommonAPI.Systems.ModLocalization;
using crecheng.DSPModSave;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using xiaoye97;

namespace DSP_Battle
{
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "3.2.5")]
    [BepInDependency(DSPModSavePlugin.MODGUID)]
    [BepInDependency(CommonAPIPlugin.GUID)]
    [BepInDependency(LDBToolPlugin.MODGUID)]
    [BepInDependency("Gnimaerd.DSP.plugin.MoreMegaStructure")]
    [CommonAPISubmoduleDependency(nameof(ProtoRegistry))]
    [CommonAPISubmoduleDependency(nameof(TabSystem))]
    [CommonAPISubmoduleDependency(nameof(LocalizationModule))]

    public class DspBattlePlugin : BaseUnityPlugin, IModCanSave
    {
        public const string GUID = "com.ckcz123.DSP_Battle";
        public static string MODID_tab = "DSPBattle";

        public static System.Random randSeed = new System.Random();
        public static int pagenum;
        public static ManualLogSource logger;
        private static ConfigFile config;
        public static ConfigEntry<int> starCannonRenderLevel;
        public static ConfigEntry<bool> starCannonDirectionReverse;
        public static ConfigEntry<bool> enableBattleBGM;
        public static ConfigEntry<float> battleBGMVolume;

        public static bool isControlDown = false;
        public static bool isShiftDown = false;

        public static bool playerInvincible = false;

        public static GameObject mainLogo = null;
        public static GameObject escLogo = null;
        public static Texture2D logoTexture = null;
        public static Texture2D escLogoTexture = null;
        public void Awake()
        {
            logger = Logger;
            config = Config;
            Configs.Init(Config);

            var pluginfolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resources = new ResourceData(GUID, "DSPBattle", pluginfolder);
            resources.LoadAssetBundle("dspbattletex");
            ProtoRegistry.AddResource(resources);
            try
            {
                using (ProtoRegistry.StartModLoad(GUID))
                {
                    //pagenum = TabSystem.RegisterTab($"{MODID_tab}:{MODID_tab}Tab", new TabData("轨道防御", "Assets/DSPBattle/dspbattletabicon"));
                    pagenum = MoreMegaStructure.MoreMegaStructure.pagenum;
                    BattleProtos.pageBias = (pagenum - 2) * 1000;
                    MoreMegaStructure.MoreMegaStructure.battlePagenum = pagenum;
                }
            }
            catch (Exception)
            {
                pagenum = 0;
            }
            starCannonRenderLevel = Config.Bind<int>("config", "StarCannonRenderLevel", 2, "[0-3] Higher Level will provide more star cannon effect and particles but might decrease the UPS and FPS when star cannon is firing. 更高的设置会提供更多的恒星炮特效，但可能会在恒星炮开火时降低帧率，反之则可能提高开火时的帧率。");
            starCannonDirectionReverse = Config.Bind<bool>("config", "starCannonDirectionReverse", false, "Deprecated. 已弃用。");
            enableBattleBGM = Config.Bind<bool>("config", "EnableBattleBGM", true, "Set to false to disable the BGM switching when Icarus is in combat. 设置为false来关闭战斗时的BGM切换。");
            battleBGMVolume = Config.Bind<float>("config", "BattleBGMVolume", 1.0f, "( 0.0 - 2.0 )Control the Battle BGM's volume, will not affect the vanilla game BGM.  控制战斗音乐的音量大小，不会影响游戏默认BGM的音量。最小为0，最大为2.");


            MoreMegaStructure.StarCannon.renderLevel = starCannonRenderLevel.Value;
            MoreMegaStructure.StarCannon.renderLevel = MoreMegaStructure.StarCannon.renderLevel > 3 ? 3 : MoreMegaStructure.StarCannon.renderLevel;
            MoreMegaStructure.StarCannon.renderLevel = MoreMegaStructure.StarCannon.renderLevel < 0 ? 0 : MoreMegaStructure.StarCannon.renderLevel;
            Configs.enableBattleBGM = enableBattleBGM.Value;
            BattleBGMController.volumeFactor = BattleBGMController.volumeBasic * (float)Maths.Clamp(battleBGMVolume.Value, 0.0f, 2.0f);
            //EnemyShips.Init();
            Harmony.CreateAndPatchAll(typeof(DspBattlePlugin));

            Harmony.CreateAndPatchAll(typeof(BattleProtos));
            Harmony.CreateAndPatchAll(typeof(NewGameOption));
            Harmony.CreateAndPatchAll(typeof(UIDialogPatch));
            Harmony.CreateAndPatchAll(typeof(Droplets));
            Harmony.CreateAndPatchAll(typeof(RendererSphere));
            Harmony.CreateAndPatchAll(typeof(PlanetEngine));
            Harmony.CreateAndPatchAll(typeof(UIRank));
            Harmony.CreateAndPatchAll(typeof(Rank));
            Harmony.CreateAndPatchAll(typeof(BattleBGMController));
            Harmony.CreateAndPatchAll(typeof(Relic));
            Harmony.CreateAndPatchAll(typeof(RelicFunctionPatcher));
            Harmony.CreateAndPatchAll(typeof(StarFortress));
            Harmony.CreateAndPatchAll(typeof(UIStarFortress));
            Harmony.CreateAndPatchAll(typeof(StationOrderFixPatch));
            Harmony.CreateAndPatchAll(typeof(DropletFleetPatchers));
            Harmony.CreateAndPatchAll(typeof(EventSystem));
            Harmony.CreateAndPatchAll(typeof(AssaultController));
            Harmony.CreateAndPatchAll(typeof(UIAssaultAlert));
            Harmony.CreateAndPatchAll(typeof(UIEscMenuPatch));
            Harmony.CreateAndPatchAll(typeof(UIHiveNamePatcher));
            Harmony.CreateAndPatchAll(typeof(WormholeRenderer));
            Harmony.CreateAndPatchAll(typeof(UIEnemyBriefInfoPatcher));

            LDBTool.PreAddDataAction += BattleProtos.AddProtos;
            BattleProtos.AddTranslate();
            //LDBTool.PostAddDataAction += BattleProtos.PostDataAction;
            BattleProtos.InitEventProtos();
            TCFVPerformanceMonitor.Awake();
            EvolveData.levelExps[100] = 2147483647; // 防止出现除以0的错误
        }

        public void Start()
        {
            BattleBGMController.InitAudioSources();

        }

        public void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Minus) && !GameMain.isPaused && UIRoot.instance?.uiGame?.buildMenu?.currentCategory == 0 && (Configs.nextWaveState == 1 || Configs.nextWaveState == 2))
            //{
            //    Configs.nextWaveFrameIndex -= 60 * 60;
            //}
            if (Configs.developerMode && Input.GetKeyDown(KeyCode.Z))
            {
                //Debug.LogWarning("Z test warning by TCFV");
                //Debug.Log("Z test log by TCFV");
                //Debug.LogError("Z error log by TCFV");
                //EnemyShips.TestDestoryStation();
                //Rank.AddExp(100000);
                if (MoreMegaStructure.MoreMegaStructure.curStar != null)
                {
                    int starIndex = MoreMegaStructure.MoreMegaStructure.curStar.index;
                    if (isControlDown)
                    {
                        //StarFortress.ConstructStarFortPoint(starIndex, 8037, 10000);
                        //StarFortress.ConstructStarFortPoint(starIndex, 8038, 10000);
                        //StarFortress.ConstructStarFortPoint(starIndex, 8039, 10000);
                    }
                    else
                    {
                        //StarFortress.ConstructStarFortPoint(starIndex, 8037, 743);
                        //StarFortress.ConstructStarFortPoint(starIndex, 8038, 743);
                        //StarFortress.ConstructStarFortPoint(starIndex, 8039, 743);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                isControlDown = true;
                UIStarFortress.RefreshSetBtnText();
            }
            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                isControlDown = false;
                UIStarFortress.RefreshSetBtnText();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                isShiftDown = true;
                UIStarFortress.RefreshSetBtnText();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                isShiftDown = false;
                UIStarFortress.RefreshSetBtnText();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && UIDevConsole.consoleObj != null && UIDevConsole.consoleObj.activeSelf)
            {
                DevConsole.PrevCommand();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && UIDevConsole.consoleObj != null && UIDevConsole.consoleObj.activeSelf)
            {
                DevConsole.NextCommand();
            }
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                if (MoreMegaStructure.MoreMegaStructure.GenesisCompatibility && isControlDown)
                    UIEventSystem.OnEventButtonClick();
                else if (!MoreMegaStructure.MoreMegaStructure.GenesisCompatibility)
                    UIEventSystem.OnEventButtonClick();
            }
            if (Input.GetKeyDown(KeyCode.L) && !VFInput.inputing)
            {
                UISkillPointsWindow.Switch();
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.Z))
            {
                Relic.PrepareNewRelic();
                int planetId = 103;
                if (GameMain.localPlanet != null)
                    planetId = GameMain.localPlanet.id;
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.M))
            {
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.K))
            {
                GameMain.mainPlayer.Kill();
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.G))
            {
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.H))
            {
                for (int i = 0; i < AssaultController.assaultHives.Count; i++)
                {
                    AssaultController.assaultHives[i].time -= 3600;
                    AssaultController.assaultHives[i].timeTillAssault -= 3600;
                }
            }
            if (Configs.developerMode && isShiftDown && Input.GetKeyDown(KeyCode.H))
            {
                for (int i = 0; i < AssaultController.assaultHives.Count; i++)
                {
                    AssaultController.assaultHives[i].time -= 3600 * 5;
                    AssaultController.assaultHives[i].timeTillAssault -= 3600 * 5;
                }
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.J))
            {
                AssaultController.InitNewAssault(-1);
            }
            if (Configs.developerMode && isShiftDown && Input.GetKeyDown(KeyCode.J))
            {
                for (int i = 0; i < AssaultController.assaultHives.Count; i++)
                {
                    AssaultController.assaultHives[i].state = EAssaultHiveState.Remove;
                }
            }

            if (playerInvincible && GameMain.mainPlayer != null)
                GameMain.mainPlayer.invincibleTicks = 60;

            UIRelic.SelectionWindowAnimationUpdate();
            UIRelic.CheckRelicSlotsWindowShowByMouse();
            UIRelic.SlotWindowAnimationUpdate();
            UIEventSystem.OnUpdate();
            BattleBGMController.BGMLogicUpdate();
            UISkillPointsWindow.Update();
            DevConsole.Update();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameMain), "OnDestroy")]
        public static void GameMain_onDestroy()
        {
            if (config == null) return;
            try
            {
                string configFile = config.ConfigFilePath;
                string path = Path.Combine(Path.GetDirectoryName(configFile), "LDBTool");
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception)
            { }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMainMenu), "_OnOpen")]
        public static void UIMainMenu_OnOpen()
        {
            UpdateLogo();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMainMenu), "_OnUpdate")]
        public static void UIMainMenu_OnUpdate()
        {
            UpdateLogo();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEscMenu), "_OnOpen")]
        public static void UIEscMenu_OnOpen()
        {
            UpdateLogo();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEscMenu), "_OnUpdate")]
        public static void UIEscMenu_OnUpdate()
        {
            UpdateLogo();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameOption), "Apply")]
        public static void UpdateGameOption_Apply()
        {
            UpdateLogo();
        }

        /// <summary>
        /// 用于拦截Esc键按下时优先关闭已打开的窗口（如果有），而非打开esc菜单
        /// </summary>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameMain), "LateUpdate")]
        public static bool EscLogicBlocker()
        {
            UIDevConsole.EscLogic();
            UIEventSystem.EscLogic();
            UISkillPointsWindow.EscLogic();
            return true;
        }

        public static void UpdateLogo()
        {
            if(mainLogo == null)
                mainLogo = GameObject.Find("UI Root/Overlay Canvas/Main Menu/dsp-logo");
            if(escLogo == null)
                escLogo = GameObject.Find("UI Root/Overlay Canvas/In Game/Esc Menu/logo");

            
            if (logoTexture == null || escLogoTexture == null)
            {
                var iconstr = DSPGame.globalOption.languageLCID == 2052
                ? "Assets/DSPBattle/LOGO-cn"
                : "Assets/DSPBattle/LOGO-en";
                var escIconstr = DSPGame.globalOption.languageLCID == 2052
                    ? "Assets/DSPBattle/LOGO-cn-c"
                    : "Assets/DSPBattle/LOGO-en";
                if (MoreMegaStructure.MoreMegaStructure.GenesisCompatibility)
                {
                    iconstr = DSPGame.globalOption.languageLCID == 2052
                    ? "Assets/DSPBattle/LOGOGB-cn"
                    : "Assets/DSPBattle/LOGOGB-en";
                    escIconstr = iconstr;
                    mainLogo.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 250);
                    escLogo.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 250);
                }
                logoTexture = Resources.Load<Sprite>(iconstr).texture;
                escLogoTexture = Resources.Load<Sprite>(escIconstr).texture;
            }
            if(mainLogo.GetComponent<RawImage>().texture != logoTexture)
                mainLogo.GetComponent<RawImage>().texture = logoTexture;
            if(escLogo.GetComponent<RawImage>().texture != escLogoTexture)
                escLogo.GetComponent<RawImage>().texture = escLogoTexture;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIAbnormalityTip), "_OnInit")]
        public static void UIAbnormalityTip_OnInit(ref UIAbnormalityTip __instance)
        {
            __instance.isWarned = true;
            __instance.willClose = true;
            __instance.mainTweener.Play1To0Continuing();
            __instance.closeDelayTime = 3f;
        }

        public static void InitStaticDataWhenLoad()
        {
            BattleProtos.RewriteTutorialProtosWhenLoad();
            BattleProtos.EditProtossWhenLoad();
            MP.InitBlocker();
        }

        public void Export(BinaryWriter w)
        {
            Configs.Export(w);
            Droplets.Export(w);
            Rank.Export(w);
            Relic.Export(w);
            EventSystem.Exprot(w);
            StarFortress.Export(w);
            SkillPoints.Export(w);
            AssaultController.Export(w);

            DevConsole.Export(w);
        }

        public void Import(BinaryReader r)
        {
            Configs.Import(r);
            Droplets.Import(r);
            Rank.Import(r);
            Relic.Import(r);
            EventSystem.Import(r);
            StarFortress.Import(r);
            SkillPoints.Import(r);
            AssaultController.Import(r);

            DevConsole.Import(r);

            BattleProtos.ReCheckTechUnlockRecipes();
            BattleProtos.UnlockTutorials();
            BattleBGMController.InitWhenLoad();

            InitStaticDataWhenLoad();

            if (RendererSphere.dropletSpheres.Count != GameMain.galaxy.starCount) RendererSphere.InitAll();
        }

        public void IntoOtherSave()
        {
            Configs.IntoOtherSave();
            Droplets.IntoOtherSave();
            Rank.IntoOtherSave();
            Relic.IntoOtherSave();
            EventSystem.IntoOtherSave();
            StarFortress.IntoOtherSave();

            DevConsole.IntoOtherSave();
            SkillPoints.IntoOtherSave();
            AssaultController.IntoOtherSave();

            BattleProtos.ReCheckTechUnlockRecipes();
            BattleProtos.UnlockTutorials();
            BattleBGMController.InitWhenLoad();

            InitStaticDataWhenLoad();

            if (RendererSphere.dropletSpheres.Count != GameMain.galaxy.starCount) RendererSphere.InitAll();
        }


    }
}

