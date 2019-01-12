using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebsocketEventThing
{
    /// <summary>
    /// performance level 1 remote event arg
    /// </summary>
    public class RemoteEventArgLevel1
    {
        public int PerformanceLevel { get; } = 1;
        
        public string Source { get; set; }
        public string EventType { get; set; }

        /// <summary>
        /// no sample just the raw wavlue
        /// </summary>
        public object Value { get; set; }

        public EventArg ConvertToEventArg()
        {
            return new EventArg(Source, EventType, Value.ToStatus());
        }

        public EventArg ConvertToEventArg(string host)
        {
            return new EventArg(Source, EventType, Value.ToStatus(),host);
        }

    }
}
