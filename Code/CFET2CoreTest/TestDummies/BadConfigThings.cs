using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// bad in various ways
/// </summary>
namespace Jtext103.CFET2.Core.Test.TestDummies
{
    /// <summary>
    /// bad: config has the same name as status
    /// </summary>
    public class TestBadThingConfig1 : Thing
    {

        [Cfet2Config]
        public int Config1 { get; set; } = 1;

        [Cfet2Config(Name = "Config1")]
        public int Stauts { get; set; } = 1;

    }



    /// <summary>
    /// bad: multiple get implemation
    /// </summary>
    public class TestBadThingConfig2 : Thing
    {
        private int con2 = 2;
 

        [Cfet2Config]
        public int Config1 { get; set; } = 1;


        [Cfet2Config(Name = "Config1", ConfigActions = ConfigAction.Get)]
        public int Config2
        {
            get
            {
                return con2 * 10;
            }
        }

    }


    /// <summary>
    /// bad: multiple set implemation
    /// </summary>
    public class TestBadThingConfig3 : Thing
    {
        private int con2 = 2;

        [Cfet2Config]
        public int Config1 { get; set; } = 1;


        [Cfet2Config(Name = "Config1", ConfigActions = ConfigAction.Set)]
        public int Config2()
        {
            return con2 * 10;
        }

    }


    /// <summary>
    /// bad: multiple set implemation
    /// </summary>
    public class TestBadThingConfig4 : Thing
    {
        [Cfet2Config(Name = "Config1", ConfigActions = ConfigAction.Set)]
        public int Config1()
        {
            return 10;
        }


        [Cfet2Config(Name = "Config1", ConfigActions = ConfigAction.Set)]
        public int Config2
        {
            get
            {
                return 10;
            }
        }

    }



    /// <summary>
    /// bad: missing set
    /// </summary>
    public class TestBadThingConfig5 : Thing
    {


        [Cfet2Config(ConfigActions = ConfigAction.Get)]
        public int Config1 { get; } = 1;

    }


    /// <summary>
    /// bad: missing get
    /// </summary>
    public class TestBadThingConfig6 : Thing
    {

        [Cfet2Config(Name = "Config1", ConfigActions = ConfigAction.Set)]
        public int Config2()
        {
            return 10;
        }

    }

    /// <summary>
    /// bad: missing set
    /// </summary>
    public class TestBadThingConfig7 : Thing
    {


        [Cfet2Config]
        public int Config1 { get; } = 1;

    }
}
