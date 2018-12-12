using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Communication
{
    /// <summary>
    /// this interface is for communication modules like DDS or REST
    /// </summary>
    [Obsolete]
    public interface ICommunication
    {
        /// <summary>
        /// remember to inject the hub into the communication module
        /// </summary>
        Hub MyHub { get;}

        /// <summary>
        /// the protocal name this module can handle, like for dds is DDS for REST is HTTP
        /// </summary>
        string ProtocalName { get; }

        ISample SetResouceSample(string resourceUri, params object[] inputs);


        ISample GetResouceSample(string resourceUri, params object[] inputs);

        void Init(Hub hub);
 


    }
}
