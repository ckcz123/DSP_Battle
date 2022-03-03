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

        public static Dictionary<int, EnemyShip> ships = new Dictionary<int, EnemyShip>();

        public static System.Random gidRandom = new System.Random();
        public static bool paused = false;
        public static List<List<EnemyShip>> sortedShips = SortShips();

        public static void Create(int stationGid, VectorLF3 initPos, int initHp, int itemId = 0)
        {
            int nextGid = gidRandom.Next(1 << 27, 1 << 29);
            while (ships.ContainsKey(nextGid)) nextGid = gidRandom.Next(1 << 27, 1 << 29);

            EnemyShip enemyShip = new EnemyShip(
                nextGid,
                stationGid,
                initPos,
                initHp,
                GameMain.history.logisticShipSailSpeedModified,
                itemId);
            Main.logger.LogInfo("=========> Init ship " + nextGid + " at station " + enemyShip.shipData.otherGId);

            ships.Add(nextGid, enemyShip);
        }

        public static List<List<EnemyShip>> SortShips()
        {
            List<List<EnemyShip>> sortedShips = new List<List<EnemyShip>>(100);
            for (var i = 0; i < 100; ++i) sortedShips.Add(new List<EnemyShip>());
            if (ships.Count == 0) return sortedShips;

            List<KeyValuePair<double, EnemyShip>> distance = new List<KeyValuePair<double, EnemyShip>>();
            foreach (EnemyShip ship in ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                distance.Add(new KeyValuePair<double, EnemyShip>(ship.distanceToTarget, ship));
            }
            distance.Sort((v1, v2) => Math.Sign(v1.Key - v2.Key));
            Main.logger.LogInfo("=====> Sort ship: " + distance.Select(v => v.Value.shipIndex + ":" + v.Key).Join(null, "; "));
            foreach (var v in distance)
            {
                sortedShips[v.Value.starIndex].Add(v.Value);
            }
            return sortedShips;
        }

        /*
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
        */

        public static void OnShipLanded(EnemyShip ship)
        {
            Main.logger.LogInfo("=========> Ship landed at station " + ship.shipData.otherGId);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void GameData_GameTick(ref GameData __instance, long time)
        {
            if (paused) return;

            List<EnemyShip> list = ships.Values.ToList();

            bool hasRemoved = false;

            list.Do(ship =>
            {
                ship.Update();
                if (ship.state != EnemyShip.State.active)
                {
                    if (ship.state == EnemyShip.State.landed) OnShipLanded(ship);
                    ships.Remove(ship.shipIndex);
                    hasRemoved = true;
                }
            });

            // time is the frame since start
            if (hasRemoved || time % 60 == 1)
            {
                sortedShips = SortShips();
            }
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

            foreach (var ship in ships.Values)
            {
                __instance.shipsArr[__instance.shipCount ++] = ship.renderingData;
            }

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

            foreach (var ship in ships.Values)
            {
                shipsArr[shipCount] = ship.renderingUIData;
                shipsArr[shipCount].rpos = (shipsArr[shipCount].upos - uiStarMap.viewTargetUPos) * 0.00025;
                shipCount++;
            }

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
            ships.Values.Do(ship => ship.Export(w));

        }

        public static void Import(BinaryReader r)
        {
            int cnt = r.ReadInt32();
            ships.Clear();
            for (var i = 0; i < cnt; ++i)
            {
                EnemyShip ship = new EnemyShip(r);
                ships.Add(ship.shipIndex, ship);
            }
            sortedShips = SortShips();
        }

        public static void IntoOtherSave()
        {
            ships.Clear();
            sortedShips = SortShips();
        }
    }
}
