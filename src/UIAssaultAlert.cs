using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace DSP_Battle
{
    public class UIAssaultAlert
    {
        // UIDarkfogMonitor OrganizeTargetList可能是用于排序黑雾威胁程度用来显示的
        // color
        // frame 和 frameTint 是主条的框色，可以理解为frame为框色的底层色，frameTint从左往右在上加盖颜色，且从左到右覆盖率逐渐降低，逐渐过渡为很偏frame本身的颜色
        // icon 和 iconTint 为左侧小图标主题色，对于地面黑雾基地的图标
        // threatBar 紫色候选 0.689 0.34 1 0.585 、 0.773 0.330 0.836 0.305
        // 一个候选的对应frame 和 frameTint的颜色 frame 0.601 0.018 0.018 0.809   frameTint 0.321 0.088 0.82 0.609


        public static Color frameColor0 = new Color(0.601f, 0.28f, 0.78f, 0.8f);
        public static Color frameTintColor0 = new Color(0.051f, 0.088f, 0.42f, 0.609f);
        
        public static Color frameColor1 = new Color(0.91f, 0.48f, 0.918f, 1f);
        public static Color frameTintColor1 = new Color(0.9f, 0.28f, 0.5f, 1f);
        
        public static Color threatBarColor = new Color(0.773f, 0.330f, 0.836f, 0.305f);
        public static Color threatBarColor2 = new Color(0.4f, 0.0f, 1f, 0.9f);

        public static Color iconFrameColor = new Color(0.6f, 0.4f, 1f, 1f);

        public static Color bgImageColor = new Color(0.2f, 0f, 0.7f, 0.8f);

        public static Color oriGuideLineColor = new Color(0.9434f, 0.1843f, 0.1646f, 1f);
        public static Color voidGuideLineColor = new Color(0.75f, 0.2f, 1f, 1f);

        public static bool showAmount = false; // 如果有一个DarkFogMonitorEntry 的鼠标放入了 threatBar, 所有巢穴都要显示数量而非倒计时


        // 虚空入侵巢穴 返回极高的威胁度
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIDarkFogMonitor), "UrgentValue")]
        public static bool UrgentValuePostfix(object obj, ref float __result)
        {
            __result = -1f;
            DFGBaseComponent dfgbaseComponent = obj as DFGBaseComponent;
            EnemyDFHiveSystem enemyDFHiveSystem = obj as EnemyDFHiveSystem;
            if (dfgbaseComponent != null && dfgbaseComponent.id != 0)
            { 
                return true;
            }
            else
            {
                int byAstroIndex = enemyDFHiveSystem.hiveAstroId - 1000000;
                if (byAstroIndex >= 0 && byAstroIndex < AssaultController.alertHives.Length)
                {
                    if (AssaultController.alertHives[byAstroIndex] >= 0)
                    {
                        int hiveListIndex = AssaultController.alertHives[byAstroIndex];
                        if (hiveListIndex >= 0 && hiveListIndex < AssaultController.assaultHives.Count)
                        {
                            __result = int.MaxValue - Math.Max(AssaultController.assaultHives[hiveListIndex].timeTillAssault, 0);
                            return false;
                        }
                    }
                }
                else if (byAstroIndex >= 0 && byAstroIndex >= AssaultController.alertHives.Length && byAstroIndex < GameMain.spaceSector.maxHiveCount)
                {
                    DspBattlePlugin.logger.LogWarning("Sbnormal byAstroIndex in UIAssaultAlert.UrgentValuePostfix");
                }
            }
            return true;
        }

        // 将其他恒星系的 虚空入侵中的巢穴 加入左上角显示列表
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDarkFogMonitor), "OrganizeTargetList")]
        public static void OrganizeTargetListPostfix(ref UIDarkFogMonitor __instance)
        {
            List<object> targetsExt = new List<object>();
            List<double> targetsUrgentsExt = new List<double>();
            for (int i = 0; i < AssaultController.assaultHives.Count; i++)
            {
                AssaultHive ah = AssaultController.assaultHives[i];
                int starIndex = ah.starIndex;
                if (__instance.localStar == null || __instance.localStar.index != starIndex)
                {
                    if (!__instance.hives.Contains(ah.hive))
                    {
                        __instance.hives.Add(ah.hive);
                        targetsExt.Add(ah.hive);
                    }
                }
            }
            if (targetsExt.Count > 0)
            {
                foreach (object target in targetsExt)
                {
                    targetsUrgentsExt.Add(UIDarkFogMonitor.UrgentValue(target));
                }

                for (int i = 0; i < __instance.targets.Count; i++)
                {
                    targetsExt.Add(__instance.targets[i]);
                    targetsUrgentsExt.Add(__instance.targetsUrgents[i]);
                }
                __instance.targets = targetsExt;
                __instance.targetsUrgents = targetsUrgentsExt;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDarkFogMonitor), "RefreshEntries")]
        public static void RefreshEntriesPostfix()
        {
            showAmount = false;
        }


        // 左上角巢穴列表 虚空入侵巢穴专属颜色
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDarkFogMonitorEntry), "RefreshValuesComplete")]
        public static void DFMonitorEntryRefreshPostPatch(ref UIDarkFogMonitorEntry __instance)
        {
            if(__instance.targetHive != null)
            {
                int byAstroIndex = __instance.targetHive.hiveAstroId - 1000000;
                if(byAstroIndex >= 0 && byAstroIndex < GameMain.spaceSector.maxHiveCount && AssaultController.alertHives[byAstroIndex] >= 0)
                {
                    int listIndex = AssaultController.alertHives[byAstroIndex];
                    if (listIndex >= 0 && listIndex < AssaultController.assaultHives.Count)
                    {
                        AssaultHive ah = AssaultController.assaultHives[listIndex];
                        __instance.expBar.fillAmount = 0;
                        EAssaultHiveState state = ah.state;

                        // 进度条填充比例
                        float barFill;
                        if (state == EAssaultHiveState.Expand || state == EAssaultHiveState.Assemble)
                        {
                            barFill = (ah.timeTotalInit - ah.timeTillAssault) / (ah.timeTotalInit + 0.0000001f);
                        }
                        else if (state == EAssaultHiveState.Assault)
                        {
                            barFill = 1f;
                        }
                        else
                        {
                            barFill = 0f;
                        }
                        barFill = barFill > 1 ? 1f : barFill;
                        barFill = barFill < 0 ? 0 : barFill;
                        float xP = -130f + Mathf.Round(barFill * 100f);
                        if (Mathf.Abs(__instance.threatBar.rectTransform.anchoredPosition.x - xP) > 0.5f)
                        {
                            __instance.threatBar.rectTransform.anchoredPosition = new Vector2(xP, __instance.threatBar.rectTransform.anchoredPosition.y);
                        }

                        // 设置颜色
                        if (state == EAssaultHiveState.Assault)
                        {
                            __instance.threatBar.gameObject.SetActive(true);
                            __instance.threatBarFull.gameObject.SetActive(true);
                            __instance.iconFrameImage.gameObject.SetActive(true);
                            __instance.frameImage.color = frameColor1;
                            __instance.frameTintImage.color = frameTintColor1;
                            __instance.threatBar.color = threatBarColor2;
                            __instance.iconFrameImage.color = iconFrameColor;
                            __instance.bgImage.color = bgImageColor;
                        }
                        else if(state == EAssaultHiveState.Assemble && ah.timeTillAssault < 3600 * 5)
                        {
                            __instance.threatBar.gameObject.SetActive(true);
                            __instance.threatBarFull.gameObject.SetActive(false);
                            __instance.iconFrameImage.gameObject.SetActive(true);
                            __instance.frameImage.color = frameColor1;
                            __instance.frameTintImage.color = frameTintColor1;
                            __instance.threatBar.color = threatBarColor;
                            __instance.iconFrameImage.color = iconFrameColor;
                            __instance.bgImage.color = bgImageColor;
                        }
                        else
                        {
                            __instance.threatBar.gameObject.SetActive(true);
                            __instance.threatBarFull.gameObject.SetActive(false);
                            __instance.iconFrameImage.gameObject.SetActive(false);
                            __instance.frameImage.color = frameColor0;
                            __instance.frameTintImage.color = frameTintColor0;
                            __instance.threatBar.color = threatBarColor;
                            __instance.bgImage.color = bgImageColor;
                        }
                        

                        if ((__instance.thrtButton._isPointerEnter || showAmount) && state != EAssaultHiveState.Assault) // 鼠标悬停时显示进攻数量
                        {
                            __instance.threatValueText.text = string.Format("虚空入侵数量".Translate(), ah.assaultNum);
                        }
                        else // 不悬停时 显示倒计时
                        {

                            if (state == EAssaultHiveState.Expand)
                            {
                                __instance.threatValueText.text = string.Format("虚空入侵扩张中".Translate(), ah.time / 60 / 60, ah.time / 60 % 60);
                            }
                            else if (state == EAssaultHiveState.Assemble)
                            {
                                __instance.threatValueText.text = string.Format("虚空入侵集结中".Translate(), ah.time / 60 / 60, ah.time / 60 % 60);
                            }
                            else if (state == EAssaultHiveState.Assault)
                            {
                                __instance.threatValueText.text = string.Format("虚空入侵进攻中".Translate(), ah.time / 60 / 60, ah.time / 60 % 60, ah.hive.totalIncomingAssaultingUnitCount);
                            }

                            if (__instance.thrtButton.tips.tipTitle != "削弱入侵标题")
                            {
                                __instance.thrtButton.tips.tipTitle = "削弱入侵标题";
                                __instance.thrtButton.tips.tipText = "削弱入侵内容";

                                __instance.thrtButton.tips.width = 345;
                                __instance.thrtButton.tips.corner = 3;

                                if (AssaultController.assaultHives[listIndex].isSuper)
                                {
                                    string modifierDesc ="\n\n" + "虚空入侵额外特性提示".Translate() + AssaultController.GetModifierDesc();
                                    __instance.thrtButton.tips.tipText = "削弱入侵内容".Translate() + modifierDesc;
                                }

                                __instance.thrtButton.UpdateTip();
                            }
                        }
                    }
                    else
                    {
                        // 还原
                        if (__instance.thrtButton.tips.tipTitle != "威胁度")
                        {
                            __instance.thrtButton.tips.tipTitle = "威胁度";
                            __instance.thrtButton.tips.tipText = "威胁度解释";
                            __instance.thrtButton.tips.width = 168;
                            __instance.thrtButton.tips.corner = 2;
                            __instance.thrtButton.UpdateTip();
                        }
                    }
                }
                else
                {
                    // 还原
                    if (__instance.thrtButton.tips.tipTitle != "威胁度")
                    {
                        __instance.thrtButton.tips.tipTitle = "威胁度";
                        __instance.thrtButton.tips.tipText = "威胁度解释";
                        __instance.thrtButton.tips.width = 168;
                        __instance.thrtButton.tips.corner = 2;
                        __instance.thrtButton.UpdateTip();
                    }
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDarkFogMonitorEntry), "_OnLateUpdate")]
        public static void EntryLateUpdate(ref UIDarkFogMonitorEntry __instance)
        {
            if(__instance.guideLine != null && __instance.targetHive != null)
            {
                int byAstroIndex = __instance.targetHive.hiveAstroId - 1000000;
                if (byAstroIndex >= 0 && byAstroIndex < GameMain.spaceSector.maxHiveCount && AssaultController.alertHives[byAstroIndex] >= 0)
                {
                    __instance.guideLine.color = voidGuideLineColor;
                }
                else
                {
                    __instance.guideLine.color = oriGuideLineColor;
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDarkFogMonitor), "RefreshEntries")]
        public static void CheckIfMouseIn(ref UIDarkFogMonitor __instance)
        {
            showAmount = false;
            if (__instance.hiveEntries != null)
            {
                for (int i = 0; i < __instance.hiveEntries.Count; i++)
                {
                    if (__instance.hiveEntries[i] != null)
                    {
                        UIDarkFogMonitorEntry entry = __instance.hiveEntries[i];
                        if (entry.targetHive != null)
                        {
                            int byAstroIndex = entry.targetHive.hiveAstroId - 1000000;
                            if (byAstroIndex >= 0 && byAstroIndex < GameMain.spaceSector.maxHiveCount && AssaultController.alertHives[byAstroIndex] >= 0)
                            {
                                int listIndex = AssaultController.alertHives[byAstroIndex];
                                if (listIndex >= 0 && listIndex < AssaultController.assaultHives.Count)
                                {
                                    if(entry.thrtButton._isPointerEnter)
                                    {
                                        showAmount = true;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(UIDarkFogMonitorEntry), "SetColorTheme0")]
        //public static bool SetColor0Test(ref UIDarkFogMonitorEntry __instance, bool showThreatBar)
        //{
        //    __instance.threatBar.gameObject.SetActive(showThreatBar);
        //    __instance.threatBarFull.gameObject.SetActive(false);
        //    __instance.iconFrameImage.gameObject.SetActive(false);
        //    return false;
        //    if (__instance.bgImage.color != __instance.monitor.bgColor0)
        //    {
        //        __instance.bgImage.color = new UnityEngine.Color(0.8f,0.8f,0f);
        //        __instance.frameImage.color = new UnityEngine.Color(0.8f, 0f, 0f);
        //        __instance.frameTintImage.color = new UnityEngine.Color(0.0f, 0f, 0.9f);
        //        __instance.threatBar.color = new UnityEngine.Color(0.8f, 0.0f, 0.4f);
        //        __instance.iconImage.color = new UnityEngine.Color(0f, 0f, 0f);
        //        __instance.iconTintImage.color = __instance.monitor.icontColor0;
        //    }
        //    return false;
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(UIDarkFogMonitorEntry), "SetColorTheme1")]
        //public static bool SetColor1Test(ref UIDarkFogMonitorEntry __instance)
        //{
        //    __instance.threatBar.gameObject.SetActive(true);
        //    __instance.threatBarFull.gameObject.SetActive(false);
        //    __instance.iconFrameImage.gameObject.SetActive(true);
        //    if (__instance.bgImage.color != __instance.monitor.bgColor0)
        //    {
        //        __instance.bgImage.color = __instance.monitor.bgColor0;
        //        __instance.frameImage.color = __instance.monitor.frameColor0;
        //        __instance.frameTintImage.color = __instance.monitor.frametColor0;
        //        __instance.threatBar.color = __instance.monitor.tbarColor0;
        //        __instance.iconImage.color = __instance.monitor.iconColor0;
        //        __instance.iconTintImage.color = __instance.monitor.icontColor0;
        //    }
        //    return false;
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(UIDarkFogMonitorEntry), "SetColorTheme2")]
        //public static bool SetColor2Test(ref UIDarkFogMonitorEntry __instance)
        //{
        //    __instance.threatBar.gameObject.SetActive(true);
        //    __instance.threatBarFull.gameObject.SetActive(false);
        //    __instance.iconFrameImage.gameObject.SetActive(true);
        //    if (__instance.bgImage.color != __instance.monitor.bgColor0)
        //    {
        //        __instance.bgImage.color = __instance.monitor.bgColor0;
        //        __instance.frameImage.color = __instance.monitor.frameColor0;
        //        __instance.frameTintImage.color = __instance.monitor.frametColor0;
        //        __instance.threatBar.color = __instance.monitor.tbarColor0;
        //        __instance.iconImage.color = __instance.monitor.iconColor0;
        //        __instance.iconTintImage.color = __instance.monitor.icontColor0;
        //    }
        //    return false;
        //}
    }
}
