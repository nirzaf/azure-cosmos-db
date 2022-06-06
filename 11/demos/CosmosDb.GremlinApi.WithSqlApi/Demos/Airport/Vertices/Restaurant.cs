using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices
{
	public class Restaurant : VertexBase
	{
		[JsonProperty(PropertyName = "city")]
		public string City { get; set; }

		[JsonProperty(PropertyName = "rating")]
		public decimal Rating { get; set; }

		[JsonProperty(PropertyName = "averagePrice")]
		public decimal AveragePrice { get; set; }

		public Restaurant()
			: base("restaurant", "City")
		{
		}

	}
}
