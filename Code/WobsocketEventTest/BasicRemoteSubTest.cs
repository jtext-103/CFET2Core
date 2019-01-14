using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.WebsocketEvent.Test.Dummy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jtext103.CFET2.WebsocketEvent.Test
{
    [TestClass]
    public class BasicRemoteSubTest: CFET2Host
    {
        EventHub hub;
        WebsocketEventThing remoteHub;
        List<Guid> results;
        List<Payload> resultsHeavy;
        EventTestThing testThing;
        string host = "ws://127.0.0.1:8081";
        [TestInitialize]
        public void init()
        {
            HubMaster.InjectHubToModule(this);
            testThing = new EventTestThing();
            MyHub.TryAddThing(testThing, @"/", "EventTest");

            remoteHub = new WebsocketEventThing();
            MyHub.TryAddThing(remoteHub, @"/", "WsEvent", new WebsocketEventConfig { Host =host});
            MyHub.EventHub.RemoteEventHubs.Add(remoteHub);
            MyHub.StartThings();
            results = new List<Guid>();
            resultsHeavy = new List<Payload>();


        }

        [TestCleanup]
        public void clean()
        {
            Hub.KillMaster();
        }


        [TestMethod]
        public void ShouldPusblshAndHandleAndUnsubscribeRemoteEvent()
        {
            
            var token1 = MyHub.EventHub.Subscribe(new EventFilter(@"/EventTest/idtest/[0-9]{0,}$", EventFilter.DefaultEventType, 1, host),
                eventHandler);
            var token2 = MyHub.EventHub.Subscribe(new EventFilter(@"/EventTest/idtest/2", EventFilter.DefaultEventType, 0, host),
                eventHandler);
            MyHub.EventHub.Subscribe(new EventFilter(@"/EventTest/idtest/2", EventFilter.DefaultEventType, 0, "ws://127.0.0.1:8082"),
                eventHandler);
            Debug.WriteLine("main sent subscribes");
            Thread.Sleep(3000);
            Debug.WriteLine("main fires");
            var id=testThing.FireOne(2);
            Thread.Sleep(500);
            token1.Dispose();
            Thread.Sleep(500);
            var id2 = testThing.FireOne(2);
            Thread.Sleep(2000);
            lock (results)
            {
                results.Count.Should().Be(3);
                results[0].Should().Be(id);
                results[1].Should().Be(id);
                results[2].Should().Be(id2);
            }


        }

        [TestMethod]
        public void ShouldRecieveHeavyPayload()
        {
            var token1 = MyHub.EventHub.Subscribe(new EventFilter(@"/EventTest/idtestHeavy/[0-9]{0,}$", EventFilter.DefaultEventType, 1, host),
               eventHandlerHeavy);
            
            Debug.WriteLine("main sent subscribes");
            Thread.Sleep(3000);
            Debug.WriteLine("main fires");
            var payload = testThing.FireHeavy(2,1000);
            Thread.Sleep(200);
            var payload2 = testThing.FireHeavy(2, 5000);
            Thread.Sleep(1000);
            lock (resultsHeavy)
            {
                resultsHeavy.Count.Should().Be(2);
                resultsHeavy[0].Id.Should().Be(payload.Id);
                resultsHeavy[0].Records.Should().BeEquivalentTo(payload.Records);
                resultsHeavy[1].Id.Should().Be(payload2.Id);
                resultsHeavy[1].Records.Should().BeEquivalentTo(payload2.Records);
            }
        }



        public void eventHandler(EventArg e)
        {
            lock (results)
            {
                results.Add(e.Sample.GetVal<Guid>());
            }
        }

        public void eventHandlerHeavy(EventArg e)
        {
            lock (resultsHeavy)
            {
                resultsHeavy.Add(e.Sample.GetVal<Payload>());
            }
        }

    }
}
