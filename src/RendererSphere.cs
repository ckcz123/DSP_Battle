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
        //public static List<DysonSphere> enemySpheres = new List<DysonSphere>();
        public static List<DysonSphere> dropletSpheres = new List<DysonSphere>();


        public static void InitAll()
        {
            //enemySpheres = new List<DysonSphere>();
            //for (int i = 0; i < GameMain.galaxy.starCount; i++)
            //{
            //    enemySpheres.Add(new DysonSphere());
            //    enemySpheres[i].Init(GameMain.data, GameMain.galaxy.stars[i]);
            //    enemySpheres[i].ResetNew();
            //    enemySpheres[i].swarm.bulletMaterial.SetColor("_Color0", new Color(1, 0, 0, 1)); //还有_Color1,2,3但是测试的时候没发现123有什么用
            //    enemySpheres[i].layerCount = -1;
            //}
            dropletSpheres = new List<DysonSphere>();
            for (int i = 0; i < GameMain.galaxy.starCount; i++)
            {
                dropletSpheres.Add(new DysonSphere());
                dropletSpheres[i].Init(GameMain.data, GameMain.galaxy.stars[i]);
                dropletSpheres[i].ResetNew();
                dropletSpheres[i].swarm.bulletMaterial.SetColor("_Color0", new Color(0, 1, 1, 1));
                dropletSpheres[i].layerCount = -1;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameMain), "Start")]
        public static void GameStartPatch()
        {
            //enemySpheres = new List<DysonSphere>();
            dropletSpheres = new List<DysonSphere>();
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static bool BeforeGameTick()
        {
            if (dropletSpheres.Count <= 0)
                InitAll();

            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RSphereGameTick(long time)
        {
            if (dropletSpheres.Count <= 0) InitAll();
            if (GameMain.localStar != null)
                dropletSpheres[GameMain.localStar.index].swarm.GameTick(time);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "OnPostDraw")]
        public static void DrawPatch1(GameData __instance)
        {
            if (dropletSpheres.Count <= 0) return;
            if (GameMain.localStar != null && DysonSphere.renderPlace == ERenderPlace.Universe)
            {
                dropletSpheres[GameMain.localStar.index].DrawPost();
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(StarmapCamera), "OnPostRender")]
        public static void DrawPatch2(StarmapCamera __instance)
        {
            if (dropletSpheres.Count <= 0) return;
            if (GameMain.localStar != null && __instance.uiStarmap.viewStarSystem != null && !UIStarmap.isChangingToMilkyWay && DysonSphere.renderPlace == ERenderPlace.Starmap)
            {
                dropletSpheres[GameMain.localStar.index].DrawPost();
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDysonEditor), "DrawDysonSphereMapPost")]
        public static void DrawPatch3(UIDysonEditor __instance)
        {
            if (dropletSpheres.Count <= 0) return;
            if (GameMain.localStar != null && __instance.selection.viewDysonSphere != null && DysonSphere.renderPlace == ERenderPlace.Dysonmap)
            {
                dropletSpheres[GameMain.localStar.index].DrawPost();
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDysonEditor), "DrawDysonSphereMapPost")]
        public static void DrawPatch4(UIDysonEditor __instance)
        {
            if (dropletSpheres.Count <= 0) return;
            if (GameMain.localStar != null && __instance.selection?.viewDysonSphere != null && DysonSphere.renderPlace == ERenderPlace.Dysonmap)
            {
                dropletSpheres[GameMain.localStar.index].DrawPost();
            }
        }


    }
}
