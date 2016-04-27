namespace FireRatingClient
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
      this.FOLV_doors = new BrightIdeasSoftware.FastObjectListView();
      this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.Refresh = new System.Windows.Forms.Button();
      this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      ((System.ComponentModel.ISupportInitialize)(this.FOLV_doors)).BeginInit();
      this.SuspendLayout();
      // 
      // FOLV_doors
      // 
      this.FOLV_doors.Activation = System.Windows.Forms.ItemActivation.TwoClick;
      this.FOLV_doors.AllColumns.Add(this.olvColumn2);
      this.FOLV_doors.AllColumns.Add(this.olvColumn1);
      this.FOLV_doors.AllColumns.Add(this.olvColumn3);
      this.FOLV_doors.AllColumns.Add(this.olvColumn4);
      this.FOLV_doors.AllColumns.Add(this.olvColumn5);
      this.FOLV_doors.AllColumns.Add(this.olvColumn6);
      this.FOLV_doors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.FOLV_doors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.FOLV_doors.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
      this.FOLV_doors.CellEditUseWholeCell = false;
      this.FOLV_doors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn2,
            this.olvColumn1,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5,
            this.olvColumn6});
      this.FOLV_doors.Cursor = System.Windows.Forms.Cursors.Default;
      this.FOLV_doors.SelectedBackColor = System.Drawing.Color.Empty;
      this.FOLV_doors.SelectedForeColor = System.Drawing.Color.Empty;
      this.FOLV_doors.Location = new System.Drawing.Point(1, 1);
      this.FOLV_doors.Name = "FOLV_doors";
      this.FOLV_doors.ShowGroups = false;
      this.FOLV_doors.Size = new System.Drawing.Size(580, 300);
      this.FOLV_doors.TabIndex = 0;
      this.FOLV_doors.UseCompatibleStateImageBehavior = false;
      this.FOLV_doors.UseFilterIndicator = true;
      this.FOLV_doors.UseFiltering = true;
      this.FOLV_doors.View = System.Windows.Forms.View.Details;
      this.FOLV_doors.VirtualMode = true;
      this.FOLV_doors.CellEditFinished += new BrightIdeasSoftware.CellEditEventHandler(this.OnDoorsCellEditFinished);
      // 
      // olvColumn2
      // 
      this.olvColumn2.AspectName = "project_id";
      this.olvColumn2.DisplayIndex = 1;
      this.olvColumn2.IsEditable = false;
      this.olvColumn2.Text = "Project Id";
      this.olvColumn2.Width = 100;
      // 
      // olvColumn1
      // 
      this.olvColumn1.AspectName = "_id";
      this.olvColumn1.DisplayIndex = 0;
      this.olvColumn1.IsEditable = false;
      this.olvColumn1.Text = "Element Id";
      this.olvColumn1.Width = 94;
      // 
      // olvColumn3
      // 
      this.olvColumn3.AspectName = "level";
      this.olvColumn3.IsEditable = false;
      this.olvColumn3.Text = "Level";
      this.olvColumn3.Width = 101;
      // 
      // olvColumn4
      // 
      this.olvColumn4.AspectName = "tag";
      this.olvColumn4.IsEditable = false;
      this.olvColumn4.Text = "Tag";
      this.olvColumn4.Width = 80;
      // 
      // olvColumn5
      // 
      this.olvColumn5.AspectName = "firerating";
      this.olvColumn5.Text = "API FireRating";
      this.olvColumn5.Width = 109;
      // 
      // Refresh
      // 
      this.Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.Refresh.Location = new System.Drawing.Point(12, 308);
      this.Refresh.Name = "Refresh";
      this.Refresh.Size = new System.Drawing.Size(75, 23);
      this.Refresh.TabIndex = 2;
      this.Refresh.Text = "Refresh";
      this.Refresh.UseVisualStyleBackColor = true;
      this.Refresh.Click += new System.EventHandler(this.Refresh_Click);
      // 
      // olvColumn6
      // 
      this.olvColumn6.AspectName = "modified";
      this.olvColumn6.IsEditable = false;
      this.olvColumn6.Text = "Modified";
      this.olvColumn6.Width = 114;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(584, 339);
      this.Controls.Add(this.Refresh);
      this.Controls.Add(this.FOLV_doors);
      this.Name = "Form1";
      this.Text = "FireRatingClient";
      ((System.ComponentModel.ISupportInitialize)(this.FOLV_doors)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView FOLV_doors;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        new private System.Windows.Forms.Button Refresh;
    private BrightIdeasSoftware.OLVColumn olvColumn6;
  }
}

