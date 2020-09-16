using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtext103.CFET2.Core.Test.TestDummies;
using Jtext103.CFET2.Core.Resource;
using FluentAssertions;
using Jtext103.CFET2.Core.Sample;
using Microsoft.CSharp;

namespace CFET2CoreTest
{
    /// <summary>
    /// test basic probing and basic resource accessing of a status
    /// </summary>
    [TestClass]
    public class ResouceProbingStatus
    {
        [TestMethod]
        public void BasciProbingStatus()
        {
            //aeegange
            var testThing = new TestThingStatus();


            //act
            var thing = new ResourceThing(testThing, "thIng",null);

            //assert
            thing.Resources.Should().ContainKeys(new string[] { "StatusP", "statusm" , "statusM1" }).And.NotContainKeys("Statusm2").And.HaveCount(4);

        }


        /// <summary>
        /// just get the status using resouces from resource thing directly
        /// </summary>
        [TestMethod]
        public void BareboneStatusGetTest()
        {
            //arange
            var testThing = new TestThingStatus();


            //act
            var thing = new ResourceThing(testThing, "thing",null);
            var statusP = thing.Resources["StatusP"] as ResourceStatus;
            var statusM = thing.Resources["StatusM"] as ResourceStatus;
            var statusM1 = thing.Resources["StatusM1"] as ResourceStatus;

            dynamic sP = statusP.Get();
            dynamic sM = statusM.Get();
            //[obsolete:this status does not take parameter but feeding it one should not course any problem , todo add event rasing for this minor error]
            //now wrong parameters will result in a invalid sample
            var sMPIvalid = statusM.Get(1) as ISample;
            dynamic sM1 = statusM1.Get(1);


            //assert
            Assert.AreEqual(typeof(Status<int>), sP.GetType());
            Assert.AreEqual(typeof(Status<string>), sM.GetType());
            Assert.AreEqual(typeof(Status<string>), sM1.GetType());

            Assert.AreEqual(10, sP.Val);
            Assert.AreEqual("10", sP.Val.ToString());
            Assert.AreEqual("Nothing!", sM.Val);
            Assert.AreEqual(false, sMPIvalid.IsValid);
            Assert.AreEqual("1", sM1.Val);

        }


        [TestMethod]
        public void SampleCastingStatus()
        {
            //aeegange
            var testThing = new TestThingStatus();


            //act
            var thing = new ResourceThing(testThing, "thing",null);
            //var statusP = thing.Resources["StatusP"] as ResourceStatus;

            var statusM1 = thing.Resources["StatusM1"] as ResourceStatus;

            //ISample sP = statusP.Get() as ISample;
            ISample sM1 = statusM1.Get(1) as ISample;
            dynamic sM1D = sM1;

            //no cast

            //Value cast
            //var sM1Int = sM1.CastValue<int>();
            //Sample Cast. todo cast to another kind of sample
            var sM1IntSample = new Status<int>(sM1.Context);
            //get value original sample
            var sM1ValInt = sM1.GetVal<int>();
            var sM1ValObj = sM1.ObjectVal;  //type of string
            object sM1Val = sM1D.Val;
            //get value casted sample
            //var sM1IntVal = sM1Int.Val;
            var sM1IntSampleVal = sM1IntSample.Val;


            //assert
            //no cast
            sM1.Should().BeOfType(typeof(Status<string>));
            //value cast
            //sM1Int.Should().BeOfType(typeof(Status<int>));
            //sample cast
            sM1IntSample.Should().BeOfType(typeof(Status<int>));
            //test original value
            sM1ValInt.Should().Be(1);
            sM1ValObj.Should().Be("1").And.BeOfType<string>();
            sM1Val.Should().Be("1").And.BeOfType<string>();
            //test casted value
            //sM1IntVal.Should().Be(1);
            sM1IntSampleVal.Should().Be(1);

        }


        [TestMethod]
        public void StatusWithMultipleParas()
        {
            //aeegange
            var testThing = new TestThingStatus();


            //act
            var thing = new ResourceThing(testThing, "thing",null);
            //var statusP = thing.Resources["StatusP"] as ResourceStatus;

            var status2Para = thing.Resources["Status2Para"] as ResourceStatus;

            //ISample sP = statusP.Get() as ISample;
            //todo try not correct NO. and type of paras
            ISample sM1 = status2Para.Get(1,"1") as ISample;
            dynamic sM1D = sM1;

            //no cast

            //Value cast
            //var sM1Int = sM1.CastValue<int>();
            //Sample Cast. todo cast to another kind of sample
            var sM1IntSample = new Status<int>(sM1.Context);
            //get value original sample
            var sM1ValInt = sM1.GetVal<int>();
            var sM1ValObj = sM1.ObjectVal;  //type of string
            object sM1Val = sM1D.Val;
            //get value casted sample
            //var sM1IntVal = sM1Int.Val;
            var sM1IntSampleVal = sM1IntSample.Val;


            //assert
            //no cast
            sM1.Should().BeOfType(typeof(Status<string>));
            //value cast
            //sM1Int.Should().BeOfType(typeof(Status<int>));
            //sample cast
            sM1IntSample.Should().BeOfType(typeof(Status<int>));
            //test original value
            sM1ValInt.Should().Be(11);
            sM1ValObj.Should().Be("11").And.BeOfType<string>();
            sM1Val.Should().Be("11").And.BeOfType<string>();
            //test casted value
            //sM1IntVal.Should().Be(11);
            sM1IntSampleVal.Should().Be(11);

        }



    }
}
