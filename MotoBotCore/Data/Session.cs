using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Data
{
    public class Session
    {
        public GrandPrix GrandPrix { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeUtc { get; set; }
    }
}
