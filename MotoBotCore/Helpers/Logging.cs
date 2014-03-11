using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoBotCore.Helpers
{
    public static class Logging
    {
        public static void Log(string message)
        {
            File.AppendAllText("bot.log", message, Encoding.UTF8);
        }

        public static void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        public static void Log(Exception error)
        {
            var err = error;
            string txt = "";

            while (err != null)
            {
                txt += string.Format("Type: {0}\nMessage: {1}\nTarget Site: {2}\nSource: {3}\n\n", err.GetType().Name, err.Message, err.TargetSite, err.Source);
                err = err.InnerException;
            }

            txt += string.Format("\n\n\nStacktrace:\n{0}", error.StackTrace);
            Log(txt);
        }
    }
}
