using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationPatterns.Interfaces
{
    public class ChannelCollectionChangedEventArgs: EventArgs
    {
        public List<IChannel> Channels { get; private set; }
        public CollectionChangedType ChangeType { get; private set; }
        public ChannelCollectionChangedEventArgs(CollectionChangedType changeType, List<IChannel> channels)
        {
            ChangeType = changeType;
            Channels = channels;
        }
    }
    public enum CollectionChangedType
    {
        Add,
        Remove
    }
}
