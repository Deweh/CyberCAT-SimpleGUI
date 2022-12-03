namespace CP2077SaveEditor.Views.Controls
{
    partial class AppearanceControl
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
            this.advancedAppearanceButton = new CP2077SaveEditor.ModernButton();
            this.saveAppearButton = new CP2077SaveEditor.ModernButton();
            this.loadAppearButton = new CP2077SaveEditor.ModernButton();
            this.appearancePreviewBox = new System.Windows.Forms.PictureBox();
            this.appearanceOptionsPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.appearancePreviewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // advancedAppearanceButton
            // 
            this.advancedAppearanceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedAppearanceButton.BackColor = System.Drawing.Color.White;
            this.advancedAppearanceButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.advancedAppearanceButton.ClickEffectEnabled = true;
            this.advancedAppearanceButton.DefaultColor = System.Drawing.Color.White;
            this.advancedAppearanceButton.HoverColor = System.Drawing.Color.LightGray;
            this.advancedAppearanceButton.Location = new System.Drawing.Point(754, -1);
            this.advancedAppearanceButton.Name = "advancedAppearanceButton";
            this.advancedAppearanceButton.Size = new System.Drawing.Size(98, 18);
            this.advancedAppearanceButton.TabIndex = 13;
            this.advancedAppearanceButton.Text = "Advanced...";
            this.advancedAppearanceButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.advancedAppearanceButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.advancedAppearanceButton.Click += new System.EventHandler(this.advancedAppearanceButton_Click);
            // 
            // saveAppearButton
            // 
            this.saveAppearButton.BackColor = System.Drawing.Color.White;
            this.saveAppearButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.saveAppearButton.ClickEffectEnabled = true;
            this.saveAppearButton.DefaultColor = System.Drawing.Color.White;
            this.saveAppearButton.HoverColor = System.Drawing.Color.LightGray;
            this.saveAppearButton.Location = new System.Drawing.Point(96, -1);
            this.saveAppearButton.Name = "saveAppearButton";
            this.saveAppearButton.Size = new System.Drawing.Size(98, 18);
            this.saveAppearButton.TabIndex = 11;
            this.saveAppearButton.Text = "Save Preset";
            this.saveAppearButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.saveAppearButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.saveAppearButton.Click += new System.EventHandler(this.saveAppearButton_Click);
            // 
            // loadAppearButton
            // 
            this.loadAppearButton.BackColor = System.Drawing.Color.White;
            this.loadAppearButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadAppearButton.ClickEffectEnabled = true;
            this.loadAppearButton.DefaultColor = System.Drawing.Color.White;
            this.loadAppearButton.HoverColor = System.Drawing.Color.LightGray;
            this.loadAppearButton.Location = new System.Drawing.Point(-1, -1);
            this.loadAppearButton.Name = "loadAppearButton";
            this.loadAppearButton.Size = new System.Drawing.Size(98, 18);
            this.loadAppearButton.TabIndex = 14;
            this.loadAppearButton.Text = "Load Preset";
            this.loadAppearButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.loadAppearButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.loadAppearButton.Click += new System.EventHandler(this.loadAppearButton_Click);
            // 
            // appearancePreviewBox
            // 
            this.appearancePreviewBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appearancePreviewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.appearancePreviewBox.Location = new System.Drawing.Point(31, 38);
            this.appearancePreviewBox.Name = "appearancePreviewBox";
            this.appearancePreviewBox.Size = new System.Drawing.Size(456, 473);
            this.appearancePreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.appearancePreviewBox.TabIndex = 20;
            this.appearancePreviewBox.TabStop = false;
            // 
            // appearanceOptionsPanel
            // 
            this.appearanceOptionsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appearanceOptionsPanel.AutoScroll = true;
            this.appearanceOptionsPanel.Location = new System.Drawing.Point(533, 38);
            this.appearanceOptionsPanel.Name = "appearanceOptionsPanel";
            this.appearanceOptionsPanel.Size = new System.Drawing.Size(295, 473);
            this.appearanceOptionsPanel.TabIndex = 19;
            // 
            // AppearanceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.appearancePreviewBox);
            this.Controls.Add(this.appearanceOptionsPanel);
            this.Controls.Add(this.loadAppearButton);
            this.Controls.Add(this.saveAppearButton);
            this.Controls.Add(this.advancedAppearanceButton);
            this.Name = "AppearanceControl";
            this.Size = new System.Drawing.Size(851, 548);
            ((System.ComponentModel.ISupportInitialize)(this.appearancePreviewBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ModernButton advancedAppearanceButton;
        private ModernButton saveAppearButton;
        private ModernButton loadAppearButton;
        private System.Windows.Forms.PictureBox appearancePreviewBox;
        private System.Windows.Forms.Panel appearanceOptionsPanel;
    }
}
