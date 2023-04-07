using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CommonAPI;
using CommonAPI.Systems;
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
                    //不再使用自己的tab页，而使用巨构的页，于此同时巨构的tab图标也改成了轨道防御的图标，且此mod开启后名称也改成了轨道防御(后面调用了ChangeTabName)
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
            starCannonDirectionReverse = Config.Bind<bool>("config", "starCannonDirectionReverse", false, "Setting to true will cause all star cannons to fire from the North Pole instead of the South Pole. 设置为true将会使所有恒星炮的从北极而非南极开火。");
            if(starCannonDirectionReverse.Value)
                StarCannon.reverseDirection = 1;
            
            StarCannon.renderLevel = starCannonRenderLevel.Value;
            StarCannon.renderLevel = StarCannon.renderLevel > 3 ? 3 : StarCannon.renderLevel;
            StarCannon.renderLevel = StarCannon.renderLevel < 0 ? 0 : StarCannon.renderLevel;
            EnemyShips.Init();
            Harmony.CreateAndPatchAll(typeof(DspBattlePlugin));
            Harmony.CreateAndPatchAll(typeof(EnemyShips));
            Harmony.CreateAndPatchAll(typeof(RemoveEntities));
            Harmony.CreateAndPatchAll(typeof(Cannon));
            Harmony.CreateAndPatchAll(typeof(BattleProtos));
            Harmony.CreateAndPatchAll(typeof(FastStartOption));
            Harmony.CreateAndPatchAll(typeof(EjectorUIPatch));
            Harmony.CreateAndPatchAll(typeof(UIAlert));
            Harmony.CreateAndPatchAll(typeof(MissileSilo));
            Harmony.CreateAndPatchAll(typeof(WormholeUIPatch));
            Harmony.CreateAndPatchAll(typeof(UIBattleStatistics));
            Harmony.CreateAndPatchAll(typeof(UIDialogPatch));
            Harmony.CreateAndPatchAll(typeof(StarCannon));
            Harmony.CreateAndPatchAll(typeof(ShieldGenerator));
            Harmony.CreateAndPatchAll(typeof(ShieldRenderer));
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

            LDBTool.PreAddDataAction += BattleProtos.AddProtos;
            LDBTool.PostAddDataAction += BattleProtos.PostDataAction;
            LDBTool.EditDataAction += BattleProtos.ChangeTabName;

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
                //EnemyShips.TestDestoryStation();
                if (MoreMegaStructure.MoreMegaStructure.curStar != null)
                {
                    int starIndex = MoreMegaStructure.MoreMegaStructure.curStar.index;
                    if (isControlDown)
                    {
                        StarFortress.ConstructStarFortPoint(starIndex, 8037, 10000);
                        //StarFortress.ConstructStarFortPoint(starIndex, 8038, 10000);
                        StarFortress.ConstructStarFortPoint(starIndex, 8039, 10000);
                    }
                    else
                    {
                        StarFortress.ConstructStarFortPoint(starIndex, 8037, 743);
                        //StarFortress.ConstructStarFortPoint(starIndex, 8038, 743);
                        StarFortress.ConstructStarFortPoint(starIndex, 8039, 743);
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
            if (Input.GetKeyDown(KeyCode.Backspace) && !GameMain.isPaused && UIRoot.instance?.uiGame?.buildMenu?.currentCategory == 0)
            {
                UIAlert.OnActiveChange();
            }
            if(Configs.developerMode && isControlDown && Input.GetKeyDown(KeyCode.Z))
            {
                Relic.PrepareNewRelic();
                int planetId = 103;
                if (GameMain.localPlanet != null)
                    planetId = GameMain.localPlanet.id;
                if (ShieldGenerator.currentShield.ContainsKey(planetId))
                    ShieldGenerator.currentShield.AddOrUpdate(planetId, 100000, (x, y) => y + 100000);
            }
            UIRelic.SelectionWindowAnimationUpdate();
            UIRelic.CheckRelicSlotsWindowShowByMouse();
            UIRelic.SlotWindowAnimationUpdate();
            BattleBGMController.BGMLogicUpdate();
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

            var iconstr = Localization.language == Language.zhCN
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

        public void Export(BinaryWriter w)
        {
            Configs.Export(w);
            EnemyShips.Export(w);
            Cannon.Export(w);
            MissileSilo.Export(w);
            UIAlert.Export(w);
            WormholeProperties.Export(w);
            StarCannon.Export(w);
            ShieldGenerator.Export(w);
            Droplets.Export(w);
            Rank.Export(w);
            Relic.Export(w);
            StarFortress.Export(w);
        }

        public void Import(BinaryReader r)
        {
            Configs.Import(r);
            EnemyShips.Import(r);
            Cannon.Import(r);
            MissileSilo.Import(r);
            UIAlert.Import(r);
            WormholeProperties.Import(r);
            StarCannon.Import(r);
            ShieldGenerator.Import(r);
            Droplets.Import(r);
            Rank.Import(r);
            Relic.Import(r);
            StarFortress.Import(r);

            WaveStages.ResetCargoAccIncTable(Configs.extraSpeedEnabled && Rank.rank>=5);
            UIBattleStatistics.InitAll();
            UIBattleStatistics.InitSelectDifficulty();
            EnemyShipUIRenderer.Init();
            EnemyShipRenderer.Init();
            BattleProtos.ReCheckTechUnlockRecipes();
            BattleBGMController.InitWhenLoad();
        }

        public void IntoOtherSave()
        {
            Configs.IntoOtherSave();
            EnemyShips.IntoOtherSave();
            Cannon.IntoOtherSave();
            MissileSilo.IntoOtherSave();
            UIAlert.IntoOtherSave();
            WormholeProperties.IntoOtherSave();
            StarCannon.IntoOtherSave();
            ShieldGenerator.IntoOtherSave();
            Droplets.IntoOtherSave();
            Rank.IntoOtherSave();
            Relic.IntoOtherSave();
            StarFortress.IntoOtherSave();

            WaveStages.ResetCargoAccIncTable(Configs.extraSpeedEnabled && Rank.rank >= 5);
            UIBattleStatistics.InitAll();
            UIBattleStatistics.InitSelectDifficulty();
            EnemyShipUIRenderer.Init();
            EnemyShipRenderer.Init();
            BattleProtos.ReCheckTechUnlockRecipes();
            BattleBGMController.InitWhenLoad();
        }


    }
}

