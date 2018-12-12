using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.Sample;

namespace Jtext103.CFET2.Core.Middleware
{
    /// <summary>
    /// the  base to all cfet2 middle ware whic is used to process the samples
    /// </summary>
    public class CfetMiddlewareBase :CFET2Module, ICfet2Middleware
    {
        /// <summary>
        /// process the sample,
        /// </summary>
        /// <param name="input">the sample to be processed</param>
        /// <param name="request">the request object that requested the origninal sample</param>
        /// <returns>a precessed sample</returns>
        public virtual ISample Process(ISample input, ResourceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
