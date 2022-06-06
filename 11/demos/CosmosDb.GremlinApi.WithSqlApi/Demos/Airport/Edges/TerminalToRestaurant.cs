using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices;
using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Edges
{
	public class TerminalToRestaurant : EdgeBase
	{
		[JsonProperty(PropertyName = "distanceInMinutes")]
		public int DistanceInMinutes { get; set; }

		public TerminalToRestaurant(Terminal terminal, Restaurant restaurant)
			: base("terminalToRestaurant", terminal, restaurant)
		{
		}

	}
}
