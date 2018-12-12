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
        /// <summary>
        /// find the resource object of a given uri, all the parameters are ignored
        /// </summary>
        /// <param name="uri">uri, can have route parameters or querries but will be ignored</param>
        /// <returns>the resource object</returns>
        public ResourceBase FindLocalResourceWithUri(string uri)
        {
            throw new NotImplementedException("");
        }

        /// <summary>
        /// find the parent of resource object of a given uri, all the parameters are ignored
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public ResourceBase FindLocalParentWithUri(string uri)
        {
            throw new NotImplementedException("");
        }

        /// <summary>
        /// find the children of resource object of a given uri, all the parameters are ignored
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>the children resource objects</returns>
        public IEnumerable<ResourceBase> FindLocalChildWithUri(string uri)
        {
            throw new NotImplementedException("");
        }

    }
}
