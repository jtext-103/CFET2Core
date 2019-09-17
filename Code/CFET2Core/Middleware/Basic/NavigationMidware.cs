using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Middleware.Basic
{
    /// <summary>
    /// this midware put parent and children path in the sample 
    /// </summary>
    public class NavigationMidware : CfetMiddlewareBase
    {
        /// <summary>
        /// the key in context for the parent path
        /// </summary>
        public static readonly string ParentPath = "ParentPath";

        /// <summary>
        /// the key in context for the children pathes
        /// </summary>
        public static readonly string ChildrenPath = "ChildrenPath";

        /// <summary>
        /// put parent and children path in the sample 
        /// </summary>
        /// <param name="input">the input sample</param>
        /// <param name="request">note: todo there is a flag indecating weather this plugin is activate</param>
        /// <returns></returns>
        public override ISample Process(ISample input, ResourceRequest request)
        {
            try
            {
                var parentPath = MyHub.FindLocalParentWithPath(input.Path).Path;
                input.Context[ParentPath] = parentPath;
            }
            catch 
            {
                input.Context[ParentPath] = "";
            }
            var childrenPath = MyHub.FindLocalChildWithUri(input.Path).Select(s=>s.Path).ToList();
            input.Context[ChildrenPath] = childrenPath;
            return input;
        }
    }
}
