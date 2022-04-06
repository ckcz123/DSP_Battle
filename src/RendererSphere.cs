using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace DSP_Battle
{
    //用于渲染不同颜色的子弹。这里面的bullet默认是没有存档的，因此需要追溯弹道的子弹需要作特殊处理才能使用RendererSphere。
    class RendererSphere
    {
        public static List<DysonSphere> enemySpheres = new List<DysonSphere>();

        public static void InitAll()
        {
            enemySpheres = new List<DysonSphere>();
            for (int i = 0; i < GameMain.galaxy.starCount; i++)
            {
                enemySpheres.Add(new DysonSphere());
                enemySpheres[i].Init(GameMain.data, GameMain.galaxy.stars[i]);
                enemySpheres[i].ResetNew();

                enemySpheres[i].swarm.bulletMaterial.SetColor("_Color0", new Color(1, 0, 0, 1)); //还有_Color1,2,3但是测试的时候没发现123有什么用
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameMain), "Start")]
        public static void GameStartPatch()
        {
            enemySpheres = new List<DysonSphere>();
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static bool BeforeGameTick()
        {
            if (enemySpheres.Count <= 0) 
                InitAll();

            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RSphereGameTick(long time)
        {
            if (enemySpheres.Count <= 0) InitAll();
            for (int i = 0; i < enemySpheres.Count; i++)
            {
                enemySpheres[i].swarm.GameTick(time);
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "OnPostDraw")]
        public static void DrawPatch1(GameData __instance)
        {
            if (enemySpheres.Count <= 0) return;
            if (__instance.localStar!=null && DysonSphere.renderPlace == ERenderPlace.Universe)
            {
                int index = __instance.localStar.index;
                if (enemySpheres[index] != null)
                {
                    enemySpheres[index].DrawPost();
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(StarmapCamera), "OnPostRender")]
        public static void DrawPatch2(StarmapCamera __instance)
        {
            if (enemySpheres.Count <= 0) return;
            if (__instance.uiStarmap.viewStarSystem != null && !UIStarmap.isChangingToMilkyWay)
            {
                DysonSphere dysonSphere = enemySpheres[__instance.uiStarmap.viewStarSystem.index];
                if (dysonSphere != null && DysonSphere.renderPlace == ERenderPlace.Starmap)
                {
                    dysonSphere.DrawPost();
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDysonEditor), "DrawDysonSphereMapPost")]
        public static void DrawPatch3(UIDysonEditor __instance)
        {
            if (enemySpheres.Count <= 0) return;
            if (__instance.selection.viewDysonSphere != null)
            {
                if (DysonSphere.renderPlace == ERenderPlace.Dysonmap)
                {
                    int index = __instance.selection.viewDysonSphere.starData.index;
                    enemySpheres[index].DrawPost();
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDysonPanel), "DrawDysonSphereMapPost")]
        public static void DrawPatch4(UIDysonPanel __instance)
        {
            if (enemySpheres.Count <= 0) return;
            if (__instance.viewDysonSphere != null)
            {
                if (DysonSphere.renderPlace == ERenderPlace.Dysonmap)
                {
                    int index = __instance.viewDysonSphere.starData.index;
                    enemySpheres[index].DrawPost();
                }
            }
        }


    }
}
