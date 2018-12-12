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
using System.Linq;

namespace CFET2CoreTest
{
    /// <summary>
    /// test basic probing and basic resource accessing of a status
    /// </summary>
    [TestClass]
    public class HubGetTest:CFET2Host
    {


        [TestInitialize]
        public void init()
        {

            HubMaster.InjectHubToModule(this);
            var comm = new DummyComm();
            MyHub.TryAddCommunicationModule(comm);
            var testThing = new TestThingStatus();
            MyHub.TryAddThing(testThing, @"/", "thing1"); // /thing1
            testThing = new TestThingStatus();
            MyHub.TryAddThing(testThing, @"/", "thing2"); // /thing2
            testThing = new TestThingStatus();
            MyHub.TryAddThing(testThing, @"/thing2/", "thing2"); // /thing2/thing2
            MyHub.TryAddThing(testThing, @"/thing2/thing3", "Thing4"); // /thing2/thing3/thing4
            MyHub.StartCommunication();
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }


        [TestMethod]
        public void HubUriGetTest()
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
            var reuestT41 = MyHub.TryGetResourceSampleWithUri(@"cfet://dddd.com/thing2/thing3/thing4/StatusM1",10);
            var reuestT411 = MyHub.TryGetResourceSampleWithUri(@"cfet://dddd.com/thing2/thing3/thing4/StatusM1/10");

            var reuest2WithQuery = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=2&n2=b");
            var reuest2WithQueryAndLast = MyHub.TryGetResourceSampleWithUri(@"/thing2/thIng2/status2para?n=3",2, "b"); //test case sensitive
            var reuest2WithQueryAndRoutePara = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=3&n2=c", "2", "b");
            var reuest2WithQueryAndRouteParaQueryIsIgnored = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=2&n3=c", "b");   //input array override the querystring

            //Assert
            reuest.ObjectVal.Should().Be(10);
            reuest2.ObjectVal.Should().Be("2b");
            reuest2f.ObjectVal.Should().Be("2b");
            reuest2WithInputs.ObjectVal.Should().Be("2b");
            reuest2WithLastInput.ObjectVal.Should().Be("2b");

            reuestT4.ObjectVal.Should().Be("Nothing!");
            reuestT41.ObjectVal.Should().Be("10");
            reuestT411.ObjectVal.Should().Be("10");

            reuest2WithQuery.ObjectVal.Should().Be("2b");
            reuest2WithQueryAndLast.ObjectVal.Should().Be("2b");
            reuest2WithQueryAndRoutePara.ObjectVal.Should().Be("2b");

            reuest2WithQueryAndRouteParaQueryIsIgnored.IsValid.Should().BeFalse();


        }

        [TestMethod]
        public void HubUriDictGetTest()
        {
            //arrange
            //done in init

            //act                    
            var reuest2Pure = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para", new Dictionary<string, object> { ["n"] = 2, ["n2"] = "b" });
            var reuest2Over = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=3&n2=c", new Dictionary<string, object> { ["n"] = 2, ["n2"] = "b" });
            var reuest2Mix = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=2", new Dictionary<string, object> { ["n2"] = "b" });

           
            var jointRouteParams =MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/2/b", new Dictionary<string, object> { ["n"] = "5" }); //"/2b" as last input
            var jointRouteParamsQuery = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/2/b?n=5"); //"/2b" as last input
            //wrong
            var reuest2InvSample = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para", new Dictionary<string, object> { ["n2"] = "b" }); //invalide sample



            //Assert
            jointRouteParams.ObjectVal.Should().Be("5/2/b");
            jointRouteParamsQuery.ObjectVal.Should().Be("5/2/b");
            reuest2InvSample.IsValid.Should().BeFalse();
            reuest2InvSample.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1); //.Should().Contain(BadResourceRequestException.DefualtMessage);


