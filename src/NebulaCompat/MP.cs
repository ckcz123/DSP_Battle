using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    // 所有需要同步数值的方法调用MP.Sync(type填写需要同步的类型)。实际处理在PacketHandler对此方法的PostPatch中根据type不同而进行不同的数据发送处理。
    public class MP
    {
        public static bool clientBlocker = false;
        public static bool NebulaEnabled = false;

        public static bool Sync(EDataType type)
        {
            return true;
        }

        public static bool Sync(EDataType type, int[] values)
        {
            return true;
        }
        public static void InitBlocker() // Postpatch将在多人模式启用
        {
            NebulaEnabled = false;
            clientBlocker = false;
        }

    }

    public enum EDataType
    {
        None = 0,

        All = 1,
        Relic = 2,
        RelicRecorded = 3,
        RankFull = 5,
        RankRelatedInfo = 6,
        StarLumin = 7,
        ProtoPrefabDesc = 8,
        AssaultData = 9,
        SpaceSector = 10,
        SpaceSectorAndAssaultData = 11,


        AddRelic = 1021,
        RemoveRelic = 1022,
        RankExp = 1051,
        RankLevel = 1052,
        MechaInfo = 1080,
        HistoryData = 1090,
        HistoryDataDamage = 1091,
        HistoryDataMine = 1092,
        HistoryCallRefreshCargoTab = 1093, // not used
        HistoryDataDroneSpd = 1094,
        SpaceSectorAndAssaultDataThenSettle = 1111,



        // Never in Packet. Only used in MP.Sync(here)
        CallOnAddRelic = 9001,
        CallOnRemoveRelic = 9002,
        CallRankFull = 9003,
        CallOnAssaultInited = 9004,
        CallOnAssaultEndSettleStart = 9005, // AssaultEnd开始结算
        CallOnLaunchAllVoidAssault = 9006,
        CallOnAssaultStateSwitch = 9007,
    }
}
