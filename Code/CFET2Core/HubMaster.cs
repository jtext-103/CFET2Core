using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.Resource;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Exception;
using System.Net.Http;
using Jtext103.CFET2.Core.Extension;
using Jtext103.CFET2.Core.Communication;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Middleware;

namespace Jtext103.CFET2.Core
{
    /// <summary>
    /// the hub master hold all raw resource, the hub does not, so to control the access right of the hub
    /// </summary>
    public class HubMaster
    {
        private static HubMaster instance;
        #region props
        /// <summary>
        /// the directory that holds all the assembly dlls for thing that later loaded into the Hub
        /// </summary>
        public string ThingAssembliesPath { get; set; }

        internal EventHub MyEventHub { get; } = new EventHub();

        internal Pipeline MyPipeline { get; set; }

        //public Dictionary<string, CommunicationModule> CommunicationModules {get;}=new Dictionary<string,CommunicationModule>(StringComparer.InvariantCultureIgnoreCase);
        internal CommunicationManager MyCommunicationManager = new CommunicationManager();


        private static bool hostLock = false;

        internal static void KillMe()
        {
            instance.MyEventHub.Dispose();
            instance = null;
            //do we need to unlock the host?
            hostLock = false;
        }

        /// <summary>
        /// all thing stored in a flat structure, will become obsolete when hierarchy is done, 
        /// the dict is case insensitive,  
        /// the key is the path to the resource
        /// </summary>
        public Dictionary<string, ResourceBase> Resources { get; } = new Dictionary<string, ResourceBase>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// just for start and dispose, you can start and dispose in the order of you added it
        /// </summary>
        internal List<string> thingPathes = new List<string>();
        #endregion

        private HubMaster()
        {
            MyEventHub.Init();
        }

        internal static HubMaster getInstance()
        {
            if(instance==null)
            {
                instance = new HubMaster();
            }
            return instance;   
        }



        /// <summary>
        /// inject a hub-proxy- into a object it can be an thing or an communication or an CFET host
        /// </summary>
        /// <param name="targetModule"></param>
        public static void InjectHubToModule(object targetModule)
        {
            var master = getInstance();

            switch (targetModule)
            {
                case Thing thing:
                    //todo make a special hub proxy for the thing
                    thing.InjectHub(new Hub(thing));
                    return;
                case CFET2Host host:
                    //one the host is jinjected no more host injection allowed
                    if (hostLock == false)
                    {
                        host.InjectHub(new Hub(host));
                        hostLock = true;
                        return;
                    }
                    throw new GeneralCfet2Exception("only one host allowed!");
                case CommunicationModule comm:
                    comm.InjectHub(new Hub(comm));
                    return;
                case CfetMiddlewareBase midWare:
                    midWare.InjectHub(new Hub(midWare));
                    return;
                case Pipeline pipeline:
                    pipeline.InjectHub(new Hub(pipeline));
                    return;
                default:
                    break;
            }
        }




    }
}
