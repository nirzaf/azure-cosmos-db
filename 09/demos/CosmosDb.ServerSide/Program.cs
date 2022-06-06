using CosmosDb.ServerSide.Demos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDb.ServerSide
{
    public static class Program
    {
        private static IDictionary<string, Func<Task>> DemoMethods;

        private static void Main(string[] args)
        {
            DemoMethods = new Dictionary<string, Func<Task>>
            {
                { "SP", StoredProceduresDemo.Run },
                { "TR", TriggersDemo.Run },
                { "UF", UserDefinedFunctionsDemo.Run },
                { "C", Cleanup.Run }
            };

            Task.Run(async () =>
            {
                ShowMenu();
                while (true)
                {
                    Console.Write("Selection: ");
                    var input = Console.ReadLine();
                    var demoId = input.ToUpper().Trim();
                    if (DemoMethods.Keys.Contains(demoId))
                    {
                        var demoMethod = DemoMethods[demoId];
                        await RunDemo(demoMethod);
                    }
                    else if (demoId == "Q")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"?{input}");
                    }
                }
            }).Wait();
        }

        private static void ShowMenu()
        {
            Console.WriteLine(@"Cosmos DB SQL API Server-Side Programming demos

SP Stored procedures
TR Triggers
UF User defined functions

C  Cleanup

Q  Quit
");
        }

        private async static Task RunDemo(Func<Task> demoMethod)
        {
            try
            {
                await demoMethod();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    message += Environment.NewLine + ex.Message;
                }
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine();
            Console.Write("Done. Press any key to continue...");
            Console.ReadKey(true);
            Console.Clear();
            ShowMenu();
        }

    }
}
