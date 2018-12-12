using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Log
{
    /// <summary>
    /// just expose Ilogger in cfet2, so user don't have to reference nlog
    /// </summary>
    public interface ICfet2Logger
    {
        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
    }
}
