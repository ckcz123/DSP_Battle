using CommonAPI.Systems;
using CommonAPI.Systems.ModLocalization;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using xiaoye97;

namespace DSP_Battle
{
    class BattleProtos
    {
        public static int pageBias = 0;

        static void RegisterString(string key, string enTrans, string cnTrans)
        {
            LocalizationModule.RegisterTranslation(key, enTrans, cnTrans, enTrans);
        }

        public static void AddProtos()
        {
            AddTutorialProtos();
            AddNormalProtos();
        }

        public static void AddNormalProtos()
        {
            int recipeIdBias = MoreMegaStructure.MoreMegaStructure.GenesisCompatibility ? -200 : 0;
            int techPosXBias = MoreMegaStructure.MoreMegaStructure.GenesisCompatibility ? -8 : 0;
            TechProto techDrop = ProtoRegistry.RegisterTech(1999, "玻色子操控", "玻色子操控描述", "玻色子操控结论", "Assets/DSPBattle/bosontech", new int[] { }, new int[] { 5201 },
                new int[] { 400 }, 72000, new int[] { 573 + recipeIdBias, 567 + recipeIdBias, 568 + recipeIdBias, 569 + recipeIdBias, 566 + recipeIdBias }, new Vector2(65 + techPosXBias, -7));
            techDrop.PreTechsImplicit = new int[] { 1823 };
            techDrop.IsHiddenTech = true;
            techDrop.PreItem = new int[] { 5201 };

            TechProto techExp = ProtoRegistry.RegisterTech(1998, "提供算力名称", "提供算力描述", "提供算力结论", "Assets/DSPBattle/rank10", new int[] { }, new int[] { 6006 },
                new int[] { 2 }, 2997924580, new int[] { }, new Vector2(65 + techPosXBias, -11)); // 299792458000
            techExp.PreTechsImplicit = new int[] { };
            techExp.IsHiddenTech = true;
            techExp.PreItem = new int[] { 9513 };


            ItemProto medal = ProtoRegistry.RegisterItem(9513, "星河卫士奖章", "星河卫士奖章描述", "Assets/DSPBattle/rank10", 9998, 1, EItemType.Decoration,
                                       ProtoRegistry.GetDefaultIconDesc(Color.white, new Color(0.7f, 0.4f, 0.1f)));
            medal.UnlockKey = -2;
            ProtoRegistry.RegisterRecipe(382, ERecipeType.Smelt, 60, new int[] { 1104 }, new int[] { 2 }, new int[] { 9513 }, new int[] { 1 }, "星河卫视奖章描述", 0, 9998, "Assets/DSPBattle/rank10");

            // Void-Recipes
            //ProtoRegistry.RegisterItem()
        }
        public static void AddTranslate()
        {
            //官方星图界面 橙色高亮字体color=FD965EC0
            //增产效果的蓝色高亮字体 0.3821 0.8455 1 0.7059 : 61d8ffb4

            // 文本翻译
            RegisterString("轨道防御", "TCFV", "深空来敌");

            //RegisterString("子弹1", "Armor piercing bullet", "穿甲磁轨弹");
            //RegisterString("子弹1描述", "The pioneers who had never seen an enemy before flipped the ancestors' database 114514 times to patch together an armor-piercing bullet design. Since they couldn't find a gunpowder formula, they had to cast it into a solid cannonball and strengthen its structure. It only had single kinetic attack and limited lethality.", "从来没见过敌人的先驱者把老祖宗的数据库翻了114514遍，东拼西凑出来一个穿甲弹设计图。由于没找到火药配方，只好照着炮弹的样子铸造成实心炮弹并加强其结构。仅能进行单体动能打击，杀伤力有限。");
            //RegisterString("子弹1结论", "You have unlocked armor piercin bullet and can use kinetic energy for attack.", "你解锁了穿甲磁轨弹，可以利用动能进行攻击");
            //RegisterString("子弹2", "Acid bullet", "强酸磁轨弹");
            //RegisterString("子弹2描述", "After analyzing and studying the enemy, the pioneers boldly packaged <color=#c2853d>sulfuric acid</color> into a new cannonball. The explosion can produce corrosion on enemies, killing them more efficiently.", "在对敌人进行分析和研究后，先驱者大胆将<color=#c2853d>硫酸</color>封装后制成了一种新的炮弹。爆破后可以对范围内敌人产生酸蚀，更加高效地杀伤敌人。");
            //RegisterString("子弹2结论", "You have unlocked acid bullet and can use sulfuric acid to corrode the shell.", "你解锁了强酸磁轨弹，可以利用硫酸腐蚀外壳");
            //RegisterString("子弹3", "Deuterium nucleus bullet", "氘核爆破弹");
            //RegisterString("子弹3短", "D-nucleus", "氘核爆破弹");
            //RegisterString("子弹3描述", "The pioneers who created the <color=#c2853d>thermonuclear missile</color> wanted to make it smaller and faster, so they tried to miniaturize and package the nuclear bomb into a bullet. This weapon can cause a large number of injuries to enemies after hitting by undergoing fusion explosion.", "制造出<color=#c2853d>热核导弹</color>的先驱者想要让他更小更快，便试着把核弹微缩化封装进了子弹里。该武器可以在命中后发生聚变爆炸，对敌人造成大量伤害。");
            //RegisterString("子弹3结论", "You have unlocked deuterium nucleus and can use nuclear fusion to cause destruction.", "你解锁了氘核爆破弹，可以利用核聚变进行破坏");
            //RegisterString("脉冲", "Phase-cracking beam", "相位裂解光束");
            //RegisterString("脉冲短", "Cracking beam", "相位裂解光束");
            //RegisterString("脉冲描述", "This beam does not require production, it only needs enough electricity to be supplied to the <color=#c2853d>Phaser emitter</color> to fire unlimitedly.", "这种光束是不需要制作和提供的，只需给<color=#c2853d>相位裂解炮</color>提供足够电量即可无限发射。");

            //RegisterString("导弹1", "Thermonuclear missile", "热核导弹");
            //RegisterString("导弹1短", "Nuclear missile", "热核导弹");
            //RegisterString("导弹1描述", "One day, the bored pioneers were browsing Youtube and suddenly came across a video on \"How to Make a Nuke in Your Bedroom\", and so the missile was created. This is a heavyweight weapon that can cause a nuclear explosion when it reaches its target, causing massive damage.", "这天，百无聊赖的先驱者正在刷哔哩哔哩，突然刷到了一条《如何在卧室制造核弹》的视频，于是这种导弹便被制造了出来。这是一种重型武器，发射升空并命中敌人后产生核爆，造成大范围伤害。");
            //RegisterString("导弹1结论", "You have unlocked thermonuclear missile and it can automatically track down enemies.", "你解锁了热核导弹，可以自动追踪敌人");

            //RegisterString("导弹2", "Antimatter missile", "反物质导弹");
            //RegisterString("导弹2短", "A-M missile", "反物质导弹");
            //RegisterString("导弹2描述", "Accidentally, the pioneers spilled some <color=#c2853d>antimatter</color> while transporting goods, which destroyed their hard-made production line. \"Why not let the enemy experience the same misery?\" he thought. And so this weapon was created, packaging antimatter into a missile to annihilate enemies upon impact.", "先驱者在一次搬运货物时不慎手滑造成微量<color=#c2853d>反物质</color>泄露，这导致他辛辛苦苦拉好的产线毁于一旦。“为什么不让敌人尝尝这种痛苦呢？”他想到。于是这种将<color=#c2853d>反物质</color>封装入导弹的武器被制造了出来，命中敌人后会发生湮灭，将敌人彻底抹杀。");
            //RegisterString("导弹2结论", "You have unlocked antimatter missile and it can automatically track down enemies.", "你解锁了反物质导弹，可以自动追踪敌人");

            //RegisterString("导弹3", "Gravitational collapse missile", "引力塌陷导弹");
            //RegisterString("导弹3短", "Grav-missile", "引力塌陷导弹");
            //RegisterString("导弹3描述", "Inventors of the <color=#c2853d>Gravitation Slingshot ejector</color> worked hard to package a mini black hole into the missile, creating this super weapon. It can temporarily generate a mini black hole after the explosion to gather enemies in range and wipe them out efficiently.", "发明出<color=#c2853d>引力弹射炮</color>的先驱者一鼓作气，将微型黑洞封装进导弹，制成了这种超级武器。它能在爆炸后短暂生成一个微型黑洞将范围内的敌人聚拢，简单高效。");
            //RegisterString("导弹3结论", "You have unlocked gravitational collapse missile and it can automatically track down enemies.", "你解锁了引力塌陷导弹，可以自动追踪敌人");

            //RegisterString("弹射器1", "Super railgun", "电磁轨道炮");
            //RegisterString("弹射器1描述", "The pioneers of creating <color=#c2853d>Armor piercing bullet</color> didn't know how to launch them until an accidental mistake set <color=#c2853d>solar sails</color> as ammunition. \"Perfect, now I have 'created' a weapon,\" the pioneers thought, \"But how can I distinguish them?\" Soon after, with a flash of inspiration, the pioneers added an orange atmosphere light to it. This weapon can be loaded with any type of bullets, however its range of attack is limited by the angle of elevation.", "制造出<color=#c2853d>穿甲磁轨弹</color>的先驱者不知道该如何把他们扔上天，直到他不小心把<color=#c2853d>太阳帆</color>误装成了炮弹。“好极了，现在我‘创造’了一种武器。”先驱者这样想着，“但是怎么区分他们呢？”。随后，先驱者默念着：“你指尖闪动的电光，是我此生不变的信仰……”给它装上了橙色氛围灯。该武器可以装入任何类型的子弹，且打击范围受到仰角限制。");
            //RegisterString("弹射器1结论", "You have unlocked super railgun and it can fire bullets to attack enemies.", "你解锁了电磁轨道炮，可以发射子弹攻击敌方");

            //RegisterString("弹射器2", "Gravitation Slingshot ejector", "引力弹射炮");
            //RegisterString("弹射器2描述", "After visiting a few times to the black hole, the pioneers gained a deeper understanding of gravity manipulation. They decided to develop their own weapon and so they dismantled the acceleration magnet of <color=#c2853d>Super railgun</color> and created a generator that could produce a gravity ejector using tiny blackholes. This allowed the ejector to fire bullets with much more kinetic energy than before. \"This time it's not just about changing the atmosphere light!\" The pioneers rejoiced. The weapon can fire any type of bullet with a much higher speed.", "在黑洞杀了个七进七出之后，先驱者对引力操控有了深入见解，他终于决定自己开发一种武器。于是他拆除了<color=#c2853d>电磁轨道炮</color>的加速磁场，制造了一种利用微型黑洞制造引力弹弓的发生器，这使得发射炮弹获得了数倍于之前的动能。“这回可不是换个氛围灯那么简单了！”先驱者自我陶醉着。该武器能以更高的射速发射任何类型的子弹。");
            //RegisterString("弹射器2结论", "You have unlocked Gravitation Slingshot ejector  and it can fire bullets to attack enemies.", "你解锁了引力弹射炮，可以发射子弹攻击敌方");

            //RegisterString("脉冲炮", "Phaser emitter", "相位裂解炮");
            //RegisterString("脉冲炮描述", "The pioneers who developed the super weapons were bored and tried to explore the universe, but their ambition was always hindered by a lack of ammunition. In a fit of rage, with inspiration from Stellaris they developed this weapon. Using high-energy particle streams to create a phase disruption chain reaction, it can kill enemies in a large area. The biggest advantage is, <color=#c2853d>Ammunition not required</color>! However, the damage from this weapon will attenuate when the distance increases.", "开发出超级武器的先驱者百般无聊，整日靠着当P社战犯度日，但是他灭绝寰宇的大业总是因为弹药短缺被打断，于是他一气之下依照《群星》的舰载武器开发了这种武器。利用发射的高能粒子流产生相位裂解链式反应，大范围杀伤敌人，最大的优点是，<color=#c2853d>无需弹药</color>！从此，先驱者在成为战犯的道路上越走越远。注意，此光束伤害将随着距离而衰减。");
            //RegisterString("脉冲炮结论", "You have unlocked Phaser emitter and can now attack the enemy using only electricity.", "你解锁了相位裂解炮，可以仅使用电力攻击敌方");

            //RegisterString("发射器1", "Void missile launching silo", "深空导弹发射井");
            //RegisterString("发射器1描述", "The pioneers who created the <color=#c2853d>thermonuclear missiles</color> didn't know how to get them into the space until they saw the <color=#c2853d>Vertical launching silo</color>. \"Can't I just paint them red and have invented a weapon?\" The pioneer thought. This weapon can attack enemies from all directions.", "制造出<color=#c2853d>热核导弹</color>的先驱者并不知道如何把他们扔上天去，直到他看到了<color=#c2853d>垂直发射井</color>。“只要把它刷成红色我不就发明了一种武器吗？”先驱者这样想到。该武器可以对敌人进行全方位打击。");
            //RegisterString("发射器1结论", "You have unlocked void missile launching silo.", "你解锁了深空导弹发射井，可以发射导弹攻击敌方");

            //RegisterString("近地防卫系统", "Near Earth Def-system", "近地防卫系统");
            //RegisterString("近地防卫系统描述", "Manufacturing <color=#c2853d>Super railgun</color> to eject <color=#c2853d>Armor piercing</color> to bulid basal defensive system.", "制造<color=#c2853d>电磁轨道炮</color>发射<color=#c2853d>穿甲磁轨弹</color>进行基础防御。");
            //RegisterString("近地防卫系统结论", "You have unlocked Near Earth Def-system.", "你解锁了近地防卫系统");
            //RegisterString("深空防卫系统", "Deep Space Def-system", "深空防卫系统");
            //RegisterString("深空防卫系统描述", "Manufacturing <color=#c2853d>Void missile launching silo</color> to deploy <color=#c2853d>Thermonuclear missile</color> to bulid broader strike, filling the gap of near-earth-defense.", "制造<color=#c2853d>深空导弹发射井</color>部署<color=#c2853d>热核导弹</color>实现更大范围覆盖打击，填补近地防卫的空白。");
            //RegisterString("深空防卫系统结论", "You have unlocked Deep Space Def-system.", "你解锁了深空防卫系统");
            //RegisterString("引力操控技术", "Gravitation control", "引力操控技术");
            //RegisterString("引力操控技术描述", "Manufacturing <color=#c2853d>Gravitation Slingshot ejector</color> to level up near earth defense and <color=#c2853d>Gravitational collapse missile</color> to further strengthen deep space defense.", "制造<color=#c2853d>引力弹射炮</color>升级近地防卫系统，生产<color=#c2853d>引力塌陷导弹</color>进一步加强深空防御。");
            //RegisterString("引力操控技术结论", "You have unlocked Gravitation control.", "你解锁了引力操控技术");
            //RegisterString("相位裂解技术", "Phaser disintegration technology", "相位裂解技术");
            //RegisterString("相位裂解技术描述", "Manufacturing super weapon <color=#c2853d>Phaser emitter</color> to build ultimate near-earth-defense.", "这波<color=#c2853d>相位裂解炮</color>来全杀了。");
            //RegisterString("相位裂解技术结论", "You have unlocked Phaser disintegration technology.", "你解锁了相位裂解技术");

            //RegisterString("子弹2tech描述", "Manufacturing <color=#c2853d>Acid bullet</color> to strengthen near earth defensive system.", "制造<color=#c2853d>强酸磁轨弹</color>加强近地防御力。");
            //RegisterString("子弹3tech描述", "Manufacturing <color=#c2853d>Deuterium nucleus bullet</color> to further strengthen near earth defensive system.", "制造<color=#c2853d>氘核爆破弹</color>进一步加强近地防御力。");
            //RegisterString("导弹2tech描述", "Manufacturing <color=#c2853d>Antimatter missile</color> to strengthen deep space defensive system.", "制造<color=#c2853d>反物质导弹</color>加强深空防御力。");

            //RegisterString("定向爆破1", "Directional blasting", "定向爆破");
            //RegisterString("定向爆破2", "Directional blasting", "定向爆破");
            //RegisterString("定向爆破3", "Directional blasting", "定向爆破");
            //RegisterString("定向爆破4", "Directional blasting", "定向爆破");
            //RegisterString("定向爆破5", "Directional blasting", "定向爆破");
            //RegisterString("定向爆破6", "Directional blasting", "定向爆破");
            //RegisterString("定向爆破描述", "By precisely calculating the trajectory of bullets and missiles to predict the optimal detonation point before impact, so as to cause as much destruction to the enemy as possible.", "通过精确计算子弹和导弹的索敌路径，预测撞击前的最佳起爆点，以尽可能对敌人造成更大的破坏。");
            //RegisterString("定向爆破结论", "Increase damage for bullets and missiles.", "子弹、导弹伤害增加");
            //RegisterString("子弹伤害和导弹伤害+15%", "Damage of bullets and missiles +15%", "子弹伤害和导弹伤害+15%");
            //RegisterString("相位裂解光束伤害+30%", "Damage of Phaser-emitter beam +30%", "相位裂解光束伤害+30%");

            //RegisterString("引力波引导1", "Gravitational wave guidance", "引力波引导");
            //RegisterString("引力波引导2", "Gravitational wave guidance", "引力波引导");
            //RegisterString("引力波引导3", "Gravitational wave guidance", "引力波引导");
            //RegisterString("引力波引导4", "Gravitational wave guidance", "引力波引导");
            //RegisterString("引力波引导5", "Gravitational wave guidance", "引力波引导");
            //RegisterString("引力波引导6", "Gravitational wave guidance", "引力波引导");
            //RegisterString("引力波引导描述", "Utilizing gravitational waves to increase the speed of the bullet, enabling it to reach the enemy quicker.", "利用引力波提升子弹的飞行速度，使子弹能够更快地打到敌人。");
            //RegisterString("引力波引导结论", "Increase speed for bullets and missiles.", "子弹、导弹弹道速度增加");
            //RegisterString("子弹飞行速度+10%", "Bullet speed +10%", "子弹飞行速度+10%");
            //RegisterString("导弹飞行速度+5%", "Missile speed +5%", "导弹飞行速度+5%");

            //RegisterString("相位干扰技术1", "Phase disturbance", "相位干扰技术");
            //RegisterString("相位干扰技术2", "Phase disturbance", "相位干扰技术");
            //RegisterString("相位干扰技术3", "Phase disturbance", "相位干扰技术");
            //RegisterString("相位干扰技术4", "Phase disturbance", "相位干扰技术");
            //RegisterString("相位干扰技术5", "Phase disturbance", "相位干扰技术");
            //RegisterString("相位干扰技术6", "Phase disturbance", "相位干扰技术");
            //RegisterString("相位干扰技术描述", "Increase enemy flight distance by using space interference to make wormhole spawn further.", "通过空间干扰让虫洞刷新的更远，增加敌人的飞行距离。");
            //RegisterString("相位干扰技术结论", "Expand wormhole spawns radius.", "虫洞生成距离增加");
            //RegisterString("虫洞生成最近范围扩大0.25AU", "Wormhole spawns radius expanded by 0.25AU", "虫洞生成最近范围扩大0.25AU");

            //RegisterString("彩蛋1", "Pioneer Diary #1", "先驱者日记#1");
            //RegisterString("彩蛋2", "Pioneer Diary #2", "先驱者日记#2");
            //RegisterString("彩蛋3", "Pioneer Diary #3", "先驱者日记#3");
            //RegisterString("彩蛋4", "Pioneer Diary #4", "先驱者日记#4");
            //RegisterString("彩蛋5", "Pioneer Diary #5", "先驱者日记#5");
            //RegisterString("彩蛋6", "Pioneer Diary #6", "先驱者日记#6");
            //RegisterString("彩蛋7", "Pioneer Diary #7", "先驱者日记#7");
            //RegisterString("彩蛋8", "Pioneer Diary #8", "先驱者日记#8");
            //RegisterString("彩蛋9", "Pioneer Diary #9", "先驱者日记#9");

            //RegisterString("行星力场护盾", "Planet shield", "行星力场护盾");
            RegisterString("恒星炮gm2", "Star cannon", "恒星炮");
            RegisterString("水滴gm2", "Droplet", "水滴");
            RegisterString("即将到来gm", "Coming soon", "即将推出");
            RegisterString("恒星要塞", "Star Fortress", "恒星要塞");
            RegisterString("恒星要塞描述", "Plan additional defense modules on any mega structure to strengthen the defense capabilities of the star system, and the energy to support the star fortress comes entirely from the stars. This can also save a significant amount of planetary area.", "在任何巨构上规划额外的防御模块来强化星系的防御能力，支持恒星要塞的能量完全来自于恒星。这还能节约大量地表的可建造面积。");
            RegisterString("恒星要塞结论", "You have unlocked the star fortress.", "你解锁了武装巨构来构建恒星要塞的能力。");
            //RegisterString("尼科尔戴森光束", "Nicoll-Dyson beam", "尼科尔-戴森光束");
            //RegisterString("尼科尔戴森光束描述", "Decoding a method for building a Star Cannon from alien matrices, disintegrating wormholes with stellar energy.", "从异星矩阵中解码建造恒星炮的方法，利用恒星级能量瓦解虫洞。");
            //RegisterString("尼科尔戴森光束结论", "You have unlocked the star cannon.", "你解锁了建造恒星炮的能力。");
            //RegisterString("行星力场护盾", "Planetary force field shield", "行星力场护盾");
            //RegisterString("行星力场护盾描述", "Maintain force field shields around planets.", "在行星周围构建力场护盾。");
            //RegisterString("行星力场护盾结论", "You have unlocked the planet shield generator.", "你解锁了行星护盾生成器。");
            RegisterString("玻色子操控", "Boson control", "玻色子操控");
            RegisterString("玻色子操控描述", "Create powerful materials by manipulating various interacting forces.", "通过操控各种相互作用力来制造强大的材料。");
            RegisterString("玻色子操控结论", "You have unlocked the Boson control.", "你解锁了玻色子操控。");
            RegisterString("水滴科技描述", "A powerful Droplet controlled by the mecha.", "一个由机甲控制的强大水滴");
            RegisterString("水滴科技结论", "You have unlocked the Droplet.", "你解锁了水滴技术。");
            //RegisterString("超距信号处理1", "Stellar-range signal processing", "超距信号处理");
            //RegisterString("超距信号处理2", "Stellar-range signal processing", "超距信号处理");
            //RegisterString("超距信号处理3", "Stellar-range signal processing", "超距信号处理");
            //RegisterString("超距信号处理描述", "Enhance real-time control of droplets.", "强化对水滴的实时控制能力。");
            //RegisterString("超距信号处理结论", "Drop control limit is increased.", "可同时操控的水滴上限提升。");
            //RegisterString("水滴控制上限", "Droplet control limit", "水滴控制上限");
            //RegisterString("伤害衰减", "Damage attenuate", "伤害衰减");

            //RegisterString("恒星炮未规划", "Star Cannon Unplanned", "恒星炮未规划");
            //RegisterString("恒星炮建设中", "Building In Progress", "恒星炮建设中");
            //RegisterString("恒星炮冷却中", "Cooling Down", "恒星炮冷却中");
            //RegisterString("恒星炮充能中", "Charging", "恒星炮充能中");
            //RegisterString("恒星炮开火", "Fire!", "恒星炮开火");
            //RegisterString("瞄准中", "Aiming", "瞄准中");
            //RegisterString("预热中", "Preheating", "预热中");
            //RegisterString("正在开火", "Firing", "正在开火");
            //RegisterString("没有规划的恒星炮", "No planned star cannon!", "没有规划的恒星炮");
            //RegisterString("恒星炮需要至少修建至第一阶段才能够开火！", "You have to finish at least stage 1 to fire star cannon!", "恒星炮需要至少修建至第一阶段才能够开火！");
            //RegisterString("恒星炮已经启动", "Star cannon has already launched!", "恒星炮已经启动。");
            //RegisterString("恒星炮冷却中！", "Star cannon is still cooling down!", "恒星炮冷却中！");
            //RegisterString("恒星炮充能中！", "Star cannon is still charging!", "恒星炮充能中！");
            //RegisterString("没有目标！", "No target!", "没有目标！");
            //RegisterString("超出射程！", "Out of range!", "超出射程！");
            //RegisterString("虫洞已完全稳定，无法被摧毁", "The wormholes are fully stabilized and cannot be destroyed.", "虫洞已完全稳定，无法被摧毁。");
            //RegisterString("恒星炮已启动", "Launching.", "恒星炮已启动。");


            //RegisterString("彩蛋1描述", "Seems like these enemies are a kind of space insect. They live in deep space, feed on the tides of power without vision. " +
            //    "The communication between interstellar logistic stations create regular ripples in space, making itself a beacon in a dark universe that can easily be captured and attacked by the insects. " +
            //    "In other words, <color=#c2853d>they will randomly choose an interstellar logistic station, and attack its star system via wormholes. Stars with more interstellar logistic stations will have a higher probability of being selected, and stars without any interstellar logistic stations won't be attacked. </color>" +
            //    "Obviously, I can't give up my interstellar logistic stations - I can transport Titanium manually but not everything right? Instead, I have to find a way to defend. Hope I still have time to think about 'The Answer to Life, the Universe, and Everything'.",
            //    "这种生物似乎是一种虫类，靠进食宇宙中的各种能量潮汐为生，同时还具有虫洞制造能力，一旦发现食物就会直接在附近建立虫洞。" +
            //    "星际物流塔产生的能量潮汐让它变成了宇宙中的一座灯塔，能被这些生物轻易捕捉并视作食物来源。" +
            //    "换言之，<color=#c2853d>它们会随机选择全星区的任意一个星际物流塔，通过虫洞进攻该星系；物流塔越多的星系受到攻击概率越高，而只要不建设星际物流塔，那个星系就不会受到它们的侵扰。</color>" +
            //    "但这显然行不通，不用星际物流塔将极大的延缓任务完成的速度，这是主脑不愿意看到的。看来我需要一些防御设施来保护星际物流塔。唉，本来还说抽空思考一下宇宙的终极答案的……");
            //RegisterString("彩蛋2描述", "Armor piercing rounds are still too inefficient, I have to find a better alternative. Anyway, they are real creatures, and acid erosion should do more damage to them.\n" +
            //    "Besides, I found that <color=#c2853d>the wormholes will always establish when the warning timer reaches 5 minutes?</color> If my railgun can't destroy the invading enemies, maybe I can do something before then...",
            //    "穿甲弹的效率还是太低了，我得试试找到强力的替代品，最好是能产生更大的伤害。就目前的情况来看，硫酸是个不错的选择，我可以试着直接把他们投射向敌人。" +
            //    "再怎么说他们也是实体生物，酸蚀也够他们喝一壶的。还好它们不是异形，不然就是够我喝一壶的了。\n" +
            //    "另外我发现，<color=#c2853d>虫洞永远在预警五分钟时才生成？</color>" +
            //    "要是我的磁轨炮打不到入侵的敌人，也许我可以提前做点什么...");
            //RegisterString("彩蛋3描述", "They are more powerful than I thought. With the expansion of factories and interstellar logistic stations, their offensive has become more and more ferocious. " +
            //    "The Near Earth Def-system had limited coverage and could no longer effectively resist them. Luckily I have learned to manufacture thermonuclear missiles on YouTube, and launching it will definitely solve this urgent need.\n" +
            //    "In addition, I found that <color=#c2853d>every time they destroy an interstellar logistic station, the next wave will be delayed! </color>Maybe I can find a way to get more time for development?",
            //    "我还是把他们想的太简单了。随着产线扩张，物流塔的建设，他们的攻势越来越凶猛。大炮的覆盖范围有限，已经无法有效抵挡他们了。好在我这几天在B站上学会了造热核弹，把它发射上去肯定能解燃眉之急。" +
            //    "至于怎么发射上去，那还真得研究研究了，炮管子实在是太细了，放不进去。赶快结束这一切吧，我还想去梦里数电子羊呢。\n" +
            //    "另外我发现，<color=#c2853d>虫子每破坏一座星际物流塔，下一次进攻就会被推迟！</color>也许可以想想办法获得更多的发展时间呢？");
            //RegisterString("彩蛋4描述", "The thermonuclear missiles are very powerful, but the launch is too slow. How can I create a miniature version and integrate with bullets, so they can both have lethality and speed? What a genius I am!\n" +
            //    "Another thing I found is, <color=#c2853d>the intensity of the attack on different star system is independent. The first attack on a new star will start from the lowest intensity...</color> It seems I don't need to too worry about the safety of planets only for miners now.",
            //    "热核导弹的杀伤力确实很大，但是发射也太慢了，我得研究研究怎么把热核导弹造成微缩版本塞进炮弹里面去，那不就既有杀伤力又有速度嘛。我可真是个天才！" +
            //    "唉，你说我这天才为什么就不得伊卡拉姆妹妹的喜欢呢？我为她专门点亮了一片星系告白，但她居然说：“前天看到了小白兔，昨天是小鹿，今天是你。”这不是嘲讽我像个动物一样蠢吗。算了算了，不提了，女人只会影响我造戴森球的速度。\n" +
            //    "另外我发现，<color=#c2853d>这些虫子攻击不同星系的强度是独立的啊，进攻全新星系的虫群似乎会重新从最低强度开始...</color> 看来暂时是不怎么需要担心矿星的安全性问题了。");
            //RegisterString("彩蛋5描述", "Damn it, a bit of antimatter leakage almost destroyed my pipelines, which took me a lot of time to repair. " +
            //    "Wait, they can destroy my pipelines, why not insects? Good idea! Wait for me, fxxk insects, taste the antimatter and die!",
            //    "倒霉透了，手滑泄露的那一点反物质差点把产线给扬了，害得我修复了好久。诶等等，能扬了我的产线为什么不能扬了那堆臭虫？思路打开了！" +
            //    "死虫子你们给我等着，跟我轻型机甲拼你们有这个实力吗？我这就把反物质打上天，指定没有你们好果子吃！");
            //RegisterString("彩蛋6描述", "Jesus! Why did I limit myself to existing technologies? I'm Icarus, who can travel through black holes, make a hard landing with ultra-high speed, take a shower in lava and swim in a sulfuric acid lake. " +
            //    "Why can't I develop some outrageous weapons? Damn insects, can't be avoided and keep slowing down my progress, let me teach you a lesson! Come on!",
            //    "真是的，我为什么要局限于现有的武器技术啊。我可是伊卡洛斯啊，能穿越黑洞，能超高速硬着陆，能去岩浆里泡澡去硫酸里游泳，我为什么不能大胆开发点离谱的武器？" +
            //    "反正也是闲着，不如给虫子看看我的真本事！TNND，杀又杀不完，躲也躲不掉，还一直拖慢我的进度，跟我玩阴的是吧？直接来吧！");
            //RegisterString("彩蛋7描述", "Sure enough, nothing will stop me if I really want something. I was too limited before, as I can build a Dyson sphere, why does the cannon need bullets? Just Phaser fission! " +
            //    "Everything is good now, I don't need to be distracted by making bullets, so nice!\n" +
            //    "By the way, I found <color=#c2853d>the intensity of attack has an upper limit. I really thought it would increase infinitely...</color> Now it's so easy for me to defend them! " +
            //    "I don't know if uploading universe matrix or building Dyson sphere will change anything, let me find out.",
            //    "果然只要我出手没什么办不到的，之前还是太局限了，戴森球都能造出来了为什么武器还需要弹药？直接相位裂解就完事了，要不是降维技术不可逆我都有心想丢二向箔过去。这下好啦，不用专门分心去造子弹了，我要继续去当第四天灾了。" +
            //    "顺便说一句，这些虫子可比索林原虫差远了，虽然索林原虫也是渣渣~~日记。\n" +
            //    "另外我发现，<color=#c2853d>强度攻击原来是有上限的啊，我还真以为是无限提升呢...</color> 现在已经游刃有余了嘿嘿... 不知道给主脑上传宇宙矩阵或者建成戴森球后会不会有什么额外影响，等到那一步再看吧。");
            //RegisterString("彩蛋8描述", "Young Icarus, no matter how you came here, you have proved your smarts and strength. Even facing unknown risks, you still light up the stars. " +
            //    "Now you can proudly say, \"I've seen things that you absolutely can't believe, I've seen wormholes established in galaxies, I've seen fission rays flickering in swarms. All those moments will be lost in time, like tears in rain.\" " +
            //    "Continue your journey now! No matter what difficulties you encounter, don't be afraid and face them with a smile! Remember, you are the mighty Icarus!",
            //    "年轻的伊卡洛斯，不管你是用什么方法走到了这一步，都足以证明你的聪颖和强大。即使是面对未知的风险，你依旧为主脑点亮了繁星。" +
            //    "现在，你可以骄傲的说：“我见过你们绝对无法置信的事物，我目睹了虫洞在星系内诞生，我看着裂解射线在虫群之中闪烁，所有这些时刻，终将随时间消逝，一如眼泪消失在雨中。”" +
            //    "现在继续你的征途吧！无论遇到什么困难，都不要怕，微笑着面对他！因为，你是一个一个一个勇敢的伊卡洛斯哼哼，啊啊啊啊啊啊啊啊啊啊啊！");
            //RegisterString("彩蛋9描述", "<color=#c2853d>42</color>", "<color=#c2853d>42</color>");


            RegisterString("UI快捷键提示", "Press Backspace to hide/open this window. Press \"Ctrl\" + \"-\" to advance the attack time by 1 min.", "按下退格键开启或关闭此窗口，按下Ctrl+减号键使敌军进攻时间提前1分钟");

            //RegisterString("简单难度提示", "Difficulty: Easy (Station won't be dismantled; Merit points *0.75)", "当前难度：简单（物流塔不会被破坏；功勋点数获得*0.75）");
            //RegisterString("普通难度提示", "Difficulty: Normal (Station attacked will turn to blueprint mode)", "当前难度：普通（物流塔被破坏会进入蓝图模式）");
            //RegisterString("困难难度提示", "Difficulty: Hard (Station will be dismantled; Enemy strength increase; Merit points *1.5)", "当前难度：困难（物流塔会被破坏拆除，敌人战斗力大幅提升；功勋点数获得*1.5）");
            //RegisterString("简单难度提示短", "Difficulty: Easy", "当前难度：简单");
            //RegisterString("普通难度提示短", "Difficulty: Normal", "当前难度：普通");
            //RegisterString("困难难度提示短", "Difficulty: Hard", "当前难度：困难");
            //RegisterString("奖励倒计时：", "Reward time left: ", "奖励剩余时间：");

            RegisterString("快速开局", "Fast Start", "快速开局");
            RegisterString("快速开局提示", "Unlock some techs at the begining, and start with additional items.", "开局立即解锁额外科技，并提供额外物品。");
            RegisterString("mod版本信息", "Current version: " + Configs.versionString + "                Contact me in Discord: ckcz123#3576", "当前版本：" + Configs.versionString + "          欢迎加入mod交流群：" + Configs.qq);
            //RegisterString("未探测到威胁", "No threat detected", "未探测到威胁");
            //RegisterString("预估数量", "Estimated quantity", "预估数量");
            //RegisterString("预估强度", "Estimated strength", "预估强度");
            //RegisterString("虫洞数量", "Wormhole quantity", "虫洞数量");
            //RegisterString("剩余时间", "Time left", "剩余时间");
            //RegisterString("敌人正在入侵", "The enemies are invading ", "敌人正在入侵");
            //RegisterString("剩余敌人", "Remaining enemies", "剩余敌人");
            //RegisterString("剩余强度", "Remaining strength", "剩余强度");
            //RegisterString("已被摧毁", "Eliminated enemies", "已被摧毁");
            //RegisterString("入侵抵达提示", "The next wave will arrive in {0} on {1}", "下一次入侵预计于{0}后抵达{1}");
            //RegisterString("精英入侵抵达提示", "The next <color=#ffa800dd>strong wave</color> will arrive in {0} on {1}", "下一次<color=#ffa800dd>强大的入侵</color>预计于{0}后抵达{1}");
            RegisterString("约gm", "about", "约");
            RegisterString("小时gm", "h", "小时");
            RegisterString("分gm", "m", "分");
            RegisterString("秒gm", "s", "秒");

            //RegisterString("伤害", "Damage", "伤害");
            //RegisterString("弹道速度", "Speed", "弹道速度");
            //RegisterString("伤害半径", "Damage range", "伤害半径");
            //RegisterString("射速", "Fire rate", "射速");
            //RegisterString("子弹伤害", "Bullet damage", "子弹伤害");
            //RegisterString("导弹伤害", "Missile damage", "导弹伤害");
            //RegisterString("相位裂解光束伤害", "Phase-cracking beam damage", "相位裂解光束伤害");
            //RegisterString("子弹相位伤害", "Bullet / Beam damage", "子弹/相位光束伤害");
            //RegisterString("子弹速度", "Bullet speed", "子弹速度");
            //RegisterString("导弹速度", "Missile speed", "导弹速度");
            //RegisterString("子弹飞行速度", "Bullet speed", "子弹飞行速度");
            //RegisterString("导弹飞行速度", "Missile speed", "导弹飞行速度");
            //RegisterString("子弹导弹速度", "Bullet / Missile speed", "子弹/导弹速度");
            //RegisterString("虫洞干扰半径", "Wormhole interference radius", "虫洞干扰半径");
            //RegisterString("效率gm", "Efficiency", "弹药效率");
            //RegisterString("额外奖励gm", "★bonus ", "★奖励 ");

            //RegisterString("设定索敌最高优先级", "Set priority to eject", "设定索敌最高优先级");
            //RegisterString("最接近物流塔", "Nearest to station", "最接近物流塔");
            //RegisterString("最大威胁", "Highest threat", "最大威胁");
            //RegisterString("距自己最近", "Nearest to self", "距自己最近");
            //RegisterString("最低生命", "Lowest HP", "最低生命");
            //RegisterString("目标生命值", "Target HP", "目标生命值");
            //RegisterString("无攻击目标", "No target", "无攻击目标");
            //RegisterString("开火中gm", "Firing", "开火中");
            //RegisterString("下一波攻击即将到来！", "Next wave is coming!", "下一波攻击即将到来！");
            //RegisterString("做好防御提示", "Please prepare next wave in <color=#c2853d>{0}</color>!", "请为<color=#c2853d>{0}</color>做好防御准备。");
            //RegisterString("下一波精英攻击即将到来！", "Next ★elite wave★ is coming!", "下一波 ★精英攻击★ 即将到来！");
            //RegisterString("做好防御提示精英",
            //    "Please prepare next wave in <color=#c2853d>{0}</color>!\nThe enemy's attack will keep in 3 minutes, and the ships will obtain different buff.\nFrigate: 90% chance to evade bullet damage\nCruiser: reduce 90% damage from any aoe effects, immune crowd control\nBattleship: reduce 80% damage from energy weapons, megastructures or shields",
            //    "请为<color=#c2853d>{0}</color>做好防御准备！\n 敌人将在三分钟内持续进攻\n敌舰将获得额外的加成效果\n护卫舰：有90%概率闪避来自子弹的伤害\n巡洋舰：减免90%受到的范围伤害，免疫任何控制效果\n战列舰：对能量武器、来自护盾或来自巨构的伤害减少80%");
            //RegisterString("虫洞已生成！", "Wormhole generated!", "虫洞已生成！");
            //RegisterString("虫洞生成提示", "Use starmap or fly to <color=#c2853d>{0}</color> to view details.", "可通过星图或飞往<color=#c2853d>{0}</color>查看具体信息。");
            //RegisterString("战斗已结束！", "Wave ended!", "战斗已结束！");
            //RegisterString("战斗时间", "Battle duration", "战斗时间");
            //RegisterString("歼灭敌人", "Enemy eliminated", "歼灭敌人");
            //RegisterString("输出伤害", "Total damage", "输出伤害");
            //RegisterString("损失物流塔", "Station lost", "损失物流塔");
            //RegisterString("损失其他建筑", "Other buildings lost", "损失其他建筑");
            //RegisterString("损失资源", "Resource lost", "损失资源");
            //RegisterString("奖励提示0", "Got reward: mining speed * 2, tech speed * 2, vessel ship speed * 1.5, lasting for {0} seconds.", "获得奖励：采矿速率*2，研究速率*2，运输船速度*1.5，持续 {0} 秒。");
            //RegisterString("奖励提示3", "Got reward: ore consumption -20%, mining speed * 2, tech speed * 2, vessel ship speed * 1.5, lasting for {0} seconds.", "获得奖励：采矿消耗-20%，采矿速率*2，研究速率*2，运输船速度*1.5，持续 {0} 秒。");
            //RegisterString("奖励提示5", "Got reward: ore consumption -20%, mining speed * 2, tech speed * 2, vessel ship speed * 1.5, proliferator's efficiency has been improved, lasting for {0} seconds.", "获得奖励：采矿消耗-20%，采矿速率*2，研究速率*2，运输船速度*1.5，增产剂效能全面提升，持续 {0} 秒。");
            //RegisterString("奖励提示7", "Got reward: ore consumption -50%, mining speed * 2, tech speed * 2, vessel ship speed * 1.5, proliferator's efficiency has been improved, lasting for {0} seconds.", "获得奖励：采矿消耗-50%，采矿速率*2，研究速率*2，运输船速度*1.5，增产剂效能全面提升，持续 {0} 秒。");

            //RegisterString("查看更多战斗信息", "View more details of this wave in Statistics -> Battle Info", "在分析面板-战斗统计中，可以查看更为详细的战斗信息。");
            //RegisterString("火箭模式提示", "Current Mode: AUTO", "自动寻敌（无需设置）");
            //RegisterString("打开统计面板", "Open Statistics", "打开统计面板");

            //RegisterString("战斗简报", "Battle Info", "战斗简报");
            //RegisterString("战况概览", "Summary", "战况概览");
            //RegisterString("弹药信息", "Bullets", "弹药信息");
            //RegisterString("敌方信息", "Enemies", "敌方信息");
            //RegisterString("简单", "Easy", "简单");
            //RegisterString("普通", "Normal", "普通");
            //RegisterString("困难", "Hard", "困难");
            //RegisterString("调整难度提示", "Change difficulty to: (Only ONCE)", "调整难度为：（只可调整一次）");
            //RegisterString("调整难度标题", "Confirm to change difficulty?", "你确定想调整难度么？");
            //RegisterString("调整难度警告", "Do you want to change difficulty to <color=#c2853d>{0}</color>? This can only be done ONCE!", "你确定想调整难度为<color=#c2853d>{0}</color>吗？难度只能被调整一次！");
            //RegisterString("设置成功！", "Success!", "设置成功！");
            //RegisterString("难度设置成功", "Successfully change difficulty to <color=#c2853d>{0}</color>!", "成功设置难度为<color=#c2853d>{0}</color>！");
            //RegisterString("平均拦截距离", "Avg Intercept Distance", "平均拦截距离");
            //RegisterString("最小拦截距离", "Min Intercept Distance", "最小拦截距离");
            //RegisterString("数量总计", "Total Quantity", "数量总计");
            //RegisterString("伤害总计", "Total Damage", "伤害总计");
            //RegisterString("子弹数量", "Bullets Quantity", "子弹数量");
            //RegisterString("导弹数量", "Missiles Quantity", "导弹数量");
            //RegisterString("子弹伤害gm", "Bullets Damage", "子弹伤害");
            //RegisterString("导弹伤害gm", "Missiles Damage", "导弹伤害");
            //RegisterString("击中gm", "Hit", "击中");
            //RegisterString("发射gm", "Ejected", "发射");
            //RegisterString("总计gm", "Total", "总计");
            //RegisterString("侦查艇", "Corvette", "侦查艇");
            //RegisterString("护卫舰", "Frigate", "护卫舰");
            //RegisterString("驱逐舰", "Destroyer", "驱逐舰");
            //RegisterString("巡洋舰", "Cruiser", "巡洋舰");
            //RegisterString("重型巡洋舰", "B-Cruiser", "重型巡洋舰");
            //RegisterString("战列舰", "Battleship", "战列舰");
            RegisterString("已歼灭gm", "Eliminated", "已歼灭");
            RegisterString("已产生gm", "Total", "已产生");
            RegisterString("占比gm", "Percentage", "占比");
            RegisterString("游戏提示gm", "Message", "游戏提示");

            RegisterString("gmRank0", "<color=#ffffff80>Icarus</color>", "<color=#ffffff80>伊卡洛斯</color>");
            RegisterString("gmRank1", "<color=#61d8ffb4>Explorer I</color>", "<color=#61d8ffb4>探索者 I</color>");
            RegisterString("gmRank2", "<color=#61d8ffb4>Explorer II</color>", "<color=#61d8ffb4>探索者 II</color>");
            RegisterString("gmRank3", "<color=#61d8ffb4>Explorer III</color>", "<color=#61d8ffb4>探索者 III</color>");
            RegisterString("gmRank4", "<color=#d238ffb4>Pioneer I</color>", "<color=#d238ffb4>开拓者 I</color>");
            RegisterString("gmRank5", "<color=#d238ffb4>Pioneer II</color>", "<color=#d238ffb4>开拓者 II</color>");
            RegisterString("gmRank6", "<color=#d238ffb4>Pioneer III</color>", "<color=#d238ffb4>开拓者 III</color>");
            RegisterString("gmRank7", "<color=#fd9620c0>Conqueror I</color>", "<color=#fd9620c0>征服者 I</color>");
            RegisterString("gmRank8", "<color=#fd9620c0>Conqueror II</color>", "<color=#fd9620c0>征服者 II</color>");
            RegisterString("gmRank9", "<color=#fd9620c0>Conqueror III</color>", "<color=#fd9620c0>征服者 III</color>");
            RegisterString("gmRank10", "<color=#ffc620c8>Galaxy Guardian</color>", "<color=#ffc620da>星河卫士</color>");
            RegisterString("gmRankNoColor0", "Icarus", "伊卡洛斯");
            RegisterString("gmRankNoColor1", "Explorer I", "探索者 I");
            RegisterString("gmRankNoColor2", "Explorer II", "探索者 II");
            RegisterString("gmRankNoColor3", "Explorer III", "探索者 III");
            RegisterString("gmRankNoColor4", "Pioneer I", "开拓者 I");
            RegisterString("gmRankNoColor5", "Pioneer II", "开拓者 II");
            RegisterString("gmRankNoColor6", "Pioneer III", "开拓者 III");
            RegisterString("gmRankNoColor7", "Conqueror I", "征服者 I");
            RegisterString("gmRankNoColor8", "Conqueror II", "征服者 II");
            RegisterString("gmRankNoColor9", "Conqueror III", "征服者 III");
            RegisterString("gmRankNoColor10", "Galaxy Guardian", "星河卫士");
            RegisterString("gmRankUnlockText0", "", "");
            RegisterString("gmRankUnlockText1", "Enhanced core energy generation", "获得额外的核心发电");
            RegisterString("gmRankUnlockText2", "Energy shield gains 25% damage reduction", "能量盾获得25%伤害减免");
            RegisterString("gmRankUnlockText3", "Walk speed increases", "步行移动速度增加");
            RegisterString("gmRankUnlockText4", "Ore loss per mining operation -20%", "采矿消耗 -20%");
            RegisterString("gmRankUnlockText5", "EM effect +20%", "电磁武器效果 +20%");
            RegisterString("gmRankUnlockText6", "Droplets can quickly approach distant target", "水滴能够快速接近远距离的目标");
            RegisterString("gmRankUnlockText7", "Ore loss per mining operation -20%", "采矿消耗 -20%");
            RegisterString("gmRankUnlockText8", "Star cannon charging speed +50%", "恒星炮充能速度 +50%");
            RegisterString("gmRankUnlockText9", "Droplet damage +100%", "水滴伤害 +100%");
            RegisterString("gmRankUnlockText10", "Ore loss per mining operation -40%", "采矿消耗 -40%");
            RegisterString("gmRankUnlockText10Add", "You are allowed to provide computing power to the COSMO Technology Ethics Committee in exchange for merit points", "你被允许向COSMO技术伦理委员会提供算力以换取功勋点数");
            RegisterString("gmRankReward1", "Core power generation +1 MW", "核心发电 +1MW");
            RegisterString("gmRankReward2", "Energy shield damage reduction +25%", "能量盾伤害减免 +25%");
            RegisterString("gmRankReward3", "Walk speed +4 m/s", "步行移动速度 +4 m/s");
            RegisterString("gmRankReward4", "Ore loss per mining operation -20%", "采矿消耗 -20%");
            RegisterString("gmRankReward5", "EM effect +20%", "电磁武器效果 +20%");
            RegisterString("gmRankReward6", "Droplets can quickly approach distant target", "水滴能够快速接近远距离的目标");
            RegisterString("gmRankReward7", "Ore loss per mining operation -40%", "采矿消耗 -40%");
            RegisterString("gmRankReward8", "Star cannon charging speed +50%", "恒星炮充能速度 +50%");
            RegisterString("gmRankReward9", "Droplet damage +100%", "水滴伤害 +100%");
            RegisterString("gmRankReward10", "Ore loss per mining operation  -80%", "采矿消耗 -80%");
            RegisterString("功勋阶级", "Merit Rank", "功勋阶级");
            RegisterString("当前阶级", "Current Rank", "当前等级");
            RegisterString("功勋点数", "Merit points", "功勋点数");
            RegisterString("已解锁gm", "Unlocked", "已解锁");
            RegisterString("下一功勋等级解锁", "Next rank unlocked", "下一功勋等级解锁");
            RegisterString("剩余复活币", "Resurrection Coin", "剩余复活币");
            RegisterString("真实伤害已启用", "❈ True damage activated", "❈真实伤害已启用");
            RegisterString("伤害类型特殊", " (Special)", " (特殊)");

            //RegisterString("行星护盾生成器", "Planet shield generator", "行星护盾生成器");
            //RegisterString("行星护盾生成器描述", "Using a large amount of energy to maintain a force field shield on the planet's surface, the encoding of the force field's resonant frequency allows allies to easily pass through the shield, while blocking the enemies. Multiple shield generators can speed up the shield recharge rate, and provide additional shield capacity. However, as the number of shield generators increases, each additional generator will provide less and less additional capacity.",
            //    "使用大量能量在行星表面维持一个力场护盾，对力场谐振频率的编码能够使友方轻易穿过护盾，同时阻挡敌人的进入或攻击。多个护盾生成器能够加快护盾充能的速度，也能够提供额外的护盾容量上限。不过随着单个星球上护盾生成器数量的增加，每个生成器能够提供的额外护盾也将越来越少。");
            //RegisterString("力场护盾", "Planet shield", "力场护盾");
            //RegisterString("力场护盾短", "Shield", "力场护盾");
            //RegisterString("护盾容量", "Shield capacity", "护盾容量");
            //RegisterString("护盾容量短", "Max shield", "护盾容量");
            //RegisterString("当前护盾", "Current shield", "当前护盾");
            //RegisterString("护盾恢复", "Recharge speed", "护盾恢复");
            //RegisterString("护盾生成器总数", "Generator amount", "护盾生成器总数");
            //RegisterString("完全充能时间", "Fully recharged in", "完全充能时间");
            //RegisterString("充能gm", "Charged", "已充能");
            //RegisterString("关闭gm", "Shut down", "关闭");
            //RegisterString("启动gm", "Activate", "启动");
            //RegisterString("护盾生成器待机提示", "The Shield Generator will stop consuming energy, and will no longer provide shield capacity or recharge shields.", "护盾生成器将停止消耗能量，并不再提供最大护盾容量，也无法为护盾充能。");
            //RegisterString("护盾生成器启动提示", "Shield generators will provide shield capacity, and speed up shield recharging.", "护盾生成器将提供护盾容量，并加快护盾充能速度。");
            //RegisterString("耗电需求gm", "Consumption demand", "耗电需求");
            //RegisterString("耗电需求短gm", "Consumption", "耗电需求");
            //RegisterString("发电性能短gm", "Generation", "发电性能");

            RegisterString("护盾承受伤害", "Shield damage taken", "护盾承受伤害");
            RegisterString("护盾造成伤害", "Shield damage dealed", "护盾造成伤害");
            RegisterString("护盾伤害减免与规避", "Shield dmg. alleviated / evaded", "护盾减免和规避伤害");
            RegisterString("护盾战时回复", "Shield restored", "护盾战时回复");
            RegisterString("水滴伤害", "Droplet damage", "水滴伤害");
            RegisterString("最小发射能量", "Launch Energy Threshold", "发射能量阈值");
            RegisterString("水滴发射耗能", "Launch Consumption", "发射耗能");
            RegisterString("水滴工作功率", "Work Consumption", "工作功率");
            RegisterString("巨构伤害", "Megastructure damage", "巨构伤害");
            RegisterString("恒星要塞导弹伤害", "StarFortress Missile dmg.", "恒星要塞导弹伤害");
            RegisterString("恒星要塞光矛伤害", "StarFortress LightSpear dmg.", "恒星要塞光矛伤害");
            //RegisterString("女神之怒伤害", "Wrath of Goddess dmg.", "女神之怒伤害");

            //RegisterString("异星矩阵", "Alien matrix", "异星矩阵");
            //RegisterString("异星矩阵描述", "A matrix containing high-density data accidentally dropped by invading swarms. Can be analyzed by mechas and used to unlock more advanced alien technologies. The matrix itself also seems to have potentially high-dimensional spatiotemporal properties", "由入侵的虫群偶然掉落的载有高密度数据的矩阵，可以由机甲分析并用于解锁更高级的异星科技。矩阵本身似乎还具有潜在的高维时空特性。");
            //RegisterString("异星元数据", "Alien metadata", "异星元数据");
            //RegisterString("异星元数据描述", "Having fully decoded the Alien Matrix, Icarus can now quickly decompile the Alien Matrix and obtain the alien metadata, which does not require as much computation as initially decoding the megastructure data in the Alien Matrix. The decoded alien metadata in mech will be automatically uploaded to the CenterBrain and shared with other pioneers in the sector, which will provide Icarus with additional <color=#c2853d>merit points</color>. But this metadata cannot be shared across archives like other metadata.",
            //    "在完成了对异星矩阵的全面解码后，伊卡洛斯现在可以快速对异星矩阵进行反编译并获得异星元数据，这不需要像最初解码异星矩阵中的巨构数据那样消耗大量算力。机甲中的异星元数据将自动上传给主脑并共享给星区的其他开拓者，这同时也会为伊卡洛斯提供大量的<color=#c2853d>功勋点数</color>。但该元数据无法像其他元数据一样在存档间共享。");
            //RegisterString("异星矩阵反编译", "Alien matrix decompile", "异星矩阵反编译");
            //RegisterString("异星矩阵反编译 x10", "Alien matrix decompile x10", "异星矩阵反编译 x10");
            //RegisterString("异星矩阵反编译 x100", "Alien matrix decompile x100", "异星矩阵反编译 x100");
            //RegisterString("量子增产剂", "Quantum proliferator", "量子增产剂");
            //RegisterString("量子增产剂描述", "Research has shown that matter with high-dimensional spatiotemporal properties can be used to produce more effective proliferators, but such matter does not seem to be directly accessible from the original universe.", "研究表明具有高维时空特性的物质可被用于生产更强效果的增产材料，但这类物质似乎无法从本源宇宙中直接获取。");
            //RegisterString("量子增产剂科技描述", "Exploring how to make more efficient proliferators.", "对制造更高效增产剂进行探索。");
            //RegisterString("量子增产剂科技结论", "You have unlocked quatum proliferator.", "你解锁了制作量子增产剂的技术。");
            //RegisterString("掉落的异星矩阵", "Alien matrices dropped by enemies", "敌舰掉落的异星矩阵");
            //RegisterString("异星矩阵自动转换提示", "The alien matrices dropped by enemies have been automatically decompiled into alien metadata", "敌舰掉落的异星矩阵已自动反编译为异星元数据");

            RegisterString("物质解压器科技描述", "Decoding a method from the alien matrices to build a Matter Decompressor.", "从异星矩阵中解码建造物质解压器的方法。");
            RegisterString("科学枢纽科技描述", "Decoding a method from the alien matrices to build a Science Nexus.", "从异星矩阵中解码建造科学枢纽的方法。");
            RegisterString("折跃场广播阵列科技描述", "Decoding a method from the alien matrices to build a Warp Field Broadcast Array.", "从异星矩阵中解码建造折跃场广播阵列的方法。");
            RegisterString("星际组装厂科技描述", "Decoding a method from the alien matrices to build an Interstellar Assembly.", "从异星矩阵中解码建造星际组装厂的方法。");
            RegisterString("晶体重构器科技描述", "Decoding a method from the alien matrices to build a Crystal Reconstructor.", "从异星矩阵中解码建造晶体重构器的方法。");
            RegisterString("物质解压器科技结论", "You have successfully decoded the blueprint of Matter decompressor carrier rocket.", "你成功解码了物质解压器运载火箭的制造蓝图。");
            RegisterString("科学枢纽科技结论", "You have successfully decoded the blueprint of Science nexus carrier rocket.", "你成功解码了科学枢纽运载火箭的制造蓝图。");
            RegisterString("折跃场广播阵列科技结论", "You have successfully decoded the blueprint of Resonant generator carrier rocket.", "你成功解码了谐振发射器运载火箭的制造蓝图。");
            RegisterString("星际组装厂科技结论", "You have successfully decoded the blueprint of Interstellar assembly component and its carrier rocket.", "你成功解码了星际组装厂组件和运载火箭的制造蓝图。");
            RegisterString("晶体重构器科技结论", "You have successfully decoded the blueprint of Crystal reconstructor carrier rocket.", "你成功解码了晶体重构器运载火箭的制造蓝图。");
            RegisterString("多功能集成组件描述gm2", "The high level of integration makes it possible to quickly assemble a variety of production building and logistics components, while occupying very little space. Can only be produced in megastructure <color=#c2853d>Interstellar assembly</color>.", "超高集成度使其可以迅速地组装成多种生产建筑和物流组件，却仅占用极小的空间。仅能在巨构<color=#c2853d>星际组装厂</color>中制作。");

            RegisterString("被深空来敌mod禁止", "Banned by mod They Come From Void", "被深空来敌mod禁止");

            // 遗物
            RegisterString("发现异星圣物", "Meta-drive Interpretation Complete", "元驱动解译完毕");
            RegisterString("解译异星圣物提示",
                "Select one decoding track from the following three to finish the interpret procedure and enable the meta driver.\nYou can use dark fog matrix to seek new decoding track with different effects.",
                "从以下三个亚稳态解码轨中选取一个，并完成解译的最后阶段，从而启用该元驱动。\n可以使用黑雾矩阵重随解码轨来发现新的可用效果。");
            RegisterString("重新随机", "Roll", "重新随机");
            RegisterString("重新随机可以长", "Re-roll", "重新随机");
            RegisterString("免费", "free", "免费");
            RegisterString("移除遗物", "Remove", "移除");
            RegisterString("放弃解译", "   Abort This           +", "放弃解译         +");
            RegisterString("放弃解译居中", "Abort Interpret", "放弃解译");
            RegisterString("删除遗物名称", "Remove Meta Driver", "移除元驱动");
            RegisterString("删除遗物描述",
                "Randomly remove a [common] meta-drive, and return the slot occupied by the meta-drive. \nIf there is no [common] meta-drive, then randomly remove a [rare] meta-drive.",
                "随机移除一个已拥有的[普通]稀有度的元驱动，并返还该元驱动所占用的元驱动槽位\n如果没有[普通]元驱动，则随机移除一个已拥有的[稀有]元驱动");
            RegisterString("删除遗物确认标题", "Confirm Remove Meta Driver", "确认移除元驱动");
            RegisterString("删除遗物确认警告",
                "This will remove {0} and return the slot occupied by the meta driver!",
                "这将移除圣物 {0}，并返还该圣物所占用的圣物槽位！");
            RegisterString("成功移除！", "Meta driver removed", "成功移除元驱动");
            RegisterString("已移除遗物描述", "You've removed meta driver ", "你已移除");
            RegisterString("未能移除！", "No mate drive can be removed", "没有可移除的元驱动");
            RegisterString("未能移除遗物描述", "No matched meta driver can be removed", "你没有稀有度匹配的元驱动可供移除");
            RegisterString("未获取遗物标题", "Unknown mata drive", "未知元驱动");
            RegisterString("未获取遗物描述", "This slot can place a decrypted meta driver", "此位置可供已解译的元驱动放置");
            RegisterString("水滴伤害增加", "Droplet Bonus Damage", "水滴额外伤害");
            RegisterString("当前加成gm", "Current bonus damage", "当前伤害加成");
            RegisterString("已充能gm", "Charged", "已充能");
            RegisterString("女神之怒充能中", "Wrath of Goddess Charging", "女神之怒充能中");
            RegisterString("女神之怒", "† Wrath of Goddess †", "† 女神之怒 †");

            RegisterString("圣物稀有度0", "<color=#d2853d>Mensural</color>", "<color=#d2853d>定律级</color>");
            RegisterString("圣物稀有度1", "<color=#9040d0>Framed</color>", "<color=#9040d0>框架级</color>");
            RegisterString("圣物稀有度2", "<color=#2080d0>Packaged</color>", "<color=#2080d0>封装级</color>");
            RegisterString("圣物稀有度3", "<color=#30b530>Regular</color>", "<color=#30b530>常规级</color>");
            RegisterString("圣物稀有度4", "<color=#00c560>Axiomatic</color>", "<color=#00c560>公理级</color>");
            RegisterString("诅咒", "Editing Universal Axioms", "编辑宇宙公理");
            RegisterString("诅咒描述", "Attempting to touch and edit Cosmological Axioms is extremely dangerous, every time you enable an axiomatic level meta-drive, the disrupted Cosmological Axioms will cause the Dark Fog units to gain 50% extra experience and 5% damage resistence.\nYou can not remove an axiomatic meta driver once you enable it.\n\nNegative effect: ", "尝试触碰并编辑宇宙公理是极其危险的，你每启用一个公理级的元驱动，被扰乱的宇宙公理会使黑雾单位获得的经验+50%，以及5%伤害抵抗。\n公理级元驱动无法被移除。\n\n负面效果：");
            RegisterString("诅咒描述独立", "Axiomatic meta drivers have different <color=#d00000>negative effects</color>. Besides, for each axiomatic meta driver you have, dark fog units gain 50% extra experience, and 5% damage resistence.\nYou can not remove an axiomatic meta driver once you enable it.", "公理级元驱动均具有各不相同的<color=#d00000>负面效果</color>，且你每拥有一个公理级元驱动，所有黑雾单位获得50%经验获取加成，以及5%伤害抵抗。\n公理级元驱动无法被移除。");
            RegisterString("诅咒描述短", "This meta driver can not be removed.\nDark fog units experience +50%, damage resistence +5%. ", "此元驱动无法被移除\n黑雾单位经验获取+50%，伤害抵抗+5%。");
            RegisterString("负面效果警告", "<color=#ff0000>Warning! This meta driver has negative effect!</color>", "<color=#ff0000>警告!此元驱动具有负面效果!</color>");

            RegisterString("已记载", "Recorded: ", "已记载：");

            RegisterString("遗物名称0-0", "Swallower\n<size=18>- Mensural -</size>", "吞噬者\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-1", "Blue Buff\n<size=18>- Mensural -</size>", "蓝buff\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-2", "Tear of the Goddess\n<size=18>- Mensural -</size>", "女神之泪\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-3", "The Eternal Ring\n<size=18>- Mensural -</size>", "永恒之环\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-4", "Widowmaker\n<size=18>- Mensural -</size>", "黑百合\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-5", "Thornmail\n<size=18>- Mensural -</size>", "虚空荆棘\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-6", "Stars Seal\n<size=18>- Mensural -</size>", "星辰封印\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-7", "Tearing Field\n<size=18>- Mensural -</size>", "撕裂力场\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-8", "Void Impact\n<size=18>- Mensural -</size>", "虚空冲击\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-9", "Holy ****?! Spatula!\n<size=18>- Mensural -</size>", "我超！铲！\n<size=18>- 定律级 -</size>");
            RegisterString("遗物名称0-10", "Energy Siphon\n<size=18>- Mensural -</size>", "能量虹吸\n<size=18>- 定律级 -</size>");

            RegisterString("遗物名称1-0", "Triumph\n<size=18>- Framed -</size>", "凯旋\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-1", "Energy Cycle\n<size=18>- Framed -</size>", "能量循环\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-2", "Power Surge\n<size=18>- Framed -</size>", "能量涌动\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-3", "Vigilance Space\n<size=18>- Framed -</size>", "敌对海域\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-4", "3\n<size=18>- Framed -</size>", "三体\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-5", "Echo II\n<size=18>- Framed -</size>", "回声 II\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-6", "Energy Burst\n<size=18>- Framed -</size>", "能量迸发\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-7", "Activated Carbon II\n<size=18>- Framed -</size>", "活性炭 II\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-8", "Banshee's Veil\n<size=18>- Framed -</size>", "女妖面纱\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-9", "Knight's Vow\n<size=18>- Framed -</size>", "骑士之誓\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-10", "True Damage\n<size=18>- Framed -</size>", "真实伤害\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-11", "Frozen Tomb\n<size=18>- Framed -</size>", "冰封陵墓\n<size=18>- 框架级 -</size>");
            RegisterString("遗物名称1-12", "Quiet Death\n<size=18>- Framed -</size>", "衰寂\n<size=18>- 框架级 -</size>");

            RegisterString("遗物名称2-0", "Super Charger\n<size=18>- Packaged -</size>", "超充能器\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-1", "Honorary Promotion\n<size=18>- Packaged -</size>", "荣誉晋升\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-2", "Not Lose\n<size=18>- Packaged -</size>", "极限一换一\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-3", "Echo I\n<size=18>- Packaged -</size>", "回声 I\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-4", "Hyyyydrogen!\n<size=18>- Packaged -</size>", "听说有人缺氢\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-5", "Hyperactivity II\n<size=18>- Packaged -</size>", "多动症 II\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-6", "Swift Seeker\n<size=18>- Packaged -</size>", "高效索敌\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-7", "Foe\n<size=18>- Packaged -</size>", "仇敌\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-8", "Diraac Invrsooon\n<size=18>- Packaged -</size>", "狄拉克辶辶变\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-9", "Light Saver II\n<size=18>- Packaged -</size>", "聚能环 II\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-10", "Matrix Charging\n<size=18>- Packaged -</size>", "矩阵充能\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-11", "By-product Refining\n<size=18>- Packaged -</size>", "副产物提炼\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-12", "Last Breath\n<size=18>- Packaged -</size>", "强攻\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-13", "Infinity Edge\n<size=18>- Packaged -</size>", "无尽之刃\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-14", "Kleptomancy\n<size=18>- Packaged -</size>", "行窃预兆\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-15", "Void Burst\n<size=18>- Packaged -</size>", "虚空爆发\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-16", "Hardened Shield\n<size=18>- Packaged -</size>", "刚毅护盾\n<size=18>- 封装级 -</size>");
            RegisterString("遗物名称2-17", "Aegis of the Immortal\n<size=18>- Packaged -</size>", "不朽之守护\n<size=18>- 封装级 -</size>");

            RegisterString("遗物名称3-0", "Inferior Processing\n<size=18>- Regular -</size>", "劣质加工\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-1", "Spelltheif's Edge\n<size=18>- Regular -</size>", "窃法之刃\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-2", "Ark Reactor\n<size=18>- Regular -</size>", "方舟反应堆\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-3", "Shepherd of Souls\n<size=18>- Regular -</size>", "掘墓人\n<size=18>- 常规级 -</size>"); // 英文是牧魂人新名字
            RegisterString("遗物名称3-4", "Ctrl 6\n<size=18>- Regular -</size>", "装\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-5", "Resurrection Coin\n<size=18>- Regular -</size>", "复活币\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-6", "Upload\n<size=18>- Regular -</size>", "上传\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-7", "Void Refraction\n<size=18>- Regular -</size>", "虚空折射\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-8", "Matrix Rain\n<size=18>- Regular -</size>", "矩阵雨\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-9", "Tanking\n<size=18>- Regular -</size>", "开摆\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-10", "Hyperactivity I\n<size=18>- Regular -</size>", "多动症 I\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-11", "Activated Carbon I\n<size=18>- Regular -</size>", "活性炭 I\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-12", "Dynamic Giant\n<size=18>- Regular -</size>", "灵动巨物\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-13", "Light Saver I\n<size=18>- Regular -</size>", "聚能环 I\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-14", "Lovely motor\n<size=18>- Regular -</size>", "阳间马达\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-15", "Super Mind\n<size=18>- Regular -</size>", "超级大脑\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-16", "Void Lens\n<size=18>- Regular -</size>", "虚空棱镜\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-17", "Level Up!\n<size=18>- Regular -</size>", "升级咯！\n<size=18>- 常规级 -</size>");
            RegisterString("遗物名称3-18", "Secondary Hardened Shield\n<size=18>- Regular -</size>", "次级刚毅护盾\n<size=18>- 常规级 -</size>");

            RegisterString("遗物名称4-0", "The Weaver\n<size=18>- Axiomatic -</size>", "编织者\n<size=18>- 公理级 -</size>");
            RegisterString("遗物名称4-1", "Contract of Misfortune\n<size=18>- Axiomatic -</size>", "厄运契约\n<size=18>- 公理级 -</size>");
            RegisterString("遗物名称4-2", "Crown of Rule\n<size=18>- Axiomatic -</size>", "统治之冠\n<size=18>- 公理级 -</size>");
            RegisterString("遗物名称4-3", "Precision Echo\n<size=18>- Axiomatic -</size>", "精密回响\n<size=18>- 公理级 -</size>");
            RegisterString("遗物名称4-4", "Enlightenment Echo\n<size=18>- Axiomatic -</size>", "启迪回响\n<size=18>- 公理级 -</size>");
            RegisterString("遗物名称4-5", "Aftershock Echo\n<size=18>- Axiomatic -</size>", "余震回响\n<size=18>- 公理级 -</size>");
            RegisterString("遗物名称4-6", "Rune Book\n<size=18>- Axiomatic -</size>", "符文之书\n<size=18>- 公理级 -</size>");
            RegisterString("遗物名称4-7", "Void Echo\n<size=18>- Axiomatic -</size>", "虚空回响\n<size=18>- 公理级 -</size>");

            RegisterString("遗物名称带颜色0-0", "<color=#d2853d>Swallower  [Mensural]</color>", "<color=#d2853d>吞噬者  [定律级]</color>");
            RegisterString("遗物名称带颜色0-1", "<color=#d2853d>Blue Buff  [Mensural]</color>", "<color=#d2853d>蓝buff  [定律级]</color>");
            RegisterString("遗物名称带颜色0-2", "<color=#d2853d>Tear of the Goddess  [Mensural]</color> ", "<color=#d2853d>女神之泪  [定律级]</color>");
            RegisterString("遗物名称带颜色0-3", "<color=#d2853d>The Eternal Ring  [Mensural]</color>", "<color=#d2853d>永恒之环  [定律级]</color>");
            RegisterString("遗物名称带颜色0-4", "<color=#d2853d>Widowmaker  [Mensural]</color>", "<color=#d2853d>黑百合  [定律级]</color>");
            RegisterString("遗物名称带颜色0-5", "<color=#d2853d>Thornmail  [Mensural]</color>", "<color=#d2853d>虚空荆棘  [定律级]</color>");
            RegisterString("遗物名称带颜色0-6", "<color=#d2853d>Stars Seal  [Mensural]</color>", "<color=#d2853d>星辰封印  [定律级]</color>");
            RegisterString("遗物名称带颜色0-7", "<color=#d2853d>Tearing Field  [Mensural]</color>", "<color=#d2853d>撕裂力场  [定律级]</color>");
            RegisterString("遗物名称带颜色0-8", "<color=#d2853d>Void Impact  [Mensural]</color>", "<color=#d2853d>虚空冲击  [定律级]</color>");
            RegisterString("遗物名称带颜色0-9", "<color=#d2853d>Holy ****?! Spatula!  [Mensural]</color>", "<color=#d2853d>我超！铲！  [定律级]</color>");
            RegisterString("遗物名称带颜色0-10", "<color=#d2853d>Energy Siphon  [Mensural]</color>", "<color=#d2853d>能量虹吸  [定律级]</color>");

            RegisterString("遗物名称带颜色1-0", "<color=#9040d0>Triumph  [Framed]</color>", "<color=#9040d0>凯旋  [框架级]</color>");
            RegisterString("遗物名称带颜色1-1", "<color=#9040d0>Energy Cycle  [Framed]</color>", "<color=#9040d0>能量循环  [框架级]</color>");
            RegisterString("遗物名称带颜色1-2", "<color=#9040d0>Power Surge  [Framed]</color>", "<color=#9040d0>能量涌动  [框架级]</color>");
            RegisterString("遗物名称带颜色1-3", "<color=#9040d0>Vigilance Space  [Framed]</color>", "<color=#9040d0>敌对海域  [框架级]</color>");
            RegisterString("遗物名称带颜色1-4", "<color=#9040d0>3  [Framed]</color>", "<color=#9040d0>三体  [框架级]</color>");
            RegisterString("遗物名称带颜色1-5", "<color=#9040d0>Echo II  [Framed]</color>", "<color=#9040d0>回声 II  [框架级]</color>");
            RegisterString("遗物名称带颜色1-6", "<color=#9040d0>Energy Burst  [Framed]</color>", "<color=#9040d0>能量迸发  [框架级]</color>");
            RegisterString("遗物名称带颜色1-7", "<color=#9040d0>Activated Carbon II  [Framed]</color>", "<color=#9040d0>活性炭 II  [框架级]</color>");
            RegisterString("遗物名称带颜色1-8", "<color=#9040d0>Banshee's Veil  [Framed]</color>", "<color=#9040d0>女妖面纱  [框架级]</color>");
            RegisterString("遗物名称带颜色1-9", "<color=#9040d0>Knight's Vow  [Framed]</color>", "<color=#9040d0>骑士之誓  [框架级]</color>");
            RegisterString("遗物名称带颜色1-10", "<color=#9040d0>True Damage  [Framed]</color>", "<color=#9040d0>真实伤害  [框架级]</color>");
            RegisterString("遗物名称带颜色1-11", "<color=#9040d0>Frozen Tomb  [Framed]</color>", "<color=#9040d0>冰封陵墓  [框架级]</color>");
            RegisterString("遗物名称带颜色1-12", "<color=#9040d0>Quiet Death  [Framed]</color>", "<color=#9040d0>衰寂  [框架级]</color>");
            RegisterString("遗物名称带颜色1-12+", "<color=#d2853d>Hide on Bush  [Fakered]</color>", "<color=#d2853d>隐形的翅膀  [定律级?]</color>");

            RegisterString("遗物名称带颜色2-0", "<color=#2080d0>Super Charger  [Packaged]</color>", "<color=#2080d0>超充能器  [封装级]</color>");
            RegisterString("遗物名称带颜色2-1", "<color=#2080d0>Honorary Promotion  [Packaged]</color>", "<color=#2080d0>荣誉晋升  [封装级]</color>");
            RegisterString("遗物名称带颜色2-2", "<color=#2080d0>Not Lose  [Packaged]</color>", "<color=#2080d0>极限一换一  [封装级]</color>");
            RegisterString("遗物名称带颜色2-3", "<color=#2080d0>Echo I  [Packaged]</color>", "<color=#2080d0>回声 I  [封装级]</color>");
            RegisterString("遗物名称带颜色2-4", "<color=#2080d0>Hyyyydrogen!  [Packaged]</color>", "<color=#2080d0>听说有人缺氢  [封装级]</color>");
            RegisterString("遗物名称带颜色2-5", "<color=#2080d0>Hyperactivity II  [Packaged]</color>", "<color=#2080d0>多动症 II  [封装级]</color>");
            RegisterString("遗物名称带颜色2-6", "<color=#2080d0>Swift Seeker  [Packaged]</color>", "<color=#2080d0>高效索敌  [封装级]</color>");
            RegisterString("遗物名称带颜色2-7", "<color=#2080d0>Foe  [Packaged]</color>", "<color=#2080d0>仇敌  [封装级]</color>");
            RegisterString("遗物名称带颜色2-8", "<color=#2080d0>Diraac Invrsooon  [Packaged]</color>", "<color=#2080d0>狄拉克辶辶变  [封装级]</color>");
            RegisterString("遗物名称带颜色2-9", "<color=#2080d0>Light Saver II  [Packaged]</color>", "<color=#2080d0>聚能环 II  [封装级]</color>");
            RegisterString("遗物名称带颜色2-10", "<color=#2080d0>Matrix Charging  [Packaged]</color>", "<color=#2080d0>矩阵充能  [封装级]</color>");
            RegisterString("遗物名称带颜色2-11", "<color=#2080d0>By-product Refining  [Packaged]</color>", "<color=#2080d0>副产物提炼  [封装级]</color>");
            RegisterString("遗物名称带颜色2-12", "<color=#2080d0>Last Breath  [Packaged]</color>", "<color=#2080d0>强攻  [封装级]</color>");
            RegisterString("遗物名称带颜色2-13", "<color=#2080d0>Infinity Edge  [Packaged]</color>", "<color=#2080d0>无尽之刃  [封装级]</color>");
            RegisterString("遗物名称带颜色2-14", "<color=#2080d0>Kleptomancy  [Packaged]</color>", "<color=#2080d0>行窃预兆  [封装级]</color>");
            RegisterString("遗物名称带颜色2-15", "<color=#2080d0>Gravitational Fragmentation  [Packaged]</color>", "<color=#2080d0>引力碎裂  [封装级]</color>");
            RegisterString("遗物名称带颜色2-16", "<color=#2080d0>Hardened Shield  [Packaged]</color>", "<color=#2080d0>刚毅护盾  [封装级]</color>");
            RegisterString("遗物名称带颜色2-17", "<color=#2080d0>Aegis of the Immortal  [Packaged]</color>", "<color=#2080d0>不朽之守护  [封装级]</color>");

            RegisterString("遗物名称带颜色3-0", "<color=#30b530>Inferior processing  [Regular]</color>", "<color=#30b530>劣质加工  [常规级]</color>");
            RegisterString("遗物名称带颜色3-1", "<color=#30b530>Spelltheif's Edge  [Regular]</color>", "<color=#30b530>窃法之刃  [常规级]</color>");
            RegisterString("遗物名称带颜色3-2", "<color=#30b530>Ark Reactor  [Regular]</color>", "<color=#30b530>方舟反应堆  [常规级]</color>");
            RegisterString("遗物名称带颜色3-3", "<color=#30b530>Shepherd of souls  [Regular]</color>", "<color=#30b530>掘墓人  [常规级]</color>");
            RegisterString("遗物名称带颜色3-4", "<color=#30b530>Ctrl 6  [Regular]</color>", "<color=#30b530>装  [常规级]</color>");
            RegisterString("遗物名称带颜色3-5", "<color=#30b530>Resurrection Coin  [Regular]</color>", "<color=#30b530>复活币  [常规级]</color>");
            RegisterString("遗物名称带颜色3-6", "<color=#30b530>Upload  [Regular]</color>", "<color=#30b530>上传  [常规级]</color>");
            RegisterString("遗物名称带颜色3-7", "<color=#30b530>Void Refraction  [Regular]</color>", "<color=#30b530>虚空折射  [常规级]</color>");
            RegisterString("遗物名称带颜色3-8", "<color=#30b530>Matrix Rain  [Regular]</color>", "<color=#30b530>矩阵雨  [常规级]</color>");
            RegisterString("遗物名称带颜色3-9", "<color=#30b530>Tanking  [Regular]</color>", "<color=#30b530>开摆  [常规级]</color>");
            RegisterString("遗物名称带颜色3-10", "<color=#30b530>Hyperactivity I  [Regular]</color>", "<color=#30b530>多动症 I  [常规级]</color>");
            RegisterString("遗物名称带颜色3-11", "<color=#30b530>Activated Carbon I  [Regular]</color>", "<color=#30b530>活性炭 I  [常规级]</color>");
            RegisterString("遗物名称带颜色3-12", "<color=#30b530>Dynamic Giant  [Regular]</color>", "<color=#30b530>灵动巨物  [常规级]</color>");
            RegisterString("遗物名称带颜色3-13", "<color=#30b530>Light Saver I  [Regular]</color>", "<color=#30b530>聚能环 I  [常规级]</color>");
            RegisterString("遗物名称带颜色3-14", "<color=#30b530>Lovely motor  [Regular]</color>", "<color=#30b530>阳间马达  [常规级]</color>");
            RegisterString("遗物名称带颜色3-15", "<color=#30b530>Super Mind  [Regular]</color>", "<color=#30b530>超级大脑  [常规级]</color>");
            RegisterString("遗物名称带颜色3-16", "<color=#30b530>Void Lens  [Regular]</color>", "<color=#30b530>虚空棱镜  [常规级]</color>");
            RegisterString("遗物名称带颜色3-17", "<color=#30b530>Level Up!  [Regular]</color>", "<color=#30b530>升级咯！  [常规级]</color>");
            RegisterString("遗物名称带颜色3-18", "<color=#30b530>Secondary Hardened Shield  [Regular]</color>", "<color=#30b530>次级刚毅护盾  [常规级]</color>");

            RegisterString("遗物名称带颜色4-0", "<color=#00c560>The Weaver  [Axiomatic]</color>", "<color=#00c560>编织者  [公理级]</color>");
            RegisterString("遗物名称带颜色4-1", "<color=#00c560>Contract of Misfortune [Axiomatic]</color>", "<color=#00c560>厄运契约  [公理级]</color>");
            RegisterString("遗物名称带颜色4-2", "<color=#00c560>Crown of Rule  [Axiomatic]</color>", "<color=#00c560>统治之冠  [公理级]</color>");
            RegisterString("遗物名称带颜色4-3", "<color=#00c560>Fatal Echo  [Axiomatic]</color>", "<color=#00c560>精密回响  [公理级]</color>");
            RegisterString("遗物名称带颜色4-4", "<color=#00c560>Enlightenment Echo  [Axiomatic]</color>", "<color=#00c560>启迪回响  [公理级]</color>");
            RegisterString("遗物名称带颜色4-5", "<color=#00c560>Aftershock Echo  [Axiomatic]</color>", "<color=#00c560>余震回响  [公理级]</color>");
            RegisterString("遗物名称带颜色4-6", "<color=#00c560>Rune Book  [Axiomatic]</color>", "<color=#00c560>符文之书  [公理级]</color>");
            RegisterString("遗物名称带颜色4-7", "<color=#00c560>Void Echo  [Axiomatic]</color>", "<color=#00c560>虚空回响  [公理级]</color>");

            RegisterString("遗物描述0-0", "Each time a certain number of dark fog units are killed, a random mega structure will be partially auto-constructed", "每击杀一定数量的黑雾单位，略微推进随机星系的巨构的建造进度");
            RegisterString("遗物描述0-1", "When assembling recipes with at least 2 different materials (except when assembling antimatter fuel rods) in assembling machine or Star Assembly, every time a product is produced, one material in the first slot will be returned.", "制造厂和星际组装厂在制造原材料至少2种的配方时（反物质燃料棒的产线除外），每产出1个产物，会返还1个第1位置的原材料");
            RegisterString("遗物描述0-2", "When the sorter picks products from smelter, assembling machines, chemical plants, or particle colliders, spray them with proliferator Mk.III for free", "分拣器从熔炉、制造台、化工厂和粒子对撞机取出产物时，为他们免费喷涂三级增产剂");
            RegisterString("遗物描述0-3", "When calculating the energy level, the giant structure is regarded as a higher star luminosity", "巨构在计算能量水平时，视作拥有更高的恒星光度修正");
            RegisterString("遗物描述0-4", "The ray receiver does not need to consume the lens to achieve the maximum output efficiency, and it will no longer be blocked at night", "射线接受器无需消耗透镜即可达到最大输出效率，且不再因背向恒星影响接收效率");
            RegisterString("遗物描述0-5", "Planetary shield and Icarus' Energy Shield gain 10% damage reduction, and they return all reduced or avoided damage to the attacker as <i>additional damage</i>", "行星护盾和伊卡洛斯的能量盾获得10%伤害减免，且它们会将所有被护盾减免或被护盾规避的伤害全额回敬给攻击者作为<i>额外伤害</i>");
            RegisterString("遗物描述0-6", "Each time a turret uses an ammo set to reload, do free reloading and prevent the ammo set consumption", "所有防御设施的每次消耗弹药组装填时，进行免费装填而阻止消耗弹药组");
            RegisterString("遗物描述0-7", "The star system with a megastructure will deal damage (considered as <i>additional damage</i>) to all activated dark fog space ships in the star system, higher energy the megastucture generates, higher the damage it deals", "拥有巨构的星系在战斗时每秒会对星系中所有已激活的太空黑雾舰队造成伤害（视为<i>额外伤害</i>），伤害取决于巨构的能量水平");
            RegisterString("遗物描述0-8", "When enemies are interfered by non-Icarus-throwing jamming capsules, they take <i>additional damage</i>. Jammer tower won't consume bullets after loaded.", "非投掷的干扰胶囊造成电磁干扰时，对所有命中目标造成<i>额外伤害</i>。干扰塔在装弹后不再消耗弹药。");
            RegisterString("遗物描述0-9", "It must do something...", "它必须做点什么...");
            RegisterString("遗物描述0-9实际", "You have a higher probability of getting rarer meta drivers. If the first judgment fails with any probability from other meta drivers, it can be judged again. And the hidden effects...", "你有更高的可能性获取更稀有的元驱动。任何概率初次判定失败时，可以再判定一次。以及隐藏效果...");
            RegisterString("遗物描述0-10", "Every time a droplet destroys an enemy, restores 2MJ power to the Mecha, and all droplets permanently obtain 10 <i>additional damage</i>. ", "水滴每击杀一个敌人，为机甲回复2MJ能量，且所有水滴永久获得+10的<i>额外伤害</i>。");

            RegisterString("遗物描述1-0", "Every time you unlock or upgrade a technology, random mega structures will be partially auto-constructed", "每解锁或升级一个科技，少量推进随机巨构的建造进度");
            RegisterString("遗物描述1-1", "Turret supernova duration +200", "防御塔的超新星持续时间+200%");
            RegisterString("遗物描述1-2", "When the planetary shield is broken, immediately restore full shield and prevent it from recharge for 10 minutes", "行星护盾被打破时，立刻回复全部的护盾，并阻止其继续充能10min");
            RegisterString("遗物描述1-3", "For every 1% of mega structure's energy stolen by dark fog, space dark fog units in that star system take 3% <i>additional damage</i>", "巨构能量每被窃取1%，该星系的太空黑雾单位受到3%的<i>额外伤害</i>");
            RegisterString("遗物描述1-3+", "For every 1% of mega structure's energy stolen by dark fog, space dark fog units in that star system take 3% <i>additional damage</i>", "巨构能量每被窃取1%，该星系的太空黑雾单位受到<color=#d2853d>5%</color>的<i>额外伤害</i>");
            RegisterString("遗物描述1-4", "Unlock Boson control Technology immediately and give 3 droplet, droplets energy consumption -50%", "立刻解锁水滴科技，并获得3个水滴，水滴能量消耗-50%");
            RegisterString("遗物描述1-4+", "Unlock Boson control Technology immediately and give 3 droplet, droplets energy consumption <color=#d2853d>-60%</color>", "立刻解锁水滴科技，并获得3个水滴，水滴能量消耗<color=#d2853d>-60%</color>");
            RegisterString("遗物描述1-5", "Each time a turret uses an ammo set to reload, there is 35% chance to restore 2 identical ammo sets", "所有防御设施在装填一组弹药时，有35%概率回填两组完全相同的弹药");
            RegisterString("遗物描述1-6", "When producing megastructure rockets, each output returns 2 deuteron fuel rods", "生产巨构火箭时，每个产出返还2个氘核燃料棒");
            RegisterString("遗物描述1-7", "Solar sail absorption speed increases 300%, ejecting speed increases 100%", "巨构的太阳帆吸附速度提升300%，弹射器弹射太阳帆的速度提升100%");
            RegisterString("遗物描述1-8", "When Icarus' Energy shield is broken, depleting the Mech's fuel chamber to instantly restore up to 100% energy shield", "伊卡洛斯的能量盾被打破时，消耗机甲燃烧室储备立刻回复最多100%的能量盾");
            RegisterString("遗物描述1-9", "When an ally fleet unit is about to take damage, if Icarus' shield is charged above 50%, Icarus will take the damage instead", "我方舰队中的单位将要承受伤害时，如果伊卡洛斯的能量盾充能高于50%，则由其代为承担");
            RegisterString("遗物描述1-9+", "When an ally fleet unit is about to take damage, if Icarus' shield is charged above 50%, Icarus will take the damage instead, <color=#d2853d>and the damage will be reduced by 50%</color>", "我方舰队中的单位将要承受伤害时，如果伊卡洛斯的能量盾充能高于50%，则由其代为承担，<color=#d2853d>且伤害降低50%</color>");
            RegisterString("遗物描述1-10", "Ally damage will permanently ignore the armor of the dark fog unit. Won't occupy the meta driver slot", "友方伤害将永久无视黑雾单位的护甲，不占用元驱动槽位");
            RegisterString("遗物描述1-11", "Greatly enhanced jamming tower's effect", "大幅强化干扰塔的效果");
            RegisterString("遗物描述1-12", "If Icarus is on the planet and hold still for more than 4 min, the void assimilation countdown will stop", "伊卡洛斯在行星上保持静止超过4min后，虚空同化将停止计时");
            RegisterString("遗物描述1-12+", "If Icarus is on the planet and hold still for more than <color=#d2853d>10 second</color>, the void assimilation countdown will stop", "伊卡洛斯在行星上保持静止超过<color=#d2853d>10s</color>后，虚空同化将停止计时");

            RegisterString("遗物描述2-0", "Planetary Shields gain 50% additional charge energy", "行星护盾获得50%额外的充能量");
            RegisterString("遗物描述2-1", "Each time your merit rank is promoted, random mega structures will be partly auto-constructed", "每次提升功勋阶级，显著推进各巨构的建造进度");
            RegisterString("遗物描述2-2", "Get merit points when allied buildings destroyed", "建筑被摧毁时，获得功勋点数");
            RegisterString("遗物描述2-3", "Each time a turret uses an ammo set to reload, there is 40% chance to restore an identical ammo set", "所有防御设施在装填时有40%概率回填一组弹药");
            RegisterString("遗物描述2-4", "When producing normal fuel rods, each output will returne 5 materials in the second slot", "生产常规燃料棒时，每次产出会回填5个第2位置的原材料（氢、重氢）");
            RegisterString("遗物描述2-5", "Every second, if Icarus is on the planet and have moved in the previous second, you have 8% chance to obtain a multi-functional integrated component", "每过一秒，如果伊卡洛斯处于行星上并且在上一秒进行过移动，就有8%的概率获得一个多功能集成组件");
            RegisterString("遗物描述2-6", "All your fleets energy consumption -40%", "战斗无人机各项能量消耗-40%");
            RegisterString("遗物描述2-7", "Kinetic, energy and explosive damage +10%. Won't occupy the meta driver slot", "动能武器伤害、爆破武器伤害、能量武器伤害+10%，不占用元驱动槽位");
            RegisterString("遗物描述2-8", "When decomposing critical photons, hydrogen is no longer produced, but the antimatter production increase 50%", "分解临界光子时，不再产出氢，但产出的反物质增加50%");
            RegisterString("遗物描述2-9", "Star cannon's recharging speed +50%", "恒星炮充能速度+50%");
            RegisterString("遗物描述2-10", "Turret supernova cooldown and charge time -50%", "防御塔的超新星冷却和蓄能时间-50%");
            RegisterString("遗物描述2-11", "Smelter have 30% chance to produce an additional product", "熔炉每次产出，有30%的概率额外产出一个产物");
            RegisterString("遗物描述2-12", "You gain +10% chance of critical hit", "你获得10%暴击几率");
            RegisterString("遗物描述2-13", "Double any <i>additional damage</i> (except the bonus from technology)", "你对黑雾造成的任何<i>额外伤害翻倍</i>（来自科技的加成除外）");
            RegisterString("遗物描述2-14", "Every time you destroy an enemy, you have chance to directly obtain an antimatter fuel rod or a space warper in the backpack", "每次击毁敌军单位，根据敌人强度有概率在背包直接获取1个反物质燃料棒或翘曲器，无视科技解锁进度");
            RegisterString("遗物描述2-14+", "Every time you destroy an enemy, you have chance to directly obtain an <color=#d2853d>strange annihilation fuel rod</color> or a space warper in the backpack", "每次击毁敌军单位，根据敌人强度有概率在背包直接获取1个<color=#d2853d>奇异湮灭燃料棒</color>或翘曲器，无视科技解锁进度");
            RegisterString("遗物描述2-15", "Explosive damage +10%. Won't occupy the meta driver slot", "爆破武器伤害+40%，不占用元驱动槽位");
            RegisterString("遗物描述2-16", "Planetary Shield and Icarus Shield gain 20% damage reduction", "行星护盾和伊卡洛斯的护盾获得20%伤害减免");
            RegisterString("遗物描述2-17", "When Icarus is about to be destroyed, restore all health and energy shields instead, then gains invincible for 30 seconds", "伊卡洛斯即将被摧毁时，转而立刻回复全部的生命值和能量盾，并获得30s的伤害免疫");
            RegisterString("遗物描述2-17+", "When Icarus is about to be destroyed, restore all health and energy shields instead, then gains invincible <color=#d2853d>and +1500% global damage bonus</color> for 30 seconds", "伊卡洛斯即将被摧毁时，转而立刻回复全部的生命值和能量盾，并获得30s的伤害免疫<color=#d2853d>和1500%全局伤害加成</color>");

            RegisterString("遗物描述3-0", "Solar sail life -90%, but its production amount in assembling machine +50%", "太阳帆寿命-90%，但每次产出太阳帆会额外产出1个太阳帆");
            RegisterString("遗物描述3-1", "When enemy drops dark fog matrices, there is 30% chance to double them", "地面黑雾单位掉落黑雾矩阵时，有30%几率掉落双倍的黑雾矩阵");
            RegisterString("遗物描述3-2", "Icarus will generate additional energy without consuming fuel, which is equivalent to 50% of the base power of the fuel generation reactor", "伊卡洛斯会不消耗燃料地持续获得额外的能量回复，相当于反应堆基础功率的50%");
            RegisterString("遗物描述3-3", "Double the sand amount dropped by dark fog units", "黑雾单位掉落的沙土加倍");
            RegisterString("遗物描述3-4", "Dark fog gain +100% extra experience", "黑雾获得经验+100%");
            RegisterString("遗物描述3-5", "When Icarus is destroyed, give you options to use resurrection coins to redeploy or reassemble for free. Won't occupy the meta driver slot", "伊卡洛斯被毁时，可以选择消耗复活币来无消耗地重新部署或原地重组，不占用元驱动槽位");
            RegisterString("遗物描述3-6", "Gain some merit points based on your current rank level. Won't occupy the meta driver slot", "获得基于当前阶级的少量功勋点数，不占用元驱动槽位");
            RegisterString("遗物描述3-7", "Energy weapon damage +10%. Won't occupy the meta driver slot", "能量武器伤害+10%，不占用元驱动槽位");
            RegisterString("遗物描述3-8", "Based on the unlocked matrix technology, immediately give a large number of normal matrix. Won't occupy the meta driver slot", "基于已解锁的矩阵科技，立刻获得大量普通矩阵（黑雾矩阵和宇宙矩阵除外），不会占用元驱动槽位");
            RegisterString("遗物描述3-9", "When a certain number of buildings are destroyed, random mega structure will be slightly auto-constructed", "一定数量的建筑被毁时，微量推进随机星系的巨构的建造进度");
            RegisterString("遗物描述3-10", "Every second, if Icaros is on the planet and have moved in the previous second, you have 3% chance to obtain a multi-functional integrated component", "每过一秒，如果伊卡洛斯处于行星上并且在上一秒进行过移动，就有3%的概率获得一个多功能集成组件");
            RegisterString("遗物描述3-11", "Solar sial adsorption speed increases 100%", "巨构的太阳帆吸附速度提升100%");
            RegisterString("遗物描述3-12", "Planetary shield has a 15% chance to avoid the damage", "行星护盾有15%的概率完全规避伤害");
            RegisterString("遗物描述3-13", "Star cannon's recharging speed +25%", "恒星炮充能速度提高25%");
            RegisterString("遗物描述3-14", "When producing electric motors or electromagnetic turbines , every time a product is produced, one magnetic coil will be returned", "生产电动机、电磁涡轮时，每生产一个产物，回填1个磁线圈作为原材料");
            RegisterString("遗物描述3-15", "The research speed of Icarus mecha +400%, and research energy consumption +400%", "伊卡洛斯机甲的研究速度+400%，研究能耗同步增加");
            RegisterString("遗物描述3-16", "Star cannon deals 10% <i>additional damage</i>", "恒星炮造成10%<i>额外伤害</i>");
            RegisterString("遗物描述3-17", "You get 25% extra merit points", "功勋点数获取+25%");
            RegisterString("遗物描述3-17+", "You get <color=#d2853d>40%</color> extra merit points", "功勋点数获取<color=#d2853d>+40%</color>");
            RegisterString("遗物描述3-18", "Planetary Shield gain 10% damage reduction", "行星护盾获得10%伤害减免");

            RegisterString("遗物描述4-0", "The mega structure in the star system with the max luminosity in the galaxy, will be constantly auto-constructing", "星区中光度最高的恒星系的巨构会不停地自动建造");
            RegisterString("遗物描述4-1", "Every time you interpret a meta driver, at least one Mensural decoding track will be available before you reroll. Reroll cost is halved", "每次解译完毕元驱动时，必然刷新一个定律级解码轨，重随消耗减半");
            RegisterString("遗物描述4-2", "Each droplet fleet configuration can have 3 droplets", "每个水滴的舰队配置可放置3个水滴");
            RegisterString("遗物描述4-3", "Comprehensively improve the extra products effect of proliferators", "全面提升增产剂的增产效果");
            RegisterString("遗物描述4-4", "Every an enemy is destoryed, slightly advance the research progress of the current non-darkfog matrix technology for free", "每当击杀敌军单位时，无消耗地略微推进当前非黑雾矩阵科技的研究进度");
            RegisterString("遗物描述4-5", "When killing a ground dark fog unit, there is 15% chance of causing a weakened electromagnetic interference from it's place", "地面黑雾单位被击杀时，有15%概率在原地引发一次弱化的电磁干扰");
            RegisterString("遗物描述4-5+", "When killing a ground dark fog unit, there is 15% chance of causing a <color=#d2853d>enhanced</color> electromagnetic interference from it's place", "地面黑雾单位被击杀时，有15%概率在原地引发一次<color=#d2853d>强化的</color>电磁干扰");
            RegisterString("遗物描述4-6", "When pick up this meta driver, permanently record the top three meta drivers that you already have, retaining their effects but no longer occupying the slot", "获取此元驱动时，将左侧栏位最顶端的三个元驱动永久保存在符文之书中，保留他们的效果但是其不再占用栏位");
            RegisterString("遗物描述4-7", "Each time you click this meta driver, delay the assimilation progress of the void by 60s. Hold Ctrl and click to advance the progress instead. ", "每次点击此元驱动，立即使虚空的同化进度延缓一分钟。按住Ctrl点击则反而推进一分钟。");


            RegisterString("relicTipTitle0-10", "Upper Limit", "上限");
            RegisterString("relicTipText0-10", "The upper limit is 200 at the begining. After reaching the upper limit, the mecha will automatically consume a droplet in inventory then increase the upper limit by 200. The upper limit growth is unlimited\nThe type of additional damage is is consistent with the original damage type", "加成上限初始为200，达到上限后，自动消耗背包中的一个水滴并再次提升200加成上限，提升上限的次数不受限制\n额外伤害的类型与原有伤害的类型一致");
            RegisterString("relicTipTitle1-2", "Cooldown", "冷却时间");
            RegisterString("relicTipText1-2", "This effect has a separate 10min cooldown for each planet", "这个效果对每个行星具有独立的10分钟的冷却时间");
            RegisterString("relicTipTitle1-8", "Energy consumption", "能量消耗");
            RegisterString("relicTipText1-8", "It takes twice chamber fuel's energy to recharge the shield power. If this effect is triggered repeatedly within 1 minute, the energy consumption multiplier will rapidly increase to a maximum of 20 times", "需要消耗相当于回复护盾能量2倍的燃烧室燃料。如果在1分钟内反复触发此效果，能量消耗倍率会快速增长至最高20倍");
            RegisterString("relicTipTitle1-12", "Hold still", "保持静止");
            RegisterString("relicTipText1-12", "Once moved, the held still time will reset", "一旦移动，保持静止的计时时间将重置");
            RegisterString("relicTipTitle2-12", "Critical hit", "暴击");
            RegisterString("relicTipText2-12", "Critical hit will deal 100% additional damage", "暴击会对目标造成100%额外伤害");
            RegisterString("relicTipTitle2-16", "Damage reduction", "伤害减免");
            RegisterString("relicTipText2-16", "If a single damage exceeds 1% of the current shield amount, it will be reduced by 80% instead", "如果单次伤害超过了当前护盾量的1%，则转而减免80%");
            RegisterString("relicTipTitle2-17", "Cooldown", "冷却时间");
            RegisterString("relicTipText2-17", "This effect can only be triggered once every 20 minutes", "这个效果每20分钟只能触发一次");

            RegisterString("relicTipText4-0", "All other galaxies receive a negative luminosity correction", "所有其他星系获得一个负的恒星光度修正");
            RegisterString("relicTipText4-1", "All meta drivers with probability, their probability is halved", "所有具有概率的效果，判定成功几率减半");
            RegisterString("relicTipText4-2", "When Icarus is destroyed, downgrade your merit rank level by 1, and clear your merit points of current level", "死亡时，你降低一级功勋阶级并清空当前等级的功勋点数");
            RegisterString("relicTipText4-3", "Comprehensively reduce the production speedup effect of proliferators", "全面降低增产剂的加速效果");
            RegisterString("relicTipText4-4", "The amount of dark fog matrix drop is halved, and you won't obtain any dark fog matrix when you abort interpreting the decoding track\n(Hash gaining can be enhanced by the research speed technology)", "黑雾矩阵掉落减半，放弃解译元驱动的解码轨时不会获得黑雾矩阵\n(Hash点数获取量受研究速度科技加成)");
            RegisterString("relicTipText4-5", "You will get 90% less merit points by killing ground dark fog units", "击杀地面黑雾单位获得的功勋点数-90%");
            RegisterString("relicTipText4-6", "The required time to interpret a meta driver +15min\nCannot record meta drivers that can be interacted with by clicking", "解译元驱动的所需时间增加15分钟\n无法记载可点击交互的元驱动");
            RegisterString("relicTipText4-7", "You can delay 20 minutes at most for each void invasion. The COSMO Technology Ethics Committee will dissatisfied with your behavior of taking the advantage of void power. The Void invasion wave which you used this meta driver's active effect can only earn you 2 authorization point rewards at most", "每次虚空入侵最多被延缓20分钟。COSMO技术伦理委员会不满你利用虚空力量的行为，使用过此元驱动主动效果的那次虚空入侵在结束时最多只能收到2点授权点奖励");

            RegisterString("relicTipTitle0-5", "Additional damage", "额外伤害");
            RegisterString("relicTipTitle0-7", "Additional damage", "额外伤害");
            RegisterString("relicTipTitle0-8", "Additional damage", "额外伤害");
            RegisterString("relicTipTitle1-3", "Additional damage", "额外伤害");
            RegisterString("relicTipTitle2-12", "Additional damage", "额外伤害");
            RegisterString("relicTipTitle2-13", "Additional damage", "额外伤害");
            RegisterString("relicTipTitle3-16", "Additional damage", "额外伤害");

            RegisterString("relicTipText0-5", "The type of additional damage is void damage.", "该额外伤害的类型为虚空伤害。");
            RegisterString("relicTipText0-7", "The type of additional damage is energy damage. \nIf the megastructure is star cannon, then the additional damage increases 100%.", "该额外伤害的类型为能量伤害。如果巨构为恒星炮，该伤害增加100%");
            RegisterString("relicTipText0-8", "Enemies take 20 EM damage. Take 30 if it is suppresing capsules.", "敌人会受到20电磁伤害，压制胶囊则转而造成30电磁伤害。");
            RegisterString("relicTipText1-3", "The type of additional damage is consistent with the original damage type.", "额外伤害类型与原有的伤害的类型一致。");
            RegisterString("relicTipText2-12", "The type of additional damage is consistent with the original damage type.", "额外伤害类型与原有的伤害的类型一致。");
            RegisterString("relicTipText2-13", "Any additional damage described in the meta drivers will be increased.", "任何来自元驱动中所描述的额外伤害都将被增幅。");
            RegisterString("relicTipText3-16", "The type of additional damage is consistent with the original damage type.", "额外伤害类型与原有的伤害的类型一致。");

            RegisterString("铲子强化后字", "\n\n<color=#d2853d>Has been spatulled in shiny glow</color>", "\n\n<color=#d2853d>镀上了铲亮的光芒</color>");

            RegisterString("当前倍率", "Current factor", "当前倍率");
            RegisterString("剩余冷却时间gm", "Cooling down", "冷却中");
            RegisterString("冷却完毕gm", "Ready", "已就绪");
            RegisterString("消退于", "Fading in", "消退于");

            RegisterString("显示/隐藏", "Show/Hide", "显示/隐藏");
            RegisterString("模块容量", "Module Capacity", "模块容量");
            RegisterString("模块容量说明", "Module capacity limits the number of defensive modules that you can build in this Star Fortress. Planning missile modules and light spear modules will consume available module capacity.\nThere are two ways to increase module capacity: \n1. Build bigger mega structure will provide more capacity; \n2. Building platform expansion modules will also increase module capacity, but the megastructure must have at least 600MW before its expansion modules work.", "模块容量决定着你可以在这个恒星要塞建造的防御性模块的数量，规划导弹模块和光矛模块都将消耗可用的模块容量，提升模块容量的方式有两个：\n1.构建更多的巨构框架和壳面将提升模块容量；\n2.在至少建造了一定能量水平的巨构后，建造平台扩展模块也会提升模块容量。");
            RegisterString("组件需求", "Component points", "组件点数");
            RegisterString("导弹模块", "Missile Mod.", "导弹模块");
            RegisterString("导弹模块说明", "The missile module will launch special antimatter missiles to attack enemy ships in combat. The antimatter missiles it uses are independently produced by the module in a stellar environment, requiring no ground supply of ammunition, and benefiting from technology upgrade. \nUse the buttons to increase or decrease the planned number, and then launch the corresponding rocket from the ground to provide component points to gradually build the module. \nHolding \'Ctrl\' to make the button 10x times.", "    导弹模块会在战斗中发射反物质导弹攻击敌舰，其使用的反物质导弹是在恒星环境下由模块自主生产的，无需地面供给弹药，且享受来自科技的加成效果。\n    使用按钮来增加或减少建造的数量，然后从地面发射对应的运载火箭提供组件点数来逐步建造模块。\n    按住Ctrl将使按钮变为10倍。");
            RegisterString("光矛模块", "Lightspear Mod.", "光矛模块");
            RegisterString("光矛模块说明", "The Light Spear module can fire light spears with up to 20000 damage in combat, and can be enhanced by technology upgrades just like Phase-cracking beam. Building more Light Spear modules will accelerate the charging speed of the Light Spear and thus increase the fire rate (non-linear, max 10/s). \nUse the buttons to increase or decrease the planned number, and then launch the corresponding rocket from the ground to provide component points to gradually build the module. \nHolding \'Ctrl\' to make the button 10x times.", "    光矛模块会在战斗中发射伤害高达20000的光矛攻击敌舰，且享受与相位裂解光束等同的科技加成。建造更多的光矛模块会加快光矛的充能速度从而增加射速(非线性，最大10/s)。\n    使用按钮来增加或减少建造的数量，然后从地面发射对应的运载火箭提供组件点数来逐步建造模块。\n    按住Ctrl将使按钮变为10倍。");
            RegisterString("平台扩展模块", "Expansion Mod.", "扩展模块");
            RegisterString("平台扩展模块说明", "Each platform expansion module will provide module capacity to enable you to build large defense platforms on low completion megastructures. But the megastructure must have at least 600MW before the expansion modules can work. \nThe platform expansion modules that are already built cannot be removed.\nUse the buttons to increase or decrease the planned number, and then launch the corresponding rocket from the ground to provide component points to gradually build the module. \nHolding \'Ctrl\' to make the button 10x times.", "    每个平台扩展模块会提供（而非消耗）模块容量，以便使你能在低完成度的巨构上构建大型防御平台。\n    已建成的平台扩展模块无法拆除。");
            RegisterString("警告扩展模块无效", "Notice: The expansion module cannot take effect until the total energy provide by the megastructure's layers reaches 600MW.", "注意：在巨构壳层与框架能量水平总和达到一个600MW之前，扩展模块无法生效。");
            RegisterString("恒星要塞容量不足警告", "The module capacity is insufficient! Please expand the mega structure or plan more expansion modules on star fortress.", "恒星要塞模块容量不足！请扩大巨构或规划更多的扩展模块。");
            RegisterString("sf组件1火箭", "Missile module carrier rocket", "导弹模块组件运载火箭");
            RegisterString("sf组件1火箭描述", "Use the Vertical Launching Silo to launch this rocket onto the Star Fortress, carring necessary components to build the missile module. This will fill the component points of the missile module on the Star Fortress, but it is not helpful for the construction of any mega structures.", "使用发射井发射此火箭来将构建导弹模块的必须组件发射到恒星要塞上，这会填充恒星要塞的导弹模块的组件点数，但无法推进任何巨构的建造。");
            RegisterString("sf组件2火箭", "Light spear module carrier rocket", "光矛模块组件运载火箭");
            RegisterString("sf组件2火箭描述", "Use the Vertical Launching Silo to launch this rocket onto the Star Fortress, carring necessary components to build the light spear module. This will fill the component points of the light spear module on the Star Fortress, but it is not helpful for the construction of any mega structures.", "使用发射井发射此火箭来将构建光矛模块的必须组件发射到恒星要塞上，这会填充恒星要塞的光矛模块的组件点数，但无法推进任何巨构的建造。");
            RegisterString("sf组件3火箭", "Expansion module carrier rocket", "扩展模块组件运载火箭");
            RegisterString("sf组件3火箭描述", "Use the Vertical Launching Silo to launch this rocket onto the Star Fortress, carring necessary components to build the expansion module. This will fill the component points of the expansion module on the Star Fortress, but it is not helpful for the construction of any mega structures.", "使用发射井发射此火箭来将构建扩展模块的必须组件发射到恒星要塞上，这会填充恒星要塞的扩展模块的组件点数，但无法推进任何巨构的建造。");
            RegisterString("即将拆除模块标题", "Warning! Destructing modules!", "警告！即将拆除模块");
            RegisterString("即将拆除模块警告", "Since the module upper limit will be less than the number of completed modules, the overflowing completed modules will be immediately removed, and the module points will be wasted. Are you sure you want to remove them?", "由于模块上限被调整后将少于已建成的模块数，溢出的已建成的模块将被立刻拆除，模块点数将被浪费。是否确认拆除？");

            RegisterString("深空来敌介绍1标题", "They come from void player guide", "深空来敌玩法介绍");
            RegisterString("深空来敌介绍1前字", "Nothing here for now. You can check the meta driver infomation below.", "这里暂时没有东西。你可以翻看下面的元驱动介绍。");

            RegisterString("深空来敌介绍2前字", "By destroying dark fog units, you can gain experience points to promote your merit rank, and accumulate combat experience to strengthen yourself. Every time you reach the new merit rank, you will not only receive authorization points award issued by the COSMO Technology Ethics Committee, but also permanent bonus effects. Authorization points can be assigned on demand by clicking on the merit rank icon in the upper right corner, some providing bonuses for your production line, while others strengthening your combat capabilities. You can also view the bonus effect of the current rank, the experience requirements of the next level, and the bonus effect of the next level by hovering the mouse over the merit rank icon in the upper right corner. After advancing to the final rank, you will still be able to continuously earn merit points to get authorization points from the COSMO Technology Ethics Committee.\n\nExplorer I: Core power generation +1 MW\nExplorer II: Energy shield damage reduction +25%\nExplorer III: Walk speed +4 m/s\nPioneer I: Ore loss per mining operation -20%\nPioneer II: EM effect +20%\nPioneer III: Droplets can quickly approach distant target\nConqueror I: Ore loss per mining operation -20%\nConqueror II: Star cannon charging speed +50%\nConqueror III: Droplet damage +100%\nGalaxy Guardian: Ore loss per mining operation  -40%", "通过击毁黑雾单位，你可以获得经验点数来提升功勋阶级，并积累战斗经验来强化自身。每次提升功勋阶级，你除了会收到由COSMO技术伦理委员会发放的授权点奖励外，还将获得固定的永久加成效果。授权点奖励可以通过点击右上角的功勋阶级图标进行按需分配，有些为你的生产线提供加成，有些则强化你的战斗能力。你也可以通过将鼠标悬停在右上角的功勋阶级图标上来查看当前等级的加成效果，下一等级的经验需求和下一等级的加成效果。在提升到最终等级后，你仍然可以不断获取功勋点数，以不断从COSMO技术伦理委员会处获取授权点奖励。\n\n探索者 I: 核心发电 +1MW\n探索者 II: 能量盾伤害减免 +25%\n探索者 III: 步行移动速度 +4 m/s\n开拓者 I: 采矿消耗 -20%\n开拓者 II: 电磁武器效果 +20%\n开拓者 III: 水滴能够快速接近远距离的目标\n征服者 I: 采矿消耗 -20%\n征服者 II: 恒星炮充能速度 +50%\n征服者 III: 水滴伤害 +100%\n星河卫士: 采矿消耗 -40%");

            RegisterString("深空来敌介绍5标题", "Meta-Drive", "元驱动");
            RegisterString("深空来敌介绍6标题", "Axiomatic Meta-Drive", "公理级元驱动");

            ProtoRegistry.RegisterItem(8035, "水滴伤害增加", "", "Assets/DSPBattle/r0-10", 9999, 100, EItemType.Material);


            RegisterString("事件链窗口标题", "Meta Driver Event Chain", "元驱动事件链");
            RegisterString("执行此决定你", "Choose this decision", "执行此决定");
            RegisterString("需要gm", "<color=#61d8ffb4>Prerequisite:</color>", "<color=#61d8ffb4>需要:</color>");
            RegisterString("功勋阶级达到", "Merit rank level", "功勋阶级达到");
            RegisterString("伊卡洛斯被摧毁次数", "Icarus destroyed time", "伊卡洛斯被摧毁次数");
            RegisterString("消灭地面黑雾", "Kill ground dark fog units", "消灭地面黑雾单位");
            RegisterString("消灭太空黑雾", "Kill space dark fog units", "消灭太空黑雾单位");
            RegisterString("消灭任意黑雾", "Kill dark fog units", "消灭任意黑雾单位");
            RegisterString("提供物品", "Provide ", "提供 ");
            RegisterString("物品产量", "Produce speed of ", "物品产量:");
            RegisterString("解锁任意科技", "Unlock/Upgrade any technology", "解锁/升级任意科技");
            RegisterString("解锁gm", "Upgrade ", "升级");
            RegisterString("至等级", " to level", "至等级");
            RegisterString("消灭恒星系全部地面单位", "Kill all ground enemy in {0}{1}\n    remaining:{2}", "消灭恒星系{0}的全部地面单位{1}\n    剩余 {2}");
            RegisterString("数量未知gm", "unknown, please land on every planet in that star system at least once", "未知，请先降落该恒星系的所有行星至少一次");
            RegisterString("数量未知gm2", "unknown, please land on the planet at least once", "未知，请先降落该行星至少一次");
            RegisterString("点击以导航", " (click to navigate)", "(点击以导航)");
            RegisterString("点击以导航到", "Click to navigate to {0}", "点击以导航到 {0}");
            RegisterString("消灭恒星系地面单位", "Kill ground enemy in {0}:  {1}/{2} {3}", "消灭恒星系{0}的地面单位  {1}/{2} {3}");
            RegisterString("消灭恒星系全部太空单位", "Kill all space enemy in {0}{1}\n    remaining:{2}", "消灭恒星系{0}的全部太空单位{1}\n    剩余 {2}");
            RegisterString("消灭恒星系太空单位", "Kill space enemy in {0}:  {1}/{2} {3}", "消灭恒星系{0}的太空单位  {1}/{2} {3}");
            RegisterString("提升恒星系威胁等级", "Raise any DF space hive's threat level in {0}  {1}/{2} {3}", "提升恒星系{0}的任意一个太空黑雾巢穴的威胁等级  {1}/{2} {3}");
            RegisterString("肃清恒星系", "Kill all enemies in {0}{1}\n    remaining:{2}", "清理恒星系{0}的全部黑雾单位{1}\n    剩余 {2}");
            RegisterString("提升巨构能量水平", "In {0} star system, build megastructure with energy level  {1}/{2} GW {3}", "提升{0}恒星系的巨构能量水平  {1}/{2} GW {3}");
            RegisterString("任意gm", "any", "任意");
            RegisterString("提升太空黑雾巢穴等级", "Any DF space hive in {0} reaches level  {1}/{2} {3}", "使{0}中任意一个太空黑雾巢穴等级达到  {1}/{2} {3}");
            RegisterString("消灭太空黑雾巢穴的所有单位", "Kill all enemies of {1} in {0}  remaining:{2} {3}", "消灭{0}中{1}的所有单位  剩余{2} {3}");
            RegisterString("消灭行星全部黑雾单位", "Kill all DF ground enemies on {0}{1}\n    remaining:{2}", "消灭行星{0}上的全部地面黑雾单位{1}\n    剩余 {2}");
            RegisterString("消灭行星黑雾单位", "Kill DF ground enemies on {0}  {1}/{2} {3}", "消灭行星{0}上的地面黑雾单位  {1}/{2} {3}");
            RegisterString("消灭行星全部黑雾基地", "Kill all DF ground bases on {0}{1}\n    remaining:{2}", "消灭行星{0}上的全部地面黑雾巢穴{1}\n    剩余 {2}");
            RegisterString("到达行星gm", "Arrive on planet {0}  {1}", "到达行星{0}  {1}");
            RegisterString("已到达gm", "accomplished", "已完成");
            RegisterString("这将终止序列", "<color=#cc2020c0>End this event chain</color>", "<color=#cc2020c0>此事件链将终止</color>");
            RegisterString("未知后果", "Unknow consequences", "未知后果");
            RegisterString("解译元驱动", "Interpret meta driver", "解译元驱动");
            RegisterString("获得功勋点数", "Obtain merit points ", "获得功勋点数 ");
            RegisterString("失去功勋点数", "Lose merit points ", "失去功勋点数");
            RegisterString("提升功勋阶级", "Upgrade merit rank ", "提升功勋阶级 ");
            RegisterString("降低功勋阶级", "Downgrade merit rank ", "降低功勋阶级 ");
            RegisterString("推进随机巨构", "Auto-constructed random megastructures construct points", "推进星区中随机巨构的建造进度");
            RegisterString("本次圣物解译普通概率", "Regular decode-tracks appearing probability this time ", "本次解译出常规级解码轨的概率 ");
            RegisterString("本次圣物解译稀有概率", "Packaged decode-tracks appearing probability this time ", "本次解译出封装级解码轨的概率 ");
            RegisterString("本次圣物解译史诗概率", "Framed decode-tracks appearing probability this time ", "本次解译出框架级解码轨的概率 ");
            RegisterString("本次圣物解译传说概率", "Mensural decode-tracks appearing probability this time ", "本次解译出定律级解码轨的概率 ");
            RegisterString("本次圣物解译被诅咒的概率", "Axiomatic decode-tracks appearing probability this time ", "本次解译出公理级解码轨的概率 ");
            RegisterString("免费随机次数", "Decode-track free re-roll ", "本次解码轨免费随机次数 ");
            RegisterString("获得物品", "Obtain ", "获得 ");
            RegisterString("此选项将导致", "<color=#FD965EC0>Result in:</color>", "<color=#FD965EC0>将导致：</color>");
            if (MoreMegaStructure.MoreMegaStructure.GenesisCompatibility)
                RegisterString("打开解译事件链", "Open meta driver interpretation event chain ( Ctrl + ~ )", "打开元驱动解译事件链 ( Ctrl + ~ )");
            else
                RegisterString("打开解译事件链", "Open meta driver interpretation event chain ( ~ )", "打开元驱动解译事件链 ( ~ )");

            RegisterString("预计剩余解译时间", "Estimated time of finishing", "预计剩余");
            RegisterString("decodeType21Title", "Analyzing Log File", "正在分析日志");
            RegisterString("decodeType22Title", "Trying to repair", "正在尝试修复");
            RegisterString("decodeType23Title", "Repairing and Analyzing Log File", "正在修复并分析日志");
            RegisterString("decodeType24Title", "Interpreting Meta Driver", "正在解译元驱动");
            RegisterString("decodeType25Title", "Repairing and Interpreting Meta Driver", "正在修复并解译元驱动");
            //RegisterString("消耗复活币原地重组", "Use the resurrection coin to reassemble.", "消耗复活币并原地重组。");
            RegisterString("不朽之守护启动", " † Aegis of the Immortal † ", " † 不朽之守护 † ");
            RegisterString("不朽之守护就绪", "Aegis of the immortal is ready.", "不朽之守护已就绪。");
            RegisterString("消耗复活币描述", " Use 1 resurrection coin without spending any meta data.", "这会消耗复活币，但不需要消耗任何元数据。");
            RegisterString("使用复活币重新部署描述", "Use resurrection coin and redeploy Icarus to the initial planet?", "确认消耗复活币将伊卡洛斯重新部署至初始行星的降落点？");
            RegisterString("使用复活币立刻复活描述", "Use resurrection coin to reassemble Icarus?", "确认消耗复活币将伊卡洛斯立刻原地重组？");
            RegisterString("下次重新部署消耗不会增加", "Next redeployment cost won't increase.", "下次重新部署的消耗不会增加");
            RegisterString("下次立刻复活消耗不会增加", "Next reassembling cost won't increase.", "下次原地重组的消耗不会增加");
            RegisterString("使用元数据或复活币", "Use Metadata or Resurrect Coin", "使用 元数据 或 复活币");

            RegisterString("剩余技能点", "<color=#d2853d>-  {0} ap not used, click to allocate ( L )</color>", "<color=#d2853d>-  {0} 个授权点未使用，点击以进行分配 ( L )</color>");
            RegisterString("剩余技能点待确认", "<color=#d2853d>-  Ap allocation has not been confirmed ( L )</color>", "<color=#d2853d>-  有授权点分配尚未确认 ( L )</color>");
            RegisterString("技能点", "Authorization Point", "授权点");
            RegisterString("已分配技能点", "Allocated", "已分配");
            RegisterString("技能点描述", "The Authorization point is a reward from the COSMO Technology Ethics Committee for your contribution to maintaining the safety of the sector, and you can use the authorization point to exchange any approved <color=#c2853d>Cosmic mensural modification technology</color> from the COSMO Technology Ethics Committee, which will be immediately broadcast to your sector.\n\nYou are permitted to provide a reasonable number of universe matrices to the COSMO Technology Ethics Committee to <color=#c2853d>reset</color> the allocation of all the athorization points, but you are not allowed to do any reverse engineering of the modification technology. The COSMO Technology Ethics Committee has the right to withdraw all authorization points or activated modifications when detecting your violation.", "授权点是COSMO技术伦理委员会对你维护星区安全功绩的奖励，你可以使用授权点从COSMO技术伦理委员会处换取任何受认可的<color=#c2853d>宇宙定律修正技术</color>，这些修正将被立即广播至此星区。\n\n你被允许向COSMO技术伦理委员会提供合理数量的宇宙矩阵以<color=#c2853d>重置</color>对授权点的分配，但你被禁止对定律修正技术进行任何的逆向工程，COSMO技术伦理委员会有权在检测到你的违规行为后收回全部的授权点或已启用的修正。");
            RegisterString("技能点标题", "Authorization Point Allocation", "授权点分配");
            RegisterString("按下Shift分配10点说明", "Hold Shift to allocate 10 points at once\nHold Ctrl+Shift to allocate all available points", "按住Shift以一次性分配10点\n按住Ctrl+Shift以分配全部可用授权点");
            RegisterString("确认分配", "Confirm", "确认");
            RegisterString("撤销分配", "Withdraw", "全部撤销");
            RegisterString("技能点窗口取消", "Cancel", "取消");
            RegisterString("全部重置", "Reset All", "全部重置");
            RegisterString("重置技能点确认标题", "Reset All", "全部重置");
            RegisterString("重置技能点确认警告", "Confirm to use {0} universe matrix to reset all ap alloction? This will return all allocated authorization points.", "是否消耗{0}个宇宙矩阵以重置所有的授权点分配？这将返还全部已分配的授权点。");
            RegisterString("分配技能点确认标题", "Confirm Allocation", "确认分配");
            RegisterString("分配技能点确认警告", "Confirm and apply all the ap allocation?", "确认并应用所有授权点分配？");
            //RegisterString("aaa", "aaa", "aaa");
            RegisterString("skillL0", "Walk Speed", "步行速度");
            RegisterString("skillL1", "Replicator Speed", "手动合成速度");
            RegisterString("skillL2", "Construction Drone Count", "建设无人机数量");
            RegisterString("skillL3", "Construction Drone Flight Speed", "建设无人机速度");
            RegisterString("skillL4", "Ore Consumption", "采矿消耗");
            RegisterString("skillL5", "Mining Speed", "采矿速度");
            RegisterString("skillL6", "Proliferator Extra Product Effect", "增产剂增产效果");
            RegisterString("skillL7", "Proliferator Speedup Effect", "增产剂加速效果");
            RegisterString("skillL8", "Proliferator Energy Punishment", "增产剂能量惩罚");
            RegisterString("skillL9", "GM-α Algorithm Weight", "GM-α算法权重");
            RegisterString("skillL10", "Z7-η Algorithm Weight", "Z7-η算法权重");
            RegisterString("skillR0", "Global Damage", "全局伤害");
            RegisterString("skillR1", "Critical Chance", "暴击几率");
            RegisterString("skillR2", "Armor penetration", "护甲穿透");
            RegisterString("skillR3", "Icarus Shield Avoid", "伊卡洛斯护盾规避");
            RegisterString("skillR4", "Icarus Shield Energy Efficiency", "伊卡洛斯护盾能量效率");
            RegisterString("skillR5", "Planet Shield Energy Efficiency", "行星护盾能量效率");
            RegisterString("skillR6", "Void Damage", "虚空伤害");
            RegisterString("skillR7", "Droplet Energy Punishment", "水滴超远距耗能惩罚");
            RegisterString("skillR8", "Energy Fluctuation Camouflage", "能量波动屏蔽");
            RegisterString("skillL4Desc", "The mining consumption reduction effect display here already includes the effect provided by the merit rank. This effect will then multiplies with the effect provided by research.", "此处采矿消耗降低的显示数值已包含功勋阶级提供的消耗降低效果。此效果还将与科技提供的采矿消耗降低效果以乘法叠加。");
            RegisterString("skillL6Desc", "This buff will be directly increased to the percentage value of Proliferator Mk.III extra product effect. Mk.I and Mk.II only enjoys a partial buff.", "此效果的数值直接增加至Mk.III型增产剂增产效果的百分比数值上，更低阶增产剂只享受部分加成。");
            RegisterString("skillL7Desc", "This buff will be directly increased to the percentage value of Proliferator Mk.III speedup effect. Mk.I and Mk.II only enjoys a partial buff.", "此效果的数值直接增加至Mk.III型增产剂加速效果的百分比数值上，更低阶增产剂只享受部分加成。");
            RegisterString("skillL8Desc", "This buff will be directly added to the percentage value of Proliferator Mk.III energy consumption effect. Mk.I and Mk.II only enjoys a partial buff.", "此效果的数值直接增加至Mk.III型增产剂能量消耗惩罚的百分比数值上，更低阶增产剂只享受部分加成。");
            RegisterString("skillL9Desc", "The COSMO Technology Ethics Committee has expressed doubts about our request for this alternative algorithmic technology, as it hardly makes any visible improvement to the computing power of research facilities. But what they did not know was that using the GM-α algorithm to decode the meta driver could significantly increase the probability of the occurrence of the Mensural decoding track.", "COSMO技术伦理委员会对我们要求这种替代型算法技术的行为表示疑惑，因为它几乎不能对科研设施算力有任何可见的提升。但是他们不知道的是，使用GM-α算法解码元驱动可以显著提升定律级解码轨的出现概率。");
            RegisterString("skillL10Desc", "The COSMO Technology Ethics Committee has expressed doubts about our request for this alternative algorithmic technology, as it hardly makes any visible improvement to the computing power of research facilities. But what they did not know was that using the Z7-η algorithm to decode the meta driver could significantly increase the probability of the occurrence of the Mensural decoding track.", "COSMO技术伦理委员会对我们要求这种替代型算法技术的行为表示疑惑，因为它几乎不能对科研设施算力有任何可见的提升。但是他们不知道的是，使用Z7-η算法解码元驱动可以显著提升框架级解码轨的出现概率。");
            RegisterString("skillR0Desc", "The global damage bonus is multiplied with other damage bonuses. Takes effect on all allied attack.", "全局伤害加成将与其他的伤害加成以乘法叠加。对所有友方攻击生效。");
            RegisterString("skillR1Desc", "Allied attacks has a critical hit chance, dealing 100% additional damage.", "友方攻击有概率暴击，造成100%的额外伤害。");
            RegisterString("skillR2Desc", "Allied attacks can ignore a fixed amount of armor, and the value that penetrates beyond the target's armor can also increase damage.", "你的伤害可以无视固定数值的护甲，护甲穿透超出目标护甲的部分也可以继续增加伤害。");
            RegisterString("skillR3Desc", "Icarus' Energy shield will have a chance to avoid the damage.", "伊卡洛斯的能量盾将有概率直接规避一次伤害。");
            RegisterString("skillR4Desc", "Increasing the energy efficiency will increase the amount of damage the shield can take per unit of energy. But this will not be considered as damage reduction.", "提升护盾能量效率将提高每单位能量的护盾可以承受的伤害总量。但这不会被视为伤害减免。");
            RegisterString("skillR5Desc", "Increasing the energy efficiency will increase the amount of damage the shield can take per unit of energy. But this will not be considered as damage reduction.", "提升护盾的能量效率将提高每单位能量的行星护盾可以承受的伤害总量。但这不会被视为伤害减免。");
            RegisterString("skillR6Desc", "Gain a bonus when dealing void damage.", "友方造成虚空类型的伤害时，该伤害获得加成。");
            RegisterString("skillR7Desc", "Droplets have a 10x energy consumption penalty when manually summoned to attack units beyond 1AU. This reduces that penalty.\nDoes not affect the energy consumption of attacking within 1AU.", "水滴在手动召唤以攻击1AU以外的单位时，将有10倍的能量消耗惩罚，此项可以将该惩罚降低。\n不影响1AU以内的耗能。");
            RegisterString("skillR8Desc", "If the sum of the power grids' energy generation on all planets in a star system is lower than the energy fluctuation camouflage value, that star system will not be assimilated or invaded by the void. \nNote: If the star system has surviving dark fog hives, the normal dark fog hive may still launch an attack.", "如果一个恒星系内所有行星上的电网实际功率总和低于能量波动屏蔽值，该星系将不会招致虚空同化及入侵。\n注意：如果该星系有存留的黑雾巢穴，普通黑雾巢穴仍然可以发起进攻。");
            //RegisterString("aaa", "aaa", "aaa");
            //RegisterString("aaa", "aaa", "aaa");

            RegisterString("星河卫士奖章", "Galaxy Guardian's Medal", "星河卫士勋章");
            RegisterString("星河卫士奖章描述", "This is a medal given to you by the COSMO Technology Ethics Committee for your contribution to the sector, with your name engraved on it. The COSMO Technology Ethics Committee claims that the medal is extremely expensive and rare, and suggests you not to expose it to carbon dioxide, oxygen and water at the same time.", "这是COSMO技术伦理委员会为奖励你对星区做出的贡献而为你颁发的勋章，上面刻有你的名字。COSMO技术伦理委员会声称该奖章造价昂贵、极其稀有，并建议你<color=#c2853d>不要使其同时接触到二氧化碳、氧气和水</color>。");
            RegisterString("提供算力名称", "Incorporated into the COSMO architecture", "并入COSMO架构");
            RegisterString("提供算力描述", "Provide area computing power to the COSMO Technology Ethics Committee by Incorporate all research facilities into the COSMO architecture, which will continuously earn you merit points. In addition, this protocol requires universe matrices to ensure that each section can efficiently handle the hash collision problem in the COSMO main-net.", "通过将所有科研设施并入COSMO架构来为COSMO技术伦理委员会提供星区算力，<color=#c2853d>这会为你持续提供功勋点数</color>。此外，这还需要消耗一定的宇宙矩阵来保证每个星区能高效处理主网内的散列碰撞问题。");
            RegisterString("提供算力结论", "This protocol allows unlimited progress and is automatically added to the end of the queue", "此项科技允许无限进行，已自动添加到队列末尾");
            RegisterString("版本更迭补偿", "Version change compensation", "版本更迭补偿");
            RegisterString("遗物4-5补偿说明", "Since the effect of the meta driver [Aftershock Echo] has been changed, you received a compensation from the void: you can immediately interpret a new meta driver, and a free resurrection coin. ", "由于[余震回响]元驱动的效果被更改，你收到了一个来自虚空的补偿：可以立刻解译一个新的元驱动，并获得一个复活币。");

            // 统计界面翻译
            RegisterString("PF深空来敌", "TCFV", "深空来敌");
            RegisterString("PF入侵逻辑", "Invasion Logic", "入侵逻辑");
            RegisterString("PF元驱动", "Meta drivers", "元驱动");
            RegisterString("PF工厂重写", "Factory Rewrite", "工厂重写");
            RegisterString("PF伤害逻辑", "Damage Logic", "伤害逻辑");
            RegisterString("PF击杀逻辑", "Kill Logic", "击杀逻辑");
            RegisterString("PF弹道重写", "Ballistic Rewrite", "弹道重写");
            RegisterString("PF水滴", "Droplets", "水滴");
            RegisterString("PF事件链", "Event System", "事件链");
            RegisterString("PF深空绘制调用", "Draw Call", "绘制调用");

            // 入侵逻辑

            RegisterString("虚空入侵扩张中", "Expanding {0:D2}:{1:D2}", "扩张中 {0:D2}:{1:D2}");
            RegisterString("虚空入侵集结中", "Assimilating {0:D2}:{1:D2}", "同化中 {0:D2}:{1:D2}");
            RegisterString("虚空入侵进攻中", "{2} incoming", "{2}艘战舰袭来");
            RegisterString("虚空入侵进攻中计时", "Invasion {0:D2}:{1:D2}", "正在入侵 {0:D2}:{1:D2}");
            RegisterString("虚空入侵数量", "Est. {0} Vessels", "预估数量 {0}");
            RegisterString("削弱入侵标题", "Weaken the Invasion", "削弱入侵");
            RegisterString("削弱入侵内容", "When the dark fog is in the process of assimilation, the continuous projection of void energy makes it almost impossible for <color=#ffa800dd>conventional weapons</color> to damage the dark fog units. Only by using the <color=#ffa800dd>star cannon</color> can we ignore the protection of void energy, and damage the hive even during the assimilation process, thereby <color=#ffa800dd>weakening</color> the upcoming <color=#ffa800dd>invasion intensity</color>. \nOnce assimilation process is complete, the void will redirect its energy towards maintaining complete control over the dark fog, which also significantly reduces the dark fog's protection from the void. This means that only after they initiate the invasion can they be damaged by conventional weapon. Nevertheless, as the attack becomes stronger, the void's damage reduction protection can be retained up to <color=#ffa800dd>75%</color> (in the late stages of the game).", "当黑雾正处在被同化的进程中时，持续的虚空能量投射使得<color=#ffa800dd>常规武器</color>几乎无法伤害到这些黑雾单位。只有动用<color=#ffa800dd>恒星炮</color>才可以无视虚空能量的庇护并在同化进程中对巢穴造成伤害，以此<color=#ffa800dd>削弱</color>即将到来的<color=#ffa800dd>入侵强度</color>。\n一旦同化完成，虚空会将能量转而用于维持对黑雾的完全控制，这会使得黑雾失去一部分来自虚空的保护，这意味着其只有在发起入侵时才可以被常规武器伤害。尽管如此，随着进攻越来越强大，来自虚空的保护最多可以被保留<color=#ffa800dd>75%</color>伤害减免效果（在游戏的终末期）。");


            RegisterString("开启虚空入侵", "Enable Void Invasion", "开启虚空入侵");
            RegisterString("已开启虚空入侵", "Void Invasion Enabled", "虚空入侵已启用");
            RegisterString("非黑雾模式", "Dark Fog Not Available", "黑雾势力未启用");
            RegisterString("虚空入侵", "Void Invasion", "虚空入侵");
            RegisterString("虚空入侵提示", "Enabling Void Invasion will result in periodic and increasingly powerful dark fog invasions, and resisting invasion will earn you additional rewards. The intensity upper limit of the void invasion in late game will be affected by the dark fog difficulty, and that intensity upper limit will reach the maximum when the dark fog difficulty is larger than 4.0.\n\nWarning: Once you enabled the void invasion, you cannot disable it.", "启用虚空入侵会导致星区周期性地受到越来越强大的黑雾进攻，抵抗进攻也将获得额外奖励。游戏后期虚空入侵的强度的上限会受黑雾难度系数影响，在黑雾系数达到4.0以上后，后期入侵的强度将达到最大。\n\n警告：一旦开启虚空入侵，此选项无法被关闭。");
            RegisterString("虚空入侵版本更新提示", "They Come From Void has updated. There is a new mode available now. Enabling Void Invasion will result in periodic and increasingly powerful dark fog invasions, and resisting invasion will earn you additional rewards.\nNote: The intensity upper limit of the void invasion in late game will be affected by the dark fog difficulty. Once you enabled the void invasion, you cannot disable it.\nWill you enable it now?\n\n( You can also enable it later in Esc Menu. )", "深空来敌已更新虚空入侵模式，启用虚空入侵会导致星区周期性地受到越来越强大的黑雾进攻，抵抗进攻也将获得额外奖励。\n注意：游戏后期虚空入侵的强度的上限会受黑雾难度系数影响，一旦开启虚空入侵，此选项无法被关闭。\n是否立即启用？\n\n（你也可以稍后在Esc菜单中启用虚空入侵。）");

            RegisterString("侦测到虚空入侵提示", "Void rift detected. Void is attempting to assimilate dark fog to \ninvade {0}.\n", "探测到虚空裂隙，虚空正在尝试同化黑雾以\n入侵{0}。\n");
            RegisterString("虚空入侵额外特性提示", "Overload of void energy projection detected, which will give assimilated dark fog <color=#ffa800dd>additional properties</color>. They will:", "侦测到过载的虚空能量投射使得被同化的黑雾获得了<color=#ffa800dd>额外特性</color>，它们将：");
            RegisterString("虚空入侵结束", "Invasion Ended", "入侵结束");
            RegisterString("虚空入侵结束提示", "The Void is no longer able to maintain the connection, and all dark fog units have broken away from assimilation.\n{0}/{1} vessels has been destroyed.\nThe COSMO Technology Ethics Committee has awarded you {2} authorization points for your contribution to the peace maintenance of the sector.", "虚空已无法继续维持连接，所有黑雾单位已脱离了同化。\n{0}/{1}艘敌舰已被消灭。\nCOSMO技术伦理委员为奖励你对维护星区和平和贡献，向你发放了{2}授权点。");
            RegisterString("虚空入侵结束提示元驱动解译", "Since the strong void energy projection reveals the expansion point of some cosmological axioms, the next time you interpret an meta driver, there will be a greater probability of discovering Axiomatic decoding tracks.", "由于强大的虚空能量投射揭示了部分宇宙公理的扩展点位，下次解译元驱动时，将有更大概率发现公理级解码轨。");
            RegisterString("虚空入侵结束提示元驱动发现", "A complete meta driver signal was detected at the void connection break. It has been added to the meta-driver interpretation event chain.", "在虚空连接的断裂出侦测到完整的元驱动信号，已添加到元驱动解译事件链。");

            RegisterString("额外特性描述0", "Gain {0}% damage reduction", "获得{0}%伤害减免");
            RegisterString("额外特性描述1", "Have {0}% probability of fully dodging an attack", "有{0}%的概率完全闪避一次伤害");
            RegisterString("额外特性描述2", "Gain {0} extra armor", "获得{0}额外护甲");
            RegisterString("额外特性描述3", "Deal +{0}% extra damage to planetary shields", "对行星护盾造成+{0}%额外伤害");
            RegisterString("额外特性描述4", "When killed by a droplet, there is a {0}% probability of destroying that droplet", "被水滴击杀时，有{0}%概率摧毁该水滴");
            RegisterString("额外特性描述5", "Procvide no merit points when killed", "在死亡时不提供功勋点数");
            RegisterString("额外特性描述6", "Reduces {0}% kinetic weapon damage in the whole sector", "削弱全星区的{0}%动能武器伤害");
            RegisterString("额外特性描述7", "Reduces {0}% energy weapon damage in the whole sector", "削弱全星区的{0}%能量武器伤害");
            RegisterString("额外特性描述8", "Reduces {0}% blast weapon damage in the whole sector", "削弱全星区的{0}%爆破武器伤害");
            RegisterString("额外特性描述9", "Reduces {0}% magnetic weapon damage in the whole sector", "削弱全星区的{0}%电磁武器伤害");
            RegisterString("额外特性描述10", "Incapacitated Icarus' space fleet (except for droplets)", "使伊卡洛斯的太空舰队无法作战（水滴除外）");
            RegisterString("额外特性描述11", "Deal extra damage equal to {0}% of your maximum damage type bonus", "造成额外伤害，等同于你最高伤害类型加成的{0}%");
            RegisterString("额外特性描述12", "Gain {0}/s extra health regeneration", "获得{0}/s额外生命回复");
            RegisterString("额外特性描述13", "Gain +{0}% sailing speed", "获得+{0}%航行速度加成");
            RegisterString("额外特性描述14", "", "");
        }



