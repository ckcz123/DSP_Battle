using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using xiaoye97;

namespace DSP_Battle
{
    class WormholeUIPatch
    {

        public static StarData[] starData = new StarData[100];
        public static StarSimulator[] simulator = new StarSimulator[100];
        public static bool[] simulatorActive = new bool[100];
        public static UIStarmapStar[] uiStar = new UIStarmapStar[100];

        private static Dictionary<StarSimulator, Material> bodyMaterialMap = new Dictionary<StarSimulator, Material>();

        public static StarData testData;
        public static StarSimulator testSimulator;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "OnGameLoaded")]
        public static void UniverseSimulator_OnGameLoaded(ref UniverseSimulator __instance)
        {
            for (int i = 0; i < 100; ++i)
            {
                if (simulator[i] != null) UnityEngine.Object.Destroy(simulator[i].gameObject);
            }

            CopyBlackHoleData();

            for (int i = 0; i < 100; ++i)
            {
                simulator[i] = UnityEngine.Object.Instantiate<StarSimulator>(__instance.starPrefab, __instance.transform);
                simulator[i].universeSimulator = __instance;
                simulator[i].SetStarData(starData[i]);
                simulator[i].gameObject.layer = 24;
                simulator[i].gameObject.name = "Wormhole_" + i;
                // simulator[i].gameObject.SetActive((Configs.nextWaveState == 2 || Configs.nextWaveState == 3) && i < Configs.nextWaveWormCount);
                simulator[i].gameObject.SetActive(false);
                simulator[i].bodyRenderer.gameObject.SetActive(false);
                simulator[i].massRenderer.gameObject.SetActive(false);
                simulator[i].atmosRenderer.gameObject.SetActive(false);
                simulator[i].effect.gameObject.SetActive(false);
                simulator[i].blackRenderer.gameObject.SetActive(false);

            }

            if (testSimulator != null) UnityEngine.Object.DestroyImmediate(testSimulator.gameObject);

            testSimulator = UnityEngine.Object.Instantiate<StarSimulator>(__instance.starPrefab, __instance.transform);
            testSimulator.universeSimulator = __instance;
            testSimulator.SetStarData(testData);
            testSimulator.gameObject.layer = 24;
            testSimulator.gameObject.name = "Test planet";
            // simulator[i].gameObject.SetActive((Configs.nextWaveState == 2 || Configs.nextWaveState == 3) && i < Configs.nextWaveWormCount);
            testSimulator.gameObject.SetActive(true);


            lastWaveState = -1;
        }

        private static int lastWaveState = -1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "GameTick")]
        public static void UniverseSimulator_GameTick(ref UniverseSimulator __instance, double time)
        {
            Vector3 position = GameMain.mainPlayer.position;
            VectorLF3 uPosition = GameMain.mainPlayer.uPosition;
            Vector3 position2 = GameCamera.main.transform.position;
            Quaternion rotation = GameCamera.main.transform.rotation;

            if (GameMain.localPlanet != null)
            {
                testSimulator.starData.uPosition = GameMain.localPlanet.uPosition;
                testSimulator.UpdateUniversalPosition(position, uPosition, position2, rotation);
            }

            if (Configs.nextWaveState != 2 && Configs.nextWaveState != 3)
            {
                if (lastWaveState != Configs.nextWaveState)
                {
                    for (var i = 0; i < 100; ++i)
                    {
                        simulator[i].gameObject.SetActive(false);
                        simulatorActive[i] = false;
                    }
                    lastWaveState = Configs.nextWaveState;
                }
                return;
            }

            for (var i = 0; i < Configs.nextWaveWormCount; ++i)
            {
                if (lastWaveState != Configs.nextWaveState)
                {
                    simulator[i].gameObject.SetActive(true);
                    simulatorActive[i] = true;
                }

                simulator[i].starData.uPosition = Configs.nextWaveWormholes[i].uPos;
                simulator[i].UpdateUniversalPosition(position, uPosition, position2, rotation);
            }

            lastWaveState = Configs.nextWaveState;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StarSimulator), "UpdateUniversalPosition")]
        public static void StarSimulator_UpdateUniversalPosition(ref StarSimulator __instance, Vector3 playerLPos, VectorLF3 playerUPos, Vector3 cameraPos, Quaternion cameraRot)
        {
            if (__instance.starData == null || __instance.starData.id != -1)
            {
                return;
            }

            Vector3 viewport = GameCamera.main.WorldToViewportPoint(__instance.transform.position);
            var distance = (__instance.starData.uPosition - playerUPos).magnitude;
            bool active = distance <= 2000 || (viewport.z > 0 && viewport.x > -0.1 && viewport.x < 1.1 && viewport.y > -0.1 && viewport.y < 1.1);
            if (active != simulatorActive[__instance.starData.index])
            {
                __instance.gameObject.SetActive(active);
                simulatorActive[__instance.starData.index] = active;
            }
            if (!active) return;

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

            float a;
            float b2;
            float num14 = 1f;
            if (GameMain.data.guideMission != null)
            {
                num14 = 8f - 7f * Mathf.Clamp01((GameMain.data.guideMission.elapseTime - 40f) / 20f);
            }
            if (__instance.starData.type == EStarType.MainSeqStar)
            {
                a = num11 * 0.7f * num14;
                b2 = num11 * 0.3f * num14;
            }
            else if (__instance.starData.type == EStarType.GiantStar)
            {
                a = num11 * 6f;
                b2 = num11 * 3f;
            }
            else if (__instance.starData.type == EStarType.WhiteDwarf)
            {
                a = num11 * 12f;
                b2 = num11 * 6f;
            }
            else if (__instance.starData.type == EStarType.NeutronStar)
            {
                a = num11 * 12f;
                b2 = num11 * 6f;
            }
            else
            {
                a = num11 * 100f;
                b2 = num11 * 50f;
            }

            float num15 = Mathf.InverseLerp(a, b2, (float)__instance.runtimeDist);

            if (!bodyMaterialMap.ContainsKey(__instance))
            {
                bodyMaterialMap.Add(__instance, AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "bodyMaterial"));
            }
            bodyMaterialMap[__instance].SetFloat("_Multiplier", 1f - num15);

