using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    public class Relic
    {
        // type 遗物类型 0=legend 1=epic 2=rare 3=common
        // 二进制存储已获取的遗物，需要存档
        public static int[] relics = { 0, 0, 0, 0 };

        public static int relicHoldMax = 8; // 最多可以持有的遗物数
        public static int[] maxRelic = { 10, 12, 18, 18 }; // 当前版本各种类型的遗物各有多少种，每种类型均不能大于30
        public static double[] relicTypeProbability = { 0.03, 0.08, 0.2, 1 }; // 各类型遗物刷新的概率，注意不是权重
        public static double firstRelicIsRare = 0.5; // 第一个遗物至少是稀有的概率
        public static bool canSelectNewRelic = false; // 当canSelectNewRelic为true时点按按钮才是有效的选择
        public static int[] alternateRelics = { -1, -1, -1 }; // 三个备选，百位数字代表稀有度类型，0代表传说，个位十位是遗物序号。
        public static int basicMatrixCost = 10; // 除每次随机赠送的一次免费随机之外，从第二次开始需要消耗的矩阵的基础值（这个第二次以此基础值的2倍开始）
        public static int rollCount = 0;
        public static int AbortReward = 500; // 放弃解译圣物直接获取的矩阵数量
        public static List<int> starsWithMegaStructure = new List<int>(); // 每秒更新，具有巨构的星系。但无论如何，使用这些序号的戴森球时一定要检查是否为null，因为加载新存档后可能不是及时更新等情况。

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RelicGameTick(long time)
        {
            if (time % 60 == 7)
                RefreshStarsWithMegaStructure();

        }

        static int N(int num)
        {
            return (int)Math.Pow(2,num);
        }

        public static void InitAll()
        {
            UIRelic.InitAll();
            canSelectNewRelic = false;
            rollCount = 0;
        }

        public static int AddRelic(int type, int num)
        {
            if (num > 30) return -1;
            if (type > 3 || type < 0) return -2;
            if ((relics[type] & 1<<num ) > 0 ) return 0;

            relics[type] |= 1<<num;
            return 1;
        }

        public static bool HaveRelic(int type, int num)
        {
            if (Configs.developerMode) return true;
            if (type > 3 || type < 0 || num > 30) return false;
            if ((relics[type] & (1<<num)) > 0 ) return true;
            return false;
        }

        // 输出遗物数量，type输入-1为获取全部类型的遗物数量总和
        static int GetRelicCount(int type = -1)
        {
            if (type < 0 || type > 3)
            {
                return GetRelicCount(0) + GetRelicCount(1) + GetRelicCount(2) + GetRelicCount(3);
            }
            else
            {
                int r = relics[type];
                int count = 0;
                while (r > 0)
                {
                    r = r & (r - 1);
                    count++;
                }
                return count;
            }
        }

        // 允许玩家选择一个新的遗物
        public static bool PrepareNewRelic()
        {
            if (GetRelicCount() >= relicHoldMax) return false;
            rollCount = -1; // 从-1开始是因为每次准备给玩家新的relic都要重新随机一次
            canSelectNewRelic = true;

            UIRelic.OpenSelectionWindow();
            UIRelic.ShowSlots(); // 打开已有遗物栏

            return true;
        }


        public static void InitRelicData()
        {

        }


        // 刷新保存当前存在巨构的星系
        public static void RefreshStarsWithMegaStructure()
        {
            starsWithMegaStructure.Clear();
            for (int i = 0; i < GameMain.data.galaxy.starCount; i++)
            {
                if (GameMain.data.dysonSpheres.Length > i)
                {
                    DysonSphere sphere = GameMain.data.dysonSpheres[i];
                    if (sphere != null)
                    {
                        if (sphere.totalStructurePoint + sphere.totalCellPoint - sphere.totalConstructedStructurePoint - sphere.totalConstructedCellPoint > 0)
                        {
                            starsWithMegaStructure.Add(i);
                        }
                    }
                }
            }
            
        }


        public static bool Verify(double possibility)
        {
            if (Utils.RandDouble() < possibility)
                return true;
            else if ((relics[0] & 1 << 9) > 0) // 具有增加幸运的遗物，则可以再判断一次
                return (Utils.RandDouble() < possibility);

            return false;
        }

        // 任何额外伤害都需要经过此函数来计算并处理，dealDamage默认为false，代表只用这个函数计算而尚未实际造成伤害
        public static int BonusDamage(double damage, double bonus, bool DealDamage = false)
        {
            if (HaveRelic(2, 13))
            {
                bonus = 2 * bonus * damage;
                
            }
            else
            {
                bonus = bonus * damage;
            }
            if (HaveRelic(0, 8) && DealDamage) // relic0-8 饮血剑效果
            {
                int starIndex = Configs.nextWaveStarIndex;
                if (starIndex > 0)
                {
                    int planetId = (starIndex + 1) * 100 + Utils.RandInt(1, GameMain.galaxy.stars[starIndex].planetCount + 1);
                    int shieldRestore = (int)(bonus * 0.1);
                    if (ShieldGenerator.currentShield.GetOrAdd(planetId, 0) < ShieldGenerator.maxShieldCapacity.GetOrAdd(planetId, 0) * 1.5)
                    {
                        ShieldGenerator.maxShieldCapacity.AddOrUpdate(planetId, shieldRestore, (x, y) => y + shieldRestore);
                        UIBattleStatistics.RegisterShieldRestoreInBattle(shieldRestore);
                    }
                }
            }
            return (int)(damage + bonus);
        }

        // 有限制地建造某一(starIndex为-1时则是随机的)巨构的固定数量(amount)的进度，不因层数、节点数多少而改变一次函数建造的进度量
        public static void AutoBuildMegaStructure(int starIndex = -1, int amount = 1)
        {
            if (starsWithMegaStructure.Count <= 0) 
                return;
            if (starIndex < 0)
            {
                starIndex = starsWithMegaStructure[Utils.RandInt(0, starsWithMegaStructure.Count)]; // 可能会出现点数被浪费的情况，因为有的巨构就差一点cell完成，差的那些正在吸附，那么就不会立刻建造，这些amount就被浪费了，但完全建成的巨构不会被包含在这个列表中所以不会经常大量浪费
            }
            if (starIndex >= 0 && starIndex < GameMain.data.dysonSpheres.Length)
            {
                DysonSphere sphere = GameMain.data.dysonSpheres[starIndex];
                if (sphere != null)
                {
                    for (int i = 0; i < sphere.layersIdBased.Length; i++)
                    {
                        DysonSphereLayer dysonSphereLayer = sphere.layersIdBased[i];
                        if (dysonSphereLayer != null)
                        {
                            int num = dysonSphereLayer.nodePool.Length;
                            for (int j = 0; j < num; j++)
                            {
                                DysonNode dysonNode = dysonSphereLayer.nodePool[j];
                                if (dysonNode != null)
                                {
                                    for (int k = 0; k < Math.Min(6, amount); k++)
                                    {
                                        if (dysonNode.spReqOrder > 0)
                                        {
                                            sphere.OrderConstructSp(dysonNode);
                                            sphere.ConstructSp(dysonNode);
                                            amount--;
                                        }
                                    }
                                    for (int l = 0; l < Math.Min(6, amount); l++)
                                    {
                                        if (dysonNode.cpReqOrder > 0)
                                        {
                                            dysonNode.cpOrdered++;
                                            dysonNode.ConstructCp();
                                            amount--;
                                        }
                                    }
                                }
                                if (amount <= 0) return;
                            }
                        }
                    }
                }
            }
        }


        public static void Export(BinaryWriter w)
        {
            w.Write(relics[0]);
            w.Write(relics[1]);
            w.Write(relics[2]);
            w.Write(relics[3]);
        }

        public static void Import(BinaryReader r)
        {
            if (Configs.versionWhenImporting >= 30221025)
            {
                relics[0] = r.ReadInt32();
                relics[1] = r.ReadInt32();
                relics[2] = r.ReadInt32();
                relics[3] = r.ReadInt32();
            }
            else
            {
                relics[0] = 0;
                relics[1] = 0;
                relics[2] = 0;
                relics[3] = 0;
            }
            InitAll();
        }

        public static void IntoOtherSave()
        {
            relics[0] = 0;
            relics[1] = 0;
            relics[2] = 0;
            relics[3] = 0;
            InitAll();
        }
    }


    public class RelicFunctionPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RelicFunctionGameTick(long time)
        {
            if (time % 60 == 8)
                CheckMegaStructureAttack();
        }



        /// <summary>
        /// relic 0-1 0-2
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="power"></param>
        /// <param name="productRegister"></param>
        /// <param name="consumeRegister"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AssemblerComponent), "InternalUpdate")]
        public static bool AssemblerInternalUpdatePatch(ref AssemblerComponent __instance, float power, int[] productRegister, int[] consumeRegister)
        {
            if (power < 0.1f)
                return true;

            if (__instance.recipeType == ERecipeType.Assemble)
            {
                // relic0-1 蓝buff效果
                if (Relic.HaveRelic(0, 1) && __instance.requires.Length > 1)
                {
                    // 原材料未堆积过多才会返还，产物堆积未被取出则不返还
                    if (__instance.served[0] < 10 * __instance.requireCounts[0])
                    {
                        // Utils.Log("time = " + __instance.time + " / " + __instance.timeSpend); 这里是能输出两个相等的值的
                        // 不能直接用__instance.time >= __instance.timeSpend代替，必须-1，即便已经相等却无法触发，为什么？
                        if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                        {
                            if(__instance.served[0] > 0)
                                __instance.incServed[0] += __instance.incServed[0] / __instance.served[0] * __instance.productCounts[0]; // 增产点数也要返还
                            __instance.served[0] += __instance.productCounts[0]; // 注意效果是每产出一个产物返还一个1号材料而非每次产出
                            int[] obj = consumeRegister;
                            lock (obj)
                            {
                                consumeRegister[__instance.requires[0]] -= __instance.productCounts[0];
                            }
                        }
                        if (__instance.extraTime >= __instance.extraTimeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                        {
                            if (__instance.served[0] > 0)
                                __instance.incServed[0] += __instance.incServed[0] / __instance.served[0] * __instance.productCounts[0];
                            __instance.served[0] += __instance.productCounts[0]; 
                            int[] obj = consumeRegister;
                            lock (obj)
                            {
                                consumeRegister[__instance.requires[0]] -= __instance.productCounts[0];
                            }
                        }
                        
                    }
                }
            }
            else if (__instance.recipeType == ERecipeType.Chemical)
            {
                // relic0-2 女神之泪效果
                if (Relic.HaveRelic(0, 2) && __instance.requires.Length > 1)
                {
                    if (__instance.served[0] < 20 * __instance.requireCounts[0])
                    {
                        if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 20 * __instance.productCounts[0])
                        {
                            if (__instance.served[0] > 0)
                                __instance.incServed[0] += __instance.incServed[0] / __instance.served[0] * __instance.requireCounts[0];
                            __instance.served[0] += __instance.requireCounts[0];
                            int[] obj = consumeRegister;
                            lock (obj)
                            {
                                consumeRegister[__instance.requires[0]] -= __instance.requireCounts[0];
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// relic0-3
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), "GameTick_Gamma")]
        public static void GammaReceiverPatch(ref PowerGeneratorComponent __instance)
        {
            if (Relic.HaveRelic(0, 3) && __instance.catalystPoint < 3600)
            {
                __instance.catalystPoint = 3500; // 为什么不是3600，因为3600在锅盖消耗后会计算一个透镜消耗
                __instance.catalystIncPoint = 14000; // 4倍是满增产
            }    
        }

        /// <summary>
        /// relic0-4
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="eta"></param>
        /// <param name="__result"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), "EnergyCap_Gamma_Req")]
        public static bool EnergyCapGammaReqPatch(ref PowerGeneratorComponent __instance, float eta, ref long __result)
        {
            if (!Relic.HaveRelic(0, 4))
                return true;

            __instance.currentStrength = 1;
            float num2 = (float)Cargo.accTableMilli[__instance.catalystIncLevel];
            __instance.capacityCurrentTick = (long)(__instance.currentStrength * (1f + __instance.warmup * 1.5f) * ((__instance.catalystPoint > 0) ? (2f * (1f + num2)) : 1f) * ((__instance.productId > 0) ? 8f : 1f) * (float)__instance.genEnergyPerTick);
            eta = 1f - (1f - eta) * (1f - __instance.warmup * __instance.warmup * 0.4f);
            __instance.warmupSpeed = 0.25f * 4f * 1.3888889E-05f;
            __result = (long)((double)__instance.capacityCurrentTick / (double)eta + 0.49999999);
            return false;
        }

        /// <summary>
        /// relic0-7
        /// </summary>
        public static void CheckMegaStructureAttack()
        {
            if (Configs.nextWaveState == 3 && Relic.HaveRelic(0,7) && Relic.starsWithMegaStructure.Contains(Configs.nextWaveStarIndex))
            {
                int starIndex = Configs.nextWaveStarIndex;
                if (starIndex < GameMain.data.dysonSpheres.Length && starIndex >= 0) // 不用判断starIndex<1000了吧
                {
                    if (GameMain.data.dysonSpheres[starIndex] != null)
                    {
                        int damage = (int)(Math.Sqrt(Math.Max(0, GameMain.data.dysonSpheres[starIndex].energyGenCurrentTick)) / 100.0); // 伤害=每tick能量开平方后除以10
                        if (Configs.developerMode) damage = damage * 10;
                        damage = Relic.BonusDamage(damage, 1) - damage;
                        if (MoreMegaStructure.MoreMegaStructure.StarMegaStructureType[starIndex] == 6) damage *= 3;
                        foreach (var ship in EnemyShips.minTargetDisSortedShips[starIndex])
                        {
                            if (ship.state == EnemyShip.State.active)
                            {
                                ship.BeAttacked(damage);
                                UIBattleStatistics.RegisterMegastructureAttack(damage);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// relic0-8 不方便在BonusDamage里面计算给护盾回复的（比如子弹、导弹等非即时命中的），命中时，在这里强制计算护盾回复
        /// </summary>
        /// <param name="bonusDamage"></param>
        public static void ForceApplyBloodthirster(int bonusDamage)
        {
            if (Relic.HaveRelic(0, 8)) // relic0-8 饮血剑效果
            {
                int starIndex = Configs.nextWaveStarIndex;
                if (starIndex > 0)
                {
                    int planetId = (starIndex + 1) * 100 + Utils.RandInt(1, GameMain.galaxy.stars[starIndex].planetCount + 1);
                    int shieldRestore = (int)(bonusDamage * 0.1);
                    if (ShieldGenerator.currentShield.GetOrAdd(planetId, 0) < ShieldGenerator.maxShieldCapacity.GetOrAdd(planetId, 0) * 1.5)
                    {
                        ShieldGenerator.maxShieldCapacity.AddOrUpdate(planetId, shieldRestore, (x, y) => y + shieldRestore);
                        UIBattleStatistics.RegisterShieldRestoreInBattle(shieldRestore);
                    }
                }
            }
        }
    }
   
}
