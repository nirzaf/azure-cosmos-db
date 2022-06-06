using Microsoft.Azure.Cosmos.Scripts;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.ServerSide.Demos
{
	public static class UserDefinedFunctionsDemo
	{
		public async static Task Run()
		{
			Debugger.Break();

			await CreateUserDefinedFunctions();

			await ViewUserDefinedFunctions();

			await Execute_udfRegEx();
			await Execute_udfIsNorthAmerica();
			await Execute_udfFormatCityStateZip();

			await DeleteUserDefinedFunctions();
		}

		private async static Task CreateUserDefinedFunctions()
		{
			Console.Clear();
			Console.WriteLine(">>> Create User Defined Functions <<<");
			Console.WriteLine();

			await CreateUserDefinedFunction("udfRegEx");
			await CreateUserDefinedFunction("udfIsNorthAmerica");
			await CreateUserDefinedFunction("udfFormatCityStateZip");
		}

		private async static Task CreateUserDefinedFunction(string udfId)
		{
			var udfBody = File.ReadAllText($@"Server\{udfId}.js");
			var udfProps = new UserDefinedFunctionProperties
			{
				Id = udfId,
				Body = udfBody
			};

			var container = Shared.Client.GetContainer("mydb", "mystore");
			var result = await container.Scripts.CreateUserDefinedFunctionAsync(udfProps);
			Console.WriteLine($"Created user defined function  {udfId} ({result.RequestCharge} RUs);");
		}

		private async static Task ViewUserDefinedFunctions()
		{
			Console.Clear();
			Console.WriteLine(">>> View UDFs <<<");
			Console.WriteLine();

			var container = Shared.Client.GetContainer("mydb", "mystore");

			var iterator = container.Scripts.GetUserDefinedFunctionQueryIterator<UserDefinedFunctionProperties>();
			var udfs = await iterator.ReadNextAsync();

			var count = 0;
			foreach (var udf in udfs)
			{
				count++;
				Console.WriteLine($" UDF Id: {udf.Id};");
			}

			Console.WriteLine();
			Console.WriteLine($"Total UDFs: {count}");
		}

		private async static Task Execute_udfRegEx()
		{
			Console.Clear();
			Console.WriteLine("Querying for Rental customers");

			var sql = "SELECT c.id, c.name FROM c WHERE udf.udfRegEx(c.name, 'Rental') != null";

			var container = Shared.Client.GetContainer("mydb", "mystore");
			var documents = (await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync()).ToList();

			Console.WriteLine($"Found {documents.Count} Rental customers:");
			foreach (var document in documents)
			{
				Console.WriteLine($" {document.name} ({document.id})");
			}
		}

		private async static Task Execute_udfIsNorthAmerica()
		{
			Console.Clear();
			Console.WriteLine("Querying for North American customers");

			var sql = @"
				SELECT c.name, c.address.countryRegionName
				FROM c
				WHERE udf.udfIsNorthAmerica(c.address.countryRegionName) = true";

			var container = Shared.Client.GetContainer("mydb", "mystore");
			var documents = (await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync()).ToList();

			Console.WriteLine($"Found {documents.Count} North American customers; first 20:");
			foreach (var document in documents.Take(20))
			{
				Console.WriteLine($" {document.name}, {document.countryRegionName}");
			}

			sql = @"
				SELECT c.name, c.address.countryRegionName
				FROM c
				WHERE udf.udfIsNorthAmerica(c.address.countryRegionName) = false";

			Console.WriteLine();
			Console.WriteLine("Querying for non North American customers");

			documents = (await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync()).ToList();

			Console.WriteLine($"Found {documents.Count} non North American customers; first 20:");
			foreach (var document in documents.Take(20))
			{
				Console.WriteLine($" {document.name}, {document.countryRegionName}");
			}
		}

		private async static Task Execute_udfFormatCityStateZip()
		{
			Console.WriteLine();
			Console.WriteLine("Listing names with city, state, zip (first 20)");

			var sql = "SELECT c.name, udf.udfFormatCityStateZip(c) AS csz FROM c";

			var container = Shared.Client.GetContainer("mydb", "mystore");
			var documents = (await (container.GetItemQueryIterator<dynamic>(sql)).ReadNextAsync()).ToList();
			foreach (var document in documents.Take(20))
			{
				Console.WriteLine($" {document.name} located in {document.csz}");
			}
		}

		private async static Task DeleteUserDefinedFunctions()
		{
			Console.WriteLine();
			Console.WriteLine(">>> Delete User Defined Functions <<<");
			Console.WriteLine();

			await DeleteUserDefinedFunction("udfRegEx");
			await DeleteUserDefinedFunction("udfIsNorthAmerica");
			await DeleteUserDefinedFunction("udfFormatCityStateZip");
		}

		private async static Task DeleteUserDefinedFunction(string udfId)
		{
			var container = Shared.Client.GetContainer("mydb", "mystore");
			await container.Scripts.DeleteUserDefinedFunctionAsync(udfId);

			Console.WriteLine($"Deleted UDF: {udfId}");
		}

	}
}
