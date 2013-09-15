using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Interfaces
{
    public interface IChannel
    {
        string Name { get; set; }
        string Motd { get; set; }
    }
}
