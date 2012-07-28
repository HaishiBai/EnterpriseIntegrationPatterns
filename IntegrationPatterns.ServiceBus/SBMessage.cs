using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Interfaces;
using Microsoft.ServiceBus.Messaging;

namespace IntegrationPatterns.ServiceBus
{
    [Serializable]
    public class SBMessage: IMessage
    {
        public object Body { get; private set; }
        public Dictionary<string, object> Headers { get; private set; }
        public SBMessage(object body)
        {
            Body = body;
            Headers = new Dictionary<string, object>();
        }
        public static SBMessage FromBrokeredMessage(BrokeredMessage message)
        {
            SBMessage ret = new SBMessage(message.GetBody());
            foreach (var key in message.Properties.Keys)
                ret.Headers.Add(key, message.Properties[key]);
            return ret;
        }
        public BrokeredMessage ToBrokeredMessage()
        {
            BrokeredMessage ret = new BrokeredMessage(Body);
            ret.ContentType = Body.GetType().AssemblyQualifiedName;
            foreach (var key in Headers.Keys)
                ret.Properties.Add(key, Headers[key]);
            return ret;
        }
    }
}
