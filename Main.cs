using BepInEx;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;

namespace DSP_Battle
{
    [BepInPlugin("com.ckcz123.DSP_Battle", "DSP_Battle", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        public void Awake()
        {
            Logger.LogInfo("=========> Done!");
            logger = Logger;
            Harmony.CreateAndPatchAll(typeof(Ship));
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                Ship.Init();
            }
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                Ship.MovePlayer();
            }
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                Ship.paused = !Ship.paused;
            }
        }

        public static ManualLogSource logger;
       
    }
}
