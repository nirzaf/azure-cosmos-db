using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices;
using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Edges
{
	public class RestaurantToTerminal : EdgeBase
	{
		[JsonProperty(PropertyName = "distanceInMinutes")]
		public int DistanceInMinutes { get; set; }

		public RestaurantToTerminal(Restaurant restaurant, Terminal terminal)
			: base("restaurantToTerminal", restaurant, terminal)
		{
		}

	}
}
