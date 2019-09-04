using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Middleware
{
    /// <summary>
    /// a container for cfet2 middleware, it runs 
    /// </summary>
    public class Pipeline : CFET2Module
    {
        /// <summary>
        /// holds midware and process the sample with them
        /// </summary>
        public Pipeline()
        {
            midwares = new List<ICfet2Middleware>();
        }
        private List<ICfet2Middleware> midwares;

        /// <summary>
        /// if the pipeline is started, if true
        /// </summary>
        public bool Started { get; private set; } = false;

        /// <summary>
        /// let all the sample be processed by the midware, sequentially
        /// </summary>
        /// <param name="input">the sample to be processed</param>
        /// <param name="request">the request that get this sample</param>
        /// <returns></returns>
        public ISample BatchProcess(ISample input, ResourceRequest request)
        {
            for (int i = 0; i < midwares.Count; i++)
            {
                input = midwares[i].Process(input, request);
            }
            return input;
        }

        /// <summary>
        /// you can not add midware after invoke this
        /// </summary>
        public void Start()
        {
            Started = true;
        }

        /// <summary>
        /// add a middle ware tot he end of the pipeline
        /// </summary>
        /// <param name="midware"></param>
        public void AddMiddleware(ICfet2Middleware midware)
        {
            if (Started == true)
            {
                throw new System.Exception("You cannot add middleware after the pipeline started");
            }
            //@chenming
            HubMaster.InjectHubToModule(midware);
            midwares.Add(midware);
        }
    }
}