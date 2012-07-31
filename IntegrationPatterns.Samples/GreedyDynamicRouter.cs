using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Infrastructure;
using IntegrationPatterns.Routers;

namespace IntegrationPatterns.Samples.Routers
{
    public class GreedyDynamicRouter: DynamicRouter
    {
        public GreedyDynamicRouter()
        {
            ControlChannels.MessageReceivedOnChannel += ControlChannels_MessageReceivedOnChannel;
        }

        void ControlChannels_MessageReceivedOnChannel(object sender, Infrastructure.ChannelMessageEventArgs e)
        {
            if (e.Message != null && e.Message.Headers.ContainsKey(HeaderName.PressureValue)
                                  && e.Message.Headers.ContainsKey(HeaderName.PressureChannel))
            {
                string name = (string)e.Message.Headers[HeaderName.PressureChannel];
                if (!mChannelPressure.ContainsKey(name))
                    mChannelPressure.Add(name, 0);
                mChannelPressure[name] += (double)e.Message.Headers[HeaderName.PressureValue];
            }
        }

        private Dictionary<string, double> mChannelPressure = new Dictionary<string, double>();
        protected override int PickOutputChannel(Infrastructure.ChannelMessageEventArgs e)
        {

            for (int i = 0; i < OutputChannels.Count; i++) //TODO: not nice
            {
                if (!mChannelPressure.ContainsKey(OutputChannels[i].Name))
                    mChannelPressure.Add(OutputChannels[i].Name, 0);
            }
            double minPressure = double.MaxValue;
            string minKey = "";
            foreach (string key in mChannelPressure.Keys)
            {
                if (mChannelPressure[key] <= minPressure)
                {
                    minPressure = mChannelPressure[key];
                    minKey = key;
                }
            }
            
            mChannelPressure[minKey] += (double)e.Message.Headers[HeaderName.PressureValue];
            return OutputChannels.IndexOf(minKey);
        }
    }
}
