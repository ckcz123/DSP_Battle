using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DSP_Battle
{
    public class EventSystem
    {
        public static Dictionary<int, EventProto> protos;
        public static List<List<Tuple<int, int>>> alterItems; // 用于上交的物品的id和数量，以等级分（alterItems[level][i])
        public static EventRecorder recorder;

        public static void ClearBeforeLoad()
        {
            ClearEvent();
        }

        public static void InitNewEvent(int id = 0)
        {
            if (id == 0)
            {
                if (Relic.GetRelicCount() == 0)
                    SetEvent(1001);
                else
                    SetEvent(1002);
            }
            else
                SetEvent(id);
        }

        public static void SetEvent(int id)
        {
            recorder = new EventRecorder(id);
            RefreshRequestMeetData();
            PrintData();
        }
        
        public static void TransferTo(int id)
        {
            EventRecorder next = new EventRecorder(id, recorder.modifier, recorder.level);
            recorder = next;
            PrintData();
        }

        public static void PrintData()
        {
            Utils.Log("requestId");
            for (int i = 0; i < recorder.requestId.Length; i++)
            {
                Utils.Log(recorder.requestId[i].ToString());
            }
            Utils.Log("requestCount");
            for (int i = 0; i < recorder.requestCount.Length; i++)
            {
                Utils.Log(recorder.requestCount[i].ToString());
            }
            Utils.Log("requestMeet");
            for (int i = 0; i < recorder.requestMeet.Length; i++)
            {
                Utils.Log(recorder.requestMeet[i].ToString());
            }
        }

        public static void ClearEvent()
        {
            recorder = new EventRecorder(0);
        }

        public static void Decision(int index)
        {
            UIEventSystem.eventWindowObj.SetActive(false);
            Utils.Log($"decision {index}");
            int cur = recorder.protoId;
            cur++;
            while(!protos.ContainsKey(cur) && cur < 9999)
            {
                cur++;
            }
            if(protos.ContainsKey(cur))
                TransferTo(cur);

            UIEventSystem.eventWindowObj.SetActive(true);
            UIEventSystem.RefreshESWindow(true);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "GameTick")]
        public static void OnUpdate()
        {
            if(GameMain.instance.timei % 60 == 15)
            {
                RefreshRequestMeetData();
            }
        }

        public static void CalcEventState()
        {

        }

        public static void Exprot(BinaryWriter w)
        {

        }
        public static void Import(BinaryReader r)
        {
            ClearBeforeLoad();
            CalcEventState();
            UIEventSystem.RefreshAll();
        }

        public static void IntoOtherSave()
        {
            ClearBeforeLoad();
            CalcEventState();
            UIEventSystem.RefreshAll();
        }



        /// <summary>
        /// 每秒更新
        /// </summary>
        public static void RefreshRequestMeetData()
        {
            if (recorder != null && recorder.protoId > 0 && recorder.requestLen > 0)
            {

                for (int i = 0; i < recorder.requestLen; i++)
                {
                    int code = recorder.requestId[i];
                    if (code <= 0)
                    {
                        recorder.requestMeet[i] = recorder.requestCount[i];
                    }
                    else if (code == 9995)
                        recorder.requestMeet[i] = Rank.rank;
                    else if (code >= 10000 && code < 20000)
                    {
                        int itemId = code - 10000;
                        recorder.requestMeet[i] = Math.Min(GameMain.mainPlayer.package.GetItemCount(itemId), recorder.requestCount[i]);
                    }
                    else if (code >= 20000 && code < 30000)
                    {

                    }
                    else if (code > 30000 && code < 40000) // 等于30000是任意科技，不需要每秒更新
                    {
                        int techId = code - 30000;
                        recorder.requestMeet[i] = GameMain.data.history.TechState(techId).curLevel;
                    }
                    else if (code >= 40000 && code < 50000)
                    {

                    }
                    else if (code >= 50000 && code < 60000)
                    {
                        if (recorder.requestCount[i] == 0)
                        {
                            int starIndex = code - 50000;
                            EnemyData[] pool = GameMain.data.spaceSector.enemyPool;
                            EnemyDFHiveSystem[] dfHivesByAstro = GameMain.data.spaceSector.dfHivesByAstro;
                            int remaining = 0;
                            for (int j = 0; j < GameMain.data.spaceSector.enemyCursor; j++)
                            {
                                ref EnemyData ptr = ref pool[j];
                                if (ptr.dfTinderId != 0)
                                    continue;
                                if (ptr.id == 0)
                                    continue;
                                EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                                if (enemyDFHiveSystem != null && enemyDFHiveSystem.starData?.index == starIndex)
                                    remaining++;
                            }
                            recorder.requestMeet[i] = -remaining;
                        }
                    }
                    else if (code >= 60000 && code < 70000)
                    {
                        int starIndex = code - 60000;
                        EnemyData[] pool = GameMain.data.spaceSector.enemyPool;
                        EnemyDFHiveSystem[] dfHivesByAstro = GameMain.data.spaceSector.dfHivesByAstro;
                        int lastOriAstroId = -1;
                        for (int j = 0; j < GameMain.data.spaceSector.enemyCursor; j++)
                        {
                            ref EnemyData ptr = ref pool[j];
                            if (ptr.dfTinderId != 0)
                                continue;
                            if (ptr.id == 0)
                                continue;
                            if (ptr.originAstroId == lastOriAstroId)
                                continue;
                            EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                            if (enemyDFHiveSystem != null && enemyDFHiveSystem.starData?.index == starIndex)
                            {
                                int cur = (int)(enemyDFHiveSystem.evolve.threat * 100.0 / enemyDFHiveSystem.evolve.maxThreat);
                                if (cur > recorder.requestMeet[i])
                                    recorder.requestMeet[i] = cur;
                            }
                            lastOriAstroId = ptr.originAstroId;
                        }
                    }
                    else if (code >= 70000 && code < 80000)
                    {
                        int starIndex = code - 70000;
                        int remaining = 0;
                        bool unknown = false;
                        int planetCount = (int)GameMain.galaxy.StarById(starIndex + 1)?.planetCount;
                        for (int j = 0; j < planetCount; j++)
                        {
                            PlanetData planet = GameMain.galaxy.StarById(starIndex + 1)?.planets[j];
                            if (planet != null && planet.type != EPlanetType.Gas)
                            {
                                PlanetFactory factory = planet.factory;
                                if (factory == null) // 尚未落足过的行星无法得知enemy数量？那就无法统计是否已消灭
                                {
                                    unknown = true;
                                    break;
                                }
                                else
                                {
                                    EnemyData[] gPool = factory.enemyPool;
                                    for (int k = 0; k < factory.enemyCursor; k++)
                                    {
                                        ref EnemyData ptr = ref gPool[k];
                                        if (ptr.id > 0)
                                            remaining++;
                                    }
                                }
                            }
                        }
                        if (!unknown)
                        {
                            EnemyData[] pool = GameMain.data.spaceSector.enemyPool;
                            EnemyDFHiveSystem[] dfHivesByAstro = GameMain.data.spaceSector.dfHivesByAstro;
                            for (int j = 0; j < GameMain.data.spaceSector.enemyCursor && !unknown; j++)
                            {
                                ref EnemyData ptr = ref pool[j];
                                if (ptr.dfTinderId != 0)
                                    continue;
                                if (ptr.id == 0)
                                    continue;
                                EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                                if (enemyDFHiveSystem != null && enemyDFHiveSystem.starData?.index == starIndex)
                                    remaining++;

                            }
                        }
                        recorder.requestMeet[i] = unknown ? int.MinValue : -remaining;
                    }
                    else if (code >= 80000 && code < 90000)
                    {
                        int starIndex = code - 80000;
                        int maxLen = GameMain.data.dysonSpheres.Length;
                        if (starIndex != 0 && starIndex < maxLen)
                        {
                            DysonSphere sphere = GameMain.data.dysonSpheres[starIndex];
                            if (sphere != null)
                            {
                                long gen = GameMain.data.dysonSpheres[starIndex].energyGenCurrentTick * 60 / 1000000;
                                if (gen > recorder.requestCount[i])
                                    gen = recorder.requestCount[i];
                                recorder.requestMeet[i] = (int)gen;
                            }
                            else
                                recorder.requestMeet[i] = 0;
                        }
                        else
                        {
                            long maxGen = 0;
                            for (int j = 0; j < maxLen; j++)
                            {
                                DysonSphere sphere = GameMain.data.dysonSpheres[j];
                                if(sphere!=null)
                                {
                                    maxGen = Math.Max(maxGen, sphere.energyGenCurrentTick * 60 / 1000000);
                                }
                            }
                            if (maxGen > int.MaxValue)
                                maxGen = int.MaxValue;
                            recorder.requestMeet[i] = (int)maxGen;
                        }
                    }
                    else if (code >= 90000 && code < 100000)
                    {
                        int starIndex = code - 90000;
                        EnemyData[] pool = GameMain.data.spaceSector.enemyPool;
                        EnemyDFHiveSystem[] dfHivesByAstro = GameMain.data.spaceSector.dfHivesByAstro;
                        int lastOriAstroId = -1;
                        for (int j = 0; j < GameMain.data.spaceSector.enemyCursor; j++)
                        {
                            ref EnemyData ptr = ref pool[j];
                            if (ptr.dfTinderId != 0)
                                continue;
                            if (ptr.id == 0)
                                continue;
                            if (ptr.originAstroId == lastOriAstroId)
                                continue;
                            EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                            if (enemyDFHiveSystem != null && enemyDFHiveSystem.starData?.index == starIndex)
                            {
                                int cur = enemyDFHiveSystem.evolve.level;
                                if (cur > recorder.requestMeet[i])
                                    recorder.requestMeet[i] = cur;
                            }
                            lastOriAstroId = ptr.originAstroId;
                        }
                    }
                    else if (code >= 1000000 && code < 2000000)
                    {
                        EnemyData[] pool = GameMain.data.spaceSector.enemyPool;
                        int remaining = 0;
                        for (int j = 0; j < GameMain.data.spaceSector.enemyCursor; j++)
                        {
                            ref EnemyData ptr = ref pool[j];
                            if (ptr.originAstroId != code)
                                continue;
                            if (ptr.dfTinderId != 0)
                                continue;
                            if (ptr.id == 0)
                                continue;
                            remaining++;
                        }
                        recorder.requestMeet[i] = -remaining;
                    }
                    else if (code >= 2000000 && code < 3000000)
                    {
                        if (recorder.requestCount[i] == 0)
                        {
                            int planetId = code - 2000000;
                            PlanetData planet = GameMain.galaxy.PlanetById(planetId);
                            if(planet != null)
                            {
                                PlanetFactory factory = planet.factory;
                                if (factory == null)
                                    recorder.requestMeet[i] = int.MinValue;
                                else
                                {
                                    int remaining = 0;
                                    EnemyData[] gPool = factory.enemyPool;
                                    for (int j = 0; j < factory.enemyCursor; i++)
                                    {
                                        ref EnemyData ptr = ref gPool[j];
                                        if (ptr.id > 0)
                                            remaining++;
                                    }
                                    recorder.requestMeet[i] = -remaining;
                                }
                            }
                            else
                            {
                                recorder.requestMeet[i] = 0;
                            }
                        }
                    }
                    else if (code >= 3000000 && code < 4000000)
                    {
                        if (recorder.requestCount[i] == 0)
                        {
                            int planetId = code - 3000000;
                            PlanetData planet = GameMain.galaxy.PlanetById(planetId);
                            if (planet != null)
                            {
                                PlanetFactory factory = planet.factory;
                                if (factory == null)
                                    recorder.requestMeet[i] = int.MinValue;
                                else
                                {
                                    int remaining = 0;
                                    EnemyData[] gPool = factory.enemyPool;
                                    for (int j = 0; j < factory.enemyCursor; i++)
                                    {
                                        ref EnemyData ptr = ref gPool[j];
                                        if (ptr.id > 0 && ptr.dfGBaseId > 0)
                                            remaining++;
                                    }
                                    recorder.requestMeet[i] = -remaining;
                                }
                            }
                            else
                            {
                                recorder.requestMeet[i] = 0;
                            }
                        }
                    }
                    else if (code >= 4000000 && code < 5000000)
                    {
                        int planetId = code - 4000000;
                        if(GameMain.data.localPlanet != null && GameMain.data.localPlanet.id == planetId)
                        {
                            recorder.requestMeet[i] = recorder.requestCount[i];
                        }
                    }
                }

            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CombatStat), "HandleZeroHp")]
        public static bool ZeroHpInceptor(ref CombatStat __instance, GameData gameData, SkillSystem skillSystem)
        {
            if (recorder != null && recorder.protoId > 0 && recorder.requestId.Length > 0 || true)
            {
                var _this = __instance;
                KillStatistics killStatistics = skillSystem.killStatistics;
                if (_this.originAstroId > 1000000)
                {
                    if (_this.objectType == 4)
                    {
                        ref EnemyData ptr = ref skillSystem.sector.enemyPool[_this.objectId];
                        if (ptr.id > 0)
                        {
                            Utils.Log($"kill space unit {ptr.unitId}");
                        }
                    }
                }
                else if (_this.originAstroId > 100 && _this.originAstroId <= 204899 && _this.originAstroId % 100 > 0)
                {
                    PlanetFactory planetFactory = skillSystem.astroFactories[_this.originAstroId];
                    if (planetFactory != null)
                    {
                        if (_this.objectType == 4)
                        {
                            ref EnemyData ptr3 = ref planetFactory.enemyPool[_this.objectId];
                            if (ptr3.id > 0)
                            {
                                Utils.Log($"kill ground unit {ptr3.dfGBaseId}" + string.Format(" wocao{0}aa{1}nmd", new string[] { "123123", "232343", "12313213baba" }));
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static void TestIfGroudBaseInited()
        {
            EnemyData[] pool = GameMain.data.spaceSector.enemyPool;
            for (int i = 0; i < GameMain.data.spaceSector.enemyCursor; i++)
            {
                ref EnemyData ptr = ref pool[i];
                if (ptr.dfRelayId == 0)
                    continue;
                Utils.Log($"oriAstro is {ptr.originAstroId} and astro is {ptr.astroId}");
            }
        }
    }
}
