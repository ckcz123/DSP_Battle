using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    public class EventProto
    {
        public int id;
        public string title;
        public string description;
        public int descInsertNum;
        public int[] descInsertId;

        public int requestLen;
        public int[] requestId;
        public int[] requestCount;

        public int decisionLen;
        public string[] decisionText;
        public int[][] decisionRequestNeed;
        public int[][] decisionResultId;
        public int[][] decisionResultCount;

        public static int maxDecisionCount = 4;
        static List<int> protoCount = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        /* CombatStat.HandleZeroHp
         * EnemyDFGroundSystem.RandomDropItemOnce
         * description插入文本规则：string.Format的第二个数组为string数组，固定为{从requestId 0 提取的恒星系名、行星名、黑雾巢穴名、物品名、物品数}
         * 
         * requestId:
         *  0                   无
         *  1-999               根据难度等级替换为此requestId*10000 + 相关数据
         *  9995                功勋阶级达到count
         *  9996                伊卡洛斯死亡count次
         *  9997                消灭count数量的地面黑雾单位
         *  9998                消灭count数量的太空黑雾单位
         *  9999                消灭count数量的任意黑雾单位
         *  10000+itemId        机甲直接上交物品，Count为个数要求
         *  20000+itemId        指定物品全星区分钟产量达标Count
         *  30000+techId        已解锁指定科技（达到count等级），30000为要求解锁任意一个新的科技
         *  40000+starIndex     压制/消灭指定恒星系的地面黑雾单位，count为个数要求     
         *  50000+starIndex     压制/消灭指定恒星系的太空黑雾单位，count为个数要求，0为清理全部（含中继器）
         *  60000+starIndex     提升指定恒星系的太空黑雾威胁等级，最高的威胁等级至少达到count%
         *  70000+starIndex     肃清恒星系，包括所有太空、地面黑雾单位和中继器
         *  80000+starIndex     指定恒星系的巨构能量水平达到count MW，89999为任意
         *  90000+starIndex     提升指定恒星系的任意一个太空黑雾巢穴等级到count
         *  1000000-1999999     消灭指定黑雾巢穴的全部太空单位，含中继器
         *  2000000+planetId    消灭指定行星的地面黑雾单位，count为个数要求，0为清理全部
         *  3000000+planetId    从指定行星的地面黑雾基地中找出一个物品（通过消灭），设定为需要消灭全部的地面黑雾基地
         *  4000000+planetId    探查指定行星（到达即可）
         *  
         * decisionResultId:
         *  -1                  终止序列
         *  0                   开启圣物选择窗口，然后终止序列
         *  1                   获得功勋点数count点
         *  2                   提升功勋阶级count级
         *  3                   推进随机星系的巨构建造count点
         *  4                   提升/降低此序列最终圣物的普通获取概率
         *  5                   提升/降低此序列最终圣物的稀有获取概率
         *  6                   提升/降低此序列最终圣物的史诗获取概率
         *  7                   提升/降低此序列最终圣物的传说获取概率
         *  8                   提升/降低此序列最终圣物的受诅咒的获取概率
         *  9                   此序列最终圣物获得count次免费随机次数
         *  10                  推进恒星炮冷却count秒，0为立刻冷却
         *  11-19               根据当前应有的难度等级，从该序列（1-9）中（id = (1-9) * 1000 + random）随机选择一个作为下个事件
         *  20+decodeType       更改下一个eventProto后，需要首先经过时间进行decodeType类型的解码（类型决定解码时的显示文本），count为所需tick数。21：正在分析日志，22：修复，23：修复并分析日志，24：解译，25：修复并解译
         *  10000+eventProtoId  进行至指定key的序列
         *  20000+itemId        获得物品count个
         *  30000+techId        解锁或升级科技count次
         *  40000+starIndex     提升/降低指定恒星系全部太空黑雾巢穴的等级count（负数为降低）
         *  50000+starIndex     提升指定恒星系全部太空黑雾巢穴的经验count
         *  4000000+oriAstroId  提升/降低指定太空黑雾巢穴的等级count
         *  5000000+oriAstroId  提升指定太空黑雾巢穴的经验count
         *  6000000+planetId    提升/降低指定行星上的全部地面黑雾巢穴的等级count
         *  7000000+planetId    提升指定行星上的全部地面黑雾巢穴的经验
         *  100000000+          隐藏对应的decisionResult，不在按钮Tip上显示
         */

        public EventProto(int id)
        {
            this.id = id;
            this.title = "ept" + id.ToString();
            this.description = "epd" + id.ToString();
            requestLen = 0;
            requestId = new int[] { };
            requestCount = new int[] { };
            decisionLen = 0;
            decisionText = new string[maxDecisionCount];
            decisionRequestNeed = new int[maxDecisionCount][];
            decisionResultId = new int[maxDecisionCount][];
            decisionResultCount = new int[maxDecisionCount][];
            for (int i = 0; i < maxDecisionCount; i++)
            {
                decisionText[i] = null;
            }
            int level = id / 1000;
            if (level < 0)
                level = 0;
            if (level > 9) 
                level = 9;
            if (!EventSystem.protos.ContainsKey(id))
                protoCount[level]++;
        }
        public void DescInsert(int[] insertId)
        {

        }
        public void SetRequest(int[] id, int[] num)
        {
            if (id != null)
            {
                requestLen = id.Length;
                requestId = id;
                requestCount = num;
            }
            else
                requestLen = 0;
        }
        public void SetDecision(int index, int[] needReqIndex, int[] resultId, int[] resultCount)
        {
            if (index < 0 || index >= 4)
                return;
            decisionRequestNeed[index] = needReqIndex;
            decisionResultId[index] = resultId;
            decisionResultCount[index] = resultCount;
            decisionText[index] = "epdt" + id.ToString() + "-" + index.ToString();
            decisionLen = 0;
            for (int i = 0; i < 4; i++)
            {
                if (decisionText[i] != null)
                    decisionLen++;
            }
        }
    }

    public class EventRecorder
    {
        public int protoId;
        public int lockedStarIndex;
        public int lockedPlanetId;
        public int lockedOriAstroId;
        public int level;
        public int requestLen;
        public int[] requestId;
        public int[] requestCount;
        public int[] requestMeet;
        public int[] modifier;
        public int decodeType;
        public int decodeTimeNeed;
        public int decodeTimeSpend;
        public EventRecorder(int protoId, int[] prevModifier = null, int forceLevel = -1)
        {
            if(EventSystem.protos.ContainsKey(protoId))
            {
                EventProto proto = EventSystem.protos[protoId];
                this.protoId = protoId;
                level = forceLevel;
                lockedStarIndex = -1;
                lockedPlanetId = -1;
                lockedOriAstroId = -1;
                if (level < 0)
                    level = Relic.GetRelicCount() + (Relic.recordRelics == null ? 0 : Relic.recordRelics.Count);
                if (proto.requestLen > 0)
                {
                    requestLen = proto.requestLen;
                    requestId = new int[requestLen];
                    requestCount = new int[requestLen];
                    requestMeet = new int[requestLen];
                    for (int i = 0; i < requestLen; i++)
                    {
                        requestId[i] = proto.requestId[i];
                        requestCount[i] = proto.requestCount[i];
                        requestMeet[i] = 0;
                    }
                    InitDefaultData();
                }
                else
                {
                    requestLen = 0;
                    requestId = new int[] { };
                    requestCount = new int[] { };
                    requestMeet = new int[] { };
                }
            }
            else
            {
                this.protoId = 0;
                requestLen = 0;
                requestId = new int[] { };
                requestCount = new int[] { };
                requestMeet = new int[] { };
            }
            if (prevModifier == null)
                modifier = new int[] { 0, 0, 0, 0, 0, 0 };
            else
            {
                int num = prevModifier.Length;
                modifier = new int[num];
                for (int i = 0; i < num; i++)
                {
                    modifier[i] = prevModifier[i];
                }
            }
            decodeType = 0;
            decodeTimeNeed = 0;
            decodeTimeSpend = 0;
        }

        /// <summary>
        /// 初始化需要随机的requestId数据，以及count赋值
        /// </summary>
        public void InitDefaultData()
        {
            List<int> usedItemId = new List<int>();
            for (int i = 0; i < requestLen; i++)
            {
                int code = requestId[i];
                if(code < 1000)
                {
                    int actual = code * 10000;
                    int realLevel = Math.Min(Math.Max(0, level), 4);
                    if (code == 1 || code == 2)
                    {
                        int count = EventSystem.alterItems[realLevel].Count;
                        int index = Utils.RandInt(0, count);
                        while (usedItemId.Count < count && usedItemId.Contains(EventSystem.alterItems[realLevel][index].Item1))
                        {
                            index = (index + 1) % count;
                        }
                        actual += EventSystem.alterItems[realLevel][index].Item1;
                        requestId[i] = actual;
                        requestCount[i] = EventSystem.alterItems[realLevel][index].Item2;
                    }
                    else if (code == 3) // 暂时替换为任意科技，需要重写
                    {
                        requestId[i] = actual;
                        requestCount[i] = 1;
                        requestMeet[i] = 0;
                    }
                    else if (code <= 9)
                    {
                        if (lockedStarIndex < 0)
                        {
                            int each = GameMain.galaxy.starCount / 4;
                            if (each < 1) each = 1;
                            int min = Math.Max(0, (realLevel - 1) * each);
                            int max = Math.Min(realLevel * each, GameMain.galaxy.starCount);
                            if (code >= 5 && code <= 9 && code != 8) // 需要寻找一个有太空黑雾巢穴核心的恒星系
                            {
                                List<int> curLevelStarPool = new List<int>();
                                List<int> allLevelStarPool = new List<int>();
                                EnemyData[] enemyPool = GameMain.data.spaceSector.enemyPool;
                                EnemyDFHiveSystem[] dfHivesByAstro = GameMain.data.spaceSector.dfHivesByAstro;
                                for (int j = 0; j < GameMain.data.spaceSector.enemyCursor; j++)
                                {
                                    ref EnemyData ptr = ref enemyPool[j];
                                    if (ptr.dfSCoreId == 0)
                                        continue;
                                    if (ptr.id == 0)
                                        continue;
                                    EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                                    if (enemyDFHiveSystem == null)
                                        continue;
                                    else if (enemyDFHiveSystem.starData.index >= min && enemyDFHiveSystem.starData.index < max)
                                        curLevelStarPool.Add(enemyDFHiveSystem.starData.index);
                                    else
                                        allLevelStarPool.Add(enemyDFHiveSystem.starData.index);
                                }
                                if (curLevelStarPool.Count > 0)
                                    lockedStarIndex = curLevelStarPool[Utils.RandInt(0, curLevelStarPool.Count)];
                                else if (allLevelStarPool.Count > 0)
                                    lockedStarIndex = allLevelStarPool[Utils.RandInt(0, allLevelStarPool.Count)];
                                else
                                {
                                    EventSystem.SetEvent(9999); // 没有任何合法的星系找到，直接跳到元驱动获取事件
                                    return;
                                }
                            }
                            else if (code == 4)
                            {
                                List<int> curLevelPlanetPool = new List<int>();
                                List<int> allLevelPlanetPool = new List<int>();
                                EnemyData[] enemyPool = GameMain.data.spaceSector.enemyPool;
                                for (int j = 0; j < GameMain.data.spaceSector.enemyCursor; j++)
                                {
                                    ref EnemyData ptr = ref enemyPool[j];
                                    if (ptr.dfRelayId == 0)
                                        continue;
                                    if (ptr.astroId > 1000000) // 代表是太空中继站（否则应该是地面中继站）
                                        continue;
                                    int planetId = ptr.astroId;
                                    int starIndex = planetId / 100 - 1;
                                    PlanetFactory factory = GameMain.galaxy.PlanetById(planetId)?.factory;
                                    bool canSelect = false;
                                    if (factory != null) // 如果有中继站，且factory是null则认为是有base，否则查看factory的enemyPool有没有base
                                    {
                                        EnemyData[] pool = factory.enemyPool;
                                        for (int k = 0; k < factory.enemyCursor; k++)
                                        {
                                            ref EnemyData prt = ref pool[k];
                                            if (ptr.id == 0)
                                                continue;
                                            if (ptr.dfGBaseId == 0)
                                                continue;
                                            canSelect = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        canSelect = true;
                                    }
                                    if (canSelect)
                                    {
                                        if (starIndex >= min && starIndex < max)
                                            curLevelPlanetPool.Add(planetId);
                                        else
                                            allLevelPlanetPool.Add(planetId);
                                    }
                                }
                                if (curLevelPlanetPool.Count > 0)
                                    lockedStarIndex = curLevelPlanetPool[Utils.RandInt(0, curLevelPlanetPool.Count)] / 100 - 1;
                                else if (allLevelPlanetPool.Count > 0)
                                    lockedStarIndex = allLevelPlanetPool[Utils.RandInt(0, allLevelPlanetPool.Count)] / 100 - 1;
                                else
                                {
                                    EventSystem.SetEvent(9999); // 没有任何合法的星系找到，直接跳到元驱动获取事件
                                    return;
                                }
                            }
                            else
                            {
                                lockedStarIndex = Utils.RandInt(min, max);
                            }
                        }
                        requestId[i] = actual + lockedStarIndex;
                    }
                    else if (code == 100)
                    {
                        if(lockedOriAstroId < 0)
                        {
                            int each = GameMain.galaxy.starCount / 4;
                            if (each < 1) each = 1;
                            int min = Math.Max(0, (realLevel - 1) * each);
                            int max = Math.Min(realLevel * each, GameMain.galaxy.starCount);
                            List<int> curLevelOriAstroPool = new List<int>();
                            List<int> allLevelOriAstroPool = new List<int>();
                            EnemyData[] enemyPool = GameMain.data.spaceSector.enemyPool;
                            EnemyDFHiveSystem[] dfHivesByAstro = GameMain.data.spaceSector.dfHivesByAstro;
                            for (int j = 0; j < GameMain.data.spaceSector.enemyCursor; j++)
                            {
                                ref EnemyData ptr = ref enemyPool[j];
                                if (ptr.dfSCoreId == 0)
                                    continue;
                                if (ptr.id == 0)
                                    continue;
                                EnemyDFHiveSystem enemyDFHiveSystem = dfHivesByAstro[ptr.originAstroId - 1000000];
                                if (enemyDFHiveSystem == null)
                                    continue;
                                else if (enemyDFHiveSystem.starData.index >= min && enemyDFHiveSystem.starData.index < max)
                                    curLevelOriAstroPool.Add(ptr.originAstroId);
                                else
                                    allLevelOriAstroPool.Add(ptr.originAstroId);
                            }
                            if (curLevelOriAstroPool.Count > 0)
                                lockedOriAstroId = curLevelOriAstroPool[Utils.RandInt(0, curLevelOriAstroPool.Count)];
                            else if (allLevelOriAstroPool.Count > 0)
                                lockedOriAstroId = allLevelOriAstroPool[Utils.RandInt(0, allLevelOriAstroPool.Count)];
                            else
                            {
                                EventSystem.SetEvent(9999); // 没有任何合法的星系找到，直接跳到元驱动获取事件
                                return;
                            }
                        }
                        requestId[i] = lockedOriAstroId; // 注意没有actual + ，因为oriAstroId就是1000000~1999999，加了就超了
                    }
                    else if (code == 200 || code == 300)
                    {
                        if (lockedPlanetId < 0)
                        {
                            int each = GameMain.galaxy.starCount / 4;
                            if (each < 1) each = 1;
                            int min = Math.Max(0, (realLevel - 1) * each);
                            int max = Math.Min(realLevel * each, GameMain.galaxy.starCount);
                            List<int> curLevelPlanetPool = new List<int>();
                            List<int> allLevelPlanetPool = new List<int>();
                            EnemyData[] enemyPool = GameMain.data.spaceSector.enemyPool;
                            for (int j = 0; j < GameMain.data.spaceSector.enemyCursor; j++)
                            {
                                ref EnemyData ptr = ref enemyPool[j];
                                if (ptr.dfRelayId == 0)
                                    continue;
                                if (ptr.astroId > 1000000) // 代表是太空中继站（否则应该是地面中继站）
                                    continue;
                                int planetId = ptr.astroId;
                                int starIndex = planetId / 100 - 1;
                                PlanetFactory factory = GameMain.galaxy.PlanetById(planetId)?.factory;
                                bool canSelect = false;
                                if (factory != null) // 如果有中继站，且factory是null则认为是有base，否则查看factory的enemyPool有没有base
                                {
                                    EnemyData[] pool = factory.enemyPool;
                                    for (int k = 0; k < factory.enemyCursor; k++)
                                    {
                                        ref EnemyData prt = ref pool[k];
                                        if (ptr.id == 0)
                                            continue;
                                        if (ptr.dfGBaseId == 0)
                                            continue;
                                        canSelect = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    canSelect = true;
                                }
                                if (canSelect)
                                {
                                    if (starIndex >= min && starIndex < max)
                                        curLevelPlanetPool.Add(planetId);
                                    else
                                        allLevelPlanetPool.Add(planetId);
                                }
                            }
                            if (curLevelPlanetPool.Count > 0)
                                lockedPlanetId = curLevelPlanetPool[Utils.RandInt(0, curLevelPlanetPool.Count)];
                            else if (allLevelPlanetPool.Count > 0)
                                lockedPlanetId = allLevelPlanetPool[Utils.RandInt(0, allLevelPlanetPool.Count)];
                            else
                            {
                                EventSystem.SetEvent(9999); // 没有任何合法的星系找到，直接跳到元驱动获取事件
                                return;
                            }
                        }
                        requestId[i] = actual + lockedPlanetId;
                    }
                    else if(code == 400)
                    {
                        if (lockedPlanetId < 0)
                        {
                            int each = GameMain.galaxy.starCount / 4;
                            if (each < 1) each = 1;
                            int min = Math.Max(0, (realLevel - 1) * each);
                            int max = Math.Min(realLevel * each, GameMain.galaxy.starCount);
                            List<int> curLevelPlanetPool = new List<int>();
                            List<int> allLevelPlanetPool = new List<int>();
                            for (int j = 0; j < GameMain.galaxy.starCount; j++)
                            {
                                StarData star = GameMain.galaxy.StarById(j + 1);
                                if (star == null)
                                    continue;
                                for (int k = 0; k < star.planetCount; k++)
                                {
                                    PlanetData planet = star.planets[k];
                                    if (planet != null)
                                    {
                                        if (GameMain.data.localPlanet != null && GameMain.data.localPlanet.id == planet.id)
                                            continue;
                                        if (j >= min && j < max)
                                            curLevelPlanetPool.Add(planet.id);
                                        else
                                            allLevelPlanetPool.Add(planet.id);
                                    }
                                }
                            }
                            if (curLevelPlanetPool.Count > 0)
                                lockedPlanetId = curLevelPlanetPool[Utils.RandInt(0, curLevelPlanetPool.Count)];
                            else if (allLevelPlanetPool.Count > 0)
                                lockedPlanetId = allLevelPlanetPool[Utils.RandInt(0, allLevelPlanetPool.Count)];
                            else
                            {
                                EventSystem.SetEvent(9999); // 没有任何合法的星系找到，直接跳到元驱动获取事件
                                return;
                            }
                        }
                        requestId[i] = actual + lockedPlanetId;
                        requestMeet[i] = -1;
                    }
                }
            }
        }
    }

}
