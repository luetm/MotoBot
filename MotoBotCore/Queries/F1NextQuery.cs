﻿using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using MotoBotCore.Classes;
using MotoBotCore.Data;
using MotoBotCore.Enums;
using MotoBotCore.Interfaces;

namespace MotoBotCore.Queries
{
    public class F1NextQuery : IQuery
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
        private const string Regex = @"^f1next( (\w+/\w+))?$";


        /// <summary>
        /// Creates a new F1NextQuery().
        /// </summary>
        public F1NextQuery()
        {
            Description = "Shows the time to ne next Formula 1 session.";
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

            if (cmd == "f1next")
            {
                var next = InformationRepository.GetNextSession(Series.Formula1);
                var now = DateTime.UtcNow;

                Output(next, next.DateTimeUtc.ToLongDateString() + " UTC", next.DateTimeUtc - now, context);
            }
            else
            {
                throw new NotImplementedException("Time Zones are horrible.");
            }
        }


        /// <summary>
        /// Outputs the result.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="timeString"></param>
        /// <param name="offset"></param>
        /// <param name="context"></param>
        private void Output(Session session, string timeString, TimeSpan offset, QueryContext context)
        {
            string timeText;
            if (offset.Days > 0)
                timeText = "{0} days, {1} hours, {2} minutes, {3} seconds".F(offset.Days, offset.Hours, offset.Minutes, offset.Seconds);
            else if (offset.Hours > 0)
                timeText = "{1} hours, {2} minutes, {3} seconds".F(offset.Days, offset.Hours, offset.Minutes, offset.Seconds);
            else if (offset.Minutes > 0)
                timeText = "{2} minutes, {3} seconds".F(offset.Days, offset.Hours, offset.Minutes, offset.Seconds);
            else
                timeText = "{3} seconds".F(offset.Days, offset.Hours, offset.Minutes, offset.Seconds);
            
            context.Bot.MessageChannel(context.Channel, "Next GP: Race #{0} - {1}".F(session.GrandPrix.Number, session.GrandPrix.Name));
            context.Bot.MessageChannel(context.Channel, "Next Session: [{0}] in {1}".F((object)timeString, timeText));
        }
    }
}