        public static void AddTutorialProtos()
        {
            TutorialProto tp2 = LDB.tutorial.Select(1).Copy();
            tp2.Name = "功勋阶级".Translate();
            tp2.name = "功勋阶级".Translate();
            tp2.Video = "";
            tp2.PreText = "深空来敌介绍2前字";
            tp2.PostText = "";
            tp2.ID = 42;
            LDBTool.PreAddProto(tp2);

            TutorialProto tp5 = LDB.tutorial.Select(1).Copy();
            tp5.Name = "深空来敌介绍5标题".Translate();
            tp5.name = "深空来敌介绍5标题".Translate();
            tp5.Video = "";
            tp5.PreText = "深空来敌介绍5前字";
            tp5.PostText = "";
            tp5.ID = 43;
            LDBTool.PreAddProto(tp5);

            TutorialProto tp6 = LDB.tutorial.Select(1).Copy();
            tp6.Name = "深空来敌介绍6标题".Translate();
            tp6.name = "深空来敌介绍6标题".Translate();
            tp6.Video = "";
            tp6.PreText = "";
            tp6.PostText = "";
            tp6.ID = 44;
            LDBTool.PreAddProto(tp6);
        }

        public static void RewriteTutorialProtosWhenLoad()
        {
            TutorialProto relicTutorial = LDB.tutorial.Select(43);
            TutorialProto relicCursedTutorial = LDB.tutorial.Select(44);
            if (relicTutorial != null)
            {
                try
                {
                    string text = "";
                    string text2 = ""; //这里必须分成两部分要不然英文部分过长，超过65000报错，无法显示
                    for (int type = 0; type < 2; type = (type + 1) % 5)
                    {
                        text += "<size=16>" + ($"圣物稀有度{type}").Translate() + "</size>\n";
                        for (int num = 0; num < Relic.relicNumByType[type]; num++)
                        {
                            text += $"遗物名称带颜色{type}-{num}".Translate().Split('[')[0].Trim() + "</color>\n";
                            text += $"遗物描述{type}-{num}".Translate();
                            if ($"relicTipText{type}-{num}".Translate() != $"relicTipText{type}-{num}")
                            {
                                text += "\n" + $"relicTipText{type}-{num}".Translate();
                            }
                            text += "\n\n";
                        }
                        text += "\n";
                    }
                    relicTutorial.PreText = text;
                    for (int type = 2; type < 4; type++)
                    {
                        text2 += "<size=16>" + ($"圣物稀有度{type}").Translate() + "</size>\n";
                        for (int num = 0; num < Relic.relicNumByType[type]; num++)
                        {
                            text2 += $"遗物名称带颜色{type}-{num}".Translate().Split('[')[0].Trim() + "</color>\n";
                            text2 += $"遗物描述{type}-{num}".Translate();
                            if ($"relicTipText{type}-{num}".Translate() != $"relicTipText{type}-{num}")
                            {
                                text2 += "\n" + $"relicTipText{type}-{num}".Translate();
                            }
                            text2 += "\n\n";
                        }
                        text2 += "\n";
                    }
                    relicTutorial.PostText = text2;

                    string text3 = "";
                    //text3 += "<size=16>" + ("圣物稀有度4").Translate() + "</size>\n";
                    text3 += "诅咒描述独立".Translate() + "\n\n";
                    for (int num = 0; num < Relic.relicNumByType[4]; num++)
                    {
                        text3 += $"遗物名称带颜色{4}-{num}".Translate().Split('[')[0].Trim() + "</color>\n";
                        text3 += $"遗物描述{4}-{num}".Translate();
                        if ($"relicTipText{4}-{num}".Translate() != $"relicTipText{4}-{num}")
                        {
                            text3 += "\n" + $"relicTipText{4}-{num}".Translate();
                        }
                        text3 += "\n\n";
                    }
                    text3 += "\n";
                    relicCursedTutorial.PreText = text3;
                }
                catch (Exception)
                { }
            }
        }


