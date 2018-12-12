using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Exception
{
    [Serializable]
    public class ResourceDoesNotExistException : GeneralCfet2Exception
    {
        public ResourceDoesNotExistException()
        {
        }

        public ResourceDoesNotExistException(string message) : base(message)
        {
        }

        public ResourceDoesNotExistException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}
