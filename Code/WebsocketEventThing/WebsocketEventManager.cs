using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Log;
using System;

namespace WebsocketEventThing
{
    public class WebsocketEventManager:Thing
    {
        public WebsocketEventConfig Config { get; set; }

        private ICfet2Logger logger;
        private EventHub myEventHub;

        public WebsocketEventManager()
        {
            
        }

        public override void TryInit(object initObj)
        {
            Config = (WebsocketEventConfig)initObj;
            logger = Cfet2LogManager.GetLogger("WsEvent@"+ Path);
        }

        public override void Start()
        {
            
        }

    }
}
