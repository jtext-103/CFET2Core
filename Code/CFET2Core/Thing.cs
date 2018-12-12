using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core
{
    public abstract class Thing:CFET2Module
    {
        /// <summary>
        /// this get called just after add thing is called, that's the thing is instantiated and probed, now you don't have a proper hub 
        /// </summary>
        /// <param name="initObj"></param>
        public virtual void TryInit(object initObj)
        {
            //does nothing by defaule
        }


        /// <summary>
        /// this get called just after all thing is added, that's the thing is instantiated and probed, now you do have a proper hub 
        /// </summary>
        public virtual void Start()
        {
            //does nothing by defaule
        }

        /// <summary>
        /// the path to the thing this is valid only after the thing is added to the hub
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// return the path for a resouce in this thing
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public string GetPathFor(string resourceName)
        {
            return Path + @"/" + resourceName;
        }
    }
}
