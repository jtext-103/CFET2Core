using Jtext103.CFET2.CFET2App.ExampleThings;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.NancyHttpCommunicationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewCopy;
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

            //拷贝视图文件夹
            var myViewsCopyer = new ViewCopyer();
            myViewsCopyer.StartCopy();
            var myContentCopyer = new ViewCopyer(null, "Content");
            myContentCopyer.StartCopy();

            //example thing
            //var pc = new PcMonitorThing();
            //MyHub.TryAddThing(pc, "/", "pc");

            //websocket
            WebsocketEventThing remoteHub = new WebsocketEventThing();
            MyHub.TryAddThing(remoteHub, @"/", "WsEvent", new WebsocketEventConfig { Host = "ws://127.0.0.1:8081" });
            //MyHub.TryAddThing(remoteHub, @"/", "WsEvent", new WebsocketEventConfig { Host = "ws://127.0.0.1:8082" });
            MyHub.EventHub.RemoteEventHubs.Add(remoteHub);

            //RTTTestSender
            var sender = new SenderThing(1, 100, 20, 50, 0);
            MyHub.TryAddThing(sender, "/", "sender", "ws://127.0.0.1:8081");

            //RTTTestEcho
            var echo = new EchoThing(0, 100, 0);
            MyHub.TryAddThing(echo, "/", "echo", "ws://127.0.0.1:8081");


        }
    }
}
