using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport
{
	public static class QueryAirportGraph
	{
		public static Uri MyGraphDbDatabaseUri =>
			UriFactory.CreateDatabaseUri("MyGraphDb");

		private static DocumentCollection _graph;

		public async static Task Run()
		{
			Debugger.Break();

			var endpoint = ConfigurationManager.AppSettings["CosmosDbEndpoint"];
			var masterKey = ConfigurationManager.AppSettings["CosmosDbMasterKey"];

			using (var client = new DocumentClient(new Uri(endpoint), masterKey))
			{
				_graph = client
					.CreateDocumentCollectionQuery(MyGraphDbDatabaseUri)
					.ToList()
					.Single(g => g.Id == "DemoGraph");

				Console.WriteLine();
				Console.WriteLine("*** Scenario 1 - First eat (> .3 rating), then switch terminals, then go to gate ***");

				var firstEatThenSwitchTerminals = @"
					// Start at T1, Gate 2
						g.V('Gate T1-2')

					// Traverse edge from gate to restaurants
						.outE('gateToRestaurant')
						.inV()

					// Filter for restaurants with a rating of .3 or higher
						.has('rating', gt(0.3))

					// Traverse edge from restaurant back to terminal (T1)
						.outE('restaurantToTerminal')
						.inV()
					
					// Traverse edge from terminal to next terminal (T2)
						.outE('terminalToNextTerminal')
						.inV()
					
					// Traverse edge from terminal (T2) to gates
						.outE('terminalToGate')
						.inV()
					
					// Filter for destination gate T2, Gate 3
						.has('id', 'Gate T2-3')
					
					// Show the possible paths
						.path()
				";

				await RunAirportQuery(client, firstEatThenSwitchTerminals);

				Console.WriteLine();
				Console.WriteLine("*** Scenario 2 - First switch terminals, then eat (> .2 rating), then go to gate ***");

				var firstSwitchTerminalsThenEat = @"
					// Start at T1, Gate 2
						g.V('Gate T1-2')
					// Traverse edge from gate to terminal T1
						.outE('gateToTerminal').inV()
					// Traverse edge from terminal to next terminal (T2)
						.outE('terminalToNextTerminal').inV()
					// Traverse edge from terminal to restaurants
						.outE('terminalToRestaurant').inV()
					// Filter for restaurants with a rating of .2 or higher
						.has('rating', gt(0.2))
					// Traverse edge from restaurant back to gates
						.outE('restaurantToGate').inV()
					// Filter for destination gate T2, Gate 3
						.has('id', 'Gate T2-3')
					// Show the possible paths
						.path()
				";

				await RunAirportQuery(client, firstSwitchTerminalsThenEat);
			}
		}

		private static async Task RunAirportQuery(DocumentClient client, string gremlin)
		{
			var query = client.CreateGremlinQuery(_graph, gremlin);
			var count = 0;

			while (query.HasMoreResults)
			{
				foreach (dynamic result in await query.ExecuteNextAsync())
				{
					count++;
					Console.WriteLine();
					Console.WriteLine($"Choice # {count}");
					var userStep = 0;
					var totalDistanceInMinutes = 0;
					var i = 0;
					foreach (var item in result.objects)
					{
						i++;
						if (item.type.ToString() == "edge")
						{
							var distanceInMinutes = item.properties.distanceInMinutes.Value;
							totalDistanceInMinutes += distanceInMinutes;
							var edgeInfo = $"    ({item.label} = {distanceInMinutes} min)";
							Console.WriteLine(edgeInfo);
						}
						else
						{
							userStep++;
							var userStepCaption = (userStep == 1 ? "Start at" : (i == result.objects.Count ? "Arrive at" : "Go to"));
							var vertexInfo = $"{userStep}. {userStepCaption} {item.label} = {item.id}";
							switch (item.label.ToString())
							{
								case "gate":
									vertexInfo += $", airline = {item.properties.airline[0].value}";
									break;

								case "restaurant":
									vertexInfo += $", rating = {item.properties.rating[0].value}";
									vertexInfo += $", avg price = {item.properties.averagePrice[0].value}";
									break;
							}

							vertexInfo += $" ({totalDistanceInMinutes} min)";
							Console.WriteLine(vertexInfo);
						}

					}
				}
			}
		}

	}
}
