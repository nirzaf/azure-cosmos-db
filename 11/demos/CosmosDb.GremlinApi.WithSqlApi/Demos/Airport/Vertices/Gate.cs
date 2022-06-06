using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices
{
	public class Gate : VertexBase
	{
		[JsonProperty(PropertyName = "airline")]
		public string Airline { get; set; }

		[JsonProperty(PropertyName = "city")]
		public string City { get; set; }

		public Gate()
			: base("gate", "City")
		{
		}

	}
}
