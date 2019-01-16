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

        private string[] host;

        private int callbackRecived;

        private object dicLock = new object();

        private Dictionary<Guid, long> eventBeginTimeGuid;
        private Dictionary<int, long> eventBeginTimeInt;

        private Dictionary<Guid, long> eventEndTimeGuid;
        private Dictionary<int, long> eventEndTimeInt;

        private Dictionary<Guid, double> eventDelayGuid;
        private Dictionary<int, double> eventDelayInt;

        private int payloadKind;


        public SenderThing(int taskCount = 1, int channelPerTask = 10, int eventPerChannel = 10, int msRandomBetweenEvent = 50, int eventLevel = 0, int payloadKind = 0)
        {
            myTestConfig = new TestConfig(taskCount, channelPerTask, eventPerChannel, msRandomBetweenEvent, eventLevel);
            this.payloadKind = payloadKind;
        }

        public override void TryInit(object echoHost)
        {
            host = (string[])echoHost;
        }

        public override void Start()
        {
            foreach (var s in host)
            {
                if (payloadKind == 0)
                {
                    MyHub.EventHub.Subscribe(new EventFilter(@"/echo/callback", EventFilter.DefaultEventType, myTestConfig.EventLevel, s), eventHandlerGuid);
                }
                else if (payloadKind == 1)
                {
                    MyHub.EventHub.Subscribe(new EventFilter(@"/echo/callback", EventFilter.DefaultEventType, myTestConfig.EventLevel, s), eventHandlerInt);
                }
            }
            ////MyHub.EventHub.Subscribe(new EventFilter(@"/echo/callback", EventFilter.DefaultEventType), eventHandlerGuid);
            //MyHub.EventHub.Subscribe(new EventFilter(@"/echo/callback", EventFilter.DefaultEventType), eventHandlerInt);
        }

        [Cfet2Method]
        public void Begin()
        {
            if (payloadKind == 0)
            {
                BeginGuid();
            }
            else if (payloadKind == 1)
            {
                BeginInt();
            }
        }

        [Cfet2Method]
        public void Write()
        {
            if (payloadKind == 0)
            {
                WriteGuid();
            }
            else if (payloadKind == 1)
            {
                WriteInt();
            }
        }

        private void eventHandlerGuid(EventArg e)
        {
            callbackRecived++;
            //Console.WriteLine("GotCallback:" + callbackRecived);
            Guid guid = e.Sample.GetVal<Guid>();
            lock (dicLock)
            {
                eventEndTimeGuid.Add(guid, DateTime.Now.Ticks);
            }
        }

        private void eventHandlerInt(EventArg e)
        {
            callbackRecived++;
            //Console.WriteLine("GotCallback:" + callbackRecived);
            int intId = e.Sample.GetVal<int>();
            lock (dicLock)
            {
                eventEndTimeInt.Add(intId, DateTime.Now.Ticks);
            }
        }

        private void BeginGuid()
        {
            eventBeginTimeGuid = new Dictionary<Guid, long>();
            eventEndTimeGuid = new Dictionary<Guid, long>();
            eventDelayGuid = new Dictionary<Guid, double>();
            callbackRecived = 0;
            for (int i = 0; i < myTestConfig.TaskCount; i++)
            {
                int j = i;
                Task.Run(() => FireTaskGuid(j));
            }
        }

        private void BeginInt()
        {
            eventBeginTimeInt = new Dictionary<int, long>();
            eventEndTimeInt = new Dictionary<int, long>();
            eventDelayInt = new Dictionary<int, double>();
            callbackRecived = 0;
            for (int i = 0; i < myTestConfig.TaskCount; i++)
            {
                int j = i;
                Task.Run(() => FireTaskInt(j));
            }
        }

        private void FireTaskGuid(int task)
        {
            Guid guid;
            for (int j = 0; j < myTestConfig.EventPerChannel; j++)
            {
                for (int i = 0; i < myTestConfig.ChannelPerTask; i++)
                {
                    guid = FireOneGuid(task * myTestConfig.ChannelPerTask + i);
                    lock (dicLock)
                    {
                        eventBeginTimeGuid.Add(guid, DateTime.Now.Ticks);
                    }
                    var random = new Random();
                    double around = 0.5 + random.Next(0, 1000) / 1000.0;
                    Thread.Sleep((int)(myTestConfig.MsRandomBetweenEvent * around));
                }
            }
        }

        private void FireTaskInt(int task)
        {
            int intId;
            int chNo;
            for (int j = 0; j < myTestConfig.EventPerChannel; j++)
            {
                for (int i = 0; i < myTestConfig.ChannelPerTask; i++)
                {
                    chNo = task * myTestConfig.ChannelPerTask + i;
                    intId = FireOneInt(chNo, (chNo + 10000) * 10000 + j);
                    lock (dicLock)
                    {
                        eventBeginTimeInt.Add(intId, DateTime.Now.Ticks);
                    }
                    var random = new Random();
                    double around = 0.5 + random.Next(0, 1000) / 1000.0;
                    Thread.Sleep((int)(myTestConfig.MsRandomBetweenEvent * around));
                }
            }
        }

        private Guid FireOneGuid(int channel)
        {
            var id = Guid.NewGuid();
            MyHub.EventHub.Publish(Path + "/idtest/" + channel.ToString(), EventFilter.DefaultEventType, id);
            return id;
        }

        private int FireOneInt(int channel, int i)
        {
            MyHub.EventHub.Publish(Path + "/idtest/" + channel.ToString(), EventFilter.DefaultEventType, i);
            return i;
        }

        private void WriteGuid()
        {
            lock (dicLock)
            {
                int i = 0;
                foreach (var s in eventBeginTimeGuid)
                {
                    eventDelayGuid.Add(s.Key, Math.Round((eventEndTimeGuid[s.Key] - s.Value) / 10.0, 0));
                    Console.WriteLine("No:" + i++ + "\tGuid:" + s.Key + "\tDelay(us):" + eventDelayGuid[s.Key]);

                    //eventDelayGuid.Add(s.Key, Math.Round((eventEndTimeGuid[s.Key] - s.Value) / 10000.0, 0));
                    //Console.WriteLine("No:" + i++ + "\tGuid:" + s.Key + "\tDelay(ms):" + eventDelayGuid[s.Key]);

                    //eventDelayGuid.Add(s.Key, (eventEndTimeGuid[s.Key] - s.Value) * 100);
                    //Console.WriteLine("No:" + i++ + "\tGuid:" + s.Key + "\tDelay(ns):" + eventDelayGuid[s.Key]);
                }
            }
            SaveFileGuid();
        }

        private void WriteInt()
        {
            lock (dicLock)
            {
                int i = 0;
                foreach (var s in eventBeginTimeInt)
                {
                    eventDelayInt.Add(s.Key, Math.Round((eventEndTimeInt[s.Key] - s.Value) / 10.0, 0));
                    Console.WriteLine("No:" + i++ + "\tintId:" + s.Key + "\tDelay(us):" + eventDelayInt[s.Key]);

                    //eventDelayInt.Add(s.Key, Math.Round((eventEndTimeInt[s.Key] - s.Value) / 10000.0, 0));
                    //Console.WriteLine("No:" + i++ + "\tintId:" + s.Key + "\tDelay(ms):" + eventDelayInt[s.Key]);

                    //eventDelayInt.Add(s.Key, (eventEndTimeInt[s.Key] - s.Value) * 100);
                    //Console.WriteLine("No:" + i++ + "\tintId:" + s.Key + "\tDelay(ns):" + eventDelayInt[s.Key]);
                }
            }
            SaveFileInt();
        }

        private void SaveFileGuid()
        {
            FileStream fs = new FileStream(System.IO.Directory.GetCurrentDirectory() + "result.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string line;
            foreach (var s in eventDelayGuid)
            {
                line = s.Key.ToString() + "," + s.Value.ToString();
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private void SaveFileInt()
        {
            FileStream fs = new FileStream(System.IO.Directory.GetCurrentDirectory() + "result.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string line;
            foreach (var s in eventDelayInt)
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
