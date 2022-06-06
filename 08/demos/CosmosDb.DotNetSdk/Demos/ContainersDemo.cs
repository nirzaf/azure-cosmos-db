using Microsoft.Azure.Cosmos;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CosmosDb.DotNetSdk.Demos
{
	public static class ContainersDemo
	{
		public async static Task Run()
		{
			Debugger.Break();

			await ViewContainers();

			await CreateContainer("MyContainer1");
			await CreateContainer("MyContainer2", 1000, "/state");
			await ViewContainers();

			await DeleteContainer("MyContainer1");
			await DeleteContainer("MyContainer2");
			await ViewContainers();
		}

		private static async Task ViewContainers()
		{
			Console.WriteLine();
			Console.WriteLine(">>> View Containers in mydb <<<");

			var database = Shared.Client.GetDatabase("mydb");
			var iterator = database.GetContainerQueryIterator<ContainerProperties>();
			var containers = await iterator.ReadNextAsync();

			var count = 0;
			foreach (var container in containers)
			{
				count++;
				Console.WriteLine();
				Console.WriteLine($" Container #{count}");
				await ViewContainer(container);
			}

			Console.WriteLine();
			Console.WriteLine($"Total containers in mydb database: {count}");
		}

		private async static Task ViewContainer(ContainerProperties containerProperties)
		{
			Console.WriteLine($"     Container ID: {containerProperties.Id}");
			Console.WriteLine($"    Last Modified: {containerProperties.LastModified}");
			Console.WriteLine($"    Partition Key: {containerProperties.PartitionKeyPath}");

			var container = Shared.Client.GetContainer("mydb", containerProperties.Id);
			var throughput = await container.ReadThroughputAsync();

			Console.WriteLine($"       Throughput: {throughput}");
		}

		private async static Task CreateContainer(
			string containerId,
			int throughput = 400,
			string partitionKey = "/partitionKey")
		{
			Console.WriteLine();
			Console.WriteLine($">>> Create Container {containerId} in mydb <<<");
			Console.WriteLine();
			Console.WriteLine($" Throughput: {throughput} RU/sec");
			Console.WriteLine($" Partition key: {partitionKey}");
			Console.WriteLine();

			var containerDef = new ContainerProperties
			{
				Id = containerId,
				PartitionKeyPath = partitionKey,
			};

			var database = Shared.Client.GetDatabase("mydb");
			var result = await database.CreateContainerAsync(containerDef, throughput);
			var container = result.Resource;

			Console.WriteLine("Created new container");
			await ViewContainer(container);	// Intermittent failures!
		}

		private async static Task DeleteContainer(string containerId)
		{
			Console.WriteLine();
			Console.WriteLine($">>> Delete Container {containerId} in mydb <<<");

			var container = Shared.Client.GetContainer("mydb", containerId);
			await container.DeleteContainerAsync();

			Console.WriteLine($"Deleted container {containerId} from database mydb");
		}

	}
}
