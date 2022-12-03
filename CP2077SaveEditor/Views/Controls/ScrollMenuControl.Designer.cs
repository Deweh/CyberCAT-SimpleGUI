namespace CP2077SaveEditor.Views.Controls
{
    partial class ScrollMenuControl
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
            this.btn_ScrollUp = new CP2077SaveEditor.ModernButton();
            this.btn_ScrollDown = new CP2077SaveEditor.ModernButton();
            this.pnl_Menu = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btn_ScrollUp
            // 
            this.btn_ScrollUp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ScrollUp.BackColor = System.Drawing.Color.White;
            this.btn_ScrollUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btn_ScrollUp.ClickEffectEnabled = true;
            this.btn_ScrollUp.DefaultColor = System.Drawing.Color.White;
            this.btn_ScrollUp.HoverColor = System.Drawing.Color.LightGray;
            this.btn_ScrollUp.Location = new System.Drawing.Point(0, 0);
            this.btn_ScrollUp.Name = "btn_ScrollUp";
            this.btn_ScrollUp.Size = new System.Drawing.Size(143, 20);
            this.btn_ScrollUp.TabIndex = 10;
            this.btn_ScrollUp.Text = "^";
            this.btn_ScrollUp.TextColor = System.Drawing.SystemColors.ControlText;
            this.btn_ScrollUp.TextFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_ScrollUp.Click += new System.EventHandler(this.btn_ScrollUp_Click);
            // 
            // btn_ScrollDown
            // 
            this.btn_ScrollDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_ScrollDown.BackColor = System.Drawing.Color.White;
            this.btn_ScrollDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btn_ScrollDown.ClickEffectEnabled = true;
            this.btn_ScrollDown.DefaultColor = System.Drawing.Color.White;
            this.btn_ScrollDown.HoverColor = System.Drawing.Color.LightGray;
            this.btn_ScrollDown.Location = new System.Drawing.Point(0, 380);
            this.btn_ScrollDown.Name = "btn_ScrollDown";
            this.btn_ScrollDown.Size = new System.Drawing.Size(143, 20);
            this.btn_ScrollDown.TabIndex = 11;
            this.btn_ScrollDown.Text = "v";
            this.btn_ScrollDown.TextColor = System.Drawing.SystemColors.ControlText;
            this.btn_ScrollDown.TextFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_ScrollDown.Click += new System.EventHandler(this.btn_ScrollDown_Click);
            // 
            // pnl_Menu
            // 
            this.pnl_Menu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnl_Menu.BackColor = System.Drawing.Color.White;
            this.pnl_Menu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Menu.Location = new System.Drawing.Point(0, 20);
            this.pnl_Menu.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pnl_Menu_MouseWheel);
            this.pnl_Menu.Name = "pnl_Menu";
            this.pnl_Menu.Size = new System.Drawing.Size(143, 360);
            this.pnl_Menu.TabIndex = 12;
            // 
            // ScrollMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnl_Menu);
            this.Controls.Add(this.btn_ScrollDown);
            this.Controls.Add(this.btn_ScrollUp);
            this.Name = "ScrollMenuControl";
            this.Size = new System.Drawing.Size(143, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private ModernButton btn_ScrollUp;
        private ModernButton btn_ScrollDown;
        private System.Windows.Forms.Panel pnl_Menu;
    }
}
