using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Collections.Concurrent;

namespace DSP_Battle
{
    class MissileSilo
    {
		public static List<ConcurrentDictionary<int, int>> MissileTargets; //记录导弹的目标
		public static List<ConcurrentDictionary<int, int>> canDoDamage; //记录导弹还能造成多少伤害


		public static float missileMaxSpeed = 5000;
		public static float missileSpeedUp = 50;

		//以下数值尽量不要改动
		//public static double distIntoTrackStage2;

		public static void ReInitAll()
        {
			MissileTargets = new List<ConcurrentDictionary<int, int>>();
			canDoDamage = new List<ConcurrentDictionary<int, int>>();
			for (int i = 0; i < GameMain.galaxy.starCount; i++)
			{
				MissileTargets.Add(new ConcurrentDictionary<int, int>());
				canDoDamage.Add(new ConcurrentDictionary<int, int>());
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameData), "GameTick")]
		public static void MissileTrack()
        {
        }



		[HarmonyPrefix]
        [HarmonyPatch(typeof(SiloComponent), "InternalUpdate")]
        public static bool SiloPatch(ref SiloComponent __instance, float power, DysonSphere sphere, AnimData[] animPool, int[] consumeRegister, uint __result)
        {
			int planetId = __instance.planetId;
			int starIndex = planetId / 100 - 1;
			PlanetFactory factory = GameMain.galaxy.stars[starIndex].planets[planetId % 100 - 1].factory;
			int gmProtoId = factory.entityPool[__instance.entityId].protoId;
			if (gmProtoId == 2312) return true; //要改的！！！改成原始发射井返回原函数

			if (__instance.needs == null)
			{
				__instance.needs = new int[6];
			}
			__instance.needs[0] = ((__instance.bulletCount >= 20) ? 0 : __instance.bulletId);
			if (__instance.fired && __instance.direction != -1)
			{
				__instance.fired = false;
			}
			if (__instance.direction == 1)
			{
				animPool[__instance.entityId].time = (float)__instance.time / (float)__instance.chargeSpend;
			}
			else if (__instance.direction == -1)
			{
				animPool[__instance.entityId].time = -(float)__instance.time / (float)__instance.coldSpend;
			}
			animPool[__instance.entityId].power = power;
			float num = (float)Cargo.accTableMilli[__instance.incLevel];
			num = 4;
			int num2 = (int)(power * 10000f * (1f + num) + 0.1f);
			Mutex dysonSphere_mx = sphere.dysonSphere_mx;
			uint result;
			lock (dysonSphere_mx)
			{
				//下面设定目标，目前是随机设定目标
				int targetIndex = 0;
				if (EnemyShips.minTargetDisSortedShips[starIndex].Count > 0) targetIndex = EnemyShips.minTargetDisSortedShips[starIndex][DspBattlePlugin.randSeed.Next(0, EnemyShips.minTargetDisSortedShips[starIndex].Count)].shipIndex;

				__instance.hasNode = (sphere.GetAutoNodeCount() > 0);
				if(targetIndex == 0)  //if (!__instance.hasNode) 原本是没有节点，因此不发射
				{
					__instance.autoIndex = 0;
					if (__instance.direction == 1)
					{
						__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
						__instance.direction = -1;
					}
					if (__instance.direction == -1)
					{
						__instance.time -= num2;
						if (__instance.time <= 0)
						{
							__instance.time = 0;
							__instance.direction = 0;
						}
					}
					if (power >= 0.1f)
					{
						result = 1U;
					}
					else
					{
						result = 0U;
					}
				}
				else if (power < 0.1f)
				{
					if (__instance.direction == 1)
					{
						__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
						__instance.direction = -1;
					}
					result = 0U;
				}
				else
				{
					uint num3 = 0U;
					bool flag2;
					num3 = ((flag2 = (__instance.bulletCount > 0)) ? 3U : 2U);
					if (__instance.direction == 1)
					{
						if (!flag2)
						{
							__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
							__instance.direction = -1;
						}
					}
					else if (__instance.direction == 0 && flag2)
					{
						__instance.direction = 1;
					}
					if (__instance.direction == 1)
					{
						__instance.time += num2;
						if (__instance.time >= __instance.chargeSpend)
						{
							AstroPose[] astroPoses = sphere.starData.galaxy.astroPoses;
							__instance.fired = true;
							//DysonNode autoDysonNode = sphere.GetAutoDysonNode(__instance.autoIndex + __instance.id); //原本获取目标节点，现在已不需要
							DysonRocket dysonRocket = default(DysonRocket);
							dysonRocket.planetId = __instance.planetId;
							dysonRocket.uPos = astroPoses[__instance.planetId].uPos + Maths.QRotateLF(astroPoses[__instance.planetId].uRot, __instance.localPos + __instance.localPos.normalized * 6.1f);
							dysonRocket.uRot = astroPoses[__instance.planetId].uRot * __instance.localRot * Quaternion.Euler(-90f, 0f, 0f);
							dysonRocket.uVel = dysonRocket.uRot * Vector3.forward;
							dysonRocket.uSpeed = 0f;
							dysonRocket.launch = __instance.localPos.normalized;
							//sphere.AddDysonRocket(dysonRocket, autoDysonNode); //原本
							int rocketIndex = AddDysonRockedGniMaerd(ref sphere, ref dysonRocket, null); //这是添加了一个目标戴森球节点为null的火箭，因此被判定为导弹

							MissileTargets[starIndex][rocketIndex] = targetIndex;
							canDoDamage[starIndex][rocketIndex] = 1250;

							__instance.autoIndex++;
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
						__instance.time -= num2;
						if (__instance.time <= 0)
						{
							__instance.time = 0;
							__instance.direction = (flag2 ? 1 : 0);
						}
					}
					else
					{
						__instance.time = 0;
					}
					result = num3;
				}
			}
			__result = result;
			return false;
		}


		[HarmonyPrefix]
		[HarmonyPatch(typeof(DysonSphere), "RocketGameTick",new Type[] { })]
		public static bool RocketGameTickNoThreadPatch(ref DysonSphere __instance)
		{
			AstroPose[] astroPoses = __instance.starData.galaxy.astroPoses;
			double num = 0.016666666666666666;
			float num2 = Mathf.Max(1f, (float)Math.Pow((double)__instance.defOrbitRadius / 40000.0 * 4.0, 0.4));
			float num3 = 7.5f;
			float num4 = 18f * num2;
			float num5 = 2800f * num2;
			for (int i = 1; i < __instance.rocketCursor; i++)
			{
				if (__instance.rocketPool[i].id == i)
				{
					DysonRocket dysonRocket = __instance.rocketPool[i];

					bool isMissile = dysonRocket.node == null;//只有null是导弹，其他的是正常的戴森火箭
					int starIndex = __instance.starData.index;

					if (isMissile)
					{
						//DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[dysonRocket.node.layerId];
						AstroPose astroPose = astroPoses[dysonRocket.planetId];
						VectorLF3 vectorLF = astroPose.uPos - dysonRocket.uPos;
						double num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
						if (dysonRocket.t <= 0f) //如果离地面很近，是个加速从地面直线向上冲的过程
						{
							if (num8 < 200.0) //如果离地面很近，是个加速从地面直线向上冲的过程
							{
								float num9 = (float)num8 / 200f;
								if (num9 < 0f)
								{
									num9 = 0f;
								}
								float num10 = num9 * num9 * 600f + 15f;
								dysonRocket.uSpeed = dysonRocket.uSpeed * 0.9f + num10 * 0.1f;
								dysonRocket.t = (num9 - 1f) * 1.2f;
								if (dysonRocket.t < -1f)
								{
									dysonRocket.t = -1f;
								}
							}
							else //离地面超过200
							{

								VectorLF3 vectorLF2 = dysonRocket.uPos;
								//根据是导弹还是火箭确定
								if (EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))//如果以前的目标敌舰还存在
								{
									vectorLF2 = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
								}
								else if (EnemyShips.minTargetDisSortedShips[starIndex].Count > 0) //否则，火箭继续寻敌
								{
									int newTargetId = EnemyShips.minTargetDisSortedShips[starIndex][DspBattlePlugin.randSeed.Next(0, EnemyShips.minTargetDisSortedShips[starIndex].Count)].shipIndex;
									MissileTargets[starIndex][i] = newTargetId;
									vectorLF2 = EnemyShips.ships[newTargetId].uPos - dysonRocket.uPos;
									dysonRocket.t = 0; //让其回到第一阶段，允许避障
								}
								else //如果一个敌人都没有，火箭就地自毁
								{
									__instance.RemoveDysonRocket(i);
									goto IL_BDF;
								}

								//根据距离地表的距离设置速度，被我改成一直加速了
								double num11 = Math.Sqrt(vectorLF2.x * vectorLF2.x + vectorLF2.y * vectorLF2.y + vectorLF2.z * vectorLF2.z);
								if (num11 < 2000.0)
								{
									dysonRocket.t = 0.0001f;
								}
								else
								{
									dysonRocket.t = 0f;
								}
								double num12 = num11 / ((double)dysonRocket.uSpeed + 0.1) * 0.382;
								double num13 = num11 / (double)num5;
								float num14 = (float)((double)dysonRocket.uSpeed * num12) + 150f;
								if (num14 > num5)
								{
									num14 = num5;
								}
								if (dysonRocket.uSpeed < missileMaxSpeed)
								{
									dysonRocket.uSpeed += missileSpeedUp;
								}
								//else if (dysonRocket.uSpeed > num14 + num4)
								//{
								//	dysonRocket.uSpeed -= num4;
								//}
								else
								{
									dysonRocket.uSpeed = missileMaxSpeed;
								}

								//下面难道是躲避巨星？
								int num15 = -1;
								double rhs = 0.0;
								double num16 = 1E+40;
								int num17 = dysonRocket.planetId / 100 * 100;
								for (int j = num17; j < num17 + 10; j++)
								{
									float uRadius = astroPoses[j].uRadius;
									if (uRadius >= 1f)
									{
										VectorLF3 vectorLF3 = dysonRocket.uPos - astroPoses[j].uPos;
										double num18 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
										double num19 = -((double)dysonRocket.uVel.x * vectorLF3.x + (double)dysonRocket.uVel.y * vectorLF3.y + (double)dysonRocket.uVel.z * vectorLF3.z);
										if ((num19 > 0.0 || num18 < (double)(uRadius * uRadius * 7f)) && num18 < num16)
										{
											rhs = ((num19 < 0.0) ? 0.0 : num19);
											num15 = j;
											num16 = num18;
										}
									}
								}
								VectorLF3 rhs2 = VectorLF3.zero;
								float num20 = 0f;
								if (num15 > 0)
								{
									float num21 = astroPoses[num15].uRadius;
									bool flag = num15 % 100 == 0;
									if (flag)
									{
										num21 = 1000 - 400f; //dysonSphereLayer.orbitRadius - 400f
									}
									double num22 = 1.25;
									VectorLF3 vectorLF4 = dysonRocket.uPos + (VectorLF3)dysonRocket.uVel * rhs - astroPoses[num15].uPos;
									double num23 = vectorLF4.magnitude / (double)num21;
									if (num23 < num22)
									{
										double num24 = Math.Sqrt(num16) - (double)num21 * 0.82;
										if (num24 < 1.0)
										{
											num24 = 1.0;
										}
										double num25 = (num23 - 1.0) / (num22 - 1.0);
										if (num25 < 0.0)
										{
											num25 = 0.0;
										}
										num25 = 1.0 - num25 * num25;
										double num26 = (double)(dysonRocket.uSpeed - 6f) / num24 * 2.5 - 0.01;
										if (num26 > 1.5)
										{
											num26 = 1.5;
										}
										else if (num26 < 0.0)
										{
											num26 = 0.0;
										}
										num26 = num26 * num26 * num25;
										num20 = (float)(flag ? 0.0 : (num26 * 0.5));
										rhs2 = vectorLF4.normalized * num26 * 2.0;
									}
								}
								float num27 = 1f / (float)num13 - 0.05f;
								num27 += num20;
								float t = Mathf.Lerp(0.005f, 0.08f, num27);
								dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF2.normalized + rhs2, t).normalized;
								Quaternion b;
								if (num11 < 350.0)
								{
									float t2 = ((float)num11 - 50f) / 300f;
									b = Quaternion.Slerp(Quaternion.LookRotation(vectorLF2), Quaternion.LookRotation(dysonRocket.uVel), t2);//b = Quaternion.Slerp(dysonSphereLayer.NodeURot(dysonRocket.node), Quaternion.LookRotation(dysonRocket.uVel), t2);
								}
								else
								{
									b = Quaternion.LookRotation(dysonRocket.uVel);
								}
								if (vectorLF2.magnitude < 1000) //如果离得很近，则增大转弯速度
								{
									dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.4f);
								}
								else if (vectorLF2.magnitude < 3000)
								{
									dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.3f);
								}
								else
								{
									dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.2f);
								}

							}
						}
						else
						{


							VectorLF3 vectorLF5 = dysonRocket.uPos;
							//之前的目标是否还存活
							if (EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))//如果以前的目标敌舰还存在
							{
								vectorLF5 = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
							}
							else if (EnemyShips.minTargetDisSortedShips[starIndex].Count > 0) //否则，火箭继续寻敌
							{
								int newTargetId = EnemyShips.minTargetDisSortedShips[starIndex][DspBattlePlugin.randSeed.Next(0, EnemyShips.minTargetDisSortedShips[starIndex].Count)].shipIndex;
								MissileTargets[starIndex][i] = newTargetId;
								vectorLF5 = EnemyShips.ships[newTargetId].uPos - dysonRocket.uPos;
								dysonRocket.t = 0; //让其回到第一阶段，允许避障
							}
							else //如果一个敌人都没有，火箭就地自毁
							{
								__instance.RemoveDysonRocket(i);
								goto IL_BDF;
							}



