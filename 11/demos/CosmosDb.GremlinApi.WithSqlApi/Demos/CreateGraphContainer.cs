using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos
{
	public static class CreateGraphContainer
	{
		public static Uri MyGraphDbDatabaseUri =>
			UriFactory.CreateDatabaseUri("MyGraphDb");

		public async static Task Run()
		{
			Debugger.Break();

			var endpoint = ConfigurationManager.AppSettings["CosmosDbEndpoint"];
			var masterKey = ConfigurationManager.AppSettings["CosmosDbMasterKey"];

			using (var client = new DocumentClient(new Uri(endpoint), masterKey))
			{
				ViewGraphs(client);

				await CreateGraph(client);
				ViewGraphs(client);
			}
		}

		private static void ViewGraphs(DocumentClient client)
		{
			Console.WriteLine();
			Console.WriteLine(">>> View Graphs in MyGraphDb <<<");

			var graphs = client
				.CreateDocumentCollectionQuery(MyGraphDbDatabaseUri)
				.ToList();

			var i = 0;
			foreach (var graph in graphs)
			{
				i++;
				Console.WriteLine();
				Console.WriteLine($" Graph #{i}");
				ViewGraph(graph);
			}

			Console.WriteLine();
			Console.WriteLine($"Total collections in MyGraphDb database: {graphs.Count}");
		}

		private static void ViewGraph(DocumentCollection graph)
		{
			Console.WriteLine($"    Graph ID: {graph.Id}");
			Console.WriteLine($" Resource ID: {graph.ResourceId}");
			Console.WriteLine($"   Self Link: {graph.SelfLink}");
			Console.WriteLine($"       E-Tag: {graph.ETag}");
			Console.WriteLine($"   Timestamp: {graph.Timestamp}");
		}

		private async static Task CreateGraph(DocumentClient client)
		{
			Console.WriteLine();
			Console.WriteLine($">>> Create Graph in MyGraphDb <<<");
			Console.WriteLine();

			var partitionKeyDefinition = new PartitionKeyDefinition();
			partitionKeyDefinition.Paths.Add("/city");

			var graphDefinition = new DocumentCollection
			{
				Id = "DemoGraph",
				PartitionKey = partitionKeyDefinition
			};
			var options = new RequestOptions { OfferThroughput = 1000 };

			var result = await client.CreateDocumentCollectionIfNotExistsAsync(MyGraphDbDatabaseUri, graphDefinition, options);
			var graph = result.Resource;

			Console.WriteLine("Created new graph DemoGraph");
			ViewGraph(graph);
		}

	}
}
