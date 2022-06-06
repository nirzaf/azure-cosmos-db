using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices
{
	public class Terminal : VertexBase
	{
		[JsonProperty(PropertyName = "city")]
		public string City { get; set; }

		public Terminal()
			: base("terminal", "City")
		{
		}

	}
}
