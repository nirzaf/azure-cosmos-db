using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork.Vertices;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork.Edges
{
	public class WorksAt : EdgeBase
	{
		[JsonProperty(PropertyName = "weekends")]
		public bool Weekends { get; set; }

		public WorksAt(Person person, Company company)
			: base("worksAt", person, company)
		{
		}

	}
}
