using HarmonyLib;
using System.Collections.Generic;
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
                DysonSphere sphere = new DysonSphere();
                //sphere.Init(GameMain.data, GameMain.galaxy.stars[i]);
                ManuallyInitSphere(ref sphere, GameMain.data, GameMain.galaxy.stars[i]);
                dropletSpheres.Add(sphere);
                dropletSpheres[i].ResetNew();
                dropletSpheres[i].swarm.bulletMaterial.SetColor("_Color0", new Color(0, 1, 1, 1));
                dropletSpheres[i].layerCount = -1;
            }
        }

        public static void ManuallyInitSphere(ref DysonSphere _this, GameData _gameData, StarData _starData)
        {
            _this.gameData = _gameData;
            _this.starData = _starData;
            _this.sunColor = Color.white;
            _this.energyGenPerSail = 0;
            _this.energyGenPerNode = 0;
            _this.energyGenPerFrame = 0;
            _this.energyGenPerShell = 0;
            if (_this.starData != null)
            {
                float num = 4f;
                _this.gravity = (float)(86646732.73933044 * (double)_this.starData.mass) * num;
                double num2 = (double)_this.starData.dysonLumino;
                _this.energyGenPerSail = (long)((double)_this.energyGenPerSail * num2);
                _this.energyGenPerNode = (long)((double)_this.energyGenPerNode * num2);
                _this.energyGenPerFrame = (long)((double)_this.energyGenPerFrame * num2);
                _this.energyGenPerShell = (long)((double)_this.energyGenPerShell * num2);
                _this.sunColor = Color.blue;
                _this.emissionColor = Color.white;
                if (_this.starData.type == EStarType.NeutronStar)
                {
                    //_this.sunColor = Configs.builtin.dysonSphereNeutronSunColor;
                    //_this.emissionColor = Configs.builtin.dysonSphereNeutronEmissionColor;
                }
                _this.defOrbitRadius = (float)((double)_this.starData.dysonRadius * 40000.0);
                _this.minOrbitRadius = _this.starData.physicsRadius * 1.5f;
                if (_this.minOrbitRadius < 4000f)
                {
                    _this.minOrbitRadius = 4000f;
                }
                _this.maxOrbitRadius = _this.defOrbitRadius * 2f;
                _this.avoidOrbitRadius = (float)(400 * 40000.0);
                if (_this.starData.type == EStarType.GiantStar)
                {
                    _this.minOrbitRadius *= 0.6f;
                }
                _this.defOrbitRadius = Mathf.Round(_this.defOrbitRadius / 100f) * 100f;
                _this.minOrbitRadius = Mathf.Ceil(_this.minOrbitRadius / 100f) * 100f;
                _this.maxOrbitRadius = Mathf.Round(_this.maxOrbitRadius / 100f) * 100f;
                _this.randSeed = _this.starData.seed;
            }
            _this.swarm = new DysonSwarm(_this);
            _this.swarm.Init();
            _this.layerCount = 0;
            _this.layersSorted = new DysonSphereLayer[10];
            _this.layersIdBased = new DysonSphereLayer[11];
            _this.rocketCapacity = 0;
            _this.rocketCursor = 1;
            _this.rocketRecycleCursor = 0;
            _this.autoNodes = new DysonNode[8];
            _this.autoNodeCount = 0;
            _this.nrdCapacity = 0;
            _this.nrdCursor = 1;
            _this.nrdRecycleCursor = 0;
            _this.modelRenderer = new DysonSphereSegmentRenderer(_this);
            _this.modelRenderer.Init();
            _this.rocketRenderer = new DysonRocketRenderer(_this);
            _this.inEditorRenderMaskL = -1;
            _this.inEditorRenderMaskS = -1;
            _this.inGameRenderMaskL = -1;
            _this.inGameRenderMaskS = -1;

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
            if (RendererSphere.dropletSpheres.Count != GameMain.galaxy.starCount) RendererSphere.InitAll();

            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void RSphereGameTick(long time)
        {
            if (RendererSphere.dropletSpheres.Count != GameMain.galaxy.starCount) RendererSphere.InitAll();
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
            //if (dropletSpheres.Count <= 0) return;
            //if (GameMain.localStar != null && __instance.selection.viewDysonSphere != null && DysonSphere.renderPlace == ERenderPlace.Dysonmap)
            //{
            //    dropletSpheres[GameMain.localStar.index].DrawPost();
            //}
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIDysonEditor), "DrawDysonSphereMapPost")]
        public static void DrawPatch4(UIDysonEditor __instance)
        {
            //if (dropletSpheres.Count <= 0) return;
            //if (GameMain.localStar != null && __instance.selection?.viewDysonSphere != null && DysonSphere.renderPlace == ERenderPlace.Dysonmap)
            //{
            //    dropletSpheres[GameMain.localStar.index].DrawPost();
            //}
        }


    }
}
