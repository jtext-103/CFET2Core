using FluentAssertions;
using Jtext103.CFET2.Core.Resource;
using Jtext103.CFET2.Core.Sample;
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
    public class MethodInvokeTest
    {
        [TestMethod]
        public void BareboneMethodInvokeTest()
        {
            //aeegange
            var testThing = new TestThingMethod();


            //act
            var thing = new ResourceThing(testThing, "thing",null);
            //var statusP = thing.Resources["StatusP"] as ResourceStatus;

            var methodVoid = thing.Resources["MethodReturnVoid"] as ResourceMethod;
            var methodJoin = thing.Resources["MethodJoin"] as ResourceMethod;



            ISample methodVoidS = methodVoid.Invoke() as ISample; //null valid
            ISample methodVoidSErr = methodVoid.Invoke(1) as ISample; //null invalid

            ISample methodJoinS = methodJoin.Invoke(null,"10",20) as ISample; //null valid
            string input = null;
            ISample methodJoinSErr = methodJoin.Invoke(input) as ISample; //null invalid
            ISample methodJoinSErr2 = methodJoin.Invoke(null, "10", 20,30) as ISample; //null invalid
            ISample methodJoinSOk = methodJoin.Invoke(10,20,30) as ISample; //valid 10:20:30
            var input2 = new Dictionary<string, object>()
            {
                ["s1"] = 1,
                ["s3"] = "3",
                ["s2"] = 2
            };
            ISample methodJoinSOk2 = methodJoin.Invoke(input2) as ISample; //valid 10:20:30

            //assert
            methodVoidS.IsValid.Should().Be(true);  //void return
            methodVoidS.ObjectVal.Should().Be(null);

            methodVoidSErr.IsValid.Should().Be(false);

            methodJoinS.IsValid.Should().Be(true);
            methodJoinS.ObjectVal.Should().Be(null);

            methodJoinSErr.IsValid.Should().Be(false);
            methodJoinSErr2.IsValid.Should().Be(false);

            methodJoinSOk.ObjectVal.Should().Be("10:20:30");

            methodJoinSOk2.ObjectVal.Should().Be("1:2:3");



        }


        
    }
}
