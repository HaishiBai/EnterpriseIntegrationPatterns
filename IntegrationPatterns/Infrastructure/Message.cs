using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Interfaces;

namespace IntegrationPatterns.Infrastructure
{
    [Serializable]
    public class Message: IMessage
    {
        public object Body { get; private set; }
        public Dictionary<string, object> Headers { get; private set; }
        public Message(object body)
        {
            Body = body;
            Headers = new Dictionary<string, object>();
        }
    }
}
