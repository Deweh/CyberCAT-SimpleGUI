using CP2077SaveEditor.Properties;

namespace CP2077SaveEditor.Views
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.swapSaveType = new CP2077SaveEditor.ModernButton();
            this.filePathLabel = new System.Windows.Forms.Label();
            this.openSaveButton = new CP2077SaveEditor.ModernButton();
            this.saveChangesButton = new CP2077SaveEditor.ModernButton();
            this.sm_Menu = new CP2077SaveEditor.Views.Controls.ScrollMenuControl();
            this.pnl_Content = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssl_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_Empty = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_Info = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // swapSaveType
            // 
            this.swapSaveType.BackColor = System.Drawing.Color.WhiteSmoke;
            this.swapSaveType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.swapSaveType.ClickEffectEnabled = true;
            this.swapSaveType.DefaultColor = System.Drawing.Color.WhiteSmoke;
            this.swapSaveType.HoverColor = System.Drawing.Color.LightGray;
            this.swapSaveType.Location = new System.Drawing.Point(12, 66);
            this.swapSaveType.Name = "swapSaveType";
            this.swapSaveType.Size = new System.Drawing.Size(143, 15);
            this.swapSaveType.TabIndex = 4;
            this.swapSaveType.Text = "Save Type: PC";
            this.swapSaveType.TextColor = System.Drawing.SystemColors.ControlText;
            this.swapSaveType.TextFont = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.swapSaveType.Click += new System.EventHandler(this.swapSaveType_Click);
            // 
            // filePathLabel
            // 
            this.filePathLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.filePathLabel.Location = new System.Drawing.Point(12, 85);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(143, 15);
            this.filePathLabel.TabIndex = 5;
            this.filePathLabel.Text = "No save file selected.";
            this.filePathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // openSaveButton
            // 
            this.openSaveButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.openSaveButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.openSaveButton.ClickEffectEnabled = true;
            this.openSaveButton.DefaultColor = System.Drawing.Color.WhiteSmoke;
            this.openSaveButton.Enabled = false;
            this.openSaveButton.HoverColor = System.Drawing.Color.LightGray;
            this.openSaveButton.Location = new System.Drawing.Point(12, 12);
            this.openSaveButton.Name = "openSaveButton";
            this.openSaveButton.Size = new System.Drawing.Size(143, 55);
            this.openSaveButton.TabIndex = 3;
            this.openSaveButton.Text = "Load Save";
            this.openSaveButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.openSaveButton.TextFont = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.openSaveButton.Click += new System.EventHandler(this.openSaveButton_Click);
            // 
            // saveChangesButton
            // 
            this.saveChangesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveChangesButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.saveChangesButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.saveChangesButton.ClickEffectEnabled = true;
            this.saveChangesButton.DefaultColor = System.Drawing.Color.WhiteSmoke;
            this.saveChangesButton.Enabled = false;
            this.saveChangesButton.HoverColor = System.Drawing.Color.LightGray;
            this.saveChangesButton.Location = new System.Drawing.Point(12, 516);
            this.saveChangesButton.Name = "saveChangesButton";
            this.saveChangesButton.Size = new System.Drawing.Size(143, 44);
            this.saveChangesButton.TabIndex = 7;
            this.saveChangesButton.Text = "Save Changes";
            this.saveChangesButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.saveChangesButton.TextFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.saveChangesButton.Click += new System.EventHandler(this.saveChangesButton_Click);
            // 
            // sm_Menu
            // 
            this.sm_Menu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.sm_Menu.Location = new System.Drawing.Point(12, 103);
            this.sm_Menu.Name = "sm_Menu";
            this.sm_Menu.Size = new System.Drawing.Size(143, 407);
            this.sm_Menu.TabIndex = 8;
            // 
            // pnl_Content
            // 
            this.pnl_Content.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl_Content.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Content.Enabled = false;
            this.pnl_Content.Location = new System.Drawing.Point(161, 12);
            this.pnl_Content.Name = "pnl_Content";
            this.pnl_Content.Size = new System.Drawing.Size(851, 548);
            this.pnl_Content.TabIndex = 9;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssl_Status,
            this.tssl_Empty,
            this.tssl_Info});
            this.statusStrip1.Location = new System.Drawing.Point(0, 572);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1024, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssl_Status
            // 
            this.tssl_Status.Name = "tssl_Status";
            this.tssl_Status.Size = new System.Drawing.Size(26, 17);
            this.tssl_Status.Text = "Idle";
            // 
            // tssl_Empty
            // 
            this.tssl_Empty.Name = "tssl_Empty";
            this.tssl_Empty.Size = new System.Drawing.Size(658, 17);
            this.tssl_Empty.Spring = true;
            // 
            // tssl_Info
            // 
            this.tssl_Info.Name = "tssl_Info";
            this.tssl_Info.Size = new System.Drawing.Size(294, 17);
            this.tssl_Info.Text = "v0.24c // WolvenKit.RED4.Save by the WolvenKit team.";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 594);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pnl_Content);
            this.Controls.Add(this.sm_Menu);
            this.Controls.Add(this.saveChangesButton);
            this.Controls.Add(this.swapSaveType);
            this.Controls.Add(this.filePathLabel);
            this.Controls.Add(this.openSaveButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.Text = "Cyberpunk 2077 Save Editor (CyberCAT-SimpleGUI)";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ModernButton swapSaveType;
        private System.Windows.Forms.Label filePathLabel;
        private ModernButton openSaveButton;
        private ModernButton saveChangesButton;
        private Controls.ScrollMenuControl sm_Menu;
        private System.Windows.Forms.Panel pnl_Content;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssl_Status;
        private System.Windows.Forms.ToolStripStatusLabel tssl_Empty;
        private System.Windows.Forms.ToolStripStatusLabel tssl_Info;
    }
}