using System;

namespace Jtext103.CFET2.Core.Log
{
    public interface ICfet2LogProvider
    {
        ICfet2Logger GetLogger(string name);
        
    }
}