# CFET2计划2020

## 现状和趋势

参加完ICALEPCS2019以后，发现加速器控制系统主要有以下几个现状：  
1. 目前大家还是以EPICS为主，大家对EPICS接受程度很高，觉得EPICS没毛病，很好用
2. 还有很多新装置计划采用EPICS，有很多老装置又Migrate to EPICS的计划
3. 很多新的开发都是围绕EPICS来的，比如新的CAC， CAS，EPICS的web相关工具，EPICS实验应用等等。  
4. EPICS主要的开发还是集中在EPICS3上面，开发EPICS 7的很少但是大家对EPICS7的评价远高于EPICS3

但是也有以下几个趋势：  
1. 最明显的是Web，但是仅限于Web前端技术，全是做EPICS各种界面的
2. 然后是Docker，各种Docker漫天飞（虽然我们很久很久以前就发过Docker的文章，我门真是走在世界前列）
3. 然后UX是开发重点，个中开发目前都是围绕提升UX来做的，但是大部分是面向前端的，UX在他们看来主要是前端
4. 基本上没有人挑战EPICS CA的地位  

## 讨论与思考

我和EPICS CSS的主要开发者Kay交流了一下，主要是和他讨论了一下EPICS为什么会做出目前这样的Design Decision和EPICS未来的发展趋势。  

EPICS为什么会有几今天天这个设计，design decision完全是EPICS团队自己根据手边的需求制定的。再没有人制定标准的年代有人制定标准就是唯一的标准。EPICS中有很多improvised设计，没有采用任何成熟的标准。他给出的解释是那些标准都不合适。我觉得确实不是perfect fit, but you always can build your ideas around it. 而EPICS给我的感觉是有点quick and dirty的解决方案。不过anyway it get thing done。他解决了他们领域内的问题比较完美的，再加上宣传和推广很快就成为了行业标准。

随着新技术的发展，加速器上采用的设备、控制器等等越来越丰富，EPICS3面临着很多不灵活的故有问题，于是EPICS核心团队开发了EPICS7，主要是采用了新的PV Access协议来代替CA协议。这个协议更加的灵活支持很多数据类型、用户自定义字段。但是它最大的问题是依然没有采用什么开放的标准，一切都是自己做的。发明了不少轮子，尤其是在序列化标准上，完全自己造了一个非常复杂的轮子。我觉得PV Access对于CA是有不少改经，但不是ground breaking的，未来依然会面领着obsolescence的问题。而且，不再用开放标准的问题是会影响community为他开发更多更好的东西。这也是为什么EPICS7没有quickly gain popularity的原因。EPICS3也是靠着30年缓慢积累的popularity。

## 计划

但是，EPICS依然是Accelerator control界holy grail，CFET短期内是不能能和EPICS相提并论的。我们的计划是向EPICS学习，学习他为什么会有这样的popularity，用户到底需要什么，未来到底需要什么，然后慢慢的在以后成为第二个EPICS，现在我们要掌握EPICS，找到和EPICS结合的地方：

### 具体的　　
1. 首先要学习EPICS，主要是EPICS IOC和CSS掌握他们解决的问题和解决问题的方法
2. 而实要应用EPICS，主要是EPICS IOC和CSS，主要是用用现成的不需要开发record、device support的那种，以及开发CSS界面，比如使用ITER CODAC Core开发数据采集、PLC应用等。
3. 然后是要CFET和EPICS的结合，这里CFET永远不要做CAS，不要host PV，CFET和EPICS结合就是通过CAC。所有的PV采用EPICS原版的Soft IOC。
4. 紧接上面的，要研究Docker技术，怎么用容器来跑IOC，别人都搞过，这个简单，要可以很快地就配置跑起来一个IOC
5. 同理CFET也要容器化
6. 然后CFET要马上吧Core freeze了，不要再增加新的feature了，吧现在要的几个feature做完就行
7. CFET的开发重点要转移到Web前端，要做很多Widget，要仿造CSS，要做到和CSS一个级别

### 目标
2年后的ICALEPCS要能够有一个牛逼的利用CFET的EPICSWeb前端，一定要是最好的Web前端。


## Ref
1. http://icalepcs2019.vrws.de/index.html
2. https://github.com/wduckitt/React-Automation-Studio
3. https://github.com/JeffersonLab/puddysticks