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
                        starsWithMegaStructure.Add(i);
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

        public static double BonusDamage(double damage)
        {
            return damage;
        }

        // 有限制地建造某一(starIndex为-1时则是随机的)巨构的固定数量(amount)的进度，不因层数、节点数多少而改变一次函数建造的进度量
        public static void AutoBuildMegaStructure(int starIndex = -1, int amount = 1)
        {
            if (starsWithMegaStructure.Count <= 0) 
                return;
            if (starIndex < 0)
            {
                starIndex = starsWithMegaStructure[Utils.RandInt(0, starsWithMegaStructure.Count)];
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

   
}
