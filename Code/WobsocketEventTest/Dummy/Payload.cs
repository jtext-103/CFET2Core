using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.WebsocketEvent.Test.Dummy
{
    /// <summary>
    /// a fake payload send via evet
    /// </summary>
    public class Payload
    {
        public Guid Id { get; set; }

        public List<int> Records { get; set; }

        public Payload(int len)
        {
            Id = Guid.NewGuid();
            Records = new List<int>();
            for (int i = 0; i < len; i++)
            {
                Records.Add((new Random()).Next(-900000, 900000));
            }
        }

        public Payload()
        {

        }

        public Payload(int len,int val)
        {
            Id = Guid.NewGuid();
            Records = new List<int>();
            for (int i = 0; i < len; i++)
            {
                Records.Add(val);
            }
        }

    }
}
