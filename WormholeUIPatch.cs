using xiaoye97;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    class WormholeUIPatch
    {

        public static StarData starData;
        public static StarSimulator simulator;
        public static UIStarmapStar uiStar;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "OnGameLoaded")]
        public static void UniverseSimulator_OnGameLoaded(ref UniverseSimulator __instance)
        {
            if (simulator != null)
            {
                UnityEngine.Object.DestroyImmediate(simulator);
                simulator = null;
            }
            CopyBlackHoleData();

            simulator = UnityEngine.Object.Instantiate<StarSimulator>(__instance.starPrefab, __instance.transform);
            simulator.universeSimulator = __instance;
            simulator.SetStarData(starData);
            simulator.gameObject.layer = 24;
            simulator.gameObject.name = "Wormhole";
            simulator.gameObject.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "GameTick")]
        public static void UniverseSimulator_GameTick(ref UniverseSimulator __instance, double time)
        {
            Vector3 position = GameMain.mainPlayer.position;
            VectorLF3 uPosition = GameMain.mainPlayer.uPosition;
            Vector3 position2 = GameCamera.main.transform.position;
            Quaternion rotation = GameCamera.main.transform.rotation;
            if (simulator != null)
            {
                simulator.UpdateUniversalPosition(position, uPosition, position2, rotation);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StarSimulator), "UpdateUniversalPosition")]
        public static void StarSimulator_UpdateUniversalPosition(ref StarSimulator __instance, Vector3 playerLPos, VectorLF3 playerUPos, Vector3 cameraPos, Quaternion cameraRot)
        {
            if (__instance.starData == null || __instance.starData.id != -1)
            {
                return;
            }
            StarData localStar = GameMain.localStar;
            if (localStar == null) return;
            // TODO: Update loc here
            __instance.starData.uPosition = (localStar.uPosition + localStar.planets[1].uPosition) / 2;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StarSimulator), "UpdateUniversalPosition")]
        public static void StarSimulator_UpdateUniversalPosition_Post(ref StarSimulator __instance, Vector3 playerLPos, VectorLF3 playerUPos, Vector3 cameraPos, Quaternion cameraRot)
        {
            if (__instance.starData == null || __instance.starData.id != -1)
            {
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
            ref Material bodyMaterial = ref AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "bodyMaterial");
			bodyMaterial.SetFloat("_Multiplier", 1f - num15);
			__instance.massRenderer.gameObject.SetActive(false);
            __instance.atmosRenderer.gameObject.SetActive(false);
            __instance.effect.gameObject.SetActive(false);
            __instance.sunFlare.brightness *= num9;
			if (__instance.sunFlare.enabled != num9 > 0.001f)
			{
                __instance.sunFlare.enabled = (num9 > 0.001f);
			}

            __instance.blackRenderer.transform.localScale = Vector3.one * (__instance.solidRadius * 2f);
		}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StarSimulator), "LateUpdate")]
        public static bool StarSimulator_LateUpdate(ref StarSimulator __instance)
        {
            if (__instance.starData == null || __instance.starData.id != -1) return true;
            __instance.sunLight.enabled = !FactoryModel.whiteMode0;

            if (!FactoryModel.whiteMode0)
            {
                Vector3 forward = __instance.transform.forward;
                Shader.SetGlobalVector("_Global_SunDir", new Vector4(forward.x, forward.y, forward.z, 0f));
                Shader.SetGlobalColor("_Global_SunsetColor0", Color.Lerp(Color.white, __instance.sunsetColor0, __instance.useSunsetColor));
                Shader.SetGlobalColor("_Global_SunsetColor1", Color.Lerp(Color.white, __instance.sunsetColor1, __instance.useSunsetColor));
                Shader.SetGlobalColor("_Global_SunsetColor2", Color.Lerp(Color.white, __instance.sunsetColor2, __instance.useSunsetColor));
            }
            else
            {
                Transform transform = GameCamera.instance.camLight.transform;
                transform.rotation = Quaternion.LookRotation((GameMain.mainPlayer.position * 0.75f - transform.position).normalized, transform.position.normalized);
                Vector3 vector = -GameCamera.instance.camLight.transform.forward;
                Shader.SetGlobalVector("_Global_SunDir", new Vector4(vector.x, vector.y, vector.z, 0f));
                Shader.SetGlobalColor("_Global_SunsetColor0", Color.white);
                Shader.SetGlobalColor("_Global_SunsetColor1", Color.white);
                Shader.SetGlobalColor("_Global_SunsetColor2", Color.white);
            }
            ref Material bodyMaterial = ref AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "bodyMaterial");
            bodyMaterial.renderQueue = 2981;
            ref Material haloMaterial = ref AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "haloMaterial");
            haloMaterial.renderQueue = 2981;
            __instance.blackRenderer.enabled = false;
            AccessTools.Method(typeof(StarSimulator), "GpuAnalysis").Invoke(__instance, new object[] { });
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "CreateAllStarUIs")]
        public static void UIStarmap_CreateAllStarUIs(ref UIStarmap __instance)
        {
            if (uiStar != null)
            {
                UnityEngine.Object.DestroyImmediate(uiStar);
                uiStar = null;
            }

            CopyBlackHoleData();

            uiStar = UnityEngine.Object.Instantiate(__instance.starUIPrefab, __instance.starUIPrefab.transform.parent);
            uiStar._Create();
            uiStar._Init(starData);
            uiStar.gameObject.name = "test2";
            uiStar.gameObject.SetActive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnOpen")]
        public static void UIStarmap_OnOpen(ref UIStarmap __instance)
        {
            uiStar?._Open();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnClose")]
        public static void UIStarmap_OnClose(ref UIStarmap __instance)
        {
            uiStar?._Close();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnUpdate")]
        public static void UIStarmap_OnUpdate(ref UIStarmap __instance)
        {
            uiStar?._Update();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnLateUpdate")]
        public static void UIStarmap_OnLateUpdate(ref UIStarmap __instance)
        {
            uiStar?._LateUpdate();
        }

        private static void CopyBlackHoleData()
        {
            if (starData != null) return;

            StarData[] datas = GameMain.galaxy.stars;
            foreach (var data in datas)
            {
                if (data.type == EStarType.BlackHole)
                {
                    starData = data.Copy();
                    starData.planetCount = 0;
                    starData.planets = new PlanetData[] { };
                    starData.id = -1;
                    starData.index = -1;
                    starData.radius = 0.2f;
                    return;
                }
            }
        }



    }
}
