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
            components = new System.ComponentModel.Container();
            groupBox2 = new System.Windows.Forms.GroupBox();
            containersListBox = new System.Windows.Forms.ListBox();
            containerGroupBox = new System.Windows.Forms.GroupBox();
            inventorySearchBox = new System.Windows.Forms.TextBox();
            inventoryListView = new System.Windows.Forms.ListView();
            inventoryNameHeader = new System.Windows.Forms.ColumnHeader();
            inventoryTypeHeader = new System.Windows.Forms.ColumnHeader();
            inventoryIdHeader = new System.Windows.Forms.ColumnHeader();
            inventorySlotHeader = new System.Windows.Forms.ColumnHeader();
            inventoryQuantityHeader = new System.Windows.Forms.ColumnHeader();
            inventoryDescriptionHeader = new System.Windows.Forms.ColumnHeader();
            groupBox1 = new System.Windows.Forms.GroupBox();
            debloatButton = new ModernButton();
            clearQuestFlagsButton = new ModernButton();
            label2 = new System.Windows.Forms.Label();
            moneyUpDown = new System.Windows.Forms.NumericUpDown();
            debloatWorker = new System.ComponentModel.BackgroundWorker();
            debloatTimer = new System.Windows.Forms.Timer(components);
            groupBox2.SuspendLayout();
            containerGroupBox.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)moneyUpDown).BeginInit();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            groupBox2.Controls.Add(containersListBox);
            groupBox2.Location = new System.Drawing.Point(537, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(311, 65);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "Inventories";
            // 
            // containersListBox
            // 
            containersListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            containersListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            containersListBox.FormattingEnabled = true;
            containersListBox.ItemHeight = 15;
            containersListBox.Location = new System.Drawing.Point(3, 19);
            containersListBox.Name = "containersListBox";
            containersListBox.ScrollAlwaysVisible = true;
            containersListBox.Size = new System.Drawing.Size(305, 43);
            containersListBox.TabIndex = 4;
            containersListBox.SelectedIndexChanged += containersListBox_SelectedIndexChanged;
            // 
            // containerGroupBox
            // 
            containerGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            containerGroupBox.Controls.Add(inventorySearchBox);
            containerGroupBox.Controls.Add(inventoryListView);
            containerGroupBox.Location = new System.Drawing.Point(3, 74);
            containerGroupBox.Name = "containerGroupBox";
            containerGroupBox.Size = new System.Drawing.Size(845, 471);
            containerGroupBox.TabIndex = 9;
            containerGroupBox.TabStop = false;
            // 
            // inventorySearchBox
            // 
            inventorySearchBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            inventorySearchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            inventorySearchBox.ForeColor = System.Drawing.Color.Silver;
            inventorySearchBox.Location = new System.Drawing.Point(6, 17);
            inventorySearchBox.Name = "inventorySearchBox";
            inventorySearchBox.Size = new System.Drawing.Size(833, 16);
            inventorySearchBox.TabIndex = 2;
            inventorySearchBox.Text = "Search";
            inventorySearchBox.TextChanged += inventorySearchBox_TextChanged;
            inventorySearchBox.GotFocus += SearchBoxGotFocus;
            inventorySearchBox.LostFocus += SearchBoxLostFocus;
            // 
            // inventoryListView
            // 
            inventoryListView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            inventoryListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            inventoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { inventoryNameHeader, inventoryTypeHeader, inventoryIdHeader, inventorySlotHeader, inventoryQuantityHeader, inventoryDescriptionHeader });
            inventoryListView.FullRowSelect = true;
            inventoryListView.GridLines = true;
            inventoryListView.Location = new System.Drawing.Point(6, 39);
            inventoryListView.MultiSelect = false;
            inventoryListView.Name = "inventoryListView";
            inventoryListView.Size = new System.Drawing.Size(833, 426);
            inventoryListView.TabIndex = 0;
            inventoryListView.UseCompatibleStateImageBehavior = false;
            inventoryListView.View = System.Windows.Forms.View.Details;
            inventoryListView.VirtualMode = true;
            inventoryListView.DoubleClick += inventoryListView_DoubleClick;
            inventoryListView.KeyDown += inventoryListView_KeyDown;
            inventoryListView.MouseClick += inventoryListView_MouseClick;
            // 
            // inventoryNameHeader
            // 
            inventoryNameHeader.Name = "inventoryNameHeader";
            inventoryNameHeader.Text = "Item Name";
            inventoryNameHeader.Width = 171;
            // 
            // inventoryTypeHeader
            // 
            inventoryTypeHeader.Name = "inventoryTypeHeader";
            inventoryTypeHeader.Text = "Type";
            inventoryTypeHeader.Width = 119;
            // 
            // inventoryIdHeader
            // 
            inventoryIdHeader.Name = "inventoryIdHeader";
            inventoryIdHeader.Text = "ID";
            inventoryIdHeader.Width = 127;
            // 
            // inventorySlotHeader
            // 
            inventorySlotHeader.Name = "inventorySlotHeader";
            inventorySlotHeader.Text = "Slot";
            inventorySlotHeader.Width = 94;
            // 
            // inventoryQuantityHeader
            // 
            inventoryQuantityHeader.Name = "inventoryQuantityHeader";
            inventoryQuantityHeader.Text = "Quantity";
            inventoryQuantityHeader.Width = 64;
            // 
            // inventoryDescriptionHeader
            // 
            inventoryDescriptionHeader.Name = "inventoryDescriptionHeader";
            inventoryDescriptionHeader.Text = "Description";
            inventoryDescriptionHeader.Width = 240;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(debloatButton);
            groupBox1.Controls.Add(clearQuestFlagsButton);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(moneyUpDown);
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(528, 65);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Quick Actions";
            // 
            // debloatButton
            // 
            debloatButton.BackColor = System.Drawing.Color.White;
            debloatButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            debloatButton.ClickEffectEnabled = true;
            debloatButton.DefaultColor = System.Drawing.Color.White;
            debloatButton.Enabled = false;
            debloatButton.HoverColor = System.Drawing.Color.LightGray;
            debloatButton.Location = new System.Drawing.Point(381, 27);
            debloatButton.Name = "debloatButton";
            debloatButton.Size = new System.Drawing.Size(131, 22);
            debloatButton.TabIndex = 3;
            debloatButton.Text = "De-Bloat";
            debloatButton.TextColor = System.Drawing.SystemColors.ControlText;
            debloatButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            debloatButton.Click += debloatButton_Click;
            // 
            // clearQuestFlagsButton
            // 
            clearQuestFlagsButton.BackColor = System.Drawing.Color.White;
            clearQuestFlagsButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            clearQuestFlagsButton.ClickEffectEnabled = true;
            clearQuestFlagsButton.DefaultColor = System.Drawing.Color.White;
            clearQuestFlagsButton.HoverColor = System.Drawing.Color.LightGray;
            clearQuestFlagsButton.Location = new System.Drawing.Point(224, 27);
            clearQuestFlagsButton.Name = "clearQuestFlagsButton";
            clearQuestFlagsButton.Size = new System.Drawing.Size(151, 22);
            clearQuestFlagsButton.TabIndex = 2;
            clearQuestFlagsButton.Text = "Clear All Item Flags";
            clearQuestFlagsButton.TextColor = System.Drawing.SystemColors.ControlText;
            clearQuestFlagsButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            clearQuestFlagsButton.Click += clearQuestFlagsButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(13, 29);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(47, 15);
            label2.TabIndex = 0;
            label2.Text = "Money:";
            // 
            // moneyUpDown
            // 
            moneyUpDown.Location = new System.Drawing.Point(64, 27);
            moneyUpDown.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            moneyUpDown.Name = "moneyUpDown";
            moneyUpDown.Size = new System.Drawing.Size(138, 23);
            moneyUpDown.TabIndex = 1;
            moneyUpDown.ValueChanged += moneyUpDown_ValueChanged;
            // 
            // debloatWorker
            // 
            debloatWorker.DoWork += debloatWorker_DoWork;
            debloatWorker.RunWorkerCompleted += debloatWorker_RunWorkerCompleted;
            // 
            // debloatTimer
            // 
            debloatTimer.Tick += debloatTimer_Tick;
            // 
            // InventoryControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(groupBox2);
            Controls.Add(containerGroupBox);
            Controls.Add(groupBox1);
            Name = "InventoryControl";
            Size = new System.Drawing.Size(851, 548);
            groupBox2.ResumeLayout(false);
            containerGroupBox.ResumeLayout(false);
            containerGroupBox.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)moneyUpDown).EndInit();
            ResumeLayout(false);
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
