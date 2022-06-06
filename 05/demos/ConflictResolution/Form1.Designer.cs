namespace ConflictResolution
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.GetConflictsButton = new System.Windows.Forms.Button();
			this.ConflictsListBox = new System.Windows.Forms.ListBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.WinnerTextBox = new System.Windows.Forms.TextBox();
			this.LoserTextBox = new System.Windows.Forms.TextBox();
			this.KeepWinnerButton = new System.Windows.Forms.Button();
			this.KeepLoserButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// GetConflictsButton
			// 
			this.GetConflictsButton.Dock = System.Windows.Forms.DockStyle.Top;
			this.GetConflictsButton.Location = new System.Drawing.Point(0, 0);
			this.GetConflictsButton.Name = "GetConflictsButton";
			this.GetConflictsButton.Size = new System.Drawing.Size(253, 23);
			this.GetConflictsButton.TabIndex = 0;
			this.GetConflictsButton.Text = "Get Conflicts";
			this.GetConflictsButton.UseVisualStyleBackColor = true;
			this.GetConflictsButton.Click += new System.EventHandler(this.GetConflictsButton_Click);
			// 
			// ConflictsListBox
			// 
			this.ConflictsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ConflictsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConflictsListBox.FormattingEnabled = true;
			this.ConflictsListBox.Location = new System.Drawing.Point(0, 23);
			this.ConflictsListBox.Name = "ConflictsListBox";
			this.ConflictsListBox.Size = new System.Drawing.Size(253, 325);
			this.ConflictsListBox.TabIndex = 1;
			this.ConflictsListBox.SelectedIndexChanged += new System.EventHandler(this.ConflictsListBox_SelectedIndexChanged);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(4, 4);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
			this.splitContainer1.Panel1.Controls.Add(this.ConflictsListBox);
			this.splitContainer1.Panel1.Controls.Add(this.GetConflictsButton);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(792, 348);
			this.splitContainer1.SplitterDistance = 253;
			this.splitContainer1.TabIndex = 2;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.WinnerTextBox);
			this.splitContainer2.Panel1.Controls.Add(this.KeepWinnerButton);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.LoserTextBox);
			this.splitContainer2.Panel2.Controls.Add(this.KeepLoserButton);
			this.splitContainer2.Size = new System.Drawing.Size(535, 348);
			this.splitContainer2.SplitterDistance = 267;
			this.splitContainer2.TabIndex = 0;
			// 
			// WinnerTextBox
			// 
			this.WinnerTextBox.BackColor = System.Drawing.Color.White;
			this.WinnerTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WinnerTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WinnerTextBox.ForeColor = System.Drawing.Color.Green;
			this.WinnerTextBox.Location = new System.Drawing.Point(0, 23);
			this.WinnerTextBox.Multiline = true;
			this.WinnerTextBox.Name = "WinnerTextBox";
			this.WinnerTextBox.ReadOnly = true;
			this.WinnerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.WinnerTextBox.Size = new System.Drawing.Size(267, 325);
			this.WinnerTextBox.TabIndex = 0;
			this.WinnerTextBox.WordWrap = false;
			// 
			// LoserTextBox
			// 
			this.LoserTextBox.BackColor = System.Drawing.Color.White;
			this.LoserTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LoserTextBox.Font = new System.Drawing.Font("Consolas", 9.75F);
			this.LoserTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.LoserTextBox.Location = new System.Drawing.Point(0, 23);
			this.LoserTextBox.Multiline = true;
			this.LoserTextBox.Name = "LoserTextBox";
			this.LoserTextBox.ReadOnly = true;
			this.LoserTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.LoserTextBox.Size = new System.Drawing.Size(264, 325);
			this.LoserTextBox.TabIndex = 1;
			this.LoserTextBox.WordWrap = false;
			// 
			// KeepWinnerButton
			// 
			this.KeepWinnerButton.Dock = System.Windows.Forms.DockStyle.Top;
			this.KeepWinnerButton.Location = new System.Drawing.Point(0, 0);
			this.KeepWinnerButton.Name = "KeepWinnerButton";
			this.KeepWinnerButton.Size = new System.Drawing.Size(267, 23);
			this.KeepWinnerButton.TabIndex = 1;
			this.KeepWinnerButton.Text = "Keep Winner";
			this.KeepWinnerButton.UseVisualStyleBackColor = true;
			this.KeepWinnerButton.Click += new System.EventHandler(this.KeepWinnerButton_Click);
			// 
			// KeepLoserButton
			// 
			this.KeepLoserButton.Dock = System.Windows.Forms.DockStyle.Top;
			this.KeepLoserButton.Location = new System.Drawing.Point(0, 0);
			this.KeepLoserButton.Name = "KeepLoserButton";
			this.KeepLoserButton.Size = new System.Drawing.Size(264, 23);
			this.KeepLoserButton.TabIndex = 2;
			this.KeepLoserButton.Text = "Keep Loser";
			this.KeepLoserButton.UseVisualStyleBackColor = true;
			this.KeepLoserButton.Click += new System.EventHandler(this.KeepLoserButton_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 356);
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "Form1";
			this.Padding = new System.Windows.Forms.Padding(4);
			this.Text = "Cosmos DB Multi-Master Conflict Resolution";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button GetConflictsButton;
		private System.Windows.Forms.ListBox ConflictsListBox;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.TextBox WinnerTextBox;
		private System.Windows.Forms.TextBox LoserTextBox;
		private System.Windows.Forms.Button KeepWinnerButton;
		private System.Windows.Forms.Button KeepLoserButton;
	}
}

