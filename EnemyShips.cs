using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    public class EnemyShips
    {
        public static List<EnemyShip> ships = new List<EnemyShip>();
        public static System.Random gidRandom = new System.Random();
        public static bool paused = false;

        public static void Create(int stationGid, VectorLF3 initPos, int initHp, int itemId = 0)
        {
            EnemyShip enemyShip = new EnemyShip(
                gidRandom.Next(1<<25, 1<<27),
                stationGid,
                initPos,
                initHp,
                GameMain.history.logisticShipSailSpeedModified,
                itemId);
            Main.logger.LogInfo("=========> Init ship at station " + enemyShip.shipData.otherGId);
            ships.Add(enemyShip);
        }

        public static EnemyShip FindNearestShip(VectorLF3 uPos)
        {
            if (ships.Count == 0) return null;
            EnemyShip ship = ships[0];
            for (var i = 1; i < ships.Count; ++i)
            {
                if ((ship.uPos - uPos).magnitude > (ships[i].uPos - uPos).magnitude)
                {
                    ship = ships[i];
                }
            }

            return ship;
        }

        public static void OnShipLanded(EnemyShip ship)
        {
            Main.logger.LogInfo("=========> Ship landed at station " + ship.shipData.otherGId);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {
            if (paused) return;

            List<EnemyShip> newList = new List<EnemyShip>();
            ships.Do(ship =>
            {
                ship.Update();
                if (ship.state == EnemyShip.State.active) newList.Add(ship);
                else if (ship.state == EnemyShip.State.landed) OnShipLanded(ship);
            });
            ships = newList;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogisticShipRenderer), "Update")]
        public static void LogisticShipRenderer_Update(ref LogisticShipRenderer __instance)
        {
            if (ships.Count == 0) return;
            if (__instance.transport == null) return;
            while (__instance.capacity < __instance.shipCount + ships.Count)
            {
                __instance.Expand2x();
            }

            for (int i = 0; i < ships.Count; ++i)
            {
                __instance.shipsArr[__instance.shipCount + i] = ships[i].renderingData;
            }
            __instance.shipCount += ships.Count;

            ref ComputeBuffer shipsBuffer = ref AccessTools.FieldRefAccess<LogisticShipRenderer, ComputeBuffer>(__instance, "shipsBuffer");
            if (shipsBuffer != null)
            {
                shipsBuffer.SetData(__instance.shipsArr, 0, 0, __instance.shipCount);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogisticShipUIRenderer), "Update")]
        public static void LogisticShipUIRenderer_Update(ref LogisticShipUIRenderer __instance)
        {
            if (ships.Count == 0) return;

            ref UIStarmap uiStarMap = ref AccessTools.FieldRefAccess<LogisticShipUIRenderer, UIStarmap>(__instance, "uiStarmap");
            ref ComputeBuffer shipsBuffer = ref AccessTools.FieldRefAccess<LogisticShipUIRenderer, ComputeBuffer>(__instance, "shipsBuffer");
            ref ShipUIRenderingData[] shipsArr = ref AccessTools.FieldRefAccess<LogisticShipUIRenderer, ShipUIRenderingData[]>(__instance, "shipsArr");
            ref int shipCount = ref AccessTools.FieldRefAccess<LogisticShipUIRenderer, int>(__instance, "shipCount");
            if (__instance.transport == null || uiStarMap == null || !uiStarMap.active) return;

            while (__instance.capacity < shipCount + ships.Count)
            {
                __instance.Expand2x();
            }

            for (int i = 0; i < ships.Count; ++i)
            {
                shipsArr[shipCount + i] = ships[i].renderingUIData;
                shipsArr[shipCount + i].rpos = (shipsArr[shipCount + i].upos - uiStarMap.viewTargetUPos) * 0.00025;
            }
            shipCount += ships.Count;

            if (shipsBuffer != null)
            {
                shipsBuffer.SetData(shipsArr, 0, 0, shipCount);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "OnDraw")]
        public static void GameData_OnDraw(ref GameData __instance, int frame)
        {
            if (__instance.galacticTransport != null && ships.Count != 0)
            {
                __instance.galacticTransport.shipRenderer.Update();
            }
        }
        public static void Export(BinaryWriter w)
        {
            w.Write(ships.Count);
            for (var i = 0; i < ships.Count; ++i)
            {
                ships[i].Export(w);
            }

        }

        public static void Import(BinaryReader r)
        {
            int cnt = r.ReadInt32();
            ships.Clear();
            for (var i = 0; i < cnt; ++i)
            {
                ships.Add(new EnemyShip(r));
            }
        }

        public static void IntoOtherSave()
        {
            ships.Clear();
        }
    }
}
