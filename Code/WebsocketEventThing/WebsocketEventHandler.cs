using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebsocketEventThing
{
    public class WebsocketEventHandler: WebSocketBehavior
    {
        public WebsocketEventHandler(WebsocketEventManager parentThing)
        {
            myParentThing = parentThing;
        }

        private ICfet2Logger logger;

        /// <summary>
        /// the connections, the key is the session id
        /// </summary>
        public static Dictionary<string, WsSubscription> Subscription = new Dictionary<string, WsSubscription>();
        private WebsocketEventManager myParentThing;

        public WebsocketEventHandler()
        {
            logger = Cfet2LogManager.GetLogger("WsEventHandler");
        }

        /// <summary>
        /// handle subscribe and unsub requests
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMessage(MessageEventArgs e)
        {
            EventRequest request;
            try
            {
                request = JsonConvert.DeserializeObject<EventRequest>(e.Data);
            }
            catch (Exception)
            {
                logger.Error("bad ws event request! : cannot deserialze request!");
                return;
            }

            switch (request.Action)
            {
                case EventRequestAction.Subscribe:

                    break;
                case EventRequestAction.Unsubscribe:
                    //unsub the local event
                    //close ws connectoion
                    //remove client info
                    break;
                default:
                    break;
            }
        }

        private void subscribe(EventRequest eventRequest)
        {
            //extract the resource path
            string path;
            if (eventRequest.ResourcePath.IsNullOrEmpty())
            {
                path = Sessions[ID].Context.RequestUri.AbsolutePath;
            }
            else
            {
                path = eventRequest.ResourcePath;
            }
            //subscribe to loacl event with a func call to push to the remote subscriber
            var filter = new EventFilter(path,eventRequest.EventType);
            var token=myParentThing.MyHub.EventHub.Subscribe(filter,(e)=> { pushToRemoteSubscriber(ID, e); });
            //save the token for unsub
            var sub = new WsSubscription { Id = ID, Session = Sessions[ID], EventRequest = eventRequest, Token = token };
            Subscription.Add(ID, sub);
        }

        private void pushToRemoteSubscriber(string id, EventArg e)
        {
            try
            {
                Subscription[id].Session.Context.WebSocket.SendAsync(JsonConvert.SerializeObject(e), (result) =>
                {
                    if (!result)
                    {
                        logger.Error("push failed!  conection error");
                    }
                });
            }
            catch (Exception)
            {
                logger.Error("push failed!");
            }
            
        }
    }
}
