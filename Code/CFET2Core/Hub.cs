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
    /// a proxy object for HubMaster, every cfet2 module has one to consume cfet2 service
    /// </summary>
    public partial class Hub
    {
        internal Hub(CFET2Module module)
        {
            //todo depending on the module types disable some function of the hub
            MyCfet2Module = module;
        }


        #region props

        /// <summary>
        /// all thing stored in a flat structure, will become obsolete when hierarchy is done
        /// </summary>
        //public Dictionary<string, ResourceBase> Resources { get; } = new Dictionary<string, ResourceBase>();


        private HubMaster myMaster
        {
            get
            {
                return HubMaster.getInstance();
            }
        }


        /// <summary>
        /// access the master's pipeline
        /// </summary>
        public Pipeline Pipeline
        {
            get
            {
                return myMaster.MyPipeline;
            }
        }

        /// <summary>
        /// the cfet module that holds this hub
        /// </summary>
        public CFET2Module MyCfet2Module { get; private set; }

        /// <summary>
        /// this is a reference to the MasterHub's Event hub
        /// </summary>
        public EventHub EventHub
        {
            get
            {
                return myMaster.MyEventHub;
            }
        }

        #endregion

       

        /// <summary>
        /// get all resources
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ResourceBase> GetAllLocalResources()
        {
            return myMaster.Resources;
        }

        #region local get set invoke

        /// <summary>
        /// get an resource objet of provided resource path,
        /// it could be thing, status, config,  and so on
        /// </summary>
        /// <param name="resourcePath">the path of the thing, in unix file path fasion</param>
        /// <returns>will return null it the path points to nothing</returns>
        public ResourceBase GetLocalResouce(string resourcePath)
        {
            if (myMaster.Resources.ContainsKey(resourcePath))
            {
                return myMaster.Resources[resourcePath];
            }
            return null;
        }


        /*get*/
        /// <summary>
        /// get an sample of that resource objet of provided resource uri, the uri must be a resource locator and contains no parameters
        /// it could be thing, status, config,  and so on, will thrown is the resource not found, give invalid sample
        /// </summary>
        /// <param name="resourcePath">the path to a locol resource, there is not input in it, todo: 1 support regex, 2. support relative path, easy</param>
        /// <param name="inputs">input parameter in array</param>
        /// <returns></returns>
        public ISample TryGetLocalResouceSample(string resourcePath, params object[] inputs)
        {
            var resource = GetLocalResouce(resourcePath);
            if (resource != null)
            {
                return resource.Get(inputs) as ISample;
            }
            throw new ResourceDoesNotExistException();
        }

        /// <summary>
        /// get an sample of that resource objet of provided resource uri, the uri must be a resource locator and contains no parameters
        /// it could be thing, status, config,  and so on, will thrown is the resource not found, give invalid sample
        /// </summary>
        /// <param name="resourcePath">the path to a locol resource, there is not input in it</param>
        /// <param name="inputDict">input parameter in dictionary</param>
        /// <returns></returns>
        public ISample TryGetLocalResouceSample(string resourcePath, Dictionary<string,object> inputDict)
        {
            var resource = GetLocalResouce(resourcePath);
            if (resource != null)
            {
                //this is almost the same as array but only add an indicator to indicate thie input has a dictionary
                return resource.Get(inputDict) as ISample;
            }
            throw new ResourceDoesNotExistException();
        }

        /*set*/

        /// <summary>
        /// only config resource support set
        /// </summary>
        /// <param name="resourcePath">the path to a locol resource, there is not input in it, todo: 1 support regex, 2. support relative path, easy</param>
        /// <param name="inputs">input parameter in array</param>
        /// <returns></returns>
        public ISample TrySetLocalResouceSample(string resourcePath, params object[] inputs)
        {
            var resource = GetLocalResouce(resourcePath);
            if (resource != null)
            {
                if (resource.ResourceType == ResourceTypes.Config)
                {
                    return (resource as ResourceConfig).Set(inputs) as ISample;
                }
                throw new WrongResourceActionException();
            }
            throw new ResourceDoesNotExistException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourcePath">the path to a locol resource, there is not input in it</param>
        /// <param name="inputDict">input parameter in dictionary</param>
        /// <returns></returns>
        public ISample TrySetLocalResouceSample(string resourcePath, Dictionary<string, object> inputDict)
        {
            var resource = GetLocalResouce(resourcePath);
            if (resource != null)
            {
                if (resource.ResourceType == ResourceTypes.Config)
                {
                    return (resource as ResourceConfig).Set(inputDict) as ISample;
                }
                throw new WrongResourceActionException();
            }
            throw new ResourceDoesNotExistException();
        }

        /*invoke*/
        /// <summary>
        /// only method resource support set
        /// </summary>
        /// <param name="resourcePath">the path to a locol resource, there is not input in it, todo: 1 support regex, 2. support relative path, easy</param>
        /// <param name="inputs">input parameter in array</param>
        /// <returns></returns>
        public ISample TryInvokeLocalResouceSample(string resourcePath, params object[] inputs)
        {
            var resource = GetLocalResouce(resourcePath);
            if (resource != null)
            {
                if (resource.ResourceType == ResourceTypes.Method)
                {
                    return (resource as ResourceMethod).Invoke(inputs) as ISample;
                }
                throw new WrongResourceActionException();
            }
            throw new ResourceDoesNotExistException();
        }

        /// <summary>
        /// invole a resource, localy, meaning no communicatin module will be invoked
        /// </summary>
        /// <param name="resourcePath">the path to a locol resource, there is not input in it</param>
        /// <param name="inputDict">input parameter in dictionary</param>
        /// <returns>result sample</returns>
        public ISample TryInvokeLocalResouceSample(string resourcePath, Dictionary<string, object> inputDict)
        {
            var resource = GetLocalResouce(resourcePath);
            if (resource != null)
            {
                if (resource.ResourceType == ResourceTypes.Method)
                {
                    return (resource as ResourceMethod).Invoke(inputDict) as ISample;
                }
                throw new WrongResourceActionException();
            }
            throw new ResourceDoesNotExistException();
        }



        #endregion



        #region get set invoke using uri 
        
        /*request resource, with pipeline support*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ISample TryAccessResourceSampleWithUri(ResourceRequest request)
        {
            ISample resource = null;
            switch (request.Action)
            {
                case AccessAction.get:
                    if (request.UsingInputDict)
                        resource = TryGetResourceSampleWithUri(request.RequestUri, request.InputDict);
                    else
                        resource = TryGetResourceSampleWithUri(request.RequestUri, request.InputArray);
                    break;
                case AccessAction.set:
                    if (request.UsingInputDict)
                        resource = TrySetResourceSampleWithUri(request.RequestUri, request.InputDict);
                    else
                        resource = TrySetResourceSampleWithUri(request.RequestUri, request.InputArray);
                    break;
                case AccessAction.invoke:
                    if (request.UsingInputDict)
                        resource = TryInvokeSampleResourceWithUri(request.RequestUri, request.InputDict);
                    else
                        resource = TryInvokeSampleResourceWithUri(request.RequestUri, request.InputArray);
                    break;
                default:
                    throw new WrongResourceActionException();
            }
            return myMaster.MyPipeline.BatchProcess(resource, request);
        }

        private ISample localAccessWithUri(Func<string, object[], ISample> arrayAccessFunc,
            Func<string, Dictionary<string, object>, ISample> dictAccessFunc,
            string requestUri, Dictionary<string, object> inputDict, object[] inputs,bool usingDictInput)
        {
            var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);
            //if is using dict indicate by setting ParseLocalRequest input = null to ignore the array input
            //the below find the real path to the resource by mathc from the path backward the excessive parts in the path is add to the input
            (var resourcePath, var realInputs, var queryInputDict) = ParseLocalRequest(uri, usingDictInput?null:inputs);

            if (queryInputDict != null || usingDictInput)
            {
                var realInputDict = new Dictionary<string, object>();
                if (queryInputDict != null)
                {
                    queryInputDict.ToList().ForEach(p => realInputDict[p.Key] = p.Value);
                }
                if (inputDict != null) //input dict override the query
                {
                    inputDict.ToList().ForEach(p => realInputDict[p.Key] = p.Value);
                }
                if (realInputs.Length > 0) //if we have querry or dict input we joint route parameter an put it at last
                {
                    var jointRouteInput = "";
                    for (int i = 0; i < realInputs.Length; i++)
                    {
                        jointRouteInput += @"/" + realInputs[i].ToString();
                    }
                    realInputDict.Add(CommonConstants.TheLastInputsKey, jointRouteInput);
                }
                return dictAccessFunc(resourcePath, realInputDict);
            }
            return arrayAccessFunc(resourcePath, realInputs);
        }

        


        /*get*/

        /// <summary>
        /// get a sample using a uri, the inputs are parameters to the resource, if you are using parameters in uri, then you can 
        /// have only one input, if you have more than one inputs, the uri must points to a resource without any route parameters and query string, 
        /// </summary>
        /// <param name="requestUri">the uri to access a resource, it can contain route parameters or query string parameters</param>
        /// <param name="inputs">if contain more than one inputs, then query string (if any) in uri will be ignored, the path of URI is point to a resource, route parameters will not be recognize</param>
        /// <returns></returns>
        public ISample TryGetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);
            if (IsRemoteUri(uri))
            {
                var comm = myMaster.MyCommunicationManager.GetMouduleFor(uri.Scheme);
                if (comm != null)
                {
                    return comm.TryGetResourceSampleWithUri(requestUri, inputs);
                }
                else
                {
                    throw new ProtocolNotSuportedException($"{uri.Scheme} is not supported");
                }
            }
            else
            {
                return localAccessWithUri(TryGetLocalResouceSample, TryGetLocalResouceSample,
                    requestUri, null, inputs, false);
            }
        }

        /// <summary>
        /// get the resource with dictionary input, using this, you can not use the route parameter, but you can still use query string parameters, the duplicated keys will be overrided by the inputDict 
        /// this is as if you have query string inthe uri but its in the input dict.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="inputDict"></param>
        /// <returns></returns>
        public ISample TryGetResourceSampleWithUri(string requestUri, Dictionary<string,object> inputDict)
        {
            var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);
            if (IsRemoteUri(uri))
            {
                var comm = myMaster.MyCommunicationManager.GetMouduleFor(uri.Scheme);
                if (comm != null)
                {
                    return comm.TryGetResourceSampleWithUri(requestUri, inputDict);
                }
                else
                {
                    throw new ProtocolNotSuportedException($"{uri.Scheme} is not supported");
                }
            }
            else
            {
                return localAccessWithUri(TryGetLocalResouceSample, TryGetLocalResouceSample,
                    requestUri, inputDict, null,  true);
            }
        }


        /*set*/
        /// <summary>
        /// set a sample using a uri, the inputs are parameters to the resource, if you are using parameters in uri, then you can 
        /// have only one input, if you have more than one inputs, the uri must points to a resource without any route parameters and query string, 
        /// </summary>
        /// <param name="requestUri">the uri to access a resource, it can contain route parameters or query string parameters</param>
        /// <param name="inputs">if contain more than one inputs, then query string (if any) in uri will be ignored, the path of URI is point to a resource, route parameters will not be recognize</param>
        /// <returns></returns>
        public ISample TrySetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);
            if (IsRemoteUri(uri))
            {
                var comm = myMaster.MyCommunicationManager.GetMouduleFor(uri.Scheme);
                if (comm != null)
                {
                    return comm.TrySetResourceSampleWithUri(requestUri, inputs);
                }
                else
                {
                    throw new ProtocolNotSuportedException($"{uri.Scheme} is not supported");
                }
            }
            else
            {
                return localAccessWithUri(TrySetLocalResouceSample, TrySetLocalResouceSample,
                    requestUri,null, inputs, false);
            }
        }

        /// <summary>
        /// set the resource with dictionary input, using this, you can not use the route parameter, but you can still use query string parameters, the duplicated keys will be overrided by the inputDict 
        /// this is as if you have query string inthe uri but its in the input dict.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="inputDict"></param>
        /// <returns></returns>
        public ISample TrySetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);

            if (IsRemoteUri(uri))
            {
                var comm = myMaster.MyCommunicationManager.GetMouduleFor(uri.Scheme);
                if (comm != null)
                {
                    return comm.TrySetResourceSampleWithUri(requestUri, inputDict);
                }
                else
                {
                    throw new ProtocolNotSuportedException($"{uri.Scheme} is not supported");
                }
            }
            else
            {
                return localAccessWithUri(TrySetLocalResouceSample, TrySetLocalResouceSample,
                    requestUri, inputDict, null, true);
            }
        }


        /*invoke*/
        /// <summary>
        /// set a sample using a uri, the inputs are parameters to the resource, if you are using parameters in uri, then you can 
        /// have only one input, if you have more than one inputs, the uri must points to a resource without any route parameters and query string, 
        /// </summary>
        /// <param name="requestUri">the uri to access a resource, it can contain route parameters or query string parameters</param>
        /// <param name="inputs">if contain more than one inputs, then query string (if any) in uri will be ignored, the path of URI is point to a resource, route parameters will not be recognize</param>
        /// <returns></returns>
        public ISample TryInvokeSampleResourceWithUri(string requestUri, params object[] inputs)
        {
            var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);
            if (IsRemoteUri(uri))
            {
                var comm = myMaster.MyCommunicationManager.GetMouduleFor(uri.Scheme);
                if (comm != null)
                {
                    return comm.TryInvokeSampleResourceWithUri(requestUri, inputs);
                }
                else
                {
                    throw new ProtocolNotSuportedException($"{uri.Scheme} is not supported");
                }
            }
            else
            {
                return localAccessWithUri(TryInvokeLocalResouceSample, TryInvokeLocalResouceSample,
                    requestUri, null, inputs, false);
            }
        }

        /// <summary>
        /// set the resource with dictionary input, using this, you can not use the route parameter, but you can still use query string parameters, the duplicated keys will be overrided by the inputDict 
        /// this is as if you have query string inthe uri but its in the input dict.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="inputDict"></param>
        /// <returns></returns>
        public ISample TryInvokeSampleResourceWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);
            if (IsRemoteUri(uri))
            {
                var comm = myMaster.MyCommunicationManager.GetMouduleFor(uri.Scheme);
                if (comm != null)
                {
                    return comm.TryInvokeSampleResourceWithUri(requestUri, inputDict);
                }
                else
                {
                    throw new ProtocolNotSuportedException($"{uri.Scheme} is not supported");
                }
            }
            else
            {
                return localAccessWithUri(TryInvokeLocalResouceSample, TryInvokeLocalResouceSample,
                    requestUri, inputDict, null, true);
            }
        }

        #endregion


    }
}
