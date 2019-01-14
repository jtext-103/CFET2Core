using Jtext103.CFET2.Core.Log;
using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Event
{
    /// <summary>
    /// let things to subscribe and publish events
    /// </summary>
    public class EventHub:IDisposable
    {
        /// <summary>
        /// create the event
        /// </summary>
        public EventHub()
        {
            logger = Cfet2LogManager.GetLogger("EventHub");
        }

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken cancelToken;


        /// <summary>
        /// the remote event thing that the event hub uses
        /// </summary>
        public List<IRemoteEventHub> RemoteEventHubs { get; } = new List<IRemoteEventHub>();



        /// <summary>
        /// init the event hub: start the event handling loop
        /// </summary>
        public void Init()
        {
            cancelToken = tokenSource.Token;
            Task.Run(() => eventHandleLoop(), cancelToken);
        }

        /// <summary>
        /// the ifinit loop of handling event
        /// </summary>
        private void eventHandleLoop()
        {
            while (true)
            {
                //block the handling loop untill a new event has come
                eventPendingEvent.WaitOne();
                while (pendingEvent.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    EventArg eventArg;
                    pendingEvent.TryDequeue(out eventArg);
                    handleEvent(eventArg);
                }
            }
        }

        private void handleEvent(EventArg eventArg)
        {
            foreach (var subscriber in subDict.Values)
            {
                if (subscriber.Filter.Predicate(eventArg))
                {
                    //fire and forget
                    Task.Run(()=>subscriber.Handler(eventArg));
                }
            }
        }

        /// <summary>
        /// dictionary for subscribes
        /// </summary>
        Dictionary<Guid, Subscriber> subDict { get; } = new Dictionary<Guid, Event.Subscriber>();

        ConcurrentQueue<EventArg> pendingEvent=new ConcurrentQueue<EventArg>();

        AutoResetEvent eventPendingEvent = new AutoResetEvent(false);
        private ICfet2Logger logger;

        /// <summary>
        /// publis a event, not that the payload you are supplying will be wrapped in a status sample if it is not a sample already.
        /// </summary>
        /// <param name="source">the source resource path</param>
        /// <param name="eventType"></param>
        /// <param name="payload">the event value, will be wrapped in status sample if it not a sample</param>
        public void Publish(string source, string eventType, object payload)
        {
            var sample=payload.ToStatus();
            var eventArg = new EventArg(source, eventType, sample);
            Publish(eventArg);
        }

        /// <summary>
        /// publis a event, not that the payload you are supplying will be wrapped in a status sample if it is not a sample already.
        /// </summary>
        /// <param name="eventArg"></param>
        public void Publish(EventArg eventArg)
        {
            pendingEvent.Enqueue(eventArg);
            newPendingEvent();
        }

        /// <summary>
        /// release the blocked event handling loop
        /// </summary>
        private void newPendingEvent()
        {
            eventPendingEvent.Set();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public Token Subscribe(EventFilter filter, Action<EventArg> handler)
        {
            if (!string.IsNullOrEmpty(filter.Host))
            {
                //try subscribe remote event
                string protocol= new Uri(filter.Host).Scheme;
                foreach (var remoteHub in RemoteEventHubs)
                {
                    if (remoteHub.Protocol == protocol)
                    {
                        var remoteToken = new Token(this,protocol);
                        remoteHub.Subscribe(remoteToken, filter,handler);
                        return remoteToken;
                    }
                }
                //no matching protocol, print log
                logger.Error("No matching remote event hub for: " + filter.Host);
                return null;
            }
            //sub local event
            var token = new Token(this);
            subDict.Add(token.Id,
                        new Subscriber(filter, handler));
            return token;
        }

        /// <summary>
        /// unsbcribe an event
        /// </summary>
        /// <param name="token"></param>
        public void Unsubscribe(Token token)
        {
            if (token.IsRemote)
            {
                foreach (var remoteHub in RemoteEventHubs)
                {
                    if (remoteHub.Protocol == token.Protocol)
                    {
                        remoteHub.Unsbscribe(token);
                        return;
                    }
                }
            }
            subDict.Remove(token.Id);
        }

        /// <summary>
        /// kill the handling loop 
        /// </summary>
        public void Dispose()
        {
            subDict.Clear();
            pendingEvent = new ConcurrentQueue<EventArg>() ;
            tokenSource.Cancel();
            //unblock the loop
            newPendingEvent();
            foreach (var remteHub in RemoteEventHubs)
            {
                remteHub.Dispose();
            }
        }
    }
}
