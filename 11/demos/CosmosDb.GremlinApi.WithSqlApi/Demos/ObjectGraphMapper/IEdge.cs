namespace CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper
{
	public interface IEdge
	{
		IVertex InVertex { get; }
		IVertex OutVertex { get; }
	}
}
