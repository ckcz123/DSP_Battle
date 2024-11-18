using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DSP_Battle
{
    public class UIEnemyBriefInfoPatcher
    {
        // modifier 2 额外护甲 UI显示
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEnemyBriefInfo), "_OnUpdate")]
        public static void UpdatePostPatch(ref UIEnemyBriefInfo __instance)
        {
            if (__instance.display)
            {
                if (__instance.frame %4 == 0 || __instance.frame <= 16)
                {
                    if (__instance.enemyInfo.prefabDesc != null && __instance.enemyInfo.enemyProto != null && __instance.enemyInfo.enemyProto.IsSpace && AssaultController.CheckHiveHasModifier(__instance.enemyInfo.astroId))
                    {
                        string finalText = "";
                        string realArmorText = "";
                        realArmorText = string.Concat(new string[]
                        {
                            ((float)__instance.enemyInfo.level * 0.5 + AssaultController.modifier[2]).ToString("0.#"),
                            __instance.greyColorPrefix,
                            " hp",
                            __instance.colorPostfix,
                            "\r\n"
                        });

                        string[] oriTexts = __instance.valuesText.text.Split('\n');
                        int armorIndex = 2;
                        for (int i = 0; i < oriTexts.Length; i++)
                        {
                            if(i == armorIndex)
                            {
                                finalText = string.Concat(new string[]
                                {
                                    finalText,
                                    realArmorText
                                });
                            }
                            else
                            {
                                finalText = string.Concat(new string[]
                                {
                                    finalText,
                                    oriTexts[i],
                                    "\n"
                                });
                            }
                        }
                        __instance.valuesText.text = finalText;
                    }
                }
            }
        }
    }
}
