using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jtext103.CFET2.Core.Resource
{
    /// <summary>
    /// describe the resource
    /// </summary>
    public class ResourceInfo
    {
        public ResourceTypes Type { get; set; }

        /// <summary>
        /// the implementions of this resource
        /// </summary>
        public Dictionary<AccessAction, MemberInfo> Implementations = new Dictionary<AccessAction, MemberInfo>();

    }
}