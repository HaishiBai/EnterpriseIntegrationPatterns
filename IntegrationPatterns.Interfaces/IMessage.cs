using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationPatterns.Interfaces
{
    public interface IMessage
    {
        Dictionary<string, object> Headers { get; }
        object Body { get;}
    }
}
