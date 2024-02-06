using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    public class StarFortressSilo
    {
		//// 以下需要存档
		//public static List<ConcurrentDictionary<int, int>> starFortressRocketProtoIds; //记录用于构建恒星要塞的火箭的Id，starFortressRocketIds[starIndex][rocketId] = protoId，其中rocketId也是其pool中的index

		//public static void InitAll()
		//{
		//	starFortressRocketProtoIds = new List<ConcurrentDictionary<int, int>>();
		//	for (int i = 0; i < GameMain.galaxy.starCount; i++)
		//	{
		//		starFortressRocketProtoIds.Add(new ConcurrentDictionary<int, int>());
		//	}
		//}

		//public static bool SiloSubPatch(ref SiloComponent __instance, float power, DysonSphere sphere, AnimData[] animPool, int[] consumeRegister, ref uint __result)
  //      {
		//	if (__instance.needs == null)
		//	{
		//		__instance.needs = new int[6];
		//	}
		//	__instance.needs[0] = ((__instance.bulletCount >= 20) ? 0 : __instance.bulletId);
		//	if (__instance.fired && __instance.direction != -1)
		//	{
		//		__instance.fired = false;
		//	}
		//	if (__instance.direction == 1)
		//	{
		//		animPool[__instance.entityId].time = (float)__instance.time / (float)__instance.chargeSpend;
		//	}
		//	else if (__instance.direction == -1)
		//	{
		//		animPool[__instance.entityId].time = -(float)__instance.time / (float)__instance.coldSpend;
		//	}
		//	animPool[__instance.entityId].power = power;
		//	float num = (float)Cargo.accTableMilli[__instance.incLevel];
		//	int num2 = (int)(power * 10000f * (1f + num) + 0.1f);
		//	if (__instance.boost)
		//	{
		//		num2 *= 10;
		//	}
		//	Mutex dysonSphere_mx = sphere.dysonSphere_mx;
		//	lock (dysonSphere_mx)
		//	{
		//		__instance.hasNode = StarFortress.NeedRocket(sphere, __instance.bulletId);
		//		if (!__instance.hasNode)
		//		{
		//			__instance.autoIndex = 0;
		//			if (__instance.direction == 1)
		//			{
		//				__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
		//				__instance.direction = -1;
		//			}
		//			if (__instance.direction == -1)
		//			{
		//				__instance.time -= num2;
		//				if (__instance.time <= 0)
		//				{
		//					__instance.time = 0;
		//					__instance.direction = 0;
		//				}
		//			}
		//			if (power >= 0.1f)
		//			{
		//				__result = 1u;
		//			}
		//			else
		//			{
		//				__result = 0u;
		//			}
		//		}
		//		else if (power < 0.1f)
		//		{
		//			if (__instance.direction == 1)
		//			{
		//				__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
		//				__instance.direction = -1;
		//			}
		//			__result = 0u;
		//		}
		//		else
		//		{
		//			uint num3 = 0u;
		//			bool flag2;
		//			num3 = ((flag2 = (__instance.bulletCount > 0)) ? 3u : 2u);
		//			if (__instance.direction == 1)
		//			{
		//				if (!flag2)
		//				{
		//					__instance.time = (int)((long)__instance.time * (long)__instance.coldSpend / (long)__instance.chargeSpend);
		//					__instance.direction = -1;
		//				}
		//			}
		//			else if (__instance.direction == 0 && flag2)
		//			{
		//				__instance.direction = 1;
		//			}
		//			if (__instance.direction == 1)
		//			{
		//				__instance.time += num2;
		//				if (__instance.time >= __instance.chargeSpend)
		//				{
		//					AstroData[] astrosData = sphere.starData.galaxy.astrosData;
		//					__instance.fired = true;
		//					DysonNode autoDysonNode = FindRandomNode(sphere, __instance.autoIndex);
		//					DysonRocket dysonRocket = default(DysonRocket);
		//					dysonRocket.planetId = __instance.planetId;
		//					dysonRocket.uPos = astrosData[__instance.planetId].uPos + Maths.QRotateLF(astrosData[__instance.planetId].uRot, __instance.localPos + __instance.localPos.normalized * 6.1f);
		//					dysonRocket.uRot = astrosData[__instance.planetId].uRot * __instance.localRot * Quaternion.Euler(-90f, 0f, 0f);
		//					dysonRocket.uVel = dysonRocket.uRot * Vector3.forward;
		//					dysonRocket.uSpeed = 0f;
		//					dysonRocket.launch = __instance.localPos.normalized;
		//					AddStarFortressRocketGniMaerd(ref sphere, ref dysonRocket, ref autoDysonNode, __instance.bulletId);
		//					__instance.autoIndex++;
		//					__instance.bulletInc -= __instance.bulletInc / __instance.bulletCount;
		//					__instance.bulletCount--;
		//					if (__instance.bulletCount == 0)
		//					{
		//						__instance.bulletInc = 0;
		//					}
		//					lock (consumeRegister)
		//					{
		//						consumeRegister[__instance.bulletId]++;
		//					}
		//					__instance.time = __instance.coldSpend;
		//					__instance.direction = -1;
		//				}
		//			}
		//			else if (__instance.direction == -1)
		//			{
		//				__instance.time -= num2;
		//				if (__instance.time <= 0)
		//				{
		//					__instance.time = 0;
		//					__instance.direction = (flag2 ? 1 : 0);
		//				}
		//			}
		//			else
		//			{
		//				__instance.time = 0;
		//			}
		//			__result = num3;
		//		}
		//	}
		//	return false;
		//}

		//public static int nextBulletId(int starIndex, int id)
		//{
		//	if (starIndex >= MoreMegaStructure.MoreMegaStructure.StarMegaStructureType.Length) return 1503;
		//	int bulletIdExpected = 1503;
		//	int megaType = MoreMegaStructure.MoreMegaStructure.StarMegaStructureType[starIndex];
		//	switch (megaType)
		//	{
		//		case 0:
		//			bulletIdExpected = 1503;
		//			break;
		//		case 1:
		//			bulletIdExpected = 9488;
		//			break;
		//		case 2:
		//			bulletIdExpected = 9489;
		//			break;
		//		case 3:
		//			bulletIdExpected = 9490;
		//			break;
		//		case 4:
		//			bulletIdExpected = 9491;
		//			break;
		//		case 5:
		//			bulletIdExpected = 9492;
		//			break;
		//		case 6:
		//			bulletIdExpected = 9510;
		//			break;
		//		default:
		//			break;
		//	}
		//	switch (id)
  //          {
		//		case 8037:
		//			return 8038;
		//		case 8038:
		//			return 8039;
		//		case 8039:
		//			return bulletIdExpected;
  //              default:
		//			return 8037;
  //          }
  //      }

		//private static DysonNode FindRandomNode(DysonSphere sphere, int randSeed)
		//{
		//	randSeed += Utils.RandInt(0, randSeed + 500);
  //          for (int i = 0; i < 10; i++)
  //          {
		//		if (sphere.layersSorted.Length > i && sphere.layersSorted[i] != null)
		//		{
		//			DysonSphereLayer layer = sphere.layersSorted[i];
		//			int begins = randSeed % layer.nodeCursor;
		//			if (begins >= layer.nodePool.Length) continue; // 防止越界，游戏逻辑保证了cursor不大于length，所以应该是不必的

		//			// 下面从（依据种子）随机的一个nodePool中的位置开始向后遍历找到第一个不是null的node返回
  //                  for (int j = begins; j < layer.nodeCursor; j++)
  //                  {
		//				if (layer.nodePool.Length > j && layer.nodePool[j] != null)
		//					return layer.nodePool[j];
  //                  }

  //                  for (int k = 0; k < begins; k++)
  //                  {
		//				if (layer.nodePool[k] != null)
		//					return layer.nodePool[k];
		//			}
		//		}
  //          }

		//	return null;
		//}

		//private static int AddStarFortressRocketGniMaerd(ref DysonSphere _this, ref DysonRocket rocket, ref DysonNode node, int rocketId)
		//{
		//	DysonRocket[] obj = _this.rocketPool;
		//	int num;
		//	lock (obj)
		//	{
		//		rocket.GiveRefNode(_this, node);
		//		if (rocket.node != null)
		//		{
		//			int num2;
		//			if (_this.rocketRecycleCursor > 0)
		//			{
		//				int[] array = _this.rocketRecycle;
		//				num = _this.rocketRecycleCursor - 1;
		//				_this.rocketRecycleCursor = num;
		//				num2 = array[num];
		//			}
		//			else
		//			{
		//				num = _this.rocketCursor;
		//				_this.rocketCursor = num + 1;
		//				num2 = num;
		//				if (num2 == _this.rocketCapacity)
		//				{
		//					_this.SetRocketCapacity(_this.rocketCapacity * 2);
		//				}
		//			}
		//			_this.rocketPool[num2] = rocket;
		//			_this.rocketPool[num2].id = num2;
		//			_this.rocketPool[num2].t = -1f;
		//			num = num2;
		//			int index = rocketId - 8037;
		//			index = Math.Min(Math.Max(0, index), 2);
		//			starFortressRocketProtoIds[_this.starData.index].AddOrUpdate(num, rocketId, (x, y) => rocketId); // 放进dictionary里面记录，在后面rocket更新时方便截获
		//			StarFortress.moduleComponentInProgress[_this.starData.index].AddOrUpdate(index, 1, (x, y) => y + 1); // 增加一个在途的火箭
		//		}
		//		else
		//		{
		//			num = 0;
		//		}
		//	}
		//	return num;
		//}

		//public static void Export(BinaryWriter w)
		//{
		//	w.Write(starFortressRocketProtoIds.Count);
		//	for (int i = 0; i < starFortressRocketProtoIds.Count; i++)
		//	{
		//		w.Write(starFortressRocketProtoIds[i].Count);
		//		foreach (var item in starFortressRocketProtoIds[i])
		//		{
		//			w.Write(item.Key);
		//			w.Write(item.Value);
		//		}
		//	}
		//}

		//public static void Import(BinaryReader r)
		//{
		//	if (Configs.versionWhenImporting >= 30230319)
		//	{
		//		int total = r.ReadInt32();
		//		for (int i = 0; i < total; i++)
		//		{
		//			int total_1 = r.ReadInt32();
		//			for (int j = 0; j < total_1; j++)
		//			{
		//				int key = r.ReadInt32();
		//				int value = r.ReadInt32();
		//				starFortressRocketProtoIds[i].AddOrUpdate(key, value, (x, y) => value);
		//			}
		//		}
		//	}
		//}

		//public static void IntoOtherSave()
		//{
		//}
	}
}