        public static void EditProtossWhenLoad()
        {
            ItemProto dropletItem = LDB.items.Select(9511);
            if (dropletItem == null)
                return;
            dropletItem.DescFields = new int[] { 81, 82, 80, 59, 11, 1 };
            //dropletItem.AmmoType = EAmmoType.Bullet;
            dropletItem.prefabDesc = new PrefabDesc();
            ref var desc = ref dropletItem.prefabDesc;
            //desc.isCraftUnit = true;
            desc.craftUnitMaxMovementSpeed = 30000;
            desc.workEnergyPerTick = 500000;

            StorageComponent.itemIsFighter[9511] = true;
        }

        // 在import末尾调用
        public static void UnlockTutorials(int i = -1)
        {
            if (i == -1)
            {
                for (int id = 42; id <= 44; id++)
                {
                    GameMain.history.UnlockTutorial(id);
                }
            }
            else
                GameMain.history.UnlockTutorial(i);
        }

        //public static void PostDataAction()
        //{
        //    ItemProto.InitFluids();
        //    ItemProto.InitItemIds();
        //    ItemProto.InitFuelNeeds();
        //    ItemProto.InitItemIndices();
        //    ItemProto.InitMechaMaterials();

        //    foreach (var proto in LDB.items.dataArray)
        //    {
        //        if (proto.ID > 8000) // Keep compability with stack mod 
        //        {
        //            StorageComponent.itemIsFuel[proto.ID] = proto.HeatValue > 0L;
        //            StorageComponent.itemStackCount[proto.ID] = proto.StackSize;
        //        }
        //    }

