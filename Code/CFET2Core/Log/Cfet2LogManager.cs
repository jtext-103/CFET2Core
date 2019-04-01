using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Log
{

    /// <summary>
    /// let thing to create log
    /// </summary>
    public class Cfet2LogManager
    {
        private static ICfet2LogProvider provider;

        /// <summary>
        /// before any thing heppend set the log, once set it can never be changed
        /// </summary>
        /// <param name="logProvider"></param>
        public static void SetLogProvider(ICfet2LogProvider logProvider)
        {
            if (provider == null)
            {
                provider = logProvider;
            }
            else
            {
                throw new System.Exception("log can be set only once");
            }
        }

        /// <summary>
        /// get the logger
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ICfet2Logger GetLogger(string name)
        {
            return provider.GetLogger(name);
        }


    }
}
