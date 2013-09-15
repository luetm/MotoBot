using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Classes
{
    public class MessageEventArgs : EventArgs
    {
        public QueryContext QueryContext { get; private set; }
        public string RawMessage { get; private set; }

        public MessageEventArgs(QueryContext queryContext, string rawMessage)
        {
            QueryContext = queryContext;
            RawMessage = rawMessage;
        }
    }
}