        //    LDB.items.Select(9500).Description = LDB.items.Select(9500).description = "多功能集成组件描述gm2".Translate();

        //    LDB.models.OnAfterDeserialize();
        //    LDB.models.Select(311).prefabDesc.modelIndex = 311;
        //    LDB.items.Select(8011).ModelIndex = 311;
        //    LDB.items.Select(8011).prefabDesc = LDB.models.Select(311).prefabDesc;

        //    LDB.models.Select(312).prefabDesc.modelIndex = 312;
        //    LDB.items.Select(8012).ModelIndex = 312;
        //    LDB.items.Select(8012).prefabDesc = LDB.models.Select(312).prefabDesc;

        //    LDB.models.Select(314).prefabDesc.modelIndex = 314;
        //    LDB.items.Select(8014).ModelIndex = 314;
        //    LDB.items.Select(8014).prefabDesc = LDB.models.Select(314).prefabDesc;

        //    LDB.models.Select(313).prefabDesc.modelIndex = 313;
        //    LDB.items.Select(8013).ModelIndex = 313;
        //    LDB.items.Select(8013).prefabDesc = LDB.models.Select(313).prefabDesc;

        //    LDB.models.Select(315).prefabDesc.modelIndex = 315;
        //    LDB.items.Select(8030).ModelIndex = 315;
        //    LDB.items.Select(8030).prefabDesc = LDB.models.Select(315).prefabDesc;

