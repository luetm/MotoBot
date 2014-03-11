using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MotoBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                var err = e.Exception;
                string txt = "";

                while (err != null)
                {
                    txt += string.Format("Type: {0}\nMessage: {1}\nTarget Site: {2}\nSource: {3}\n\n", err.GetType().Name, err.Message, err.TargetSite, err.Source);
                    err = err.InnerException;
                }

                txt += string.Format("\n\n\nStacktrace:\n{0}", e.Exception.StackTrace);
                File.WriteAllText("crash.log", txt, Encoding.UTF8);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("Writing to log failed: {0}: {1}", err.GetType().Name, err.Message));
            }
        }
    }
}
