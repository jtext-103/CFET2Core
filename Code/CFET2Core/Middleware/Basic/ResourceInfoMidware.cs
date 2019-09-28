using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core.Sample;

namespace Jtext103.CFET2.Core.Middleware.Basic
{
    /// <summary>
    /// this plugin adds resource info like input and return type into the sample, see TestAddParametersListThing for use case
    /// </summary>
    public class ResourceInfoMidware : CfetMiddlewareBase
    {
        /// <summary>
        /// the string key in context indicating the outpyt type
        /// </summary>
        public static readonly string OutputType = "OutputType";

        /// <summary>
        /// the string key in context indicating the resource type
        /// </summary>
        public static readonly string ResourceType = "ResourceType";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string Actions = "Actions";

        /// <summary>
        /// get the resour info from the resource put in to the context
        /// </summary>
        /// <param name="input">the input sample</param>
        /// <param name="request">note: todo there is a flag indecating weather this plugin is activate</param>
        /// <returns></returns>
        public override ISample Process(ISample input, ResourceRequest request)
        {
            //find the resource using the path in the sample
            var resource=MyHub.FindLocalResourceWithPath(input.Path);
            //ask for resource info
            var info = resource.Info;
            //translate them
            input.Context.Add(ResourceType, info.Type.ToString());
            input.Context.Add(Actions, new Dictionary<string, ActionInfo>());
            var actions = input.Context[Actions] as Dictionary<string, ActionInfo>;
            foreach (var action in info.Implementations)
            {
                actions[action.Key.ToString()] = generateActionInfo(action.Value, action.Key);
            }
            //if it is a config the set always have the same output type as get
            if (actions.Keys.Contains(AccessAction.set.ToString()))
            {
                actions[AccessAction.set.ToString()].OutputType = actions[AccessAction.get.ToString()].OutputType;
            }
            return input;
        }

        private ActionInfo generateActionInfo(MemberInfo info, AccessAction action)
        {
            var actionInfo = new ActionInfo();
            switch (info)
            {
                case MethodInfo methodInfo:
                    actionInfo.OutputType = methodInfo.ReturnType.Name.ToString();
                    foreach (var param in methodInfo.GetParameters())
                    {
                        actionInfo.Parameters.Add(param.Name, param.ParameterType.Name.ToString());
                    }
                    break;
                case PropertyInfo propInfo:
                    actionInfo.OutputType = propInfo.PropertyType.Name.ToString();
                    if (action==AccessAction.set)
                    {
                        actionInfo.Parameters.Add(propInfo.Name, actionInfo.OutputType);
                    }
                    break;
                default:
                    break;
            }
            return actionInfo;
        }
    }
}
