using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.ServiceBus.Messaging;

namespace IntegrationPatterns.ServiceBus
{
    public static class BrokeredMessageExtensions
    {
        public static object GetBody(this BrokeredMessage message)
        {
            
            var method = typeof(BrokeredMessage).GetMethod("GetBody", new Type[]{});
            var genericMethod = method.MakeGenericMethod(new Type[]{Type.GetType(message.ContentType)});
            return genericMethod.Invoke(message, null);
        }
    }
}
