using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    /// <summary>
    /// ViewSelector配置文件内容
    /// </summary>
    public class ViewConfig
    {
        public Dictionary<string, ViewPathAndModel> RegexURLtoViewPathAndModel;

        /// <summary>
        /// 从配置文件中生成一个实例
        /// </summary>
        /// <param name="configFilePath"></param>
        public ViewConfig(string configFilePath)
        {
            JsonConvert.PopulateObject(File.ReadAllText(configFilePath, Encoding.Default), this);
        }
    }

    public class ViewPathAndModel
    {
        /// <summary>
        /// 在bin目录下的Views文件夹下的视图路径
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// 页面很可能会请求多个子页面，这里给出路径
        /// </summary>
        public string[] ParamArray { get; set; }

        /// <summary>
        /// 其它信息，自定义
        /// </summary>
        public Dictionary<string, string> Params { get; set; }
    }
}
