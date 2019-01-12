using Jtext103.CFET2.Core.Event;
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
    public class RemoteSubscription
    {
        private ConcurrentQueue<EventFilter> pendingSubscriptions = new ConcurrentQueue<EventFilter>();

        /// <summary>
        /// the performance level of these subscriptions, all subscriptoins to the same host share the same performance level, that the lowest one
        /// </summary>
        public int PerformanceLevel { get; private set; } = 100;

        /// <summary>
        /// the web socket to recieve the push event
        /// </summary>
        public WebSocket WebSocket { get; set; }

        public string Host { get; set; }

        public Guid ClientId { get; set; }

        /// <summary>
        /// the guid is the token id
        /// </summary>
        public Dictionary<Guid, EventFilter> EventFilters { get; } = new Dictionary<Guid, EventFilter>();

        private bool isConnecting=false;
        private object connectionLock = new object();

        /// <summary>
        /// this is locked, so thread save, if this host is sending or connecting
        /// </summary>
        /// <returns></returns>
        public bool GetIsSending()
        {
            lock (connectionLock)
            {
                return isConnecting;
            }
        }

        /// <summary>
        ///  this is locked, so thread sace
        /// </summary>
        public void SetIsConnection(bool connecting)
        {
            lock (connectionLock)
            {
                isConnecting=connecting;
            }
        }

        internal async Task SendSubscriptionAsync()
        {
            await Task.Run(()=> {
                sendSubs();
            });

        }

        /// <summary>
        /// this is called in a thread, and will keep connecting to the web socket untill connected 
        /// </summary>
        private void sendSubs()
        {
            //if not already sending or connection, 
            if (!GetIsSending())
            {
                SetIsConnection(true);
                //send subscribe action
                while (WebSocket.ReadyState != WebSocketState.Open)
                {
                    WebSocket.ConnectAsync();
                    Thread.Sleep(1000);
                }
                //connected!
                while (pendingSubscriptions.Count > 0)
                {
                    if (WebSocket.ReadyState == WebSocketState.Open)
                    {
                        EventFilter filter;
                        pendingSubscriptions.TryDequeue(out filter);
                        var eventRequest = new EventRequest(filter,EventRequestAction.Subscribe,ClientId);
                        WebSocket.SendAsync(JsonConvert.SerializeObject(eventRequest), (result) => { });
                    }
                }
            }
            //if it is connection after connection all pending subscription will be send
        }

        internal void AddNewSubscription(Guid tokenId, EventFilter filter)
        {
            if (PerformanceLevel>filter.PerformanceLevel)
            {
                PerformanceLevel = filter.PerformanceLevel;
            }
            EventFilters.Add(tokenId, filter);
            pendingSubscriptions.Enqueue(filter);
        }

        /// <summary>
        /// this is non blocking, it will put a event into pending list and start re-sending them
        /// </summary>
        internal void Reconnect()
        {
            //emtpty the quere so i can re add all event filter to it
            //potential bug, it the sending thread is trying to dequeue and it get recreated, it will crash, this is a extremely rare case
            //todo fix this by suing lock or somehting
            pendingSubscriptions = new ConcurrentQueue<EventFilter>();
            foreach (var filter in EventFilters)
            {
                pendingSubscriptions.Enqueue(filter.Value);
            }
            SendSubscriptionAsync();
        }

        public bool UnSubscribe(Guid tokenId)
        {
            if (!EventFilters.ContainsKey(tokenId))
            {
                return false;
            }

            //send unsubscribption, if connected
            WebSocket.SendAsync(JsonConvert.SerializeObject(new EventRequest(EventFilters[tokenId], EventRequestAction.Unsubscribe,ClientId)), (result) => { });

            //remove the sub from the dictionery
            var lastPl = EventFilters[tokenId].PerformanceLevel;
            EventFilters.Remove(tokenId);

            //if it is the last one return ture to let the out side to 
            if (EventFilters.Count==0)
            {
                return true;
            }
            //check the performace level
            int pl = 100;
            foreach (var filter in EventFilters.Values)
            {
                if (pl>filter.PerformanceLevel)
                {
                    pl = filter.PerformanceLevel;
                }
            }
            
            if (lastPl<pl)
            {   //the remaining pl are all higher then the one removed, then rais the pl
                PerformanceLevel = pl;
            }
            return false;
        }

        //todo for better reliability there shoud be a method to replace the subscriptions on the server.
    }
}
