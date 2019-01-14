using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Sample;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Jtext103.CFET2.WebsocketEvent
{
    /// <summary>
    /// use this to subscribe to remote event
    /// </summary>
    public class WebsocketEventClient
    {
        /// <summary>
        /// the key is TOken id
        /// </summary>
        public Dictionary<Guid, RemoteSubscription> RemoteSubscriptions  {get;  }= new Dictionary<Guid , RemoteSubscription>();

        /// <summary>
        /// this will subscribe to the remote event, it will keep trying at 500ms interval is failed.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task  SubscribeAync(EventFilter filter,Token token,Action<EventArg> handler)
        {
            var subscription = new RemoteSubscription(filter,handler);
            RemoteSubscriptions.Add(token.Id, subscription);
            await subscription.SendSubscriptionAsync();
        }


        /// <summary>
        /// unsubscribe the remote event, this is a fire&forget method, non-blocking
        /// </summary>
        /// <param name="token"></param>
        public void Unsubscribe(Token token)
        {
            RemoteSubscriptions[token.Id].UnSubscribe();
            RemoteSubscriptions.Remove(token.Id);
        }

        public void Dispose()
        {
            List<Guid> toRemove = new List<Guid>();
            foreach (var sub in RemoteSubscriptions)
            {
                sub.Value.UnSubscribe();
                toRemove.Add(sub.Key);
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                RemoteSubscriptions.Remove(toRemove[i]);
            }
        }
  

    }
}
