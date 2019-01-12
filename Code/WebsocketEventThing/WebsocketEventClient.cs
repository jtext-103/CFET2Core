using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Sample;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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

        internal static WebsocketEventThing ParentThing;

        public Guid ClientId { get; } = Guid.NewGuid();



        /// <summary>
        /// the key is the host address,NOTE: only one connection for a remote host, muiltple subscriptions to the same host  share one connection
        /// </summary>
        public Dictionary<string, RemoteSubscription> RemoteSubscriptions  {get;  }= new Dictionary<string, RemoteSubscription>();

        /// <summary>
        /// the key is hash to a web socket, the value is the host address
        /// </summary>
        public Dictionary<int, string> WebsocketDict { get;  } = new Dictionary<int, string>();

        /// <summary>
        /// this will subscribe to the remote event, it will keep trying at 500ms interval is failed.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task  SubscribeAync(EventFilter filter,Token token)
        {
            await Task.Run(() => {
                subscribe(filter, token.Id);
            });
        }

        private void subscribe(EventFilter filter, Guid tokenId)
        {
            //connect to webwocket
            //check if a connection to a host alraedy exsits
            if (!(RemoteSubscriptions.ContainsKey(filter.Host)))
            {
                //if not make a connection
                var websocket = new WebSocket(filter.Host);
                websocket.OnMessage += handleArrivedEvent;
                websocket.OnClose += reconnect;
                RemoteSubscriptions.Add(filter.Host,new RemoteSubscription { Host=filter.Host, WebSocket=websocket,ClientId=ClientId});
                WebsocketDict.Add(websocket.GetHashCode(), filter.Host);
            }
            //save it to remote subscriptions
            RemoteSubscriptions[filter.Host].AddNewSubscription(tokenId, filter);

            //do not block
            RemoteSubscriptions[filter.Host].SendSubscriptionAsync();   
        }

 

        private void reconnect(object sender, CloseEventArgs e)
        {
            var websocket = (WebSocket)sender;
            if (WebsocketDict.ContainsKey(websocket.GetHashCode()))
            {
                RemoteSubscriptions[WebsocketDict[websocket.GetHashCode()]].Reconnect();
            }
        }

        private void handleArrivedEvent(object sender, MessageEventArgs e)
        {
            dynamic incomingEvent = JsonConvert.DeserializeObject(e.Data);
            switch (incomingEvent.PerformanceLevel.ToString())
            {
                //hi performance mode use remote event arg
                case "1":
                    ParentThing.MyHub.EventHub.Publish(JsonConvert.DeserializeObject<RemoteEventArgLevel1>(e.Data).ConvertToEventArg());
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
            foreach (var sub in RemoteSubscriptions.Values)
            {
                if (sub.UnSubscribe(token.Id))
                {
                    ///so when i close it will not reconnect
                    sub.WebSocket.OnClose -= reconnect;
                    sub.WebSocket.Close();
                    RemoteSubscriptions.Remove(sub.Host);
                }
            }
        }

  

    }
}
