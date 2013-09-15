using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoBotCore.Interfaces;

namespace MotoBotCore.Irc
{
    public class Network : INetwork
    {
        public string Name { get; set; }

        public string Address { get; set; }
        public int Port { get; set; }

        public string Nickname { get; set; }
        public string NicknameAlt { get; set; }

        public IEnumerable<string> OnConnectCommands { get; set; }
    }
}
