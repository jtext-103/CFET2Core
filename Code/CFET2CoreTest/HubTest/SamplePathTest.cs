using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtext103.CFET2.Core.Test.TestDummies;
using FluentAssertions;

namespace Jtext103.CFET2.Core.Test.HubTest
{
    /// <summary>
    /// Summary description for SamplePathTest
    /// </summary>
    [TestClass]
    public class SamplePathTest:CFET2Host
    {
       

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
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }


        [TestMethod]
        public void SamplePathShouldMatch()
        {
            //arrange
            //done in init

            //act                    
            var reuest = MyHub.TryGetResourceSampleWithUri(@"cfet://sdfsdf:99/thing1/StatusP");
            var reuest2 = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/status2para/2/b"); //test case sensitive
            var reuest2f = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/2/b/");
            var reuest2WithInputs = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para", 2, "b");
            var reuest2WithLastInput = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/2", "b");

            var reuestT4 = MyHub.TryGetResourceSampleWithUri(@"cfet://dddd.com/thing2/thing3/thing4/StatusM");
            var reuestT41 = MyHub.TryGetResourceSampleWithUri(@"cfet://dddd.com/thing2/thing3/thing4/StatusM1", 10);
            var reuestT411 = MyHub.TryGetResourceSampleWithUri(@"cfet://dddd.com/thing2/thing3/thing4/StatusM1/10");

            var reuest2WithQuery = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=2&n2=b");
            var reuest2WithQueryAndLast = MyHub.TryGetResourceSampleWithUri(@"/thing2/thIng2/status2para?n=2", "b"); //test case sensitive
            var reuest2WithQueryAndRoutePara = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=3&n2=c", "2", "b");
            var reuest2WithQueryAndRouteParaIgnore = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=2&n3=c", "b");   //excessive but ignored

            //Assert
            reuest.Path.Should().BeEquivalentTo(@"/thing1/StatusP");
            reuest2.Path.Should().BeEquivalentTo(@"/thing2/thing2/status2para");
            reuest2f.Path.Should().BeEquivalentTo(@"/thing2/thing2/Status2Para");
            reuest2WithInputs.Path.Should().BeEquivalentTo(@"/thing2/thing2/Status2Para");

            reuestT4.Path.Should().BeEquivalentTo(@"/thing2/thing3/thing4/StatusM");
            reuestT41.Path.Should().BeEquivalentTo(@"/thing2/thing3/thing4/StatusM1");
            reuestT411.Path.Should().BeEquivalentTo(@"/thing2/thing3/thing4/StatusM1");

            reuest2WithQueryAndLast.Path.Should().BeEquivalentTo(@"/thing2/thing2/Status2Para");


        }

        [TestMethod]
        public void InvalidSamplePathShouldMatch()
        {
            //arrange
            //done in init
            
            var reuest2InvSample = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para", new Dictionary<string, object> { ["n2"] = "b" }); //invalide sample
            //Assert
            reuest2InvSample.Path.Should().BeEquivalentTo(@"/thing2/thing2/Status2Para");
        }

        [TestMethod]
        public void SamplePathShouldMatchDictInputs()
        {
            //arrange
            //done in init

            //act                    
            var reuest2Pure = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para", new Dictionary<string, object> { ["n"] = 2, ["n2"] = "b" });
            var reuest2Over = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=3&n2=c", new Dictionary<string, object> { ["n"] = 2, ["n2"] = "b" });
            var reuest2Mix = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=2", new Dictionary<string, object> { ["n2"] = "b" });

            reuest2Pure.Path.Should().BeEquivalentTo(@"/thing2/thing2/Status2Para");
            reuest2Over.Path.Should().BeEquivalentTo(@"/thing2/thing2/Status2Para");
            reuest2Mix.Path.Should().BeEquivalentTo(@"/thing2/thing2/Status2Para");

        }

    }
}