							double num28 = Math.Sqrt(vectorLF5.x * vectorLF5.x + vectorLF5.y * vectorLF5.y + vectorLF5.z * vectorLF5.z);
							if (num28 < 100.0)
							{
								//借助太阳帆弹射的效果触发爆炸动画
								int bulletIndex = __instance.swarm.AddBullet(new SailBullet
								{
									maxt = 0.000f,
									lBegin = dysonRocket.uPos,
									uEndVel = dysonRocket.uPos, 
									uBegin = dysonRocket.uPos,
									uEnd = vectorLF5 + dysonRocket.uPos
								}, 1);

								__instance.swarm.bulletPool[bulletIndex].state = 0;
								//范围伤害
								var shipsHit = EnemyShips.FindShipsInRange(dysonRocket.uPos, 500);
								foreach (var item in shipsHit)
								{
									if (EnemyShips.ships.ContainsKey(item))
										EnemyShips.ships[item].BeAttacked(canDoDamage[starIndex][i]);
								}
								canDoDamage[starIndex][i] = 0;
								__instance.RemoveDysonRocket(i);
								goto IL_BDF;
							}
							float num29 = (float)(num28 * 0.75 + 15.0);
							if (num29 > num5)
							{
								num29 = num5;
							}
							if (dysonRocket.uSpeed < missileMaxSpeed)
							{
								dysonRocket.uSpeed += missileSpeedUp;
							}
							//else if (dysonRocket.uSpeed > num29 + num4)
							//{
							//	dysonRocket.uSpeed -= num4;
							//}
							else
							{
								dysonRocket.uSpeed = missileMaxSpeed;
							}
							dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF5.normalized, 0.1f);
							//dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.2f);
							if (vectorLF5.magnitude < 1000) //如果离得很近，则增大转弯速度
							{
								dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.4f);
							}
							else if (vectorLF5.magnitude < 3000)
							{
								dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.3f);
							}
							else
							{
								dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.2f);
							}

							dysonRocket.t = (350f - (float)num28) / 330f;
							if (dysonRocket.t > 1f)
							{
								dysonRocket.t = 1f;
							}
							else if (dysonRocket.t < 0.0001f)
							{
								dysonRocket.t = 0.0001f;
							}
						}


						VectorLF3 vectorLF6 = Vector3.zero;
						bool flag2 = false;
						double num30 = (double)(2f - (float)num8 / 200f);
						if (num30 > 1.0)
						{
							num30 = 1.0;
						}
						else if (num30 < 0.0)
						{
							num30 = 0.0;
						}
						if (num30 > 0.0)
						{
							VectorLF3 v = dysonRocket.uPos - astroPose.uPos;
							VectorLF3 v2 = Maths.QInvRotateLF(astroPose.uRot, v);
							VectorLF3 lhs = Maths.QRotateLF(astroPose.uRotNext, v2) + astroPose.uPosNext;
							Quaternion rhs3 = Quaternion.Inverse(astroPose.uRot) * dysonRocket.uRot;
							Quaternion b2 = astroPose.uRotNext * rhs3;
							num30 = (3.0 - num30 - num30) * num30 * num30;
							vectorLF6 = (lhs - dysonRocket.uPos) * num30;
							dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b2, (float)num30);
							flag2 = true;
						}
						
						double num33 = (double)dysonRocket.uSpeed * num;
						//下面很关键，如果导弹离目标过于近，则无视旋转直接直线飞过去
						if (MissileTargets[starIndex].ContainsKey(i) && EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))
						{
							VectorLF3 toTarget = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
							double distance = toTarget.magnitude;
							if (distance < num33) //距离小于一帧的量
							{
								dysonRocket.uPos.x += toTarget.x;
								dysonRocket.uPos.y += toTarget.y;
								dysonRocket.uPos.z += toTarget.z;
							}
							else if (distance < 1000) //直线过去
							{
								dysonRocket.uPos = dysonRocket.uPos + toTarget.normalized * num33;
							}
							else //这是原始规则
							{
								dysonRocket.uPos.x = dysonRocket.uPos.x + (double)dysonRocket.uVel.x * num33 + vectorLF6.x;
								dysonRocket.uPos.y = dysonRocket.uPos.y + (double)dysonRocket.uVel.y * num33 + vectorLF6.y;
								dysonRocket.uPos.z = dysonRocket.uPos.z + (double)dysonRocket.uVel.z * num33 + vectorLF6.z;
							}

						}
						vectorLF = astroPose.uPos - dysonRocket.uPos;
						num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
						if (num8 < 180.0)
						{
							dysonRocket.uPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, (VectorLF3)dysonRocket.launch * ((double)astroPose.uRadius + num8));
							dysonRocket.uRot = astroPose.uRot * Quaternion.LookRotation(dysonRocket.launch);
						}
						__instance.rocketPool[i] = dysonRocket;



					}
					else//普通火箭不要动！！！！！！！！！！！
					{

						DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[dysonRocket.node.layerId];
						AstroPose astroPose = astroPoses[dysonRocket.planetId];
						VectorLF3 vectorLF = astroPose.uPos - dysonRocket.uPos;
						double num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
						if (dysonRocket.t <= 0f) //如果离地面很近，是个加速从地面直线向上冲的过程
						{
							if (num8 < 200.0) //如果离地面很近，是个加速从地面直线向上冲的过程
							{
								float num9 = (float)num8 / 200f;
								if (num9 < 0f)
								{
									num9 = 0f;
								}
								float num10 = num9 * num9 * 600f + 15f;
								dysonRocket.uSpeed = dysonRocket.uSpeed * 0.9f + num10 * 0.1f;
								dysonRocket.t = (num9 - 1f) * 1.2f;
								if (dysonRocket.t < -1f)
								{
									dysonRocket.t = -1f;
								}
							}
							else //离地面超过200
							{
								VectorLF3 vectorLF2 = dysonSphereLayer.NodeEnterUPos(dysonRocket.node) - dysonRocket.uPos;

								double num11 = Math.Sqrt(vectorLF2.x * vectorLF2.x + vectorLF2.y * vectorLF2.y + vectorLF2.z * vectorLF2.z);
								if (num11 < 50.0)
								{
									dysonRocket.t = 0.0001f;
								}
								else
								{
									dysonRocket.t = 0f;
								}
								double num12 = num11 / ((double)dysonRocket.uSpeed + 0.1) * 0.382;
								double num13 = num11 / (double)num5;
								float num14 = (float)((double)dysonRocket.uSpeed * num12) + 150f;
								if (num14 > num5)
								{
									num14 = num5;
								}
								if (dysonRocket.uSpeed < num14 - num3)
								{
									dysonRocket.uSpeed += num3;
								}
								else if (dysonRocket.uSpeed > num14 + num4)
								{
									dysonRocket.uSpeed -= num4;
								}
								else
								{
									dysonRocket.uSpeed = num14;
								}
								int num15 = -1;
								double rhs = 0.0;
								double num16 = 1E+40;
								int num17 = dysonRocket.planetId / 100 * 100;
								for (int j = num17; j < num17 + 10; j++)
								{
									float uRadius = astroPoses[j].uRadius;
									if (uRadius >= 1f)
									{
										VectorLF3 vectorLF3 = dysonRocket.uPos - astroPoses[j].uPos;
										double num18 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
										double num19 = -((double)dysonRocket.uVel.x * vectorLF3.x + (double)dysonRocket.uVel.y * vectorLF3.y + (double)dysonRocket.uVel.z * vectorLF3.z);
										if ((num19 > 0.0 || num18 < (double)(uRadius * uRadius * 7f)) && num18 < num16)
										{
											rhs = ((num19 < 0.0) ? 0.0 : num19);
											num15 = j;
											num16 = num18;
										}
									}
								}
								VectorLF3 rhs2 = VectorLF3.zero;
								float num20 = 0f;
								if (num15 > 0)
								{
									float num21 = astroPoses[num15].uRadius;
									bool flag = num15 % 100 == 0;
									if (flag)
									{
										num21 = dysonSphereLayer.orbitRadius - 400f;
									}
									double num22 = 1.25;
									VectorLF3 vectorLF4 = dysonRocket.uPos + (VectorLF3)dysonRocket.uVel * rhs - astroPoses[num15].uPos;
									double num23 = vectorLF4.magnitude / (double)num21;
									if (num23 < num22)
									{
										double num24 = Math.Sqrt(num16) - (double)num21 * 0.82;
										if (num24 < 1.0)
										{
											num24 = 1.0;
										}
										double num25 = (num23 - 1.0) / (num22 - 1.0);
										if (num25 < 0.0)
										{
											num25 = 0.0;
										}
										num25 = 1.0 - num25 * num25;
										double num26 = (double)(dysonRocket.uSpeed - 6f) / num24 * 2.5 - 0.01;
										if (num26 > 1.5)
										{
											num26 = 1.5;
										}
										else if (num26 < 0.0)
										{
											num26 = 0.0;
										}
										num26 = num26 * num26 * num25;
										num20 = (float)(flag ? 0.0 : (num26 * 0.5));
										rhs2 = vectorLF4.normalized * num26 * 2.0;
									}
								}
								float num27 = 1f / (float)num13 - 0.05f;
								num27 += num20;
								float t = Mathf.Lerp(0.005f, 0.08f, num27);
								dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF2.normalized + rhs2, t).normalized;
								Quaternion b;
								if (num11 < 350.0)
								{
									float t2 = ((float)num11 - 50f) / 300f;
									b = Quaternion.Slerp(dysonSphereLayer.NodeURot(dysonRocket.node), Quaternion.LookRotation(dysonRocket.uVel), t2);
								}
								else
								{
									b = Quaternion.LookRotation(dysonRocket.uVel);
								}
								dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.2f);
							}
						}
						else
						{
							VectorLF3 vectorLF5 = dysonSphereLayer.NodeSlotUPos(dysonRocket.node) - dysonRocket.uPos;
							double num28 = Math.Sqrt(vectorLF5.x * vectorLF5.x + vectorLF5.y * vectorLF5.y + vectorLF5.z * vectorLF5.z);
							if (num28 < 2.0)
							{
								__instance.ConstructSp(dysonRocket.node);
								__instance.RemoveDysonRocket(i);
								goto IL_BDF;
							}
							float num29 = (float)(num28 * 0.75 + 15.0);
							if (num29 > num5)
							{
								num29 = num5;
							}
							if (dysonRocket.uSpeed < num29 - num3)
							{
								dysonRocket.uSpeed += num3;
							}
							else if (dysonRocket.uSpeed > num29 + num4)
							{
								dysonRocket.uSpeed -= num4;
							}
							else
							{
								dysonRocket.uSpeed = num29;
							}
							dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF5.normalized, 0.1f);
							dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, dysonSphereLayer.NodeURot(dysonRocket.node), 0.2f);
							dysonRocket.t = (350f - (float)num28) / 330f;
							if (dysonRocket.t > 1f)
							{
								dysonRocket.t = 1f;
							}
							else if (dysonRocket.t < 0.0001f)
							{
								dysonRocket.t = 0.0001f;
							}
						}
						VectorLF3 vectorLF6 = Vector3.zero;
						bool flag2 = false;
						double num30 = (double)(2f - (float)num8 / 200f);
						if (num30 > 1.0)
						{
							num30 = 1.0;
						}
						else if (num30 < 0.0)
						{
							num30 = 0.0;
						}
						if (num30 > 0.0)
						{
							VectorLF3 v = dysonRocket.uPos - astroPose.uPos;
							VectorLF3 v2 = Maths.QInvRotateLF(astroPose.uRot, v);
							VectorLF3 lhs = Maths.QRotateLF(astroPose.uRotNext, v2) + astroPose.uPosNext;
							Quaternion rhs3 = Quaternion.Inverse(astroPose.uRot) * dysonRocket.uRot;
							Quaternion b2 = astroPose.uRotNext * rhs3;
							num30 = (3.0 - num30 - num30) * num30 * num30;
							vectorLF6 = (lhs - dysonRocket.uPos) * num30;
							dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b2, (float)num30);
							flag2 = true;
						}
						if (!flag2)
						{
							VectorLF3 vectorLF7 = dysonRocket.uPos - __instance.starData.uPosition;
							double num31 = Math.Abs(Math.Sqrt(vectorLF7.x * vectorLF7.x + vectorLF7.y * vectorLF7.y + vectorLF7.z * vectorLF7.z) - (double)dysonSphereLayer.orbitRadius);
							double num32 = 1.5 - (double)((float)num31 / 1800f);
							if (num32 > 1.0)
							{
								num32 = 1.0;
							}
							else if (num32 < 0.0)
							{
								num32 = 0.0;
							}
							if (num32 > 0.0)
							{
								VectorLF3 v3 = Maths.QInvRotateLF(dysonSphereLayer.currentRotation, vectorLF7);
								VectorLF3 lhs2 = Maths.QRotateLF(dysonSphereLayer.nextRotation, v3) + __instance.starData.uPosition;
								Quaternion rhs4 = Quaternion.Inverse(dysonSphereLayer.currentRotation) * dysonRocket.uRot;
								Quaternion b3 = dysonSphereLayer.nextRotation * rhs4;
								num32 = (3.0 - num32 - num32) * num32 * num32;
								vectorLF6 = (lhs2 - dysonRocket.uPos) * num32;
								dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b3, (float)num32);
							}
						}
						double num33 = (double)dysonRocket.uSpeed * num;
						dysonRocket.uPos.x = dysonRocket.uPos.x + (double)dysonRocket.uVel.x * num33 + vectorLF6.x;
						dysonRocket.uPos.y = dysonRocket.uPos.y + (double)dysonRocket.uVel.y * num33 + vectorLF6.y;
						dysonRocket.uPos.z = dysonRocket.uPos.z + (double)dysonRocket.uVel.z * num33 + vectorLF6.z;
						vectorLF = astroPose.uPos - dysonRocket.uPos;
						num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
						if (num8 < 180.0)
						{
							dysonRocket.uPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, (VectorLF3)dysonRocket.launch * ((double)astroPose.uRadius + num8));
							dysonRocket.uRot = astroPose.uRot * Quaternion.LookRotation(dysonRocket.launch);
						}
						__instance.rocketPool[i] = dysonRocket;

					}
				}
			IL_BDF:;
			}

			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(DysonSphere), "RocketGameTick", new Type[] { typeof(int), typeof(int), typeof(int) })]
		public static bool RocketGameTickThreadPatch(ref DysonSphere __instance, int _usedThreadCnt, int _curThreadIdx, int _minimumMissionCnt)
        {
			AstroPose[] astroPoses = __instance.starData.galaxy.astroPoses;
			double num = 0.016666666666666666;
			float num2 = Mathf.Max(1f, (float)Math.Pow((double)__instance.defOrbitRadius / 40000.0 * 4.0, 0.4));
			float num3 = 7.5f;
			float num4 = 18f * num2;
			float num5 = 2800f * num2;
			int num6;
			int num7;
			if (!WorkerThreadExecutor.CalculateMissionIndex(1, __instance.rocketCursor - 1, _usedThreadCnt, _curThreadIdx, _minimumMissionCnt, out num6, out num7))
			{
				return false;
			}
			for (int i = num6; i < num7; i++)
			{
				if (__instance.rocketPool[i].id == i)
				{
					DysonRocket dysonRocket = __instance.rocketPool[i];

					bool isMissile = dysonRocket.node == null;//只有null是导弹，其他的是正常的戴森火箭
					int starIndex = __instance.starData.index;

					if (isMissile)
					{
						try
						{
							//DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[dysonRocket.node.layerId];
							AstroPose astroPose = astroPoses[dysonRocket.planetId];
							VectorLF3 vectorLF = astroPose.uPos - dysonRocket.uPos;
							double num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
							if (dysonRocket.t <= 0f) //如果离地面很近，是个加速从地面直线向上冲的过程
							{
								if (num8 < 200.0) //如果离地面很近，是个加速从地面直线向上冲的过程
								{
									float num9 = (float)num8 / 200f;
									if (num9 < 0f)
									{
										num9 = 0f;
									}
									float num10 = num9 * num9 * 600f + 15f;
									dysonRocket.uSpeed = dysonRocket.uSpeed * 0.9f + num10 * 0.1f;
									dysonRocket.t = (num9 - 1f) * 1.2f;
									if (dysonRocket.t < -1f)
									{
										dysonRocket.t = -1f;
									}
								}
								else //离地面超过或等于200
								{

									VectorLF3 vectorLF2 = dysonRocket.uPos;
									//根据是导弹还是火箭确定
									if (EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))//如果以前的目标敌舰还存在
									{
										vectorLF2 = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
									}
									else if (EnemyShips.minTargetDisSortedShips[starIndex].Count > 0) //否则，火箭继续寻敌
									{
										int newTargetId = EnemyShips.minTargetDisSortedShips[starIndex][DspBattlePlugin.randSeed.Next(0, EnemyShips.minTargetDisSortedShips[starIndex].Count)].shipIndex;
										MissileTargets[starIndex][i] = newTargetId;
										vectorLF2 = EnemyShips.ships[newTargetId].uPos - dysonRocket.uPos;
										dysonRocket.t = 0; //让其回到第一阶段，允许避障
									}
									else //如果一个敌人都没有，火箭就地自毁
									{
										__instance.RemoveDysonRocket(i);
										goto IL_BDF;
									}

									//根据距离地表的距离设置速度
									double num11 = Math.Sqrt(vectorLF2.x * vectorLF2.x + vectorLF2.y * vectorLF2.y + vectorLF2.z * vectorLF2.z);
									if (num11 < 2000.0) //如果与目标足够近，进入下一阶段
									{
										dysonRocket.t = 0.0001f;
									}
									else
									{
										dysonRocket.t = 0f;
									}
									double num12 = num11 / ((double)dysonRocket.uSpeed + 0.1) * 0.382;
									double num13 = num11 / (double)num5;
									float num14 = (float)((double)dysonRocket.uSpeed * num12) + 150f;
									if (num14 > num5)
									{
										num14 = num5;
									}
									if (dysonRocket.uSpeed < missileMaxSpeed)
									{
										dysonRocket.uSpeed += missileSpeedUp;
									}
									//else if (dysonRocket.uSpeed > num14 + num4)
									//{
									//	dysonRocket.uSpeed -= num4;
									//}
									else
									{
										dysonRocket.uSpeed = missileMaxSpeed;
									}

									//下面难道是躲避巨星？
									int num15 = -1;
									double rhs = 0.0;
									double num16 = 1E+40;
									int num17 = dysonRocket.planetId / 100 * 100;
									for (int j = num17; j < num17 + 10; j++)
									{
										float uRadius = astroPoses[j].uRadius;
										if (uRadius >= 1f)
										{
											VectorLF3 vectorLF3 = dysonRocket.uPos - astroPoses[j].uPos;
											double num18 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
											double num19 = -((double)dysonRocket.uVel.x * vectorLF3.x + (double)dysonRocket.uVel.y * vectorLF3.y + (double)dysonRocket.uVel.z * vectorLF3.z);
											if ((num19 > 0.0 || num18 < (double)(uRadius * uRadius * 7f)) && num18 < num16)
											{
												rhs = ((num19 < 0.0) ? 0.0 : num19);
												num15 = j;
												num16 = num18;
											}
										}
									}
									VectorLF3 rhs2 = VectorLF3.zero;
									float num20 = 0f;
									if (num15 > 0)
									{
										float num21 = astroPoses[num15].uRadius;
										bool flag = num15 % 100 == 0;
										if (flag)
										{
											num21 = 1000 - 400f; //dysonSphereLayer.orbitRadius - 400f
										}
										double num22 = 1.25;
										VectorLF3 vectorLF4 = dysonRocket.uPos + (VectorLF3)dysonRocket.uVel * rhs - astroPoses[num15].uPos;
										double num23 = vectorLF4.magnitude / (double)num21;
										if (num23 < num22)
										{
											double num24 = Math.Sqrt(num16) - (double)num21 * 0.82;
											if (num24 < 1.0)
											{
												num24 = 1.0;
											}
											double num25 = (num23 - 1.0) / (num22 - 1.0);
											if (num25 < 0.0)
											{
												num25 = 0.0;
											}
											num25 = 1.0 - num25 * num25;
											double num26 = (double)(dysonRocket.uSpeed - 6f) / num24 * 2.5 - 0.01;
											if (num26 > 1.5)
											{
												num26 = 1.5;
											}
											else if (num26 < 0.0)
											{
												num26 = 0.0;
											}
											num26 = num26 * num26 * num25;
											num20 = (float)(flag ? 0.0 : (num26 * 0.5));
											rhs2 = vectorLF4.normalized * num26 * 2.0;
										}
									}
									float num27 = 1f / (float)num13 - 0.05f;
									num27 += num20;
									float t = Mathf.Lerp(0.005f, 0.08f, num27);
									dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF2.normalized + rhs2, t).normalized;
									Quaternion b;
									if (num11 < 350.0)
									{
										float t2 = ((float)num11 - 50f) / 300f;
										b = Quaternion.Slerp(Quaternion.LookRotation(vectorLF2), Quaternion.LookRotation(dysonRocket.uVel), t2);//b = Quaternion.Slerp(dysonSphereLayer.NodeURot(dysonRocket.node), Quaternion.LookRotation(dysonRocket.uVel), t2);
									}
									else
									{
										b = Quaternion.LookRotation(dysonRocket.uVel);
									}
									if (vectorLF2.magnitude < 1000) //如果离得很近，则增大转弯速度
									{
										dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.4f);
									}
									else if(vectorLF2.magnitude < 3000)
									{
										dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.3f);
									}
									else
                                    {
										dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.2f);
									}

								}
							}
							else //远距离
							{

								VectorLF3 vectorLF5 = dysonRocket.uPos;
								//之前的目标是否还存活
								if (EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))//如果以前的目标敌舰还存在
								{
									vectorLF5 = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
								}
								else if (EnemyShips.minTargetDisSortedShips[starIndex].Count > 0) //否则，火箭继续寻敌
								{
									int newTargetId = EnemyShips.minTargetDisSortedShips[starIndex][DspBattlePlugin.randSeed.Next(0, EnemyShips.minTargetDisSortedShips[starIndex].Count)].shipIndex;
									MissileTargets[starIndex][i] = newTargetId;
									vectorLF5 = EnemyShips.ships[newTargetId].uPos - dysonRocket.uPos;
									dysonRocket.t = 0; //让其回到第一阶段，允许避障
								}
								else //如果一个敌人都没有，火箭就地自毁
								{
									__instance.RemoveDysonRocket(i);
									goto IL_BDF;
								}



								double num28 = Math.Sqrt(vectorLF5.x * vectorLF5.x + vectorLF5.y * vectorLF5.y + vectorLF5.z * vectorLF5.z);
								if (num28 < 100.0)
								{
									//借助太阳帆弹射的效果触发爆炸动画
									int bulletIndex = __instance.swarm.AddBullet(new SailBullet
									{
										maxt = 0.000f,
										lBegin = dysonRocket.uPos,
										uEndVel = dysonRocket.uPos,
										uBegin = dysonRocket.uPos,
										uEnd = vectorLF5 + dysonRocket.uPos
									}, 1);

									__instance.swarm.bulletPool[bulletIndex].state = 0;

									//范围伤害
									var shipsHit = EnemyShips.FindShipsInRange(dysonRocket.uPos, 500);
                                    foreach (var item in shipsHit)
                                    {
										if(EnemyShips.ships.ContainsKey(item))
											EnemyShips.ships[item].BeAttacked(canDoDamage[starIndex][i]);
									}
									canDoDamage[starIndex][i] = 0;
									__instance.RemoveDysonRocket(i);
									goto IL_BDF;
								}
								float num29 = (float)(num28 * 0.75 + 15.0);
								if (num29 > num5)
								{
									num29 = num5;
								}
								if (dysonRocket.uSpeed < missileMaxSpeed)
								{
									dysonRocket.uSpeed += missileSpeedUp;
								}
								//else if (dysonRocket.uSpeed > num29 + num4)
								//{
								//	dysonRocket.uSpeed -= num4;
								//}
								else
								{
									dysonRocket.uSpeed = missileMaxSpeed;
								}
								dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF5.normalized, 0.1f);
								//dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.2f);
								if (vectorLF5.magnitude < 1000) //如果离得很近，则增大转弯速度
								{
									dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.4f);
								}
								else if (vectorLF5.magnitude < 3000)
								{
									dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.3f);
								}
								else
								{
									dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, Quaternion.LookRotation(vectorLF5), 0.2f);
								}
								dysonRocket.t = (350f - (float)num28) / 330f;
								if (dysonRocket.t > 1f)
								{
									dysonRocket.t = 1f;
								}
								else if (dysonRocket.t < 0.0001f)
								{
									dysonRocket.t = 0.0001f;
								}
							}


							VectorLF3 vectorLF6 = Vector3.zero;
							bool flag2 = false;
							double num30 = (double)(2f - (float)num8 / 200f);
							if (num30 > 1.0)
							{
								num30 = 1.0;
							}
							else if (num30 < 0.0)
							{
								num30 = 0.0;
							}
							if (num30 > 0.0)
							{
								VectorLF3 v = dysonRocket.uPos - astroPose.uPos;
								VectorLF3 v2 = Maths.QInvRotateLF(astroPose.uRot, v);
								VectorLF3 lhs = Maths.QRotateLF(astroPose.uRotNext, v2) + astroPose.uPosNext;
								Quaternion rhs3 = Quaternion.Inverse(astroPose.uRot) * dysonRocket.uRot;
								Quaternion b2 = astroPose.uRotNext * rhs3;
								num30 = (3.0 - num30 - num30) * num30 * num30;
								vectorLF6 = (lhs - dysonRocket.uPos) * num30;
								dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b2, (float)num30);
								flag2 = true;
							}
							double num33 = (double)dysonRocket.uSpeed * num; //这里是乘了1/60
							VectorLF3 distanceCheck = new VectorLF3(999, 999, 999);
							//下面很关键，如果导弹离目标过于近，则无视旋转直接直线飞过去
							if(MissileTargets[starIndex].ContainsKey(i) && EnemyShips.ships.ContainsKey(MissileTargets[starIndex][i]))
                            {
								VectorLF3 toTarget = EnemyShips.ships[MissileTargets[starIndex][i]].uPos - dysonRocket.uPos;
								double distance = toTarget.magnitude;
								if(distance < num33) //距离小于一帧的量
                                {
									dysonRocket.uPos.x += toTarget.x;
									dysonRocket.uPos.y += toTarget.y;
									dysonRocket.uPos.z += toTarget.z;
								}
								else if(distance < 1000) //直线过去
                                {
									dysonRocket.uPos = dysonRocket.uPos + toTarget.normalized * num33;
                                }
								else //这是原始规则
                                {
									dysonRocket.uPos.x = dysonRocket.uPos.x + (double)dysonRocket.uVel.x * num33 + vectorLF6.x;
									dysonRocket.uPos.y = dysonRocket.uPos.y + (double)dysonRocket.uVel.y * num33 + vectorLF6.y;
									dysonRocket.uPos.z = dysonRocket.uPos.z + (double)dysonRocket.uVel.z * num33 + vectorLF6.z;
								}

							}
							vectorLF = astroPose.uPos - dysonRocket.uPos;
							num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
							if (num8 < 180.0)
							{
								dysonRocket.uPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, (VectorLF3)dysonRocket.launch * ((double)astroPose.uRadius + num8));
								dysonRocket.uRot = astroPose.uRot * Quaternion.LookRotation(dysonRocket.launch);
							}
							__instance.rocketPool[i] = dysonRocket;

						}
						catch (Exception)
						{
							//Main.logger.LogInfo("Missile track error, might caused by multi-thread.");
							__instance.RemoveDysonRocket(i);
						}

					}
					else //普通火箭不要管！！！！！！！！！！！！！！！！！！！！！！！！！！！
					{

						DysonSphereLayer dysonSphereLayer = __instance.layersIdBased[dysonRocket.node.layerId];
						AstroPose astroPose = astroPoses[dysonRocket.planetId];
						VectorLF3 vectorLF = astroPose.uPos - dysonRocket.uPos;
						double num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
						if (dysonRocket.t <= 0f) //如果离地面很近，是个加速从地面直线向上冲的过程
						{
							if (num8 < 200.0) //如果离地面很近，是个加速从地面直线向上冲的过程
							{
								float num9 = (float)num8 / 200f;
								if (num9 < 0f)
								{
									num9 = 0f;
								}
								float num10 = num9 * num9 * 600f + 15f;
								dysonRocket.uSpeed = dysonRocket.uSpeed * 0.9f + num10 * 0.1f;
								dysonRocket.t = (num9 - 1f) * 1.2f;
								if (dysonRocket.t < -1f)
								{
									dysonRocket.t = -1f;
								}
							}
							else //离地面超过200
							{
								VectorLF3 vectorLF2 = dysonSphereLayer.NodeEnterUPos(dysonRocket.node) - dysonRocket.uPos;

								double num11 = Math.Sqrt(vectorLF2.x * vectorLF2.x + vectorLF2.y * vectorLF2.y + vectorLF2.z * vectorLF2.z);
								if (num11 < 50.0)
								{
									dysonRocket.t = 0.0001f;
								}
								else
								{
									dysonRocket.t = 0f;
								}
								double num12 = num11 / ((double)dysonRocket.uSpeed + 0.1) * 0.382;
								double num13 = num11 / (double)num5;
								float num14 = (float)((double)dysonRocket.uSpeed * num12) + 150f;
								if (num14 > num5)
								{
									num14 = num5;
								}
								if (dysonRocket.uSpeed < num14 - num3)
								{
									dysonRocket.uSpeed += num3;
								}
								else if (dysonRocket.uSpeed > num14 + num4)
								{
									dysonRocket.uSpeed -= num4;
								}
								else
								{
									dysonRocket.uSpeed = num14;
								}
								int num15 = -1;
								double rhs = 0.0;
								double num16 = 1E+40;
								int num17 = dysonRocket.planetId / 100 * 100;
								for (int j = num17; j < num17 + 10; j++)
								{
									float uRadius = astroPoses[j].uRadius;
									if (uRadius >= 1f)
									{
										VectorLF3 vectorLF3 = dysonRocket.uPos - astroPoses[j].uPos;
										double num18 = vectorLF3.x * vectorLF3.x + vectorLF3.y * vectorLF3.y + vectorLF3.z * vectorLF3.z;
										double num19 = -((double)dysonRocket.uVel.x * vectorLF3.x + (double)dysonRocket.uVel.y * vectorLF3.y + (double)dysonRocket.uVel.z * vectorLF3.z);
										if ((num19 > 0.0 || num18 < (double)(uRadius * uRadius * 7f)) && num18 < num16)
										{
											rhs = ((num19 < 0.0) ? 0.0 : num19);
											num15 = j;
											num16 = num18;
										}
									}
								}
								VectorLF3 rhs2 = VectorLF3.zero;
								float num20 = 0f;
								if (num15 > 0)
								{
									float num21 = astroPoses[num15].uRadius;
									bool flag = num15 % 100 == 0;
									if (flag)
									{
										num21 = dysonSphereLayer.orbitRadius - 400f;
									}
									double num22 = 1.25;
									VectorLF3 vectorLF4 = dysonRocket.uPos + (VectorLF3)dysonRocket.uVel * rhs - astroPoses[num15].uPos;
									double num23 = vectorLF4.magnitude / (double)num21;
									if (num23 < num22)
									{
										double num24 = Math.Sqrt(num16) - (double)num21 * 0.82;
										if (num24 < 1.0)
										{
											num24 = 1.0;
										}
										double num25 = (num23 - 1.0) / (num22 - 1.0);
										if (num25 < 0.0)
										{
											num25 = 0.0;
										}
										num25 = 1.0 - num25 * num25;
										double num26 = (double)(dysonRocket.uSpeed - 6f) / num24 * 2.5 - 0.01;
										if (num26 > 1.5)
										{
											num26 = 1.5;
										}
										else if (num26 < 0.0)
										{
											num26 = 0.0;
										}
										num26 = num26 * num26 * num25;
										num20 = (float)(flag ? 0.0 : (num26 * 0.5));
										rhs2 = vectorLF4.normalized * num26 * 2.0;
									}
								}
								float num27 = 1f / (float)num13 - 0.05f;
								num27 += num20;
								float t = Mathf.Lerp(0.005f, 0.08f, num27);
								dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF2.normalized + rhs2, t).normalized;
								Quaternion b;
								if (num11 < 350.0)
								{
									float t2 = ((float)num11 - 50f) / 300f;
									b = Quaternion.Slerp(dysonSphereLayer.NodeURot(dysonRocket.node), Quaternion.LookRotation(dysonRocket.uVel), t2);
								}
								else
								{
									b = Quaternion.LookRotation(dysonRocket.uVel);
								}
								dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b, 0.2f);
							}
						}
						else
						{
							VectorLF3 vectorLF5 = dysonSphereLayer.NodeSlotUPos(dysonRocket.node) - dysonRocket.uPos;
							double num28 = Math.Sqrt(vectorLF5.x * vectorLF5.x + vectorLF5.y * vectorLF5.y + vectorLF5.z * vectorLF5.z);
							if (num28 < 2.0)
							{
								__instance.ConstructSp(dysonRocket.node);
								__instance.RemoveDysonRocket(i);
								goto IL_BDF;
							}
							float num29 = (float)(num28 * 0.75 + 15.0);
							if (num29 > num5)
							{
								num29 = num5;
							}
							if (dysonRocket.uSpeed < num29 - num3)
							{
								dysonRocket.uSpeed += num3;
							}
							else if (dysonRocket.uSpeed > num29 + num4)
							{
								dysonRocket.uSpeed -= num4;
							}
							else
							{
								dysonRocket.uSpeed = num29;
							}
							dysonRocket.uVel = Vector3.Slerp(dysonRocket.uVel, vectorLF5.normalized, 0.1f);
							dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, dysonSphereLayer.NodeURot(dysonRocket.node), 0.2f);
							dysonRocket.t = (350f - (float)num28) / 330f;
							if (dysonRocket.t > 1f)
							{
								dysonRocket.t = 1f;
							}
							else if (dysonRocket.t < 0.0001f)
							{
								dysonRocket.t = 0.0001f;
							}
						}
						VectorLF3 vectorLF6 = Vector3.zero;
						bool flag2 = false;
						double num30 = (double)(2f - (float)num8 / 200f);
						if (num30 > 1.0)
						{
							num30 = 1.0;
						}
						else if (num30 < 0.0)
						{
							num30 = 0.0;
						}
						if (num30 > 0.0)
						{
							VectorLF3 v = dysonRocket.uPos - astroPose.uPos;
							VectorLF3 v2 = Maths.QInvRotateLF(astroPose.uRot, v);
							VectorLF3 lhs = Maths.QRotateLF(astroPose.uRotNext, v2) + astroPose.uPosNext;
							Quaternion rhs3 = Quaternion.Inverse(astroPose.uRot) * dysonRocket.uRot;
							Quaternion b2 = astroPose.uRotNext * rhs3;
							num30 = (3.0 - num30 - num30) * num30 * num30;
							vectorLF6 = (lhs - dysonRocket.uPos) * num30;
							dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b2, (float)num30);
							flag2 = true;
						}
						if (!flag2)
						{
							VectorLF3 vectorLF7 = dysonRocket.uPos - __instance.starData.uPosition;
							double num31 = Math.Abs(Math.Sqrt(vectorLF7.x * vectorLF7.x + vectorLF7.y * vectorLF7.y + vectorLF7.z * vectorLF7.z) - (double)dysonSphereLayer.orbitRadius);
							double num32 = 1.5 - (double)((float)num31 / 1800f);
							if (num32 > 1.0)
							{
								num32 = 1.0;
							}
							else if (num32 < 0.0)
							{
								num32 = 0.0;
							}
							if (num32 > 0.0)
							{
								VectorLF3 v3 = Maths.QInvRotateLF(dysonSphereLayer.currentRotation, vectorLF7);
								VectorLF3 lhs2 = Maths.QRotateLF(dysonSphereLayer.nextRotation, v3) + __instance.starData.uPosition;
								Quaternion rhs4 = Quaternion.Inverse(dysonSphereLayer.currentRotation) * dysonRocket.uRot;
								Quaternion b3 = dysonSphereLayer.nextRotation * rhs4;
								num32 = (3.0 - num32 - num32) * num32 * num32;
								vectorLF6 = (lhs2 - dysonRocket.uPos) * num32;
								dysonRocket.uRot = Quaternion.Slerp(dysonRocket.uRot, b3, (float)num32);
							}
						}
						double num33 = (double)dysonRocket.uSpeed * num;
						dysonRocket.uPos.x = dysonRocket.uPos.x + (double)dysonRocket.uVel.x * num33 + vectorLF6.x;
						dysonRocket.uPos.y = dysonRocket.uPos.y + (double)dysonRocket.uVel.y * num33 + vectorLF6.y;
						dysonRocket.uPos.z = dysonRocket.uPos.z + (double)dysonRocket.uVel.z * num33 + vectorLF6.z;
						vectorLF = astroPose.uPos - dysonRocket.uPos;
						num8 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z) - (double)astroPose.uRadius;
						if (num8 < 180.0)
						{
							dysonRocket.uPos = astroPose.uPos + Maths.QRotateLF(astroPose.uRot, (VectorLF3)dysonRocket.launch * ((double)astroPose.uRadius + num8));
							dysonRocket.uRot = astroPose.uRot * Quaternion.LookRotation(dysonRocket.launch);
						}
						__instance.rocketPool[i] = dysonRocket;

					}
				}
			IL_BDF:;
			}

			return false;
        }


		public static int AddDysonRockedGniMaerd(ref DysonSphere _this, ref DysonRocket rocket, DysonNode node = null)
        {
			DysonRocket[] obj = _this.rocketPool;
			int num;
			lock (obj)
			{
				//rocket.GiveRefNode(_this, node);
				rocket.nodeLayerId = 0;
				rocket.nodeId = 0;
				rocket.node = null;
				if (rocket.node == null)
				{
					int num2;
					if (_this.rocketRecycleCursor > 0)
					{
						int[] array = _this.rocketRecycle;
						num = _this.rocketRecycleCursor - 1;
						_this.rocketRecycleCursor = num;
						num2 = array[num];
					}
					else
					{
						num = _this.rocketCursor;
						_this.rocketCursor = num + 1;
						num2 = num;
						if (num2 == _this.rocketCapacity)
						{
							// Traverse.Create(_this).Method("SetRocketCapacity").GetValue(_this.rocketCapacity * 2);
							AccessTools.Method(typeof(DysonSphere), "SetRocketCapacity").Invoke(_this, new object[] { _this.rocketCapacity * 2 });
							// _this.SetRocketCapacity(_this.rocketCapacity * 2);
						}
					}
					_this.rocketPool[num2] = rocket;
					_this.rocketPool[num2].id = num2;
					_this.rocketPool[num2].t = -1f;
					//_this.OrderConstructSp(node);必须删除！！！
					num = num2;
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}


		public static void Export(BinaryWriter w)
		{
			w.Write(MissileTargets.Count);
            for (int i1 = 0; i1 < MissileTargets.Count; i1++)
            {
				w.Write(MissileTargets[i1].Count);
                foreach (var item in MissileTargets[i1])
                {
					w.Write(item.Key);
					w.Write(item.Value);
                }
            }
			w.Write(canDoDamage.Count);
            for (int i2 = 0; i2 < canDoDamage.Count; i2++)
            {
				w.Write(canDoDamage[i2].Count);
                foreach (var item in canDoDamage[i2])
                {
					w.Write(item.Key);
					w.Write(item.Value);
                }
            }
		}

		public static void Import(BinaryReader r)
		{
			ReInitAll();
			int total1 = r.ReadInt32();
            for (int c1 = 0; c1 < total1 - MissileTargets.Count; c1++)
            {
				MissileTargets.Add(new ConcurrentDictionary<int, int>());
            }
            for (int i1 = 0; i1 < total1; i1++)
            {
				int num1 = r.ReadInt32();
                for (int j1 = 0; j1 < num1; j1++)
                {
					MissileTargets[i1].TryAdd(r.ReadInt32(),r.ReadInt32());
                }
            }

			int total2 = r.ReadInt32();
            for (int c2 = 0; c2 < total2 - canDoDamage.Count; c2++)
            {
				canDoDamage.Add(new ConcurrentDictionary<int, int>());
            }
			for (int i2 = 0; i2 < total2; i2++)
			{
				int num2 = r.ReadInt32();
				for (int j2 = 0; j2 < num2; j2++)
				{
					canDoDamage[i2].TryAdd(r.ReadInt32(), r.ReadInt32());
				}
			}
		}

		public static void IntoOtherSave()
		{
			ReInitAll();
		}
	}
}
