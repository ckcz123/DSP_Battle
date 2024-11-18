using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    public class TCFVPerformanceMonitor
    {
        public static int MainLogic;
        public static int Assault;
        public static int MetaDrive;
        //public static int Factory;
        public static int Damage;
        public static int Kill;
        public static int Ballistic;
        public static int Droplet;
        public static int EventSys;
        public static int DrawCall;
        public static void Awake()
        {
            MainLogic = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF深空来敌", 1, 2);
            Assault = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF入侵逻辑", 2, MainLogic);
            MetaDrive = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF元驱动", 2, MainLogic);
            //Factory = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF工厂重写", -1, MetaDrive);
            Damage = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF伤害逻辑", -1, MetaDrive);
            Kill = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF击杀逻辑", -1, MetaDrive);
            Ballistic = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF弹道重写", -1, MetaDrive);
            Droplet = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF水滴", 2, MainLogic);
            EventSys = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF事件链", 2, MainLogic);
            DrawCall = MoreMegaStructure.PerformanceMonitorPatcher.AddCpuSampleLogic("PF深空绘制调用", 2, MainLogic);
        }
    }
}
