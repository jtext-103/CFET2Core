using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.BasicThings.CustomView
{
    public class CustomViewConfig
    {
        static string matchFileName = @"\CustomViewConfig.json";

        public string DirPath { get; set; }

        public Dictionary<string, string> RegularMatches { get; set; }

        public CustomViewConfig(string dirPath)
        {
            DirPath = dirPath;
            JsonConvert.PopulateObject(File.ReadAllText(dirPath + matchFileName, Encoding.Default), this);
        }
    }
}
