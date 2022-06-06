using CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork.Edges;
using CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork.Vertices;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork
{
	public static class SocialNetworkDemo
	{
		private static bool CreateObjectsViaDocument = true;

		public static Uri MyGraphDbDatabaseUri =>
			UriFactory.CreateDatabaseUri("MyGraphDb");

		public static Uri DemoGraphUri =>
			UriFactory.CreateDocumentCollectionUri("MyGraphDb", "DemoGraph");

		public async static Task Run()
		{
			Debugger.Break();

			var endpoint = ConfigurationManager.AppSettings["CosmosDbEndpoint"];
			var masterKey = ConfigurationManager.AppSettings["CosmosDbMasterKey"];

			using (var client = new DocumentClient(new Uri(endpoint), masterKey))
			{
				var graph = client
					.CreateDocumentCollectionQuery(MyGraphDbDatabaseUri)
					.ToList()
					.Single(g => g.Id == "DemoGraph");

				await ClearGraph(client, graph);

				var john = await CreatePerson(client, graph, "John", "NY", 25, "pizza");
				var alan = await CreatePerson(client, graph, "Alan", "NY", 25, "seafood");
				var acme = await CreateCompany(client, graph, "Acme", "NY", 2001);

				await CreateWorksAtEdge(client, graph, john, acme, true);
				await CreateWorksAtEdge(client, graph, alan, acme, false);
				await CreateManagesEdge(client, graph, alan, john);
			}
		}

		private static async Task ClearGraph(DocumentClient client, DocumentCollection graph)
		{
			await client.CreateGremlinQuery(graph, "g.V().drop()").ExecuteNextAsync();
		}

		private static async Task<Person> CreatePerson(DocumentClient client, DocumentCollection graph, string id, string city, int age, string likes)
		{
			var person = new Person
			{
				Id = id,
				City = city,
				Age = age,
				Likes = likes,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = person.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = person.ToGremlin();
				await client.CreateGremlinQuery(graph, gremlin).ExecuteNextAsync();
			}

			return person;
		}

		private static async Task<Company> CreateCompany(DocumentClient client, DocumentCollection graph, string id, string city, int founded)
		{
			var company = new Company
			{
				Id = id,
				City = city,
				Founded = founded,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = company.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = company.ToGremlin();
				await client.CreateGremlinQuery(graph, gremlin).ExecuteNextAsync();
			}

			return company;
		}

		private static async Task<WorksAt> CreateWorksAtEdge(DocumentClient client, DocumentCollection graph, Person person, Company company, bool weekends)
		{
			var worksAt = new WorksAt(person, company)
			{
				Weekends = weekends,
			};

			if (CreateObjectsViaDocument)
			{
				var doc = worksAt.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = worksAt.ToGremlin();
				await client.CreateGremlinQuery(graph, gremlin).ExecuteNextAsync();
			}

			return worksAt;
		}

		private static async Task<Manages> CreateManagesEdge(DocumentClient client, DocumentCollection graph, Person manager, Person person)
		{
			var manages = new Manages(manager, person);

			if (CreateObjectsViaDocument)
			{
				var doc = manages.ToDocument();
				await client.CreateDocumentAsync(DemoGraphUri, doc);
			}
			else
			{
				var gremlin = manages.ToGremlin();
				await client.CreateGremlinQuery(graph, gremlin).ExecuteNextAsync();
			}

			return manages;
		}

	}
}
