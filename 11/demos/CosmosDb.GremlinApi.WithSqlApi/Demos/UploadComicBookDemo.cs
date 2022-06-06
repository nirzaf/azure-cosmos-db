using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos
{
	public static class UploadComicBookDemo
	{
		public async static Task Run()
		{
			Debugger.Break();

			var endpoint = ConfigurationManager.AppSettings["CosmosDbEndpoint"];
			var masterKey = ConfigurationManager.AppSettings["CosmosDbMasterKey"];

			var filenames =
				Directory.GetFiles(@"..\..\ComicBookData", "vertex*.*").Concat(
				Directory.GetFiles(@"..\..\ComicBookData", "icon*.*").Concat(
				Directory.GetFiles(@"..\..\ComicBookData", "edge*.*")));

			var uri = UriFactory.CreateDocumentCollectionUri("GraphDb", "ComicBook");

			using (var client = new DocumentClient(new Uri(endpoint), masterKey))
			{
				var i = 0;
				foreach (var filename in filenames)
				{
					i++;
					var documentDefinitionJson = File.ReadAllText(filename);
					if (filename.Contains("icon ") && documentDefinitionJson.Contains("heroes"))
					{
						documentDefinitionJson = documentDefinitionJson.Replace("heroes", "ComicBook");
					}
					if (filename.Contains("edge ") && documentDefinitionJson.Contains("seenin"))
					{
						documentDefinitionJson = documentDefinitionJson.Replace("seenin", "seenIn");
					}
					var documentObject = JsonConvert.DeserializeObject(documentDefinitionJson);
					await client.CreateDocumentAsync(uri, documentObject);
					Console.WriteLine($"{i}. {filename}");
				}

			}
		}

	}
}
