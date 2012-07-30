using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using IntegrationPatterns.Infrastructure;
using IntegrationPatterns.Infrastructure.Configuraiton;
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

            cprintln(ConsoleColor.White, "Press [Enter] to exit", true);
            Console.ReadLine();
        }

        static void DynamicRouter()
        {
            //Scenario 1: A greedy dynamic router that routs jobs to different processors based on
            //            the pressure feedbacks it receives from the control channel. It will pick
            //            the processor that is least pressured. The pressure by default is the
            //            number of jobs in processing queue, but it can be overridden by custom
            //            logics.

            int nodeNumber = 2;
            int taskNumber = 100;
            double[] processingTime = new double[nodeNumber];

            List<SBMessage> tasks = generateTasks(taskNumber);
            double totalSize = getTotalProcessingTime(tasks);

            var host = ProcessingUnitHost.FromConfiguration((MessagePipelineSection)ConfigurationManager.GetSection("Scenario1"));
            host.Open();

            var pipeline = host.GetProcessingUnit("loadbalancer");

            int processedTasks = 0;

            pipeline.OutputChannels.MessageReceivedOnChannel += new EventHandler<ChannelMessageEventArgs>((obj, e) =>
            {
                IMessage task = (IMessage)e.Message;
                Trace.TraceInformation(string.Format("Received [{0}]", task.Body));

                Thread.Sleep((int)((double)task.Headers[HeaderName.PressureValue] * 1000)); //simulate processing time
                int id = pipeline.OutputChannels.IndexOf(e.Channel);
                processingTime[id] += (double)task.Headers[HeaderName.PressureValue];
                processedTasks++;
                Trace.TraceInformation(string.Format("Processed [{0}]", task.Body));

                Message msg = new Message("");  //provide feedback to report reduced pressure on the channel
                msg.Headers.Add(HeaderName.PressureValue, -(double)task.Headers[HeaderName.PressureValue]);
                ((GreedyDynamicRouter)pipeline).ControlChannels[0].Send(msg);
            });

            foreach (var task in tasks)
            {
                Trace.TraceInformation(string.Format("Sending [{0}]", task.Body));
                pipeline.InputChannels[0].Send(task);
                Thread.Sleep(rand.Next(100, 1000));
            }

            while (processedTasks < taskNumber)
                Thread.Sleep(1000);

            cprintln(ConsoleColor.Yellow, "Total workload: " + totalSize);
            cprintln(ConsoleColor.Yellow, "  Ideal processing time      : " + totalSize / nodeNumber);
            cprintln(ConsoleColor.Yellow, "  Round-robin processing time: " + getRoundRobinProcessingTime(tasks, nodeNumber));
            cprintln(ConsoleColor.Yellow, "  Greedy load-balancing processing time: " + maxItem(processingTime));
        }

        static Random rand = new Random();

        #region Test helpers
        private static List<SBMessage> generateTasks(int taskNumber)
        {
            List<SBMessage> tasks = new List<SBMessage>();

            for (int i = 1; i <= taskNumber; i++)
            {
                SBMessage msg = new SBMessage("Test task " + i);
                msg.Headers.Add(HeaderName.PressureValue, rand.Next(1000, 5000) / 1000.0);
                tasks.Add(msg);
            }

            return tasks;
        }
        public static double getTotalProcessingTime(List<SBMessage> tasks)
        {
            double total = 0;

            foreach (var task in tasks)
                total += (double)task.Headers[HeaderName.PressureValue];

            return total;
        }
        public static double getRoundRobinProcessingTime(List<SBMessage> tasks, int nodeNumber)
        {
            int index = 0;
            double[] processingTime = new double[nodeNumber];
            foreach (var task in tasks)
            {
                processingTime[index] += (double)task.Headers[HeaderName.PressureValue];
                index = (index + 1) % nodeNumber;
            }

            return maxItem(processingTime);
        }
        public static double maxItem(double[] array)
        {
            double max = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] >= max)
                    max = array[i];
            }
            return max;
        }
        #endregion

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
            if (width > Console.WindowWidth - 6)
            {
                width = Console.WindowWidth - 6;
                height = text.Length / (Console.WindowWidth - 6) + 1;
            }
            cprintln(color, "╔" + new string('═', width + 2) + "╗", true);
            int index = 0;
            for (int i = 0; i < height; i++)
            {
                int offset = Math.Min(text.Length - index, Console.WindowWidth - 6);
                string content = "║ "
                                 + (height > 1 ? new string(' ', (Console.WindowWidth - 6 - offset) / 2) : "")
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
