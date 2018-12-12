using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtext103.CFET2.Core.Test.TestDummies;
using FluentAssertions;

namespace Jtext103.CFET2.Core.Test.HubTest
{
    [TestClass]
    public class DisposeTest:CFET2Host
    {
        [TestInitialize]
        public void init()
        {

            HubMaster.InjectHubToModule(this);
           
            var testThing = new DisposibleThing();
            MyHub.TryAddThing(testThing, @"/", "thing1"); // /thing1
            MyHub.StartThings();
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }


        [TestMethod]
        public void ThingShouldDispose()
        {
            //arrange 
            //act
            MyHub.DisposeThings();
            //assert
            DisposibleThing.disposeCount.Should().Be(1);
        }
    }
}
