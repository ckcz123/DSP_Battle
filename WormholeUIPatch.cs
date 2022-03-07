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

        public static StarData[] starData = new StarData[100];
        public static StarSimulator[] simulator = new StarSimulator[100];
        public static UIStarmapStar[] uiStar = new UIStarmapStar[100];

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "OnGameLoaded")]
        public static void UniverseSimulator_OnGameLoaded(ref UniverseSimulator __instance)
        {
            for (int i = 0; i < 100; ++i)
            {
                if (simulator[i] == null) simulator[i] = UnityEngine.Object.Instantiate<StarSimulator>(__instance.starPrefab, __instance.transform);
            }

            CopyBlackHoleData();

            for (int i = 0; i < 100; ++i)
            {
                simulator[i].universeSimulator = __instance;
                simulator[i].SetStarData(starData[i]);
                simulator[i].gameObject.layer = 24;
                simulator[i].gameObject.name = "Wormhole_" + i;
                // simulator[i].gameObject.SetActive((Configs.nextWaveState == 2 || Configs.nextWaveState == 3) && i < Configs.nextWaveWormCount);
                simulator[i].gameObject.SetActive(false);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "GameTick")]
        public static void UniverseSimulator_GameTick(ref UniverseSimulator __instance, double time)
        {
            if (time % 60 != 1)
                return;
            if (Configs.nextWaveState != 2 && Configs.nextWaveState != 3)
            {
                for (var i = 0; i < 100; ++i) simulator[i].gameObject.SetActive(false);
                return;
            }

            Vector3 position = GameMain.mainPlayer.position;
            VectorLF3 uPosition = GameMain.mainPlayer.uPosition;
            Vector3 position2 = GameCamera.main.transform.position;
            Quaternion rotation = GameCamera.main.transform.rotation;
            PlanetData planet = GameMain.galaxy.PlanetById(Configs.nextWavePlanetId);

            for (var i = 0; i < Configs.nextWaveWormCount; ++i)
            {
                simulator[i].gameObject.SetActive(true);

                int angle1 = Configs.nextWaveAngle1[i];
                int angle2 = Configs.nextWaveAngle2[i];
                simulator[i].starData.uPosition = 
                    planet.uPosition
                    + new VectorLF3(
                            (Configs.wormholeRange + planet.radius) * Math.Cos(angle1 * Math.PI / 360) * Math.Cos(angle2 * Math.PI / 360), // rcosAcosB
                            (Configs.wormholeRange + planet.radius) * Math.Cos(angle1 * Math.PI / 360) * Math.Sin(angle2 * Math.PI / 360), // rcosAsinB
                            (Configs.wormholeRange + planet.radius) * Math.Sin(angle1 * Math.PI / 360) // rsinA
                    );
                simulator[i].UpdateUniversalPosition(position, uPosition, position2, rotation);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StarSimulator), "UpdateUniversalPosition")]
        public static void StarSimulator_UpdateUniversalPosition(ref StarSimulator __instance, Vector3 playerLPos, VectorLF3 playerUPos, Vector3 cameraPos, Quaternion cameraRot)
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "CreateAllStarUIs")]
        public static void UIStarmap_CreateAllStarUIs(ref UIStarmap __instance)
        {
            for (var i = 0; i < 100; ++i)
            {
                if (uiStar[i] == null) uiStar[i] = UnityEngine.Object.Instantiate(__instance.starUIPrefab, __instance.starUIPrefab.transform.parent);
            }

            CopyBlackHoleData();

            for (var i = 0; i < 100; ++i)
            {
                uiStar[i]._Create();
                uiStar[i]._Init(starData[i]);
                uiStar[i].gameObject.name = "WormholeUI_" + i;
                uiStar[i].gameObject.SetActive(false);
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnUpdate")]
        public static void UIStarmap_OnUpdate(ref UIStarmap __instance)
        {
            if (GameMain.instance.timei % 60 != 0)
                return;

            for (var i = 0; i < 100; ++i)
            {
                if ((Configs.nextWaveState != 2 && Configs.nextWaveState != 3) || i >= Configs.nextWaveWormCount)
                {
                    if (uiStar[i].active)
                    {
                        uiStar[i]._Close();
                    }
                    if (uiStar[i].starObject.gameObject.activeSelf) 
                    {
                        uiStar[i].starObject.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!uiStar[i].active)
                    {
                        uiStar[i]._Open();
                    }
                    if (!uiStar[i].starObject.gameObject.activeSelf)
                    {
                        uiStar[i].gameObject.SetActive(true);
                    }

                    uiStar[i]._Update();
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "_OnLateUpdate")]
        public static void UIStarmap_OnLateUpdate(ref UIStarmap __instance)
        {
            if (GameMain.instance.timei % 60 != 0)
                return;
            if (Configs.nextWaveState != 2 && Configs.nextWaveState != 3) return;
            for (var i = 0; i < Configs.nextWaveWormCount; ++i) uiStar[i]._LateUpdate();
        }

        private static void CopyBlackHoleData()
        {
            if (starData[0] != null) return;

            StarData data = GameMain.galaxy.stars.Where(e => e.type == EStarType.BlackHole).First();
            for (var i = 0; i < 100; ++i) {
                starData[i] = data.Copy();
                starData[i].planetCount = 0;
                starData[i].planets = new PlanetData[] { };
                starData[i].id = -1;
                starData[i].index = -1;
                starData[i].radius = 0.2f;
            }
        }

    }
}
