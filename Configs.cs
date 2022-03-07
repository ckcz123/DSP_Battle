using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    class Configs
    {
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

        public static int missile3Range;

        // --- 敌方战舰信息
        public static int[] enemyIntensity = new int[10];

        public static int[] enemyHp = new int[10];

        public static float[] enemySpeed = new float[10];

        public static int[] enemyRange = new int[10];

        public static int[] enemyItemIds = new int[] { 6001, 6002, 6003, 6004, 6005, 6006 };

        public static int[] enemyLandCnt = new int[] { 1, 3, 5, 5, 10000 };

        // --- 虫洞信息
        public static int _wormholeRange;

        public static int[] intensity;

        public static int[] coldTime;

        public static double bulletSpeedScale = 1.0;
        public static double bulletAtkScale = 1.0;
        public static int wormholeRangeAdded = 0;

        // --- 战斗信息
        public static long nextWaveFrameIndex = -1;
        public static int nextWavePlanetId = -1;
        public static int nextWaveState = 0; // 0: not generated; 1: wave generated; 2: wormhole generated; 3: in battle

        public static int nextWaveIntensity = 0;
        public static int nextWaveWormCount = 0;
        public static int[] nextWaveEnemy = new int[10];
        public static int[] nextWaveAngle1 = new int[100];
        public static int[] nextWaveAngle2 = new int[100];

        public static int[] wavePerStar = new int[100];

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
            get { return Mathf.RoundToInt((float)(_bullet4Atk * (1.0 + (Configs.bulletAtkScale - 1.0) * 5 / 3))); }
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
            _bullet1Speed = config.Bind("config", "bullet1Speed", defaultValue: 5000.0, "穿甲磁轨弹速度").Value;
            _bullet1Atk = config.Bind("config", "bullet1Atk", defaultValue: 100, "穿甲磁轨弹攻击力").Value;
            _bullet2Speed = config.Bind("config", "bullet2Speed", defaultValue: 5000.0, "强酸磁轨弹速度").Value;
            _bullet2Atk = config.Bind("config", "bullet2Atk", defaultValue: 180, "强酸磁轨弹攻击力").Value;
            _bullet3Speed = config.Bind("config", "bullet3Speed", defaultValue: 5000.0, "氘核爆破弹速度").Value;
            _bullet3Atk = config.Bind("config", "bullet3Atk", defaultValue: 400, "氘核爆破弹攻击力").Value;
            _bullet4Speed = config.Bind("config", "bullet4Speed", defaultValue: 250000.0, "中子脉冲束速度").Value;
            _bullet4Atk = config.Bind("config", "bullet4Atk", defaultValue: 10, "中子脉冲束攻击力").Value;

            _missile1Speed = config.Bind("config", "missile1Speed", defaultValue: 5000.0, "热核导弹速度（米每秒）").Value;
            _missile1Atk = config.Bind("config", "missile1Atk", defaultValue: 5000, "热核导弹攻击力").Value;
            missile1Range = config.Bind("config", "missile1Range", defaultValue: 500, "热核导弹破坏范围").Value;

            _missile2Speed = config.Bind("config", "missile2Speed", defaultValue: 5000.0, "反物质导弹速度（米每秒）").Value;
            _missile2Atk = config.Bind("config", "missile2Atk", defaultValue: 20000, "反物质导弹攻击力").Value;
            missile2Range = config.Bind("config", "missile2Range", defaultValue: 800, "反物质导弹破坏范围").Value;

            _missile3Speed = config.Bind("config", "missile3Speed", defaultValue: 5000.0, "引力塌陷导弹速度（米每秒）").Value;
            _missile3Atk = config.Bind("config", "missile3Atk", defaultValue: 2500, "引力塌陷导弹攻击力").Value;
            missile3Range = config.Bind("config", "missile3Range", defaultValue: 5000, "引力塌陷导弹破坏范围").Value;

            enemyIntensity[0] = config.Bind("config", "enemy1Intensity", defaultValue: 1, "敌方飞船1强度").Value;
            enemyHp[0] = config.Bind("config", "enemy1Hp", defaultValue: 4000, "敌方飞船1血量").Value;
            enemySpeed[0] = config.Bind("config", "enemy1Speed", defaultValue: 1500f, "敌方飞船1速度（米每秒）").Value;
            enemyRange[0] = config.Bind("config", "enemy1Range", defaultValue: 20, "敌方飞船1破坏范围").Value;

            enemyIntensity[1] = config.Bind("config", "enemy2Intensity", defaultValue: 4, "敌方飞船2强度").Value;
            enemyHp[1] = config.Bind("config", "enemy2Hp", defaultValue: 10000, "敌方飞船2血量").Value;
            enemySpeed[1] = config.Bind("config", "enemy2Speed", defaultValue: 2000f, "敌方飞船2速度（米每秒）").Value;
            enemyRange[1] = config.Bind("config", "enemy2Range", defaultValue: 40, "敌方飞船2破坏范围").Value;

            enemyIntensity[2] = config.Bind("config", "enemy3Intensity", defaultValue: 8, "敌方飞船3强度").Value;
            enemyHp[2] = config.Bind("config", "enemy3Hp", defaultValue: 10000, "敌方飞船3血量").Value;
            enemySpeed[2] = config.Bind("config", "enemy3Speed", defaultValue: 5000f, "敌方飞船3速度（米每秒）").Value;
            enemyRange[2] = config.Bind("config", "enemy3Range", defaultValue: 60, "敌方飞船3破坏范围").Value;

            enemyIntensity[3] = config.Bind("config", "enemy4Intensity", defaultValue: 8, "敌方飞船4强度").Value;
            enemyHp[3] = config.Bind("config", "enemy4Hp", defaultValue: 60000, "敌方飞船4血量").Value;
            enemySpeed[3] = config.Bind("config", "enemy4Speed", defaultValue: 750f, "敌方飞船4速度（米每秒）").Value;
            enemyRange[3] = config.Bind("config", "enemy4Range", defaultValue: 80, "敌方飞船4破坏范围").Value;

            enemyIntensity[4] = config.Bind("config", "enemy5Intensity", defaultValue: 15, "敌方飞船5强度").Value;
            enemyHp[4] = config.Bind("config", "enemy5Hp", defaultValue: 80000, "敌方飞船5血量").Value;
            enemySpeed[4] = config.Bind("config", "enemy5Speed", defaultValue: 3000f, "敌方飞船5速度（米每秒）").Value;
            enemyRange[4] = config.Bind("config", "enemy5Range", defaultValue: 100, "敌方飞船5破坏范围").Value;

            _wormholeRange = config.Bind("config", "wormholeRange", defaultValue: 20000, "初始虫洞刷新范围，米为单位").Value;

            intensity = config.Bind("config", "intensity", defaultValue: "2,5,10,15,20,30,50,80,100,150,250,300,400,500,600,800,1000,1100,1500,1800,2000,2500,3000,4000", "每波总强度（以逗号分隔）")
                .Value.Split(',').Select(e=>int.Parse(e)).ToArray();

            coldTime = config.Bind("config", "coldTime", defaultValue: "60,50,50,45,45,45,40,40,40,40,30,30,30,30,30,20,20,20,20,20,15,15,15,15,15,10", "相邻两波间隔时间").Value.Split(',').Select(e => int.Parse(e)).ToArray();
        }

        public static void Export(BinaryWriter w)
        {
            w.Write(bulletSpeedScale);
            w.Write(bulletAtkScale);
            w.Write(wormholeRangeAdded);

            w.Write(nextWaveFrameIndex);
            w.Write(nextWavePlanetId);
            w.Write(nextWaveState);

            w.Write(nextWaveIntensity);
            w.Write(nextWaveWormCount);

            for (var i = 0; i < 10; ++i) w.Write(nextWaveEnemy[i]);
            for (var i = 0; i < 100; ++i) w.Write(nextWaveAngle1[i]);
            for (var i = 0; i < 100; ++i) w.Write(nextWaveAngle2[i]);
            for (var i = 0; i < 100; ++i) w.Write(wavePerStar[i]);

        }

        public static void Import(BinaryReader r)
        {
            bulletSpeedScale = r.ReadDouble();
            bulletAtkScale = r.ReadDouble();
            wormholeRangeAdded = r.ReadInt32();

            nextWaveFrameIndex = r.ReadInt64();
            nextWavePlanetId = r.ReadInt32();
            nextWaveState = r.ReadInt32();

            nextWaveIntensity = r.ReadInt32();
            nextWaveWormCount = r.ReadInt32();

            for (var i = 0; i < 10; ++i) nextWaveEnemy[i] = r.ReadInt32();
            for (var i = 0; i < 100; ++i) nextWaveAngle1[i] = r.ReadInt32();
            for (var i = 0; i < 100; ++i) nextWaveAngle2[i] = r.ReadInt32();
            for (var i = 0; i < 100; ++i) wavePerStar[i] = r.ReadInt32();
        }

        public static void IntoAnotherSave()
        {
            bulletSpeedScale = 1.0;
            bulletAtkScale = 1.0;
            wormholeRangeAdded = 0;

            nextWaveFrameIndex = -1;
            nextWavePlanetId = -1;
            nextWaveState = 0;

            nextWaveIntensity = 0;
            nextWaveWormCount = 0;

            nextWaveEnemy = new int[10];
            nextWaveAngle1 = new int[100];
            nextWaveAngle2 = new int[100];
            wavePerStar = new int[100];
        }

    }
}
