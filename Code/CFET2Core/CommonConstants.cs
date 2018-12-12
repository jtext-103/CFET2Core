using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core
{
    public static class CommonConstants
    {
        public static string TheLastInputsKey { get; } = "CFET2CORE_COMMON_LASTINPUT";
        public static Uri LocalBaseUri { get; } = new Uri("cfet://localhost/");
    }
}
