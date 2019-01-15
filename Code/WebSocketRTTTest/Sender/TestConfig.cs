using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketRTTTest
{
    /// <summary>
    /// RTT测试配置
    /// 发布事件的路径包括了Channel，发布事件的内容包括了Payload
    /// </summary>
    public class TestConfig
    {
        /// <summary>
        /// 测试线程数
        /// </summary>
        public int TaskCount { get; set; }

        /// <summary>
        /// 每个线程的通道数
        /// </summary>
        public int ChannelPerTask { get; set; }

        /// <summary>
        /// 每个通道需要发布的事件数量
        /// </summary>
        public int EventPerChannel { get; set; }

        /// <summary>
        /// 订阅echo端的事件级别
        /// </summary>
        public int SubEventLevel { get; set; }

        /// <summary>
        /// 每个Event发布后等待的毫秒数
        /// </summary>
        public int MsRandomBetweenEvent { get; set; }

        public TestConfig(int taskCount, int channelPerTask, int eventPerChannel, int msRandomBetweenEvent, int subEventLevel)
        {
            TaskCount = taskCount;
            ChannelPerTask = channelPerTask;
            EventPerChannel = eventPerChannel;
            MsRandomBetweenEvent = msRandomBetweenEvent;
            SubEventLevel = subEventLevel;
        }
    }
}
