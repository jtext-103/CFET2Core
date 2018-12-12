using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// hold all kinds of cfet2core exceptions that may occur and need to be handle befor app crashes
/// </summary>
namespace Jtext103.CFET2.Core.Exception
{

    /// <summary>
    /// when user provide a thing that is unable to be handled by CFET2
    /// </summary>
    [Serializable]
    public class BadThingImplementaionException : GeneralCfet2Exception
    {
        public BadThingImplementaionException():base() { }
        public BadThingImplementaionException(string message) : base(message) { }
        public BadThingImplementaionException(string message, System.Exception inner) : base(message, inner) { }

        public string BadConfigImplemationMessage { get; } = "Bad Thing implementation!!";

    }
}