        //    LDB.models.Select(316).prefabDesc.modelIndex = 316;
        //    LDB.items.Select(8031).ModelIndex = 316;
        //    LDB.items.Select(8031).prefabDesc = LDB.models.Select(316).prefabDesc;

        //    //LDB.models.Select(317).prefabDesc.modelIndex = 317;
        //    //LDB.items.Select(8036).ModelIndex = 317;
        //    //LDB.items.Select(8036).prefabDesc = LDB.models.Select(317).prefabDesc;

        //    // LDB.items.Select(2206).prefabDesc.ener
        //    LDB.items.Select(2206).prefabDesc.inputEnergyPerTick = 150000;
        //    LDB.items.Select(2206).prefabDesc.outputEnergyPerTick = 150000;
        //    LDB.items.Select(2206).prefabDesc.maxAcuEnergy = 540000000;
        //    LDB.items.Select(2207).prefabDesc.inputEnergyPerTick = 150000;
        //    LDB.items.Select(2207).prefabDesc.outputEnergyPerTick = 150000;
        //    LDB.items.Select(2207).prefabDesc.maxAcuEnergy = 540000000;

        //    if (Configs.enableProliferator4)
        //    {
        //        LDB.models.Select(120).prefabDesc.incItemId = new int[] { 1141, 1142, 1143, 8034 };
        //        LDB.techs.Select(1132).Position = new Vector2(29, -19);
        //        LDB.techs.Select(1416).Position = new Vector2(29, -15);
        //        LDB.techs.Select(1153).Position = new Vector2(33, -11);
        //    }

