using CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Vertices;
using CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper;
using Newtonsoft.Json;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.Airport.Edges
{
	public class TerminalToNextTerminal : EdgeBase
	{
		[JsonProperty(PropertyName = "distanceInMinutes")]
		public int DistanceInMinutes { get; set; }

		public TerminalToNextTerminal(Terminal terminal, Terminal nextTerminal)
			: base("terminalToNextTerminal", terminal, nextTerminal)
		{
		}

	}
}
