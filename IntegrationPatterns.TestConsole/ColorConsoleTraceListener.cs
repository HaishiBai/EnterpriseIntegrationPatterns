using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace IntegrationPatterns.TestConsole
{
    public class ColorConsoleTraceListener: ConsoleTraceListener
    {
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            var color = Console.ForegroundColor;
            if (eventType == TraceEventType.Information)
                Console.ForegroundColor = ConsoleColor.Blue;
            else if (eventType == TraceEventType.Error)
                Console.ForegroundColor = ConsoleColor.Red;
            base.TraceEvent(eventCache, "", eventType, id, message);
            Console.ForegroundColor = color;
        }
        public override void WriteLine(string message)
        {
            if (message.Contains("\"Microsoft.ServiceBus.ConnectionString\""))
                return;
            base.WriteLine(message);
        }
    }
}
