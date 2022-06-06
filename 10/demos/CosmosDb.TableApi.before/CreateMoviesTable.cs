using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;

namespace CosmosDb.TableApi
{
	public static class CreateMoviesTable
	{
		public static void Run()
		{
			var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
			var account = CloudStorageAccount.Parse(connectionString);
			var client = account.CreateCloudTableClient();

			var table = client.GetTableReference("Movies");
			table.Create();

			var movie1 = new MovieEntity("sci-fi", "Star Wars IV - A New Hope")
			{
				Year = 1977,
				Length = "2hr, 1min",
				Description = "Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a Wookiee and two droids to save the galaxy from the Empire's world-destroying battle-station while also attempting to rescue Princess Leia from the evil Darth Vader."
			};
			var movie2 = new MovieEntity("sci-fi", "Star Wars V - The Empire Strikes Back")
			{
				Year = 1980,
				Length = "2hr, 4min",
				Description = "After the rebels are overpowered by the Empire on the ice planet Hoth, Luke Skywalker begins Jedi training with Yoda. His friends accept shelter from a questionable ally as Darth Vader hunts them in a plan to capture Luke."
			};
			var batchOperation = new TableBatchOperation();
			batchOperation.Insert(movie1);
			batchOperation.Insert(movie2);
			table.ExecuteBatch(batchOperation);
		}

	}
}
