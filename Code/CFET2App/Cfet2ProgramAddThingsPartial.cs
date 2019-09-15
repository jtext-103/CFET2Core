using Jtext103.CFET2.CFET2App.ExampleThings;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.NancyHttpCommunicationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.CFET2App
{
    public partial class Cfet2Program : CFET2Host
    {
        private void AddThings()
        {
            //If you don't want dynamic load things, please comment out the line below
            //var loader = new DynamicThingsLoader(this);

            //you can add Thing by coding here

            //nancy HTTP
            var nancyCM = new NancyCommunicationModule(new Uri("http://localhost:9001"));
            MyHub.TryAddCommunicationModule(nancyCM);

            var fakeAI = new FakeAIThing();
            MyHub.TryAddThing(fakeAI, "/", "fakeCard", 16);
        }
    }
}
