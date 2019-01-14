using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Jtext103.CFET2.WebsocketEvent
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
                Debug.WriteLine("recived a remote subscription");
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

        /// <summary>
        /// unsubscribe all localevent this is used at disposing the server
        /// </summary>
        internal static void UnsubAllLocal()
        {
            lock (Subscription)
            {
                foreach (var request in Subscription)
                {
                    request.Value.Token.Dispose();
                    //ParentThing.MyHub.EventHub.Unsubscribe(request.Value.Token);
                }
            }
        }

        private void unsubscribe(EventRequest request)
        {
            lock (Subscription)
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
        }

        private void subscribe(EventRequest eventRequest)
        {
            lock (Subscription)
            {
                EventFilter eventFilter;
                //extract the resource path
                string path;
                if (eventRequest.SourcesAndTypes == null || eventRequest.SourcesAndTypes.Count == 0)
                {
                    path = Sessions[ID].Context.RequestUri.AbsolutePath;
                    eventFilter = new EventFilter(path, EventFilter.DefaultEventType);
                }
                else
                {
                    eventFilter = new EventFilter(eventRequest);
                    //transform the remote to local event
                    eventFilter.Host = "";
                }
                //subscribe to loacl event with a func call to push to the remote subscriber
                var token = ParentThing.MyHub.EventHub.Subscribe(eventFilter, (e) => { pushToRemoteSubscriber(ID, e); });
                //save the token for unsub
                var sub = new WsSubscription { Id = ID, Session = Sessions[ID], EventRequest = eventRequest, Token = token };
                Subscription[ID] = sub;
            }   
        }

        private void pushToRemoteSubscriber(string id, EventArg e)
        {
            try
            {
                lock (Subscription)
                {
                    //compose the message to send based on performance level
                    string pushMsg = "";
                    switch (Subscription[id].EventRequest.PerformanceLevel)
                    {
                        case 1:
                            ///for max performance just push the object val
                            pushMsg = JsonConvert.SerializeObject(new RemoteEventArgLevel1(e));
                            break;
                        //level 0 is default
                        default:
                            //for max infomation, pusht he whole event arg
                            var pushEvent = new EventArg(e.Source, e.EventType, e.Sample, ParentThing.Config.Host);
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
