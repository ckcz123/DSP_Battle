using crecheng.DSPModSave;
using NebulaAPI;
using NebulaAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle.src.NebulaCompat
{
    public class Synchronizer
    {
        public static void BroadcastRelic()
        {
            int[] relicData = new int[7];
            relicData[0] = Relic.relics[0];
            relicData[1] = Relic.relics[1];
            relicData[2] = Relic.relics[2];
            relicData[3] = Relic.relics[3];
            relicData[4] = Relic.relics[4];
            relicData[5] = Relic.trueDamageActive;
            relicData[6] = Relic.resurrectCoinCount;
            Send(EDataType.Relic, relicData);

            if (Relic.recordRelics != null && Relic.recordRelics.Count > 0)
            {
                int[] recordedRelicData = new int[Relic.recordRelics.Count];
                for (int i = 0; i < Relic.recordRelics.Count; i++)
                {
                    recordedRelicData[i] = Relic.recordRelics[i];
                }
                Send(EDataType.RelicRecorded, recordedRelicData);
            }

            float[] starLumin = new float[GameMain.galaxy.starCount];
            for (int i = 0; i < GameMain.galaxy.starCount; i++)
            {
                starLumin[i] = GameMain.galaxy.StarById(i + 1).luminosity;
            }
            Send(EDataType.StarLumin, starLumin);

            BroadcastHistory_DamageScales(); // 已被Rank包含
            BroadcastMechaInfo();
        }

        public static void BroadcastRankExp()
        {
            Send(EDataType.RankExp, Rank.exp);
        }

        //public static void BroadcastRankLevel()
        //{

        //}

        // 只有在升降级时调用
        public static void BroadcastRankFull()
        {
            int[] rankData = new int[] { Rank.rank, Rank.exp };
            Send(EDataType.RankFull, rankData);
            BroadcastHistory_Mining();
            BroadcastHistory_DamageScales();
            BroadcastMechaInfo();
            BroadcastSkillPoints();
        }

        public static void BroadcastSkillPoints()
        {

        }

        public static void BroadcastMechaInfo()
        {
            float[] mechaInfo = new float[] { GameMain.mainPlayer.mecha.walkSpeed, GameMain.mainPlayer.mecha.replicateSpeed, GameMain.mainPlayer.mecha.energyShieldEnergyRate };
            Send(EDataType.MechaInfo, mechaInfo);
        }

        public static void BroadcastHistory_DamageScales()
        {
            float[] damageScales = new float[] { GameMain.history.kineticDamageScale, GameMain.history.blastDamageScale, GameMain.history.energyDamageScale, GameMain.history.magneticDamageScale };
            Send(EDataType.HistoryDataDamage, damageScales);
        }

        public static void BroadcastHistory_Mining()
        {
            float[] miningInfo = new float[] { GameMain.history.miningCostRate, GameMain.history.miningSpeedScale };
            Send(EDataType.HistoryDataMine, miningInfo);
        }

        public static void BroadcastHistory_DroneSpeed()
        {
            float spd = GameMain.history.constructionDroneSpeed;
            Send(EDataType.HistoryDataDroneSpd, spd);
        }

        // 不需要，因为客机会在每次收到relic信息后，执行refreshConfig，如果有增产剂相关的元驱动，会自行计算出结果
        public static void BroadcastCargoTable(){}

        public static void BroadcastProtoPrefabDesc()
        {

        }

        public static void BroadcastAssaultData()
        {
            IWriterProvider p = NebulaModAPI.GetBinaryWriter();
            AssaultController.Export(p.BinaryWriter);
            Send(EDataType.AssaultData, p.CloseAndGetBytes());
        }
        public static void BroadcastSpaceSectorAndAssaultData()
        {
            IWriterProvider p = NebulaModAPI.GetBinaryWriter();
            GameMain.spaceSector.BeginSave();
            GameMain.spaceSector.Export(p.BinaryWriter);
            GameMain.spaceSector.EndSave();
            AssaultController.Export(p.BinaryWriter);
            Send(EDataType.SpaceSectorAndAssaultData, p.CloseAndGetBytes());
        }

        public static void BroadcastSpaceSectorAndAssaultDataWhenAssaultEnd()
        {
            IWriterProvider p = NebulaModAPI.GetBinaryWriter();
            GameMain.spaceSector.BeginSave();
            GameMain.spaceSector.Export(p.BinaryWriter);
            GameMain.spaceSector.EndSave();
            AssaultController.Export(p.BinaryWriter);
            Send(EDataType.SpaceSectorAndAssaultDataThenSettle, p.CloseAndGetBytes());
        }

        public static void BroadcastTCFVAll()
        {
            IWriterProvider p = NebulaModAPI.GetBinaryWriter();
            if(NebulaCompatPlugin.TCFVSave != null)
            {
                NebulaCompatPlugin.TCFVSave.Export(p.BinaryWriter);
                Send(EDataType.All, p.CloseAndGetBytes());
            }
        }


        public static void BroadcastDSPSpaceSector()
        {
            IWriterProvider p = NebulaModAPI.GetBinaryWriter();
            GameMain.spaceSector.BeginSave();
            GameMain.spaceSector.Export(p.BinaryWriter);
            GameMain.spaceSector.EndSave();
            Send(EDataType.SpaceSector, p.CloseAndGetBytes());
        }

        internal static void Send(EDataType type, int data)
        {
            NebulaModAPI.MultiplayerSession.Network.SendPacket(new TCFVPacket(type, data));
        }
        internal static void Send(EDataType type, int[] data)
        {
            NebulaModAPI.MultiplayerSession.Network.SendPacket(new TCFVPacket(type, data));
        }
        internal static void Send(EDataType type, float data)
        {
            NebulaModAPI.MultiplayerSession.Network.SendPacket(new TCFVPacket(type, data));
        }
        internal static void Send(EDataType type, float[] data)
        {
            NebulaModAPI.MultiplayerSession.Network.SendPacket(new TCFVPacket(type, data));
        }
        internal static void Send(EDataType type, double data)
        {
            NebulaModAPI.MultiplayerSession.Network.SendPacket(new TCFVPacket(type, data));
        }
        internal static void Send(EDataType type, double[] data)
        {
            NebulaModAPI.MultiplayerSession.Network.SendPacket(new TCFVPacket(type, data));
        }
        internal static void Send(EDataType type, byte[] data)
        {
            NebulaModAPI.MultiplayerSession.Network.SendPacket(new TCFVPacket(type, data));
        }
    }
}
