using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleware
{
    [Serializable]
     public class MyMessage
    {
        public string body { get; set; }
        public int someVal { get; set; }
    }
}
