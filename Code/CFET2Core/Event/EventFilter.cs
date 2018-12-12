using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Event
{
    /// <summary>
    /// a event filter to select the event you have subscribed
    /// </summary>
    public  class EventFilter
    {
        /// <summary>
        /// a regex expression that indicate the resources you interested in
        /// </summary>
        public string Source { get;  }


        /// <summary>
        /// a regex expression to specify the event tyep you interested in
        /// </summary>
        public string EventType { get;  }

        private readonly Regex resourceRegexMatch;
        private readonly Regex eventTypeRegexMatch;


        /// <summary>
        /// create a event filter with the input
        /// </summary>
        /// <param name="resource">IgnoreCase</param>
        /// <param name="eventType">IgnoreCase</param>
        public EventFilter(string resource, string eventType)
        {
            Source = resource;
            EventType = eventType;
            resourceRegexMatch = new Regex(Source,RegexOptions.IgnoreCase);
            eventTypeRegexMatch = new Regex(EventType, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// returen if a event matches the filter
        /// </summary>
        /// <param name="eventArg">the event to filter</param>
        /// <returns>is this event match the event filter, if so it means it need to be handled</returns>
        public bool Predicate(EventArg eventArg)
        {
            string resource = eventArg.Source;
            string eventType = eventArg.EventType;
            var souResult = resourceRegexMatch.Match(resource);
            if (souResult.Success && souResult.Value==resource)
            {
                //test event type
                var eventTypeResult = eventTypeRegexMatch.Match(eventType);
                if (eventTypeResult.Success && eventTypeResult.Value==eventType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
