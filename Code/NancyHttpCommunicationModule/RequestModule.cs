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

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    public class RequestModule : NancyModule
    {
        //private ViewSelector viewSelector = new ViewSelector();

        string viewPath = "/view";

        public RequestModule()
        {
            Get["/"] = r =>
            {
                return GetResponse(AccessAction.get);
            };

            Get["/{name*}"] = r =>
            {
                if (Request.Url.Path.StartsWith(viewPath))
                {
                    return GetResponse(AccessAction.get, true);
                }
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

        #region GetResponseOldVersion
        //private object GetResponse(AccessAction action)
        //{
        //    //无论是否从浏览器请求，都先请求一个 ISample 出来
        //    ResourceRequest request = new ResourceRequest(this.Request.Url.Path + this.Request.Url.Query.ToString(), action, null, null, null);
        //    ISample result;

        //    Status<string> fakeSample = null;
        //    string viewPath = null;

        //    //这里的逻辑是，如果先从 Hub 获取 ISample，然后，如果请求不是来自浏览器则直接返回获取的 ISample（不管是不是 Vaild）或者错误信息；
        //    //如果请求来自浏览器，则将 ISample（区分是否存在，也就是是否被 catch） 交给 ViewSelector ，然后 ViewSelector 返回对应的视图
        //    //也就是说，只要请求不是来自浏览器，则在这段程序中全部处理了并返回 C# 类型；如果请求来自浏览器，则全部返回 View
        //    try
        //    {
        //        result = NancyServer.TheHub.TryAccessResourceSampleWithUri(request);
        //    }
        //    //只有没有从Hub中获取到ISample才会去设置fakeSample
        //    catch (ResourceDoesNotExistException e)
        //    {
        //        if (!isFromBrowser())
        //        {
        //            var response = new NotFoundResponse();
        //            response.StatusCode = HttpStatusCode.NotFound;
        //            return response;
        //        }
        //        else
        //        {
        //            viewSelector.GetViewPath(this.Request, null, ref viewPath, ref fakeSample);
        //            return View[viewPath, fakeSample];
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (!isFromBrowser())
        //        {
        //            var response = new NotFoundResponse();
        //            response.StatusCode = HttpStatusCode.BadRequest;
        //            return response;
        //        }
        //        else
        //        {
        //            viewSelector.GetViewPath(this.Request, null, ref viewPath, ref fakeSample);
        //            return View[viewPath, fakeSample];
        //        }
        //    }
        //    if (!isFromBrowser())
        //    {
        //        //这里之前是result，导致冗余，数据大时序列化时间过长
        //        var response = JsonConvert.SerializeObject(result.Context);
        //        return Response.AsText(response);
        //    }
        //    viewSelector.GetViewPath(this.Request, result, ref viewPath, ref fakeSample);
        //    return View[viewPath, result];
        //}
        #endregion

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
