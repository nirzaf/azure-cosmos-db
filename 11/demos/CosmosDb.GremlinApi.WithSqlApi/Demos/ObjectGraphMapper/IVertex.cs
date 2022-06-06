namespace CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper
{
	public interface IVertex : IGraphObject
	{
		object GetPartitionKeyValue();
		string GetPartitionKeyName();
	}
}
