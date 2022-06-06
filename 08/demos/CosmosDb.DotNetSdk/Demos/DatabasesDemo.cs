using Microsoft.Azure.Cosmos;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CosmosDb.DotNetSdk.Demos
{
	public static class DatabasesDemo
	{
		public async static Task Run()
		{
			Debugger.Break();

			await ViewDatabases();

			await CreateDatabase();
			await ViewDatabases();

			await DeleteDatabase();
			await ViewDatabases();
		}

		private async static Task ViewDatabases()
		{
			Console.WriteLine();
			Console.WriteLine(">>> View Databases <<<");

			var iterator = Shared.Client.GetDatabaseQueryIterator<DatabaseProperties>();
			var databases = await iterator.ReadNextAsync();

			var count = 0;
			foreach (var database in databases)
			{
				count++;
				Console.WriteLine($" Database Id: {database.Id}; Modified: {database.LastModified}");
			}

			Console.WriteLine();
			Console.WriteLine($"Total databases: {count}");
		}

		private async static Task CreateDatabase()
		{
			Console.WriteLine();
			Console.WriteLine(">>> Create Database <<<");

			var result = await Shared.Client.CreateDatabaseAsync("MyNewDatabase");
			var database = result.Resource;

			Console.WriteLine($" Database Id: {database.Id}; Modified: {database.LastModified}");
		}

		private async static Task DeleteDatabase()
		{
			Console.WriteLine();
			Console.WriteLine(">>> Delete Database <<<");

			await Shared.Client.GetDatabase("MyNewDatabase").DeleteAsync();
		}

	}
}
