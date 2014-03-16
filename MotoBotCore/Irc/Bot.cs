using Meebey.SmartIrc4net;
using MotoBotCore.Classes;
using MotoBotCore.Enums;
using MotoBotCore.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ErrorEventArgs = MotoBotCore.Classes.ErrorEventArgs;

namespace MotoBotCore.Irc
{
    [Export(typeof(IBot))]
    public class Bot : IBot
    {
        #region Public Properties

        public IrcClient Client { get; private set; }

        public List<IChannel> Channels
        {
            get
            {
                return Client.JoinedChannels
                    .Cast<string>()
                    .Select(x => new Channel("", x))
                    .Cast<IChannel>()
                    .ToList();
            }
        }

        public INetwork Network { get; set; }

        public bool IsConnected
        {
            get { return Client != null && Client.IsConnected; }
        }

        #endregion

        #region Private Fields



        #endregion

        #region Events

        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<ErrorEventArgs> ErrorReceived;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<SystemMessageEventArgs> SystemMessageReceived;

        #endregion

        #region Methods

        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="network"></param>
        public void Connect(INetwork network)
        {
            // Create client
            Client = new IrcClient
            {
                Encoding = Encoding.UTF8,
                SendDelay = 200,
                ActiveChannelSyncing = true,
            };

            Network = network;


            // Hook up to events
            Client.OnList += OnChannelListReceived;
            Client.OnConnected += OnConnected;
            Client.OnConnectionError += OnError;
            Client.OnDisconnected += OnDisconnected;
            Client.OnError += OnError;
            Client.OnErrorMessage += OnErrorMessage;
            Client.OnMotd += OnMotd; // << USED AS *CONNECTED* FOR NOW
            Client.OnChannelMessage += OnChannelMessage;
            Client.OnQueryMessage += OnQueryMessage;


            Client.OnRawMessage += OnRawMessage;
            Client.OnRegistered += OnRegistered;

            Client.Connect(network.Address, network.Port);
            Task.Run(() => Client.Listen());
        }

        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="message"></param>
        public void Disconnect(string message = "")
        {
            if (Client == null || !Client.IsConnected)
                throw new InvalidOperationException("Not connected.");
        }


        /// <summary>
        /// Joins a channel.
        /// </summary>
        /// <param name="channel">Channel name with #.</param>
        public void Join(string channel)
        {
            Client.RfcJoin(channel);
        }

        /// <summary>
        /// Part from a channel.
        /// </summary>
        /// <param name="channel"></param>
        public void Part(string channel)
        {
            Client.RfcPart(channel);
        }

        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="text"></param>
        public void MessageUser(IUser user, string text)
        {
            // Check if connected
            if (!IsConnected)
                throw new InvalidOperationException("Not connected.");

            // Get irc user from irc client
            var ircUser = Client.GetIrcUser(user.Name);

            // Check if found
            if (ircUser == null)
                throw new InvalidOperationException("User {0} not found.".F(user.Name));

            // Message
            foreach (var line in text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Client.SendMessage(SendType.Message, ircUser.Nick, line);
            }
        }

        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="text"></param>
        public void MessageChannel(IChannel channel, string text)
        {
            // Check if connected
            if (!IsConnected)
                throw new InvalidOperationException("Not connected.");

            // Get irc user from irc client
            var ircChannel = Client.GetChannel(channel.Name);

            // Check if found
            if (ircChannel == null)
                throw new InvalidOperationException("Channel {0} not found.".F(channel.Name));

            // Message
            Client.SendMessage(SendType.Message, ircChannel.Name, text);
        }


        #region Event Methods OnXyz

        /// <summary>
        /// Run when connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnConnected(object sender, EventArgs eventArgs)
        {
            Client.Login(new[] { Network.Nickname, Network.NicknameAlt }, "MotoBot");
        }

        /// <summary>
        /// Run when disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisconnected(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Run when an error occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnError(object sender, EventArgs e)
        {
            Debugger.Break();
        }

        /// <summary>
        /// Run when an error message was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnErrorMessage(object sender, IrcEventArgs e)
        {
            Debugger.Break();
        }

        /// <summary>
        /// Run when the channel list was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChannelListReceived(object sender, ListEventArgs e)
        {

        }


        /// <summary>
        /// On modt ACTS AS CONNECTED.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMotd(object sender, EventArgs e)
        {
            // Raise event.
            if (Connected != null)
            {
                Connected(this, EventArgs.Empty);
            }

            // Perform connect commands.
            if (Network.OnConnectCommands != null)
            {
                foreach (var cmd in Network.OnConnectCommands)
                {
                    Client.WriteLine(cmd);
                    Thread.Sleep(500);
                }
            }
        }

        /// <summary>
        /// Run when a message was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRawMessage(object sender, IrcEventArgs e)
        {
            if (SystemMessageReceived != null)
            {
                SystemMessageReceived(this, new SystemMessageEventArgs(e.Data.RawMessage));
            }
        }

        private void OnChannelMessage(object sender, IrcEventArgs e)
        {
            var user = new User(e.Data.From, UserMode.None);
            var channel = new Channel("", e.Data.Channel);
            var context = new QueryContext(this, user, channel, false);

            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageEventArgs(context, e.Data.Message));
            }
        }

        private void OnQueryMessage(object sender, IrcEventArgs e)
        {
            var user = new User(e.Data.From, UserMode.None);
            var context = new QueryContext(this, user, null, true);

            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageEventArgs(context, e.Data.Message));
            }
        }

        /// <summary>
        /// Run when we registered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRegistered(object sender, EventArgs e)
        {

        }

        public void SendCommand(string message)
        {
            Client.WriteLine(message);
        }

        #endregion
        #endregion
    }
}
