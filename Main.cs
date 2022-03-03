using crecheng.DSPModSave;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using System.IO;

namespace DSP_Battle
{
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "1.0.0")]
    [BepInDependency(DSPModSavePlugin.MODGUID)]
    public class Main : BaseUnityPlugin, IModCanSave
    {
        public void Awake()
        {
            Logger.LogInfo("=========> DSP_Battle initialized!");
            logger = Logger;
            Harmony.CreateAndPatchAll(typeof(EnemyShips));
            Harmony.CreateAndPatchAll(typeof(Cannon));
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                int stationGid = 2;
                int planetId = GameMain.data.galacticTransport.stationPool[stationGid].planetId;
                PlanetData planet = GameMain.galaxy.PlanetById(planetId);

                EnemyShips.Create(stationGid, 
                    (planet.star.uPosition + planet.uPosition) / 2
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
        }

        public void Import(BinaryReader r)
        {
            EnemyShips.Import(r);
        }

        public void IntoOtherSave()
        {
            EnemyShips.IntoOtherSave();
        }

        public static ManualLogSource logger;
       
        

    }
}
