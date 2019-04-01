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
            //start the event handle loop
            
        }

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken cancelToken;


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
        }
    }
}
