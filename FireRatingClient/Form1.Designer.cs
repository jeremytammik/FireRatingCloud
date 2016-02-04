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
      this.FOLV_Puertas = new BrightIdeasSoftware.FastObjectListView();
      this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.Refresh = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.FOLV_Puertas)).BeginInit();
      this.SuspendLayout();
      // 
      // FOLV_Puertas
      // 
      this.FOLV_Puertas.Activation = System.Windows.Forms.ItemActivation.TwoClick;
      this.FOLV_Puertas.AllColumns.Add(this.olvColumn2);
      this.FOLV_Puertas.AllColumns.Add(this.olvColumn1);
      this.FOLV_Puertas.AllColumns.Add(this.olvColumn3);
      this.FOLV_Puertas.AllColumns.Add(this.olvColumn4);
      this.FOLV_Puertas.AllColumns.Add(this.olvColumn5);
      this.FOLV_Puertas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.FOLV_Puertas.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
      this.FOLV_Puertas.CellEditUseWholeCell = false;
      this.FOLV_Puertas.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn2,
            this.olvColumn1,
            this.olvColumn3,
            this.olvColumn4,
            this.olvColumn5});
      this.FOLV_Puertas.Cursor = System.Windows.Forms.Cursors.Default;
      this.FOLV_Puertas.SelectedBackColor = System.Drawing.Color.Empty;
      this.FOLV_Puertas.SelectedForeColor = System.Drawing.Color.Empty;
      this.FOLV_Puertas.Location = new System.Drawing.Point(12, 9);
      this.FOLV_Puertas.Name = "FOLV_Puertas";
      this.FOLV_Puertas.ShowGroups = false;
      this.FOLV_Puertas.Size = new System.Drawing.Size(852, 389);
      this.FOLV_Puertas.TabIndex = 0;
      this.FOLV_Puertas.UseCompatibleStateImageBehavior = false;
      this.FOLV_Puertas.UseFilterIndicator = true;
      this.FOLV_Puertas.UseFiltering = true;
      this.FOLV_Puertas.View = System.Windows.Forms.View.Details;
      this.FOLV_Puertas.VirtualMode = true;
      this.FOLV_Puertas.CellEditFinished += new BrightIdeasSoftware.CellEditEventHandler(this.FOLV_Puertas_CellEditFinished);
      // 
      // olvColumn2
      // 
      this.olvColumn2.AspectName = "project_id";
      this.olvColumn2.DisplayIndex = 1;
      this.olvColumn2.IsEditable = false;
      this.olvColumn2.Text = "Project Id";
      this.olvColumn2.Width = 186;
      // 
      // olvColumn1
      // 
      this.olvColumn1.AspectName = "_id";
      this.olvColumn1.DisplayIndex = 0;
      this.olvColumn1.IsEditable = false;
      this.olvColumn1.Text = "Element Id";
      this.olvColumn1.Width = 275;
      // 
      // olvColumn3
      // 
      this.olvColumn3.AspectName = "level";
      this.olvColumn3.IsEditable = false;
      this.olvColumn3.Text = "Level";
      this.olvColumn3.Width = 138;
      // 
      // olvColumn4
      // 
      this.olvColumn4.AspectName = "tag";
      this.olvColumn4.Text = "Tag";
      // 
      // olvColumn5
      // 
      this.olvColumn5.AspectName = "firerating";
      this.olvColumn5.Text = "API FireRating";
      this.olvColumn5.Width = 200;
      // 
      // Refresh
      // 
      this.Refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.Refresh.Location = new System.Drawing.Point(12, 404);
      this.Refresh.Name = "Refresh";
      this.Refresh.Size = new System.Drawing.Size(75, 23);
      this.Refresh.TabIndex = 2;
      this.Refresh.Text = "Refresh";
      this.Refresh.UseVisualStyleBackColor = true;
      this.Refresh.Click += new System.EventHandler(this.Refresh_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(876, 434);
      this.Controls.Add(this.Refresh);
      this.Controls.Add(this.FOLV_Puertas);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.FOLV_Puertas)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView FOLV_Puertas;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private BrightIdeasSoftware.OLVColumn olvColumn4;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        new private System.Windows.Forms.Button Refresh;
    }
}

