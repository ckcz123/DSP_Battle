using CommonAPI.Systems;
using HarmonyLib;
using System;
using System.Collections.Generic;
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
            //官方星图界面 橙色高亮字体color=FD965EC0
            //增产效果的蓝色高亮字体 0.3821 0.8455 1 0.7059 : 61d8ffb4

            ProtoRegistry.RegisterString("轨道防御", "Defense", "轨道防御");

            ProtoRegistry.RegisterString("子弹1", "Armour piercing", "穿甲磁轨弹");
            ProtoRegistry.RegisterString("子弹1描述", "A cheap bullet, attack single enemy.", "从来没见过敌人的先驱者把老祖宗的数据库翻了114514遍，东拼西凑出来一个穿甲弹设计图。由于没找到火药配方，只好照着炮弹的样子铸造成实心炮弹并加强其结构。仅能进行单体动能打击，杀伤力有限。");
            ProtoRegistry.RegisterString("子弹1结论", "You have unlocked armour piercing.", "你解锁了穿甲磁轨弹，可以利用动能进行攻击");
            ProtoRegistry.RegisterString("子弹2", "Acid bullet", "强酸磁轨弹");
            ProtoRegistry.RegisterString("子弹2描述", "A powerful bullet by sulfuric acid.", "在对敌人进行分析和研究后，先驱者大胆将<color=#c2853d>硫酸</color>封装后制成了一种新的炮弹。爆破后可以对范围内敌人产生酸蚀，更加高效地杀伤敌人。");
            ProtoRegistry.RegisterString("子弹2结论", "You have unlocked acid bullet.", "你解锁了强酸磁轨弹，可以利用硫酸腐蚀外壳");
            ProtoRegistry.RegisterString("子弹3", "Deuterium nucleus", "氘核爆破弹");
            ProtoRegistry.RegisterString("子弹3短", "D-nucleus", "氘核爆破弹");
            ProtoRegistry.RegisterString("子弹3描述", "A more powerful bullet with a micro-thermonuclear-boom in it.", "制造出<color=#c2853d>热核导弹</color>的先驱者想要让他更小更快，便试着把核弹微缩化封装进了子弹里。该武器可以在命中后发生聚变爆炸，对敌人造成大量伤害。");
            ProtoRegistry.RegisterString("子弹3结论", "You have unlocked deuterium nucleus", "你解锁了氘核爆破弹，可以利用核聚变进行破坏");
            ProtoRegistry.RegisterString("脉冲", "Phase-cracking beam", "相位裂解光束");
            ProtoRegistry.RegisterString("脉冲短", "Cracking beam", "相位裂解光束");
            ProtoRegistry.RegisterString("脉冲描述", "The beam doesn't need to be produced, just provide enough energy for the <color=#c2853d>Phaser emitter</color> to eject.", "这种光束是不需要制作和提供的，只需给<color=#c2853d>相位裂解炮</color>提供足够电量即可无限发射。");

            ProtoRegistry.RegisterString("导弹1", "Thermonuclear missile", "热核导弹");
            ProtoRegistry.RegisterString("导弹1短", "Nuclear missile", "热核导弹");
            ProtoRegistry.RegisterString("导弹1描述", "A powerful nuclear missile", "这天，百无聊赖的先驱者正在刷哔哩哔哩，突然刷到了一条《如何在卧室制造核弹》的视频，于是这种导弹便被制造了出来。这是一种重型武器，发射升空并命中敌人后产生核爆，造成大范围伤害。");
            ProtoRegistry.RegisterString("导弹1结论", "You have unlocked thermonuclear missile.", "你解锁了热核导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("导弹2", "Antimatter missile", "反物质导弹");
            ProtoRegistry.RegisterString("导弹2短", "A-M missile", "反物质导弹");
            ProtoRegistry.RegisterString("导弹2描述", "A missile has a bit of antimatter in it, turning anything into oblivion after explosion.", "先驱者在一次搬运货物时不慎手滑造成微量<color=#c2853d>反物质</color>泄露，这导致他辛辛苦苦拉好的产线毁于一旦。“为什么不让敌人尝尝这种痛苦呢？”他想到。于是这种将<color=#c2853d>反物质</color>封装入导弹的武器被制造了出来，命中敌人后会发生湮灭，将敌人彻底抹杀。");
            ProtoRegistry.RegisterString("导弹2结论", "You have unlocked antimatter missile.", "你解锁了反物质导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("导弹3", "Gravitational collapse missile", "引力塌陷导弹");
            ProtoRegistry.RegisterString("导弹3短", "Grav-missile", "引力塌陷导弹");
            ProtoRegistry.RegisterString("导弹3描述", "A missile can creates a micro-black-hole instantaneously after exposion.", "发明出<color=#c2853d>引力弹射炮</color>的先驱者一鼓作气，将微型黑洞封装进导弹，制成了这种超级武器。它能在爆炸后短暂生成一个微型黑洞将范围内的敌人聚拢，简单高效。");
            ProtoRegistry.RegisterString("导弹3结论", "You have unlocked gravitational collapse missile.", "你解锁了引力塌陷导弹，可以自动追踪敌人");

            ProtoRegistry.RegisterString("弹射器1", "Super railgun", "电磁轨道炮");
            ProtoRegistry.RegisterString("弹射器1描述", "An giant railgun based on electromagnetic-ejection technology, ejecting any physical ammunition.", "制造出<color=#c2853d>穿甲磁轨弹</color>的先驱者不知道该如何把他们扔上天，直到他不小心把<color=#c2853d>太阳帆</color>误装成了炮弹。“好极了，现在我‘创造’了一种武器。”先驱者这样想着，“但是怎么区分他们呢？”。随后，先驱者默念着：“你指尖闪动的电光，是我此生不变的信仰……”给它装上了橙色氛围灯。该武器可以装入任何类型的子弹，且打击范围受到仰角限制。");
            ProtoRegistry.RegisterString("弹射器1结论", "You have unlocked super railgun.", "你解锁了电磁轨道炮，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("弹射器2", "Gravitation Slingshot ejector", "引力弹射炮");
            ProtoRegistry.RegisterString("弹射器2描述", "An giant ejector, speed up any physical ammunition by two micro-black-hole inside it.", "在黑洞杀了个七进七出之后，先驱者对引力操控有了深入见解，他终于决定自己开发一种武器。于是他拆除了<color=#c2853d>电磁轨道炮</color>的加速磁场，制造了一种利用微型黑洞制造引力弹弓的发生器，这使得发射炮弹获得了数倍于之前的动能。“这回可不是换个氛围灯那么简单了！”先驱者自我陶醉着。该武器能以更高的射速发射任何类型的子弹。");
            ProtoRegistry.RegisterString("弹射器2结论", "You have unlocked Gravitation Slingshot ejector.", "你解锁了引力弹射炮，可以发射子弹攻击敌方");

            ProtoRegistry.RegisterString("脉冲炮", "Phaser emitter", "相位裂解炮");
            ProtoRegistry.RegisterString("脉冲炮描述", "Just like weapons you've seen in sci-fi movies and games, ejects powerful radiation to break down enemy. <color=#c2853d>Physical bullets needless.</color>", "开发出超级武器的先驱者百般无聊，整日靠着当P社战犯度日，但是他灭绝寰宇的大业总是因为弹药短缺被打断，于是他一气之下依照《群星》的舰载武器开发了这种武器。利用发射的高能粒子流产生相位裂解链式反应，大范围杀伤敌人，最大的优点是，<color=#c2853d>无需弹药</color>！从此，先驱者在成为战犯的道路上越走越远。");
            ProtoRegistry.RegisterString("脉冲炮结论", "You have unlocked Phaser emitter.", "你解锁了相位裂解炮，可以仅使用电力攻击敌方");

            ProtoRegistry.RegisterString("发射器1", "Void missile launching silo", "深空导弹发射井");
            ProtoRegistry.RegisterString("发射器1描述", "Just a launching-silo.", "制造出<color=#c2853d>热核导弹</color>的先驱者并不知道如何把他们扔上天去，直到他看到了<color=#c2853d>垂直发射井</color>。“只要把它刷成红色我不就发明了一种武器吗？”先驱者这样想到。该武器可以对敌人进行全方位打击。");
            ProtoRegistry.RegisterString("发射器1结论", "You have unlocked void missile launching silo.", "你解锁了深空导弹发射井，可以发射导弹攻击敌方");

            ProtoRegistry.RegisterString("近地防卫系统", "Near Earth Def-system", "近地防卫系统");
            ProtoRegistry.RegisterString("近地防卫系统描述", "Manufacturing <color=#c2853d>super-railgun</color> to eject <color=#c2853d>armour-piercing</color> to bulid basal defensive system.", "制造<color=#c2853d>电磁轨道炮</color>发射<color=#c2853d>穿甲磁轨弹</color>进行基础防御。");
            ProtoRegistry.RegisterString("近地防卫系统结论", "You have unlocked Near Earth Def-system.", "你解锁了近地防卫系统");
            ProtoRegistry.RegisterString("深空防卫系统", "Void Def-system", "深空防卫系统");
            ProtoRegistry.RegisterString("深空防卫系统描述", "Manufacturing <color=#c2853d>void-missile-launching-silo</color> to deploy <color=#c2853d>thermonuclear-missile</color> to bulid broader strike, Filling the gap in near-earth-defense.", "制造<color=#c2853d>深空导弹发射井</color>部署<color=#c2853d>热核导弹</color>实现更大范围覆盖打击，填补近地防卫的空白。");
            ProtoRegistry.RegisterString("深空防卫系统结论", "You have unlocked Void Def-system.", "你解锁了深空防卫系统");
            ProtoRegistry.RegisterString("引力操控技术", "Gravitation control", "引力操控技术");
            ProtoRegistry.RegisterString("引力操控技术描述", "Manufacturing <color=#c2853d>gravitation-slingshot-ejector</color> to level up near-earth-defense and <color=#c2853d>gravitational-collapse-missile</color> to further strengthen void-defense.", "制造<color=#c2853d>引力弹射炮</color>升级近地防卫系统，生产<color=#c2853d>引力塌陷导弹</color>进一步加强深空防御。");
            ProtoRegistry.RegisterString("引力操控技术结论", "You have unlocked Gravitation control.", "你解锁了引力操控技术");
            ProtoRegistry.RegisterString("相位裂解技术", "Phaser fission", "相位裂解技术");
            ProtoRegistry.RegisterString("相位裂解技术描述", "Manufacturing super weapon <color=#c2853d>phaser-fission</color> to bulid ultimate near-earth-defense.", "这波<color=#c2853d>相位裂解炮</color>来全杀了。");
            ProtoRegistry.RegisterString("相位裂解技术结论", "You have unlocked Phaser fission.", "你解锁了相位裂解技术");

            ProtoRegistry.RegisterString("子弹2tech描述", "Manufacturing <color=#c2853d>Acid bullet</color> to strengthen defensive system.", "制造<color=#c2853d>强酸磁轨弹</color>加强近地防御力。");
            ProtoRegistry.RegisterString("子弹3tech描述", "Manufacturing <color=#c2853d>Deuterium nucleus</color> to strengthen defensive system.", "制造<color=#c2853d>氘核爆破弹</color>进一步加强近地防御力。");
            ProtoRegistry.RegisterString("导弹2tech描述", "Manufacturing <color=#c2853d>Antimatter missile</color> to strengthen defensive system.", "制造<color=#c2853d>反物质导弹</color>加强深空防御力。");

            ProtoRegistry.RegisterString("定向爆破1", "Directional blasting", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破2", "Directional blasting", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破3", "Directional blasting", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破4", "Directional blasting", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破5", "Directional blasting", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破6", "Directional blasting", "定向爆破");
            ProtoRegistry.RegisterString("定向爆破描述", "Increase damage for bullets and missiles.", "通过精确计算子弹和导弹的索敌路径，预测撞击前的最佳起爆点，以尽可能对敌人造成更大的破坏。");
            ProtoRegistry.RegisterString("定向爆破结论", "Increase damage for bullets and missiles.", "子弹、导弹伤害增加");
            ProtoRegistry.RegisterString("子弹伤害和导弹伤害+15%", "Damage of bullets and missiles +15%", "子弹伤害和导弹伤害+15%");
            ProtoRegistry.RegisterString("相位裂解光束伤害+30%", "Damage of Phaser-emitter beam +30%", "相位裂解光束伤害+30%");

            ProtoRegistry.RegisterString("引力波引导1", "Gravitational wave guidance", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导2", "Gravitational wave guidance", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导3", "Gravitational wave guidance", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导4", "Gravitational wave guidance", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导5", "Gravitational wave guidance", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导6", "Gravitational wave guidance", "引力波引导");
            ProtoRegistry.RegisterString("引力波引导描述", "Increase speed for bullets and missiles.", "利用引力波提升子弹的飞行速度，使子弹能够更快地打到敌人。");
            ProtoRegistry.RegisterString("引力波引导结论", "Increase speed for bullets and missiles.", "子弹、导弹弹道速度增加");
            ProtoRegistry.RegisterString("子弹飞行速度+10%", "Bullet speed +10%", "子弹飞行速度+10%");
            ProtoRegistry.RegisterString("导弹飞行速度+5%", "Missile speed +5%", "导弹飞行速度+5%");

            ProtoRegistry.RegisterString("相位干扰技术1", "Phase disturbance", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术2", "Phase disturbance", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术3", "Phase disturbance", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术4", "Phase disturbance", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术5", "Phase disturbance", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术6", "Phase disturbance", "相位干扰技术");
            ProtoRegistry.RegisterString("相位干扰技术描述", "Expand wormhole spawns radius.", "通过空间干扰让虫洞刷新的更远，增加敌人的飞行距离。");
            ProtoRegistry.RegisterString("相位干扰技术结论", "Expand wormhole spawns radius.", "虫洞生成距离增加");
            ProtoRegistry.RegisterString("虫洞生成最近范围扩大0.25AU", "Wormhole spawns radius expanded by 0.25AU", "虫洞生成最近范围扩大0.25AU");
           
            ProtoRegistry.RegisterString("彩蛋1", "Pioneer Diary #1", "先驱者日记#1");
            ProtoRegistry.RegisterString("彩蛋2", "Pioneer Diary #2", "先驱者日记#2");
            ProtoRegistry.RegisterString("彩蛋3", "Pioneer Diary #3", "先驱者日记#3");
            ProtoRegistry.RegisterString("彩蛋4", "Pioneer Diary #4", "先驱者日记#4");
            ProtoRegistry.RegisterString("彩蛋5", "Pioneer Diary #5", "先驱者日记#5");
            ProtoRegistry.RegisterString("彩蛋6", "Pioneer Diary #6", "先驱者日记#6");
            ProtoRegistry.RegisterString("彩蛋7", "Pioneer Diary #7", "先驱者日记#7");
            ProtoRegistry.RegisterString("彩蛋8", "Pioneer Diary #8", "先驱者日记#8");
            ProtoRegistry.RegisterString("彩蛋9", "Pioneer Diary #9", "先驱者日记#9");

            //ProtoRegistry.RegisterString("行星力场护盾", "Planet shield", "行星力场护盾");
            ProtoRegistry.RegisterString("星际要塞", "Star fortress", "星际要塞");
            ProtoRegistry.RegisterString("恒星炮gm2", "Star cannon", "恒星炮");
            ProtoRegistry.RegisterString("水滴gm2", "Droplet", "水滴"); 
            ProtoRegistry.RegisterString("即将到来gm", "Coming soon", "即将推出");

            ProtoRegistry.RegisterString("尼科尔戴森光束", "Nicoll-Dyson beam", "尼科尔-戴森光束");
            ProtoRegistry.RegisterString("尼科尔戴森光束描述", "Decoding a method for building a Star Cannon from alien matrices, disintegrating wormholes with stellar energy.", "从异星矩阵中解码建造恒星炮的方法，利用恒星级能量瓦解虫洞。");
            ProtoRegistry.RegisterString("尼科尔戴森光束结论", "You have unlocked the star cannon.", "你解锁了建造恒星炮的能力。");
            ProtoRegistry.RegisterString("行星力场护盾", "Planetary force field shield", "行星力场护盾");
            ProtoRegistry.RegisterString("行星力场护盾描述", "Maintain force field shields around planets.", "在行星周围构建力场护盾。");
            ProtoRegistry.RegisterString("行星力场护盾结论", "You have unlocked the planet shield generator.", "你解锁了行星护盾生成器。");
            ProtoRegistry.RegisterString("玻色子操控", "Boson control", "玻色子操控");
            ProtoRegistry.RegisterString("玻色子操控描述", "Create powerful materials by manipulating various interacting forces.", "通过操控各种相互作用力来制造强大的材料。");
            ProtoRegistry.RegisterString("玻色子操控结论", "You have unlocked the Boson control.", "你解锁了玻色子操控。");
            ProtoRegistry.RegisterString("超距信号处理1", "Stellar-range signal processing", "超距信号处理");
            ProtoRegistry.RegisterString("超距信号处理2", "Stellar-range signal processing", "超距信号处理");
            ProtoRegistry.RegisterString("超距信号处理3", "Stellar-range signal processing", "超距信号处理");
            ProtoRegistry.RegisterString("超距信号处理描述", "Enhance real-time control of droplets.", "强化对水滴的实时控制能力。");
            ProtoRegistry.RegisterString("超距信号处理结论", "Drop control limit is increased.", "可同时操控的水滴上限提升。");
            ProtoRegistry.RegisterString("水滴控制上限", "Droplet control limit", "水滴控制上限");

            ProtoRegistry.RegisterString("恒星炮未规划", "Star Cannon Unplanned", "恒星炮未规划");
            ProtoRegistry.RegisterString("恒星炮建设中", "Building In Progress", "恒星炮建设中");
            ProtoRegistry.RegisterString("恒星炮冷却中", "Cooling Down", "恒星炮冷却中");
            ProtoRegistry.RegisterString("恒星炮充能中", "Charging", "恒星炮充能中");
            ProtoRegistry.RegisterString("恒星炮开火", "Star Cannon Open Fire", "恒星炮开火");
            ProtoRegistry.RegisterString("瞄准中", "Aiming", "瞄准中");
            ProtoRegistry.RegisterString("预热中", "Preheating", "预热中");
            ProtoRegistry.RegisterString("正在开火", "Firing", "正在开火");
            ProtoRegistry.RegisterString("没有规划的恒星炮", "Firing", "没有规划的恒星炮");
            ProtoRegistry.RegisterString("恒星炮需要至少修建至第一阶段才能够开火！", "Firing", "恒星炮需要至少修建至第一阶段才能够开火！");
            ProtoRegistry.RegisterString("恒星炮已经启动", "Star cannon has already launched.", "恒星炮已经启动。");
            ProtoRegistry.RegisterString("恒星炮冷却中！", "Star cannon is cooling down.", "恒星炮冷却中！");
            ProtoRegistry.RegisterString("恒星炮充能中！", "Star cannon is charging.", "恒星炮充能中！");
            ProtoRegistry.RegisterString("没有目标！", "Can not find target.", "没有目标！");
            ProtoRegistry.RegisterString("超出射程！", "Out of range.", "超出射程！");
            ProtoRegistry.RegisterString("虫洞已完全稳定，无法被摧毁", "The wormholes are fully stabilized and cannot be destroyed.", "虫洞已完全稳定，无法被摧毁。");
            ProtoRegistry.RegisterString("恒星炮已启动", "Launching.", "恒星炮已启动。");
            

            ProtoRegistry.RegisterString("彩蛋1描述", "Seems like these enemies are a kind of space insect. They live in deep space, feed on the tides of power without vision. " +
                "The interstellar station creates regular ripples in space, making itself a beacon in a dark universe that can easily be captured and attacked by the insects. " +
                "In other words, <color=#c2853d>they will randomly choose an interstellar station, and attack its star system via wormholes. Stars with more interstellar stations will have a higher probability of being selected, and stars without any interstellar stations won't be attacked. </color>" +
                "Obviously, I can't give up my interstellar stations as progress will slow without item transportation. Instead, I have to find a way to defend. Hope I still have time to think about 'The Answer to Life, the Universe, and Everything'.", 
                "这种生物似乎是一种虫类，靠进食宇宙中的各种能量潮汐为生，同时还具有虫洞制造能力，一旦发现食物就会直接在附近建立虫洞。" +
                "星际物流塔产生的能量潮汐让它变成了宇宙中的一座灯塔，能被这些生物轻易捕捉并视作食物来源。" +
                "换言之，<color=#c2853d>它们会随机选择全星区的任意一个星际物流塔，通过虫洞进攻该星系；物流塔越多的星系受到攻击概率越高，而只要不建设星际物流塔，那个星系就不会受到它们的侵扰。</color>" +
                "但这显然行不通，不用星际物流塔将极大的延缓任务完成的速度，这是主脑不愿意看到的。看来我需要一些防御设施来保护星际物流塔。唉，本来还说抽空思考一下宇宙的终极答案的……");
            ProtoRegistry.RegisterString("彩蛋2描述", "Armour piercing rounds are still too inefficient, I have to find a better alternative. Anyway, they are real creatures, and acid erosion should do more damage to them.\n" +
                "Besides, I found that <color=#c2853d>the wormholes will always establish when the warning timer reaches 5 minutes?</color> If my railgun can't destroy the invading enemies, maybe I can do something before then...", 
                "穿甲弹的效率还是太低了，我得试试找到强力的替代品，最好是能产生更大的伤害。就目前的情况来看，硫酸是个不错的选择，我可以试着直接把他们投射向敌人。" +
                "再怎么说他们也是实体生物，酸蚀也够他们喝一壶的。还好它们不是异形，不然就是够我喝一壶的了。\n" +
                "另外我发现，<color=#c2853d>虫洞永远在预警五分钟时才生成？</color>" +
                "要是我的磁轨炮打不到入侵的敌人，也许我可以提前做点什么...");
            ProtoRegistry.RegisterString("彩蛋3描述", "They are more powerful than I thought. With the expansion of pipelines and interstellar stations, their offensive has become more and more ferocious. " +
                "The Near Earth Def-system had limited coverage and could no longer effectively resist them. Luckily I have learned to manufacture thermonuclear missiles on YouTube, and launching it will definitely solve this urgent need.\n" +
                "In addition, I found that <color=#c2853d>every time they destroy an interstellar station, the next wave will be delayed! </color>Maybe I can find a way to get more time for building?", 
                "我还是把他们想的太简单了。随着产线扩张，物流塔的建设，他们的攻势越来越凶猛。大炮的覆盖范围有限，已经无法有效抵挡他们了。好在我这几天在B站上学会了造热核弹，把它发射上去肯定能解燃眉之急。" +
                "至于怎么发射上去，那还真得研究研究了，炮管子实在是太细了，放不进去。赶快结束这一切吧，我还想去梦里数电子羊呢。\n" +
                "另外我发现，<color=#c2853d>虫子每破坏一座星际物流塔，下一次进攻就会被推迟！</color>也许可以想想办法获得更多的发展时间呢？");
            ProtoRegistry.RegisterString("彩蛋4描述", "The thermonuclear missiles are very powerful, but the launch is too slow. How can I create a miniature version and integrate with bullets, so they can both have lethality and speed? What a genius I am!\n" +
                "Another thing I found is, <color=#c2853d>the intensity of the attack on different stars is independent. The first attack on a new star will start from the lowest intensity...</color> It seems I don't need to worry about the safety of planets with only for miners now.", 
                "热核导弹的杀伤力确实很大，但是发射也太慢了，我得研究研究怎么把热核导弹造成微缩版本塞进炮弹里面去，那不就既有杀伤力又有速度嘛。我可真是个天才！" +
                "唉，你说我这天才为什么就不得伊卡拉姆妹妹的喜欢呢？我为她专门点亮了一片星系告白，但她居然说：“前天看到了小白兔，昨天是小鹿，今天是你。”这不是嘲讽我像个动物一样蠢吗。算了算了，不提了，女人只会影响我造戴森球的速度。\n" +
                "另外我发现，<color=#c2853d>这些虫子攻击不同星系的强度是独立的啊，进攻全新星系的虫群似乎会重新从最低强度开始...</color> 看来暂时是不怎么需要担心矿星的安全性问题了。");
            ProtoRegistry.RegisterString("彩蛋5描述", "Damn it, a bit of antimatter leakage almost destroyed my pipelines, which took me a lot of time to repair. " +
                "Wait, they can destroy my pipelines, why not insects? Good idea! Wait for me, fxxk insects, taste the antimatter and die!", 
                "倒霉透了，手滑泄露的那一点反物质差点把产线给扬了，害得我修复了好久。诶等等，能扬了我的产线为什么不能扬了那堆臭虫？思路打开了！" +
                "死虫子你们给我等着，跟我轻型机甲拼你们有这个实力吗？我这就把反物质打上天，指定没有你们好果子吃！");
            ProtoRegistry.RegisterString("彩蛋6描述", "Jesus! Why did I limit myself to existing technologies? I'm Icarus, who can travel through black holes, make a hard landing with ultra-high speed, take a shower in lava and swim in a sulfuric acid lake. " +
                "Why can't I develop some outrageous weapons? Damn insects, can't be avoided and keep slowing down my progress, let me teach you a lesson! Come on!", 
                "真是的，我为什么要局限于现有的武器技术啊。我可是伊卡洛斯啊，能穿越黑洞，能超高速硬着陆，能去岩浆里泡澡去硫酸里游泳，我为什么不能大胆开发点离谱的武器？" +
                "反正也是闲着，不如给虫子看看我的真本事！TNND，杀又杀不完，躲也躲不掉，还一直拖慢我的进度，跟我玩阴的是吧？直接来吧！");
            ProtoRegistry.RegisterString("彩蛋7描述", "Sure enough, nothing will stop me if I really want something. I was too limited before, as I can build a Dyson sphere, why does the cannon need bullets? Just Phaser fission! " +
                "Everything is good now, I don't need to be distracted by making bullets, so nice!\n" +
                "By the way, I found <color=#c2853d>the intensity of attack has an upper limit. I really thought it would increase infinitely...</color> Now it's so easy for me to defend them! " +
                "I don't know if uploading universe matrix or building Dyson sphere will change anything, let me find out.", 
                "果然只要我出手没什么办不到的，之前还是太局限了，戴森球都能造出来了为什么武器还需要弹药？直接相位裂解就完事了，要不是降维技术不可逆我都有心想丢二向箔过去。这下好啦，不用专门分心去造子弹了，我要继续去当第四天灾了。" +
                "顺便说一句，这些虫子可比索林原虫差远了，虽然索林原虫也是渣渣~~日记。\n" +
                "另外我发现，<color=#c2853d>强度攻击原来是有上限的啊，我还真以为是无限提升呢...</color> 现在已经游刃有余了嘿嘿... 不知道给主脑上传宇宙矩阵或者建成戴森球后会不会有什么额外影响，等到那一步再看吧。");
            ProtoRegistry.RegisterString("彩蛋8描述", "Young Icarus, no matter how you came here, you have proved your smarts and strength. Even facing unknown risks, you still light up the stars. " +
                "Now you can proudly say, \"I've seen things that you absolutely can't believe, I've seen wormholes established in galaxies, I've seen fission rays flickering in swarms. All those moments will be lost in time, like tears in rain.\" " +
                "Continue your journey now! No matter what difficulties you encounter, don't be afraid and face them with a smile! Remember, you are the mighty Icarus!",
                "年轻的伊卡洛斯，不管你是用什么方法走到了这一步，都足以证明你的聪颖和强大。即使是面对未知的风险，你依旧为主脑点亮了繁星。" +
                "现在，你可以骄傲的说：“我见过你们绝对无法置信的事物，我目睹了虫洞在星系内诞生，我看着裂解射线在虫群之中闪烁，所有这些时刻，终将随时间消逝，一如眼泪消失在雨中。”" +
                "现在继续你的征途吧！无论遇到什么困难，都不要怕，微笑着面对他！因为，你是一个一个一个勇敢的伊卡洛斯哼哼，啊啊啊啊啊啊啊啊啊啊啊！");
            ProtoRegistry.RegisterString("彩蛋9描述", "<color=#c2853d>42</color>", "<color=#c2853d>42</color>");


            ProtoRegistry.RegisterString("UI快捷键提示", "Press Backspace to hide/open this window. Press \"Ctrl\" + \"-\" to advance the attack time by 1 min.", "按下退格键开启或关闭此窗口，按下Ctrl+减号键使敌军进攻时间提前1分钟");

            ProtoRegistry.RegisterString("简单难度提示", "Difficulty: Easy (Station won't be destroyed; Merit points earned *0.75)", "当前难度：简单（物流塔不会被破坏；功勋点数获得*0.75）");
            ProtoRegistry.RegisterString("普通难度提示", "Difficulty: Normal (Station attacked will turn to blueprint mode)", "当前难度：普通（物流塔被破坏会进入蓝图模式）");
            ProtoRegistry.RegisterString("困难难度提示", "Difficulty: Hard (Station will be dismantled; Enemy strength will increase; Merit points earned *1.5)", "当前难度：困难（物流塔会被破坏拆除，敌人战斗力大幅提升；功勋点数获得*1.5）");
            ProtoRegistry.RegisterString("奖励倒计时：", "Reward time left: ", "奖励剩余时间：");

            ProtoRegistry.RegisterString("mod版本信息", "Current version: " + Configs.versionString, "当前版本：" + Configs.versionString + "          欢迎加入mod交流群：" + Configs.qq);
            ProtoRegistry.RegisterString("未探测到威胁", "No threat detected", "未探测到威胁");
            ProtoRegistry.RegisterString("预估数量", "Estimated quantity", "预估数量");
            ProtoRegistry.RegisterString("预估强度", "Estimated strength", "预估强度");
            ProtoRegistry.RegisterString("虫洞数量", "Wormhole quantity", "虫洞数量");
            ProtoRegistry.RegisterString("敌人正在入侵", "The enemies are invading ", "敌人正在入侵");
            ProtoRegistry.RegisterString("剩余敌人", "Remaining enemies", "剩余敌人");
            ProtoRegistry.RegisterString("剩余强度", "Remaining strength", "剩余强度");
            ProtoRegistry.RegisterString("已被摧毁", "Eliminated enemies", "已被摧毁");
            ProtoRegistry.RegisterString("入侵抵达提示", "The next wave will arrive in {0} on {1}", "下一次入侵预计于{0}后抵达{1}");
            ProtoRegistry.RegisterString("约gm", "", "约");
            ProtoRegistry.RegisterString("小时gm", "h", "小时");
            ProtoRegistry.RegisterString("分gm", "m", "分");
            ProtoRegistry.RegisterString("秒gm", "s", "秒");

            ProtoRegistry.RegisterString("伤害", "Damage", "伤害");
            ProtoRegistry.RegisterString("弹道速度", "Speed", "弹道速度");
            ProtoRegistry.RegisterString("伤害半径", "Damage range", "伤害半径");
            ProtoRegistry.RegisterString("射速", "Fire rate", "射速");
            ProtoRegistry.RegisterString("子弹伤害", "Bullet damage", "子弹伤害");
            ProtoRegistry.RegisterString("导弹伤害", "Missile damage", "导弹伤害");
            ProtoRegistry.RegisterString("相位裂解光束伤害", "Phase-cracking beam damage", "相位裂解光束伤害");
            ProtoRegistry.RegisterString("子弹相位伤害", "Bullet / Beam damage", "子弹/相位光束伤害");
            ProtoRegistry.RegisterString("子弹速度", "Bullet speed", "子弹速度");
            ProtoRegistry.RegisterString("导弹速度", "Missile speed", "导弹速度");
            ProtoRegistry.RegisterString("子弹飞行速度", "Bullet speed", "子弹飞行速度");
            ProtoRegistry.RegisterString("导弹飞行速度", "Missile speed", "导弹飞行速度");
            ProtoRegistry.RegisterString("子弹导弹速度", "Bullet / Missile speed", "子弹/导弹速度");
            ProtoRegistry.RegisterString("虫洞干扰半径", "Wormhole interference radius", "虫洞干扰半径");
            ProtoRegistry.RegisterString("效率gm", "Efficiency", "弹药效率");
            ProtoRegistry.RegisterString("额外奖励gm", "★bonus ", "★奖励 ");

            ProtoRegistry.RegisterString("设定索敌最高优先级", "Set priority to eject", "设定索敌最高优先级");
            ProtoRegistry.RegisterString("最接近物流塔", "Nearest to station", "最接近物流塔");
            ProtoRegistry.RegisterString("最大威胁", "Highest threat", "最大威胁");
            ProtoRegistry.RegisterString("距自己最近", "Nearest to self", "距自己最近");
            ProtoRegistry.RegisterString("最低生命", "Lowest HP", "最低生命");
            ProtoRegistry.RegisterString("目标生命值", "Target HP", "目标生命值");
            ProtoRegistry.RegisterString("无攻击目标", "No target", "无攻击目标");
            ProtoRegistry.RegisterString("开火中gm", "Firing", "开火中");
            ProtoRegistry.RegisterString("下一波攻击即将到来！", "Next wave is coming!", "下一波攻击即将到来！");
            ProtoRegistry.RegisterString("做好防御提示", "Please prepare next wave in <color=#c2853d>{0}</color>!", "请为<color=#c2853d>{0}</color>做好防御准备。");
            ProtoRegistry.RegisterString("虫洞已生成！", "Wormhole generated!", "虫洞已生成！");
            ProtoRegistry.RegisterString("虫洞生成提示", "Use starmap or fly to <color=#c2853d>{0}</color> to view details.", "可通过星图或飞往<color=#c2853d>{0}</color>查看具体信息。");
            ProtoRegistry.RegisterString("战斗已结束！", "Wave ended!", "战斗已结束！");
            ProtoRegistry.RegisterString("战斗时间", "Battle duration", "战斗时间");
            ProtoRegistry.RegisterString("歼灭敌人", "Enemy eliminated", "歼灭敌人");
            ProtoRegistry.RegisterString("输出伤害", "Total damage", "输出伤害");
            ProtoRegistry.RegisterString("损失物流塔", "Station lost", "损失物流塔");
            ProtoRegistry.RegisterString("损失其他建筑", "Other buildings lost", "损失其他建筑");
            ProtoRegistry.RegisterString("损失资源", "Resource lost", "损失资源");
            ProtoRegistry.RegisterString("奖励提示0", "Got reward: mining speed * 2, tech speed * 2, vessel ship speed * 1.5, lasting for {0} seconds.", "获得奖励：采矿速率*2，研究速率*2，运输船速度*1.5，持续 {0} 秒。");
            ProtoRegistry.RegisterString("奖励提示3", "Got reward: ore consumption -20%, mining speed * 2, tech speed * 2, vessel ship speed * 1.5, lasting for {0} seconds.", "获得奖励：采矿消耗-20%，采矿速率*2，研究速率*2，运输船速度*1.5，持续 {0} 秒。");
            ProtoRegistry.RegisterString("奖励提示5", "Got reward: ore consumption -20%, mining speed * 2, tech speed * 2, vessel ship speed * 1.5, proliferator's efficiency has been improved, lasting for {0} seconds.", "获得奖励：采矿消耗-20%，采矿速率*2，研究速率*2，运输船速度*1.5，增产剂效能全面提升，持续 {0} 秒。");
            ProtoRegistry.RegisterString("奖励提示7", "Got reward: ore consumption -50%, mining speed * 2, tech speed * 2, vessel ship speed * 1.5, proliferator's efficiency has been improved, lasting for {0} seconds.", "获得奖励：采矿消耗-50%，采矿速率*2，研究速率*2，运输船速度*1.5，增产剂效能全面提升，持续 {0} 秒。");

            ProtoRegistry.RegisterString("查看更多战斗信息", "View more details of this wave in Statistics -> Battle Info", "在分析面板-战斗统计中，可以查看更为详细的战斗信息。");
            ProtoRegistry.RegisterString("火箭模式提示", "Current Mode: AUTO", "自动寻敌（无需设置）");
            ProtoRegistry.RegisterString("打开统计面板", "Open Statistics", "打开统计面板");

            ProtoRegistry.RegisterString("战斗简报", "Battle Info", "战斗简报");
            ProtoRegistry.RegisterString("战况概览", "Summary", "战况概览");
            ProtoRegistry.RegisterString("弹药信息", "Bullets", "弹药信息");
            ProtoRegistry.RegisterString("敌方信息", "Enemies", "敌方信息");
            ProtoRegistry.RegisterString("简单", "Easy", "简单");
            ProtoRegistry.RegisterString("普通", "Normal", "普通");
            ProtoRegistry.RegisterString("困难", "Hard", "困难");
            ProtoRegistry.RegisterString("调整难度提示", "Change difficulty to: (Only ONCE)", "调整难度为：（只可调整一次）");
            ProtoRegistry.RegisterString("调整难度标题", "Confirm to change difficulty?", "你确定想调整难度么？");
            ProtoRegistry.RegisterString("调整难度警告", "Do you want to change difficulty to <color=#c2853d>{0}</color>? This can only be done ONCE!", "你确定想调整难度为<color=#c2853d>{0}</color>吗？难度只能被调整一次！");
            ProtoRegistry.RegisterString("设置成功！", "Success!", "设置成功！");
            ProtoRegistry.RegisterString("难度设置成功", "Successfully change difficulty to <color=#c2853d>{0}</color>!", "成功设置难度为<color=#c2853d>{0}</color>！");
            ProtoRegistry.RegisterString("平均拦截距离", "Avg Intercept Distance", "平均拦截距离");
            ProtoRegistry.RegisterString("最小拦截距离", "Min Intercept Distance", "最小拦截距离");
            ProtoRegistry.RegisterString("数量总计", "Total Quantity", "数量总计");
            ProtoRegistry.RegisterString("伤害总计", "Total Damage", "伤害总计");
            ProtoRegistry.RegisterString("子弹数量", "Bullets Quantity", "子弹数量");
            ProtoRegistry.RegisterString("导弹数量", "Missiles Quantity", "导弹数量");
            ProtoRegistry.RegisterString("子弹伤害gm", "Bullets Damage", "子弹伤害");
            ProtoRegistry.RegisterString("导弹伤害gm", "Missiles Damage", "导弹伤害");
            ProtoRegistry.RegisterString("击中gm", "Hit", "击中");
            ProtoRegistry.RegisterString("发射gm", "Ejected", "发射");
            ProtoRegistry.RegisterString("总计gm", "Total", "总计");
            ProtoRegistry.RegisterString("侦查艇", "Corvette", "侦查艇");
            ProtoRegistry.RegisterString("护卫舰", "Frigate", "护卫舰");
            ProtoRegistry.RegisterString("驱逐舰", "Destroyer", "驱逐舰");
            ProtoRegistry.RegisterString("巡洋舰", "Cruiser", "巡洋舰");
            ProtoRegistry.RegisterString("重型巡洋舰", "B-Cruiser", "重型巡洋舰");
            ProtoRegistry.RegisterString("战列舰", "Battleship", "战列舰");
            ProtoRegistry.RegisterString("已歼灭gm", "Eliminated", "已歼灭");
            ProtoRegistry.RegisterString("已产生gm", "Total", "已产生");
            ProtoRegistry.RegisterString("占比gm", "Percentage", "占比");
            ProtoRegistry.RegisterString("游戏提示gm", "Message", "游戏提示");

            ProtoRegistry.RegisterString("gmRank0", "<color=#ffffff80>Icarus</color>", "<color=#ffffff80>伊卡洛斯</color>");
            ProtoRegistry.RegisterString("gmRank1", "<color=#61d8ffb4>Explorer I</color>", "<color=#61d8ffb4>探索者 I</color>");
            ProtoRegistry.RegisterString("gmRank2", "<color=#61d8ffb4>Explorer II</color>", "<color=#61d8ffb4>探索者 II</color>");
            ProtoRegistry.RegisterString("gmRank3", "<color=#61d8ffb4>Explorer III</color>", "<color=#61d8ffb4>探索者 III</color>");
            ProtoRegistry.RegisterString("gmRank4", "<color=#d238ffb4>Pioneer I</color>", "<color=#d238ffb4>开拓者 I</color>");
            ProtoRegistry.RegisterString("gmRank5", "<color=#d238ffb4>Pioneer II</color>", "<color=#d238ffb4>开拓者 II</color>");
            ProtoRegistry.RegisterString("gmRank6", "<color=#d238ffb4>Pioneer III</color>", "<color=#d238ffb4>开拓者 III</color>");
            ProtoRegistry.RegisterString("gmRank7", "<color=#fd9620c0>Conqueror I</color>", "<color=#fd9620c0>征服者 I</color>");
            ProtoRegistry.RegisterString("gmRank8", "<color=#fd9620c0>Conqueror II</color>", "<color=#fd9620c0>征服者 II</color>");
            ProtoRegistry.RegisterString("gmRank9", "<color=#fd9620c0>Conqueror III</color>", "<color=#fd9620c0>征服者 III</color>");
            ProtoRegistry.RegisterString("gmRank10", "<color=#ffc620c8>Galaxy Guardian</color>", "<color=#ffc620da>星河卫士</color>");
            ProtoRegistry.RegisterString("gmRankNoColor0", "Icarus", "伊卡洛斯");
            ProtoRegistry.RegisterString("gmRankNoColor1", "Explorer I", "探索者 I");
            ProtoRegistry.RegisterString("gmRankNoColor2", "Explorer II", "探索者 II");
            ProtoRegistry.RegisterString("gmRankNoColor3", "Explorer III", "探索者 III");
            ProtoRegistry.RegisterString("gmRankNoColor4", "Pioneer I", "开拓者 I");
            ProtoRegistry.RegisterString("gmRankNoColor5", "Pioneer II", "开拓者 II");
            ProtoRegistry.RegisterString("gmRankNoColor6", "Pioneer III", "开拓者 III");
            ProtoRegistry.RegisterString("gmRankNoColor7", "Conqueror I", "征服者 I");
            ProtoRegistry.RegisterString("gmRankNoColor8", "Conqueror II", "征服者 II");
            ProtoRegistry.RegisterString("gmRankNoColor9", "Conqueror III", "征服者 III");
            ProtoRegistry.RegisterString("gmRankNoColor10", "Galaxy Guardian", "星河卫士");
            ProtoRegistry.RegisterString("gmRankUnlockText0", "", "");
            ProtoRegistry.RegisterString("gmRankUnlockText1", "Wave reward: mining speed * 2, tech speed * 2, vessel ship speed * 1.5. Lasts up to 5 minutes", "战斗结束后给予采矿速率*2，研究速率*2，运输船速度*1.5的奖励，持续5分钟");
            ProtoRegistry.RegisterString("gmRankUnlockText2", "Wave reward duration +20%", "战斗奖励持续时间增加20%");
            ProtoRegistry.RegisterString("gmRankUnlockText3", "New wave reward: mining consumption -20%", "战斗结束后额外获得采矿消耗-20%的奖励");
            ProtoRegistry.RegisterString("gmRankUnlockText4", "Wave reward duration +20%", "战斗奖励持续时间增加20%");
            ProtoRegistry.RegisterString("gmRankUnlockText5", "New wave reward: proliferator's efficiency has been improved", "战斗结束后额外获得增产剂效果全面加强的奖励");
            ProtoRegistry.RegisterString("gmRankUnlockText6", "Wave reward duration +20%", "战斗奖励持续时间增加20%");
            ProtoRegistry.RegisterString("gmRankUnlockText7", "Wave reward: mining consumption -20% --> -50%", "采矿消耗降低的奖励强化为-50%消耗");
            ProtoRegistry.RegisterString("gmRankUnlockText8", "Wave reward duration +20%", "战斗奖励持续时间增加20%");
            ProtoRegistry.RegisterString("gmRankUnlockText9", "Star cannon charging speed +100%", "恒星炮充能速度增加100%");
            ProtoRegistry.RegisterString("gmRankUnlockText10", "Droplet damage greatly increased. Wave reward duration +20%", "水滴伤害获得大幅度加强，战斗结束后的奖励持续时间增加20%");
            ProtoRegistry.RegisterString("gmRankReward1", "Wave reward unlocked", "战斗结束后给予战斗奖励");
            ProtoRegistry.RegisterString("gmRankReward7", "Additional wave reward: mining consumption -50%", "额外的战斗奖励：采矿消耗-50%");
            ProtoRegistry.RegisterString("gmRankReward3", "Additional wave reward: mining consumption -20%", "额外的战斗奖励：采矿消耗-20%");
            ProtoRegistry.RegisterString("gmRankReward5", "Additional wave reward: proliferator enhancement", "额外的战斗奖励：增产剂效果强化");
            ProtoRegistry.RegisterString("gmRankReward2", "Wave reward duration +", "战斗奖励持续时间+");
            ProtoRegistry.RegisterString("gmRankReward9", "Star cannon charging speed +100%", "恒星炮充能速度+100%");
            ProtoRegistry.RegisterString("gmRankReward10", "Droplet damage +400%", "水滴伤害+400%");
            ProtoRegistry.RegisterString("功勋阶级", "Merit Rank", "功勋阶级");
            ProtoRegistry.RegisterString("当前阶级", "Current Rank", "当前等级");
            ProtoRegistry.RegisterString("功勋点数", "Merit points", "功勋点数");
            ProtoRegistry.RegisterString("已解锁gm", "Unlocked", "已解锁");
            ProtoRegistry.RegisterString("下一功勋等级解锁", "Next rank unlocked", "下一功勋等级解锁");

            ProtoRegistry.RegisterString("行星护盾生成器", "Planet shield generator", "行星护盾生成器");
            ProtoRegistry.RegisterString("行星护盾生成器描述", "Using a large amount of energy to maintain a force field shield on the planet's surface, the encoding of the force field's resonant frequency allows allies to easily pass through the shield, while blocking the enemies. Multiple shield generators can speed up the shield recharge rate, and provide additional shield capacity. However, as the number of shield generators increases, each additional generator will provide less and less additional capacity.", 
                "使用大量能量在行星表面维持一个力场护盾，对力场谐振频率的编码能够使友方轻易穿过护盾，同时阻挡敌人的进入或攻击。多个护盾生成器能够加快护盾充能的速度，也能够提供额外的护盾容量上限。不过随着单个星球上护盾生成器数量的增加，每个生成器能够提供的额外护盾也将越来越少。");
            ProtoRegistry.RegisterString("力场护盾", "Planet shield", "力场护盾");
            ProtoRegistry.RegisterString("力场护盾短", "Shield", "力场护盾");
            ProtoRegistry.RegisterString("护盾容量", "Shield capacity", "护盾容量");
            ProtoRegistry.RegisterString("护盾容量短", "Max shield", "护盾容量");
            ProtoRegistry.RegisterString("当前护盾", "Current shield", "当前护盾");
            ProtoRegistry.RegisterString("护盾恢复", "Recharge speed", "护盾恢复");
            ProtoRegistry.RegisterString("护盾生成器总数", "Generator amount", "护盾生成器总数");
            ProtoRegistry.RegisterString("完全充能时间", "Fully recharged in", "完全充能时间");
            ProtoRegistry.RegisterString("充能gm", "Charged", "已充能");
            ProtoRegistry.RegisterString("关闭gm", "Shut down", "关闭");
            ProtoRegistry.RegisterString("启动gm", "Activate", "启动");
            ProtoRegistry.RegisterString("护盾生成器待机提示", "The Shield Generator will stop consuming energy, and will no longer provide shield capacity or recharge shields.", "护盾生成器将停止消耗能量，并不再提供最大护盾容量，也无法为护盾充能。");
            ProtoRegistry.RegisterString("护盾生成器启动提示", "Shield generators will provide shield capacity, and speed up shield recharging.", "护盾生成器将提供护盾容量，并加快护盾充能速度。");
            ProtoRegistry.RegisterString("耗电需求gm", "Consumption demand", "耗电需求");
            ProtoRegistry.RegisterString("耗电需求短gm", "Consumption", "耗电需求");
            ProtoRegistry.RegisterString("发电性能短gm", "Generation", "发电性能");

            ProtoRegistry.RegisterString("护盾承受伤害", "Shield damage taken", "护盾承受伤害");
            ProtoRegistry.RegisterString("护盾造成伤害", "Shield damage dealed", "护盾造成伤害");
            ProtoRegistry.RegisterString("水滴伤害", "Droplet damage", "水滴伤害");
            ProtoRegistry.RegisterString("最小发射能量", "Launch Energy Threshold", "发射能量阈值");
            ProtoRegistry.RegisterString("水滴发射耗能", "Launch Consumption", "发射耗能");
            ProtoRegistry.RegisterString("水滴工作功率", "Work Consumption", "工作功率");

            ProtoRegistry.RegisterString("异星矩阵", "Alien matrix", "异星矩阵");
            ProtoRegistry.RegisterString("异星矩阵描述", "A matrix containing high-density data accidentally dropped by invading swarms. Can be analyzed by mechs and used to unlock more advanced alien technologies. The matrix itself also seems to have potentially high-dimensional spatiotemporal properties", "由入侵的虫群偶然掉落的载有高密度数据的矩阵，可以由机甲分析并用于解锁更高级的异星科技。矩阵本身似乎还具有潜在的高维时空特性。");
            ProtoRegistry.RegisterString("异星元数据", "Alien metadata", "异星元数据");
            ProtoRegistry.RegisterString("异星元数据描述", "Having fully decoded the Alien Matrix, Icarus can now quickly decompile the Alien Matrix and obtain the alien metadata, which does not require as much computation as initially decoding the megastructure data in the Alien Matrix. The decoded alien metadata in mech will be automatically uploaded to the CenterBrain and shared with other pioneers in the sector, which will provide Icarus with additional <color=#c2853d>merit points</color>. But this metadata cannot be shared across archives like other metadata.",
                "在完成了对异星矩阵的全面解码后，伊卡洛斯现在可以快速对异星矩阵进行反编译并获得异星元数据，这不需要像最初解码异星矩阵中的巨构数据那样消耗大量算力。机甲中的异星元数据将自动上传给主脑并共享给星区的其他开拓者，这同时也会为伊卡洛斯提供大量的<color=#c2853d>功勋点数</color>。但该元数据无法像其他元数据一样在存档间共享。");
            ProtoRegistry.RegisterString("异星矩阵反编译", "Alien matrix decompile", "异星矩阵反编译");
            ProtoRegistry.RegisterString("异星矩阵反编译 x10", "Alien matrix decompile x10", "异星矩阵反编译 x10"); 
            ProtoRegistry.RegisterString("异星矩阵反编译 x100", "Alien matrix decompile x100", "异星矩阵反编译 x100");
            ProtoRegistry.RegisterString("量子增产剂", "Quantum proliferator", "量子增产剂");
            ProtoRegistry.RegisterString("量子增产剂描述", "Research has shown that matter with high-dimensional spatiotemporal properties can be used to produce more effective proliferators, but such matter does not seem to be directly accessible from the original universe.", "研究表明具有高维时空特性的物质可被用于生产更强效果的增产材料，但这类物质似乎无法从本源宇宙中直接获取。");
            ProtoRegistry.RegisterString("量子增产剂科技描述", "Exploring how to make more efficient proliferators.", "对制造更高效增产剂进行探索。");
            ProtoRegistry.RegisterString("量子增产剂科技结论", "You have unlocked quatum proliferator.", "你解锁了制作量子增产剂的技术。");
            ProtoRegistry.RegisterString("掉落的异星矩阵", "Alien matrices dropped by enemies", "敌舰掉落的异星矩阵");
            ProtoRegistry.RegisterString("异星矩阵自动转换提示", "The alien matrices dropped by enemies have been automatically decompiled into alien metadata", "敌舰掉落的异星矩阵已自动反编译为异星元数据");
            
            ProtoRegistry.RegisterString("物质解压器科技描述", "Decoding a method from the alien matrices to build a Matter Decompressor.", "从异星矩阵中解码建造物质解压器的方法。");
            ProtoRegistry.RegisterString("科学枢纽科技描述", "Decoding a method from the alien matrices to build a Science Nexus.", "从异星矩阵中解码建造科学枢纽的方法。");
            ProtoRegistry.RegisterString("折跃场广播阵列科技描述", "Decoding a method from the alien matrices to build a Warp Field Broadcast Array.", "从异星矩阵中解码建造折跃场广播阵列的方法。");
            ProtoRegistry.RegisterString("星际组装厂科技描述", "Decoding a method from the alien matrices to build an Interstellar Assembly.", "从异星矩阵中解码建造星际组装厂的方法。");
            ProtoRegistry.RegisterString("晶体重构器科技描述", "Decoding a method from the alien matrices to build a Crystal Reconstructor.", "从异星矩阵中解码建造晶体重构器的方法。");
            ProtoRegistry.RegisterString("物质解压器科技结论", "You have successfully decoded the blueprint of Matter decompressor carrier rocket.", "你成功解码了物质解压器运载火箭的制造蓝图。");
            ProtoRegistry.RegisterString("科学枢纽科技结论", "You have successfully decoded the blueprint of Science nexus carrier rocket.", "你成功解码了科学枢纽运载火箭的制造蓝图。");
            ProtoRegistry.RegisterString("折跃场广播阵列科技结论", "You have successfully decoded the blueprint of Resonant generator carrier rocket.", "你成功解码了谐振发射器运载火箭的制造蓝图。");
            ProtoRegistry.RegisterString("星际组装厂科技结论", "You have successfully decoded the blueprint of Interstellar assembly component and its carrier rocket.", "你成功解码了星际组装厂组件和运载火箭的制造蓝图。");
            ProtoRegistry.RegisterString("晶体重构器科技结论", "You have successfully decoded the blueprint of Crystal reconstructor carrier rocket.", "你成功解码了晶体重构器运载火箭的制造蓝图。");

            ProtoRegistry.RegisterString("被深空来敌mod禁止", "Banned by mod They Come From Void", "被深空来敌mod禁止");
            
            // 遗物
            ProtoRegistry.RegisterString("发现异星圣物", "Alien Relic Found", "发现异星圣物");
            ProtoRegistry.RegisterString("解译异星圣物提示", "English Translation Needed.", "从以下三个解码轨中选取一个进行解译以获取对应的圣物加成。\n可以使用异星矩阵重随解码轨来发现新的可用效果，每次重新随机会使圣物更加不稳定，从而使下次重新随机的消耗翻倍。");
            ProtoRegistry.RegisterString("重新随机", "Roll", "重新随机");
            ProtoRegistry.RegisterString("免费", "free", "免费");
            ProtoRegistry.RegisterString("放弃解译", "Abort Study         +", "放弃解译         +");
            ProtoRegistry.RegisterString("删除遗物名称", "Remove Relic", "移除圣物");
            ProtoRegistry.RegisterString("删除遗物描述", "English Translation Needed", "随机移除一个已拥有的[普通]稀有度的圣物，并返还该圣物所占用的圣物槽位\n如果没有[普通]圣物，则随机移除一个已拥有的[稀有]圣物");
            ProtoRegistry.RegisterString("删除遗物确认标题", "Confirm Remove Relic", "确认移除圣物");
            ProtoRegistry.RegisterString("删除遗物确认警告", "English Translation Needed", "这将随机移除一个你已拥有的<color=#30b530>[普通]</color>稀有度的圣物，并返还该圣物所占用的圣物槽位\n如果你没有任何<color=#30b530>[普通]</color>圣物，则随机移除一个你已拥有的<color=#2080d0>[稀有]</color>圣物！");
            ProtoRegistry.RegisterString("成功移除！", "Relic removed", "成功移除圣物");
            ProtoRegistry.RegisterString("已移除遗物描述", "You've removed relic ", "你已移除");
            ProtoRegistry.RegisterString("未能移除！", "No relic can be removed", "没有可移除的圣物");
            ProtoRegistry.RegisterString("未能移除遗物描述", "No matched relic can be removed", "你没有稀有度匹配的圣物可供移除");

            ProtoRegistry.RegisterString("遗物名称0-0", "", "吞噬者\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-1", "", "蓝buff\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-2", "", "女神之泪\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-3", "", "坚固透镜\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-4", "", "黑百合\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-5", "", "虚空荆棘\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-6", "", "京级巨炮\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-7", "", "撕裂力场\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-8", "", "饮血剑\n<size=18>- 传说 -</size>");
            ProtoRegistry.RegisterString("遗物名称0-9", "", "五叶草\n<size=18>- 传说 -</size>");

            ProtoRegistry.RegisterString("遗物名称1-0", "", "凯旋\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-1", "", "虚空爆发\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-2", "", "能量涌动\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-3", "", "敌对海域\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-4", "", "三体\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-5", "", "回声 II\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-6", "", "能量迸发\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-7", "", "活性炭 II\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-8", "", "女妖面纱\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-9", "", "骑士之誓\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-10", "", "真实伤害\n<size=18>- 史诗 -</size>");
            ProtoRegistry.RegisterString("遗物名称1-11", "", "冰封陵墓\n<size=18>- 史诗 -</size>");

            ProtoRegistry.RegisterString("遗物名称2-0", "", "超充能器\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-1", "", "互惠互利\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-2", "", "极限一换一\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-3", "", "回声 I\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-4", "", "听说有人缺氢\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-5", "", "多动症 II\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-6", "", "高效索敌\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-7", "", "仇敌\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-8", "", "狄拉克辶辶变\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-9", "", "聚能环 II\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-10", "", "矩阵充能\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-11", "", "副产物提炼\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-12", "", "强攻\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-13", "", "无尽之刃\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-14", "", "行窃预兆\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-15", "", "引力碎裂\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-16", "", "刚毅护盾\n<size=18>- 稀有 -</size>");
            ProtoRegistry.RegisterString("遗物名称2-17", "", "不朽之守护\n<size=18>- 稀有 -</size>");

            ProtoRegistry.RegisterString("遗物名称3-0", "", "劣质加工\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-1", "", "窃法之刃\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-2", "", "方舟反应堆\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-3", "", "掘墓人\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-4", "", "装\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-5", "", "复活币\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-6", "", "光刻机\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-7", "", "虚空折射\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-8", "", "矩阵雨\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-9", "", "开摆\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-10", "", "多动症 I\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-11", "", "活性炭 I\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-12", "", "灵动巨物\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-13", "", "聚能环 I\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-14", "", "阳间马达\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-15", "", "超级大脑\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-16", "", "虚空棱镜\n<size=18>- 普通 -</size>");
            ProtoRegistry.RegisterString("遗物名称3-17", "", "荣誉晋升\n<size=18>- 普通 -</size>");

            ProtoRegistry.RegisterString("遗物描述0-0", "", "每次击毁敌舰，根据敌舰强度有概率略微推进巨构的建造进度");
            ProtoRegistry.RegisterString("遗物描述0-1", "", "制造厂在制造原材料至少2种的配方时，每产出1个产物，会返还1个第1位置的原材料");
            ProtoRegistry.RegisterString("遗物描述0-2", "", "化工厂在生产原材料至少2种的配方时，返还第1位置的全部原材料");
            ProtoRegistry.RegisterString("遗物描述0-3", "", "射线接受器无需消耗透镜即可达到最大输出效率");
            ProtoRegistry.RegisterString("遗物描述0-4", "", "射线接受器不再受任何星球阻挡");
            ProtoRegistry.RegisterString("遗物描述0-5", "", "行星力场护盾会向伤害来源反弹10%受到的基础伤害作为<i>额外伤害</i>");
            ProtoRegistry.RegisterString("遗物描述0-6", "", "穿甲磁轨弹造成50000%<i>额外伤害</i>，弹道速度大幅增加，但每次消耗5发弹药，且充能与冷却时间增加900%");
            ProtoRegistry.RegisterString("遗物描述0-7", "", "拥有巨构的星系在战斗时每秒会对星系中所有敌舰造成<i>额外伤害</i>，伤害取决于巨构的能量水平\n如果巨构为恒星炮，该伤害增加200%");
            ProtoRegistry.RegisterString("遗物描述0-8", "", "我方对敌舰造成的任何伤害时，会为正在发生战斗星系的一个随机行星立刻回复相当于实际伤害10%的护盾");
            ProtoRegistry.RegisterString("遗物描述0-9", "", "❤你会更加幸运❤");

            ProtoRegistry.RegisterString("遗物描述1-0", "", "每次入侵结束，根据来袭总强度少量推进巨构的建造进度");
            ProtoRegistry.RegisterString("遗物描述1-1", "", "导弹的范围伤害（以及范围效果）不再随距离衰减");
            ProtoRegistry.RegisterString("遗物描述1-2", "", "拥有巨构的星系会自动缓慢充能该星系全部星球的护盾，充能速度取决于巨构能量水平");
            ProtoRegistry.RegisterString("遗物描述1-3", "", "拥有巨构的星系在战斗开始的前60秒对所有敌舰施加30%减速效果，并使他们在这期间受到30%的<i>额外伤害</i>");
            ProtoRegistry.RegisterString("遗物描述1-4", "", "立刻获得一个水滴，水滴能量消耗-50%");
            ProtoRegistry.RegisterString("遗物描述1-5", "", "导弹发射井在每次发射后向本体回填一发免费弹药");
            ProtoRegistry.RegisterString("遗物描述1-6", "", "生产巨构火箭时，每个产出返还2个氘核燃料棒");
            ProtoRegistry.RegisterString("遗物描述1-7", "", "巨构的太阳帆吸附速度提升300%");
            ProtoRegistry.RegisterString("遗物描述1-8", "", "每场入侵中，降落的前十个敌舰不会摧毁任何建筑或资源");
            ProtoRegistry.RegisterString("遗物描述1-9", "", "行星力场护盾受到伤害时，如果你机甲的能量高于20%，你将损失相当于伤害2000倍的能量，并使该次伤害无效化");
            ProtoRegistry.RegisterString("遗物描述1-10", "", "虫洞不再随着被连续摧毁的数量增加而提升伤害减免（最后一个除外）");
            ProtoRegistry.RegisterString("遗物描述1-11", "", "引力塌陷导弹不再聚拢敌舰，而是使他们在原地冻结3s，距离爆心越远受到的影响越小");

            ProtoRegistry.RegisterString("遗物描述2-0", "", "来自行星护盾生成器的护盾充能速度提升50%");
            ProtoRegistry.RegisterString("遗物描述2-1", "", "将异星元数据上传给主脑时，略微推进巨构的建造进度");
            ProtoRegistry.RegisterString("遗物描述2-2", "", "建筑被敌舰摧毁时，获得大量功勋点数，被毁物流塔内的贵重资源会增加获取的功勋点数");
            ProtoRegistry.RegisterString("遗物描述2-3", "", "引力炮台有75%概率在发射后回填一颗弹药");
            ProtoRegistry.RegisterString("遗物描述2-4", "", "生产燃料棒时，每次产出会回填5个第2位置的原材料（氢、重氢）");
            ProtoRegistry.RegisterString("遗物描述2-5", "", "每过一秒，如果伊卡洛斯处于行星上并且在上一秒进行过移动，就有8%的概率获得一个多功能集成组件");
            ProtoRegistry.RegisterString("遗物描述2-6", "", "水滴能量消耗-40%");
            ProtoRegistry.RegisterString("遗物描述2-7", "", "所有导弹对首要目标造成100%<i>额外伤害</i>");
            ProtoRegistry.RegisterString("遗物描述2-8", "", "分解临界光子时，不再产出氢，但产出的反物质增加50%");
            ProtoRegistry.RegisterString("遗物描述2-9", "", "恒星炮充能速度+50%");
            ProtoRegistry.RegisterString("遗物描述2-10", "", "拥有巨构的星系，行星的最大护盾上限提升，根据巨构的能量产出，最多提升50%");
            ProtoRegistry.RegisterString("遗物描述2-11", "", "熔炉每次产出，有30%的概率额外产出一个产物");
            ProtoRegistry.RegisterString("遗物描述2-12", "", "你对敌舰造成的任何伤害有10%的概率造成100%<i>额外伤害</i>");
            ProtoRegistry.RegisterString("遗物描述2-13", "", "你对敌舰造成的任何<i>额外伤害翻倍</i>（来自科技的加成除外）");
            ProtoRegistry.RegisterString("遗物描述2-14", "", "每次击毁敌舰，根据敌舰强度有概率在背包直接获取1个反物质燃料棒或翘曲器，无视科技解锁进度");
            ProtoRegistry.RegisterString("遗物描述2-15", "", "引力塌陷导弹对首要目标造成1000%<i>额外伤害</i>");
            ProtoRegistry.RegisterString("遗物描述2-16", "", "敌舰对护盾造成的强化伤害随时间的增幅变缓");
            ProtoRegistry.RegisterString("遗物描述2-17", "", "每场入侵中，第一个被打破的行星护盾会立刻回填相当于50%护盾上限的护盾量");

            ProtoRegistry.RegisterString("遗物描述3-0", "", "太阳帆寿命-90%，但每次产出太阳帆会额外产出1个太阳帆");
            ProtoRegistry.RegisterString("遗物描述3-1", "", "每次击毁敌舰，基于敌舰强度和波次掉落额外的异星矩阵");
            ProtoRegistry.RegisterString("遗物描述3-2", "", "伊卡洛斯会不消耗燃料地持续获得额外的能量回复，相当于反应堆基础功率的50%");
            ProtoRegistry.RegisterString("遗物描述3-3", "", "击毁敌舰时，基于敌舰强度获得沙土");
            ProtoRegistry.RegisterString("遗物描述3-4", "", "手动加速敌军来袭时，因加速而损失的预期矩阵掉落数减半");
            ProtoRegistry.RegisterString("遗物描述3-5", "", "立刻重置敌舰进攻的总计次，并将难度调整为普通，同时给予一次重新调整难度的机会\n不会占用圣物槽位");
            ProtoRegistry.RegisterString("遗物描述3-6", "", "生产芯片、量子芯片时，来自增产剂的增产效果加强50%");
            ProtoRegistry.RegisterString("遗物描述3-7", "", "任何子弹命中时会对一个随机其他敌舰造成相当于20%的<i>额外伤害</i>");
            ProtoRegistry.RegisterString("遗物描述3-8", "", "基于已解锁的科技，立刻获得大量普通矩阵（异星矩阵和宇宙矩阵除外），不会占用圣物槽位");
            ProtoRegistry.RegisterString("遗物描述3-9", "", "敌舰降落时，立刻少量推进随机巨构的建造进度");
            ProtoRegistry.RegisterString("遗物描述3-10", "", "每过一秒，如果伊卡洛斯处于行星上并且在上一秒进行过移动，就有3%的概率获得一个多功能集成组件");
            ProtoRegistry.RegisterString("遗物描述3-11", "", "巨构的太阳帆吸附速度提升100%");
            ProtoRegistry.RegisterString("遗物描述3-12", "", "行星力场护盾有15%的概率完全规避伤害");
            ProtoRegistry.RegisterString("遗物描述3-13", "", "恒星炮充能速度提高25%");
            ProtoRegistry.RegisterString("遗物描述3-14", "", "生产电动机、电磁涡轮时，每生产一个产物，回填1个磁线圈作为原材料");
            ProtoRegistry.RegisterString("遗物描述3-15", "", "伊卡洛斯机甲的研究速度+400%，研究能耗同步增加");
            ProtoRegistry.RegisterString("遗物描述3-16", "", "恒星炮对主要目标造成10%<i>额外伤害</i>");
            ProtoRegistry.RegisterString("遗物描述3-17", "", "每次提升功勋阶级，显著推进各巨构的建造进度");

            ProtoRegistry.RegisterString("遗物名称带颜色0-0", "", "<color=#d2853d>吞噬者  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-1", "", "<color=#d2853d>蓝buff  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-2", "", "<color=#d2853d>女神之泪  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-3", "", "<color=#d2853d>坚固透镜  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-4", "", "<color=#d2853d>黑百合  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-5", "", "<color=#d2853d>虚空荆棘  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-6", "", "<color=#d2853d>京级巨炮  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-7", "", "<color=#d2853d>撕裂力场  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-8", "", "<color=#d2853d>饮血剑  [传说]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色0-9", "", "<color=#d2853d>五叶草  [传说]</color>");

            ProtoRegistry.RegisterString("遗物名称带颜色1-0", "", "<color=#9040d0>凯旋  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-1", "", "<color=#9040d0>虚空爆发  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-2", "", "<color=#9040d0>能量涌动  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-3", "", "<color=#9040d0>敌对海域  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-4", "", "<color=#9040d0>三体  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-5", "", "<color=#9040d0>回声 II  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-6", "", "<color=#9040d0>能量迸发  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-7", "", "<color=#9040d0>活性炭 II  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-8", "", "<color=#9040d0>女妖面纱  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-9", "", "<color=#9040d0>骑士之誓  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-10", "", "<color=#9040d0>真实伤害  [史诗]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色1-11", "", "<color=#9040d0>冰封陵墓  [史诗]</color>");

            ProtoRegistry.RegisterString("遗物名称带颜色2-0", "", "<color=#2080d0>超充能器  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-1", "", "<color=#2080d0>互惠互利  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-2", "", "<color=#2080d0>极限一换一  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-3", "", "<color=#2080d0>回声 I  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-4", "", "<color=#2080d0>听说有人缺氢  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-5", "", "<color=#2080d0>多动症 II  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-6", "", "<color=#2080d0>高效索敌  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-7", "", "<color=#2080d0>仇敌  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-8", "", "<color=#2080d0>狄拉克辶辶变  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-9", "", "<color=#2080d0>聚能环 II  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-10", "", "<color=#2080d0>矩阵充能  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-11", "", "<color=#2080d0>副产物提炼  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-12", "", "<color=#2080d0>强攻  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-13", "", "<color=#2080d0>无尽之刃  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-14", "", "<color=#2080d0>行窃预兆  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-15", "", "<color=#2080d0>引力碎裂  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-16", "", "<color=#2080d0>刚毅护盾  [稀有]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色2-17", "", "<color=#2080d0>不朽之守护  [稀有]</color>");

            ProtoRegistry.RegisterString("遗物名称带颜色3-0", "", "<color=#30b530>劣质加工  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-1", "", "<color=#30b530>窃法之刃  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-2", "", "<color=#30b530>方舟反应堆  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-3", "", "<color=#30b530>掘墓人  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-4", "", "<color=#30b530>装  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-5", "", "<color=#30b530>复活币  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-6", "", "<color=#30b530>光刻机  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-7", "", "<color=#30b530>虚空折射  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-8", "", "<color=#30b530>矩阵雨  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-9", "", "<color=#30b530>开摆  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-10", "", "<color=#30b530>多动症 I  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-11", "", "<color=#30b530>活性炭 I  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-12", "", "<color=#30b530>灵动巨物  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-13", "", "<color=#30b530>聚能环 I  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-14", "", "<color=#30b530>阳间马达  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-15", "", "<color=#30b530>超级大脑  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-16", "", "<color=#30b530>虚空棱镜  [普通]</color>");
            ProtoRegistry.RegisterString("遗物名称带颜色3-17", "", "<color=#30b530>荣誉晋升  [普通]</color>");

            ItemProto bullet1 = ProtoRegistry.RegisterItem(8001, "子弹1", "子弹1描述", "Assets/DSPBattle/bullet1", 2701 + pageBias, 100, EItemType.Material);
            ItemProto bullet2 = ProtoRegistry.RegisterItem(8002, "子弹2", "子弹2描述", "Assets/DSPBattle/bullet2", 2702 + pageBias, 100, EItemType.Material);
            ItemProto bullet3 = ProtoRegistry.RegisterItem(8003, "子弹3", "子弹3描述", "Assets/DSPBattle/bullet3", 2703 + pageBias, 100, EItemType.Material);
            ItemProto missile1 = ProtoRegistry.RegisterItem(8004, "导弹1", "导弹1描述", "Assets/DSPBattle/missile1", 2705 + pageBias, 100, EItemType.Material);
            ItemProto missile2 = ProtoRegistry.RegisterItem(8005, "导弹2", "导弹2描述", "Assets/DSPBattle/missile2", 2706 + pageBias, 100, EItemType.Material);
            ItemProto missile3 = ProtoRegistry.RegisterItem(8006, "导弹3", "导弹3描述", "Assets/DSPBattle/missile3", 2707 + pageBias, 100, EItemType.Material);
            ItemProto bullet4 = ProtoRegistry.RegisterItem(8007, "脉冲", "脉冲描述", "Assets/DSPBattle/bullet4", 9999, 100, EItemType.Material);
            var icondesc = ProtoRegistry.GetDefaultIconDesc(Color.white, new Color(0f, 0.1f, 1f));
            icondesc.solidAlpha = 0f;
            ItemProto alienMatrix = ProtoRegistry.RegisterItem(8032, "异星矩阵", "异星矩阵描述", "Assets/DSPBattle/alienmatrix", 2712 + pageBias, 1000000, EItemType.Matrix, icondesc);
            alienMatrix.Productive = true;
            ItemProto alienMeta = ProtoRegistry.RegisterItem(8033, "异星元数据", "异星元数据描述", "Assets/DSPBattle/alienmeta", 2711 + pageBias, 1000000000, EItemType.Matrix, icondesc);
            alienMeta.Productive = false;
            if (Configs.enableProliferator4)
            {
                var icondesc2 = ProtoRegistry.GetDefaultIconDesc(new Color(1f, 1f, 1f), new Color(0f, 1f, 1f), new Color(0, 0.2f, 0.3f), new Color(0, 0.4f, 0.5f));
                ItemProto proliferator4 = ProtoRegistry.RegisterItem(8034, "量子增产剂", "量子增产剂描述", "Assets/DSPBattle/accelerator4", 2710 + pageBias, 200, EItemType.Material, icondesc2);
                proliferator4.Productive = true;
                proliferator4.Ability = 6;
                proliferator4.HpMax = 120;
                proliferator4.DescFields = new int[] { 29, 41, 42, 43, 1, 40 };
            }

            bullet1.DescFields = new int[] { 50, 51, 1 };
            bullet2.DescFields = new int[] { 50, 51, 1 };
            bullet3.DescFields = new int[] { 50, 51, 1 };
            bullet4.DescFields = new int[] { 50, 51 };
            missile1.DescFields = new int[] { 50, 51, 52, 1 };
            missile2.DescFields = new int[] { 50, 51, 52, 1 };
            missile3.DescFields = new int[] { 50, 51, 52, 1 };

            ProtoRegistry.RegisterItem(8021, "彩蛋1", "彩蛋1描述", "Assets/DSPBattle/notes-of-pioneer-01", 9999, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8022, "彩蛋2", "彩蛋2描述", "Assets/DSPBattle/notes-of-pioneer-02", 9999, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8023, "彩蛋3", "彩蛋3描述", "Assets/DSPBattle/notes-of-pioneer-03", 9999, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8024, "彩蛋4", "彩蛋4描述", "Assets/DSPBattle/notes-of-pioneer-04", 9999, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8025, "彩蛋5", "彩蛋5描述", "Assets/DSPBattle/notes-of-pioneer-05", 9999, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8026, "彩蛋6", "彩蛋6描述", "Assets/DSPBattle/notes-of-pioneer-06", 9999, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8027, "彩蛋7", "彩蛋7描述", "Assets/DSPBattle/notes-of-pioneer-07", 9999, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8028, "彩蛋8", "彩蛋8描述", "Assets/DSPBattle/notes-of-pioneer-08", 9999, 100, EItemType.Material);
            ProtoRegistry.RegisterItem(8029, "彩蛋9", "彩蛋9描述", "Assets/DSPBattle/notes-of-pioneer-09", 9999, 100, EItemType.Material);


            var Cannon1 = ProtoRegistry.RegisterItem(8011, "弹射器1", "弹射器1描述", "Assets/DSPBattle/cannon1", 2601 + pageBias, 50, EItemType.Production);
            Cannon1.BuildIndex = 607;
            Cannon1.BuildMode = 1;
            Cannon1.IsEntity = true;
            Cannon1.isRaw = false;
            Cannon1.CanBuild = true;
            Cannon1.Upgrades = new int[] { };
            //Cannon1.Grade = 1;
            //Cannon1.Upgrades = new int[] { 8011, 8012, 8014 };
            Cannon1.DescFields = new int[] { 53, 11, 12, 1, 40 };
            var Cannon2 = ProtoRegistry.RegisterItem(8012, "弹射器2", "弹射器2描述", "Assets/DSPBattle/cannon2", 2602 + pageBias, 50, EItemType.Production);
            Cannon2.BuildIndex = 608;
            Cannon2.BuildMode = 1;
            Cannon2.IsEntity = true;
            Cannon2.isRaw = false;
            Cannon2.CanBuild = true;
            Cannon2.Upgrades = new int[] { };
            //Cannon2.Grade = 2;
            //Cannon2.Upgrades = new int[] { 8011, 8012, 8014 };
            Cannon2.DescFields = new int[] { 53, 11, 12, 1, 40 };
            var Cannon3 = ProtoRegistry.RegisterItem(8014, "脉冲炮", "脉冲炮描述", "Assets/DSPBattle/cannon3", 2604 + pageBias, 50, EItemType.Production);
            Cannon3.BuildIndex = 609;
            Cannon3.BuildMode = 1;
            Cannon3.IsEntity = true;
            Cannon3.isRaw = false;
            Cannon3.CanBuild = true;
            Cannon3.Upgrades = new int[] { };
            //Cannon3.Grade = 3;
            //Cannon3.Upgrades = new int[] { 8011, 8012, 8014 };
            Cannon3.DescFields = new int[] { 50, 51, 53, 11, 12, 1, 40 };


            var Silo = ProtoRegistry.RegisterItem(8013, "发射器1", "发射器1描述", "Assets/DSPBattle/missilesilo", 2603 + pageBias, 50, EItemType.Production);
            Silo.BuildIndex = 610;
            Silo.BuildMode = 1;
            Silo.IsEntity = true;
            Silo.isRaw = false;
            Silo.CanBuild = true;
            Silo.Upgrades = new int[] { };
            Silo.DescFields = new int[] { 35, 11, 12, 1, 40 };


            var ShieldGenerator = ProtoRegistry.RegisterItem(8030, "行星护盾生成器", "行星护盾生成器描述", "Assets/MegaStructureTab/shieldGen", 2605 + pageBias, 10, EItemType.Production);
            ShieldGenerator.BuildMode = 1;
            ShieldGenerator.IsEntity = true;
            ShieldGenerator.isRaw = false;
            ShieldGenerator.CanBuild = true;
            ShieldGenerator.Upgrades = new int[] { };
            ShieldGenerator.DescFields = new int[] { 11, 57, 58, 1, 40 };

            int hideMask = 393;
            if (Configs.developerMode) hideMask = 0;
            var TestEngine = ProtoRegistry.RegisterItem(8031, "测试用发动机", "测试用发动机描述", "Assets/MegaStructureTab/shieldGen", 2606 + hideMask + pageBias, 10, EItemType.Production);
            TestEngine.BuildMode = 1;
            TestEngine.IsEntity = true;
            TestEngine.isRaw = false;
            TestEngine.CanBuild = true;
            TestEngine.Upgrades = new int[] { };


            ProtoRegistry.RegisterRecipe(801, ERecipeType.Assemble, 60, new int[] { 1112, 1103 }, new int[] { 1, 1 }, new int[] { 8001 }, new int[] { 1 }, "子弹1描述",
                1901, 2701 + pageBias, "Assets/DSPBattle/bullet1");
            ProtoRegistry.RegisterRecipe(802, ERecipeType.Assemble, 90, new int[] { 1118, 1110, 1116 }, new int[] { 1, 1, 1 }, new int[] { 8002 }, new int[] { 2 }, "子弹2描述",
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
            ProtoRegistry.RegisterRecipe(812, ERecipeType.Assemble, 360, new int[] { 1107, 9480, 1303, 1209 }, new int[] { 10, 10, 10, 3 }, new int[] { 8012 }, new int[] { 1 }, "弹射器2描述",
                1914, 2602 + pageBias, "Assets/DSPBattle/cannon2");
            ProtoRegistry.RegisterRecipe(813, ERecipeType.Assemble, 900, new int[] { 1107, 1125, 1209, 1305 }, new int[] { 40, 15, 5, 5 }, new int[] { 8013 }, new int[] { 1 }, "发射器1描述",
                1911, 2603 + pageBias, "Assets/DSPBattle/missilesilo");
            ProtoRegistry.RegisterRecipe(814, ERecipeType.Assemble, 900, new int[] { 1125, 9481, 9486 }, new int[] { 20, 5, 2 }, new int[] { 8014 }, new int[] { 1 }, "脉冲炮描述",
                1915, 2604 + pageBias, "Assets/DSPBattle/cannon3");
            ProtoRegistry.RegisterRecipe(815, ERecipeType.Assemble, 900, new int[] { 9503, 1305, 1125 }, new int[] { 30, 20, 30 }, new int[] { 8030 }, new int[] { 1 }, "行星护盾生成器描述",
                1916, 2605 + pageBias, "Assets/MegaStructureTab/shieldGen");
            ProtoRegistry.RegisterRecipe(816, ERecipeType.Assemble, 10, new int[] { 1101 }, new int[] { 1 }, new int[] { 8031 }, new int[] { 1 }, "测试用发动机描述",
                1916, 2606 + hideMask + pageBias, "Assets/MegaStructureTab/shieldGen");
            ProtoRegistry.RegisterRecipe(817, ERecipeType.Research, 60, new int[] { 8032 }, new int[] { 2 }, new int[] { 8032 }, new int[] { 1 }, "异星矩阵描述",
                1901, 9999 + pageBias, "Assets/DSPBattle/alienmatrix");

            RecipeProto decompileRecipe0 = ProtoRegistry.RegisterRecipe(818, ERecipeType.Research, 60, new int[] { 8032 }, new int[] { 10 }, new int[] { 8033 }, new int[] { 500 }, "异星元数据描述",
                1924, 2712 + pageBias, "异星矩阵反编译", "Assets/DSPBattle/alienmetax100");
            decompileRecipe0.Explicit = true;
            decompileRecipe0.NonProductive = true;
            //RecipeProto decompileRecipe1 = ProtoRegistry.RegisterRecipe(819, ERecipeType.Research, 60, new int[] { 8032 }, new int[] { 10 }, new int[] { 8033 }, new int[] { 500 }, "异星元数据描述",
            //     1924, 2712 + pageBias, "异星矩阵反编译 x10", "Assets/DSPBattle/alienmetax100");
            //decompileRecipe1.Explicit = true;
            if (Configs.enableProliferator4)
            {
                RecipeProto proliferator4Recipe = ProtoRegistry.RegisterRecipe(820, ERecipeType.Assemble, 240, new int[] { 1143, 8032 }, new int[] { 8, 1 }, new int[] { 8034 }, new int[] { 4 }, "量子增产剂描述",
                     1925, 2711 + pageBias, "量子增产剂", "Assets/DSPBattle/accelerator4");
            }


            //给船染色用物品
            ProtoRegistry.RegisterItem(8040, "侦查艇".Translate(), "敌船0".Translate(), "Assets/DSPBattle/enemyShip0", 9999, 200,
                EItemType.Component, ProtoRegistry.GetDefaultIconDesc(new Color(0.8f, 0f, 0f), new Color(0.8f, 0f, 0f), new Color(0.8f, 0f, 0f), new Color(0.8f, 0f, 0f)));
            ProtoRegistry.RegisterItem(8041, "护卫舰".Translate(), "敌船1".Translate(), "Assets/DSPBattle/enemyShip1", 9999, 200,
                EItemType.Component, ProtoRegistry.GetDefaultIconDesc(new Color(0.5f, 0.2f, 0f), new Color(0.5f, 0.2f, 0f), new Color(0.5f, 0.2f, 0f), new Color(0.5f, 0.2f, 0f)));
            ProtoRegistry.RegisterItem(8042, "驱逐舰".Translate(), "敌船2".Translate(), "Assets/DSPBattle/enemyShip2", 9999, 200,
               
                EItemType.Component, ProtoRegistry.GetDefaultIconDesc(new Color(1f, 0.8f, 0f), new Color(1f, 0.8f, 0f), new Color(1f, 0.7f, 0f), new Color(1f, 0.7f, 0f)));
            ProtoRegistry.RegisterItem(8043, "重型巡洋舰".Translate(), "敌船3".Translate(), "Assets/DSPBattle/enemyShip3", 9999, 200,
                EItemType.Component, ProtoRegistry.GetDefaultIconDesc(new Color(0f, 1f, 0f), new Color(0f, 1f, 0f), new Color(0f, 1f, 0f), new Color(0f, 1f, 0f)));
            ProtoRegistry.RegisterItem(8044, "战列舰".Translate(), "敌船4".Translate(), "Assets/DSPBattle/enemyShip4", 9999, 200,
                EItemType.Component, ProtoRegistry.GetDefaultIconDesc(new Color(1f, 0f, 0.6f), new Color(1f, 0f, 0.6f), new Color(1f, 0f, 0.6f), new Color(1f, 0f, 0.6f)));
            //ProtoRegistry.RegisterRecipe(820, ERecipeType.Assemble, 1, new int[] { 1101 }, new int[] { 1 }, new int[] { 8040 }, new int[] { 1 }, "测试用1",
            //    1901, 9999, "Assets/DSPBattle/enemyShip");
            //ProtoRegistry.RegisterRecipe(821, ERecipeType.Assemble, 1, new int[] { 1101 }, new int[] { 1 }, new int[] { 8041 }, new int[] { 1 }, "测试用1",
            //    1901, 9999, "Assets/DSPBattle/enemyShip");
            //ProtoRegistry.RegisterRecipe(822, ERecipeType.Assemble, 1, new int[] { 1101 }, new int[] { 1 }, new int[] { 8042 }, new int[] { 1 }, "测试用1",
            //    1901, 9999, "Assets/DSPBattle/enemyShip");
            //ProtoRegistry.RegisterRecipe(823, ERecipeType.Assemble, 1, new int[] { 1101 }, new int[] { 1 }, new int[] { 8043 }, new int[] { 1 }, "测试用1",
            //    1901, 9999, "Assets/DSPBattle/enemyShip");
            //ProtoRegistry.RegisterRecipe(824, ERecipeType.Assemble, 1, new int[] { 1101 }, new int[] { 1 }, new int[] { 8044 }, new int[] { 1 }, "测试用1",
            //    1901, 9999, "Assets/DSPBattle/enemyShip");

            TechProto techBullet1 = ProtoRegistry.RegisterTech(1901, "近地防卫系统", "近地防卫系统描述", "近地防卫系统结论", "Assets/DSPBattle/bullet1tech", new int[] { 1711 }, new int[] { 6001, 6002 }, new int[] { 20, 20 },
                72000, new int[] { 801, 811, 817 }, new Vector2(29, -43));

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
                new int[] { 24, 24, 24, 24, 24 }, 300000, new int[] { 814 }, new Vector2(53, -43));
            techBullet4.AddItems = new int[] { 8027 };
            techBullet4.AddItemCounts = new int[] { 1 };


            TechProto techShield1 = ProtoRegistry.RegisterTech(1916, "行星力场护盾", "行星力场护盾描述", "行星力场护盾结论", "Assets/DSPBattle/shieldtech", new int[] { 1705 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24 }, 300000, new int[] { 565,815 }, new Vector2(53, -31));
            
            TechProto techStellarFortress = ProtoRegistry.RegisterTech(1917, "星际要塞", "星际要塞描述", "星际要塞结论", "Assets/DSPBattle/cannon3tech", new int[] { 1903 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24 }, 150000, new int[] { 816 }, new Vector2(41, -43));
            techStellarFortress.Published = false;
            TechProto techStarCannon = ProtoRegistry.RegisterTech(1918, "尼科尔戴森光束", "尼科尔戴森光束描述", "尼科尔戴森光束结论", "Assets/DSPBattle/starcannontech", new int[] { 1144 }, new int[] { 8032 },
                new int[] { 200 }, 36000, new int[] { 570, 571, 572 }, new Vector2(65, -3));
            
            TechProto techDrop = ProtoRegistry.RegisterTech(1919, "玻色子操控", "玻色子操控描述", "玻色子操控结论", "Assets/DSPBattle/bosontech", new int[] { 1915 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 24, 24, 24, 24, 24 }, 450000, new int[] { 566, 567, 568, 569, 573 }, new Vector2(57, -43));


            TechProto techMegaMD = ProtoRegistry.RegisterTech(1920, "物质解压器", "物质解压器科技描述", "物质解压器科技结论", "Assets/DSPBattle/techMD", new int[] {  }, new int[] { 8032 },
                new int[] { 150 }, 36000, new int[] { 538 }, new Vector2(65, -7)); 
            techMegaMD.PreTechsImplicit = new int[] { 1522 };
            TechProto techMegaWBA = ProtoRegistry.RegisterTech(1921, "折跃场广播阵列", "折跃场广播阵列科技描述", "折跃场广播阵列科技结论", "Assets/DSPBattle/techWBA", new int[] { }, new int[] { 8032 },
                new int[] { 240 }, 27000, new int[] { 540 }, new Vector2(65, -11));
            techMegaWBA.PreTechsImplicit = new int[] { 1522 };
            TechProto techMegaIA = ProtoRegistry.RegisterTech(1922, "星际组装厂", "星际组装厂科技描述", "星际组装厂科技结论", "Assets/DSPBattle/techIA", new int[] { }, new int[] { 8032 },
                new int[] { 200 }, 36000, new int[] { 537, 541 }, new Vector2(65, -15));
            techMegaIA.PreTechsImplicit = new int[] { 1522 };
            TechProto techMegaCR = ProtoRegistry.RegisterTech(1923, "晶体重构器", "晶体重构器科技描述", "晶体重构器科技结论", "Assets/DSPBattle/techCR", new int[] { }, new int[] { 8032 },
                new int[] { 240 }, 27000, new int[] { 542 }, new Vector2(65, -19));
            techMegaCR.PreTechsImplicit = new int[] { 1522 };
            TechProto techMegaSN = ProtoRegistry.RegisterTech(1924, "科学枢纽", "科学枢纽科技描述", "科学枢纽科技结论", "Assets/DSPBattle/techSN", new int[] { 1918, 1920, 1921, 1922, 1923 }, new int[] { 8032 },
                new int[] { 200 }, 45000, new int[] { 539, 818, 819 }, new Vector2(69, -11));
            techMegaSN.PreTechsImplicit = new int[] { 1522 };

            if (Configs.enableProliferator4)
            {
                TechProto techProliferator4 = ProtoRegistry.RegisterTech(1925, "量子增产剂", "量子增产剂科技描述", "量子增产剂科技结论", "Assets/DSPBattle/accelerator4tech", new int[] { 1153 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 8, 6, 4, 4 }, 450000, new int[] { 820 }, new Vector2(37, -11));
                techMegaSN.PreTechsImplicit = new int[] { 1522 };
            }

            TechProto winGame = LDB.techs.Select(1508);
            winGame.AddItems = new int[] { 8028, 8029 };
            winGame.AddItemCounts = new int[] { 1, 1 };

            //循环科技 分别是+20%子弹伤害  +10%子弹速度和2%导弹速度  以及扩充虫洞安全区
            TechProto techBulletDamage1 = ProtoRegistry.RegisterTech(4901, "定向爆破1", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level1_tech", new int[] { }, new int[] { 6001, 6002, 6003 },
                new int[] { 20, 20, 20 }, 180000, new int[] { }, new Vector2(9, -51));
            techBulletDamage1.PreTechsImplicit = new int[] { 1911 };
            techBulletDamage1.UnlockFunctions = new int[] { 50 };
            techBulletDamage1.UnlockValues = new double[] { 0.15 };
            techBulletDamage1.Level = 1;
            techBulletDamage1.MaxLevel = 1;
            techBulletDamage1.LevelCoef1 = 0;
            techBulletDamage1.LevelCoef2 = 0;
            TechProto techBulletDamage2 = ProtoRegistry.RegisterTech(4902, "定向爆破2", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level2_tech", new int[] { 4901 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 20, 20, 20, 20 }, 180000, new int[] { }, new Vector2(13, -51));
            techBulletDamage2.UnlockFunctions = new int[] { 50 };
            techBulletDamage2.UnlockValues = new double[] { 0.15 };
            techBulletDamage2.Level = 2;
            techBulletDamage2.MaxLevel = 2;
            techBulletDamage2.LevelCoef1 = 0;
            techBulletDamage2.LevelCoef2 = 0;
            TechProto techBulletDamage3 = ProtoRegistry.RegisterTech(4903, "定向爆破3", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level3_tech", new int[] { 4902 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 12, 12, 12 }, 480000, new int[] { }, new Vector2(17, -51));
            techBulletDamage3.UnlockFunctions = new int[] { 50 };
            techBulletDamage3.UnlockValues = new double[] { 0.15 };
            techBulletDamage3.Level = 3;
            techBulletDamage3.MaxLevel = 3;
            techBulletDamage3.LevelCoef1 = 0;
            techBulletDamage3.LevelCoef2 = 0;
            TechProto techBulletDamage4 = ProtoRegistry.RegisterTech(4904, "定向爆破4", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level4_tech", new int[] { 4903 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 4, 4, 4, 4, 4 }, 1800000, new int[] { }, new Vector2(21, -51));
            techBulletDamage4.UnlockFunctions = new int[] { 50 };
            techBulletDamage4.UnlockValues = new double[] { 0.15 };
            techBulletDamage4.Level = 4;
            techBulletDamage4.MaxLevel = 4;
            techBulletDamage4.LevelCoef1 = 0;
            techBulletDamage4.LevelCoef2 = 0;
            TechProto techBulletDamage5 = ProtoRegistry.RegisterTech(4905, "定向爆破5", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level5_tech", new int[] { 4904 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 4, 4, 4, 4, 4 }, 2700000, new int[] { }, new Vector2(25, -51));
            techBulletDamage5.UnlockFunctions = new int[] { 50 };
            techBulletDamage5.UnlockValues = new double[] { 0.15 };
            techBulletDamage5.Level = 5;
            techBulletDamage5.MaxLevel = 5;
            techBulletDamage5.LevelCoef1 = 0;
            techBulletDamage5.LevelCoef2 = 0;
            TechProto techBulletDamageInf = ProtoRegistry.RegisterTech(4906, "定向爆破6", "定向爆破描述", "定向爆破结论", "Assets/DSPBattle/attack-level-infinitude_tech", new int[] { 4905 }, new int[] { 6006 },
                new int[] { 4 }, -18000000, new int[] { }, new Vector2(29, -51));
            techBulletDamageInf.UnlockFunctions = new int[] { 50 };
            techBulletDamageInf.UnlockValues = new double[] { 0.15 };
            techBulletDamageInf.Level = 6;
            techBulletDamageInf.MaxLevel = 10000;
            techBulletDamageInf.LevelCoef1 = 3600000;
            techBulletDamageInf.LevelCoef2 = 0;


            TechProto techBulletSpeed1 = ProtoRegistry.RegisterTech(4911, "引力波引导1", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech1", new int[] { }, new int[] { 6001, 6002, 6003 },
                 new int[] { 20, 20, 20 }, 180000, new int[] { }, new Vector2(9, -55));
            techBulletSpeed1.PreTechsImplicit = new int[] { 1911 };
            techBulletSpeed1.UnlockFunctions = new int[] { 51 };
            techBulletSpeed1.UnlockValues = new double[] { 0.1 };
            techBulletSpeed1.Level = 1;
            techBulletSpeed1.MaxLevel = 1;
            techBulletSpeed1.LevelCoef1 = 0;
            techBulletSpeed1.LevelCoef2 = 0;
            TechProto techBulletSpeed2 = ProtoRegistry.RegisterTech(4912, "引力波引导2", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech2", new int[] { 4911 }, new int[] { 6001, 6002, 6003, 6004 },
                 new int[] { 20, 20, 20, 20 }, 180000, new int[] { }, new Vector2(13, -55));
            techBulletSpeed2.UnlockFunctions = new int[] { 51 };
            techBulletSpeed2.UnlockValues = new double[] { 0.1 };
            techBulletSpeed2.Level = 2;
            techBulletSpeed2.MaxLevel = 2;
            techBulletSpeed2.LevelCoef1 = 0;
            techBulletSpeed2.LevelCoef2 = 0;
            TechProto techBulletSpeed3 = ProtoRegistry.RegisterTech(4913, "引力波引导3", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech3", new int[] { 4912 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 12, 12, 12 }, 480000, new int[] { }, new Vector2(17, -55));
            techBulletSpeed3.UnlockFunctions = new int[] { 51 };
            techBulletSpeed3.UnlockValues = new double[] { 0.1 };
            techBulletSpeed3.Level = 3;
            techBulletSpeed3.MaxLevel = 3;
            techBulletSpeed3.LevelCoef1 = 0;
            techBulletSpeed3.LevelCoef2 = 0;
            TechProto techBulletSpeed4 = ProtoRegistry.RegisterTech(4914, "引力波引导4", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech4", new int[] { 4913 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 4, 4, 4, 4, 4 }, 1800000, new int[] { }, new Vector2(21, -55));
            techBulletSpeed4.UnlockFunctions = new int[] { 51 };
            techBulletSpeed4.UnlockValues = new double[] { 0.1 };
            techBulletSpeed4.Level = 4;
            techBulletSpeed4.MaxLevel = 4;
            techBulletSpeed4.LevelCoef1 = 0;
            techBulletSpeed4.LevelCoef2 = 0;
            TechProto techBulletSpeed5 = ProtoRegistry.RegisterTech(4915, "引力波引导5", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech5", new int[] { 4914 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                new int[] { 4, 4, 4, 4, 4 }, 2700000, new int[] { }, new Vector2(25, -55));
            techBulletSpeed5.UnlockFunctions = new int[] { 51 };
            techBulletSpeed5.UnlockValues = new double[] { 0.1 };
            techBulletSpeed5.Level = 5;
            techBulletSpeed5.MaxLevel = 5;
            techBulletSpeed5.LevelCoef1 = 0;
            techBulletSpeed5.LevelCoef2 = 0;
            TechProto techBulletSpeedInf = ProtoRegistry.RegisterTech(4916, "引力波引导6", "引力波引导描述", "引力波引导结论", "Assets/DSPBattle/bulletspeedtech0", new int[] { 4915 }, new int[] { 6006 },
                new int[] { 4 }, -18000000, new int[] { }, new Vector2(29, -55));
            techBulletSpeedInf.UnlockFunctions = new int[] { 51 };
            techBulletSpeedInf.UnlockValues = new double[] { 0.1 };
            techBulletSpeedInf.Level = 6;
            techBulletSpeedInf.MaxLevel = 100;
            techBulletSpeedInf.LevelCoef1 = 3600000;
            techBulletSpeedInf.LevelCoef2 = 0;

            TechProto techWormDistance1 = ProtoRegistry.RegisterTech(4921, "相位干扰技术1", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level1_tech", new int[] { }, new int[] { 6001, 6002, 6003 },
                new int[] { 20, 20, 20 }, 144000, new int[] { }, new Vector2(9, -59));
            techWormDistance1.PreTechsImplicit = new int[] { 1911 };
            techWormDistance1.UnlockFunctions = new int[] { 52 };
            techWormDistance1.UnlockValues = new double[] { 10000 };
            techWormDistance1.Level = 1;
            techWormDistance1.MaxLevel = 1;
            techWormDistance1.LevelCoef1 = 0;
            techWormDistance1.LevelCoef2 = 0;
            TechProto techWormDistance2 = ProtoRegistry.RegisterTech(4922, "相位干扰技术2", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level2_tech", new int[] { 4921 }, new int[] { 6001, 6002, 6003, 6004 },
                new int[] { 12, 12, 12, 12 }, 300000, new int[] { }, new Vector2(13, -59));
            techWormDistance2.UnlockFunctions = new int[] { 52 };
            techWormDistance2.UnlockValues = new double[] { 10000 };
            techWormDistance2.Level = 2;
            techWormDistance2.MaxLevel = 2;
            techWormDistance2.LevelCoef1 = 0;
            techWormDistance2.LevelCoef2 = 0;
            TechProto techWormDistance3 = ProtoRegistry.RegisterTech(4923, "相位干扰技术3", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level3_tech", new int[] { 4922 }, new int[] { 6001, 6002, 6003, 6004 },
                 new int[] { 12, 12, 12, 12 }, 360000, new int[] { }, new Vector2(17, -59));
            techWormDistance3.UnlockFunctions = new int[] { 52 };
            techWormDistance3.UnlockValues = new double[] { 10000 };
            techWormDistance3.Level = 3;
            techWormDistance3.MaxLevel = 3;
            techWormDistance3.LevelCoef1 = 0;
            techWormDistance3.LevelCoef2 = 0;
            TechProto techWormDistance4 = ProtoRegistry.RegisterTech(4924, "相位干扰技术4", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level4_tech", new int[] { 4923 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                 new int[] { 12, 12, 12, 12, 12 }, 360000, new int[] { }, new Vector2(21, -59));
            techWormDistance4.UnlockFunctions = new int[] { 52 };
            techWormDistance4.UnlockValues = new double[] { 10000 };
            techWormDistance4.Level = 4;
            techWormDistance4.MaxLevel = 4;
            techWormDistance4.LevelCoef1 = 0;
            techWormDistance4.LevelCoef2 = 0;
            TechProto techWormDistance5 = ProtoRegistry.RegisterTech(4925, "相位干扰技术5", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level5_tech", new int[] { 4924 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                 new int[] { 4, 4, 4, 4, 4 }, 1440000, new int[] { }, new Vector2(25, -59));
            techWormDistance5.UnlockFunctions = new int[] { 52 };
            techWormDistance5.UnlockValues = new double[] { 10000 };
            techWormDistance5.Level = 5;
            techWormDistance5.MaxLevel = 5;
            techWormDistance5.LevelCoef1 = 0;
            techWormDistance5.LevelCoef2 = 0;
            TechProto techWormDistanceInf = ProtoRegistry.RegisterTech(4926, "相位干扰技术6", "相位干扰技术描述", "相位干扰技术结论", "Assets/DSPBattle/signal-interference-level-infinitude_tech", new int[] { 4925 }, new int[] { 6006 },
                 new int[] { 4 }, 45000000, new int[] { }, new Vector2(29, -59));
            techWormDistanceInf.UnlockFunctions = new int[] { 52 };
            techWormDistanceInf.UnlockValues = new double[] { 10000 };
            techWormDistanceInf.Level = 6;
            techWormDistanceInf.MaxLevel = 60;
            techWormDistanceInf.LevelCoef1 = -18000000;
            techWormDistanceInf.LevelCoef2 = 1800000;


            TechProto dorpletControll1 = ProtoRegistry.RegisterTech(4927, "超距信号处理1", "超距信号处理描述", "超距信号处理结论", "Assets/DSPBattle/dropletcontroltech1", new int[] { }, new int[] { 6001, 6002, 6003, 6004 },
                 new int[] { 20, 20, 20, 20 }, 180000, new int[] { }, new Vector2(33, -51));
            dorpletControll1.PreTechsImplicit = new int[] { 1919 };
            dorpletControll1.UnlockFunctions = new int[] { 53 };
            dorpletControll1.UnlockValues = new double[] { 1 };
            dorpletControll1.Level = 1;
            dorpletControll1.MaxLevel = 1;
            dorpletControll1.LevelCoef1 = 0;
            dorpletControll1.LevelCoef2 = 0;
            TechProto dorpletControll2 = ProtoRegistry.RegisterTech(4928, "超距信号处理2", "超距信号处理描述", "超距信号处理结论", "Assets/DSPBattle/dropletcontroltech2", new int[] { 4927 }, new int[] { 6001, 6002, 6003, 6004, 6005 },
                  new int[] { 20, 20, 20, 20, 20 }, 360000, new int[] { }, new Vector2(37, -51));
            dorpletControll2.UnlockFunctions = new int[] { 53 };
            dorpletControll2.UnlockValues = new double[] { 1 };
            dorpletControll2.Level = 2;
            dorpletControll2.MaxLevel = 2;
            dorpletControll2.LevelCoef1 = 0;
            dorpletControll2.LevelCoef2 = 0;
            TechProto dorpletControll3 = ProtoRegistry.RegisterTech(4929, "超距信号处理3", "超距信号处理描述", "超距信号处理结论", "Assets/DSPBattle/dropletcontroltech3", new int[] { 4928 }, new int[] { 6006 },
                  new int[] { 20 }, 900000, new int[] { }, new Vector2(41, -51));
            dorpletControll3.UnlockFunctions = new int[] { 53 };
            dorpletControll3.UnlockValues = new double[] { 1 };
            dorpletControll3.Level = 3;
            dorpletControll3.MaxLevel = 3;
            dorpletControll3.LevelCoef1 = 0;
            dorpletControll3.LevelCoef2 = 0;


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
            SiloModel.prefabDesc.siloChargeFrame = 120;
            SiloModel.prefabDesc.siloColdFrame = 360;
            LDBTool.PreAddProto(SiloModel);

            var ShieldGenModel = CopyModelProto(45, 315, Color.blue);
            ShieldGenModel.prefabDesc.emptyId = 9999;
            ShieldGenModel.prefabDesc.fullId = 1208;
            ShieldGenModel.prefabDesc.exchangeEnergyPerTick = 1500000;
            ShieldGenModel.prefabDesc.workEnergyPerTick = 1500000;
            LDBTool.PreAddProto(ShieldGenModel);


            ModelProto TestEngineModel = CopyModelProto(68, 316, new Color(0,1,1,1));
            TestEngineModel.prefabDesc.idleEnergyPerTick = 500000; //卫星配电站只走idle耗电，数还需要改
            LDBTool.PreAddProto(TestEngineModel);


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

        public static void PostDataAction()
        {
            ItemProto.InitFluids();
            ItemProto.InitItemIds();
            ItemProto.InitFuelNeeds();
            ItemProto.InitItemIndices();
            ItemProto.InitMechaMaterials();

            foreach (var proto in LDB.items.dataArray)
            {
                if (proto.ID > 8000) // Keep compability with stack mod 
                {
                    StorageComponent.itemIsFuel[proto.ID] = proto.HeatValue > 0L;
                    StorageComponent.itemStackCount[proto.ID] = proto.StackSize;
                }
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

            LDB.models.Select(315).prefabDesc.modelIndex = 315;
            LDB.items.Select(8030).ModelIndex = 315;
            LDB.items.Select(8030).prefabDesc = LDB.models.Select(315).prefabDesc;

            LDB.models.Select(316).prefabDesc.modelIndex = 316;
            LDB.items.Select(8031).ModelIndex = 316;
            LDB.items.Select(8031).prefabDesc = LDB.models.Select(316).prefabDesc;

            // LDB.items.Select(2206).prefabDesc.ener
            LDB.items.Select(2206).prefabDesc.inputEnergyPerTick = 150000;
            LDB.items.Select(2206).prefabDesc.outputEnergyPerTick = 150000;
            LDB.items.Select(2206).prefabDesc.maxAcuEnergy = 540000000;
            LDB.items.Select(2207).prefabDesc.inputEnergyPerTick = 150000;
            LDB.items.Select(2207).prefabDesc.outputEnergyPerTick = 150000;
            LDB.items.Select(2207).prefabDesc.maxAcuEnergy = 540000000;

            if (Configs.enableProliferator4)
            {
                LDB.models.Select(120).prefabDesc.incItemId = new int[] { 1141, 1142, 1143, 8034 };
                LDB.techs.Select(1132).Position = new Vector2(29, -19);
                LDB.techs.Select(1416).Position = new Vector2(29, -15);
                LDB.techs.Select(1153).Position = new Vector2(33, -11);
            }

            //以下为星球发动机模型修改
            if (Configs.developerMode)
            {
                var prefab = LDB.items.Select(8031).prefabDesc;
                var oriPrefab = LDB.models.Select(68).prefabDesc;
                var originalMeshVertices = new Vector3[prefab.lodCount][];
                for (int i = 0; i < prefab.lodCount; i++)
                {
                    var vertices = prefab.lodMeshes[i].vertices;
                    originalMeshVertices[i] = new Vector3[vertices.Length];
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        originalMeshVertices[i][j] = vertices[j];
                    }
                }
                //碰撞
                prefab.colliders = new ColliderData[oriPrefab.colliders.Length];
                for (int i = 0; i < prefab.colliders.Length; i++)
                {
                    prefab.colliders[i] = oriPrefab.colliders[i];
                    prefab.colliders[i].ext.x *= 2f;
                    prefab.colliders[i].ext.y *= 3f;
                    prefab.colliders[i].ext.z *= 2f;
                }
                prefab.buildColliders = new ColliderData[oriPrefab.buildColliders.Length];
                for (int i = 0; i < prefab.buildColliders.Length; i++)
                {
                    prefab.buildColliders[i] = oriPrefab.buildColliders[i];
                    prefab.buildColliders[i].ext.x *= 2f;
                    prefab.buildColliders[i].ext.y *= 3f;
                    prefab.buildColliders[i].ext.z *= 2f;
                }
                prefab.buildCollider.ext.x *= 1f;
                prefab.buildCollider.ext.z *= 1f;
                //静态顶点（prebuild）
                for (int i = 0; i < prefab.lodCount; i++)
                {
                    var mesh = prefab.lodMeshes[i];
                    var oriVerts = originalMeshVertices[i];
                    var vertices = mesh.vertices;
                    for (int j = 0; j < oriVerts.Length; j++)
                    {
                        Vector3 vert = oriVerts[j];
                        vert.x *= 4;
                        vert.y *= 2f;
                        vert.z *= 4;
                        if(vert.y <=3)
                        {
                            vert.x *= 1.7f;
                            vert.z *= 1.7f;
                        }
                        else if(vert.y >= 4 && vert.y <= 6)
                        {
                            vert.x *= 2.2f;
                            vert.z *= 2.2f;
                        }
                        else if (vert.y >= 7)
                        {
                            vert.x *= 0.5f;
                            vert.z *= 0.5f;
                        }
                        vertices[j] = vert;
                    }
                    mesh.vertices = vertices;
                }
                //动画顶点
                for (int i = 0; i < 1; i++)
                {
                    List<int> centerHighP = new List<int>();
                    List<int> highP2 = new List<int>();
                    int loop = (int)prefab.lodVertas[i].vertexType;
                    if (loop == 0) loop = 12;
                    //Utils.Log($"lodV index {i} have dataLength{prefab.lodVertas[i].dataLength} whose type is {prefab.lodVertas[i].vertexType}\n frame count is {prefab.lodVertas[i].frameCount} and frameStride is {prefab.lodVertas[i].frameStride} vertex count is {prefab.lodVertas[i].vertexCount} and vertex size is {prefab.lodVertas[i].vertexSize}");
                    for (int j = 0; j < prefab.lodVertas[i].dataLength; j++)
                    {
                        if (j % loop == 2) //整体放大
                        {
                            prefab.lodVertas[i].data[j - 2] *= 4;
                            prefab.lodVertas[i].data[j - 1] *= 3;
                            prefab.lodVertas[i].data[j] *= 4;
                        }
                        if(j%loop == 2 && prefab.lodVertas[i].data[j - 1] <=3) //底座放大
                        {
                            prefab.lodVertas[i].data[j - 2] *= 1.75f;
                            prefab.lodVertas[i].data[j] *= 1.75f;
                        }
                        if (j % loop == 2 && prefab.lodVertas[i].data[j - 1] >= 6 && prefab.lodVertas[i].data[j - 1] <= 8) //中间不再收紧
                        {
                            prefab.lodVertas[i].data[j - 2] *= 2.2f;
                            prefab.lodVertas[i].data[j] *= 2.2f;
                        }
                        if (j % loop == 2 && j > 15318 * 60)//得到结果是j%15318>=4772的
                        {
                            if (prefab.lodVertas[i].data[j - 1] > 155 && !highP2.Contains(j % 15318))
                                highP2.Add(j % 15318);
                        }
                    }
                    for (int j = 0; j < prefab.lodVertas[i].dataLength; j++)
                    {
                        if (j % loop == 2 && highP2.Contains(j%15318))
                        {
                            prefab.lodVertas[i].data[j - 2] = 0f;
                            prefab.lodVertas[i].data[j - 1] =400;
                            prefab.lodVertas[i].data[j] = 0f;
                        }
                    }
                }
            }


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
                case 53:
                    Droplets.maxDroplet += 0; //(int)value; // 改成了每帧根据科技等级刷新max数。就不存档了。
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
                __result = "子弹伤害和导弹伤害+15%".Translate() + "\n" + "相位裂解光束伤害+30%".Translate();
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
            else if(__instance.ID >= 4927 && __instance.ID <= 4929)
                __result = "水滴控制上限".Translate() + "+1";
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
            
            if (infoLabel.text.Split('\n').Length < 38)
            {
                //infoLabel.text = infoLabel.text + "\r\n\r\n" + "子弹伤害".Translate() + "\r\n" + "相位裂解光束伤害".Translate() + "\r\n"
                //    + "导弹伤害".Translate() + "\r\n" + "子弹飞行速度".Translate() + "\r\n" + "导弹飞行速度".Translate() + "\r\n" + "虫洞干扰半径".Translate() + "\r\n" + "水滴控制上限".Translate();
                infoLabel.text = infoLabel.text + "\r\n\r\n" + "子弹相位伤害".Translate() + "\r\n" +
                    "子弹导弹速度".Translate() + "\r\n" + "虫洞干扰半径".Translate() + "\r\n" + "水滴控制上限".Translate();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UITechTree), "RefreshDataValueText")]
        public static void RefreshDataValueTextPatch(ref UITechTree __instance)
        {
            if (Configs.extraSpeedEnabled)
            {
                string[] txt = __instance.dataValueText.text.Split('\n');
                string final = "";
                for (int i = 0; i < txt.Length - 1; i++)
                {
                    if (i == 25 || i == 27 || i == 28 || (i == 30 && Rank.rank >= 3) || i == 31 || i == 32)
                    {
                        final += "<color=#61d8ffb4>" + "额外奖励gm".Translate() + txt[i].Trim() + "</color>\r\n";
                    }
                    else
                    {
                        final += txt[i].Trim() + "\r\n";
                    }
                }
                final += txt[txt.Length - 1];
                __instance.dataValueText.text = final;
            }
            /*
            __instance.dataValueText.text = __instance.dataValueText.text + "\r\n\r\n" + Configs.bulletAtkScale.ToString("0%") + "\r\n"
                + (1 + (Configs.bulletAtkScale - 1) * 2).ToString("0%") + "\r\n" + Configs.bulletAtkScale.ToString("0%")
                + "\r\n" + Configs.bulletSpeedScale.ToString("0%") + "\r\n" + (1 + (Configs.bulletSpeedScale - 1) * 0.5).ToString("0%") + "\r\n" + (Configs.wormholeRange / 40000.0).ToString() + "AU" + "\r\n"
                + Droplets.maxDroplet.ToString();
            */
            __instance.dataValueText.text = __instance.dataValueText.text + "\r\n\r\n" + Configs.bulletAtkScale.ToString("0%") + " / "
                + (1 + (Configs.bulletAtkScale - 1) * 2).ToString("0%")
                + "\r\n" + Configs.bulletSpeedScale.ToString("0%") + " / " + (1 + (Configs.bulletSpeedScale - 1) * 0.5).ToString("0%")
                + "\r\n" + (Configs.wormholeRange / 40000.0).ToString() + "AU" + "\r\n"
                + Droplets.maxDroplet.ToString();
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
                case 54:
                    __result = "水滴发射耗能".Translate();
                    return;
                case 55:
                    __result = "水滴工作功率".Translate();
                    return;
                case 56:
                    __result = "最小发射能量".Translate();
                    return;
                case 57:
                    __result = "护盾容量".Translate();
                    return;
                case 58:
                    __result = "护盾恢复".Translate();
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
                case 54:
                    __result = Utils.KMGFormat(Droplets.energyComsumptionPerLaunch) + "J";
                    return;
                case 55:
                    __result = Utils.KMGFormat(Droplets.energyComsumptionPerTick * 60) + "W";
                    return;
                case 56:
                    __result = Utils.KMGFormat(Droplets.energyComsumptionPerLaunch * 2) + "J";
                    return;
                case 57:
                    __result = "0 - " + Configs.capacityPerGenerator[0].Item2.ToString();
                    return;
                case 58:
                    __result = (Configs.shieldGenPerTick * 60).ToString() + "/s";
                    return;
            }
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(LabMatrixEffect), "Update")]
        public static bool LabMatrixEffectUpdatePatch(ref LabMatrixEffect __instance)
        {
            __instance.time += (double)Time.deltaTime;
            double num = __instance.time / 1000.0;
            float num2 = (float)((num - Math.Floor(num)) * 1000.0);
            Vector3 localEulerAngles = new Vector3(num2 * 37f, num2 * 101f, num2 * 23f);
            __instance.matrixCube.localEulerAngles = localEulerAngles;
            int currentTech = __instance.history.currentTech;
            TechProto techProto = LDB.techs.Select(currentTech);
            bool flag = false;
            if (currentTech > 0 && techProto != null && techProto.IsLabTech && GameMain.history.techStates.ContainsKey(currentTech))
            {
                TechState techState = __instance.history.techStates[currentTech];
                flag = true;
            }
            if (!flag)
            {
            }
            __instance.techGroup.gameObject.SetActive(flag);
            Array.Clear(__instance.techMatUse, 0, __instance.techMatUse.Length);
            if (flag)
            {
                for (int i = 0; i < techProto.Items.Length; i++)
                {
                    int num3 = techProto.Items[i] - LabComponent.matrixIds[0];
                    if (num3 < 0 || num3 > __instance.techMatUse.Length) continue;
                    __instance.techMatUse[num3] = true;
                }
            }
            for (int j = 0; j < __instance.techCubes.Length; j++)
            {
                __instance.techCubes[j].gameObject.SetActive(__instance.techMatUse[j]);
            }
            int num4 = 0;
            for (int k = 0; k < __instance.techCubes.Length; k++)
            {
                if (__instance.techMatUse[k])
                {
                    num4++;
                }
            }
            int num5 = 0;
            for (int l = 0; l < __instance.techCubes.Length; l++)
            {
                if (__instance.techCubes[l].gameObject.activeSelf)
                {
                    float num6 = 360f * (float)num5 / (float)num4;
                    float f = (num2 * 47f + num6) * 0.017453292f;
                    float f2 = num2 * 121f * 0.017453292f;
                    Vector3 localPosition = new Vector3(Mathf.Cos(f) * 0.7f, Mathf.Sin(f) * 0.7f, Mathf.Sin(f2) * 0.5f);
                    __instance.techCubes[l].localPosition = localPosition;
                    Vector3 localEulerAngles2 = new Vector3(num2 * 37f, num2 * 101f + num6, num2 * 23f);
                    __instance.techCubes[l].localEulerAngles = localEulerAngles2;
                    num5++;
                }
            }

            return false;
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
                case 8014:
                    return new int[] { Configs.bullet4Atk, Mathf.RoundToInt((float)Configs.bullet4Speed), 0 };
                case 9511: //水滴
                    return new int[] { Configs.dropletAtk, Mathf.RoundToInt((float)Configs.dropletSpd), 0 };
                default:
                    return new int[] { 0, 0, 0 };
            }
        }

        public static void ChangeTabName(Proto proto)
        {
            if(proto is StringProto && proto.name == "MegaStructures")
            {
                var item = proto as StringProto;
                item.ZHCN = "轨道防御";
                item.ENUS = "Defense";
                item.FRFR = "Defense";
            }
        }

        public static void ReCheckTechUnlockRecipes()
        {
            if(!GameMain.history.TechState(1920).unlocked && GameMain.history.RecipeUnlocked(538))
            {
                GameMain.history.recipeUnlocked.Remove(538);
            }
            if (!GameMain.history.TechState(1921).unlocked && GameMain.history.RecipeUnlocked(540))
            {
                GameMain.history.recipeUnlocked.Remove(540);
            }
            if (!GameMain.history.TechState(1922).unlocked)
            {
                if (GameMain.history.RecipeUnlocked(537))
                    GameMain.history.recipeUnlocked.Remove(537);
                if (GameMain.history.RecipeUnlocked(541))
                    GameMain.history.recipeUnlocked.Remove(541);
            }
            if (!GameMain.history.TechState(1923).unlocked && GameMain.history.RecipeUnlocked(542))
            {
                GameMain.history.recipeUnlocked.Remove(542);
            }
            if (!GameMain.history.TechState(1924).unlocked && GameMain.history.RecipeUnlocked(539))
            {
                GameMain.history.recipeUnlocked.Remove(539);
            }
        }


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(TechProto), "GenPropertyOverrideItems")]
        //public static bool TechGenOverrideItemsPatch(ref TechProto __instance, TechProto proto)
        //{
        //    if (!Configs.developerMode) return true;
        //    if (proto.PropertyOverrideItems != null) return true;
        //    __instance.PropertyItemCounts = new int[] { 1 };
        //    __instance.PropertyOverrideItems = new int[] { 6001 };
        //    __instance.PropertyOverrideItemArray = new IDCNT[1];
        //    __instance.PropertyOverrideItemArray[0] = new IDCNT(6001, 1);
        //    return false;
        //}


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UITechNode), "OnBuyoutButtonClick")]
        public static bool BuyoutTechPrePatch(ref UITechNode __instance, int _data)
        {
            TechProto tech = __instance.techProto;
            if (tech == null || tech.Items == null) return true;
            for (int i = 0; i < tech.Items.Length; i++)
            {
                if(tech.Items[i] >= 6003 && tech.Items[i] <= 6006 || tech.Items[i] == 8032 || tech.ID == 1901)
                {
                    UIRealtimeTip.Popup("被深空来敌mod禁止".Translate(), true, 0);
                    return false;
                }
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UITechNode), "UpdateInfoDynamic")]
        public static void UITechNodeUpdateInfoDynamicPostPatch(ref UITechNode __instance)
        {
            TechProto tech = __instance.techProto;
            if (tech == null || tech.Items == null) return;
            for (int i = 0; i < tech.Items.Length; i++)
            {
                if (tech.Items[i] >= 6003 && tech.Items[i] <= 6006 || tech.Items[i] == 8032 || tech.ID == 1901)
                {
                    __instance.buyoutButton.transitions[0].normalColor = __instance.buyoutNormalColor1;
                    __instance.buyoutButton.transitions[0].mouseoverColor = __instance.buyoutMouseOverColor1;
                    __instance.buyoutButton.transitions[0].pressedColor = __instance.buyoutPressedColor1;
                    return;
                }
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UISpraycoaterWindow), "RefreshSpraycoaterWindow")]
        public static void RefreshSpraycoaterWindowPostPatch(ref UISpraycoaterWindow __instance, SpraycoaterComponent spraycoater)
        {
            if (Configs.enableProliferator4 && spraycoater.incItemId != 0 && spraycoater.incCount > 0)
            {
                ItemProto itemProto = LDB.items.Select(spraycoater.incItemId);
                if (itemProto.Ability >= 6)
                {
                    Color color = new Color(0, 0.5f, 0.6f);
                    __instance.tankFillOutlineImage.color = new Color(color.r, color.g, color.b, color.a * 4f);
                    __instance.tankCountText.color = new Color(color.r, color.g, color.b, color.a * 4f);
                    __instance.tankFillMaskImage.color = color;
                    __instance.incInfoText1.color = new Color(color.r, color.g, color.b, color.a * 4f);
                    __instance.incInfoText2.color = new Color(color.r, color.g, color.b, color.a * 4f);
                    __instance.incInfoText3.color = new Color(color.r, color.g, color.b, color.a * 4f);
                }

            }
        }
    }
}
