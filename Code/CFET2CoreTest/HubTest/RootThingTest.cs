using System;
using FluentAssertions;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Test.TestDummies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jtext103.CFET2.Core.Test.HubTest
{
    [TestClass]
    public class RootThingTest:CFET2Host
    {
        /// <summary>
        /// Summary description for SamplePathTest
        /// </summary>
        

        [TestInitialize]
        public void init()
        {

            HubMaster.InjectHubToModule(this);

            var testThing = new TestThingStatus();
            MyHub.TryAddThing(testThing, @"/", "thing1"); // /thing1
            testThing = new TestThingStatus();
            MyHub.TryAddThing(testThing, @"/", "thing2"); // /thing2
            testThing = new TestThingStatus();
            MyHub.TryAddThing(testThing, @"/thing2/", "thing2"); // /thing2/thing2
            MyHub.TryAddThing(testThing, @"/thing2/thing3", "Thing4"); // /thing2/thing3/thing4
            MyHub.StartThings();
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }


        [TestMethod]
        public void ShouldFindRoot()
        {
            ResourceRequest req1 = new ResourceRequest(@"/", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.ResourceType.Should().Be(ResourceTypes.Thing);
            sample.Path.Should().Be("/");

            req1 = new ResourceRequest(@"/hostname", AccessAction.get, null, null, null);
            sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.ResourceType.Should().Be(ResourceTypes.Config);
            sample.Path.ToLower().Should().Be("/hostname");
            sample.ObjectVal.Should().Be("host");
        }
    }
}