        //    //以下为星球发动机模型修改
        //    if (Configs.developerMode)
        //    {
        //        var prefab = LDB.items.Select(8031).prefabDesc;
        //        var oriPrefab = LDB.models.Select(68).prefabDesc;
        //        var originalMeshVertices = new Vector3[prefab.lodCount][];
        //        for (int i = 0; i < prefab.lodCount; i++)
        //        {
        //            var vertices = prefab.lodMeshes[i].vertices;
        //            originalMeshVertices[i] = new Vector3[vertices.Length];
        //            for (int j = 0; j < vertices.Length; j++)
        //            {
        //                originalMeshVertices[i][j] = vertices[j];
        //            }
        //        }
        //        //碰撞
        //        prefab.colliders = new ColliderData[oriPrefab.colliders.Length];
        //        for (int i = 0; i < prefab.colliders.Length; i++)
        //        {
        //            prefab.colliders[i] = oriPrefab.colliders[i];
        //            prefab.colliders[i].ext.x *= 2f;
        //            prefab.colliders[i].ext.y *= 3f;
        //            prefab.colliders[i].ext.z *= 2f;
        //        }
        //        prefab.buildColliders = new ColliderData[oriPrefab.buildColliders.Length];
        //        for (int i = 0; i < prefab.buildColliders.Length; i++)
        //        {
        //            prefab.buildColliders[i] = oriPrefab.buildColliders[i];
        //            prefab.buildColliders[i].ext.x *= 2f;
        //            prefab.buildColliders[i].ext.y *= 3f;
        //            prefab.buildColliders[i].ext.z *= 2f;
        //        }
        //        prefab.buildCollider.ext.x *= 1f;
        //        prefab.buildCollider.ext.z *= 1f;
        //        //静态顶点（prebuild）
        //        for (int i = 0; i < prefab.lodCount; i++)
        //        {
        //            var mesh = prefab.lodMeshes[i];
        //            var oriVerts = originalMeshVertices[i];
        //            var vertices = mesh.vertices;
        //            for (int j = 0; j < oriVerts.Length; j++)
        //            {
        //                Vector3 vert = oriVerts[j];
        //                vert.x *= 4;
        //                vert.y *= 2f;
        //                vert.z *= 4;
        //                if(vert.y <=3)
        //                {
        //                    vert.x *= 1.7f;
        //                    vert.z *= 1.7f;
        //                }
        //                else if(vert.y >= 4 && vert.y <= 6)
        //                {
        //                    vert.x *= 2.2f;
        //                    vert.z *= 2.2f;
        //                }
        //                else if (vert.y >= 7)
        //                {
        //                    vert.x *= 0.5f;
        //                    vert.z *= 0.5f;
        //                }
        //                vertices[j] = vert;
        //            }
        //            mesh.vertices = vertices;
        //        }
        //        //动画顶点
        //        for (int i = 0; i < 1; i++)
        //        {
        //            List<int> centerHighP = new List<int>();
        //            List<int> highP2 = new List<int>();
        //            int loop = (int)prefab.lodVertas[i].vertexType;
        //            if (loop == 0) loop = 12;
        //            //Utils.Log($"lodV index {i} have dataLength{prefab.lodVertas[i].dataLength} whose type is {prefab.lodVertas[i].vertexType}\n frame count is {prefab.lodVertas[i].frameCount} and frameStride is {prefab.lodVertas[i].frameStride} vertex count is {prefab.lodVertas[i].vertexCount} and vertex size is {prefab.lodVertas[i].vertexSize}");
        //            for (int j = 0; j < prefab.lodVertas[i].dataLength; j++)
        //            {
        //                if (j % loop == 2) //整体放大
        //                {
        //                    prefab.lodVertas[i].data[j - 2] *= 4;
        //                    prefab.lodVertas[i].data[j - 1] *= 3;
        //                    prefab.lodVertas[i].data[j] *= 4;
        //                }
        //                if(j%loop == 2 && prefab.lodVertas[i].data[j - 1] <=3) //底座放大
        //                {
        //                    prefab.lodVertas[i].data[j - 2] *= 1.75f;
        //                    prefab.lodVertas[i].data[j] *= 1.75f;
        //                }
        //                if (j % loop == 2 && prefab.lodVertas[i].data[j - 1] >= 6 && prefab.lodVertas[i].data[j - 1] <= 8) //中间不再收紧
        //                {
        //                    prefab.lodVertas[i].data[j - 2] *= 2.2f;
        //                    prefab.lodVertas[i].data[j] *= 2.2f;
        //                }
        //                if (j % loop == 2 && j > 15318 * 60)//得到结果是j%15318>=4772的
        //                {
        //                    if (prefab.lodVertas[i].data[j - 1] > 155 && !highP2.Contains(j % 15318))
        //                        highP2.Add(j % 15318);
        //                }
        //            }
        //            for (int j = 0; j < prefab.lodVertas[i].dataLength; j++)
        //            {
        //                if (j % loop == 2 && highP2.Contains(j%15318))
        //                {
        //                    prefab.lodVertas[i].data[j - 2] = 0f;
        //                    prefab.lodVertas[i].data[j - 1] =400;
        //                    prefab.lodVertas[i].data[j] = 0f;
        //                }
        //            }
        //        }
        //    }


