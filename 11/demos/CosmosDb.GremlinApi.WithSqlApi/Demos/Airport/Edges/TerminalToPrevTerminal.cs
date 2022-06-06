using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices;
using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Edges
{
	public class TerminalToPrevTerminal : EdgeBase
	{
		[JsonProperty(PropertyName = "distanceInMinutes")]
		public int DistanceInMinutes { get; set; }

		public TerminalToPrevTerminal(Terminal terminal, Terminal prevTerminal)
			: base("terminalToPrevTerminal", terminal, prevTerminal)
		{
		}

	}
}
