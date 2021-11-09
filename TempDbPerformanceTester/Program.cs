using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TempDbPerformanceTester
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!AreArgsValid(args))
            {
                return;
            }

            var threads = int.Parse(args[0]);
            var delay = int.Parse(args[1]);
            var itemCount = int.Parse(args[2]);
            string connectionString = connectionString = args[3].TrimStart('"').TrimEnd('"'); ;

            Random random = new Random();

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threads; i++)
            {
                var id = i + 1;
                tasks.Add(Task.Run(() =>
                {
                    var worker = new TempDbQueryWorker(id, random, delay, itemCount, connectionString);
                    worker.Run();
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static bool AreArgsValid(string[] args)
        {
            if (args.Length < 4 || !int.TryParse(args[0], out _) || !int.TryParse(args[1], out _) || !int.TryParse(args[2], out _))
            {
                WriteUsageInfoToConsole();
                return false;
            }

            return true;
        }

        private static void WriteUsageInfoToConsole()
        {
            Console.WriteLine("Invalid Arguments");
            Console.WriteLine("Usage TempDbPerformanceTester.exe <threads> <delay between queries in ms> <item-count> <connection string - (optional - defaults to local)>");
            Console.WriteLine("e.g. TempDbPerformanceTester.exe 5 100 100");
            Console.WriteLine("e.g. TempDbPerformanceTester.exe 5 100 100 Server=myserver;Database=SAFE;Integrated Security=True");
            Console.WriteLine("Threads represents the number of active threads that will be run by the application making queries");
            Console.WriteLine("The delay is how long each thread should wait between calls to the database");
            Console.WriteLine("Item count is the number of records that will be added to the TempDb table");
        }
    }
}