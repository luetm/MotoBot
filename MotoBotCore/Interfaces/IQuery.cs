using MotoBotCore.Classes;
using MotoBotCore.Enums;

namespace MotoBotCore.Interfaces
{
    /// <summary>
    /// All queries implement this interface.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// The description of the query, shown in the /help command.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The privilege needed to run the query.
        /// </summary>
        QueryPrivilegeLevel PrivilegeLevel { get; set; }


        /// <summary>
        /// Identifies, if the given command applies to this syntax. If so, this method returns true.
        /// </summary>
        /// <param name="cmd">The command to be tested.</param>
        /// <returns></returns>
        bool CanExecute(string cmd);

        /// <summary>
        /// Execute the query with a given command.
        /// </summary>
        /// <param name="cmd">The command to be executed.</param>
        /// <param name="context">Query context, providing information about the call.</param>
        void Execute(string cmd, QueryContext context);
    }
}
