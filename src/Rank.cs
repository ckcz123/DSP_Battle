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

        public static int autoConstructMegaStructureCountDown = 0; // relic3-17 逐渐推进巨构建造的剩余帧数倒数，无需存档，读档时设置为0

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
            if (rank < 10 && exp >= Configs.expToNextRank[rank])
            {
                Promotion();
            }
            int inc;
            if(GameMain.mainPlayer.package.TakeItem(8033, 1, out inc)>0)
            {
                AddExp(Configs.expPerAlienMeta);
                if (time % 60 == 10 && Relic.HaveRelic(2,1)) // relic2-1 互惠互利 上传元数据的时候建造巨构，每60帧建造12个点数。由于异星元数据很多，一般会连续上传数十秒，因此不怕判定点不在第10帧的微小损失
                {
                    Relic.AutoBuildMegaStructure();
                }
            }
            if (autoConstructMegaStructureCountDown > 0) // relic3-17 荣誉晋升每次提升功勋阶级显著推荐巨构建造进度
            {
                Relic.AutoBuildMegaStructure(-1, 120);
                autoConstructMegaStructureCountDown--;
            }
        }

        public static void AddExp(int num)
        {
            if (rank >= 10) return;
            int realExp = (int)(num * Configs.expRatioByDifficulty[Configs.difficulty + 1]);
            Interlocked.Add(ref exp, realExp);
        }

        public static void LoseHalfExp()
        {
            int loss = (int)(exp * 0.5); 
            Interlocked.Add(ref exp, -loss);
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
            if (Relic.HaveRelic(3, 17))
                autoConstructMegaStructureCountDown = 120;
            UIRank.ForceRefreshAll();
            UIRank.UIPromotionNotify();
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
            autoConstructMegaStructureCountDown = 0;
            UIRank.InitUI();
        }

        public static void IntoOtherSave()
        {
            rank = 0;
            exp = 0;
            autoConstructMegaStructureCountDown = 0;
            UIRank.InitUI();
        }


        
    }
}
