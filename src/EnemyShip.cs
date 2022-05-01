using System;
using System.IO;
using UnityEngine;

namespace DSP_Battle
{
    public class EnemyShip
    {
        public ShipData shipData;
        public ShipRenderingData renderingData;
        public ShipUIRenderingData renderingUIData;
        public int hp;
        public float maxSpeed;
        public int intensity;
        public int damageRange;
        public int countDown;
        public int wormholeIndex;
        public bool isFiring = false;
        public bool isBlockedByShield = false;
        // 强制位移参数，时间很短不需要存档
        public VectorLF3 forceDisplacement;
        public int forceDisplacementTime = 0;
        public float movePerTick; //每tick位移占剩余位移的百分之多少

        public static int minForcedMove = 0; //每tick产生的最小强制位移


        public enum State
        {
            active,
            distroyed,
            landed,
            uninitialized,
        }
        public State state;

        public VectorLF3 uPos
        {
            get { return shipData.uPos; }
            set { shipData.uPos = value; }
        }

        public int shipIndex
        {
            get { return shipData.shipIndex; }
        }

        public double distanceToTarget
        {
            get
            {
                if (state != State.active) return -1;
                AstroPose[] astroPoses = GameMain.data.galaxy.astroPoses;
                StationComponent station = targetStation;
                if (station == null) return -1;

                AstroPose pose = astroPoses[shipData.planetB];
                VectorLF3 targetUPos = pose.uPos + Maths.QRotateLF(pose.uRot, station.shipDockPos + station.shipDockPos.normalized * 25f);
                return (targetUPos - uPos).magnitude;
            }
        }

        public double threat
        {
            get
            {
                if (state != State.active) return -1;
                return hp * shipData.uSpeed / distanceToTarget;
            }
        }

        public StationComponent targetStation
        {
            get
            {
                if (shipData.otherGId < 0) return null;
                StationComponent[] gStationPool = GameMain.data.galacticTransport.stationPool;
                if (gStationPool.Length <= shipData.otherGId) return null;
                StationComponent station = gStationPool[shipData.otherGId];
                return EnemyShips.ValidStellarStation(station) ? station : null;
            }
        }

        public int starIndex
        {
            get { return shipData.planetB / 100 - 1; }
        }

        public EnemyShip(BinaryReader r)
        {
            shipData = new ShipData();
            renderingData = new ShipRenderingData();
            renderingUIData = new ShipUIRenderingData();

            Import(r);
        }

        public EnemyShip(int gid, int stationId, int wormholeIndex, int enemyId, int countDown)
        {
            shipData = new ShipData();
            shipData.direction = 1;
            shipData.stage = 0;

            shipData.direction = 1;
            shipData.stage = 0;
            shipData.uAngularVel = Vector3.zero;
            shipData.uAngularSpeed = 0;
            shipData.shipIndex = gid;
            this.wormholeIndex = wormholeIndex;

            shipData.otherGId = stationId;
            shipData.planetB =  GameMain.data.galacticTransport.stationPool[stationId].planetId;
            
            shipData.uPos = Configs.nextWaveWormholes[wormholeIndex].uPos;
            shipData.itemId = Configs.enemyItemIds[enemyId];
            shipData.inc = Configs.enemyLandCnt[enemyId];
            shipData.uRot = new Quaternion((float)DspBattlePlugin.randSeed.NextDouble(), (float)DspBattlePlugin.randSeed.NextDouble(), (float)DspBattlePlugin.randSeed.NextDouble(), (float)DspBattlePlugin.randSeed.NextDouble());
            shipData.uRot.Normalize();
            maxSpeed = Configs.enemySpeed[enemyId];
            if (Configs.difficulty == 1) maxSpeed += maxSpeed / 2;
            shipData.uSpeed = ((float)DspBattlePlugin.randSeed.NextDouble()) * 0.25f * maxSpeed;
            hp = Configs.enemyHp[enemyId];
            if (Configs.difficulty == 1) hp *= 2;
            damageRange = Configs.enemyRange[enemyId];
            intensity = Configs.enemyIntensity[enemyId];
            this.countDown = countDown;

            state = countDown > 0 ? State.uninitialized : State.active;

            renderingData = new ShipRenderingData();
            renderingData.SetEmpty();
            renderingData.gid = gid;

            renderingUIData = new ShipUIRenderingData();
            renderingUIData.SetEmpty();
            renderingUIData.gid = gid;

            isFiring = false;
            isBlockedByShield = false;
        }

        public double distanceTo(VectorLF3 pos)
        {
            return (pos - uPos).magnitude;
        }

