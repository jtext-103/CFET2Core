using System;
using System.Collections.Generic;
using System.Text;

namespace WebsocketEventThing
{
    /// <summary>
    /// this is the client send to the event object to subscrive the event
    /// </summary>
    public class EventRequest
    {
        /// <summary>
        /// if subscrube or unsub
        /// </summary>
        public EventRequestAction Action { get; set; }

        /// <summary>
        /// this is a regex just like one you used to subscribe a lock resource
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// 0 for lowest full sample,, 1 for minimum overhead
        /// </summary>
        public int PerformanceLevel { get; set; }

        public string EventType { get; set; }
    }
    public enum EventRequestAction
    {
        Subscribe, Unsubscribe
    }
}
