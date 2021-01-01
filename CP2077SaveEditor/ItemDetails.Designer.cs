
namespace CP2077SaveEditor
{
    partial class ItemDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemDetails));
            this.basicInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.quantityUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.modInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.modsTreeView = new System.Windows.Forms.TreeView();
            this.modsBaseIdBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pasteLegendaryIdButton = new CP2077SaveEditor.ModernButton();
            this.applyButton = new CP2077SaveEditor.ModernButton();
            this.closeButton = new CP2077SaveEditor.ModernButton();
            this.quickActionsGroupBox = new System.Windows.Forms.GroupBox();
            this.questItemCheckBox = new System.Windows.Forms.CheckBox();
            this.flagsGroupBox = new System.Windows.Forms.GroupBox();
            this.unknownFlag1CheckBox = new System.Windows.Forms.CheckBox();
            this.basicInfoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.quantityUpDown)).BeginInit();
            this.modInfoGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.quickActionsGroupBox.SuspendLayout();
            this.flagsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // basicInfoGroupBox
            // 
            this.basicInfoGroupBox.Controls.Add(this.quantityUpDown);
            this.basicInfoGroupBox.Controls.Add(this.label1);
            this.basicInfoGroupBox.Location = new System.Drawing.Point(12, 12);
            this.basicInfoGroupBox.Name = "basicInfoGroupBox";
            this.basicInfoGroupBox.Size = new System.Drawing.Size(432, 64);
            this.basicInfoGroupBox.TabIndex = 0;
            this.basicInfoGroupBox.TabStop = false;
            this.basicInfoGroupBox.Text = "Basic Info";
            // 
            // quantityUpDown
            // 
            this.quantityUpDown.Location = new System.Drawing.Point(83, 27);
            this.quantityUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.quantityUpDown.Name = "quantityUpDown";
            this.quantityUpDown.Size = new System.Drawing.Size(322, 22);
            this.quantityUpDown.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Quantity:";
            // 
            // modInfoGroupBox
            // 
            this.modInfoGroupBox.Controls.Add(this.groupBox1);
            this.modInfoGroupBox.Controls.Add(this.modsBaseIdBox);
            this.modInfoGroupBox.Controls.Add(this.label2);
            this.modInfoGroupBox.Location = new System.Drawing.Point(12, 201);
            this.modInfoGroupBox.Name = "modInfoGroupBox";
            this.modInfoGroupBox.Size = new System.Drawing.Size(432, 356);
            this.modInfoGroupBox.TabIndex = 1;
            this.modInfoGroupBox.TabStop = false;
            this.modInfoGroupBox.Text = "Mods/Special Info";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.modsTreeView);
            this.groupBox1.Location = new System.Drawing.Point(26, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 264);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mods Tree";
            // 
            // modsTreeView
            // 
            this.modsTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.modsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modsTreeView.Location = new System.Drawing.Point(3, 18);
            this.modsTreeView.Name = "modsTreeView";
            this.modsTreeView.Size = new System.Drawing.Size(373, 243);
            this.modsTreeView.TabIndex = 0;
            // 
            // modsBaseIdBox
            // 
            this.modsBaseIdBox.Location = new System.Drawing.Point(89, 34);
            this.modsBaseIdBox.Name = "modsBaseIdBox";
            this.modsBaseIdBox.Size = new System.Drawing.Size(316, 22);
            this.modsBaseIdBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Special ID:";
            // 
            // pasteLegendaryIdButton
            // 
            this.pasteLegendaryIdButton.BackColor = System.Drawing.Color.White;
            this.pasteLegendaryIdButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pasteLegendaryIdButton.ClickEffectEnabled = true;
            this.pasteLegendaryIdButton.DefaultColor = System.Drawing.Color.White;
            this.pasteLegendaryIdButton.HoverColor = System.Drawing.Color.LightGray;
            this.pasteLegendaryIdButton.Location = new System.Drawing.Point(6, 21);
            this.pasteLegendaryIdButton.Name = "pasteLegendaryIdButton";
            this.pasteLegendaryIdButton.Size = new System.Drawing.Size(420, 24);
            this.pasteLegendaryIdButton.TabIndex = 2;
            this.pasteLegendaryIdButton.Text = "Make Legendary (May not work on everything)";
            this.pasteLegendaryIdButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.pasteLegendaryIdButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pasteLegendaryIdButton.Click += new System.EventHandler(this.pasteLegendaryIdButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.BackColor = System.Drawing.Color.White;
            this.applyButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.applyButton.ClickEffectEnabled = true;
            this.applyButton.DefaultColor = System.Drawing.Color.White;
            this.applyButton.HoverColor = System.Drawing.Color.LightGray;
            this.applyButton.Location = new System.Drawing.Point(230, 565);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(104, 26);
            this.applyButton.TabIndex = 2;
            this.applyButton.Text = "Apply";
            this.applyButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.applyButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.White;
            this.closeButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.closeButton.ClickEffectEnabled = true;
            this.closeButton.DefaultColor = System.Drawing.Color.White;
            this.closeButton.HoverColor = System.Drawing.Color.LightGray;
            this.closeButton.Location = new System.Drawing.Point(340, 565);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(104, 26);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.closeButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // quickActionsGroupBox
            // 
            this.quickActionsGroupBox.Controls.Add(this.pasteLegendaryIdButton);
            this.quickActionsGroupBox.Location = new System.Drawing.Point(12, 138);
            this.quickActionsGroupBox.Name = "quickActionsGroupBox";
            this.quickActionsGroupBox.Size = new System.Drawing.Size(432, 57);
            this.quickActionsGroupBox.TabIndex = 4;
            this.quickActionsGroupBox.TabStop = false;
            this.quickActionsGroupBox.Text = "Quick Actions";
            // 
            // questItemCheckBox
            // 
            this.questItemCheckBox.AutoSize = true;
            this.questItemCheckBox.Location = new System.Drawing.Point(263, 21);
            this.questItemCheckBox.Name = "questItemCheckBox";
            this.questItemCheckBox.Size = new System.Drawing.Size(92, 17);
            this.questItemCheckBox.TabIndex = 3;
            this.questItemCheckBox.Text = "Is Quest Item";
            this.questItemCheckBox.UseVisualStyleBackColor = true;
            // 
            // flagsGroupBox
            // 
            this.flagsGroupBox.Controls.Add(this.unknownFlag1CheckBox);
            this.flagsGroupBox.Controls.Add(this.questItemCheckBox);
            this.flagsGroupBox.Location = new System.Drawing.Point(12, 82);
            this.flagsGroupBox.Name = "flagsGroupBox";
            this.flagsGroupBox.Size = new System.Drawing.Size(432, 50);
            this.flagsGroupBox.TabIndex = 5;
            this.flagsGroupBox.TabStop = false;
            this.flagsGroupBox.Text = "Flags";
            // 
            // unknownFlag1CheckBox
            // 
            this.unknownFlag1CheckBox.AutoSize = true;
            this.unknownFlag1CheckBox.Location = new System.Drawing.Point(73, 21);
            this.unknownFlag1CheckBox.Name = "unknownFlag1CheckBox";
            this.unknownFlag1CheckBox.Size = new System.Drawing.Size(111, 17);
            this.unknownFlag1CheckBox.TabIndex = 4;
            this.unknownFlag1CheckBox.Text = "Unknown Flag 1";
            this.unknownFlag1CheckBox.UseVisualStyleBackColor = true;
            // 
            // ItemDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(456, 603);
            this.Controls.Add(this.flagsGroupBox);
            this.Controls.Add(this.quickActionsGroupBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.modInfoGroupBox);
            this.Controls.Add(this.basicInfoGroupBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ItemDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ItemDetails";
            this.Load += new System.EventHandler(this.ItemDetails_Load);
            this.basicInfoGroupBox.ResumeLayout(false);
            this.basicInfoGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.quantityUpDown)).EndInit();
            this.modInfoGroupBox.ResumeLayout(false);
            this.modInfoGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.quickActionsGroupBox.ResumeLayout(false);
            this.flagsGroupBox.ResumeLayout(false);
            this.flagsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox basicInfoGroupBox;
        private System.Windows.Forms.NumericUpDown quantityUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox modInfoGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private ModernButton pasteLegendaryIdButton;
        private System.Windows.Forms.TextBox modsBaseIdBox;
        private System.Windows.Forms.Label label2;
        private ModernButton applyButton;
        private ModernButton closeButton;
        private System.Windows.Forms.GroupBox quickActionsGroupBox;
        private System.Windows.Forms.TreeView modsTreeView;
        private System.Windows.Forms.CheckBox questItemCheckBox;
        private System.Windows.Forms.GroupBox flagsGroupBox;
        private System.Windows.Forms.CheckBox unknownFlag1CheckBox;
    }
}