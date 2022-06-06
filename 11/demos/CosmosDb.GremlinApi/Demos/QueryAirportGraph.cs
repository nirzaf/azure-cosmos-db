using Gremlin.Net.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CosmosDb.GremlinApi.Demos
{
	public static class QueryAirportGraph
	{
		private const int Port = 443;
		private const string DatabaseName = "GraphDb";
		private const string GraphName = "Airport";

		public async static Task Run()
		{
			Debugger.Break();

			var hostname = ConfigurationManager.AppSettings["CosmosDbHostName"];
			var masterKey = ConfigurationManager.AppSettings["CosmosDbMasterKey"];
			var username = $"/dbs/{DatabaseName}/colls/{GraphName}";

			var gremlinServer = new GremlinServer(hostname, Port, true, username, masterKey);

			using (var client = new GremlinClient(gremlinServer))
			{
				Console.WriteLine();
				Console.WriteLine("*** Scenario 1 - First eat (> .3 rating), then switch terminals, then go to gate ***");

				var firstEatThenSwitchTerminals = @"
					// Start at T1, Gate 2
						g.V('Gate T1-2')

					// Traverse edge from gate to restaurants
						.outE('gateToRestaurant')
						.inV()

					// Filter for restaurants with a rating higher than .3
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
						.outE('gateToTerminal')
						.inV()

					// Traverse edge from terminal to next terminal (T2)
						.outE('terminalToNextTerminal')
						.inV()

					// Traverse edge from terminal to restaurants
						.outE('terminalToRestaurant')
						.inV()
					
					// Filter for restaurants with a rating higher than .2
						.has('rating', gt(0.2))
					
					// Traverse edge from restaurant back to gates
						.outE('restaurantToGate')
						.inV()
					
					// Filter for destination gate T2, Gate 3
						.has('id', 'Gate T2-3')
					
					// Show the possible paths
						.path()
				";

				await RunAirportQuery(client, firstSwitchTerminalsThenEat);
			}
		}

		private static async Task RunAirportQuery(GremlinClient client, string gremlinCode)
		{
			var results = await client.SubmitAsync<dynamic>(gremlinCode);

			var count = 0;

			foreach (var result in results)
			{
				count++;
				var jResult = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(result));
				var steps = (JArray)jResult["objects"];

				var userStep = 0;
				var totalDistanceInMinutes = 0;
				var i = 0;

				Console.WriteLine();
				Console.WriteLine($"Choice # {count}");

				foreach (var step in steps)
				{
					i++;
					if (step["type"].Value<string>() == "vertex")
					{
						userStep++;
						var userStepCaption = (userStep == 1 ? "Start at" : (i == steps.Count ? "Arrive at" : "Go to"));
						var vertexInfo = $"{userStep}. {userStepCaption} {step["label"]} = {step["id"]}";

						if (step["label"].Value<string>() == "restaurant")
						{
							vertexInfo += $", rating = {step["properties"]["rating"][0]["value"]}";
							vertexInfo += $", avg price = {step["properties"]["averagePrice"][0]["value"]}";
						}

						vertexInfo += $" ({totalDistanceInMinutes} min)";
						Console.WriteLine(vertexInfo);
					}
					else
					{
						var distanceInMinutes = step["properties"]["distanceInMinutes"].Value<int>();
						totalDistanceInMinutes += distanceInMinutes;
						var edgeInfo = $"    ({step["label"]} = {distanceInMinutes} min)";
						Console.WriteLine(edgeInfo);
					}
				}

			}
		}

	}
}
