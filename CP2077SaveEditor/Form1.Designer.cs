
namespace CP2077SaveEditor
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.optionsPanel = new System.Windows.Forms.Panel();
            this.editorPanel = new System.Windows.Forms.Panel();
            this.filePathLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.appearancePanel = new System.Windows.Forms.Panel();
            this.earsBox = new System.Windows.Forms.TextBox();
            this.jawBox = new System.Windows.Forms.TextBox();
            this.mouthBox = new System.Windows.Forms.TextBox();
            this.noseBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.eyesBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.inventoryPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.containersListBox = new System.Windows.Forms.ListBox();
            this.containerGroupBox = new System.Windows.Forms.GroupBox();
            this.inventoryListView = new System.Windows.Forms.ListView();
            this.inventoryNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.inventoryIdHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.inventoryQuantityHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.inventoryDescriptionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.moneyUpDown = new System.Windows.Forms.NumericUpDown();
            this.factsPanel = new System.Windows.Forms.Panel();
            this.factsListView = new System.Windows.Forms.ListView();
            this.factsValueHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.factsNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loadAppearButton = new CP2077SaveEditor.ModernButton();
            this.saveAppearButton = new CP2077SaveEditor.ModernButton();
            this.openSaveButton = new CP2077SaveEditor.ModernButton();
            this.factsButton = new CP2077SaveEditor.ModernButton();
            this.inventoryButton = new CP2077SaveEditor.ModernButton();
            this.saveChangesButton = new CP2077SaveEditor.ModernButton();
            this.appearanceButton = new CP2077SaveEditor.ModernButton();
            this.optionsPanel.SuspendLayout();
            this.appearancePanel.SuspendLayout();
            this.inventoryPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.containerGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.moneyUpDown)).BeginInit();
            this.factsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionsPanel
            // 
            this.optionsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.optionsPanel.BackColor = System.Drawing.Color.White;
            this.optionsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.optionsPanel.Controls.Add(this.factsButton);
            this.optionsPanel.Controls.Add(this.inventoryButton);
            this.optionsPanel.Controls.Add(this.saveChangesButton);
            this.optionsPanel.Controls.Add(this.appearanceButton);
            this.optionsPanel.Enabled = false;
            this.optionsPanel.Location = new System.Drawing.Point(12, 103);
            this.optionsPanel.Name = "optionsPanel";
            this.optionsPanel.Size = new System.Drawing.Size(143, 1064);
            this.optionsPanel.TabIndex = 1;
            // 
            // editorPanel
            // 
            this.editorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editorPanel.Enabled = false;
            this.editorPanel.Location = new System.Drawing.Point(161, 12);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(851, 548);
            this.editorPanel.TabIndex = 2;
            // 
            // filePathLabel
            // 
            this.filePathLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filePathLabel.Location = new System.Drawing.Point(12, 85);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(143, 15);
            this.filePathLabel.TabIndex = 2;
            this.filePathLabel.Text = "No save file selected.";
            this.filePathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(1511, 1154);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(379, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "v0.01a // CyberCAT by SirBitesalot and other contributors";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.Location = new System.Drawing.Point(161, 1154);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.statusLabel.Size = new System.Drawing.Size(379, 13);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Idle";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // appearancePanel
            // 
            this.appearancePanel.Controls.Add(this.loadAppearButton);
            this.appearancePanel.Controls.Add(this.saveAppearButton);
            this.appearancePanel.Controls.Add(this.earsBox);
            this.appearancePanel.Controls.Add(this.jawBox);
            this.appearancePanel.Controls.Add(this.mouthBox);
            this.appearancePanel.Controls.Add(this.noseBox);
            this.appearancePanel.Controls.Add(this.label7);
            this.appearancePanel.Controls.Add(this.label6);
            this.appearancePanel.Controls.Add(this.label5);
            this.appearancePanel.Controls.Add(this.label4);
            this.appearancePanel.Controls.Add(this.eyesBox);
            this.appearancePanel.Controls.Add(this.label3);
            this.appearancePanel.Enabled = false;
            this.appearancePanel.Location = new System.Drawing.Point(1018, 12);
            this.appearancePanel.Name = "appearancePanel";
            this.appearancePanel.Size = new System.Drawing.Size(851, 548);
            this.appearancePanel.TabIndex = 4;
            // 
            // earsBox
            // 
            this.earsBox.Location = new System.Drawing.Point(60, 136);
            this.earsBox.Name = "earsBox";
            this.earsBox.ReadOnly = true;
            this.earsBox.Size = new System.Drawing.Size(236, 22);
            this.earsBox.TabIndex = 9;
            // 
            // jawBox
            // 
            this.jawBox.Location = new System.Drawing.Point(60, 108);
            this.jawBox.Name = "jawBox";
            this.jawBox.ReadOnly = true;
            this.jawBox.Size = new System.Drawing.Size(236, 22);
            this.jawBox.TabIndex = 8;
            // 
            // mouthBox
            // 
            this.mouthBox.Location = new System.Drawing.Point(60, 80);
            this.mouthBox.Name = "mouthBox";
            this.mouthBox.ReadOnly = true;
            this.mouthBox.Size = new System.Drawing.Size(236, 22);
            this.mouthBox.TabIndex = 7;
            // 
            // noseBox
            // 
            this.noseBox.Location = new System.Drawing.Point(60, 52);
            this.noseBox.Name = "noseBox";
            this.noseBox.ReadOnly = true;
            this.noseBox.Size = new System.Drawing.Size(236, 22);
            this.noseBox.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Ears:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Jaw:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Mouth:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Nose:";
            // 
            // eyesBox
            // 
            this.eyesBox.Location = new System.Drawing.Point(60, 24);
            this.eyesBox.Name = "eyesBox";
            this.eyesBox.ReadOnly = true;
            this.eyesBox.Size = new System.Drawing.Size(236, 22);
            this.eyesBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Eyes:";
            // 
            // inventoryPanel
            // 
            this.inventoryPanel.Controls.Add(this.groupBox2);
            this.inventoryPanel.Controls.Add(this.containerGroupBox);
            this.inventoryPanel.Controls.Add(this.groupBox1);
            this.inventoryPanel.Enabled = false;
            this.inventoryPanel.Location = new System.Drawing.Point(161, 566);
            this.inventoryPanel.Name = "inventoryPanel";
            this.inventoryPanel.Size = new System.Drawing.Size(851, 548);
            this.inventoryPanel.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.containersListBox);
            this.groupBox2.Location = new System.Drawing.Point(271, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(574, 65);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Inventories";
            // 
            // containersListBox
            // 
            this.containersListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.containersListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containersListBox.FormattingEnabled = true;
            this.containersListBox.Location = new System.Drawing.Point(3, 18);
            this.containersListBox.Name = "containersListBox";
            this.containersListBox.Size = new System.Drawing.Size(568, 44);
            this.containersListBox.TabIndex = 4;
            this.containersListBox.SelectedIndexChanged += new System.EventHandler(this.containersListBox_SelectedIndexChanged);
            // 
            // containerGroupBox
            // 
            this.containerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.containerGroupBox.Controls.Add(this.inventoryListView);
            this.containerGroupBox.Location = new System.Drawing.Point(3, 86);
            this.containerGroupBox.Name = "containerGroupBox";
            this.containerGroupBox.Size = new System.Drawing.Size(845, 459);
            this.containerGroupBox.TabIndex = 6;
            this.containerGroupBox.TabStop = false;
            this.containerGroupBox.Visible = false;
            // 
            // inventoryListView
            // 
            this.inventoryListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inventoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.inventoryNameHeader,
            this.inventoryIdHeader,
            this.inventoryQuantityHeader,
            this.inventoryDescriptionHeader});
            this.inventoryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inventoryListView.FullRowSelect = true;
            this.inventoryListView.GridLines = true;
            this.inventoryListView.HideSelection = false;
            this.inventoryListView.Location = new System.Drawing.Point(3, 18);
            this.inventoryListView.MultiSelect = false;
            this.inventoryListView.Name = "inventoryListView";
            this.inventoryListView.Size = new System.Drawing.Size(839, 438);
            this.inventoryListView.TabIndex = 0;
            this.inventoryListView.UseCompatibleStateImageBehavior = false;
            this.inventoryListView.View = System.Windows.Forms.View.Details;
            // 
            // inventoryNameHeader
            // 
            this.inventoryNameHeader.Text = "Item Name";
            this.inventoryNameHeader.Width = 171;
            // 
            // inventoryIdHeader
            // 
            this.inventoryIdHeader.Text = "ID";
            this.inventoryIdHeader.Width = 127;
            // 
            // inventoryQuantityHeader
            // 
            this.inventoryQuantityHeader.Text = "Quantity";
            // 
            // inventoryDescriptionHeader
            // 
            this.inventoryDescriptionHeader.Text = "Description";
            this.inventoryDescriptionHeader.Width = 456;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.moneyUpDown);
            this.groupBox1.Location = new System.Drawing.Point(6, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 65);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quick Actions";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
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
            this.moneyUpDown.Size = new System.Drawing.Size(138, 22);
            this.moneyUpDown.TabIndex = 1;
            this.moneyUpDown.ValueChanged += new System.EventHandler(this.moneyUpDown_ValueChanged);
            // 
            // factsPanel
            // 
            this.factsPanel.Controls.Add(this.factsListView);
            this.factsPanel.Enabled = false;
            this.factsPanel.Location = new System.Drawing.Point(1018, 566);
            this.factsPanel.Name = "factsPanel";
            this.factsPanel.Size = new System.Drawing.Size(851, 548);
            this.factsPanel.TabIndex = 13;
            // 
            // factsListView
            // 
            this.factsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.factsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.factsValueHeader,
            this.factsNameHeader});
            this.factsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.factsListView.FullRowSelect = true;
            this.factsListView.GridLines = true;
            this.factsListView.HideSelection = false;
            this.factsListView.LabelEdit = true;
            this.factsListView.Location = new System.Drawing.Point(0, 0);
            this.factsListView.MultiSelect = false;
            this.factsListView.Name = "factsListView";
            this.factsListView.Size = new System.Drawing.Size(851, 548);
            this.factsListView.TabIndex = 0;
            this.factsListView.UseCompatibleStateImageBehavior = false;
            this.factsListView.View = System.Windows.Forms.View.Details;
            // 
            // factsValueHeader
            // 
            this.factsValueHeader.Text = "Value";
            this.factsValueHeader.Width = 59;
            // 
            // factsNameHeader
            // 
            this.factsNameHeader.Text = "Name";
            this.factsNameHeader.Width = 764;
            // 
            // loadAppearButton
            // 
            this.loadAppearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadAppearButton.BackColor = System.Drawing.Color.White;
            this.loadAppearButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loadAppearButton.ClickEffectEnabled = true;
            this.loadAppearButton.DefaultColor = System.Drawing.Color.White;
            this.loadAppearButton.HoverColor = System.Drawing.Color.LightGray;
            this.loadAppearButton.Location = new System.Drawing.Point(636, 501);
            this.loadAppearButton.Name = "loadAppearButton";
            this.loadAppearButton.Size = new System.Drawing.Size(98, 33);
            this.loadAppearButton.TabIndex = 11;
            this.loadAppearButton.Text = "Load Preset";
            this.loadAppearButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.loadAppearButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadAppearButton.Click += new System.EventHandler(this.loadAppearButton_Click);
            // 
            // saveAppearButton
            // 
            this.saveAppearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveAppearButton.BackColor = System.Drawing.Color.White;
            this.saveAppearButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.saveAppearButton.ClickEffectEnabled = true;
            this.saveAppearButton.DefaultColor = System.Drawing.Color.White;
            this.saveAppearButton.HoverColor = System.Drawing.Color.LightGray;
            this.saveAppearButton.Location = new System.Drawing.Point(740, 501);
            this.saveAppearButton.Name = "saveAppearButton";
            this.saveAppearButton.Size = new System.Drawing.Size(98, 33);
            this.saveAppearButton.TabIndex = 10;
            this.saveAppearButton.Text = "Save Preset";
            this.saveAppearButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.saveAppearButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveAppearButton.Click += new System.EventHandler(this.saveAppearButton_Click);
            // 
            // openSaveButton
            // 
            this.openSaveButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.openSaveButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.openSaveButton.ClickEffectEnabled = true;
            this.openSaveButton.DefaultColor = System.Drawing.Color.WhiteSmoke;
            this.openSaveButton.HoverColor = System.Drawing.Color.LightGray;
            this.openSaveButton.Location = new System.Drawing.Point(12, 12);
            this.openSaveButton.Name = "openSaveButton";
            this.openSaveButton.Size = new System.Drawing.Size(143, 70);
            this.openSaveButton.TabIndex = 0;
            this.openSaveButton.Text = "Load Save";
            this.openSaveButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.openSaveButton.TextFont = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openSaveButton.Click += new System.EventHandler(this.openSaveButton_Click);
            // 
            // factsButton
            // 
            this.factsButton.BackColor = System.Drawing.Color.White;
            this.factsButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.factsButton.ClickEffectEnabled = true;
            this.factsButton.DefaultColor = System.Drawing.Color.White;
            this.factsButton.HoverColor = System.Drawing.Color.DarkGray;
            this.factsButton.Location = new System.Drawing.Point(-1, 119);
            this.factsButton.Name = "factsButton";
            this.factsButton.Size = new System.Drawing.Size(143, 61);
            this.factsButton.TabIndex = 2;
            this.factsButton.Text = "Quest Facts";
            this.factsButton.TextColor = System.Drawing.Color.Black;
            this.factsButton.TextFont = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.factsButton.Click += new System.EventHandler(this.factsButton_Click);
            // 
            // inventoryButton
            // 
            this.inventoryButton.BackColor = System.Drawing.Color.White;
            this.inventoryButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inventoryButton.ClickEffectEnabled = true;
            this.inventoryButton.DefaultColor = System.Drawing.Color.White;
            this.inventoryButton.HoverColor = System.Drawing.Color.DarkGray;
            this.inventoryButton.Location = new System.Drawing.Point(-1, 59);
            this.inventoryButton.Name = "inventoryButton";
            this.inventoryButton.Size = new System.Drawing.Size(143, 61);
            this.inventoryButton.TabIndex = 1;
            this.inventoryButton.Text = "Inventory";
            this.inventoryButton.TextColor = System.Drawing.Color.Black;
            this.inventoryButton.TextFont = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inventoryButton.Click += new System.EventHandler(this.inventoryButton_Click);
            // 
            // saveChangesButton
            // 
            this.saveChangesButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.saveChangesButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.saveChangesButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.saveChangesButton.ClickEffectEnabled = true;
            this.saveChangesButton.DefaultColor = System.Drawing.Color.WhiteSmoke;
            this.saveChangesButton.HoverColor = System.Drawing.Color.LightGray;
            this.saveChangesButton.Location = new System.Drawing.Point(-1, 1019);
            this.saveChangesButton.Name = "saveChangesButton";
            this.saveChangesButton.Size = new System.Drawing.Size(143, 44);
            this.saveChangesButton.TabIndex = 1;
            this.saveChangesButton.Text = "Save Changes";
            this.saveChangesButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.saveChangesButton.TextFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveChangesButton.Click += new System.EventHandler(this.saveChangesButton_Click);
            // 
            // appearanceButton
            // 
            this.appearanceButton.BackColor = System.Drawing.Color.White;
            this.appearanceButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.appearanceButton.ClickEffectEnabled = false;
            this.appearanceButton.DefaultColor = System.Drawing.Color.White;
            this.appearanceButton.HoverColor = System.Drawing.Color.DarkGray;
            this.appearanceButton.Location = new System.Drawing.Point(-1, -1);
            this.appearanceButton.Name = "appearanceButton";
            this.appearanceButton.Size = new System.Drawing.Size(143, 61);
            this.appearanceButton.TabIndex = 0;
            this.appearanceButton.Text = "Appearance";
            this.appearanceButton.TextColor = System.Drawing.Color.Black;
            this.appearanceButton.TextFont = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appearanceButton.Click += new System.EventHandler(this.appearanceButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1902, 1179);
            this.Controls.Add(this.factsPanel);
            this.Controls.Add(this.inventoryPanel);
            this.Controls.Add(this.appearancePanel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.filePathLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.editorPanel);
            this.Controls.Add(this.openSaveButton);
            this.Controls.Add(this.optionsPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cyberpunk 2077 Save Editor (CyberCAT-SimpleGUI)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.optionsPanel.ResumeLayout(false);
            this.appearancePanel.ResumeLayout(false);
            this.appearancePanel.PerformLayout();
            this.inventoryPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.containerGroupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.moneyUpDown)).EndInit();
            this.factsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel optionsPanel;
        private ModernButton openSaveButton;
        private ModernButton appearanceButton;
        private ModernButton saveChangesButton;
        private System.Windows.Forms.Panel editorPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label filePathLabel;
        private ModernButton inventoryButton;
        private System.Windows.Forms.Panel appearancePanel;
        private System.Windows.Forms.Panel inventoryPanel;
        private System.Windows.Forms.NumericUpDown moneyUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox eyesBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox earsBox;
        private System.Windows.Forms.TextBox jawBox;
        private System.Windows.Forms.TextBox mouthBox;
        private System.Windows.Forms.TextBox noseBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox containersListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox containerGroupBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private ModernButton loadAppearButton;
        private ModernButton saveAppearButton;
        private ModernButton factsButton;
        private System.Windows.Forms.Panel factsPanel;
        private System.Windows.Forms.ListView factsListView;
        private System.Windows.Forms.ColumnHeader factsNameHeader;
        private System.Windows.Forms.ColumnHeader factsValueHeader;
        private System.Windows.Forms.ListView inventoryListView;
        private System.Windows.Forms.ColumnHeader inventoryNameHeader;
        private System.Windows.Forms.ColumnHeader inventoryIdHeader;
        private System.Windows.Forms.ColumnHeader inventoryQuantityHeader;
        private System.Windows.Forms.ColumnHeader inventoryDescriptionHeader;
    }
}

