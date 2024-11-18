# They Come From Void / 深空来敌

With the Rise of  the DarkFog updated, They Come From Void has been transformed into a supplementary gameplay enhancement mod. The Relic system (now called the Meta Drive) and the Merit rank system were retained, as were the droplets, and the Star Cannon was transplanted into the MMS mod. Use these strange meta-drives to enhance your combat and production capabilities against enemies from the voi..... I mean.....  dark fog!

随着黑雾崛起的更新，深空来敌改头换面变成了一个辅助玩法强化的mod。遗物系统（现在叫元驱动）和功勋等级系统得以保留，水滴也还在，恒星炮则被移植到了巨构中。使用这些奇怪的元驱动强化你的战斗和生产能力，迎击深空来敌！

## Installation / 安装方法

### With Mod Manager / 使用Mod管理器

Simply open the mod manager (if you don't have it install it [here](https://dsp.thunderstore.io/package/ebkr/r2modman/)), select **They Come From Void by ckcz**, then **Download**.

If prompted to download with dependencies, select `Yes`.

Then just click **Start modded**, and the game will run with the mod installed.

只需要简单的打开mod管理器（如果你还没安装可以[点此安装](https://dsp.thunderstore.io/package/ebkr/r2modman/)，选择**They Come From Void by ckcz**，下载即可）。

如果弹窗提示需要下载前置，点击确定即可。

### Install manually / 手动安装

Install BepInEx from [here](https://dsp.thunderstore.io/package/xiaoye97/BepInEx/)<br/>
Install LDBTool from [here](https://dsp.thunderstore.io/package/xiaoye97/LDBTool/)<br/>
Install DSPModSave from [here](https://dsp.thunderstore.io/package/CommonAPI/DSPModSave/)<br/>
Install CommonAPI from [here](https://dsp.thunderstore.io/package/CommonAPI/CommonAPI/)<br/>
Install MoreMegaStructure from [here](https://dsp.thunderstore.io/package/jinxOAO/MoreMegaStructure/)<br/>

Then download the mod manully and unzip into `plugins` (including the `dll` and `dspbattletex` file).

在上述地址安装框架和几个前置mod，然后将本mod解压到`plugins`目录（包括`dll`和`dspbattletex`文件）。


## Compatibility / 兼容性
This mod is compatible with GenesisBook now.    
这个mod目前和创世之书完成了兼容，两个mod可以同时游玩。  
Installing this mod will lock the Star cannon from MoreMegaStructure with a hidden tech. You need to unlock the Nicoll-Dyson Beam tech before you plan a star cannon.  
安装这个mod会使得恒星炮被一个隐藏科技锁定，你需要先解锁该科技，再规划并建造恒星炮。  


## Feedback and suggestions / 意见和反馈

The config numbers of TCFV may still need to be considered. It may still have bugs. If you have any feedback, please file a new issue via [github](https://github.com/jinxOAO/DSP_Battle), or contact us in Discord `ckcz123#3576` or `jinxOAO`.

Also welcome to anyone who'd like to contribute to the mod.

深空来敌的数值可能仍需要斟酌，并且可能存在一些bug。如果你有任何的问题，可以通过[github](https://github.com/jinxOAO/DSP_Battle)发起一个issue，或者加QQ群`694213906`或`141801294`找群主和管理员反馈。

欢迎任何向为此mod进行贡献的人。

## Update Log


### 2024-8-15 V3.1.4
 - Updated to work with game version 0.10.30.23272.  
 - 更新以适配游戏版本0.10.30.23272。  
 - Fixed a bug where the meta driver Power Surge will always take effect even if you don't have it. 
 - 修复了一个bug，该bug曾导致即使你不具有能量涌动元驱动，其效果也会生效。
 - Reduced the free mega points by meta driver Honorary Promotion after rank 10 (galaxy guardian).
 - 减少了荣誉晋升元驱动在满级后提供的免费巨构点数。

### 2024-6-17 V3.1.3
 - Fixed a bug that the BGM setting won't show up.  
 - 修复一个BGM设置不出现的bug。  
 - Fixed an issue where armor penetration would increase the laser turret's damage abnormally. Now its calculation logic is consistent with the armor resistance logic in vanilla game.  
 - 修复一个护甲穿透对激光塔异常增伤的问题，现在它与游戏原本护甲对激光的减伤计算机制保持一致。  

### 2024-5-12 V3.1.2
 - Significantly reduces the volume of BGM during battles and provides a configuration for you to customize its volume level in config file. Additionally, you can also disable the BGM switching feature.  
 - 显著减小了战斗时BGM的音量，并且提供一个游戏外的配置来让你自定义其音量大小，此外，你也可以在配置文件中禁用战斗时改变BGM的功能。  
 - Meta drive Buff: Now the "Void Impact" will also prevent the Jammer Tower consuming the bullets once it is loaded.  
 - 元驱动增强：现在“虚空冲击”还会阻止干扰塔消耗弹药（在首次装载弹药之后）。  

### 2024-4-27 V3.1.1
 - Fixed a bug that "Shepherd of souls" doesn't work.   
 - 修复了一个“掘墓人”不生效的bug。 
 - Fixed a bug that "Honorary Promotion" will give tons of free megastructure points when you are promoted after reaching the highest rank. Now it will provide less points after rank 10 (the points before you reach the highest rank will not change).   
 - 修复了一个bug，该bug曾导致：“荣誉晋升”会在你达到满级之后继续升级时，提供与所需经验不匹配的巨量免费巨构点数。现在，它将在满级后提供更少的点数（满级前提供的点数不变）。  
 - Fixed a bug that the "Energy Burst" doesn't work with Genesis Book when making Dyson Sphere rockets.  
 - 修复了一个“能量迸发”在创世之书中对戴森球运载火箭不生效的bug。  
 - Changed some text description.  
 - 更改了一些文本描述。  

### 2024-3-24 V3.1.0
 - Added authorization point system, where you can gain buffs as you wish each time you raise your merit rank. In addition, after reaching the highest rank, you can still continuously earn merit points to repeatedly earn authorization points.  
 - 新增了授权点系统，你可以通过每次提升功勋阶级获得授权点来自由地强化你的生产或战斗能力。此外，在达到最高等级后，你仍然可以不断地获取功勋点数来反复获得授权点。
 - After reaching the final rank, a technology can be used to earn merit points.
 - 在达到最高等级后，会解锁一个科技用来获取功勋点数。  
 - Slightly increased the merit points required for promotions of lower rank.  
 - 略微增加了低阶晋升所需要的功勋点数。  
 - Reworked "Aftershock Echo" to trigger a weakened electromagnetic pulse when you kill ground units. If your archive already has this meta driver and there are no currently open meta-drive interpretation events, you will immediately receive a near-finished meta-drive interpretation event as compensation.  
 - 重做了“余震回响”，使得你有概率在击杀地面单位时引发一个弱化的电磁脉冲。如果你的存档已经拥有该元驱动，且当前没有已开启的元驱动解译事件，你会立刻获得一个接近结束的元驱动解译事件作为补偿。  
 - Now, "Tear of the goddess" and "Energy Burst" can work on the Star Assembly.  
 - 现在，“女神之泪”和“能量迸发”可以对星际组装厂生效。  
 - Fixed a bug that "Activated Carbon II" won't accelerate the EM ejector.  
 - 修复了“活性炭II”的加快弹射器弹射太阳帆速度的效果没有正常生效的bug。  
 - Fixed a bug of the "Frozen Tomb" and a joint interaction bug with "Void Impact".  
 - 修复了一个“冰封陵墓”的判定bug，以及与“虚空冲击”的联合交互bug。
 - Fixed a bug that the "Precision Echo" doesn't work correctly.  
 - 修复了“精密回响”未被正常加强的错误。
 - Significantly buffed the "Swallower" when killing the ground dark fog units.  
 - 显著加强了“吞噬者”对地面单位的效果。
 - Changed and corrected some description.  
 - 更改和更正了一些文本描述。  

<details>
  <summary> Click to view all </summary>  

### 2024-3-14 V3.0.5  
 - Fixed an error when having "Lovely motor" meta dirve with Genesis Book installed.  
 - 修复了一个安装有创世之书时阳间马达元驱动报错的问题。  

### 2024-3-10 V3.0.4  
 - Fixed an error when picking up meta drive "3".  
 - 修复了一个选择“三体”元驱动时报错的bug。  
 - Fixed a bug that the "Blue Buff" only effect the basic assembler machine in mod Genesis Book.   
 - 修复了“蓝buff”在创世之书中只对基础制造台生效的bug。 
 - Enhanced the positive effect of meta drive "Precision Echo".  
 - 强化了“精密回响”元驱动的正面效果。  
 - Changed some description.  
 - 更改了一些文本描述。  

### 2024-3-6 V3.0.3
 - Now the droplets can be put into the hangar, and can be used to auto replenish.  
 - 现在，水滴可以被放入机舱中并且自动填充。  
 - Fixed a combat bgm bug.  
 - 修复了一个战斗音乐的bug。  
 - Changed some description.  
 - 更改了一些文本描述。  

### 2024-3-5 V3.0.1&V3.0.2
 - Fixed an confliction with GS2 when you have binary star systems.  
 - 修复了一个和GS2的双星系统的冲突。  
 - Fixed a bug where you can't select the droplet fleet config.  
 - 修复了一个bug，该bug曾导致你无法选择水滴舰队类型。  
 - Fixed a compatible bug with Genesis Book.
 - 修复了一个和创世之书不兼容的bug。  
 - A UI displaying the rarity probability of meta drives was added to the event chain system.  
 - 在事件链系统中添加了显示元驱动稀有度概率的UI。  

### 2024-3-3 V3.0.0
 - Remove all combat systems from the old TCFV, including TCFV-enemies, wormholes, turret and planetary shield generator.  
 - 移除了深空来敌原有的战斗系统，这包括敌人、虫洞、防御塔和护盾生成器。  
 - Added a small event chain system to obtain the meta drive.   
 - 新增了一个小的事件链系统来获取元驱动。  
 - Changed a lot of meta drives (relics) effect to work properly with Rise of the Darkfog.   
 - 修改了许多元驱动（圣物）的效果来使其与黑雾崛起良好互动。   

### 2023-8-8 V2.2.8
 - Fix an error when removing a relic.  
 - 修复一个移除圣物导致的报错问题。  

### 2023-8-7 V2.2.7
 - Add new cursed relics, which have super powerful buffs, and also some negative effects.  
 - 新增受诅咒的圣物，他们是具有很强大效果但同时具有负面效果的圣物。
 - Echo II new effect: When you have Echo II, if you are holding missiles in your hand and you click a Missile Silo, automatically put one handheld missile into that silo immediately when you click the silo. (If that silo is empty)  
 - 回声 II 额外的效果：当你拥有回声II圣物后，手持某种类型的导弹并点击导弹发射井时，若该导弹发射井为空，则在点击时立刻填充1个手持的导弹。  
 - Fix a bug that your buildings won't be destroyed if you don't have Banshee's Veil.  
 - 修复了一个bug，该bug曾导致你不具有女妖面纱圣物时，建筑无法被摧毁。  
 - Fix a bug that the proliferator points disappear when the ammo is reloaded by Echo I or Echo II. 
 - 修复了回声I和回声II回填的弹药增产点数丢失的问题。  
 - Fix a confliction with BlueprintTweaks.
 - 修复了一个与BlueprintTweaks的初始化冲突问题。   
 - Fix some typo issues.  
 - 修复了一些文本错误。  

### 2023-5-5 V2.2.6
 - Fix a bug where missiles sometimes stay near the ground or return to the silo after launch.
 - 修复导弹发射后有时停留在地面附近或返回发射井的bug。
 - Add the datas of the Star Cannon and all relics to the game guidance. 
 - 游戏引导现在可以查看恒星炮和所有圣物的数据和说明。 
 - Fixed a bug where the silo may receive rocket input infinitely when the star cannon is firing.
 - 修复了火箭发射井在恒星炮开火时可能会无限制地接受火箭输入的bug。

### 2023-5-5 V2.2.5
 - Add game guidance and instructions, briefly displaying mod gameplay and enemy data.
 - 新增游戏引导和说明，简要展示mod玩法和敌人的数据。
 - Remake relic: Tear of the Goddess. New effect: When destroy an enemy ship in normal wave, gain 1 sorrow (max 1000). Each sorrow deals 0.02% <i>additional damage</i> to all enemy ships. After 1000 sorrow, replace the [star cannon fire] button with [Wrath of Goddess] during invasion. [Wrath of Goddess] consumes all sorrows, repels all enemy ships by at least 1AU (The closer the enemy ship is to the star, the farther it is repelled), and deals 95% of their max health as true damage. \nAfter launching the Wrath of Goddess, you will not be able to gain any sorrow until this wave ends.
 - If your existing archive already has the Tear of the Goddess, the original effect will not be changed, unless you remove the Tear of the Goddess and then repick it.
 - 女神泪重做：现在，新获取的女神之泪圣物效果会改为：我方在非精英波次摧毁敌舰时，叠加1层哀痛(上限1000)，每层哀痛使你造成0.02%额外伤害。满层后，在战斗中将你的[恒星炮开火]替换为[女神之怒]；发动[女神之怒]消耗所有哀痛，将所有敌船击退1AU以上（敌船距离恒星越近，击退得越远），并对他们造成最大生命值95%的真实伤害。在任何一次入侵中发动女神之怒后，该次入侵无法继续叠加哀痛。
 - 如果你的现有存档已经拥有女神之泪圣物，那么更新后不会改动其原本的效果，除非你移除女神之泪，之后重新获取它。
 - Fix a bug that Bloodthirster could not work correctly. Besides, now it will restore shields to the planet with the lowest shield value instead of random planets (if that planet's existing shield has not yet reached 150% of its maximum shield limit).
 - 修复了饮血剑无法正确生效的bug，并且现在他将为护盾值最低的行星恢复护盾而非随机的行星，前提是该行星现有护盾尚未达到护盾上限的150%。
 - Additional effect for relic Thornmail: If the damage that the shield should be taken is avoided by other relics, it will instead rebound 100% rather than 10%.
 - 虚空荆棘新增效果：如果原本将要作用于护盾的伤害被转移或被规避，则转而反弹100%的伤害而非10%。
 - Relic Knight's Vow will now correctly transfer the damage caused by suicide enemy ships, of course, you need to be prepared for the mech's energy to be greatly depleted.
 - 骑士之誓现在会正确地转移自杀型敌舰造成的巨量伤害，当然，你得做好机甲能量被大幅消耗的准备。
 - Fix a bug that the maximum shield limit keep randomly changing during battles.
 - 修复战斗中护盾上限乱跳的bug。
 - Fix some UI issues.
 - 修复一些UI问题。

### 2023-4-20 V2.2.4
 - Fix an error when starting a new game.
 - 修复一个开始新游戏时报错的问题。

### 2023-4-18 V2.2.3
 - The missile module will no longer reduce its firing rate after 90 seconds. The required construction points will be reduced from 100 to 20, but the firing rate provided by each module will be reduced from 6/min to 0.6/min.
 - 导弹模块不会再在战斗的90秒后降低射速，且需求的建造点数由100降低至20，但每个模块提供的射速由6/min降低至0.6/min。
 - The light spear module's damage will be enhanced +30% by each technology level, as same as the phase beam.
 - 光矛模块的输出伤害现在会受到与相位裂解光束一样的科技加成。
 - Relic: 3 and relic: Void seeker will now correctly reduce the launch energy consumption and launch energy threshold of droplets.
 - 圣物：三体 和 圣物：虚空索敌 现在会正确地降低水滴的发射能量消耗和发射能量判定阈值。
 - Relic: Banshee's Veil will now enhanced by the wave intensity, upto 200times immunity when the intensity reaches 10000.
 - 圣物：女妖面纱 现在会随着进攻波次强度的提升，最高提升至200次免疫效果（将在10000强度时达到上限）。
 - Rank: Conqueror II now has new effect: The droplets can quickly approach distant target.
 - 功勋阶级：征服者II 现在拥有了额外的奖励效果：水滴在追踪较远距离的敌人时能够快速接近目标。
 - BGM's random logic has been slightly adjusted.
 - 战斗BGM的随机逻辑进行了微小的调整。
 - Fix a bug that all the enemies will come out from the same wormhole after game loaded.
 - 修复了一个在读档后所有敌船将从同一个虫洞中生成的bug。
 - The missiles come from the star fortress will be launched more evenly from each megastructure's nodes.
 - 恒星要塞导弹现在会更均匀地从每个巨构节点中发射。
 - Fix some translation errors.  
 - 修复了一些翻译和文本错误。

### 2023-4-8 V2.2.2
 - Fix an error that may occur when loading a galaxy with less than 60 stars.
 - 修复一个可能在少于60个星系的存档中报错的问题。

### 2023-4-7 V2.2.0 & V2.2.1
 - Add the Star Fortress which can defend the entire star system, and largely save your planet surface area. You need to first build a mega structure (no matter what type it is), and then you can plan missiles or light spear modules. After that, you should use the vertical lauch silo to lauch the module rocket to build the star fortress's module points.
 - 新增了恒星要塞，它可以防御整个恒星系并极大地节约你的地表面积。你需要首先规划一个巨构（任何种类的巨构都可以），然后你就能规划增加导弹模块或光矛模块，之后你需要在地表用垂直发射井发射对应的运载火箭来建造这些模块。  
 - The relic Giga Cannon's additional damage 50,000% -> 20,000%.
 - 京级巨炮圣物提供的额外伤害：50000% -> 20000%。
 - Fix a bug that, when a station is destroied, if it had drones or vessels which are on their ways, the destination station's related order would not be removed correctly.
 - 修复了一个bug，该bug曾导致如果一个“已派出物流飞机且还没到达目的地”的物流塔被摧毁，目标物流塔会永久保留对应物品的需求。
 - Fix some translation errors.  
 - 修复了一些翻译和文本错误。

### 2022-12-9 V2.1.2
 - Fix a bug that the cannons sometimes keep changing targets without firing
 - 修复一个炮台反复寻敌而不开火的问题
 - Now the strong wave will sometimes give you an option to remove a specific relic rather than a random relic, when you already have more than 5 relics.
 - 现在，如果你已经拥有五个或者更多个圣物，那么在强大的波次结束后，你有可能获得一个选项来移除一个确定的圣物（而不再是随机的）。 
 - Add a relic that will give droplets bonus damage when droplet kills an enemy
 - 加入一个新的遗物，可以让水滴在击杀后永久提升伤害
 - Optimize some battle BGM's loop clip length and ending clip lenth
 - 优化部分战斗时BGM的循环节长度和结束片段的长度


### 2022-11-06 V2.1.1

 - Fix performance issue in battle caused by cannon
 - 修复战斗过程中炮弹寻敌的卡顿问题
 - Fix missile target selection bug
 - 修复火箭的目标选取的bug
 - Antimatter energy fuel receipt won't be affected by relic
 - 反物质燃料棒的配方不会被圣物效果所影响
 - Fix relic random issue
 - 修复圣物随机过程的bug

### 2022-10-24 V2.1.0
 - Added strong wave and relic system
 - 新增精英级别强大的进攻和遗物系统
 - A strong wave will appear in every 5 waves. Each strong wave will last at least 3 minutes. The enemy ship will also gain additional buffs
 - 强大的进攻会在每5次总计的进攻中出现，每次强大的进攻至少持续3分钟，敌舰也将获得加成效果
 - Relic will be found after a strong wave. Players can choose from randomly refreshing relics. Each relic has a different powerful bonus effect. They may significantly simplify the production line or strengthen the combat ability
 - 强大的进攻结束后会掉落圣物，玩家可以从随机刷新的圣物中选择，圣物均带有长久性的强大的加成效果，他们可能大幅简化产线或强化战斗能力
 - New BGM before, during and after the invasion
 - 战斗前夕、战斗中和结束后均加入了全新的BGM

### 2022-10-24 V2.0.9
 - Tiny fix
 - 修复一些错误

### 2022-10-23 V2.0.8

 - Star cannon will auto fire after phase 3
 - 恒星炮在阶段三后将自动开火
 - Optimize performance when battle is not on-going
 - 大幅优化非战斗状态下的游戏性能
 - Optimize performance of beam
 - 优化相位裂解光束的性能
 - Other balance adjustment
 - 其他的平衡性调整

### 2022-10-22 V2.0.7

 - The recovery efficiency of multiple shield generators will decrease in sequence
 - 多个护盾发生器对护盾的回复效率将依次递减
 - The shield energy won't decrease without power
 - 护盾在断电情况下将不再逸散
 - Increase the star cannon damage to wormhole; Increase the number of fire for early stages.
 - 大幅提升恒星炮的伤害值；在前几个阶段大幅提升开火次数
 - Star cannon can attack multiple wormhole one time.
 - 恒星炮增加溅射效果，可以同时攻击多个虫洞
 - Optimize the enemy generation logic, to keep almost same number for each enemty type.
 - 优化了敌舰的生成逻辑，现在总强度不变时会尽量让每种敌舰数量保持一致
 - Decrease the initial enemy speed, but they will accelerate during flying.
 - 降低了敌舰的初始速度，但是敌舰会在飞行时匀加速运动
 - Fix some UI issues
 - 部分UI显示的优化

### 2022-10-17 V2.0.6

 - Enemy damage to planet shield will increase over time.
 - 随着时间而提升敌人对护盾的伤害值
 - Fix the issue that the planet shield disappear if there is no enough energy
 - 修复没有足够能量时行星护盾立刻消失的问题

### 2022-09-28 V2.0.5
 - Updated to work with game version 0.9.27.14546
 - 更新以适配游戏版本0.9.27.14546

### 2022-06-16 V2.0.4
 - Updated to work with game version 0.9.26.12891
 - 更新以适配游戏版本0.9.26.12891

### 2022-05-11 V2.0.3
 - Fixed an issue where mining consumption rewards were stacking incorrectly.
 - 修复采矿消耗奖励错误叠加的问题。  
 - The maximum speed of the missile has been increased, but the firing frequency has been reduced to reduce missile waste.  
 - 导弹的最大速度大幅提升，但发射频率降低，以减少导弹的浪费。  

### 2022-04-29 V2.0.2
 - Fixed the bug that the game crashes when enemy droped alien matrix under certain circumstances.
 - 修复特定情况下敌人掉落物品闪退的bug。
 - You can now reverse the muzzle direction of the star cannon in the config file.
 - 现在你可以在config文件中让恒星炮的炮口方向反转。

### 2022-04-29 V2.0.1
 - Fix an import bug when loading old version archive.
 - 修复一个读取老版存档时发生的问题。

### 2022-04-29 V2.0.0
 - Added Nicoll-Dyson Beam, Planet shield, Droplet.  
 - 新增尼科尔戴森光束、行星护盾和水滴  
 - Added Merit level system, which can unlock better battle rewards and industrial production bonuses by leveling up. More merit points (Experience) in higher difficulties.  
 - 新增功勋等级系统，通过提升等级能够解锁更好的战斗奖励和工业生产加成，高难度下获得的功勋点数（经验值）更多   
 - Enemy ships now use a different model than logistics vessels; Gravitational collapse missiles now have a forced displacement effect; new items have been added   
 - 敌舰现在使用与物流船不同的模型；引力塌陷导弹现在具有强制位移（聚怪）效果；增添了全新的物品   


### 2022-04-03 V1.2.3

 - Updated to work with game version 0.9.25.11985
 - 更新以适配游戏版本0.9.25.11985

### 2022-04-03 V1.2.2

 - Miners won't be destroyed in Normal mode
 - 普通模式下，矿机不会被拆毁
 - Improve some translatations
 - 优化部分翻译

### 2022-03-25 v1.2.0

 - In normal mode, station attacked will turn into blueprint mode
 - 普通模式下，被攻击的物流塔会进入蓝图待建筑状态。
 - Optimize the determination of missiles
 - 优化火箭的伤害范围判定
 - Accelerate the missile fire rate with proliferator
 - 增加喷涂增产剂后火箭的射速

### 2022-03-20 V1.1.0

 - Enhance the damage of Phase-cracking beam
 - 增强相位裂解光束的伤害
 - Optimize the algorithm of missiles
 - 优化火箭的自动寻怪的算法
 - Fix the minus key on sand / dismantle mode
 - 修复地基和拆除模式下的减号键冲突问题
 - Fix crash when star count > 100
 - 修复当星系数量大于100时的报错

### 2022-03-19 V1.0.2

 - Fix UI on small resolution
 - 修复低分辨率下的显示问题

### 2022-03-17 V1.0.0
 - Initial Version
 - 初始版本

</details>