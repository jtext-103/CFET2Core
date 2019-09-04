using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.Sample;

namespace Jtext103.CFET2.Core.Middleware
{
    /// <summary>
    /// this plugin adds resource info like input and return type into the sample 
    /// </summary>
    public class ResourceInfoMidware : CfetMiddlewareBase
    {
        /// <summary>
        /// get the resour info from the resource put in to the context
        /// </summary>
        /// <param name="input">the input sample</param>
        /// <param name="request">note: todo there is a flag indecating weather this plugin is activate</param>
        /// <returns></returns>
        public override ISample Process(ISample input, ResourceRequest request)
        {
            //find the resource using the path in the sample
            //ask for resource info
            // put in context
        }
    }
}
