using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jtext103.CFET2.WebsocketEvent
{
    /// <summary>
    /// performance level 1 remote event arg
    /// </summary>
    public class RemoteEventArgLevel1
    {
        public int PerformanceLevel { get; } = 1;
        
        public string Source { get; set; }
        public string EventType { get; set; }

        public RemoteEventArgLevel1(EventArg e)
        {
            Source = e.Source;
            EventType = e.EventType;
            Value = e.Sample.ObjectVal;
        }

        public RemoteEventArgLevel1()
        {

        }

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
            var eventArg= new EventArg(Source, EventType, Value.ToStatus(),host);
            eventArg.PerformanceLevel = PerformanceLevel;
            return eventArg;
        }

       

    }
}