<<<<<<< HEAD

            if (__instance.starData.type == EStarType.BlackHole)
=======
            __instance.sunFlare.brightness *= num9;
            if (__instance.sunFlare.enabled != num9 > 0.001f)
>>>>>>> master
            {
                __instance.massRenderer.gameObject.SetActive(false);
                __instance.atmosRenderer.gameObject.SetActive(false);
                __instance.effect.gameObject.SetActive(false);
                __instance.sunFlare.brightness *= num9;
                if (__instance.sunFlare.enabled != num9 > 0.001f)
                {
                    __instance.sunFlare.enabled = (num9 > 0.001f);
                }
            }
            else if (num15 > 0.001f)
            {
                ref Material massMaterial = ref AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "massMaterial");
                ref Material atmoMaterial = ref AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "atmoMaterial");
                ref Material effectMaterial = ref AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "effectMaterial");
                ref float _atmo_param = ref AccessTools.FieldRefAccess<StarSimulator, float>(__instance, "_atmo_param");
                ref float _effect_param = ref AccessTools.FieldRefAccess<StarSimulator, float>(__instance, "_effect_param");

                __instance.massRenderer.gameObject.SetActive(true);
                __instance.effect.gameObject.SetActive(true);
                __instance.atmosRenderer.gameObject.SetActive(false);
                __instance.massRenderer.transform.rotation = Quaternion.Inverse(GameMain.data.relativeRot);
                __instance.effectRenderer.transform.rotation = Quaternion.Inverse(GameMain.data.relativeRot);
                massMaterial.SetFloat("_Multiplier", num15 / 2);
                atmoMaterial.SetFloat("_Multiplier", num15);
                atmoMaterial.SetVector("_SunPos", __instance.posVector);
                effectMaterial.SetVector("_SunPos", __instance.posVector);
                effectMaterial.SetFloat("_Intensity", num15);
                if (__instance.effect.isStopped)
                {
                    __instance.effect.Play();
                }
                atmoMaterial.SetFloat("_AtmoThickness", _atmo_param * __instance.solidRadius * 0.04f);
                effectMaterial.SetFloat("_Radius1", _effect_param * __instance.solidRadius * 0.04f);
                __instance.massRenderer.transform.localScale = Vector3.one * (__instance.solidRadius * 2f);
                __instance.atmosRenderer.transform.position = playerLPos;
                __instance.atmosRenderer.transform.rotation = cameraRot;
                __instance.atmosRenderer.transform.localScale = Vector3.one * (__instance.solidRadius * 7f);
                __instance.effect.transform.localScale = Vector3.one * (0.04f * __instance.solidRadius);
                __instance.sunFlare.brightness *= 1f - num15;
                if (__instance.sunFlare.enabled != __instance.sunFlare.brightness > 0.001f)
                {
                    __instance.sunFlare.enabled = (__instance.sunFlare.brightness > 0.001f);
                }
            }
            else
            {
                __instance.massRenderer.gameObject.SetActive(false);
                __instance.atmosRenderer.gameObject.SetActive(false);
                __instance.effect.gameObject.SetActive(false);
            }
            __instance.blackRenderer.transform.localScale = Vector3.one * (__instance.solidRadius * 2f);


        }

        private static int lastWaveState2 = -1;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "CreateAllStarUIs")]
        public static void UIStarmap_CreateAllStarUIs(ref UIStarmap __instance)
        {
            for (var i = 0; i < 100; ++i)
            {
                if (uiStar[i] != null)
                {
                    uiStar[i]._Destroy();
                    UnityEngine.Object.Destroy(uiStar[i].gameObject);
                }
            }

            CopyBlackHoleData();

            for (var i = 0; i < 100; ++i)
            {
                uiStar[i] = UnityEngine.Object.Instantiate(__instance.starUIPrefab, __instance.starUIPrefab.transform.parent);
                uiStar[i]._Create();
                uiStar[i]._Init(starData[i]);
                uiStar[i].gameObject.name = "WormholeUI_" + i;
                uiStar[i].gameObject.SetActive(false);
            }

            lastWaveState2 = -1;

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnUpdate")]
        public static void UIStarmap_OnUpdate(ref UIStarmap __instance)
        {
            for (var i = 0; i < 100; ++i)
            {
                if ((Configs.nextWaveState != 2 && Configs.nextWaveState != 3) || i >= Configs.nextWaveWormCount)
                {
                    if (lastWaveState2 != Configs.nextWaveState)
                    {
                        uiStar[i]._Close();
                        uiStar[i].starObject.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (lastWaveState2 != Configs.nextWaveState)
                    {
                        uiStar[i]._Open();
                        uiStar[i].starObject.gameObject.SetActive(true);
                    }

                    uiStar[i]._Update();
                }
            }
            lastWaveState2 = Configs.nextWaveState;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnLateUpdate")]
        public static void UIStarmap_OnLateUpdate(ref UIStarmap __instance)
        {
            if (Configs.nextWaveState != 2 && Configs.nextWaveState != 3) return;
            for (var i = 0; i < Configs.nextWaveWormCount; ++i) uiStar[i].starObject._LateUpdate();
        }

        private static void CopyBlackHoleData()
        {
            if (starData[0] != null) return;

            StarData data = GameMain.galaxy.stars.Where(e => e.type == EStarType.BlackHole).First();
            for (var i = 0; i < 100; ++i)
            {
                starData[i] = data.Copy();
                starData[i].planetCount = 0;
                starData[i].planets = new PlanetData[] { };
                starData[i].id = -1;
                starData[i].index = i;
                starData[i].radius = 0.2f;
            }

            testData = GameMain.galaxy.stars.Where(e => e.spectr == ESpectrType.O).First().Copy();
            testData.planetCount = 0;
            testData.planets = new PlanetData[] { };
            testData.id = -1;
            testData.index = -1;
            testData.radius = 0.35f;

        }

    }
}
