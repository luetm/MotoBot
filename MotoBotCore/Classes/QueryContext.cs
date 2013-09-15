using MotoBotCore.Interfaces;

namespace MotoBotCore.Classes
{
    public class QueryContext
    {
        public IBot Bot { get; private set; }
        public IUser User { get; private set; }
        public IChannel Channel { get; private set; }
        public bool IsPrivateMessage { get; private set; }
        
        public QueryContext(IBot bot, IUser user, IChannel channel, bool isPrivateMessage)
        {
            Bot = bot;
            User = user;
            Channel = channel;
            IsPrivateMessage = isPrivateMessage;
        }
    }
}