            reuest2Pure.ObjectVal.Should().Be("2b");
            reuest2Over.ObjectVal.Should().Be("2b");
            reuest2Mix.ObjectVal.Should().Be("2b");

        }


        [TestMethod]
        public void HubUriGetWrongParameterTest()
        {
            //arrange
            //done in init
            
            //in this test we still can find the resource so no exception
            //act 
            //act                    
            var reuestExcessive = MyHub.TryGetResourceSampleWithUri(@"cfet://sdfsdf:99/thing1/StatusP/a/d"); //excessive input
            var reuest2Lack = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/2");    //lack of input
            var reuest2Lack2 = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para"); //lack of input
            var reuest2Lack1 = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para", "b"); //lack of input
            var reuest2Excessice = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para", 2,3,"b"); //excessive input
            var reuestWrongType = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/b/b");//can not convert

            var reuest2QueryWrongName = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n3=2&n4=b");    //wrong input name
            var reuest2Conflict = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n2=b", 1);    //confilict last parameter
            var reuest2Conflict2 = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=3&n2=c",  "b"); //confilict last parameter
            var reuest2QueryLack = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=2");    //lack of input
            var reuest2QueryWrongType = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para?n=b","b");    //can not convert



            //Assert
            reuestExcessive.IsValid.Should().BeFalse();
            reuestExcessive.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1);  //.Should().Contain(BadResourceRequestException.DefualtMessage);

            reuest2Lack.IsValid.Should().BeFalse();
            reuest2Lack.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1);  //Should().Contain(BadResourceRequestException.DefualtMessage);

            reuest2Lack2.IsValid.Should().BeFalse();
            reuest2Lack2.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1); //Should().Contain(BadResourceRequestException.DefualtMessage);

            reuest2Lack1.IsValid.Should().BeFalse();
            reuest2Lack1.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1); //Should().Contain(BadResourceRequestException.DefualtMessage);

            reuest2QueryWrongName.IsValid.Should().BeFalse();
            reuest2QueryWrongName.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1); //Should().Contain(BadResourceRequestException.DefualtMessage);

            reuestWrongType.IsValid.Should().BeFalse();
            reuestWrongType.ErrorMessages.Where(m => m.Contains("Input Parameter wrong")).Count().Should().Be(1); //Should().Contain("类型“System.String”的对象无法转换为类型“System.Int32”。"); //this exception is thrown by invoking method using the wrong type of parameter

            reuest2Conflict.IsValid.Should().BeFalse();
            reuest2Conflict.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1); //Should().Contain(BadResourceRequestException.DefualtMessage);

            reuest2Conflict2.IsValid.Should().BeFalse();
            reuest2Conflict2.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1); //Should().Contain(BadResourceRequestException.DefualtMessage);

            reuest2QueryLack.IsValid.Should().BeFalse();
            reuest2QueryLack.ErrorMessages.Where(m => m.Contains(BadResourceRequestException.DefualtMessage)).Count().Should().Be(1); //Should().Contain(BadResourceRequestException.DefualtMessage);

            reuest2QueryWrongType.IsValid.Should().BeFalse();
            reuest2QueryWrongType.ErrorMessages.Where(m => m.Contains("Input Parameter wrong")).Count().Should().Be(1); //Should().Contain("类型“System.String”的对象无法转换为类型“System.Int32”。");





        }


        [TestMethod]
        public void HubUriGetNoResourceTest()
        {
            //arrange
            //done in init

            //act 
            //act                    
            var reuestThing1      = MyHub.TryGetResourceSampleWithUri(@"cfet://sdfsdf:99/thing1");     //valid sample, accessing thing, should be a thing sample
            Action reuestThing2Wring      =()=> MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Parav/2/b"); //valid sample,  accessing thing
            var reuest2Wrong2Para  = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/2/b" ,2,"b"); //excessive parameter
            
            Action reuest2Wrong2Para2 =()=>MyHub.TryGetResourceSampleWithUri(@"/thing2v/thing2v/Status2Parav" ,2,"b"); //wrong path
            Action reuest2WrongPath   =()=>MyHub.TryGetResourceSampleWithUri(@"/thing/thing/Status2Para"); //wrong path
           
            var extraRouteAndQueryParams   = MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/2?n=2&n2=b");//excessive parameter
            var routeAndQueryParams =  MyHub.TryGetResourceSampleWithUri(@"/thing2/thing2/Status2Para/2?n=5");//ok
            Action reuest2QueryWPParent = () => MyHub.TryGetResourceSampleWithUri(@"/thing2/thing/Status2Para?n2=b");//wrong path

            //Assert
            reuestThing1.IsValid.Should().BeTrue();
            reuestThing1.ResourceType.Should().Be(ResourceTypes.Thing);
            reuestThing1.Path.Should().Be("/thing1");

            reuestThing2Wring.ShouldThrow<ResourceDoesNotExistException>();


            reuest2Wrong2Para.IsValid.Should().BeFalse();
            reuest2Wrong2Para.Path.Should().Be("/thing2/thing2/Status2Para");


            reuest2Wrong2Para2.ShouldThrow<ResourceDoesNotExistException>();
            reuest2WrongPath.ShouldThrow<ResourceDoesNotExistException>();

            extraRouteAndQueryParams.IsValid.Should().BeFalse();
            routeAndQueryParams.ObjectVal.Should().Be("5/2");
            reuest2QueryWPParent.ShouldThrow<ResourceDoesNotExistException>();

        }

        [TestMethod]
        public void HubUriCommunicationModuleSupportTest()
        {
            Action reuest =()=> MyHub.TryGetResourceSampleWithUri(@"hehe://sdfsdf:99/thing1/StatusP");

            var reuest2 = MyHub.TryGetResourceSampleWithUri(@"dummy://sdfsdf:99/thing2/thing2/Status2Para?n=2&n2=b");
            var reuest3 = MyHub.TryGetResourceSampleWithUri(@"test:///thing2/thing2/Status2Para",2 ,"b");

            reuest.ShouldThrow<ProtocolNotSuportedException>();
            reuest2.ObjectVal.Should().Be("2b");
            reuest3.ObjectVal.Should().Be("2b");
        }



    }
}
