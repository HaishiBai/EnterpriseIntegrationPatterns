using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
