using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices;
using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Edges
{
	public class GateToPrevGate : EdgeBase
	{
		[JsonProperty(PropertyName = "distanceInMinutes")]
		public int DistanceInMinutes { get; set; }

		public GateToPrevGate(Gate gate, Gate prevGate)
			: base("gateToPrevGate", gate, prevGate)
		{
		}

	}
}
