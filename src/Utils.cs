using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    class Utils
    {
        public static VectorLF3 RandPosDelta()
        {
            return new VectorLF3(DspBattlePlugin.randSeed.NextDouble() - 0.5, DspBattlePlugin.randSeed.NextDouble() - 0.5, DspBattlePlugin.randSeed.NextDouble() - 0.5);
        }
    }
}
