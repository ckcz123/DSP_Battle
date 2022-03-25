using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    class WaveStages
    {
        public static void Update(long time)
        {
            switch (Configs.nextWaveState)
            {
                case 0: UpdateWaveStage0(time); break;
                case 1: UpdateWaveStage1(time); break;
                case 2: UpdateWaveStage2(time); break;
                case 3: UpdateWaveStage3(time); break;
            }
        }

        private static void UpdateWaveStage0(long time)
        {
            if (time % 1800 != 1 || time < Configs.nextWaveFrameIndex + 60 * 60 * 2) return;
            DspBattlePlugin.logger.LogInfo("=====> Initializing next wave");
            StationComponent[] stations = GameMain.data.galacticTransport.stationPool.Where(EnemyShips.ValidStellarStation).ToArray();
            if (stations.Length == 0) return;
            int starId = stations[EnemyShips.random.Next(0, stations.Length)].planetId / 100 - 1;

            // Gen next wave
            int deltaFrames = (Configs.coldTime[Math.Min(Configs.coldTime.Length - 1, Configs.totalWave)] + 1) * 3600;
            Configs.nextWaveFrameIndex = time + deltaFrames + Configs.nextWaveDelay;
            Configs.nextWaveDelay = 0;
            Configs.nextWaveIntensity = Configs.intensity[Math.Min(Configs.intensity.Length - 1, Configs.wavePerStar[starId])];
            // Extra intensity
            long cube = (long)(GameMain.history.universeMatrixPointUploaded * 0.0002777777777777778);
            if (cube > 100000) Configs.nextWaveIntensity += (int)((cube - 100000) / 1000);

            ulong energy = 0;
            DysonSphere[] dysonSpheres = GameMain.data.dysonSpheres;
            int num3 = dysonSpheres.Length;
            for (int i = 0; i < num3; i++)
            {
                if (dysonSpheres[i] != null)
                {
                    energy += (ulong)dysonSpheres[i].energyGenCurrentTick;
                }
            }
            energy *= 60;
            energy /= (1024 * 1024 * 1024L);

            if (energy > 300) // 300G
                Configs.nextWaveIntensity += (int)(energy - 300) * 5;
            if (Configs.nextWaveIntensity > 30000) Configs.nextWaveIntensity = 30000;

            Configs.nextWaveStarIndex = starId;
            Configs.nextWaveState = 1;

            int intensity = Configs.nextWaveIntensity;
            for (int i = 4; i >= 1; --i)
            {
                double v = EnemyShips.random.NextDouble() / 2 + 0.25;
                Configs.nextWaveEnemy[i] = (int)(intensity * v / Configs.enemyIntensity[i]);
                intensity -= Configs.nextWaveEnemy[i] * Configs.enemyIntensity[i];
            }
            Configs.nextWaveEnemy[0] = intensity / Configs.enemyIntensity[0];
            Configs.nextWaveWormCount = EnemyShips.random.Next(Math.Min(Configs.nextWaveIntensity / 100, 40), Math.Min(80, Configs.nextWaveEnemy.Sum())) + 1;

            UIDialogPatch.ShowUIDialog("下一波攻击即将到来！".Translate(),
                string.Format("做好防御提示".Translate(), GameMain.galaxy.stars[Configs.nextWaveStarIndex].displayName));

            UIAlert.ShowAlert(true);
        }

        private static void UpdateWaveStage1(long time)
        {
            if (time < Configs.nextWaveFrameIndex - 3600 * 5) return;
            StarData star = GameMain.galaxy.stars[Configs.nextWaveStarIndex];

            StationComponent[] stations = GameMain.data.galacticTransport.stationPool.Where(e => e != null && e.isStellar && !e.isCollector && e.gid != 0 && e.id != 0 && e.planetId / 100 - 1 == Configs.nextWaveStarIndex).ToArray();

            for (int i = 0; i < Configs.nextWaveWormCount; ++i)
            {
                while (true)
                {
                    int planetId = 100 * (Configs.nextWaveStarIndex + 1) + 1;
                    if (stations.Length != 0) planetId = stations[EnemyShips.random.Next(0, stations.Length)].planetId;

                    Wormhole wormhole = new Wormhole(planetId, UnityEngine.Random.onUnitSphere);
                    VectorLF3 pos = wormhole.uPos;

                    if ((star.uPosition - pos).magnitude < Configs.wormholeRange + star.radius - 10) continue;
                    if (star.planets.Any(planetData => planetData.type != EPlanetType.Gas && (planetData.uPosition - pos).magnitude < Configs.wormholeRange + planetData.radius - 10)) continue;

                    Configs.nextWaveWormholes[i] = wormhole;
                    break;
                }
            }

            Configs.nextWaveState = 2;


            UIDialogPatch.ShowUIDialog("虫洞已生成！".Translate(),
                string.Format("虫洞生成提示".Translate(), GameMain.galaxy.stars[Configs.nextWaveStarIndex].displayName));

            UIAlert.ShowAlert(true);
        }

        private static void UpdateWaveStage2(long time)
        {
            if (time < Configs.nextWaveFrameIndex) return;
            int u = 0;
            for (int i = 0; i <= 4; ++i)
            {
                for (int j = 0; j < Configs.nextWaveEnemy[i]; ++j)
                {
                    EnemyShips.Create(Configs.nextWaveStarIndex, u % Configs.nextWaveWormCount, i, EnemyShips.random.Next(0, Math.Min(u + 1, 30)));
                    u++;
                }
            }

            Configs.nextWaveState = 3;
        }

        private static void UpdateWaveStage3(long time)
        {
            UIBattleStatistics.RegisterBattleTime(time);
            if (EnemyShips.ships.Count == 0)
            {
                Configs.wavePerStar[Configs.nextWaveStarIndex]++;
                Configs.nextWaveState = 0;
                Configs.nextWaveStarIndex = 0;
                Configs.nextWaveWormCount = 0;
                Configs.nextWaveFrameIndex = time;
                RemoveEntities.distroyedStation.Clear();

                long rewardBase = 5 * 60 * 60;
                if (Configs.difficulty == -1) rewardBase = rewardBase * 3 / 4;
                if (Configs.difficulty == 1) rewardBase *= 2;

                long extraSpeedFrame = UIBattleStatistics.totalEnemyEliminated * rewardBase / UIBattleStatistics.totalEnemyGen;
                Configs.extraSpeedFrame = time + extraSpeedFrame;
                Configs.extraSpeedEnabled = true;
                GameMain.history.miningSpeedScale *= 2;
                GameMain.history.techSpeed *= 2;
                GameMain.history.logisticDroneSpeedScale *= 1.5f;
                GameMain.history.logisticShipSpeedScale *= 1.5f;

                UIDialogPatch.ShowUIDialog("战斗已结束！".Translate(),
                    "战斗时间".Translate() + ": " + string.Format("{0:00}:{1:00}", new object[] { UIBattleStatistics.battleTime / 60 / 60, UIBattleStatistics.battleTime / 60 % 60 }) + "; " +
                    "歼灭敌人".Translate() + ": " + UIBattleStatistics.totalEnemyEliminated.ToString("N0") + "; " +
                    "输出伤害".Translate() + ": " + UIBattleStatistics.totalDamage.ToString("N0") + "; " +
                    "损失物流塔".Translate() + ": " + UIBattleStatistics.stationLost.ToString("N0") + "; " +
                    "损失其他建筑".Translate() + ": " + UIBattleStatistics.othersLost.ToString("N0") + "; " +
                    "损失资源".Translate() + ": " + UIBattleStatistics.resourceLost.ToString("N0") + "." +
                    "\n\n<color=#c2853d>" + string.Format("奖励提示".Translate(), extraSpeedFrame / 60) + "</color>\n\n" +
                    "查看更多战斗信息".Translate()
                    );

            }
        }

    }
}
