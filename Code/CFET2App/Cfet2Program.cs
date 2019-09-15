using Jtext103.CFET2.CFET2App.cli;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.CFET2App
{
    partial class Cfet2Program:CFET2Host
    {
        static void Main(string[] args)
        {
            //dynamic load:
            //load config file from args=>LoadConfig
            //Loader.LoadLog(LoadConfig.Log);
            //Loader.LoadCommunication(LoadConfig.Communication,host);
            //Loader.LoadThing(LoadConfig.Things,host);

            //very first thing to do is congfig logger
            //初始化 LogManager
            NlogProvider.Config();
            Cfet2LogManager.SetLogProvider(new NlogProvider());

            //inject and init the host app module
            var host = new Cfet2Program();
            HubMaster.InjectHubToModule(host);

            //add communication module

            //var comm= new NancyCM("http://localhost:13345");

            //var comm = new AspNetCoreCommunicatonModule();
            //host.MyHub.TryAddCommunicationModule(comm);
            //todo lock comminication so no more communication can be added

            //add things
            host.AddThings(); //this is defined in a partial file of this class f12 to modify

            //start communication modules
            host.MyHub.StartCommunication();

            //start all thins
            host.MyHub.StartThings();


            //start cli loop 
            var cli = new CliParser(host);
            cli.Host = host;
            Console.WriteLine("Cfet2 host Cli started");
            while (true)
            {
                Console.Write("Cfet2> ");
                var command=Console.ReadLine();
                try
                {
                    cli.Execute(command);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (cli.MySesstion.ShouldExit)
                {
                    //quit the app
                    break;
                }
            }
        }
    }
}
