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


        public static bool Judge(double possibility)
        {
            if (Utils.RandDouble() < possibility)
                return true;
            else if ((relics[0] & 1 << 8) > 0) // 具有增加幸运的遗物，则可以再判断一次
                return (Utils.RandDouble() < possibility);

            return false;
        }

        public static double BonusDamage(double damage)
        {
            return damage;
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
            if (Configs.versionWhenImporting >= 30221107)
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

    class RelicData    
    {
        
    }
}
