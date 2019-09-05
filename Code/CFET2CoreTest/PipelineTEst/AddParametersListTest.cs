using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Test.TestDummies;
using System.Collections.Generic;
using Jtext103.CFET2.Core.Middleware;
using Jtext103.CFET2.Core.Middleware.Basic;
using System.Diagnostics;
using Jtext103.CFET2.Core.Exception;
using System.Linq;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Test.TestDummies;

namespace Jtext103.CFET2.Core.Test.PipelineTEst
{
    [TestClass]
    public class AddParametersListTest : CFET2Host
    {
        [TestInitialize]
        public void init()
        {
            HubMaster.InjectHubToModule(this);
            MyHub.Pipeline.AddMiddleware(new ResourceInfoMidware());
            MyHub.TryAddThing(new TestThingStatus(), @"/", "st");
            MyHub.TryAddThing(new TestThingConfig(), @"/", "cfg"); 
            MyHub.TryAddThing(new TestThingMethod(), @"/", "mth");
            MyHub.StartPipeline();
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }

        [TestMethod]
        public void PropertySetShouldHaveCorrectParams()
        {
            ResourceRequest req1 = new ResourceRequest(@"/cfg/Config1", AccessAction.get,null,null,null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[ResourceInfoMidware.ResourceType].Should().Be("Config");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>).Count.Should().Be(2);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].OutputType.Should().Be("Int32");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].Parameters.Count.Should().Be(0);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.set.ToString()].OutputType.Should().Be("Int32");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.set.ToString()].Parameters.Count.Should().Be(0);
        }

        [TestMethod]
        public void PropertyGetShouldHaveCorrectParams()
        {
            ResourceRequest req1 = new ResourceRequest(@"/st/StatusP", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[ResourceInfoMidware.ResourceType].Should().Be("Status");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>).Count.Should().Be(1);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].OutputType.Should().Be("Int32");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].Parameters.Count.Should().Be(0);
        }

        [TestMethod]
        public void MethodGetSetSholdHaveTheCorrectoParams()
        {
            //get
            ResourceRequest req1 = new ResourceRequest(@"/st/Status2Para", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[ResourceInfoMidware.ResourceType].Should().Be("Status");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>).Count.Should().Be(1);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].OutputType.Should().Be("String");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].Parameters.Count.Should().Be(2);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].Parameters.Keys.Should().BeEquivalentTo(new string[] { "n", "n2" });
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].Parameters["n"].Should().Be("Int32");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].Parameters["n2"].Should().Be("String");

            //set
            req1 = new ResourceRequest(@"/cfg/Config2", AccessAction.get, null, null, null);
            sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[ResourceInfoMidware.ResourceType].Should().Be("Config");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>).Count.Should().Be(2);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.set.ToString()].OutputType.Should().Be("Int32");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.set.ToString()].Parameters.Keys.Should().BeEquivalentTo(new string[] { "val" });
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.set.ToString()].Parameters["val"].Should().Be("Int32");
        }

        [TestMethod]
        public void MethodShoudWorkHaveTheCorrectoParams()
        {
            //EvenAccessedWithGet
            //get
            ResourceRequest req1 = new ResourceRequest(@"/mth/MethodReturnVoid", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.IsValid.Should().BeFalse();
            sample.Context[ResourceInfoMidware.ResourceType].Should().Be("Method");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>).Count.Should().Be(1);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.invoke.ToString()].OutputType.Should().Be("Void");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.invoke.ToString()].Parameters.Count.Should().Be(0);

            req1 = new ResourceRequest(@"/mth/MethodJoin", AccessAction.get, null, null, null);
            sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.IsValid.Should().BeFalse();
            sample.Context[ResourceInfoMidware.ResourceType].Should().Be("Method");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>).Count.Should().Be(1);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.invoke.ToString()].OutputType.Should().Be("String");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.invoke.ToString()].Parameters.Count.Should().Be(3);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.invoke.ToString()].Parameters.Keys.Should().BeEquivalentTo(new string[] { "s1", "s2","s3" });
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.invoke.ToString()].Parameters["s1"].Should().Be("String");
        }
    }
}
