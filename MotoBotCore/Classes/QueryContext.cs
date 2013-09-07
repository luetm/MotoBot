using MotoBotCore.Interfaces;

namespace MotoBotCore.Classes
{
    public class QueryContext
    {
        public IBot Bot { get; private set; }
        public IUser User { get; private set; }

        public QueryContext(IBot bot, IUser user)
        {
            Bot = bot;
            User = user;
        }
    }
}
