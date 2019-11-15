using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Sample
{
    /// <summary>
    /// a simple inter face for all samples
    /// </summary>
    public interface ISample
    {

        [Obsolete("This is useless try use the sample constructor to cast ti another generic type sample")]
        SampleBase<TVal> CastValue<TVal>();

        /// <summary>
        /// get the value of the sample as an object
        /// </summary>
        object ObjectVal { get; }

        /// <summary>
        /// get the value of the sample as the tpye of TVal
        /// </summary>
        /// <typeparam name="TVal">it can be smple cast of complex convert</typeparam>
        /// <returns></returns>
        TVal GetVal<TVal>();

        /// <summary>
        /// the context of the sample, every thing should be stored here, other property just get/set value from it
        /// change "set" attribute to public
        /// </summary>
        Dictionary<string, object> Context { get; set; }

        /// <summary>
        /// if this sample is a valid sample, if not you should not use the Value in this sample.
        /// </summary>
        bool IsValid{get; }

        /// <summary>
        /// if this sample is from a remote host
        /// </summary>
        bool IsRemote { get; set; }

        /// <summary>
        /// add an error message to the sample, indicating what's wrong with this sample. 
        /// one called, the sample became invalid
        /// </summary>
        /// <param name="msg"></param>
        ISample AddErrorMessage(string msg);

        /// <summary>
        /// if this sample is invalid, this field usually contains error message of this sample.
        /// </summary>
        IEnumerable<string> ErrorMessages { get; }

        /// <summary>
        /// tha path of the resource that generate the sample works mainly on get sample
        /// </summary>
        string Path { get; }

        /// <summary>
        /// the the path of the sample by the resources, use in get action, this is a fluent function
        /// </summary>
        /// <param name="path"></param>
        ISample SetPath(string path);

        /// <summary>
        /// the type of the sample todo: rename this and implement in sample base as return the type name
        /// </summary>
        ResourceTypes ResourceType { get;} 
    }
    public enum ResourceTypes
    {
        Abstract,
        Status,
        Config,
        Method,
        Thing
    };
}
