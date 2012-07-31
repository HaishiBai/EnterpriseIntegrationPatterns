using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Infrastructure;
using IntegrationPatterns.Interfaces;

namespace IntegrationPatterns.Routers
{
    public class MessageRouter : ProcessingUnit
    {
        public MessageRouter()
        {
            InputChannels.MessageReceivedOnChannel += InputChannels_MessageReceivedOnChannel;
        }
        void InputChannels_MessageReceivedOnChannel(object sender, ChannelMessageEventArgs e)
        {
                OutputChannels[PickOutputChannel(e)].Send(e.Message);
        }
        private int mChannelIndex = 0;
        protected virtual int PickOutputChannel(ChannelMessageEventArgs e)
        {
            mChannelIndex = OutputChannels.Count > 0 ? (mChannelIndex + 1) % OutputChannels.Count : 0;
            return mChannelIndex;
        }
    }
}
