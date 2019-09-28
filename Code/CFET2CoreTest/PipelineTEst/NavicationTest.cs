using System;
using System.Collections.Generic;
using FluentAssertions;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Middleware.Basic;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Test.TestDummies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jtext103.CFET2.Core.Test.PipelineTest
{
    [TestClass]
    public class NavicationTest:CFET2Host
    {

        [TestInitialize]
        public void init()
        {
            HubMaster.InjectHubToModule(this);
            MyHub.Pipeline.AddMiddleware(new ResourceInfoMidware());
            MyHub.Pipeline.AddMiddleware(new NavigationMidware());
            MyHub.TryAddThing(new TestThingStatus(), @"/", "st");
            MyHub.TryAddThing(new TestThingStatus(), @"/", "st2");
            MyHub.TryAddThing(new TestThingConfig(), @"/st", "cfg");
            MyHub.TryAddThing(new TestThingMethod(), @"/st", "mth");
            MyHub.TryAddThing(new TestThingMethod(), @"/st/node", "mth");
            MyHub.TryAddThing(new TestThingMethod(), @"/st/node/mth/", "mth");
            MyHub.TryAddThing(new TestThingMethod(), @"/node", "mth");
            // /st
            // /st/cfg
            // /st/mth
            // /st/node/mth
            // /st/node/mth/mth
            // /node/mth
            MyHub.StartThings();
            MyHub.StartPipeline();
        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }

        [TestMethod]
        public void TheOldPluginShouldStillWork()
        {
            ResourceRequest req1 = new ResourceRequest(@"/st/cfg/Config1", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[ResourceInfoMidware.ResourceType].Should().Be("Config");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>).Count.Should().Be(2);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].OutputType.Should().Be("Int32");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.get.ToString()].Parameters.Count.Should().Be(0);
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.set.ToString()].OutputType.Should().Be("Int32");
            (sample.Context[ResourceInfoMidware.Actions] as Dictionary<string, ActionInfo>)[AccessAction.set.ToString()].Parameters.Count.Should().Be(0);
        }


        [TestMethod]
        public void ShouldWorkOnEndResource()
        {
            ResourceRequest req1 = new ResourceRequest(@"/st/cfg/Config1", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[NavigationMidware.ParentPath].Should().Be("/st/cfg");
            ((IEnumerable<string>)sample.Context[NavigationMidware.ChildrenPath]).ShouldBeEquivalentTo(new List<string>());
        }

        [TestMethod]
        public void ShouldWorkOnThing()
        {
            ResourceRequest req1 = new ResourceRequest(@"/st/cfg", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[NavigationMidware.ParentPath].Should().Be("/st");
            ((IEnumerable<string>)sample.Context[NavigationMidware.ChildrenPath]).ShouldBeEquivalentTo(new string[] { "/st/cfg/Config1", "/st/cfg/Config2", "/st/cfg/Config3" });

            req1 = new ResourceRequest(@"/st", AccessAction.get, null, null, null);
            sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[NavigationMidware.ParentPath].Should().Be("/");
            ((IEnumerable<string>)sample.Context[NavigationMidware.ChildrenPath]).ShouldBeEquivalentTo(new string[] { "/st/cfg", "/st/mth", "/st/StatusP", "/st/StatusM", "/st/StatusM1", "/st/Status2Para", "/st/node/mth" });
        }

        [TestMethod]
        public void ShouldWorkOnFakeNode()
        {
            ResourceRequest req1 = new ResourceRequest(@"/st/node/mth", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[NavigationMidware.ParentPath].Should().Be("/st");
            ((IEnumerable<string>)sample.Context[NavigationMidware.ChildrenPath]).Should().Contain(new string[] { "/st/node/mth/mth", "/st/node/mth/MethodReturnVoid" });

        }

        [TestMethod]
        public void ShouldWorkOnRealFakeNode_todo()
        {
            ResourceRequest req1 = new ResourceRequest(@"/st/node", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[NavigationMidware.ParentPath].Should().Be("/st");
            ((IEnumerable<string>)sample.Context[NavigationMidware.ChildrenPath]).Should().BeEquivalentTo(new string[] { "/st/node/mth"});

        }

        [TestMethod]
        public void ShouldWorkOnRoot_todo()
        {
            //todo
            ResourceRequest req1 = new ResourceRequest(@"/", AccessAction.get, null, null, null);
            ISample sample = MyHub.TryAccessResourceSampleWithUri(req1);
            sample.Context[NavigationMidware.ParentPath].Should().Be("");
            ((IEnumerable<string>)sample.Context[NavigationMidware.ChildrenPath]).Should().Contain(new string[] { "/st" ,"/st2","/node/mth"});
        }


    }
}
