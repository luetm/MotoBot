using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using IrcDotNet;
using MotoBotCore.Classes;
using MotoBotCore.Enums;
using MotoBotCore.Interfaces;

namespace MotoBotCore.Irc
{
    [Export(typeof(IBot))]
    public class Bot : IBot
    {
        #region Public Properties

        public IrcClient Client { get; private set; }
        public List<IChannel> Channels { get; private set; }
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
            Client = new IrcClient();
            Network = network;

            // Hook up to events
            Client.ChannelListReceived += OnChannelListReceived;
            Client.ClientInfoReceived += OnClientInfoReceived;
            Client.Connected += OnConnected;
            Client.ConnectFailed += OnError;
            Client.Disconnected += OnDisconnected;
            Client.Error += OnError;
            Client.ErrorMessageReceived += OnErrorMessage;
            Client.MotdReceived += OnMotd; // << USED AS *CONNECTED* FOR NOW
            Client.NetworkInformationReceived += NetworkInformationReceived;
            Client.ProtocolError += OnProtoclError;
            Client.RawMessageReceived += OnMessage;
            Client.Registered += OnRegistered;
            Client.ServerBounce += OnServerBounce;

            // Connect
            var registration = new IrcUserRegistrationInfo
            {
                NickName = network.Nickname,
                Password = "none",
                RealName = network.Nickname,
                UserName = network.Nickname,
            };
            Client.Connect(network.Address, network.Port, false, registration);
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
            Client.SendRawMessage("join {0}".F(channel));
        }

        /// <summary>
        /// Part from a channel.
        /// </summary>
        /// <param name="channel"></param>
        public void Part(string channel)
        {
            Client.SendRawMessage("part {0}".F(channel));
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
            var ircUser = Client.Channels
                .SelectMany(c => c.Users)
                .FirstOrDefault(u => u.User.NickName == user.Name);

            // Check if found
            if (ircUser == null)
                throw new InvalidOperationException("User {0} not found.".F(user.Name));

            // Message
            foreach (var line in text.Split(new [] {'\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                Client.LocalUser.SendMessage(ircUser.User, line);
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
            var ircChannel = Client.Channels
                .FirstOrDefault(c => c.Name == channel.Name);

            // Check if found
            if (ircChannel == null)
                throw new InvalidOperationException("Channel {0} not found.".F(channel.Name));

            // Message
            Client.LocalUser.SendMessage(ircChannel, text);
        }


        #region Event Methods OnXyz

        /// <summary>
        /// Run when connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnConnected(object sender, EventArgs eventArgs)
        {
            
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
        private void OnError(object sender, IrcErrorEventArgs e)
        {
            Debugger.Break();
        }

        /// <summary>
        /// Run when an error message was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnErrorMessage(object sender, IrcErrorMessageEventArgs e)
        {
            Debugger.Break();
        }

        /// <summary>
        /// Run when the channel list was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChannelListReceived(object sender, IrcChannelListReceivedEventArgs e)
        {
            Channels = e.Channels
                .Select(c => new Channel(c.Topic, c.Name) as IChannel)
                .ToList();
        }

        /// <summary>
        /// Run when client info was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientInfoReceived(object sender, EventArgs e)
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
                    Client.SendRawMessage(cmd);
                    Thread.Sleep(500);
                }
            }
        }

        /// <summary>
        /// Run when network info was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NetworkInformationReceived(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Run when a protocol error occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProtoclError(object sender, IrcProtocolErrorEventArgs e)
        {
            Debugger.Break();
        }

        /// <summary>
        /// Run when a message was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessage(object sender, IrcRawMessageEventArgs e)
        {
            // On message by a user
            if (e.Message.Command == "PRIVMSG")
            {
                if (e.Message.Source is IrcUser)
                {
                    // If in channel
                    if (e.Message.Parameters[0].StartsWith("#"))
                    {
                        var user = new User(e.Message.Source.Name, UserMode.None);
                        var channel = new Channel("", e.Message.Parameters[0]);
                        var context = new QueryContext(this, user, channel, false);

                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new MessageEventArgs(context, e.Message.Parameters[1]));
                        }
                    }

                    // If in PM
                    else
                    {
                        var user = new User(e.Message.Source.Name, UserMode.None);
                        var context = new QueryContext(this, user, null, true);

                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new MessageEventArgs(context, e.Message.Parameters[1]));
                        }
                    }
                }
            }
            // On system message
            else
            {
                if (SystemMessageReceived != null)
                {
                    SystemMessageReceived(this, new SystemMessageEventArgs(e.RawContent));
                }
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

        /// <summary>
        /// Run when the server sent a bounce message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnServerBounce(object sender, IrcServerInfoEventArgs e)
        {
            
        }

        #endregion
        #endregion
    }
}
