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
        public static UIStarmapStar[] uiStar = new UIStarmapStar[100];

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "OnGameLoaded")]
        public static void UniverseSimulator_OnGameLoaded(ref UniverseSimulator __instance)
        {
            for (int i = 0; i < 100; ++i)
            {
                if (simulator[i] != null) UnityEngine.Object.DestroyImmediate(simulator[i].gameObject);
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
            lastWaveState = -1;
        }

        private static int lastWaveState = -1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "GameTick")]
        public static void UniverseSimulator_GameTick(ref UniverseSimulator __instance, double time)
        {
            if (Configs.nextWaveState != 2 && Configs.nextWaveState != 3)
            {
                if (lastWaveState != Configs.nextWaveState)
                {
                    for (var i = 0; i < 100; ++i) simulator[i].gameObject.SetActive(false);
                    lastWaveState = Configs.nextWaveState;
                }
                return;
            }

            Vector3 position = GameMain.mainPlayer.position;
            VectorLF3 uPosition = GameMain.mainPlayer.uPosition;
            Vector3 position2 = GameCamera.main.transform.position;
            Quaternion rotation = GameCamera.main.transform.rotation;

            for (var i = 0; i < Configs.nextWaveWormCount; ++i)
            {
                if (lastWaveState != Configs.nextWaveState)
                {
                    simulator[i].gameObject.SetActive(true);
                }

                simulator[i].starData.uPosition = Configs.nextWaveWormholes[i].uPos;
                simulator[i].UpdateUniversalPosition(position, uPosition, position2, rotation);
            }

            lastWaveState = Configs.nextWaveState;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlackHoleHandler), "LateUpdate")]
        public static void BlackHoleHandler_LateUpdate(ref BlackHoleHandler __instance)
        {
            if (__instance.radius <= 800 * 0.2 + 1)
            {
                __instance.particle.gameObject.SetActive(false);
                // __instance.bodyRenderer.gameObject.SetActive(false);

                // Only render the wormholes which are on the viewport.
                Vector3 targetPosition = __instance.transform.parent.position;
                Vector3 cameraPosition = GameCamera.main.transform.localPosition;
                Vector3 viewport = GameCamera.main.WorldToViewportPoint(targetPosition);
                double distance = (targetPosition - cameraPosition).magnitude;
                __instance.bodyRenderer.gameObject.SetActive(distance < 2000 || (viewport.z > 0 && viewport.x > -0.1 && viewport.x < 1.1 && viewport.y > -0.1 && viewport.y < 1.1));
            }
        }


        private static int lastWaveState2 = -1;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "CreateAllStarUIs")]
        public static void UIStarmap_CreateAllStarUIs(ref UIStarmap __instance)
        {
            for (var i = 0; i < 100; ++i)
            {
                if (uiStar[i] != null) UnityEngine.Object.DestroyImmediate(uiStar[i].gameObject);
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
                starData[i].index = -1;
                starData[i].radius = 0.2f;
            }
        }

    }
}
