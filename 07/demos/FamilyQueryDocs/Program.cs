using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilyQueryDocs
{
	class Program
	{
		static void Main(string[] args)
		{
			CreateFamiliesCollectionForQueryDemos().Wait();
		}

		public static async Task CreateFamiliesCollectionForQueryDemos()
		{
			var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

			var endpoint = config["CosmosEndpoint"];
			var masterKey = config["CosmosMasterKey"];

			var client = new CosmosClient(endpoint, masterKey);

			var database = (await client.CreateDatabaseAsync("Families")).Database;
			var container = (await database.CreateContainerAsync("Families", "/location/state", 400)).Container;

			var docs = GenerateFamilyDocuments();

			foreach (var doc in docs)
			{
				await container.CreateItemAsync(doc, new PartitionKey(doc.location.state));
			}

			Console.WriteLine("Successfully created Families collection for SQL Query demos");
		}

		private static IEnumerable<dynamic> GenerateFamilyDocuments()
		{
			dynamic andersen = new {
				id = "AndersenFamily",
				lastName = "Andersen",
				parents = new dynamic [] {
					new {
						firstName = "Thomas",
						relationship = "father"
					},
					new {
						firstName = "Mary Kay",
						relationship = "mother"
					}
				},
				children = new dynamic [] {
					new {
						firstName = "Henriette Thaulow",
						gender = "female",
						grade = 5,
						pets = new dynamic [] {
							new {
								givenName = "Fluffy",
								type = "Rabbit"
							}
						}
					}
				},
				location = new {
					state = "WA",
					county = "King",
					city = "Seattle"
				},
				geo = new {
					type = "Point",
					coordinates = new [] { -122.3295, 47.60357 }
				},
				isRegistered = true
			};

			dynamic smith = new {
				id = "SmithFamily",
				parents = new dynamic[] {
					new {
						familyName = "Smith",
						givenName = "James"
					},
					new {
						familyName = "Curtis",
						givenName = "Helen"
					}
				},
				children = new dynamic [] {
					new {
						givenName = "Michelle",
						gender = "female",
						grade = 1,
					},
					new {
						givenName = "John",
						gender = "male",
						grade = 7,
						pets = new dynamic [] {
							new {
								givenName = "Tweetie",
								type = "Bird"
							}
						}
					}
				},
				location = new {
					state = "NY",
					county = "Queens",
					city = "Forest Hills"
				},
				geo = new {
					type = "Point",
					coordinates = new [] { -73.84791, 40.72266 }
				},
				isRegistered = true
			};

			dynamic wakefield = new
			{
				id = "WakefieldFamily",
				parents = new dynamic[] {
					new {
						familyName = "Wakefield",
						givenName = "Robin"
					},
					new {
						familyName = "Miller",
						givenName = "Ben"
					}
				},
				children = new dynamic[] {
					new {
						familyName = "Merriam",
						givenName = "Jesse",
						gender = "female",
						grade = 6,
						pets = new dynamic [] {
							new {
								givenName = "Charlie Brown",
								type = "Dog"
							},
							new {
								givenName = "Tiger",
								type = "Cat"
							},
							new {
								givenName = "Princess",
								type = "Cat"
							}
						}
					},
					new {
						familyName = "Miller",
						givenName = "Lisa",
						gender = "female",
						grade = 3,
						pets = new dynamic [] {
							new {
								givenName = "Jake",
								type = "Snake"
							}
						}
					}
				},
				location = new
				{
					state = "NY",
					county = "Manhattan",
					city = "NY"
				},
				geo = new
				{
					type = "Point",
					coordinates = new[] { -73.992, 40.73103 }
				},
				isRegistered = false
			};

			var documents = new List<dynamic>()
			{
				andersen,
				smith,
				wakefield
			};

			return documents;
		}

	}
}
