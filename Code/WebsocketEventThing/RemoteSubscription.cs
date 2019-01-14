using Jtext103.CFET2.Core.Event;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Jtext103.CFET2.WebsocketEvent
{
    public class RemoteSubscription
    {
        private object lockObject = new object();
        private bool isSending = false;
        private bool isClosing = false;

        public RemoteSubscription( EventFilter filter,Action<EventArg> handler)
        {
            Host = filter.Host;
            EventFilter = filter;
            Handler = handler;
            WebSocket = new WebSocket(Host);
            WebSocket.OnMessage += onEventArrival;
            WebSocket.OnClose += onReconnect;
        }

        private void onReconnect(object sender, CloseEventArgs e)
        {
            Debug.WriteLine("ConnectionClosed, reconnect: " + WebSocket.ReadyState.ToString());
            Reconnect();
        }

        private void onEventArrival(object sender, MessageEventArgs e)
        {
            Debug.WriteLine("recieved: "+e.Data);
            EventArg eventArg;
            switch (EventFilter.PerformanceLevel)
            {                
                //hi performance mode use remote event arg
                case 1:
                    eventArg = JsonConvert.DeserializeObject<RemoteEventArgLevel1>(e.Data).ConvertToEventArg(Host);
                    eventArg.Sample.IsRemote = true;
                    eventArg.Sample.SetPath (eventArg.Source);
                    Handler(eventArg);
                    break;
                default:
                    //solve that you can not deserialize into an isample
                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new SampleJsonConverter());
                    eventArg = JsonConvert.DeserializeObject<EventArg>(e.Data, settings);
                    eventArg.Sample.IsRemote = true;
                    Handler(eventArg);
                    break;
            }
        }

        private bool getIsSending()
        {
            lock (lockObject)
            {
                return isSending;
            }
        }

        private void setIsSending(bool sending)
        {
            lock (lockObject)
            {
                isSending=sending;
            }
        }

        /// <summary>
        /// the web socket to recieve the push event
        /// </summary>
        public WebSocket WebSocket { get; set; }

        public string Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EventFilter EventFilter { get; }

        public Action<EventArg> Handler { get; }

        /// <summary>
        /// connect to the ws server and send the subscription object
        /// </summary>
        /// <returns></returns>
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
            if (getIsSending())
            {
                return;
            }
            setIsSending(true);
            while (WebSocket.ReadyState != WebSocketState.Open)
            {
                if (isClosing)
                {
                    return;
                }
                WebSocket.Connect();
                Debug.WriteLine("ConnectionState: " + WebSocket.ReadyState.ToString());
                Thread.Sleep(500);
            }
            //connected!
            Debug.WriteLine("Connected sending subscription: " + WebSocket.ReadyState.ToString());
            var eventRequest = new EventRequest(EventFilter, EventRequestAction.Subscribe);
            WebSocket.SendAsync(JsonConvert.SerializeObject(eventRequest), (result) => { });
            setIsSending(false);
        }

        /// <summary>
        /// this is non blocking, it will put a event into pending list and start re-sending them
        /// </summary>
        internal void Reconnect()
        {
            //just re subscribe

            SendSubscriptionAsync();
        }

        public void UnSubscribe()
        {
            WebSocket.OnClose -= onReconnect;
            WebSocket.OnMessage -= onEventArrival;
            isClosing = true;
            if (WebSocket.ReadyState == WebSocketState.Open)
            {
                WebSocket.SendAsync(JsonConvert.SerializeObject(new EventRequest(EventFilter, EventRequestAction.Unsubscribe)), (result) => { });
            }
            WebSocket.CloseAsync();
        }

        
        
    }
}
