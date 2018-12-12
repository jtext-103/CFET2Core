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
    public class BadResourceRequestException : GeneralCfet2Exception
    {
        public static string DefualtMessage = "The request to a resource did not return a valid value, consider using the right parameters for this resource.";
        public BadResourceRequestException() : base(DefualtMessage) { }
        public BadResourceRequestException(string message) : base(message) { }
        public BadResourceRequestException(string message, System.Exception inner) : base(message, inner) { }

    }

}
