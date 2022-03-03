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
        public static DysonSwarm[] swarms;
		public static List<List<int>> sailBulletsIndex; //记录应该变成太阳帆的子弹，原本是记录攻击用子弹，但是总有漏网之鱼变成太阳帆，找不到原因，所以反过来记录应该变成太阳帆的子弹，这可能导致0.1%（目测，或许远低于此）的太阳帆无法生成
		public static List<Dictionary<int,EnemyShip>> BulletTargets; //记录子弹的目标
		public static List<Dictionary<int,int>> canDoDamage; //记录子弹还能造成多少伤害
		public static int testFrameCount = 0;
        public static double BulletMaxtDivisor = 12000.0; // 原本5000.0
        public static float numMinus = 0.0f;
        public static VectorLF3 endPos = new VectorLF3(2000,2000,2000);
        public static bool doTrack = true;
        public static System.Random rand = new System.Random();

        /// <summary>
        /// 每帧调用刷新子弹终点
        /// </summary>
        public static void BulletTrack()
        {
            testFrameCount = (testFrameCount + 1) % 60;
            if (!doTrack) return;
            try
            {
                for (int i = 0; i < GameMain.data.dysonSpheres.Length; i++)
                {
					DysonSwarm swarm = GameMain.data.dysonSpheres[i].swarm;
					int starIndex = GameMain.data.dysonSpheres[i].starData.index;
					if (swarm != null)
					{
						for (int j = 1; j < swarm.bulletCursor; j++)
						{
							//if ((curSwarm.bulletPool[i].uEnd - curSwarm.bulletPool[i].uBegin).magnitude > 500)
							//{
							//	curSwarm.bulletPool[i].uBegin += (curSwarm.bulletPool[i].uEnd - curSwarm.bulletPool[i].uBegin) * curSwarm.bulletPool[i].t / (curSwarm.bulletPool[i].maxt+0.7f);
							//	curSwarm.bulletPool[i].maxt = curSwarm.bulletPool[i].maxt - curSwarm.bulletPool[i].t;
							//	curSwarm.bulletPool[i].t = 0;
							//}
							if (!sailBulletsIndex[starIndex].Contains(j)) //只有对应swarm的对应位置的bullet不是之前存下来的solarsail的Bullet的时候才改变目标终点
							{
								if (BulletTargets[starIndex].ContainsKey(j) && BulletTargets[starIndex][j] != null && BulletTargets[starIndex][j].state == EnemyShip.State.active)
								{
									swarm.bulletPool[j].uEnd = BulletTargets[starIndex][j].uPos;
								}
							}

						}
					}

				}
                
            }
            catch (Exception)
            {
            }


        }

		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameSave), "LoadCurrentGame")]
		public static void ReInitAll()
        {
            try
			{
				sailBulletsIndex = new List<List<int>>();
				BulletTargets = new List<Dictionary<int, EnemyShip>>();
				canDoDamage = new List<Dictionary<int, int>>();

				for (int i = 0; i < GameMain.galaxy.starCount; i++)
				{
					sailBulletsIndex.Add(new List<int>());
					BulletTargets.Add(new Dictionary<int, EnemyShip>());
					canDoDamage.Add(new Dictionary<int, int>());
				}

			}
            catch (Exception)
            {
				Main.logger.LogWarning("Cannon ReInit ERROR");
            }
        }

		[HarmonyPrefix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static bool EjectorPatch(ref EjectorComponent __instance, float power, DysonSwarm swarm, AstroPose[] astroPoses, AnimData[] animPool, int[] consumeRegister, ref uint __result)
        {
			curSwarm = swarm;

			int planetId = __instance.planetId;
			PlanetFactory factory = GameMain.galaxy.stars[planetId / 100 - 1].planets[planetId % 100 - 1].factory;
			int gmProtoId = factory.entityPool[__instance.entityId].protoId;

			//if (gmProtoId != 9801) return true; // 应该是true，但是现在其他配合还没写完，所以暂时阻止所有普通弹射器运行。如果不是自己自定义的炮，的建筑protoId，就返回原函数

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
			if(__instance.orbitId == 0)
            {
				__instance.orbitId = 1;

			}

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

				//不该参与循环的部分，换到循环前了

				bool flag2 = __instance.bulletCount > 0;

				//下面的参数根据是否是炮还是太阳帆的弹射器有不同的修改
				double maxtDivisor = 5000.0;
				int loopNum = 1;
				EnemyShip curTarget = null;

                if (gmProtoId == 9801)
				{

					loopNum = EnemyShips.ships.Count;
					maxtDivisor = BulletMaxtDivisor;
				}


				for (int gm = 0; gm < loopNum; gm++)
				{

					//新增的，每次循环开始必须重置
					__instance.targetState = EjectorComponent.ETargetState.OK;
					flag = true;
					flag2 = __instance.bulletCount > 0;

					if (gmProtoId == 9801)
                    {
						vectorLF2 = EnemyShips.ships[gm].uPos;
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
					if(gmProtoId == 9801 && __instance.targetState != EjectorComponent.ETargetState.Blocked && __instance.targetState != EjectorComponent.ETargetState.AngleLimit)
                    {
						curTarget = EnemyShips.ships[gm];
						break;
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

				//如果没有船/船没血了，就不打炮了
				if (curTarget == null && gmProtoId == 9801)
				{
					__result = 0U;
					return false;
				}
				else if (curTarget!= null && curTarget.hp <= 0 && gmProtoId == 9801)
				{
					__result = 0U;
					return false;
				}
                
				if (__instance.direction == 1)
				{
					__instance.time += num3;
					if (__instance.time >= __instance.chargeSpend)
					{


						__instance.fired = true;
						animPool[__instance.entityId].time = 10f;
						//下面是添加子弹
						int bulletIndex = swarm.AddBullet(new SailBullet
						{
							maxt = (float)(__instance.targetDist / maxtDivisor),
							lBegin = vector,
							uEndVel = VectorLF3.Cross(vectorLF2 - uPos, swarm.orbits[__instance.orbitId].up).normalized * Math.Sqrt((double)(swarm.dysonSphere.gravity / swarm.orbits[__instance.orbitId].radius)),
							uBegin = vectorLF,
							uEnd = vectorLF2
						}, __instance.orbitId);



						//将添加的用于攻击的子弹的index存储，便于后续更新其弹道，又能防止影响正常的太阳帆
						if (gmProtoId == 2311 && !sailBulletsIndex[swarm.starData.index].Contains(bulletIndex))
						{
							sailBulletsIndex[swarm.starData.index].Add(bulletIndex);
						}
						//如果是炮，设定子弹目标
						else if (gmProtoId == 9801)
						{
                            try
							{
								BulletTargets[swarm.starData.index][bulletIndex] = curTarget;
								canDoDamage[swarm.starData.index][bulletIndex] = 1;//后续可以根据子弹类型/炮类型设定不同数值
								swarm.bulletPool[bulletIndex].state = 0; //设置成0，该子弹将不会生成太阳帆
							}
                            catch (Exception)
                            {
								Main.logger.LogInfo("bullet info set error.");
                            }

						}

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
				else if (gmProtoId == 2311)
				{
					return true; //如果不是炮，又没发射，就返回。这是为了防止return false影响潜在的其他mod的prepatch。 有必要吗？？？？
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
        /// 这个函数用于从GameTick手中截胡攻击用的子弹，不让它创建太阳帆（而非以往的创建太阳帆后删除），这样可以保证正常的太阳帆发射行为不受干扰，但是，缺点是会提前一帧触发某些效果，但我觉得一帧无所谓。
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSwarm), "GameTick")]
        public static void EarlyCalcBulletState(ref DysonSwarm __instance)
        {
			int starIndex = __instance.starData.index;

			for (int i = 1; i < __instance.bulletCursor; i++)
            {
                if (__instance.bulletPool[i].id == i && !sailBulletsIndex[starIndex].Contains(i)) //后面的判断条件就是说只对攻击用的子弹生效，不对正常的太阳帆操作
                {
                    //SailBullet[] array = __instance.bulletPool;
                    //int num3 = i;
                    //此处原本函数有array[num3].t = array[num3].t + num;其中num是1/60，但是这是postfix，已经执行过t增加了，所以不能再加一次，否则会使子弹变成二倍速

					//为什么下面的if注释掉了呢？我发现在创建子弹时就设置成state=0貌似也没什么影响。。那既然state=0不会创建太阳帆，那就一开始设置吧，不要每帧检测了。
                    //if (__instance.bulletPool[i].t >= __instance.bulletPool[i].maxt - 0.02f) //
                    //{
                    //    __instance.bulletPool[i].state = 0; //这就阻止了后续创建太阳帆的可能，但也可能带来其他影响，但无所谓，只有最多2帧的异常帧
                        
                    //}
					if (__instance.bulletPool[i].t >= __instance.bulletPool[i].maxt && BulletTargets[starIndex].ContainsKey(i) && canDoDamage[starIndex].ContainsKey(i))
					{
						BulletTargets[starIndex][i].BeAttacked(canDoDamage[starIndex][i]); //击中造成伤害  //如果在RemoveBullet的postpatch写这个，可以不用每帧循环检测，但是伤害将在爆炸动画后结算，感觉不太合理
						canDoDamage[starIndex][i] = 0; //该子弹已造成过伤害，还能造成0伤害
					}
				}
				
            }
        }



		[HarmonyPostfix]
		[HarmonyPatch(typeof(DysonSwarm), "RemoveBullet")]
		public static void RemoveBulletThenRemoveSailMark(DysonSwarm __instance, int id)
        {
			if (sailBulletsIndex[__instance.starData.index].Contains(id))
			{
				sailBulletsIndex[__instance.starData.index].Remove(id); //删除，i不再被记为太阳帆子弹。子弹实体会在后续自动被游戏原本逻辑移除
			}

		}




		//    /// <summary>
		//    /// 创建太阳帆后立即删除。即将被弃用。
		//    /// </summary>
		//    /// <param name="__instance"></param>
		//    /// <param name="__result"></param>
		//    [HarmonyPostfix]
		//    [HarmonyPatch(typeof(DysonSwarm), "AddSolarSail")]
		//    public static void GetSailIndexWhenAdd(ref DysonSwarm __instance, int __result)
		//    {
		//        try
		//        {
		//            __instance.RemoveSolarSail(__result);
		//Ship.CurHp += 2;
		//if(Ship.CurHp <= 0)
		//            {
		//	Ship.shipData.shipIndex = 0;
		//            }
		//        }
		//        catch (Exception)
		//        {

		//        }
		//    }


		/// <summary>
		/// 每帧/特定情况下创建子弹
		/// </summary>
		/// <param name="__instance"></param>
		/// <param name="power"></param>
		/// <param name="swarm"></param>
		/// <param name="astroPoses"></param>
		/// <param name="animPool"></param>
		/// <param name="consumeRegister"></param>
		//[HarmonyPostfix]
		//[HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
		//public static void EjectorPatch(ref EjectorComponent __instance, float power, DysonSwarm swarm, AstroPose[] astroPoses, AnimData[] animPool, int[] consumeRegister)
		//{
		//    curSwarm = swarm;
		//    if (testFrameCount % 20 != 0)
		//        return;
		//    try
		//    {
		//        int num4 = __instance.planetId / 100 * 100;
		//        float num5 = __instance.localAlt + __instance.pivotY + (__instance.muzzleY - __instance.pivotY) / Mathf.Max(0.1f, Mathf.Sqrt(1f - __instance.localDir.y * __instance.localDir.y));
		//        //Vector3 vector = new Vector3(__instance.localPosN.x * num5, __instance.localPosN.y * num5, __instance.localPosN.z * num5);

		//        //VectorLF3 vectorLF = astroPoses[__instance.planetId].uPos + Maths.QRotateLF(astroPoses[__instance.planetId].uRot, vector);
		//        //VectorLF3 uPos = astroPoses[num4].uPos;
		//        ////uPos = new VectorLF3(500, 700, 900);
		//        //VectorLF3 b = uPos - vectorLF;
		//        //VectorLF3 vectorLF2 = uPos;

		//        Vector3 vector = new Vector3(__instance.localPosN.x * num5, __instance.localPosN.y * num5, __instance.localPosN.z * num5);
		//        VectorLF3 vectorLF = astroPoses[__instance.planetId].uPos + Maths.QRotateLF(astroPoses[__instance.planetId].uRot, vector);
		//        Quaternion q = astroPoses[__instance.planetId].uRot * __instance.localRot;
		//        VectorLF3 uPos = astroPoses[num4].uPos;
		//        VectorLF3 b = uPos - vectorLF;
		//        VectorLF3 vectorLF2 = uPos + VectorLF3.Cross(swarm.orbits[__instance.orbitId].up, b).normalized * (double)swarm.orbits[__instance.orbitId].radius;
		//        try
		//        {
		//            vectorLF2 = Ship.shipData.uPos;
		//        }
		//        catch (Exception)
		//        {
		//        }
		//        VectorLF3 vectorLF3 = vectorLF2 - vectorLF;
		//        __instance.targetDist = vectorLF3.magnitude;



		//        swarm.AddBullet(new SailBullet
		//        {
		//            maxt = (float)(__instance.targetDist / BulletMaxtDivisor),//子弹飞行到终点的时间，可能即使后续更改终点，时间到了也会到达（或消失？），然后创建太阳帆
		//            lBegin = vector,
		//            uEndVel = VectorLF3.Cross(vectorLF2 - uPos, swarm.orbits[__instance.orbitId].up).normalized * Math.Sqrt((double)(swarm.dysonSphere.gravity / swarm.orbits[__instance.orbitId].radius)),
		//            //uEndVel = vectorLF2,
		//            uBegin = vectorLF,
		//            uEnd = vectorLF2
		//        }, __instance.orbitId);
		//    }
		//    catch (Exception)
		//    {
		//        Main.logger.LogWarning("new bullet err");
		//    }

		//}


		/*
		[HarmonyPrefix]
		[HarmonyPatch(typeof(DysonSwarm), "GameTick")]
		public static bool GameTickPrePatch(ref DysonSwarm __instance, long time)
        {
			int propGravityId = (int)Traverse.Create(__instance).Field("propGravityId").GetValue();
			int kernelUpdatePosId = (int)Traverse.Create(__instance).Field("kernelUpdatePosId").GetValue();
			int propBufferId = (int)Traverse.Create(__instance).Field("propBufferId").GetValue();
			int propInfoBufferId = (int)Traverse.Create(__instance).Field("propInfoBufferId").GetValue();
			int propNodeBufferId = (int)Traverse.Create(__instance).Field("propNodeBufferId").GetValue();
			int kernelUpdateVelId = (int)Traverse.Create(__instance).Field("kernelUpdateVelId").GetValue();
			int propNewBufferId = (int)Traverse.Create(__instance).Field("propNewBufferId").GetValue();
			int propGameTickId = (int)Traverse.Create(__instance).Field("propGameTickId").GetValue();
			ComputeBuffer swarmBuffer = (ComputeBuffer)Traverse.Create(__instance).Field("swarmBuffer").GetValue();
			ComputeBuffer swarmInfoBuffer = (ComputeBuffer)Traverse.Create(__instance).Field("swarmInfoBuffer").GetValue();
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetFloat(propGravityId, __instance.dysonSphere.gravity);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetBuffer(kernelUpdatePosId, propBufferId, swarmBuffer);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetBuffer(kernelUpdatePosId, propNewBufferId, swarmBuffer);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetBuffer(kernelUpdatePosId, propInfoBufferId, swarmInfoBuffer);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetBuffer(kernelUpdatePosId, propNodeBufferId, __instance.dysonSphere.nrdBuffer);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetBuffer(kernelUpdateVelId, propBufferId, swarmBuffer);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetBuffer(kernelUpdateVelId, propNewBufferId, swarmBuffer);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetBuffer(kernelUpdateVelId, propInfoBufferId, swarmInfoBuffer);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetBuffer(kernelUpdateVelId, propNodeBufferId, __instance.dysonSphere.nrdBuffer);
			((ComputeShader)Traverse.Create(__instance).Field("computeShader").GetValue()).SetInt(propGameTickId, (int)((uint)(time)));
			__instance.Dispatch_UpdateVel();
			__instance.Dispatch_UpdatePos();
			while (__instance.expiryCursor != __instance.expiryEnding)
			{
				if (time < __instance.expiryOrder[__instance.expiryCursor].time)
				{
					break;
				}
				__instance.RemoveSolarSail(__instance.expiryOrder[__instance.expiryCursor].index);
				__instance.expiryOrder[__instance.expiryCursor].time = 0L;
				__instance.expiryOrder[__instance.expiryCursor].index = 0;
				__instance.expiryCursor++;
				if (__instance.expiryCursor == __instance.sailCapacity)
				{
					__instance.expiryCursor = 0;
				}
			}
			while (__instance.absorbCursor != __instance.absorbEnding && time >= __instance.absorbOrder[__instance.absorbCursor].time)
			{
				DysonNode dysonNode = __instance.dysonSphere.FindNode(__instance.absorbOrder[__instance.absorbCursor].layer, __instance.absorbOrder[__instance.absorbCursor].node);
				if (dysonNode != null && dysonNode.ConstructCp() != null)
				{
					__instance.dysonSphere.productRegister[11903]++;
				}
				__instance.RemoveSolarSail(__instance.absorbOrder[__instance.absorbCursor].index);
				__instance.absorbOrder[__instance.absorbCursor].time = 0L;
				__instance.absorbOrder[__instance.absorbCursor].index = 0;
				__instance.absorbOrder[__instance.absorbCursor].layer = 0;
				__instance.absorbOrder[__instance.absorbCursor].node = 0;
				__instance.absorbCursor++;
				if (__instance.absorbCursor == __instance.sailCapacity)
				{
					__instance.absorbCursor = 0;
				}
			}
			PerformanceMonitor.BeginSample(ECpuWorkEntry.DysonBullet);
			float num = 0.016666668f;
			int num2 = (int)(GameMain.history.solarSailLife * 60f + 0.1f);
			VectorLF3 relativePos = __instance.gameData.relativePos;
			Quaternion relativeRot = __instance.gameData.relativeRot;
			for (int i = 1; i < __instance.bulletCursor; i++)
			{
				if (__instance.bulletPool[i].id == i)
				{
					SailBullet[] array = __instance.bulletPool;
					int num3 = i;
					array[num3].t = array[num3].t + num;
					if (__instance.bulletPool[i].t >= __instance.bulletPool[i].maxt)
					{
						if (__instance.bulletPool[i].state > 0)
						{
							if (__instance.bulletPool[i].state < __instance.orbitCursor && __instance.orbits[__instance.bulletPool[i].state].id == __instance.bulletPool[i].state && !sailBulletsIndex[__instance.starData.index].Contains(i))
							{
								DysonSail ss = default(DysonSail);
								VectorLF3 vectorLF = __instance.bulletPool[i].uEnd - __instance.starData.uPosition;
								ss.px = (float)vectorLF.x;
								ss.py = (float)vectorLF.y;
								ss.pz = (float)vectorLF.z;
								vectorLF = __instance.bulletPool[i].uEndVel;
								vectorLF += RandomTable.SphericNormal(ref __instance.randSeed, 0.5);
								ss.vx = (float)vectorLF.x;
								ss.vy = (float)vectorLF.y;
								ss.vz = (float)vectorLF.z;
								ss.gs = 1f;
								__instance.AddSolarSail(ss, __instance.bulletPool[i].state, time + (long)num2);
							}
						}
						else if (__instance.bulletPool[i].t > __instance.bulletPool[i].maxt + 0.7f)
						{
							__instance.RemoveBullet(i);
							sailBulletsIndex[__instance.starData.index].Remove(i); //删除，i不再被记为攻击用子弹。子弹实体会在后续自动被游戏原本逻辑移除
						}
						__instance.bulletPool[i].state = 0;
					}
					if (DysonSphere.renderPlace == ERenderPlace.Universe)
					{
						__instance.bulletPool[i].rBegin = Maths.QInvRotateLF(relativeRot, __instance.bulletPool[i].uBegin - relativePos);
						__instance.bulletPool[i].rEnd = Maths.QInvRotateLF(relativeRot, __instance.bulletPool[i].uEnd - relativePos);
					}
					else if (DysonSphere.renderPlace == ERenderPlace.Starmap)
					{
						__instance.bulletPool[i].rBegin = (__instance.bulletPool[i].uBegin - UIStarmap.viewTargetStatic) * 0.00025;
						__instance.bulletPool[i].rEnd = (__instance.bulletPool[i].uEnd - UIStarmap.viewTargetStatic) * 0.00025;
					}
					else if (DysonSphere.renderPlace == ERenderPlace.Dysonmap)
					{
						__instance.bulletPool[i].rBegin = (__instance.bulletPool[i].uBegin - __instance.starData.uPosition) * 0.00025;
						__instance.bulletPool[i].rEnd = (__instance.bulletPool[i].uEnd - __instance.starData.uPosition) * 0.00025;
					}
				}
			}
			for (int j = 1; j < __instance.orbitCursor; j++)
			{
				if (__instance.orbits[j].id == j && __instance.orbits[j].count == 0 && !__instance.orbits[j].enabled)
				{
					__instance.RemoveOrbit(j);
				}
			}
			if (Traverse.Create(__instance).Field("bulletBuffer").GetValue() == null)
			{
				Traverse.Create(__instance).Field("bulletBuffer").SetValue(new ComputeBuffer(__instance.bulletCapacity, 112, ComputeBufferType.Default));
			}
			if (((ComputeBuffer)Traverse.Create(__instance).Field("bulletBuffer").GetValue()).count != __instance.bulletCapacity)
			{
				Traverse.Create(__instance).Field("bulletBuffer").Method("Release").GetValue();
				Traverse.Create(__instance).Field("bulletBuffer").SetValue(new ComputeBuffer(__instance.bulletCapacity, 112, ComputeBufferType.Default));
			}
			((ComputeBuffer)Traverse.Create(__instance).Field("bulletBuffer").GetValue()).SetData(__instance.bulletPool);
			PerformanceMonitor.EndSample(ECpuWorkEntry.DysonBullet);
			return false;
		}
		*/


	}


}
