using Jtext103.CFET2.Core.Exception;
using Jtext103.CFET2.Core.Extension;
using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Resource
{
    public class ResourceMethod : ResourceStatus
    {
        protected MethodInfo MethodInvoke { get; set; }


        public ResourceMethod(string name, ResourceThing resourceThing) : base(name, resourceThing)
        {
            ResourceType = ResourceTypes.Method;
        }


        public override object Get(params object[] inputs)
        {
            var parameters = MethodInvoke.GetParameters();
            return new SampleBase<MethodInfo>(MethodInvoke).AddErrorMessage(BadResourceRequestException.DefualtMessage)
                .AddErrorMessage("Method Only support invoke!").ToMethod().SetPath(Path); 
        }

        public virtual object Invoke(params object[] inputs)
        {
            try
            {
                var realInput = inputs.MapInputDictinary(MethodInvoke);
                var result = InvokeResoureMethod(realInput, MethodInvoke);
                return result.ToMethod().SetPath(Path);
            }
            catch (System.Exception exception)
            {
                //return SampleBase<object>.GetInvalideSample(exception.Message); seem will never hit this line
                return SampleBase<object>.GetInvalideSample(exception.ToString()).SetPath(Path);
            }


        }

        /// <summary>
        /// invoke the method with parameters in dictionery,
        /// get the value of the resource, you provide a dictionary as an input,
        /// the key is the parameter name, note you can use the key CommonConst.TheLastInputsKey as the key for the last object if you dont know its parameter name, 
        /// parameter in the dictionary not matching tha parameter name in the method will be dropped, 
        /// failed to return a value then no exception will be thrown but you get a invalide sample.
        /// </summary>
        /// <param name="inputDict"></param>
        /// <returns></returns>
        public virtual object Invoke(Dictionary<string, object> inputDict)
        {
            return this.Invoke(new DictionaryInputsIndicator(), inputDict);
        }


        public override void AddImplementation(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Method)
            {
                MethodInvoke = member as MethodInfo;
            }
            else
            {
                throw new BadThingImplementaionException("At resource: " + Name + "method implementatioin member must be a method");
            }
        }

        public override ResourceInfo Info
        {
            get
            {
                var info = new ResourceInfo();
                info.Type = ResourceTypes.Method;
                info.Implementations.Add(AccessAction.invoke, MethodInvoke);
                return info;
            }
        }
    }

}
