using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.WebsocketEvent;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Event;

namespace WebSocketRTTTest
{
    public class EchoThing : Thing
    {
        private string host;

        private int channelStart;

        private int channelCount;

        private int subEventLevel;

        private int senderRecived;

        public EchoThing(int channelStart = 0, int channelCount = 1, int subEventLevel = 0)
        {
            this.subEventLevel = subEventLevel;
            this.channelStart = channelStart;
            this.channelCount = channelCount;
            senderRecived = 0;
        }

        public override void TryInit(object senderHost)
        {
            host = (string)senderHost;
        }

        public override void Start()
        {
            for(int i = channelStart; i < channelCount; i++)
            {
                MyHub.EventHub.Subscribe(new EventFilter(@"/sender/idtest/" + i.ToString(), EventFilter.DefaultEventType, subEventLevel, host), eventHandler);
            }
        }

        private void eventHandler(EventArg e)
        {
            senderRecived++;      
            string channel = e.Source.Substring(e.Source.LastIndexOf("/") + 1, e.Source.Length - e.Source.LastIndexOf("/") - 1);
            //Console.WriteLine("GotSender:" + senderRecived +"\tChannel:" + channel);
            MyHub.EventHub.Publish(Path + "/callback", EventFilter.DefaultEventType, e.Sample.GetVal<Guid>());
        }

        [Cfet2Method]
        public void Reset()
        {
            senderRecived = 0;
        }
    }
}
