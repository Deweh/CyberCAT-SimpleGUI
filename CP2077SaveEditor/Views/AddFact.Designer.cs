
namespace CP2077SaveEditor
{
    partial class AddFact
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.factEntryBox = new System.Windows.Forms.TextBox();
            this.factValueUpDown = new System.Windows.Forms.NumericUpDown();
            this.factTypeBox = new System.Windows.Forms.ComboBox();
            this.addFactButton = new CP2077SaveEditor.ModernButton();
            ((System.ComponentModel.ISupportInitialize)(this.factValueUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fact:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Value:";
            // 
            // factEntryBox
            // 
            this.factEntryBox.Location = new System.Drawing.Point(56, 15);
            this.factEntryBox.Name = "factEntryBox";
            this.factEntryBox.Size = new System.Drawing.Size(201, 22);
            this.factEntryBox.TabIndex = 2;
            // 
            // factValueUpDown
            // 
            this.factValueUpDown.Location = new System.Drawing.Point(56, 41);
            this.factValueUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.factValueUpDown.Name = "factValueUpDown";
            this.factValueUpDown.Size = new System.Drawing.Size(98, 22);
            this.factValueUpDown.TabIndex = 3;
            // 
            // factTypeBox
            // 
            this.factTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.factTypeBox.FormattingEnabled = true;
            this.factTypeBox.Items.AddRange(new object[] {
            "Name",
            "Hash"});
            this.factTypeBox.Location = new System.Drawing.Point(262, 16);
            this.factTypeBox.Name = "factTypeBox";
            this.factTypeBox.Size = new System.Drawing.Size(78, 21);
            this.factTypeBox.TabIndex = 4;
            // 
            // addFactButton
            // 
            this.addFactButton.BackColor = System.Drawing.Color.White;
            this.addFactButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addFactButton.ClickEffectEnabled = true;
            this.addFactButton.DefaultColor = System.Drawing.Color.White;
            this.addFactButton.HoverColor = System.Drawing.Color.LightGray;
            this.addFactButton.Location = new System.Drawing.Point(255, 65);
            this.addFactButton.Name = "addFactButton";
            this.addFactButton.Size = new System.Drawing.Size(85, 26);
            this.addFactButton.TabIndex = 5;
            this.addFactButton.Text = "Add";
            this.addFactButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.addFactButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addFactButton.Click += new System.EventHandler(this.addFactButton_Click);
            // 
            // AddFact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(358, 103);
            this.Controls.Add(this.addFactButton);
            this.Controls.Add(this.factTypeBox);
            this.Controls.Add(this.factValueUpDown);
            this.Controls.Add(this.factEntryBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddFact";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Fact";
            ((System.ComponentModel.ISupportInitialize)(this.factValueUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox factEntryBox;
        private System.Windows.Forms.NumericUpDown factValueUpDown;
        private System.Windows.Forms.ComboBox factTypeBox;
        private ModernButton addFactButton;
    }
}