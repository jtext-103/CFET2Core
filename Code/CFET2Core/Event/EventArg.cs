using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Event
{
    /// <summary>
    /// when a event is published this will be created to contain everything the event is about
    /// </summary>
    public class EventArg
    {
        /// <summary>
        /// a string that indicate the resources generated this event
        /// </summary>
        public string Source { get; }


        /// <summary>
        /// a string to specify the event tyep you interested in
        /// </summary>
        public string EventType { get; }

        /// <summary>
        /// the payload of the event wrapped in sample
        /// </summary>
        public ISample Sample { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="sample"></param>
        public EventArg(string source,string type,ISample sample)
        {
            Source = source;
            EventType = type;
            Sample = sample;
        }

    }
}
