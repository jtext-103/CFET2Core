using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Exception
{
    [Serializable]
    public class ProtocolNotSuportedException : GeneralCfet2Exception
    {
        public ProtocolNotSuportedException()
        {
        }

        public ProtocolNotSuportedException(string message) : base(message)
        {
        }

        public ProtocolNotSuportedException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}
