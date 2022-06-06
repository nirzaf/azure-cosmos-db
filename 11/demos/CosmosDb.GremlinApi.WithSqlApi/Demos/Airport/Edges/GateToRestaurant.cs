using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices;
using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Edges
{
	public class GateToRestaurant : EdgeBase
	{
		[JsonProperty(PropertyName = "distanceInMinutes")]
		public int DistanceInMinutes { get; set; }

		public GateToRestaurant(Gate gate, Restaurant restaurant)
			: base("gateToRestaurant", gate, restaurant)
		{
		}

	}
}
