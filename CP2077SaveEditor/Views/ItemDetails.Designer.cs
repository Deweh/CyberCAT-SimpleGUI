
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
            addCurveStatButton = new ModernButton();
            addCombinedStatButton = new ModernButton();
            statsListView = new System.Windows.Forms.ListView();
            statTypeHeader = new System.Windows.Forms.ColumnHeader();
            statModifierHeader = new System.Windows.Forms.ColumnHeader();
            statNameHeader = new System.Windows.Forms.ColumnHeader();
            statValueHeader = new System.Windows.Forms.ColumnHeader();
            removeStatButton = new ModernButton();
            addConstantStatButton = new ModernButton();
            statsPlaceholderTab = new System.Windows.Forms.TabPage();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            createStatDataButton = new ModernButton();
            modInfoTab = new System.Windows.Forms.TabPage();
            newModNodeButton = new ModernButton();
            deleteModNodeButton = new ModernButton();
            closeButton = new ModernButton();
            applyButton = new ModernButton();
            basicInfoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)quantityUpDown).BeginInit();
            quickActionsGroupBox.SuspendLayout();
            flagsGroupBox.SuspendLayout();
            detailsTabControl.SuspendLayout();
            statsTab.SuspendLayout();
            statsPlaceholderTab.SuspendLayout();
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
            detailsTabControl.Controls.Add(modInfoTab);
            detailsTabControl.Location = new System.Drawing.Point(12, 201);
            detailsTabControl.Name = "detailsTabControl";
            detailsTabControl.SelectedIndex = 0;
            detailsTabControl.Size = new System.Drawing.Size(432, 356);
            detailsTabControl.TabIndex = 6;
            // 
            // statsTab
            // 
            statsTab.Controls.Add(addCurveStatButton);
            statsTab.Controls.Add(addCombinedStatButton);
            statsTab.Controls.Add(statsListView);
            statsTab.Controls.Add(removeStatButton);
            statsTab.Controls.Add(addConstantStatButton);
            statsTab.Location = new System.Drawing.Point(4, 22);
            statsTab.Name = "statsTab";
            statsTab.Padding = new System.Windows.Forms.Padding(3);
            statsTab.Size = new System.Drawing.Size(424, 330);
            statsTab.TabIndex = 1;
            statsTab.Text = "Stats";
            statsTab.UseVisualStyleBackColor = true;
            // 
            // addCurveStatButton
            // 
            addCurveStatButton.BackColor = System.Drawing.Color.White;
            addCurveStatButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            addCurveStatButton.ClickEffectEnabled = true;
            addCurveStatButton.DefaultColor = System.Drawing.Color.White;
            addCurveStatButton.HoverColor = System.Drawing.Color.LightGray;
            addCurveStatButton.Location = new System.Drawing.Point(90, 10);
            addCurveStatButton.Name = "addCurveStatButton";
            addCurveStatButton.Size = new System.Drawing.Size(81, 20);
            addCurveStatButton.TabIndex = 5;
            addCurveStatButton.Text = "+ New Curve";
            addCurveStatButton.TextColor = System.Drawing.SystemColors.ControlText;
            addCurveStatButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            addCurveStatButton.Click += addCurveStatButton_Click;
            // 
            // addCombinedStatButton
            // 
            addCombinedStatButton.BackColor = System.Drawing.Color.White;
            addCombinedStatButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            addCombinedStatButton.ClickEffectEnabled = true;
            addCombinedStatButton.DefaultColor = System.Drawing.Color.White;
            addCombinedStatButton.HoverColor = System.Drawing.Color.LightGray;
            addCombinedStatButton.Location = new System.Drawing.Point(177, 10);
            addCombinedStatButton.Name = "addCombinedStatButton";
            addCombinedStatButton.Size = new System.Drawing.Size(107, 20);
            addCombinedStatButton.TabIndex = 4;
            addCombinedStatButton.Text = "+ New Combined";
            addCombinedStatButton.TextColor = System.Drawing.SystemColors.ControlText;
            addCombinedStatButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            addCombinedStatButton.Click += addCombinedStatButton_Click;
            // 
            // statsListView
            // 
            statsListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            statsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { statTypeHeader, statModifierHeader, statNameHeader, statValueHeader });
            statsListView.FullRowSelect = true;
            statsListView.Location = new System.Drawing.Point(18, 36);
            statsListView.MultiSelect = false;
            statsListView.Name = "statsListView";
            statsListView.Size = new System.Drawing.Size(379, 283);
            statsListView.TabIndex = 5;
            statsListView.UseCompatibleStateImageBehavior = false;
            statsListView.View = System.Windows.Forms.View.Details;
            // 
            // statTypeHeader
            // 
            statTypeHeader.Name = "statTypeHeader";
            statTypeHeader.Text = "Type";
            statTypeHeader.Width = 77;
            // 
            // statModifierHeader
            // 
            statModifierHeader.DisplayIndex = 2;
            statModifierHeader.Name = "statModifierHeader";
            statModifierHeader.Text = "Modifier";
            statModifierHeader.Width = 87;
            // 
            // statNameHeader
            // 
            statNameHeader.DisplayIndex = 1;
            statNameHeader.Name = "statNameHeader";
            statNameHeader.Text = "Stat";
            statNameHeader.Width = 131;
            // 
            // statValueHeader
            // 
            statValueHeader.Name = "statValueHeader";
            statValueHeader.Text = "Value";
            // 
            // removeStatButton
            // 
            removeStatButton.BackColor = System.Drawing.Color.White;
            removeStatButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            removeStatButton.ClickEffectEnabled = true;
            removeStatButton.DefaultColor = System.Drawing.Color.White;
            removeStatButton.HoverColor = System.Drawing.Color.LightGray;
            removeStatButton.Location = new System.Drawing.Point(18, 10);
            removeStatButton.Name = "removeStatButton";
            removeStatButton.Size = new System.Drawing.Size(66, 20);
            removeStatButton.TabIndex = 4;
            removeStatButton.Text = "- Delete";
            removeStatButton.TextColor = System.Drawing.SystemColors.ControlText;
            removeStatButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            removeStatButton.Click += removeStatButton_Click;
            // 
            // addConstantStatButton
            // 
            addConstantStatButton.BackColor = System.Drawing.Color.White;
            addConstantStatButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            addConstantStatButton.ClickEffectEnabled = true;
            addConstantStatButton.DefaultColor = System.Drawing.Color.White;
            addConstantStatButton.HoverColor = System.Drawing.Color.LightGray;
            addConstantStatButton.Location = new System.Drawing.Point(290, 10);
            addConstantStatButton.Name = "addConstantStatButton";
            addConstantStatButton.Size = new System.Drawing.Size(107, 20);
            addConstantStatButton.TabIndex = 3;
            addConstantStatButton.Text = "+ New Constant";
            addConstantStatButton.TextColor = System.Drawing.SystemColors.ControlText;
            addConstantStatButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            addConstantStatButton.Click += addConstantStatButton_Click;
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
            // modInfoTab
            // 
            modInfoTab.Controls.Add(newModNodeButton);
            modInfoTab.Controls.Add(deleteModNodeButton);
            modInfoTab.Controls.Add(modsTreeView);
            modInfoTab.Location = new System.Drawing.Point(4, 22);
            modInfoTab.Name = "modInfoTab";
            modInfoTab.Padding = new System.Windows.Forms.Padding(3);
            modInfoTab.Size = new System.Drawing.Size(424, 330);
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
        private ModernButton addConstantStatButton;
        private ModernButton removeStatButton;
        private System.Windows.Forms.ListView statsListView;
        private System.Windows.Forms.ColumnHeader statTypeHeader;
        private System.Windows.Forms.ColumnHeader statModifierHeader;
        private System.Windows.Forms.ColumnHeader statNameHeader;
        private System.Windows.Forms.ColumnHeader statValueHeader;
        private ModernButton addCurveStatButton;
        private ModernButton addCombinedStatButton;
        private ModernButton newModNodeButton;
        private ModernButton deleteModNodeButton;
        private ModernButton infuseLegendaryComponentsButton;
        private System.Windows.Forms.TabPage statsPlaceholderTab;
        private System.Windows.Forms.Label label3;
        private ModernButton createStatDataButton;
        private System.Windows.Forms.Label label4;
    }
}