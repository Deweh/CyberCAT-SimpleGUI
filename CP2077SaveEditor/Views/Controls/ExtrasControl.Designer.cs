namespace CP2077SaveEditor.Views.Controls
{
    partial class ExtrasControl
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
            gb_WardrobeExtra = new System.Windows.Forms.GroupBox();
            btn_ClearBlacklist = new ModernButton();
            btn_UnlockAll = new ModernButton();
            gb_Player = new System.Windows.Forms.GroupBox();
            btn_MakeVulnerable = new ModernButton();
            gb_WardrobeExtra.SuspendLayout();
            gb_Player.SuspendLayout();
            SuspendLayout();
            // 
            // gb_WardrobeExtra
            // 
            gb_WardrobeExtra.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            gb_WardrobeExtra.Controls.Add(btn_ClearBlacklist);
            gb_WardrobeExtra.Enabled = false;
            gb_WardrobeExtra.Location = new System.Drawing.Point(3, 64);
            gb_WardrobeExtra.Name = "gb_WardrobeExtra";
            gb_WardrobeExtra.Size = new System.Drawing.Size(845, 55);
            gb_WardrobeExtra.TabIndex = 0;
            gb_WardrobeExtra.TabStop = false;
            gb_WardrobeExtra.Text = "Wardrobe Extra";
            // 
            // btn_ClearBlacklist
            // 
            btn_ClearBlacklist.BackColor = System.Drawing.Color.LightGray;
            btn_ClearBlacklist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ClearBlacklist.ClickEffectEnabled = true;
            btn_ClearBlacklist.DefaultColor = System.Drawing.Color.LightGray;
            btn_ClearBlacklist.HoverColor = System.Drawing.Color.Silver;
            btn_ClearBlacklist.Location = new System.Drawing.Point(6, 22);
            btn_ClearBlacklist.Name = "btn_ClearBlacklist";
            btn_ClearBlacklist.Size = new System.Drawing.Size(124, 24);
            btn_ClearBlacklist.TabIndex = 59;
            btn_ClearBlacklist.Text = "Clear Blacklist";
            btn_ClearBlacklist.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ClearBlacklist.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btn_ClearBlacklist.Click += btn_ClearBlacklist_Click;
            // 
            // btn_UnlockAll
            // 
            btn_UnlockAll.BackColor = System.Drawing.Color.LightGray;
            btn_UnlockAll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_UnlockAll.ClickEffectEnabled = true;
            btn_UnlockAll.DefaultColor = System.Drawing.Color.LightGray;
            btn_UnlockAll.HoverColor = System.Drawing.Color.Silver;
            btn_UnlockAll.Location = new System.Drawing.Point(6, 22);
            btn_UnlockAll.Name = "btn_UnlockAll";
            btn_UnlockAll.Size = new System.Drawing.Size(124, 24);
            btn_UnlockAll.TabIndex = 60;
            btn_UnlockAll.Text = "Unlock Fast Travel";
            btn_UnlockAll.TextColor = System.Drawing.SystemColors.ControlText;
            btn_UnlockAll.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btn_UnlockAll.Click += btn_UnlockAll_Click;
            // 
            // gb_Player
            // 
            gb_Player.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            gb_Player.Controls.Add(btn_UnlockAll);
            gb_Player.Controls.Add(btn_MakeVulnerable);
            gb_Player.Enabled = false;
            gb_Player.Location = new System.Drawing.Point(3, 3);
            gb_Player.Name = "gb_Player";
            gb_Player.Size = new System.Drawing.Size(845, 55);
            gb_Player.TabIndex = 61;
            gb_Player.TabStop = false;
            gb_Player.Text = "Player";
            // 
            // btn_MakeVulnerable
            // 
            btn_MakeVulnerable.BackColor = System.Drawing.Color.LightGray;
            btn_MakeVulnerable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_MakeVulnerable.ClickEffectEnabled = true;
            btn_MakeVulnerable.DefaultColor = System.Drawing.Color.LightGray;
            btn_MakeVulnerable.HoverColor = System.Drawing.Color.Silver;
            btn_MakeVulnerable.Location = new System.Drawing.Point(136, 22);
            btn_MakeVulnerable.Name = "btn_MakeVulnerable";
            btn_MakeVulnerable.Size = new System.Drawing.Size(124, 24);
            btn_MakeVulnerable.TabIndex = 60;
            btn_MakeVulnerable.Text = "Make Vulnerable";
            btn_MakeVulnerable.TextColor = System.Drawing.SystemColors.ControlText;
            btn_MakeVulnerable.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btn_MakeVulnerable.Click += btn_MakeVulnerable_Click;
            // 
            // ExtrasControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(gb_Player);
            Controls.Add(gb_WardrobeExtra);
            Name = "ExtrasControl";
            Size = new System.Drawing.Size(851, 548);
            gb_WardrobeExtra.ResumeLayout(false);
            gb_Player.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox gb_WardrobeExtra;
        private ModernButton btn_ClearBlacklist;
        private ModernButton btn_UnlockAll;
        private System.Windows.Forms.GroupBox gb_Player;
        private ModernButton btn_MakeVulnerable;
    }
}
