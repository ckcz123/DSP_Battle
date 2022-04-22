using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace DSP_Battle
{
    public class Rank
    {
        public static int rank = 0;
        public static int exp = 0;

        public static void InitRank()
        {
            rank = 0;
            exp = 0;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RankGameTick(ref GameData __instance, long time)
        {
            //检查升级，因为AddExp有多线程调用，为了避免问题不在每次AddExp后检查升级，而是每帧检查
            if (rank < 10 && exp > Configs.expToNextRank[rank])
            {
                Promotion();
            }
        }

        public static void AddExp(int num)
        {
            if (rank >= 10) return;
            int realExp = (int)(num * Configs.expRatioByDifficulty[Configs.difficulty + 1]);
            Interlocked.Add(ref exp, realExp);
        }

        private static void Promotion()
        {
            Interlocked.Add(ref exp, -Configs.expToNextRank[rank]);
            rank += 1;
            if (Configs.extraSpeedEnabled) //如果正处在奖励中升级，则刷新一下新增的奖励内容，防止奖励结束时计算出错
            {
                if (rank == 3)
                {
                    GameMain.history.miningCostRate *= 0.8f;
                }
                else if (rank == 5)
                {
                    WaveStages.ResetCargoAccIncTable(true);
                }
                else if (rank == 7)
                {
                    GameMain.history.miningCostRate *= 0.625f;
                }
            }
            UIRank.ForceRefreshAll();
            UIRank.UIPromotionNotify();
        }

        public static string RankInfoText()
        {
            string res = "";



            return res;
        }


        public static void Export(BinaryWriter w)
        {
            w.Write(rank);
            w.Write(exp);
        }

        public static void Import(BinaryReader r)
        {
            if (Configs.versionWhenImporting >= 30220420)
            {
                rank = r.ReadInt32();
                exp = r.ReadInt32();
            }
            else
            {
                InitRank();
            }
            UIRank.InitUI();
        }

        public static void IntoOtherSave()
        {
            rank = 0;
            exp = 0;
            UIRank.InitUI();
        }


        //下面测试用

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGeneralTips), "OnTechUnlocked")]
        public static bool TechUnlockNotifyPatch(ref UIGeneralTips __instance, int techId, int level)
        {
            return true;
        }
    }
}
