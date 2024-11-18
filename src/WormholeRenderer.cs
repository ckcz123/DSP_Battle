using Compressions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xiaoye97;

namespace DSP_Battle
{
    public class WormholeRenderer
    {

        public static StarData[] starData = new StarData[32];
        public static StarSimulator[] simulator = new StarSimulator[32];
        public static bool[] simulatorActive = new bool[32];
        public static UIStarmapStar[] uiStar = new UIStarmapStar[32];

        private static Dictionary<StarSimulator, Material> bodyMaterialMap = new Dictionary<StarSimulator, Material>();


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "OnGameLoaded")]
        public static void UniverseSimulator_OnGameLoaded(ref UniverseSimulator __instance)
        {
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.DrawCall);
            for (int i = 0; i < 32; ++i)
            {
                if (simulator[i] != null) UnityEngine.Object.Destroy(simulator[i].gameObject);
            }

            CopyBlackHoleData();

            for (int i = 0; i < 32; ++i)
            {
                simulator[i] = UnityEngine.Object.Instantiate<StarSimulator>(__instance.starPrefab, __instance.transform);
                simulator[i].universeSimulator = __instance;
                simulator[i].SetStarData(starData[i]);
                simulator[i].gameObject.layer = 24;
                simulator[i].gameObject.name = "Wormhole_" + i;
                // simulator[i].gameObject.SetActive((Configs.combatState == 2 || Configs.combatState == 3) && i < Configs.nextWaveWormCount);
                simulator[i].gameObject.SetActive(false);
                simulator[i].bodyRenderer.gameObject.SetActive(false);
                simulator[i].massRenderer.gameObject.SetActive(false);
                simulator[i].atmosRenderer.gameObject.SetActive(false);
                simulator[i].effect.gameObject.SetActive(false);
                simulator[i].blackRenderer.gameObject.SetActive(false);
                simulator[i].gameObject.transform.localRotation = new Quaternion((float)Utils.RandDouble() - 0.5f, (float)Utils.RandDouble() - 0.5f, (float)Utils.RandDouble() - 0.5f, (float)Utils.RandDouble() - 0.5f);

            }
            lastWaveState = -1;
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
        }

        private static int lastWaveState = -1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "GameTick")]
        public static void UniverseSimulator_GameTick(ref UniverseSimulator __instance, double time)
        {
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.DrawCall);
            if (Configs.combatState == 0 || AssaultController.assaultHives == null || AssaultController.assaultHives.Count <= 0)
            {
                if (lastWaveState != Configs.combatState)
                {
                    for (var i = 0; i < 32; ++i)
                    {
                        simulator[i].gameObject.SetActive(false);
                        simulatorActive[i] = false;
                    }
                    lastWaveState = Configs.combatState;
                }
                MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
                MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
                return;
            }
            //if (lastWaveState != 2 && Configs.combatState == 2)
            //{
            //    WormholeProperties.InitWormholeProperties();
            //}

            Vector3 position = GameMain.mainPlayer.position;
            VectorLF3 uPosition = GameMain.mainPlayer.uPosition;
            Vector3 position2 = GameCamera.main.transform.position;
            Quaternion rotation = GameCamera.main.transform.rotation;

            for (var i = 0; i < AssaultController.assaultHives.Count; ++i)
            {
                switch (AssaultController.assaultHives[i].state)
                {
                    case EAssaultHiveState.Idle:
                    case EAssaultHiveState.End:
                    case EAssaultHiveState.Remove:
                        if (simulator[i].gameObject.activeSelf)
                        {
                            simulator[i].gameObject.SetActive(false);
                            simulatorActive[i] = false;
                        }
                        break;
                    case EAssaultHiveState.Expand:
                    case EAssaultHiveState.Assemble:
                    case EAssaultHiveState.Assault:
                        if (!simulator[i].gameObject.activeSelf)
                        {
                            simulator[i].gameObject.SetActive(true);
                            simulatorActive[i] = false;
                        }
                        if (AssaultController.assaultHives[i].hive != null)
                        {
                            simulator[i].starData.uPosition = GameMain.spaceSector.astros[AssaultController.assaultHives[i].byAstroIndex].uPos;
                            simulator[i].UpdateUniversalPosition(position, uPosition, position2, rotation);
                        }
                        break;
                    default:
                        break;
                }

                //if (AssaultController.assaultHives[0].state == EAssaultHiveState.Expand || AssaultController.assaultHives[0].state == EAssaultHiveState.Assemble)
                //{
                //    simulator[i].gameObject.SetActive(true);
                //    simulatorActive[i] = true;
                //}

            }

            lastWaveState = Configs.combatState;
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StarSimulator), "UpdateUniversalPosition")]
        public static void StarSimulator_UpdateUniversalPosition(ref StarSimulator __instance, Vector3 playerLPos, VectorLF3 playerUPos, Vector3 cameraPos, Quaternion cameraRot)
        {
            if (__instance.starData == null || __instance.starData.id != -1)
            {
                return;
            }

            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.DrawCall);
            Vector3 viewport = GameCamera.main.WorldToViewportPoint(__instance.transform.position);
            var distance = (__instance.starData.uPosition - playerUPos).magnitude;
            bool active = distance <= 40000 * 8 && (distance <= 2000 || (viewport.z > 0 && viewport.x > -0.1 && viewport.x < 1.1 && viewport.y > -0.1 && viewport.y < 1.1));

            if (__instance.starData.index >= 0 && active != simulatorActive[__instance.starData.index])
            {
                __instance.gameObject.SetActive(active);
                simulatorActive[__instance.starData.index] = active;
            }
            if (!active)
            {
                MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
                MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
                return;
            }

            float num4 = (float)(__instance.runtimeDist / 2400000.0);
            float num7 = 20f / (num4 + 3f);
            float num8 = __instance.starData.luminosity;
            if (num7 > 1f)
            {
                num7 = (float)Math.Log((double)num7) + 1f;
                num7 = (float)Math.Log((double)num7) + 1f;
            }
            if (num8 > 1f)
            {
                num8 = (float)Math.Log((double)num8) + 1f;
            }
            float num9 = num7 * num8;
            if (num9 < 1f)
            {
                num9 = num9 * 0.5f + 0.5f;
            }

            float num11 = __instance.visualScale * 6000f * __instance.starData.radius;
            float a = num11 * 100f;
            float b2 = num11 * 50f;
            float num15 = Mathf.InverseLerp(a, b2, (float)__instance.runtimeDist);

            if (!bodyMaterialMap.ContainsKey(__instance))
            {
                bodyMaterialMap.Add(__instance, AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "bodyMaterial"));
            }
            bodyMaterialMap[__instance].SetFloat("_Multiplier", 1f - num15);

            __instance.sunFlare.brightness *= num9;
            if (__instance.sunFlare.enabled != num9 > 0.001f)
            {
                __instance.sunFlare.enabled = (num9 > 0.001f);
            }

            __instance.blackRenderer.transform.localScale = Vector3.one * (__instance.solidRadius * 2f);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
        }

        private static int lastWaveState2 = -1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "CreateAllStarUIs")]
        public static void UIStarmap_CreateAllStarUIs(ref UIStarmap __instance)
        {
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.DrawCall);
            for (var i = 0; i < 32; ++i)
            {
                if (uiStar[i] != null)
                {
                    uiStar[i]._Destroy();
                    UnityEngine.Object.Destroy(uiStar[i].gameObject);
                }
            }

            CopyBlackHoleData();

            for (var i = 0; i < 32; ++i)
            {
                uiStar[i] = UnityEngine.Object.Instantiate(__instance.starUIPrefab, __instance.starUIPrefab.transform.parent);
                uiStar[i]._Create();
                uiStar[i]._Init(starData[i]);
                uiStar[i].gameObject.name = "WormholeUI_" + i;
                uiStar[i].gameObject.SetActive(false);
                uiStar[i].transform.SetAsFirstSibling();
            }

            lastWaveState2 = -1;
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);

        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(UIStarmap), "_OnUpdate")] // 注释掉这个是因为星图ui界面的黑洞会挡住黑雾巢穴的鼠标选取，即使更改gameobject的顺序也没办法
        public static void UIStarmap_OnUpdate(ref UIStarmap __instance)
        {
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.DrawCall);
            if (AssaultController.assaultHives != null && AssaultController.assaultHives.Count >= 0)
            {
                for (int i = 0; i < AssaultController.assaultHives.Count; i++)
                {

                    switch (AssaultController.assaultHives[i].state)
                    {
                        case EAssaultHiveState.Idle:
                        case EAssaultHiveState.End:
                        case EAssaultHiveState.Remove:
                            if (uiStar[i].starObject.gameObject.activeSelf)
                            {
                                uiStar[i]._Close();
                                uiStar[i].starObject.gameObject.SetActive(false);
                            }
                            break;
                        case EAssaultHiveState.Expand:
                        case EAssaultHiveState.Assemble:
                        case EAssaultHiveState.Assault:
                            if (!uiStar[i].starObject.gameObject.activeSelf)
                            {
                                uiStar[i]._Open();
                                uiStar[i].starObject.gameObject.SetActive(true);
                            }
                            uiStar[i]._Update();
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    if (uiStar[i].starObject.gameObject.activeSelf)
                    {
                        uiStar[i]._Close();
                        uiStar[i].starObject.gameObject.SetActive(false);
                    }
                }

            }
            
            lastWaveState2 = Configs.combatState;
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnLateUpdate")]
        public static void UIStarmap_OnLateUpdate(ref UIStarmap __instance)
        {
            if (Configs.combatState == 0 || AssaultController.assaultHives == null || AssaultController.assaultHives.Count <= 0) return;

            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.DrawCall);
            for (var i = 0; i < AssaultController.assaultHives.Count; ++i) uiStar[i].starObject._LateUpdate();
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
        }

        private static void CopyBlackHoleData()
        {
            if (starData[0] != null) return;

            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.MainLogic);
            MoreMegaStructure.MMSCPU.BeginSample(TCFVPerformanceMonitor.DrawCall);
            StarData data = GameMain.galaxy.stars.Where(e => e.type == EStarType.BlackHole).First();
            for (var i = 0; i < 32; ++i)
            {
                starData[i] = data.Copy();
                starData[i].planetCount = 0;
                starData[i].planets = new PlanetData[] { };
                starData[i].id = -1;
                starData[i].index = i;
                starData[i].radius = 1f;
            }
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.DrawCall);
            MoreMegaStructure.MMSCPU.EndSample(TCFVPerformanceMonitor.MainLogic);
        }
    }
}
