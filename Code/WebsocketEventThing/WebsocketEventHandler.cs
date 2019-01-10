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
    /// <summary>
    /// handle local event and send it to remote clients
    /// </summary>
    public class WebsocketEventHandler: WebSocketBehavior
    {

        private ICfet2Logger logger;

        /// <summary>
        /// the connections, the key is the session id
        /// </summary>
        public static Dictionary<string, WsSubscription> Subscription = new Dictionary<string, WsSubscription>();
        internal static WebsocketEventThing ParentThing;

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
                    subscribe(request);
                    break;
                case EventRequestAction.Unsubscribe:
                    unsubscribe(request);
                    break;
                default:
                    break;
            }
        }

        private void unsubscribe(EventRequest request)
        {
            if (!Subscription.ContainsKey(ID))
            {
                return;
            }
            //unsub the local event
            var token = Subscription[ID].Token;
            ParentThing.MyHub.EventHub.Unsubscribe(token);
            //close ws connectoion
            Subscription[ID].Session.Context.WebSocket.CloseAsync();
            //remove client info
            Subscription.Remove(ID);
        }

        private void subscribe(EventRequest eventRequest)
        {
            //extract the resource path
            string path;
            if (eventRequest.Source.IsNullOrEmpty())
            {
                path = Sessions[ID].Context.RequestUri.AbsolutePath;
            }
            else
            {
                path = eventRequest.Source;
            }
            //subscribe to loacl event with a func call to push to the remote subscriber
            var filter = new EventFilter(path,eventRequest.EventType);
            var token=ParentThing.MyHub.EventHub.Subscribe(filter,(e)=> { pushToRemoteSubscriber(ID, e); });
            //save the token for unsub
            var sub = new WsSubscription { Id = ID, Session = Sessions[ID], EventRequest = eventRequest, Token = token };
            Subscription[ID]= sub;
        }

        private void pushToRemoteSubscriber(string id, EventArg e)
        {
            try
            {
                //compose the message to send based on performance level
                string pushMsg = "";
                switch (Subscription[id].EventRequest.PerformanceLevel)
                {
                    case 1:
                        ///for max performance just push the object val
                        pushMsg = JsonConvert.SerializeObject(e.Sample.ObjectVal);
                        break;
                    //level 0 is default
                    default:
                        //for max infomation, pusht he whole event arg
                        var pushEvent = new EventArg( e.Source,e.EventType,e.Sample, ParentThing.Config.Host );
                        pushMsg = JsonConvert.SerializeObject(pushEvent);
                        break;
                }
                //send via web socket
                Subscription[id].Session.Context.WebSocket.SendAsync(pushMsg, (result) =>
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

        /// <summary>
        /// remove all the cloased connection, also removes the co-responding subscriptions
        /// </summary>
        public void PurgeClosedConnections()
        {
            throw new NotImplementedException("");
        }
    }
}
