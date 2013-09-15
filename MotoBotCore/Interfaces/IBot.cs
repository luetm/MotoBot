using System;
using MotoBotCore.Classes;

namespace MotoBotCore.Interfaces
{
    public interface IBot
    {   
        // Properties
        INetwork Network { get; set; }
        bool IsConnected { get; }


        // Methods
        void Connect(INetwork network);
        void Disconnect(string message = "");

        void Join(string channel);
        void Part(string channel);

        void MessageUser(IUser user, string text);
        void MessageChannel(IChannel channel, string text);

        // Events
        event EventHandler<EventArgs> Connected;
        event EventHandler<EventArgs> Disconnected;
        event EventHandler<ErrorEventArgs> ErrorReceived;
        event EventHandler<MessageEventArgs> MessageReceived;
        event EventHandler<SystemMessageEventArgs> SystemMessageReceived;
    }
}
