using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Communication
{
    /// <summary>
    /// implement cross process/machine resource access
    /// </summary>
    public abstract class CommunicationModule:CFET2Module
    {
        
        /// <summary>
        /// the protocal name this module can handle, like for dds is DDS for REST is HTTP
        /// </summary>
        public abstract string[]  ProtocolNames { get; }

        /// <summary>
        /// this is called after all things have been started, 
        /// </summary>
        public virtual void Start()
        {
            
        }

        #region get set invoke you need to implemented this, check the hub for reference

        public abstract ISample TryGetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict);
        public abstract ISample TryGetResourceSampleWithUri(string requestUri, params object[] inputs);
        public abstract ISample TryInvokeSampleResourceWithUri(string requestUri, Dictionary<string, object> inputDict);
        public abstract ISample TryInvokeSampleResourceWithUri(string requestUri, params object[] inputs);
        public abstract ISample TrySetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict);
        public abstract ISample TrySetResourceSampleWithUri(string requestUri, params object[] inputs);
        
        #endregion
    }
}
