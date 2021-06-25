
namespace CP2077SaveEditor
{
    partial class AdvancedAppearanceDialog
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
            this.optionsBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.firstBox = new System.Windows.Forms.TextBox();
            this.secondBox = new System.Windows.Forms.TextBox();
            this.pathBox = new System.Windows.Forms.TextBox();
            this.applyButton = new CP2077SaveEditor.ModernButton();
            this.SuspendLayout();
            // 
            // optionsBox
            // 
            this.optionsBox.FormattingEnabled = true;
            this.optionsBox.Location = new System.Drawing.Point(12, 12);
            this.optionsBox.Name = "optionsBox";
            this.optionsBox.Size = new System.Drawing.Size(471, 264);
            this.optionsBox.TabIndex = 0;
            this.optionsBox.SelectedIndexChanged += new System.EventHandler(this.optionsBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(540, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data String:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(495, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Identification String:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(575, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Path:";
            // 
            // firstBox
            // 
            this.firstBox.Location = new System.Drawing.Point(614, 37);
            this.firstBox.Name = "firstBox";
            this.firstBox.Size = new System.Drawing.Size(431, 22);
            this.firstBox.TabIndex = 4;
            // 
            // secondBox
            // 
            this.secondBox.Location = new System.Drawing.Point(614, 9);
            this.secondBox.Name = "secondBox";
            this.secondBox.Size = new System.Drawing.Size(431, 22);
            this.secondBox.TabIndex = 5;
            // 
            // pathBox
            // 
            this.pathBox.Location = new System.Drawing.Point(614, 65);
            this.pathBox.Multiline = true;
            this.pathBox.Name = "pathBox";
            this.pathBox.Size = new System.Drawing.Size(431, 50);
            this.pathBox.TabIndex = 6;
            // 
            // applyButton
            // 
            this.applyButton.BackColor = System.Drawing.Color.White;
            this.applyButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.applyButton.ClickEffectEnabled = true;
            this.applyButton.DefaultColor = System.Drawing.Color.White;
            this.applyButton.Enabled = false;
            this.applyButton.HoverColor = System.Drawing.Color.LightGray;
            this.applyButton.Location = new System.Drawing.Point(925, 247);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(120, 29);
            this.applyButton.TabIndex = 7;
            this.applyButton.Text = "Apply";
            this.applyButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.applyButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // AdvancedAppearanceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1061, 291);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.pathBox);
            this.Controls.Add(this.secondBox);
            this.Controls.Add(this.firstBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.optionsBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AdvancedAppearanceDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Appearance";
            this.Load += new System.EventHandler(this.AdvancedAppearanceDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox optionsBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox firstBox;
        private System.Windows.Forms.TextBox secondBox;
        private System.Windows.Forms.TextBox pathBox;
        private ModernButton applyButton;
    }
}