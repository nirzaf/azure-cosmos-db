using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos
{
	public static class DownloadComicBookDemo
	{
		public async static Task Run()
		{
			Debugger.Break();

			var endpoint = ConfigurationManager.AppSettings["ComicBookDemoEndpoint"];
			var masterKey = ConfigurationManager.AppSettings["ComicBookDemoMasterKey"];

			using (var client = new DocumentClient(new Uri(endpoint), masterKey))
			{
				var collUri = UriFactory.CreateDocumentCollectionUri("marvel", "heroes");
				var docs = client.CreateDocumentQuery(collUri, "SELECT * FROM c").ToList();
				Console.WriteLine($"Downloading {docs.Count} documents...");

				foreach (var doc in docs)
				{
					var filename = default(string);
					var dict = (IDictionary<string, object>)doc;
					if (dict.ContainsKey("_isEdge"))
					{
						filename = $"edge {doc.id}";
					}
					else if (dict.ContainsKey("_graph_icon_set"))
					{
						filename = $"icon {dict["_graph_icon_property_value"]}";
					}
					else
					{
						var name = ((JValue)((JProperty)(((JArray)dict["name"])[0]).First()).Value).Value.ToString();
						var label = dict["label"];
						filename = ($"vertex ({label}) {name}");
					}
					filename = filename.Replace(@"/", "-").Replace(":", "-").Replace("|", "-");
					File.WriteAllText($@"C:\Users\lenni\OneDrive\Projects\ComicBookDemo\{filename}.json", doc.ToString());
				}

			}
		}

	}
}
