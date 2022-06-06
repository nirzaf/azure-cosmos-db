using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.ServerSide.Demos
{
    public static class Cleanup
	{
		public async static Task Run()
		{
			Console.WriteLine();
			Console.WriteLine(">>> Cleanup <<<");

			var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

			var endpoint = config["CosmosEndpoint"];
			var masterKey = config["CosmosMasterKey"];

			using (var client = new CosmosClient(endpoint, masterKey))
			{
				// Delete documents created by demos
				Console.WriteLine("Deleting documents created by demos...");
				var sql = @"
					SELECT c.id, c.address.postalCode
					FROM c
					WHERE
						STARTSWITH(c.name, 'New Customer') OR
						STARTSWITH(c.id, '_meta') OR
						IS_DEFINED(c.weekdayOff) OR
						STARTSWITH(c.id, 'DUPEJ') OR
						c.lastName = 'Einstein'
				";

                var container = Shared.Client.GetContainer("mydb", "mystore");

                var documentKeys = (await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync()).ToList();
				foreach (var documentKey in documentKeys)
				{
					string id = documentKey.id;
					string pk = documentKey.postalCode;
					await container.DeleteItemAsync<dynamic>(id, new PartitionKey(pk));
				}

                // Delete all stored procedures
                Console.WriteLine("Deleting all stored procedures...");
                var sprocIterator = container.Scripts.GetStoredProcedureQueryIterator<StoredProcedureProperties>();
                var sprocs = await sprocIterator.ReadNextAsync();
                foreach (var sproc in sprocs)
                {
                    await container.Scripts.DeleteStoredProcedureAsync(sproc.Id);
                }

                // Delete all triggers
                Console.WriteLine("Deleting all triggers...");
                var triggerIterator = container.Scripts.GetTriggerQueryIterator<TriggerProperties>();
                var triggers = await triggerIterator.ReadNextAsync();
                foreach (var trigger in triggers)
                {
                    await container.Scripts.DeleteTriggerAsync(trigger.Id);
                }

                // Delete all user defined functions
                Console.WriteLine("Deleting all user defined functions...");
                var udfIterator = container.Scripts.GetUserDefinedFunctionQueryIterator<UserDefinedFunctionProperties>();
                var udfs = await sprocIterator.ReadNextAsync();
                foreach (var udf in udfs)
                {
                    await container.Scripts.DeleteUserDefinedFunctionAsync(udf.Id);
                }

            }
		}

	}
}
