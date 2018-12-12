using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test.TestDummies
{
    class TestAddParametersListThing:Thing
    {
        //status
        [Cfet2Status("ram")]
        public float GetRam()
        {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
            return ramCounter.NextValue();
        }

        [Cfet2Status("defaultStatus")]
        public string GetDefaultStatus()
        {
            return this.Path + @"/defaultStatus";
        }
        [Cfet2Status]
        public int Value(int input) => input * 2;
        [Cfet2Method]
        public void Say(string msg)
        {
            Console.WriteLine("you said: " + msg);
        }
    }
}
