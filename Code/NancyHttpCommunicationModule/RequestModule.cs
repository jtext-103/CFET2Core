using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Middleware;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Exception;
using Nancy;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Nancy.Conventions;
using MsgPack.Serialization;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{

    public class RequestModule : NancyModule
    {
        //private ViewSelector viewSelector = new ViewSelector();

        string viewPath = "/views/index.html";

        public RequestModule()
        {
            Get("/", args =>
            {
                return GetResponse(AccessAction.get);
            });

            Get("/{name*}", args =>
            {
                return GetResponse(AccessAction.get);
            });

            Put("/{name*}", args =>
            {
                return GetResponse(AccessAction.invoke);
            });

            Post("/{name*}", args =>
            {
                return GetResponse(AccessAction.set);
            });

        }

        private object GetResponse(AccessAction action, bool shouldReturnView = false)
        {
            if (shouldReturnView)
            {
                return View["index"];
            }

            if (isFromBrowser())
            {
                string requestPath = this.Request.Url.Path;
                string queryString = this.Request.Url.Query.ToString();
                string hashTag = viewPath + "#" + requestPath + queryString;
                return Response.AsRedirect(hashTag, Nancy.Responses.RedirectResponse.RedirectType.Permanent);
            }
            else
            {
                ResourceRequest request = new ResourceRequest(this.Request.Url.Path + this.Request.Url.Query.ToString(), action, null, null, null);
                ISample result;

                try
                {
                    result = NancyServer.TheHub.TryAccessResourceSampleWithUri(request);
                }
                catch (ResourceDoesNotExistException e)
                {
                    var response = new NotFoundResponse();
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }
                catch (Exception e)
                {
                    var response = new NotFoundResponse();
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }

                if (this.Request.Headers.AcceptEncoding.Contains("MessagePack"))
                {
                    var serializer = MessagePackSerializer.Get<Dictionary<string, object>>();
                    MemoryStream stream = new MemoryStream();
                    serializer.Pack(stream, result.Context);

                    SetData(stream);
                    var rightResponse = new Nancy.Responses.StreamResponse(GetData, "application/octet-stream");
                    rightResponse.Headers.Add(new KeyValuePair<string, string>("Content-Encoding", "MessagePack"));
                    return rightResponse;
                }
                else
                {
                    var rightResponse = JsonConvert.SerializeObject(result.Context);
                    return Response.AsText(rightResponse);
                }
            }
        }

        //配合Nancy返回数据
        Stream middle;
        private void SetData(Stream stream)
        {
            middle = stream;
        }
        private Stream GetData()
        {
            //非常非常非常非常重要！
            middle.Position = 0;

            return middle;
        }

        private bool isFromBrowser()
        {
            var accept = this.Request.Headers.Accept;
            foreach (var i in accept)
            {
                if (i.Item1 == "text/html")
                    return true;
            }
            return false;
        }
    }
}
