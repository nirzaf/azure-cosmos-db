using CosmosDb.GremlinApi.WithSqlApi.Demos;
using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport;
using CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDb.GremlinApi.WithSqlApi
{
	public static class Program
	{
		private static IDictionary<string, Func<Task>> DemoMethods;

		private static void Main(string[] args)
		{
			DemoMethods = new Dictionary<string, Func<Task>>
			{
				{ "GC", CreateGraphContainer.Run },
				{ "SN", SocialNetworkDemo.Run },
				{ "PA", PopulateAirportGraphAlt.Run },
				{ "QA", QueryAirportGraph.Run },
				{ "DC", DownloadComicBookDemo.Run },
				{ "UC", UploadComicBookDemo.Run },
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
			Console.WriteLine(@"Cosmos DB Gremlin API .NET SDK demos

GC Create graph container
SN Social network demo
PA Populate airport graph
QA Query airport graph
DC Download comic book demo
UC Upload comic book demo

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
