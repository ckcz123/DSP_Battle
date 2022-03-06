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
        public static void AddProtos()
        {
            ProtoRegistry.RegisterString("子弹1", "", "穿甲磁轨弹");
            ProtoRegistry.RegisterString("子弹1描述", "", "利用动能攻击的子弹");
            ProtoRegistry.RegisterString("子弹1结论", "", "你解锁了穿甲磁轨弹，可以利用动能进行攻击");
            ProtoRegistry.RegisterString("子弹2", "", "强酸磁轨弹");
            ProtoRegistry.RegisterString("子弹2描述", "", "利用硫酸腐蚀外壳的子弹");
            ProtoRegistry.RegisterString("子弹2结论", "", "你解锁了强酸磁轨弹，可以利用硫酸腐蚀外壳");
            ProtoRegistry.RegisterString("子弹3", "", "氘核爆破弹");
            ProtoRegistry.RegisterString("子弹3描述", "", "利用核聚变进行破坏的子弹");
            ProtoRegistry.RegisterString("子弹3结论", "", "你解锁了氘核爆破弹，可以利用核聚变进行破坏");

            ProtoRegistry.RegisterString("导弹1", "", "热核导弹");
            ProtoRegistry.RegisterString("导弹1描述", "", "能自动追踪敌人的导弹");
            ProtoRegistry.RegisterString("导弹1结论", "", "你解锁了热核导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("弹射器1", "", "子弹弹射器");
            ProtoRegistry.RegisterString("弹射器1描述", "", "可以发射子弹攻击敌方的弹射器");
            ProtoRegistry.RegisterString("弹射器1结论", "", "你解锁了子弹弹射器，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("发射器1", "", "导弹发射器");
            ProtoRegistry.RegisterString("发射器1描述", "", "可以发射导弹攻击敌方的发射器");
            ProtoRegistry.RegisterString("发射器1结论", "", "你解锁了导弹发射器，可以发射导弹攻击敌方");

            ProtoRegistry.RegisterItem(8001, "子弹1", "子弹1描述", "Assets/texpack/硅晶圆", 2701, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8002, "子弹2", "子弹2描述", "Assets/texpack/钛掺杂的硅晶圆", 2702, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8003, "子弹3", "子弹3描述", "Assets/texpack/钨掺杂的硅晶圆", 2703, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8004, "导弹1", "导弹1描述", "Assets/texpack/超级机械组件", 2704, 100, EItemType.Material);
            var Cannon = ProtoRegistry.RegisterItem(8011, "弹射器1", "弹射器1描述", "Icons/ItemRecipe/em-rail-ejector", 2601, 50, EItemType.Production);
            Cannon.BuildIndex = 509;
            Cannon.BuildMode = 1;
            Cannon.IsEntity = true;
            Cannon.isRaw = false;
            Cannon.CanBuild = true;
            Cannon.Upgrades = new int[] { };
            var Silo = ProtoRegistry.RegisterItem(8013, "发射器1", "发射器1描述", "Icons/ItemRecipe/vertical-launching-silo", 2603, 50, EItemType.Production);
            Silo.BuildIndex = 510;
            Silo.BuildMode = 1;
            Silo.IsEntity = true;
            Silo.isRaw = false;
            Silo.CanBuild = true;
            Silo.Upgrades = new int[] { };

            ProtoRegistry.RegisterRecipe(801, ERecipeType.Assemble, 60, new int[] { 1112, 1103 }, new int[] { 2, 2 }, new int[] { 8001 }, new int[] { 1 }, "子弹1描述",
                1901, 2701, "Assets/texpack/硅晶圆");
            ProtoRegistry.RegisterRecipe(802, ERecipeType.Assemble, 90, new int[] { 1118, 1110, 1116 }, new int[] { 1, 1, 1 }, new int[] { 8002 }, new int[] { 1 }, "子弹2描述",
                1902, 2702, "Assets/texpack/钛掺杂的硅晶圆");
            ProtoRegistry.RegisterRecipe(803, ERecipeType.Assemble, 120, new int[] { 1118, 1121, 1016 }, new int[] { 1, 4, 2 }, new int[] { 8003 }, new int[] { 1 }, "子弹3描述",
                1903, 2703, "Assets/texpack/钨掺杂的硅晶圆");
            ProtoRegistry.RegisterRecipe(804, ERecipeType.Assemble, 120, new int[] { 1802, 1303 }, new int[] { 1, 1 }, new int[] { 8004 }, new int[] { 1 }, "导弹1描述",
                1904, 2704, "Assets/texpack/超级机械组件");
            ProtoRegistry.RegisterRecipe(811, ERecipeType.Assemble, 360, new int[] { 1103, 1201, 1303, 1205 }, new int[] { 10, 10, 3, 5 }, new int[] { 8011 }, new int[] { 1 }, "弹射器1描述",
                1901, 2601, "Icons/ItemRecipe/em-rail-ejector");
            ProtoRegistry.RegisterRecipe(813, ERecipeType.Assemble, 900, new int[] { 1107, 1125, 1209, 1305 }, new int[] { 40, 15, 10, 5 }, new int[] { 8013 }, new int[] { 1 }, "发射器1描述",
                1913, 2603, "Icons/ItemRecipe/vertical-launching-silo");

            ProtoRegistry.RegisterTech(1901, "子弹1", "子弹1描述", "子弹1结论", "Icons/Tech/1112", new int[] { 1001 }, new int[] { 6001, 6002 }, new int[] { 20, 20 },
                72000, new int[] { 801, 811 }, new Vector2(13, -43));
            ProtoRegistry.RegisterTech(1902, "子弹2", "子弹2描述", "子弹2结论", "Icons/Tech/1112", new int[] { 1901 }, new int[] { 6001, 6002, 6003 }, new int[] { 12, 12, 12 },
                150000, new int[] { 802 }, new Vector2(21, -43));
            ProtoRegistry.RegisterTech(1903, "子弹3", "子弹3描述", "子弹3结论", "Icons/Tech/1112", new int[] { 1902 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 48, 24, 24 }, 150000, new int[] { 803 }, new Vector2(29, -43));
            ProtoRegistry.RegisterTech(1911, "导弹1", "导弹1描述", "导弹1结论", "Icons/Tech/1112", new int[] { 1903 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 804, 813 }, new Vector2(37, -43));

            var CannonModel = CopyModelProto(72, 311, Color.red);
            CannonModel.prefabDesc.ejectorBulletId = 8001; //子弹的Id
            CannonModel.prefabDesc.ejectorChargeFrame = 40; //充能时间（所需帧数，下同）
            CannonModel.prefabDesc.ejectorColdFrame = 20; //冷却时间
            LDBTool.PreAddProto(CannonModel);

            var SiloModel = CopyModelProto(74, 313, Color.red);
            SiloModel.prefabDesc.siloBulletId = 8004; // 导弹的Id
            SiloModel.prefabDesc.siloChargeFrame = 120;
            SiloModel.prefabDesc.siloColdFrame = 60;
            LDBTool.PreAddProto(SiloModel);

        }

        private static ModelProto CopyModelProto(int oriId, int id, Color color)
        {
            var oriModel = LDB.models.Select(oriId);
            var model = oriModel.Copy();
            model.ID = id;
            PrefabDesc desc = oriModel.prefabDesc;
            model.prefabDesc = new PrefabDesc(id, desc.prefab, desc.colliderPrefab);
            for (int i = 0; i < model.prefabDesc.lodMaterials.Length; i++)
            {
                if (model.prefabDesc.lodMaterials[i] == null) continue;
                for (int j = 0; j < model.prefabDesc.lodMaterials[i].Length; j++)
                {
                    if (model.prefabDesc.lodMaterials[i][j] == null) continue;
                    model.prefabDesc.lodMaterials[i][j] = new Material(desc.lodMaterials[i][j]);
                }
            }
            try
            {
                model.prefabDesc.lodMaterials[0][0].color = color;
                model.prefabDesc.lodMaterials[1][0].color = color;
                model.prefabDesc.lodMaterials[2][0].color = color;
            }
            catch (Exception)
            {
            }
            model.prefabDesc.hasBuildCollider = true;
            model.prefabDesc.colliders = desc.colliders;
            model.prefabDesc.buildCollider = desc.buildCollider;
            model.prefabDesc.buildColliders = desc.buildColliders;
            model.prefabDesc.colliderPrefab = desc.colliderPrefab;
            model.sid = "";
            model.SID = "";
            return model;
        }

        public static void CopyPrefabDesc()
        {
            LDB.models.OnAfterDeserialize();
            LDB.models.Select(311).prefabDesc.modelIndex = 311;
            LDB.items.Select(8011).ModelIndex = 311;
            LDB.items.Select(8011).prefabDesc = LDB.models.Select(311).prefabDesc;
            LDB.models.Select(313).prefabDesc.modelIndex = 313;
            LDB.items.Select(8013).ModelIndex = 313;
            LDB.items.Select(8013).prefabDesc = LDB.models.Select(313).prefabDesc;
            GameMain.gpuiManager.Init();
        }
    }
}