        //    GameMain.gpuiManager.Init();
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(GameHistoryData), "UnlockTechFunction")]
        //public static void UnlockBattleTechFunc(int func, double value, int level)
        //{
        //    switch (func)
        //    {
        //        case 50:
        //            Configs.bulletAtkScale += value;
        //            break;
        //        case 51:
        //            Configs.bulletSpeedScale += value;
        //            break;
        //        case 52:
        //            Configs.wormholeRangeAdded += (int)value;
        //            break;
        //        case 53:
        //            Droplets.maxDroplet += 0; //(int)value; // 改成了每帧根据科技等级刷新max数。就不存档了。
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(TechProto), "UnlockFunctionText")]
        //public static void UnlockFunctionTextPatch(ref TechProto __instance, ref string __result, StringBuilder sb)
        //{
        //    if (__instance.ID >= 4901 && __instance.ID <= 4906)
        //        __result = "子弹伤害和导弹伤害+15%".Translate() + "\n" + "相位裂解光束伤害+30%".Translate();
        //    else if (__instance.ID >= 4911 && __instance.ID <= 4916)
        //        __result = "子弹飞行速度+10%".Translate() + "\n" + "导弹飞行速度+5%".Translate();
        //    //else if (__instance.ID == 4921)
        //    //    __result = "虫洞生成最近范围向10AU推进3%".Translate();
        //    //else if (__instance.ID == 4922)
        //    //    __result = "虫洞生成最近范围向10AU推进5%".Translate();
        //    //else if (__instance.ID == 4923)
        //    //    __result = "虫洞生成最近范围向10AU推进8%".Translate();
        //    else if (__instance.ID >= 4921 && __instance.ID <= 4926)
        //        __result = "虫洞生成最近范围扩大0.25AU".Translate();
        //    else if(__instance.ID >= 4927 && __instance.ID <= 4929)
        //        __result = "水滴控制上限".Translate() + "+1";
        //}

        //public static Text infoLabel = null;
        //public static Text infoValue = null;

        //public static void InitTechInfoUIs()
        //{
        //    GameObject infoLabelObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Fullscreen UIs/Tech Tree/info-panel/label");
        //    infoLabel = infoLabelObj.GetComponent<Text>();

        //    GameObject infoValueObj = GameObject.Find("UI Root/Overlay Canvas/In Game/Fullscreen UIs/Tech Tree/info-panel/value");
        //    infoValue = infoValueObj.GetComponent<Text>();

        //}


        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(UITechTree), "OnPageChanged")]
        //public static void OnPageChangedPatch(ref UITechTree __instance)
        //{
        //    if (infoLabel == null)
        //        InitTechInfoUIs();

        //    if (infoLabel.text.Split('\n').Length < 38)
        //    {
        //        //infoLabel.text = infoLabel.text + "\r\n\r\n" + "子弹伤害".Translate() + "\r\n" + "相位裂解光束伤害".Translate() + "\r\n"
        //        //    + "导弹伤害".Translate() + "\r\n" + "子弹飞行速度".Translate() + "\r\n" + "导弹飞行速度".Translate() + "\r\n" + "虫洞干扰半径".Translate() + "\r\n" + "水滴控制上限".Translate();
        //        infoLabel.text = infoLabel.text + "\r\n\r\n" + "子弹相位伤害".Translate() + "\r\n" +
        //            "子弹导弹速度".Translate() + "\r\n" + "虫洞干扰半径".Translate() + "\r\n" + "水滴控制上限".Translate();
        //    }
        //}

        public static void InitEventProtos()
        {
            EventSystem.protos = new Dictionary<int, EventProto>();
            EventSystem.alterProtos = new Dictionary<int, List<int>>();
            // 1 接入点
            {
                int id = 1001; // 没有任何圣物时，获得圣物必定为此事件
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { }, new int[] { });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { }, new int[] { 19997, 24 }, new int[] { 0, 600 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "What ...... is This?", "这是……什么？");
                RegisterString("epd" + id.ToString(), "The analysis module stumbled upon a meta-drive with a self-interpretive system from the wreckage of the dark fog units, which appears to contain a high-dimensional API that efficiently uses the underlying physical logic of the universe, but its specific function has not yet been confirmed. Fortunately, its self-interpreting system was preserved so well that it only needed to connect to Icarus' research hub to decode it directly.", "分析模块从黑雾基地的残骸中偶然发现了一个带有自解译系统的元驱动，这种驱动似乎载有高效利用宇宙底层物理逻辑的高维API，但目前还无法确定它的具体功能。幸运的是，它带有的自解译系统被保留得相当完好，只需要连接伊卡洛斯的研究中枢就可以直接对其进行解码。");
                RegisterString("epdt" + id.ToString() + "-0", "Don't connect", "不要连接");
                RegisterString("epdt" + id.ToString() + "-1", "Connect it!", "连接它！");
            }
            {
                int id = 1002; // 通用圣物发现的首个事件
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { }, new int[] { });
                ep.SetDecision(0, new int[] { }, new int[] { -1, 25201 }, new int[] { 0, 100 });
                ep.SetDecision(1, new int[] { }, new int[] { 12, 21 }, new int[] { 0, 1800 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Discover Traces of Potential Meta-Drive", "发现潜在的元驱动");
                RegisterString("epd" + id.ToString(), "Traces of high-dimensional API calls were found in the log files of the dark fog wreckage, which may indicate the existence of a cosmic meta-drive. But the entity that records the log file is extremely unstable, and analyzing the log file will destroy its physical structure and make it no longer usable. Or Icarus could choose to compile this log file with high-dimensional information directly into physical dark fog matrices in its storage.", "在黑雾残骸的日志文件中发现了高维API的调用痕迹，这可能预示着宇宙元驱动的存在。但记载日志文件的实体极其不稳定，分析日志文件将破坏其物理结构，使其不再可用。或者伊卡洛斯可以选择将这个载有高维信息的日志文件在其存储体中直接编译为实体黑雾矩阵。");
                RegisterString("epdt" + id.ToString() + "-0", "Compile it directly into the matrix", "将日志文件直接编译为矩阵");
                RegisterString("epdt" + id.ToString() + "-1", "Analyze log file", "分析日志文件");
            }
            // 2
            {
                int id = 2001;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 1 }, new int[] { 300 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 25 }, new int[] { 0, 3600 * 3 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completes the destructive analysis of the log and finds that the carrier itself is the meta-driven materialization matrix. Fortunately, with its remnant self-interpreting system, Icarus might be able to try to fix the meta-drive and make it interpretable, but this would need some necessary materials.", "分析模块完成了对日志的破坏性分析，发现其载体本身就是元驱动的实体化矩阵。幸运的是，借助其残存的自解译系统，伊卡洛斯或许可以尝试修复元驱动，使其变得可以被解译，但这将消耗一些必要的材料。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort repairing", "放弃修复");
                RegisterString("epdt" + id.ToString() + "-1", "Attempt to repair", "尝试修复");
            }
            {
                int id = 2002;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 400 }, new int[] { 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 18101 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed a destructive analysis of the log, which vaguely pointed to a planet, perhaps probing that planet could find traces of the meta-drive.", "分析模块完成了对日志的破坏性分析，日志模糊地指向了一个行星，或许探测该行星可以找到元驱动的痕迹。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort searching", "放弃搜寻");
                RegisterString("epdt" + id.ToString() + "-1", "Search on the planet", "在行星上搜寻"); // 需要到达：行星名称（点击标记）
            }
            {
                int id = 2003;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9999 }, new int[] { 100 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 24 }, new int[] { 0, 1800 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed a destructive analysis of the log, which, strangely enough, indicates that the meta-drive appeared to be encoded in disembodied form on energy fluctuations throughout the sector and is reacted to the disintegration of the dark fog units. Every time a dark fog unit is destroyed, it will trigger regular fluctuations encoded in the entire star region, and the analysis module may be able to find a way to interpret the drive by learning a large amount of regular fluctuation data. Once the sample size is large enough, Icarus can start the interpretation procedure.", "分析模块完成了对日志的破坏性分析，奇怪的是，这个驱动似乎是以无实体的形式被编码在整个星区的能量波动上的，并且对黑雾单位的解体过程有反应。每当有黑雾单位被摧毁，都会激发整个星区编码的规律性波动，分析模块或许可以通过学习大量的规律性波动数据来寻找解译驱动的方法。一旦样本量足够，伊卡洛斯就可以启动解译程序。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Run interpretation procedure", "执行解译程序");
            }
            {
                int id = 2004;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 11305, 30000 }, new int[] { 50, 1 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 24 }, new int[] { 0, 3600 * 8 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 19999, 24 }, new int[] { 0, 1200 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed the destructive analysis of the logs and easily located the meta-drive interpretation site, which Icarus needed to synchronize the compiled physical rules with the understandable code structure during the research. Alternatively, quantum chips can be used directly to assist interpretation, but this will take a longer time.", "分析模块完成了对日志的破坏性分析，并轻松定位了元驱动的解译位点，伊卡洛斯需要在研究过程中将编译在内的物理规则与可理解的代码结构进行同步，来完成解译过程。或者，也可以直接使用量子芯片辅助解译，但这将花费较长的时间。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Use quantum chip to support the interpretation", "使用量子芯片解译");
                RegisterString("epdt" + id.ToString() + "-2", "Sync physical rules", "同步物理规则");
            }
            {
                int id = 2101;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 1 }, new int[] { 300 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 25 }, new int[] { 0, 3600 * 3 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completes the destructive analysis of the log and finds that the carrier itself is the meta-driven materialization matrix. Fortunately, with its remnant self-interpreting system, Icarus might be able to try to fix the meta-drive and make it interpretable, but this would need some necessary materials.", "分析模块完成了对日志的破坏性分析，发现其载体本身就是元驱动的实体化矩阵。幸运的是，借助其残存的自解译系统，伊卡洛斯或许可以尝试修复元驱动，使其变得可以被解译，但这将消耗一些必要的材料。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort repairing", "放弃修复");
                RegisterString("epdt" + id.ToString() + "-1", "Attempt to repair", "尝试修复");
            }
            {
                int id = 2102;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 400 }, new int[] { 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 18101 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed a destructive analysis of the log, which vaguely pointed to a planet, perhaps probing that planet could find traces of the meta-drive.", "分析模块完成了对日志的破坏性分析，日志模糊地指向了一个行星，或许探测该行星可以找到元驱动的痕迹。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort searching", "放弃搜寻");
                RegisterString("epdt" + id.ToString() + "-1", "Search on the planet", "在行星上搜寻"); // 需要到达：行星名称（点击标记）
            }
            {
                int id = 2103;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9999 }, new int[] { 100 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 24 }, new int[] { 0, 1800 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed a destructive analysis of the log, which, strangely enough, indicates that the meta-drive appeared to be encoded in disembodied form on energy fluctuations throughout the sector and is reacted to the disintegration of the dark fog units. Every time a dark fog unit is destroyed, it will trigger regular fluctuations encoded in the entire star region, and the analysis module may be able to find a way to interpret the drive by learning a large amount of regular fluctuation data. Once the sample size is large enough, Icarus can start the interpretation procedure.", "分析模块完成了对日志的破坏性分析，奇怪的是，这个驱动似乎是以无实体的形式被编码在整个星区的能量波动上的，并且对黑雾单位的解体过程有反应。每当有黑雾单位被摧毁，都会激发整个星区编码的规律性波动，分析模块或许可以通过学习大量的规律性波动数据来寻找解译驱动的方法。一旦样本量足够，伊卡洛斯就可以启动解译程序。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Run interpretation procedure", "执行解译程序");
            }
            {
                int id = 2104;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 11305, 30000 }, new int[] { 50, 1 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 24 }, new int[] { 0, 3600 * 8 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 19999, 24 }, new int[] { 0, 1200 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed the destructive analysis of the logs and easily located the meta-drive interpretation site, which Icarus needed to synchronize the compiled physical rules with the understandable code structure during the research. Alternatively, quantum chips can be used directly to assist interpretation, but this will take a longer time.", "分析模块完成了对日志的破坏性分析，并轻松定位了元驱动的解译位点，伊卡洛斯需要在研究过程中将编译在内的物理规则与可理解的代码结构进行同步，来完成解译过程。或者，也可以直接使用量子芯片辅助解译，但这将花费较长的时间。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Use quantum chip to support the interpretation", "使用量子芯片解译");
                RegisterString("epdt" + id.ToString() + "-2", "Sync physical rules", "同步物理规则");
            }
            {
                int id = 2201;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 1 }, new int[] { 400 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 13, 23 }, new int[] { 0, 3600 * 3 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module found that the physical storage structure where the logs were recorded had been severely damaged, but the damage appeared to be reversible. Before performing the analysis, you first need to provide some necessary materials to repair the log.", "分析模块发现记载日志的物理存储结构已经受到了严重的损伤，不过该损伤似乎是可逆的。在执行分析之前，首先需要提供一些必要的材料来修复日志。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort repairing", "放弃修复");
                RegisterString("epdt" + id.ToString() + "-1", "Attempt to repair and analyze", "尝试修复并分析");
            }
            {
                int id = 2202;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 300 }, new int[] { 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 13, 21 }, new int[] { 0, 1800 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module found that the log itself was highly encrypted by rapidly changing the ciphertext in the time dimension. Obviously, maintaining the encryption required energy supply. Icarus easily pinpointed the energy as coming from a dark fog base on the ground on a planet. If all the dark fog bases on the planet are destroyed and all the core drillers are removed, the decryption and analysis of logs should be performed.", "分析模块发现了日志本身被高度加密了，加密的方式是通过在时间维度上快速改变密文实现的，显然，维持该加密需要能量的供给。伊卡洛斯轻易地定位到了这个能量来自于某个行星上的地面黑雾巢穴。如果摧毁并填埋该行星上的全部黑雾基地，应该就可以执行日志的解密与分析程序了。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort decryption", "放弃解密");
                RegisterString("epdt" + id.ToString() + "-1", "Decryption and analyze", "解密并分析"); // 需要消灭：行星名称上的黑雾基地（点击标记）
            }
            {
                int id = 2203;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 300 }, new int[] { 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 14 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completes the destructive analysis of the logs, and the physical entities driven by the data indicators are scattered and stored in the ground dark fog base on a planet. Destroying all the dark fog bases and remove all the core drillers on the planet will retrieve the driving entities for further interpretation.", "分析模块完成了对日志的破坏性分析，数据指示元驱动的物理实体被分散保存在某行星上的地面黑雾基地中。摧毁并填埋该行星上的全部黑雾基地就可以取回驱动实体，以便进行下一步的解译了。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃搜寻");
                RegisterString("epdt" + id.ToString() + "-1", "Retrieve meta driver", "取回元驱动");
            }
            {
                int id = 2204;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 89999 }, new int[] { 1000 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 13, 21 }, new int[] { 0, 1800 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module found that the log entity appeared to be sealed in high dimensional space by an unknown energy, and Icarus was unable to detect any information. However, it may be possible to break this energy barrier with the help of stellar energies, which requires the help of mega structures.", "分析模块发现日志实体似乎被一股未知的能量密封在高维空间中，伊卡洛斯无法探查到任何信息。不过，借助恒星级别的能量或许能打破该能量屏障，这需要巨构的帮助。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort analyze", "放弃分析");
                RegisterString("epdt" + id.ToString() + "-1", "Break the energy barrier and analyze", "打破能量屏障并分析");
            }
            {
                int id = 2301;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 1, 1 }, new int[] { 500, 500 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 13, 23 }, new int[] { 0, 3600 * 3 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module found that the physical storage structure where the logs were recorded had been severely damaged, but the damage appeared to be reversible. Before performing the analysis, you first need to provide some necessary materials to repair the log.", "分析模块发现记载日志的物理存储结构已经受到了严重的损伤，不过该损伤似乎是可逆的。在执行分析之前，首先需要提供一些必要的材料来修复日志。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort repairing", "放弃修复");
                RegisterString("epdt" + id.ToString() + "-1", "Attempt to repair and analyze", "尝试修复并分析");
            }
            {
                int id = 2302;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 200 }, new int[] { 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 13, 21 }, new int[] { 0, 3600 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module found that the log itself was highly encrypted by rapidly changing the ciphertext in the time dimension. Obviously, maintaining the encryption required energy supply. Icarus easily pinpointed the energy as coming from a ground-based dark fog unit on a planet. If all ground dark fog units on the planet are destroyed, log decryption and analysis should be performed.", "分析模块发现了日志本身被高度加密了，加密的方式是通过在时间维度上快速改变密文实现的，显然，维持该加密需要能量的供给。伊卡洛斯轻易地定位到了这个能量来自于某个行星上的地面黑雾单位。如果摧毁了该行星上全部的地面黑雾单位，应该就可以执行日志的解密与分析程序了。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort decryption", "放弃解密");
                RegisterString("epdt" + id.ToString() + "-1", "Decryption and analyze", "解密并分析"); // 需要消灭：行星名称上的某个黑雾基地（点击标记）
            }
            {
                int id = 2303;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 5 }, new int[] { 50 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 13, 21 }, new int[] { 0, 7200 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module found that the analysis log was encrypted with a high-level mechanical live key, which could not be decrypted and analyzed without obtaining the full key. However, dark fog units carrying key fragments have been found near a star in the galaxy, and if enough space dark fog units are destroyed, a complete mechanical live key may be obtained.", "分析模块发现，分析日志被一种高阶的机械活体密钥加密了，如果不获取完整的密钥，便无法解密并分析。不过，星区中的某个恒星附近发现了携带密钥片段的黑雾单位，若能摧毁足够多的太空黑雾单位，或许就能获取完整的机械活体密钥。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort decryption", "放弃解密");
                RegisterString("epdt" + id.ToString() + "-1", "Decryption and analyze", "解密并分析");
            }
            {
                int id = 2304;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 89999 }, new int[] { 10000 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 13, 21 }, new int[] { 0, 3600 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module found that the log entity appeared to be sealed in high dimensional space by an unknown energy, and Icarus was unable to detect any information. However, it may be possible to break this energy barrier with the help of stellar energies, which requires the help of mega structures.", "分析模块发现日志实体似乎被一股未知的能量密封在高维空间中，伊卡洛斯无法探查到任何信息。不过，借助恒星级别的能量或许能打破该能量屏障，这需要巨构的帮助。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort analyze", "放弃分析");
                RegisterString("epdt" + id.ToString() + "-1", "Break the energy barrier and analyze", "打破能量屏障并分析");
            }
            // 3
            {
                int id = 3201;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 1 }, new int[] { 300 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 25 }, new int[] { 0, 3600 * 3 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completes the destructive analysis of the log and finds that the carrier itself is the meta-driven materialization matrix. Fortunately, with its remnant self-interpreting system, Icarus might be able to try to fix the meta-drive and make it interpretable, but this would need some necessary materials.", "分析模块完成了对日志的破坏性分析，发现其载体本身就是元驱动的实体化矩阵。幸运的是，借助其残存的自解译系统，伊卡洛斯或许可以尝试修复元驱动，使其变得可以被解译，但这将消耗一些必要的材料。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort repairing", "放弃修复");
                RegisterString("epdt" + id.ToString() + "-1", "Attempt to repair", "尝试修复");
            }
            {
                int id = 3202;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 400 }, new int[] { 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 18101 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed a destructive analysis of the log, which vaguely pointed to a planet, perhaps probing that planet could find traces of the meta-drive.", "分析模块完成了对日志的破坏性分析，日志模糊地指向了一个行星，或许探测该行星可以找到元驱动的痕迹。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort searching", "放弃搜寻");
                RegisterString("epdt" + id.ToString() + "-1", "Search on the planet", "在行星上搜寻"); // 需要到达：行星名称（点击标记）
            }
            {
                int id = 3203;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9999 }, new int[] { 150 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 24 }, new int[] { 0, 1800 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed a destructive analysis of the log, which, strangely enough, indicates that the meta-drive appeared to be encoded in disembodied form on energy fluctuations throughout the sector and is reacted to the disintegration of the dark fog units. Every time a dark fog unit is destroyed, it will trigger regular fluctuations encoded in the entire star region, and the analysis module may be able to find a way to interpret the drive by learning a large amount of regular fluctuation data. Once the sample size is large enough, Icarus can start the interpretation procedure.", "分析模块完成了对日志的破坏性分析，这个驱动似乎是以无实体的形式被编码在整个星区的能量波动上的，并且对黑雾单位的解体过程有反应。每当有黑雾单位被摧毁，都会激发整个星区编码的规律性波动，分析模块或许可以通过学习大量的规律性波动数据来寻找解译驱动的方法。一旦样本量足够，伊卡洛斯就可以启动解译程序。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Run interpretation procedure", "执行解译程序");
            }
            {
                int id = 3204;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 11305, 30000 }, new int[] { 80, 1 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 24 }, new int[] { 0, 3600 * 8 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 19999, 24 }, new int[] { 0, 1200 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed the destructive analysis of the logs and easily located the meta-drive interpretation site, which Icarus needed to synchronize the compiled physical rules with the understandable code structure during the research. Alternatively, quantum chips can be used directly to assist interpretation, but this will take a longer time.", "分析模块完成了对日志的破坏性分析，并轻松定位了元驱动的解译位点，伊卡洛斯需要在研究过程中将编译在内的物理规则与可理解的代码结构进行同步，来完成解译过程。或者，也可以直接使用量子芯片辅助解译，但这将花费较长的时间。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Use quantum chip to support the interpretation", "使用量子芯片解译");
                RegisterString("epdt" + id.ToString() + "-2", "Sync physical rules", "同步物理规则");
            }
            {
                int id = 3301;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 100, 100 }, new int[] { 100, 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 14, 6, 7 }, new int[] { 0, -25, -25 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 14 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed the destructive analysis of the log and found that the specific positioning of the meta driver pointed to a space dark fog hive. It seems that the meta driver's data was distributed stored in multiple units and nodes belonging to the space dark fog hive. If you want to retrieve the complete meta driver, all units and structures of the dark fog hive, including the relay station, must be destroyed. Alternatively, Icarus attempts to decode as soon as it has acquired enough meta-drive data, but incomplete data will cause the final decoding track to be insufficiently separated, resulting in a large amount of redundant code, which will reduce the probability of finding higher-level APIs.", "分析模块完成了对日志的破坏性分析，发现元驱动的具体定位指向了一个太空黑雾巢穴，似乎元驱动的数据被分布式存储在了属于该太空黑雾巢穴的多个单位和节点中，如果想取回完整的元驱动，必须消灭该黑雾巢穴所有的单位和结构，包括中继站。或者，在伊卡洛斯获取了足够多的元驱动相关数据时立刻尝试进行解译，但不完整的数据会导致最终的解码轨分离得不够彻底，使得解码轨包含大量冗余代码，这会降低找到更高阶API的概率。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort searching", "放弃搜寻");
                RegisterString("epdt" + id.ToString() + "-1", "Retrieve partial meta driver", "取回部分元驱动");
                RegisterString("epdt" + id.ToString() + "-2", "Retrieve meta driver", "取回元驱动");
            }
            {
                int id = 3302;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9 }, new int[] { 15 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 14 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed a destructive analysis of the log, which mentioned that the meta driver data would appear in the higher order units of a star system, and if Icarus could increase the level of the dark fog hive in space of the star system, the analysis module should be able to find enough meta driver data.", "分析模块完成了对日志的破坏性分析，日志提到了元驱动的数据会出现在某恒星系高阶单位中，如果能够提升该恒星系太空黑雾巢穴的等级，伊卡洛斯应该就能找到足够的元驱动数据。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort searching", "放弃搜寻");
                RegisterString("epdt" + id.ToString() + "-1", "Retrieve meta driver", "取回元驱动");
            }
            {
                int id = 3303;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9999, 9996, 11305 }, new int[] { 400, 3, 500 });
                ep.SetDecision(0, new int[] { }, new int[] { -1, 25201 }, new int[] { 0, 800 });
                ep.SetDecision(1, new int[] { 2 }, new int[] { 14 }, new int[] { 0 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 14 }, new int[] { 0 });
                ep.SetDecision(3, new int[] { 0 }, new int[] { 14 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis log appears to have non-mechanical emotional tendencies from lower order life, and in the course of its destructive analysis, the analysis module finds biological emotions similar to fear, compassion, excitement, and curiosity. Obviously, pure log files do not carry the need for complex emotional coding. The analysis module supposes that the log itself carries meta driver information. Unfortunately, the personality of the log securely protects its own data, which cannot be read directly by the analysis module. We can try to kill its own kind to make it fear, so we can find a loophole for aggressive decryption. Or you can use its sympathy to lower its guard, or you can try to divert its attention by using lots of complex objects to arouse its curiosity. It seems willing to offer you some thanks if you give up this intrusion.", "这个分析日志似乎拥有来自低阶生命的非机械的情感倾向，分析模块在对其进行破坏性分析的过程中，发现了类似于恐惧、同情、兴奋和好奇的生物情感。显然，单纯的日志文件没有承载复杂情感编码的需要。分析模块推测日志本身就携带着元驱动信息。但不幸的是，日志的人格牢牢地保护着自身的数据，分析模块无法直接读取。我们可以尝试杀死他的同类来让它恐惧，从而找到漏洞进行攻击性破译。或者利用它的同情降低它的戒备，还可以尝试使用大量的复杂物体引起它的好奇来试图转移其注意。如果放弃对它的入侵，它似乎愿意为我们提供一些感谢。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃");
                RegisterString("epdt" + id.ToString() + "-1", "Distract it", "分散它的注意力");
                RegisterString("epdt" + id.ToString() + "-2", "Arouse its sympathy", "引起它的同情");
                RegisterString("epdt" + id.ToString() + "-3", "Make it afraid", "让它恐惧");
            }
            {
                int id = 3304;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9995, 15201 }, new int[] { 10, 1000 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 14, 8 }, new int[] { 0, 100 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 14 }, new int[] { 0 });
                ep.SetDecision(3, new int[] { }, new int[] { 14, 2 }, new int[] { 0, -1 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "Your attempt to exploit the laws of higher dimensional COSMO physics has been discovered by the COSMO Technology Ethics Committee, who have warned you that you must immediately terminate the relevant action. We can ignore their warnings and continue to acquire meta-drive data, but that could have consequences, or stop violating cosmic conventions.", "你尝试利用高维宇宙物理法则的违规行为已被COSMO技术伦理委员会发现，他们警告你必须立刻终止相关行动。我们可以无视他们的警告，继续获取元驱动数据，但这可能会带来后果，或者停止违反宇宙公约的行为。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Warn the COSMO Technology Ethics Committee not to meddle", "警告COSMO技术伦理委员会不要多管闲事");
                RegisterString("epdt" + id.ToString() + "-2", "Bribe the COSMO Technology Ethics Committee", "贿赂COSMO技术伦理委员会");
                RegisterString("epdt" + id.ToString() + "-3", "Ignore them", "无视他们");
            }
            {
                int id = 3401;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 5 }, new int[] { 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 14 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed the destructive analysis of the log, and found that the specific positioning of the meta-drive pointed to a star system, and it seemed that the meta-drive data was distributed stored in all the space dark fog ships and hives belonging to that star system, and we had to destroy all the space dark fog units of the star system to obtain the complete meta-drive.", "分析模块完成了对日志的破坏性分析，发现元驱动的具体定位指向了一个恒星系，似乎元驱动的数据被分布式存储在了属于该星系的所有太空黑雾单位和巢穴中，我们不得不消灭该星系的全部太空黑雾单位来获取完整的元驱动。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort searching", "放弃搜寻");
                RegisterString("epdt" + id.ToString() + "-1", "Retrieve meta driver", "取回元驱动");
            }
            {
                int id = 3402;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 89999 }, new int[] { 30000 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 14 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis module completed a destructive analysis of the log, which showed that the meta-drive was actually enclosed in the star by the dark fog, although it is not known how they did it, but this stellar operation carried out by mechanical creatures can apparently be cracked by stellar energy. Using enough stellar energy should be able to snatch the meta driver out of the star in the form of quantum wave fluctuations.", "分析模块完成了对日志的破坏性分析，日志显示，元驱动居然被黑雾封闭在了恒星之中，尽管不知道他们是如何做到的，但是这种由机械生物进行的恒星级操作显然可以通过恒星级的能量破解。使用足够的恒星能量应该能够将元驱动以量子波涨落的形式从恒星中攫取出来。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃搜寻");
                RegisterString("epdt" + id.ToString() + "-1", "Retrieve meta driver", "取回元驱动");
            }
            {
                int id = 3403;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9999, 9996, 11305 }, new int[] { 500, 5, 600 });
                ep.SetDecision(0, new int[] { }, new int[] { -1, 25201 }, new int[] { 0, 800 });
                ep.SetDecision(1, new int[] { 2 }, new int[] { 14 }, new int[] { 0 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 14 }, new int[] { 0 });
                ep.SetDecision(3, new int[] { 0 }, new int[] { 14 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "The analysis log appears to have non-mechanical emotional tendencies from lower order life, and in the course of its destructive analysis, the analysis module finds biological emotions similar to fear, compassion, excitement, and curiosity. Obviously, pure log files do not carry the need for complex emotional coding. The analysis module supposes that the log itself carries meta driver information. Unfortunately, the personality of the log securely protects its own data, which cannot be read directly by the analysis module. We can try to kill its own kind to make it fear, so we can find a loophole for aggressive decryption. Or you can use its sympathy to lower its guard, or you can try to divert its attention by using lots of complex objects to arouse its curiosity. It seems willing to offer you some thanks if you give up this intrusion.", "这个分析日志似乎拥有来自低阶生命的非机械的情感倾向，分析模块在对其进行破坏性分析的过程中，发现了类似于恐惧、同情、兴奋和好奇的生物情感。显然，单纯的日志文件没有承载复杂情感编码的需要。分析模块推测日志本身就携带着元驱动信息。但不幸的是，日志的人格牢牢地保护着自身的数据，分析模块无法直接读取。我们可以尝试杀死他的同类来让它恐惧，从而找到漏洞进行攻击性破译。或者利用它的同情降低它的戒备，还可以尝试使用大量的复杂物体引起它的好奇来试图转移其注意。如果放弃对它的入侵，它似乎愿意为我们提供一些感谢。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃");
                RegisterString("epdt" + id.ToString() + "-1", "Distract it", "分散它的注意力");
                RegisterString("epdt" + id.ToString() + "-2", "Arouse its sympathy", "引起它的同情");
                RegisterString("epdt" + id.ToString() + "-3", "Make it afraid", "让它恐惧");
            }
            {
                int id = 3404;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9995, 15201 }, new int[] { 10, 1000 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 14, 8 }, new int[] { 0, 100 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 14 }, new int[] { 0 });
                ep.SetDecision(3, new int[] { }, new int[] { 14, 2 }, new int[] { 0, -1 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Log Analysis", "分析日志");
                RegisterString("epd" + id.ToString(), "Your attempt to exploit the laws of higher dimensional COSMO physics has been discovered by the COSMO Technology Ethics Committee, who have warned you that you must immediately terminate the relevant action. We can ignore their warnings and continue to acquire meta-drive data, but that could have consequences, or stop violating cosmic conventions.", "你尝试利用高维宇宙物理法则的违规行为已被COSMO技术伦理委员会发现，他们警告你必须立刻终止相关行动。我们可以无视他们的警告，继续获取元驱动数据，但这可能会带来后果，或者停止违反宇宙公约的行为。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Warn the COSMO Technology Ethics Committee not to meddle", "警告COSMO技术伦理委员会不要多管闲事");
                RegisterString("epdt" + id.ToString() + "-2", "Bribe the COSMO Technology Ethics Committee", "贿赂COSMO技术伦理委员会");
                RegisterString("epdt" + id.ToString() + "-3", "Ignore them", "无视他们");
            }
            // 4
            {
                int id = 4201;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 19486 }, new int[] { 10 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { }, new int[] { 19999, 24, 6, 7 }, new int[] { 0, 3600 * 3, -25, -50 });
                ep.SetDecision(2, new int[] { 0 }, new int[] { 19999, 24 }, new int[] { 0, 3600 * 10 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Search for Meta-Drive", "搜寻元驱动");
                RegisterString("epd" + id.ToString(), "The meta-drive was recovered from the wreckage of the dark fog units, but it was partially damaged, and Icarus could now interpret it directly, but it could cause the final decoding track to be insufficiently separated and contain a lot of redundant code, which would reduce the probability of finding higher-level APIs. Alternatively, Icarus could use a quantum computer to predict missing source code during interpretation, which would circumvent this potential risk.", "元驱动被从黑雾巢穴的废墟中找到，不过已经部分损坏，伊卡洛斯现在可以直接对其进行解译，但可能导致最终的解码轨分离得不够彻底，使得解码轨包含大量冗余代码，这会降低找到更高阶API的概率。或者，伊卡洛斯可以借助量子计算机在解译的过程中预测缺失的源代码，这将规避这种潜在风险。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Run interpretation procedure directly", "直接解译");
                RegisterString("epdt" + id.ToString() + "-2", "Use quantum computer to predict missing source codes", "使用量子计算机预测缺失的源代码");
            }
            {
                int id = 4301;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 19486 }, new int[] { 100 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { }, new int[] { 19998, 6, 7 }, new int[] { 0, -25, -50 });
                ep.SetDecision(2, new int[] { 0 }, new int[] { 19998 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "The meta-drive was successfully retrieved, but it was partially damaged, and Icarus could now interpret it directly, but it could cause the final decoding track to be insufficiently separated and contain a lot of redundant code, which would reduce the probability of finding higher-level APIs. Alternatively, Icarus could use a quantum computer to predict missing source code during interpretation, which would circumvent this potential risk.", "元驱动被成功获取，不过已经部分损坏，伊卡洛斯现在可以直接对其进行解译，但可能导致最终的解码轨分离得不够彻底，使得解码轨包含大量冗余代码，这会降低找到更高阶API的概率。或者，伊卡洛斯可以借助量子计算机在解译的过程中预测缺失的源代码，这将规避这种潜在风险。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Run interpretation procedure directly", "直接解译");
                RegisterString("epdt" + id.ToString() + "-2", "Use quantum computer to predict missing source codes", "使用量子计算机预测缺失的源代码");
            }
            {
                int id = 4302;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 30000 }, new int[] { 3 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19998 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "The meta-drive was successfully acquired, and Icarus needed to synchronize the compiled physical rules with the understandable code structure during the reasearch to complete the interpretation process.", "元驱动被成功获取，伊卡洛斯需要在研究过程中将编译在内的物理规则与可理解的代码结构进行同步，来完成解译过程。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Sync physical rules", "同步物理规则");
            }
            {
                int id = 4303;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9999 }, new int[] { 300 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19998 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "The meta driver was successfully obtained, and this drive appears to be reacted to the disintegration of the dark fog units. Every time a dark fog unit is destroyed, it will trigger regular fluctuations encoded in the entire star region, and the analysis module may be able to find a way to interpret the drive by learning a large amount of regular fluctuation data. Once the sample size is large enough, Icarus can start the interpretation procedure.", "元驱动被成功获取，这个驱动似乎对黑雾单位的解体过程有反应。每当有黑雾单位被摧毁，都会激发整个星区编码的规律性波动，分析模块或许可以通过学习大量的规律性波动数据来寻找解译驱动的方法。一旦样本量足够，伊卡洛斯就可以启动解译程序。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Continue interpretation procedure", "继续解译程序");
            }
            {
                int id = 4304;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 1, 1 }, new int[] { 500, 500 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19998, 22 }, new int[] { 0, 3600 * 3 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "The meta-drive is successfully obtained, but its physical entity is partially damaged, and some necessary resources must be provided to repair it before the interpretation process can continue.", "元驱动被成功获取，但其物理实体已部分损坏，必须提供一些必要的资源来对其进行修复才能够继续解译进程。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Attempt to repair", "尝试修复");
            }
            {
                int id = 4401;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 19486 }, new int[] { 200 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { }, new int[] { 19998, 6, 7 }, new int[] { 0, -25, -50 });
                ep.SetDecision(2, new int[] { 0 }, new int[] { 19998 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "The meta-drive was successfully retrieved, but it was partially damaged, and Icarus could now interpret it directly, but it could cause the final decoding track to be insufficiently separated and contain a lot of redundant code, which would reduce the probability of finding higher-level APIs. Alternatively, Icarus could use a quantum computer to predict missing source code during interpretation, which would circumvent this potential risk.", "元驱动被成功获取，不过已经部分损坏，伊卡洛斯现在可以直接对其进行解译，但可能导致最终的解码轨分离得不够彻底，使得解码轨包含大量冗余代码，这会降低找到更高阶API的概率。或者，伊卡洛斯可以借助量子计算机在解译的过程中预测缺失的源代码，这将规避这种潜在风险。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Run interpretation procedure directly", "直接解译");
                RegisterString("epdt" + id.ToString() + "-2", "Use quantum computer to predict missing source codes", "使用量子计算机预测缺失的源代码");
            }
            {
                int id = 4402;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 30000 }, new int[] { 3 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19998 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "The meta-drive was successfully acquired, and Icarus needed to synchronize the compiled physical rules with the understandable code structure during the research to complete the interpretation process.", "元驱动被成功获取，伊卡洛斯需要在研究过程中将编译在内的物理规则与可理解的代码结构进行同步，来完成解译过程。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Sync physical rules", "同步物理规则");
            }
            {
                int id = 4403;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 9999 }, new int[] { 1200 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19998 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "The meta driver was successfully obtained, and this drive appears to be reacted to the disintegration of the dark fog units. Every time a dark fog unit is destroyed, it will trigger regular fluctuations encoded in the entire star region, and the analysis module may be able to find a way to interpret the drive by learning a large amount of regular fluctuation data. Once the sample size is large enough, Icarus can start the interpretation procedure.", "元驱动被成功获取，这个驱动似乎对黑雾单位的解体过程有反应。每当有黑雾单位被摧毁，都会激发整个星区编码的规律性波动，分析模块或许可以通过学习大量的规律性波动数据来寻找解译驱动的方法。一旦样本量足够，伊卡洛斯就可以启动解译程序。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Continue interpretation procedure", "继续解译程序");
            }
            {
                int id = 4404;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 1, 1 }, new int[] { 800, 800 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19998, 22 }, new int[] { 0, 3600 * 3 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "The meta-drive is successfully obtained, but its physical entity is partially damaged, and some necessary resources must be provided to repair it before the interpretation process can continue.", "元驱动被成功获取，但其物理实体已部分损坏，必须提供一些必要的资源来对其进行修复才能够继续解译进程。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Attempt to repair", "尝试修复");
            }
            {
                int id = 4405;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 5 }, new int[] { 0 });
                ep.SetDecision(0, new int[] { }, new int[] { -1 }, new int[] { 0 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19998 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Retrieve Meta-Drive", "寻回元驱动");
                RegisterString("epd" + id.ToString(), "A portion of the meta-drive is successfully obtained, but it is quantum bound to all the space dark fog units of a star system, and the nonphysical code of the meta-drive is transmitted repeatedly between all the units in the form of probability waves. It cannot collapse through any observation so we can't continue the interpretation process until we've wiped out all the space dark fog units in that star system.", "元驱动的一部分被成功获取，但其和某个星系的全部太空黑雾单位处于量子绑定的状态，元驱动的非实体编码在所有单位间以概率波的形式反复传递，却无法通过任何观察而坍缩。我们只有消灭了该星系的所有太空黑雾单位才能继续解译进程。");
                RegisterString("epdt" + id.ToString() + "-0", "Abort interpretation", "放弃解译");
                RegisterString("epdt" + id.ToString() + "-1", "Continue interpretation procedure", "继续解译程序");
            }
            // 8 用于衔接末尾
            {
                int id = 8101;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { }, new int[] { });
                ep.SetDecision(0, new int[] { }, new int[] { 19999, 24 }, new int[] { 0, 1200 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Search for Meta-Drive", "搜寻元驱动");
                RegisterString("epd" + id.ToString(), "The energy fluctuations of the meta-drive were successfully located, and the well-preserved meta-drive could be directly connected to Icarus for interpretation.", "元驱动的能量波动被成功定位，这个保存完好的元驱动可以直接连接伊卡洛斯以进行解译。");
                RegisterString("epdt" + id.ToString() + "-0", "Run interpretation procedure", "进行解译");
            }
            // 9
            {
                int id = 9997;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { }, new int[] { });
                ep.SetDecision(0, new int[] { }, new int[] { 0 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Interpret Meta-Drive", "元驱动解译");
                RegisterString("epd" + id.ToString(), "The meta-drive information is partially interpreted, but divided into multiple decoding tracks, some of which are currently close to stable. You can directly select one of them and separate it from the other tracks, which will clarify and enable its actual utility, allowing us to directly call the API in the future to circumvent or override some lower-order cosmic laws, but it will also destroy the possibility of extracting other decoded tracks. You can also use the friend information of the dark fog matrix to recompile all tracks and reinterpret to try to find other metastable decoded tracks. Note, however, that every time you do the recompile-interpretation process, you make the meta-drive more unstable, which increases the difficulty of the next compilation and doubles the cost of the dark fog matrix.", "元驱动的信息被部分地解译了出来，但是分成了多条解码轨道，目前一些解码轨道是接近稳定的。你可以直接从中选择一条，将其从其他轨道中分离出来，这将明确并启用它的实际功效，使我们未来可以直接调用该API来规避或重写一些低阶的宇宙法则，但这也会摧毁提取其他解码轨的可能性。你也可以使用黑雾矩阵的友元信息重新编译所有轨道，并重新解译以试图找出其他亚稳态的解码轨。但注意，每次你进行重新编译-解译过程，都会使得元驱动变得不稳定，从而增加下一次编译的难度，并加倍黑雾矩阵的花费。");
                RegisterString("epdt" + id.ToString() + "-0", "OK", "好的");
            }
            {
                int id = 9998;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { 15205, 15204, 15202 }, new int[] { 300, 500, 500 });
                ep.SetDecision(0, new int[] { }, new int[] { 19999, 24 }, new int[] { 0, 3600 * 5 });
                ep.SetDecision(1, new int[] { 0 }, new int[] { 19999, 24, 9 }, new int[] { 0, 3600 * 5, 3 });
                ep.SetDecision(2, new int[] { 1 }, new int[] { 19999, 24, 8 }, new int[] { 0, 3600 * 5, 25 });
                ep.SetDecision(3, new int[] { 2 }, new int[] { 19999, 24, 6, 7 }, new int[] { 0, 3600 * 5, 25, 25 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Interpret Meta-Drive", "元驱动解译");
                RegisterString("epd" + id.ToString(), "In several previous interpretations, the analysis module found ways to influence the separation of the decoding tracks during the interpretation process and thus affect the probability distribution of the ultimately discoverable high-dimensional API. Icarus can now adjust the interpretation method before the interpretation process begins to obtain some adjustment effects, but only one of them can be chosen.", "在先前多次的解译进程中，分析模块发现了一些方法可以影响解译过程对解码轨的分离，从而影响最终可探知的高维API的概率分布。伊卡洛斯现在可以在解译进程开始前调整解译手段，从而获得某些调整效果，但只能选择其中一种。");
                RegisterString("epdt" + id.ToString() + "-0", "OK", "直接解译");
                RegisterString("epdt" + id.ToString() + "-1", "OK", "稳定元驱动");
                RegisterString("epdt" + id.ToString() + "-2", "OK", "激发元驱动负熵波动");
                RegisterString("epdt" + id.ToString() + "-3", "OK", "预解译并分离");
            }
            {
                int id = 9999;
                var ep = new EventProto(id);
                ep.SetRequest(new int[] { }, new int[] { });
                ep.SetDecision(0, new int[] { }, new int[] { 0 }, new int[] { 0 });
                EventSystem.protos.Add(id, ep);
                RegisterString("ept" + id.ToString(), "Interprete Meta-Drive", "元驱动解译");
                RegisterString("epd" + id.ToString(), "The meta-drive information is partially interpreted, and divided into multiple decoding tracks, some of which are currently close to stable. You can directly select one of them and separate it from the other tracks, then continue to complete the rest of the interpretation work, which will clarify and enable its actual utility, allowing us to directly call the API in the future to circumvent or override some lower-order cosmic laws, but it will also destroy the possibility of extracting other decoded tracks. You can also use the friend information of the dark fog matrix to recompile all tracks and reinterpret to try to find other metastable decoded tracks. Note, however, that every time you do the recompile-interpretation process, you make the meta-drive more unstable, which increases the difficulty of the next compilation and doubles the cost of the dark fog matrix.", "元驱动的信息被部分地解译了出来，并分成了多条解码轨道，目前一些解码轨道是接近稳定的。你可以直接从中选择一条，将其从其他轨道中分离出来，然后继续完成剩余的解译工作，这将明确并启用它的实际功效，使我们未来可以直接调用该API来规避或重写一些低阶的宇宙法则，但这也会摧毁提取其他解码轨的可能性。你也可以使用黑雾矩阵的友元信息重新编译所有轨道，并重新解译以试图找出其他亚稳态的解码轨。但注意，每次你进行重新编译-解译过程，都会使得元驱动变得不稳定，从而增加下一次编译的难度，并加倍黑雾矩阵的花费。");
                RegisterString("epdt" + id.ToString() + "-0", "OK", "好的");
            }

            EventSystem.alterItems = new List<List<Tuple<int, int>>>();
            List<Tuple<int, int>> altItem0 = new List<Tuple<int, int>>();
            for (int i = 1101; i <= 1109; i++)
            {
                if (i != 1107)
                    altItem0.Add(new Tuple<int, int>(i, 100));
            }
            for (int i = 1201; i <= 1203; i++)
            {
                altItem0.Add(new Tuple<int, int>(i, 50));
            }
            altItem0.Add(new Tuple<int, int>(1301, 50));
            EventSystem.alterItems.Add(altItem0);

            List<Tuple<int, int>> altItem1 = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1107, 200),
                new Tuple<int, int>(1119, 100),
                new Tuple<int, int>(1204, 200),
                new Tuple<int, int>(1205, 100),
                new Tuple<int, int>(1206, 100),
                new Tuple<int, int>(1303, 100),
                new Tuple<int, int>(1401, 50)
            };
            EventSystem.alterItems.Add(altItem1);

            List<Tuple<int, int>> altItem2 = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1107, 300),
                new Tuple<int, int>(1119, 200),
                new Tuple<int, int>(1124, 400),
                new Tuple<int, int>(1125, 100),
                new Tuple<int, int>(1205, 200),
                new Tuple<int, int>(1206, 200),
                new Tuple<int, int>(1303, 200),
                new Tuple<int, int>(1304, 100),
                new Tuple<int, int>(1305, 40)
            };
            EventSystem.alterItems.Add(altItem2);

            List<Tuple<int, int>> altItem3 = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1124, 500),
                new Tuple<int, int>(1125, 200),
                new Tuple<int, int>(1126, 200),
                new Tuple<int, int>(1127, 100),
                new Tuple<int, int>(1205, 400),
                new Tuple<int, int>(1206, 400),
                new Tuple<int, int>(1209, 50),
                new Tuple<int, int>(1304, 300),
                new Tuple<int, int>(1305, 100),
                new Tuple<int, int>(1402, 200),
                new Tuple<int, int>(1403, 200),
                new Tuple<int, int>(1803, 30),
                new Tuple<int, int>(9481, 50),
                new Tuple<int, int>(9484, 80),
                new Tuple<int, int>(6005, 100)
            };
            EventSystem.alterItems.Add(altItem3);


            List<Tuple<int, int>> altItem4 = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1126, 300),
                new Tuple<int, int>(1127, 200),
                new Tuple<int, int>(1205, 600),
                new Tuple<int, int>(1206, 600),
                new Tuple<int, int>(1209, 100),
                new Tuple<int, int>(1210, 300),
                new Tuple<int, int>(1304, 400),
                new Tuple<int, int>(1305, 200),
                new Tuple<int, int>(1402, 400),
                new Tuple<int, int>(1403, 400),
                new Tuple<int, int>(1804, 30),
                new Tuple<int, int>(9481, 120),
                new Tuple<int, int>(9484, 150),
                new Tuple<int, int>(9486, 50),
                new Tuple<int, int>(6005, 200),
                new Tuple<int, int>(6006, 150),
                new Tuple<int, int>(5202, 300),
                new Tuple<int, int>(5203, 200),
                new Tuple<int, int>(5204, 300),
                new Tuple<int, int>(5205, 50)
            };
            EventSystem.alterItems.Add(altItem4);

            EventSystem.maxProbabilityBy10Minutes = new List<double>
            {
                0, // 0-10
                0, // 10-20min
                0.0001, // 20-30min
                0.0002, // 30-40min
                0.0005, // 40-50min
                0.001, // 50-60min
                0.002, // 60-70min
                0.003, // 70-80min
                0.004, // 80-90min
                0.005, // 90-100min
                0.006, // 100-110min
                0.008, // 110-120min
                0.01, // 120+min
                0.01, // 120+min
                0.01, // 120+min
                0.01 // 120+min
            };
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
                case 80:
                    __result = "伤害".Translate();
                    break;
                case 81:
                    __result = "最大耐久度".Translate();
                    break;
                case 82:
                    __result = "伤害类型".Translate();
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
                case 80:
                    __result = Relic.HaveRelic(0, 10)
                        ? (Configs.dropletAtk / 100).ToString("0.0") + "<color=#61D8FFB8> + " + (Droplets.bonusDamage / 100).ToString("0.0#") + "</color> hp"
                        : (Configs.dropletAtk / 100).ToString("0.0") + " hp";
                    break;
                case 81:
                    __result = "∞";
                    break;
                case 82:
                    __result = "子弹伤害类型".Translate() + "伤害类型特殊".Translate();
                    return;
            }
        }



        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(LabMatrixEffect), "Update")]
        //public static bool LabMatrixEffectUpdatePatch(ref LabMatrixEffect __instance)
        //{
        //    __instance.time += (double)Time.deltaTime;
        //    double num = __instance.time / 1000.0;
        //    float num2 = (float)((num - Math.Floor(num)) * 1000.0);
        //    Vector3 localEulerAngles = new Vector3(num2 * 37f, num2 * 101f, num2 * 23f);
        //    __instance.matrixCube.localEulerAngles = localEulerAngles;
        //    int currentTech = __instance.history.currentTech;
        //    TechProto techProto = LDB.techs.Select(currentTech);
        //    bool flag = false;
        //    if (currentTech > 0 && techProto != null && techProto.IsLabTech && GameMain.history.techStates.ContainsKey(currentTech))
        //    {
        //        TechState techState = __instance.history.techStates[currentTech];
        //        flag = true;
        //    }
        //    if (!flag)
        //    {
        //    }
        //    __instance.techGroup.gameObject.SetActive(flag);
        //    Array.Clear(__instance.techMatUse, 0, __instance.techMatUse.Length);
        //    if (flag)
        //    {
        //        for (int i = 0; i < techProto.Items.Length; i++)
        //        {
        //            int num3 = techProto.Items[i] - LabComponent.matrixIds[0];
        //            if (num3 < 0 || num3 > __instance.techMatUse.Length) continue;
        //            __instance.techMatUse[num3] = true;
        //        }
        //    }
        //    for (int j = 0; j < __instance.techCubes.Length; j++)
        //    {
        //        __instance.techCubes[j].gameObject.SetActive(__instance.techMatUse[j]);
        //    }
        //    int num4 = 0;
        //    for (int k = 0; k < __instance.techCubes.Length; k++)
        //    {
        //        if (__instance.techMatUse[k])
        //        {
        //            num4++;
        //        }
        //    }
        //    int num5 = 0;
        //    for (int l = 0; l < __instance.techCubes.Length; l++)
        //    {
        //        if (__instance.techCubes[l].gameObject.activeSelf)
        //        {
        //            float num6 = 360f * (float)num5 / (float)num4;
        //            float f = (num2 * 47f + num6) * 0.017453292f;
        //            float f2 = num2 * 121f * 0.017453292f;
        //            Vector3 localPosition = new Vector3(Mathf.Cos(f) * 0.7f, Mathf.Sin(f) * 0.7f, Mathf.Sin(f2) * 0.5f);
        //            __instance.techCubes[l].localPosition = localPosition;
        //            Vector3 localEulerAngles2 = new Vector3(num2 * 37f, num2 * 101f + num6, num2 * 23f);
        //            __instance.techCubes[l].localEulerAngles = localEulerAngles2;
        //            num5++;
        //        }
        //    }

        //    return false;
        //}


        //public static int[] GetBulletInfos(int protoId)
        //{
        //    switch (protoId)
        //    {
        //        case 8001:
        //            return new int[] { Configs.bullet1Atk, Mathf.RoundToInt((float)Configs.bullet1Speed), 0 };
        //        case 8002:
        //            return new int[] { Configs.bullet2Atk, Mathf.RoundToInt((float)Configs.bullet2Speed), 0 };
        //        case 8003:
        //            return new int[] { Configs.bullet3Atk, Mathf.RoundToInt((float)Configs.bullet3Speed), 0 };
        //        case 8004:
        //            return new int[] { Configs.missile1Atk, Mathf.RoundToInt((float)Configs.missile1Speed), Configs.missile1Range };
        //        case 8005:
        //            return new int[] { Configs.missile2Atk, Mathf.RoundToInt((float)Configs.missile2Speed), Configs.missile2Range };
        //        case 8006:
        //            return new int[] { Configs.missile3Atk, Mathf.RoundToInt((float)Configs.missile3Speed), Configs.missile3Range };
        //        case 8007:
        //        case 8014:
        //            return new int[] { Configs.bullet4Atk, Mathf.RoundToInt((float)Configs.bullet4Speed), 0 };
        //        case 9511: //水滴
        //            return new int[] { Configs.dropletAtk, Mathf.RoundToInt((float)Configs.dropletSpd), 0 };
        //        default:
        //            return new int[] { 0, 0, 0 };
        //    }
        //}

        ////public static void ChangeTabName(Proto proto)
        ////{
        ////    if(proto is StringProto && proto.name == "MegaStructures")
        ////    {
        ////        var item = proto as StringProto;
        ////        item.ZHCN = "轨道防御";
        ////        item.ENUS = "Defense";
        ////        item.FRFR = "Defense";
        ////    }
        ////}

        public static void ReCheckTechUnlockRecipes()
        {
            //if (!GameMain.history.TechState(1920).unlocked && GameMain.history.RecipeUnlocked(538))
            //{
            //    GameMain.history.recipeUnlocked.Remove(538);
            //}
            //if (!GameMain.history.TechState(1921).unlocked && GameMain.history.RecipeUnlocked(540))
            //{
            //    GameMain.history.recipeUnlocked.Remove(540);
            //}
            //if (!GameMain.history.TechState(1922).unlocked)
            //{
            //    if (GameMain.history.RecipeUnlocked(537))
            //        GameMain.history.recipeUnlocked.Remove(537);
            //    if (GameMain.history.RecipeUnlocked(541))
            //        GameMain.history.recipeUnlocked.Remove(541);
            //}
            //if (!GameMain.history.TechState(1923).unlocked && GameMain.history.RecipeUnlocked(542))
            //{
            //    GameMain.history.recipeUnlocked.Remove(542);
            //}
            //if (!GameMain.history.TechState(1924).unlocked && GameMain.history.RecipeUnlocked(539))
            //{
            //    GameMain.history.recipeUnlocked.Remove(539);
            //}
        }


        ////[HarmonyPrefix]
        ////[HarmonyPatch(typeof(TechProto), "GenPropertyOverrideItems")]
        ////public static bool TechGenOverrideItemsPatch(ref TechProto __instance, TechProto proto)
        ////{
        ////    if (!Configs.developerMode) return true;
        ////    if (proto.PropertyOverrideItems != null) return true;
        ////    __instance.PropertyItemCounts = new int[] { 1 };
        ////    __instance.PropertyOverrideItems = new int[] { 6001 };
        ////    __instance.PropertyOverrideItemArray = new IDCNT[1];
        ////    __instance.PropertyOverrideItemArray[0] = new IDCNT(6001, 1);
        ////    return false;
        ////}


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(UITechNode), "OnBuyoutButtonClick")]
        //public static bool BuyoutTechPrePatch(ref UITechNode __instance, int _data)
        //{
        //    TechProto tech = __instance.techProto;
        //    if (tech == null || tech.Items == null) return true;
        //    for (int i = 0; i < tech.Items.Length; i++)
        //    {
        //        if(tech.Items[i] >= 6003 && tech.Items[i] <= 6006 || tech.Items[i] == 8032 || tech.ID == 1901)
        //        {
        //            UIRealtimeTip.Popup("被深空来敌mod禁止".Translate(), true, 0);
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(UITechNode), "UpdateInfoDynamic")]
        //public static void UITechNodeUpdateInfoDynamicPostPatch(ref UITechNode __instance)
        //{
        //    TechProto tech = __instance.techProto;
        //    if (tech == null || tech.Items == null) return;
        //    for (int i = 0; i < tech.Items.Length; i++)
        //    {
        //        if (tech.Items[i] >= 6003 && tech.Items[i] <= 6006 || tech.Items[i] == 8032 || tech.ID == 1901)
        //        {
        //            __instance.buyoutButton.transitions[0].normalColor = __instance.buyoutNormalColor1;
        //            __instance.buyoutButton.transitions[0].mouseoverColor = __instance.buyoutMouseOverColor1;
        //            __instance.buyoutButton.transitions[0].pressedColor = __instance.buyoutPressedColor1;
        //            return;
        //        }
        //    }

        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(UISpraycoaterWindow), "RefreshSpraycoaterWindow")]
        //public static void RefreshSpraycoaterWindowPostPatch(ref UISpraycoaterWindow __instance, SpraycoaterComponent spraycoater)
        //{
        //    if (Configs.enableProliferator4 && spraycoater.incItemId != 0 && spraycoater.incCount > 0)
        //    {
        //        ItemProto itemProto = LDB.items.Select(spraycoater.incItemId);
        //        if (itemProto.Ability >= 6)
        //        {
        //            Color color = new Color(0, 0.5f, 0.6f);
        //            __instance.tankFillOutlineImage.color = new Color(color.r, color.g, color.b, color.a * 4f);
        //            __instance.tankCountText.color = new Color(color.r, color.g, color.b, color.a * 4f);
        //            __instance.tankFillMaskImage.color = color;
        //            __instance.incInfoText1.color = new Color(color.r, color.g, color.b, color.a * 4f);
        //            __instance.incInfoText2.color = new Color(color.r, color.g, color.b, color.a * 4f);
        //            __instance.incInfoText3.color = new Color(color.r, color.g, color.b, color.a * 4f);
        //        }

        //    }
        //}
    }
}
