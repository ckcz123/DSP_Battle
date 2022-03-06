using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace DSP_Battle
{
    class AddWormHoles
    {
        //下面是需要存档的信息
        public static int additionalBlackHoles = 5;

        

        public static void ExpandStarsArray()
        {
            //StarData[] newStarDatas = new StarData[GameMain.galaxy.starCount + addtionalBlackHoles];
            //Array.Copy(GameMain.galaxy.stars, 0, newStarDatas, 0, GameMain.galaxy.starCount);
            //GameMain.galaxy.stars = newStarDatas;
            //BlackHoleGen(GameMain.galaxy.starCount, addtionalBlackHoles);
            //GameMain.galaxy.stars[GameMain.galaxy.starCount] = GameMain.galaxy.stars[2];
            //GameMain.galaxy.stars[0].type = EStarType.BlackHole;
            //GameMain.galaxy.starCount = GameMain.galaxy.starCount + 1;
        }

        public static void BlackHoleGen(int beginIndex, int num)
        {
            for (int i = beginIndex; i < beginIndex + num; i++)
            {
                GameMain.galaxy.stars[i] = StarGen.CreateStar(GameMain.galaxy, new VectorLF3((Main.randSeed.NextDouble()-0.5) * 80000, (Main.randSeed.NextDouble()-0.5) * 40000, (Main.randSeed.NextDouble()-0.5) * 40000), i + 1, 19685, EStarType.BlackHole, ESpectrType.X);
                GameMain.galaxy.stars[i].planetCount = 0;
                GameMain.galaxy.stars[i].planets = new PlanetData[1];
                GameMain.galaxy.astroPoses[GameMain.galaxy.stars[i].id * 100].uPos = (GameMain.galaxy.astroPoses[GameMain.galaxy.stars[i].id * 100].uPosNext = GameMain.galaxy.stars[i].uPosition);
                GameMain.galaxy.astroPoses[GameMain.galaxy.stars[i].id * 100].uRot = (GameMain.galaxy.astroPoses[GameMain.galaxy.stars[i].id * 100].uRotNext = Quaternion.identity);
                GameMain.galaxy.astroPoses[GameMain.galaxy.stars[i].id * 100].uRadius = GameMain.galaxy.stars[i].physicsRadius;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "Import")]
        public static void GameDataImportPostPatch(ref GameData __instance)
        {
            __instance.gameDesc.starCount += additionalBlackHoles;
        }


        public static void ChangeIntoBlackHole()
        {
            int num = GameMain.galaxy.starCount;
            for (int i = num-additionalBlackHoles-1; i < num; i++)
            {
                GameMain.galaxy.stars[i].uPosition = new VectorLF3((Main.randSeed.NextDouble() - 0.5) * 8000, (Main.randSeed.NextDouble() - 0.5) * 4000, (Main.randSeed.NextDouble() - 0.5) * 4000);
                GameMain.galaxy.stars[i].position = GameMain.galaxy.stars[i].uPosition / 2400000.0;
                GameMain.galaxy.stars[i].type = EStarType.BlackHole;
                GameMain.galaxy.stars[i].planetCount = 0;
                GameMain.galaxy.stars[i].radius = 0.3f * GameMain.galaxy.stars[0].radius;
            }
            GameMain.galaxy.starCount -= additionalBlackHoles;
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(GalaxyData), "UpdatePoses")]
        //public static void UpdatePosesPatch(ref GalaxyData __instance, double time)
        //{
        //    for (int i = __instance.starCount; i < __instance.starCount + additionalBlackHoles; i++)
        //    {
        //        for (int j = 0; j < __instance.stars[i].planetCount; j++)
        //        {
        //            __instance.stars[i].planets[j].UpdateRuntimePose(time);
        //        }
        //    }
        //}


        public static void Export(BinaryWriter w)
        {
        }

        public static void Import(BinaryReader r)
        {
            //ChangeIntoBlackHole();
        }

        public static void IntoOtherSave()
        {
            //ChangeIntoBlackHole();
        }

    }
}
