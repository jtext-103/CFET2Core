using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Jtext103.CFET2.Core.Log;

namespace Jtext103.CFET2.Core.Log
{
    

    public class Cfet2Logger : ICfet2Logger
    {
        /// <summary>
        /// hold the Nlog logger
        /// </summary>
        public Logger Logger { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger">NLOG Logger</param>
        public Cfet2Logger(Logger logger)
        {
            Logger = logger;
        }


        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Error(string message)
        {
            Logger.Error(message);
        }

        public void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Trace(string message)
        {
            Logger.Trace(message);
        }

        public void Warn(string message)
        {
            Logger.Warn(message);
        }
    }
}
