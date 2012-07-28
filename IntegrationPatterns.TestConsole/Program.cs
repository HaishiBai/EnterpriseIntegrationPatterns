using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using IntegrationPatterns.Infrastructure;
using IntegrationPatterns.Interfaces;
using IntegrationPatterns.Routers;
using IntegrationPatterns.ServiceBus;

namespace IntegrationPatterns.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            cbanner(ConsoleColor.Cyan, "Enterprise Integration Patterns Test Console v1.0");

            DynamicRouter();

            cprintln(ConsoleColor.White, "Press [Enter] to exit",true);
            Console.ReadLine();
        }

        static void DynamicRouter()
        {
            //Scenario 1: A greedy dynamic router that routs jobs to different processors based on
            //            the pressure feedbacks it receives from the control channel. It will pick
            //            the processor that is least pressured. The pressure by default is the
            //            number of jobs in processing queue, but it can be overridden by custom
            //            logics.
            //TODO: Contruct the unit from configuration (using a factory?)

            List<SBMessage> tasks = new List<SBMessage>();
            double totalSize = 0;
            int nodeNumber = 2;
            int taskNumber = 20;

            for (int i = 1; i <= taskNumber; i++)
            {
                SBMessage msg = new SBMessage("Test task " + i);
                msg.Headers.Add(HeaderName.PressureValue, rand.Next(1000, 5000) / 1000.0);
                totalSize += (double)msg.Headers[HeaderName.PressureValue];
                tasks.Add(msg);
            }

            double[] processingTime = new double[nodeNumber];
            int index = 0;
            for (int i = 0; i < taskNumber; i++)
            {
                processingTime[index] += (double)tasks[i].Headers[HeaderName.PressureValue];
                index = (index + 1) % nodeNumber;
            }

            double maxProcessingTime = 0;
            for (int i = 0; i < nodeNumber; i++)
            {
                if (processingTime[i] >= maxProcessingTime)
                    maxProcessingTime = processingTime[i];
            }

            cprintln(ConsoleColor.Yellow, "Total workload: " + totalSize);
            cprintln(ConsoleColor.Yellow, "  Ideal processing time      : " + totalSize / nodeNumber);
            cprintln(ConsoleColor.Yellow, "  Round-robin processing time: " + maxProcessingTime);

            ProcessingUnitHost host = new ProcessingUnitHost();
            
            GreedyDynamicRouter router = new GreedyDynamicRouter();
            router.InputChannels.Add(new ServiceBus.Channel("v1input1"));
            for (int i = 1; i <= nodeNumber; i++)
                router.OutputChannels.Add(new ServiceBus.Channel("v1output" + i));
            router.ControlChannels.Add(new ServiceBus.Channel("v1control1"));

            host.AddProcessingUnit("pipeline", router);
            host.Open();

            var pipeline = host.GetProcessingUnit("pipeline");

            for (int i = 0; i < nodeNumber; i++)
                processingTime[i] = 0;
            int processedTasks = 0;

            pipeline.OutputChannels.MessageReceivedOnChannel += new EventHandler<ChannelMessageEventArgs>((obj, e) =>
            {
                IMessage task = (IMessage)e.Message;
                Trace.TraceInformation(string.Format("Received [{0}]", task.Body));

                //Message msg = new Message("");
                //msg.Headers.Add(HeaderName.PressureValue, task.Size / 1000.0);
                //msg.Headers.Add(HeaderName.PressureChannel, e.Channel.Name);
                //((GreedyDynamicRouter)pipeline).ControlChannels[0].Send(msg);

                Thread.Sleep((int)((double)task.Headers[HeaderName.PressureValue] * 1000));
                int id = pipeline.OutputChannels.IndexOf(e.Channel);
                processingTime[id] += (double)task.Headers[HeaderName.PressureValue];
                processedTasks++;
                Trace.TraceInformation(string.Format("Processed [{0}]", task.Body));

                Message msg = new Message("");
                msg.Headers.Add(HeaderName.PressureValue, -(double)task.Headers[HeaderName.PressureValue]);
                ((GreedyDynamicRouter)pipeline).ControlChannels[0].Send(msg);
            });

            foreach(var task in tasks)
            {
                Trace.TraceInformation(string.Format("Sending [{0}]", task.Body));
                pipeline.InputChannels[0].Send(task);
                Thread.Sleep(rand.Next(100, 1000));
            }

            while (processedTasks < taskNumber)
                Thread.Sleep(1000);

            maxProcessingTime = 0;
            for (int i = 0; i < nodeNumber; i++)
            {
                if (processingTime[i] >= maxProcessingTime)
                    maxProcessingTime = processingTime[i];
            }

            cprintln(ConsoleColor.Yellow, "  Greedy load-balancing processing time: " + maxProcessingTime);
        }
        static Random rand = new Random();

        #region Console output helpers
        static void println(string text, bool center = false)
        {
            if (center)
            {
                if (text.Length <= Console.WindowWidth)
                    Console.CursorLeft = (Console.WindowWidth - text.Length) / 2;
            }
            Console.WriteLine(text);
        }
        static void print(string text)
        {
            Console.Write(text);
        }
        static void cprintln(ConsoleColor color, string text, bool center = false)
        {
            var col = Console.ForegroundColor;
            Console.ForegroundColor = color;
            println(text, center);
            Console.ForegroundColor = col;
        }
        static void cprint(ConsoleColor color, string text)
        {
            var col = Console.ForegroundColor;
            Console.ForegroundColor = color;
            print(text);
            Console.ForegroundColor = col;
        }
        static void cbanner(ConsoleColor color, string text)
        {
            int width = text.Length;
            int height = 1;
            if (width > Console.WindowWidth-6)
            {
                width = Console.WindowWidth-6;
                height = text.Length / (Console.WindowWidth-6) + 1;
            }
            cprintln(color, "╔" + new string('═', width + 2) + "╗", true);
            int index = 0;
            for (int i =0; i < height;i++)
            {
                int offset = Math.Min(text.Length-index, Console.WindowWidth - 6);
                string content = "║ " 
                                 + (height >1 ? new string(' ', (Console.WindowWidth - 6 - offset) /2): "") 
                                 + text.Substring(index, offset)
                                 + (height > 1 ? new string(' ', (Console.WindowWidth - 6 - offset) / 2
                                 + (offset % 2 == 0 ? 0 : 1)) : "") + " ║";
                cprintln(color, content, true);
                index += offset;
            }
            cprintln(color, "╚" + new string('═', width + 2) + "╝", true);
        }
        #endregion
    }
}
