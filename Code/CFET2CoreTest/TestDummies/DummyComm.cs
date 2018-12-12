using Jtext103.CFET2.Core.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.Sample;

namespace Jtext103.CFET2.Core.Test.TestDummies
{
    public class DummyComm : CommunicationModule
    {
        public override string[] ProtocolNames =>  new string[]{"test","dummy"};
        
        

        public override ISample TryGetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            //this is just test code, 
            //you should  not do this, normally you will get a sample object here, but not as sample type, it may be serialized,
            //so tosample will wrap it as a sample within a sample, which is wrong, try convert of deserialie it into corresponding sample types.
            var localUrl = new UriBuilder(requestUri);
            localUrl.Scheme = "cfet";
            return MyHub.TryGetResourceSampleWithUri(localUrl.Uri.ToString(),inputDict);
        }

        public override ISample TryGetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            //this is just test code, 
            //you should  not do this, normally you will get a sample object here, but not as sample type, it may be serialized,
            //so tosample will wrap it as a sample within a sample, which is wrong, try convert of deserialie it into corresponding sample types.
            var localUrl = new UriBuilder(requestUri);
            localUrl.Scheme = "cfet";
            return MyHub.TryGetResourceSampleWithUri(localUrl.Uri.ToString(), inputs);
        }

        public override ISample TryInvokeSampleResourceWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            var localUrl = new UriBuilder(requestUri);
            localUrl.Scheme = "cfet";
            return MyHub.TryInvokeSampleResourceWithUri(localUrl.Uri.ToString(), inputDict);
        }

        public override ISample TryInvokeSampleResourceWithUri(string requestUri, params object[] inputs)
        {
            var localUrl = new UriBuilder(requestUri);
            localUrl.Scheme = "cfet";
            return MyHub.TryInvokeSampleResourceWithUri(localUrl.Uri.ToString(), inputs);
        }

        public override ISample TrySetResourceSampleWithUri(string requestUri, Dictionary<string, object> inputDict)
        {
            var localUrl = new UriBuilder(requestUri);
            localUrl.Scheme = "cfet";
            return MyHub.TrySetResourceSampleWithUri(localUrl.Uri.ToString(), inputDict);
        }

        public override ISample TrySetResourceSampleWithUri(string requestUri, params object[] inputs)
        {
            var localUrl = new UriBuilder(requestUri);
            localUrl.Scheme = "cfet";
            return MyHub.TrySetResourceSampleWithUri(localUrl.Uri.ToString(), inputs);
        }
    }
}
