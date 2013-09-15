using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Classes
{
    public class ErrorEventArgs : EventArgs
    {
        public string Error { get; private set; }

        public ErrorEventArgs(string error)
        {
            Error = error;
        }
    }
}
