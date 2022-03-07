using BepInEx;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using xiaoye97;
using CommonAPI;
using crecheng.DSPModSave;
using System.IO;
using CommonAPI.Systems;
using System.Reflection;
using BepInEx.Configuration;

namespace DSP_Battle
{
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "1.0.0")]
    [BepInDependency(DSPModSavePlugin.MODGUID)]
    [BepInDependency(CommonAPIPlugin.GUID)]
    [BepInDependency(LDBToolPlugin.MODGUID)]
    [CommonAPISubmoduleDependency(nameof(ProtoRegistry))]
    public class DspBattlePlugin : BaseUnityPlugin, IModCanSave
    {
        public static string GUID = "com.ckcz123.DSP_Battle";
        public static string MODID_tab = "DSPBattle";

        public static bool developerMode = true;

        public static System.Random randSeed = new System.Random();
        public static int minShowDetaisSecond; //取决于科技
        public static int framesUntilNextWave;
        public static int nextWaveShipCount;
        public static int nextWaveStrength;
        public static int nextWaveAward;
        public static int destroyedCount;
        public static bool preparingNextWave = false;

        public static int pagenum;

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

            LDBTool.PreAddDataAction += BattleProtos.AddProtos;
            LDBTool.PostAddDataAction += BattleProtos.CopyPrefabDesc;

            //Cannon.testFrameCount = 0;
            //Cannon.ReInitAll();
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

            /*
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                InitNew();
            }
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                EnemyShips.paused = !EnemyShips.paused;
            }
            if(Input.GetKeyDown(KeyCode.Backspace))
            {
                UIAlert.OnActiveChange();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                framesUntilNextWave = 3700 * 60;
                nextWaveShipCount = 12;
                nextWaveStrength = 240;
                nextWaveAward = 3724;
                preparingNextWave = true;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                framesUntilNextWave -= 3600;
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                preparingNextWave = false;
            }
            //Cannon.BulletTrack();
            if (preparingNextWave)
            {
                framesUntilNextWave -= 1;
                UIAlert.CountDownRefresh(preparingNextWave, framesUntilNextWave, 0, nextWaveShipCount, nextWaveStrength, nextWaveAward, 0, false); //第二个0应该传入已经摧毁的建筑数
            } */
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
        }

        public void IntoOtherSave()
        {
            Configs.IntoAnotherSave();
            EnemyShips.IntoOtherSave();
            Cannon.IntoOtherSave();
            MissileSilo.IntoOtherSave();
            UIAlert.IntoOtherSave();
        }

        public static ManualLogSource logger;
       
    }
}

