using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Middleware
{
    /// <summary>
    /// all the middleware that controlled by pipleline need to implement this interface
    /// </summary>
    public interface ICfet2Middleware
    {
        Hub MyHub {  get; }

        /// <summary>
        /// process the sample
        /// </summary>
        /// <param name="input"></param>
        /// <param name="request">the request to the sample</param>
        /// <returns></returns>
        ISample Process(ISample input, ResourceRequest request);
    }
}
