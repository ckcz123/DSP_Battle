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
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "1.0.0")]
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
                    pagenum = TabSystem.RegisterTab($"{MODID_tab}:{MODID_tab}Tab", new TabData("轨道防御", "Assets/DSPBattle/dspbattletabicon"));
                    BattleProtos.pageBias = (pagenum - 2) * 1000;
                    MoreMegaStructure.MoreMegaStructure.battlePagenum = pagenum;

                }
            }
            catch (Exception)
            {
                pagenum = 0;
            }

            EnemyShips.Init();
            Harmony.CreateAndPatchAll(typeof(DspBattlePlugin));
            Harmony.CreateAndPatchAll(typeof(EnemyShips));
            Harmony.CreateAndPatchAll(typeof(RemoveEntities));
            Harmony.CreateAndPatchAll(typeof(Cannon));
            Harmony.CreateAndPatchAll(typeof(BattleProtos));
            Harmony.CreateAndPatchAll(typeof(EjectorUIPatch));
            Harmony.CreateAndPatchAll(typeof(UIAlert));
            Harmony.CreateAndPatchAll(typeof(MissileSilo));
            Harmony.CreateAndPatchAll(typeof(WormholeUIPatch));
            Harmony.CreateAndPatchAll(typeof(UIBattleStatistics));
            Harmony.CreateAndPatchAll(typeof(UIDialogPatch));
            Harmony.CreateAndPatchAll(typeof(StarCannon));
            Harmony.CreateAndPatchAll(typeof(ShieldGenerator));

            LDBTool.PreAddDataAction += BattleProtos.AddProtos;
            LDBTool.PostAddDataAction += BattleProtos.PostDataAction;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Minus) && !GameMain.isPaused && UIRoot.instance?.uiGame?.buildMenu?.currentCategory == 0 && (Configs.nextWaveState == 1 || Configs.nextWaveState == 2))
            {
                Configs.nextWaveFrameIndex -= 60 * 60;
            }
            if (Input.GetKeyDown(KeyCode.Backspace) && !GameMain.isPaused && UIRoot.instance?.uiGame?.buildMenu?.currentCategory == 0)
            {
                UIAlert.OnActiveChange();
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameMain), "OnDestroy")]
        public static void GameMain_onDestroy()
        {
            if (config == null) return;
            string configFile = config.ConfigFilePath;
            string path = Path.Combine(Path.GetDirectoryName(configFile), "LDBTool");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
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

            UIBattleStatistics.InitAll();
            UIBattleStatistics.InitSelectDifficulty();
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

            UIBattleStatistics.InitAll();
            UIBattleStatistics.InitSelectDifficulty();
        }


    }
}

