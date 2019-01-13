using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Core.Log.Dummy
{
    public class DummyLogger : ICfet2Logger
    {
        public DummyLogger(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void Debug(string message)
        {
            Console.WriteLine("Debug - " + Name + " :" + message);
        }

        public void Error(string message)
        {
            Console.WriteLine("Error - " + Name + " :" + message);
        }

        public void Fatal(string message)
        {
            Console.WriteLine("Fatal - " + Name + " :" + message);
        }

        public void Info(string message)
        {
            Console.WriteLine("Info - " + Name + " :" + message);
        }

        public void Trace(string message)
        {
            Console.WriteLine("Trace - " + Name + " :" + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine("Warn - " + Name + " :" + message);
        }
    }
}
