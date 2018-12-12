using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Attributes
{
    /// <summary>
    /// this indecates that it is a status for a thing
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class Cfet2StatusAttribute:Cfet2AttributeBase
    {
        public Cfet2StatusAttribute(string name):base(name)
        {
        }

        public Cfet2StatusAttribute():base()
        {

        }
    }
}
