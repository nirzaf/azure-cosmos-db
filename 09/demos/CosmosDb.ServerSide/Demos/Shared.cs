using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace CosmosDb.ServerSide.Demos
{
	public static class Shared
	{
		public static CosmosClient Client { get; private set; }

		static Shared()
		{
			var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var endpoint = config["CosmosEndpoint"];
            var masterKey = config["CosmosMasterKey"];
            Client = new CosmosClient(endpoint, masterKey);
		}
	}
}
