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
	public static class PopulateAirportGraph
	{
		private static DocumentCollection _graph;

		public async static Task Run()
		{
			Debugger.Break();

			var endpoint = ConfigurationManager.AppSettings["CosmosDbEndpoint"];
			var masterKey = ConfigurationManager.AppSettings["CosmosDbMasterKey"];

			using (var client = new DocumentClient(new Uri(endpoint), masterKey))
			{
				_graph = client
					.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri("MyGraphDb"))
					.ToList()
					.FirstOrDefault(g => g.Id == "DemoGraph");

				await ClearGraph(client);

				// --- Terminal 1 ---

				// V: Terminal 1
				await CreateTerminal(client, "Terminal 1", "NY");

				// V: Gates in terminal 1
				await CreateGate(client, "Gate T1-1", "NY", "Continental");
				await CreateGate(client, "Gate T1-2", "NY", "Continental");
				await CreateGate(client, "Gate T1-3", "NY", "Continental");

				// V: Restaurants in terminal 2
				await CreateRestaurant(client, "Wendys", "NY", 0.4m, 9.5m);
				await CreateRestaurant(client, "McDonalds", "NY", 0.3m, 8.15m);
				await CreateRestaurant(client, "Chipotle", "NY", 0.6m, 12.5m);

				// E: TerminalToGate (cyan)
				await CreateTerminalToGate(client, "Terminal 1", "Gate T1-1", 3);
				await CreateTerminalToGate(client, "Terminal 1", "Gate T1-2", 5);
				await CreateTerminalToGate(client, "Terminal 1", "Gate T1-3", 7);

				// E: TerminalToRestaurant (purple)
				await CreateTerminalToRestaurant(client, "Terminal 1", "Wendys", 5);
				await CreateTerminalToRestaurant(client, "Terminal 1", "McDonalds", 7);
				await CreateTerminalToRestaurant(client, "Terminal 1", "Chipotle", 10);

				// E: GateToNextGate / GateToPrevGate (cyan dashed)
				await CreateGateToGate(client, "Gate T1-1", "Gate T1-2", 2);
				await CreateGateToGate(client, "Gate T1-2", "Gate T1-3", 2);

				// E: GateToRestaurant (purple dashed)
				await CreateGateToRestaurant(client, "Gate T1-1", "Wendys", 2);
				await CreateGateToRestaurant(client, "Gate T1-1", "McDonalds", 4);
				await CreateGateToRestaurant(client, "Gate T1-1", "Chipotle", 6);
				await CreateGateToRestaurant(client, "Gate T1-2", "Wendys", 2);
				await CreateGateToRestaurant(client, "Gate T1-2", "McDonalds", 4);
				await CreateGateToRestaurant(client, "Gate T1-2", "Chipotle", 6);
				await CreateGateToRestaurant(client, "Gate T1-3", "Wendys", 6);
				await CreateGateToRestaurant(client, "Gate T1-3", "McDonalds", 4);
				await CreateGateToRestaurant(client, "Gate T1-3", "Chipotle", 2);

				// --- Terminal 2 ---

				// V: Terminal 2
				await CreateTerminal(client, "Terminal 2", "NY");

				// V: Gates in terminal 2
				await CreateGate(client, "Gate T2-1", "NY", "Delta");
				await CreateGate(client, "Gate T2-2", "NY", "Delta");
				await CreateGate(client, "Gate T2-3", "NY", "Delta");

				// V: Restaurants in terminal 2
				await CreateRestaurant(client, "Jack in the Box", "NY", 0.3m, 3.15m);
				await CreateRestaurant(client, "Kentucky Fried Chicken", "NY", 0.4m, 7.5m);
				await CreateRestaurant(client, "Burger King", "NY", 0.2m, 7.15m);

				// E: TerminalToGate
				await CreateTerminalToGate(client, "Terminal 2", "Gate T2-1", 3);
				await CreateTerminalToGate(client, "Terminal 2", "Gate T2-2", 5);
				await CreateTerminalToGate(client, "Terminal 2", "Gate T2-3", 7);

				// E: TerminalToRestaurant
				await CreateTerminalToRestaurant(client, "Terminal 2", "Jack in the Box", 5);
				await CreateTerminalToRestaurant(client, "Terminal 2", "Kentucky Fried Chicken", 7);
				await CreateTerminalToRestaurant(client, "Terminal 2", "Burger King", 10);

				// E: GateToNextGate / GateToPrevGate
				await CreateGateToGate(client, "Gate T2-1", "Gate T2-2", 2);
				await CreateGateToGate(client, "Gate T2-2", "Gate T2-3", 2);

				// E: GateToRestaurant
				await CreateGateToRestaurant(client, "Gate T2-1", "Jack in the Box", 2);
				await CreateGateToRestaurant(client, "Gate T2-1", "Kentucky Fried Chicken", 4);
				await CreateGateToRestaurant(client, "Gate T2-1", "Burger King", 6);
				await CreateGateToRestaurant(client, "Gate T2-2", "Jack in the Box", 2);
				await CreateGateToRestaurant(client, "Gate T2-2", "Kentucky Fried Chicken", 4);
				await CreateGateToRestaurant(client, "Gate T2-2", "Burger King", 6);
				await CreateGateToRestaurant(client, "Gate T2-3", "Jack in the Box", 6);
				await CreateGateToRestaurant(client, "Gate T2-3", "Kentucky Fried Chicken", 4);
				await CreateGateToRestaurant(client, "Gate T2-3", "Burger King", 2);

				// --- Terminal to Terminal ---

				// E: TerminalToNextTerminal / TerminalToPrevTerminal
				await CreateTerminalToTerminal(client, "Terminal 1", "Terminal 2", 10);
			}
		}

		private static async Task ClearGraph(DocumentClient client)
		{
			var gremlin = $@"
				g.V()
				.drop()
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine("Graph has been cleared");
		}

		private static async Task CreateTerminal(DocumentClient client, string id, string city)
		{
			var gremlin = $@"
				g.addV('terminal')
				.property('id', '{id}')
				.property('city', '{city}')
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created Terminal vertex '{id}'");
		}

		private static async Task CreateGate(DocumentClient client, string id, string city, string airline)
		{
			var gremlin = $@"
				g.addV('gate')
				.property('id', '{id}')
				.property('city', '{city}')
				.property('airline', '{airline}')
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created Gate vertex '{id}'");
		}

		private static async Task CreateRestaurant(DocumentClient client, string id, string city, decimal rating, decimal averagePrice)
		{
			var gremlin = $@"
				g.addV('restaurant')
				.property('id', '{id}')
				.property('city', '{city}')
				.property('rating', {rating})
				.property('averagePrice', {averagePrice})
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created Restaurant vertex '{id}'");
		}

		private static async Task CreateTerminalToGate(DocumentClient client, string terminal, string gate, int distanceInMinutes)
		{
			var gremlin = $@"
				g.V()
				.has('id', '{terminal}')
				.addE('terminalToGate')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{gate}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created TerminalToGate edge '{terminal}' > '{gate}'");

			gremlin = $@"
				g.V()
				.has('id', '{gate}')
				.addE('gateToTerminal')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{terminal}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created GateToTerminal edge '{gate}' > '{terminal}'");
		}

		private static async Task CreateTerminalToRestaurant(DocumentClient client, string terminal, string restaurant, int distanceInMinutes)
		{
			var gremlin = $@"
				g.V()
				.has('id', '{terminal}')
				.addE('terminalToRestaurant')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{restaurant}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created TerminalToRestaurant edge '{terminal}' > '{restaurant}'");

			gremlin = $@"
				g.V()
				.has('id', '{restaurant}')
				.addE('restaurantToTerminal')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{terminal}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created RestaurantToTerminal edge '{restaurant}' > '{terminal}'");
		}

		private static async Task CreateGateToGate(DocumentClient client, string gate1, string gate2, int distanceInMinutes)
		{
			var gremlin = $@"
				g.V()
				.has('id', '{gate1}')
				.addE('gateToNextGate')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{gate2}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created GateToNextGate edge '{gate1}' > '{gate2}'");

			gremlin = $@"
				g.V()
				.has('id', '{gate2}')
				.addE('gateToPrevGate')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{gate1}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created GateToPrevGate edge '{gate2}' > '{gate1}'");
		}

		private static async Task CreateGateToRestaurant(DocumentClient client, string gate, string restaurant, int distanceInMinutes)
		{
			var gremlin = $@"
				g.V()
				.has('id', '{gate}')
				.addE('gateToRestaurant')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{restaurant}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created GateToRestaurant edge '{gate}' > '{restaurant}'");

			gremlin = $@"
				g.V()
				.has('id', '{restaurant}')
				.addE('restaurantToGate')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{gate}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created RestaurantToGate edge '{restaurant}' > '{gate}'");
		}

		private static async Task CreateTerminalToTerminal(DocumentClient client, string terminal1, string terminal2, int distanceInMinutes)
		{
			var gremlin = $@"
				g.V()
				.has('id', '{terminal1}')
				.addE('terminalToNextTerminal')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{terminal2}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created TerminalToNextTerminal edge '{terminal1}' > '{terminal2}'");

			gremlin = $@"
				g.V()
				.has('id', '{terminal2}')
				.addE('terminalToPrevTerminal')
				.property('distanceInMinutes', {distanceInMinutes})
				.to(
					g.V()
					.has('id', '{terminal1}'))
			";

			await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			Console.WriteLine($"Created TerminalToPrevTerminal edge '{terminal2}' > '{terminal1}'");
		}

	}
}
