using Microsoft.Azure.CosmosDB.Table;   // replaces Microsoft.WindowsAzure.Storage.Table
using Microsoft.Azure.Storage;          // replaces Microsoft.WindowsAzure.Storage
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace CosmosDb.TableApi
{
	public static class TableDemo
	{
		public static void Run()
		{
			Debugger.Break();

			var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
			var account = CloudStorageAccount.Parse(connectionString);
			var client = account.CreateCloudTableClient();
			var table = client.GetTableReference("Movies");

			ViewSciFiMovies(table);

			AddSciFiMovie(table);

			ViewSciFiMovies(table);

			DeleteSciFiMovie(table);

			Console.ReadKey();
		}

		private static void ViewSciFiMovies(CloudTable table)
		{
			var sciFiFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "sci-fi");
			var query = new TableQuery<MovieEntity>().Where(sciFiFilter);
			var movies = table.ExecuteQuery(query);

			Console.WriteLine($"Found {movies.Count()} Sci-Fi movies");
			Console.WriteLine();

			foreach (var movie in movies)
			{
				Console.WriteLine($" Title: {movie.Title}");
				Console.WriteLine($" Genre: {movie.Genre}");
				Console.WriteLine($" Year: {movie.Year}");
				Console.WriteLine($" Length: {movie.Length}");
				Console.WriteLine($" Description: {movie.Description}");
				Console.WriteLine();
			}
		}

		private static void AddSciFiMovie(CloudTable table)
		{
			var movie = new MovieEntity("sci-fi", "Star Wars VI - Return of the Jedi")
			{
				Year = 1983,
				Length = "2hr, 11min",
				Description = "After a daring mission to rescue Han Solo from Jabba the Hutt, the rebels dispatch to Endor to destroy a more powerful Death Star. Meanwhile, Luke struggles to help Vader back from the dark side without falling into the Emperor's trap."
			};
			var insertOperation = TableOperation.Insert(movie);
			var result = table.Execute(insertOperation);
		}

		private static void DeleteSciFiMovie(CloudTable table)
		{
			var queryOperation = TableOperation.Retrieve<MovieEntity>("sci-fi", "Star Wars VI - Return of the Jedi");
			var result = table.Execute(queryOperation);
			var entity = (MovieEntity)result.Result;

			var deleteOperation = TableOperation.Delete(entity);
			table.Execute(deleteOperation);
		}

	}
}
