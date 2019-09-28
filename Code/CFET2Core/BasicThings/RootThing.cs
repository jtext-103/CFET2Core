using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.BasicThings
{
    /// <summary>
    /// this thing does absulte nothing.  
    /// it is for navigational usage
    /// </summary>
    public class RootThing : Thing
    {
        /// <summary>
        /// 
        /// </summary>
        [Cfet2Config]
        public string HostName { get; set; } = "host";
    }
}
