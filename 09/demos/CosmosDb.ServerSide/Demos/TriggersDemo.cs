using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.ServerSide.Demos
{
    public static class TriggersDemo
	{
		public async static Task Run()
		{
			Debugger.Break();

			await CreateTriggers();

			await ViewTriggers();

			await Execute_trgValidateDocument();
			await Execute_trgUpdateMetadata();

			await DeleteTriggers();
		}

		private async static Task CreateTriggers()
		{
			Console.WriteLine();
			Console.WriteLine(">>> Create Triggers <<<");
			Console.WriteLine();

			// Create pre-trigger
			var trgValidateDocument = File.ReadAllText(@"Server\trgValidateDocument.js");
			await CreateTrigger("trgValidateDocument", trgValidateDocument, TriggerType.Pre, TriggerOperation.All);

			// Create post-trigger
			var trgUpdateMetadata = File.ReadAllText(@"Server\trgUpdateMetadata.js");
			await CreateTrigger("trgUpdateMetadata", trgUpdateMetadata, TriggerType.Post, TriggerOperation.Create);
		}

		private async static Task CreateTrigger(
			string triggerId,
			string triggerBody,
			TriggerType triggerType,
			TriggerOperation triggerOperation)
		{
			var triggerProps = new TriggerProperties
			{
				Id = triggerId,
				Body = triggerBody,
				TriggerType = triggerType,
				TriggerOperation = triggerOperation
			};

			var container = Shared.Client.GetContainer("mydb", "mystore");
			var result = await container.Scripts.CreateTriggerAsync(triggerProps);
			Console.WriteLine($"Created trigger {triggerId}; ({result.RequestCharge} RUs)");
		}

		private async static Task ViewTriggers()
		{
			Console.WriteLine();
			Console.WriteLine(">>> View Triggers <<<");
			Console.WriteLine();

			var container = Shared.Client.GetContainer("mydb", "mystore");

			var iterator = container.Scripts.GetTriggerQueryIterator<TriggerProperties>();
			var triggers = await iterator.ReadNextAsync();

			var count = 0;
			foreach (var trigger in triggers)
			{
				count++;
				Console.WriteLine($" Trigger Id: {trigger.Id}; Type: {trigger.TriggerType}; Operation: {trigger.TriggerOperation};");
			}

			Console.WriteLine();
			Console.WriteLine($"Total triggers: {count}");
		}

		private static async Task Execute_trgValidateDocument()
		{
			// Create three documents
			var doc1Id = await CreateDocumentWithValidation("mon");       // Monday
			var doc2Id = await CreateDocumentWithValidation("THURS");     // Thursday
			var doc3Id = await CreateDocumentWithValidation("sonday");    // error - won't get created

			// Update one of them
			await UpdateDocumentWithValidation(doc2Id, "FRI");            // Thursday > Friday

			// Delete them
			var container = Shared.Client.GetContainer("mydb", "mystore");
			var pk = new PartitionKey("12345");
			await container.DeleteItemAsync<dynamic>(doc1Id, pk);
			await container.DeleteItemAsync<dynamic>(doc2Id, pk);
		}

		private async static Task<string> CreateDocumentWithValidation(string weekdayOff)
		{
			dynamic document = new
			{
				id = Guid.NewGuid().ToString(),
				name = "John Doe",
				address = new { postalCode = "12345" },
				weekdayOff
			};

			var container = Shared.Client.GetContainer("mydb", "mystore");
			var pk = new PartitionKey("12345");

			try
			{
                var options = new ItemRequestOptions { PreTriggers = new[] { "trgValidateDocument" } };
                var result = await container.CreateItemAsync<dynamic>(document, pk, options);
                document = result.Resource;

				Console.WriteLine(" Result:");
				Console.WriteLine($"  Id = {document.id}");
				Console.WriteLine($"  Weekday off = {document.weekdayOff}");
				Console.WriteLine($"  Weekday # off = {document.weekdayNumberOff}");
				Console.WriteLine();

				return document.id;
			}
			catch (CosmosException ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				Console.WriteLine();

				return null;
			}
		}

		private async static Task UpdateDocumentWithValidation(string documentId, string weekdayOff)
		{
			var sql = $"SELECT * FROM c WHERE c.id = '{documentId}'";
			var container = Shared.Client.GetContainer("mydb", "mystore");
			var document = (await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync()).FirstOrDefault();

			document.weekdayOff = weekdayOff;

			var pk = new PartitionKey("12345");
            var options = new ItemRequestOptions { PreTriggers = new[] { "trgValidateDocument" } };
            var result = await container.ReplaceItemAsync(document, documentId, pk, options);
            document = result.Resource;

            Console.WriteLine(" Result:");
			Console.WriteLine($"  Id = {document.id}");
			Console.WriteLine($"  Weekday off = {document.weekdayOff}");
			Console.WriteLine($"  Weekday # off = {document.weekdayNumberOff}");
			Console.WriteLine();
		}

		private async static Task Execute_trgUpdateMetadata()
		{
			Console.Clear();

			// Show no metadata documents
			await ViewMetaDocs();

			// Create a bunch of documents across two partition keys
			var docs = new List<dynamic>
			{
				// 11229
				new { id = "11229a", address = new { postalCode = "11229" }, name = "New Customer ABCD" },
				new { id = "11229b", address = new { postalCode = "11229" }, name = "New Customer ABC" },
				new { id = "11229c", address = new { postalCode = "11229" }, name = "New Customer AB" },			// smallest
				new { id = "11229d", address = new { postalCode = "11229" }, name = "New Customer ABCDEF" },
				new { id = "11229e", address = new { postalCode = "11229" }, name = "New Customer ABCDEFG" },		// largest
				new { id = "11229f", address = new { postalCode = "11229" }, name = "New Customer ABCDE" },
				// 11235
				new { id = "11235a", address = new { postalCode = "11235" }, name = "New Customer AB" },
				new { id = "11235b", address = new { postalCode = "11235" }, name = "New Customer ABCDEFGHIJKL" },	// largest
				new { id = "11235c", address = new { postalCode = "11235" }, name = "New Customer ABC" },
				new { id = "11235d", address = new { postalCode = "11235" }, name = "New Customer A" },				// smallest
				new { id = "11235e", address = new { postalCode = "11235" }, name = "New Customer ABC" },
				new { id = "11235f", address = new { postalCode = "11235" }, name = "New Customer ABCDE" },
			};

            var container = Shared.Client.GetContainer("mydb", "mystore");
            foreach (var doc in docs)
			{
                var pk = new PartitionKey(doc.address.postalCode);
                var options = new ItemRequestOptions { PostTriggers = new[] { "trgUpdateMetadata" } };
                var result = await container.CreateItemAsync(doc, pk, options);
			}

			// Show two metadata documents
			await ViewMetaDocs();

			// Cleanup
			var sql = @"
				SELECT c.id, c.address.postalCode
				FROM c
				WHERE c.address.postalCode IN('11229', '11235')
			";

			var documentIds = await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync();
			foreach (var documentKey in documentIds)
			{
                var id = documentKey.id.Value;
                var pk = documentKey.postalCode.Value;

                await container.DeleteItemAsync<dynamic>(id, new PartitionKey(pk));
			}
		}

		private static async Task ViewMetaDocs()
		{
			var sql = @"SELECT * FROM c WHERE c.isMetaDoc";

			var container = Shared.Client.GetContainer("mydb", "mystore");
			var metaDocs = (await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync()).ToList();

			Console.WriteLine();
			Console.WriteLine($" Found {metaDocs.Count} metadata documents:");
			foreach (var metaDoc in metaDocs)
			{
				Console.WriteLine();
				Console.WriteLine($"  MetaDoc ID: {metaDoc.id}");
				Console.WriteLine($"  Metadata for: {metaDoc.address.postalCode}");
				Console.WriteLine($"  Smallest doc size: {metaDoc.minSize} ({metaDoc.minSizeId})");
				Console.WriteLine($"  Largest doc size: {metaDoc.maxSize} ({metaDoc.maxSizeId})");
				Console.WriteLine($"  Total doc size: {metaDoc.totalSize}");
			}
		}

		private async static Task DeleteTriggers()
		{
			Console.WriteLine();
			Console.WriteLine(">>> Delete Triggers <<<");
			Console.WriteLine();

			await DeleteTrigger("trgValidateDocument");
			await DeleteTrigger("trgUpdateMetadata");
		}

		private async static Task DeleteTrigger(string triggerId)
		{
			var container = Shared.Client.GetContainer("mydb", "mystore");
			await container.Scripts.DeleteTriggerAsync(triggerId);

			Console.WriteLine($"Deleted trigger: {triggerId}");
		}

	}
}
