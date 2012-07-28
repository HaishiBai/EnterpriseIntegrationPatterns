using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Interfaces;

namespace IntegrationPatterns.Infrastructure
{
    public class ChannelMessageEventArgs: EventArgs
    {
        public IChannel Channel { get; private set; }
        public IMessage Message { get; private set; }
        public ChannelMessageEventArgs(IChannel channel, IMessage message)
        {
            Channel = channel;
            Message = message;
        }
    }
}
