using System;
using System.ComponentModel.Composition;
using System.Text;
using System.Text.RegularExpressions;
using MotoBotCore;
using MotoBotCore.Classes;
using MotoBotCore.Enums;
using MotoBotCore.Interfaces;

namespace MotoBotCore.Queries
{
    public class HelpQuery : IQuery
    {
        /// <summary>
        /// See interface.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// See interface.
        /// </summary>
        public QueryPrivilegeLevel PrivilegeLevel { get; set; }


        [Import]
        public IInformationRepository InformationRepository { get; set; }


        // Query regex
        private const string Regex = @"^help$";


        /// <summary>
        /// Ctor.
        /// </summary>
        public HelpQuery()
        {
            Description = "Displays the help.";
            PrivilegeLevel = QueryPrivilegeLevel.Public;
        }


        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool CanExecute(string cmd)
        {
            var regex = new Regex(Regex);
            return regex.IsMatch(cmd);
        }

        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="context"></param>
        public void Execute(string cmd, QueryContext context)
        {
            if (!CanExecute(cmd))
                throw new InvalidOperationException("You cannot execute the query with this command. Check CanExecute() first.");

            var helpText = InformationRepository.ReadAllFromFile("help.txt", Encoding.UTF8);
            context.Bot.MessageUser(context.User, helpText);
        }
    }
}
