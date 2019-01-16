using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.WebsocketEvent;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Event;
using System.Threading;

namespace WebSocketRTTTest
{
    public class EchoThing : Thing
    {
        private string host;

        private int senderRecived;

        private int channelStart;

        private int channelCount;

        private int eventLevel;

        private int payloadKind;

        public EchoThing(int channelStart = 0, int channelCount = 1, int eventLevel = 0, int payloadKind = 0)
        {
            this.eventLevel = eventLevel;
            this.channelStart = channelStart;
            this.channelCount = channelCount;
            this.payloadKind = payloadKind;
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
                MyHub.EventHub.Subscribe(new EventFilter(@"/sender/idtest/" + i.ToString(), EventFilter.DefaultEventType, eventLevel, host), eventHandler);
                //MyHub.EventHub.Subscribe(new EventFilter(@"/sender/idtest/" + i.ToString(), EventFilter.DefaultEventType), eventHandler);
            }
        }

        private void eventHandler(EventArg e)
        {
            senderRecived++;      
            string channel = e.Source.Substring(e.Source.LastIndexOf("/") + 1, e.Source.Length - e.Source.LastIndexOf("/") - 1);
            //Console.WriteLine("GotSender:" + senderRecived +"\tChannel:" + channel);
            if(payloadKind == 0)
            {
                MyHub.EventHub.Publish(Path + "/callback", EventFilter.DefaultEventType, e.Sample.GetVal<Guid>());
            }
            else if(payloadKind == 1)
            {
                MyHub.EventHub.Publish(Path + "/callback", EventFilter.DefaultEventType, e.Sample.GetVal<int>());
            }
        }

        [Cfet2Method]
        public void Reset()
        {
            senderRecived = 0;
        }
    }
}
