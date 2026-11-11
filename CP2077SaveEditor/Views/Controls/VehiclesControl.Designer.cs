namespace CP2077SaveEditor.Views.Controls
{
    partial class VehiclesControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.vehiclesListView = new System.Windows.Forms.ListView();
            this.vehicleIDHeader = new System.Windows.Forms.ColumnHeader();
            this.vehicleNameHeader = new System.Windows.Forms.ColumnHeader();
            this.pnl_Top = new System.Windows.Forms.Panel();
            this.txt_Search = new System.Windows.Forms.TextBox();
            this.btn_UnlockAll = new System.Windows.Forms.Button();
            this.btn_LockAll = new System.Windows.Forms.Button();
            this.pnl_Top.SuspendLayout();
            this.SuspendLayout();
            // 
            // vehiclesListView
            // 
            this.vehiclesListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vehiclesListView.CheckBoxes = true;
            this.vehiclesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.vehicleIDHeader,
            this.vehicleNameHeader});
            this.vehiclesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vehiclesListView.GridLines = true;
            this.vehiclesListView.Location = new System.Drawing.Point(0, 33);
            this.vehiclesListView.Name = "vehiclesListView";
            this.vehiclesListView.Size = new System.Drawing.Size(851, 515);
            this.vehiclesListView.TabIndex = 1;
            this.vehiclesListView.UseCompatibleStateImageBehavior = false;
            this.vehiclesListView.View = System.Windows.Forms.View.Details;
            this.vehiclesListView.VirtualMode = true;
            this.vehiclesListView.DoubleClick += new System.EventHandler(this.vehiclesListView_DoubleClick);
            // 
            // vehicleIDHeader
            // 
            this.vehicleIDHeader.Name = "vehicleIDHeader";
            this.vehicleIDHeader.Text = "ID";
            this.vehicleIDHeader.Width = 480;
            // 
            // vehicleNameHeader
            // 
            this.vehicleNameHeader.Name = "vehicleNameHeader";
            this.vehicleNameHeader.Text = "Name";
            this.vehicleNameHeader.Width = 340;
            // 
            // pnl_Top
            // 
            this.pnl_Top.Controls.Add(this.txt_Search);
            this.pnl_Top.Controls.Add(this.btn_UnlockAll);
            this.pnl_Top.Controls.Add(this.btn_LockAll);
            this.pnl_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_Top.Location = new System.Drawing.Point(0, 0);
            this.pnl_Top.Name = "pnl_Top";
            this.pnl_Top.Size = new System.Drawing.Size(851, 33);
            this.pnl_Top.TabIndex = 0;
            // 
            // txt_Search
            // 
            this.txt_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txt_Search.Location = new System.Drawing.Point(3, 5);
            this.txt_Search.Name = "txt_Search";
            this.txt_Search.PlaceholderText = "Search";
            this.txt_Search.Size = new System.Drawing.Size(631, 23);
            this.txt_Search.TabIndex = 0;
            this.txt_Search.TextChanged += new System.EventHandler(this.txt_Search_TextChanged);
            // 
            // btn_UnlockAll
            // 
            this.btn_UnlockAll.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btn_UnlockAll.Location = new System.Drawing.Point(640, 4);
            this.btn_UnlockAll.Name = "btn_UnlockAll";
            this.btn_UnlockAll.Size = new System.Drawing.Size(100, 25);
            this.btn_UnlockAll.TabIndex = 1;
            this.btn_UnlockAll.Text = "Unlock all";
            this.btn_UnlockAll.UseVisualStyleBackColor = true;
            this.btn_UnlockAll.Click += new System.EventHandler(this.btn_UnlockAll_Click);
            // 
            // btn_LockAll
            // 
            this.btn_LockAll.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btn_LockAll.Location = new System.Drawing.Point(746, 4);
            this.btn_LockAll.Name = "btn_LockAll";
            this.btn_LockAll.Size = new System.Drawing.Size(100, 25);
            this.btn_LockAll.TabIndex = 2;
            this.btn_LockAll.Text = "Lock all";
            this.btn_LockAll.UseVisualStyleBackColor = true;
            this.btn_LockAll.Click += new System.EventHandler(this.btn_LockAll_Click);
            // 
            // VehiclesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vehiclesListView);
            this.Controls.Add(this.pnl_Top);
            this.Name = "VehiclesControl";
            this.Size = new System.Drawing.Size(851, 548);
            this.pnl_Top.ResumeLayout(false);
            this.pnl_Top.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView vehiclesListView;
        private System.Windows.Forms.ColumnHeader vehicleIDHeader;
        private System.Windows.Forms.ColumnHeader vehicleNameHeader;
        private System.Windows.Forms.Panel pnl_Top;
        private System.Windows.Forms.TextBox txt_Search;
        private System.Windows.Forms.Button btn_UnlockAll;
        private System.Windows.Forms.Button btn_LockAll;
    }
}
