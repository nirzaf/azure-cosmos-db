using Newtonsoft.Json.Linq;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper
{
	public interface IGraphObject
	{
		string Id { get; set; }
		string Label { get; }

		string ToGremlin();
		JObject ToDocument();
	}
}
