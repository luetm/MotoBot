using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoBotCore.Interfaces;

namespace MotoBotCore.Irc
{
    public class Channel : IChannel
    {
        public string Name { get; set; }
        public string Motd { get; set; }

        public Channel(string motd, string name)
        {
            Motd = motd;
            Name = name;
        }
    }
}
