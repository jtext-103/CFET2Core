using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Middleware;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Exception;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    /// <summary>
    /// 根据用户输入的URL返回对应的视图
    /// </summary>
    public class ViewSelector
    {
        static ViewConfig myViewConfig;

        static ViewSelector()
        {
            myViewConfig = new ViewConfig(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.DirectorySeparatorChar + "Views" + Path.DirectorySeparatorChar + "ViewSelector.json");
        }

        /// <summary>
        /// 根据 Requset 匹配 View 在执行目录（bin）下的路径，供return View[]使用，一定会返回，哪怕是错误页面
        /// </summary>
        /// <param name="request"></param>
        /// <returns>路径</returns>
        public void GetViewPath(Request request, ISample result, ref string viewPath, ref Status<string> fakeSample)
        {
            string specialPath = dealWithSpecialRequest(request, result, ref fakeSample);
            if(specialPath != null)
            {
                viewPath = specialPath;
                return;
            }

            //如果在上层被 catch 住了或者 result 不是基本类型（Status, Method, Config, Thing）
            //这里由于使用浏览器请求 method 和 config 会导致 result.IsValid 为 false，所以要处理一下
            if (result == null)
            {
                viewPath = @"404";
                return;
            }
            switch(result.ResourceType)
            {
                case ResourceTypes.Thing:
                    {
                        viewPath = @"thing";
                        return;
                    }
                case ResourceTypes.Status:
                    {
                        viewPath = "status";
                        return;
                    }
                case ResourceTypes.Method:
                    {
                        viewPath = "method";
                        return;
                    }
                case ResourceTypes.Config:
                    {
                        viewPath = "config";
                        return;
                    }            
            }
            viewPath = @"400";
        }

        //需要重构
        private string dealWithSpecialRequest(Request request, ISample result, ref Status<string> fakeSample)
        {
            foreach(var v in myViewConfig.RegexURLtoViewPathAndModel)
            {
                //这里的匹配逻辑是，Path匹配上了某个配置文件中的正则表达式，但是由于"/"会匹配到任何项，所以这里根据长度来限制，
                //也就是说，只有网页路径短于正则表达式路径才能匹配
                if(Regex.IsMatch(request.Path, v.Key) && request.Path.Length <= v.Key.Length) 
                {
                    fakeSample = new Status<string>();
                    fakeSample.SetPath(request.Path);
                    if(myViewConfig.RegexURLtoViewPathAndModel[v.Key].Params != null)
                    {
                        foreach (var p in myViewConfig.RegexURLtoViewPathAndModel[v.Key].Params)
                        {
                            fakeSample.Context.Add(p.Key, p.Value);
                        }
                    }
                    return v.Value.ViewPath;
                }
            }
            return null;
        }
    }
}
