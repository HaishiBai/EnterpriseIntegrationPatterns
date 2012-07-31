using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Infrastructure.Configuraiton;
using IntegrationPatterns.Interfaces;
using IntegrationPatterns.Routers;

namespace IntegrationPatterns.Infrastructure
{
    public class ProcessingUnitHost
    {
        private Dictionary<string, ProcessingUnit> mProcessingUnits = new Dictionary<string, ProcessingUnit>();
        public void AddProcessingUnit(string key, ProcessingUnit unit)
        {
            mProcessingUnits.Add(key, unit);
        }
        public ProcessingUnit GetProcessingUnit(string key)
        {
            if (mProcessingUnits.ContainsKey(key))
                return mProcessingUnits[key];
            else
                return null;
        }
        public void Open()
        {
            foreach (var unit in mProcessingUnits.Values)
            {
                unit.InputChannels.Open();
                unit.OutputChannels.Open();
                if (unit is DynamicRouter)
                    ((DynamicRouter)unit).ControlChannels.Open();
            }
        }
        public void Close()
        {
            foreach (var unit in mProcessingUnits.Values)
            {
                unit.InputChannels.Close();
                unit.OutputChannels.Close();
                if (unit is DynamicRouter)
                    ((DynamicRouter)unit).ControlChannels.Close();
            }
        }
        public static ProcessingUnitHost FromConfiguration(MessagePipelineSection configuration)
        {
            ProcessingUnitHost host = new ProcessingUnitHost();
            Dictionary<string,IChannel> mChannelLookup = new Dictionary<string,IChannel>();
            for (int i = 0; i < configuration.Pipelines.Count; i++)
            {
                for (int j = 0; j < configuration.Pipelines[i].Channels.Count; j++)
                {
                    var channel = configuration.Pipelines[i].Channels[j];
                    if (channel.Scheme != "sb")
                        throw new NotImplementedException("Schemes other than 'sb' are not supported.");
                    //TODO: this is not nice... other/better way to do injection (support custom implementation)
                    mChannelLookup.Add(channel.Name, (IChannel)Activator.CreateInstance
                    (Type.GetType("IntegrationPatterns.ServiceBus.Channel, IntegrationPatterns.ServiceBus"),
                     channel.Name));

                }
                for (int j = 0; j < configuration.Pipelines[i].ProcessingUnits.Count; j++)
                {
                    var config = configuration.Pipelines[i].ProcessingUnits[j];
                    var unit = Activator.CreateInstance(Type.GetType(config.UnitType));
                    fixChannelReferences(((ProcessingUnit)unit).InputChannels, config.Inputs, mChannelLookup);
                    fixChannelReferences(((ProcessingUnit)unit).OutputChannels, config.Outputs, mChannelLookup);
                    fixChannelReferences(((ProcessingUnit)unit).ControlChannels, config.Controls, mChannelLookup);
                    host.AddProcessingUnit(config.Name, (ProcessingUnit)unit);
                }
            }
            return host;
        }
        private static void fixChannelReferences(ChannelCollection<IChannel> collection,
                                            ChannelReferencesCollection reference,
                                            Dictionary<string, IChannel> lookup)
        {
            for (int k = 0; k < reference.Count; k++)
            {
                if (!lookup.ContainsKey(reference[k].Name))
                    throw new KeyNotFoundException(string.Format("Channel '{0}' undefined.", reference[k].Name));
                collection.Add(lookup[reference[k].Name]);
            }
        }
    }
}
