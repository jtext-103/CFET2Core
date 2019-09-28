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

namespace Jtext103.CFET2.Core.Test.HubTest
{
    /// <summary>
    /// test basic probing and basic resource accessing of a status
    /// </summary>
    [TestClass]
    public class HubRequestParsingTest:Thing
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
            MyHub.TryAddThing(testThing, @"/thing2/thing3", "thing4"); // /thing2/thing3/thing4
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }


        [TestMethod]
        public void HubGetResourceThingTest()
        {
            //arrange
            //done in init

            //act                    
            var t1 = MyHub.GetLocalResouce(@"/thing1");
            var t2 = MyHub.GetLocalResouce(@"/thing2");
            var t3 = MyHub.GetLocalResouce(@"/thing2/thing2");
            var t4 = MyHub.GetLocalResouce(@"/thing2/thing3/thing4");
            var t5 = MyHub.GetLocalResouce(@"/thing2/thing2/thing4");
            var t6 = MyHub.GetLocalResouce(@"/thing2/thing2?r=9");
            //Assert
            t1.Name.Should().Be("thing1");
            t2.Name.Should().Be("thing2");
            t3.Name.Should().Be("thing2");
            t4.Name.Should().Be("thing4");
            t5.Should().BeNull();
            t6.Should().BeNull("becase is has query string");
        }


        [TestMethod]
        public void HubGetResource()
        {
            //arrange
            //done in init

            //act                    
            var t1p = MyHub.GetLocalResouce(@"/thing1/StatusP");
            var t1m = MyHub.GetLocalResouce(@"/thing1/StatusM");
            var t2m = MyHub.GetLocalResouce(@"/thing2/StatusM");
            var t32P = MyHub.GetLocalResouce(@"/thing2/thing2/Status2Para");
            var t4m = MyHub.GetLocalResouce(@"/thing2/thing3/thing4/StatusM1");
            var t5 = MyHub.GetLocalResouce(@"/thing2/thing2/thing4/StatusM");

            //Assert
            t1p.Name.Should().Be("StatusP");
            t1m.Name.Should().Be("StatusM");
            t2m.Name.Should().Be("StatusM");
            t32P.Name.Should().Be("Status2Para");
            t4m.Name.Should().Be("StatusM1");
            t5.Should().BeNull();
        }

        [TestMethod]
        public void HubParsingUriTest()
        {
            //arrange
            //done in init

            //act                    
            var parsedReuest = MyHub.ParseLocalRequest(new Uri(@"cfet://sdfsdf:99/thing1/StatusP", UriKind.RelativeOrAbsolute));
            var parsedReuest2 = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing2/Status2Para/2/b", UriKind.RelativeOrAbsolute));
            var parsedReuest2WithInputs = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing2/Status2Para", UriKind.RelativeOrAbsolute), 2, "b");
            var parsedReuest2WithLastInput = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing2/Status2Para/2", UriKind.RelativeOrAbsolute),"b");
            var parsedReuest2With2Input = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing2/Status2Para/b", UriKind.RelativeOrAbsolute), 2,"b");   // not fail, but inccurect path
            var parsedReuest2f = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing2/Status2Para/2/b/", UriKind.RelativeOrAbsolute));
            //below result should be route parameter + input array, not fail
            var parsedReuestT4 = MyHub.ParseLocalRequest(new Uri(@"cfet://dddd.com/thing2/thing3/thing4/StatusM1", UriKind.RelativeOrAbsolute));
            var parsedReuestT4Routinput = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing3/thing4/StatusM1/a/s/d/f", UriKind.RelativeOrAbsolute));
            var parsedReuestT4Routinput2 = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing3/thing4/StatusM1/a/s/d", UriKind.RelativeOrAbsolute),"f");
            var parsedReuestT4RoutinputFail = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing3/thing4/StatusM1/a/s", UriKind.RelativeOrAbsolute), "d","f"); 


            var parsedReuest2WithQuery = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing2/Status2Para?n=2&n2=b", UriKind.RelativeOrAbsolute),null,null); //input arry set, query is ignoed
            
            var parsedReuest2WithQueryAndRoutePara = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing2/Status2Para?n=3&n2=c", UriKind.RelativeOrAbsolute),2, "b");
            //Action parsedReuest2WithQueryAndRoutePara = () => MyHub.ParseRequest(new Uri(@"/thing2/thing2/Status2Para/2/b?n=2", UriKind.RelativeOrAbsolute)); //fail

            //Action parsedReuestWrongPath = () => MyHub.ParseRequest(new Uri(@"/thing2/thing6/Status2Para?n=5", UriKind.RelativeOrAbsolute)); //fail
            Action parsedReuestWrongPath2 = () => MyHub.ParseLocalRequest(new Uri(@"/thing6/thing6/Status2Para", UriKind.RelativeOrAbsolute)); //fail
            var parsedReuestWrongPathOk = MyHub.ParseLocalRequest(new Uri(@"/thing2/thing6/Status2Para", UriKind.RelativeOrAbsolute));
            //Action parsedReuestWrongProtocol = () => MyHub.ParseRequest(new Uri(@"http://ssss/thing2/thing6/Status2Para", UriKind.RelativeOrAbsolute)); ; //fail


            //Assert
            parsedReuest.ResourcePath.Should().Be("/thing1/StatusP");
            parsedReuest.Inputs.Length.Should().Be(0);
            parsedReuest.InputDict.Should().BeNull();

            parsedReuest2.ResourcePath.Should().Be(@"/thing2/thing2/Status2Para");
            parsedReuest2.Inputs.Length.Should().Be(2);
            parsedReuest2.InputDict.Should().BeNull();

            parsedReuest2WithInputs.ResourcePath.Should().Be(@"/thing2/thing2/Status2Para");
            parsedReuest2WithInputs.Inputs.Length.Should().Be(2);
            parsedReuest2WithInputs.InputDict.Should().BeNull();

            parsedReuest2WithLastInput.ResourcePath.Should().Be(@"/thing2/thing2/Status2Para");
            parsedReuest2WithLastInput.Inputs.Length.Should().Be(2);
            parsedReuest2WithLastInput.InputDict.Should().BeNull();

            parsedReuestT4.ResourcePath.Should().Be(@"/thing2/thing3/thing4/StatusM1");
            parsedReuestT4.Inputs.Length.Should().Be(0);
            parsedReuestT4.InputDict.Should().BeNull();

            parsedReuestT4Routinput.ResourcePath.Should().Be(@"/thing2/thing3/thing4/StatusM1");
            parsedReuestT4Routinput.Inputs.Length.Should().Be(4);
            parsedReuestT4Routinput.Inputs.Should().Equal("a","s","d","f");
            parsedReuestT4Routinput.InputDict.Should().BeNull();

            parsedReuestT4Routinput2.ResourcePath.Should().Be(@"/thing2/thing3/thing4/StatusM1");
            parsedReuestT4Routinput2.Inputs.Length.Should().Be(4);
            parsedReuestT4Routinput2.Inputs.Should().Equal("a", "s", "d", "f");
            parsedReuestT4Routinput2.InputDict.Should().BeNull();


            var indict = new Dictionary<string, object>()
            {
                ["n"] = "2",
                ["n2"] = "b",
            };
            var indictLast = new Dictionary<string, object>()
            {
                ["n"] = "2",
                [CommonConstants.TheLastInputsKey] = "b",
            };
            parsedReuest2WithQuery.ResourcePath.Should().Be(@"/thing2/thing2/Status2Para");
            parsedReuest2WithQuery.Inputs.Length.Should().Be(2);
            parsedReuest2WithQuery.InputDict.Should().BeNull();

            parsedReuest2WithQueryAndRoutePara.ResourcePath.Should().Be(@"/thing2/thing2/Status2Para");
            parsedReuest2WithQueryAndRoutePara.Inputs.Length.Should().Be(2);
            parsedReuest2WithQueryAndRoutePara.InputDict.Should().BeNull();
            parsedReuest2WithQueryAndRoutePara.Inputs.Should().Equal(2, "b");

            parsedReuestWrongPathOk.ResourcePath.Should().Be(@"/thing2");
            parsedReuestWrongPathOk.Inputs.Length.Should().Be(2);
            parsedReuestWrongPathOk.InputDict.Should().BeNull();

            parsedReuest2With2Input.ResourcePath.Should().Be(@"/thing2/thing2/Status2Para");
            parsedReuest2With2Input.Inputs.Length.Should().Be(3);
            parsedReuest2With2Input.InputDict.Should().BeNull();
            

            parsedReuest2f.ResourcePath.Should().Be(@"/thing2/thing2/Status2Para");
            parsedReuest2f.Inputs.Length.Should().Be(2);
            parsedReuest2With2Input.InputDict.Should().BeNull();

            parsedReuestT4RoutinputFail.ResourcePath.Should().Be(@"/thing2/thing3/thing4/StatusM1");
            parsedReuestT4RoutinputFail.Inputs.Length.Should().Be(4);
            parsedReuestT4RoutinputFail.Inputs.Should().Equal("a","s","d", "f");
            parsedReuestT4RoutinputFail.InputDict.Should().BeNull();

            //parsedReuest2WithQueryAndRoutePara.ShouldThrow<ResourceDoesNotExistException>();
            //parsedReuestWrongPath.ShouldThrow<ResourceDoesNotExistException>();
            parsedReuestWrongPath2.ShouldThrow<ResourceDoesNotExistException>();
            //parsedReuestWrongProtocol.ShouldThrow<ProtocolNotSuportedException>();


        }




    }
}
