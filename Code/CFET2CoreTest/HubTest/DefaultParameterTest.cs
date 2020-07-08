using FluentAssertions;
using Jtext103.CFET2.Core.Test.TestDummies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test.HubTest
{
    [TestClass]
    public class DefaultParameterTest:CFET2Host
    {
        [TestInitialize]
        public void init()
        {

            HubMaster.InjectHubToModule(this);
            var testThing = new ThingWithDefualtParameters();
            MyHub.TryAddThing(testThing, @"/", "thing"); // /thing2/thing2

        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }

        [TestMethod]
        public void ShouldUseDefaultParams()
        {
            //arrange
            var resultSt = MyHub.TryGetResourceSampleWithUri(@"/thing/StatusWithDefualtParams"); //"0+ASS-1"
            var resultSt2 = MyHub.TryGetResourceSampleWithUri(@"/thing/StatusWithDefualtParams/1"); //1+ASS-1
            var resultSt3 = MyHub.TryGetResourceSampleWithUri(@"/thing/StatusWithDefualtParams",1,"2"); //12ASS-1

            var resultMethod = MyHub.TryInvokeSampleResourceWithUri(@"/thing/MethodWithDefualtParams"); //"0+ASS-1"

            var resultConSet = MyHub.TrySetResourceSampleWithUri(@"/thing/ConfigWithDefualtParams"); //"0+ASS-1"
            var resultCon = MyHub.TryGetResourceSampleWithUri(@"/thing/ConfigWithDefualtParams"); //"0+ASS-1"

            //assert
            resultSt.ObjectVal.Should().Be("0+ASS-1");
            resultSt2.ObjectVal.Should().Be("1+ASS-1");
            resultSt3.ObjectVal.Should().Be("12ASS-1");

            resultMethod.ObjectVal.Should().Be("0+ASS-1");

            resultConSet.ObjectVal.Should().Be(ThingWithDefualtParameters.MyPoco);
            resultCon.ObjectVal.Should().Be(ThingWithDefualtParameters.MyPoco);
        }

        [TestMethod]
        public void ShouldUseDefaultParamsWithQueryString()
        {
            //this will fail!!! 
            //arrange
            //StatusWithDefualtParams(int i= 0,  string s= "+", PocoParam o= null)
            var resultSt = MyHub.TryGetResourceSampleWithUri(@"/thing/StatusWithDefualtParams?s=a"); //"0#ASS-1"
            var resultSt2 = MyHub.TryGetResourceSampleWithUri(@"/thing/StatusWithDefualtParams",new Dictionary<string, object>(){ { "s", "#"} }); //"0#ASS-1"


            //assert
            resultSt.ObjectVal.Should().Be("0aASS-1");
            resultSt2.ObjectVal.Should().Be("0#ASS-1");
        }
    }
}
