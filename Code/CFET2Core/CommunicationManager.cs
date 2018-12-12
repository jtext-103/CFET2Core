using Jtext103.CFET2.Core.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core
{
    /// <summary>
    /// manages all the remote protocols
    /// </summary>
    internal class CommunicationManager
    {

        Dictionary<string, CommunicationModule> moduleDict = new Dictionary<string, CommunicationModule>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// return the communication module for the protocol
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public CommunicationModule GetMouduleFor(string protocol)
        {
            return moduleDict.ContainsKey(protocol) ? moduleDict[protocol]:null;
        }

        /// <summary>
        /// add module to the manager, and register the protocol name with the module
        /// </summary>
        /// <param name="module"></param>
        public void TryAddCommunicationModule(CommunicationModule module)
        {
            HubMaster.InjectHubToModule(module);
            foreach (var protocol in module.ProtocolNames)
            {
                moduleDict.Add(protocol, module);
            }
        }

        internal void StartAll()
        {
            foreach (var item in moduleDict.Values)
            {
                item.Start();
            }
        }
    }
}