        public int BeAttacked(int atk)
        {
            int result = 0;
            lock (this)
            {
                if (state != State.active) return 0;
                result = hp < atk ? hp : atk;
                hp -= atk;
                if (hp <= 0)
                {
                    UIBattleStatistics.RegisterEliminate(intensity); //记录某类型的敌人被摧毁
                    UIBattleStatistics.RegisterIntercept(this); //记录拦截距离
                    hp = 0;
                    state = State.distroyed;
                    EnemyShips.OnShipDistroyed(this);
                    Rank.AddExp(intensity * 10); //获得经验
                    try //根据期望掉落矩阵
                    {
                        double dropExpectation = intensity * 1.0 / Configs.nextWaveIntensity * Configs.nextWaveMatrixExpectation;
                        int dropItemId = 8032;
                        if (GameMain.history.TechUnlocked(1924))
                        {
                            dropExpectation *= 50;
                            dropItemId = 8033;
                        }
                        if (dropExpectation > 1) //期望超过1的部分必然掉落
                        {
                            int guaranteed = (int)dropExpectation;
                            dropExpectation -= guaranteed;

                            GameMain.mainPlayer.TryAddItemToPackage(dropItemId, guaranteed, 0, true);
                            Utils.UIItemUp(dropItemId, guaranteed, 180);
                            UIBattleStatistics.RegisterAlienMatrixGain(guaranteed);
                        }
                        if (Utils.RandDouble() < dropExpectation) //根据概率决定是否掉落
                        {
                            GameMain.mainPlayer.TryAddItemToPackage(dropItemId, 1, 0, true);
                            Utils.UIItemUp(dropItemId, 1, 180);
                            UIBattleStatistics.RegisterAlienMatrixGain(1);
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
            return result;
        }

        public void FindAnotherStation()
        {
            int nextStationId = EnemyShips.FindNearestPlanetStation(GameMain.galaxy.PlanetById(shipData.planetB).star, shipData.uPos);
            if (nextStationId < 0)
            {
                shipData.otherGId = -1;
                shipData.stage = 0;
                maxSpeed *= 10;
                UIBattleStatistics.RegisterIntercept(this, 0); //当找不到目标说明星系内物流塔都被毁了，那么所有剩余船的拦截距离都要被注册为0，即已经冲到脸上了
                // state = State.distroyed;
                return;
            }

            shipData.stage = 0;
            shipData.otherGId = nextStationId;
            shipData.planetB = GameMain.data.galacticTransport.stationPool[nextStationId].planetId;
        }

        public void InitForceDisplacement(VectorLF3 targetUPos, int needTime = 40, float moveFactor = 0.04f)
        {
            forceDisplacement = targetUPos;
            forceDisplacementTime = needTime;
            movePerTick = moveFactor;
        }

        public void Update(long time)
        {
            VectorLF3 wormholePos = Configs.nextWaveWormholes[wormholeIndex].uPos;
            if (state == State.uninitialized)
            {
                shipData.uPos = wormholePos;
                if (time % 60 != 0) return;
                countDown--;
                if (countDown <= 0) state = State.active;
            }

            if (state != State.active) return;

            //强制位移
            if (forceDisplacementTime > 0 && forceDisplacement != null && distanceToTarget > 3000) //最后一个判断条件让其不要在距离地表过近的位置被强制位移
            {
                VectorLF3 direction = forceDisplacement - shipData.uPos;
                double fullDistance = direction.magnitude;
                double forceDispDistance = fullDistance * movePerTick + minForcedMove;
                if (fullDistance <= minForcedMove)
                {
                    forceDisplacementTime = 1;
                    forceDispDistance = fullDistance;
                }
                shipData.uPos = shipData.uPos + direction.normalized * forceDispDistance;
                forceDisplacementTime -= 1;
            }

            StationComponent station = targetStation;
            if (station == null || station.id == 0)
            {
                if (shipData.otherGId >= 0) FindAnotherStation();
                // if (state != State.active) return;
            }

            //如果目标行星有护盾且距离行星小于开火射程，则开火
            int planetId = shipData.planetB;
            int shipTypeNum = Configs.enemyIntensity2TypeMap[intensity];
            if (ShieldGenerator.currentShield.ContainsKey(planetId) && ShieldGenerator.currentShield[planetId] > 0 && distanceToTarget <= Configs.enemyFireRange[shipTypeNum] && shipData.otherGId >= 0)
            {
                if (shipTypeNum != 3) //3号是自爆船，不能开火
                    isFiring = true;
                else
                    isFiring = false;
                //目前设定是只要护盾量大于1就可以阻挡飞船，但是护盾没有战时回复。飞船被阻挡的位置约为其射程的90%。
                if (ShieldGenerator.currentShield[planetId] > ShieldRenderer.shieldRenderMin && distanceToTarget <= Configs.enemyFireRange[shipTypeNum] * 0.9f)
                {
                    if (shipTypeNum != 3)
                        isBlockedByShield = true;
                    else
                        isBlockedByShield = false;
                    //如果在护盾>有效护盾（暂定50000）的情况下撞上了护盾，船承受巨量伤害，同时也会对护盾造成伤害。自爆船将会造成巨量伤害，暂定为intensity=9的3号船
                    if((GameMain.galaxy.PlanetById(planetId).uPosition - uPos).magnitude <= ShieldRenderer.shieldRadius * 810)
                    {
                        UIBattleStatistics.RegisterShieldAttack(BeAttacked(999999));
                        ShieldGenerator.currentShield.AddOrUpdate(planetId, 0, (x, y) => y - Configs.enemyDamagePerBullet[shipTypeNum]);
                        if(ShieldGenerator.currentShield.GetOrAdd(planetId, 0) < 0)
                            ShieldGenerator.currentShield.AddOrUpdate(planetId, 0, (x, y) => 0);
                        UIBattleStatistics.RegisterShieldTakeDamage(Configs.enemyDamagePerBullet[shipTypeNum]);

                        //爆炸效果
                        DysonSwarm swarm = GameMain.data.dysonSpheres[planetId / 100 - 1]?.swarm;
                        if (swarm != null)
                        {
                            int maxi = 1;
                            if (shipTypeNum == 3) maxi = 10;
                            for (int i = 0; i < maxi; i++)
                            {
                                int bulletIndex = swarm.AddBullet(new SailBullet
                                {
                                    maxt = 0.016667f * i,
                                    lBegin = new Vector3(0, 0, 0),
                                    uEndVel = new Vector3(0, 0, 0),
                                    uBegin = uPos,
                                    uEnd = uPos
                                }, 1);

                                swarm.bulletPool[bulletIndex].state = 0;
                            }
                        }

                    }
                }
                else
                    isBlockedByShield = false;
            }
            else
            {
                isFiring = false;
                isBlockedByShield = false;
            }

            Quaternion quaternion = Quaternion.identity;
            bool flag7 = false;
            if (shipData.stage == 0) UpdateStage0(out quaternion, out flag7);
            else if (shipData.stage == 1) UpdateStage1();

            PlanetData planet = GameMain.galaxy.PlanetById(shipData.planetB);
            renderingData.SetPose(shipData.uPos, flag7 ? quaternion : shipData.uRot, GameMain.data.relativePos, GameMain.data.relativeRot, shipData.uVel * shipData.uSpeed, shipData.itemId);
            renderingUIData.SetPose(shipData.uPos, flag7 ? quaternion : shipData.uRot, (float)(
                planet.star.uPosition - (shipData.otherGId < 0 ? wormholePos : planet.uPosition)).magnitude, shipData.uSpeed, shipData.itemId);
            if (renderingData.anim.z < 0) renderingData.anim.z = 0;

        }

        private void UpdateStage0(out Quaternion quaternion, out bool flag7)
        {
            //飞船开火
            if(isFiring)
            {
                int shipTypeNum = Configs.enemyIntensity2TypeMap[intensity];
                int interval = Configs.enemyFireInterval[shipTypeNum];
                if(GameMain.instance.timei % interval == shipIndex % interval)
                {
                    //造成伤害
                    int planetId = shipData.planetB;
                    ShieldGenerator.currentShield.AddOrUpdate(planetId, 0, (x, y) => y - Configs.enemyDamagePerBullet[shipTypeNum]);
                    if (ShieldGenerator.currentShield[planetId] < 0) 
                        ShieldGenerator.currentShield.AddOrUpdate(planetId, 0, (x, y) => 0);
                    UIBattleStatistics.RegisterShieldTakeDamage(Configs.enemyDamagePerBullet[shipTypeNum]);
                    //子弹
                    DysonSwarm swarm = RendererSphere.enemySpheres[planetId / 100 - 1]?.swarm;
                    if(swarm!=null)
                    {
                        int bulletIndex = swarm.AddBullet(new SailBullet
                        {
                            maxt = 0.1f,
                            lBegin = new Vector3(0, 0, 0),
                            uEndVel = new Vector3(0, 0, 0),
                            uBegin = uPos,
                            uEnd = (uPos - GameMain.galaxy.PlanetById(planetId).uPosition).normalized * ShieldRenderer.shieldRadius * 820 + GameMain.galaxy.PlanetById(planetId).uPosition
                        }, 1);

                        swarm.bulletPool[bulletIndex].state = 0;
                    }
                }

            }

            //飞船移动
            // float shipSailSpeed = GameMain.history.logisticShipSailSpeedModified;
            float shipSailSpeed = maxSpeed;

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

            quaternion = Quaternion.identity;
            flag7 = false;
            StationComponent station = targetStation;

            AstroPose astroPose2 = astroPoses[shipData.planetB];
            VectorLF3 lhs3 =
                station == null ? Configs.nextWaveWormholes[wormholeIndex].uPos : 
                (astroPose2.uPos + Maths.QRotateLF(astroPose2.uRot, station.shipDockPos + station.shipDockPos.normalized * 25f));
            VectorLF3 vectorLF = lhs3 - shipData.uPos;
            double num38 = Math.Sqrt(vectorLF.x * vectorLF.x + vectorLF.y * vectorLF.y + vectorLF.z * vectorLF.z);
            bool flag8 = false;
            bool flag9 = false;

            if (station == null && num38 < 1000.0)
            {
                state = State.distroyed;
                return;
            }

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
            else if(!isBlockedByShield)
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
            else
            {
                if (shipData.uSpeed > 200)
                    shipData.uSpeed = shipData.uSpeed * 0.9f - 180;
                else
                    shipData.uSpeed = 0;
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
                quaternion = Quaternion.Slerp(shipData.uRot, astroPose2.uRot * (station.shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f)), num77);

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
        private void UpdateStage1()
        {
            float shipSailSpeed = maxSpeed;
            float num31 = Mathf.Sqrt(shipSailSpeed / 600f);
            float num36 = num31 * 0.006f + 1E-05f;
            AstroPose[] astroPoses = GameMain.data.galaxy.astroPoses;
            StationComponent station = targetStation;

            AstroPose astroPose3 = astroPoses[shipData.planetB];
            float num78 = 0f;
            if (shipData.direction > 0)
            {
                shipData.t -= num36 * 0.6666667f;
                num78 = shipData.t;
                if (shipData.t < 0f)
                {
                    shipData.t = 0f;
                    state = State.landed;
                    return;

                }

                num78 = (3f - num78 - num78) * num78 * num78;
                float num79 = num78 * 2f;
                float num80 = num78 * 2f - 1f;
                VectorLF3 lhs7 = astroPose3.uPos + Maths.QRotateLF(astroPose3.uRot, station.shipDockPos + station.shipDockPos.normalized * 7.27000046f);
                if (num78 > 0.5f)
                {
                    VectorLF3 lhs8 = astroPose3.uPos + Maths.QRotateLF(astroPose3.uRot, shipData.pPosTemp);
                    shipData.uPos = lhs7 * (1f - num80) + lhs8 * num80;
                    shipData.uRot = astroPose3.uRot * Quaternion.Slerp(station.shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f), shipData.pRotTemp, num80 * 1.5f - 0.5f);
                }
                else
                {
                    VectorLF3 lhs9 = astroPose3.uPos + Maths.QRotateLF(astroPose3.uRot, station.shipDockPos + station.shipDockPos.normalized * -14.4f);
                    shipData.uPos = lhs9 * (1f - num79) + lhs7 * num79;
                    shipData.uRot = astroPose3.uRot * (station.shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f));
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
                shipData.uPos = astroPose3.uPos + Maths.QRotateLF(astroPose3.uRot, station.shipDockPos + station.shipDockPos.normalized * (-14.4f + 39.4f * num78));
                shipData.uRot = astroPose3.uRot * (station.shipDockRot * new Quaternion(0.707106769f, 0f, 0f, -0.707106769f));
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

        public void Export(BinaryWriter w)
        {
            shipData.Export(w);
            w.Write(hp);
            w.Write(maxSpeed);
            w.Write((int)state);
            w.Write(intensity);
            w.Write(damageRange);
            w.Write(countDown);
        }

        public void Import(BinaryReader r)
        {
            shipData.Import(r);
            hp = r.ReadInt32();
            maxSpeed = r.ReadSingle();
            state = (State)r.ReadInt32();
            intensity = r.ReadInt32();
            damageRange = r.ReadInt32();
            countDown = r.ReadInt32();

            renderingData.SetEmpty();
            renderingData.gid = shipData.shipIndex;
            renderingUIData.SetEmpty();
            renderingUIData.gid = shipData.shipIndex;
            isFiring = false;
            isBlockedByShield =false;
        }

    }
}

