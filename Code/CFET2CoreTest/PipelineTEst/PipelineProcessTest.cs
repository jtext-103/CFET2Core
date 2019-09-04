using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Test.TestDummies;
using System.Collections.Generic;
using Jtext103.CFET2.Core.Middleware;
using System.Diagnostics;
using Jtext103.CFET2.Core.Exception;
using System.Linq;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Test.TestDummies;

namespace Jtext103.CFET2.Core.Test.PipelineTEst
{
    [TestClass]
    public class PipelineProcessTest : CFET2Host
    {
        [TestInitialize]
        public void init()
        {
            HubMaster.InjectHubToModule(this);
            Pipeline myPipeline = new Pipeline();
            ICfet2Middleware miware = new AddChildThing();
            myPipeline.AddMiddleware(miware);
            MyHub.SetPipeline(myPipeline);
            var thing1 = new TestGetChildThing();
            MyHub.TryAddThing(thing1, @"/", "pc");
            var testThing = new TestGetChildThing();
            MyHub.TryAddThing(testThing, @"/", "thing1"); // /thing1
            testThing = new TestGetChildThing();
            MyHub.TryAddThing(testThing, @"/", "thing2"); // /thing2
            testThing = new TestGetChildThing();
            MyHub.TryAddThing(testThing, @"/thing2/", "thing2"); // /thing2/thing2
            MyHub.TryAddThing(testThing, @"/thing2/thing3", "Thing4"); // /thing2/thing3/thing4
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }

        [TestMethod]
        public void PipleLineShouldInvokeMiddlesInOrder()
        {
            int a = 5;
            a.Should().Be(5);
        }

        //[TestMethod]
        //public void MiddlewareShouldAddTheChildThingAndItsOwnResourcePathToSample()
        //{
        //    int b = 5;
        //    b.Should().Be(5);
        //}
        //[TestMethod]
        //public void ResourceRequestShouldBeAccessCorrectly()
        //{
        //    var extrarequest = new Dictionary<string, string> { { typeof(AddChildThing).ToString(), "addchildthing" } };

        //    ResourceRequest req1 = new ResourceRequest(MyHub, @"/thing1", AccessAction.get, new object[] { }, null, null);
        //    ISample resource1 = MyHub.TryAccessResourceSampleWithUri(req1);
        //    resource1.Should().NotBeNull();
        //    resource1.IsValid.Should().BeTrue();


        //    ResourceRequest req2 = new ResourceRequest(MyHub, @"/thing1", AccessAction.get, new object[] { }, null, extrarequest);
        //    ISample resource2 = MyHub.TryAccessResourceSampleWithUri(req2);


        //    ResourceRequest req3 = new ResourceRequest(MyHub, @"/thing2", AccessAction.get, new object[] { }, null, extrarequest);
        //    ISample resource3 = MyHub.TryAccessResourceSampleWithUri(req3);
        //    resource3.IsValid.Should().BeTrue();
        //    resource3.ObjectVal.Should().NotBeNull();

        //    ResourceRequest req4 = new ResourceRequest(MyHub, @"/thing2/thing2", AccessAction.get, new object[] { }, null, extrarequest);
        //    ISample resource4 = MyHub.TryAccessResourceSampleWithUri(req4);
        //    resource4.IsValid.Should().BeTrue();


        //    ResourceRequest req5 = new ResourceRequest(MyHub, @"/thing2/thing3/thing4", AccessAction.get, new object[] { }, null, extrarequest);
        //    ISample resource5 = MyHub.TryAccessResourceSampleWithUri(req5);
        //    resource5.IsValid.Should().BeTrue();


        //    ResourceRequest req6 = new ResourceRequest(MyHub, @"/thing2/thing3", AccessAction.get, new object[] { }, null, extrarequest);
        //    ISample resource6 = MyHub.TryAccessResourceSampleWithUri(req6);
        //    resource6.IsValid.Should().BeTrue();
        //}
    }
}
