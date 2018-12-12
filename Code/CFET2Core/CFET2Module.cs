using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core
{
    /// <summary>
    /// this is the base class that user may implemented to use cfet
    /// do not instatiate this class!! do not derive from this class directly!!! 
    /// derive from CommunicationModule,CFET2Host and Thing
    /// </summary>
    public abstract class CFET2Module
    {
        /// <summary>
        /// all CFET2 Modules has this to consume the sfet2 service
        /// </summary>
        public Hub MyHub { get; private set; }

        internal void InjectHub(Hub hub)
        {
            MyHub = hub;
        }
    }
}
