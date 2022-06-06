using Microsoft.Azure.Cosmos;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.DotNetSdk.Demos
{
	public static class IndexingDemo
	{
		public async static Task Run()
		{
			Debugger.Break();

			await ExcludedPaths();
			await CompositeIndexes();
			await SpatialIndexes();	
		}

		private async static Task ExcludedPaths()
		{
			Console.Clear();
			Console.WriteLine(">>> Exclude Index Paths <<<");
			Console.WriteLine();

			var containerProps = new ContainerProperties
			{
				Id = "customindexing",
				PartitionKeyPath = "/zipCode",
			};

			// Exclude everything under /miscellaneous from indexing, except for /miscellaneous/rating
			containerProps.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });
			containerProps.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/miscellaneous/*" });
			containerProps.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/miscellaneous/rating/?" });

			await Shared.Client.GetDatabase("mydb").CreateContainerAsync(containerProps, 400);

			// Load documents 
			var container = Shared.Client.GetContainer("mydb", "customindexing");

			for (var i = 1; i <= 100; i++)
			{
				dynamic doc = new
				{
					id = Guid.NewGuid().ToString(),
					zipCode = "12345",
					title = $"Document {i}",
					rating = i,
					miscellaneous = new
					{
						title = $"Document {i}",
						rating = i,
					}
				};
				await container.CreateItemAsync(doc, new PartitionKey(doc.zipCode));
			}

			// Querying on indexed properties is most efficient

			var sql = "SELECT * FROM c WHERE c.title = 'Document 90'";
			var result = await container.GetItemQueryIterator<dynamic>(sql).ReadNextAsync();
			Console.WriteLine($"Query indexed string property     Cost = {result.RequestCharge} RUs");

			sql = "SELECT * FROM c WHERE c.rating = 90";
			result = await container.GetItemQueryIterator<dynamic>(sql).ReadNextAsync();
			Console.WriteLine($"Query indexed number property     Cost = {result.RequestCharge} RUs");
			Console.WriteLine();

			// Querying on unindexed properties requires a sequential scan, and costs more RUs

			sql = "SELECT * FROM c WHERE c.miscellaneous.title = 'Document 90'";
			result = await container.GetItemQueryIterator<dynamic>(sql).ReadNextAsync();
			Console.WriteLine($"Query unindexed string property   Cost = {result.RequestCharge} RUs");

			// Excluded property that was explictly included gets indexed

			sql = "SELECT * FROM c WHERE c.miscellaneous.rating = 90";
			result = await container.GetItemQueryIterator<dynamic>(sql).ReadNextAsync();
			Console.WriteLine($"Query indexed number property     Cost = {result.RequestCharge} RUs");
			Console.WriteLine();

			// Sorting on indexed properties is supported

			sql = "SELECT * FROM c ORDER BY c.title";
			result = await container.GetItemQueryIterator<dynamic>(sql).ReadNextAsync();
			var docs = result.ToList();
			Console.WriteLine($"Sort on indexed string property   Cost = {result.RequestCharge} RUs");

			sql = "SELECT * FROM c ORDER BY c.rating";
			result = await container.GetItemQueryIterator<dynamic>(sql).ReadNextAsync();
			docs = result.ToList();
			Console.WriteLine($"Sort on indexed number property   Cost = {result.RequestCharge} RUs");
			Console.WriteLine();

			// Sorting on unindexed properties is not supported

			sql = "SELECT * FROM c ORDER BY c.miscellaneous.title";
			try
			{
				result = await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Sort on unindexed property failed");
				Console.WriteLine(ex.Message);
			}

			// Delete the container
			await container.DeleteContainerAsync();
		}

		private async static Task CompositeIndexes()
		{
			Console.Clear();
			Console.WriteLine(">>> Composite Indexes <<<");
			Console.WriteLine();

			var sql = @"
				SELECT TOP 20 *
				FROM c
				WHERE c.address.countryRegionName = 'United States'
				ORDER BY
					c.address.location.stateProvinceName,
					c.address.location.city,
					c.name
			";

			var container = Shared.Client.GetContainer("mydb", "mystore");

			// Query won't work without explicitly defined composite indexes
			Console.WriteLine("Multi-property ORDER BY without composite indexes");
			try
			{
				var page1 = await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine();
			}

			// Retrieve the container's current indexing policy
			var response = await container.ReadContainerAsync();
			var containerProperties = response.Resource;

			// Add composite indexes to the indexing policy
			var compositePaths = new Collection<CompositePath>
			{
				new CompositePath { Path = "/address/location/stateProvinceName", Order = CompositePathSortOrder.Ascending },
				new CompositePath { Path = "/address/location/city", Order = CompositePathSortOrder.Ascending },
				new CompositePath { Path = "/name", Order = CompositePathSortOrder.Ascending },
			};
			containerProperties.IndexingPolicy.CompositeIndexes.Add(compositePaths);
			await container.ReplaceContainerAsync(containerProperties);

			// The query works now
			Console.WriteLine("Multi-property ORDER BY with composite indexes");
			var page = await (container.GetItemQueryIterator<Customer>(sql)).ReadNextAsync();
			foreach (var doc in page)
			{
				Console.WriteLine($"{doc.Name,-42}{doc.Address.Location.StateProvinceName,-12}{doc.Address.Location.City,-30}");
			}

			// Remove composite indexes from the indexing policy
			containerProperties.IndexingPolicy.CompositeIndexes.Clear();
			await container.ReplaceContainerAsync(containerProperties);
		}

		private async static Task SpatialIndexes()
		{
			Console.Clear();
			Console.WriteLine(">>> Spatial Indexes <<<");
			Console.WriteLine();

			var containerDef = new ContainerProperties
			{
				Id = "spatialindexing",
				PartitionKeyPath = "/state",
			};

			// Add a spatial index for the point data in the GeoJSON property /geo1
			var geoPath = new SpatialPath { Path = "/geo1/?" };
			geoPath.SpatialTypes.Add(SpatialType.Point);
			containerDef.IndexingPolicy.SpatialIndexes.Add(geoPath);

			await Shared.Client.GetDatabase("mydb").CreateContainerAsync(containerDef, 1000);
			var container = Shared.Client.GetContainer("mydb", "spatialindexing");

			for (var i = 1; i <= 1000; i++)
			{
				var longitude = (i % 100 == 0 ? -73.992 : -119.417931);
				var latitude = (i % 100 == 0 ? 40.73104 : 36.778259);
				var state = (i % 100 == 0 ? "NY" : "CA");
				dynamic doc = new
				{
					id = Guid.NewGuid().ToString(),
					title = $"Document {i}",
					state,
					geo1 = new
					{
						type = "Point",
						coordinates = new[] { longitude, latitude },
					},
					geo2 = new
					{
						type = "Point",
						coordinates = new[] { longitude, latitude },
					},
				};
				await container.CreateItemAsync(doc, new PartitionKey(doc.state));
			}

			var sql = @"
				SELECT * FROM c WHERE
				 ST_DISTANCE(c.geo1, {
				   'type': 'Point',
				   'coordinates': [-73.992, 40.73104]
				 }) <= 10";

			var result = await container.GetItemQueryIterator<dynamic>(sql).ReadNextAsync();
			var list = result.ToList();
			Console.WriteLine($"Query indexed spatial property    Cost = {result.RequestCharge} RUs for {list.Count} results");

			sql = @"
				SELECT * FROM c WHERE
				 ST_DISTANCE(c.geo2, {
				   'type': 'Point',
				   'coordinates': [-73.992, 40.73104]
				 }) <= 10";

			result = await container.GetItemQueryIterator<dynamic>(sql).ReadNextAsync();
			list = result.ToList();
			Console.WriteLine($"Query unindexed spatial property  Cost = {result.RequestCharge} RUs for {list.Count} results");

			// Delete the container
			await container.DeleteContainerAsync();
		}

	}
}
