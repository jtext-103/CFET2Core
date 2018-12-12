using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Attributes
{
    /// <summary>
    /// this indecates that it is a method for a thing
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Cfet2MethodAttribute : Cfet2AttributeBase
    {
        public Cfet2MethodAttribute(string name) : base(name)
        {
        }


        public Cfet2MethodAttribute():base()
        {

        }
    }
}
