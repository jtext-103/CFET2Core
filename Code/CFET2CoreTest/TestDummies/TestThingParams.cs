using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject2.TestDummies
{
    public class TestThingParams:Thing
    {
        [Cfet2Status]
        public string JustParams(params string[] inputs)
        {
            return string.Join(":",inputs);
        }

        [Cfet2Status]
        public string ParamsAndMore(int a,int b, params string[] inputs)
        {
            return string.Join(":", a,b,inputs);
        }
    }
}
