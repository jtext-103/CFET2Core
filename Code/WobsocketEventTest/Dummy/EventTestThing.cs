using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.WebsocketEvent.Test.Dummy
{
    public class EventTestThing:Thing
    {
        /// <summary>
        /// no use
        /// </summary>
        [Cfet2Status]
        public Guid IdTest { get; set; }

        [Cfet2Method]
        public Guid FireOne(int channel)
        {
            var id = Guid.NewGuid();
            MyHub.EventHub.Publish(Path+"/IdTest/"+channel.ToString(),EventFilter.DefaultEventType,id);
            return id;
        }

        [Cfet2Method]
        public Payload FireHeavy(int channel,int size)
        {
            var payload = new Payload(size,100);
            MyHub.EventHub.Publish(Path + "/IdTestHeavy/" + channel.ToString(), EventFilter.DefaultEventType, payload);
            return payload;
        }

        [Cfet2Method]
        public Payload FireHeavyRandom(int channel, int size)
        {
            var payload = new Payload(size);
            MyHub.EventHub.Publish(Path + "/IdTestHeavy/" + channel.ToString(), EventFilter.DefaultEventType, payload);
            return payload;
        }
    }
}
