using Jtext103.CFET2.CFET2App.ExampleThings;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.NancyHttpCommunicationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.WebsocketEvent;
using WebSocketRTTTest;

namespace Jtext103.CFET2.CFET2App
{
    partial  class Cfet2Program : CFET2Host
    {
        private void AddThings()
        {
            //nancy HTTP
            var nancyCM = new NancyCommunicationModule(new Uri("http://localhost:8000"));
            MyHub.TryAddCommunicationModule(nancyCM);

            //example thing
            var pc = new PcMonitorThing();
            MyHub.TryAddThing(pc, "/", "pc");

            //websocket
            WebsocketEventThing remoteHub = new WebsocketEventThing();
            MyHub.TryAddThing(remoteHub, @"/", "WsEvent", new WebsocketEventConfig { Host = "ws://192.168.0.238:12036" });
            //MyHub.TryAddThing(remoteHub, @"/", "WsEvent", new WebsocketEventConfig { Host = "ws://127.0.0.1:8081" });
            MyHub.EventHub.RemoteEventHubs.Add(remoteHub);

            //RTTTestSender
            var sender = new SenderThing(1, 1000, 5, 0);
            MyHub.TryAddThing(sender, "/", "sender", new string[] { "ws://192.168.0.116:12036" });
            //MyHub.TryAddThing(sender, "/", "sender", new string[] { "ws://127.0.0.1:8081" });

            //RTTTestEcho
            //var echo = new EchoThing(0, 1, 0);
            ////MyHub.TryAddThing(echo, "/", "echo", "ws://192.168.0.221:12036");
            //MyHub.TryAddThing(echo, "/", "echo", "ws://127.0.0.1:8081");


        }
    }
}
