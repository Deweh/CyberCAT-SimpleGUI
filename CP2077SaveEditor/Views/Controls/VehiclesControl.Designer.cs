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
            this.SuspendLayout();
            // 
            // vehiclesListView
            // 
            this.vehiclesListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vehiclesListView.CheckBoxes = true;
            this.vehiclesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.vehicleIDHeader});
            this.vehiclesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vehiclesListView.GridLines = true;
            this.vehiclesListView.Location = new System.Drawing.Point(0, 0);
            this.vehiclesListView.MultiSelect = false;
            this.vehiclesListView.Name = "vehiclesListView";
            this.vehiclesListView.Size = new System.Drawing.Size(851, 548);
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
            this.vehicleIDHeader.Width = 820;
            // 
            // VehiclesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vehiclesListView);
            this.Name = "VehiclesControl";
            this.Size = new System.Drawing.Size(851, 548);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView vehiclesListView;
        private System.Windows.Forms.ColumnHeader vehicleIDHeader;
    }
}
