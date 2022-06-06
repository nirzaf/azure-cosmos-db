using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos
{
	public static class Cleanup
	{
		public async static Task Run()
		{
			Console.WriteLine();
			Console.WriteLine(">>> Cleanup <<<");

			var endpoint = ConfigurationManager.AppSettings["CosmosDbEndpoint"];
			var masterKey = ConfigurationManager.AppSettings["CosmosDbMasterKey"];

			//using (var client = new DocumentClient(new Uri(endpoint), masterKey))
			//{
				//		// Delete documents created by demos
				//		Console.WriteLine("Deleting documents created by demos...");
				//		var sql = @"
				//			SELECT c._self, c.address.postalCode
				//			FROM c
				//			WHERE
				//				STARTSWITH(c.name, 'New Customer') OR
				//				STARTSWITH(c.id, '_meta') OR
				//				IS_DEFINED(c.weekdayOff)
				//		";

				//		var collectionUri = UriFactory.CreateDocumentCollectionUri("mydb", "mystore");
				//		var feedOptions = new FeedOptions { EnableCrossPartitionQuery = true };
				//		var documentKeys = client.CreateDocumentQuery(collectionUri, sql, feedOptions).AsEnumerable();
				//		foreach (var documentKey in documentKeys)
				//		{
				//			var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(documentKey.postalCode) };
				//			await client.DeleteDocumentAsync(documentKey._self, requestOptions);
				//		}

				//		var sprocs = client.CreateStoredProcedureQuery(collectionUri).AsEnumerable();

				//		// Delete all stored procedures
				//		Console.WriteLine("Deleting all stored procedures...");
				//		foreach (var sproc in sprocs)
				//		{
				//			await client.DeleteStoredProcedureAsync(sproc.SelfLink);
				//		}

				//		// Delete all user defined functions
				//		Console.WriteLine("Deleting all user defined functions...");
				//		var udfs = client.CreateUserDefinedFunctionQuery(collectionUri).AsEnumerable();
				//		foreach (var udf in udfs)
				//		{
				//			await client.DeleteUserDefinedFunctionAsync(udf.SelfLink);
				//		}

				//		// Delete all triggers
				//		Console.WriteLine("Deleting all triggers...");
				//		var triggers = client.CreateTriggerQuery(collectionUri).AsEnumerable();
				//		foreach (var trigger in triggers)
				//		{
				//			await client.DeleteTriggerAsync(trigger.SelfLink);
				//		}

				//		// Delete all users
				//		Console.WriteLine("Deleting all users...");
				//		var databaseUri = UriFactory.CreateDatabaseUri("mydb");
				//		var users = client.CreateUserQuery(databaseUri).AsEnumerable();
				//		foreach (var user in users)
				//		{
				//			await client.DeleteUserAsync(user.SelfLink);
				//		}

			//}
		}

	}
}
