using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Extension;
using Jtext103.CFET2.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jtext103.CFET2.CFET2App.ExampleThings
{
    /// <summary>
    /// 可以用于请求另一台主机上的资源
    /// </summary>
    public class RemoteRequester : Thing
    {
        [Cfet2Status]
        public object Get(string requestUri)
        {

            object result = MyHub.TryGetResourceSampleWithUri(requestUri);
            object val = MyHub.TryGetResourceSampleWithUri(requestUri).ObjectVal;
            return val;
        }
    }
}
