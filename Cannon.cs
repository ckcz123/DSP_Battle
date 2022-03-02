using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    class Cannon
    {
        public static DysonSwarm curSwarm;
        // public static DysonSwarm[] swarms;
        public static double BulletMaxtDivisor = 2000.0; // 原本5000.0
        public static float numMinus = 0.0f;
        public static VectorLF3 endPos = new VectorLF3(2000,2000,2000);
        public static bool doTrack = true;
        public static System.Random rand = new System.Random();

		/// <summary>
		/// 每帧调用刷新子弹终点
		/// </summary>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameData), "GameTick")]
		public static void GameData_GameTick(ref GameData __instance, long time)
        {
			if (!doTrack) return;

			// TODO: Find nearest ship
			EnemyShip ship = EnemyShips.FindNearestShip(VectorLF3.zero);
			if (ship == null) return;
			try
			{
				for (int i = 1; i < curSwarm.bulletCursor; i++)
				{
					curSwarm.bulletPool[i].uEnd = ship.uPos;// + new VectorLF3(rand.NextDouble()*100, rand.NextDouble() * 100, rand.NextDouble() * 100)
				}

			}
			catch (Exception)
			{
				// Main.logger.LogWarning("redirecting err");
			}
		}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static bool EjectorPatch(ref EjectorComponent __instance, float power, DysonSwarm swarm, AstroPose[] astroPoses, AnimData[] animPool, int[] consumeRegister, ref uint __result)
        {
			EnemyShip ship = EnemyShips.FindNearestShip(VectorLF3.zero);
			if (ship == null) return true;

			curSwarm = swarm;

			int planetId = __instance.planetId;
			PlanetFactory factory = GameMain.galaxy.stars[planetId / 100 - 1].planets[planetId % 100 - 1].factory;
			int gmProtoId = factory.entityPool[__instance.entityId].protoId;

			// if (gmProtoId != 9999) return true; // 如果不是自己自定义的炮，的建筑protoId，就返回原函数

			if (__instance.needs == null)
			{
				__instance.needs = new int[6];
			}
			__instance.needs[0] = ((__instance.bulletCount >= 20) ? 0 : __instance.bulletId);
			animPool[__instance.entityId].prepare_length = __instance.localDir.x;
			animPool[__instance.entityId].working_length = __instance.localDir.y;
			animPool[__instance.entityId].power = __instance.localDir.z;
			__instance.targetState = EjectorComponent.ETargetState.None;
			if (__instance.fired)
			{
				int num = __instance.entityId;
				animPool[num].time = animPool[num].time + 0.016666668f;
				if (animPool[__instance.entityId].time >= 11f)
				{
					__instance.fired = false;
					animPool[__instance.entityId].time = 0f;
				}
			}
			else if (__instance.direction > 0)
			{
				animPool[__instance.entityId].time = (float)__instance.time / (float)__instance.chargeSpend;
			}
			else if (__instance.direction < 0)
			{
				animPool[__instance.entityId].time = -(float)__instance.time / (float)__instance.coldSpend;
			}
			else
			{
				animPool[__instance.entityId].time = 0f;
			}
			if (__instance.orbitId < 0 || __instance.orbitId >= swarm.orbitCursor || swarm.orbits[__instance.orbitId].id != __instance.orbitId || !swarm.orbits[__instance.orbitId].enabled)
			{
				__instance.orbitId = 0;
			}
			float num2 = (float)Cargo.accTableMilli[__instance.incLevel];
			int num3 = (int)(power * 10000f * (1f + num2) + 0.1f);
			if (__instance.orbitId == 0)
			{
				if (__instance.direction == 1)
				{
					__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
					__instance.direction = -1;
				}
				if (__instance.direction == -1)
				{
					__instance.time -= num3;
					if (__instance.time <= 0)
					{
						__instance.time = 0;
						__instance.direction = 0;
					}
				}
				if (power >= 0.1f)
				{
					__instance.localDir.x = __instance.localDir.x * 0.9f;
					__instance.localDir.y = __instance.localDir.y * 0.9f;
					__instance.localDir.z = __instance.localDir.z * 0.9f + 0.1f;
					__result = 1U;
					return false;
				}
				__result = 0U;
				return false;
			}
			else
			{
				if (power < 0.1f)
				{
					if (__instance.direction == 1)
					{
						__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
						__instance.direction = -1;
					}
					__result = 0U;
					return false;
				}
				uint result = 0U;
				__instance.targetState = EjectorComponent.ETargetState.OK;
				bool flag = true;
				int num4 = __instance.planetId / 100 * 100;
				float num5 = __instance.localAlt + __instance.pivotY + (__instance.muzzleY - __instance.pivotY) / Mathf.Max(0.1f, Mathf.Sqrt(1f - __instance.localDir.y * __instance.localDir.y));
				Vector3 vector = new Vector3(__instance.localPosN.x * num5, __instance.localPosN.y * num5, __instance.localPosN.z * num5);
				VectorLF3 vectorLF = astroPoses[__instance.planetId].uPos + Maths.QRotateLF(astroPoses[__instance.planetId].uRot, vector);
				Quaternion q = astroPoses[__instance.planetId].uRot * __instance.localRot;
				VectorLF3 uPos = astroPoses[num4].uPos;
				VectorLF3 b = uPos - vectorLF;
				VectorLF3 vectorLF2 = uPos + VectorLF3.Cross(swarm.orbits[__instance.orbitId].up, b).normalized * (double)swarm.orbits[__instance.orbitId].radius;
                
				try //设定目标
                {
					vectorLF2 = ship.uPos;
				}
                catch (Exception)
                {
					return false;
                }

                //如果没有船/船没血了，就不打炮了
                try
                {
					if (ship.hp <= 0)
						return false;

				}
                catch (Exception)
                {
					return false;
                }


				VectorLF3 vectorLF3 = vectorLF2 - vectorLF;
				__instance.targetDist = vectorLF3.magnitude;
				vectorLF3.x /= __instance.targetDist;
				vectorLF3.y /= __instance.targetDist;
				vectorLF3.z /= __instance.targetDist;
				Vector3 vector2 = Maths.QInvRotate(q, vectorLF3);
				__instance.localDir.x = __instance.localDir.x * 0.9f + vector2.x * 0.1f;
				__instance.localDir.y = __instance.localDir.y * 0.9f + vector2.y * 0.1f;
				__instance.localDir.z = __instance.localDir.z * 0.9f + vector2.z * 0.1f;
				if ((double)vector2.y < 0.08715574 || vector2.y > 0.8660254f)
				{
					__instance.targetState = EjectorComponent.ETargetState.AngleLimit;
					flag = false;
				}
				bool flag2 = __instance.bulletCount > 0;
				if (flag2 && flag)
				{
					for (int i = num4 + 1; i <= __instance.planetId + 2; i++)
					{
						if (i != __instance.planetId)
						{
							double num6 = (double)astroPoses[i].uRadius;
							if (num6 > 1.0)
							{
								VectorLF3 vectorLF4 = astroPoses[i].uPos - vectorLF;
								double num7 = vectorLF4.x * vectorLF4.x + vectorLF4.y * vectorLF4.y + vectorLF4.z * vectorLF4.z;
								double num8 = vectorLF4.x * vectorLF3.x + vectorLF4.y * vectorLF3.y + vectorLF4.z * vectorLF3.z;
								if (num8 > 0.0)
								{
									double num9 = num7 - num8 * num8;
									num6 += 120.0;
									if (num9 < num6 * num6)
									{
										flag = false;
										__instance.targetState = EjectorComponent.ETargetState.Blocked;
										break;
									}
								}
							}
						}
					}
				}
				bool flag3 = flag && flag2;
				result = (flag2 ? (flag ? 4U : 3U) : 2U);
				if (__instance.direction == 1)
				{
					if (!flag3)
					{
						__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
						__instance.direction = -1;
					}
				}
				else if (__instance.direction == 0 && flag3)
				{
					__instance.direction = 1;
				}
				if (__instance.direction == 1)
				{
					__instance.time += num3;
					if (__instance.time >= __instance.chargeSpend)
					{
						__instance.fired = true;
						animPool[__instance.entityId].time = 10f;
						swarm.AddBullet(new SailBullet
						{
							maxt = (float)(__instance.targetDist / 5000.0),
							lBegin = vector,
							uEndVel = VectorLF3.Cross(vectorLF2 - uPos, swarm.orbits[__instance.orbitId].up).normalized * Math.Sqrt((double)(swarm.dysonSphere.gravity / swarm.orbits[__instance.orbitId].radius)),
							uBegin = vectorLF,
							uEnd = vectorLF2
						}, __instance.orbitId);
						__instance.bulletInc -= __instance.bulletInc / __instance.bulletCount;
						__instance.bulletCount--;
						if (__instance.bulletCount == 0)
						{
							__instance.bulletInc = 0;
						}
						lock (consumeRegister)
						{
							consumeRegister[__instance.bulletId]++;
						}
						__instance.time = __instance.coldSpend;
						__instance.direction = -1;
					}
				}
				else if (__instance.direction == -1)
				{
					__instance.time -= num3;
					if (__instance.time <= 0)
					{
						__instance.time = 0;
						__instance.direction = (flag3 ? 1 : 0);
					}
				}
				else
				{
					__instance.time = 0;
				}

				__result = result;
				return false;
			}
		}

        /// <summary>
        /// 创建后删除子弹
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__result"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSwarm), "AddSolarSail")]
        public static void GetSailIndexWhenAdd(ref DysonSwarm __instance, int __result)
        {
			// TODO: Find nearest ship
			EnemyShip ship = EnemyShips.FindNearestShip(VectorLF3.zero);
            try
            {
                __instance.RemoveSolarSail(__result);

				if (ship != null)
                {
					ship.BeAttacked(2);
                }
            }
            catch (Exception)
            {

            }
        }


       

    }

    
}
