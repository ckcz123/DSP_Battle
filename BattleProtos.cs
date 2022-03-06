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
            
            ProtoRegistry.RegisterString("弹射器1", "", "子弹弹射器1");
            ProtoRegistry.RegisterString("弹射器1描述", "", "可以发射子弹攻击敌方的弹射器");
            ProtoRegistry.RegisterString("弹射器1结论", "", "你解锁了子弹弹射器，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("弹射器2", "", "子弹弹射器2");
            ProtoRegistry.RegisterString("弹射器2描述", "", "可以发射子弹攻击敌方的弹射器");
            ProtoRegistry.RegisterString("弹射器2结论", "", "你解锁了子弹弹射器，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("脉冲炮", "", "脉冲炮");
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
            ProtoRegistry.RegisterItem(8004, "导弹1", "导弹1描述", "Assets/texpack/超级机械组件", 2705, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8005, "导弹2", "导弹2描述", "Assets/texpack/超级机械组件", 2706, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8006, "导弹3", "导弹3描述", "Assets/texpack/超级机械组件", 2707, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8007, "脉冲", "脉冲描述", "Icons/ItemRecipe/em-rail-ejector", 9999, 100, EItemType.Material);

            var Cannon = ProtoRegistry.RegisterItem(8011, "弹射器1", "弹射器1描述", "Icons/ItemRecipe/em-rail-ejector", 2601, 50, EItemType.Production);
            Cannon.BuildIndex = 607;
            Cannon.BuildMode = 1;
            Cannon.IsEntity = true;
            Cannon.isRaw = false;
            Cannon.CanBuild = true;
            Cannon.Upgrades = new int[] { };
            var Cannon2 = ProtoRegistry.RegisterItem(8012, "弹射器2", "弹射器2描述", "Icons/ItemRecipe/em-rail-ejector", 2602, 50, EItemType.Production);
            Cannon2.BuildIndex = 608;
            Cannon2.BuildMode = 1;
            Cannon2.IsEntity = true;
            Cannon2.isRaw = false;
            Cannon2.CanBuild = true;
            Cannon2.Upgrades = new int[] { };
            var Cannon3 = ProtoRegistry.RegisterItem(8014, "脉冲炮", "脉冲炮描述", "Icons/ItemRecipe/em-rail-ejector", 2604, 50, EItemType.Production);
            Cannon3.BuildIndex = 609;
            Cannon3.BuildMode = 1;
            Cannon3.IsEntity = true;
            Cannon3.isRaw = false;
            Cannon3.CanBuild = true;
            Cannon3.Upgrades = new int[] { };


            var Silo = ProtoRegistry.RegisterItem(8013, "发射器1", "发射器1描述", "Icons/ItemRecipe/vertical-launching-silo", 2603, 50, EItemType.Production);
            Silo.BuildIndex = 610;
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
                1904, 2705, "Assets/texpack/超级机械组件");
            ProtoRegistry.RegisterRecipe(806, ERecipeType.Assemble, 120, new int[] { 1209, 1303, 1406 }, new int[] { 1, 1, 1 }, new int[] { 8006 }, new int[] { 1 }, "导弹3描述",
                1904, 2706, "Assets/texpack/超级机械组件");
            ProtoRegistry.RegisterRecipe(811, ERecipeType.Assemble, 360, new int[] { 1103, 1201, 1303, 1205 }, new int[] { 10, 10, 3, 5 }, new int[] { 8011 }, new int[] { 1 }, "弹射器1描述",
                1901, 2601, "Icons/ItemRecipe/em-rail-ejector");
            ProtoRegistry.RegisterRecipe(812, ERecipeType.Assemble, 360, new int[] { 1103, 1201, 1303, 1205 }, new int[] { 10, 10, 3, 5 }, new int[] { 8012 }, new int[] { 1 }, "弹射器2描述",
                1901, 2602, "Icons/ItemRecipe/em-rail-ejector");
            ProtoRegistry.RegisterRecipe(813, ERecipeType.Assemble, 900, new int[] { 1107, 1125, 1209, 1305 }, new int[] { 40, 15, 10, 5 }, new int[] { 8013 }, new int[] { 1 }, "发射器1描述",
                1913, 2603, "Icons/ItemRecipe/vertical-launching-silo");
            ProtoRegistry.RegisterRecipe(814, ERecipeType.Assemble, 900, new int[] { 1107, 1125, 1209, 1305 }, new int[] { 40, 15, 10, 5 }, new int[] { 8014 }, new int[] { 1 }, "脉冲炮描述",
                1913, 2604, "Icons/ItemRecipe/em-rail-ejector");

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
            TechProto techBulletDamage1 = ProtoRegistry.RegisterTech(4901, "定向爆破1", "定向爆破描述", "定向爆破结论", "Icons/Tech/1112", new int[] {}, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { }, new Vector2(9, -43));
            techBulletDamage1.PreTechsImplicit = new int[] { 1312 }; 
            techBulletDamage1.UnlockFunctions = new int[] { 50 };
            techBulletDamage1.UnlockValues = new double[] { 0.2 };
            techBulletDamage1.Level = 1;
            techBulletDamage1.MaxLevel = 1;
            techBulletDamage1.LevelCoef1 = 0;
            techBulletDamage1.LevelCoef2 = 0;
            TechProto techBulletDamage2 = ProtoRegistry.RegisterTech(4902, "定向爆破2", "定向爆破描述", "定向爆破结论", "Icons/Tech/1112", new int[] { 4901 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 24, 24, 24, 24, 24 }, 240000, new int[] { }, new Vector2(13, -43));
            techBulletDamage2.UnlockFunctions = new int[] { 50 };
            techBulletDamage2.UnlockValues = new double[] { 0.2 };
            techBulletDamage2.Level = 2;
            techBulletDamage2.MaxLevel = 2;
            techBulletDamage2.LevelCoef1 = 0;
            techBulletDamage2.LevelCoef2 = 0;
            TechProto techBulletDamage3 = ProtoRegistry.RegisterTech(4903, "定向爆破3", "定向爆破描述", "定向爆破结论", "Icons/Tech/1112", new int[] { 4902 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24, 24 }, 450000, new int[] { }, new Vector2(17, -43));
            techBulletDamage3.UnlockFunctions = new int[] { 50 };
            techBulletDamage3.UnlockValues = new double[] { 0.2 };
            techBulletDamage3.Level = 3;
            techBulletDamage3.MaxLevel = 3;
            techBulletDamage3.LevelCoef1 = 0;
            techBulletDamage3.LevelCoef2 = 0;
            TechProto techBulletDamageInf = ProtoRegistry.RegisterTech(4904, "定向爆破4", "定向爆破描述", "定向爆破结论", "Icons/Tech/1112", new int[] { 4903 }, new int[] { 6006 },
                new int[] { 30 }, 600000, new int[] { }, new Vector2(21, -43));
            techBulletDamageInf.UnlockFunctions = new int[] { 50 };
            techBulletDamageInf.UnlockValues = new double[] { 0.2 };
            techBulletDamageInf.Level = 4;
            techBulletDamageInf.MaxLevel = 10000;
            techBulletDamageInf.LevelCoef1 = 60;
            techBulletDamageInf.LevelCoef2 = 12;


            TechProto techBulletSpeed1 = ProtoRegistry.RegisterTech(4911, "引力波引导1", "引力波引导描述", "引力波引导结论", "Icons/Tech/1112", new int[] { }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24, 24 }, 150000, new int[] { }, new Vector2(25, -43));
            techBulletSpeed1.PreTechsImplicit = new int[] { 1705 };
            techBulletSpeed1.UnlockFunctions = new int[] { 51 };
            techBulletSpeed1.UnlockValues = new double[] { 0.1 };
            techBulletSpeed1.Level = 1;
            techBulletSpeed1.MaxLevel = 1;
            techBulletSpeed1.LevelCoef1 = 0;
            techBulletSpeed1.LevelCoef2 = 0;
            TechProto techBulletSpeed2 = ProtoRegistry.RegisterTech(4912, "引力波引导2", "引力波引导描述", "引力波引导结论", "Icons/Tech/1112", new int[] { 4911 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                 new int[] { 24, 24, 24, 24, 24, 24 }, 300000, new int[] { }, new Vector2(29, -43));
            techBulletSpeed2.UnlockFunctions = new int[] { 51 };
            techBulletSpeed2.UnlockValues = new double[] { 0.1 };
            techBulletSpeed2.Level = 2;
            techBulletSpeed2.MaxLevel = 2;
            techBulletSpeed2.LevelCoef1 = 0;
            techBulletSpeed2.LevelCoef2 = 0;
            TechProto techBulletSpeed3 = ProtoRegistry.RegisterTech(4913, "引力波引导3", "引力波引导描述", "引力波引导结论", "Icons/Tech/1112", new int[] { 4912 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24, 24 }, 600000, new int[] { }, new Vector2(33, -43));
            techBulletSpeed3.UnlockFunctions = new int[] { 51 };
            techBulletSpeed3.UnlockValues = new double[] { 0.1 };
            techBulletSpeed3.Level = 3;
            techBulletSpeed3.MaxLevel = 3;
            techBulletSpeed3.LevelCoef1 = 0;
            techBulletSpeed3.LevelCoef2 = 0;
            TechProto techBulletSpeedInf = ProtoRegistry.RegisterTech(4914, "引力波引导4", "引力波引导描述", "引力波引导结论", "Icons/Tech/1112", new int[] { 4913 }, new int[] { 6006 },
                new int[] { 24 }, 150000, new int[] { }, new Vector2(37, -43));
            techBulletSpeedInf.UnlockFunctions = new int[] { 51 };
            techBulletSpeedInf.UnlockValues = new double[] { 0.1 };
            techBulletSpeedInf.Level = 4;
            techBulletSpeedInf.MaxLevel = 100;
            techBulletSpeedInf.LevelCoef1 = 60;
            techBulletSpeedInf.LevelCoef2 = 24;

            TechProto techWormDistance1 = ProtoRegistry.RegisterTech(4921, "相位干扰技术1", "相位干扰技术描述", "相位干扰技术结论", "Icons/Tech/1112", new int[] { }, new int[] { 6002, 6003, 6005 },
                new int[] { 30, 30, 30 }, 150000, new int[] { }, new Vector2(9, -47));
            techWormDistance1.PreTechsImplicit = new int[] { 1914 };
            techWormDistance1.UnlockFunctions = new int[] { 52 };
            techWormDistance1.UnlockValues = new double[] { 0.03 }; //每次升级向200k的最大距离逼近剩余距离的x%，对于0.1，则是10%
            techWormDistance1.Level = 1;
            techWormDistance1.MaxLevel = 1;
            techWormDistance1.LevelCoef1 = 0;
            techWormDistance1.LevelCoef2 = 0;
            TechProto techWormDistance2 = ProtoRegistry.RegisterTech(4922, "相位干扰技术2", "相位干扰技术描述", "相位干扰技术结论", "Icons/Tech/1112", new int[] { 4921 }, new int[] { 6002, 6003, 6005 },
                new int[] { 40, 40, 40 }, 150000, new int[] { }, new Vector2(13, -47));
            techWormDistance2.UnlockFunctions = new int[] { 52 };
            techWormDistance2.UnlockValues = new double[] { 0.05 }; //每次升级向200k的最大距离逼近剩余距离的x%，对于0.1，则是10%
            techWormDistance2.Level = 2;
            techWormDistance2.MaxLevel = 2;
            techWormDistance2.LevelCoef1 = 0;
            techWormDistance2.LevelCoef2 = 0;
            TechProto techWormDistance3 = ProtoRegistry.RegisterTech(4923, "相位干扰技术3", "相位干扰技术描述", "相位干扰技术结论", "Icons/Tech/1112", new int[] { 4922 }, new int[] { 6002, 6003, 6005 },
                 new int[] { 50, 50, 50 }, 150000, new int[] { }, new Vector2(17, -47));
            techWormDistance3.UnlockFunctions = new int[] { 52 };
            techWormDistance3.UnlockValues = new double[] { 0.08 }; //每次升级向200k的最大距离逼近剩余距离的x%，对于0.1，则是10%
            techWormDistance3.Level = 3;
            techWormDistance3.MaxLevel = 3;
            techWormDistance3.LevelCoef1 = 0;
            techWormDistance3.LevelCoef2 = 0;
            TechProto techWormDistanceInf = ProtoRegistry.RegisterTech(4924, "相位干扰技术4", "相位干扰技术描述", "相位干扰技术结论", "Icons/Tech/1112", new int[] { 4923 }, new int[] { 6006 },
                 new int[] { 60 }, 150000, new int[] { }, new Vector2(21, -47));
            techWormDistanceInf.UnlockFunctions = new int[] { 52 };
            techWormDistanceInf.UnlockValues = new double[] { 0.1 }; //每次升级向200k的最大距离逼近剩余距离的x%，对于0.1，则是10%
            techWormDistanceInf.Level = 4;
            techWormDistanceInf.MaxLevel = 10000;
            techWormDistanceInf.LevelCoef1 = 60;
            techWormDistanceInf.LevelCoef2 = 12;



            var CannonModel = CopyModelProto(72, 311, Color.red);
            CannonModel.prefabDesc.ejectorBulletId = 8001; //子弹的Id
            CannonModel.prefabDesc.ejectorChargeFrame = 40; //充能时间（所需帧数，下同）
            CannonModel.prefabDesc.ejectorColdFrame = 20; //冷却时间
            LDBTool.PreAddProto(CannonModel);

            var CannonModel2 = CopyModelProto(72, 312, Color.green);
            CannonModel2.prefabDesc.ejectorBulletId = 8001; //子弹的Id
            CannonModel2.prefabDesc.ejectorChargeFrame = 20; 
            CannonModel2.prefabDesc.ejectorColdFrame = 10;
            CannonModel2.prefabDesc.workEnergyPerTick = 80000;
            CannonModel2.prefabDesc.idleEnergyPerTick = 2000;
            LDBTool.PreAddProto(CannonModel2);

            var CannonModel3 = CopyModelProto(72, 314, Color.yellow);
            CannonModel3.prefabDesc.ejectorBulletId = 8007; //子弹的Id
            CannonModel3.prefabDesc.ejectorChargeFrame = 1; 
            CannonModel3.prefabDesc.ejectorColdFrame = 1;
            CannonModel3.prefabDesc.workEnergyPerTick = 300000;
            CannonModel3.prefabDesc.idleEnergyPerTick = 2000;
            LDBTool.PreAddProto(CannonModel3);

            var SiloModel = CopyModelProto(74, 313, Color.red);
            SiloModel.prefabDesc.siloBulletId = 8004; // 导弹的Id
            SiloModel.prefabDesc.siloChargeFrame = 180;
            SiloModel.prefabDesc.siloColdFrame = 120;
            LDBTool.PreAddProto(SiloModel);

        }

        private static ModelProto CopyModelProto(int oriId, int id, Color color)
        {
            var oriModel = LDB.models.Select(oriId);
            var model = oriModel.Copy();
            model.name = id.ToString();
            model.Name = id.ToString();//这俩至少有一个必须加，否则LDBTool报冲突导致后面null
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
            ItemProto.InitFluids();
            ItemProto.InitItemIds();
            ItemProto.InitFuelNeeds();
            ItemProto.InitItemIndices();
            ItemProto.InitMechaMaterials();

            foreach (var proto in LDB.items.dataArray)
            {
                StorageComponent.itemIsFuel[proto.ID] = proto.HeatValue > 0L;
                StorageComponent.itemStackCount[proto.ID] = proto.StackSize;
            }

            LDB.models.OnAfterDeserialize();
            LDB.models.Select(311).prefabDesc.modelIndex = 311;
            LDB.items.Select(8011).ModelIndex = 311;
            LDB.items.Select(8011).prefabDesc = LDB.models.Select(311).prefabDesc;

            LDB.models.Select(312).prefabDesc.modelIndex = 312;
            LDB.items.Select(8012).ModelIndex = 312;
            LDB.items.Select(8012).prefabDesc = LDB.models.Select(312).prefabDesc;

            LDB.models.Select(314).prefabDesc.modelIndex = 314;
            LDB.items.Select(8014).ModelIndex = 314;
            LDB.items.Select(8014).prefabDesc = LDB.models.Select(314).prefabDesc;

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
                    Configs.wormholeRangeAdded += (int)((490000 - Configs.wormholeRangeAdded)*value);
                    break;
                default:
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TechProto), "UnlockFunctionText")]
        public static void UnlockFunctionTextPatch(ref TechProto __instance, ref string __result, StringBuilder sb)
        {
            if (__instance.ID >= 4901 && __instance.ID <= 4904)
                __result = "子弹伤害和导弹伤害+20%".Translate() + "\n" + "脉冲中子束伤害+40%".Translate() ;
            else if(__instance.ID >= 4911 && __instance.ID <= 4914)
                __result = "子弹飞行速度+10%".Translate()+"\n"+"火箭飞行速度+2%".Translate();
            else if (__instance.ID == 4921)
                __result = "虫洞生成最近范围向10AU推进3%".Translate();
            else if (__instance.ID == 4922)
                __result = "虫洞生成最近范围向10AU推进5%".Translate();
            else if (__instance.ID == 4923)
                __result = "虫洞生成最近范围向10AU推进8%".Translate();
            else if (__instance.ID == 4924)
                __result = "虫洞生成最近范围向10AU推进10%".Translate();
        }
    }
}
