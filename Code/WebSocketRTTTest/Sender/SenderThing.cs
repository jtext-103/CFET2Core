using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int channelCount;
        private int eventPerChannel;
        private int msBetweenEvent;
        private int eventLevel;

        private string[] host;

        private int callbackRecived;

        private List<Stopwatch> stopwatches;

        private List<long> delay;


        public SenderThing(int channelCount = 1, int eventPerChannel = 100, int msBetweenEvent = 50, int eventLevel = 0)
        {
            this.channelCount = channelCount;
            this.eventPerChannel = eventPerChannel;
            this.msBetweenEvent = msBetweenEvent;
            this.eventLevel = eventLevel;  
        }

        public override void TryInit(object echoHost)
        {
            host = (string[])echoHost;
        }

        public override void Start()
        {
            foreach (var s in host)
            {
                MyHub.EventHub.Subscribe(new EventFilter(@"/echo/callback", EventFilter.DefaultEventType, eventLevel, s), eventHandler);
            }
            //MyHub.EventHub.Subscribe(new EventFilter(@"/echo/callback", EventFilter.DefaultEventType), eventHandler);
        }

        [Cfet2Method]
        public void Begin()
        {
            callbackRecived = 0;
            stopwatches = new List<Stopwatch>();
            for(int i = 0; i < channelCount * eventPerChannel; i++)
            {
                stopwatches.Add(new Stopwatch());
            }
            for (int i = 0; i < channelCount; i++)
            {
                for (int j = 0; j < eventPerChannel; j++)
                {
                    FireOne(i, i * eventPerChannel + j);
                    stopwatches[i * eventPerChannel + j].Start();
                    Thread.Sleep(msBetweenEvent);
                }
            }
        }

        [Cfet2Method]
        public void Write()
        {
            delay = new List<long>();
            for(int i = 0; i < callbackRecived; i++)
            {
                delay.Add(new long());
                delay[i] = (long)(stopwatches[i].ElapsedTicks / 3.914);
                Console.WriteLine("Id:" + i + "\tDelay(us):" + delay[i]);
            }
            SaveFile();
        }

        private void eventHandler(EventArg e)
        { 
            stopwatches[callbackRecived].Stop();
            callbackRecived++;
            //Console.WriteLine("GotCallback:" + callbackRecived);
        }

        private void FireOne(int channel, int id)
        {
            MyHub.EventHub.Publish(Path + "/idtest/" + channel.ToString(), EventFilter.DefaultEventType, id);
        }

        private void SaveFile()
        {
            FileStream fs = new FileStream(System.IO.Directory.GetCurrentDirectory() + "result.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string line;
            for(int i = 0; i < channelCount * eventPerChannel; i++)
            {
                line = i + "," + delay[i].ToString();
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
