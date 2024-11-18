using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    public class UIHiveNamePatcher
    {
        public static Color UIStarmapVoidHiveColor = new Color(1, 0.3655f, 1f, 0.811f);
        public static Color UIStarmapNormalHiveColor = new Color(1, 0.3655f, 0.2972f, 0.811f);
        // 普通界面 普通巢穴颜色 1 0.861 0.466 1  星图界面则是 1 0.3655 0.2972 0.811

        public static Color UISpaceGuideVoidHiveColor = new Color(1, 0.3655f, 1f, 0.811f);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmapDFHive), "_OnLateUpdate")]
        public static void UIStarmapDFHiveLateUpdate(ref UIStarmapDFHive __instance)
        {
            bool isVoidHive = false;
            if(__instance.hive != null)
            {
                int byAstroId = __instance.hive.hiveAstroId - 1000000;
                if(byAstroId >= 0 && byAstroId < AssaultController.alertHives.Length)
                {
                    isVoidHive = AssaultController.alertHives[byAstroId] >= 0;
                }
            }
            if (__instance.nameText != null)
            {
                if (isVoidHive)
                {
                    __instance.nameText.color = UIStarmapVoidHiveColor;
                }
                else
                {
                    __instance.nameText.color = UIStarmapNormalHiveColor;
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UISpaceGuideEntry), "_OnLateUpdate")]
        public static void UISpaeGuideEntryLateUpdate(ref UISpaceGuideEntry __instance)
        {
            if (__instance.guideType == ESpaceGuideType.DFHive)
            {
                bool isVoidHive = false;
                int byAstroId = __instance.objId - 1000000;
                if (byAstroId >= 0 && byAstroId < AssaultController.alertHives.Length)
                {
                    isVoidHive = AssaultController.alertHives[byAstroId] >= 0;
                }
                bool flag4 = (__instance.indicatorButton.highlighted = (__instance.player.navigation.indicatorAstroId == __instance.objId));
                float num = (__instance.rpos - __instance.playerTrans.localPosition).magnitude - __instance.radius;
                if (isVoidHive)
                {
                    Color color = UISpaceGuideVoidHiveColor;
                    float num13 = 24f - __instance.player.warpState * 9f;
                    float num14 = 1f;
                    if (!__instance.mouseIn && !flag4)
                    {
                        num14 = Mathf.InverseLerp((float)((double)num13 * 2400000.0), 9600000f, num);
                        num14 = Mathf.Pow(num14, 2f);
                    }
                    float num15 = 0.15f - 0.15f * __instance.player.warpState;
                    color.a = num15 + num14 * (1f - num15);
                    __instance.nameText.color = color;
                    __instance.distText.color = color;
                    __instance.updateColor = false;

                    __instance.markIcon.color = UISpaceGuideVoidHiveColor;
                }
                else
                {
                    if (__instance.updateCounter % 15 == 0 || (double)__instance.player.warpState > 0.001 || __instance.mouseIn || __instance.updateColor)
                    {
                        Color color =__instance.enemyTextColor;
                        float num13 = 24f - __instance.player.warpState * 9f;
                        float num14 = 1f;
                        if (!__instance.mouseIn && !flag4)
                        {
                            num14 = Mathf.InverseLerp((float)((double)num13 * 2400000.0), 9600000f, num);
                            num14 = Mathf.Pow(num14, 2f);
                        }
                        float num15 = 0.15f - 0.15f * __instance.player.warpState;
                        color.a = num15 + num14 * (1f - num15);
                        __instance.nameText.color = color;
                        __instance.distText.color = color;
                        __instance.updateColor = false;

                        __instance.markIcon.color = __instance.enemyColor;
                    }

                }
            }
        }

    }
}
