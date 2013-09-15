using System;

namespace MotoBotCore.Classes
{
    public class SystemMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public SystemMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
