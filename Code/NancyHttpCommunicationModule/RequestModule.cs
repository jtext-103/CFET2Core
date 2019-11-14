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

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{

    public class RequestModule : NancyModule
    {
        //private ViewSelector viewSelector = new ViewSelector();

        string viewPath = "/views/index.html";

        public RequestModule()
        {
            Get["/"] = r =>
            {
                return GetResponse(AccessAction.get);
            };

            Get["/{name*}"] = r =>
            {
                return GetResponse(AccessAction.get);
            };

            Put["/{name*}"] = r =>
            {
                return GetResponse(AccessAction.invoke);
            };

            Post["/{name*}"] = r =>
            {
                return GetResponse(AccessAction.set);
            };

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

                var rightResponse = JsonConvert.SerializeObject(result.Context);
                return Response.AsText(rightResponse);
            }
        }

        #region GZipNotFinished
        static string GZipCompressString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                byte[] zippedData = Compress(rawData);
                return (string)(Convert.ToBase64String(zippedData));
            }

        }

        static byte[] Compress(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }
        #endregion

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
