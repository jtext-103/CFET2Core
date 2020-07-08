using Jtext103.CFET2.Core.Exception;
using Jtext103.CFET2.Core.Resource;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Extension
{
    public static class HeplerExtensions
    {
        //todo: split this file into specific helpers

        /// <summary>
        /// this is a helper, 
        /// first it will try convertable for primitives
        /// convert an object to a specified type, this will try its best to convert using JSON and Jobject Jarray, 
        /// if convert is not possible by the json string is not deserializable into target type, exception will be thrown,
        /// if convert is not possible by the target object not matching the deserialized object, it will try its best to fit properties into the target, 
        /// those can not be fitted will remain its defualt value.
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <param name="targetType"></param>
        /// <param name="forceConvert">force convert to another type even it is assignable, default is false means if it is assignabl to the target 
        /// type no convertion will happen, just return</param>
        /// <returns>object of the target type</returns>
        public static object TryConvertTo(this object sourceObj, Type targetType,bool forceConvert=false)
        {

            if (forceConvert==false && targetType.IsAssignableFrom(sourceObj.GetType()))
            {
                return sourceObj;
            }
            //first thing first check if it is a convertable
            //try use convert to make it work for value types, later can be mapper
            //could put in a helper
            if (sourceObj is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
            {
                return Convert.ChangeType(sourceObj, targetType);
            }

            //if not convertable then use json to convert
            switch (sourceObj)
            {
                case JObject jObj:
                    return jObj.ToObject(targetType);
                case JArray jArray:
                    return jArray.ToObject(targetType);
                default:
                    //at this stage there could be a case that it is not a poco nor array nor a primitvie that is convertable
                    //that it will throw
                    try
                    {
                        //not an array
                        return JObject.FromObject(sourceObj).ToObject(targetType);
                    }
                    catch (System.Exception)
                    {
                        //may be an array, if the above the target type is array it will also thrown
                        return JArray.FromObject(sourceObj).ToObject(targetType);
                    }
            }
        }


        /// <summary>
        /// helper it maps the dictionary (parameter name,parameters into a right sequence input array)
        /// </summary>
        /// <param name="inputs">if the first is DictionaryInputsIndicator then this will do the trick else it does nothing</param>
        /// <param name="methodParameters"></param>
        /// <returns>input array in the order of the method parameters</returns>
        public static object[] MapInputDictinary(this object[] inputs, MethodInfo method)
        {
            var methodParameters = method.GetParameters();
            var newInputs = new List<object>();
            if (inputs.Length > 1 && inputs[0] is DictionaryInputsIndicator)
            {
                var inputDict = inputs[1] as Dictionary<string, object>;
                for (int i = 0; i < methodParameters.Count(); i++)
                {
                    if (inputDict.ContainsKey(methodParameters[i].Name))
                    {
                        newInputs.Add(inputDict[methodParameters[i].Name]);
                    }
                    else if (i != methodParameters.Count() - 1)     //last one missing is ok, if the last has default value it will be used later
                    {
                        //if not last on try to add default value, if there is not default value then throw
                        if (methodParameters[i].HasDefaultValue)
                        {
                            newInputs.Add(methodParameters[i].DefaultValue);
                            continue;
                        }
                        //newInputs.Add(null);    //you have to fill this missing parameter or else the rest will be screwed 
                        throw new BadResourceRequestException();    //missing parameter is not ok!!
                    }
                }
                //if there is a last parameter, add to last
                if (inputDict.ContainsKey(CommonConstants.TheLastInputsKey))
                {                   
                    newInputs.Add(inputDict[CommonConstants.TheLastInputsKey]);
                }
                return newInputs.ToArray();
            }
            return inputs;
        }


        /// <summary>
        /// this is a helper, 
        /// map the inputs object to the coorected type according to the parameters for a method,  
        /// if the object is not mapped sussesfully the original object will just be put into place(todo? throw exceptions?)  
        /// It works like: if there is just right number of parameters that just use it as input, if there are less input then the method parameters try using default, if there 
        /// are more, put the excessive parameters in params []
        /// </summary>
        /// <param name="inputs">the orignal input</param>
        /// <param name="methodParameters">the parameters for a method</param>
        /// <returns>mapped parameters</returns>
        public static object[] MapInputParameters(this object[] inputs, ParameterInfo[] methodParameters)
        {
            var outputList = new List<object>();
            //iterate throhg the object
            for (int i = 0; i < methodParameters.Count(); i++)
            {
                if (i >= inputs.Count())
                {
                    //use default parameters
                    if (methodParameters[i].HasDefaultValue)
                    {
                        outputList.Add(methodParameters[i].DefaultValue);
                        continue;
                    }
                    //not enough input
                    throw new System.Exception("Parameter Number does not match");
                }

                if (i== methodParameters.Count()-1)
                {
                    //check if the last paremeter is params [], if so add all the input left to the params
                    if (methodParameters[i].IsDefined(typeof(ParamArrayAttribute), false))
                    {
                        var elementType = methodParameters[i].ParameterType.GetElementType();
                        var paramsCount = inputs.Count() - methodParameters.Count() + 1;
                        var paramsArray = Array.CreateInstance(elementType, paramsCount);
                        for (int j = i; j < inputs.Count(); j++)
                        {
                            var paramsElement = inputs[j].TryConvertTo(elementType);
                            paramsArray.SetValue( paramsElement, j - i);
                        }
                        outputList.Add(paramsArray);
                        continue;
                    }
                }

                if (inputs[i] == null)
                { //null does not need to be converted
                    outputList.Add(inputs[i]);
                    continue;
                }
                //if not convert using json
                try
                {
                    var tParam = inputs[i].TryConvertTo(methodParameters[i].ParameterType);
                    outputList.Add(tParam);
                }
                catch (System.Exception e)
                {
                    throw new System.Exception("Input Parameter wrong for parameter: " +
                        methodParameters[i].Name+". Error:"+ e.Message);
                }
                
            }
            return outputList.ToArray();
        }

        /// <summary>
        /// return a readble parameter info list
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<string> FormatParameters(this ParameterInfo[] parameters)
        {
            return parameters.ToList().Select(p=>p.ToString()).ToList();
        }

        /// <summary>
        /// get a sub set of a array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] RangeSubset<T>(this T[] array, int startIndex, int length)
        {
            if (array.Count() == 0)
            {
                return array;
            }
            T[] subset = new T[length];
            Array.Copy(array, startIndex, subset, 0, length);
            return subset;
        }


        /// <summary>
        /// get the parent path of an uri
        /// </summary>
        /// <param name="absolutUri"></param>
        /// <param name="isDirectory">if the parent is a directory, if not the trailing / is removed</param>
        /// <returns></returns>
        public static (string ParentPath,string ChildName) GetParentPath(this Uri absolutUri,bool isDirectory=false)
        {
            var parentPath = string.Join("", absolutUri.Segments.RangeSubset(0, absolutUri.Segments.Length-1));
            var childName = absolutUri.Segments[absolutUri.Segments.Length - 1];
            if (childName.EndsWith(@"/"))
            {
                childName = childName.Substring(0, childName.Length - 1);
            }

            if (isDirectory==false && parentPath.EndsWith(@"/") && parentPath.LastIndexOf(@"/") != 0)
            {
                parentPath = parentPath.Substring(0, parentPath.Length - 1);
            }
            return (parentPath,childName);
        }

        /// <summary>
        /// get parameters form a query string
        /// </summary>
        /// <param name="absolutUri"></param>
        /// <param name="isDirectory">if the parent is a directory, if not the trailing / is removed</param>
        /// <returns></returns>
        public static Dictionary<string,object> ExtractParamsFromQuery(this Uri absolutUri)
        {
            var query = absolutUri.ParseQueryString();
            return query.ToDictionary();
        }

        public static Dictionary<string, object> ToDictionary(this NameValueCollection col)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }
            return dict;
        }



    }
}
