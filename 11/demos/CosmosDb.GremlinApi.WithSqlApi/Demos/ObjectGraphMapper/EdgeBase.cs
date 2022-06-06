using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper
{
	public abstract class EdgeBase : GraphObjectBase, IEdge
	{
		[JsonIgnore]
		public IVertex InVertex { get; }

		[JsonIgnore]
		public IVertex OutVertex { get; }

		public EdgeBase(string label, IVertex inVertex, IVertex outVertex)
			: base(label)
		{
			this.InVertex = inVertex;
			this.OutVertex = outVertex;
		}

		public override string ToGremlin()
		{
			var sb = new StringBuilder($"g.V().has('id', '{this.InVertex.Id}').addE('{base.Label}')");

			var propertyInfos = this.GetType().GetProperties().Where(pi => pi.Name != "Id");

			foreach (var propertyInfo in propertyInfos)
			{
				var jsonIgnoreAttribute = propertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonIgnoreAttribute));
				if (jsonIgnoreAttribute != null)
				{
					continue;
				}

				var jsonPropertyAttribute = propertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonPropertyAttribute));

				var propertyName =
					jsonPropertyAttribute == null ?
						propertyInfo.Name :
						jsonPropertyAttribute.NamedArguments[0].TypedValue.Value;

				var propertyValue = base.FormatGremlinPropertyValue(propertyInfo);

				sb.Append($".property('{propertyName}', {propertyValue})");
			}

			sb.Append($".to(g.V().Has('id', '{this.OutVertex.Id}'))");

			return sb.ToString();
		}

		public override JObject ToDocument()
		{
			var partitionKeyValue = this.OutVertex.GetPartitionKeyValue();
			var partitionKeyName = this.OutVertex.GetPartitionKeyName();

			var jo = new JObject
			{
				new JProperty("label", base.Label),
				new JProperty("id", Guid.NewGuid().ToString()),
				new JProperty(partitionKeyName, partitionKeyValue),
				new JProperty("_sink", this.OutVertex.Id),
				new JProperty("_sinkLabel", this.OutVertex.Label),
				new JProperty("_sinkPartition", partitionKeyValue),
				new JProperty("_vertexId", this.InVertex.Id),
				new JProperty("_vertexLabel", this.InVertex.Label),
				new JProperty("_isEdge", true),
			};

			var propertyInfos = this.GetType().GetProperties().Where(pi => pi.Name != "Id");

			foreach (var propertyInfo in propertyInfos)
			{
				var jsonIgnoreAttribute = propertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonIgnoreAttribute));
				if (jsonIgnoreAttribute != null)
				{
					continue;
				}

				var jsonPropertyAttribute = propertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonPropertyAttribute));

				var propertyName =
					jsonPropertyAttribute == null ?
						propertyInfo.Name :
						jsonPropertyAttribute.NamedArguments[0].TypedValue.Value.ToString();

				var propertyValue = propertyInfo.GetValue(this);

				jo.Add(new JProperty(propertyName, propertyValue));
			}

			return jo;
		}

	}
}
