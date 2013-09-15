using MotoBotCore.Enums;
using MotoBotCore.Interfaces;

namespace MotoBotCore.Irc
{   
    /// <summary>
    /// A IRC user.
    /// </summary>
    public class User : IUser
    {
        /// <summary>
        /// Username.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Usermode (@, +, ...)
        /// </summary>
        public UserMode Mode { get; private set; }



        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="name">Username</param>
        /// <param name="mode">Usermode (@, +, ...)</param>
        public User(string name, UserMode mode)
        {
            Name = name;
            Mode = mode;
        }
    }
}
