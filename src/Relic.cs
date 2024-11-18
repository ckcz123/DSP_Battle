using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

namespace DSP_Battle
{
    public class Relic
    {
        // type 遗物类型 0=legend 1=epic 2=rare 3=common 4=cursed
        // 二进制存储已获取的遗物，需要存档
        public static int[] relics = { 0, 0, 0, 0, 0 };
        // 其他需要存档的数据
        public static int relic0_2Version = 1; // 女神泪重做，老存档此项为0不改效果，新存档此项为1才改效果
        public static int relic0_2Charge = 0; // 新版女神泪充能计数
        public static int relic0_2CanActivate = 1; // 新版女神泪在每次入侵中只能激活一次，激活后设置为0。下次入侵才设置为1
        public static int minShieldPlanetId = -1; // 饮血剑现在会给护盾量最低的星球回盾，但是每秒才更新一次护盾量最低的星球
        public static List<int> recordRelics = new List<int>(); // 被Relic4-6保存的圣物
        public static int autoConstructMegaStructureCountDown = 0;
        public static int autoConstructMegaStructurePPoint = 0;
        public static int trueDamageActive = 0;
        public static int bansheesVeilFactor = 2;
        public static int bansheesVeilIncreaseCountdown = 0; // relic 1-8 女妖面纱 短时间内反复触发的时间倒数，倒数值过大时会增加消耗系数
        public static int aegisOfTheImmortalCooldown = 0; // 不朽之守护冷却时间
        public static int resurrectCoinCount = 0; // 复活币持有个数，要在rank的UI里面显示

        //不存档的设定参数
        public static List<int> orderedRelics = new List<int>(); // 与左侧ui显示一直的排序后的元驱动
        public static int relicHoldMax = 8; // 最多可以持有的遗物数
        public static int[] relicNumByType = { 11, 13, 18, 19, 8 }; // 当前版本各种类型的遗物各有多少种，每种类型均不能大于30
        public static List<int> relicOnlyForInvasion = new List<int> { 407, 112 };
        public static List<int> relicCanClick = new List<int> { 400, 407 };
        public static double[] relicTypeProbability = { 0.03, 0.06, 0.13, 0.76, 0.02 }; // 各类型遗物刷新的权重
        public static double[] relicTypeProbabilityBuffed = { 0.045, 0.09, 0.195, 0.63, 0.04 }; // 五叶草buff后
        public static int[] modifierByEvent = new int[] { 0, 0, 0, 0, 0, 0 };
        public static double[] relicRemoveProbabilityByRelicCount = { 0, 0, 0, 0, 0.05, 0.1, 0.12, 0.15, 1, 1, 1 }; // 拥有i个reilc时，第三个槽位刷新的是删除relic的概率
        public static double firstRelicIsRare = 0.5; // 第一个遗物至少是稀有的概率
        public static bool canSelectNewRelic = false; // 当canSelectNewRelic为true时点按按钮才是有效的选择
        public static int[] alternateRelics = { -1, -1, -1 }; // 三个备选，百位数字代表稀有度类型，0代表传说，个位十位是遗物序号。
        public const int defaultBasicMatrixCost = 10; // 除每次随机赠送的一次免费随机之外，从第二次开始需要消耗的矩阵的基础值（这个第二次以此基础值的2倍开始）
        public static int basicMatrixCost = 10; // 除每次随机赠送的一次免费随机之外，从第二次开始需要消耗的矩阵的基础值（这个第二次以此基础值的2倍开始）
        public static int rollCount = 0; // 本次连续随机了几次的计数
        public static int AbortReward = 500; // 放弃解译圣物直接获取的矩阵数量
        public static List<int> starsWithMegaStructure = new List<int>(); // 每秒更新，具有巨构的星系。
        public static List<int> starsWithMegaStructureUnfinished = new List<int>(); // 每秒更新，具有巨构且未完成建造的星系.
        public static Vector3 playerLastPos = new VectorLF3(0, 0, 0); // 上一秒玩家的位置
        public static int playerIdleTime = 0; // 玩家在行星上持续静止的帧数
        public static int playerIdleTimeMax = 14400; // 持续静止达到这个帧数则停止倒计时，如果有relic1-12
        public static bool alreadyRecalcDysonStarLumin = false; // 不需要存档，如果需要置false则会在读档时以及选择特定遗物时自动完成
        public static int dropletDamageGrowth = 1000; // relic0-10每次水滴击杀的伤害成长
        public static int dropletDamageLimitGrowth = 20000; // relic0-10每次消耗水滴提供的伤害成长上限的成长
        public static int dropletEnergyRestore = 2000000; // relic0-10每次击杀回复的机甲能量
        public static int relic0_2MaxCharge = 1000; // 新版女神泪充能上限
        public static int disturbDamage1612 = 2000; // relic0-8胶囊伤害
        public static int disturbDamage1613 = 3000; // relic0-8胶囊伤害
        public static int energyPerMegaDamage = 10; // tickEnergy开根号后除以此项得到伤害
        public static double ThornmailDamageRatio = 0.2; // relic0-5反伤比例
        public static double ThornmailFieldDamageRatio = 0.2; // relic0-5行星护盾反伤比例
        public static int higherResistFactorDivisor = 100; // relic2-18触发更高级减免的前提：单次伤害超出了护盾量的1/higherResistFactorDivisor
        public const int bansheesVeilBasicFactor = 2; // 女妖面纱消耗能量的基础系数，如果短时间内多次触发则系数增大
        public const int bansheesVeilMaxFactor = 20; // 最大系数
        public const int bansheesVeilMaxCountdown = 3600; // 最大倒数
        public const double kleptomancyProbability = 0.03; // 行窃预兆偷窃概率
        public const int resurrectCoinMaxCount = 10; // 复活币最大持有数量
        public const int hashGainByGroundEnemy = 50; // relic4-4击杀地面黑雾单位提供的科研点数
        public const int hashGainBySpaceEnemy = 150; // relic4-4击杀太空黑雾单位提供的科研点数

        public static int starIndexWithMaxLuminosity = 0; // 具有最大光度的恒星系的index， 读档时刷新
        public static bool isUsingResurrectCoin = false;

        public static GameObject respawnTitleText = null;

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

        public static void InitAllAfterLoad()
        {
            starsWithMegaStructure.Clear();
            starsWithMegaStructureUnfinished.Clear();
            UIRelic.InitAll();
            canSelectNewRelic = false;
            rollCount = 0;
            Configs.relic1_8Protection = int.MaxValue;
            Configs.relic2_17Activated = 0;
            RelicFunctionPatcher.CheckSolarSailLife();
            Configs.eliteDurationFrames = 3600 * 3 + 60 * 20 * Relic.GetCursedRelicCount();
            RefreshConfigs();
            UIEventSystem.InitAll();
            UIEventSystem.InitWhenLoad();
            isUsingResurrectCoin = false;
            //RelicFunctionPatcher.starCannonDamageSlice = MoreMegaStructure.StarCannon.damageSlice;
        }

        public static void RefreshConfigs()
        {
            RelicFunctionPatcher.RefreshRerollCost();
            RelicFunctionPatcher.RefreshCargoAccIncTable();
            RelicFunctionPatcher.RefreshDisturbPrefabDesc();
            RelicFunctionPatcher.RefreshBlueBuffStarAssemblyEffect();
            RelicFunctionPatcher.RefreshTearOfTheGoddessEffect();
            RelicFunctionPatcher.RefreshEnergyBurstEffect();
            RelicFunctionPatcher.RefreshStarCannonBuffs();
        }

