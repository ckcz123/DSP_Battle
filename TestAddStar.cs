using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    class TestAddStar
    {

        public static void ExpandStarsArray()
        {
            StarData[] newStarDatas = new StarData[GameMain.galaxy.starCount + 50];
            Array.Copy(GameMain.galaxy.stars, 0, newStarDatas, 0, GameMain.galaxy.starCount);
            GameMain.galaxy.stars = newStarDatas;
            GameMain.galaxy.starCount = GameMain.galaxy.starCount + 50;
        }

        public static void BlackHoleGen()
        {

        }


        public static void Export(BinaryWriter w)
        {
        }

        public static void Import(BinaryReader r)
        {
            ExpandStarsArray();
        }

        public static void IntoOtherSave()
        {
            ExpandStarsArray();
        }

    }
}
