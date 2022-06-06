using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConflictResolution
{
	public partial class Form1 : Form
	{
		private Container _container;
		private List<ConflictProperties> _conflicts;
		private ConflictProperties _conflict;
		private string _partitionKey;
		private string _id;

		private dynamic _loser;

		public Form1()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var endpoint = ConfigurationManager.AppSettings["CosmosEndpoint"];
			var masterKey = ConfigurationManager.AppSettings["CosmosMasterKey"];

			var client = new CosmosClient(endpoint, masterKey);
			this._container = client.GetContainer("Families", "Families");
		}

		private async void GetConflictsButton_Click(object sender, EventArgs e)
		{
			this.Enabled = false;
			this.ConflictsListBox.Items.Clear();
			this._conflicts = new List<ConflictProperties>();
			this._conflict = null;

#warning Need to test this next line of code after SDK3 GA
			var iterator = this._container.Conflicts.GetConflictQueryIterator<dynamic>();
			if (iterator.HasMoreResults)
			{
				var conflicts = await iterator.ReadNextAsync();
				foreach (var conflict in conflicts)
				{
					var item = $"{conflict.Id} ({conflict.OperationKind})";
					this.ConflictsListBox.Items.Add(item);
					this._conflicts.Add(conflict);
				}
			}
			this.Enabled = true;
		}

		private async void ConflictsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.ConflictsListBox.SelectedIndex == -1)
			{
				return;
			}

			this._conflict = this._conflicts[this.ConflictsListBox.SelectedIndex];

			this._loser = this._container.Conflicts.ReadConflictContent<object>(this._conflict);

			this._id = this._loser.id;
			this._partitionKey = this._loser.address.zipCode;

			var winnerResponse = await this._container.ReadItemAsync<object>(this._id, new PartitionKey(this._partitionKey));
			dynamic winner = winnerResponse.Resource;

			var loserJson = JsonConvert.SerializeObject(this._loser, Formatting.Indented);
			var winnerJson = JsonConvert.SerializeObject(winner, Formatting.Indented);

			this.LoserTextBox.Text = loserJson;
			this.WinnerTextBox.Text = winnerJson;
		}

		private async void KeepWinnerButton_Click(object sender, EventArgs e)
		{
			if (this._conflict == null)
			{
				MessageBox.Show("No conflict selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			await this.RemoveConflict();

			MessageBox.Show("Conflict Resolved", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private async void KeepLoserButton_Click(object sender, EventArgs e)
		{
			if (this._conflict == null)
			{
				MessageBox.Show("No conflict selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			this._container.ReplaceItemAsync(this._loser, this._id, new PartitionKey(this._partitionKey));

			await this.RemoveConflict();

			MessageBox.Show("Conflict Reversed", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private async Task RemoveConflict()
		{
			await this._container.Conflicts.DeleteAsync(this._conflict, new PartitionKey(this._partitionKey));

			this._conflict = null;
			this._conflicts.RemoveAt(this.ConflictsListBox.SelectedIndex);
			this.ConflictsListBox.Items.RemoveAt(this.ConflictsListBox.SelectedIndex);
			this.ConflictsListBox.SelectedIndex = -1;
			this.LoserTextBox.Text = string.Empty;
			this.WinnerTextBox.Text = string.Empty;
		}

	}
}
