using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Resource
{
    /// <summary>
    /// the base of all resources, holds basic info
    /// the cfet2hub hold a original instance of the resources, and will make a cpoy of it the cpoy can be modified accoding to permission control 
    /// (todo: implement the copy, now just give everyone the orignal)
    /// </summary>
    public abstract class ResourceBase
    {
        public string  Name { get; protected set; }

        /// <summary>
        /// the path to this resource
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// is this the orignal copy
        /// </summary>
        public bool IsOriginal { get; internal set; } = true;

        public ResourceTypes ResourceType { get; protected set; } = ResourceTypes.Abstract;

        /// <summary>
        /// get the describtion of this 
        /// </summary>
        public virtual ResourceInfo Info{ get;}

        public virtual object Get(params object[] inputs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get the value of the resource, you provide a dictionary as an input,
        /// the key is the parameter name, 
        /// failed to return a value then no exception will be thrown but you get a invalide sample.
        /// </summary>
        /// <param name="inputDict"></param>
        /// <returns></returns>
        public virtual object Get(Dictionary<string, object> inputDict)
        {
            return this.Get(new DictionaryInputsIndicator(), inputDict);
        }
    }



}
