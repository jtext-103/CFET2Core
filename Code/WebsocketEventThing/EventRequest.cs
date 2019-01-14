using Jtext103.CFET2.Core.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtext103.CFET2.WebsocketEvent
{
    /// <summary>
    /// this is the client send to the event server to subscrive the event
    /// </summary>
    public class EventRequest:EventFilter
    {
        public EventRequest()
        {

        }

        public EventRequest(string resource, string eventType, EventRequestAction action):base( resource,  eventType)
        {
            Action = action;
        }

        /// <summary>
        /// if subscrube or unsub
        /// </summary>
        public EventRequestAction Action { get; set; }

        public EventRequest(EventFilter filter,EventRequestAction action): base(filter)
        {
            Action = action;
        }

    }
    public enum EventRequestAction
    {
        Subscribe, Unsubscribe
    }
}
