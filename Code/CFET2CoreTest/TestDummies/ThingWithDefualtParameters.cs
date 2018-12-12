using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test.TestDummies
{

    public class ThingWithDefualtParameters : Thing
    {
        public static   PocoParam MyPoco = new PocoParam { MyPropString="ASS",MyPropInt=-1};

        Dictionary<int, PocoParam> dictionary = new Dictionary<int, PocoParam> ();
        

        [Cfet2Status]
        public string StatusWithDefualtParams(int i= 0,  string s= "+", PocoParam o= null)
        {
            return i.ToString() + s.ToString() + (o ?? MyPoco).ToString();
        }

        [Cfet2Method]
        public string MethodWithDefualtParams(int i = 0, string s = "+", PocoParam o = null)
        {
            return i.ToString() + s.ToString() + (o ?? MyPoco).ToString();
        }

        [Cfet2Config(ConfigActions = ConfigAction.Set)]
        public void ConfigWithDefualtParams(int i = 0, PocoParam o = null)
        {
            dictionary[i] = (o ?? MyPoco);
        }

        [Cfet2Config(ConfigActions = ConfigAction.Get)]
        public PocoParam ConfigWithDefualtParams(int i = 0)
        {
            return dictionary[i];
        }
    }

    public class PocoParam
    {
        public string MyPropString { get; set; } = "Hehe";
        public int MyPropInt { get; set; } = 6;

        public override string ToString()
        {
            return MyPropString + MyPropInt.ToString();
        }
    }
}
