using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Interfaces
{
    public interface INetwork
    {
        string Name { get; set; }

        string Address { get; set; }
        int Port { get; set; }

        string Nickname { get; set; }
        string NicknameAlt { get; set; }

        IEnumerable<string> OnConnectCommands { get; set; }
    }
}
