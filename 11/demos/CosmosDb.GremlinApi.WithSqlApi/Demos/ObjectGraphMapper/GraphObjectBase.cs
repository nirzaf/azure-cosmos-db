using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace CosmosDb.GremlinApi.WithSqlApi.Demos.ObjectGraphMapper
{
	public abstract class GraphObjectBase : IGraphObject
	{
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		[JsonIgnore]
		public string Label { get; private set; }

		public abstract string ToGremlin();

		public abstract JObject ToDocument();

		public GraphObjectBase(string label)
		{
			this.Label = label;
		}

		protected string FormatGremlinPropertyValue(PropertyInfo pi)
		{
			var type = pi.PropertyType;
			var value = pi.GetValue(this);

			if (Type.GetTypeCode(type) == TypeCode.Boolean)
			{
				return ((bool)value) ? "true" : "false";
			}

			if (this.IsNumericType(type))
			{
				return value.ToString();
			}

			return $"'{value}'";
		}

		private bool IsNumericType(Type type)
		{
			if (type == null)
			{
				return false;
			}

			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
				case TypeCode.Object:
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						return this.IsNumericType(Nullable.GetUnderlyingType(type));
					}
					return false;
			}
			return false;
		}

	}
}
