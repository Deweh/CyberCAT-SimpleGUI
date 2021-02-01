
namespace CP2077SaveEditor
{
    partial class WrongDefaultDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WrongDefaultDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.errorBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.continueButton = new CP2077SaveEditor.ModernButton();
            this.cancelButton = new CP2077SaveEditor.ModernButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(380, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Your save contains an unexpected value. Please report this issue with the\r\ngenera" +
    "ted error message below.";
            // 
            // errorBox
            // 
            this.errorBox.Location = new System.Drawing.Point(14, 48);
            this.errorBox.Multiline = true;
            this.errorBox.Name = "errorBox";
            this.errorBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorBox.Size = new System.Drawing.Size(380, 77);
            this.errorBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(365, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "You can ignore this message and continue loading, or cancel loading.";
            // 
            // continueButton
            // 
            this.continueButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.continueButton.ClickEffectEnabled = true;
            this.continueButton.DefaultColor = System.Drawing.Color.White;
            this.continueButton.HoverColor = System.Drawing.Color.LightGray;
            this.continueButton.Location = new System.Drawing.Point(286, 164);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(108, 28);
            this.continueButton.TabIndex = 4;
            this.continueButton.Text = "Continue";
            this.continueButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.continueButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cancelButton.ClickEffectEnabled = true;
            this.cancelButton.DefaultColor = System.Drawing.Color.White;
            this.cancelButton.HoverColor = System.Drawing.Color.LightGray;
            this.cancelButton.Location = new System.Drawing.Point(182, 164);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(98, 28);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.cancelButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // WrongDefaultDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(406, 204);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.errorBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WrongDefaultDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warning";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox errorBox;
        private System.Windows.Forms.Label label2;
        private ModernButton continueButton;
        private ModernButton cancelButton;
    }
}