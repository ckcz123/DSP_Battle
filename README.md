# They Come From Void / 深空来敌

A tower-defense-like combat system for Dyson Sphere Program. Prepare your defense system, build your weapons and bullets, and... FIRE!!!!!

一个类塔防的戴森球计划战斗系统。构建你的防御体系，制作武器和子弹，迎击来犯之敌！

![Previews](https://raw.githubusercontent.com/ckcz123/DSP_Battle/master/previews/1.png)

![Previews](https://raw.githubusercontent.com/ckcz123/DSP_Battle/master/previews/2.jpg)

![Previews](https://raw.githubusercontent.com/ckcz123/DSP_Battle/master/previews/3.png)

![Previews](https://raw.githubusercontent.com/ckcz123/DSP_Battle/master/previews/4.png)

[![They Come From Void](https://raw.githubusercontent.com/ckcz123/DSP_Battle/master/previews/youtube.jpg)](https://youtu.be/PKGK_TdQsDE "They Come From Void")

或者可以在B站查看宣传视频： https://www.bilibili.com/video/BV1jR4y1F7t5/

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

Then download the mod manully and unzip into `plugins` (including the `dll` and `dspbattletex` file). If you can see the new logo, then the mod is installed successfully.

在上述地址安装框架和几个前置mod，然后将本mod解压到`plugins`目录（包括`dll`和`dspbattletex`文件）。如果开始游戏后能看到新的logo，那么mod就安装成功了。

## How to play / 如何游戏

They come when your first interstellar logistics station is placed. From then on, stronger and stronger waves will assault you about every 60 minutes. They will never stop. They will never end.

They may target any planet with an interstellar logistics station. Though the first few waves will be in one system and easily manageable, assuming it will stay that way will lead to demise. Luckily, notifications and after-battle reports will fill you in on where you need to focus your rebuilding efforts. More information on ai targeting, notifications, and other mechanics are available in-game.

They are going to be a challenge to EXPERIENCED players who have extensive knowledge of the game and a library of finely tuned production blueprints. Though different difficulty levels exist, even easy is not recommended for new players or experienced players that want a casual run.

So far, we haven't encountered any issues installing this mod mid-run. Want time to develop your foothold and prepare before you watch your galaxy burn? Great!
Personal Opinion for users who want a casual start but a challenge late-game, I recommend installing the mod when you either make your first white science or your dyson sphere reaches 4GW.

Do not remove this mod once installed or you risk breaking your save.

## Feedback and suggestions / 意见和反馈

If you have any feedback or suggestions, please file a new issue via [github](https://github.com/ckcz123/DSP_Battle), or contact me in Discord `ckcz123#3576`.

Also welcome to anyone who'd like to contribute to the mod, better if you have experience of Unity games or modeling.

如果你有任何的意见或建议，可以通过[github](https://github.com/ckcz123/DSP_Battle)发起一个issue，或者加QQ群`694213906`找群主反馈。

欢迎任何向为此mod进行贡献的人，我们尤其需要有Unity经验或者会建模的大佬。

## Update Log

### 2022-03-18 V2.1.4

 - Add "Fast Start" option when creating the game. It will unlock all blue/red science and give a lot of buildings to help on early game.
 - 游戏开始时增加“快速开局”选项；它会直接解锁所有红蓝糖科技，并赠送大量建筑，助你快速读过无聊的前期。
 - Remove abnormality tips
 - 移除游戏异常的提示。

### 2022-12-30 V2.1.3

 - Optimize some English translation
 - 优化部分文本的英文翻译
 - Move Droplet to an individual technique
 - 将水滴拆分成一个独立的科技

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
