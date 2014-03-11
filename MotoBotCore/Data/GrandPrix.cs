using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Data
{
    public class GrandPrix
    {
        public Series Series { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public List<Session> Sessions { get; set; }
    }
}
