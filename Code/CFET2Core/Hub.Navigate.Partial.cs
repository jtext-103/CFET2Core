﻿using System;
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
        /// <param name="path">uri, can have route parameters or querries but will be ignored, only local uri is accepted</param>
        /// <returns>the resource object</returns>
        public ResourceBase FindLocalResourceWithPath(string path)
        {
            var realPath = extractResourcePath(path, new List<object>());
            return GetLocalResouce(realPath);
        }

        /// <summary>
        /// find the parent of resource object of a given uri, all the parameters are ignored
        /// </summary>
        /// <param name="path">uri, can have route parameters or querries but will be ignored, only local uri is accepted</param>
        /// <returns></returns>
        public ResourceBase FindLocalParentWithPath(string path)
        {
            path = (new Uri(CommonConstants.LocalBaseUri, path)).GetParentPath().ParentPath;
            var realPath = extractResourcePath(path, new List<object>());
            return GetLocalResouce(realPath);
        }

        /// <summary>
        /// find the children of resource object of a given uri, all the parameters are ignored
        /// </summary>
        /// <param name="path"></param>
        /// <returns>the children resource objects</returns>
        public IEnumerable<ResourceBase> FindLocalChildWithUri(string path)
        {
            //check start with / if not looking for root's children
            foreach (var resource in myMaster.Resources)
            {
                if (resource.Key.StartsWith(path) && resource.Key!=path && (resource.Key[path.Length].ToString()=="/" || path == "/"))
                {
                    if (FindLocalParentWithPath(resource.Value.Path).Path==path)
                    {
                        yield return resource.Value;
                    }
                }
            }
        }

    }
}
