using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Exception
{

    /// <summary>
    /// when user provide wrong parameters to request a resource
    /// </summary>
    [Serializable]
    public class GeneralCfet2Exception : System.Exception
    {
        public GeneralCfet2Exception():base() { }
        public GeneralCfet2Exception(string message) : base(message) { }
        public GeneralCfet2Exception(string message, System.Exception inner) : base(message, inner) { }

    }

}
