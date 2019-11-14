using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.Communication;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Attributes;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;

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

        #region
        public override ISample TryGetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUri);
            req.Method = "GET";
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            
            Stream stream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            ISample result = new SampleBase<object>() ;
            try
            {
                result.Context = (JsonConvert.DeserializeObject<Dictionary<string, object>>(content));
            }
            catch (Exception e)
            {
                throw new Exception("Cannot convert response content to a ISample! Message: " + e.ToString());
            }

            return result;
        }

        public override ISample TryGetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {

            throw new NotImplementedException();
        }

        public override ISample TryInvokeSampleResourceWithUri(string requestUri, params object[] inputs)
        {
            throw new NotImplementedException();
        }

        public override ISample TryInvokeSampleResourceWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            throw new NotImplementedException();
        }

        public override ISample TrySetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            throw new NotImplementedException();
        }

        public override ISample TrySetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
