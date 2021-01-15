
namespace CP2077SaveEditor
{
    partial class StatDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatDetails));
            this.label1 = new System.Windows.Forms.Label();
            this.statTabControl = new System.Windows.Forms.TabControl();
            this.combinedStatTab = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.combinedValue = new System.Windows.Forms.TextBox();
            this.combinedStatType = new System.Windows.Forms.ComboBox();
            this.combinedRefStatType = new System.Windows.Forms.ComboBox();
            this.combinedRefObject = new System.Windows.Forms.ComboBox();
            this.combinedOperation = new System.Windows.Forms.ComboBox();
            this.combinedModifier = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.constantStatTab = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.constantValue = new System.Windows.Forms.TextBox();
            this.constantStatType = new System.Windows.Forms.ComboBox();
            this.constantModifier = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.curveStatTab = new System.Windows.Forms.TabPage();
            this.curveName = new System.Windows.Forms.TextBox();
            this.curveColumnName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.curveStatType = new System.Windows.Forms.ComboBox();
            this.curveStat = new System.Windows.Forms.ComboBox();
            this.curveModifier = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.applyCloseButton = new CP2077SaveEditor.ModernButton();
            this.statTabControl.SuspendLayout();
            this.combinedStatTab.SuspendLayout();
            this.constantStatTab.SuspendLayout();
            this.curveStatTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Modifier:";
            // 
            // statTabControl
            // 
            this.statTabControl.Controls.Add(this.combinedStatTab);
            this.statTabControl.Controls.Add(this.constantStatTab);
            this.statTabControl.Controls.Add(this.curveStatTab);
            this.statTabControl.Location = new System.Drawing.Point(12, 12);
            this.statTabControl.Name = "statTabControl";
            this.statTabControl.SelectedIndex = 0;
            this.statTabControl.Size = new System.Drawing.Size(366, 210);
            this.statTabControl.TabIndex = 1;
            // 
            // combinedStatTab
            // 
            this.combinedStatTab.Controls.Add(this.label6);
            this.combinedStatTab.Controls.Add(this.label5);
            this.combinedStatTab.Controls.Add(this.label4);
            this.combinedStatTab.Controls.Add(this.label3);
            this.combinedStatTab.Controls.Add(this.combinedValue);
            this.combinedStatTab.Controls.Add(this.combinedStatType);
            this.combinedStatTab.Controls.Add(this.combinedRefStatType);
            this.combinedStatTab.Controls.Add(this.combinedRefObject);
            this.combinedStatTab.Controls.Add(this.combinedOperation);
            this.combinedStatTab.Controls.Add(this.combinedModifier);
            this.combinedStatTab.Controls.Add(this.label2);
            this.combinedStatTab.Controls.Add(this.label1);
            this.combinedStatTab.Location = new System.Drawing.Point(4, 22);
            this.combinedStatTab.Name = "combinedStatTab";
            this.combinedStatTab.Padding = new System.Windows.Forms.Padding(3);
            this.combinedStatTab.Size = new System.Drawing.Size(358, 184);
            this.combinedStatTab.TabIndex = 0;
            this.combinedStatTab.Text = "Combined";
            this.combinedStatTab.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(59, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Value:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(67, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Stat:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Ref Stat:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Ref Object:";
            // 
            // combinedValue
            // 
            this.combinedValue.Location = new System.Drawing.Point(103, 147);
            this.combinedValue.Name = "combinedValue";
            this.combinedValue.Size = new System.Drawing.Size(231, 22);
            this.combinedValue.TabIndex = 7;
            // 
            // combinedStatType
            // 
            this.combinedStatType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.combinedStatType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.combinedStatType.FormattingEnabled = true;
            this.combinedStatType.Location = new System.Drawing.Point(103, 120);
            this.combinedStatType.Name = "combinedStatType";
            this.combinedStatType.Size = new System.Drawing.Size(231, 21);
            this.combinedStatType.TabIndex = 6;
            // 
            // combinedRefStatType
            // 
            this.combinedRefStatType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.combinedRefStatType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.combinedRefStatType.FormattingEnabled = true;
            this.combinedRefStatType.Location = new System.Drawing.Point(103, 93);
            this.combinedRefStatType.Name = "combinedRefStatType";
            this.combinedRefStatType.Size = new System.Drawing.Size(231, 21);
            this.combinedRefStatType.TabIndex = 5;
            // 
            // combinedRefObject
            // 
            this.combinedRefObject.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.combinedRefObject.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.combinedRefObject.FormattingEnabled = true;
            this.combinedRefObject.Items.AddRange(new object[] {
            "Self",
            "Owner",
            "Root",
            "Parent",
            "Target",
            "Player",
            "Instigator",
            "Count",
            "Invalid"});
            this.combinedRefObject.Location = new System.Drawing.Point(103, 66);
            this.combinedRefObject.Name = "combinedRefObject";
            this.combinedRefObject.Size = new System.Drawing.Size(231, 21);
            this.combinedRefObject.TabIndex = 4;
            // 
            // combinedOperation
            // 
            this.combinedOperation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.combinedOperation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.combinedOperation.FormattingEnabled = true;
            this.combinedOperation.Items.AddRange(new object[] {
            "Addition",
            "Subtraction",
            "Multiplication",
            "Division",
            "Modulo",
            "Invert",
            "Count",
            "Invalid"});
            this.combinedOperation.Location = new System.Drawing.Point(103, 39);
            this.combinedOperation.Name = "combinedOperation";
            this.combinedOperation.Size = new System.Drawing.Size(231, 21);
            this.combinedOperation.TabIndex = 3;
            // 
            // combinedModifier
            // 
            this.combinedModifier.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.combinedModifier.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.combinedModifier.FormattingEnabled = true;
            this.combinedModifier.Items.AddRange(new object[] {
            "Additive",
            "AdditiveMultiplier",
            "Multiplier",
            "Count",
            "Invalid"});
            this.combinedModifier.Location = new System.Drawing.Point(103, 12);
            this.combinedModifier.Name = "combinedModifier";
            this.combinedModifier.Size = new System.Drawing.Size(231, 21);
            this.combinedModifier.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Operation:";
            // 
            // constantStatTab
            // 
            this.constantStatTab.Controls.Add(this.label7);
            this.constantStatTab.Controls.Add(this.label8);
            this.constantStatTab.Controls.Add(this.constantValue);
            this.constantStatTab.Controls.Add(this.constantStatType);
            this.constantStatTab.Controls.Add(this.constantModifier);
            this.constantStatTab.Controls.Add(this.label12);
            this.constantStatTab.Location = new System.Drawing.Point(4, 22);
            this.constantStatTab.Name = "constantStatTab";
            this.constantStatTab.Padding = new System.Windows.Forms.Padding(3);
            this.constantStatTab.Size = new System.Drawing.Size(358, 184);
            this.constantStatTab.TabIndex = 1;
            this.constantStatTab.Text = "Constant";
            this.constantStatTab.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(59, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Value:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(67, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Stat:";
            // 
            // constantValue
            // 
            this.constantValue.Location = new System.Drawing.Point(103, 66);
            this.constantValue.Name = "constantValue";
            this.constantValue.Size = new System.Drawing.Size(231, 22);
            this.constantValue.TabIndex = 19;
            // 
            // constantStatType
            // 
            this.constantStatType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.constantStatType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.constantStatType.FormattingEnabled = true;
            this.constantStatType.Location = new System.Drawing.Point(103, 39);
            this.constantStatType.Name = "constantStatType";
            this.constantStatType.Size = new System.Drawing.Size(231, 21);
            this.constantStatType.TabIndex = 18;
            // 
            // constantModifier
            // 
            this.constantModifier.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.constantModifier.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.constantModifier.FormattingEnabled = true;
            this.constantModifier.Items.AddRange(new object[] {
            "Additive",
            "AdditiveMultiplier",
            "Multiplier",
            "Count",
            "Invalid"});
            this.constantModifier.Location = new System.Drawing.Point(103, 12);
            this.constantModifier.Name = "constantModifier";
            this.constantModifier.Size = new System.Drawing.Size(231, 21);
            this.constantModifier.TabIndex = 14;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(43, 15);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Modifier:";
            // 
            // curveStatTab
            // 
            this.curveStatTab.Controls.Add(this.curveName);
            this.curveStatTab.Controls.Add(this.curveColumnName);
            this.curveStatTab.Controls.Add(this.label10);
            this.curveStatTab.Controls.Add(this.label11);
            this.curveStatTab.Controls.Add(this.label13);
            this.curveStatTab.Controls.Add(this.curveStatType);
            this.curveStatTab.Controls.Add(this.curveStat);
            this.curveStatTab.Controls.Add(this.curveModifier);
            this.curveStatTab.Controls.Add(this.label14);
            this.curveStatTab.Controls.Add(this.label15);
            this.curveStatTab.Location = new System.Drawing.Point(4, 22);
            this.curveStatTab.Name = "curveStatTab";
            this.curveStatTab.Size = new System.Drawing.Size(358, 184);
            this.curveStatTab.TabIndex = 2;
            this.curveStatTab.Text = "Curve";
            this.curveStatTab.UseVisualStyleBackColor = true;
            // 
            // curveName
            // 
            this.curveName.Location = new System.Drawing.Point(103, 66);
            this.curveName.Name = "curveName";
            this.curveName.Size = new System.Drawing.Size(231, 22);
            this.curveName.TabIndex = 24;
            // 
            // curveColumnName
            // 
            this.curveColumnName.Location = new System.Drawing.Point(103, 39);
            this.curveColumnName.Name = "curveColumnName";
            this.curveColumnName.Size = new System.Drawing.Size(231, 22);
            this.curveColumnName.TabIndex = 23;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(67, 123);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Stat:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(35, 96);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Curve Stat:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(26, 69);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "Curve Name:";
            // 
            // curveStatType
            // 
            this.curveStatType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.curveStatType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.curveStatType.FormattingEnabled = true;
            this.curveStatType.Location = new System.Drawing.Point(103, 120);
            this.curveStatType.Name = "curveStatType";
            this.curveStatType.Size = new System.Drawing.Size(231, 21);
            this.curveStatType.TabIndex = 18;
            // 
            // curveStat
            // 
            this.curveStat.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.curveStat.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.curveStat.FormattingEnabled = true;
            this.curveStat.Location = new System.Drawing.Point(103, 93);
            this.curveStat.Name = "curveStat";
            this.curveStat.Size = new System.Drawing.Size(231, 21);
            this.curveStat.TabIndex = 17;
            // 
            // curveModifier
            // 
            this.curveModifier.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.curveModifier.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.curveModifier.FormattingEnabled = true;
            this.curveModifier.Items.AddRange(new object[] {
            "Additive",
            "AdditiveMultiplier",
            "Multiplier",
            "Count",
            "Invalid"});
            this.curveModifier.Location = new System.Drawing.Point(103, 12);
            this.curveModifier.Name = "curveModifier";
            this.curveModifier.Size = new System.Drawing.Size(231, 21);
            this.curveModifier.TabIndex = 14;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 42);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 13);
            this.label14.TabIndex = 13;
            this.label14.Text = "Column Name:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(43, 15);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(54, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "Modifier:";
            // 
            // applyCloseButton
            // 
            this.applyCloseButton.BackColor = System.Drawing.Color.White;
            this.applyCloseButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.applyCloseButton.ClickEffectEnabled = true;
            this.applyCloseButton.DefaultColor = System.Drawing.Color.White;
            this.applyCloseButton.HoverColor = System.Drawing.Color.LightGray;
            this.applyCloseButton.Location = new System.Drawing.Point(12, 231);
            this.applyCloseButton.Name = "applyCloseButton";
            this.applyCloseButton.Size = new System.Drawing.Size(366, 25);
            this.applyCloseButton.TabIndex = 10;
            this.applyCloseButton.Text = "Apply && Close";
            this.applyCloseButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.applyCloseButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.applyCloseButton.Click += new System.EventHandler(this.applyCloseButton_Click);
            // 
            // StatDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(393, 267);
            this.Controls.Add(this.applyCloseButton);
            this.Controls.Add(this.statTabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StatDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Stat Details";
            this.statTabControl.ResumeLayout(false);
            this.combinedStatTab.ResumeLayout(false);
            this.combinedStatTab.PerformLayout();
            this.constantStatTab.ResumeLayout(false);
            this.constantStatTab.PerformLayout();
            this.curveStatTab.ResumeLayout(false);
            this.curveStatTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl statTabControl;
        private System.Windows.Forms.TabPage combinedStatTab;
        private System.Windows.Forms.TabPage constantStatTab;
        private System.Windows.Forms.TabPage curveStatTab;
        private System.Windows.Forms.ComboBox combinedOperation;
        private System.Windows.Forms.ComboBox combinedModifier;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox combinedValue;
        private System.Windows.Forms.ComboBox combinedStatType;
        private System.Windows.Forms.ComboBox combinedRefStatType;
        private System.Windows.Forms.ComboBox combinedRefObject;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox constantValue;
        private System.Windows.Forms.ComboBox constantStatType;
        private System.Windows.Forms.ComboBox constantModifier;
        private System.Windows.Forms.Label label12;
        private ModernButton applyCloseButton;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox curveStatType;
        private System.Windows.Forms.ComboBox curveStat;
        private System.Windows.Forms.ComboBox curveModifier;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox curveName;
        private System.Windows.Forms.TextBox curveColumnName;
    }
}