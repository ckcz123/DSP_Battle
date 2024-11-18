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
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "2.1.2")]
    [BepInDependency(DSPModSavePlugin.MODGUID)]
    [BepInDependency(CommonAPIPlugin.GUID)]
    [BepInDependency(LDBToolPlugin.MODGUID)]
    [BepInDependency("Gnimaerd.DSP.plugin.MoreMegaStructure")]
    [CommonAPISubmoduleDependency(nameof(ProtoRegistry))]
    [CommonAPISubmoduleDependency(nameof(TabSystem))]
    [CommonAPISubmoduleDependency(nameof(LocalizationModule))]
    
    public class DspBattlePlugin : BaseUnityPlugin, IModCanSave
    {
        public static string GUID = "com.ckcz123.DSP_Battle";
        public static string MODID_tab = "DSPBattle";

        public static System.Random randSeed = new System.Random();
        public static int pagenum;
        public static ManualLogSource logger;
        private static ConfigFile config;
        public static ConfigEntry<int> starCannonRenderLevel;
        public static ConfigEntry<bool> starCannonDirectionReverse;

        public static bool isControlDown = false;
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

            
            MoreMegaStructure.StarCannon.renderLevel = starCannonRenderLevel.Value;
            MoreMegaStructure.StarCannon.renderLevel = MoreMegaStructure.StarCannon.renderLevel > 3 ? 3 : MoreMegaStructure.StarCannon.renderLevel;
            MoreMegaStructure.StarCannon.renderLevel = MoreMegaStructure.StarCannon.renderLevel < 0 ? 0 : MoreMegaStructure.StarCannon.renderLevel;
            //EnemyShips.Init();
            Harmony.CreateAndPatchAll(typeof(DspBattlePlugin));
            
            Harmony.CreateAndPatchAll(typeof(BattleProtos));
            Harmony.CreateAndPatchAll(typeof(FastStartOption));
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

            LDBTool.PreAddDataAction += BattleProtos.AddProtos;
            BattleProtos.AddTranslate();
            //LDBTool.PostAddDataAction += BattleProtos.PostDataAction;
            BattleProtos.InitEventProtos();
        }

        public void Start()
        {
            //BattleBGMController.InitAudioSources();

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
                Rank.AddExp(100000);
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
            if (Input.GetKeyDown(KeyCode.Minus) && isControlDown && !GameMain.isPaused && (Configs.nextWaveState == 1 || Configs.nextWaveState == 2))
            {
                Configs.nextWaveFrameIndex -= 60 * 60;
                if (Configs.nextWaveFrameIndex < GameMain.instance.timei) Configs.nextWaveFrameIndex = GameMain.instance.timei; // 对于精英波次，如果一次回退超过了倒计时，会减少精英波次的持续时间
                //由于强行使进攻提前到来，期望掉落的矩阵数减少10%，最少降低到无时间加成（即10min间隔）的对应波次基础期望的10%。
                if (Relic.HaveRelic(3,4)) // relic 3-4 只减少5%
                    Configs.nextWaveMatrixExpectation = (int)(Configs.nextWaveMatrixExpectation * 0.95f);
                else
                    Configs.nextWaveMatrixExpectation = (int)(Configs.nextWaveMatrixExpectation * 0.9f);

                int minExpectation = (int)(Configs.expectationMatrices[Math.Min(Configs.expectationMatrices.Length - 1, Configs.wavePerStar[Configs.nextWaveStarIndex])] * 0.1f);
                if (Configs.nextWaveMatrixExpectation < minExpectation) Configs.nextWaveMatrixExpectation = minExpectation;
            }
            if(Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.Z))
            {
                Relic.PrepareNewRelic();
                int planetId = 103;
                if (GameMain.localPlanet != null)
                    planetId = GameMain.localPlanet.id;
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.G))
            {
                EventSystem.InitNewEvent();
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.H))
            {
                EventSystem.ClearEvent();
            }
            if (Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.J))
            {
                EventSystem.TestIfGroudBaseInited();
            }
            UIRelic.SelectionWindowAnimationUpdate();
            UIRelic.CheckRelicSlotsWindowShowByMouse();
            UIRelic.SlotWindowAnimationUpdate();
            UIEventSystem.OnUpdate();
            //BattleBGMController.BGMLogicUpdate();
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
        [HarmonyPatch(typeof(UIEscMenu), "_OnOpen")]
        public static void UIEscMenu_OnOpen()
        {
            UpdateLogo();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameOption), "Apply")]
        public static void UpdateGameOption_Apply()
        {
            UpdateLogo();
        }

        public static void UpdateLogo()
        {
            var mainLogo = GameObject.Find("UI Root/Overlay Canvas/Main Menu/dsp-logo");
            var escLogo = GameObject.Find("UI Root/Overlay Canvas/In Game/Esc Menu/logo");

            var iconstr = DSPGame.globalOption.languageLCID == 2052
                ? "Assets/DSPBattle/logocn"
                : "Assets/DSPBattle/logoen";
            var texture = Resources.Load<Sprite>(iconstr).texture;

            mainLogo.GetComponent<RawImage>().texture = texture;
            escLogo.GetComponent<RawImage>().texture = texture;
            mainLogo.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
            escLogo.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
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
        }

        public void Export(BinaryWriter w)
        {
            Configs.Export(w);
            Droplets.Export(w);
            Rank.Export(w);
            Relic.Export(w);
            EventSystem.Exprot(w);
            //StarFortress.Export(w);
            //DevConsole.Export(w);
        }

        public void Import(BinaryReader r)
        {
            Configs.Import(r);
            Droplets.Import(r);
            Rank.Import(r);
            Relic.Import(r);
            EventSystem.Import(r);
            //StarFortress.Import(r);
            //DevConsole.Import(r);

            BattleProtos.ReCheckTechUnlockRecipes();
            BattleProtos.UnlockTutorials();
            //BattleBGMController.InitWhenLoad();

            InitStaticDataWhenLoad();
        }

        public void IntoOtherSave()
        {
            Configs.IntoOtherSave();
            //EnemyShips.IntoOtherSave();
            //MissileSilo.IntoOtherSave();
            Droplets.IntoOtherSave();
            Rank.IntoOtherSave();
            Relic.IntoOtherSave();
            EventSystem.IntoOtherSave();
            StarFortress.IntoOtherSave();

            DevConsole.IntoOtherSave();

            //EnemyShipUIRenderer.Init();
            //EnemyShipRenderer.Init();
            BattleProtos.ReCheckTechUnlockRecipes();
            BattleProtos.UnlockTutorials();
            //BattleBGMController.InitWhenLoad();

            InitStaticDataWhenLoad();
        }


    }
}

