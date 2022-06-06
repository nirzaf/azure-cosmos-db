using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper
{
	public abstract class VertexBase : GraphObjectBase, IVertex
	{
		[JsonIgnore]
		public string PartitionKeyPropertyName { get; private set; }

		public VertexBase(string label, string partitionKeyPropertyName)
			: base(label)
		{
			this.PartitionKeyPropertyName = partitionKeyPropertyName;
		}

		public override string ToGremlin()
		{
			var sb = new StringBuilder($"g.addV('{base.Label}')");

			var propertyInfos = this.GetType().GetProperties();

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

			return sb.ToString();
		}

		public string GetPartitionKeyName()
		{
			var partitionKeyPropertyInfo = this.GetType().GetProperty(this.PartitionKeyPropertyName);
			var jsonPropertyAttribute = partitionKeyPropertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonPropertyAttribute));

			var propertyName =
				jsonPropertyAttribute == null ?
					partitionKeyPropertyInfo.Name :
					jsonPropertyAttribute.NamedArguments[0].TypedValue.Value.ToString();

			return propertyName;
		}

		public object GetPartitionKeyValue()
		{
			var partitionKeyPropertyInfo = this.GetType().GetProperty(this.PartitionKeyPropertyName);
			var jsonPropertyAttribute = partitionKeyPropertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonPropertyAttribute));

			var propertyName =
				jsonPropertyAttribute == null ?
					partitionKeyPropertyInfo.Name :
					jsonPropertyAttribute.NamedArguments[0].TypedValue.Value.ToString();

			var propertyValue = partitionKeyPropertyInfo.GetValue(this);

			return propertyValue;
		}

		public override JObject ToDocument()
		{
			var jo = new JObject
			{
				new JProperty("label", base.Label),
				new JProperty("id", base.Id)
			};

			var partitionKeyPropertyInfo = this.GetType().GetProperty(this.PartitionKeyPropertyName);
			var jsonPropertyAttribute = partitionKeyPropertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonPropertyAttribute));

			var propertyName =
				jsonPropertyAttribute == null ?
					partitionKeyPropertyInfo.Name :
					jsonPropertyAttribute.NamedArguments[0].TypedValue.Value.ToString();

			var propertyValue = partitionKeyPropertyInfo.GetValue(this);

			jo.Add(new JProperty(propertyName, propertyValue));

			var propertyInfos = this.GetType().GetProperties().Where(pi => pi.Name != "Id" && pi.Name != this.PartitionKeyPropertyName);

			foreach (var propertyInfo in propertyInfos)
			{
				var jsonIgnoreAttribute = propertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonIgnoreAttribute));
				if (jsonIgnoreAttribute != null)
				{
					continue;
				}

				jsonPropertyAttribute = propertyInfo.CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(JsonPropertyAttribute));

				propertyName =
					jsonPropertyAttribute == null ?
						propertyInfo.Name :
						jsonPropertyAttribute.NamedArguments[0].TypedValue.Value.ToString();

				propertyValue = propertyInfo.GetValue(this);

				var embeddedPropertyValue = new JArray
				{
					new JObject
					{
						new JProperty("_value", propertyValue),
						new JProperty("id", Guid.NewGuid().ToString())
					}
				};

				jo.Add(new JProperty(propertyName, embeddedPropertyValue));
			}

			return jo;
		}

	}
}
