using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Test.TestDummies
{
    public class DisposibleThing : Thing, IDisposable
    {
        public static int disposeCount = 0;
        public void Dispose()
        {
            disposeCount++;
        }
    }
}
