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
    public class WrongResourceActionException : GeneralCfet2Exception
    {
        public static string DefualtMessage = "The resource does not support the action you requested!";
        public WrongResourceActionException() : base(DefualtMessage) { }
        public WrongResourceActionException(string message) : base(message) { }
        public WrongResourceActionException(string message, System.Exception inner) : base(message, inner) { }

    }

}
