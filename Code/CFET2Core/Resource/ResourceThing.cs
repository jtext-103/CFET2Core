using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Exception;
using Jtext103.CFET2.Core.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Resource
{
    /// <summary>
    /// this resource represent the thing, you can probe and access other reource of this thing
    /// </summary>
    public class ResourceThing:ResourceBase
    {
        private Dictionary<MemberInfo,Cfet2AttributeBase> thingMembers=new Dictionary<MemberInfo, Cfet2AttributeBase>();


        /// <summary>
        /// holds the instance of a thing, it's publuc get yet, todo: make access control of it
        /// all the resource must be related to a thing
        /// </summary>
        internal Thing TheThing {  get; private set; }




        /// <summary>
        /// the config, status, method of this thing
        /// this does not include child thing
        /// ,todo make it private afte some test
        /// , the resource name is case insensitive
        /// </summary>
        public Dictionary<string, ResourceBase> Resources { get; internal set; } = new Dictionary<string, ResourceBase>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// the defualt status, if set get this thing will be getting this
        /// </summary>
        public ResourceStatus DefualtStatus { get; private set; } = null;



        /// <summary>
        /// thing must have a unique name on the same level
        /// </summary>
        /// <param name="theThing"></param>
        /// <param name="name"></param>
        /// <param name="initObject">the object of initial configuration</param>
        public ResourceThing(Thing  theThing,string name,object initObject=null)
        {
            ResourceType = ResourceTypes.Thing;
            Name = name;
            TheThing = theThing;
            ProbeTheThing();
            theThing.TryInit(initObject);
        }


        private void ProbeTheThing()
        {
            var thingType = TheThing.GetType();
            extractMenbers(thingType);
            foreach (var member in thingMembers)
            {
                switch (member.Value)
                {
                    case Cfet2StatusAttribute status:
                        AddToStatus(member.Key);
                        break;
                    case Cfet2ConfigAttribute status:
                        AddToConfig(member.Key);
                        break;
                    case Cfet2MethodAttribute config:
                        AddToMethod(member.Key);
                        break;
                    default:
                        break;
                }
                /* 
                 * handled be the switch pattern matching
                ////if it is a status
                //if (member.Value is Cfet2StatusAttribute)
                //{
                //    AddToStatus(member.Key);
                //}
                //else if(member.Value is Cfet2ConfigAttribute)
                //{
                //    AddToConfig(member.Key);
                //} 
                */
            }
            //after probing, do an final check
            Resources.Where(r => r.Value.ResourceType == ResourceTypes.Config).ToList().ForEach(r=>((ResourceConfig)r.Value).CheckConfigImplementation());

        }

        private void AddToStatus(MemberInfo member)
        {
            var resName = getResourceName(member);
            //since status allows only get so only one implemation per status
            if (Resources.ContainsKey(resName))
            {
                throw new BadThingImplementaionException("multiple implementaions for status: " + resName);
            }
            var newStatus = new ResourceStatus(resName,this);
            newStatus.AddImplementation(member);
            Resources[resName] = newStatus;
        }
        
        private void AddToMethod(MemberInfo member)
        {
            var resName = getResourceName(member);
            //since method allows only one method implementaition for one resource
            if (Resources.ContainsKey(resName))
            {
                throw new BadThingImplementaionException("multiple implementaions for method: " + resName);
            }
            var newMethod = new ResourceMethod(resName, this);
            newMethod.AddImplementation(member);
            Resources[resName] = newMethod;
        }


        private void AddToConfig(MemberInfo member)
        {
            var resName = getResourceName(member);
            //config can have multiple implemeations but the name should not be the config same as the other types of resource
            var newRes = Resources.ContainsKey(resName) ? Resources[resName] : null;
            if (newRes == null)
            {
                newRes = new ResourceConfig(resName, this);
                Resources[resName] = newRes;

            }
            else if (!(newRes is ResourceConfig))
            {
                throw new BadThingImplementaionException("multiple implementaions for: " + resName);
            }
           ((ResourceConfig)newRes).AddImplementation(member);

        }


        /// <summary>
        /// return defualt status or empty status sample indicating thing
        /// </summary>
        /// <param name="inputs">input for the status</param>
        /// <returns>status in sample</returns>
        public override object Get(params object[] inputs)
        {
            if (DefualtStatus != null)
            {
                //change the path to the thing
                return DefualtStatus.Get(inputs).ToStatus().SetPath(Path);
            }
            else
            {
                if (inputs != null && inputs.Length > 0)
                {
                    throw new ResourceDoesNotExistException();
                }
                //if there is no defual status just return a stauts sample representing this thing.  
                //It has no value but has the path to this thing, so midware can do somthing about it
                return new SampleBase<object>(null, ResourceTypes.Thing).SetPath(Path);
            }
        }

        /// <summary>
        /// get thing itself
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        [Obsolete]
        protected object tryGetFromThing(object[] inputs)
        {

            throw new NotImplementedException("this should not be called");
        }

        #region helpers

        /// <summary>
        /// extract all the members for later use
        /// </summary>
        private void extractMenbers(Type thingType)
        {
            var members = thingType.GetMembers();
            foreach (var member in members)
            {
                var attrs = member.GetCustomAttributes();
                foreach (var attr in attrs)
                {
                    Cfet2AttributeBase cfetAttr = attr as Cfet2AttributeBase;
                    if (cfetAttr != null)
                    {
                        thingMembers.Add(member, cfetAttr);
                    }
                }

            }
        }

        private string getResourceName(MemberInfo member)
        {
            return string.IsNullOrEmpty(member.GetCustomAttribute<Cfet2AttributeBase>().Name) ? member.Name : member.GetCustomAttribute<Cfet2AttributeBase>().Name;
        }

        #endregion





    }
}
