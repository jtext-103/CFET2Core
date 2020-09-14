# CFET2DeviceProtocol_stringImplementation

基本的 CFET2DeviceProtocol 请参考[CFET Device 协议 1.0](./../CFET%20Device协议%201。1.docx)。

这个主要是将一个具体的 CFET2DeviceProtocol 实现，这里说的如果不清楚的请参考上面的文档，

这个协议为了保证兼容新，给它起一个具体的版本交错 CDP.SI.1

原来写的主要是针对 MQTT 的，但是这里做一个定义可以吧原来写的编程针对全部的。

**MQTT 中的 Topic 就是 CDP.S.1 中的地址**

## 实现方法

因为这个可能会使用多种传输方式实现，所以分为两层：

1. 消息层：用于处理协议的消息
   1. 消息层只管消息的内容和逻辑，不管发送
   2. 消息层要管地址，因为可能有多个消息层共用一个网络层，消息层会把发送的地址给网络层，网络也会把收到的地址给消息曾
2. 网络层：用于首发消息
   1. 负责发送和接受
   2. 负责首发的 QoS，比如消息是否需要 ACK，自动重发等等。

## 地址

对于 CDP.SI.1 来说，地址就相当于 MQTT 中的 Topic，有两个重要的地址，一个是**HOST 地址{HOST_ADDR}**，一个**自己的地址(DEVICE_ADDR)**。

下文{}中的试变量。

自己的地址在初始化的时候给网络层，HOST 地址在完成注册（有可能试初始话的时候完成注册，也有可能是通过 HOST 发现完成注册）。

协议里面有很多 TOPIC 实际上只要是发给 HOST 的消息就用 HOST 的地址，只要是发给自己的就用自己的地址。 例如：

一下都是传感器的地址：

- {传感器订阅的 topic——确认注册 topic}
- {发布的 Topic}
- {CFET Host 心跳连接 Topic+传感器心跳 Topic}
- {查询 Topic}

以下试 HOST 的地址：

- {CFET 的 host 订阅的 topic——注册 topic}

## 具体协议实现

由于历史原因一下传感器=Device

我再原有文档上做了一些修改协议的具体格式如下：

1. 发现

- 方向：Host -> 传感器
- 格式：DISC;{HOST_ADDR}

2. 注册

- 方向：传感器 -> HOST
- 格式：REG:{DEVICE_ADDR};{DEVICE_NAME};{PING_TIME};{DEVICE_DESC_JSON}
- {DEVICE_DESC_JSON}参考[CFET Device 协议 1.0](./../CFET%20Device协议%201。1.docx)。
- {PING_TIME}，这个是心跳发送的周期

3. 注册确认

- 方向：Host -> 传感器
- 格式：REGCONFIRM;{HOST_ADDR}

4. 发布

- 方向：传感器 -> HOST
- 格式：PUB;{UPDATE_MSG_JSON}
- {UPDATE_MSG_JSON}，这个是试更新的消息，JSON 形式，但是极其简单：

```json
{{ 1: value1 }, { 2: value2 }}
```

就是一个`Dictionary<int,float>`int 试通道编号，Float 试通道值

5. 查询

- 方向：Host -> 传感器
- 查询分为读写:
- 读：GET;{READ_CHANNEL_LIST_JSON}
- 这个{READ_CHANNEL_LIST_JSON}是一个 JSON list，里面是要读取的通道号的列表：
  ```json
  [1, 5, 6, 9]
  ```
- 写：SET;{UPDATE_MSG_JSON},这个和发布里面格式一样
- 写的回复是：PUB;{UPDATE_MSG_JSON},这个要注意的是这里回复的，通道号是写入的通道号，但是值，如果成功是 0，失败位非零，并不代表最新的值。

6. 心跳

- 方向：先 传感器->HOST 再 Host -> 传感器
- 格式：PING;
- 心跳功能说明: 传感器再{PING_TIME}时间内一定会向主机发送消息，只要发送，任意一条消息{PING_TIME}计时器会重置。如果{PING_TIME}内没有发送消息，那就发一条心跳消息。并且再{PING_TIME}内需要收到 HOST 回复的心跳消息。如果传感器发送了任何一条消息，再 M*{PING_TIME}内没有收到 HOST 的回复，则自动反注册 HOST，发送反注册消息。如果 HOST，再 N*{PING_TIME}内没有收到传感器来的任何消息，则把传感器置为离线，不在查询这个传感器，知道该传感器再次发送消息，或重新注册或者发现。

7. 反注册

- 方向：传感器 -> HOST
- 格式：UNREG;{DEVICE_ADDR}

## 发现与注册行为

### 发现

HOST 可以通过广播发现，也可以对某个地址单独发送发现。

比如有冲突的网络，不能使用广播发现，因为大家一起回复就冲突了。可以单独的给每个设备发送一次发现让他注册。

或着，当一个设备离线了，可以单独给他发送发现，让他重新注册。

### 注册

注册也有两种。当收到发现消息后，发现消息里面有 HOST 地址，Device 可以向 HOST 注册。或着我提前告诉 Device 有某个 HOST，在没有发现消息的时候也可以主动去那个 HOST 注册。

但是要注意一定是收到注册确认才是注册成功了。

HOST 收到注册消息，如果没啥问题都会回复注册确认，并且在本地注册，不需要和发现挂上钩。

### 反注册

Device 发送反注册以后就反注册成功了，不需要等待 HOST 确认，HOST 收到反注册以后就把 Device 反注册掉，不需要回复。
