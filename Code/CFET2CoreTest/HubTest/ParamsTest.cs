using System;
using FluentAssertions;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Sample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject2.TestDummies;

namespace Jtext103.CFET2.Core.Test.HubTest
{
    [TestClass]
    public class ParamsTest: CFET2Host
    {
        [TestInitialize]
        public void init()
        {
            HubMaster.InjectHubToModule(this);
            MyHub.TryAddThing(new TestThingParams(), @"/", "params");
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }

        [TestMethod]
        public void ShouldWorkWithJustParams_todo()
        {
            ResourceRequest req1 = new ResourceRequest(@"/params/justparams/a/b/c/d/e", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.ObjectVal.ToString().Should().Be("a:b:c:d:e");
        }
    }
}
