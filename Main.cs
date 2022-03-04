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

namespace DSP_Battle
{
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "1.0.0")]
    [BepInDependency(DSPModSavePlugin.MODGUID)]
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
            Harmony.CreateAndPatchAll(typeof(EnemyShips));
            Harmony.CreateAndPatchAll(typeof(Cannon));
            Harmony.CreateAndPatchAll(typeof(BattleProtos));
            Harmony.CreateAndPatchAll(typeof(EjectorUIPatch));
            Harmony.CreateAndPatchAll(typeof(UIAlert));

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

        public void InitNew()
        {
            int planetId = 103;

            PlanetData planetData = GameMain.galaxy.PlanetById(planetId);
            PlanetFactory factory = planetData.factory;
            PlanetTransport transport = factory.transport;
            List<StationComponent> stations = new List<StationComponent>(transport.stationPool);
            if (stations.Count == 0) return;
            int index = randSeed.Next(0, stations.Count);
            for (var i = 0; i < stations.Count; ++i)
            {
                StationComponent component = stations[(index + i) % stations.Count];
                if (component != null && component.id != 0 && component.isStellar)
                {
                    EnemyShips.Create(
                        component.gid,
                        planetData.uPosition +
                             new VectorLF3((randSeed.NextDouble() - 0.5) * 60000, (randSeed.NextDouble() - 0.5) * 60000, (randSeed.NextDouble() - 0.5) * 60000),
                        100, 50, 6002
                        );
                    return;
                }
            }
        }

        public void Export(BinaryWriter w)
        {
            EnemyShips.Export(w);
            Cannon.Export(w);
        }

        public void Import(BinaryReader r)
        {
            EnemyShips.Import(r);
            Cannon.Import(r);
        }

        public void IntoOtherSave()
        {
            EnemyShips.IntoOtherSave();
            Cannon.IntoOtherSave();
        }

        public static ManualLogSource logger;
       
    }
}
