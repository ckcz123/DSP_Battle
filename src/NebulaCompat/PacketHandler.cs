using HarmonyLib;
using NebulaAPI;
using NebulaAPI.Interfaces;
using NebulaAPI.Networking;
using NebulaAPI.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle.src.NebulaCompat
{

    public class MPSyncPatcher
    {
        public static bool isHost
        {
            get
            {
                if (NebulaModAPI.MultiplayerSession?.LocalPlayer != null) 
                    return NebulaModAPI.MultiplayerSession.LocalPlayer.IsHost; 
                else 
                    return false;
            }
        }
        public static bool isClient
        {
            get
            {
                if (NebulaModAPI.MultiplayerSession?.LocalPlayer != null)
                    return NebulaModAPI.MultiplayerSession.LocalPlayer.IsClient;
                else
                    return false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MP), "Sync", new Type[] { typeof(EDataType) })]
        public static void SyncInceptor(EDataType type, ref bool __result)
        {
            __result = SyncData(type, null, null);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MP), "Sync", new Type[] { typeof(EDataType), typeof(int[]) })]
        public static void SyncInceptor(EDataType type, int[] values, ref bool __result)
        {
            __result = SyncData(type, values, null);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void SyncAfterGameTick(long time)
        {
            if (NebulaModAPI.MultiplayerSession?.LocalPlayer != null)
            {
                if (isHost)
                {
                    if (time % 60 == 0)
                    {
                        Synchronizer.BroadcastRankExp();
                    }
                }
            }
        }

        public static bool SyncData(EDataType type, int[] arrayInt, double[] arrayDouble)
        {
            if (NebulaModAPI.MultiplayerSession?.LocalPlayer != null)
            {
                if (type == EDataType.CallOnAddRelic)
                {
                    if (isClient) // 客机在新增元驱动时，告诉主机执行新增元驱动然后再由主机处理后续工作（即下发元驱动相关信息）
                    {
                        if (arrayInt != null && arrayInt.Length > 1)
                        {
                            int relicCode = arrayInt[0] * 100 + arrayInt[1];
                            Synchronizer.Send(EDataType.AddRelic, relicCode);
                        }
                    }
                    else // 若是主机，直接把元驱动相关信息同步给客机
                    {
                        Synchronizer.BroadcastRelic();
                    }
                }
                else if (type == EDataType.CallOnRemoveRelic)
                {
                    if (isClient) // 客机在移除元驱动时，告诉主机执行移除元驱动然后再由主机处理后续工作（即下发元驱动相关信息）
                    {
                        if (arrayInt != null && arrayInt.Length > 1)
                        {
                            int relicCode = arrayInt[0] * 100 + arrayInt[1];
                            Synchronizer.Send(EDataType.RemoveRelic, relicCode);
                        }
                    }
                    else // 若是主机，直接把元驱动相关信息同步给客机
                    {
                        Synchronizer.BroadcastRelic();
                    }
                }
                else if (type == EDataType.CallRankFull)
                {
                    if (isHost)
                        Synchronizer.BroadcastRankFull();
                }
                else if (type == EDataType.CallOnAssaultInited)
                {
                    if (isHost)
                    {
                        Synchronizer.BroadcastSpaceSectorAndAssaultData();
                    }
                }
                else if (type == EDataType.CallOnAssaultStateSwitch)
                {
                    if (isHost)
                    {
                        Synchronizer.BroadcastDSPSpaceSector();
                    }
                }
                else if (type == EDataType.CallOnLaunchAllVoidAssault)
                {
                    if (isHost)
                    {
                        Synchronizer.BroadcastSpaceSectorAndAssaultData();
                        Synchronizer.BroadcastHistory_DamageScales();
                    }
                }
                else if (type == EDataType.CallOnAssaultEndSettleStart)
                {
                    if (isHost)
                    {
                        Synchronizer.BroadcastSpaceSectorAndAssaultData();
                    }
                }
            }
            return true;
        }

    }

    [RegisterPacketProcessor]
    public class TCFVPacketProcessor : BasePacketProcessor<TCFVPacket>
    {
        public static Dictionary<EDataType, TCFVPacket> deferredPackets = new Dictionary<EDataType, TCFVPacket> (); // Packets that need to wait a sign before being processed.
        public static Dictionary<EDataType, int> deferredPacketProcessSign = new Dictionary<EDataType, int> (); // value is the time (seconds) before processing the packet with the key's EdataType in deferredPackets.

        public override void ProcessPacket(TCFVPacket packet, INebulaConnection conn)
        {
            EDataType type = (EDataType)packet.type;
            //Utils.Log($"received packet type {packet.type}");
            if (type == EDataType.All)
            {
                if (NebulaCompatPlugin.TCFVSave != null)
                {
                    IReaderProvider p = NebulaModAPI.GetBinaryReader(packet.dataBinary);
                    NebulaCompatPlugin.TCFVSave.Import(p.BinaryReader);
                }
            }
            else if (type == EDataType.AddRelic)
            {
                if (NebulaModAPI.MultiplayerSession.LocalPlayer.IsHost)
                {
                    Relic.AddRelic(packet.valueInt32 / 100, packet.valueInt32 % 100);
                    // Synchronizer.BroadcastRelic(); // 不需要再执行一遍broadcast了，因为上面的AddRelic完成时，会调用Sync(CallOnAddRelic)继而会因为是host而调用BroadcastRelic
                }
            }
            else if (type == EDataType.RemoveRelic)
            {
                if (NebulaModAPI.MultiplayerSession.LocalPlayer.IsHost)
                {
                    Relic.RemoveRelic(packet.valueInt32 / 100, packet.valueInt32 % 100);
                    // Synchronizer.BroadcastRelic();
                }
            }
            else if (type == EDataType.Relic)
            {
                Relic.relics[0] = packet.arrayInt32[0];
                Relic.relics[1] = packet.arrayInt32[1];
                Relic.relics[2] = packet.arrayInt32[2];
                Relic.relics[3] = packet.arrayInt32[3];
                Relic.relics[4] = packet.arrayInt32[4];
                Relic.trueDamageActive = packet.arrayInt32[5];
                Relic.resurrectCoinCount = packet.arrayInt32[6];
                UIRelic.RefreshSlotsWindowUI();
                Relic.RefreshConfigs();
            }
            else if (type == EDataType.RelicRecorded)
            {
                int count = packet.arrayInt32.Length;
                if (count > 0)
                {
                    Relic.recordRelics = new List<int>();
                    for (int i = 0; i < count; i++)
                    {
                        Relic.recordRelics.Add(packet.arrayInt32[i]);
                    }
                }
                UIRelic.RefreshSlotsWindowUI();
            }
            else if (type == EDataType.RankFull)
            {
                Rank.rank = packet.arrayInt32[0];
                Rank.exp = packet.arrayInt32[1];
                UIRank.ForceRefreshAll();
            }
            else if (type == EDataType.StarLumin)
            {
                int starCount = Math.Min(packet.arrayFloat.Length, GameMain.galaxy.starCount);
                for (int i = 0; i < starCount; i++)
                {
                    GameMain.galaxy.StarById(i + 1).luminosity = packet.arrayFloat[i];
                }
                Relic.alreadyRecalcDysonStarLumin = false;
            }
            else if (type == EDataType.RankExp)
            {
                Rank.exp = packet.valueInt32;
            }
            else if (type == EDataType.MechaInfo)
            {
                GameMain.mainPlayer.mecha.walkSpeed = packet.arrayFloat[0];
                GameMain.mainPlayer.mecha.replicateSpeed = packet.arrayFloat[1];
                GameMain.mainPlayer.mecha.energyShieldEnergyRate = (long)packet.arrayFloat[2];
            }
            else if (type == EDataType.HistoryDataDamage)
            {
                GameMain.history.kineticDamageScale = packet.arrayFloat[0];
                GameMain.history.blastDamageScale = packet.arrayFloat[1];
                GameMain.history.energyDamageScale = packet.arrayFloat[2];
                GameMain.history.magneticDamageScale = packet.arrayFloat[3];
            }
            else if (type == EDataType.HistoryDataMine)
            {
                GameMain.history.miningCostRate = packet.arrayFloat[0];
                GameMain.history.miningSpeedScale = packet.arrayFloat[1];
            }
            else if (type == EDataType.HistoryDataDroneSpd)
            {
                GameMain.history.constructionDroneSpeed = packet.valueFloat;
            }
            else if (type == EDataType.SpaceSectorAndAssaultData)
            {
                IReaderProvider p = NebulaModAPI.GetBinaryReader(packet.dataBinary);
                GameMain.spaceSector.Init(GameMain.data);
                GameMain.spaceSector.Import(p.BinaryReader);
                AssaultController.Import(p.BinaryReader);
            }
            else if (type == EDataType.SpaceSectorAndAssaultDataThenSettle)
            {
                IReaderProvider p = NebulaModAPI.GetBinaryReader(packet.dataBinary);
                GameMain.spaceSector.Init(GameMain.data);
                GameMain.spaceSector.Import(p.BinaryReader);
                AssaultController.Import(p.BinaryReader);
                AssaultController.OnAssaultEnd();
            }
            else if (type == EDataType.SpaceSector)
            {
                IReaderProvider p = NebulaModAPI.GetBinaryReader(packet.dataBinary);
                GameMain.spaceSector.Init(GameMain.data);
                GameMain.spaceSector.Import(p.BinaryReader);
            }
            else if (type == EDataType.AssaultData)
            {
                IReaderProvider p = NebulaModAPI.GetBinaryReader(packet.dataBinary);
                AssaultController.Import(p.BinaryReader);
            }
        }
    }
}

