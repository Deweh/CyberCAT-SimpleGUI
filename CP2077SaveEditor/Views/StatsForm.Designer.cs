namespace CP2077SaveEditor.Views
{
    partial class StatsForm
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
            statsControl1 = new Controls.StatsControl();
            SuspendLayout();
            // 
            // statsControl1
            // 
            statsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            statsControl1.Location = new System.Drawing.Point(0, 0);
            statsControl1.Name = "statsControl1";
            statsControl1.Size = new System.Drawing.Size(456, 603);
            statsControl1.TabIndex = 0;
            // 
            // StatsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(456, 603);
            Controls.Add(statsControl1);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "StatsForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Player";
            ResumeLayout(false);
        }

        #endregion

        private Controls.StatsControl statsControl1;
    }
}