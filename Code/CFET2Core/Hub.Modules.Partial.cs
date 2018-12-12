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
    public partial class Hub
    {
        //this file has the code handle add thing and communicaiton etc. all about external modules
        /// <summary>
        /// for test only, only can only called by host
        /// </summary>
        public static void KillMaster()
        {
            HubMaster.KillMe();
        }


        /// <summary>
        /// start the communication this should done before start all things after all things been added
        /// </summary>
        public void StartCommunication()
        {
            myMaster.MyCommunicationManager.StartAll();
        }


        /// <summary>
        /// add the communication module to the hub.
        /// </summary>
        /// <param name="communicationModule"></param>
        public void TryAddCommunicationModule(CommunicationModule communicationModule)
        {
            myMaster.MyCommunicationManager.TryAddCommunicationModule(communicationModule);
        }





        /// <summary>
        /// add a thing object into the cfet app, will thrown if not sucesesful, 
        /// also you things are init here,hub is injected here then the TryInit is called here
        /// </summary>
        /// <param name="thing">the thing you want to add</param>
        /// <param name="name">the name of the thing</param>
        /// <param name="mountPath">the path you want your thing be mounted, the path will be /[mountpath]/[name]</param>
        /// <param name="initObject"></param>
        public void TryAddThing(Thing thing, string mountPath, string name, object initObject = null)
        {
            //make the path for the thing
            if (mountPath.EndsWith(@"/") == false)
            {
                mountPath = mountPath + @"/";
            }
            var thingPath = (mountPath + name);
            if (myMaster.Resources.ContainsKey(thingPath))
            {
                throw new GeneralCfet2Exception("Duplacated path!!");
            }
            //inject  hub here
            HubMaster.InjectHubToModule(thing);
            //TryInitThing is called here
            //thing get probed here
            var tThing = new ResourceThing(thing, name, initObject);
            tThing.Path = thingPath;
            thing.Path = thingPath;
            //add thing to resources
            myMaster.Resources[thingPath] = tThing;
            myMaster.thingPathes.Add(thingPath);
            //add the resources of the thing to the dict
            foreach (var item in tThing.Resources)
            {
                //make the path
                var resPath = thingPath + @"/" + item.Key;
                item.Value.Path = resPath;
                myMaster.Resources[resPath] = item.Value;
            }
        }


        /// <summary>
        /// this should only be called by host, it will call the start method on the things
        /// </summary>
        public void StartThings()
        {
            for (int i = 0; i < myMaster.thingPathes.Count; i++)
            {
                var thing = myMaster.Resources[myMaster.thingPathes[i]];
                if (thing.ResourceType == ResourceTypes.Thing)
                {
                    (thing as ResourceThing).TheThing.Start();
                }
            }
        }

        /// <summary>
        /// this should only be called by host, it will call the start method on the things
        /// </summary>
        public void DisposeThings()
        {
            for (int i = 0; i < myMaster.thingPathes.Count; i++)
            {
                var thing = myMaster.Resources[myMaster.thingPathes[i]];
                if (thing.ResourceType == ResourceTypes.Thing)
                {
                    var theThing = (thing as ResourceThing).TheThing as IDisposable;
                    if (theThing != null)
                    {
                        theThing.Dispose();
                    }
                }
            }

        }


        /// <summary>
        /// load configs of all thing fomr a directory, the diretory is the "/" each folder represents a thing, 
        /// the folder name is the instance name of the thing, the folder contains a config.json file for the thing, 
        /// there could be a dll in the folder if it want to load the thing using its own dll instead of ones in the ThingAssembliesPath
        /// folders without a config.json file is an empty node that hold other things
        /// todo hierachy, now all the things is all flat
        /// </summary>
        /// <param name="configRoot">the root diretory that contains all thing configs</param>
        public void LoadThingsFromDiretoryConfig(string configRoot)
        {
            throw new NotImplementedException();
        }
    }
}
