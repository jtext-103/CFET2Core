using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Exception;
using Jtext103.CFET2.Core.Extension;

namespace Jtext103.CFET2.Core.Resource
{
    /// <summary>
    /// manage a config resource
    /// </summary>
    public class ResourceConfig : ResourceStatus
    {
        /// <summary>
        /// set sample to this property
        /// </summary>
        protected PropertyInfo PropertySet { get; set; }

        /// <summary>
        /// set sample value using this mehtod
        /// </summary>
        protected MethodInfo MethodSet { get; set; }


        public ResourceConfig(string name, ResourceThing resourceThing) : base(name, resourceThing)
        {
            ResourceType = ResourceTypes.Config;
        }


        /// <summary>
        /// get the value of the resource, you can provide o or many parameters to further locate the resource,
        /// if the resouce requires a parameter but you failed to provide one or the input is invalid or anything the bad happened and the thing
        /// failed to return a value then no exception will be thrown but you get a invalide sample(todo!!).
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override object Get(params object[] inputs)
        {
            try
            {
                return tryGetFromThing(inputs).ToConfig().SetPath(Path);
            }
            catch (System.Exception exception)
            {
                //return SampleBase<object>.GetInvalideSample(exception.Message);
                return SampleBase<object>.GetInvalideSample(exception.ToString()).SetPath(Path).ToConfig();
            }
        }



        /// <summary>
        /// get the value of the resource, you provide a dictionary as an input,
        /// the key is the parameter name, note you can use the key CommonConst.TheLastInputsKey as the key for the last object if you dont know its parameter name, 
        /// in this case you still need to provide the rest parameters witht he name, those is the locators, the last one is the value to be set, 
        /// parameter in the dictionary not matching tha parameter name in the method will be dropped, 
        /// failed to return a value then no exception will be thrown but you get a invalide sample.
        /// </summary>
        /// <param name="inputDict"></param>
        /// <returns></returns>
        public virtual object Set(Dictionary<string, object> inputDict)
        {
            return this.Set(new DictionaryInputsIndicator(), inputDict);
        }



        /// <summary>
        /// Set is tricky. Note, Important, You must always pass a value not a sample to set a config. 
        /// But the thing may implements the config as property of type sample (which is not recommanded unless necessary), to the Set here will automaticlly wrap the value into sample. 
        /// And if it is a Method Set, the arguments must have only no sample type, not poiunt to accept sample, since the sample is for get only
        /// </summary>
        /// <param name="inputs">the input to set the config, if it is a property set of method with only one parameter as the value to be set (config value), then there are only one para nneded, 
        /// if there are more parameter the value must be of the last one, all preceeding ones are the locators</param>
        /// <returns></returns>
        public virtual object Set(params object[] inputs)
        {
            var member = actionImplementedBy(ConfigAction.Set);
            try
            {
                //property set
                if (member.MemberType == MemberTypes.Property)
                {
                    if (inputs.Length != 1)
                    {
                        return new SampleBase<MemberInfo>(member).AddErrorMessage(BadResourceRequestException.DefualtMessage)
                        .AddErrorMessage("Should not have more than 1 parameters!").ToConfig().SetPath(Path);
                    }
                    var setter = member as PropertyInfo;
                    //sample or not
                    object val = inputs[0];
                    if (typeof(ISample).IsAssignableFrom(setter.PropertyType))
                    {
                        val = val.ToConfig();
                    }
                    setter.SetValue(ParentResource.TheThing, val.TryConvertTo(setter.PropertyType));
                    //return the newlly set value
                    return Get();
                }
                //method set, no sample allowed
                var realInput = inputs.MapInputDictinary((MethodInfo)member);
                InvokeResoureMethod(realInput, ((MethodInfo)member));
                return Get(realInput.RangeSubset(0, realInput.Length-1));
            }
            catch (System.Exception exception)
            {
                //return SampleBase<object>.GetInvalideSample(exception.Message);
                return new SampleBase<MemberInfo>(member).AddErrorMessage(BadResourceRequestException.DefualtMessage)
                        .AddErrorMessage(exception.Message).SetPath(Path).ToConfig();
            }

        }




