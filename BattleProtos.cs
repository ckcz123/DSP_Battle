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

            ProtoRegistry.RegisterString("导弹2", "", "反物质导弹");
            ProtoRegistry.RegisterString("导弹2描述", "", "能自动追踪敌人的导弹");
            ProtoRegistry.RegisterString("导弹2结论", "", "你解锁了热核导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("导弹3", "", "引力塌陷导弹");
            ProtoRegistry.RegisterString("导弹3描述", "", "能自动追踪敌人的导弹");
            ProtoRegistry.RegisterString("导弹3结论", "", "你解锁了热核导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("弹射器1", "", "子弹弹射器");
            ProtoRegistry.RegisterString("弹射器1描述", "", "可以发射子弹攻击敌方的弹射器");
            ProtoRegistry.RegisterString("弹射器1结论", "", "你解锁了子弹弹射器，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("弹射器2", "", "子弹弹射器");
            ProtoRegistry.RegisterString("弹射器2描述", "", "可以发射子弹攻击敌方的弹射器");
            ProtoRegistry.RegisterString("弹射器2结论", "", "你解锁了子弹弹射器，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("脉冲炮", "", "子弹弹射器");
            ProtoRegistry.RegisterString("脉冲炮描述", "", "脉冲炮描述");
            ProtoRegistry.RegisterString("脉冲炮结论", "", "脉冲炮结论");

            ProtoRegistry.RegisterString("发射器1", "", "导弹发射器");
            ProtoRegistry.RegisterString("发射器1描述", "", "可以发射导弹攻击敌方的发射器");
            ProtoRegistry.RegisterString("发射器1结论", "", "你解锁了导弹发射器，可以发射导弹攻击敌方");

            ProtoRegistry.RegisterString("定向爆破", "", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破描述", "", "通过精确计算子弹和导弹的索敌路径，预测撞击前的最佳起爆点，以尽可能对敌舰造成更大的破坏。");
            ProtoRegistry.RegisterString("定向爆破结论", "", "");

            ProtoRegistry.RegisterItem(8001, "子弹1", "子弹1描述", "Assets/texpack/硅晶圆", 2701, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8002, "子弹2", "子弹2描述", "Assets/texpack/钛掺杂的硅晶圆", 2702, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8003, "子弹3", "子弹3描述", "Assets/texpack/钨掺杂的硅晶圆", 2703, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8004, "导弹1", "导弹1描述", "Assets/texpack/超级机械组件", 2704, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8005, "导弹2", "导弹2描述", "Assets/texpack/超级机械组件", 2704, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8006, "导弹3", "导弹3描述", "Assets/texpack/超级机械组件", 2704, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8007, "脉冲", "脉冲描述", "Assets/texpack/超级机械组件", 2704, 100, EItemType.Material);

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
            ProtoRegistry.RegisterRecipe(804, ERecipeType.Assemble, 120, new int[] { 1802, 1303, 1406 }, new int[] { 1, 1, 1 }, new int[] { 8004 }, new int[] { 1 }, "导弹1描述",
                1904, 2704, "Assets/texpack/超级机械组件");
            ProtoRegistry.RegisterRecipe(805, ERecipeType.Assemble, 120, new int[] { 1803, 1303, 1406 }, new int[] { 1, 1, 1 }, new int[] { 8005 }, new int[] { 1 }, "导弹2描述",
                1904, 2704, "Assets/texpack/超级机械组件");
            ProtoRegistry.RegisterRecipe(806, ERecipeType.Assemble, 120, new int[] { 1209, 1303, 1406 }, new int[] { 1, 1, 1 }, new int[] { 8006 }, new int[] { 1 }, "导弹3描述",
                1904, 2704, "Assets/texpack/超级机械组件");
            ProtoRegistry.RegisterRecipe(811, ERecipeType.Assemble, 360, new int[] { 1103, 1201, 1303, 1205 }, new int[] { 10, 10, 3, 5 }, new int[] { 8011 }, new int[] { 1 }, "弹射器1描述",
                1901, 2601, "Icons/ItemRecipe/em-rail-ejector");
            ProtoRegistry.RegisterRecipe(812, ERecipeType.Assemble, 360, new int[] { 1103, 1201, 1303, 1205 }, new int[] { 10, 10, 3, 5 }, new int[] { 8012 }, new int[] { 1 }, "弹射器2描述",
                1901, 2601, "Icons/ItemRecipe/em-rail-ejector");
            ProtoRegistry.RegisterRecipe(813, ERecipeType.Assemble, 900, new int[] { 1107, 1125, 1209, 1305 }, new int[] { 40, 15, 10, 5 }, new int[] { 8013 }, new int[] { 1 }, "发射器1描述",
                1913, 2603, "Icons/ItemRecipe/vertical-launching-silo");
            ProtoRegistry.RegisterRecipe(814, ERecipeType.Assemble, 900, new int[] { 1107, 1125, 1209, 1305 }, new int[] { 40, 15, 10, 5 }, new int[] { 8014 }, new int[] { 1 }, "脉冲炮描述",
                1913, 2603, "Icons/ItemRecipe/em-rail-ejector");

            ProtoRegistry.RegisterTech(1901, "子弹1", "子弹1描述", "子弹1结论", "Icons/Tech/1112", new int[] { 1711 }, new int[] { 6001, 6002 }, new int[] { 20, 20 },
                72000, new int[] { 801, 811 }, new Vector2(29, -43));
            ProtoRegistry.RegisterTech(1902, "子弹2", "子弹2描述", "子弹2结论", "Icons/Tech/1112", new int[] { 1901 }, new int[] { 6001, 6002, 6003 }, new int[] { 12, 12, 12 },
                150000, new int[] { 802 }, new Vector2(33, -43));
            ProtoRegistry.RegisterTech(1903, "子弹3", "子弹3描述", "子弹3结论", "Icons/Tech/1112", new int[] { 1902 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 48, 24, 24 }, 150000, new int[] { 803 }, new Vector2(37, -43));
            ProtoRegistry.RegisterTech(1911, "导弹1", "导弹1描述", "导弹1结论", "Icons/Tech/1112", new int[] { 1114 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 804, 813 }, new Vector2(37, -31));
            ProtoRegistry.RegisterTech(1912, "导弹2", "导弹2描述", "导弹2结论", "Icons/Tech/1112", new int[] { 1911 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 804, 813 }, new Vector2(41, -31));
            //ProtoRegistry.RegisterTech(1913, "导弹3", "导弹3描述", "导弹3结论", "Icons/Tech/1112", new int[] { 1914 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
            //    new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 804, 813 }, new Vector2(49, -41));

            ProtoRegistry.RegisterTech(1914, "引力脉冲技术", "引力脉冲技术描述", "引力脉冲技术结论", "Icons/Tech/1112", new int[] { 1704 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 12, 48, 24, 24, 24 }, 150000, new int[] { 812, 806 }, new Vector2(49, -43));
            ProtoRegistry.RegisterTech(1915, "相位脉冲技术", "相位脉冲技术描述", "相位脉冲技术结论", "Icons/Tech/1112", new int[] { 1914 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 814 }, new Vector2(53, -43));

            //循环科技 分别是+20%子弹伤害  +10%子弹速度和2%导弹速度  以及扩充虫洞安全区，最大20万码，每级向最大值扩充x%
            TechProto techBulletDamage = ProtoRegistry.RegisterTech(2916, "定向爆破", "定向爆破描述", "定向爆破结论", "Icons/Tech/1112", new int[] { 1903 }, new int[] { 6006 },
                new int[] { 24 }, 150000, new int[] { }, new Vector2(41, -43));
            TechProto techBulletSpeed = ProtoRegistry.RegisterTech(2917, "引力波引导", "引力波引导描述", "引力波引导结论", "Icons/Tech/1112", new int[] { 1705 }, new int[] { 6006 },
                new int[] { 24 }, 150000, new int[] { }, new Vector2(53, -31));
            TechProto techWormDistance = ProtoRegistry.RegisterTech(2918, "相位干扰技术", "相位干扰技术描述", "相位干扰技术结论", "Icons/Tech/1112", new int[] { 1915 }, new int[] { 6006 },
                new int[] { 24 }, 150000, new int[] { }, new Vector2(57, -43));

            techBulletDamage.UnlockFunctions = new int[] { 50 };
            techBulletDamage.UnlockValues = new double[] { 0.2 };
            techBulletDamage.MaxLevel = 10000;
            techBulletDamage.LevelCoef1 = 60;
            techBulletDamage.LevelCoef2 = 12;

            techBulletSpeed.UnlockFunctions = new int[] { 51 };
            techBulletSpeed.UnlockValues = new double[] { 0.1 };
            techBulletSpeed.MaxLevel = 100;
            techBulletSpeed.LevelCoef1 = 60;
            techBulletSpeed.LevelCoef2 = 24;

            techWormDistance.UnlockFunctions = new int[] { 52 };
            techWormDistance.UnlockValues = new double[] { 0.1 }; //每次升级向200k的最大距离逼近剩余距离的x%，对于0.1，则是10%
            techWormDistance.MaxLevel = 10000;
            techWormDistance.LevelCoef1 = 60;
            techWormDistance.LevelCoef2 = 12;

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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameHistoryData), "UnlockTechFunction")]
        public static void UnlockBattleTechFunc(int func, double value, int level)
        {
            switch (func)
            {
                case 50:
                    Configs.bulletAtkScale += value;
                    break;
                case 51:
                    Configs.bulletSpeedScale += value;
                    break;
                case 52:
                    Configs.wormholeRangeAdded += (int)((190000 - Configs.wormholeRangeAdded)*value);
                    break;
                default:
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TechProto), "UnlockFunctionText")]
        public static void UnlockFunctionTextPatch(ref TechProto __instance, ref string __result, StringBuilder sb)
        {
            switch (__instance.ID)
            {
                case 1000: //还没写
                    __result = "".Translate();
                    break;

                    break;
            }
        }
    }
}
