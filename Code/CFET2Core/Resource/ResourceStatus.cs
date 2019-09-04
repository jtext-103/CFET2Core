using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Exception;
using Jtext103.CFET2.Core.Extension;
using Jtext103.CFET2.Core.Sample;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Jtext103.CFET2.Core.Resource
{
    /// <summary>
    /// the resource for status, this is read only,
    /// it support property getter, method 
    /// NOTE: VERY IMPORTANT!:
    /// One thing can only have a resource of the same name, 
    /// for status it can be implemented by the thing either as a proptery or a method returning a sample and takes 0 or 1 parameter.
    /// </summary>
    public class ResourceStatus : ResourceBase
    {

        /// <summary>
        /// get sample value from this property
        /// </summary>
        protected PropertyInfo PropertyGet { get; set; }

        /// <summary>
        /// get sample value from the return value of this methods
        /// </summary>
        protected MethodInfo MethodGet { get; set; }

        /// <summary>
        /// hold the parent resource thing for invoke the thing implementations
        /// </summary>
        protected ResourceThing ParentResource { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">the name of this source</param>
        /// <param name="resourceThing">the parent thing of this resource</param>
        public ResourceStatus(string name, ResourceThing resourceThing) 
        {
            Name = name;
            ResourceType = ResourceTypes.Status;
            this.ParentResource = resourceThing;
        }

        /// <summary>
        /// why it's not the constructure?
        /// becourse this my be called multiple times to add more implementations into the resource
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void AddImplementation(MemberInfo member)
        {
            tryAssignImplemationGet(member);
        }






        /// <summary>
        /// get the value of the resource, you can provide 0 or 1 input parameter,
        /// if the resouce requires a parameter but you failed to provide one or the input is invalid or anything the bad happened and the thing
        /// failed to return a value then no exception will be thrown but you get a invalide sample.
        /// </summary>
        /// <param name="inputs">input for the status</param>
        /// <returns>status in sample</returns>
        public override object Get(params object[] inputs)
        {
            try
            {
                return tryGetFromThing(inputs).ToStatus().SetPath(Path);
            }
            catch(System.Exception exception)
            {
                //return SampleBase<object>.GetInvalideSample(exception.Message);
                return SampleBase<object>.GetInvalideSample(exception.ToString()).SetPath(Path);
            }
        }



        /// <summary>
        /// try to assign coorisponding implemeation to right property
        /// </summary>
        /// <param name="member"></param>
        protected void tryAssignImplemationGet(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                var tMember = member as PropertyInfo;
                if (tMember.GetGetMethod() == null)
                {
                    throw new BadThingImplementaionException("At resource: " + Name + "property implementation missing getter");
                }
                PropertyGet = tMember;
            }
            else if (member.MemberType == MemberTypes.Method)
            {

                ////check signiture, todo allow more parameters=done
                //if (((MethodInfo)member).GetParameters().Count() > 1)
                //{
                //    throw new BadThingImplementaionException("At resource: " + Name + "method status can only take 0 or 1 parameter!");
                //}

                //if (((MethodInfo)member).ReturnType == typeof(void))
                //{
                //    throw new BadThingImplementaionException("At resource: " + Name + " method status must have return value");
                //}
                MethodGet = member as MethodInfo;
            }
            else
            {
                throw new BadThingImplementaionException("At resource: " + Name + "member not allowed to be ");
            }
        }



        /// <summary>
        /// this is useful that for method and config they all allow get, this can be reused
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        protected object tryGetFromThing(object[] inputs)
        {
            if (PropertyGet != null)
            {
                if (inputs.Length != 0) //should not have any parameters
                {
                    return new SampleBase<PropertyInfo>(PropertyGet).AddErrorMessage(BadResourceRequestException.DefualtMessage)
                        .AddErrorMessage("Should not have any parameters!").SetPath(Path).ToStatus();
                }
                return PropertyGet.GetValue(ParentResource.TheThing);
            }
            //use method
            var realInput = inputs.MapInputDictinary(MethodGet);
            return InvokeResoureMethod(realInput, MethodGet);
        }

        #region helper
        /// <summary>
        /// helper invoke an method in a resource.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected object InvokeResoureMethod(object[] inputs,MethodInfo method)
        {
            //convert inout into valid input
            var methodParameters = method.GetParameters();
            if (inputs.Length > methodParameters.Count()) //parameters number not match
            {
                //return the MethodInfo list
                return new SampleBase<MethodInfo>(method).AddErrorMessage(BadResourceRequestException.DefualtMessage)
                    .AddErrorMessage("Excessive inputs!!").SetPath(Path).ToStatus();
            }
            //if so cast (map) the input and invoke
            //cast, convert 
            object[] mappedInputs;
            try
            {
                mappedInputs = inputs.MapInputParameters(methodParameters);
            }
            catch (System.Exception e)
            {
                return new SampleBase<MethodInfo>(method).AddErrorMessage(e.Message)
                    .AddErrorMessage(BadResourceRequestException.DefualtMessage).SetPath(Path).ToStatus();
            }

            return method.Invoke(ParentResource.TheThing, mappedInputs);
        }

        /// <summary>
        /// return the madatory parameter count
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        protected int GetMandatoryParameterCount(ParameterInfo[] parameterInfo)
        {
            var paraNum = 0;
            for (int i = 0; i < parameterInfo.Count(); i++)
            {
                if (parameterInfo[i].HasDefaultValue)
                {
                    continue;
                }
                paraNum++;
            }
            return paraNum;
        }

        #endregion





    }
}
