namespace CP2077SaveEditor.Views.Controls
{
    partial class InventoryControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.containersListBox = new System.Windows.Forms.ListBox();
            this.containerGroupBox = new System.Windows.Forms.GroupBox();
            this.inventorySearchBox = new System.Windows.Forms.TextBox();
            this.inventoryListView = new System.Windows.Forms.ListView();
            this.inventoryNameHeader = new System.Windows.Forms.ColumnHeader();
            this.inventoryTypeHeader = new System.Windows.Forms.ColumnHeader();
            this.inventoryIdHeader = new System.Windows.Forms.ColumnHeader();
            this.inventorySlotHeader = new System.Windows.Forms.ColumnHeader();
            this.inventoryQuantityHeader = new System.Windows.Forms.ColumnHeader();
            this.inventoryDescriptionHeader = new System.Windows.Forms.ColumnHeader();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.debloatButton = new CP2077SaveEditor.ModernButton();
            this.clearQuestFlagsButton = new CP2077SaveEditor.ModernButton();
            this.label2 = new System.Windows.Forms.Label();
            this.moneyUpDown = new System.Windows.Forms.NumericUpDown();
            this.debloatWorker = new System.ComponentModel.BackgroundWorker();
            this.debloatTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox2.SuspendLayout();
            this.containerGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.moneyUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.containersListBox);
            this.groupBox2.Location = new System.Drawing.Point(537, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 65);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Inventories";
            // 
            // containersListBox
            // 
            this.containersListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.containersListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containersListBox.FormattingEnabled = true;
            this.containersListBox.Location = new System.Drawing.Point(3, 19);
            this.containersListBox.Name = "containersListBox";
            this.containersListBox.Size = new System.Drawing.Size(305, 43);
            this.containersListBox.TabIndex = 4;
            this.containersListBox.SelectedIndexChanged += new System.EventHandler(this.containersListBox_SelectedIndexChanged);
            // 
            // containerGroupBox
            // 
            this.containerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.containerGroupBox.Controls.Add(this.inventorySearchBox);
            this.containerGroupBox.Controls.Add(this.inventoryListView);
            this.containerGroupBox.Location = new System.Drawing.Point(3, 74);
            this.containerGroupBox.Name = "containerGroupBox";
            this.containerGroupBox.Size = new System.Drawing.Size(845, 471);
            this.containerGroupBox.TabIndex = 9;
            this.containerGroupBox.TabStop = false;
            // 
            // inventorySearchBox
            // 
            this.inventorySearchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inventorySearchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inventorySearchBox.ForeColor = System.Drawing.Color.Silver;
            this.inventorySearchBox.Location = new System.Drawing.Point(6, 17);
            this.inventorySearchBox.Name = "inventorySearchBox";
            this.inventorySearchBox.Size = new System.Drawing.Size(833, 16);
            this.inventorySearchBox.TabIndex = 2;
            this.inventorySearchBox.Text = "Search";
            this.inventorySearchBox.TextChanged += new System.EventHandler(this.inventorySearchBox_TextChanged);
            this.inventorySearchBox.GotFocus += new System.EventHandler(this.SearchBoxGotFocus);
            this.inventorySearchBox.LostFocus += new System.EventHandler(this.SearchBoxLostFocus);
            // 
            // inventoryListView
            // 
            this.inventoryListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inventoryListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inventoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.inventoryNameHeader,
            this.inventoryTypeHeader,
            this.inventoryIdHeader,
            this.inventorySlotHeader,
            this.inventoryQuantityHeader,
            this.inventoryDescriptionHeader});
            this.inventoryListView.FullRowSelect = true;
            this.inventoryListView.GridLines = true;
            this.inventoryListView.Location = new System.Drawing.Point(6, 39);
            this.inventoryListView.MultiSelect = false;
            this.inventoryListView.Name = "inventoryListView";
            this.inventoryListView.Size = new System.Drawing.Size(833, 426);
            this.inventoryListView.TabIndex = 0;
            this.inventoryListView.UseCompatibleStateImageBehavior = false;
            this.inventoryListView.View = System.Windows.Forms.View.Details;
            this.inventoryListView.VirtualMode = true;
            this.inventoryListView.DoubleClick += new System.EventHandler(this.inventoryListView_DoubleClick);
            this.inventoryListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inventoryListView_KeyDown);
            this.inventoryListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.inventoryListView_MouseClick);
            // 
            // inventoryNameHeader
            // 
            this.inventoryNameHeader.Name = "inventoryNameHeader";
            this.inventoryNameHeader.Text = "Item Name";
            this.inventoryNameHeader.Width = 171;
            // 
            // inventoryTypeHeader
            // 
            this.inventoryTypeHeader.Name = "inventoryTypeHeader";
            this.inventoryTypeHeader.Text = "Type";
            this.inventoryTypeHeader.Width = 119;
            // 
            // inventoryIdHeader
            // 
            this.inventoryIdHeader.Name = "inventoryIdHeader";
            this.inventoryIdHeader.Text = "ID";
            this.inventoryIdHeader.Width = 127;
            // 
            // inventorySlotHeader
            // 
            this.inventorySlotHeader.Name = "inventorySlotHeader";
            this.inventorySlotHeader.Text = "Slot";
            this.inventorySlotHeader.Width = 94;
            // 
            // inventoryQuantityHeader
            // 
            this.inventoryQuantityHeader.Name = "inventoryQuantityHeader";
            this.inventoryQuantityHeader.Text = "Quantity";
            this.inventoryQuantityHeader.Width = 64;
            // 
            // inventoryDescriptionHeader
            // 
            this.inventoryDescriptionHeader.Name = "inventoryDescriptionHeader";
            this.inventoryDescriptionHeader.Text = "Description";
            this.inventoryDescriptionHeader.Width = 240;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.debloatButton);
            this.groupBox1.Controls.Add(this.clearQuestFlagsButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.moneyUpDown);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(528, 65);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quick Actions";
            // 
            // debloatButton
            // 
            this.debloatButton.BackColor = System.Drawing.Color.White;
            this.debloatButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.debloatButton.ClickEffectEnabled = true;
            this.debloatButton.DefaultColor = System.Drawing.Color.White;
            this.debloatButton.Enabled = false;
            this.debloatButton.HoverColor = System.Drawing.Color.LightGray;
            this.debloatButton.Location = new System.Drawing.Point(381, 27);
            this.debloatButton.Name = "debloatButton";
            this.debloatButton.Size = new System.Drawing.Size(131, 22);
            this.debloatButton.TabIndex = 3;
            this.debloatButton.Text = "De-Bloat";
            this.debloatButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.debloatButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.debloatButton.Click += new System.EventHandler(this.debloatButton_Click);
            // 
            // clearQuestFlagsButton
            // 
            this.clearQuestFlagsButton.BackColor = System.Drawing.Color.White;
            this.clearQuestFlagsButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clearQuestFlagsButton.ClickEffectEnabled = true;
            this.clearQuestFlagsButton.DefaultColor = System.Drawing.Color.White;
            this.clearQuestFlagsButton.HoverColor = System.Drawing.Color.LightGray;
            this.clearQuestFlagsButton.Location = new System.Drawing.Point(224, 27);
            this.clearQuestFlagsButton.Name = "clearQuestFlagsButton";
            this.clearQuestFlagsButton.Size = new System.Drawing.Size(151, 22);
            this.clearQuestFlagsButton.TabIndex = 2;
            this.clearQuestFlagsButton.Text = "Clear All Item Flags";
            this.clearQuestFlagsButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.clearQuestFlagsButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.clearQuestFlagsButton.Click += new System.EventHandler(this.clearQuestFlagsButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Money:";
            // 
            // moneyUpDown
            // 
            this.moneyUpDown.Location = new System.Drawing.Point(64, 27);
            this.moneyUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.moneyUpDown.Name = "moneyUpDown";
            this.moneyUpDown.Size = new System.Drawing.Size(138, 23);
            this.moneyUpDown.TabIndex = 1;
            this.moneyUpDown.ValueChanged += new System.EventHandler(this.moneyUpDown_ValueChanged);
            // 
            // debloatWorker
            // 
            this.debloatWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.debloatWorker_DoWork);
            this.debloatWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.debloatWorker_RunWorkerCompleted);
            // 
            // debloatTimer
            // 
            this.debloatTimer.Tick += new System.EventHandler(this.debloatTimer_Tick);
            // 
            // InventoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.containerGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Name = "InventoryControl";
            this.Size = new System.Drawing.Size(851, 548);
            this.groupBox2.ResumeLayout(false);
            this.containerGroupBox.ResumeLayout(false);
            this.containerGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.moneyUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox containersListBox;
        private System.Windows.Forms.GroupBox containerGroupBox;
        private System.Windows.Forms.TextBox inventorySearchBox;
        private System.Windows.Forms.ListView inventoryListView;
        private System.Windows.Forms.ColumnHeader inventoryNameHeader;
        private System.Windows.Forms.ColumnHeader inventoryTypeHeader;
        private System.Windows.Forms.ColumnHeader inventoryIdHeader;
        private System.Windows.Forms.ColumnHeader inventorySlotHeader;
        private System.Windows.Forms.ColumnHeader inventoryQuantityHeader;
        private System.Windows.Forms.ColumnHeader inventoryDescriptionHeader;
        private System.Windows.Forms.GroupBox groupBox1;
        private ModernButton debloatButton;
        private ModernButton clearQuestFlagsButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown moneyUpDown;
        private System.ComponentModel.BackgroundWorker debloatWorker;
        private System.Windows.Forms.Timer debloatTimer;
    }
}
