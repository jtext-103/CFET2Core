using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test.TestDummies
{
    public class TestThingMethod:Thing
    {
        //return void
        [Cfet2Method]
        public void MethodReturnVoid()
        {
            return;
        }


        /// <summary>
        /// //return joint string you pass in
        /// </summary>
        /// <param name="s1">if null return null</param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <returns></returns>
        [Cfet2Method]
        public string MethodJoin(string s1, string s2, string s3)
        {
            if (s1 == null)
            {
                return null;
            }
            return String.Join(":", s1, s2, s3);
        }
    }

    public class TestThingMethodBad:Thing
    {
        //return void
        [Cfet2Method]
        public void MethodReturnVoid()
        {

        }

        /// <summary>
        ///over load allowed but need to have another name using the attribute
        /// </summary>
        /// <param name="hehe"></param>
        [Cfet2Method]
        public void MethodReturnVoid(string hehe)
        {

        }
    }
}
