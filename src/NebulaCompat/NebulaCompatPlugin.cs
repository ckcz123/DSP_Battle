using BepInEx;
using NebulaAPI;
using NebulaAPI.Interfaces;
using NebulaAPI.Networking;
using NebulaAPI.Packets;
using crecheng.DSPModSave;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;
using HarmonyLib;
using System.IO;
using NebulaAPI.GameState;

namespace DSP_Battle.src.NebulaCompat
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("dsp.nebula-multiplayer")]
    [BepInDependency(NebulaModAPI.API_GUID)]
    [BepInDependency("crecheng.DSPModSave")]
    [BepInDependency(DspBattlePlugin.GUID)]
    public class NebulaCompatPlugin : BaseUnityPlugin, IMultiplayerMod
    {
        public const string GUID = "Gnimaerd.DSP.plugin.TCMVNebulaCompat";
        public const string NAME = "TCMVNebulaCompat";
        public const string VERSION = "0.0.1";

        public static IModCanSave TCFVSave;

        public string Version => VERSION;

        public bool CheckVersion(string hostVersion, string clientVersion)
        {
            return hostVersion.Equals(clientVersion);
        }

        public void Awake()
        {
            NebulaModAPI.RegisterPackets(Assembly.GetExecutingAssembly());
            Harmony.CreateAndPatchAll(typeof(NebulaCompatPlugin));
            Harmony.CreateAndPatchAll(typeof(MPSyncPatcher));
            NebulaModAPI.OnPlayerJoinedGame += playerData => { OnPlayerJoined(); };
        }

        public void Update()
        {
            if (NebulaModAPI.MultiplayerSession?.LocalPlayer != null)
            {
                if (NebulaModAPI.MultiplayerSession.LocalPlayer.IsClient)
                    MP.clientBlocker = true;
                else
                    MP.clientBlocker = false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(VFPreload), nameof(VFPreload.InvokeOnLoadWorkEnded))]
        public static void InitAfterVFPreLoad()
        {
            if (!BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue(DspBattlePlugin.GUID, out var pluginInfo))
            {
                return;
            }
            Assembly assembly = pluginInfo.Instance.GetType().Assembly;

            try
            {
                TCFVSave = pluginInfo.Instance as IModCanSave;
            }
            catch 
            {
                Utils.Log("Error when init TCFVSave");
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(MP), nameof(MP.InitBlocker))]
        public static void MPBlockerInitPostfix()
        {
            if (NebulaModAPI.MultiplayerSession?.LocalPlayer != null)
            {
                MP.NebulaEnabled = true;
                if (NebulaModAPI.MultiplayerSession.LocalPlayer.IsClient)
                    MP.clientBlocker = true;
                else
                    MP.clientBlocker = false;
            }
        }

        public static void InitWhenLoad()
        {
            TCFVPacketProcessor.deferredPackets = new Dictionary<EDataType, TCFVPacket> ();
            TCFVPacketProcessor.deferredPacketProcessSign = new Dictionary<EDataType, int> ();
        }

        public static void OnPlayerJoined()
        {
            if (NebulaModAPI.MultiplayerSession?.LocalPlayer != null)
            {
                if (NebulaModAPI.MultiplayerSession.LocalPlayer.IsHost)
                    Synchronizer.BroadcastTCFVAll();
            }
        }

        public void Export(BinaryWriter w)
        {

        }

        public void Import(BinaryReader r)
        {
            InitWhenLoad();
        }
    }
}
