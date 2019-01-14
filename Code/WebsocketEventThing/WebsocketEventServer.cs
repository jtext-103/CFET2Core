using Jtext103.CFET2.Core.Event;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp.Server;

namespace Jtext103.CFET2.WebsocketEvent
{
    /// <summary>
    /// this object will send event to the remote subscribers over web socket
    /// and accept remote subscriptions
    /// </summary>
    public class WebsocketEventServer
    {
       

        private WebSocketServer myWsServer;

        public WebsocketEventServer(string host)
        {
            myWsServer = new WebSocketServer(host);
        }

        public void StartServer()
        {
            myWsServer.Start();
        }

        public void AddEndPoint(string path)
        {
            myWsServer.AddWebSocketService<WebsocketEventHandler>(path);
        }

        public void Dispose()
        {
            myWsServer.Stop();
            WebsocketEventHandler.UnsubAllLocal();
        }

    }
}
