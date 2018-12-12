using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using Jtext103.CFET2.Core.Log;

namespace Jtext103.CFET2.Core.Log
{
    public  class NlogProvider: ICfet2LogProvider
    {
        public static void Config()
        {
            //配置 Log 输出
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            //输出到文件
            // Step 2. Create targets and add them to the configuration 
            var fileTarget = new FileTarget();
            config.AddTarget("", fileTarget);

            // Step 3. Set target properties  
            fileTarget.FileName = "${basedir}/Log_${shortdate}.txt";
            fileTarget.Layout = @"${date} ${logger} ${level} ${newline}${message}";

            // Step 4. Define rules
            var ruleFile = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(ruleFile);

            //输出到控制台
            var consoleTarget = new ConsoleTarget();
            config.AddTarget("", consoleTarget);

            consoleTarget.Layout = @"${date} ${logger} ${level} ${newline}${message}";

            var ruleConsole = new LoggingRule("*", LogLevel.Warn, consoleTarget);
            config.LoggingRules.Add(ruleConsole);

            // Step 5. Activate the configuration
            LogManager.Configuration = config; 
            
        }

        /// <summary>
        /// get the logger with a spercific name
        /// </summary>
        /// <param name="name"></param>
        public ICfet2Logger GetLogger(string name)
        {
            return new Cfet2Logger(LogManager.GetLogger(name));
        }

    }
}
