using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Log;
using System;

namespace WebsocketEventThing
{
    public class WebsocketEventThing:Thing
    {
        public WebsocketEventConfig Config { get; set; }

        private ICfet2Logger logger;
        private EventHub myEventHub;
        private WebsocketEventServer wsServer;

        public WebsocketEventThing()
        {
            
        }

        public override void TryInit(object initObj)
        {
            Config = (WebsocketEventConfig)initObj;
            logger = Cfet2LogManager.GetLogger("WsEvent@"+ Path);
        }

        public override void Start()
        {
            wsServer = new WebsocketEventServer(Config.Host);
            //todo: loop thrpugh a;; resource and create end points
            var resources = MyHub.GetAllLocalResources();
            foreach (var resource in resources)
            {
                wsServer.AddEndPoint(resource.Key);
            }
            wsServer.StartServer();
        }

    }
}
