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
        public static void AddNewCannons()
        {

            var oriModel = LDB.models.Select(72);
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
            CannonModel.prefabDesc.ejectorBulletId = 1901; //子弹的Id
            CannonModel.prefabDesc.ejectorChargeFrame = 30; //充能时间（所需帧数，下同）
            CannonModel.prefabDesc.ejectorColdFrame = 15; //冷却时间

            var Cannon = ProtoRegistry.RegisterItem(9801, "轨道炮测试用", "轨道炮测试用描述", "Icons/ItemRecipe/em-rail-ejector", 2702, 1000, EItemType.Production);
            Cannon.BuildIndex = 509;
            Cannon.BuildMode = 1;
            Cannon.IsEntity = true;
            Cannon.isRaw = false;
            Cannon.CanBuild = true;

            ProtoRegistry.RegisterRecipe(700, ERecipeType.Assemble, 360, new int[] { 1103, 1201, 1303, 1205 }, new int[] { 20, 20, 5, 10 }, new int[] { 9801 }, new int[] { 1 }, "轨道测试炮",
                1901, 2702, "Icons/ItemRecipe/em-rail-ejector");

            ProtoRegistry.RegisterItem(8001, "金刚石氢弹", "两块高强度的金刚石压制成弹头，将液氢棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆液氢棒。", "Icons/ItemRecipe/iron-plate", 2703, 1000, EItemType.Material);
            ProtoRegistry.RegisterItem(8002, "钛氘弹", "两块高强度的钛晶石压制成弹头，将氚棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆氘棒。", "Icons/ItemRecipe/copper-plate", 2704, 1000, EItemType.Material);
            ProtoRegistry.RegisterItem(8003, "反物质弹", "两块高强度的钛晶石压制成弹头，将反物质棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆反物质棒。", "Icons/ItemRecipe/silicium-single-crystal", 2705, 1000, EItemType.Material);

            ProtoRegistry.RegisterRecipe(701, ERecipeType.Assemble, 60, new int[] { 1112, 1801 }, new int[] { 2, 1 }, new int[] { 8001 }, new int[] { 1 }, "两块高强度的金刚石压制成弹头，将液氢棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆液氢棒。",
                1901, 2703, "Icons/ItemRecipe/iron-plate");
            ProtoRegistry.RegisterRecipe(702, ERecipeType.Assemble, 120, new int[] { 1118, 1802 }, new int[] { 2, 1 }, new int[] { 8002 }, new int[] { 1 }, "两块高强度的钛晶石压制成弹头，将氘棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆氘棒。",
                1902, 2704, "Icons/ItemRecipe/copper-plate");
            ProtoRegistry.RegisterRecipe(703, ERecipeType.Assemble, 180, new int[] { 1118, 1803 }, new int[] { 2, 1 }, new int[] { 8003 }, new int[] { 1 }, "两块高强度的钛晶石压制成弹头，将反物质棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆反物质棒。",
                1903, 2705, "Icons/ItemRecipe/silicium-single-crystal");

            ProtoRegistry.RegisterTech(1901, "金刚石氢弹", "两块高强度的金刚石压制成弹头，将液氢棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆液氢棒。", "", "Icons/Tech/1112", new int[] { 1403 }, new int[] { 6001, 6002 }, new int[] { 20, 20 },
                144000, new int[] { 700, 701 }, new Vector2(25, 9));
            ProtoRegistry.RegisterTech(1902, "钛氘弹", "两块高强度的钛晶石压制成弹头，将氘棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆氘棒。", "", "Icons/Tech/1112", new int[] { 1124 }, new int[] { 6001, 6002, 6003 }, new int[] { 12, 12, 12 },
                300000, new int[] { 702 }, new Vector2(37, -31));
            ProtoRegistry.RegisterTech(1903, "反物质弹", "两块高强度的钛晶石压制成弹头，将反物质棒固定其中，通过电磁炮发射，命中时造成动能穿甲伤害，并引爆反物质棒。", "", "Icons/Tech/1112", new int[] { 1902 }, new int[] { 6001, 6002, 6003, 6005 },
                new int[] { 12, 48, 24, 24 }, 300000, new int[] { 703 }, new Vector2(41, -31));

            LDB.techs.Select(1511).Position = new Vector2(25, 5);

            //Traverse.Create(Cannon).Field("_iconSprite").SetValue(Traverse.Create(oriRecipe).Field("_iconSprite").GetValue());
            LDBTool.PreAddProto(CannonModel);


            AddTechProtos();

        }

        private static void AddTechProtos()
        {



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
