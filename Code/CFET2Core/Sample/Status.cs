using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Sample
{
    /// <summary>
    /// represent a 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Status<T>:SampleBase<T>
    {
  
        #region ctor

        public Status():base()
        {
            ResourceType = ResourceTypes.Status;
        }
        public Status(T initVal) :base(initVal)
        {
            ResourceType = ResourceTypes.Status;
        }

        public Status(Dictionary<string, object> context) : base(context)
        {
            ResourceType = ResourceTypes.Status;
        }

        public Status(T initVal, bool isValid) : base(initVal, isValid)
        {
            ResourceType = ResourceTypes.Status;
        }
        #endregion

        //public override SampleBase<Tval> CastValue<Tval>()
        //{
        //    return new Status<Tval>(Context);
        //}

    }

   
}
