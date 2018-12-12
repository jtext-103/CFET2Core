using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Sample
{
    /// <summary>
    /// extesion method related to sample
    /// </summary>
    public static class SampleExtension
    {

        /// <summary>
        /// wrap any object into a sample of given type,
        /// if it is already a sample the ignore
        /// </summary>
        /// <param name="val"></param>
        /// <param name="genericSampleType">genericSampleType, status? config ?</param>
        /// <returns>wrapped sample</returns>
        public static ISample ToSample(this object val, Type genericSampleType)
        {
            if (val is ISample)
            {
                var sample = val as ISample;
                Type valType;
                if (sample.ObjectVal != null)
                {
                    valType = sample.ObjectVal.GetType();
                }
                else
                {
                    valType = typeof(object);
                }
                Type sampleClass = genericSampleType.MakeGenericType(valType);
                object created = Activator.CreateInstance(sampleClass, sample.Context);
                return (ISample)created;
            }
            else
            {
                Type valType;
                if (val != null)
                {
                    valType = val.GetType();
                }
                else
                {
                    valType = typeof(object);
                }
                Type sampleClass = genericSampleType.MakeGenericType(valType);
                object created = Activator.CreateInstance(sampleClass, val);
                return (ISample)created;
            }
        }


        /// <summary>
        /// wrap any object into a status sample, if it is a sample aready then does nothing but cast
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ISample ToStatus(this object val)
        {
               return val.ToSample(typeof(Status<>));
        }


        /// <summary>
        /// wrap any object into a method sample, if it is a sample aready then does nothing but cast
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ISample ToMethod(this object val)
        {
            return val.ToSample(typeof(Method<>));
        }



        /// <summary>
        /// wrap any object into a config sample, if it is a sample aready then does nothing but cast
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ISample ToConfig(this object val)
        {

                return val.ToSample(typeof(Config<>));

        }

    }
}
