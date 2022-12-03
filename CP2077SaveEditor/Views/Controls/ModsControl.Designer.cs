namespace CP2077SaveEditor.Views.Controls
{
    partial class ModsControl
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
            this.gb_WardrobeExtra = new System.Windows.Forms.GroupBox();
            this.btn_ClearBlacklist = new CP2077SaveEditor.ModernButton();
            this.gb_WardrobeExtra.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_WardrobeExtra
            // 
            this.gb_WardrobeExtra.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_WardrobeExtra.Controls.Add(this.btn_ClearBlacklist);
            this.gb_WardrobeExtra.Enabled = false;
            this.gb_WardrobeExtra.Location = new System.Drawing.Point(3, 3);
            this.gb_WardrobeExtra.Name = "gb_WardrobeExtra";
            this.gb_WardrobeExtra.Size = new System.Drawing.Size(845, 55);
            this.gb_WardrobeExtra.TabIndex = 0;
            this.gb_WardrobeExtra.TabStop = false;
            this.gb_WardrobeExtra.Text = "Wardrobe Extra";
            // 
            // btn_ClearBlacklist
            // 
            this.btn_ClearBlacklist.BackColor = System.Drawing.Color.LightGray;
            this.btn_ClearBlacklist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btn_ClearBlacklist.ClickEffectEnabled = true;
            this.btn_ClearBlacklist.DefaultColor = System.Drawing.Color.LightGray;
            this.btn_ClearBlacklist.HoverColor = System.Drawing.Color.Silver;
            this.btn_ClearBlacklist.Location = new System.Drawing.Point(6, 22);
            this.btn_ClearBlacklist.Name = "btn_ClearBlacklist";
            this.btn_ClearBlacklist.Size = new System.Drawing.Size(124, 24);
            this.btn_ClearBlacklist.TabIndex = 59;
            this.btn_ClearBlacklist.Text = "Clear Blacklist";
            this.btn_ClearBlacklist.TextColor = System.Drawing.SystemColors.ControlText;
            this.btn_ClearBlacklist.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_ClearBlacklist.Click += new System.EventHandler(this.btn_ClearBlacklist_Click);
            // 
            // ModsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gb_WardrobeExtra);
            this.Name = "ModsControl";
            this.Size = new System.Drawing.Size(851, 548);
            this.gb_WardrobeExtra.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_WardrobeExtra;
        private ModernButton btn_ClearBlacklist;
    }
}
