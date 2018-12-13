using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.Communication;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Attributes;
using Newtonsoft.Json.Linq;


namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    /// <summary>
    /// 使用 Nancy 进行 HTTP 通信，处理网页请求
    /// </summary>
    public class NancyCommunicationModule : CommunicationModule
    {
        public override string[] ProtocolNames => new string[] { "http" };

        NancyServer myServer;

        Uri uriHost;
        public NancyCommunicationModule(Uri myUriHost)
        {
            uriHost = myUriHost;
        }

        public override void Start()
        {
            myServer = new NancyServer(MyHub, uriHost);
            myServer.Start();
        }

        //目前没有完善
        #region
        public override ISample TryGetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            //string retString = myHttpRequest.HttpGet(requestUri, null, -1);
            //return JObject.Parse(retString).ToObject<Status<Object>>();
            throw new NotImplementedException();          
        }


        public override ISample TryGetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            throw new NotImplementedException();
        }

        public override ISample TryInvokeSampleResourceWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            throw new NotImplementedException();
        }

        public override ISample TryInvokeSampleResourceWithUri(string requestUri, params object[] inputs)
        {
            throw new NotImplementedException();
        }

        public override ISample TrySetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            throw new NotImplementedException();
        }

        public override ISample TrySetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
