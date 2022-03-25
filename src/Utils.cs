using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    public class Utils
    {
        static Random randSeed = new Random();

        public static VectorLF3 RandPosDelta()
        {
            return new VectorLF3(randSeed.NextDouble() - 0.5, randSeed.NextDouble() - 0.5, randSeed.NextDouble() - 0.5);
        }

        public static int RandInt(int min, int max)
        {
            return randSeed.Next(min, max);
        }

        public static void Check(int num = 1, string str = "check ")
        {
            DspBattlePlugin.logger.LogInfo(str + num.ToString());
        }

        public static void Log(string str, int isWarning = 0)
        {
            if(isWarning>0)
            {
                DspBattlePlugin.logger.LogWarning(str);
            }
            else
            {
                DspBattlePlugin.logger.LogInfo(str);
            }
        }

        
    }
}
