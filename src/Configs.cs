using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DSP_Battle
{
    public class Configs
    {
        public static string versionString = "2.2.5";
        public static string qq = "694213906";
        public static bool developerMode = true; //发布前务必修改！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
        
        public static bool enableProliferator4 = false;
        public static bool enableBattleBGM = true;
        public static bool enableAlertTextGlowing = true;
        public static int versionWhenImporting = -1;
        public static int versionCode = 30230426;


        public static int difficulty = 0; // -1 easy, 0 normal, 1 hard

        // --- 子弹信息
        public static double _bullet1Speed;

        public static int _bullet1Atk;

        public static double _bullet2Speed;

        public static int _bullet2Atk;

        public static double _bullet3Speed;

        public static int _bullet3Atk;

        public static double _bullet4Speed;

        public static int _bullet4Atk;

        public static double _missile1Speed;

        public static int _missile1Atk;

        public static int missile1Range;

        public static double _missile2Speed;

        public static int _missile2Atk;

        public static int missile2Range;

        public static double _missile3Speed;

        public static int _missile3Atk;

        public static int _lightSpearAtk;

        public static int missile3Range;

        public static int dropletAtk = 400000;

        public static double dropletSpd = 30000.0;

        public static double laserDamageReducePerAU = 0.2;

        // --- 敌方战舰信息
        public static int[] enemyIntensity = new int[10];

        public static int[] enemyHp = new int[10];

        public static float[] enemySpeed = new float[10];

        public static int[] enemyRange = new int[10];

        public static int[] enemyItemIds = new int[] { 8040, 8041, 8042, 8043, 8044, 8044 };
        // public static int[] enemyItemIds = new int[] { 6001, 6002, 6003, 6004, 6005, 6006 };

        public static int[] enemyLandCnt = new int[] { 1, 3, 5, 5, 10000 };

        public static int[] enemyFireInterval = new int[] {120, 60, int.MaxValue, 60, 60 }; //每攻击一次间隔的tick0

        public static int[] enemyFireRange = new int[] { 5000, 8000, 15000, 15000, 30000 }; //射程

        public static int[] enemyDamagePerBullet = new int[] { 500, 500, 200000, 500, 2000 }; //dps = 250,500,-,500,2000, dps per intensity = 250,125,-,42,133
        //public static int[] enemyDamagePerBullet = new int[] { 2, 2, 3, 200, 2 };

        //public static int[] enemyBulletSpeed = new int[] {20000, 30000, 50000, 80000, 150000 }; //虽然有speed设置，但是为了减少运算，子弹伤害是在发射时就结算的。speed应该设置的比较大来减少视觉上的误差

        public static int[] enemyIntensity2TypeMap = new int[] { 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4 };
        //-------------------------------------------------------0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15,16,17,18,19,20,21,22,23

        public static int eliteDurationFrames = 3600 * 3; // 精英进攻持续的时间

        // --- 虫洞信息
        public static int _wormholeRange;

        public static int[] intensity;

        public static int[] coldTime;

        public static int[] expectationMatrices;

        public static double bulletSpeedScale = 1.0;
        public static double bulletAtkScale = 1.0;
        public static int wormholeRangeAdded = 0;

        // --- 战斗信息
        public static long nextWaveFrameIndex = -1;
        public static int nextWaveStarIndex = -1;
        public static int nextWaveState = 0; // 0: not generated; 1: wave generated; 2: wormhole generated; 3: in battle

        public static long nextWaveDelay = 0;
        public static long extraSpeedFrame = -1;
        public static bool extraSpeedEnabled = false;

        public static int nextWaveIntensity = 0;
        public static int nextWaveWormCount = 0;
        public static int[] nextWaveEnemy = new int[10];
        public static int nextWaveElite = 0;

        public static int nextWaveMatrixExpectation = 0;

        public static int starCount = 100;
        public static int[] wavePerStar;

        public static bool isEnemyWeakenedByRelic = false; // relic1-3此项每帧更新，不存档，也不在读档时重置
        public static int relic2_17Activated = 0; // 下个护盾在被摧毁时立刻回填50%上限的护盾量。此项在战斗开始时刷新，读档时设置为false，不存档
        public static int relic1_8Protection = 99; // 前十个降落的敌舰不摧毁建筑。此项在战斗开始时刷新,此项读档时设置为99，不存档

        // --- 护盾信息
        public static int shieldGenPerTick = 12; //每个生成器提供的回复速度不会因为护盾生成器数量变多而有削减，是线性的增长，回复速度如此之慢是为保留：可能加入的，回复护盾的，巨构的存在价值
        public static List<Tuple<int, int>> capacityPerGenerator =
            new List<Tuple<int, int>> {
                new Tuple<int, int>(0, 5000000),
                new Tuple<int, int>(5000000, 3000000),
                new Tuple<int, int>(10000000, 2000000),
                new Tuple<int, int>(30000000, 1000000),
                new Tuple<int, int>(50000000, 0) };

        //Rank信息
        public static int[] expToNextRank = new int[] { 10, 100, 800, 3000, 20000, 100000, 500000, 1000000, 8000000, 30000000, 0, 0, 0};
        public static float[] rewardTimeRatio = new float[] { 1.0f, 1.0f, 1.2f, 1.2f, 1.4f, 1.4f, 1.6f, 1.6f, 1.8f, 1.8f, 2.0f, 2.0f, 2.0f, 2.0f };
        public static float[] expRatioByDifficulty = new float[] { 0.75f, 1f, 1.5f };
        public static int expPerAlienMeta = 20; //每个解码后的异星元数据上传提供的基础功勋点数

        // 遗物信息 决定直接写入函数不再读取
        //public static double[] relic0settings = { 0.1, -1, -1, 1, -1, 0.1, 500, 20, 1, -1 }; // 传说遗物的功能概率或数值设定，不涉及概率和数值的用任意数字占位（最好是-1）
        //public static double[] relic1settings = { 0.1, -1, -1, 1, -1, 0.1, 500, 20, 1, -1 }; // 史诗遗物的功能概率或数值设定
        //public static double[] relic2settings = { 0.1, -1, -1, 1, -1, 0.1, 500, 20, 1, -1 }; // 稀有遗物的功能概率或数值设定
        //public static double[] relic3settings = { 0.1, -1, -1, 1, -1, 0.1, 500, 20, 1, -1 }; // 普通遗物的功能概率或数值设定

        public static int totalWave
        {
            get { return wavePerStar.Sum(); }
        }

        public static double bullet1Speed
        {
            get { return _bullet1Speed * bulletSpeedScale; }
        }
        public static double bullet2Speed
        {
            get { return _bullet2Speed * bulletSpeedScale; }
        }
        public static double bullet3Speed
        {
            get { return _bullet3Speed * bulletSpeedScale; }
        }
        public static double bullet4Speed //不会被循环科技加成，因为足够快了
        {
            get { return _bullet4Speed; }
        }

        public static int bullet1Atk
        {
            get { return Mathf.RoundToInt((float)(_bullet1Atk * bulletAtkScale)); }
        }
        public static int bullet2Atk
        {
            get { return Mathf.RoundToInt((float)(_bullet2Atk * bulletAtkScale)); }
        }
        public static int bullet3Atk
        {
            get { return Mathf.RoundToInt((float)(_bullet3Atk * bulletAtkScale)); }
        }
        public static int bullet4Atk //激光炮从伤害的循环科技中获得双倍加成
        {
            get { return Mathf.RoundToInt((float)(_bullet4Atk * (1.0 + (Configs.bulletAtkScale - 1.0) * 2))); }
        }
        public static int lightSpearAtk
        {
            get { return Mathf.RoundToInt((float)(_lightSpearAtk * (1.0 + (Configs.bulletAtkScale - 1.0) * 2))); }
        }
        public static double missile1Speed
        {
            get { return _missile1Speed * (1.0 + (bulletSpeedScale - 1.0) * 0.5); }
        }
        public static int missile1Atk
        {
            get { return Mathf.RoundToInt((float)(_missile1Atk * bulletAtkScale)); }
        }
        public static double missile2Speed
        {
            get { return _missile2Speed * (1.0 + (bulletSpeedScale - 1.0) * 0.5); }
        }
        public static int missile2Atk
        {
            get { return Mathf.RoundToInt((float)(_missile2Atk * bulletAtkScale)); }
        }
        public static double missile3Speed
        {
            get { return _missile3Speed * (1.0 + (bulletSpeedScale - 1.0) * 0.5); }
        }
        public static int missile3Atk
        {
            get { return Mathf.RoundToInt((float)(_missile3Atk * bulletAtkScale)); }
        }
        public static int wormholeRange
        {
            get { return _wormholeRange + wormholeRangeAdded; }
        }


        public static void Init(ConfigFile config)
        {
            _bullet1Speed = 7500.0; // config.Bind("config", "bullet1Speed", defaultValue: 5000.0, "穿甲磁轨弹速度").Value;
            _bullet1Atk = 100; // config.Bind("config", "bullet1Atk", defaultValue: 100, "穿甲磁轨弹攻击力").Value;
            _bullet2Speed = 7500.0; // config.Bind("config", "bullet2Speed", defaultValue: 5000.0, "强酸磁轨弹速度").Value;
            _bullet2Atk = 300; // config.Bind("config", "bullet2Atk", defaultValue: 180, "强酸磁轨弹攻击力").Value;
            _bullet3Speed = 7500.0; // config.Bind("config", "bullet3Speed", defaultValue: 5000.0, "氘核爆破弹速度").Value;
            _bullet3Atk = 500; // config.Bind("config", "bullet3Atk", defaultValue: 400, "氘核爆破弹攻击力").Value;
            _bullet4Speed = 250000.0; //  config.Bind("config", "bullet4Speed", defaultValue: 250000.0, "中子脉冲束速度").Value;
            _bullet4Atk = 10; //  config.Bind("config", "bullet4Atk", defaultValue: 10, "中子脉冲束攻击力").Value;
            _lightSpearAtk = 20000;

            _missile1Speed = 10000.0; // config.Bind("config", "missile1Speed", defaultValue: 5000.0, "热核导弹速度（米每秒）").Value;
            _missile1Atk = 5000; // config.Bind("config", "missile1Atk", defaultValue: 5000, "热核导弹攻击力").Value;
            missile1Range = 400; // config.Bind("config", "missile1Range", defaultValue: 400, "热核导弹破坏范围").Value;

            _missile2Speed = 10000.0; // config.Bind("config", "missile2Speed", defaultValue: 5000.0, "反物质导弹速度（米每秒）").Value;
            _missile2Atk = 20000; // config.Bind("config", "missile2Atk", defaultValue: 20000, "反物质导弹攻击力").Value;
            missile2Range = 500; // config.Bind("config", "missile2Range", defaultValue: 500, "反物质导弹破坏范围").Value;

            _missile3Speed = 12000.0; // config.Bind("config", "missile3Speed", defaultValue: 8000.0, "引力塌陷导弹速度（米每秒）").Value;
            _missile3Atk = 1500;//00; // config.Bind("config", "missile3Atk", defaultValue: 2500, "引力塌陷导弹攻击力").Value;
            missile3Range = 8000; // config.Bind("config", "missile3Range", defaultValue: 2000, "引力塌陷导弹破坏范围").Value;

            enemyIntensity[0] = 1; // 无特殊效果
            enemyHp[0] = 4000; // config.Bind("config", "enemy1Hp", defaultValue: 4000, "敌方飞船1血量").Value;
            enemySpeed[0] = 1000f; // config.Bind("config", "enemy1Speed", defaultValue: 1500f, "敌方飞船1速度（米每秒）").Value;
            enemyRange[0] = 20; // config.Bind("config", "enemy1Range", defaultValue: 20, "敌方飞船1破坏范围").Value;
            
            enemyIntensity[1] = 4; // 对子弹类型的伤害（不包括相位）具有90%闪避概率
            enemyHp[1] = 20000; // config.Bind("config", "enemy2Hp", defaultValue: 20000, "敌方飞船2血量").Value;
            enemySpeed[1] = 1500f; // config.Bind("config", "enemy2Speed", defaultValue: 2000f, "敌方飞船2速度（米每秒）").Value;
            enemyRange[1] = 40; // config.Bind("config", "enemy2Range", defaultValue: 40, "敌方飞船2破坏范围").Value;
            
            enemyIntensity[2] = 6; // 会直接撞向护盾，摧毁自己并对护盾造成巨量伤害
            enemyHp[2] = 10000; // config.Bind("config", "enemy3Hp", defaultValue: 10000, "敌方飞船3血量").Value;
            enemySpeed[2] = 3000f; // config.Bind("config", "enemy3Speed", defaultValue: 5000f, "敌方飞船3速度（米每秒）").Value;
            enemyRange[2] = 60; // config.Bind("config", "enemy3Range", defaultValue: 60, "敌方飞船3破坏范围").Value;

            enemyIntensity[3] = 12; // 对aoe类型的伤害（导弹的所有次要目标以及巨构的aoe伤害）具有90%伤害减免，免疫控制效果
            enemyHp[3] = 100000; // config.Bind("config", "enemy4Hp", defaultValue: 120000, "敌方飞船4血量").Value;
            enemySpeed[3] = 1000f; // config.Bind("config", "enemy4Speed", defaultValue: 1000f, "敌方飞船4速度（米每秒）").Value;
            enemyRange[3] = 120; // config.Bind("config", "enemy4Range", defaultValue: 80, "敌方飞船4破坏范围").Value;

            enemyIntensity[4] = 15; // 对能量类（相位炮、巨构伤害）和护盾类（护盾反伤、护盾直接碰撞伤害）的伤害具有80%伤害减免
            enemyHp[4] = 80000; // config.Bind("config", "enemy5Hp", defaultValue: 80000, "敌方飞船5血量").Value;
            enemySpeed[4] = 1500f; // config.Bind("config", "enemy5Speed", defaultValue: 3000f, "敌方飞船5速度（米每秒）").Value;
            enemyRange[4] = 100; // config.Bind("config", "enemy5Range", defaultValue: 100, "敌方飞船5破坏范围").Value;

            _wormholeRange = 20000; // config.Bind("config", "wormholeRange", defaultValue: 20000, "初始虫洞刷新范围，米为单位").Value;

            if (Configs.developerMode)
            {
                //dropletAtk = 400000;
            }

            intensity =
                new int[] { 2, 5, 10, 15, 20, 30, 50, 80, 100, 150, 250, 300, 400, 500, 600, 800, 1000, 1100, 1500, 1800, 2000, 2200, 2500, 3000, 3500, 4000 };
            // config.Bind("config", "intensity", defaultValue: "2,5,10,15,20,30,50,80,100,150,250,300,400,500,600,800,1000,1100,1500,1800,2000,2500,3000,4000", "每波总强度（以逗号分隔）")
            // .Value.Split(',').Select(e => int.Parse(e)).ToArray();

            coldTime =
                new int[] { 60, 50, 50, 45, 45, 45, 40, 40, 40, 40, 30, 30, 30, 30, 30, 20, 20, 20, 20, 20, 15, 15, 15, 15, 15, 10 };
            // config.Bind("config", "coldTime", defaultValue: "60,50,50,45,45,45,40,40,40,40,30,30,30,30,30,20,20,20,20,20,15,15,15,15,15,10", "相邻两波间隔时间").Value.Split(',').Select(e => int.Parse(e)).ToArray();

            expectationMatrices =
                new int[] { 25, 35, 45, 55, 75, 100, 155, 205, 255, 305, 310, 315, 320, 325, 330, 335, 340, 345, 350, 355, 360, 380, 400, 425, 450, 500, 600, 700, 800 };
            //               2, 5, 10, 15, 20, 30,  50,  80,   100, 150, 250, 300, 400, 500, 600, 800, 1000, 1100, 1500, 1800, 2000, 2200, 2500, 3000, 3500, 4000

            IntoOtherSave();
        }

        public static void Export(BinaryWriter w)
        {
            w.Write(versionCode);

            w.Write(difficulty);

            w.Write(bulletSpeedScale);
            w.Write(bulletAtkScale);
            w.Write(wormholeRangeAdded);

            w.Write(nextWaveFrameIndex);
            w.Write(nextWaveStarIndex);
            w.Write(nextWaveState);
            w.Write(nextWaveDelay);

            w.Write(extraSpeedFrame);
            w.Write(extraSpeedEnabled);

            w.Write(nextWaveIntensity);
            
            w.Write(nextWaveWormCount);

            for (var i = 0; i < 10; ++i) w.Write(nextWaveEnemy[i]);

            w.Write(starCount);
            for (var i = 0; i < starCount; ++i) w.Write(wavePerStar[i]);

            w.Write(nextWaveMatrixExpectation);

            w.Write(nextWaveElite);
        }

        public static void Import(BinaryReader r)
        {
            int importVersion = r.ReadInt32();
            versionWhenImporting = importVersion;
            difficulty = r.ReadInt32();

            bulletSpeedScale = r.ReadDouble();
            bulletAtkScale = r.ReadDouble();
            wormholeRangeAdded = r.ReadInt32();

            nextWaveFrameIndex = r.ReadInt64();
            nextWaveStarIndex = r.ReadInt32();
            nextWaveState = r.ReadInt32();
            nextWaveDelay = r.ReadInt64();

            extraSpeedFrame = r.ReadInt64();
            extraSpeedEnabled = r.ReadBoolean();

            nextWaveIntensity = r.ReadInt32();

            nextWaveWormCount = r.ReadInt32();

            for (var i = 0; i < 10; ++i) nextWaveEnemy[i] = r.ReadInt32();

            if (importVersion >= 20220320)
            {
                starCount = r.ReadInt32();
                wavePerStar = new int[starCount];
                for (var i = 0; i < starCount; ++i) wavePerStar[i] = r.ReadInt32();
            }
            else
            {
                starCount = Mathf.Max(GameMain.galaxy?.starCount ?? 90, 90) + 10;
                wavePerStar = new int[starCount];
                for (var i = 0; i < 100; ++i) wavePerStar[i] = r.ReadInt32();
            }

            if(importVersion >= 30220420)
            {
                nextWaveMatrixExpectation = r.ReadInt32();
            }
            else
            {
                nextWaveMatrixExpectation = expectationMatrices[0];
                if(nextWaveState>0 && nextWaveStarIndex>=0)
                {
                    nextWaveMatrixExpectation = expectationMatrices[Math.Min(expectationMatrices.Length-1, wavePerStar[nextWaveStarIndex])];
                }
            }

            if (importVersion >= 30221029)
            {
                nextWaveElite = r.ReadInt32();
            }
            else
            {
                nextWaveElite = 0;
            }

        }

        public static void IntoOtherSave()
        {
            difficulty = 0;

            bulletSpeedScale = 1.0;
            bulletAtkScale = 1.0;
            wormholeRangeAdded = 0;

            nextWaveFrameIndex = -1;
            nextWaveStarIndex = -1;
            nextWaveState = 0;
            nextWaveDelay = 0;

            extraSpeedFrame = -1;
            extraSpeedEnabled = false;

            nextWaveIntensity = 0;
            nextWaveWormCount = 0;

            nextWaveEnemy = new int[10];

            starCount = Mathf.Max(GameMain.galaxy?.starCount ?? 90, 90) + 10;
            wavePerStar = new int[starCount];
            nextWaveMatrixExpectation = expectationMatrices[0];

            nextWaveElite = 0;
        }

    }
}
