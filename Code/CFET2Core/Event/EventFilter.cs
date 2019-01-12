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
        /// a collection of  SourceAndTypeFilter, any mathch will fire the handler
        /// </summary>
        public List<SourceAndTypeFilter> SourcesAndTypes { get; } = new List<SourceAndTypeFilter>();

        /// <summary>
        /// the remote host of the event publisher
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// only applicable for remote event, 0 for lowest full sample,, 1 for minimum overhead
        /// </summary>
        public int PerformanceLevel { get; set; }


        private readonly List<Regex> resourceRegexMatches;
        private readonly List<Regex> eventTypeRegexMatches;


        /// <summary>
        /// create a event filter with the input
        /// </summary>
        /// <param name="resource">IgnoreCase</param>
        /// <param name="eventType">IgnoreCase</param>
        public EventFilter(string resource, string eventType)
        {
            SourcesAndTypes.Add(new SourceAndTypeFilter { Source=resource,EventType=eventType});
            (resourceRegexMatches, eventTypeRegexMatches) = makeRegexFilter(SourcesAndTypes);
            Host = "";
            PerformanceLevel = 0;
        }

        /// <summary>
        /// create a event filter with the input
        /// </summary>
        public EventFilter(EventFilter oldFilter)
        {
            SourcesAndTypes.AddRange(oldFilter.SourcesAndTypes);
            (resourceRegexMatches, eventTypeRegexMatches) = makeRegexFilter(SourcesAndTypes);
            Host = oldFilter.Host;
            PerformanceLevel = oldFilter.PerformanceLevel;
        }

        /// <summary>
        /// create a event filter with the input
        /// </summary>
        /// <param name="resource">IgnoreCase</param>
        /// <param name="eventType">IgnoreCase</param>
        /// <param name="performanceLevel"></param>
        /// <param name="host">the remote host incluting protocol</param>
        public EventFilter(string resource, string eventType,int performanceLevel,string host):this(resource, eventType)
        {
            Host = host;
            PerformanceLevel = performanceLevel;
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

            int matchIndex = 0;
            for (matchIndex=0; matchIndex < resourceRegexMatches.Count(); matchIndex++)
            {
                var souResult = resourceRegexMatches[matchIndex].Match(resource);
                if (souResult.Success && souResult.Value == resource)
                {
                    //test event type
                    var eventTypeResult = eventTypeRegexMatches[matchIndex].Match(eventType);
                    if (eventTypeResult.Success && eventTypeResult.Value == eventType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        private (List<Regex>, List<Regex>) makeRegexFilter(List<SourceAndTypeFilter> filters)
        {
            List<Regex> resources = new List<Regex>();
            List<Regex> eventTypes = new List<Regex>();
            foreach (var filter in filters)
            {
                resources.Add(new Regex(filter.Source, RegexOptions.IgnoreCase));
                eventTypes.Add( new Regex(filter.EventType, RegexOptions.IgnoreCase));
            }
            return (resources, eventTypes);
        }
    }
}
