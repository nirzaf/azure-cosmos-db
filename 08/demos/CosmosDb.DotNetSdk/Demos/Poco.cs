using Newtonsoft.Json;

namespace CosmosDb.DotNetSdk.Demos
{
	public class Customer
	{
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }	// Must be nullable, unless generating unique values for new customers on client

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "address")]
		public Address Address { get; set; }
	}

	public class Address
	{
		[JsonProperty(PropertyName = "addressType")]
		public string AddressType { get; set; }

		[JsonProperty(PropertyName = "addressLine1")]
		public string AddressLine1 { get; set; }

		[JsonProperty(PropertyName = "location")]
		public Location Location { get; set; }

		[JsonProperty(PropertyName = "postalCode")]
		public string PostalCode { get; set; }

		[JsonProperty(PropertyName = "countryRegionName")]
		public string CountryRegionName { get; set; }
	}

	public class Location
	{
		[JsonProperty(PropertyName = "city")]
		public string City { get; set; }

		[JsonProperty(PropertyName = "stateProvinceName")]
		public string StateProvinceName { get; set; }
	}

}
