using Jtext103.CFET2.Core.Event;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;

namespace WebsocketEventThing
{
    public class RemoteSubscription
    {
        /// <summary>
        /// the web socket to recieve the push event
        /// </summary>
        public WebSocket WebSocket { get; set; }

        public EventFilter Filter { get; set; }
    }
}
