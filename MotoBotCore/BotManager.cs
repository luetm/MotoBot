using MotoBotCore.Classes;
using MotoBotCore.Interfaces;
using MotoBotCore.Irc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;

namespace MotoBotCore
{
    public class BotManager
    {
        public INetwork Network { get; private set; }
        public Bot Bot { get; private set; }
        public IList<IQuery> Queries { get; private set; }

        public BotManager(INetwork network)
        {
            Initialize(network);
        }

        public BotManager(string botName, string address, int port, string nickname, string nicknameAlt)
        {
            Initialize(new Network
            {
                Address = address,
                Name = botName,
                Nickname = nickname,
                NicknameAlt = nicknameAlt,
                Port = port,
            });
        }


        public void Start()
        {
            Bot.Connect(Network);
            Bot.MessageReceived += OnMessage;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            // We are only interested in ! commands.
            if (!e.RawMessage.StartsWith("!")) return;

            // Get context and command
            var context = e.QueryContext;
            var cmd = e.RawMessage.Substring(1);

            foreach (var query in Queries)
            {
                if (query.CanExecute(cmd))
                {
                    query.Execute(cmd, context);
                    break;
                }
            }
        }


        private void Initialize(INetwork network)
        {
            Network = network;
            Bot = new Bot();

            // Prepare plugin path
            var fi = new FileInfo(GetType().Assembly.Location);
            var catalog = new AggregateCatalog();

            // Create plugin directory if it doesn't exist
            if (fi.DirectoryName != null)
            {
                var pluginPath = new DirectoryInfo(Path.Combine(fi.DirectoryName, "Plugins"));

                if (pluginPath.Exists)
                {
                    pluginPath.Create();
                }
                catalog.Catalogs.Add(new DirectoryCatalog(pluginPath.FullName));
            }

            // Get queries from assemblies
            catalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));

            try
            {
                var container = new CompositionContainer(catalog);
                Queries = container.GetExportedValues<IQuery>().ToList();
            }
            catch (Exception err)
            {
                throw new InvalidOperationException("Could not get queries. Plugin system is broken.", err);
            }
        }
    }
}
