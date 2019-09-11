using Jtext103.CFET2.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Sample
{
    /// <summary>
    /// the method return value is put in the value field
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Method<T> : SampleBase<T>
    {
     
        #region ctor
        public Method():base()
        {
            ResourceType = ResourceTypes.Method;
        }
        public Method(T initVal) : base(initVal)
        {
            ResourceType = ResourceTypes.Method;
        }

        public Method(Dictionary<string, object> context) : base(context)
        {
            ResourceType = ResourceTypes.Method;
        }

        public Method(T initVal, bool isValid) : base(initVal, isValid)
        {
            ResourceType = ResourceTypes.Method;
        }

        //public override SampleBase<Tval> CastValue<Tval>()
        //{
        //    return new Method<Tval>(Context);
        //}
        #endregion

    }
}
