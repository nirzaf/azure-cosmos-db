using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork.Vertices
{
	public class Person : VertexBase
	{
		[JsonProperty(PropertyName = "city")]
		public string City { get; set; }

		[JsonProperty(PropertyName = "age")]
		public int Age { get; set; }

		[JsonProperty(PropertyName = "likes")]
		public string Likes { get; set; }

		public Person()
			: base("person", "City")
		{
		}

	}
}
