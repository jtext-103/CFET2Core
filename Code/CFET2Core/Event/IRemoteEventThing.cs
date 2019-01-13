using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Event
{
    /// <summary>
    /// an remote event thing interface, this interface allow the local event hub to forward event to remote event thing and subscribe to remote event
    /// </summary>
    public interface IRemoteEventHub:IDisposable
    {
        /// <summary>
        /// the protocol this thing support
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// let the remote event thing handle the subscription to remote event
        /// </summary>
        /// <param name="token"></param>
        /// <param name=""></param>
        /// <param name="handler"></param>
        void Subscribe(Token token, EventFilter filter, Action<EventArg> handler);

        /// <summary>
        /// unsubscribe the remote evnet
        /// </summary>
        /// <param name="token"></param>
        void Unsbscribe(Token token);

    }
}
