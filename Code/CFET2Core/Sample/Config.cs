using Jtext103.CFET2.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Sample
{
    public class Config<T> : Status<T>
    {
        public override ResourceTypes ResourceType { get; set; } = ResourceTypes.Config;
        #region static fields
        public static readonly string KEYOFSUPORTEDACTION = "CFET2CORE_SAMPLE_SUPORTACTION";


        #endregion

        /// <summary>
        /// user can get the availabe status of the sample by only the resource can set if
        /// </summary>
        public ConfigAction[] SupportedActions
        {
            get
            {
                return (ConfigAction[])Context[KEYOFSUPORTEDACTION];
            }
            internal set
            {
                Context[KEYOFSUPORTEDACTION] = value;
            }
        }

        #region ctor

        public Config():base()
        {
            //by defual it support get set and even you accidential change it, it will just ignore missing get and set, it will support this any way
            Context[KEYOFSUPORTEDACTION] = new ConfigAction[] { ConfigAction.Get, ConfigAction.Set };
            ResourceType = ResourceTypes.Config;
        }

        public Config(T initVal) : base(initVal)
        {
            //by defual it support get set and even you accidential change it, it will just ignore missing get and set, it will support this any way
            Context[KEYOFSUPORTEDACTION] = new ConfigAction[] { ConfigAction.Get, ConfigAction.Set };
            ResourceType = ResourceTypes.Config;
        }

        public Config(Dictionary<string, object> context) : base(context)
        {
            //by defual it support get set and even you accidential change it, it will just ignore missing get and set, it will support this any way
            Context[KEYOFSUPORTEDACTION] = new ConfigAction[] { ConfigAction.Get, ConfigAction.Set };
            ResourceType = ResourceTypes.Config;
        }

        public Config(T initVal, bool isValid) : base(initVal, isValid)
        {
            //by defual it support get set and even you accidential change it, it will just ignore missing get and set, it will support this any way
            Context[KEYOFSUPORTEDACTION] = new ConfigAction[] { ConfigAction.Get, ConfigAction.Set };
            ResourceType = ResourceTypes.Config;
        }

#endregion

    }

}
