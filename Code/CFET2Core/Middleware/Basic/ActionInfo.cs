using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Middleware.Basic
{
    /// <summary>
    /// a simple readable version of member info
    /// </summary>
    public class ActionInfo
    {
        /// <summary>
        /// the parameters 
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public string OutputType { get; set; } = "";
    }
}
