using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    public class Ship
    {

        public static ShipData shipData = new ShipData();
        public static int targetStationGid = 2;
		public static ShipRenderingData renderingData = new ShipRenderingData();
		public static ShipUIRenderingData renderingUIData = new ShipUIRenderingData();
        
        public static void Init()
        {
			renderingData.SetEmpty();
			renderingData.gid = 1000;
			renderingData.SetEmpty();
			renderingUIData.gid = 1000;

            shipData = new ShipData();

            shipData.direction = 1;
            shipData.stage = 0;
            shipData.shipIndex = 1;
			shipData.uAngularVel = Vector3.zero;
			shipData.uAngularSpeed = 0;
            shipData.otherGId = targetStationGid;
            shipData.planetB = GameMain.data.galacticTransport.stationPool[targetStationGid].planetId;

            PlanetData planet = GameMain.galaxy.PlanetById(shipData.planetB);
            VectorLF3 starPos = planet.star.uPosition;
            VectorLF3 planetPos = planet.uPosition;
            shipData.uPos = (starPos + planetPos) / 2;
            shipData.uRot = GameMain.data.galaxy.astroPoses[shipData.planetB].uRot;

            Main.logger.LogInfo("==========> Started!");
            // MovePlayer();
        }

        public static void MovePlayer()
        {
            GameMain.mainPlayer.uPosition = shipData.uPos;
        }

		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameData), "GameTick")]
		public static void GameData_GameTick(ref GameData __instance, long time)
        {
			if (shipData.shipIndex == 0) return;
			Quaternion quaternion = Quaternion.identity;
			bool flag7 = false;
			if (shipData.stage == 0) UpdateStage0(out quaternion, out flag7);
			else if (shipData.stage == 1) UpdateStage1();
			else if (shipData.stage == 2) UpdateStage2();


			PlanetData planet = GameMain.galaxy.PlanetById(shipData.planetB);
			renderingData.SetPose(shipData.uPos, flag7 ? quaternion : shipData.uRot, GameMain.data.relativePos, GameMain.data.relativeRot, shipData.uVel * shipData.uSpeed, 6002);
			renderingUIData.SetPose(shipData.uPos, flag7 ? quaternion : shipData.uRot, (float)(planet.star.uPosition - planet.uPosition).magnitude, shipData.uSpeed, 6002);
            if (renderingData.anim.z < 0) renderingData.anim.z = 0;


            Main.logger.LogInfo("=======> " + flag7 + ", " + shipData.uPos+ ", " + renderingData.pos);
        }

		// Decompile the original code
		static void UpdateStage0(out Quaternion quaternion, out bool flag7)
        {
            float shipSailSpeed = GameMain.history.logisticShipSailSpeedModified;

            float num31 = Mathf.Sqrt(shipSailSpeed / 600f);
            float num32 = num31;
            if (num32 > 1f)
            {
                num32 = Mathf.Log(num32) + 1f;
            }
            float num33 = shipSailSpeed * 0.03f * num32;
            float num34 = shipSailSpeed * 0.12f * num32;
            float num35 = shipSailSpeed * 0.4f * num31;
            double dt = 0.016666666666666666;
            AstroPose[] astroPoses = GameMain.data.galaxy.astroPoses;
            StationComponent[] gStationPool = GameMain.data.galacticTransport.stationPool;

			quaternion = Quaternion.identity;
            flag7 = false;

            AstroPose astroPose2 = astroPoses[shipData.planetB];
            VectorLF3 lhs3 = astroPose2.uPos + Maths.QRotateLF(astroPose2.uRot, gStationPool[shipData.otherGId].shipDockPos + gStationPool[shipData.otherGId].shipDockPos.normalized * 25f);
            VectorLF3 vectorLF = lhs3 - shipData.uPos;
            double num38 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z);
            bool flag8 = false;
            bool flag9 = false;
            if (num38 < 6.0)
            {
                shipData.t = 1f;
                shipData.stage = shipData.direction;
                flag9 = true;
            }

            float num40 = 0f;

            double num46 = num38 / ((double)shipData.uSpeed + 0.1) * 0.382 * (double)num32;
            float num47 = 0f;
            if (shipData.warpState > 0f)
            {
                num47 = (shipData.uSpeed = shipSailSpeed + num40);
                if (num47 > shipSailSpeed)
                {
                    num47 = shipSailSpeed;
                }
            }
            else
            {
                float num48 = (float)((double)shipData.uSpeed * num46) + 6f;
                if (num48 > shipSailSpeed)
                {
                    num48 = shipSailSpeed;
                }

                float num49 = (float)dt * (flag8 ? num33 : num34);
                if (shipData.uSpeed < num48 - num49)
                {
                    shipData.uSpeed += num49;
                }
                else if (shipData.uSpeed > num48 + num35)
                {
                    shipData.uSpeed -= num35;
                }
                else
                {
                    shipData.uSpeed = num48;
                }
                num47 = shipData.uSpeed;
            }

            int num50 = -1;
            double rhs = 0.0;
            double num51 = 1E+40;
            int num52 = shipData.planetA / 100 * 100;
            int num53 = shipData.planetB / 100 * 100;
            for (int k = num52; k < num52 + 10; k++)
            {
                float uRadius = astroPoses[k].uRadius;
                if (!(uRadius < 1f))
                {
                    VectorLF3 vectorLF3 = shipData.uPos - astroPoses[k].uPos;
                    double num54 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
                    double num55 = 0.0 - ((double)shipData.uVel.x * vectorLF3.x + (double)shipData.uVel.y * vectorLF3.y + (double)shipData.uVel.z * vectorLF3.z);
                    if ((num55 > 0.0 || num54 < (double)(uRadius * uRadius * 7f)) && num54 < num51)
                    {
                        rhs = ((num55 < 0.0) ? 0.0 : num55);
                        num50 = k;
                        num51 = num54;
                    }
                }
            }

            if (num53 != num52)
            {
                for (int l = num53; l < num53 + 10; l++)
                {
                    float uRadius2 = astroPoses[l].uRadius;
                    if (!(uRadius2 < 1f))
                    {
                        VectorLF3 vectorLF4 = shipData.uPos - astroPoses[l].uPos;
                        double num56 = vectorLF4.x * vectorLF4.x + vectorLF4.y * vectorLF4.y + vectorLF4.z * vectorLF4.z;
                        double num57 = 0.0 - ((double)shipData.uVel.x * vectorLF4.x + (double)shipData.uVel.y * vectorLF4.y + (double)shipData.uVel.z * vectorLF4.z);
                        if ((num57 > 0.0 || num56 < (double)(uRadius2 * uRadius2 * 7f)) && num56 < num51)
                        {
                            rhs = ((num57 < 0.0) ? 0.0 : num57);
                            num50 = l;
                            num51 = num56;
                        }
                    }
                }
            }

            VectorLF3 zero = VectorLF3.zero;
            VectorLF3 rhs2 = VectorLF3.zero;
            float num58 = 0f;
            VectorLF3 vectorLF5 = Vector3.zero;
            if (num50 > 0)
            {
                float num59 = astroPoses[num50].uRadius;
                if (num50 % 100 == 0)
                {
                    num59 *= 2.5f;
                }

                double num60 = Math.Max(1.0, ((astroPoses[num50].uPosNext - astroPoses[num50].uPos).magnitude - 0.5) * 0.6);
                double num61 = 1.0 + 1600.0 / (double)num59;
                double num62 = 1.0 + 250.0 / (double)num59;
                num61 *= num60 * num60;
                double num63 = (num50 == shipData.planetA || num50 == shipData.planetB) ? 1.25f : 1.5f;
                double num64 = Math.Sqrt(num51);
                double num65 = (double)num59 / num64 * 1.6 - 0.1;
                if (num65 > 1.0)
                {
                    num65 = 1.0;
                }
                else if (num65 < 0.0)
                {
                    num65 = 0.0;
                }

                double num66 = num64 - (double)num59 * 0.82;
                if (num66 < 1.0)
                {
                    num66 = 1.0;
                }

                double num67 = (double)(num47 - 6f) / (num66 * (double)num32) * 0.6 - 0.01;
                if (num67 > 1.5)
                {
                    num67 = 1.5;
                }
                else if (num67 < 0.0)
                {
                    num67 = 0.0;
                }

                VectorLF3 vectorLF6 = shipData.uPos + (VectorLF3)shipData.uVel * rhs - astroPoses[num50].uPos;
                double num68 = vectorLF6.magnitude / (double)num59;
                if (num68 < num63)
                {
                    double num69 = (num68 - 1.0) / (num63 - 1.0);
                    if (num69 < 0.0)
                    {
                        num69 = 0.0;
                    }

                    num69 = 1.0 - num69 * num69;
                    rhs2 = vectorLF6.normalized * (num67 * num67 * num69 * 2.0 * (double)(1f - shipData.warpState));
                }

                VectorLF3 v = shipData.uPos - astroPoses[num50].uPos;
                VectorLF3 lhs4 = new VectorLF3(v.x / num64, v.y / num64, v.z / num64);
                zero += lhs4 * num65;
                num58 = (float)num65;
                double num70 = num64 / (double)num59;
                num70 *= num70;
                num70 = (num61 - num70) / (num61 - num62);
                if (num70 > 1.0)
                {
                    num70 = 1.0;
                }
                else if (num70 < 0.0)
                {
                    num70 = 0.0;
                }

                if (num70 > 0.0)
                {
                    VectorLF3 v2 = Maths.QInvRotateLF(astroPoses[num50].uRot, v);
                    VectorLF3 lhs5 = Maths.QRotateLF(astroPoses[num50].uRotNext, v2) + astroPoses[num50].uPosNext;
                    num70 = (3.0 - num70 - num70) * num70 * num70;
                    vectorLF5 = (lhs5 - shipData.uPos) * num70;
                }
            }

            shipData.uRot.ForwardUp(out shipData.uVel, out Vector3 up);
            Vector3 vector = up * (1f - num58) + (Vector3)zero * num58;
            vector -= Vector3.Dot(vector, shipData.uVel) * shipData.uVel;
            vector.Normalize();
            Vector3 vector2 = vectorLF.normalized + rhs2;
            Vector3 a = Vector3.Cross(shipData.uVel, vector2);
            float num71 = shipData.uVel.x * vector2.x + shipData.uVel.y * vector2.y + shipData.uVel.z * vector2.z;
            Vector3 a2 = Vector3.Cross(up, vector);
            float num72 = up.x * vector.x + up.y * vector.y + up.z * vector.z;
            if (num71 < 0f)
            {
                a = a.normalized;
            }

            if (num72 < 0f)
            {
                a2 = a2.normalized;
            }

            float d = (num46 < 3.0) ? ((3.25f - (float)num46) * 4f) : (num47 / shipSailSpeed * (flag8 ? 0.2f : 1f));
            a = a * d + a2 * 2f;
            Vector3 a3 = a - shipData.uAngularVel;
            float d2 = (a3.sqrMagnitude < 0.1f) ? 1f : 0.05f;
            shipData.uAngularVel += a3 * d2;
            double num73 = (double)shipData.uSpeed * dt;
            shipData.uPos.x = shipData.uPos.x + (double)shipData.uVel.x * num73 + vectorLF5.x;
            shipData.uPos.y = shipData.uPos.y + (double)shipData.uVel.y * num73 + vectorLF5.y;
            shipData.uPos.z = shipData.uPos.z + (double)shipData.uVel.z * num73 + vectorLF5.z;
            Vector3 normalized = shipData.uAngularVel.normalized;
            double num74 = (double)shipData.uAngularVel.magnitude * dt * 0.5;
            float w = (float)Math.Cos(num74);
            float num75 = (float)Math.Sin(num74);
            Quaternion lhs6 = new Quaternion(normalized.x * num75, normalized.y * num75, normalized.z * num75, w);
            shipData.uRot = lhs6 * shipData.uRot;
            if (shipData.warpState > 0f)
            {
                float num76 = shipData.warpState * shipData.warpState * shipData.warpState;
                shipData.uRot = Quaternion.Slerp(shipData.uRot, Quaternion.LookRotation(vector2, vector), num76);
                shipData.uAngularVel *= 1f - num76;
            }

            if (num38 < 100.0)
            {
                float num77 = 1f - (float)num38 / 100f;
                num77 = (3f - num77 - num77) * num77 * num77;
                num77 *= num77;
                quaternion = Quaternion.Slerp(shipData.uRot, astroPose2.uRot * (gStationPool[shipData.otherGId].shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f)), num77);

                flag7 = true;
            }

            if (flag9)
            {
                shipData.uRot = quaternion;
                shipData.pPosTemp = Maths.QInvRotateLF(astroPose2.uRot, shipData.uPos - astroPose2.uPos);
                shipData.pRotTemp = Quaternion.Inverse(astroPose2.uRot) * shipData.uRot;


                quaternion = Quaternion.identity;
                flag7 = false;
            }

            if (renderingData.anim.z > 1f)
            {
                renderingData.anim.z -= (float)dt * 0.3f;
            }
            else
            {
                renderingData.anim.z = 1f;
            }

            renderingData.anim.w = shipData.warpState;
        }

		static void UpdateStage1()
        {

			float shipSailSpeed = GameMain.history.logisticDroneSpeedModified;
            float num31 = Mathf.Sqrt(shipSailSpeed / 600f);
            float num36 = num31 * 0.006f + 1E-05f;
            AstroPose[] astroPoses = GameMain.data.galaxy.astroPoses;
			StationComponent[] gStationPool = GameMain.data.galacticTransport.stationPool;

            AstroPose astroPose3 = astroPoses[shipData.planetB];
            float num78 = 0f;
            if (shipData.direction > 0)
            {
                shipData.t -= num36 * 0.6666667f;
                num78 = shipData.t;
                if (shipData.t < 0f)
                {
                    shipData.t = 1f;
                    num78 = 0f;
                    shipData.stage = 2;
                }

                num78 = (3f - num78 - num78) * num78 * num78;
                float num79 = num78 * 2f;
                float num80 = num78 * 2f - 1f;
                VectorLF3 lhs7 = astroPose3.uPos + Maths.QRotateLF(astroPose3.uRot, gStationPool[shipData.otherGId].shipDockPos + gStationPool[shipData.otherGId].shipDockPos.normalized * 7.27000046f);
                if (num78 > 0.5f)
                {
                    VectorLF3 lhs8 = astroPose3.uPos + Maths.QRotateLF(astroPose3.uRot, shipData.pPosTemp);
                    shipData.uPos = lhs7 * (1f - num80) + lhs8 * num80;
                    shipData.uRot = astroPose3.uRot * Quaternion.Slerp(gStationPool[shipData.otherGId].shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f), shipData.pRotTemp, num80 * 1.5f - 0.5f);
                }
                else
                {
                    VectorLF3 lhs9 = astroPose3.uPos + Maths.QRotateLF(astroPose3.uRot, gStationPool[shipData.otherGId].shipDockPos + gStationPool[shipData.otherGId].shipDockPos.normalized * -14.4f);
                    shipData.uPos = lhs9 * (1f - num79) + lhs7 * num79;
                    shipData.uRot = astroPose3.uRot * (gStationPool[shipData.otherGId].shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f));
                }
            }
            else
            {
                shipData.t += num36;
                num78 = shipData.t;
                if (shipData.t > 1f)
                {
                    shipData.t = 1f;
                    num78 = 1f;
                    shipData.stage = 0;
                }

                num78 = (3f - num78 - num78) * num78 * num78;
                shipData.uPos = astroPose3.uPos + Maths.QRotateLF(astroPose3.uRot, gStationPool[shipData.otherGId].shipDockPos + gStationPool[shipData.otherGId].shipDockPos.normalized * (-14.4f + 39.4f * num78));
                shipData.uRot = astroPose3.uRot * (gStationPool[shipData.otherGId].shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f));
            }

            shipData.uVel.x = 0f;
            shipData.uVel.y = 0f;
            shipData.uVel.z = 0f;
            shipData.uSpeed = 0f;
            shipData.uAngularVel.x = 0f;
            shipData.uAngularVel.y = 0f;
            shipData.uAngularVel.z = 0f;
            shipData.uAngularSpeed = 0f;
            renderingData.anim.z = num78 * 1.7f - 0.7f;
        }
		static void UpdateStage2()
        {
			shipData.t -= 0.0334f;

			if (shipData.t <= 0f)
            {
				shipData.t = 0f;
				shipData.shipIndex = 0;
				Main.logger.LogInfo("==========> Finished!");
				return;
            }

			AstroPose[] astroPoses = GameMain.data.galaxy.astroPoses;
			StationComponent[] gStationPool = GameMain.data.galacticTransport.stationPool;

            AstroPose astroPose4 = astroPoses[shipData.planetB];
            shipData.uPos = astroPose4.uPos + Maths.QRotateLF(astroPose4.uRot, gStationPool[shipData.otherGId].shipDockPos + gStationPool[shipData.otherGId].shipDockPos.normalized * -14.4f);
            shipData.uVel.x = 0f;
            shipData.uVel.y = 0f;
            shipData.uVel.z = 0f;
            shipData.uSpeed = 0f;
            shipData.uRot = astroPose4.uRot * (gStationPool[shipData.otherGId].shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f));
            shipData.uAngularVel.x = 0f;
            shipData.uAngularVel.y = 0f;
            shipData.uAngularVel.z = 0f;
            shipData.uAngularSpeed = 0f;
            shipData.pPosTemp = Vector3.zero;
            shipData.pRotTemp = Quaternion.identity;
            renderingData.anim.z = 0;

        }

		[HarmonyPostfix]
		[HarmonyPatch(typeof(LogisticShipRenderer), "Update")]
		public static void LogisticShipRenderer_Update(ref LogisticShipRenderer __instance)
		{
			if (shipData.shipIndex == 0) return;
			if (__instance.transport == null) return;
			if (__instance.capacity < __instance.shipCount + 1)
			{
				__instance.Expand2x();
			}

			__instance.shipsArr[__instance.shipCount] = renderingData;
			__instance.shipCount++;

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
			if (shipData.shipIndex == 0) return;

			ref UIStarmap uiStarMap = ref AccessTools.FieldRefAccess<LogisticShipUIRenderer, UIStarmap>(__instance, "uiStarmap");
			ref ComputeBuffer shipsBuffer = ref AccessTools.FieldRefAccess<LogisticShipUIRenderer, ComputeBuffer>(__instance, "shipsBuffer");
			ref ShipUIRenderingData[] shipsArr = ref AccessTools.FieldRefAccess<LogisticShipUIRenderer, ShipUIRenderingData[]>(__instance, "shipsArr");
			ref int shipCount = ref AccessTools.FieldRefAccess<LogisticShipUIRenderer, int>(__instance, "shipCount");
			if (__instance.transport == null || uiStarMap == null || !uiStarMap.active) return;

			if (__instance.capacity < shipCount + 1)
			{
				__instance.Expand2x();
			}

            renderingUIData.rpos = (renderingUIData.upos - uiStarMap.viewTargetUPos) * 0.00025;

            shipsArr[shipCount++] = renderingUIData;

			if (shipsBuffer != null)
			{
				shipsBuffer.SetData(shipsArr, 0, 0, shipCount);
			}
		}

    }
}
