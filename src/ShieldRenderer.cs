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
        public static StarSimulator[] shieldSimulator = new StarSimulator[20];

        private static Dictionary<StarSimulator, Material> bodyMaterialMap = new Dictionary<StarSimulator, Material>();
        public static Dictionary<StarSimulator, Material> shieldMassMatMap = new Dictionary<StarSimulator, Material>();
        public static Dictionary<StarSimulator, Material> shieldAtomMatMap = new Dictionary<StarSimulator, Material>();
        public static Dictionary<StarSimulator, Material> shieldEffectMatMap = new Dictionary<StarSimulator, Material>();
        public static Dictionary<StarSimulator, float> shieldAtomParamMap = new Dictionary<StarSimulator, float>();
        public static Dictionary<StarSimulator, float> shieldEffectParamMap = new Dictionary<StarSimulator, float>();

        public static Color shieldColor1 = new Color(0, 0.35f, 0.9f); //整个半透明的发光颜色
        public static Color shieldColor2 = new Color(0, 0.1f, 0.3f); //整个颜色，饱和度不能太高
        public static Color shieldColor3 = new Color(0.1f, 0.55f, 1f); //外环光晕
        public static int shieldRenderMin = 1; //低于这个护盾值不会渲染护盾
        public static int shieldFullyRendered = 250000; //护盾值高于这个数量就渲染满护盾的饱和度，否则护盾量越少颜色越暗
        public static bool activeProp1 = true;
        public static bool activeProp2 = true;
        public static bool activeProp3 = false;
        public static float shieldRadius = 0.43f; //行星护盾的半径，0.4或以下会导致在地表时摄像机拉到最远会被护盾遮挡，使得整体有一层白雾的感觉

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

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UniverseSimulator), "GameTick")]
        public static void UniverseSimulator_GameTick(ref UniverseSimulator __instance, double time)
        {
            Vector3 position = GameMain.mainPlayer.position;
            VectorLF3 uPosition = GameMain.mainPlayer.uPosition;
            Vector3 position2 = GameCamera.main.transform.position;
            Quaternion rotation = GameCamera.main.transform.rotation;
            int starId100 = -100000;
            if(GameMain.data.localStar != null)
            {
                starId100 = GameMain.data.localStar.id * 100;
            }
            for (int i = 1; i < 20; i++)
            {
                int planetId = starId100 + i; //此时shieldSimulator的stardata的index实际上与planet的index+1相等、相对应，也就是说shieldSimulator[0]永远不会被用到。
                if (starId100 >= 0 && ShieldGenerator.currentShield.ContainsKey(planetId) && ShieldGenerator.currentShield[planetId] > shieldRenderMin)
                {
                    shieldSimulator[i].gameObject.SetActive(true);
                    shieldSimulator[i].starData.uPosition = GameMain.galaxy.PlanetById(planetId).uPosition;
                    shieldSimulator[i].UpdateUniversalPosition(position, uPosition, position2, rotation);
                }
                else
                {
                    shieldSimulator[i].starData.radius = 0.2f;
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

                float shieldCharged = 1f;
                if(GameMain.localStar != null)
                {
                    int planetId = GameMain.localStar.id * 100 + __instance.starData.index; //不要用id（因为全是-2）也不要+1（跟其他逻辑不同），这里simulator的stardata的index是和planetId对应的，这是由于上面的GameTick的patch决定的
                    if(ShieldGenerator.currentShield.ContainsKey(planetId) && ShieldGenerator.currentShield[planetId] < shieldFullyRendered)
                    {
                        shieldCharged = (float)((ShieldGenerator.currentShield[planetId] - shieldRenderMin) * 1.0 / (shieldFullyRendered - shieldRenderMin));
                    }
                }

                //颜色根据护盾已充能的数量变得更亮，护盾大小逐渐扩大
                massMaterial.SetColor("_Color1", new Color(shieldColor1.r * shieldCharged, shieldColor1.g * shieldCharged, shieldColor1.b * shieldCharged) ); //整个半透明的发光颜色
                massMaterial.SetColor("_Color2", new Color(shieldColor2.r * shieldCharged, shieldColor2.g * shieldCharged, shieldColor2.b * shieldCharged)); //整个颜色，饱和度不能太高
                massMaterial.SetColor("_Color3", new Color(shieldColor3.r * shieldCharged, shieldColor3.g * shieldCharged, shieldColor3.b * shieldCharged)); //外环光晕
                if (Configs.nextWaveState != 3 || (GameMain.localStar != null && Configs.nextWaveStarIndex != GameMain.localStar.index)) //如果不在战斗状态，会渲染护盾从充能过程逐渐变大的过程
                {
                    float shrank = __instance.starData.radius - (0.25f + (shieldRadius - 0.25f) * shieldCharged);
                    if (shieldCharged >= 0.000000001f && shrank > 0.008f)
                        shrank = 0.008f;
                    __instance.starData.radius -= shrank;
                }
                else //如果在战斗过程，护盾迅速扩大到标准大小来应对敌人，但并不会加强任何实际的护盾强度，只是为了匹配飞船开火显示
                {
                    float expand = shieldRadius - __instance.starData.radius;
                    expand = expand > 0.008f ? 0.008f : expand;
                    __instance.starData.radius += expand;
                }

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
            //-----
            __instance.blackRenderer.transform.localScale = Vector3.one * (__instance.solidRadius * 2f);
        }

        private static void CopyWhiteDwarfData()
        {
            if (starData[0] != null) return;

            StarData testData = GameMain.galaxy.stars.Where(e => e.type == EStarType.WhiteDwarf).First().Copy();

            for (int i = 0; i < 20; i++)
            {
                starData[i] = testData.Copy();
                starData[i].planetCount = 0;
                starData[i].planets = new PlanetData[] { };
                starData[i].id = -2;
                starData[i].index = i;
                starData[i].radius = shieldRadius;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStarmap), "UpdateCursorView")]
        public static void UIStarmapUpdateCursorViewPatch(ref UIStarmap __instance)
        {
            UIStarmapPlanet uistarmapPlanet = __instance.mouseHoverPlanet;
            if (__instance.focusPlanet != null)
            {
                uistarmapPlanet = __instance.focusPlanet;
            }

            if (uistarmapPlanet == null)
                return;

            int planetId = uistarmapPlanet.planet.id;
            int curShield = 0;
            int maxShield = 0;
            ShieldGenerator.currentShield.TryGetValue(planetId, out curShield);
            ShieldGenerator.maxShieldCapacity.TryGetValue(planetId, out maxShield);
            if (curShield > 0)
            {
                int length = __instance.cursorViewText.text.Length;
                if (length > 5 && __instance.cursorViewText.text[length-1] != 'k')
                    __instance.cursorViewText.text += "\n" + "力场护盾短".Translate() + "  " + (curShield / 1000).ToString() + "k / " + (maxShield / 1000).ToString() + "k";                    
            }
            __instance.cursorViewTrans.sizeDelta = new Vector2(__instance.cursorViewText.preferredWidth * 0.5f + 44f, __instance.cursorViewText.preferredHeight * 0.5f + 14f);
            __instance.cursorRightDeco.sizeDelta = new Vector2(__instance.cursorViewTrans.sizeDelta.y - 12f, 5f);

        }

    }
}

