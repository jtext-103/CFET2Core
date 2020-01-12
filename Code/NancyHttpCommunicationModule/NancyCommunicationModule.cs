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
using MsgPack.Serialization;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    /// <summary>
    /// 使用 Nancy 进行 HTTP 通信，处理网页请求
    /// </summary>
    public class NancyCommunicationModule : CommunicationModule
    {
        public override string[] ProtocolNames => new string[] { "http" };

        NancyServer myServer;

        public string AcceptEncoding;

        Uri uriHost;

        /// <summary>
        /// 设置 Nancy HTTP 通信模块实例
        /// </summary>
        /// <param name="myUriHost">本地 Host 地址</param>
        /// <param name="accept">请求编码方式，默认为 "JSON"， 还支持 "MessagePack"</param>
        public NancyCommunicationModule(Uri myUriHost, string accept = "JSON")
        {
            AcceptEncoding = accept;
            uriHost = myUriHost;
        }

        public override void Start()
        {
            myServer = new NancyServer(MyHub, uriHost);
            myServer.Start();
        }

        public override ISample TryGetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            return HTTPRequest("GET", requestUri, null, inputs);
        }

        public override ISample TryGetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            return HTTPRequest("GET", requestUri, inputDict, null);
        }

        public override ISample TryInvokeSampleResourceWithUri(string requestUri, params object[] inputs)
        {
            return HTTPRequest("PUT", requestUri, null, inputs);
        }

        public override ISample TryInvokeSampleResourceWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            return HTTPRequest("PUT", requestUri, inputDict, null);
        }

        public override ISample TrySetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            return HTTPRequest("POST", requestUri, null, inputs);
        }

        public override ISample TrySetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            return HTTPRequest("POST", requestUri, inputDict, null);
        }

        private ISample HTTPRequest(string method, string requestUri, Dictionary<string, object> inputDict, params object[] inputs)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUri);
            req.Method = method;
            req.ContentLength = 0;

            if (AcceptEncoding == "MessagePack")
            {
                //req.Headers.Set(HttpRequestHeader.Accept, "application/octet-stream");
                req.Headers.Set(HttpRequestHeader.AcceptEncoding, "MessagePack");
            }

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
            ISample result = new SampleBase<object>();

            try
            {
                if (AcceptEncoding == "MessagePack")
                {
                    MemoryStream realStream = new MemoryStream();
                    stream.CopyTo(realStream);

                    var deserializer = MessagePackSerializer.Get<Dictionary<string, object>>();
                    realStream.Position = 0;
                    result.Context = deserializer.Unpack(realStream);
                }
                else
                {
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();
                    result.Context = (JsonConvert.DeserializeObject<Dictionary<string, object>>(content));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Cannot convert response content to a ISample! Message: " + e.ToString());
            }

            return result;
        }
    }
}