        public static int AddRelic(int type, int num)
        {
            if (num > 30) return -1; // 序号不存在
            if (type > 4 || type < 0) return -2; // 稀有度不存在
            if ((relics[type] & 1 << num) > 0) return 0; // 已有

            // 下面是一些特殊的Relic在选择时不是简单地改一个拥有状态就行，需要单独对待if (type == 0 && num == 2)
            if (type == 0 && num == 2)
            {
                relics[type] |= 1 << num;
                relic0_2Version = 1;
                relic0_2Charge = 0;
                relic0_2CanActivate = 1;
            }
            else if ((type == 0 && num == 3) || (type == 4 && num == 0))
            {
                relics[type] |= 1 << num;
                RelicFunctionPatcher.CheckAndModifyStarLuminosity(type * 100 + num);
            }
            else if ((type == 1 && num == 4))
            {
                relics[type] |= 1 << num;
                //GameMain.data.history.UnlockTechUnlimited(1918, false);
                GameMain.data.history.UnlockTechUnlimited(1999, false);
                GameMain.data.mainPlayer.TryAddItemToPackage(9511, 3, 0, true);
                Utils.UIItemUp(9511, 3);
            }
            else if (type == 1 && num == 10)
            {
                trueDamageActive = 1;
            }
            else if (type == 2 && num == 7)
            {
                GameMain.history.kineticDamageScale += 0.1f;
                GameMain.history.blastDamageScale += 0.1f;
                GameMain.history.energyDamageScale += 0.1f;
            }
            else if (type == 2 && num == 15)
            {
                GameMain.history.blastDamageScale += 0.4f;
            }
            else if (type == 3 && num == 5)
            {
                if (resurrectCoinCount < resurrectCoinMaxCount)
                    resurrectCoinCount++;
            }
            else if (type == 3 && num == 6) // 3-6
            {
                if (Rank.rank < 10 && Rank.rank >= 0)
                {
                    int exp = (int)(Configs.expToNextRank[Rank.rank] * 0.05);
                    if (exp < 50)
                        exp = 50;
                    if (exp > 300000)
                        exp = 300000;
                    Rank.AddExp(exp);
                }
            }
            else if (type == 3 && num == 7)
            {
                GameMain.history.energyDamageScale += 0.1f;
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
            else if (type == 4 && num == 6)
            {
                if (Relic.HaveRelic(4, 6)) return 0;
                recordRelics.Clear();
                int count = 0;
                const int maxCount = 3; // 最多记录三个
                for (int haveType = 4; haveType < 5 && count < maxCount; haveType = (haveType + 1) % 5)
                {
                    for (int haveNum = 0; haveNum < relicNumByType[haveType] && count < maxCount; haveNum++)
                    {
                        int fullNum = haveType * 100 + haveNum;
                        if (Relic.HaveRelic(haveType, haveNum) && !relicCanClick.Contains(fullNum))
                        {
                            recordRelics.Add(haveType * 100 + haveNum);
                            count++;
                        }
                    }
                    if (haveType == 3)
                        break;
                }
                relics[type] |= 1 << num;
            }
            else
            {
                if (GetRelicCount() >= relicHoldMax) return -3; // 超上限
                relics[type] |= 1 << num;
            }
            RefreshConfigs();
            int[] data = new int[] { type, num };
            MP.Sync(EDataType.CallOnAddRelic, data);
            return 1;
        }

        public static void AskRemoveRelic(int removeType, int removeNum)
        {
            if (removeType > 4 || removeNum > 30)
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
            UIMessageBox.Show("删除遗物确认标题".Translate(), String.Format("删除遗物确认警告".Translate(), ("遗物名称" + removeType.ToString() + "-" + removeNum.ToString()).Translate().Split('\n')[0]),
            "否".Translate(), "是".Translate(), 1, new UIMessageBox.Response(RegretRemoveRelic), new UIMessageBox.Response(() =>
            {
                RemoveRelic(removeType, removeNum);

                //UIMessageBox.Show("成功移除！".Translate(), "已移除遗物描述".Translate() + ("遗物名称" + removeType.ToString() + "-" + removeNum.ToString()).Translate().Split('\n')[0], "确定".Translate(), 1);

                UIRelic.CloseSelectionWindow();
                UIRelic.HideSlots();
            }));
        }

        public static void RemoveRelic(int removeType, int removeNum)
        {
            if (removeType == 1 && removeNum == 10)
            {
                trueDamageActive = 0;
            }
            if (Relic.HaveRelic(removeType, removeNum))
            {
                relics[removeType] = relics[removeType] ^ 1 << removeNum;
                UIRelic.RefreshSlotsWindowUI();
                RefreshConfigs();
            }

            int[] data = new int[] { removeType, removeNum };
            MP.Sync(EDataType.CallOnRemoveRelic, data);
        }

        public static void RegretRemoveRelic()
        {
            canSelectNewRelic = true;
        }

        public static bool HaveRelic(int type, int num)
        {
            //if (Configs.developerMode &&( type == 0 && num == 5 ||  type == 1 && num == 9 )) return true;
            if (type > 4 || type < 0 || num > 30) return false;
            if ((relics[type] & (1 << num)) > 0) return true;
            return false;
        }

        public static bool isRecorded(int type, int num)
        {
            return recordRelics.Contains(type * 100 + num);
        }

        // 输出遗物数量，type输入-1为获取全部类型的遗物数量总和
        public static int GetRelicCount(int type = -1)
        {
            if (type < 0 || type > 4)
            {
                return GetRelicCount(0) + GetRelicCount(1) + GetRelicCount(2) + GetRelicCount(3) + GetRelicCount(4);
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
                int recorded = 0;
                if (recordRelics.Count > 0)
                {
                    foreach (var item in recordRelics)
                    {
                        if (item / 100 == type)
                            recorded++;
                    }
                }
                return count - recorded;
            }
        }

        // 返回受诅咒的遗物的数量
        public static int GetCursedRelicCount()
        {
            return GetRelicCount(4);
        }

        // 允许玩家选择一个新的遗物
        public static bool PrepareNewRelic(int bonusRollCount = 0)
        {
            //if (GetRelicCount() >= relicHoldMax) return false;
            rollCount = -1 - bonusRollCount; // 从-1开始是因为每次准备给玩家新的relic都要重新随机一次
            canSelectNewRelic = true;

            EventSystem.tickFromLastRelic = 0;
            EventSystem.probabilityForNewEvent = 0;
            UIRelic.OpenSelectionWindow();
            UIRelic.ShowSlots(); // 打开已有遗物栏

            // 下面是实现：开启虚空入侵的首次入侵开始，之后的首次刷新元驱动，必定会刷出虚空回响。
            if(AssaultController.nextRollVoidEcho > 0 && AssaultController.voidInvasionEnabled && (AssaultController.assaultHives.Count > 0 || Configs.totalWave > 0) && UIRelic.forceType < 0 && UIRelic.forceNum < 0)
            {
                UIRelic.forceType = 4;
                UIRelic.forceNum = 7;
                AssaultController.nextRollVoidEcho = 0;
            }

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

        public static bool Verify(double probability)
        {
            if ((relics[4] & 1 << 1) > 0) // relic4-1负面效果：概率减半
                probability = 0.5 * probability;
            if (Utils.RandDouble() < probability)
                return true;
            else if ((relics[0] & 1 << 9) > 0) // 具有增加幸运的遗物，则可以再判断一次
                return (Utils.RandDouble() < probability);

            return false;
        }

        // 任何额外伤害都需要经过此函数来计算并处理，dealDamage默认为false，代表只用这个函数计算而尚未实际造成伤害
        public static int BonusDamage(double damage, double bonus)
        {
            if (HaveRelic(2, 13))
            {
                bonus = 2 * bonus * damage;
            }
            else
            {
                bonus = bonus * damage;
            }
            return (int)bonus;
        }

        public static int BonusedDamage(double damage, double bonus)
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
        public static void AutoBuildMegaStructure(int starIndex = -1, int amount = 12, int frameCost = 5)
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
                                    for (int k = 0; k < Math.Min(6, amount / frameCost); k++)
                                    {
                                        if (dysonNode.spReqOrder > 0)
                                        {
                                            sphere.OrderConstructSp(dysonNode);
                                            sphere.ConstructSp(dysonNode);
                                            amount -= frameCost; // 框架结构点数由于本身是需要火箭才能建造的，自然比细胞点数昂贵一些。这里默认设置为昂贵五倍。
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
            w.Write(relics[4]);
            w.Write(relic0_2Version);
            w.Write(relic0_2Charge);
            w.Write(relic0_2CanActivate);
            w.Write(minShieldPlanetId);
            w.Write(recordRelics.Count);
            foreach (var item in recordRelics)
            {
                w.Write(item);
            }
            w.Write(autoConstructMegaStructureCountDown);
            w.Write(autoConstructMegaStructurePPoint);
            w.Write(trueDamageActive);
            w.Write(bansheesVeilFactor);
            w.Write(bansheesVeilIncreaseCountdown);
            w.Write(aegisOfTheImmortalCooldown);
            w.Write(resurrectCoinCount);
        }

        public static void Import(BinaryReader r)
        {
            if (Configs.versionWhenImporting >= 30221025)
            {
                relics[0] = r.ReadInt32();
                relics[1] = r.ReadInt32();
                relics[2] = r.ReadInt32();
                relics[3] = r.ReadInt32();
                if (Configs.versionWhenImporting >= 30230519)
                    relics[4] = r.ReadInt32();
                else
                    relics[4] = 0;
                RelicFunctionPatcher.CheckAndModifyStarLuminosity();
            }
            else
            {
                relics[0] = 0;
                relics[1] = 0;
                relics[2] = 0;
                relics[3] = 0;
                relics[4] = 0;
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
            recordRelics.Clear();
            if (Configs.versionWhenImporting >= 30230523)
            {
                int count = r.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    recordRelics.Add(r.ReadInt32());
                }
            }
            autoConstructMegaStructureCountDown = r.ReadInt32();
            autoConstructMegaStructurePPoint = r.ReadInt32();
            trueDamageActive = r.ReadInt32();
            bansheesVeilFactor = r.ReadInt32();
            bansheesVeilIncreaseCountdown = r.ReadInt32();
            aegisOfTheImmortalCooldown = r.ReadInt32();
            resurrectCoinCount = r.ReadInt32();
            InitAllAfterLoad();
        }

        public static void IntoOtherSave()
        {
            relics[0] = 0;
            relics[1] = 0;
            relics[2] = 0;
            relics[3] = 0;
            relics[4] = 0;
            recordRelics.Clear();
            autoConstructMegaStructureCountDown = 0;
            autoConstructMegaStructurePPoint = 0;
            trueDamageActive = 0;
            bansheesVeilFactor = bansheesVeilBasicFactor;
            bansheesVeilIncreaseCountdown = 0;
            aegisOfTheImmortalCooldown = 0;
            resurrectCoinCount = 0;
            InitAllAfterLoad();
        }
    }


    public class RelicFunctionPatcher
    {
        private static Sprite r3_5_coin = Resources.Load<Sprite>("Assets/DSPBattle/r3-5-coin");
        public static int starCannonDamageSlice = 7; // 读档时从MoreMegaStructure中获取

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RelicFunctionGameTick(long time)
        {
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
            if (time % 60 == 8)
                CheckMegaStructureAttack();
            //else if (time % 60 == 9)
            //    AutoChargeShieldByMegaStructure();
            else if (time % 60 == 10)
                CheckPlayerHasaki();

            TryRecalcDysonLumin();
            AutoBuildMega();
            AutoBuildMegaOfMaxLuminStar(time);
            CheckBansheesVeilCountdown(time);
            AegisOfTheImmortalCountDown(time);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
        }


        /// <summary>
        /// relic 0-1 1-6 2-4 2-11 2-8 3-0 3-14
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
            if (__instance.recipeType == ERecipeType.Assemble || (int)__instance.recipeType == 9 || (int)__instance.recipeType == 10 || (int)__instance.recipeType == 12)
            {
                // relic1-6
                if (Relic.HaveRelic(1, 6))
                {
                    if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 10 * __instance.productCounts[0])
                    {
                        int rocketId = __instance.products[0];
                        int rodNum = -1;
                        if (rocketId >= 9488 && rocketId <= 9490 || rocketId == 1503 && MoreMegaStructure.MoreMegaStructure.GenesisCompatibility)
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
                //else if ((__instance.products[0] == 1303 || __instance.products[0] == 1305) && Relic.HaveRelic(3, 6)) // relic3-6 光刻机的增产效果已经被废弃，这个元驱动效果已经变更
                //{
                //    if (__instance.replicating)
                //    {
                //        __instance.extraTime += (int)(0.5 * __instance.extraSpeed);
                //    }
                //}
                else if ((__instance.products[0] == 1203 || __instance.products[0] == 1204) && Relic.HaveRelic(3, 14)) // relic3-14
                {
                    int reloadNum = __instance.products[0] == 1203 ? 2 : 1;
                    if (MoreMegaStructure.MoreMegaStructure.GenesisCompatibility)
                        reloadNum = __instance.produced[0] == 1203 ? 1 : -1;
                    if (reloadNum >= 0 && __instance.served[reloadNum] < 10 * __instance.requireCounts[reloadNum])
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
                // relic0-2 老女神之泪效果
                //if (Relic.HaveRelic(0, 2) && __instance.requires.Length > 1)
                //{
                //    if (__instance.served[0] < 20 * __instance.requireCounts[0])
                //    {
                //        if (__instance.time >= __instance.timeSpend - 1 && __instance.produced[0] < 20 * __instance.productCounts[0])
                //        {
                //            //if (__instance.served[0] > 0)
                //            //    __instance.incServed[0] += __instance.incServed[0] / __instance.served[0] * __instance.requireCounts[0];
                //            __instance.incServed[0] += 4 * __instance.requireCounts[0];
                //            __instance.served[0] += __instance.requireCounts[0];
                //            int[] obj = consumeRegister;
                //            lock (obj)
                //            {
                //                consumeRegister[__instance.requires[0]] -= __instance.requireCounts[0];
                //            }
                //        }
                //    }
                //}
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

        // relic 0-1
        public static void RefreshBlueBuffStarAssemblyEffect()
        {
            if (Relic.HaveRelic(0, 1))
                MoreMegaStructure.StarAssembly.blueBuffByTCFV = 1;
            else
                MoreMegaStructure.StarAssembly.blueBuffByTCFV = 0;
        }
        // relic 0-2
        public static void RefreshTearOfTheGoddessEffect()
        {
            if (Relic.HaveRelic(0, 2))
                MoreMegaStructure.StarAssembly.r002ByTCFV = 1;
            else
                MoreMegaStructure.StarAssembly.r002ByTCFV = 0;
        }
        // relic 1-6
        public static void RefreshEnergyBurstEffect()
        {
            if (Relic.HaveRelic(1, 6))
                MoreMegaStructure.StarAssembly.r106ByTCFV = 1;
            else
                MoreMegaStructure.StarAssembly.r106ByTCFV = 0;
        }


        /// <summary>
        /// relic0-2
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="entityId"></param>
        /// <param name="offset"></param>
        /// <param name="filter"></param>
        /// <param name="needs"></param>
        /// <param name="stack"></param>
        /// <param name="inc"></param>
        /// <param name="__result"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlanetFactory), "PickFrom")]
        public static void AutoProliferate(ref PlanetFactory __instance, int entityId, int offset, int filter, int[] needs, ref byte stack, ref byte inc, ref int __result)
        {
            if (!Relic.HaveRelic(0, 2)) return;
            int itemId = __result;
            if (itemId == 0) return;

            var _this = __instance;
            int beltId = _this.entityPool[entityId].beltId;
            if (beltId <= 0)
            {
                int assemblerId = _this.entityPool[entityId].assemblerId;
                if (assemblerId > 0)
                {
                    Mutex obj = _this.entityMutexs[entityId];
                    lock (obj)
                    {
                        int[] products = _this.factorySystem.assemblerPool[assemblerId].products;
                        int num = products.Length;
                        for (int i = 0; i < num; i++)
                        {
                            if (products[i] == itemId)
                            {
                                inc = (byte)(4 * stack);
                                return;
                            }
                        }
                        return;
                    }
                }
                int labId = _this.entityPool[entityId].labId;
                if (labId > 0)
                {
                    Mutex obj = _this.entityMutexs[entityId];
                    lock (obj)
                    {
                        int[] products = _this.factorySystem.labPool[labId].products;
                        int num = products.Length;
                        for (int i = 0; i < num; i++)
                        {
                            if (products[i] == itemId)
                            {
                                inc = (byte)(4 * stack);
                                return;
                            }
                        }
                    }
                }
            }
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

        /// <summary>
        /// relic 0-5 2-16 3-12 虚空荆棘反弹伤害，各种护盾伤害减免和规避
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="caster"></param>
        /// <param name="damage"></param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SkillSystem), "MechaEnergyShieldResist", new Type[] { typeof(SkillTarget), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
        public static bool ThornmailAttackPreMarker(ref int damage, ref int __state)
        {
            float factor = 1.0f;
            if (Relic.Verify(SkillPoints.IcarusShieldEvade))
            {
                factor = 0f;
            }
            else
            {
                if (Relic.HaveRelic(0, 5))
                    factor *= 0.9f;
                if (Relic.HaveRelic(2, 16))
                {
                    Mecha mecha = GameMain.data.mainPlayer.mecha;
                    if (mecha.energyShieldEnergy / mecha.energyShieldEnergyRate / Relic.higherResistFactorDivisor < damage)
                        factor *= 0.2f;
                    else
                        factor *= 0.8f;
                }
                if (Rank.rank >= 2)
                    factor *= 0.75f;
            }
            __state = (int)(damage * (1 - factor));
            damage = (int)(damage * factor);
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SkillSystem), "MechaEnergyShieldResist", new Type[] { typeof(SkillTargetLocal), typeof(int), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref })]
        public static bool ThornmailLocalAttackPreMarker(ref int damage, ref int __state)
        {
            float factor = 1.0f; if (Relic.Verify(SkillPoints.IcarusShieldEvade))
            {
                factor = 0f;
            }
            else
            {
                if (Relic.HaveRelic(0, 5))
                    factor *= 0.9f;
                if (Relic.HaveRelic(2, 16))
                {
                    Mecha mecha = GameMain.data.mainPlayer.mecha;
                    if (mecha.energyShieldEnergy / mecha.energyShieldEnergyRate / Relic.higherResistFactorDivisor < damage)
                        factor *= 0.2f;
                    else
                        factor *= 0.8f;
                }
                if (Rank.rank >= 2)
                    factor *= 0.75f;
            }
            __state = (int)(damage * (1 - factor));
            damage = (int)(damage * factor);
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SkillSystem), "MechaEnergyShieldResist", new Type[] { typeof(SkillTarget), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
        public static void ThornmailAttackPostHandler(SkillTarget caster, int damage, ref int __state)
        {
            if (Relic.HaveRelic(0, 5))
            {
                SkillTarget casterPlayer;
                casterPlayer.id = 1;
                casterPlayer.type = ETargetType.Player;
                casterPlayer.astroId = 0;
                int realDamage = (int)(Relic.BonusDamage(__state, 1) * SkillPoints.voidDamageRate);
                GameMain.data.spaceSector.skillSystem.DamageObject(realDamage, 1, ref caster, ref casterPlayer);
            }
            BattleBGMController.PlayerTakeDamage();
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SkillSystem), "MechaEnergyShieldResist", new Type[] { typeof(SkillTargetLocal), typeof(int), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref })]
        public static void ThornmailAttackPostHandler(SkillTargetLocal caster, int astroId, int damage, ref int __state)
        {
            if (Relic.HaveRelic(0, 5))
            {
                SkillTarget target;
                target.id = caster.id;
                target.astroId = astroId;
                target.type = caster.type;
                SkillTarget casterPlayer;
                casterPlayer.id = 1;
                casterPlayer.type = ETargetType.Player;
                casterPlayer.astroId = 0;
                int realDamage = (int)(Relic.BonusDamage(__state, 1) * SkillPoints.voidDamageRate);
                GameMain.data.spaceSector.skillSystem.DamageObject(realDamage, 1, ref target, ref casterPlayer);
            }
            BattleBGMController.PlayerTakeDamage();
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(GeneralExpImpProjectile), "TickSkillLogic")]
        public static bool ThornmailFieldCounterattackGeneralExpImpProjectile(ref GeneralExpImpProjectile __instance, SkillSystem skillSystem, PlanetFactory[] factories, ref float __state)
        {
            float factor = 1.0f;
            if (__instance.atfAstroId > 0 && __instance.atfRayId > 0)
            {
                if (AssaultController.CheckCasterOrTargetHasModifier(ref __instance.caster)) // 虚空入侵修改器增伤
                {
                    int value = AssaultController.modifier[3];
                    factor += value * 1.0f / 100f;

                    int value2 = AssaultController.modifier[11];
                    float maxDB = Math.Max(GameMain.data.history.kineticDamageScale, GameMain.data.history.blastDamageScale);
                    maxDB = Math.Max(maxDB, GameMain.data.history.energyDamageScale);
                    maxDB = Math.Max(maxDB, GameMain.data.history.magneticDamageScale);
                    factor += (maxDB - 1.0f) * value2 / 100f;
                }
                bool r0005 = Relic.HaveRelic(0, 5);
                bool r0216 = Relic.HaveRelic(2, 16);
                bool r0312 = Relic.HaveRelic(3, 12);
                bool r0318 = Relic.HaveRelic(3, 18);
                if (r0312 && Relic.Verify(0.15)) // 灵动巨物护盾完全规避伤害
                {
                    factor = 0f;
                }
                if (r0216) // 刚毅护盾减伤
                {
                    PlanetATField shield = skillSystem.astroFactories[__instance.atfAstroId].planetATField;
                    if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < __instance.damage)
                        factor *= 0.2f;
                    else
                        factor *= 0.8f;
                }
                if(r0318) // 次级刚毅护盾减伤
                {
                    if (factor > 0.1f)
                        factor -= 0.1f;
                    else
                        factor = 0f;
                }
                if (r0005) // 虚空荆棘
                {
                    int realDamage = (int)(Relic.BonusDamage(__instance.damage * (1.0 - factor), 1) * SkillPoints.voidDamageRate);
                    skillSystem.DamageObject(realDamage, 1, ref __instance.caster, ref __instance.target);
                }
                if (factor != 1.0f)
                {
                    __instance.damage = (int)(__instance.damage * factor);
                }
            }

            //__state保留原始伤害变化系数，传值到postfix，并还原damage，因为有的skill经过多次TickSkillLogic，不能每次都乘减伤系数越来越小，所以必须每次都还原伤害（亦或者只有创建时/第一次乘系数，但麻烦）
            __state = factor;
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GeneralExpImpProjectile), "TickSkillLogic")]
        public static void RestoreDamageGeneralExpImpProjectile(ref GeneralExpImpProjectile __instance, ref float __state)
        {
            if (__state != 0)
                __instance.damage = (int)(__instance.damage / __state);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GeneralProjectile), "TickSkillLogic")]
        public static bool ThornmailFieldCounterattackGeneralProjectile(ref GeneralProjectile __instance, SkillSystem skillSystem, ref float __state)
        {
            float factor = 1.0f;
            if (__instance.atfAstroId > 0 && __instance.atfRayId > 0)
            {
                if (AssaultController.CheckCasterOrTargetHasModifier(ref __instance.caster)) // 虚空入侵修改器增伤
                {
                    int value = AssaultController.modifier[3];
                    factor += value * 1.0f / 100f;

                    int value2 = AssaultController.modifier[11];
                    float maxDB = Math.Max(GameMain.data.history.kineticDamageScale, GameMain.data.history.blastDamageScale);
                    maxDB = Math.Max(maxDB, GameMain.data.history.energyDamageScale);
                    maxDB = Math.Max(maxDB, GameMain.data.history.magneticDamageScale);
                    factor += (maxDB - 1.0f) * value2 / 100f;
                }
                bool r0005 = Relic.HaveRelic(0, 5);
                bool r0216 = Relic.HaveRelic(2, 16);
                bool r0312 = Relic.HaveRelic(3, 12);
                bool r0318 = Relic.HaveRelic(3, 18);
                if (r0312 && Relic.Verify(0.15)) // 灵动巨物护盾完全规避伤害
                {
                    factor = 0f;
                }
                if (r0216) // 刚毅护盾减伤
                {
                    PlanetATField shield = skillSystem.astroFactories[__instance.atfAstroId].planetATField;
                    if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < __instance.damage)
                        factor *= 0.2f;
                    else
                        factor *= 0.8f;
                }
                if (r0318) // 次级刚毅护盾减伤
                {
                    if (factor > 0.1f)
                        factor -= 0.1f;
                    else
                        factor = 0f;
                }
                if (r0005) // 虚空荆棘
                {
                    int realDamage = (int)(Relic.BonusDamage(__instance.damage * (1.0 - factor), 1) * SkillPoints.voidDamageRate);
                    skillSystem.DamageObject(realDamage, 1, ref __instance.caster, ref __instance.target);
                }
                if (factor != 1.0f)
                {
                    __instance.damage = (int)(__instance.damage * factor);
                }
            }
            __state = factor;
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GeneralProjectile), "TickSkillLogic")]
        public static void RestoreDamageGeneralProjectile(ref GeneralProjectile __instance, ref float __state)
        {
            if (__state != 0)
                __instance.damage = (int)(__instance.damage / __state);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpaceLaserOneShot), "TickSkillLogic")]
        public static bool ThornmailFieldCounterattackSpaceLaserOneShot(ref SpaceLaserOneShot __instance, SkillSystem skillSystem, ref float __state)
        {
            float factor = 1.0f;
            if (__instance.atfAstroId > 0 && __instance.atfRayId > 0)
            {
                if (AssaultController.CheckCasterOrTargetHasModifier(ref __instance.caster)) // 虚空入侵修改器增伤
                {
                    int value = AssaultController.modifier[3];
                    factor += value * 1.0f / 100f;

                    int value2 = AssaultController.modifier[11];
                    float maxDB = Math.Max(GameMain.data.history.kineticDamageScale, GameMain.data.history.blastDamageScale);
                    maxDB = Math.Max(maxDB, GameMain.data.history.energyDamageScale);
                    maxDB = Math.Max(maxDB, GameMain.data.history.magneticDamageScale);
                    factor += (maxDB - 1.0f) * value2 / 100f;
                }
                bool r0005 = Relic.HaveRelic(0, 5);
                bool r0216 = Relic.HaveRelic(2, 16);
                bool r0312 = Relic.HaveRelic(3, 12);
                bool r0318 = Relic.HaveRelic(3, 18);
                if (r0312 && Relic.Verify(0.15)) // 灵动巨物护盾完全规避伤害
                {
                    factor = 0f;
                }
                if (r0216) // 刚毅护盾减伤
                {
                    PlanetATField shield = skillSystem.astroFactories[__instance.atfAstroId].planetATField;
                    if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < __instance.damage)
                        factor *= 0.2f;
                    else
                        factor *= 0.8f;
                }
                if (r0318) // 次级刚毅护盾减伤
                {
                    if (factor > 0.1f)
                        factor -= 0.1f;
                    else
                        factor = 0f;
                }
                if (r0005) // 虚空荆棘
                {
                    int realDamage = (int)(Relic.BonusDamage(__instance.damage * (1.0 - factor), 1) * SkillPoints.voidDamageRate);
                    skillSystem.DamageObject(realDamage, 1, ref __instance.caster, ref __instance.target);
                }
                if (factor != 1.0f)
                {
                    __instance.damage = (int)(__instance.damage * factor);
                }
            }
            __state = factor;
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpaceLaserOneShot), "TickSkillLogic")]
        public static void RestoreDamageSpaceLaserOneShot(ref SpaceLaserOneShot __instance, ref float __state)
        {
            if (__state != 0)
                __instance.damage = (int)(__instance.damage / __state);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpaceLaserSweep), "TickSkillLogic")]
        public static bool ThornmailFieldCounterattackSpaceLaserSweep(ref SpaceLaserSweep __instance, SkillSystem skillSystem, ref float __state)
        {
            float factor = 1.0f;
            int timeTick = __instance.lifemax - (__instance.life - 1);
            if (__instance.life == 1)
                timeTick = __instance.lifemax - (-9);
            if (__instance.atfAstroId > 0 && __instance.atfRayId > 0 && timeTick % __instance.damageInterval == 0) // 对于sweep，原本逻辑只有每10tick造成一次伤害，所以反伤和减免也是同时计算，但是由于prepatch的时候lifeMax还没减小
            {
                if (AssaultController.CheckCasterOrTargetHasModifier(ref __instance.caster)) // 虚空入侵修改器增伤
                {
                    int value = AssaultController.modifier[3];
                    factor += value * 1.0f / 100f;

                    int value2 = AssaultController.modifier[11];
                    float maxDB = Math.Max(GameMain.data.history.kineticDamageScale, GameMain.data.history.blastDamageScale);
                    maxDB = Math.Max(maxDB, GameMain.data.history.energyDamageScale);
                    maxDB = Math.Max(maxDB, GameMain.data.history.magneticDamageScale);
                    factor += (maxDB - 1.0f) * value2 / 100f;
                }
                bool r0005 = Relic.HaveRelic(0, 5);
                bool r0216 = Relic.HaveRelic(2, 16);
                bool r0312 = Relic.HaveRelic(3, 12);
                bool r0318 = Relic.HaveRelic(3, 18);
                if (r0312 && Relic.Verify(0.15)) // 灵动巨物护盾完全规避伤害
                {
                    factor = 0f;
                }
                if (r0216) // 刚毅护盾减伤
                {
                    PlanetATField shield = skillSystem.astroFactories[__instance.atfAstroId].planetATField;
                    if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < __instance.damage)
                        factor *= 0.2f;
                    else
                        factor *= 0.8f;
                }
                if (r0318) // 次级刚毅护盾减伤
                {
                    if (factor > 0.1f)
                        factor -= 0.1f;
                    else
                        factor = 0f;
                }
                if (r0005) // 虚空荆棘
                {
                    int realDamage = (int)(Relic.BonusDamage(__instance.damage * (1.0 - factor), 1) * SkillPoints.voidDamageRate);
                    SkillTarget emptyCaster;
                    emptyCaster.id = 0;
                    emptyCaster.type = ETargetType.None;
                    emptyCaster.astroId = __instance.atfAstroId;
                    skillSystem.DamageObject(realDamage, 1, ref __instance.caster, ref emptyCaster);
                }
                if (factor != 1.0f)
                {
                    __instance.damage = (int)(__instance.damage * factor);
                }
            }
            __state = factor;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpaceLaserSweep), "TickSkillLogic")]
        public static void RestoreDamageSpaceLaserSweep(ref SpaceLaserSweep __instance, ref float __state)
        {
            if (__state != 0)
                __instance.damage = (int)(__instance.damage / __state);
        }


        /// <summary>
        /// relic 0-6
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="consumeRegister"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TurretComponent), "LoadAmmo")]
        public static bool LoadAmmoPatch(ref TurretComponent __instance, ref int[] consumeRegister)
        {
            if (Relic.HaveRelic(0, 6))
            {
                ref var _this = ref __instance;
                if (_this.itemCount == 0 || _this.bulletCount > 0)
                {
                    return false;
                }
                int num = (int)((float)_this.itemInc / (float)_this.itemCount + 0.5f);
                num = ((num > 10) ? 10 : num);
                short num2 = (short)((double)_this.itemBulletCount * Cargo.incTableMilli[num] + ((_this.itemBulletCount < 12) ? 0.51 : 0.1));
                _this.bulletCount = (short)(_this.itemBulletCount + num2);
                //_this.itemCount -= 1;
                //_this.itemInc -= (short)num;
                _this.currentBulletInc = (byte)num;
                consumeRegister[(int)_this.itemId]++;
                return false;
            }
            else if (Relic.HaveRelic(1, 5) && Relic.Verify(0.35))
            {
                ref var _this = ref __instance;
                if (_this.itemCount == 0 || _this.bulletCount > 0)
                {
                    return false;
                }
                int num = (int)((float)_this.itemInc / (float)_this.itemCount + 0.5f);
                num = ((num > 10) ? 10 : num);
                short num2 = (short)((double)_this.itemBulletCount * Cargo.incTableMilli[num] + ((_this.itemBulletCount < 12) ? 0.51 : 0.1));
                _this.bulletCount = (short)(_this.itemBulletCount + num2);
                if (_this.itemCount < 100)
                {
                    _this.itemCount += 1; // 不消耗反而回填
                    _this.itemInc += (short)num;
                }
                _this.currentBulletInc = (byte)num;
                consumeRegister[(int)_this.itemId]++;
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// relic0-7
        /// </summary>
        public static void CheckMegaStructureAttack()
        {
            if (!Relic.HaveRelic(0, 7))
                return;

            SpaceSector sector = GameMain.data.spaceSector;
            if (sector == null) return;
            EnemyData[] pool = GameMain.data.spaceSector.enemyPool;
            for (int i = 0; i < sector.enemyCursor; i++)
            {
                ref EnemyData e = ref pool[i];
                if (e.unitId <= 0 || e.id <= 0)
                    continue;

                EnemyDFHiveSystem[] hivesByAstro = sector.dfHivesByAstro;
                EnemyDFHiveSystem hive = hivesByAstro[e.originAstroId - 1000000];
                int starIndex = hive?.starData?.index ?? -1;
                if (starIndex >= 0 && GameMain.data.dysonSpheres != null)
                {
                    DysonSphere sphere = GameMain.data.dysonSpheres[starIndex];
                    if (sphere != null && sphere.energyGenCurrentTick > 0)
                    {
                        long tickEnergy = sphere.energyGenCurrentTick;
                        int damage = (int)(Math.Pow(tickEnergy, 0.5) / Relic.energyPerMegaDamage);
                        if (starIndex < 1000 && MoreMegaStructure.MoreMegaStructure.StarMegaStructureType[starIndex] == 6)
                            damage *= 2;
                        damage = Relic.BonusDamage(damage, 1);
                        SkillTarget target;
                        SkillTarget caster;
                        target.id = e.id;
                        target.astroId = e.originAstroId;
                        target.type = ETargetType.Enemy;
                        caster.id = 1;
                        caster.type = ETargetType.Player;
                        caster.astroId = 0;
                        sector.skillSystem.DamageObject(damage, 1, ref target, ref caster);
                    }
                }
            }
        }


        /// <summary>
        /// relic 0-8
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="skillSystem"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalDisturbingWave), "TickSkillLogic")]
        public static bool DisturbingDamagePatch(ref LocalDisturbingWave __instance, ref SkillSystem skillSystem)
        {
            ref var _this = ref __instance;
            if (_this.life <= 0)
            {
                return false;
            }
            _this.currentDiffuseRadius += _this.diffusionSpeed * 0.016666668f;
            if (_this.caster.id == 0)
            {
                return false;
            }
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.Ballistic);
            float num = _this.thickness * 0.5f;
            float num2 = _this.currentDiffuseRadius - num;
            float num3 = _this.currentDiffuseRadius + num;
            if (num2 < 0f)
            {
                num2 = 0f;
            }
            if (num3 > _this.diffusionMaxRadius)
            {
                num3 = _this.diffusionMaxRadius;
                _this.life = 0;
            }
            float num4 = num2 * num2;
            float num5 = num3 * num3;
            float num6 = 0.016666668f;
            PlanetFactory planetFactory = skillSystem.astroFactories[_this.astroId];
            EnemyData[] enemyPool = planetFactory.enemyPool;
            EnemyUnitComponent[] buffer = planetFactory.enemySystem.units.buffer;
            int[] consumeRegister = null;
            int num7 = 0;
            TurretComponent[] array = null;
            int num8 = 0;
            int num9 = 0;
            bool flag = (_this.caster.type == ETargetType.None || _this.caster.type == ETargetType.Ruin) && planetFactory.entityPool[_this.caster.id].turretId > 0; // 判断条件额外增加了ruin是自己设定的
            bool flagEnemy = _this.caster.type == ETargetType.Enemy; // 来自于敌人死亡触发的
            if (flag)
            {
                num7 = planetFactory.entityPool[_this.caster.id].turretId;
                array = planetFactory.defenseSystem.turrets.buffer;
                VSLayerMask vslayerMask = array[num7].vsCaps & array[num7].vsSettings;
                num8 = (int)(vslayerMask & VSLayerMask.GroundHigh);
                num9 = (int)((int)(vslayerMask & VSLayerMask.AirHigh) >> 2);
                consumeRegister = GameMain.statistics.production.factoryStatPool[planetFactory.index].consumeRegister;
            }
            else if (flagEnemy)
            {
                num7 = 1;
                array = planetFactory.defenseSystem.turrets?.buffer;
                VSLayerMask vslayerMask = VSLayerMask.All;
                num8 = (int)(vslayerMask & VSLayerMask.GroundHigh);
                num9 = (int)((int)(vslayerMask & VSLayerMask.AirHigh) >> 2);
                consumeRegister = GameMain.statistics.production.factoryStatPool[planetFactory.index].consumeRegister;
            }
            Vector3 normalized = _this.center.normalized;
            float num10 = _this.diffusionMaxRadius;
            if (flag)
            {
                ref TurretComponent ptr = ref array[num7];
                HashSystem hashSystemDynamic = planetFactory.hashSystemDynamic;
                int[] hashPool = hashSystemDynamic.hashPool;
                int[] bucketOffsets = hashSystemDynamic.bucketOffsets;
                int[] bucketCursors = hashSystemDynamic.bucketCursors;
                TurretSearchPair[] turretSearchPairs = planetFactory.defenseSystem.turretSearchPairs;
                int num11 = ptr.searchPairBeginIndex + ptr.searchPairCount;
                for (int i = ptr.searchPairBeginIndex; i < num11; i++)
                {
                    if (turretSearchPairs[i].searchType == ESearchType.HashBlock)
                    {
                        int searchId = turretSearchPairs[i].searchId;
                        int num12 = bucketOffsets[searchId];
                        int num13 = bucketCursors[searchId];
                        for (int j = 0; j < num13; j++)
                        {
                            int num14 = num12 + j;
                            int num15 = hashPool[num14];
                            if (num15 != 0)
                            {
                                int num16 = num15 >> 28;
                                if ((1 << num16 & (int)_this.mask) != 0)
                                {
                                    int num17 = num15 & 268435455;
                                    if (num16 == 4)
                                    {
                                        ref EnemyData ptr2 = ref enemyPool[num17];
                                        if (ptr2.id == num17 && !ptr2.isInvincible && ptr2.unitId != 0)
                                        {
                                            Vector3 vector = (Vector3)ptr2.pos - _this.center;
                                            Vector3 vector2 = Vector3.Dot(normalized, vector) * normalized - vector;
                                            float num18 = vector2.x * vector2.x + vector2.y * vector2.y + vector2.z * vector2.z;
                                            if (num18 >= num4 && num18 <= num5)
                                            {
                                                ref EnemyUnitComponent ptr3 = ref buffer[ptr2.unitId];
                                                float num19 = (2f - Mathf.Sqrt(num18) / _this.diffusionMaxRadius) * 0.5f * _this.disturbStrength * (Relic.HaveRelic(1, 11) ? 6 : 1); // relic 1-11 额外强度，为什么不在turret那里改呢？因为改strength会导致特效太浓，干扰玩家的视角
                                                if (ptr3.disturbValue < num19)
                                                {
                                                    bool flag2 = true;
                                                    if (ptr.IsAirEnemy((int)ptr2.protoId))
                                                    {
                                                        if (num9 == 0)
                                                        {
                                                            flag2 = false;
                                                        }
                                                    }
                                                    else if (num8 == 0)
                                                    {
                                                        flag2 = false;
                                                    }
                                                    if (ptr3.disturbValue + num6 < num19)
                                                    {
                                                        if (flag2 && ptr.bulletCount == 0)
                                                        {
                                                            if (ptr.itemCount > 0)
                                                            {
                                                                ptr.LoadAmmo(consumeRegister);
                                                            }
                                                            else
                                                            {
                                                                flag2 = false;
                                                            }
                                                        }
                                                        if (flag2 && _this.caster.type == ETargetType.None && !Relic.HaveRelic(0, 8)) // 由relic1-11发射的额外波，设置为casterType是ruin，不消耗额外弹药
                                                        {
                                                            ref TurretComponent ptr4 = ref ptr;
                                                            ptr4.bulletCount -= 1;
                                                        }
                                                    }
                                                    if (flag2)
                                                    {
                                                        // 造成伤害
                                                        if (Relic.HaveRelic(0, 8))
                                                        {
                                                            int realDamage = ptr.itemId == 1612 ? Relic.disturbDamage1612 : Relic.disturbDamage1613;
                                                            realDamage = (int)(realDamage * 1.0 * GameMain.history.magneticDamageScale);
                                                            realDamage = Relic.BonusDamage(realDamage, 1);
                                                            SkillTargetLocal skillTargetLocal = default(SkillTargetLocal);
                                                            skillTargetLocal.type = ETargetType.Enemy;
                                                            skillTargetLocal.id = ptr2.id;
                                                            skillSystem.DamageGroundObjectByLocalCaster(planetFactory, realDamage, 1, ref skillTargetLocal, ref _this.caster);
                                                        }
                                                        // 原逻辑 外加relic 1-11 的强化效果
                                                        ptr3.disturbValue = num19;
                                                        DFGBaseComponent dfgbaseComponent = planetFactory.enemySystem.bases[(int)ptr2.owner];
                                                        if (dfgbaseComponent != null && dfgbaseComponent.id == (int)ptr2.owner)
                                                        {
                                                            skillSystem.AddGroundEnemyHatred(dfgbaseComponent, ref ptr2, ETargetType.None, _this.caster.id, (int)(num19 * 800f + 0.5f));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (flagEnemy)
            {
                // 根据防御塔自身的SearchPair逻辑创建一个bucketIndexes用于寻敌
                List<int> bucketIndexes = new List<int>();
                HashSystem hashSystemDynamic = planetFactory.hashSystemDynamic;
                int[] hashPool = hashSystemDynamic.hashPool;
                int[] bucketOffsets = hashSystemDynamic.bucketOffsets;
                Vector3 vector3 = _this.center;
                float num55 = 100;
                float cellSize = HashSystem.cellSize;
                int num77 = 5;
                HashSystem.Cell[] bucketMap = HashSystem.bucketMap;
                int num88 = (int)((vector3.x + 270f) / cellSize);
                int num99 = (int)((vector3.y + 270f) / cellSize);
                int num1010 = (int)((vector3.z + 270f) / cellSize);
                num88 = ((num88 < 99) ? ((num88 < 0) ? 0 : num88) : 99);
                num99 = ((num99 < 99) ? ((num99 < 0) ? 0 : num99) : 99);
                num1010 = ((num1010 < 99) ? ((num1010 < 0) ? 0 : num1010) : 99);
                int num1111 = num1010 * 10000 + num99 * 100 + num88 << num77;
                for (int k = 0; k < 32; k++)
                {
                    HashSystem.Cell cell = bucketMap[num1111 + k];
                    if ((float)cell.dist > num55)
                    {
                        break;
                    }
                    bucketIndexes.Add(cell.bucketIndex);
                }
                //HashSystem hashSystemDynamic = planetFactory.hashSystemDynamic;
                //int[] hashPool = hashSystemDynamic.hashPool;
                //int[] bucketOffsets = hashSystemDynamic.bucketOffsets;
                int[] bucketCursors = hashSystemDynamic.bucketCursors;
                for (int si = 0; si < bucketIndexes.Count; si++)
                {
                    int searchId = bucketIndexes[si];
                    int num12 = bucketOffsets[searchId];
                    int num13 = bucketCursors[searchId];
                    for (int j = 0; j < num13; j++)
                    {
                        int num14 = num12 + j;
                        int num15 = hashPool[num14];
                        if (num15 != 0)
                        {
                            int num16 = num15 >> 28;
                            if ((1 << num16 & (int)_this.mask) != 0)
                            {
                                int num17 = num15 & 268435455;
                                if (num16 == 4)
                                {
                                    ref EnemyData ptr2 = ref enemyPool[num17];
                                    if (ptr2.id == num17 && !ptr2.isInvincible && ptr2.unitId != 0)
                                    {
                                        Vector3 vector = (Vector3)ptr2.pos - _this.center;
                                        Vector3 vector2 = Vector3.Dot(normalized, vector) * normalized - vector;
                                        float num18 = vector2.x * vector2.x + vector2.y * vector2.y + vector2.z * vector2.z;
                                        if (num18 >= num4 && num18 <= num5)
                                        {
                                            ref EnemyUnitComponent ptr3 = ref buffer[ptr2.unitId];
                                            float num19 = (2f - Mathf.Sqrt(num18) / _this.diffusionMaxRadius) * 0.5f * _this.disturbStrength * (Relic.HaveRelic(1, 11) ? 2 : 0.5f);
                                            if(Relic.HaveRelic(0, 9)) // 金铲铲额外强化
                                                num19 = (2f - Mathf.Sqrt(num18) / _this.diffusionMaxRadius) * 0.5f * _this.disturbStrength * (Relic.HaveRelic(1, 11) ? 7 : 1.5f);
                                            if (ptr3.disturbValue < num19)
                                            {
                                                bool flag2 = true;
                                                if (flag2)
                                                {
                                                    // 造成伤害
                                                    if (Relic.HaveRelic(0, 8) || (Relic.HaveRelic(0, 9) && Relic.HaveRelic(4, 5))) // 金铲铲额外强化，没有0-8也能造成伤害
                                                    {
                                                        int realDamage = Relic.disturbDamage1613;
                                                        realDamage = (int)(realDamage * GameMain.history.magneticDamageScale * 0.2f); // 只有铲铲或者只有0-8其中之一的时候，造成弱化伤害，都没有的话是没伤害的
                                                        if(Relic.HaveRelic(0, 8) && Relic.HaveRelic(0, 9)) // 金铲铲额外强化，既有0-8又有金铲铲则造成强化伤害
                                                            realDamage = (int)(realDamage * GameMain.history.magneticDamageScale * 1.2f);
                                                        realDamage = Relic.BonusDamage(realDamage, 1);
                                                        SkillTargetLocal skillTargetLocal = default(SkillTargetLocal);
                                                        skillTargetLocal.type = ETargetType.Enemy;
                                                        skillTargetLocal.id = ptr2.id;
                                                        skillSystem.DamageGroundObjectByLocalCaster(planetFactory, realDamage, 1, ref skillTargetLocal, ref _this.caster);
                                                    }
                                                    ptr3.disturbValue = num19;
                                                    // 不加仇恨，因为caster的id不是原本意义
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.Ballistic);
            return false;
        }


        /// <summary>
        /// 所有免费建造随机巨构的效果结算
        /// </summary>
        public static void AutoBuildMega()
        {
            if (Relic.autoConstructMegaStructurePPoint >= 1000)
            {
                Interlocked.Add(ref Relic.autoConstructMegaStructureCountDown, Relic.autoConstructMegaStructurePPoint / 1000);
                Interlocked.Exchange(ref Relic.autoConstructMegaStructurePPoint, Relic.autoConstructMegaStructurePPoint % 1000);
            }
            if (Relic.autoConstructMegaStructureCountDown > 0)
            {
                Relic.AutoBuildMegaStructure(-1, 120);
                Interlocked.Add(ref Relic.autoConstructMegaStructureCountDown, -1);
            }
        }


        /// <summary>
        /// relic 1-0
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameHistoryData), "NotifyTechUnlock")]
        public static void AutoConstructMegaWhenTechUnlock()
        {
            if (Relic.HaveRelic(1, 0))
                Interlocked.Add(ref Relic.autoConstructMegaStructureCountDown, 10 * 60);
        }

        /// <summary>
        /// relic 1-1 2-10
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TurretComponent), "InternalUpdate")]
        public static void TurretComponentPostPatch(ref TurretComponent __instance)
        {
            ref var _this = ref __instance;
            // relic 1-1
            if (Relic.HaveRelic(1, 1))
            {
                if (_this.supernovaStrength == 30f)
                {
                    _this.supernovaTick = 1501;
                }
                if (_this.supernovaTick >= 900)
                {
                    _this.supernovaStrength = 29.46f;
                }
            }
            if (Relic.HaveRelic(2, 10))
            {
                if (_this.supernovaTick <= 601 && _this.supernovaTick > 300)
                {
                    _this.supernovaTick = 300;
                }
                else if (_this.supernovaTick < -120)
                {
                    _this.supernovaTick = -120;
                }
            }
        }


        /// <summary>
        /// relic1-2
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetATField), "BreakShield")]
        public static bool BreakFieldPrePatch(ref PlanetATField __instance)
        {
            if (Relic.HaveRelic(1, 2))
            {
                if (__instance.recoverCD <= 0)
                {
                    __instance.energy = __instance.energyMax;
                    __instance.recoverCD = 36000;
                }
                else
                {
                    __instance.energy = 0L;
                    if (__instance.rigidTime == 0)
                    {
                        __instance.recoverCD = Math.Max(360, __instance.recoverCD);
                    }
                    __instance.ClearFieldResistHistory();
                }
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// relic 1-3 1-10 2-12
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="damage"></param>
        /// <param name="slice"></param>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SkillSystem), "DamageObject")]
        public static bool DamageObjectPrePatch(ref SkillSystem __instance, ref int damage, int slice, ref SkillTarget target, ref SkillTarget caster)
        {
            //if(target.type == ETargetType.Player)
            //{
            //    string res = " unknown";
            //    if (caster.type == ETargetType.Player)
            //        res = " myself";
            //    else if (caster.astroId > 1000000)
            //    {
            //        res = " caster astro id > 1000000";
            //        if(caster.type == ETargetType.Enemy)
            //        {
            //            ref EnemyData ptr = ref __instance.sector.enemyPool[target.id];
            //            res += $"-- enemy, is relay ? {ptr.dfRelayId}, is unit ? {ptr.unitId}";
            //        }
            //        else if (caster.type == ETargetType.Craft)
            //        {
            //            res += "-- craft";
            //        }
            //    }
            //    Utils.Log($"damage player direct by" + res);
            //}

            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.Damage);
            bool r0103 = Relic.HaveRelic(1, 3);
            bool r0109 = Relic.HaveRelic(1, 9);
            bool r0110 = Relic.trueDamageActive > 0;
            bool r0212 = Relic.HaveRelic(2, 12);
            int cursedRelicCount = Relic.GetRelicCount(4);

            ref var _this = ref __instance;
            float factor = 1.0f;
            int antiArmor = 0;
            int astroId = target.astroId;
            if (astroId > 1000000)
            {
                if (target.type == ETargetType.Enemy)
                {
                    EnemyDFHiveSystem enemyDFHiveSystem = _this.sector.dfHivesByAstro[astroId - 1000000];
                    int starIndex = enemyDFHiveSystem?.starData?.index ?? -1;
                    if (r0103 && starIndex >= 0 && starIndex < GameMain.data.dysonSpheres.Length)
                    {
                        DysonSphere sphere = GameMain.data.dysonSpheres[starIndex];
                        if (sphere != null)
                        {
                            if (Relic.HaveRelic(0, 9))
                                factor += (float)(5 * (1.0 - sphere.energyDFHivesDebuffCoef));
                            else
                                factor += (float)(3 * (1.0 - sphere.energyDFHivesDebuffCoef));
                        }
                    }
                    if (r0110 && enemyDFHiveSystem != null)
                    {
                        int level = enemyDFHiveSystem.evolve.level;
                        int num2 = 100 / slice;
                        int num3 = level * num2 / 2;
                        antiArmor = num3;
                    }
                    float critical = r0212 ? 0 : 0.1f + SkillPoints.criticalRate;
                    if (Relic.Verify(critical)) // relic 2-12
                    {
                        factor += 1f;
                    }
                    factor *= SkillPoints.globalDamageRate;
                    antiArmor += SkillPoints.armorPenetration / slice;
                    damage = (int)(damage * (1 - 0.05f * cursedRelicCount));
                }
                else if (target.type == ETargetType.Craft)
                {
                    if (r0109 && __instance.mecha.energyShieldEnergy > __instance.mecha.energyShieldCapacity * 0.5)
                    {
                        if (Relic.HaveRelic(0, 9))
                            damage = (int)(damage * 0.5f);
                        __instance.MechaEnergyShieldResist(caster, ref damage);
                        if (damage > 0)
                        {
                            __instance.mecha.TakeDamage(damage);
                            __instance.AddMechaHatred(caster.astroId, caster.id, damage);
                        }
                        damage = 0;
                    }
                }
            }
            else if (astroId > 100 && astroId <= 204899 && astroId % 100 > 0)
            {
                if (caster.astroId == astroId)
                {
                    MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.Damage);
                    MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
                    MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
                    return true; // 交由DamageGroundObjectByLocalCaster的prePatch自行处理，因为这个DamageGroundObjectByLocalCaster不止被DamageObject调用，还被各种skill的TickSkillLogic调用
                }
                else
                {
                    MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.Damage);
                    MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
                    MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
                    return true; // 也交由DamageGroundObjectByRemoteCaster的prePatch自行处理
                }
            }
            else if (astroId % 100 == 0 && target.type == ETargetType.Craft)
            {
                if (r0109 && __instance.mecha.energyShieldEnergy > __instance.mecha.energyShieldCapacity * 0.5)
                {
                    if (Relic.HaveRelic(0, 9))
                        damage = (int)(damage * 0.5f);
                    __instance.MechaEnergyShieldResist(caster, ref damage);
                    if (damage > 0)
                    {
                        __instance.mecha.TakeDamage(damage);
                        __instance.AddMechaHatred(caster.astroId, caster.id, damage);
                    }
                    damage = 0;
                }
            }
            damage = Relic.BonusedDamage(damage, factor - 1) + antiArmor;
            

            // 虚空入侵的敌人，在入侵开始前几乎免疫常规武器伤害
            if(target.astroId > 1000000 && target.type == ETargetType.Enemy) 
            {
                EnemyDFHiveSystem enemyDFHiveSystem = __instance.sector.dfHivesByAstro[target.astroId - 1000000];
                int byAstroId = target.astroId - 1000000;
                if (slice != starCannonDamageSlice && byAstroId >= 0 && byAstroId < AssaultController.invincibleHives.Length) // slice == 7 为恒星炮伤害，不为7的认定为常规武器
                {
                    ref EnemyData ptr = ref __instance.sector.enemyPool[target.id];
                    if(!ptr.isAssaultingUnit && AssaultController.invincibleHives[byAstroId] >= 0 && ptr.dfRelayId == 0)
                        damage = Math.Min(1000, (int)(damage * 0.001)); // 只能造成1‰的伤害，且单次不能超过10点。免伤效果对于正在进攻中的单位无效，对于中继站无效
                }
                if(slice != starCannonDamageSlice && byAstroId >= 0 && byAstroId < AssaultController.alertHives.Length) // 所有被虚空同化的黑雾获得一个至多75%伤害减免buff，用来平衡后期舰队数量降低的buff
                {
                    if (AssaultController.alertHives[byAstroId] >= 0 && AssaultController.assaultHives.Count > 0)
                    {
                        int starIndex = AssaultController.assaultHives[0] != null ? AssaultController.assaultHives[0].starIndex : 0;
                        int curStarWave = Configs.wavePerStar[starIndex];
                        if (curStarWave < 0)
                            curStarWave = 0;
                        if (curStarWave >= Configs.voidEnemyTakeDamageFactor.Count)
                            curStarWave = Configs.voidEnemyTakeDamageFactor.Count - 1;
                        damage = (int)(damage * Configs.voidEnemyTakeDamageFactor[curStarWave]);
                    }
                }
                if(AssaultController.modifierEnabled)
                {
                    if(byAstroId >= 0 && byAstroId < AssaultController.modifierHives.Length && AssaultController.modifierHives[byAstroId] >= 0)
                    {
                        int value = AssaultController.modifier[0];
                        if(value > 0)
                        {
                            damage = (int)(damage * (100f - value) / 100f);
                        }
                        value = AssaultController.modifier[1];
                        if (value > 0)
                        {
                            if (Utils.RandDouble() * 100 < value)
                                damage = 0;
                        }
                        value = AssaultController.modifier[2];
                        if (value > 0)
                        {
                            if (!r0110)
                                damage -= value * 100;
                            if (damage < 0)
                                damage = 0;
                        }
                        value = AssaultController.modifier[3];
                        if (value > 0)
                        {

                        }
                    }
                }
            }

            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.Damage);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SkillSystem), "DamageGroundObjectByLocalCaster")]
        public static bool DamageGroundObjectByLocalCasterPrePatch(ref SkillSystem __instance, PlanetFactory factory, ref int damage, int slice, ref SkillTargetLocal target, ref SkillTargetLocal caster)
        {
            if(target.type == ETargetType.Player)
            {
                //Utils.Log($"damage ground player direct by local caster, type : {caster.type} and {caster.id}");
            }
            if (target.id <= 0)
            {
                return true;
            }
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.Damage);
            bool r0109 = Relic.HaveRelic(1, 9);
            bool r0110 = Relic.trueDamageActive > 0;
            bool r0212 = Relic.HaveRelic(2, 12);
            int cursedRelicCount = Relic.GetRelicCount(4);

            ref var _this = ref __instance;
            float factor = 1.0f;
            int antiArmor = 0;
            if (target.type == ETargetType.Enemy)
            {
                ref EnemyData ptr2 = ref factory.enemyPool[target.id];
                if (ptr2.id != target.id || ptr2.isInvincible)
                {
                    return true;
                }
                DFGBaseComponent dfgbaseComponent = null;
                if (ptr2.owner > 0)
                {
                    dfgbaseComponent = factory.enemySystem.bases[(int)ptr2.owner];
                    if (dfgbaseComponent.id != (int)ptr2.owner)
                    {
                        dfgbaseComponent = null;
                    }
                }
                if (r0110 && dfgbaseComponent != null)
                {
                    int level = dfgbaseComponent.evolve.level;
                    int num2 = 100 / slice;
                    int num3 = level * num2 / 5;
                    antiArmor = num3;
                }
                float critical = r0212 ? 0 : 0.1f + SkillPoints.criticalRate;
                if (Relic.Verify(critical)) // relic 2-12
                {
                    factor += 1f;
                }
                factor *= SkillPoints.globalDamageRate;
                damage = (int)(damage * (1 - 0.05f * cursedRelicCount));
                antiArmor += SkillPoints.armorPenetration / slice;
            }
            else if (target.type == ETargetType.Craft)
            {
                if (r0109 && __instance.mecha.energyShieldEnergy > __instance.mecha.energyShieldCapacity * 0.5)
                {
                    if (Relic.HaveRelic(0, 9))
                        damage = (int)(damage * 0.5f);
                    __instance.MechaEnergyShieldResist(caster, factory.planetId, ref damage);
                    if (damage > 0)
                    {
                        __instance.mecha.TakeDamage(damage);
                        __instance.AddMechaHatred(factory.planetId, caster.id, damage);
                    }
                    damage = 0;
                }
            }
            damage = Relic.BonusedDamage(damage, factor - 1) + antiArmor;
            
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.Damage);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SkillSystem), "DamageGroundObjectByRemoteCaster")]
        public static bool DamageGroundObjectByRemoteCastPrePatch(ref SkillSystem __instance, PlanetFactory factory, ref int damage, int slice, ref SkillTargetLocal target, ref SkillTarget caster)
        {
            if (target.type == ETargetType.Player)
            {
                //Utils.Log($"damage ground player direct by remote caster, type : {caster.type} and {caster.id}");
            }
            if (target.id <= 0)
            {
                return true;
            }
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.Damage);
            bool r0109 = Relic.HaveRelic(1, 9);
            bool r0110 = Relic.trueDamageActive > 0;
            bool r0212 = Relic.HaveRelic(2, 12);
            int cursedRelicCount = Relic.GetRelicCount(4);

            ref var _this = ref __instance;
            float factor = 1.0f;
            int antiArmor = 0;
            if (target.type == ETargetType.Enemy)
            {
                ref EnemyData ptr2 = ref factory.enemyPool[target.id];
                if (ptr2.id != target.id || ptr2.isInvincible)
                {
                    return true;
                }
                DFGBaseComponent dfgbaseComponent = null;
                if (ptr2.owner > 0)
                {
                    dfgbaseComponent = factory.enemySystem.bases[(int)ptr2.owner];
                    if (dfgbaseComponent.id != (int)ptr2.owner)
                    {
                        dfgbaseComponent = null;
                    }
                }
                if (r0110 && dfgbaseComponent != null)
                {
                    int level = dfgbaseComponent.evolve.level;
                    int num2 = 100 / slice;
                    int num3 = level * num2 / 5;
                    antiArmor = num3;
                }
                float critical = r0212 ? 0 : 0.1f + SkillPoints.criticalRate;
                if (Relic.Verify(critical)) // relic 2-12
                {
                    factor += 1f;
                }
                factor *= SkillPoints.globalDamageRate;
                damage = (int)(damage * (1 - 0.05f * cursedRelicCount));
                antiArmor += SkillPoints.armorPenetration / slice;
            }
            else if (target.type == ETargetType.Craft)
            {
                if (r0109 && __instance.mecha.energyShieldEnergy > __instance.mecha.energyShieldCapacity * 0.5)
                {
                    if (Relic.HaveRelic(0, 9))
                        damage = (int)(damage * 0.5f);
                    __instance.MechaEnergyShieldResist(caster, ref damage);
                    if (damage > 0)
                    {
                        __instance.mecha.TakeDamage(damage);
                        __instance.AddMechaHatred(caster.astroId, caster.id, damage);
                    }
                    damage = 0;
                }
            }
            damage = Relic.BonusedDamage(damage, factor - 1) + antiArmor;
            
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.Damage);
            return true;
        }

        // Test
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mecha), "TakeDamage")]
        public static bool MechaTakeDamagePrefix(ref Mecha __instance, ref int damage)
        {
            __instance.EnergyShieldResist(ref damage);
            if (damage > 0)
            {
                if (!__instance.player.invincible)
                {
                    __instance.hpRecoverCD = 360;
                    __instance.hp -= damage;
                }
                if (__instance.hp <= 0)
                {
                    __instance.hp = 0;
                    __instance.hpRecoverCD = 0;
                    __instance.energyShieldRecoverCD = 0;
                    __instance.player.Kill();
                }
            }
            return false;
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
        /// relic 1-7 弹射器射速翻倍
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
        public static void EjectorInternalUpdatePostFix(ref EjectorComponent __instance, float power)
        {
            if (Relic.HaveRelic(1, 7))
            {
                float num2 = (float)Cargo.accTableMilli[__instance.incLevel];
                int num3 = (int)(power * 10000f * (1f + num2) + 0.1f);
                if (__instance.orbitId != 0 && power >= 0.1f)
                {
                    if (__instance.direction == 1)
                    {
                        __instance.time += num3;
                        if (__instance.time > __instance.chargeSpend)
                            __instance.time = __instance.chargeSpend;
                    }
                    else if (__instance.direction == -1)
                    {
                        __instance.time -= num3;
                        if (__instance.time <= 0)
                        {
                            __instance.time = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// relic 1-8 有两个同名方法，虽然第二个没有分析道被其他方法调用，但是还是patch一下吧
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mecha), "EnergyShieldResist", new Type[] { typeof(int) }, new ArgumentType[] { ArgumentType.Ref })]
        public static void MechaEnergyShieldResistPostPatch0(ref Mecha __instance)
        {
            if (__instance.energyShieldEnergy <= 0 && Relic.HaveRelic(1, 8))
            {
                lock (__instance)
                {
                    long maxShieldEnergy = __instance.energyShieldCapacity;
                    long totalNeedEnergy = maxShieldEnergy * Relic.bansheesVeilFactor;
                    long needEnergy = totalNeedEnergy;
                    if (needEnergy > __instance.reactorEnergy)
                    {
                        needEnergy -= (long)__instance.reactorEnergy;
                        __instance.reactorEnergy = 0;
                        for (int i = __instance.reactorStorage.size - 1; i >= 0; i--)
                        {
                            int itemId = __instance.reactorStorage.grids[i].itemId;
                            if (itemId > 0 && __instance.reactorStorage.grids[i].count > 0)
                            {
                                long heat = LDB.items.Select(itemId)?.HeatValue ?? 0;
                                long totalHeat = heat * __instance.reactorStorage.grids[i].count;
                                if (totalHeat > needEnergy)
                                {
                                    int needCount = (int)(needEnergy / heat);
                                    int inc = 0;
                                    inc = __instance.reactorStorage.split_inc(ref __instance.reactorStorage.grids[i].count, ref __instance.reactorStorage.grids[i].inc, needCount);
                                    needEnergy -= heat * needCount;
                                    if (needEnergy > 0)
                                    {
                                        inc = __instance.reactorStorage.split_inc(ref __instance.reactorStorage.grids[i].count, ref __instance.reactorStorage.grids[i].inc, 1);
                                        long energyLeft = Math.Max(0, heat - needEnergy);
                                        __instance.reactorItemId = itemId;
                                        __instance.reactorItemInc = inc;
                                        __instance.reactorEnergy = energyLeft;
                                    }
                                    break;
                                }
                                else
                                {
                                    needEnergy -= totalHeat;
                                    __instance.reactorStorage.grids[i].count = 0;
                                    __instance.reactorStorage.grids[i].itemId = __instance.reactorStorage.grids[i].filter;
                                    __instance.reactorStorage.grids[i].inc = 0;
                                }
                            }
                        }
                        long realShieldEnergyRestored = (totalNeedEnergy - needEnergy) / Relic.bansheesVeilFactor;
                        __instance.energyShieldEnergy = realShieldEnergyRestored;
                        __instance.reactorStorage.NotifyStorageChange();
                    }
                    else
                    {
                        __instance.energyShieldEnergy = __instance.energyShieldCapacity;
                        __instance.reactorEnergy -= needEnergy;
                    }
                }
                if (Relic.bansheesVeilIncreaseCountdown > 0)
                {
                    Relic.bansheesVeilFactor *= 2;
                    if (Relic.bansheesVeilFactor > Relic.bansheesVeilMaxFactor)
                        Relic.bansheesVeilFactor = Relic.bansheesVeilMaxFactor;
                }
                Relic.bansheesVeilIncreaseCountdown = Relic.bansheesVeilMaxCountdown;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mecha), "EnergyShieldResist", new Type[] { typeof(int), typeof(int) }, new ArgumentType[] { ArgumentType.Ref, ArgumentType.Out })]
        public static void MechaEnergyShieldResistPostPatch1(ref Mecha __instance)
        {
            if (__instance.energyShieldEnergy <= 0 && Relic.HaveRelic(1, 8) && __instance.reactorEnergy > Relic.bansheesVeilFactor * __instance.energyShieldEnergyRate)
            {
                lock (__instance)
                {
                    long maxShieldEnergy = __instance.energyShieldCapacity;
                    long totalNeedEnergy = maxShieldEnergy * Relic.bansheesVeilFactor;
                    long needEnergy = totalNeedEnergy;
                    if (needEnergy > __instance.reactorEnergy)
                    {
                        needEnergy -= (long)__instance.reactorEnergy;
                        __instance.reactorEnergy = 0;
                        for (int i = __instance.reactorStorage.size - 1; i >= 0; i--)
                        {
                            int itemId = __instance.reactorStorage.grids[i].itemId;
                            if (itemId > 0 && __instance.reactorStorage.grids[i].count > 0)
                            {
                                long heat = LDB.items.Select(itemId)?.HeatValue ?? 0;
                                long totalHeat = heat * __instance.reactorStorage.grids[i].count;
                                if (totalHeat > needEnergy)
                                {
                                    int needCount = (int)(needEnergy / heat);
                                    int inc = 0;
                                    inc = __instance.reactorStorage.split_inc(ref __instance.reactorStorage.grids[i].count, ref __instance.reactorStorage.grids[i].inc, needCount);
                                    needEnergy -= heat * needCount;
                                    if (needEnergy > 0)
                                    {
                                        inc = __instance.reactorStorage.split_inc(ref __instance.reactorStorage.grids[i].count, ref __instance.reactorStorage.grids[i].inc, 1);
                                        long energyLeft = Math.Max(0, heat - needEnergy);
                                        __instance.reactorItemId = itemId;
                                        __instance.reactorItemInc = inc;
                                        __instance.reactorEnergy = energyLeft;
                                    }
                                    break;
                                }
                                else
                                {
                                    needEnergy -= totalHeat;
                                    __instance.reactorStorage.grids[i].count = 0;
                                    __instance.reactorStorage.grids[i].itemId = __instance.reactorStorage.grids[i].filter;
                                    __instance.reactorStorage.grids[i].inc = 0;
                                }
                            }
                        }
                        long realShieldEnergyRestored = (totalNeedEnergy - needEnergy) / Relic.bansheesVeilFactor;
                        __instance.energyShieldEnergy = realShieldEnergyRestored;
                        __instance.reactorStorage.NotifyStorageChange();
                    }
                    else
                    {
                        __instance.energyShieldEnergy = __instance.energyShieldCapacity;
                        __instance.reactorEnergy -= needEnergy;
                    }
                }
                if (Relic.bansheesVeilIncreaseCountdown > 0)
                {
                    Relic.bansheesVeilFactor *= 2;
                    if (Relic.bansheesVeilFactor > Relic.bansheesVeilMaxFactor)
                        Relic.bansheesVeilFactor = Relic.bansheesVeilMaxFactor;
                }
                Relic.bansheesVeilIncreaseCountdown = Relic.bansheesVeilMaxCountdown;
            }
        }

        // relic 1-8短时间内反复触发的倒计时
        public static void CheckBansheesVeilCountdown(long time)
        {
            if (Relic.bansheesVeilIncreaseCountdown > Relic.bansheesVeilMaxCountdown)
                Relic.bansheesVeilIncreaseCountdown = Relic.bansheesVeilMaxCountdown;
            else if (Relic.bansheesVeilIncreaseCountdown > 0)
                Relic.bansheesVeilIncreaseCountdown--;
            else if (Relic.bansheesVeilIncreaseCountdown <= 0 && time % 60 == 1)
            {
                if (Relic.bansheesVeilFactor > Relic.bansheesVeilBasicFactor)
                    Relic.bansheesVeilFactor /= 2;
                else if (Relic.bansheesVeilFactor < Relic.bansheesVeilBasicFactor)
                    Relic.bansheesVeilFactor = Relic.bansheesVeilBasicFactor;
            }
        }

        /// <summary>
        /// relic 1-11
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="factory"></param>
        /// <param name="pdesc"></param>
        /// <param name="power"></param>
        /// <param name="gameTick"></param>
        /// <param name="combatUpgradeData"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TurretComponent), "Shoot_Disturb")]
        public static bool ShootDisturbPostPatch(ref TurretComponent __instance, PlanetFactory factory, PrefabDesc pdesc, float power, long gameTick, ref CombatUpgradeData combatUpgradeData)
        {
            if (power < 0.1f || (__instance.bulletCount == 0 && __instance.itemCount == 0))
            {
                return false;
            }
            int num = __instance.phasePos;
            int num2 = pdesc.turretRoundInterval / pdesc.turretROF;
            int flag = 0;
            if (num % num2 == (int)(gameTick % (long)num2))
            {
                flag = 1;
            }
            else if (Relic.HaveRelic(1, 11))
            {
                if (num % num2 == (int)((gameTick - 30) % (long)num2))
                    flag = 2;
                else if (num % num2 == (int)((gameTick - 60) % (long)num2))
                    flag = 2;
            }
            if (flag > 0)
            {
                ref LocalDisturbingWave ptr = ref GameMain.data.spaceSector.skillSystem.turretDisturbingWave.Add();
                ptr.astroId = factory.planetId;
                ptr.protoId = (int)__instance.itemId;
                ptr.center = factory.entityPool[__instance.entityId].pos;
                ptr.rot = factory.entityPool[__instance.entityId].rot;
                ptr.mask = ETargetTypeMask.Enemy;
                ptr.caster.type = flag == 1 ? ETargetType.None : ETargetType.Ruin;
                ptr.caster.id = __instance.entityId;
                ptr.disturbStrength = (float)__instance.bulletDamage * pdesc.turretDamageScale * combatUpgradeData.magneticDamageScale * power * 0.01f;
                ptr.thickness = 2.5f;
                ptr.diffusionSpeed = 45f;
                ptr.diffusionMaxRadius = pdesc.turretMaxAttackRange;
                ptr.StartToDiffuse();
            }
            return false;
        }

        /// <summary>
        /// trlic 1-11
        /// </summary>
        public static void RefreshDisturbPrefabDesc()
        {
            if (Relic.HaveRelic(1, 11))
            {
                PlanetFactory.PrefabDescByModelIndex[422].turretMaxAttackRange = 60;
            }
            else
            {
                PlanetFactory.PrefabDescByModelIndex[422].turretMaxAttackRange = 40;
            }
        }

        /// <summary>
        /// relic 2-0 
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__state"></param>
        /// <returns></returns>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerSystem), "GameTick")]
        public static void PowerSystemPostPatch(ref PowerSystem __instance)
        {
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
            if (Relic.HaveRelic(2, 0))
            {
                PlanetATField planetATField = __instance.factory.planetATField;
                long extra = 0;
                if (planetATField.atFieldRechargeCurrent > 0)
                {
                    extra = (long)(planetATField.atFieldRechargeCurrent / 60 * 0.5);
                    if (extra > planetATField.energyMax - planetATField.energy)
                        extra = planetATField.energyMax - planetATField.energy;
                }
                planetATField.energy += extra;
                planetATField.atFieldRechargeCurrent += extra * 60L;
            }
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
        }

        // relic 2-2 2-14 3-9 4-4 在EventSystem的ZeroHpInceptor处实现
        public static void QuickNavi()
        {
            CombatStat c = new CombatStat();
            EventSystem.ZeroHpInceptor(ref c, GameMain.data, GameMain.spaceSector.skillSystem);
        }

        /// <summary>
        /// relic2-5 3-10 1-12
        /// </summary>
        public static void CheckPlayerHasaki()
        {
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
            if (Relic.HaveRelic(2, 5) || Relic.HaveRelic(3, 10))
            {
                Vector3 pos = GameMain.mainPlayer.position;
                if (pos.x != Relic.playerLastPos.x || pos.y != Relic.playerLastPos.y || pos.z != Relic.playerLastPos.z)
                {
                    if (Relic.HaveRelic(2, 5) && Relic.Verify(0.08))
                    {
                        GameMain.mainPlayer.TryAddItemToPackage(9500, 1, 0, true);
                        Utils.UIItemUp(9500, 1, 200);
                    }
                    if (Relic.HaveRelic(3, 10) && Relic.Verify(0.03))
                    {
                        GameMain.mainPlayer.TryAddItemToPackage(9500, 1, 0, true);
                        Utils.UIItemUp(9500, 1, 200);
                    }

                    Relic.playerLastPos = new Vector3(pos.x, pos.y, pos.z);
                }
            }
            if (Relic.HaveRelic(1, 12))
            {
                if(Relic.HaveRelic(0, 9)) // 铲子强化
                {
                    Relic.playerIdleTimeMax = 600;
                }
                else
                {
                    Relic.playerIdleTimeMax = 3600 * 4;
                }
                Vector3 pos = GameMain.mainPlayer.position;
                if (pos.x == Relic.playerLastPos.x && pos.y == Relic.playerLastPos.y && pos.z == Relic.playerLastPos.z && GameMain.mainPlayer.planetId > 0)
                {
                    Relic.playerIdleTime += 60;
                    if (Relic.playerIdleTime > Relic.playerIdleTimeMax)
                        Relic.playerIdleTime = Relic.playerIdleTimeMax;
                }
                else
                {
                    Relic.playerIdleTime = 0;
                }
                Relic.playerLastPos = new Vector3(pos.x, pos.y, pos.z);
            }
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
        }

        /// <summary>
        /// relic 2-6 每次mark机甲能量消耗func==12的时候是战斗无人机消耗，此时返还一些机甲能量，即可以达到减少机甲能量消耗的效果。（因为涉及mecha的coreEnergy的改变的方法太多，一个个拦截有点麻烦）
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="func"></param>
        /// <param name="change"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mecha), "MarkEnergyChange")]
        public static void MechaMarkEnergyChangePostPatch(ref Mecha __instance, int func, double change)
        {
            if ((func == 12 || func == 13) && change < 0 && Relic.HaveRelic(2, 6))
            {
                Mecha obj = __instance;
                lock (obj)
                {
                    double restore = change * -0.4;
                    if (restore > __instance.coreEnergyCap - __instance.coreEnergy)
                        restore = __instance.coreEnergyCap - __instance.coreEnergy;
                    __instance.coreEnergy += restore;
                    __instance.MarkEnergyChange(func, restore);
                }
            }
        }

        /// <summary>
        /// relic 2-9 3-13 3-16 恒星炮充能速度buff和伤害buff
        /// </summary>
        public static void RefreshStarCannonBuffs()
        {
            float chargeSpeedFactor = 1.0f;
            if (Relic.HaveRelic(2, 9))
                chargeSpeedFactor += 0.5f;
            if (Relic.HaveRelic(3, 13))
                chargeSpeedFactor += 0.25f;
            if (Rank.rank >= 8)
                chargeSpeedFactor += 0.5f;
            MoreMegaStructure.StarCannon.chargeSpeedFactorByTCFV = chargeSpeedFactor;

            float damageFactor = 1.0f;
            if (Relic.HaveRelic(3, 16))
            {
                if (Relic.HaveRelic(2, 13))
                    damageFactor += 0.2f;
                else
                    damageFactor += 0.1f;
            }
            MoreMegaStructure.StarCannon.damageFactorByTCFV = damageFactor;
        }

        /// <summary>
        /// relic 2-17 不朽之守护
        /// </summary>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "Kill")]
        public static bool PlayerKillPrePatch(ref Player __instance)
        {
            if (__instance.isAlive)
            {
                if (Relic.HaveRelic(2, 17) && Relic.aegisOfTheImmortalCooldown <= 0)
                {
                    Relic.aegisOfTheImmortalCooldown = 3600 * 20;
                    Mecha mecha = GameMain.data.mainPlayer.mecha;
                    mecha.coreEnergy = mecha.coreEnergyCap;
                    mecha.hp = mecha.hpMaxApplied;
                    mecha.hpRecoverCD = 0;
                    mecha.energyShieldEnergy = mecha.energyShieldCapacity;
                    mecha.energyShieldRecoverCD = 0;
                    GameMain.data.mainPlayer.invincibleTicks = 1800;
                    UIRealtimeTip.Popup("不朽之守护启动".Translate());
                    return false;
                }
                if (Relic.HaveRelic(4, 2))
                {
                    Rank.DownGrade();
                }
            }
            return true;
        }
        public static void AegisOfTheImmortalCountDown(long time)
        {
            if (Relic.aegisOfTheImmortalCooldown > 0)
            {
                Relic.aegisOfTheImmortalCooldown--;
                if (Relic.aegisOfTheImmortalCooldown == 0 && Relic.HaveRelic(2, 17))
                    UIRealtimeTip.Popup("不朽之守护就绪".Translate());
            }
            if (time % 60 == 32)
            {
                //for (int slotNum = 0; slotNum < UIRelic.relicSlotUIBtns.Count; slotNum++)
                //{
                //    if (UIRelic.relicInSlots[slotNum] == 108)
                //    {
                //        UIRelic.relicSlotUIBtns[slotNum].tips.tipText = "遗物描述1-8".Translate();
                //        UIRelic.AddTipText(1, 8, UIRelic.relicSlotUIBtns[slotNum], true); // 对于一些原本描述较短的，还要将更详细的描述加入
                //        UIRelic.AddTipVarData(1, 8, UIRelic.relicSlotUIBtns[slotNum]); // 对于部分需要展示实时数据的，还需要加入数据
                //        if (UIRelic.relicSlotUIBtns[slotNum].tipShowing)
                //        {
                //            UIRelic.relicSlotUIBtns[slotNum].OnPointerExit(null);
                //            UIRelic.relicSlotUIBtns[slotNum].OnPointerEnter(null);
                //            UIRelic.relicSlotUIBtns[slotNum].enterTime = 1;
                //        }
                //    }
                //    else if (UIRelic.relicInSlots[slotNum] == 217)
                //    {
                //        UIRelic.relicSlotUIBtns[slotNum].tips.tipText = "遗物描述2-17".Translate();
                //        UIRelic.AddTipText(2, 17, UIRelic.relicSlotUIBtns[slotNum], true); // 对于一些原本描述较短的，还要将更详细的描述加入
                //        UIRelic.AddTipVarData(2, 17, UIRelic.relicSlotUIBtns[slotNum]); // 对于部分需要展示实时数据的，还需要加入数据
                //        if (UIRelic.relicSlotUIBtns[slotNum].tipShowing)
                //        {
                //            UIRelic.relicSlotUIBtns[slotNum].OnPointerExit(null);
                //            UIRelic.relicSlotUIBtns[slotNum].OnPointerEnter(null);
                //            UIRelic.relicSlotUIBtns[slotNum].enterTime = 1;
                //        }
                //    }
                //}
                UIRelic.RefreshSlotsWindowUI(true);
                if (Relic.HaveRelic(0, 9) && Relic.HaveRelic(2, 17) && GameMain.mainPlayer.invincible)
                {
                    SkillPoints.globalDamageRate = 1.0f + SkillPoints.skillValuesR[0] / 100.0f * SkillPoints.skillLevelR[0] + 15;
                }
                else
                {
                    SkillPoints.globalDamageRate = 1.0f + SkillPoints.skillValuesR[0] / 100.0f * SkillPoints.skillLevelR[0];
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
        /// relic 3-1 3-3 4-5 // 额外掉落 4-5的负面影响在eventSystem的ZeroHp
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFGroundSystem), "NotifyEnemyKilled")]
        public static bool NotifyEnemyKilledPrePatch(ref EnemyDFGroundSystem __instance, ref EnemyData enemy)
        {
            if (Relic.HaveRelic(3, 1) || Relic.HaveRelic(4, 5) || Relic.HaveRelic(3, 3))
            {
                if (enemy.id != 0)
                {
                    int owner = (int)enemy.owner;
                    DFGBaseComponent dfgbaseComponent = __instance.bases.buffer[owner];
                    if (dfgbaseComponent == null || dfgbaseComponent.id != owner)
                    {
                        return false;
                    }
                    MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
                    MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
                    MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.Kill);
                    float num = (float)RandomTable.Integer(ref __instance.rtseed, 101) * 0.01f + 1.5f;
                    int count = (int)((float)SkillSystem.EnemySandCountByModelIndex[(int)enemy.modelIndex] * num * 0.2f + 0.5f);
                    if (Relic.HaveRelic(3, 3)) // relic 3-3 掉落双倍沙土
                    {
                        count *= 2;
                    }
                    __instance.gameData.trashSystem.AddTrashFromGroundEnemy(1099, count, 900, enemy.id, __instance.factory);
                    if (enemy.dynamic)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int itemId;
                            int itemCount;
                            int life;
                            __instance.RandomDropItemOnce(dfgbaseComponent.evolve.level, out itemId, out itemCount, out life);
                            if (itemId > 0 && itemCount > 0 && life > 0)
                            {
                                if (Relic.HaveRelic(3, 1) && itemId == 5201 && Relic.Verify(0.3))
                                {
                                    itemCount *= 2;
                                }
                                if (Relic.HaveRelic(4, 4) && itemId == 5201)
                                {
                                    itemCount /= 2;
                                    if (itemCount < 1 && Utils.RandDouble() <= 0.5)
                                        itemCount = 1;
                                }
                                __instance.gameData.trashSystem.AddTrashFromGroundEnemy(itemId, itemCount, life, enemy.id, __instance.factory);
                                if (false && Relic.HaveRelic(4, 5) && itemId >= 5201 && itemId <= 5205) // relic 4-5 余震回响额外掉落，已被重做
                                {
                                    int level = dfgbaseComponent.evolve.level;
                                    if (level >= 12)
                                    {
                                        if (itemId != 5201)
                                            __instance.gameData.trashSystem.AddTrashFromGroundEnemy(5201, Relic.HaveRelic(4, 4) ? (itemCount / 4) : (itemCount / 2), life, enemy.id, __instance.factory);
                                        if (level >= 15)
                                        {
                                            if (itemId != 5203)
                                                __instance.gameData.trashSystem.AddTrashFromGroundEnemy(5203, itemCount / 2, life, enemy.id, __instance.factory);
                                            if (level >= 18)
                                            {
                                                if (itemId != 5202)
                                                    __instance.gameData.trashSystem.AddTrashFromGroundEnemy(5202, itemCount / 2, life, enemy.id, __instance.factory);
                                                if (level >= 21)
                                                {
                                                    if (itemId != 5204)
                                                        __instance.gameData.trashSystem.AddTrashFromGroundEnemy(5204, itemCount / 2, life, enemy.id, __instance.factory);
                                                    if (level >= 24)
                                                    {
                                                        if (itemId != 5205)
                                                            __instance.gameData.trashSystem.AddTrashFromGroundEnemy(5205, itemCount, life, enemy.id, __instance.factory);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (dfgbaseComponent.evolve.waveTicks == 0 && dfgbaseComponent.turboTicks > 0)
                    {
                        double magnitude = (__instance.factory.enemyPool[dfgbaseComponent.enemyId].pos - enemy.pos).magnitude;
                        double num5 = 1.28 - magnitude / (double)dfgbaseComponent.currentSensorRange * 1.6;
                        if (num5 > 1.0)
                        {
                            num5 = 1.0;
                        }
                        if (num5 > 0.0)
                        {
                            int num6 = 5 + (int)(20.0 * num5 + 0.5);
                            if (dfgbaseComponent.turboRepress < num6)
                            {
                                dfgbaseComponent.turboRepress = num6;
                            }
                        }
                    }
                    if (enemy.dynamic)
                    {
                        __instance.NotifyUnitKilled(ref enemy, dfgbaseComponent);
                    }
                }
                MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
                MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
                MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.Damage);
                return false;
            }
            return true;
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
                double change = __instance.reactorPowerGen * 0.5 / 60;
                __instance.coreEnergy += change;
                GameMain.mainPlayer.mecha.MarkEnergyChange(0, change); // 算在核心发电
                if (__instance.coreEnergy > __instance.coreEnergyCap) __instance.coreEnergy = __instance.coreEnergyCap;
            }
            if (Rank.rank >= 1)
            {
                double change = 1000000 / 60;
                __instance.coreEnergy += change;
                GameMain.mainPlayer.mecha.MarkEnergyChange(0, change); // 算在核心发电
                if (__instance.coreEnergy > __instance.coreEnergyCap) __instance.coreEnergy = __instance.coreEnergyCap;
            }
        }

        /// <summary>
        /// relic 3-4 黑雾获得双倍经验
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EvolveData), "AddExp")]
        public static bool AddExpPrePatch(ref int _addexp)
        {
            if (Relic.HaveRelic(3, 4))
            {
                _addexp *= 2;
            }
            _addexp = (int)(_addexp * (1 + 0.5f * Relic.GetCursedRelicCount()));
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EvolveData), "AddExpPoint")]
        public static bool AddExpPointPrePatch(ref int _addexpp)
        {
            if (Relic.HaveRelic(3, 4))
            {
                _addexpp *= 2;
            }
            _addexpp = (int)(_addexpp * (1 + 0.5f * Relic.GetCursedRelicCount()));
            return true;
        }


        /// <summary>
        /// relic 3-5 复活币
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDeathPanel), "CalculateRespawnOptionsGroup")]
        public static void CalculateRespawnOptionsGroupPostPatch(ref UIDeathPanel __instance)
        {
            if (GameMain.data.gameDesc.isSandboxMode)
                return;
            if (Relic.resurrectCoinCount > 0)
            {
                ref var _this = ref __instance;
                int num = __instance.GetCurrentOptions().Length;

                RectTransform rectTransform = UnityEngine.Object.Instantiate<RectTransform>(_this.respawnOptionPrefab, _this.respawnRect);
                RectTransform rectTransform2 = UnityEngine.Object.Instantiate<RectTransform>(_this.respawnOptionPrefab, _this.respawnRect);
                UIButton component = rectTransform.GetComponent<UIButton>();
                UIButton component2 = rectTransform2.GetComponent<UIButton>();
                component.tips.tipTitle = "重新部署".Translate();
                component.tips.tipText = "重新部署描述".Translate() + "消耗复活币描述".Translate();
                component2.tips.tipTitle = "立刻复活".Translate();
                component2.tips.tipText = "立刻复活描述".Translate() + "消耗复活币描述".Translate();
                component.gameObject.GetComponentInChildren<Text>().text = "重新部署".Translate();
                component2.gameObject.GetComponentInChildren<Text>().text = "立刻复活".Translate();
                component.data = 2 * num;
                component2.data = 2 * num + PlayerAction_Death.kRespawnCostsCount;
                component.onClick += (x) => { OnResurrentCoinRespawnButtonClick(x); };
                component2.onClick += (x) => { OnResurrentCoinRespawnButtonClick(x); };
                Utils.Log($"data 1 2 is {2 * num} and {2 * num + 1}");
                UIIconCount uiiconCount = UnityEngine.Object.Instantiate<UIIconCount>(_this.propertyItemRespawnPrefab, rectTransform);
                UIIconCount uiiconCount2 = UnityEngine.Object.Instantiate<UIIconCount>(_this.propertyItemRespawnPrefab, rectTransform2);
                uiiconCount.gameObject.SetActive(true);
                uiiconCount2.gameObject.SetActive(true);
                _this.costTexts.Add(uiiconCount.transform.Find("value").GetComponent<Text>());
                _this.costTexts.Add(uiiconCount2.transform.Find("value").GetComponent<Text>());
                _this.zeroTexts.Add(uiiconCount.transform.Find("zero").GetComponent<Text>());
                _this.zeroTexts.Add(uiiconCount2.transform.Find("zero").GetComponent<Text>());
                uiiconCount.transform.Find("value").GetComponent<Text>().text = "1";
                uiiconCount2.transform.Find("value").GetComponent<Text>().text = "1";
                uiiconCount.GetComponent<Image>().sprite = r3_5_coin;
                uiiconCount2.GetComponent<Image>().sprite = r3_5_coin;
                uiiconCount.rectTrans.anchoredPosition = new Vector2(0, 0);
                uiiconCount2.rectTrans.anchoredPosition = new Vector2(0, 0);
                // 修改原始按钮盒新按钮的位置
                _this.respawnOptionButtons[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(((num == 2) ? -500 : -300), -60f);
                _this.respawnOptionButtons[0].transform.Find("or").gameObject.SetActive(true);
                _this.respawnOptionButtons[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -60f);
                _this.respawnOptionButtons[1].transform.Find("or").gameObject.SetActive(true);
                if (_this.respawnOptionButtons.Count >= 4)
                {
                    _this.respawnOptionButtons[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, -60f);
                    _this.respawnOptionButtons[2].transform.Find("or").gameObject.SetActive(true);
                    _this.respawnOptionButtons[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(300, -60f);
                    _this.respawnOptionButtons[3].transform.Find("or").gameObject.SetActive(true);
                }
                component.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, -60);
                component.transform.Find("or").gameObject.SetActive(true);
                component2.GetComponent<RectTransform>().anchoredPosition = new Vector2(((num == 2) ? 500 : 300), -60);
                component2.transform.Find("or").gameObject.SetActive(false);
                _this.respawnRect.sizeDelta = new Vector2((float)((num == 2) ? 960 : 780), 240f);
                component.gameObject.SetActive(true);
                component2.gameObject.SetActive(true);
                _this.respawnOptionButtons.Add(component);
                _this.respawnOptionButtons.Add(component2);
            }
        }

        public static void OnResurrentCoinRespawnButtonClick(int x)
        {
            UIDeathPanel panel = UIRoot.instance.uiGame.deathPanel;
            panel.seletedOption = x;
            if (x == panel.GetCurrentOptions().Length * 2)
            {
                panel.respawnDetailText.text = "使用复活币重新部署描述".Translate();
                panel.respawnNextText.text = "下次重新部署消耗不会增加".Translate();
            }
            else
            {
                panel.respawnDetailText.text = "使用复活币立刻复活描述".Translate();
                panel.respawnNextText.text = "下次立刻复活消耗不会增加".Translate();
            }
            panel.respawnNextIcon0.gameObject.SetActive(false);
            panel.respawnNextIcon1.gameObject.SetActive(false);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIDeathPanel), "UpdatePropertyGroup")]
        public static bool UIDeathPanelUpdatePropertyGroupPostPatch(ref UIDeathPanel __instance)
        {
            if (GameMain.data.gameDesc.isSandboxMode)
                return false;
            if (__instance.seletedOption % PlayerAction_Death.kRespawnCostsCount < __instance.GetCurrentOptions().Length)
                return true;

            __instance.propertyRect.gameObject.SetActive(true);
            long clusterSeedKey = GameMain.data.GetClusterSeedKey();
            PropertySystem propertySystem = DSPGame.propertySystem;
            int[] matrixIds = PropertySystem.matrixIds;
            for (int i = 0; i < matrixIds.Length; i++)
            {
                int itemAvaliablePropertyForRespawn = propertySystem.GetItemAvaliablePropertyForRespawn(clusterSeedKey, matrixIds[i]);
                __instance.propertyItems[i].SetCountTextN0(itemAvaliablePropertyForRespawn);
            }
            for (int j = 0; j < __instance.propertyTips.Length; j++)
            {
                __instance.propertyTips[j].gameObject.SetActive(false);
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIDeathPanel), "OnReassembleButtonClick")]
        public static bool OnReassembleButtonClickPrePatch(ref UIDeathPanel __instance)
        {
            if (UIStarmap.isChangingToMilkyWay)
            {
                return false;
            }
            if (GameMain.data.gameDesc.isSandboxMode)
            {
                return true;
            }
            if (__instance.seletedOption % PlayerAction_Death.kRespawnCostsCount < __instance.GetCurrentOptions().Length)
            {
                Relic.isUsingResurrectCoin = false;
                return true;
            }
            Relic.isUsingResurrectCoin = true;
            GameMain.data.mainPlayer.deathCount--;
            __instance.actionDeath.Respawn(2);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIDeathPanel), "OnRedeployButtonClick")]
        public static bool OnRedeployButtonClickPrePatch(ref UIDeathPanel __instance)
        {
            if (UIStarmap.isChangingToMilkyWay)
            {
                return false;
            }
            if (GameMain.data.gameDesc.isSandboxMode)
            {
                return true;
            }
            if (__instance.seletedOption % PlayerAction_Death.kRespawnCostsCount < __instance.GetCurrentOptions().Length)
            {
                Relic.isUsingResurrectCoin = false;
                return true;
            }
            Relic.isUsingResurrectCoin = true;
            GameMain.data.mainPlayer.deathCount--;
            __instance.actionDeath.Respawn(3);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDeathPanel), "_OnUpdate")]
        public static void UIDeathPenalOnUpdatePostPatch(ref UIDeathPanel __instance)
        {
            if (DSPGame.GameDesc.isSandboxMode)
                return;
            if (Relic.respawnTitleText == null) // 70 -30
            {
                Relic.respawnTitleText = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Death Panel/bg-trans/content/respawn-group/title");
            }
            if (Relic.respawnTitleText != null)
            {
                if (Relic.resurrectCoinCount > 0)
                {
                    Relic.respawnTitleText.GetComponent<Text>().text = "使用元数据或复活币".Translate();
                    Relic.respawnTitleText.transform.Find("tip-button").localPosition = new Vector3(140, -30, 0);
                }
                else
                {
                    Relic.respawnTitleText.GetComponent<Text>().text = "使用元数据".Translate();
                    Relic.respawnTitleText.transform.Find("tip-button").localPosition = new Vector3(70, -30, 0);
                }
            }
            if (__instance.respawnOptionButtons.Count > __instance.GetCurrentOptions().Length * 2)
            {
                if (__instance.respawnOptionButtons[__instance.GetCurrentOptions().Length * 2].isPointerEnter)
                    __instance.costTexts[__instance.costTexts.Count - 2].text = $"1/{Relic.resurrectCoinCount}";
                else
                    __instance.costTexts[__instance.costTexts.Count - 2].text = $"1";

                if (__instance.respawnOptionButtons[__instance.GetCurrentOptions().Length * 2 + 1].isPointerEnter)
                    __instance.costTexts[__instance.costTexts.Count - 1].text = $"1/{Relic.resurrectCoinCount}";
                else
                    __instance.costTexts[__instance.costTexts.Count - 1].text = $"1";
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerAction_Death), "SettleRespawnCost")]
        public static bool SettleRespawnCostPrePatch()
        {
            if (Relic.resurrectCoinCount > 0 && Relic.isUsingResurrectCoin)
            {
                Relic.isUsingResurrectCoin = false;
                Relic.resurrectCoinCount--;
                return false;
            }
            return true;
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
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MetaDrive);
            if (Relic.HaveRelic(3, 15))
            {
                __instance.lab.GameTick(time, dt);
                __instance.lab.GameTick(time, dt);
                __instance.lab.GameTick(time, dt);
                __instance.lab.GameTick(time, dt);
            }
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MetaDrive);
        }

        // relic 3-17 在Rank.AddExp内实现

        /// <summary>
        /// relic0-3。如果移除了relic0-3则需要重新进游戏才能应用，因为不太好算就特别地写一个移除relic0-3的计算了。
        /// </summary>
        public static void CheckAndModifyStarLuminosity(int newRelic = -1) // -1代表是游戏加载存档的操作，按顺序进行0-3和4-0的计算即可。如果不是-1，则序号代表此次增加的relic是哪个，则按条件计算
        {
            if (Relic.HaveRelic(0, 3) && (newRelic == -1 || newRelic == 3))
            {
                if (Relic.HaveRelic(4, 0) && newRelic == 3) // 说明是此次仅增加relic0-3，之前已经有了relic4-0编织者额负面buff了，则先还原其负面buff
                {
                    float maxL = 0;
                    for (int i = 0; i < GameMain.galaxy.starCount; i++)
                    {
                        StarData starData = GameMain.galaxy.stars[i];
                        if ((starData != null) && (starData.luminosity > maxL))
                        {
                            maxL = starData.luminosity;
                            Relic.starIndexWithMaxLuminosity = i;
                        }
                    }
                    for (int i = 0; i < GameMain.galaxy.starCount; i++)
                    {
                        StarData starData = GameMain.galaxy.stars[i];
                        if (starData != null && i != Relic.starIndexWithMaxLuminosity)
                            starData.luminosity /= (float)Math.Pow(0.7, 0.33000001311302185); // 此处是除
                    }
                }

                for (int i = 0; i < GameMain.galaxy.starCount; i++)
                {
                    StarData starData = GameMain.galaxy.stars[i];
                    if (starData != null)
                        starData.luminosity = (float)(Math.Pow((Mathf.Round((float)Math.Pow((double)starData.luminosity, 0.33000001311302185) * 1000f) / 1000f + 1.0), 1.0 / 0.33000001311302185) - starData.luminosity);

                }
                //还要重新计算并赋值每个戴森球之前已初始化好的属性
                Relic.alreadyRecalcDysonStarLumin = false;
            }
            if (Relic.HaveRelic(4, 0)) // 无论如何只要有relic4-0都要计算一遍，因为，即使只是addrelic0-3，那么在其功能里已经还原过4-0了，因此还要再正向计算一次4-0把debuff加回来
            {
                float maxL = 0;
                for (int i = 0; i < GameMain.galaxy.starCount; i++)
                {
                    StarData starData = GameMain.galaxy.stars[i];
                    if ((starData != null) && (starData.luminosity > maxL))
                    {
                        maxL = starData.luminosity;
                        Relic.starIndexWithMaxLuminosity = i;
                    }
                }
                for (int i = 0; i < GameMain.galaxy.starCount; i++)
                {
                    StarData starData = GameMain.galaxy.stars[i];
                    if (starData != null && i != Relic.starIndexWithMaxLuminosity)
                        starData.luminosity *= (float)Math.Pow(0.7, 0.33000001311302185);
                }
                //还要重新计算并赋值每个戴森球之前已初始化好的属性
                Relic.alreadyRecalcDysonStarLumin = false;
            }
        }




        /// <summary>
        /// 每帧调用检查，不能在import的时候调用，会因为所需的DysonSphere是null而无法完成重新计算和赋值
        /// </summary>
        public static void TryRecalcDysonLumin()
        {
            if (!Relic.alreadyRecalcDysonStarLumin && (Relic.HaveRelic(0, 3) || Relic.HaveRelic(4, 0)))
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
                UIRelic.RefreshSlotsWindowUI();
            }
        }

        /// <summary>
        /// Relic 4-0 自动建造光度最高星系的巨构
        /// </summary>
        /// <param name="time"></param>
        public static void AutoBuildMegaOfMaxLuminStar(long time)
        {
            int timeStep = 2;
            if (GameMain.data.dysonSpheres.Length > Relic.starIndexWithMaxLuminosity && GameMain.data.dysonSpheres[Relic.starIndexWithMaxLuminosity] != null)
            {
                DysonSphere sphere = GameMain.data.dysonSpheres[Relic.starIndexWithMaxLuminosity];
                long energy = sphere.energyGenCurrentTick_Layers;
                if (energy > 16666666667) // 1T
                    timeStep = 4;
                else if (energy > 1000000000) // 60G
                    timeStep = 60;
                else if (energy > 16666667) // 1G
                    timeStep = 10;
                else
                    timeStep = 2;
            }
            if (Relic.HaveRelic(4, 0) && time % timeStep == 1)
            {
                Relic.AutoBuildMegaStructure(Relic.starIndexWithMaxLuminosity, 70, 30);
            }
        }

        /// <summary>
        /// relic 4-3
        /// </summary>
        public static void RefreshCargoAccIncTable()
        {
            double incMilliBonusByAP = SkillPoints.skillValuesL[6] / 100.0 * SkillPoints.skillLevelL[6];
            int incBonusByAP = (int)(incMilliBonusByAP * 1000);
            double accMilliBonusByAP = SkillPoints.skillValuesL[7] / 100.0 * SkillPoints.skillLevelL[7];
            int accBonusByAP = (int)(accMilliBonusByAP * 1000);
            double powerMilliBonusByAP = SkillPoints.skillValuesL[8] / 100.0 * SkillPoints.skillLevelL[8];
            int powerBonusByAP = (int)(powerMilliBonusByAP * 1000);

            if (Relic.HaveRelic(4, 3))
            {
                Cargo.incTable = new int[] { 0, 250, 300, 350, 400, 425, 450, 475, 500, 525, 550 };
                Cargo.incTableMilli = new double[] { 0.0, 0.250, 0.300, 0.350, 0.400, 0.425, 0.450, 0.475, 0.500, 0.525, 0.550 };
                Cargo.accTable = new int[] { 0, 200, 350, 500, 750, 1000, 1250, 1500, 1750, 2000, 2250 };
                Cargo.accTableMilli = new double[] { 0.0, 0.200, 0.350, 0.500, 0.750, 1.000, 1.250, 1.500, 1.750, 2.000, 2.250 };
            }
            else
            {
                Cargo.incTable = new int[] { 0, 125, 200, 225, 250, 275, 300, 325, 350, 375, 400 };
                Cargo.incTableMilli = new double[] { 0.0, 0.125, 0.200, 0.225, 0.250, 0.275, 0.300, 0.325, 0.350, 0.375, 0.400 };
                Cargo.accTable = new int[] { 0, 250, 500, 750, 1000, 1250, 1500, 1750, 2000, 2250, 2500 };
                Cargo.accTableMilli = new double[] { 0.0, 0.250, 0.500, 0.750, 1.000, 1.250, 1.500, 1.750, 2.000, 2.250, 2.500 };
            }

            Cargo.powerTable = new int[] { 0, 300, 700, 1100, 1500, 1900, 2300, 2700, 3100, 3500, 3900 };
            Cargo.powerTableRatio = new double[] { 1.0, 1.3, 1.7, 2.1, 2.5, 2.9, 3.3, 3.7, 4.1, 4.5, 4.9 };

            for (int i = 1; i < 4; i++)
            {
                Cargo.incTable[i] += incBonusByAP / 4 * i;
                Cargo.incTableMilli[i] += incMilliBonusByAP / 4.0 * i;
                Cargo.accTable[i] += accBonusByAP / 4 * i;
                Cargo.accTableMilli[i] += accMilliBonusByAP / 4.0 * i;
                Cargo.powerTable[i] += powerBonusByAP / 4 * i;
                Cargo.powerTableRatio[i] += powerMilliBonusByAP / 4.0 * i;
            }
            for (int i = 4; i < Cargo.incTable.Length; i++)
            {
                Cargo.incTable[i] += incBonusByAP;
                Cargo.incTableMilli[i] += incMilliBonusByAP;
                Cargo.accTable[i] += accBonusByAP;
                Cargo.accTableMilli[i] += accMilliBonusByAP;
                Cargo.powerTable[i] += powerBonusByAP;
                Cargo.powerTableRatio[i] += powerMilliBonusByAP;
            }
        }

        /// <summary>
        /// Relic 4-4 启迪回响自动建造恒星要塞，当巨构框架或节点被建造时。 已废弃，因为恒星要塞尚未完成
        /// </summary>
        /// <param name="__instance"></param>
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(DysonSphere), "ConstructSp")]
        //public static void AutoBuildStarFortressWhenNodeConstructed(ref DysonSphere __instance)
        //{
        //    if (Relic.HaveRelic(4, 4))
        //    {
        //        int starIndex = __instance.starData.index;
        //        List<int> needCompModules = new List<int>();
        //        for (int i = 0; i < 4; i++)
        //        {
        //            if (StarFortress.moduleComponentCount[starIndex][i] + StarFortress.moduleComponentInProgress[starIndex][i] < StarFortress.moduleMaxCount[starIndex][i] * StarFortress.compoPerModule[i])
        //                needCompModules.Add(i);
        //        }
        //        if (needCompModules.Count > 0)
        //        {
        //            int moduleIndex = needCompModules[Utils.RandInt(0, needCompModules.Count)];
        //            StarFortress.moduleComponentCount[starIndex][moduleIndex] += 1;
        //        }
        //    }
        //}


        // relic 4-4 击杀时调用的增加科研进度的方法，触发处在EventSystem的ZeroHp
        public static void AddNotDFTechHash(long hashPoint)
        {
            GameHistoryData history = GameMain.history;
            bool flag = true;
            int num = history.currentTech;
            if (num <= 0)
                return;
            TechProto techProto = LDB.techs.Select(num);
            for (int i = 0; i < techProto.Items.Length; i++) // 科学枢纽不支持研究黑雾矩阵相关科技
            {
                if (techProto.Items[i] == 5201)
                    return;
            }
            hashPoint = (long)(hashPoint * history.techSpeed); // 每级研究速度提供100%加成
            history.AddTechHash(hashPoint);
        }

        public static void RefreshRerollCost()
        {
            if (Relic.HaveRelic(4, 1))
                Relic.basicMatrixCost = (int)(0.5 * Relic.defaultBasicMatrixCost);
            else
                Relic.basicMatrixCost = Relic.defaultBasicMatrixCost;
        }


        // 原始relic 0-5 2-16 3-12 虚空荆棘（行星护盾部分）反弹伤害，各种护盾伤害减免和规避的逻辑，因为轰炸非护盾部位也会错误地反伤/减伤（原因是没有判断atfRayId，它决定交火点是否与护盾相交，而atfRayId是在DeterminePlanetATFieldRaytestInStar之后的其他方法进行设置的，没办法从这里获得），已被废弃
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(SkillSystem), "DeterminePlanetATFieldRaytestInStar")]
        //public static void ThornmailFieldAttckHandler(ref SkillSystem __instance, int starAstroId, ERayTestSkillType skillType, int skillId, int __result)
        //{
        //    if (__result > 0 && skillId > 0)
        //    {
        //        float factor = 1.0f;
        //        bool r0005 = Relic.HaveRelic(0, 5);
        //        bool r0216 = Relic.HaveRelic(2, 16);
        //        bool r0312 = Relic.HaveRelic(3, 12);
        //        if (r0312 && Relic.Verify(0.15))
        //        {
        //            factor = 0f;
        //        }
        //        if (r0005 || r0216 || factor < 1)
        //        {
        //            ref var _this = ref __instance;
        //            if (skillType == ERayTestSkillType.humpbackPlasma)
        //            {
        //                int cursor = _this.humpbackProjectiles.cursor;
        //                GeneralExpImpProjectile[] buffer = _this.humpbackProjectiles.buffer;
        //                if (skillId < cursor)
        //                {
        //                    ref GeneralExpImpProjectile ptr = ref buffer[skillId];
        //                    if (ptr.id == skillId)
        //                    {
        //                        if(r0216)
        //                        {
        //                            int atfAstroId = __result;
        //                            PlanetATField shield = __instance.astroFactories[atfAstroId].planetATField;
        //                            if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < ptr.damage)
        //                                factor *= 0.2f;
        //                            else
        //                                factor *= 0.8f;
        //                        }
        //                        if (r0005)
        //                        {
        //                            int realDamage = Relic.BonusDamage(ptr.damage * (1.0 - factor), 1);
        //                            __instance.DamageObject(realDamage, 1, ref ptr.caster, ref ptr.target);
        //                        }
        //                        ptr.damage = (int)(ptr.damage * factor);
        //                    }
        //                }
        //            }
        //            else if (skillType == ERayTestSkillType.lancerSpacePlasma)
        //            {
        //                int cursor = _this.lancerSpacePlasma.cursor;
        //                GeneralProjectile[] buffer = _this.lancerSpacePlasma.buffer;
        //                if (skillId < cursor)
        //                {
        //                    ref GeneralProjectile ptr = ref buffer[skillId];
        //                    if (ptr.id == skillId)
        //                    {
        //                        if (r0216)
        //                        {
        //                            int atfAstroId = __result;
        //                            PlanetATField shield = __instance.astroFactories[atfAstroId].planetATField;
        //                            if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < ptr.damage)
        //                                factor *= 0.2f;
        //                            else
        //                                factor *= 0.8f;
        //                        }
        //                        if (r0005)
        //                        {
        //                            int realDamage = Relic.BonusDamage(ptr.damage * (1.0 - factor), 1);
        //                            __instance.DamageObject(realDamage, 1, ref ptr.caster, ref ptr.target);
        //                        }
        //                        ptr.damage = (int)(ptr.damage * factor);
        //                    }
        //                }
        //            }
        //            else if (skillType == ERayTestSkillType.lancerLaserOneShot)
        //            {
        //                int cursor = _this.lancerLaserOneShots.cursor;
        //                SpaceLaserOneShot[] buffer = _this.lancerLaserOneShots.buffer;
        //                if (skillId < cursor)
        //                {
        //                    ref SpaceLaserOneShot ptr = ref buffer[skillId];
        //                    if (ptr.id == skillId)
        //                    {
        //                        if (r0216)
        //                        {
        //                            int atfAstroId = __result;
        //                            PlanetATField shield = __instance.astroFactories[atfAstroId].planetATField;
        //                            if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < ptr.damage)
        //                                factor *= 0.2f;
        //                            else
        //                                factor *= 0.8f;
        //                        }
        //                        if (r0005)
        //                        {
        //                            int realDamage = Relic.BonusDamage(ptr.damage * (1.0 - factor), 1);
        //                            __instance.DamageObject(realDamage, 1, ref ptr.caster, ref ptr.target);
        //                        }
        //                        ptr.damage = (int)(ptr.damage * factor);
        //                    }
        //                }
        //            }
        //            else if (skillType == ERayTestSkillType.lancerLaserSweep)
        //            {
        //                int cursor = _this.lancerLaserSweeps.cursor;
        //                SpaceLaserSweep[] buffer = _this.lancerLaserSweeps.buffer;
        //                if (skillId < cursor)
        //                {
        //                    ref SpaceLaserSweep ptr = ref buffer[skillId];
        //                    if (ptr.id == skillId && (ptr.lifemax - ptr.life) % ptr.damageInterval == 0)
        //                    {
        //                        if (r0216)
        //                        {
        //                            int atfAstroId = __result;
        //                            PlanetATField shield = __instance.astroFactories[atfAstroId].planetATField;
        //                            if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < ptr.damage)
        //                                factor *= 0.2f;
        //                            else
        //                                factor *= 0.8f;
        //                        }
        //                        if (r0005)
        //                        {
        //                            int realDamage = Relic.BonusDamage(ptr.damage * (1.0 - factor), 1);
        //                            SkillTarget emptyCaster;
        //                            emptyCaster.id = 0;
        //                            emptyCaster.type = ETargetType.None;
        //                            emptyCaster.astroId = starAstroId;
        //                            __instance.DamageObject(realDamage, 1, ref ptr.caster, ref emptyCaster);
        //                        }
        //                        ptr.damage = (int)(ptr.damage * factor);
        //                    }
        //                }
        //            }
        //            else if (skillType == ERayTestSkillType.spaceLaserSweep)
        //            {
        //                int cursor = _this.spaceLaserSweeps.cursor;
        //                SpaceLaserSweep[] buffer = _this.spaceLaserSweeps.buffer;
        //                if (skillId < cursor)
        //                {
        //                    ref SpaceLaserSweep ptr = ref buffer[skillId];
        //                    if (ptr.id == skillId)
        //                    {
        //                        if (r0216)
        //                        {
        //                            int atfAstroId = __result;
        //                            PlanetATField shield = __instance.astroFactories[atfAstroId].planetATField;
        //                            if (shield.energy / GameMain.history.planetaryATFieldEnergyRate / Relic.higherResistFactorDivisor < ptr.damage)
        //                                factor *= 0.2f;
        //                            else
        //                                factor *= 0.8f;
        //                        }
        //                        if (r0005)
        //                        {
        //                            int realDamage = Relic.BonusDamage(ptr.damage * (1.0 - factor), 1);
        //                            SkillTarget emptyCaster;
        //                            emptyCaster.id = 0;
        //                            emptyCaster.type = ETargetType.None;
        //                            emptyCaster.astroId = starAstroId;
        //                            __instance.DamageObject(realDamage, 1, ref ptr.caster, ref emptyCaster);
        //                        }
        //                        ptr.damage = (int)(ptr.damage * factor);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
