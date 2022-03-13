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
using xiaoye97;

namespace DSP_Battle
{
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "1.0.0")]
    [BepInDependency(DSPModSavePlugin.MODGUID)]
    [BepInDependency(CommonAPIPlugin.GUID)]
    [BepInDependency(LDBToolPlugin.MODGUID)]
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
                    string tabName = "Defense";
                    if (Localization.language == Language.zhCN) tabName = "轨道防御";
                    pagenum = TabSystem.RegisterTab($"{MODID_tab}:{MODID_tab}Tab", new TabData(tabName, "Assets/DSPBattle/dspbattletabicon"));
                    BattleProtos.pageBias = (pagenum - 2) * 1000 - 500;
                }
            }
            catch (Exception)
            {
                pagenum = 0;
            }

            EnemyShips.Init();
            Harmony.CreateAndPatchAll(typeof(DspBattlePlugin));
            Harmony.CreateAndPatchAll(typeof(EnemyShips));
            Harmony.CreateAndPatchAll(typeof(Cannon));
            Harmony.CreateAndPatchAll(typeof(BattleProtos));
            Harmony.CreateAndPatchAll(typeof(EjectorUIPatch));
            Harmony.CreateAndPatchAll(typeof(UIAlert));
            Harmony.CreateAndPatchAll(typeof(MissileSilo));
            Harmony.CreateAndPatchAll(typeof(WormholeUIPatch));
            Harmony.CreateAndPatchAll(typeof(UIBattleStatistics));
            Harmony.CreateAndPatchAll(typeof(UIDialogPatch));

            LDBTool.PreAddDataAction += BattleProtos.AddProtos;
            LDBTool.PostAddDataAction += BattleProtos.CopyPrefabDesc;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Minus) && (Configs.nextWaveState == 1 || Configs.nextWaveState == 2))
            {
                Configs.nextWaveFrameIndex -= 60 * 60;
            }
            if (Input.GetKeyDown(KeyCode.Backspace) && !GameMain.isPaused)
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

        public void Export(BinaryWriter w)
        {
            Configs.Export(w);
            EnemyShips.Export(w);
            Cannon.Export(w);
            MissileSilo.Export(w);
            UIAlert.Export(w);
        }

        public void Import(BinaryReader r)
        {
            Configs.Import(r);
            EnemyShips.Import(r);
            Cannon.Import(r);
            MissileSilo.Import(r);
            UIAlert.Import(r);

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

            UIBattleStatistics.InitAll();
            UIBattleStatistics.InitSelectDifficulty();
        }


    }
}

