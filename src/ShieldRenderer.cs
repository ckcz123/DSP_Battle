using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using xiaoye97;
using System.Reflection;

namespace DSP_Battle
{
    public class ShieldRenderer
    {
        public static StarData[] starData = new StarData[20];

        private static Dictionary<StarSimulator, Material> bodyMaterialMap = new Dictionary<StarSimulator, Material>();
        public static Dictionary<StarSimulator, Material> shieldMassMatMap = new Dictionary<StarSimulator, Material>();
        public static Dictionary<StarSimulator, Material> shieldAtomMatMap = new Dictionary<StarSimulator, Material>();
        public static Dictionary<StarSimulator, Material> shieldEffectMatMap = new Dictionary<StarSimulator, Material>();
        public static Dictionary<StarSimulator, float> shieldAtomParamMap = new Dictionary<StarSimulator, float>();
        public static Dictionary<StarSimulator, float> shieldEffectParamMap = new Dictionary<StarSimulator, float>();

        public static StarData testData;
        public static StarSimulator testSimulator;
        public static StarSimulator[] shieldSimulator = new StarSimulator[20];

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "OnGameLoaded")]
        public static void UniverseSimulator_OnGameLoaded(ref UniverseSimulator __instance)
        {

            CopyWhiteDwarfData();

            for (int i = 0; i < 20; i++)
            {
                if (shieldSimulator[i] != null) UnityEngine.Object.Destroy(shieldSimulator[i].gameObject);

                shieldSimulator[i] = UnityEngine.Object.Instantiate<StarSimulator>(__instance.starPrefab, __instance.transform);
                shieldSimulator[i].universeSimulator = __instance;
                shieldSimulator[i].SetStarData(starData[i]);
                shieldSimulator[i].gameObject.layer = 24;
                shieldSimulator[i].gameObject.name = "shieldSim_" + i.ToString();
                shieldSimulator[i].gameObject.SetActive(false);
            }
            //if (testSimulator != null) UnityEngine.Object.DestroyImmediate(testSimulator.gameObject);

            //testSimulator = UnityEngine.Object.Instantiate<StarSimulator>(__instance.starPrefab, __instance.transform);
            //testSimulator.universeSimulator = __instance;
            //testSimulator.SetStarData(testData);
            //testSimulator.gameObject.layer = 24;
            //testSimulator.gameObject.name = "Test planet";
            //// simulator[i].gameObject.SetActive((Configs.nextWaveState == 2 || Configs.nextWaveState == 3) && i < Configs.nextWaveWormCount);
            //testSimulator.gameObject.SetActive(true);

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "GameTick")]
        public static void UniverseSimulator_GameTick(ref UniverseSimulator __instance, double time)
        {
            Vector3 position = GameMain.mainPlayer.position;
            VectorLF3 uPosition = GameMain.mainPlayer.uPosition;
            Vector3 position2 = GameCamera.main.transform.position;
            Quaternion rotation = GameCamera.main.transform.rotation;
            if (false)
            {
                testSimulator.starData.uPosition = GameMain.galaxy.PlanetById(103).uPosition;
                testSimulator.UpdateUniversalPosition(position, uPosition, position2, rotation);
            }
            int starId100 = GameMain.data.localStar.id * 100;
            for (int i = 1; i < 20; i++)
            {
                int planetId = starId100 + i;
                if (ShieldGenerator.currentShield.ContainsKey(planetId) && ShieldGenerator.currentShield[planetId] > 20000)
                {
                    shieldSimulator[i].gameObject.SetActive(true);
                    shieldSimulator[i].starData.uPosition = GameMain.galaxy.PlanetById(planetId).uPosition;
                    shieldSimulator[i].UpdateUniversalPosition(position, uPosition, position2, rotation);
                }
                else
                {
                    shieldSimulator[i].gameObject.SetActive(false);
                }
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StarSimulator), "UpdateUniversalPosition")]
        public static void StarSimulator_UpdateUniversalPosition(ref StarSimulator __instance, Vector3 playerLPos, VectorLF3 playerUPos, Vector3 cameraPos, Quaternion cameraRot)
        {
            if (__instance.starData == null || __instance.starData.id != -2) //-1代表虫洞，-2代表护盾
            {
                return;
            }

            //下面这个if是为了确保护盾不会在过远的视角下被渲染为一个巨亮的环
            if (__instance.starData.id == -2) 
                __instance.runtimeDist = 100;

            float num4 = (float)(__instance.runtimeDist / 2400000.0);
            //num4 = (float)(10 / 2400000.0);
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
            // from shield
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
            // ---

            float num15 = Mathf.InverseLerp(a, b2, (float)__instance.runtimeDist);

            if (!bodyMaterialMap.ContainsKey(__instance))
            {
                bodyMaterialMap.Add(__instance, AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "bodyMaterial"));
            }
            bodyMaterialMap[__instance].SetFloat("_Multiplier", 1f - num15);

            //from shield
            if (__instance.starData.type == EStarType.BlackHole)
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
                if (!shieldMassMatMap.ContainsKey(__instance))
                {
                    shieldMassMatMap.Add(__instance, AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "massMaterial"));
                    shieldAtomMatMap.Add(__instance, AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "atmoMaterial"));
                    shieldEffectMatMap.Add(__instance, AccessTools.FieldRefAccess<StarSimulator, Material>(__instance, "effectMaterial"));
                    shieldAtomParamMap.Add(__instance, AccessTools.FieldRefAccess<StarSimulator, float>(__instance, "_atmo_param"));
                    shieldEffectParamMap.Add(__instance, AccessTools.FieldRefAccess<StarSimulator, float>(__instance, "_effect_param"));
                }
                Material massMaterial = shieldMassMatMap[__instance];
                Material atmoMaterial = shieldAtomMatMap[__instance];
                Material effectMaterial = shieldEffectMatMap[__instance];
                float _atmo_param = shieldAtomParamMap[__instance];
                float _effect_param = shieldEffectParamMap[__instance];

                __instance.massRenderer.gameObject.SetActive(true);
                __instance.effect.gameObject.SetActive(true);
                __instance.atmosRenderer.gameObject.SetActive(false);
                __instance.massRenderer.transform.rotation = Quaternion.Inverse(GameMain.data.relativeRot);
                __instance.effectRenderer.transform.rotation = Quaternion.Inverse(GameMain.data.relativeRot);
                massMaterial.SetFloat("_Multiplier", num15 / 2);
                massMaterial.SetColor("_Color3", new Color(0.0f, 0.35f, 0.9f));
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
                //新增
                //__instance.sunFlare.enabled = false;
            }
            else
            {
                __instance.massRenderer.gameObject.SetActive(false);
                __instance.atmosRenderer.gameObject.SetActive(false);
                __instance.effect.gameObject.SetActive(false);
            }
            //-----
            __instance.blackRenderer.transform.localScale = Vector3.one * (__instance.solidRadius * 2f);
        }

        private static void CopyWhiteDwarfData()
        {
            if (testData != null) return;

            testData = GameMain.galaxy.stars.Where(e => e.type == EStarType.WhiteDwarf).First().Copy();
            testData.planetCount = 0;
            testData.planets = new PlanetData[] { };
            testData.id = -1;
            testData.index = -1;
            testData.radius = 0.35f;

            for (int i = 0; i < 20; i++)
            {
                starData[i] = testData.Copy();
                starData[i].planetCount = 0;
                starData[i].planets = new PlanetData[] { };
                starData[i].id = -2;
                starData[i].index = i;
                starData[i].radius = 0.35f;
            }
        }
    }
}
