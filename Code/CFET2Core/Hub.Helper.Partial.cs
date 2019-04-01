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
        #region helpers


        /// <summary>
        /// get inpus or inputdict from the request uri, check the inputdict==null to see which input use, if not null use it otherwise use inputs public for test
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public (string ResourcePath, object[] Inputs, Dictionary<string, object> InputDict) ParseLocalRequest(Uri requestUri, params object[] inputs)
        {
            if (requestUri.IsAbsoluteUri == false)  //make sure it is a absolut uri
            {
                requestUri = new Uri(CommonConstants.LocalBaseUri, requestUri);
            }
            var resourcePathToStart = requestUri.AbsolutePath;    //this path may include parameters
            var resourcePath = resourcePathToStart;         //used for error message todo

            //we have some kinds of uri parameters here
            var routeInputs = new List<object>();
            Dictionary<string, object> queryInputDict = null;

            //find the resouce and use the excessive route segments as route parameter
            resourcePath = extractResourcePath(resourcePathToStart, routeInputs);

            if (inputs != null && inputs.Length > 0) //if we have input array the query string and input dict will be ignored
            {
                for (int i = 0; i < inputs.Length; i++) //using for to matain the order
                {
                    routeInputs.Add(inputs[i]);
                }
                //queryInputDict will be null
            }
            else //if we have query srting or dict as input, all route parameters will be joined with "/" and put to the last
            {
                //extract parameters from query
                queryInputDict = requestUri.ExtractParamsFromQuery();
                if (queryInputDict.Count == 0)
                {
                    queryInputDict = null; //indicating for the outside that it has not dict input or query string
                }
            }
            return (resourcePath, routeInputs.ToArray(), queryInputDict);
        }

        /// <summary>
        /// beside extract a path to a resource it also populate the input with route parameters
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="routeInputs">output the route parameter</param>
        /// <returns></returns>
        private string extractResourcePath(string resourcePath, List<object> routeInputs)
        {
            while (GetLocalResouce(resourcePath) == null)
            {
                //loop go up and up the last segment in parameters
                if (resourcePath == @"/" || string.IsNullOrEmpty(resourcePath))
                {
                    //no resource found
                    throw new ResourceDoesNotExistException();
                }
                //go up
                var child = "";
                (resourcePath, child) = (new Uri(CommonConstants.LocalBaseUri, resourcePath)).GetParentPath();
                if (string.IsNullOrEmpty(child) == false)
                {
                    routeInputs.Insert(0, child);
                }
            }

            return resourcePath;
        }


        //BUG in linux
        //private static bool IsRemoteUri(Uri uri) => uri.IsAbsoluteUri == true && uri.Scheme != "cfet";

        /// <summary>
        /// return if a uri is a remote request
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static bool IsRemoteUri(Uri uri)
        {
            if (uri.IsAbsoluteUri == false) return false;
            if (uri.Scheme == "cfet") return false;

            //in linux, all path like "/pc" will be convert to file:///pc
            if (uri.IsFile == true) return false;
            return true;
        }

        #endregion
    }
}
