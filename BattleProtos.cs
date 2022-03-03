using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xiaoye97;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using CommonAPI;
using CommonAPI.Systems;

namespace DSP_Battle
{
    class BattleProtos
    {
        [CommonAPISubmoduleDependency(nameof(ProtoRegistry))]
        public static void AddNewCannons()
        {
            var ejectorOri = LDB.items.Select(2311);
            var oriRecipe = LDB.recipes.Select(71);
            var oriModel = LDB.models.Select(72);

            var CannonRecipe = oriRecipe.Copy();
            //var Cannon = ejectorOri.Copy();

            CannonRecipe.ID = 410;
            CannonRecipe.Name = "轨道炮测试用";
            CannonRecipe.name = "轨道炮测试用".Translate();
            CannonRecipe.Description = "轨道炮测试用描述";
            CannonRecipe.description = "轨道炮测试用描述".Translate();
            CannonRecipe.Items = new int[] { 1101 };
            CannonRecipe.ItemCounts = new int[] { 1 };
            CannonRecipe.Results = new int[] { 9801 };
            CannonRecipe.ResultCounts = new int[] { 1000 };
            CannonRecipe.TimeSpend = 10;
            CannonRecipe.GridIndex = 2702;
            CannonRecipe.preTech = LDB.techs.Select(1704);
            Traverse.Create(CannonRecipe).Field("_iconSprite").SetValue(Traverse.Create(oriRecipe).Field("_iconSprite").GetValue());


            var CannonModel = oriModel.Copy();
            CannonModel.ID = 310;
            PrefabDesc desc = oriModel.prefabDesc;
            CannonModel.prefabDesc = new PrefabDesc(310, desc.prefab, desc.colliderPrefab);
            for (int i = 0; i < CannonModel.prefabDesc.lodMaterials.Length; i++)
            {
                if (CannonModel.prefabDesc.lodMaterials[i] == null) continue;
                for (int j = 0; j < CannonModel.prefabDesc.lodMaterials[i].Length; j++)
                {
                    if (CannonModel.prefabDesc.lodMaterials[i][j] == null) continue;
                    CannonModel.prefabDesc.lodMaterials[i][j] = new Material(desc.lodMaterials[i][j]);
                }
            }
            try
            {
                CannonModel.prefabDesc.lodMaterials[0][0].color = Color.red;
                CannonModel.prefabDesc.lodMaterials[1][0].color = Color.red;
                CannonModel.prefabDesc.lodMaterials[2][0].color = Color.red;
            }
            catch (Exception)
            {
            }
            CannonModel.prefabDesc.colliders = desc.colliders;
            //CannonModel.prefabDesc.buildCollider = desc.buildCollider;
            CannonModel.prefabDesc.buildColliders = desc.buildColliders;
            CannonModel.sid = "";
            CannonModel.SID = "";
            CannonModel.prefabDesc.ejectorBulletId = 1101; //子弹的Id
            CannonModel.prefabDesc.ejectorChargeFrame = 30; //充能时间（所需帧数，下同）
            CannonModel.prefabDesc.ejectorColdFrame = 15; //冷却时间

            var Cannon = ProtoRegistry.RegisterItem(9801, "轨道炮测试用", "轨道炮测试用描述", "", 2702,1000,ejectorOri.Type);
            Cannon.UnlockKey = -1;
            Cannon.BuildIndex = 509;
            Cannon.BuildMode = 1;
            Cannon.IsEntity = true;
            Cannon.isRaw = false;
            Cannon.CanBuild = true;
            Cannon.IconPath = LDB.items.Select(2311).IconPath;
            //Traverse.Create(Cannon).Field("_iconSprite").SetValue(Traverse.Create(oriRecipe).Field("_iconSprite").GetValue());
            LDBTool.PreAddProto(CannonModel);
            LDBTool.PreAddProto(CannonRecipe);
        }

        public static void CopyPrefabDesc()
        {
            
            LDB.models.OnAfterDeserialize();
            LDB.models.Select(310).prefabDesc.modelIndex = 310;
            LDB.items.Select(9801).ModelIndex = 310;
            LDB.items.Select(9801).prefabDesc = LDB.models.Select(310).prefabDesc;
            GameMain.gpuiManager.Init();
            

        }
    }
}
