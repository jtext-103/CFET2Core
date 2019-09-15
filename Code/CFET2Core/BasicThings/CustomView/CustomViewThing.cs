using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Extension;
using Jtext103.CFET2.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.BasicThings.CustomView;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;

namespace Jtext103.CFET2.Core.BasicThings
{
    public class CustomViewThing : Thing
    {
        private CustomViewConfig myConfig;

        public override void TryInit(object dirPath)
        {
            myConfig = new CustomViewConfig((string)dirPath);
        }

        [Cfet2Status]
        public string Template(string path)
        {
            foreach(var r in myConfig.RegularMatches)
            {
                Regex regex = new Regex(r.Key);
                if(regex.IsMatch(path))
                {
                    try
                    {
                        string fullFilePath = myConfig.DirPath + System.IO.Path.DirectorySeparatorChar + r.Value;
                        return File.ReadAllText(fullFilePath, Encoding.Default);
                    }
                    catch
                    {
                        throw new System.Exception("Config File has been damaged!");
                    }
                }
            }
            return null;
        }

        [Cfet2Status]
        public Dictionary<string, string> AllInfo()
        {
            return myConfig.RegularMatches;
        }
    }
}
