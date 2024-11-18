using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DSP_Battle
{
    /// <summary>
    /// Created by starfi5h
    /// Source: https://gist.github.com/starfi5h/fe172485aebb251a38c30dc85ea20f04
    /// 用于修复游戏本身bug：在删除物流塔时，若该物流塔有工作中的物流船正在递送物品，目的地物流塔将永久保持这些即将到来的货物order占用
    /// </summary>
    class StationOrderFixPatch
    {
        //[HarmonyTranspiler, HarmonyPatch(typeof(PlanetTransport), "RemoveStationComponent")]
        //public static IEnumerable<CodeInstruction> RemoveStationComponent_Transpiler(IEnumerable<CodeInstruction> instructions)
        //{
        //    try
        //    {
        //        var methodInfo = AccessTools.Method(typeof(StationComponent), "Reset");
        //        var codeMacher = new CodeMatcher(instructions)
        //            .MatchForward(false, new CodeMatch(OpCodes.Callvirt, methodInfo))
        //            .RemoveInstruction()
        //            .Insert(
        //                new CodeInstruction(OpCodes.Ldarg_0),
        //                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StationOrderFixPatch), nameof(ResetFix)))
        //            );

        //        return codeMacher.InstructionEnumeration();
        //    }
        //    catch (Exception e)
        //    {
        //        //Log.Warn(e);
        //        return instructions;
        //    }
        //}

        //public static void ResetFix(StationComponent station, PlanetTransport transport)
        //{
        //    var stationPool = transport.stationPool;
        //    var gStationPool = GameMain.data.galacticTransport.stationPool;

        //    for (int n = 0; n < station.workDroneCount; n++)
        //    {
        //        // For drones carrying item to other station, remove corresponding local order
        //        if (station.workDroneDatas[n].itemCount != 0 && station.workDroneDatas[n].direction > 0)
        //        {
        //            var stores = stationPool[station.workDroneDatas[n].endId]?.storage;
        //            var otherIndex = station.workDroneOrders[n].otherIndex;
        //            // In Nebula multiplayer mod, there are dummy stations which stationStore length is 0.
        //            if (stores == null || stores.Length <= otherIndex)
        //                continue;

        //            stores[otherIndex].localOrder -= station.workDroneOrders[n].otherOrdered;
        //        }
        //    }

        //    for (int n = 0; n < station.workShipCount; n++)
        //    {
        //        // For ships carrying item to other station, remove corresponding remote order
        //        if (station.workShipDatas[n].itemCount != 0 && station.workShipDatas[n].direction > 0)
        //        {
        //            var stores = gStationPool[station.workShipDatas[n].otherGId]?.storage;
        //            var otherIndex = station.workShipOrders[n].otherIndex;
        //            // In Nebula multiplayer mod, there are dummy stations which stationStore length is 0.
        //            if (stores == null || stores.Length <= otherIndex)
        //                continue;

        //            stores[otherIndex].remoteOrder -= station.workShipOrders[n].otherOrdered;
        //        }
        //    }

        //    station.Reset();
        //}
    }
}
