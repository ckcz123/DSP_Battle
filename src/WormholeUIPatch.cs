using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
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
                    for (var i = 0; i < 100; ++i)
                    {
                        simulator[i].gameObject.SetActive(false);
                        simulatorActive[i] = false;
                    }
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
            bool active = distance <= 40000 * 8 && (distance <= 2000 || (viewport.z > 0 && viewport.x > -0.1 && viewport.x < 1.1 && viewport.y > -0.1 && viewport.y < 1.1));
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
        }

    }

    class WormholeProperties
    {
        //虫洞血量等和恒星炮互动的数据
        public static int[] wormholeHp = new int[100];
        public static int initialWormholeCount = -1; //初始的虫洞数量
        public static int initialIntensity = -1; //初始的强度

        public static void InitWormholeProperties()
        {
            DspBattlePlugin.logger.LogInfo("Init wormhole properties");
            for (int i = 0; i < 100; i++)
            {
                wormholeHp[i] = 1000000; //虫洞默认血量
            }
            initialWormholeCount = Configs.nextWaveWormCount;
            initialIntensity = Configs.nextWaveIntensity;
        }

        public static int TryTakeDamage(int damage, int index=-1)
        {
            DspBattlePlugin.logger.LogInfo($"taking damage");
            if (index == -1) //默认攻击最后一个虫洞，暂时不考虑修改
            {
                index = Configs.nextWaveWormCount - 1;
            }
            if(initialWormholeCount <= 1 || Configs.nextWaveState >= 3) //只有一个虫洞时，无法对虫洞造成伤害。已经开始刷敌舰后，也无法造成伤害。
            {
                //DspBattlePlugin.logger.LogInfo($"initialWormholeCount is {initialWormholeCount}");
                return -1;
            }
            int realDamage = -1;
            if(index<100 && index > 0)
            {
                float ratio = (Configs.nextWaveWormCount - 1) * 1f / (initialWormholeCount - 1); //目前是线性减伤，因此消灭所需时间是反比例增长，且最后一个虫洞减伤是100%，不会被消灭
                damage = Mathf.RoundToInt(damage * ratio);
                //DspBattlePlugin.logger.LogInfo($"damage after debuff is {damage}");
                realDamage = Mathf.Min(damage, wormholeHp[index]);
                wormholeHp[index] -= damage;
                //DspBattlePlugin.logger.LogInfo($"wormhole {index} hp is {wormholeHp[index]}");
                if(wormholeHp[index] <= 0)
                {
                    Configs.nextWaveWormCount -= 1;
                    Configs.nextWaveIntensity = Mathf.RoundToInt(initialIntensity * (Configs.nextWaveWormCount * 1f / initialWormholeCount));

                    //显示更新，星图界面和实际界面
                    if (WormholeUIPatch.uiStar.Length > index && WormholeUIPatch.uiStar[index] != null)
                    {
                        WormholeUIPatch.uiStar[index]._Close();
                        WormholeUIPatch.uiStar[index].starObject.gameObject.SetActive(false);
                    }
                    if (WormholeUIPatch.simulatorActive.Length > index && WormholeUIPatch.simulatorActive[index])
                    {
                        WormholeUIPatch.simulator[index].gameObject.SetActive(false);
                        WormholeUIPatch.simulatorActive[index] = false;
                    }

                    //重新执行一遍敌舰数量设定
                    int intensity = Configs.nextWaveIntensity;
                    for (int i = 4; i >= 1; --i)
                    {
                        double v = EnemyShips.random.NextDouble() / 2 + 0.25;
                        Configs.nextWaveEnemy[i] = (int)(intensity * v / Configs.enemyIntensity[i]);
                        intensity -= Configs.nextWaveEnemy[i] * Configs.enemyIntensity[i];
                    }
                    Configs.nextWaveEnemy[0] = intensity / Configs.enemyIntensity[0];
                }
            }
            return realDamage;
        }


        public static void Export(BinaryWriter w)
        {
            w.Write(wormholeHp.Length);
            for (int i = 0; i < wormholeHp.Length; i++)
            {
                w.Write(wormholeHp[i]);
            }
            w.Write(initialWormholeCount);
            w.Write(initialIntensity);
        }

        public static void Import(BinaryReader r)
        {
            if(Configs.versionWhenImporting >= 20220321)
            {
                int length = r.ReadInt32();
                for (int i = 0; i < length; i++)
                {
                    wormholeHp[i] = r.ReadInt32();
                }
                initialWormholeCount = r.ReadInt32();
                initialIntensity = r.ReadInt32();
            }
            else
            {
                InitWormholeProperties();
            }
        }

        public static void IntoOtherSave()
        {
            InitWormholeProperties();
        }
    }
}
