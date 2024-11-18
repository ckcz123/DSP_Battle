using HarmonyLib;
using System;
using System.IO;
using System.Threading;

namespace DSP_Battle
{
    public class Rank
    {
        public static int rank = 0;
        public static int exp = 0; 
        public static int maxRankHistory = 0; // 历史达到过的最高功勋阶级。升到某级已经给过这一级（以及之前）的授权点了，下次再升到此等级（或以下等级）则不再给授权点。

        public static long lastHash = -9999; // -9999代表
        public static int hash2ExpDivisor = 100; // 每这么多hash点数提供1经验值
        public static int linearThreshold = 20; // 通过提升每帧hash数，以线性方式获得的exp提升上限为20exp/帧，超出的部分每帧开平方加到20上，作为最终此帧的exp获取量
        public static bool nextEnqueueExpTech = false;
        public static void InitRank()
        {
            rank = 0;
            exp = 0;
        }

        public static void InitBeforeLoad()
        {
            maxRankHistory = 0;
            lastHash = -hash2ExpDivisor - 1;
            nextEnqueueExpTech = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RankGameTick(ref GameData __instance, long time)
        {
            //检查升级，因为AddExp有多线程调用，为了避免问题不在每次AddExp后检查升级，而是每帧检查
            if (exp >= Configs.expToNextRank[rank])
            {
                Promotion();
            }
            CheckExpTechUploadHash();
            //int inc;
            //if(GameMain.mainPlayer.package.TakeItem(8033, 1, out inc)>0)
            //{
            //    AddExp(Configs.expPerAlienMeta);
            //}

        }

        public static void AddExp(int num)
        {
            if (rank > 10) return;
            int realExp = num;
            if (Relic.HaveRelic(3, 17)) // relic 3-17
            {
                if(Relic.HaveRelic(0, 9))
                    realExp = (int)(realExp * 1.4);
                else
                    realExp = (int)(realExp * 1.25);
            }
            Interlocked.Add(ref exp, realExp);
        }

        private static void Promotion()
        {
            Interlocked.Add(ref exp, -Configs.expToNextRank[rank]);
            if (rank < 10)
            {
                rank += 1;
                rank = rank > 0 ? rank : 0;
                // SkillPoints.totalPoints = Math.Max(SkillPoints.totalPoints, SkillPoints.spMinByRank[rank]);
                if (rank > maxRankHistory)
                {
                    SkillPoints.totalPoints += SkillPoints.spGainByRank[rank];
                    maxRankHistory = rank;
                }
                UIRank.UIPromotionNotify();
                if (Relic.HaveRelic(2, 1)) // relic 2-1
                    Interlocked.Add(ref Relic.autoConstructMegaStructureCountDown, rank * rank * rank * 60);
            }
            else
            {
                SkillPoints.totalPoints += SkillPoints.spGainFullLevel;
                UIRank.UIPointsGainNotify();
                if (Relic.HaveRelic(2, 1)) // relic 2-1
                    Interlocked.Add(ref Relic.autoConstructMegaStructureCountDown, rank * 2 * 60);
            }

            if (rank >= 10 && !GameMain.data.history.ItemUnlocked(9513))
            {
                GameMain.mainPlayer.TryAddItemToPackage(9513, 1, 0, true);
                GameMain.data.history.UnlockSpecialItem(9513);
                Utils.UIItemUp(9513, 1, 200);
            }
            if (rank == 3)
            {
                GameMain.data.mainPlayer.mecha.walkSpeed += 4;
            }
            else if (rank == 4)
            {
                //GameMain.history.miningCostRate *= 0.8f;
                SkillPoints.RefreshMiningConsumption();
                if (EventSystem.recorder == null || EventSystem.recorder.protoId == 0) // 第一次升到四级固定获取元驱动
                {
                    if (EventSystem.neverStartTheFirstEvent > 0)
                        EventSystem.InitNewEvent();
                }
            }
            else if (rank == 5)
            {
                GameMain.history.magneticDamageScale += 0.2f;
            }
            else if (rank == 7)
            {
                //    GameMain.history.miningCostRate /= 0.8f;
                //    GameMain.history.miningCostRate *= 0.6f;
                SkillPoints.RefreshMiningConsumption();
            }
            else if (rank == 10)
            {
                //GameMain.history.miningCostRate /= 0.6f;
                //GameMain.history.miningCostRate *= 0.2f;
                SkillPoints.RefreshMiningConsumption();
            }
            if (rank > 8 && Utils.RandDouble() < 0.3)
            {
                if (EventSystem.recorder == null || EventSystem.recorder.protoId == 0)
                {
                    EventSystem.InitNewEvent();
                }
            }
            MP.Sync(EDataType.CallRankFull);
            UIRank.ForceRefreshAll();
        }

        public static void DownGrade(bool clearExp = true)
        {
            if (clearExp)
                Interlocked.Exchange(ref exp, 0);
            if (rank > 0)
            {
                if (rank == 3)
                {
                    GameMain.data.mainPlayer.mecha.walkSpeed -= 4;
                }
                else if (rank == 4)
                {
                    //GameMain.history.miningCostRate /= 0.8f;
                }
                else if (rank == 5)
                {
                    GameMain.history.magneticDamageScale -= 0.2f;
                }
                else if (rank == 7)
                {
                    //GameMain.history.miningCostRate /= 0.6f;
                    //GameMain.history.miningCostRate *= 0.8f;
                }
                else if (rank == 10)
                {
                    //GameMain.history.miningCostRate /= 0.2f;
                    //GameMain.history.miningCostRate *= 0.6f;
                }
                rank -= 1;
                SkillPoints.RefreshMiningConsumption();
                UIRank.ForceRefreshAll();
            }
            MP.Sync(EDataType.CallRankFull);
        }

        // 下面为提供算力科技不断提供经验值的patch
        /// <summary>
        /// 此科技完成时设置一个标志，用来重新将此科技加入队尾
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__state"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameHistoryData), "DequeueTech")]
        public static bool CatchExpTechWhenDequeue(ref GameHistoryData __instance)
        {
            if (__instance.currentTech == 1998)
            {
                nextEnqueueExpTech = true;
            }
            return true;
        }

        // 检查科技哈希点数的获取以及进行经验获取，检测到科技解锁时，重置
        public static void CheckExpTechUploadHash()
        {
            if (GameMain.data.history.techStates.ContainsKey(1998))
            {
                TechState techState = default(TechState);
                techState = GameMain.data.history.techStates[1998];
                long nowHash = techState.hashUploaded;
                if (lastHash > -hash2ExpDivisor - 1) // 说明不是刚载入游戏的状态，这样做每次载入游戏会丢失一帧的经验值，为了避免存档lastHash，无所谓
                {
                    if (nowHash < lastHash)
                    {
                        lastHash = -(LDB.techs.Select(1998).GetHashNeeded(0) - lastHash);
                    }
                    long hashChanged = nowHash - lastHash;
                    if (hashChanged > hash2ExpDivisor)
                    {
                        lastHash += AddExpByHash(hashChanged) * hash2ExpDivisor;
                    }
                }
                else
                {
                    lastHash = nowHash;
                }
                if (techState.unlocked || techState.curLevel > 0)
                {
                    techState.unlocked = false;
                    techState.curLevel = 0;
                    techState.hashUploaded = 0L;
                    TechProto tp = LDB.techs.Select(1998);
                    techState.hashNeeded = tp.GetHashNeeded(0);
                    GameMain.data.history.techStates[1998] = techState;
                }
                if (nextEnqueueExpTech)
                {
                    GameMain.data.history.EnqueueTech(1998);
                    nextEnqueueExpTech = false;
                }
            }
            else
            {
                lastHash = 0;
            }
        }

        public static int AddExpByHash(long hashChanged)
        {
            int expGain = (int)(hashChanged / hash2ExpDivisor);
            int realExpGain = expGain;
            if (realExpGain > linearThreshold)
            {
                realExpGain = linearThreshold + (int)Math.Sqrt(realExpGain - linearThreshold);
            }
            AddExp(realExpGain);
            return expGain;
        }

        public static void Export(BinaryWriter w)
        {
            w.Write(rank);
            w.Write(exp);
            w.Write(maxRankHistory);
        }

        public static void Import(BinaryReader r)
        {
            InitBeforeLoad();
            if (Configs.versionWhenImporting >= 30220420)
            {
                rank = r.ReadInt32();
                exp = r.ReadInt32();
            }
            else
            {
                InitRank();
            }
            try
            {
                if (rank >= 10 && !GameMain.data.history.ItemUnlocked(9513))
                {
                    GameMain.mainPlayer.TryAddItemToPackage(9513, 1, 0, true);
                    GameMain.data.history.UnlockSpecialItem(9513);
                    Utils.UIItemUp(9513, 1, 200);
                }
            }
            catch (Exception) { }

            if(Configs.versionWhenImporting >= 30240708)
            {
                maxRankHistory = r.ReadInt32();
            }
            else
            {
                maxRankHistory = rank;
            }

            UIRank.InitUI();
        }

        public static void IntoOtherSave()
        {
            InitBeforeLoad();
            rank = 0;
            exp = 0;
            lastHash = -hash2ExpDivisor - 1;
            UIRank.InitUI();
        }
    }
}