        public override void AddImplementation(MemberInfo member)
        {
            //first check its action
            var attr = member.GetCustomAttribute<Cfet2ConfigAttribute>();
            //each action of a config only has one implemeation
            if (actionImplementedBy(attr.ConfigActions) != null)
            {
                throw new BadThingImplementaionException("multiple implementaions for: " + Name + ", one action per config!");
            }

            switch (attr.ConfigActions)
            {
                case ConfigAction.Get:
                    {
                        tryAssignImplemationGet(member);
                        return;
                    }

                case ConfigAction.Set:
                    {
                        tryAssignImplemationSet(member);
                        return;
                    }
                case ConfigAction.Insert:
                    break;
                case ConfigAction.Delete:
                    break;
                case ConfigAction.GetAndSet:
                    {
                        //get set allow property only
                        if (member.MemberType == MemberTypes.Property)
                        {
                            tryAssignImplemationGet(member);
                            tryAssignImplemationSet(member);
                            return;
                        }
                        throw new NotImplementedException("At resource: " + Name + "member not allowed to be a config");
                    }
                default:
                    break;
            }
            throw new NotImplementedException("At resource: " + Name + "config action not implemented yet");

        }


        /// <summary>
        /// check if this config has both set and get, will throw eception when failed
        /// </summary>
        public void CheckConfigImplementation()
        {
            if (actionImplementedBy(ConfigAction.Get) == null)
            {
                throw new BadThingImplementaionException("Config: " + Name + " missing Get implementation");
            }
            if (actionImplementedBy(ConfigAction.Set) == null)
            {
                throw new BadThingImplementaionException("Config: " + Name + " missing Set implementation");
            }
        }

        /// <summary>
        /// return 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private MemberInfo actionImplementedBy(ConfigAction action)
        {
            switch (action)
            {
                case ConfigAction.Get:
                    {
                        return PropertyGet == null ? (MemberInfo)MethodGet : (MemberInfo)PropertyGet;
                    }
                    
                case ConfigAction.Set:
                    {
                        return PropertySet == null ? (MemberInfo)MethodSet : (MemberInfo)PropertySet;
                    }
                    
                case ConfigAction.Insert:
                    break;
                case ConfigAction.Delete:
                    break;
                case ConfigAction.GetAndSet:
                    {
                        //for get and set they are only propery and always the same
                        //if any get of get is implemented get and set will not work
                        //return ((PropertySet?? PropertyGet) ?? (MemberInfo)MethodGet)??MethodSet;
                        if ((PropertySet == PropertyGet) && MethodSet==null && MethodGet==null)
                        {
                            return PropertyGet;
                        }
                        return typeof(BadThingImplementaionException).GetProperty("BadConfigImplemationMessage");
                    }
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// try to assign coorisponding implemeation to right property
        /// </summary>
        /// <param name="member"></param>
        protected void tryAssignImplemationSet(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                var tMember = member as PropertyInfo;
                if (tMember.GetSetMethod() == null)
                {
                    throw new BadThingImplementaionException("At resource: " + Name + "property implementation missing Setter");
                }
                PropertySet = tMember;
            }
            else if (member.MemberType == MemberTypes.Method)
            {
                MethodSet = member as MethodInfo;
            }
            else
            {
                throw new BadThingImplementaionException("At resource: " + Name + "member not allowed to be ");
            }
        }

        public override ResourceInfo Info
        {
            get
            {
                var info = new ResourceInfo();
                info.Type = ResourceTypes.Config;
                info.Implementations.Add(AccessAction.get, actionImplementedBy(ConfigAction.Get));
                info.Implementations.Add(AccessAction.set, actionImplementedBy(ConfigAction.Set));
                return info;
            }
        }
    }

}
