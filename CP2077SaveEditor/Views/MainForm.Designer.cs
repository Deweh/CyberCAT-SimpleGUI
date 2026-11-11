using CP2077SaveEditor.Properties;

namespace CP2077SaveEditor.Views
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            filePathLabel = new System.Windows.Forms.Label();
            openSaveButton = new ModernButton();
            cloudButton = new ModernButton();
            saveChangesButton = new ModernButton();
            quickSaveButton = new ModernButton();
            sm_Menu = new CP2077SaveEditor.Views.Controls.ScrollMenuControl();
            pnl_Content = new System.Windows.Forms.Panel();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            tssl_Status = new System.Windows.Forms.ToolStripStatusLabel();
            tssl_Empty = new System.Windows.Forms.ToolStripStatusLabel();
            tssl_Info = new System.Windows.Forms.ToolStripStatusLabel();
            tspb_Busy = new System.Windows.Forms.ToolStripProgressBar();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // filePathLabel
            // 
            filePathLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            filePathLabel.Location = new System.Drawing.Point(12, 74);
            filePathLabel.Name = "filePathLabel";
            filePathLabel.Size = new System.Drawing.Size(176, 18);
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
            openSaveButton.Size = new System.Drawing.Size(176, 54);
            openSaveButton.TabIndex = 3;
            openSaveButton.Text = "Load Save";
            openSaveButton.TextColor = System.Drawing.SystemColors.ControlText;
            openSaveButton.TextFont = new System.Drawing.Font("Segoe UI", 13.5F);
            openSaveButton.Click += openSaveButton_Click;
            //
            // cloudButton
            //
            cloudButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            cloudButton.BackColor = System.Drawing.Color.WhiteSmoke;
            cloudButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            cloudButton.ClickEffectEnabled = true;
            cloudButton.DefaultColor = System.Drawing.Color.WhiteSmoke;
            cloudButton.Enabled = false;
            cloudButton.HoverColor = System.Drawing.Color.LightGray;
            cloudButton.Location = new System.Drawing.Point(12, 434);
            cloudButton.Name = "cloudButton";
            cloudButton.Size = new System.Drawing.Size(176, 36);
            cloudButton.TabIndex = 4;
            cloudButton.Text = "Cloud...";
            cloudButton.TextColor = System.Drawing.SystemColors.ControlText;
            cloudButton.TextFont = new System.Drawing.Font("Segoe UI", 11F);
            cloudButton.Click += cloudButton_Click;
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
            saveChangesButton.Location = new System.Drawing.Point(12, 520);
            saveChangesButton.Name = "saveChangesButton";
            saveChangesButton.Size = new System.Drawing.Size(176, 40);
            saveChangesButton.TabIndex = 7;
            saveChangesButton.Text = "Save As...";
            saveChangesButton.TextColor = System.Drawing.SystemColors.ControlText;
            saveChangesButton.TextFont = new System.Drawing.Font("Segoe UI", 11F);
            saveChangesButton.Click += saveChangesButton_Click;
            // 
            // quickSaveButton
            // 
            quickSaveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            quickSaveButton.BackColor = System.Drawing.Color.WhiteSmoke;
            quickSaveButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            quickSaveButton.ClickEffectEnabled = true;
            quickSaveButton.DefaultColor = System.Drawing.Color.WhiteSmoke;
            quickSaveButton.Enabled = false;
            quickSaveButton.HoverColor = System.Drawing.Color.LightGray;
            quickSaveButton.Location = new System.Drawing.Point(12, 474);
            quickSaveButton.Name = "quickSaveButton";
            quickSaveButton.Size = new System.Drawing.Size(176, 40);
            quickSaveButton.TabIndex = 6;
            quickSaveButton.Text = "Save";
            quickSaveButton.TextColor = System.Drawing.SystemColors.ControlText;
            quickSaveButton.TextFont = new System.Drawing.Font("Segoe UI", 11F);
            quickSaveButton.Click += quickSaveButton_Click;
            // 
            // sm_Menu
            // 
            sm_Menu.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            sm_Menu.Location = new System.Drawing.Point(12, 98);
            sm_Menu.Name = "sm_Menu";
            sm_Menu.Size = new System.Drawing.Size(176, 332);
            sm_Menu.TabIndex = 8;
            // 
            // pnl_Content
            // 
            pnl_Content.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pnl_Content.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnl_Content.Enabled = false;
            pnl_Content.Location = new System.Drawing.Point(196, 12);
            pnl_Content.Name = "pnl_Content";
            pnl_Content.Size = new System.Drawing.Size(856, 548);
            pnl_Content.TabIndex = 9;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tssl_Status, tspb_Busy, tssl_Empty, tssl_Info });
            statusStrip1.Location = new System.Drawing.Point(0, 572);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1064, 22);
            statusStrip1.TabIndex = 10;
            statusStrip1.Text = "statusStrip1";
            // 
            // tssl_Status
            // 
            tssl_Status.Name = "tssl_Status";
            tssl_Status.Size = new System.Drawing.Size(26, 17);
            tssl_Status.Text = "Idle";
            // 
            // tspb_Busy
            // 
            tspb_Busy.MarqueeAnimationSpeed = 30;
            tspb_Busy.Name = "tspb_Busy";
            tspb_Busy.Size = new System.Drawing.Size(100, 16);
            tspb_Busy.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            tspb_Busy.Visible = false;
            // 
            // tssl_Empty
            // 
            tssl_Empty.Name = "tssl_Empty";
            tssl_Empty.Size = new System.Drawing.Size(951, 17);
            tssl_Empty.Spring = true;
            // 
            // tssl_Info
            // 
            tssl_Info.Name = "tssl_Info";
            tssl_Info.Size = new System.Drawing.Size(41, 17);
            tssl_Info.Text = "v0.28c";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1064, 594);
            MinimumSize = new System.Drawing.Size(960, 580);
            Controls.Add(statusStrip1);
            Controls.Add(pnl_Content);
            Controls.Add(sm_Menu);
            Controls.Add(quickSaveButton);
            Controls.Add(saveChangesButton);
            Controls.Add(filePathLabel);
            Controls.Add(openSaveButton);
            Controls.Add(cloudButton);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "MainForm";
            Text = "CyberCAT Reloaded by bernardomcma";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            KeyDown += MainForm_KeyDown;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label filePathLabel;
        private ModernButton openSaveButton;
        private ModernButton cloudButton;
        private ModernButton saveChangesButton;
        private ModernButton quickSaveButton;
        private Controls.ScrollMenuControl sm_Menu;
        private System.Windows.Forms.Panel pnl_Content;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssl_Status;
        private System.Windows.Forms.ToolStripStatusLabel tssl_Empty;
        private System.Windows.Forms.ToolStripStatusLabel tssl_Info;
        private System.Windows.Forms.ToolStripProgressBar tspb_Busy;
    }
}
