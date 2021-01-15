
namespace CP2077SaveEditor
{
    partial class AddStat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddStat));
            this.modifierObjectBox = new System.Windows.Forms.ComboBox();
            this.addButton = new CP2077SaveEditor.ModernButton();
            this.SuspendLayout();
            // 
            // modifierObjectBox
            // 
            this.modifierObjectBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modifierObjectBox.FormattingEnabled = true;
            this.modifierObjectBox.Items.AddRange(new object[] {
            "GameConstantStatModifierData",
            "GameCombinedStatModifierData",
            "GameCurveStatModifierData"});
            this.modifierObjectBox.Location = new System.Drawing.Point(21, 21);
            this.modifierObjectBox.Name = "modifierObjectBox";
            this.modifierObjectBox.Size = new System.Drawing.Size(205, 21);
            this.modifierObjectBox.TabIndex = 0;
            // 
            // addButton
            // 
            this.addButton.BackColor = System.Drawing.Color.White;
            this.addButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addButton.ClickEffectEnabled = true;
            this.addButton.DefaultColor = System.Drawing.Color.White;
            this.addButton.HoverColor = System.Drawing.Color.LightGray;
            this.addButton.Location = new System.Drawing.Point(155, 62);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(71, 21);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.addButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // AddStat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(249, 95);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.modifierObjectBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddStat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Stat";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox modifierObjectBox;
        private ModernButton addButton;
    }
}