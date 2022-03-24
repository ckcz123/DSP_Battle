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

        public static void Check(int num = 1, string str = "check ")
        {
            DspBattlePlugin.logger.LogInfo(str + num.ToString());
        }
    }
}
