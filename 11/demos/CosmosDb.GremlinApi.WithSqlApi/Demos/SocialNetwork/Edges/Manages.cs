using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork.Vertices;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.SocialNetwork.Edges
{
	public class Manages : EdgeBase
	{
		public Manages(Person manager, Person person)
			: base("manages", manager, person)
		{
		}

	}
}
