using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Attributes;

namespace Jtext103.CFET2.Core.Test.TestDummies
{
    public class TestThingStatus:Thing
    {
        /// <summary>
        ///  = 10
        /// </summary>
        [Cfet2Status(Name ="StatusP")]
        public int StatusP { get; set; } = 10;

        

        /// <summary>
        /// [Cfet2Status(Name = "StatusM")]  
        /// return ("Nothing!").ToStatus();
        /// </summary>
        /// <returns></returns>
        [Cfet2Status(Name = "StatusM")]
        public ISample StatusM1()
        {
            return ("Nothing!").ToStatus();
        }

        /// <summary>
        /// return n.ToString();
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [Cfet2Status]
        public string StatusM1(int n)
        {
            return n.ToString();
        }

        /// <summary>
        /// return n.ToString()+n2;
        /// </summary>
        /// <param name="n"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        [Cfet2Status]
        public string Status2Para(int n,string n2)
        {
            return n.ToString()+n2;
        }



        /// <summary>
        /// shoild not be probed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Status<string> StatusM2(int n)
        {
            return new Status<string>(n.ToString());
        }

    }

    public class TestBadThingStatus : Thing
    {
        [Cfet2Status(Name = "StatusP")]
        public Status<int> StatusP { get; set; } = new Status<int>(10);


        /// <summary>
        /// multiple implementation
        /// </summary>
        /// <returns></returns>
        [Cfet2Status]
        public Status<string> StatusM1()
        {
            return new Status<string>("Nothing!");
        }

        [Cfet2Status]
        public Status<string> StatusM1(int n)
        {
            return new Status<string>(n.ToString() + " is Nothing!");
        }

        /// <summary>
        /// shoild not be probed
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Status<string> StatusM2(int n)
        {
            return new Status<string>(n.ToString() + " is Nothing!");
        }
    }


}
