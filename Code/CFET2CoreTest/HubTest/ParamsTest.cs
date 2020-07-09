using System;
using System.Collections.Generic;
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
        public void ShouldWorkWithJustParams()
        {
            //public string JustParams(params string[] inputs)
            ResourceRequest req1 = new ResourceRequest(@"/params/justparams/a/b/c/d/e", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.ObjectVal.ToString().Should().Be("a:b:c:d:e");
        }

        [TestMethod]
        public void ShouldWorkWithJustParamsAndRoutePArameter()
        {
 
            ResourceRequest req2 = new ResourceRequest(@"/params/justparams/a/b/c/d/e", AccessAction.get, new string[] { "h","i"}, null, null);
            ISample sample2 = MyHub.TryAccessResourceSampleWithUri(req2);
            sample2.ObjectVal.ToString().Should().Be("a:b:c:d:e:h:i");
        }

        [TestMethod]
        public void ShouldWorkWithParamsAndDictInput()
        {
            //public string ParamsAndMore(int a, int b, params string[] inputs)
            ResourceRequest req2 = new ResourceRequest(@"/params/ParamsAndMore/0/1/c/d/e", AccessAction.get, null, null, null);
            ISample sample2 = MyHub.TryAccessResourceSampleWithUri(req2);
            sample2.ObjectVal.ToString().Should().Be("0:1:c:d:e");
        }

    }
}
