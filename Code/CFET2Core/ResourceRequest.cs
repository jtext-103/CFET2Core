using System.Collections.Generic;

namespace Jtext103.CFET2.Core
{
    /// <summary>
    /// the request object that used to access a resource
    /// </summary>
    public class ResourceRequest
    {        
        /// <summary>
        /// constructor         
        /// </summary>
   
        public ResourceRequest(string uri,AccessAction action, object[] inputarray, 
            Dictionary<string, object> inputdict, Dictionary<string, string> extraRequest, 
            bool usingdict = false)
        {
            //todo ResourceRequest does not need hub
            
            RequestUri = uri;
            Action = action;
            UsingInputDict = usingdict;
            InputArray = inputarray;
            InputDict = inputdict;
            ExtraRequests = extraRequest;
        }
       

        /// <summary>
        /// the request uri
        /// </summary>
        public string RequestUri { get; set; }
        /// <summary>    
        /// the input parameter in array
        /// </summary>
        public object[] InputArray { get; set; }
        /// <summary>
        /// the input parameter in dictionary
        /// </summary>
        public Dictionary<string, object> InputDict { get; set; }
        /// <summary>
        /// the access action 
        /// </summary>
        public AccessAction Action { get; set; } //get,set, invoke

        /// <summary>
        /// to define use dictionary or not
        /// </summary>
        public bool UsingInputDict { get; set; } = false;

        /// <summary>
        /// extrarequest about pipeline
        /// </summary>
        public Dictionary<string, string> ExtraRequests { set; get; }
    }
    /// <summary>
    /// the type  of the http request 
    /// </summary>
    public enum AccessAction { get,set,invoke};
}