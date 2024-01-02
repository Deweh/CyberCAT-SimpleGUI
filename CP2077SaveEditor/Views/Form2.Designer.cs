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
            swapSaveType = new ModernButton();
            filePathLabel = new System.Windows.Forms.Label();
            openSaveButton = new ModernButton();
            saveChangesButton = new ModernButton();
            sm_Menu = new Controls.ScrollMenuControl();
            pnl_Content = new System.Windows.Forms.Panel();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            tssl_Status = new System.Windows.Forms.ToolStripStatusLabel();
            tssl_Empty = new System.Windows.Forms.ToolStripStatusLabel();
            tssl_Info = new System.Windows.Forms.ToolStripStatusLabel();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // swapSaveType
            // 
            swapSaveType.BackColor = System.Drawing.Color.WhiteSmoke;
            swapSaveType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            swapSaveType.ClickEffectEnabled = true;
            swapSaveType.DefaultColor = System.Drawing.Color.WhiteSmoke;
            swapSaveType.HoverColor = System.Drawing.Color.LightGray;
            swapSaveType.Location = new System.Drawing.Point(12, 66);
            swapSaveType.Name = "swapSaveType";
            swapSaveType.Size = new System.Drawing.Size(143, 15);
            swapSaveType.TabIndex = 4;
            swapSaveType.Text = "Save Type: PC";
            swapSaveType.TextColor = System.Drawing.SystemColors.ControlText;
            swapSaveType.TextFont = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            swapSaveType.Click += swapSaveType_Click;
            // 
            // filePathLabel
            // 
            filePathLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            filePathLabel.Location = new System.Drawing.Point(12, 85);
            filePathLabel.Name = "filePathLabel";
            filePathLabel.Size = new System.Drawing.Size(143, 15);
            filePathLabel.TabIndex = 5;
            filePathLabel.Text = "No save file selected.";
            filePathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // openSaveButton
            // 
            openSaveButton.BackColor = System.Drawing.Color.WhiteSmoke;
            openSaveButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            openSaveButton.ClickEffectEnabled = true;
            openSaveButton.DefaultColor = System.Drawing.Color.WhiteSmoke;
            openSaveButton.Enabled = false;
            openSaveButton.HoverColor = System.Drawing.Color.LightGray;
            openSaveButton.Location = new System.Drawing.Point(12, 12);
            openSaveButton.Name = "openSaveButton";
            openSaveButton.Size = new System.Drawing.Size(143, 55);
            openSaveButton.TabIndex = 3;
            openSaveButton.Text = "Load Save";
            openSaveButton.TextColor = System.Drawing.SystemColors.ControlText;
            openSaveButton.TextFont = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            openSaveButton.Click += openSaveButton_Click;
            // 
            // saveChangesButton
            // 
            saveChangesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            saveChangesButton.BackColor = System.Drawing.Color.WhiteSmoke;
            saveChangesButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            saveChangesButton.ClickEffectEnabled = true;
            saveChangesButton.DefaultColor = System.Drawing.Color.WhiteSmoke;
            saveChangesButton.Enabled = false;
            saveChangesButton.HoverColor = System.Drawing.Color.LightGray;
            saveChangesButton.Location = new System.Drawing.Point(12, 516);
            saveChangesButton.Name = "saveChangesButton";
            saveChangesButton.Size = new System.Drawing.Size(143, 44);
            saveChangesButton.TabIndex = 7;
            saveChangesButton.Text = "Save Changes";
            saveChangesButton.TextColor = System.Drawing.SystemColors.ControlText;
            saveChangesButton.TextFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            saveChangesButton.Click += saveChangesButton_Click;
            // 
            // sm_Menu
            // 
            sm_Menu.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            sm_Menu.Location = new System.Drawing.Point(12, 103);
            sm_Menu.Name = "sm_Menu";
            sm_Menu.Size = new System.Drawing.Size(143, 407);
            sm_Menu.TabIndex = 8;
            // 
            // pnl_Content
            // 
            pnl_Content.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pnl_Content.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnl_Content.Enabled = false;
            pnl_Content.Location = new System.Drawing.Point(161, 12);
            pnl_Content.Name = "pnl_Content";
            pnl_Content.Size = new System.Drawing.Size(851, 548);
            pnl_Content.TabIndex = 9;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tssl_Status, tssl_Empty, tssl_Info });
            statusStrip1.Location = new System.Drawing.Point(0, 572);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1024, 22);
            statusStrip1.TabIndex = 10;
            statusStrip1.Text = "statusStrip1";
            // 
            // tssl_Status
            // 
            tssl_Status.Name = "tssl_Status";
            tssl_Status.Size = new System.Drawing.Size(26, 17);
            tssl_Status.Text = "Idle";
            // 
            // tssl_Empty
            // 
            tssl_Empty.Name = "tssl_Empty";
            tssl_Empty.Size = new System.Drawing.Size(662, 17);
            tssl_Empty.Spring = true;
            // 
            // tssl_Info
            // 
            tssl_Info.Name = "tssl_Info";
            tssl_Info.Size = new System.Drawing.Size(321, 17);
            tssl_Info.Text = "v0.27c // WolvenKit.RED4.Save by the WolvenKit team.";
            // 
            // Form2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1024, 594);
            Controls.Add(statusStrip1);
            Controls.Add(pnl_Content);
            Controls.Add(sm_Menu);
            Controls.Add(saveChangesButton);
            Controls.Add(swapSaveType);
            Controls.Add(filePathLabel);
            Controls.Add(openSaveButton);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "Form2";
            Text = "Cyberpunk 2077 Save Editor (CyberCAT-SimpleGUI)";
            FormClosing += Form2_FormClosing;
            Load += Form2_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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