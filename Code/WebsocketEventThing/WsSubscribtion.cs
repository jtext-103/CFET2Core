using Jtext103.CFET2.Core.Event;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp.Server;

namespace Jtext103.CFET2.WebsocketEvent
{
    /// <summary>
    /// subscribtion stored on server side
    /// </summary>
    public class WsSubscription
    {
        /// <summary>
        /// redundent id
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// the connection session to push
        /// </summary>
        public IWebSocketSession Session { get; set; }

        /// <summary>
        /// for cancle
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// mostly used for perfomance level
        /// </summary>
        public EventFilter EventRequest { get; set; }

    }
}
