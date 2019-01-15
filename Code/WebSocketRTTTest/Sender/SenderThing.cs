using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Event;

namespace WebSocketRTTTest
{
    public class SenderThing : Thing
    {
        private TestConfig myTestConfig;

        private string host;

        private int callbackRecived;

        private object dicLock = new object();

        private Dictionary<Guid, long> eventBeginTime;

        private Dictionary<Guid, long> eventEndTime;

        private Dictionary<Guid, double> eventDelay;


        public SenderThing(int taskCount = 1, int channelPerTask = 10, int eventPerChannel = 10, int msRandomBetweenEvent = 50, int subEventLevel = 0)
        {
            myTestConfig = new TestConfig(taskCount, channelPerTask, eventPerChannel, msRandomBetweenEvent, subEventLevel);
        }

        public override void TryInit(object echoHost)
        {
            host = (string)echoHost;
        }

        public override void Start()
        {
            MyHub.EventHub.Subscribe(new EventFilter(@"/echo/callback", EventFilter.DefaultEventType, myTestConfig.SubEventLevel, host), eventHandler);
        }

        private void eventHandler(EventArg e)
        {
            callbackRecived++;
            //Console.WriteLine("GotCallback:" + callbackRecived);
            Guid guid = e.Sample.GetVal<Guid>();
            lock (dicLock)
            {
                eventEndTime.Add(guid, DateTime.Now.Ticks);
            }
        }

        [Cfet2Method]
        public void Begin()
        {
            eventBeginTime = new Dictionary<Guid, long>();
            eventEndTime = new Dictionary<Guid, long>();
            eventDelay = new Dictionary<Guid, double>();
            callbackRecived = 0;
            for (int i = 0; i < myTestConfig.TaskCount; i++)
            {
                int j = i;
                Task.Run(() => FireTask(j));
            }
        }

        private void FireTask(int task)
        {
            Guid guid;
            for (int i = 0; i < myTestConfig.ChannelPerTask; i++)
            {
                for (int j = 0; j < myTestConfig.EventPerChannel; j++)
                {
                    guid = FireOne(task * myTestConfig.ChannelPerTask + i);
                    lock (dicLock)
                    {
                        eventBeginTime.Add(guid, DateTime.Now.Ticks);
                    }
                    var random = new Random();
                    double around = 0.5 + random.Next(0, 1000) / 1000.0;
                    Thread.Sleep((int)(myTestConfig.MsRandomBetweenEvent * around));
                }
            }
        }

        private Guid FireOne(int channel)
        {
            var id = Guid.NewGuid();
            MyHub.EventHub.Publish(Path + "/idtest/" + channel.ToString(), EventFilter.DefaultEventType, id);
            return id;
        }

        /// <summary>
        /// 将当前eventDelay中的数据写入文件
        /// </summary>
        [Cfet2Method]
        public void Write()
        {
            lock (dicLock)
            {
                int i = 0;
                foreach (var s in eventBeginTime)
                {
                    eventDelay.Add(s.Key, Math.Round((eventEndTime[s.Key] - s.Value) / 10000.0, 2));
                    Console.WriteLine("No:" + i++ + "\tGuid:" + s.Key + "\tDelay(ms):" + eventDelay[s.Key]);        
                }
            }
            SaveFile();
        }

        private void SaveFile()
        {
            FileStream fs = new FileStream(System.IO.Directory.GetCurrentDirectory() + "result.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string line;
            foreach(var s in eventDelay)
            {
                line = s.Key.ToString() + "," + s.Value.ToString();
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
