using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Infrastructure;

namespace IntegrationPatterns.Interfaces
{
    public interface IChannel
    {
        event EventHandler<ChannelMessageEventArgs> MessageReceived;
        string Name { get;}
        void Open();
        void Send(IMessage value);
    }
}
