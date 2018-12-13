using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Nancy;
using Nancy.Hosting.Self;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    public class NancyServer
    {
        //用来与 CFET2 进行通信
        static public Hub TheHub;
        public Uri UriHost;
            
        public NancyServer(Hub hub, Uri myUriHost)
        {
            TheHub = hub;
            UriHost = myUriHost;
        }

        public void Start()
        {
            Task.Run(() => serverLoop());
        }

        public void serverLoop()
        {
            HostConfiguration hostConfigs = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            using (var host = new NancyHost(UriHost, new DefaultNancyBootstrapper(), hostConfigs))
            {
                host.Start();
                Console.WriteLine("Running on " + UriHost);
                while (true)
                {
                    Thread.Sleep(10000);
                }
            }

        }
    }
}
