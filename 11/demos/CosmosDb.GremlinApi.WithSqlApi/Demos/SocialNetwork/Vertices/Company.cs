using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork.Vertices
{
	public class Company : VertexBase
	{
		[JsonProperty(PropertyName = "founded")]
		public int Founded { get; set; }

		[JsonProperty(PropertyName = "city")]
		public string City { get; set; }

		public Company()
			: base("company", "City")
		{
		}

	}
}
