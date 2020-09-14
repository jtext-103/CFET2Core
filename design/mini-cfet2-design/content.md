# Mini Cfet2 Design

<!-- keywords:CFET2;key2; -->
<!-- description:Mini CFET2 主要是为了让用户，包括 app developer 和 end user 快速掌握 CFET2 做的控制系统中的一些关键的功能 -->

Mini CFET2 主要是为了让用户，包括 app developer 和 end user 快速掌握 CFET2 做的控制系统中的一些关键的功能:

## 整体布局

整体有几个功能组成：DAQ，Data Archive，Logic，Trigger 组成。

以上每个部署在 Mini CFET2 对应的电脑上，每个东西是一个 self contain 的文件夹，不包含源代码，包含所有的配置，里面有一个入口，运行起来就可以了。这些东西放在对应的文件夹下如：

```
~/deploy/daq/pulse-daq
~/deploy/daq/continuous-daq
~/deploy/archive
```

同时除了部署，还有开发，包含 CFET2 全套每个项目的 git 的库。房子对应的目录下：
~/develop/cfet-core
~/develop/cfet-ui
~/develop/cfet-baseAI

## 整体要求

**非常重要：**

**要求用户双击就可以运行，同时用户可以非常简单的配置如数据存储结构、位置；采集的通道，控制逻辑等等。**

开发方式：

1. 第一步先能用，先不关配置简单，先能用，功能也非常少
2. 然后一步一步的把功能加上
3. 几个相关的功能都加齐全了，就把这一套做的好配置还用
4. 最后全部都好用了

## DAQ

分为连续采集和脉冲采集

### pulse-daq

这个就我们现在用的采集，要求：

1. 用户可以方便的配置使用什么采集卡，几个采集卡，几个采集卡挂载在哪里，这个没有界面，但是用户可以通过简单的复制粘贴文件夹就实现，一个文件夹就是一个卡
2. 用户可以方便的更改配置，这个可以再文件夹里面改配置文件，同时需要在界面上直接改，并且可以保存到同样的配置文件中
3. 用户有界面，可以监控每个卡的状态，并且可以操作每个卡
4. 有一个 custom view 可以显示十有卡的状态，要是能够自动根据卡的多少显示就好了，可能需要做个聚合的 thing，前端估计没这个逻辑
5. 要非常稳定

### continuous-daq

就是连续的，类似那个降雨的

1. 需要写一个新的 cfet-plc 的 thing
2. 这个好简单，没啥难的

## Data-Archive

### pulse-archive

就是一炮一炮的

1. 这个就是要把王昱星以前那个 tag server 还啥的整好
2. 可能要做一个 widget 界面来课时话存储的结构，thing 也要改
3. 关键也是要稳定可靠

### continuous-archive

就是你一先写的 mongodb 存湿度的

1. 要把读取书觉得 api 做了，以前没有的

### scope

看数据的，这个是大头

1. 把以前的正好，功能和 jscope 一样
2. 时间轴支持绝对时间

## logic

### 状态机

没啥好说的

用一个 json 描述状态机，可以再前端直接改。

然后采集也可以用这个实现状态转换，代替 AI management thing

### 脚本

就是运行 cs 的脚本

### 完整的控制

利用上面的实现，按一个按钮，然后 daq 自己 arm，然后 trigger 触发，完成采集，最后回到空闲。

## trigger

这个最简单，就是江易星写的那个东西
