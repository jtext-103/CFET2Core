using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Event
{
    /// <summary>
    /// store the source and type string for a event filter, regex expression that indicate the resources you interested in
    /// </summary>
    public class SourceAndTypeFilter
    {
        /// <summary>
        /// filter the source of the event
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// filter the type of the event, a regex expression to specify the event tyep you interested in
        /// </summary>
        public string EventType { get; set; }
    }
}
