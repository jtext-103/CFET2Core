using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Sample;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace WebsocketEventThing
{
    /// <summary>
    /// use this to subscribe to remote event
    /// </summary>
    public class WebsocketEventClient
    {
        internal static WebsocketEventManager ParentThing;

        public Dictionary<Token, RemoteSubscription> RemoteSubscriptions { get;  } = new Dictionary<Token, RemoteSubscription>();

        public Dictionary<int, Token> websocketDict { get;  } = new Dictionary<int, Token>();

        /// <summary>
        /// this will subscribe to the remote event, it will keep trying at 500ms interval is failed.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task  SubscribeAync(EventFilter filter,Token token)
        {
            
        }

        private void subscribe(EventFilter filter, Token token)
        {
            //connect to webwocket
            var websocket = new WebSocket(filter.Host);
            var eventRequest = new EventRequest(filter, EventRequestAction.Subscribe);
            websocket.OnMessage += handleArrivedEvent;
            websocket.OnClose += reconnect;
            //send subscribe action
            while (websocket.ReadyState!=WebSocketState.Open)
            {
                websocket.ConnectAsync();
                Thread.Sleep(1000);
            }
            websocket.SendAsync(JsonConvert.SerializeObject(eventRequest),(result)=> { });
            //save the connection instence
            RemoteSubscriptions.Add(token,new RemoteSubscription { WebSocket= websocket, Filter=filter});
            websocketDict.Add(websocket.GetHashCode(), token);
        }

        private void reconnect(object sender, CloseEventArgs e)
        {
            var websocket = (WebSocket)sender;
            if (!websocketDict.ContainsKey(websocket.GetHashCode()))
            {
                return;
            }
            while (websocket.ReadyState != WebSocketState.Open)
            {
                websocket.ConnectAsync();
                Thread.Sleep(1000);
            }
            //resubscribe
            var eventRequest = new EventRequest(RemoteSubscriptions[websocketDict[websocket.GetHashCode()]].Filter, EventRequestAction.Subscribe);
            websocket.SendAsync(JsonConvert.SerializeObject(eventRequest), (result) => { });
        }

        private void handleArrivedEvent(object sender, MessageEventArgs e)
        {
            var token = websocketDict[sender.GetHashCode()];
            var eventFilter = RemoteSubscriptions[token].Filter;

            switch (eventFilter.PerformanceLevel)
            {
                case 0:
                    ParentThing.MyHub.EventHub.Publish(new EventArg(eventFilter.Source,eventFilter.EventType,e.Data.ToStatus(),eventFilter.Host));
                    break;
                default:
                    ParentThing.MyHub.EventHub.Publish(JsonConvert.DeserializeObject<EventArg>(e.Data));
                    break;
            }
        }

        /// <summary>
        /// unsubscribe the remote event, this is a fire&forget method, non-blocking
        /// </summary>
        /// <param name="token"></param>
        public void Unsubscribe(Token token)
        {
            //remove from websocket dic so when closing it will not try to reconnect
            if (websocketDict.ContainsKey(RemoteSubscriptions[token].WebSocket.GetHashCode()))
            {
                websocketDict.Remove(RemoteSubscriptions[token].WebSocket.GetHashCode());
            }

            

            //send unsubscribe
            if (!RemoteSubscriptions.ContainsKey(token))
            {
                return;
            }
            RemoteSubscriptions[token].WebSocket.SendAsync(JsonConvert.SerializeObject(new EventRequest(null, EventRequestAction.Unsubscribe)),(result)=> {
                //close connection
                RemoteSubscriptions[token].WebSocket.Close();
            });
            //remove form subscriptions dict
            RemoteSubscriptions.Remove(token);
        }

  

    }
}
