using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Interfaces;

namespace IntegrationPatterns.Infrastructure
{
    public class ProcessingUnit
    {
        public ChannelCollection<IChannel> InputChannels { get; set; }
        public ChannelCollection<IChannel> OutputChannels { get; set; }
        public ProcessingUnit()
        {
            InputChannels = new ChannelCollection<IChannel>(1);
            OutputChannels = new ChannelCollection<IChannel>(Multiplicity.Many);
        }
    }
}
