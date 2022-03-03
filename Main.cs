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
        public void Awake()
        {
            Logger.LogInfo("=========> Done!");
            logger = Logger;
            Harmony.CreateAndPatchAll(typeof(EnemyShips));
            Harmony.CreateAndPatchAll(typeof(Cannon));
            Harmony.CreateAndPatchAll(typeof(BattleProtos));
            Harmony.CreateAndPatchAll(typeof(EjectorUIPatch));

            LDBTool.PreAddDataAction += BattleProtos.AddNewCannons;
            LDBTool.PostAddDataAction += BattleProtos.CopyPrefabDesc;

            //Cannon.testFrameCount = 0;
            //Cannon.ReInitAll();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                int stationGid = 2;
                int planetId = GameMain.data.galacticTransport.stationPool[stationGid].planetId;
                PlanetData planet = GameMain.galaxy.PlanetById(planetId);

                EnemyShips.Create(stationGid, 
                    (planet.star.uPosition + planet.uPosition) / 2 + new VectorLF3((randSeed.NextDouble()-0.5)*60000, (randSeed.NextDouble() - 0.5) * 60000, (randSeed.NextDouble()-0.5) * 60000)
                    , 100, 6002);
            }
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                EnemyShips.paused = !EnemyShips.paused;
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
