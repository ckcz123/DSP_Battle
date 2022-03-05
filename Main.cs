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

namespace DSP_Battle
{
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "1.0.0")]
    [BepInDependency(DSPModSavePlugin.MODGUID)]
    [BepInDependency(CommonAPIPlugin.GUID)]
    [BepInDependency(LDBToolPlugin.MODGUID)]
    [CommonAPISubmoduleDependency(nameof(ProtoRegistry))]
    public class Main : BaseUnityPlugin, IModCanSave
    {
        public static System.Random randSeed = new System.Random();
        public static int minShowDetaisSecond; //取决于科技
        public static int framesUntilNextWave;
        public static int nextWaveShipCount;
        public static int nextWaveStrength;
        public static int nextWaveAward;
        public static int destroyedCount;
        public static bool preparingNextWave = false;


        public void Awake()
        {
            Logger.LogInfo("=========> Done!");
            logger = Logger;
            EnemyShips.Init();
            Harmony.CreateAndPatchAll(typeof(EnemyShips));
            Harmony.CreateAndPatchAll(typeof(Cannon));
            Harmony.CreateAndPatchAll(typeof(BattleProtos));
            Harmony.CreateAndPatchAll(typeof(EjectorUIPatch));
            Harmony.CreateAndPatchAll(typeof(UIAlert));

            Harmony.CreateAndPatchAll(typeof(MissileSilo));
            Harmony.CreateAndPatchAll(typeof(WormholeUIPatch));

            LDBTool.PreAddDataAction += BattleProtos.AddNewCannons;
            LDBTool.PostAddDataAction += BattleProtos.CopyPrefabDesc;

            //Cannon.testFrameCount = 0;
            //Cannon.ReInitAll();
        }

        public void Update()
        {
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
            }
        }

        public static void InitNew()
        {
            StarData star = GameMain.localStar;
            if (star == null) return;

            /*
            EnemyShips.Create(star.index, star.uPosition + new VectorLF3((randSeed.NextDouble() - 0.5) * 60000, (randSeed.NextDouble() - 0.5) * 60000, (randSeed.NextDouble() - 0.5) * 60000),
                100, 50, 6002);
            */
            EnemyShips.Create(star.index, WormholeUIPatch.starData.uPosition, 100, 50, 6002);
        }

        public void Export(BinaryWriter w)
        {
            EnemyShips.Export(w);
            Cannon.Export(w);
            MissileSilo.Export(w);
        }

        public void Import(BinaryReader r)
        {
            EnemyShips.Import(r);
            Cannon.Import(r);
            MissileSilo.Import(r);
        }

        public void IntoOtherSave()
        {
            EnemyShips.IntoOtherSave();
            Cannon.IntoOtherSave();
            MissileSilo.IntoOtherSave();
        }

        public static ManualLogSource logger;
       
    }
}
