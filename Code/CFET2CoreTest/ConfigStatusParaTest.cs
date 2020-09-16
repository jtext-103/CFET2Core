using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jtext103.CFET2.Core.Test.TestDummies;
using Jtext103.CFET2.Core.Resource;
using Jtext103.CFET2.Core.Sample;
using FluentAssertions;
using Jtext103.CFET2.Core.Exception;
using System.Collections.Generic;

namespace Jtext103.CFET2.Core.Test
{
    /// <summary>
    /// test the config with different kinds of parameters and more
    /// </summary>
    [TestClass]
    public class ConfigStatusComplexGetSetTest
    {

        /// <summary>
        /// test the config with different kinds of parameters
        /// </summary>
        [TestMethod]
        public void ConfigParaTest()
        {
            //aeegange
            var testThing = new TestThingConfigComplex();
            var thing = new ResourceThing(testThing, "thing",null);
            var ConfigInt = thing.Resources["ConfigInt"] as ResourceConfig;
            var ConfigDict = thing.Resources["ConfigDict"] as ResourceConfig;


            //act

            //act
            //property get set
            var ConfigIntS = ConfigInt.Set("10");
            //property get method set
            var ConfigDictS = ConfigDict.Set(20, new MyPocoIntercect()); //expect string,MyPoco

            var inputs = new object[] { "30", new MyPocoIntercect() { MyProperty = "5" } };
            var ConfigDictSUsingArrary = ConfigDict.Set(inputs); //expect string,MyPoco arrary as input


            var ConfigDictSGot = ConfigDict.Get("20");


            //assert todo test sample are equal
            ConfigIntS.As<ISample>().ObjectVal.Should().Be(10);
            ConfigDictS.As<ISample>().ObjectVal.As<MyPoco>().MyProperty.Should().Be(10);
            ConfigDictSUsingArrary.As<ISample>().ObjectVal.As<MyPoco>().MyProperty.Should().Be(5);

            ConfigDictSGot.As<ISample>().ObjectVal.As<MyPoco>().MyProperty.Should().Be(10);
        }


        [TestMethod]
        public void Config3ParaDictTest()
        {
            //aeegange
            var testThing = new TestThingConfigComplex();
            var thing = new ResourceThing(testThing, "thing",null);
            var ConfigDict3 = thing.Resources["ConfigDict3"] as ResourceConfig;


            //act

            //property get method set
            var ConfigDict3S = ConfigDict3.Set(20,20, 40); //set 40

            var inputs = new Dictionary<string, object>
            {
                ["loc2"] = 20,
                ["val"] = 30,
                ["loc1"] = 10
            };
            var ConfigDict3SDict = ConfigDict3.Set(inputs); //set using dict 30

            inputs = new Dictionary<string, object>
            {
                ["loc2"] = 20,
                [CommonConstants.TheLastInputsKey] = 35,
                ["loc1"] = 15
            };
            var ConfigDict3SDictLast = ConfigDict3.Set(inputs); //set using dict, with TheLastInputsKey 35

            inputs = new Dictionary<string, object>
            {
                ["loc22"] = 20,
                [CommonConstants.TheLastInputsKey] = 35,
                ["loc1"] = 15
            };
            var ConfigDict3SDictError = ConfigDict3.Set(inputs); //set using dict, with wrong input, invalid

            inputs = new Dictionary<string, object>
            {
                ["loc22"] = 20,
                ["loc2"] = 20,
                [CommonConstants.TheLastInputsKey] = 32,
                ["loc1"] = 12
            };
            var ConfigDict3SDictError2 = ConfigDict3.Set(inputs); //set using dict, with wrong input, invalid? no, it's valid, parameter in the dictionary not matching tha parameter name in the method will be dropped


            //get
            inputs = new Dictionary<string, object>
            {
                ["loc2"] = 20,
                ["loc1"] = 10
            };
            var ConfigDict3SGotDict = ConfigDict3.Get(inputs); //get 30
            var ConfigDict3SGot = ConfigDict3.Get("10","20");   //get should be the same as above 30




            //assert todo test sample are equal
            ConfigDict3S.As<ISample>().ObjectVal.Should().Be(40);
            ConfigDict3SDict.As<ISample>().ObjectVal.Should().Be(30);
            ConfigDict3SDictLast.As<ISample>().ObjectVal.Should().Be(35);
            ConfigDict3SDictError.As<ISample>().IsValid.Should().Be(false); //set failure
            ConfigDict3SDictError2.As<ISample>().ObjectVal.Should().Be(32);

            ConfigDict3SGotDict.As<ISample>().ObjectVal.Should().Be(30);
            ConfigDict3SGot.As<ISample>().ObjectVal.Should().Be(30);
        }


        [TestMethod]
        public void BadRequestTest()
        {
            //aeegange
            var testThing = new TestThingConfig();
            var thing = new ResourceThing(testThing, "thing",null);
            var config1 = thing.Resources["Config1"] as ResourceConfig;
            var config2 = thing.Resources["Config2"] as ResourceConfig;

            var testThingStatus = new TestThingStatus();
            var thingStatus = new ResourceThing(testThingStatus, "thing",null);
            var status2Para = thingStatus.Resources["Status2Para"] as ResourceStatus;
            var statusM = thingStatus.Resources["StatusM"] as ResourceStatus;

            //act

            //act
            //property get set
            var con1s = config1.Set(10,10); //set error
            //property get method set
            var con2s = config2.Set(20,10);
            //get
            var con1 = config1.Get();
            var con2 = config2.Get(10);

            //Status get
            var status2ParaS = status2Para.Get();
            var StatusMS = statusM.Get(10);


            //assert todo test sample are equal
            con1s.As<ISample>().IsValid.Should().Be(false); //set failure
            con1s.As<ISample>().ErrorMessages.Should().BeEquivalentTo(new string [] { BadResourceRequestException.DefualtMessage, "Should not have more than 1 parameters!"});
            con2s.As<ISample>().IsValid.Should().Be(false);
            con2s.As<ISample>().ErrorMessages.Should().BeEquivalentTo(new string[] { BadResourceRequestException.DefualtMessage,  "Should not have any parameters!"});

            con1.As<ISample>().ObjectVal.Should().Be(1);
            con2.As<ISample>().IsValid.Should().Be(false);  //get failure
            con2.As<ISample>().ErrorMessages.Should().BeEquivalentTo(new string[] { BadResourceRequestException.DefualtMessage, "Should not have any parameters!" });

            status2ParaS.As<ISample>().IsValid.Should().Be(false); //get failure
            status2ParaS.As<ISample>().ErrorMessages.Should().Contain(BadResourceRequestException.DefualtMessage);
            StatusMS.As<ISample>().IsValid.Should().Be(false); //get failure
            StatusMS.As<ISample>().ErrorMessages.Should().Contain(BadResourceRequestException.DefualtMessage);

        }
    }
}
