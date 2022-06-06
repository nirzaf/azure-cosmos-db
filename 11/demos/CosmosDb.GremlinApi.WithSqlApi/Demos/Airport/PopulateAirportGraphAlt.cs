using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Edges;
using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices;
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
	public static class PopulateAirportGraphAlt
	{
		private static bool CreateObjectsViaDocument = true;

		// If using SQL API to write vertices and edges
		public static Uri DemoGraphUri =>
			UriFactory.CreateDocumentCollectionUri("MyGraphDb", "DemoGraph");

		// If using Gremlin API to write vertices and edges
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
				var started = DateTime.Now;

				// --- Terminal 1 ---

				// V: Terminal 1
				var terminal1 = await CreateTerminal(client, "Terminal 1", "NY");

				// V: Gates in terminal 1
				var gate11 = await CreateGate(client, "Gate T1-1", "NY", "Continental");
				var gate12 = await CreateGate(client, "Gate T1-2", "NY", "Continental");
				var gate13 = await CreateGate(client, "Gate T1-3", "NY", "Continental");

				// V: Restaurants in terminal 2
				var wendys = await CreateRestaurant(client, "Wendys", "NY", 0.4m, 9.5m);
				var mcDonalds = await CreateRestaurant(client, "McDonalds", "NY", 0.3m, 8.15m);
				var chipotle = await CreateRestaurant(client, "Chipotle", "NY", 0.6m, 12.5m);

				// E: TerminalToGate (cyan)
				await CreateTerminalToGate(client, terminal1, gate11, 3);
				await CreateTerminalToGate(client, terminal1, gate12, 5);
				await CreateTerminalToGate(client, terminal1, gate13, 7);

				// E: TerminalToRestaurant (purple)
				await CreateTerminalToRestaurant(client, terminal1, wendys, 5);
				await CreateTerminalToRestaurant(client, terminal1, mcDonalds, 7);
				await CreateTerminalToRestaurant(client, terminal1, chipotle, 10);

				// E: GateToNextGate / GateToPrevGate (cyan dashed)
				await CreateGateToGate(client, gate11, gate12, 2);
				await CreateGateToGate(client, gate12, gate13, 2);

				// E: GateToRestaurant (purple dashed)
				await CreateGateToRestaurant(client, gate11, wendys, 2);
				await CreateGateToRestaurant(client, gate11, mcDonalds, 4);
				await CreateGateToRestaurant(client, gate11, chipotle, 6);
				await CreateGateToRestaurant(client, gate12, wendys, 2);
				await CreateGateToRestaurant(client, gate12, mcDonalds, 4);
				await CreateGateToRestaurant(client, gate12, chipotle, 6);
				await CreateGateToRestaurant(client, gate13, wendys, 6);
				await CreateGateToRestaurant(client, gate13, mcDonalds, 4);
				await CreateGateToRestaurant(client, gate13, chipotle, 2);

				// --- Terminal 2 ---

				// V: Terminal 2
				var terminal2 = await CreateTerminal(client, "Terminal 2", "NY");

				// V: Gates in terminal 2
				var gate21 = await CreateGate(client, "Gate T2-1", "NY", "Delta");
				var gate22 = await CreateGate(client, "Gate T2-2", "NY", "Delta");
				var gate23 = await CreateGate(client, "Gate T2-3", "NY", "Delta");

				// V: Restaurants in terminal 2
				var jackBox = await CreateRestaurant(client, "Jack in the Box", "NY", 0.3m, 3.15m);
				var kfc = await CreateRestaurant(client, "Kentucky Fried Chicken", "NY", 0.4m, 7.5m);
				var burgerKing = await CreateRestaurant(client, "Burger King", "NY", 0.2m, 7.15m);

				// E: TerminalToGate
				await CreateTerminalToGate(client, terminal2, gate21, 3);
				await CreateTerminalToGate(client, terminal2, gate22, 5);
				await CreateTerminalToGate(client, terminal2, gate23, 7);

				// E: TerminalToRestaurant
				await CreateTerminalToRestaurant(client, terminal2, jackBox, 5);
				await CreateTerminalToRestaurant(client, terminal2, kfc, 7);
				await CreateTerminalToRestaurant(client, terminal2, burgerKing, 10);

				// E: GateToNextGate / GateToPrevGate
				await CreateGateToGate(client, gate21, gate22, 2);
				await CreateGateToGate(client, gate22, gate23, 2);

				// E: GateToRestaurant
				await CreateGateToRestaurant(client, gate21, jackBox, 2);
				await CreateGateToRestaurant(client, gate21, kfc, 4);
				await CreateGateToRestaurant(client, gate21, burgerKing, 6);
				await CreateGateToRestaurant(client, gate22, jackBox, 2);
				await CreateGateToRestaurant(client, gate22, kfc, 4);
				await CreateGateToRestaurant(client, gate22, burgerKing, 6);
				await CreateGateToRestaurant(client, gate23, jackBox, 6);
				await CreateGateToRestaurant(client, gate23, kfc, 4);
				await CreateGateToRestaurant(client, gate23, burgerKing, 2);

				// --- Terminal to Terminal ---

				// E: TerminalToNextTerminal / TerminalToPrevTerminal
				await CreateTerminalToTerminal(client, terminal1, terminal2, 10);

				var elapsed = DateTime.Now.Subtract(started);
				Console.WriteLine($"Elapsed: {elapsed}");
			}
		}

		private static async Task ClearGraph(DocumentClient client)
		{
			await client.CreateGremlinQuery(_graph, "g.V().drop()").ExecuteNextAsync();
			Console.WriteLine("Graph has been cleared");
		}

		private static async Task<Terminal> CreateTerminal(DocumentClient client, string id, string city)
		{
			var terminal = new Terminal
			{
				Id = id,
				City = city,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = terminal.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = terminal.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}

			Console.WriteLine($"Created Terminal vertex '{id}'");
			return terminal;
		}

		private static async Task<Gate> CreateGate(DocumentClient client, string id, string city, string airline)
		{
			var gate = new Gate
			{
				Id = id,
				City = city,
				Airline = airline,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = gate.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = gate.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}

			Console.WriteLine($"Created Gate vertex '{id}'");
			return gate;
		}

		private static async Task<Restaurant> CreateRestaurant(DocumentClient client, string id, string city, decimal rating, decimal averagePrice)
		{
			var restaurant = new Restaurant
			{
				Id = id,
				City = city,
				Rating = rating,
				AveragePrice = averagePrice,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = restaurant.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = restaurant.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}

			Console.WriteLine($"Created Restaurant vertex '{id}'");
			return restaurant;
		}

		private static async Task<Tuple<TerminalToGate, GateToTerminal>> CreateTerminalToGate(DocumentClient client, Terminal terminal, Gate gate, int distanceInMinutes)
		{
			var edge1 = new TerminalToGate(terminal, gate)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge1.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge1.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created TerminalToGate edge '{terminal.Id}' > '{gate.Id}'");

			var edge2 = new GateToTerminal(gate, terminal)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge2.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge2.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created GateToTerminal edge '{gate.Id}' > '{terminal.Id}'");

			return new Tuple<TerminalToGate, GateToTerminal>(edge1, edge2);
		}

		private static async Task<Tuple<TerminalToRestaurant, RestaurantToTerminal>> CreateTerminalToRestaurant(DocumentClient client, Terminal terminal, Restaurant restaurant, int distanceInMinutes)
		{
			var edge1 = new TerminalToRestaurant(terminal, restaurant)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge1.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge1.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created TerminalToRestaurant edge '{terminal.Id}' > '{restaurant.Id}'");

			var edge2 = new RestaurantToTerminal(restaurant, terminal)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge2.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge2.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created RestaurantToTerminal edge '{restaurant.Id}' > '{terminal.Id}'");

			return new Tuple<TerminalToRestaurant, RestaurantToTerminal>(edge1, edge2);
		}

		private static async Task<Tuple<GateToNextGate, GateToPrevGate>> CreateGateToGate(DocumentClient client, Gate gate1, Gate gate2, int distanceInMinutes)
		{
			var edge1 = new GateToNextGate(gate1, gate2)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge1.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge1.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created GateToNextGate edge '{gate1.Id}' > '{gate2.Id}'");

			var edge2 = new GateToPrevGate(gate2, gate1)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge2.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge2.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created GateToPrevGate edge '{gate2.Id}' > '{gate1.Id}'");

			return new Tuple<GateToNextGate, GateToPrevGate>(edge1, edge2);
		}

		private static async Task<Tuple<GateToRestaurant, RestaurantToGate>> CreateGateToRestaurant(DocumentClient client, Gate gate, Restaurant restaurant, int distanceInMinutes)
		{
			var edge1 = new GateToRestaurant(gate, restaurant)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge1.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge1.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created GateToRestaurant edge '{gate.Id}' > '{restaurant.Id}'");

			var edge2 = new RestaurantToGate(restaurant, gate)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge2.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge2.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created RestaurantToGate edge '{restaurant.Id}' > '{gate.Id}'");

			return new Tuple<GateToRestaurant, RestaurantToGate>(edge1, edge2);
		}

		private static async Task<Tuple<TerminalToNextTerminal, TerminalToPrevTerminal>> CreateTerminalToTerminal(DocumentClient client, Terminal terminal1, Terminal terminal2, int distanceInMinutes)
		{
			var edge1 = new TerminalToNextTerminal(terminal1, terminal2)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge1.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge1.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created TerminalToNextTerminal edge '{terminal1.Id}' > '{terminal2.Id}'");

			var edge2 = new TerminalToPrevTerminal(terminal2, terminal1)
			{
				DistanceInMinutes = distanceInMinutes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = edge2.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = edge2.ToGremlin();
				await client.CreateGremlinQuery(_graph, gremlin).ExecuteNextAsync();
			}
			Console.WriteLine($"Created TerminalToPrevTerminal edge '{terminal2.Id}' > '{terminal1.Id}'");

			return new Tuple<TerminalToNextTerminal, TerminalToPrevTerminal>(edge1, edge2);
		}

	}
}
