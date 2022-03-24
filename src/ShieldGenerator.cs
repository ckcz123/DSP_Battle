using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DSP_Battle
{
    public class ShieldGenerator
    {


        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerExchangerComponent), "InputUpdate")]
        public static void ShieldGenPowerUpdatePatch1(ref PowerExchangerComponent __instance)
        {
            if (__instance.emptyId != 1208)
                return;
            if(__instance.currPoolEnergy * 1.0 / __instance.maxPoolEnergy > 0.5)
                __instance.currPoolEnergy -= 2 * __instance.currEnergyPerTick;
        }

    }
}
