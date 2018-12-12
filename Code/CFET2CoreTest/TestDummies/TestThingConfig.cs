using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test.TestDummies
{
    public class TestThingConfig : Thing
    {
        private int con2 = 2;
        private int con3 = 3;

        [Cfet2Config]
        public int Config1 { get; set; } = 1;


        [Cfet2Config(ConfigActions = ConfigAction.Get)]
        public int Config2
        {
            get
            {
                return con2 * 10;
            }
        }

        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "Config2")]
        public void Config2Set(int val)
        {
            con2 = val;
        }

        [Cfet2Config(ConfigActions = ConfigAction.Set)]
        public int Config3
        {
            set
            {
                con3 = value;
            }
        }

        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "Config3")]
        public int Config3Get()
        {
            return con3 * 10;
        }

    }

    public class TestThingConfigComplex : Thing
    {



        private Dictionary<string, MyPoco> configDict = new Dictionary<string, MyPoco>();
        private Dictionary<string, int> configDict2 = new Dictionary<string, int>();


        [Cfet2Config]
        public int ConfigInt { get; set; } = 1;


        
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "ConfigDict")]
        public void ConfigDictSet(string loc, MyPoco val)
        {
            configDict[loc] = val;
        }

        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "ConfigDict")]
        public MyPoco ConfigDictGet(string loc)
        {
            return configDict[loc];
        }


        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "ConfigDict3")]
        public void ConfigDictSet(string loc1,string loc2, int val)
        {
            configDict2[loc1+loc2] = val;
        }

        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "ConfigDict3")]
        public int ConfigDictGet(string loc1,string loc2)
        {
            return configDict2[loc1+loc2];
        }


    }


    public class MyPoco
    {
        public int MyProperty { get; set; } = 1;
        public int MyProperty2 { get; set; } = 2;
    }


    public class MyPocoIntercect
    {
        public string MyProperty { get; set; } = "10";
        public int MyProperty3 { get; set; } = 20;
    }


}
