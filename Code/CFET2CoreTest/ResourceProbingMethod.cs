using FluentAssertions;
using Jtext103.CFET2.Core.Resource;
using Jtext103.CFET2.Core.Test.TestDummies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test
{
    /// <summary>
    /// test basic probing and basic resource accessing of a method
    /// </summary>
    [TestClass]
    public class ResourceProbingMethod
    {
        [TestMethod]
        public void BasciProbingMethod()
        {
            //aeegange
            var testThing = new TestThingMethod();


            //act
            var thing = new ResourceThing(testThing, "thing",null);

            //assert
            thing.Resources.Should().ContainKeys(new string[] { "MethodReturnVoid", "MethodJoin" }).And.HaveCount(2);

        }
    }
}
