using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Attributes
{
    /// <summary>
    /// By defual config  support get set and even you accidential change it, it will just ignore missing get and set, it will support this any way
    /// Note: a config can return either sample of bare bone values when people get, they always get samples. when getting a config, it the same as getting a status. but when user set or add of delete config it all ways be the bare bone value.
    /// for GetAndSet, they are preserved for property with public getter and setter, the type can be sample or bare bone value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class Cfet2ConfigAttribute : Cfet2AttributeBase
    {
        public Cfet2ConfigAttribute()
        {
        }

        public Cfet2ConfigAttribute(string name) : base(name)
        {
        }

        public Cfet2ConfigAttribute(ConfigAction action,string name=null) : base(name)
        {
            ConfigActions = action;
        }


        public ConfigAction ConfigActions { get; set; } = ConfigAction.GetAndSet;
    }


    /// <summary>
    /// /By defual config  support get set and even you accidential change it, it will just ignore missing get and set, it will support this any way
    /// Note: a config can return either sample of bare bone values when people get, they always get samples. when getting a config, it the same as getting a status. but when user set or add of delete config it all ways be the bare bone value.
    /// for GetAndSet, they are preserved for property with public getter and setter, the type can be sample or bare bone value.
    /// </summary>
    public enum ConfigAction { Get, Set, Insert, Delete,GetAndSet }
}
