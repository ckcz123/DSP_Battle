using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DSP_Battle
{
    public class SkillPoints
    {
        // 需要存档的数据
        public static int totalPoints = 0;
        public static int[] skillLevelL = new int[30]; // 左列已分配的点数
        public static int[] skillLevelR = new int[30];

        // 参数
        public static int skillCountL = 11;
        public static int skillCountR = 9;
        public static List<int> spMinByRank = new List<int> { 0, 1, 3, 6, 10, 15, 21, 27, 37, 52, 72, 72, 72 }; // 满级前，sp不会低于
        public static List<int> spGainByRank = new List<int> { 0, 1, 2, 3, 4, 5, 6, 8, 10, 15, 20, 0, 0 }; // 首次升到该等级给予的授权点数
        public static int spGainFullLevel = 5; // 满级后每次升级给的点数
        public static List<int> skillMaxLevelL = new List<int> { 10, 200, 200, 400, 100, 1000, 100, 200, 100, 100, 100, 999, 999, 999 };
        public static List<int> skillMaxLevelR = new List<int> { 1000, 200, 200, 100, 400, 100, 400, 90, 1000, 200, 200, 200, 200, 200 };
        public static List<float> skillValuesL = new List<float> { 1f, 10f, 1f, 0.5f, -0.2f, 1f, 0.1f, 0.5f, -1f, 2f, 2f, 999f };
        public static List<float> skillValuesR = new List<float> { 1f, 0.25f, 0.2f, 0.3f, 1f, 2f, 1f, -1f, 2f, 999f, 999f, 999f };
        public static List<string> skillSuffixesL = new List<string> { "m/s", "%", "", "m/s", "%", "%", "%", "%", "%", "%", "%" }; // 文本后缀
        public static List<string> skillSuffixesR = new List<string> { "%", "%", "", "%", "%", "%", "%", "%", "MW", "%", "%" };

        // 不需要存档
        public static float relic0WeightBuff = 0f;
        public static float relic1WeightBuff = 0f;

        public static float globalDamageRate = 1.0f;
        public static float criticalRate = 0f;
        public static int armorPenetration = 0; // 已经乘过100了，存储在此处的值
        public static float IcarusShieldEvade = 0f;
        public static float voidDamageRate = 1.0f;
        public static float dropletEnergyPunishmentRate = 1.0f;

        public static bool isFullLevel = false;

        // 0.94 ** (tech[3606].curLevel-1) 是游戏默认的采矿消耗率，如果3601-3605均已解锁，否则，按照解锁的最后一个计算
        // 行星力场护盾 能量效率 2000初始（反编译4000，哪里变的呢？不知道） 1818 1600 1379 1176 1000 科技 5701-5705 10% 15% 20% 25% 30%

        public static void InitBeforeLoad()
        {
            UISkillPointsWindow.InitAll();
            totalPoints = 0;
            skillLevelL = new int[skillCountL];
            for (int i = 0; i < skillLevelL.Length; i++)
            {
                skillLevelL[i] = 0;
            }
            skillLevelR = new int[skillCountR];
            for (int i = 0; i < skillLevelR.Length; i++)
            {
                skillLevelR[i] = 0;
            }
            UISkillPointsWindow.ClearTempLevelAdded();
            relic0WeightBuff = 0f;
            relic1WeightBuff = 0f;
            globalDamageRate = 1.0f;
            criticalRate = 0f;
            armorPenetration = 0;
            IcarusShieldEvade = 0f;
            voidDamageRate = 1.0f;
            dropletEnergyPunishmentRate = 1.0f;
        }

        public static void InitAfterLoad()
        {
            // SkillPoints.totalPoints = Math.Max(SkillPoints.totalPoints, SkillPoints.spMinByRank[Rank.rank]);
            RefreshCargoAccIncTable();
            RefreshInGameData();
        }

        public static int UnusedPoints()
        {
            return totalPoints - skillLevelL.Sum() - skillLevelR.Sum();
        }

        public static void ConfirmAll()
        {
            for (int i = 0; i < skillCountL; i++)
            {
                int allocatePoints = UISkillPointsWindow.tempLevelAddedL[i];
                if (allocatePoints > 0)
                {
                    skillLevelL[i] += allocatePoints;
                    switch (i)
                    {
                        case 0:
                            GameMain.data.mainPlayer.mecha.walkSpeed += skillValuesL[i] * allocatePoints;
                            break;
                        case 1:
                            GameMain.data.mainPlayer.mecha.replicateSpeed += skillValuesL[i] / 100.0f * allocatePoints;
                            break;
                        case 2:
                            GameMain.data.mainPlayer.mecha.constructionModule.droneCount += (int)skillValuesL[i] * allocatePoints;
                            GameMain.data.mainPlayer.mecha.constructionModule.droneAliveCount += (int)skillValuesL[i] * allocatePoints;
                            GameMain.data.mainPlayer.mecha.constructionModule.droneIdleCount += (int)skillValuesL[i] * allocatePoints;
                            break;
                        case 3:
                            GameMain.data.history.constructionDroneSpeed += skillValuesL[i] * allocatePoints;
                            break;
                        case 4:
                            RefreshMiningConsumption();
                            break;
                        case 5:
                            GameMain.data.history.miningSpeedScale += skillValuesL[i] / 100.0f * allocatePoints;
                            break;
                        case 6:
                        case 7:
                        case 8:
                            RefreshCargoAccIncTable();
                            break;
                        case 9:
                        case 10:
                            RefreshInGameData();
                            break;
                        default:
                            break;
                    }
                }
            }
            for (int i = 0; i < skillCountR; i++)
            {
                int allocatePoints = UISkillPointsWindow.tempLevelAddedR[i];
                if (allocatePoints > 0)
                {
                    skillLevelR[i] += allocatePoints;
                    switch (i)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 5:
                        case 6:
                        case 7:
                            RefreshInGameData();
                            break;
                        case 4:
                            GameMain.data.mainPlayer.mecha.energyShieldEnergyRate = (long)(500 / (1.0f + skillValuesR[i] / 100.0f * skillLevelR[i]));
                            break;
                        default:
                            break;
                    }
                }
            }
            UISkillPointsWindow.ClearTempLevelAdded();
        }
        public static void ResetAll()
        {
            for (int i = 0; i < skillLevelL.Length; i++)
            {
                int allocatedPoints = skillLevelL[i];
                skillLevelL[i] = 0;
                if (allocatedPoints > 0)
                {
                    switch (i)
                    {
                        case 0:
                            GameMain.data.mainPlayer.mecha.walkSpeed -= skillValuesL[i] * allocatedPoints;
                            break;
                        case 1:
                            GameMain.data.mainPlayer.mecha.replicateSpeed -= skillValuesL[i] / 100.0f * allocatedPoints;
                            break;
                        case 2:
                            GameMain.data.mainPlayer.mecha.constructionModule.droneCount -= (int)skillValuesL[i] * allocatedPoints;
                            GameMain.data.mainPlayer.mecha.constructionModule.droneAliveCount -= (int)skillValuesL[i] * allocatedPoints;
                            GameMain.data.mainPlayer.mecha.constructionModule.droneIdleCount -= (int)skillValuesL[i] * allocatedPoints;
                            break;
                        case 3:
                            GameMain.data.history.constructionDroneSpeed -= skillValuesL[i] * allocatedPoints;
                            break;
                        case 4:
                            RefreshMiningConsumption();
                            break;
                        case 5:
                            GameMain.data.history.miningSpeedScale -= skillValuesL[i] / 100.0f * allocatedPoints;
                            break;
                        case 6:
                        case 7:
                        case 8:
                            RefreshCargoAccIncTable();
                            break;
                        case 9:
                            relic0WeightBuff = 0;
                            break;
                        case 10:
                            relic1WeightBuff = 0;
                            break;
                        default:
                            break;
                    }
                }
            }
            for (int i = 0; i < skillLevelR.Length; i++)
            {
                skillLevelR[i] = 0;
            }
            GameMain.data.mainPlayer.mecha.energyShieldEnergyRate = 500;
            RefreshInGameData();
        }
        public static void RefreshMiningConsumption()
        {
            float byResearh = 1.0f;
            GameHistoryData history = GameMain.data.history;
            if (history.techStates.ContainsKey(3606) && history.techStates.ContainsKey(3605) && history.techStates[3605].unlocked)
            {
                byResearh = (float)Math.Pow(0.94, history.techStates[3606].curLevel - 1);
            }
            else if (history.TechState(3605).unlocked)
            {
                byResearh = (float)Math.Pow(0.94, 5);
            }
            else if (history.TechState(3604).unlocked)
            {
                byResearh = (float)Math.Pow(0.94, 4);
            }
            else if (history.TechState(3603).unlocked)
            {
                byResearh = (float)Math.Pow(0.94, 3);
            }
            else if (history.TechState(3602).unlocked)
            {
                byResearh = (float)Math.Pow(0.94, 2);
            }
            else if (history.TechState(3601).unlocked)
            {
                byResearh = 0.94f;
            }

            float byAp = 1.0f;
            if (Rank.rank >= 10)
                byAp = 0.2f;
            else if (Rank.rank >= 7)
                byAp = 0.6f;
            else if (Rank.rank >= 4)
                byAp = 0.8f;

            byAp += skillValuesL[4] / 100.0f * skillLevelL[4];
            if (byAp < 0)
                byAp = 0;
            
            history.miningCostRate = byResearh * byAp;
        }

        public static void RefreshCargoAccIncTable()
        {
            RelicFunctionPatcher.RefreshCargoAccIncTable();
        }

        public static void RefreshInGameData()
        {
            globalDamageRate = 1.0f + skillValuesR[0] / 100.0f * skillLevelR[0];
            criticalRate = skillValuesR[1] / 100.0f * skillLevelR[1];
            armorPenetration = (int)(skillValuesR[2] * 100 * skillLevelR[2]);
            IcarusShieldEvade = skillValuesR[3] / 100.0f * skillLevelR[3];
            voidDamageRate = 1.0f + skillValuesR[6] / 100.0f * skillLevelR[6];
            dropletEnergyPunishmentRate = 1.0f + skillValuesR[7] / 100.0f * skillLevelR[7];

            GameHistoryData history = GameMain.data.history;
            float eff = 1.0f;
            if (history.techStates.ContainsKey(5705) && history.techStates[5705].unlocked)
                eff = 2.0f;
            else if (history.techStates.ContainsKey(5704) && history.techStates[5704].unlocked)
                eff = 1.7f;
            else if (history.techStates.ContainsKey(5703) && history.techStates[5703].unlocked)
                eff = 1.45f;
            else if (history.techStates.ContainsKey(5702) && history.techStates[5702].unlocked)
                eff = 1.25f;
            else if (history.techStates.ContainsKey(5701) && history.techStates[5701].unlocked)
                eff = 1.1f;

            eff += skillValuesR[5] / 100.0f * skillLevelR[5];
            history.planetaryATFieldEnergyRate = (long)(2000 / eff + 0.5);

            relic0WeightBuff = skillValuesL[9] / 100.0f * skillLevelL[9];
            relic1WeightBuff = skillValuesL[10] / 100.0f * skillLevelL[10];

        }


        public static void Export(BinaryWriter w)
        {
            w.Write(totalPoints);
            w.Write(skillLevelL.Length);
            for (int i = 0; i < skillLevelL.Length; i++)
            {
                w.Write(skillLevelL[i]);
            }
            w.Write(skillLevelR.Length);
            for (int i = 0; i < skillLevelR.Length; i++)
            {
                w.Write(skillLevelR[i]);
            }
        }

        public static void Import(BinaryReader r)
        {
            InitBeforeLoad();
            if (Configs.versionWhenImporting >= 30240320)
            {
                totalPoints = r.ReadInt32();
                int countL = r.ReadInt32();
                skillLevelL = new int[skillCountL];
                for (int i = 0; i < countL; i++)
                {
                    if (i < skillCountL)
                        skillLevelL[i] = r.ReadInt32();
                    else
                        _ = r.ReadInt32();
                }
                int countR = r.ReadInt32();
                skillLevelR = new int[skillCountR];
                for (int i = 0; i < countR; i++)
                {
                    if (i < skillCountR)
                        skillLevelR[i] = r.ReadInt32();
                    else
                        _ = r.ReadInt32();
                }
            }
            InitAfterLoad();
        }

        public static void IntoOtherSave()
        {
            InitBeforeLoad();
        }
    }
}
