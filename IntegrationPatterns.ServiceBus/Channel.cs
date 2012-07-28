using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using IntegrationPatterns.Infrastructure;
using IntegrationPatterns.Interfaces;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace IntegrationPatterns.ServiceBus
{
    public class Channel: IChannel
    {
        protected QueueClient myClient;
        protected bool myRunningState = false;
        public string Name { get; private set; }
        public event EventHandler<ChannelMessageEventArgs> MessageReceived;
        public delegate void ProcessMessage(IMessage message);
        public Channel()
            : this(Guid.NewGuid().ToString("N"))
        {
        }
        protected void RaiseMessageReceivedEvent(object sender, ChannelMessageEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(sender, e);
        }
        public Channel(string name)
        {
            Name = name;
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager =NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(name))
                namespaceManager.CreateQueue(name);
            myClient = QueueClient.CreateFromConnectionString(connectionString, name, ReceiveMode.PeekLock);
        }
        public void Open()
        {
            myRunningState = true;
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                while (myRunningState)
                {
                    try
                    {
                        var message = myClient.Receive(new TimeSpan(0, 0, 1));
                        if (message != null)
                        {                               
                            RaiseMessageReceivedEvent(this, new Infrastructure.ChannelMessageEventArgs(this, SBMessage.FromBrokeredMessage(message)));
                            message.Complete();
                        }
                    }
                    catch (Exception exp)
                    {
                        Trace.TraceError(exp.ToString());
                    }
                }
            });
        }
        public void Send(IMessage value)
        {
            if (value != null)
            {
                BrokeredMessage message = null;
                if (value is SBMessage)
                    message = ((SBMessage)value).ToBrokeredMessage();
                else
                {
                    message = new BrokeredMessage(value);
                    message.ContentType = value.GetType().AssemblyQualifiedName;
                }
                myClient.Send(message);
            }
        }
    }
}
