using CommonAPI.Systems;
using HarmonyLib;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xiaoye97;

namespace DSP_Battle
{
    class BattleProtos
    {
        public static int pageBias = 0;
        public static void AddProtos()
        {
            //bc7e35
            //c2853d
            ProtoRegistry.RegisterString("子弹1", "Armour-piercing bullet", "穿甲磁轨弹");
            ProtoRegistry.RegisterString("子弹1描述", "", "从来没见过敌人的先驱者把老祖宗的数据库翻了114514遍，东拼西凑出来一个穿甲弹设计图。由于没找到火药配方，只好照着炮弹的样子铸造成实心炮弹并加强其结构。仅能进行单体打击，杀伤力有限。");
            ProtoRegistry.RegisterString("子弹1结论", "", "你解锁了穿甲磁轨弹，可以利用动能进行攻击");
            ProtoRegistry.RegisterString("子弹2", "Acid bullet", "强酸磁轨弹");
            ProtoRegistry.RegisterString("子弹2描述", "", "在对敌人进行分析和研究后，先驱者大胆将<color=#c2853d>硫酸</color>封装后制成了一种新的炮弹。爆破后可以对范围内敌人产生酸蚀，更加高效地杀伤敌人。“果然AOE才是王道！”先驱者自言自语道。");
            ProtoRegistry.RegisterString("子弹2结论", "", "你解锁了强酸磁轨弹，可以利用硫酸腐蚀外壳");
            ProtoRegistry.RegisterString("子弹3", "", "氘核爆破弹");
            ProtoRegistry.RegisterString("子弹3描述", "", "制造出<color=#c2853d>热核导弹</color>的先驱者想要让他更小更快，便试着把核弹微缩化封装进了子弹里。这种强力的炮弹被制造出来的同时，先驱者将敌人命名为了“ ”。该武器可以在命中后发生爆炸造成大范围杀伤。");
            ProtoRegistry.RegisterString("子弹3结论", "", "你解锁了氘核爆破弹，可以利用核聚变进行破坏");
            ProtoRegistry.RegisterString("脉冲", "", "相位裂解光束");
            ProtoRegistry.RegisterString("脉冲描述", "", "-");

            ProtoRegistry.RegisterString("导弹1", "", "热核导弹");
            ProtoRegistry.RegisterString("导弹1描述", "", "这天，百无聊赖的先驱者正在刷哔哩哔哩，突然刷到了一条《如何在卧室制造核弹》的视频，于是这种导弹便被制造了出来。这是一种重型武器，发射升空并命中敌人后产生核爆，造成大范围伤害。");
            ProtoRegistry.RegisterString("导弹1结论", "", "你解锁了热核导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("导弹2", "", "反物质导弹");
            ProtoRegistry.RegisterString("导弹2描述", "", "先驱者在一次搬运货物时不慎手滑造成微量<color=#c2853d>反物质</color>泄露，这导致他辛辛苦苦拉好的产线毁于一旦。“为什么不让敌人常常这种痛苦呢？”他想到。于是这种将<color=#c2853d>湮灭约束球</color>封装入导弹的武器被制造了出来，命中敌人后会发生湮灭，将敌人彻底抹杀。");
            ProtoRegistry.RegisterString("导弹2结论", "", "你解锁了反物质导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("导弹3", "", "引力塌陷导弹");
            ProtoRegistry.RegisterString("导弹3描述", "", "发明出<color=#c2853d>引力弹射炮</color>的先驱者一鼓作气，将微型黑洞封装进导弹，制成了这种超级武器。它能在爆炸后短暂生成一个微型黑洞将范围内的敌人吸入其中，简单高效。");
            ProtoRegistry.RegisterString("导弹3结论", "", "你解锁了引力塌陷导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("弹射器1", "", "电磁轨道炮");
            ProtoRegistry.RegisterString("弹射器1描述", "", "制造出<color=#c2853d>穿甲磁轨弹</color>的先驱者不知道该如何把他们扔上天，直到他不小心把<color=#c2853d>太阳帆</color>误装成了炮弹。“好极了，现在我‘创造’了一种武器。”先驱者这样想着，“但是怎么区分他们呢？”。随后，先驱者默念着：“你指尖闪动的电光，是我此生不变的信仰……”给它装上了橙色氛围灯。该武器打击范围受到仰角限制。");
            ProtoRegistry.RegisterString("弹射器1结论", "", "你解锁了电磁轨道炮，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("弹射器2", "", "引力弹射炮");
            ProtoRegistry.RegisterString("弹射器2描述", "", "在黑洞杀了个七进七出之后，先驱者对引力操控有了深入见解，他终于决定自己开发一种武器。于是他拆除了<color=#c2853d>电磁轨道炮</color>的加速磁场，制造了一种利用微型黑洞制造引力弹弓的发生器，这使得发射炮弹获得了数倍于之前的动能。“这回可不是换个氛围灯那么简单了！”先驱者自我陶醉着。");
            ProtoRegistry.RegisterString("弹射器2结论", "", "你解锁了引力弹射炮，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("脉冲炮", "", "相位裂解炮");
            ProtoRegistry.RegisterString("脉冲炮描述", "", "开发出超级武器的先驱者百般无聊，整日靠着当P社战犯度日，但是他灭绝寰宇的大业总是因为弹药短缺被打断，于是他一气之下依照《群星》的舰载武器开发了这种武器。利用发射的高能粒子流产生相位裂解链式反应，大范围杀伤敌人，最大的优点是，无需弹药！从此，先驱者在成为战犯的道路上越走越远。");
            ProtoRegistry.RegisterString("脉冲炮结论", "", "你解锁了相位裂解炮，可以仅使用电力攻击敌方");

            ProtoRegistry.RegisterString("发射器1", "", "深空导弹发射井");
            ProtoRegistry.RegisterString("发射器1描述", "", "制造出<color=#c2853d>热核导弹</color>的先驱者并不知道如何把他们扔上天去，直到他看到了<color=#c2853d>垂直发射井</color>。“只要把它刷成红色我不就发明了一种武器吗？”先驱者这样想到。该武器可以对敌人进行全方位打击。");
            ProtoRegistry.RegisterString("发射器1结论", "", "你解锁了深空导弹发射井，可以发射导弹攻击敌方");

            ProtoRegistry.RegisterString("近地防卫系统", "", "近地防卫系统");
            ProtoRegistry.RegisterString("近地防卫系统描述", "", "制造<color=#c2853d>电磁轨道炮</color>发射<color=#c2853d>穿甲磁轨弹</color>进行基础防御。");
            ProtoRegistry.RegisterString("近地防卫系统结论", "", "你解锁了近地防卫系统");
            ProtoRegistry.RegisterString("深空防卫系统", "", "深空防卫系统");
            ProtoRegistry.RegisterString("深空防卫系统描述", "", "制造<color=#c2853d>深空导弹发射井</color>部署<color=#c2853d>热核导弹</color>实现更大范围覆盖打击，填补近地防卫的空白。");
            ProtoRegistry.RegisterString("深空防卫系统结论", "", "你解锁了深空防卫系统");
            ProtoRegistry.RegisterString("引力操控技术", "", "引力操控技术");
            ProtoRegistry.RegisterString("引力操控技术描述", "", "制造<color=#c2853d>引力弹射炮</color>升级近地防卫系统，生产<color=#c2853d>引力塌陷导弹</color>进一步加强深空防御。");
            ProtoRegistry.RegisterString("引力操控技术结论", "", "你解锁了引力操控技术");
            ProtoRegistry.RegisterString("相位裂解技术", "", "相位裂解技术");
            ProtoRegistry.RegisterString("相位裂解技术描述", "", "制造强力武器<color=#c2853d>相位裂解炮</color>获得终极近地防御能力。");
            ProtoRegistry.RegisterString("相位裂解技术结论", "", "你解锁了相位裂解技术");


            ProtoRegistry.RegisterString("子弹2tech描述", "", "制造<color=#c2853d>强酸磁轨弹</color>加强近地防御力。");
            ProtoRegistry.RegisterString("子弹3tech描述", "", "制造<color=#c2853d>氘核爆破弹</color>进一步加强近地防御力。");
            ProtoRegistry.RegisterString("导弹2tech描述", "", "制造<color=#c2853d>反物质导弹</color>加强深空防御力。");


            ProtoRegistry.RegisterString("定向爆破1", "", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破2", "", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破3", "", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破4", "", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破5", "", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破6", "", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破描述", "", "通过精确计算子弹和导弹的索敌路径，预测撞击前的最佳起爆点，以尽可能对敌舰造成更大的破坏。");
            ProtoRegistry.RegisterString("定向爆破结论", "", "子弹、导弹伤害增加");

            ProtoRegistry.RegisterString("引力波引导1", "", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导2", "", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导3", "", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导4", "", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导5", "", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导6", "", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导描述", "", "弹道速度。");
            ProtoRegistry.RegisterString("引力波引导结论", "", "弹道速度增加");

            ProtoRegistry.RegisterString("相位干扰技术1", "", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术2", "", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术3", "", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术4", "", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术5", "", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术6", "", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术描述", "", "冲动刷新距离。");
            ProtoRegistry.RegisterString("相位干扰技术结论", "", "虫洞生成距离增加");


            ProtoRegistry.RegisterString("彩蛋1", "", "先驱者日记#1");
            ProtoRegistry.RegisterString("彩蛋2", "", "先驱者日记#2");
            ProtoRegistry.RegisterString("彩蛋3", "", "先驱者日记#3");
            ProtoRegistry.RegisterString("彩蛋4", "", "先驱者日记#4");
            ProtoRegistry.RegisterString("彩蛋5", "", "先驱者日记#5");
            ProtoRegistry.RegisterString("彩蛋6", "", "先驱者日记#6");
            ProtoRegistry.RegisterString("彩蛋7", "", "先驱者日记#7");
            ProtoRegistry.RegisterString("彩蛋8", "", "先驱者日记#8");
            ProtoRegistry.RegisterString("彩蛋9", "", "先驱者日记#9");


            ProtoRegistry.RegisterString("彩蛋1描述", "", "这种生物似乎是一种虫类，靠进食宇宙中的各种能量潮汐为生，同时还具有虫洞制造能力，一旦发现食物就会直接在附近建立虫洞。星际物流塔产生的能量潮汐让它变成了宇宙中的一座灯塔，能被这些生物轻易捕捉并视作食物来源。换言之，只要不建设星际物流塔，就不会惊扰它们。但这显然行不通，不用星际物流塔将极大的延缓任务完成的速度，这是主脑不愿意看到的。看来我需要一些防御设施来保护星际物流塔。唉，本来还说抽空思考一下宇宙的终极答案的……");
            ProtoRegistry.RegisterString("彩蛋2描述", "", "穿甲弹的效率还是太低了，我得试试找到强力的替代品，最好是能大范围产生伤害。就目前的情况来看，硫酸是个不错的选择，我可以试着直接把他们投射向敌人。再怎么说他们也是实体生物，酸蚀也够他们喝一壶的。还好它们不是异形，不然就是够我喝一壶的了。");
            ProtoRegistry.RegisterString("彩蛋3描述", "", "我还是把他们想的太简单了。随着产线扩张，物流塔的建设，他们的攻势越来越凶猛。大炮的覆盖范围有限，已经无法有效抵挡他们了。好在我这几天在B站上学会了造热核弹，把它发射上去肯定能解燃眉之急。至于怎么发射上去，那还真得研究研究了，炮管子实在是太细了，放不进去。赶快结束这一切吧，我还想去梦里数电子羊呢。");
            ProtoRegistry.RegisterString("彩蛋4描述", "", "热核导弹的杀伤力确实很大，但是发射也太慢了，我得研究研究怎么把热核导弹造成微缩版本塞进炮弹里面去，那不就既有杀伤力又有速度嘛。我可真是个天才！唉，你说我这天才为什么就不得伊卡拉姆妹妹的喜欢呢？我为她专门点亮了一片星系告白，但她居然说：“前天看到了小白兔，昨天是小鹿，今天是你。”这不是嘲讽我像个动物一样蠢吗。算了算了，不提了，女人只会影响我造戴森球的速度。");
            ProtoRegistry.RegisterString("彩蛋5描述", "", "倒霉透了，手滑泄露的那一点反物质差点把产线给扬了，害得我修复了好久。诶等等，能扬了我的产线为什么不能扬了那堆臭虫？思路打开了！死虫子你们给我等着，跟我轻型机甲拼你们有这个实力吗？我这就把反物质打上天，指定没有你们好果汁吃！");
            ProtoRegistry.RegisterString("彩蛋6描述", "", "真是的，我为什么要局限于现有的武器技术啊。我可是伊卡洛斯啊，能穿越黑洞，能超高速硬着陆，能去岩浆里泡澡去硫酸里游泳，我为什么不能大胆开发点离谱的武器？反正也是闲着，不如给虫子看看我的真本事！TNND，杀又杀不完，躲也躲不掉，还一直拖慢我的进度，跟我玩阴的是吧？直接来吧！");
            ProtoRegistry.RegisterString("彩蛋7描述", "", "先驱者果然只要我出手没什么办不到的，之前还是太局限了，戴森球都能造出来了为什么武器还需要弹药？直接相位裂解就完事了，要不是降维技术不可逆我都有心想丢二向箔过去。这下好啦，不用专门分心去造子弹了，我要继续去当第四天灾了。顺便说一句，这些虫子可比索林原虫差远了，虽然索林原虫也是渣渣~~日记");
            ProtoRegistry.RegisterString("彩蛋8描述", "", "年轻的伊卡洛斯，不管你是用什么方法走到了这一步，都足以证明你的聪颖和强大。即使是面对未知的风险，你依旧为主脑点亮了繁星。现在，你可以骄傲的说：“我见过你们绝对无法置信的事物，我目睹了虫洞在星系内诞生，我看着裂解射线在虫群之中闪烁，所有这些时刻，终将随时间消逝，一如眼泪消失在雨中。”现在继续你的征途吧！无论遇到什么困难，都不要怕，微笑着面对他！因为，你是一个一个一个勇敢的伊卡洛斯哼哼，啊啊啊啊啊啊啊啊啊啊啊！");
            ProtoRegistry.RegisterString("彩蛋9描述", "", "<color=#c2853d>42</color>");


            ItemProto bullet1 = ProtoRegistry.RegisterItem(8001, "子弹1", "子弹1描述", "Assets/DSPBattle/bullet1", 2701 + pageBias, 100, EItemType.Material);
            ItemProto bullet2 = ProtoRegistry.RegisterItem(8002, "子弹2", "子弹2描述", "Assets/DSPBattle/bullet2", 2702 + pageBias, 100, EItemType.Material);
            ItemProto bullet3 = ProtoRegistry.RegisterItem(8003, "子弹3", "子弹3描述", "Assets/DSPBattle/bullet3", 2703 + pageBias, 100, EItemType.Material);
            ItemProto missile1 = ProtoRegistry.RegisterItem(8004, "导弹1", "导弹1描述", "Assets/DSPBattle/missile1", 2705 + pageBias, 100, EItemType.Material);
            ItemProto missile2 = ProtoRegistry.RegisterItem(8005, "导弹2", "导弹2描述", "Assets/DSPBattle/missile2", 2706 + pageBias, 100, EItemType.Material);
            ItemProto missile3 = ProtoRegistry.RegisterItem(8006, "导弹3", "导弹3描述", "Assets/DSPBattle/missile3", 2707 + pageBias, 100, EItemType.Material);
            ItemProto bullet4 = ProtoRegistry.RegisterItem(8007, "脉冲", "脉冲描述", "Assets/DSPBattle/bullet4", 9999, 100, EItemType.Material);

            bullet1.DescFields = new int[] { 50, 51, 1 };
            bullet2.DescFields = new int[] { 50, 51, 1 };
            bullet3.DescFields = new int[] { 50, 51, 1 };
            bullet4.DescFields = new int[] { 50, 51 };
            missile1.DescFields = new int[] { 50, 51, 52, 1 };
            missile2.DescFields = new int[] { 50, 51, 52, 1 };
            missile3.DescFields = new int[] { 50, 51, 52, 1 };

            ItemProto easterEgg1 = ProtoRegistry.RegisterItem(8021, "彩蛋1", "彩蛋1描述", "Assets/DSPBattle/notes-of-pioneer-01", 9999, 100, EItemType.Material);
            ItemProto easterEgg2 = ProtoRegistry.RegisterItem(8022, "彩蛋2", "彩蛋2描述", "Assets/DSPBattle/notes-of-pioneer-02", 9999, 100, EItemType.Material);
            ItemProto easterEgg3 = ProtoRegistry.RegisterItem(8023, "彩蛋3", "彩蛋3描述", "Assets/DSPBattle/notes-of-pioneer-03", 9999, 100, EItemType.Material);
            ItemProto easterEgg4 = ProtoRegistry.RegisterItem(8024, "彩蛋4", "彩蛋4描述", "Assets/DSPBattle/notes-of-pioneer-04", 9999, 100, EItemType.Material);
            ItemProto easterEgg5 = ProtoRegistry.RegisterItem(8025, "彩蛋5", "彩蛋5描述", "Assets/DSPBattle/notes-of-pioneer-05", 9999, 100, EItemType.Material);
            ItemProto easterEgg6 = ProtoRegistry.RegisterItem(8026, "彩蛋6", "彩蛋6描述", "Assets/DSPBattle/notes-of-pioneer-06", 9999, 100, EItemType.Material);
            ItemProto easterEgg7 = ProtoRegistry.RegisterItem(8027, "彩蛋7", "彩蛋7描述", "Assets/DSPBattle/notes-of-pioneer-07", 9999, 100, EItemType.Material);
            ItemProto easterEgg8 = ProtoRegistry.RegisterItem(8028, "彩蛋8", "彩蛋8描述", "Assets/DSPBattle/notes-of-pioneer-08", 9999, 100, EItemType.Material);
            ItemProto easterEgg9 = ProtoRegistry.RegisterItem(8029, "彩蛋9", "彩蛋9描述", "Assets/DSPBattle/notes-of-pioneer-09", 9999, 100, EItemType.Material);


            var Cannon = ProtoRegistry.RegisterItem(8011, "弹射器1", "弹射器1描述", "Assets/DSPBattle/cannon1", 2601 + pageBias, 50, EItemType.Production);
            Cannon.BuildIndex = 607;
            Cannon.BuildMode = 1;
            Cannon.IsEntity = true;
            Cannon.isRaw = false;
            Cannon.CanBuild = true;
            Cannon.Upgrades = new int[] { };
            Cannon.DescFields = new int[] { 53, 11, 12, 1, 40 };
            var Cannon2 = ProtoRegistry.RegisterItem(8012, "弹射器2", "弹射器2描述", "Assets/DSPBattle/cannon2", 2602 + pageBias, 50, EItemType.Production);
            Cannon2.BuildIndex = 608;
            Cannon2.BuildMode = 1;
            Cannon2.IsEntity = true;
            Cannon2.isRaw = false;
            Cannon2.CanBuild = true;
            Cannon2.Upgrades = new int[] { };
            Cannon2.DescFields = new int[] { 53, 11, 12, 1, 40 };
            var Cannon3 = ProtoRegistry.RegisterItem(8014, "脉冲炮", "脉冲炮描述", "Assets/DSPBattle/cannon3", 2604 + pageBias, 50, EItemType.Production);
            Cannon3.BuildIndex = 609;
            Cannon3.BuildMode = 1;
            Cannon3.IsEntity = true;
            Cannon3.isRaw = false;
            Cannon3.CanBuild = true;
            Cannon3.Upgrades = new int[] { };
            Cannon3.DescFields = new int[] { 53, 11, 12, 1, 40 };


            var Silo = ProtoRegistry.RegisterItem(8013, "发射器1", "发射器1描述", "Assets/DSPBattle/missilesilo", 2603 + pageBias, 50, EItemType.Production);
            Silo.BuildIndex = 610;
            Silo.BuildMode = 1;
            Silo.IsEntity = true;
            Silo.isRaw = false;
            Silo.CanBuild = true;
            Silo.Upgrades = new int[] { };
            Silo.DescFields = new int[] { 35, 11, 12, 1, 40 };

            ProtoRegistry.RegisterRecipe(801, ERecipeType.Assemble, 60, new int[] { 1112, 1103 }, new int[] { 1, 1 }, new int[] { 8001 }, new int[] { 1 }, "子弹1描述",
                1901, 2701 + pageBias, "Assets/DSPBattle/bullet1");
            ProtoRegistry.RegisterRecipe(802, ERecipeType.Assemble, 90, new int[] { 1118, 1110, 1116 }, new int[] { 1, 1, 1 }, new int[] { 8002 }, new int[] { 1 }, "子弹2描述",
                1902, 2702 + pageBias, "Assets/DSPBattle/bullet2");
            ProtoRegistry.RegisterRecipe(803, ERecipeType.Assemble, 120, new int[] { 1118, 1121, 1206 }, new int[] { 1, 4, 1 }, new int[] { 8003 }, new int[] { 1 }, "子弹3描述",
                1903, 2703 + pageBias, "Assets/DSPBattle/bullet3");
            ProtoRegistry.RegisterRecipe(804, ERecipeType.Assemble, 120, new int[] { 1802, 1303, 1406 }, new int[] { 1, 1, 1 }, new int[] { 8004 }, new int[] { 1 }, "导弹1描述",
                1911, 2704 + pageBias, "Assets/DSPBattle/missile1");
            ProtoRegistry.RegisterRecipe(805, ERecipeType.Assemble, 120, new int[] { 1803, 1303, 1406 }, new int[] { 1, 1, 1 }, new int[] { 8005 }, new int[] { 1 }, "导弹2描述",
                1912, 2705 + pageBias, "Assets/DSPBattle/missile2");
            ProtoRegistry.RegisterRecipe(806, ERecipeType.Assemble, 120, new int[] { 1209, 1303, 1406 }, new int[] { 1, 2, 1 }, new int[] { 8006 }, new int[] { 1 }, "导弹3描述",
                1914, 2706 + pageBias, "Assets/DSPBattle/missile3");
            ProtoRegistry.RegisterRecipe(811, ERecipeType.Assemble, 360, new int[] { 1103, 1201, 1303, 1205 }, new int[] { 10, 10, 10, 5 }, new int[] { 8011 }, new int[] { 1 }, "弹射器1描述",
                1901, 2601 + pageBias, "Assets/DSPBattle/cannon1");
            ProtoRegistry.RegisterRecipe(812, ERecipeType.Assemble, 360, new int[] { 1107, 1206, 1303, 1209 }, new int[] { 10, 10, 10, 3 }, new int[] { 8012 }, new int[] { 1 }, "弹射器2描述",
                1914, 2602 + pageBias, "Assets/DSPBattle/cannon2");
            ProtoRegistry.RegisterRecipe(813, ERecipeType.Assemble, 900, new int[] { 1107, 1125, 1209, 1305 }, new int[] { 40, 15, 5, 5 }, new int[] { 8013 }, new int[] { 1 }, "发射器1描述",
                1911, 2603 + pageBias, "Assets/DSPBattle/missilesilo");
            ProtoRegistry.RegisterRecipe(814, ERecipeType.Assemble, 900, new int[] { 1125, 1402, 1304, 1305 }, new int[] { 20, 10, 10, 5 }, new int[] { 8014 }, new int[] { 1 }, "脉冲炮描述",
                1915, 2604 + pageBias, "Assets/DSPBattle/cannon3");

            TechProto techBullet1 = ProtoRegistry.RegisterTech(1901, "近地防卫系统", "近地防卫系统描述", "近地防卫系统结论", "Assets/DSPBattle/bullet1tech", new int[] { 1711 }, new int[] { 6001, 6002 }, new int[] { 20, 20 },
                72000, new int[] { 801, 811 }, new Vector2(29, -43));
            techBullet1.PreTechsImplicit = new int[] { 1503 };
            techBullet1.AddItems = new int[] { 8021 };
            techBullet1.AddItemCounts = new int[] { 1 };
            TechProto techBullet2 = ProtoRegistry.RegisterTech(1902, "子弹2", "子弹2tech描述", "子弹2结论", "Assets/DSPBattle/bullet2tech", new int[] { 1901 }, new int[] { 6001, 6002, 6003 }, new int[] { 12, 12, 12 },
                150000, new int[] { 802 }, new Vector2(33, -43));
            techBullet2.AddItems = new int[] { 8022 };
            techBullet2.AddItemCounts = new int[] { 1 };
            TechProto techBullet3 = ProtoRegistry.RegisterTech(1903, "子弹3", "子弹3tech描述", "子弹3结论", "Assets/DSPBattle/bullet3tech", new int[] { 1902 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 48, 24, 24 }, 150000, new int[] { 803 }, new Vector2(37, -43));
            techBullet3.PreTechsImplicit = new int[] { 1911 };
            techBullet3.AddItems = new int[] { 8024 };
            techBullet3.AddItemCounts = new int[] { 1 };
            TechProto techMissile1 = ProtoRegistry.RegisterTech(1911, "深空防卫系统", "深空防卫系统描述", "深空防卫系统结论", "Assets/DSPBattle/missile1tech", new int[] { 1114 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 804, 813 }, new Vector2(37, -31));
            techMissile1.PreTechsImplicit = new int[] { 1522, 1416 };
            techMissile1.AddItems = new int[] { 8023 };
            techMissile1.AddItemCounts = new int[] { 1 };
            TechProto techMissile2 = ProtoRegistry.RegisterTech(1912, "导弹2", "导弹2tech描述", "导弹2结论", "Assets/DSPBattle/missile2tech", new int[] { 1911 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 805 }, new Vector2(41, -31));
            techMissile2.AddItems = new int[] { 8025 };
            techMissile2.AddItemCounts = new int[] { 1 };
            //ProtoRegistry.RegisterTech(1913, "导弹3", "导弹3描述", "导弹3结论", "Icons/Tech/1112", new int[] { 1914 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
            //    new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 804, 813 }, new Vector2(49, -41));

            TechProto techMissile3 = ProtoRegistry.RegisterTech(1914, "引力操控技术", "引力操控技术描述", "引力操控技术结论", "Assets/DSPBattle/missile3tech", new int[] { 1704 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 12, 48, 24, 24, 24 }, 150000, new int[] { 812, 806 }, new Vector2(49, -43));
            techMissile3.AddItems = new int[] { 8026 };
            techMissile3.AddItemCounts = new int[] { 1 };
            TechProto techBullet4 = ProtoRegistry.RegisterTech(1915, "相位裂解技术", "相位裂解技术描述", "相位裂解技术结论", "Assets/DSPBattle/cannon3tech", new int[] { 1914 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 814 }, new Vector2(53, -43));
            techBullet4.AddItems = new int[] { 8027 };
            techBullet4.AddItemCounts = new int[] { 1 };

            TechProto winGame = LDB.techs.Select(1508);
            winGame.AddItems = new int[] { 8028, 8029 };
            winGame.AddItemCounts = new int[] { 1, 1 };

            //循环科技 分别是+20%子弹伤害  +10%子弹速度和2%导弹速度  以及扩充虫洞安全区
            TechProto techBulletDamage1 = ProtoRegistry.RegisterTech(4901, "定向爆破1", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level1_tech", new int[] { }, new int[] { 6001, 6002, 6003 },
                new int[] { 20, 20, 20 }, 180000, new int[] { }, new Vector2(9, -47));
            techBulletDamage1.PreTechsImplicit = new int[] { 1911 };
            techBulletDamage1.UnlockFunctions = new int[] { 50 };
            techBulletDamage1.UnlockValues = new double[] { 0.15 };
            techBulletDamage1.Level = 1;
            techBulletDamage1.MaxLevel = 1;
            techBulletDamage1.LevelCoef1 = 0;
            techBulletDamage1.LevelCoef2 = 0;
            TechProto techBulletDamage2 = ProtoRegistry.RegisterTech(4902, "定向爆破2", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level2_tech", new int[] { 4901 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 20, 20, 20, 20 }, 180000, new int[] { }, new Vector2(13, -47));
            techBulletDamage2.UnlockFunctions = new int[] { 50 };
            techBulletDamage2.UnlockValues = new double[] { 0.15 };
            techBulletDamage2.Level = 2;
            techBulletDamage2.MaxLevel = 2;
            techBulletDamage2.LevelCoef1 = 0;
            techBulletDamage2.LevelCoef2 = 0;
            TechProto techBulletDamage3 = ProtoRegistry.RegisterTech(4903, "定向爆破3", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level3_tech", new int[] { 4902 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 12, 12, 12 }, 480000, new int[] { }, new Vector2(17, -47));
            techBulletDamage3.UnlockFunctions = new int[] { 50 };
            techBulletDamage3.UnlockValues = new double[] { 0.15 };
            techBulletDamage3.Level = 3;
            techBulletDamage3.MaxLevel = 3;
            techBulletDamage3.LevelCoef1 = 0;
            techBulletDamage3.LevelCoef2 = 0;
            TechProto techBulletDamage4 = ProtoRegistry.RegisterTech(4904, "定向爆破4", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level4_tech", new int[] { 4903 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 4, 4, 4, 4, 4 }, 1800000, new int[] { }, new Vector2(21, -47));
            techBulletDamage4.UnlockFunctions = new int[] { 50 };
            techBulletDamage4.UnlockValues = new double[] { 0.15 };
            techBulletDamage4.Level = 4;
            techBulletDamage4.MaxLevel = 4;
            techBulletDamage4.LevelCoef1 = 0;
            techBulletDamage4.LevelCoef2 = 0;
            TechProto techBulletDamage5 = ProtoRegistry.RegisterTech(4905, "定向爆破5", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level5_tech", new int[] { 4904 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 4, 4, 4, 4, 4 }, 2700000, new int[] { }, new Vector2(25, -47));
            techBulletDamage5.UnlockFunctions = new int[] { 50 };
            techBulletDamage5.UnlockValues = new double[] { 0.15 };
            techBulletDamage5.Level = 5;
            techBulletDamage5.MaxLevel = 5;
            techBulletDamage5.LevelCoef1 = 0;
            techBulletDamage5.LevelCoef2 = 0;
            TechProto techBulletDamageInf = ProtoRegistry.RegisterTech(4906, "定向爆破6", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level-infinitude_tech", new int[] { 4905 }, new int[] { 6006 },
                new int[] { 4 }, -18000000, new int[] { }, new Vector2(29, -47));
            techBulletDamageInf.UnlockFunctions = new int[] { 50 };
            techBulletDamageInf.UnlockValues = new double[] { 0.15 };
            techBulletDamageInf.Level = 6;
            techBulletDamageInf.MaxLevel = 10000;
            techBulletDamageInf.LevelCoef1 = 3600000;
            techBulletDamageInf.LevelCoef2 = 0;


            TechProto techBulletSpeed1 = ProtoRegistry.RegisterTech(4911, "引力波引导1", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech1", new int[] { }, new int[] { 6001, 6002, 6003 },
                 new int[] { 20, 20, 20 }, 180000, new int[] { }, new Vector2(9, -51));
            techBulletSpeed1.PreTechsImplicit = new int[] { 1911 };
            techBulletSpeed1.UnlockFunctions = new int[] { 51 };
            techBulletSpeed1.UnlockValues = new double[] { 0.1 };
            techBulletSpeed1.Level = 1;
            techBulletSpeed1.MaxLevel = 1;
            techBulletSpeed1.LevelCoef1 = 0;
            techBulletSpeed1.LevelCoef2 = 0;
            TechProto techBulletSpeed2 = ProtoRegistry.RegisterTech(4912, "引力波引导2", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech2", new int[] { 4911 }, new int[] { 6001, 6002, 6003, 6004 },
                 new int[] { 20, 20, 20, 20 }, 180000, new int[] { }, new Vector2(13, -51));
            techBulletSpeed2.UnlockFunctions = new int[] { 51 };
            techBulletSpeed2.UnlockValues = new double[] { 0.1 };
            techBulletSpeed2.Level = 2;
            techBulletSpeed2.MaxLevel = 2;
            techBulletSpeed2.LevelCoef1 = 0;
            techBulletSpeed2.LevelCoef2 = 0;
            TechProto techBulletSpeed3 = ProtoRegistry.RegisterTech(4913, "引力波引导3", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech3", new int[] { 4912 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 12, 12, 12 }, 480000, new int[] { }, new Vector2(17, -51));
            techBulletSpeed3.UnlockFunctions = new int[] { 51 };
            techBulletSpeed3.UnlockValues = new double[] { 0.1 };
            techBulletSpeed3.Level = 3;
            techBulletSpeed3.MaxLevel = 3;
            techBulletSpeed3.LevelCoef1 = 0;
            techBulletSpeed3.LevelCoef2 = 0;
            TechProto techBulletSpeed4 = ProtoRegistry.RegisterTech(4914, "引力波引导4", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech4", new int[] { 4913 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 4, 4, 4, 4, 4 }, 1800000, new int[] { }, new Vector2(21, -51));
            techBulletSpeed4.UnlockFunctions = new int[] { 51 };
            techBulletSpeed4.UnlockValues = new double[] { 0.1 };
            techBulletSpeed4.Level = 3;
            techBulletSpeed4.MaxLevel = 3;
            techBulletSpeed4.LevelCoef1 = 0;
            techBulletSpeed4.LevelCoef2 = 0;
            TechProto techBulletSpeed5 = ProtoRegistry.RegisterTech(4915, "引力波引导5", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech5", new int[] { 4914 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 4, 4, 4, 4, 4 }, 2700000, new int[] { }, new Vector2(25, -51));
            techBulletSpeed5.UnlockFunctions = new int[] { 51 };
            techBulletSpeed5.UnlockValues = new double[] { 0.1 };
            techBulletSpeed5.Level = 3;
            techBulletSpeed5.MaxLevel = 3;
            techBulletSpeed5.LevelCoef1 = 0;
            techBulletSpeed5.LevelCoef2 = 0;
            TechProto techBulletSpeedInf = ProtoRegistry.RegisterTech(4916, "引力波引导6", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech0", new int[] { 4915 }, new int[] { 6006 },
                new int[] { 4 }, -18000000, new int[] { }, new Vector2(29, -51));
            techBulletSpeedInf.UnlockFunctions = new int[] { 51 };
            techBulletSpeedInf.UnlockValues = new double[] { 0.1 };
            techBulletSpeedInf.Level = 6;
            techBulletSpeedInf.MaxLevel = 100;
            techBulletSpeedInf.LevelCoef1 = 3600000;
            techBulletSpeedInf.LevelCoef2 = 0;

            TechProto techWormDistance1 = ProtoRegistry.RegisterTech(4921, "相位干扰技术1", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level1_tech", new int[] { }, new int[] { 6001, 6002, 6003 },
                new int[] { 20, 20, 20 }, 144000, new int[] { }, new Vector2(9, -55));
            techWormDistance1.PreTechsImplicit = new int[] { 1911 };
            techWormDistance1.UnlockFunctions = new int[] { 52 };
            techWormDistance1.UnlockValues = new double[] { 10000 };
            techWormDistance1.Level = 1;
            techWormDistance1.MaxLevel = 1;
            techWormDistance1.LevelCoef1 = 0;
            techWormDistance1.LevelCoef2 = 0;
            TechProto techWormDistance2 = ProtoRegistry.RegisterTech(4922, "相位干扰技术2", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level2_tech", new int[] { 4921 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 12, 12, 12 }, 300000, new int[] { }, new Vector2(13, -55));
            techWormDistance2.UnlockFunctions = new int[] { 52 };
            techWormDistance2.UnlockValues = new double[] { 10000 };
            techWormDistance2.Level = 2;
            techWormDistance2.MaxLevel = 2;
            techWormDistance2.LevelCoef1 = 0;
            techWormDistance2.LevelCoef2 = 0;
            TechProto techWormDistance3 = ProtoRegistry.RegisterTech(4923, "相位干扰技术3", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level3_tech", new int[] { 4922 }, new int[] { 6001, 6002, 6003, 6004 },
                 new int[] { 12, 12, 12, 12 }, 360000, new int[] { }, new Vector2(17, -55));
            techWormDistance3.UnlockFunctions = new int[] { 52 };
            techWormDistance3.UnlockValues = new double[] { 10000 };
            techWormDistance3.Level = 3;
            techWormDistance3.MaxLevel = 3;
            techWormDistance3.LevelCoef1 = 0;
            techWormDistance3.LevelCoef2 = 0;
            TechProto techWormDistance4 = ProtoRegistry.RegisterTech(4924, "相位干扰技术4", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level4_tech", new int[] { 4923 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                 new int[] { 12, 12, 12, 12, 12 }, 360000, new int[] { }, new Vector2(21, -55));
            techWormDistance4.UnlockFunctions = new int[] { 52 };
            techWormDistance4.UnlockValues = new double[] { 10000 };
            techWormDistance4.Level = 4;
            techWormDistance4.MaxLevel = 4;
            techWormDistance4.LevelCoef1 = 0;
            techWormDistance4.LevelCoef2 = 0;
            TechProto techWormDistance5 = ProtoRegistry.RegisterTech(4925, "相位干扰技术5", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level5_tech", new int[] { 4924 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                 new int[] { 4, 4, 4, 4, 4 }, 1440000, new int[] { }, new Vector2(25, -55));
            techWormDistance5.UnlockFunctions = new int[] { 52 };
            techWormDistance5.UnlockValues = new double[] { 10000 };
            techWormDistance5.Level = 5;
            techWormDistance5.MaxLevel = 5;
            techWormDistance5.LevelCoef1 = 0;
            techWormDistance5.LevelCoef2 = 0;
            TechProto techWormDistanceInf = ProtoRegistry.RegisterTech(4926, "相位干扰技术6", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level-infinitude_tech", new int[] { 4925 }, new int[] { 6006 },
                 new int[] { 4 }, 45000000, new int[] { }, new Vector2(29, -55));
            techWormDistanceInf.UnlockFunctions = new int[] { 52 };
            techWormDistanceInf.UnlockValues = new double[] { 10000 };
            techWormDistanceInf.Level = 6;
            techWormDistanceInf.MaxLevel = 60;
            techWormDistanceInf.LevelCoef1 = -18000000;
            techWormDistanceInf.LevelCoef2 = 1800000;



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


            LDBTool.SetBuildBar(6, 7, 8011);
            LDBTool.SetBuildBar(6, 8, 8012);
            LDBTool.SetBuildBar(6, 9, 8014);
            LDBTool.SetBuildBar(6, 10, 8013);
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
                    Configs.wormholeRangeAdded += (int)value;
                    break;
                default:
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TechProto), "UnlockFunctionText")]
        public static void UnlockFunctionTextPatch(ref TechProto __instance, ref string __result, StringBuilder sb)
        {
            if (__instance.ID >= 4901 && __instance.ID <= 4906)
                __result = "子弹伤害和导弹伤害+15%".Translate() + "\n" + "脉冲中子束伤害+25%".Translate();
            else if (__instance.ID >= 4911 && __instance.ID <= 4916)
                __result = "子弹飞行速度+10%".Translate() + "\n" + "导弹飞行速度+5%".Translate();
            //else if (__instance.ID == 4921)
            //    __result = "虫洞生成最近范围向10AU推进3%".Translate();
            //else if (__instance.ID == 4922)
            //    __result = "虫洞生成最近范围向10AU推进5%".Translate();
            //else if (__instance.ID == 4923)
            //    __result = "虫洞生成最近范围向10AU推进8%".Translate();
            else if (__instance.ID >= 4921 && __instance.ID <= 4926)
                __result = "虫洞生成最近范围扩大0.25AU".Translate();
        }

        public static Text infoLabel = null;
        public static Text infoValue = null;

        public static void InitTechInfoUIs()
        {
            GameObject infoLabelObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Fullscreen UIs/Tech Tree/info-panel/label");
            infoLabel = infoLabelObj.GetComponent<Text>();

            GameObject infoValueObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Fullscreen UIs/Tech Tree/info-panel/value");
            infoValue = infoValueObj.GetComponent<Text>();

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UITechTree), "OnPageChanged")]
        public static void OnPageChangedPatch(ref UITechTree __instance)
        {
            if (infoLabel == null)
                InitTechInfoUIs();
            if (infoLabel.text.Split('\n').Length < 35)
            {
                infoLabel.text = infoLabel.text + "\r\n\r\n" + "子弹伤害".Translate() + "\r\n" + "脉冲中子束伤害".Translate() + "\r\n"
                    + "导弹伤害".Translate() + "\r\n" + "子弹飞行速度".Translate() + "\r\n" + "导弹飞行速度".Translate() + "\r\n" + "虫洞干扰半径".Translate();
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(UITechTree), "RefreshDataValueText")]
        public static void UnlockFuncTextPatch(ref UITechTree __instance)
        {
            
            __instance.dataValueText.text = __instance.dataValueText.text + "\r\n\r\n" + Configs.bulletAtkScale.ToString("0%") + "\r\n"
                + (1 + (Configs.bulletAtkScale - 1) * 5 / 3).ToString("0%") + "\r\n" + Configs.bulletAtkScale.ToString("0%")
                + "\r\n" + Configs.bulletSpeedScale.ToString("0%") + "\r\n" + (1 + (Configs.bulletSpeedScale - 1) * 0.5).ToString("0%") + "\r\n" + (Configs.wormholeRange / 40000.0).ToString() + "AU";

        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemProto), "GetPropName")]
        public static void GetPropNamePatch(ref ItemProto __instance, int index, ref string __result)
        {
            if ((ulong)index >= (ulong)((long)__instance.DescFields.Length))
            {
                __result = "";
                return;
            }
            switch (__instance.DescFields[index])
            {
                case 50:
                    __result = "伤害".Translate();
                    return;
                case 51:
                    __result = "弹道速度".Translate();
                    return;
                case 52:
                    __result = "伤害半径".Translate();
                    return;
                case 53:
                    __result = "射速".Translate();
                    return;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemProto), "GetPropValue")]
        public static void GetPropValuePatch(ref ItemProto __instance, int index, ref string __result)
        {
            if ((ulong)index >= (ulong)((long)__instance.DescFields.Length))
            {
                __result = "";
                return;
            }
            switch (__instance.DescFields[index])
            {
                case 50:
                    __result = GetBulletInfos(__instance.ID)[0].ToString();
                    return;
                case 51:
                    __result = GetBulletInfos(__instance.ID)[1].ToString() + " m/s";
                    return;
                case 52:
                    __result = GetBulletInfos(__instance.ID)[2].ToString() + " m";
                    return;
                case 53:
                    if (__instance.prefabDesc.isEjector)
                    {
                        __result = (3600.0 / (double)(__instance.prefabDesc.ejectorChargeFrame + __instance.prefabDesc.ejectorColdFrame)).ToString("0.##") + "/min";
                        return;
                    }
                    if (__instance.prefabDesc.isSilo)
                    {
                        __result = (3600.0 / (double)(__instance.prefabDesc.siloChargeFrame + __instance.prefabDesc.siloColdFrame)).ToString("0.##") + "/min";
                        return;
                    }
                    return;
            }
        }


        public static int[] GetBulletInfos(int protoId)
        {
            switch (protoId)
            {
                case 8001:
                    return new int[] { Configs.bullet1Atk, Mathf.RoundToInt((float)Configs.bullet1Speed), 0 };
                case 8002:
                    return new int[] { Configs.bullet2Atk, Mathf.RoundToInt((float)Configs.bullet2Speed), 0 };
                case 8003:
                    return new int[] { Configs.bullet3Atk, Mathf.RoundToInt((float)Configs.bullet3Speed), 0 };
                case 8004:
                    return new int[] { Configs.missile1Atk, Mathf.RoundToInt((float)Configs.missile1Speed), Configs.missile1Range };
                case 8005:
                    return new int[] { Configs.missile2Atk, Mathf.RoundToInt((float)Configs.missile2Speed), Configs.missile2Range };
                case 8006:
                    return new int[] { Configs.missile3Atk, Mathf.RoundToInt((float)Configs.missile3Speed), Configs.missile3Range };
                case 8007:
                    return new int[] { Configs.bullet4Atk, Mathf.RoundToInt((float)Configs.bullet4Speed), 0 };

                default:
                    return new int[] { 0, 0, 0 };
            }
        }

    }
}
