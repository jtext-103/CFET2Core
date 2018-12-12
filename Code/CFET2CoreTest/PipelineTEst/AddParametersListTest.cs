using FluentAssertions;
using Jtext103.CFET2.Core.Middleware;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Test.TestDummies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test.PipelineTEst
{
    [TestClass]
    public class AddParametersListTest:CFET2Host
    {
        [TestInitialize]
        public void init()
        {
            HubMaster.InjectHubToModule(this);
            Pipeline myPipeline = new Pipeline();
            ICfet2Middleware miware = new AddParametersList();
            myPipeline.AddMiddleware(miware);
            MyHub.SetPipeline(myPipeline);
            var testThing = new TestAddParametersListThing();
            MyHub.TryAddThing(testThing, @"/", "thing1"); // /thing1
            testThing = new TestAddParametersListThing();
            MyHub.TryAddThing(testThing, @"/", "thing2"); // /thing2
            testThing = new TestAddParametersListThing();
            MyHub.TryAddThing(testThing, @"/thing2/", "thing2"); // /thing2/thing2
        }
        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }

        //[TestMethod]
        //public void AddParametersListShouldBeCorrectly()
        //{
        //    var extrarequest = new Dictionary<string, string> { { typeof(AddParametersList).ToString(), "AddParametersList" } };

        //    ResourceRequest req1 = new ResourceRequest(MyHub, @"/thing1/Value", AccessAction.get, new object[] { }, null, extrarequest);
        //    ISample resource1 = MyHub.TryAccessResourceSampleWithUri(req1);
        //    resource1.Should().NotBeNull();
        //    resource1.IsValid.Should().BeFalse();

        //    string[] inputarray = new string[] { "5"};
        //    ResourceRequest req2 = new ResourceRequest(MyHub, @"/thing1/Say", AccessAction.get, inputarray, null, extrarequest);
        //    ISample resource2 = MyHub.TryAccessResourceSampleWithUri(req2);
        //    resource2.Should().NotBeNull();
        //    resource2.IsValid.Should().BeFalse();
        //    resource2.ObjectVal.Should().BeOfType<ParameterInfo[]>();
        //}
    }
}
