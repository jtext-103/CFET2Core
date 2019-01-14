using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.WebsocketEvent.Test.Dummy
{
    public class EventEcho:Thing
    {
        public override void TryInit(object initObj)
        {
            base.TryInit(initObj);
            //here give it a list of event to subscribe, tpye: defualt
        }

        public void echo(EventArg e)
        {
            //fire each event as: /echo/{original event path} 
            //sample as old event sample
        }

    }
}
