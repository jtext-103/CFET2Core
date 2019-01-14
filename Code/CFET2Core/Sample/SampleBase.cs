using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Sample
{
    /// <summary>
    /// the resource sample
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SampleBase<T> : ISample
    {
        //todo inplement the follwoing
        /*
        public override bool Equals(object obj)
        {
            var valueObject = obj as T;

            if (ReferenceEquals(valueObject, null))
                return false;

            return EqualsCore(valueObject);
        }

        protected abstract bool EqualsCore(T other);

        public override int GetHashCode()
        {
            return GetHashCodeCore();
        }

        protected abstract int GetHashCodeCore();

        public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
        {
            return !(a == b);
        }
        */


        #region static fields
        public static readonly string KeyOfVal = "CFET2CORE_SAMPLE_VAL";
        public static readonly string KeyOfIsValid = "CFET2CORE_SAMPLE_ISVALID";
        public static readonly string KeyOfErrorMessage = "CFET2CORE_SAMPLE_ERRORMESSAGE";
        public static readonly string KeyOfPath = "CFET2CORE_SAMPLE_PATH";
        public static readonly string KeyOfCchildThing = "CFET2CORE_SAMPLE_CHILDTHING";
        public static readonly string KeyOfParametersList = "CFET2CORE_SAMPLE_PARAMETERSLIST";
        public static readonly string KeyOfResourceType = "CFET2CORE_SAMPLE_RESOURCETYPE";
        public static readonly string KeyOfIsRemote = "CFET2CORE_SAMPLE_ISREMOTE";

        /// <summary>
        /// return an invalid sample
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static SampleBase<object> GetInvalideSample(string errorMessage = null)
        {
            var sample= new SampleBase<object>(null, false);
            if (errorMessage != null)
            {
                sample.AddErrorMessage(errorMessage);
            }
            return sample;
        }

        #endregion



        /// <summary>
        /// holds the context dictionary, note that all the value that a sample holds should be in the context
        /// </summary>
        public Dictionary<string, object> Context { get; protected set; } = new Dictionary<string, object>();


        #region context property



        /// <summary>
        /// the value of this sample
        /// </summary>
        public virtual T Val
        {
            get
            {
                return GetVal<T>();
            }
        }

        /// <summary>
        /// if the thing failed to prove a sample, a empty sampple with IsValid=false will be returned,
        ///  instead of throwing exception
        /// </summary>
        public bool IsValid
        {
            get
            {
                object isValid;
                Context.TryGetValue(KeyOfIsValid,out isValid);
                return (bool)(isValid ?? false);
            }
            internal set
            {
                Context[KeyOfIsValid] = value;
            }

        }


        /// <summary>
        /// returns a List<string> contains the error messages of an invalid sample
        /// </summary>
        public IEnumerable<string> ErrorMessages
        {
            get
            {
                if (Context.ContainsKey(KeyOfErrorMessage) == false || (Context[KeyOfErrorMessage] is List<string>) == false)
                {
                    return new List<string>();
                }
                return Context[KeyOfErrorMessage] as List<string>;
            }
        }

        /// <summary>
        /// the path of the resouce that generate this sample 
        /// </summary>
        public string Path
        {
            get
            {
                object path;
                Context.TryGetValue(KeyOfPath, out path);
                return (string)(path ?? "");
            }
        }

        public virtual ResourceTypes ResourceType
        {
            get
            {
                object type;
                Context.TryGetValue(KeyOfResourceType, out type);
                return (ResourceTypes)(type ?? ResourceTypes.Abstract);
            }
            set
            {
                Context[KeyOfResourceType] = value;
            }
        }

        /// <summary>
        /// is this sample from a remote host
        /// </summary>
        public bool IsRemote
        {
            get
            {
                object isRemote;
                Context.TryGetValue(KeyOfIsRemote, out isRemote);
                return (bool)(isRemote ?? false);
            }
            set
            {
                Context[KeyOfIsRemote] = value;
            }
        }

        #endregion


        /// <summary>
        /// set the path of the resouce that generate this sample
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ISample SetPath(string path)
        {
            Context[KeyOfPath] = path;
            return this;
        }


        /// <summary>
        /// override the to string, return the Val.tostring
        /// </summary>
        /// <returns>sample Value in string</returns>
        public override string ToString()
        {
            if (IsValid == true)
            {
                return Val==null?"NULL object":Val.ToString();
            }
            return "Invalid Sample!, Error Message: " + string.Join(Environment.NewLine, ErrorMessages.ToArray());
        }




        #region ctor

        /// <summary>
        /// create the sample with an initial value
        /// </summary>
        /// <param name="initVal"></param>
        public SampleBase(T initVal)
        {
            ResourceType = ResourceTypes.Abstract;
            Context[KeyOfVal] = initVal;
            SetPath("");
            IsRemote = false;
            IsValid = true;
        }
        /// <summary>
        /// create the sample with an initial value and the thing type 
        /// </summary>
        /// <param name="initVal"></param>
        /// <param name="thingType"></param>
        public SampleBase(T initVal, ResourceTypes resType)
        {
            Context[KeyOfVal] = initVal;
            SetPath("");
            IsRemote = false;
            IsValid = true;
            ResourceType = resType;
        }

        /// <summary>
        /// create the sample with an initial value, and specifies the isValid state
        /// </summary>
        /// <param name="initVal"></param>
        /// <param name="isValid"></param>
        public SampleBase(T initVal, bool isValid)
        {
            ResourceType = ResourceTypes.Abstract;
            Context[KeyOfVal] = initVal;
            IsValid = isValid;
            SetPath("");
            IsRemote = false;
        }

        /// <summary>
        /// To clone a sample from a given sample context
        /// </summary>
        public SampleBase(Dictionary<string, object> context)
        {
            if (context != null)
            {
                Context = new Dictionary<string, object>(context);
            }
            else
            {
                Context[KeyOfVal] = null;
                IsValid = true;
            }
        }

        /// <summary>
        /// reserved for deserialzation do not use derectly
        /// </summary>
        public SampleBase()
        {
            ResourceType = ResourceTypes.Abstract;
            SetPath("");
            IsRemote = false;
            Context[KeyOfVal] = null;
        }

        #endregion


        /// <summary>
        /// simple return Val as an object
        /// </summary>
        public object ObjectVal => this.Val;

        /// <summary>
        /// 
        /// </summary>
       


        /// <summary>
        /// return a copy of this sample but the val in different generic type. 
        /// note, if the val in orignal sample can not be cast into the target type there will be a exception when you access the val
        /// </summary>
        /// <typeparam name="Tval"></typeparam>
        /// <returns></returns>
        [Obsolete("This is useless try use the sample constructor to cast ti another generic type sample")]
        public virtual SampleBase<Tval> CastValue<Tval>()
        {
            return new SampleBase<Tval>(Context);
        }

        public TVal GetVal<TVal>() 
        {
            //try use convert to make it work for value types, later can be mapper
            //could put in a helper
            if (Context[KeyOfVal] is IConvertible && typeof(IConvertible).IsAssignableFrom(typeof(TVal)))
            {
                return (TVal)Convert.ChangeType(Context[KeyOfVal], typeof(TVal));
            }

            try
            {
                return (TVal)Context[KeyOfVal];
            }
            catch (System.Exception)
            {

            }

            try
            {
                var str = JsonConvert.SerializeObject(Context[KeyOfVal]);
                return JsonConvert.DeserializeObject<TVal>(str);
            }
            catch (System.Exception)
            {
                return default(TVal);
            }


        }

        /// <summary>
        /// after called this sample is set to be invalid
        /// </summary>
        /// <param name="msg"></param>
        public ISample AddErrorMessage(string msg)
        {
            if (Context.ContainsKey(KeyOfErrorMessage) == false || (Context[KeyOfErrorMessage] is List<string> ) == false)
            {
                Context[KeyOfErrorMessage] = new List<string>();
            }
            var messages=Context[KeyOfErrorMessage] as List<string>;
            messages.Add(msg);
            IsValid = false;
            return this;
        }
    }
}
