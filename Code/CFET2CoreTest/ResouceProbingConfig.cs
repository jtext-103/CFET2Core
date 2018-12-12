using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtext103.CFET2.Core.Test.TestDummies;
using Jtext103.CFET2.Core.Resource;
using FluentAssertions;
using Jtext103.CFET2.Core.Sample;
using Microsoft.CSharp;
using Jtext103.CFET2.Core;
using System.Collections.Generic;
using Jtext103.CFET2.Core.Exception;

namespace CFET2CoreTest
{
    /// <summary>
    /// test basic probing and basic resource accessing of a status
    /// </summary>
    [TestClass]
    public class ResouceProbingConfig
    {
        [TestMethod]
        public void BasciProbingConfig()
        {
            //aeegange
            var testThing = new TestThingConfig();


            //act
            var thing = new ResourceThing(testThing, "thing");

            //assert
            thing.Resources.Should().ContainKeys(new string[] { "Config1", "Config2" , "Config3" }).And.HaveCount(3);

        }

        [TestMethod]
        public void ProbingBadThingConfig()
        {
            //aeegange
            List<Thing> things = new List<Thing>();
            things.Add( new TestBadThingConfig1());
            things.Add( new TestBadThingConfig2());
            things.Add( new TestBadThingConfig3());
            things.Add(  new TestBadThingConfig4());
            things.Add( new TestBadThingConfig5());
            things.Add(new TestBadThingConfig6());
            things.Add(new TestBadThingConfig7());

            int total = 7;
            int n = 0;
            //act
            Action[] acts = new Action[total];
            for (int i = 0;i< total; i++)
            {
                acts[i] = () => new ResourceThing(things[n], "thing"+n);
            }

            //assert
            for (n = 0; n < total; n++)
            {
                acts[n].ShouldThrow<BadThingImplementaionException>("because thing "+(n+1)+" should fail");
                
            }
        }


        /// <summary>
        /// just get the config using resouces from resource thing directly
        /// </summary>
        [TestMethod]
        public void BareboneConfigGetTest()
        {
            //aeegange
            var testThing = new TestThingConfig();
            var thing = new ResourceThing(testThing, "thing");
            var config1 = thing.Resources["Config1"] as ResourceConfig;
            var config2 = thing.Resources["Config2"] as ResourceConfig;
            var config3 = thing.Resources["Config3"] as ResourceConfig;

            //act
            //property get set
            var con1 = config1.Get();
            //property get method set
            var con2 = config2.Get();
            //property set method set
            var con3 = config3.Get();


            //assert
            Assert.AreEqual(typeof(Config<int>), con1.GetType());
            Assert.AreEqual(typeof(Config<int>), con2.GetType());
            Assert.AreEqual(typeof(Config<int>), con3.GetType());

            con1.As<ISample>().ObjectVal.Should().Be(1);
            //missing test of sample.tostring
            con1.ToString().Should().Be("1");
            con2.As<ISample>().ObjectVal.Should().Be(20);
            con3.As<ISample>().ObjectVal.Should().Be(30);


        }

        /// <summary>
        /// try set the thing using different setter
        /// </summary>
        [TestMethod]
        public void BareboneConfigSetTest()
        {
            //aeegange
            var testThing = new TestThingConfig();
            var thing = new ResourceThing(testThing, "thing");
            var config1 = thing.Resources["Config1"] as ResourceConfig;
            var config2 = thing.Resources["Config2"] as ResourceConfig;
            var config3 = thing.Resources["Config3"] as ResourceConfig;

            //act

            //act
            //property get set
            var con1s = config1.Set(10);
            //property get method set
            var con2s = config2.Set(20);
            //property set method set
            var con3s = config3.Set(30);



            var con1 = config1.Get();
            var con2 = config2.Get();
            var con3 = config3.Get();


            //assert todo test sample are equal
            con1.As<ISample>().ObjectVal.Should().Be(10);
            con2.As<ISample>().ObjectVal.Should().Be(200);
            con3.As<ISample>().ObjectVal.Should().Be(300);

            con1s.As<ISample>().ObjectVal.Should().Be(10);
            con2s.As<ISample>().ObjectVal.Should().Be(200);
            con3s.As<ISample>().ObjectVal.Should().Be(300);


        }

    }
}
