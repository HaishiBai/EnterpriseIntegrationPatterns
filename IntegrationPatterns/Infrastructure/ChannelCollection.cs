using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Interfaces;

namespace IntegrationPatterns.Infrastructure
{
    public class ChannelCollection<T> where T:IChannel
    {
        public event EventHandler<ChannelMessageEventArgs> MessageReceivedOnChannel;
        public event EventHandler<ChannelCollectionChangedEventArgs> CollectionChanged;
        private List<T> mChannels = new List<T>();
        private int mMultiplicity = 1;
        public ChannelCollection()
            : this(1)
        {
        }
        public ChannelCollection(int multiplicity)
        {
            mMultiplicity = multiplicity;
        }
        public T this[int index]
        {
            get
            {
                return mChannels[index];
            }
            set
            {
                mChannels[index] = value;
            }
        }
        public int Count
        {
            get
            {
                return mChannels.Count;
            }
        }
        public int IndexOf(string name)
        {
            var channel = mChannels.FirstOrDefault(c => c.Name == name);
            if (channel != null)
                return mChannels.IndexOf(channel);
            else
                return -1;
        }
        public int IndexOf(T channel)
        {
            return mChannels.IndexOf(channel);
        }
        public void Add(T channel)
        {
            mChannels.Add(channel);
            channel.MessageReceived += channel_MessageReceived;
            if (CollectionChanged!=null)
                CollectionChanged(this, new ChannelCollectionChangedEventArgs(CollectionChangedType.Add,
                    new List<IChannel>(new IChannel[]{channel})));
        }
        public void Remove(T channel)
        {
            channel.MessageReceived -= channel_MessageReceived;
            mChannels.Remove(channel);
            if (CollectionChanged != null)
                CollectionChanged(this, new ChannelCollectionChangedEventArgs(CollectionChangedType.Remove,
                    new List<IChannel>(new IChannel[] { channel })));
        }
        public void Open()
        {
            foreach (var channel in mChannels)
                channel.Open();
        }
        void channel_MessageReceived(object sender, ChannelMessageEventArgs e)
        {
            if (MessageReceivedOnChannel != null)
                MessageReceivedOnChannel(this, e);
        }
    }
}
