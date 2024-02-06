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
    public class Relic
    {
        // type 遗物类型 0=legend 1=epic 2=rare 3=common
        // 二进制存储已获取的遗物，需要存档
        public static int[] relics = { 0, 0, 0, 0 };
        // 其他需要存档的数据
        public static int relic0_2Version = 1; // 女神泪重做，老存档此项为0不改效果，新存档此项为1才改效果
        public static int relic0_2Charge = 0; // 新版女神泪充能计数
        public static int relic0_2CanActivate = 1; // 新版女神泪在每次入侵中只能激活一次，激活后设置为0。下次入侵才设置为1
        public static int minShieldPlanetId = -1; // 饮血剑现在会给护盾量最低的星球回盾，但是每秒才更新一次护盾量最低的星球

        //不存档的设定参数
        public static int relicHoldMax = 8; // 最多可以持有的遗物数
        public static int[] relicNumByType = { 11, 12, 18, 18 }; // 当前版本各种类型的遗物各有多少种，每种类型均不能大于30
        public static double[] relicTypeProbability = { 0.03, 0.09, 0.2, 1 }; // 各类型遗物刷新的概率，注意不是权重，是有次序地判断随机数
        public static double[] relicRemoveProbabilityByRelicCount = { 0, 0, 0, 0, 0.05, 0.1, 0.12, 0.15, 1, 1, 1 }; // 拥有i个reilc时，第三个槽位刷新的是删除relic的概率
        public static double firstRelicIsRare = 0.5; // 第一个遗物至少是稀有的概率
        public static bool canSelectNewRelic = false; // 当canSelectNewRelic为true时点按按钮才是有效的选择
        public static int[] alternateRelics = { -1, -1, -1 }; // 三个备选，百位数字代表稀有度类型，0代表传说，个位十位是遗物序号。
        public static int basicMatrixCost = 10; // 除每次随机赠送的一次免费随机之外，从第二次开始需要消耗的矩阵的基础值（这个第二次以此基础值的2倍开始）
        public static int rollCount = 0;
        public static int AbortReward = 500; // 放弃解译圣物直接获取的矩阵数量
        public static List<int> starsWithMegaStructure = new List<int>(); // 每秒更新，具有巨构的星系。
        public static List<int> starsWithMegaStructureUnfinished = new List<int>(); // 每秒更新，具有巨构且未完成建造的星系.
        public static Vector3 playerLastPos = new VectorLF3(0, 0, 0); // 上一秒玩家的位置
        public static bool alreadyRecalcDysonStarLumin = false; // 不需要存档，如果需要置false则会在读档时以及选择特定遗物时自动完成
        public static int dropletDamageGrowth = 10; // relic0-10每次水滴击杀的伤害成长
        public static int dropletDamageLimitGrowth = 400; // relic0-10每次消耗水滴提供的伤害成长上限的成长
        public static int relic0_2MaxCharge = 1000; // 新版女神泪充能上限

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RelicGameTick(long time)
        {
            if (time % 60 == 7)
                RefreshStarsWithMegaStructure();
            if (time % 60 == 8)
                RefreshMinShieldPlanet();

        }

        static int N(int num)
        {
            return (int)Math.Pow(2, num);
        }

        public static void InitAll()
        {
            starsWithMegaStructure.Clear();
            starsWithMegaStructureUnfinished.Clear();
            UIRelic.InitAll();
            canSelectNewRelic = false;
            rollCount = 0;
            Configs.relic1_8Protection = int.MaxValue;
            Configs.relic2_17Activated = 0;
            RelicFunctionPatcher.CheckSolarSailLife();
        }

        public static int AddRelic(int type, int num)
        {
            if (num > 30) return -1; // 序号不存在
            if (type > 3 || type < 0) return -2; // 稀有度不存在
            if ((relics[type] & 1 << num) > 0) return 0; // 已有
            if (GetRelicCount() >= relicHoldMax) return -3; // 超上限

            // 下面是一些特殊的Relic在选择时不是简单地改一个拥有状态就行，需要单独对待
            if(type == 0 && num == 2)
            {
                relics[type] |= 1 << num;
                relic0_2Version = 1;
                relic0_2Charge = 0;
                relic0_2CanActivate = 1;
            }
            else if (type == 0 && num == 3)
            {
                relics[type] |= 1 << num;
                RelicFunctionPatcher.CheckAndModifyStarLuminosity();
            }
            else if (type == 1 && num == 4)
            {
                GameMain.mainPlayer.TryAddItemToPackage(9511, 1, 0, true);
                relics[type] |= 1 << num;
            }
            else if (type == 3 && num == 5)
            {
                RelicFunctionPatcher.ReInitBattleRoundAndDiff();
            }
            else if (type == 3 && num == 8)
            {
                if (GameMain.history.techStates.ContainsKey(1002) && GameMain.history.techStates[1002].unlocked)
                {
                    GameMain.mainPlayer.ThrowTrash(6001, 1000, 0, 0);
                }
                if (GameMain.history.techStates.ContainsKey(1111) && GameMain.history.techStates[1111].unlocked)
                {
                    GameMain.mainPlayer.ThrowTrash(6002, 800, 0, 0);
                }
                if (GameMain.history.techStates.ContainsKey(1124) && GameMain.history.techStates[1124].unlocked)
                {
                    GameMain.mainPlayer.ThrowTrash(6003, 600, 0, 0);
                }
                if (GameMain.history.techStates.ContainsKey(1312) && GameMain.history.techStates[1312].unlocked)
                {
                    GameMain.mainPlayer.ThrowTrash(6004, 400, 0, 0);
                }
                if (GameMain.history.techStates.ContainsKey(1705) && GameMain.history.techStates[1705].unlocked)
                {
                    GameMain.mainPlayer.ThrowTrash(6001, 400, 0, 0);
                }
            }
            else
            {
                relics[type] |= 1 << num;
            }
            return 1;
        }

        public static void AskRemoveRelic(int removeType, int removeNum)
        {
            if (removeType > 3 || removeNum > 30)
            {
                UIMessageBox.Show("Failed".Translate(), "Failed. Unknown relic.".Translate(), "确定".Translate(), 1);
                RegretRemoveRelic();
                return;
            }
            else if (!Relic.HaveRelic(removeType, removeNum))
            {
                UIMessageBox.Show("Failed".Translate(), "Failed. Relic not have.".Translate(), "确定".Translate(), 1);
                RegretRemoveRelic();
                return;
            }
            UIMessageBox.Show("删除遗物确认标题".Translate(), String.Format( "删除遗物确认警告".Translate(), ("遗物名称" + removeType.ToString() + "-" + removeNum.ToString()).Translate().Split('\n')[0]),
            "否".Translate(), "是".Translate(), 1, new UIMessageBox.Response(RegretRemoveRelic), new UIMessageBox.Response(() =>
            {
                relics[removeType] = relics[removeType] ^ 1 << removeNum;

                //UIMessageBox.Show("成功移除！".Translate(), "已移除遗物描述".Translate() + ("遗物名称" + removeType.ToString() + "-" + removeNum.ToString()).Translate().Split('\n')[0], "确定".Translate(), 1);

                UIRelic.CloseSelectionWindow();
                UIRelic.RefreshSlotsWindowUI();
                UIRelic.HideSlots();
            }));
        }

        public static void RegretRemoveRelic()
        {
            canSelectNewRelic = true;
        }

        public static bool HaveRelic(int type, int num)
        {
            //if (Configs.developerMode &&( type == 0 && num == 5 ||  type == 1 && num == 9 )) return true;
            if (type > 3 || type < 0 || num > 30) return false;
            if ((relics[type] & (1 << num)) > 0) return true;
            return false;
        }

        // 输出遗物数量，type输入-1为获取全部类型的遗物数量总和
        public static int GetRelicCount(int type = -1)
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
            //if (GetRelicCount() >= relicHoldMax) return false;
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
            starsWithMegaStructureUnfinished.Clear();
            for (int i = 0; i < GameMain.data.galaxy.starCount; i++)
            {
                if (GameMain.data.dysonSpheres.Length > i)
                {
                    DysonSphere sphere = GameMain.data.dysonSpheres[i];
                    if (sphere != null)
                    {
                        starsWithMegaStructure.Add(i);
                        if (sphere.totalStructurePoint + sphere.totalCellPoint - sphere.totalConstructedStructurePoint - sphere.totalConstructedCellPoint > 0)
                        {
                            starsWithMegaStructureUnfinished.Add(i);
                        }
                    }
                }
            }
        }

        // 刷新保存护盾量最低的行星
        public static void RefreshMinShieldPlanet()
        {
            
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
            return (int)(damage + bonus);
        }

        // 有限制地建造某一(starIndex为-1时则是随机的)巨构的固定数量(amount)的进度，不因层数、节点数多少而改变一次函数建造的进度量
        public static void AutoBuildMegaStructure(int starIndex = -1, int amount = 12)
        {
            if (starsWithMegaStructureUnfinished.Count <= 0)
                return;
            if (starIndex < 0)
            {
                starIndex = starsWithMegaStructureUnfinished[Utils.RandInt(0, starsWithMegaStructureUnfinished.Count)]; // 可能会出现点数被浪费的情况，因为有的巨构就差一点cell完成，差的那些正在吸附，那么就不会立刻建造，这些amount就被浪费了，但完全建成的巨构不会被包含在这个列表中，前面的情况也不会经常发生，所以不会经常大量浪费
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
                                            amount -= 5; // 框架结构点数由于本身是需要火箭才能建造的，自然比细胞点数昂贵一些。这里设置为昂贵五倍。
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
            w.Write(relic0_2Version);
            w.Write(relic0_2Charge);
            w.Write(relic0_2CanActivate);
            w.Write(minShieldPlanetId);
        }

        public static void Import(BinaryReader r)
        {
            if (Configs.versionWhenImporting >= 30221025)
            {
                relics[0] = r.ReadInt32();
                relics[1] = r.ReadInt32();
                relics[2] = r.ReadInt32();
                relics[3] = r.ReadInt32();
                RelicFunctionPatcher.CheckAndModifyStarLuminosity();
            }
            else
            {
                relics[0] = 0;
                relics[1] = 0;
                relics[2] = 0;
                relics[3] = 0;
            }
            if (Configs.versionWhenImporting >= 30230426)
            {
                relic0_2Version = r.ReadInt32();
                relic0_2Charge = r.ReadInt32();
                relic0_2CanActivate = r.ReadInt32();
                minShieldPlanetId = r.ReadInt32();
            }
            else
            {
                relic0_2Version = 0;
                relic0_2Charge = 0;
                relic0_2CanActivate = 1;
                minShieldPlanetId = -1;
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
            else if (time % 60 == 9)
                AutoChargeShieldByMegaStructure();
            else if (time % 60 == 10)
                CheckPlayerHasaki();

            TryRecalcDysonLumin();
            AutoBuildMegaWhenWaveFinished();
        }



        /// <summary>
        /// relic 0-1 0-2 1-6 2-4 2-11 2-8 3-0 3-6 3-14
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
                // relic1-6
                if (Relic.HaveRelic(1, 6))
                {
                    if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                    {
                        int rocketId = __instance.products[0];
                        int rodNum = -1;
                        if (rocketId >= 9488 && rocketId <= 9490)
                            rodNum = 2;
                        else if (rocketId == 9491 || rocketId == 9492 || rocketId == 9510 || rocketId == 1503)
                            rodNum = 1;

                        if (rodNum > 0 && __instance.served[rodNum] < 10 * __instance.requireCounts[rodNum]) // 判断原材料是否已满
                        {
                            //if (__instance.served[rodNum] > 0)
                            //    __instance.incServed[rodNum] += __instance.incServed[rodNum] / __instance.served[rodNum] * 2; // 增产点数也要返还
                            __instance.incServed[rodNum] += 8; // 返还满级增产点数
                            __instance.served[rodNum] += 2;
                            int[] obj = consumeRegister;
                            lock (obj)
                            {
                                consumeRegister[__instance.requires[rodNum]] -= 2;
                            }
                        }
                    }
                }

                // relic2-4
                if (__instance.products[0] == 1801 || __instance.products[0] == 1802)
                {
                    if (Relic.HaveRelic(2, 4) && __instance.requires.Length > 1)
                    {
                        if (__instance.served[1] < 10 * __instance.requireCounts[1])
                        {
                            if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                            {
                                __instance.incServed[1] += 20; // 返还满级增产点数
                                __instance.served[1] += 5;
                                int[] obj = consumeRegister;
                                lock (obj)
                                {
                                    consumeRegister[__instance.requires[1]] -= 5;
                                }
                            }
                            if (__instance.extraTime >= __instance.extraTimeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                            {
                                __instance.incServed[1] += 20; // 返还满级增产点数
                                __instance.served[1] += 5;
                                int[] obj = consumeRegister;
                                lock (obj)
                                {
                                    consumeRegister[__instance.requires[1]] -= 5;
                                }
                            }

                        }
                    }
                }
                else if (__instance.products[0] == 1501 && Relic.HaveRelic(3, 0)) // relic3-0
                {
                    if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                    {
                        __instance.produced[0]++;
                        int[] obj = productRegister;
                        lock (obj)
                        {
                            productRegister[1501] += 1;
                        }
                    }
                    if (__instance.extraTime >= __instance.extraTimeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                    {
                        __instance.produced[0]++;
                        int[] obj = productRegister;
                        lock (obj)
                        {
                            productRegister[1501] += 1;
                        }
                    }
                }
                else if ((__instance.products[0] == 1303 || __instance.products[0] == 1305) && Relic.HaveRelic(3, 6)) // relic3-6
                {
                    if (__instance.replicating)
                    {
                        __instance.extraTime += (int)(0.5 * __instance.extraSpeed);
                    }
                }
                else if ((__instance.products[0] == 1203 || __instance.products[0] == 1204) && Relic.HaveRelic(3, 14)) // relic3-14
                {
                    int reloadNum = __instance.products[0] == 1203 ? 2 : 1;
                    if (__instance.served[reloadNum] < 10 * __instance.requireCounts[reloadNum])
                    {
                        if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                        {
                            __instance.incServed[reloadNum] += 4;
                            __instance.served[reloadNum] += 1;
                            int[] obj = consumeRegister;
                            lock (obj)
                            {
                                consumeRegister[__instance.requires[reloadNum]] -= 1;
                            }
                        }
                        if (__instance.extraTime >= __instance.extraTimeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                        {
                            __instance.incServed[reloadNum] += 4;
                            __instance.served[reloadNum] += 1;
                            int[] obj = consumeRegister;
                            lock (obj)
                            {
                                consumeRegister[__instance.requires[reloadNum]] -= 1;
                            }
                        }

                    }
                }

                // relic0-1 蓝buff效果 要放在最后面，因为前面有加time的遗物，所以这个根据time结算的要放在最后
                if (Relic.HaveRelic(0, 1) && __instance.requires.Length > 1)
                {
                    // 原材料未堆积过多才会返还，产物堆积未被取出则不返还。黑棒产线无视此遗物效果
                    if (__instance.served[0] < 10 * __instance.requireCounts[0] && __instance.products[0] != 1803)
                    {
                        // Utils.Log("time = " + __instance.time + " / " + __instance.timeSpend); 这里是能输出两个相等的值的
                        // 不能直接用__instance.time >= __instance.timeSpend代替，必须-1，即便已经相等却无法触发，为什么？
                        if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                        {
                            //if(__instance.served[0] > 0)
                            //    __instance.incServed[0] += __instance.incServed[0] / __instance.served[0] * __instance.productCounts[0]; // 增产点数也要返还
                            __instance.incServed[0] += 4 * __instance.productCounts[0]; // 返还满级增产点数
                            __instance.served[0] += __instance.productCounts[0]; // 注意效果是每产出一个产物返还一个1号材料而非每次产出，因此还需要在extraTime里再判断回填原料
                            int[] obj = consumeRegister;
                            lock (obj)
                            {
                                consumeRegister[__instance.requires[0]] -= __instance.productCounts[0];
                            }
                        }
                        if (__instance.extraTime >= __instance.extraTimeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                        {
                            //if (__instance.served[0] > 0)
                            //    __instance.incServed[0] += __instance.incServed[0] / __instance.served[0] * __instance.productCounts[0];
                            __instance.incServed[0] += 4 * __instance.productCounts[0]; // 返还满级增产点数
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
                            //if (__instance.served[0] > 0)
                            //    __instance.incServed[0] += __instance.incServed[0] / __instance.served[0] * __instance.requireCounts[0];
                            __instance.incServed[0] += 4 * __instance.requireCounts[0];
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
            else if (__instance.recipeType == ERecipeType.Smelt)
            {
                // relic 2-11 副产物提炼
                if (Relic.HaveRelic(2, 11))
                {
                    if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] + __instance.productCounts[0] < 100 && Relic.Verify(0.3))
                    {
                        __instance.produced[0]++;
                        int[] obj = productRegister;
                        lock (obj)
                        {
                            productRegister[__instance.products[0]] += 1;
                        }
                    }
                    if (__instance.extraTime >= __instance.extraTimeSpend - 1 && __instance.produced[0] + __instance.productCounts[0] < 100 && Relic.Verify(0.3))
                    {
                        __instance.produced[0]++;
                        int[] obj = productRegister;
                        lock (obj)
                        {
                            productRegister[__instance.products[0]] += 1;
                        }
                    }

                }
            }
            else if (__instance.recipeType == ERecipeType.Particle && Relic.HaveRelic(2, 8)) // relic2-8
            {
                if (__instance.products.Length > 1 && __instance.products[0] == 1122)
                {
                    if (__instance.replicating)
                    {
                        __instance.extraTime += (int)(power * __instance.speedOverride * 5); // 因为extraSpeed填满需要正常speed填满的十倍
                    }
                    __instance.produced[1] = -5;
                }

            }
            return true;
        }


        /// <summary>
        /// relic0-4
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), "GameTick_Gamma")]
        public static void GammaReceiverPatch(ref PowerGeneratorComponent __instance)
        {
            if (Relic.HaveRelic(0, 4) && __instance.catalystPoint < 3600)
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

        public static void WrathOfGoddess()
        {
            
        }

        /// <summary>
        /// relic0-7
        /// </summary>
        public static void CheckMegaStructureAttack()
        {
            
        }

        /// <summary>
        /// relic0-8 命中时，在这里强制计算护盾回复。
        /// </summary>
        /// <param name="damage"></param>
        public static void ApplyBloodthirster(int damage)
        {
            if (Relic.HaveRelic(0, 8)) // relic0-8 饮血剑效果
            {
                
            }
        }

        /// <summary>
        /// relic1-0
        /// </summary>
        public static void AutoBuildMegaWhenWaveFinished()
        {
            if (Configs.nextWaveState == 0 && Relic.HaveRelic(1, 0) && Configs.nextWaveIntensity > 0)
            {
                Configs.nextWaveIntensity -= 20;
                Relic.AutoBuildMegaStructure(-1, 120);
            }
        }

        /// <summary>
        /// relic1-2
        /// </summary>
        public static void AutoChargeShieldByMegaStructure()
        {
            if (Relic.HaveRelic(1, 2))
            {
                
            }
        }

        /// <summary>
        /// relic1-7 relic3-11
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="gameTick"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DysonSphereLayer), "GameTick")]
        public static void DysonLayerGameTickPostPatchToAccAbsorb(ref DysonSphereLayer __instance, long gameTick)
        {
            DysonSwarm swarm = __instance.dysonSphere.swarm;
            if (Relic.HaveRelic(1, 7))
            {
                int num = (int)(gameTick % 40L);
                for (int i = 1; i < __instance.nodeCursor; i++)
                {
                    DysonNode dysonNode = __instance.nodePool[i];
                    if (dysonNode != null && dysonNode.id == i && dysonNode.id % 40 == num && dysonNode.sp == dysonNode.spMax)
                    {
                        dysonNode.OrderConstructCp(gameTick, swarm);
                    }
                }
            }
            if (Relic.HaveRelic(3, 1))
            {
                int num = (int)(gameTick % 120L);
                for (int i = 1; i < __instance.nodeCursor; i++)
                {
                    DysonNode dysonNode = __instance.nodePool[i];
                    if (dysonNode != null && dysonNode.id == i && dysonNode.id % 120 == num && dysonNode.sp == dysonNode.spMax)
                    {
                        dysonNode.OrderConstructCp(gameTick, swarm);
                    }
                }
            }
        }

        /// <summary>
        /// relic2-5 3-10
        /// </summary>
        public static void CheckPlayerHasaki()
        {
            if (Relic.HaveRelic(2, 5) || Relic.HaveRelic(3, 10))
            {
                Vector3 pos = GameMain.mainPlayer.position;
                if (pos.x != Relic.playerLastPos.x || pos.y != Relic.playerLastPos.y || pos.z != Relic.playerLastPos.z)
                {
                    if (Relic.HaveRelic(2, 5) && Relic.Verify(0.08))
                    {
                        GameMain.mainPlayer.TryAddItemToPackage(9500, 1, 0, true);
                        Utils.UIItemUp(9500, 1, 180);
                    }
                    if (Relic.HaveRelic(3, 10) && Relic.Verify(0.03))
                    {
                        GameMain.mainPlayer.TryAddItemToPackage(9500, 1, 0, true);
                        Utils.UIItemUp(9500, 1, 180);
                    }

                    Relic.playerLastPos = new Vector3(pos.x, pos.y, pos.z);
                }
            }
        }


        /// <summary>
        /// relic3-0 解锁科技时调用重新计算太阳帆寿命并覆盖
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameHistoryData), "UnlockTechFunction")]
        public static void UnlockTechPostPatch()
        {
            CheckSolarSailLife();
        }

        /// <summary>
        /// relic3-0 重新计算太阳帆寿命
        /// </summary>
        public static void CheckSolarSailLife()
        {
            if (!Relic.HaveRelic(3, 0)) return;
            float solarSailLife = 540;
            if (GameMain.history.techStates.ContainsKey(3106) && GameMain.history.techStates[3106].unlocked)
            {
                solarSailLife += 360;
            }
            else if (GameMain.history.techStates.ContainsKey(3105) && GameMain.history.techStates[3105].unlocked)
            {
                solarSailLife += 270;
            }
            else if (GameMain.history.techStates.ContainsKey(3104) && GameMain.history.techStates[3104].unlocked)
            {
                solarSailLife += 180;
            }
            else if (GameMain.history.techStates.ContainsKey(3103) && GameMain.history.techStates[3103].unlocked)
            {
                solarSailLife += 120;
            }
            else if (GameMain.history.techStates.ContainsKey(3102) && GameMain.history.techStates[3102].unlocked)
            {
                solarSailLife += 60;
            }
            else if (GameMain.history.techStates.ContainsKey(3101) && GameMain.history.techStates[3101].unlocked)
            {
                solarSailLife += 30;
            }
            GameMain.history.solarSailLife = solarSailLife;
        }

        /// <summary>
        /// relic3-2
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mecha), "GenerateEnergy")]
        public static void MechaEnergyBonusRestore(ref Mecha __instance)
        {
            if (Relic.HaveRelic(3, 2))
            {
                __instance.coreEnergy += __instance.reactorPowerGen * 0.5 / 60;
                if (__instance.coreEnergy > __instance.coreEnergyCap) __instance.coreEnergy = __instance.coreEnergyCap;
            }
        }

        /// <summary>
        /// relic3-4
        /// </summary>
        public static void ReInitBattleRoundAndDiff()
        {
            for (int i = 0; i < Configs.wavePerStar.Length; i++)
            {
                Configs.wavePerStar[i] = 0;
            }
            Configs.difficulty = 0;
        }

        /// <summary>
        /// relic3-15
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="time"></param>
        /// <param name="dt"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mecha), "GameTick")]
        public static void MechaGameTickPostPatch(Mecha __instance, long time, float dt)
        {
            if (Relic.HaveRelic(3, 15))
            {
                __instance.lab.GameTick(time, dt);
                __instance.lab.GameTick(time, dt);
                __instance.lab.GameTick(time, dt);
                __instance.lab.GameTick(time, dt);
            }
        }

        /// <summary>
        /// relic0-3
        /// </summary>
        public static void CheckAndModifyStarLuminosity()
        {
            if (Relic.HaveRelic(0, 3))
            {
                for (int i = 0; i < GameMain.galaxy.starCount; i++)
                {
                    StarData starData = GameMain.galaxy.stars[i];
                    if (starData != null)
                        starData.luminosity = (float)(Math.Pow((Mathf.Round((float)Math.Pow((double)starData.luminosity, 0.33000001311302185) * 1000f) / 1000f + 1.0), 1.0 / 0.33000001311302185) - starData.luminosity);

                    //还要重新计算并赋值每个戴森球之前已初始化好的属性
                    Relic.alreadyRecalcDysonStarLumin = false;
                }

            }
        }

        /// <summary>
        /// Relic1-7
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static void Carbon2Patch(ref EjectorComponent __instance, float power, DysonSwarm swarm, AstroData[] astroPoses, AnimData[] animPool, int[] consumeRegister, ref uint __result)
        {
            
            return;
        }


        /// <summary>
        /// 每帧调用检查，不能在import的时候调用，会因为所需的DysonSphere是null而无法完成重新计算和赋值
        /// </summary>
        public static void TryRecalcDysonLumin()
        {
            if (!Relic.alreadyRecalcDysonStarLumin && Relic.HaveRelic(0, 3))
            {
                for (int i = 0; i < GameMain.galaxy.starCount; i++)
                {
                    if (i < GameMain.data.dysonSpheres.Length && GameMain.data.dysonSpheres[i] != null)
                    {
                        DysonSphere sphere = GameMain.data.dysonSpheres[i];
                        double num5 = (double)sphere.starData.dysonLumino;
                        sphere.energyGenPerSail = (long)(400.0 * num5);
                        sphere.energyGenPerNode = (long)(1500.0 * num5);
                        sphere.energyGenPerFrame = (long)(1500 * num5);
                        sphere.energyGenPerShell = (long)(300 * num5);
                    }
                }
                Relic.alreadyRecalcDysonStarLumin = true;
            }
        }
    }
}
