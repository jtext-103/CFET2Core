using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Attributes
{
    /// <summary>
    /// this is the base class for all Cfet2 resource Attribute
    /// a cfet resource can be a status or a config or a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Cfet2AttributeBase:Attribute
    {
        /// <summary>
        /// the name of thes cfet resource, this will be used in making up tha route for the resource, it can be null, so use nameof of the property of method instade
        /// </summary>
        public string Name { get; set; }

        public Cfet2AttributeBase(string name)
        {
            Name = name;
        }


        public Cfet2AttributeBase()
        {
           
        }

    }
}
