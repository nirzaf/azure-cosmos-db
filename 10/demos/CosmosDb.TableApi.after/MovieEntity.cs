using Microsoft.Azure.CosmosDB.Table;   // replaces Microsoft.WindowsAzure.Storage.Table

namespace CosmosDb.TableApi
{
	public class MovieEntity : TableEntity
	{
		public MovieEntity(string genre, string movieTitle)
		{
			base.PartitionKey = genre;
			base.RowKey = movieTitle;
		}

		public MovieEntity() { }

		public string Genre => base.PartitionKey;

		public string Title => base.RowKey;

		public int Year { get; set; }

		public string Length { get; set; }

		public string Description { get; set; }

	}
}
