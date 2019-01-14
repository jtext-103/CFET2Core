using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Log;
using System;

namespace Jtext103.CFET2.WebsocketEvent
{
    public class WebsocketEventThing : Thing, IRemoteEventHub
    {


        public WebsocketEventConfig Config { get; set; }

        /// <summary>
        /// the uri shceme
        /// </summary>
        public string Protocol => "ws";

        private ICfet2Logger logger;
        private WebsocketEventServer wsServer;
        private WebsocketEventClient wsClient;

        public WebsocketEventThing()
        {
            
        }

        public override void TryInit(object initObj)
        {
            Config = (WebsocketEventConfig)initObj;
            logger = Cfet2LogManager.GetLogger("WsEvent@"+ Path);
            WebsocketEventHandler.ParentThing = this;
        }

        public override void Start()
        {
            wsServer = new WebsocketEventServer(Config.Host);
            wsClient = new WebsocketEventClient();
            //todo: loop thrpugh a;; resource and create end points
            var resources = MyHub.GetAllLocalResources();
            wsServer.AddEndPoint("/");
            foreach (var resource in resources)
            {
                wsServer.AddEndPoint(resource.Key);
            }
            wsServer.StartServer();
        }

        public void Subscribe(Token token, EventFilter filter, Action<EventArg> handler)
        {
            wsClient.SubscribeAync(filter, token, handler);
        }

        public void Unsbscribe(Token token)
        {
            wsClient.Unsubscribe(token);
        }

        public void Dispose()
        {
            wsClient.Dispose();
            wsServer.Dispose();
        }
    }
}
