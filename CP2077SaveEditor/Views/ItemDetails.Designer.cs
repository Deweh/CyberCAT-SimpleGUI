
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
            basicInfoGroupBox = new System.Windows.Forms.GroupBox();
            quantityUpDown = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            modsTreeView = new System.Windows.Forms.TreeView();
            quickActionsGroupBox = new System.Windows.Forms.GroupBox();
            infuseLegendaryComponentsButton = new ModernButton();
            pasteLegendaryIdButton = new ModernButton();
            questItemCheckBox = new System.Windows.Forms.CheckBox();
            flagsGroupBox = new System.Windows.Forms.GroupBox();
            unknownFlag1CheckBox = new System.Windows.Forms.CheckBox();
            detailsTabControl = new System.Windows.Forms.TabControl();
            statsTab = new System.Windows.Forms.TabPage();
            statsPlaceholderTab = new System.Windows.Forms.TabPage();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            createStatDataButton = new ModernButton();
            additionalInfoTab = new System.Windows.Forms.TabPage();
            btn_MaxLevel = new ModernButton();
            unknown3Box = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            unknown1Box = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            txt_LootItemName = new System.Windows.Forms.TextBox();
            lbl_LootItemName = new System.Windows.Forms.Label();
            lbl_LootItemId = new System.Windows.Forms.Label();
            txt_LootItemId = new System.Windows.Forms.TextBox();
            modInfoTab = new System.Windows.Forms.TabPage();
            newModNodeButton = new ModernButton();
            deleteModNodeButton = new ModernButton();
            closeButton = new ModernButton();
            applyButton = new ModernButton();
            statsControl1 = new Views.Controls.StatsControl();
            basicInfoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)quantityUpDown).BeginInit();
            quickActionsGroupBox.SuspendLayout();
            flagsGroupBox.SuspendLayout();
            detailsTabControl.SuspendLayout();
            statsTab.SuspendLayout();
            statsPlaceholderTab.SuspendLayout();
            additionalInfoTab.SuspendLayout();
            modInfoTab.SuspendLayout();
            SuspendLayout();
            // 
            // basicInfoGroupBox
            // 
            basicInfoGroupBox.Controls.Add(quantityUpDown);
            basicInfoGroupBox.Controls.Add(label1);
            basicInfoGroupBox.Location = new System.Drawing.Point(12, 12);
            basicInfoGroupBox.Name = "basicInfoGroupBox";
            basicInfoGroupBox.Size = new System.Drawing.Size(432, 64);
            basicInfoGroupBox.TabIndex = 0;
            basicInfoGroupBox.TabStop = false;
            basicInfoGroupBox.Text = "Basic Info";
            // 
            // quantityUpDown
            // 
            quantityUpDown.Location = new System.Drawing.Point(83, 27);
            quantityUpDown.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            quantityUpDown.Name = "quantityUpDown";
            quantityUpDown.Size = new System.Drawing.Size(322, 22);
            quantityUpDown.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(23, 29);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(54, 13);
            label1.TabIndex = 0;
            label1.Text = "Quantity:";
            // 
            // modsTreeView
            // 
            modsTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            modsTreeView.Location = new System.Drawing.Point(18, 32);
            modsTreeView.Name = "modsTreeView";
            modsTreeView.Size = new System.Drawing.Size(379, 287);
            modsTreeView.TabIndex = 0;
            // 
            // quickActionsGroupBox
            // 
            quickActionsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            quickActionsGroupBox.Controls.Add(infuseLegendaryComponentsButton);
            quickActionsGroupBox.Controls.Add(pasteLegendaryIdButton);
            quickActionsGroupBox.Location = new System.Drawing.Point(12, 138);
            quickActionsGroupBox.Name = "quickActionsGroupBox";
            quickActionsGroupBox.Size = new System.Drawing.Size(432, 57);
            quickActionsGroupBox.TabIndex = 4;
            quickActionsGroupBox.TabStop = false;
            quickActionsGroupBox.Text = "Quick Actions";
            // 
            // infuseLegendaryComponentsButton
            // 
            infuseLegendaryComponentsButton.BackColor = System.Drawing.Color.White;
            infuseLegendaryComponentsButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            infuseLegendaryComponentsButton.ClickEffectEnabled = true;
            infuseLegendaryComponentsButton.DefaultColor = System.Drawing.Color.White;
            infuseLegendaryComponentsButton.HoverColor = System.Drawing.Color.LightGray;
            infuseLegendaryComponentsButton.Location = new System.Drawing.Point(212, 21);
            infuseLegendaryComponentsButton.Name = "infuseLegendaryComponentsButton";
            infuseLegendaryComponentsButton.Size = new System.Drawing.Size(214, 24);
            infuseLegendaryComponentsButton.TabIndex = 3;
            infuseLegendaryComponentsButton.Text = "Infuse Legendary Components";
            infuseLegendaryComponentsButton.TextColor = System.Drawing.SystemColors.ControlText;
            infuseLegendaryComponentsButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            infuseLegendaryComponentsButton.Click += infuseLegendaryComponentsButton_Click;
            // 
            // pasteLegendaryIdButton
            // 
            pasteLegendaryIdButton.BackColor = System.Drawing.Color.White;
            pasteLegendaryIdButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pasteLegendaryIdButton.ClickEffectEnabled = true;
            pasteLegendaryIdButton.DefaultColor = System.Drawing.Color.White;
            pasteLegendaryIdButton.HoverColor = System.Drawing.Color.LightGray;
            pasteLegendaryIdButton.Location = new System.Drawing.Point(6, 21);
            pasteLegendaryIdButton.Name = "pasteLegendaryIdButton";
            pasteLegendaryIdButton.Size = new System.Drawing.Size(200, 24);
            pasteLegendaryIdButton.TabIndex = 2;
            pasteLegendaryIdButton.Text = "Make Legendary";
            pasteLegendaryIdButton.TextColor = System.Drawing.SystemColors.ControlText;
            pasteLegendaryIdButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            pasteLegendaryIdButton.Click += pasteLegendaryIdButton_Click;
            // 
            // questItemCheckBox
            // 
            questItemCheckBox.AutoSize = true;
            questItemCheckBox.Location = new System.Drawing.Point(263, 21);
            questItemCheckBox.Name = "questItemCheckBox";
            questItemCheckBox.Size = new System.Drawing.Size(92, 17);
            questItemCheckBox.TabIndex = 3;
            questItemCheckBox.Text = "Is Quest Item";
            questItemCheckBox.UseVisualStyleBackColor = true;
            // 
            // flagsGroupBox
            // 
            flagsGroupBox.Controls.Add(unknownFlag1CheckBox);
            flagsGroupBox.Controls.Add(questItemCheckBox);
            flagsGroupBox.Location = new System.Drawing.Point(12, 82);
            flagsGroupBox.Name = "flagsGroupBox";
            flagsGroupBox.Size = new System.Drawing.Size(432, 50);
            flagsGroupBox.TabIndex = 5;
            flagsGroupBox.TabStop = false;
            flagsGroupBox.Text = "Flags";
            // 
            // unknownFlag1CheckBox
            // 
            unknownFlag1CheckBox.AutoSize = true;
            unknownFlag1CheckBox.Location = new System.Drawing.Point(73, 21);
            unknownFlag1CheckBox.Name = "unknownFlag1CheckBox";
            unknownFlag1CheckBox.Size = new System.Drawing.Size(133, 17);
            unknownFlag1CheckBox.TabIndex = 4;
            unknownFlag1CheckBox.Text = "Is Not Unequippable";
            unknownFlag1CheckBox.UseVisualStyleBackColor = true;
            // 
            // detailsTabControl
            // 
            detailsTabControl.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            detailsTabControl.Controls.Add(statsTab);
            detailsTabControl.Controls.Add(statsPlaceholderTab);
            detailsTabControl.Controls.Add(additionalInfoTab);
            detailsTabControl.Controls.Add(modInfoTab);
            detailsTabControl.Location = new System.Drawing.Point(12, 201);
            detailsTabControl.Name = "detailsTabControl";
            detailsTabControl.SelectedIndex = 0;
            detailsTabControl.Size = new System.Drawing.Size(432, 356);
            detailsTabControl.TabIndex = 6;
            // 
            // statsTab
            // 
            statsTab.Controls.Add(statsControl1);
            statsTab.Location = new System.Drawing.Point(4, 22);
            statsTab.Name = "statsTab";
            statsTab.Padding = new System.Windows.Forms.Padding(3);
            statsTab.Size = new System.Drawing.Size(424, 330);
            statsTab.TabIndex = 1;
            statsTab.Text = "Stats";
            statsTab.UseVisualStyleBackColor = true;
            // 
            // statsPlaceholderTab
            // 
            statsPlaceholderTab.Controls.Add(label4);
            statsPlaceholderTab.Controls.Add(label3);
            statsPlaceholderTab.Controls.Add(createStatDataButton);
            statsPlaceholderTab.Location = new System.Drawing.Point(4, 24);
            statsPlaceholderTab.Name = "statsPlaceholderTab";
            statsPlaceholderTab.Size = new System.Drawing.Size(424, 328);
            statsPlaceholderTab.TabIndex = 2;
            statsPlaceholderTab.Text = "Stats";
            statsPlaceholderTab.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(31, 186);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(362, 26);
            label4.TabIndex = 2;
            label4.Text = "Note: This feature is in beta and has only been tested on clothes and\r\nweapons. Using it on other items may cause corruption.";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(43, 90);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(335, 13);
            label3.TabIndex = 1;
            label3.Text = "This item has no stat data, but a new entry can be created for it.";
            // 
            // createStatDataButton
            // 
            createStatDataButton.BackColor = System.Drawing.Color.White;
            createStatDataButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            createStatDataButton.ClickEffectEnabled = true;
            createStatDataButton.DefaultColor = System.Drawing.Color.White;
            createStatDataButton.HoverColor = System.Drawing.Color.LightGray;
            createStatDataButton.Location = new System.Drawing.Point(109, 129);
            createStatDataButton.Name = "createStatDataButton";
            createStatDataButton.Size = new System.Drawing.Size(200, 32);
            createStatDataButton.TabIndex = 0;
            createStatDataButton.Text = "Create Stat Entry";
            createStatDataButton.TextColor = System.Drawing.SystemColors.ControlText;
            createStatDataButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            createStatDataButton.Click += createStatDataButton_Click;
            // 
            // additionalInfoTab
            // 
            additionalInfoTab.Controls.Add(btn_MaxLevel);
            additionalInfoTab.Controls.Add(unknown3Box);
            additionalInfoTab.Controls.Add(label8);
            additionalInfoTab.Controls.Add(unknown1Box);
            additionalInfoTab.Controls.Add(label5);
            additionalInfoTab.Controls.Add(txt_LootItemName);
            additionalInfoTab.Controls.Add(lbl_LootItemName);
            additionalInfoTab.Controls.Add(lbl_LootItemId);
            additionalInfoTab.Controls.Add(txt_LootItemId);
            additionalInfoTab.Location = new System.Drawing.Point(4, 24);
            additionalInfoTab.Name = "additionalInfoTab";
            additionalInfoTab.Padding = new System.Windows.Forms.Padding(3);
            additionalInfoTab.Size = new System.Drawing.Size(424, 328);
            additionalInfoTab.TabIndex = 3;
            additionalInfoTab.Text = "Additional Info";
            additionalInfoTab.UseVisualStyleBackColor = true;
            // 
            // btn_MaxLevel
            // 
            btn_MaxLevel.BackColor = System.Drawing.Color.White;
            btn_MaxLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_MaxLevel.ClickEffectEnabled = true;
            btn_MaxLevel.DefaultColor = System.Drawing.Color.White;
            btn_MaxLevel.HoverColor = System.Drawing.Color.LightGray;
            btn_MaxLevel.Location = new System.Drawing.Point(373, 90);
            btn_MaxLevel.Name = "btn_MaxLevel";
            btn_MaxLevel.Size = new System.Drawing.Size(45, 22);
            btn_MaxLevel.TabIndex = 36;
            btn_MaxLevel.Text = "None";
            btn_MaxLevel.TextColor = System.Drawing.SystemColors.ControlText;
            btn_MaxLevel.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btn_MaxLevel.Click += btn_MaxLevel_Click;
            // 
            // unknown3Box
            // 
            unknown3Box.Location = new System.Drawing.Point(130, 90);
            unknown3Box.Name = "unknown3Box";
            unknown3Box.Size = new System.Drawing.Size(237, 22);
            unknown3Box.TabIndex = 40;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(42, 93);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(82, 13);
            label8.TabIndex = 39;
            label8.Text = "RequiredLevel:";
            // 
            // unknown1Box
            // 
            unknown1Box.Location = new System.Drawing.Point(130, 62);
            unknown1Box.Name = "unknown1Box";
            unknown1Box.Size = new System.Drawing.Size(288, 22);
            unknown1Box.TabIndex = 38;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(11, 65);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(113, 13);
            label5.TabIndex = 37;
            label5.Text = "Unknown 1 (UInt32):";
            // 
            // txt_LootItemName
            // 
            txt_LootItemName.Location = new System.Drawing.Point(130, 6);
            txt_LootItemName.Name = "txt_LootItemName";
            txt_LootItemName.Size = new System.Drawing.Size(288, 22);
            txt_LootItemName.TabIndex = 35;
            txt_LootItemName.TextChanged += txt_LootItemName_TextChanged;
            // 
            // lbl_LootItemName
            // 
            lbl_LootItemName.AutoSize = true;
            lbl_LootItemName.Location = new System.Drawing.Point(37, 9);
            lbl_LootItemName.Name = "lbl_LootItemName";
            lbl_LootItemName.Size = new System.Drawing.Size(87, 13);
            lbl_LootItemName.TabIndex = 34;
            lbl_LootItemName.Text = "LootItem Name:";
            // 
            // lbl_LootItemId
            // 
            lbl_LootItemId.AutoSize = true;
            lbl_LootItemId.Location = new System.Drawing.Point(55, 37);
            lbl_LootItemId.Name = "lbl_LootItemId";
            lbl_LootItemId.Size = new System.Drawing.Size(69, 13);
            lbl_LootItemId.TabIndex = 33;
            lbl_LootItemId.Text = "LootItem ID:";
            // 
            // txt_LootItemId
            // 
            txt_LootItemId.Location = new System.Drawing.Point(130, 34);
            txt_LootItemId.Name = "txt_LootItemId";
            txt_LootItemId.Size = new System.Drawing.Size(288, 22);
            txt_LootItemId.TabIndex = 32;
            txt_LootItemId.TextChanged += txt_LootItemId_TextChanged;
            // 
            // modInfoTab
            // 
            modInfoTab.Controls.Add(newModNodeButton);
            modInfoTab.Controls.Add(deleteModNodeButton);
            modInfoTab.Controls.Add(modsTreeView);
            modInfoTab.Location = new System.Drawing.Point(4, 24);
            modInfoTab.Name = "modInfoTab";
            modInfoTab.Padding = new System.Windows.Forms.Padding(3);
            modInfoTab.Size = new System.Drawing.Size(424, 328);
            modInfoTab.TabIndex = 0;
            modInfoTab.Text = "Mods/Special";
            modInfoTab.UseVisualStyleBackColor = true;
            // 
            // newModNodeButton
            // 
            newModNodeButton.BackColor = System.Drawing.Color.White;
            newModNodeButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            newModNodeButton.ClickEffectEnabled = true;
            newModNodeButton.DefaultColor = System.Drawing.Color.White;
            newModNodeButton.HoverColor = System.Drawing.Color.LightGray;
            newModNodeButton.Location = new System.Drawing.Point(305, 6);
            newModNodeButton.Name = "newModNodeButton";
            newModNodeButton.Size = new System.Drawing.Size(92, 20);
            newModNodeButton.TabIndex = 7;
            newModNodeButton.Text = "+ New Node";
            newModNodeButton.TextColor = System.Drawing.SystemColors.ControlText;
            newModNodeButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            newModNodeButton.Click += newModNodeButton_Click;
            // 
            // deleteModNodeButton
            // 
            deleteModNodeButton.BackColor = System.Drawing.Color.White;
            deleteModNodeButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            deleteModNodeButton.ClickEffectEnabled = true;
            deleteModNodeButton.DefaultColor = System.Drawing.Color.White;
            deleteModNodeButton.HoverColor = System.Drawing.Color.LightGray;
            deleteModNodeButton.Location = new System.Drawing.Point(18, 6);
            deleteModNodeButton.Name = "deleteModNodeButton";
            deleteModNodeButton.Size = new System.Drawing.Size(93, 20);
            deleteModNodeButton.TabIndex = 6;
            deleteModNodeButton.Text = "- Delete Node";
            deleteModNodeButton.TextColor = System.Drawing.SystemColors.ControlText;
            deleteModNodeButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            deleteModNodeButton.Click += deleteModNodeButton_Click;
            // 
            // closeButton
            // 
            closeButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            closeButton.BackColor = System.Drawing.Color.White;
            closeButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            closeButton.ClickEffectEnabled = true;
            closeButton.DefaultColor = System.Drawing.Color.White;
            closeButton.HoverColor = System.Drawing.Color.LightGray;
            closeButton.Location = new System.Drawing.Point(340, 565);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(104, 26);
            closeButton.TabIndex = 3;
            closeButton.Text = "Close";
            closeButton.TextColor = System.Drawing.SystemColors.ControlText;
            closeButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            closeButton.Click += closeButton_Click;
            // 
            // applyButton
            // 
            applyButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            applyButton.BackColor = System.Drawing.Color.White;
            applyButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            applyButton.ClickEffectEnabled = true;
            applyButton.DefaultColor = System.Drawing.Color.White;
            applyButton.HoverColor = System.Drawing.Color.LightGray;
            applyButton.Location = new System.Drawing.Point(230, 565);
            applyButton.Name = "applyButton";
            applyButton.Size = new System.Drawing.Size(104, 26);
            applyButton.TabIndex = 2;
            applyButton.Text = "Apply";
            applyButton.TextColor = System.Drawing.SystemColors.ControlText;
            applyButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            applyButton.Click += applyButton_Click;
            // 
            // statsControl1
            // 
            statsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            statsControl1.Location = new System.Drawing.Point(3, 3);
            statsControl1.Name = "statsControl1";
            statsControl1.Size = new System.Drawing.Size(418, 324);
            statsControl1.TabIndex = 0;
            // 
            // ItemDetails
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(456, 603);
            Controls.Add(detailsTabControl);
            Controls.Add(flagsGroupBox);
            Controls.Add(quickActionsGroupBox);
            Controls.Add(closeButton);
            Controls.Add(applyButton);
            Controls.Add(basicInfoGroupBox);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "ItemDetails";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "ItemDetails";
            basicInfoGroupBox.ResumeLayout(false);
            basicInfoGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)quantityUpDown).EndInit();
            quickActionsGroupBox.ResumeLayout(false);
            flagsGroupBox.ResumeLayout(false);
            flagsGroupBox.PerformLayout();
            detailsTabControl.ResumeLayout(false);
            statsTab.ResumeLayout(false);
            statsPlaceholderTab.ResumeLayout(false);
            statsPlaceholderTab.PerformLayout();
            additionalInfoTab.ResumeLayout(false);
            additionalInfoTab.PerformLayout();
            modInfoTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox basicInfoGroupBox;
        private System.Windows.Forms.NumericUpDown quantityUpDown;
        private System.Windows.Forms.Label label1;
        private ModernButton pasteLegendaryIdButton;
        private ModernButton applyButton;
        private ModernButton closeButton;
        private System.Windows.Forms.GroupBox quickActionsGroupBox;
        private System.Windows.Forms.TreeView modsTreeView;
        private System.Windows.Forms.CheckBox questItemCheckBox;
        private System.Windows.Forms.GroupBox flagsGroupBox;
        private System.Windows.Forms.CheckBox unknownFlag1CheckBox;
        private System.Windows.Forms.TabControl detailsTabControl;
        private System.Windows.Forms.TabPage modInfoTab;
        private System.Windows.Forms.TabPage statsTab;
        private ModernButton newModNodeButton;
        private ModernButton deleteModNodeButton;
        private ModernButton infuseLegendaryComponentsButton;
        private System.Windows.Forms.TabPage statsPlaceholderTab;
        private System.Windows.Forms.Label label3;
        private ModernButton createStatDataButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage additionalInfoTab;
        private System.Windows.Forms.TextBox txt_LootItemName;
        private System.Windows.Forms.Label lbl_LootItemName;
        private System.Windows.Forms.Label lbl_LootItemId;
        private System.Windows.Forms.TextBox txt_LootItemId;
        private ModernButton btn_MaxLevel;
        private System.Windows.Forms.TextBox unknown3Box;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox unknown1Box;
        private System.Windows.Forms.Label label5;
        private Views.Controls.StatsControl statsControl1;
    }
}