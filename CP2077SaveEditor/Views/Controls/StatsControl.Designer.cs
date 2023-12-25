namespace CP2077SaveEditor.Views.Controls
{
    partial class StatsControl
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
            lv_Modifiers = new System.Windows.Forms.ListView();
            statTypeHeader = new System.Windows.Forms.ColumnHeader();
            statModifierHeader = new System.Windows.Forms.ColumnHeader();
            statNameHeader = new System.Windows.Forms.ColumnHeader();
            statValueHeader = new System.Windows.Forms.ColumnHeader();
            tabControl1 = new System.Windows.Forms.TabControl();
            tab_ModifiersBuffer = new System.Windows.Forms.TabPage();
            pnl_ModifiersHide = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.Label();
            btn_ModifiersCreate = new ModernButton();
            label5 = new System.Windows.Forms.Label();
            pnl_ModifiersMenu = new System.Windows.Forms.Panel();
            btn_ModifierDelete = new ModernButton();
            btn_ModifierAddCurve = new ModernButton();
            btn_ModifierAddConstant = new ModernButton();
            btn_ModifierAddCombined = new ModernButton();
            tab_ForcedModifiersBuffer = new System.Windows.Forms.TabPage();
            pnl_ForcedModifiersHide = new System.Windows.Forms.Panel();
            label2 = new System.Windows.Forms.Label();
            btn_ForcedModifiersCreate = new ModernButton();
            label1 = new System.Windows.Forms.Label();
            lv_ForcedModifiers = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            pnl_ForcedModifiersMenu = new System.Windows.Forms.Panel();
            btn_ForcedModifierDelete = new ModernButton();
            btn_ForcedModifierAddCurve = new ModernButton();
            btn_ForcedModifierAddConstant = new ModernButton();
            btn_ForcedModifierAddCombined = new ModernButton();
            tab_InactiveStats = new System.Windows.Forms.TabPage();
            pnl_InactiveStatsHide = new System.Windows.Forms.Panel();
            label3 = new System.Windows.Forms.Label();
            btn_InactiveStatsCreate = new ModernButton();
            label4 = new System.Windows.Forms.Label();
            lv_InactiveStats = new System.Windows.Forms.ListView();
            col_StatType = new System.Windows.Forms.ColumnHeader();
            pnl_InactiveStatsMenu = new System.Windows.Forms.Panel();
            cmb_StatType = new System.Windows.Forms.ComboBox();
            btn_InactiveStatsDelete = new ModernButton();
            btn_InactiveStatsAdd = new ModernButton();
            tab_ModifierGroup = new System.Windows.Forms.TabPage();
            pnl_ModifierGroupHide = new System.Windows.Forms.Panel();
            label7 = new System.Windows.Forms.Label();
            btn_ModifierGroupCreate = new ModernButton();
            label8 = new System.Windows.Forms.Label();
            tv_ModifierGroups = new System.Windows.Forms.TreeView();
            pnl_ModifierGroupMenu = new System.Windows.Forms.Panel();
            btn_ModifierGroupDelete = new ModernButton();
            btn_ModifierGroupAddNode = new ModernButton();
            btn_ModifierGroupAddStat = new ModernButton();
            cmb_ModifierGroupNodeType = new System.Windows.Forms.ComboBox();
            cmb_ModifierGroupStatType = new System.Windows.Forms.ComboBox();
            tabControl1.SuspendLayout();
            tab_ModifiersBuffer.SuspendLayout();
            pnl_ModifiersHide.SuspendLayout();
            pnl_ModifiersMenu.SuspendLayout();
            tab_ForcedModifiersBuffer.SuspendLayout();
            pnl_ForcedModifiersHide.SuspendLayout();
            pnl_ForcedModifiersMenu.SuspendLayout();
            tab_InactiveStats.SuspendLayout();
            pnl_InactiveStatsHide.SuspendLayout();
            pnl_InactiveStatsMenu.SuspendLayout();
            tab_ModifierGroup.SuspendLayout();
            pnl_ModifierGroupHide.SuspendLayout();
            pnl_ModifierGroupMenu.SuspendLayout();
            SuspendLayout();
            // 
            // lv_Modifiers
            // 
            lv_Modifiers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lv_Modifiers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { statTypeHeader, statModifierHeader, statNameHeader, statValueHeader });
            lv_Modifiers.Dock = System.Windows.Forms.DockStyle.Fill;
            lv_Modifiers.FullRowSelect = true;
            lv_Modifiers.Location = new System.Drawing.Point(3, 31);
            lv_Modifiers.MultiSelect = false;
            lv_Modifiers.Name = "lv_Modifiers";
            lv_Modifiers.Size = new System.Drawing.Size(538, 340);
            lv_Modifiers.TabIndex = 10;
            lv_Modifiers.UseCompatibleStateImageBehavior = false;
            lv_Modifiers.View = System.Windows.Forms.View.Details;
            lv_Modifiers.DoubleClick += lv_Modifiers_DoubleClick;
            lv_Modifiers.KeyDown += lv_Modifiers_KeyDown;
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
            // tabControl1
            // 
            tabControl1.Controls.Add(tab_ModifiersBuffer);
            tabControl1.Controls.Add(tab_ForcedModifiersBuffer);
            tabControl1.Controls.Add(tab_InactiveStats);
            tabControl1.Controls.Add(tab_ModifierGroup);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(552, 402);
            tabControl1.TabIndex = 11;
            // 
            // tab_ModifiersBuffer
            // 
            tab_ModifiersBuffer.Controls.Add(pnl_ModifiersHide);
            tab_ModifiersBuffer.Controls.Add(lv_Modifiers);
            tab_ModifiersBuffer.Controls.Add(pnl_ModifiersMenu);
            tab_ModifiersBuffer.Location = new System.Drawing.Point(4, 24);
            tab_ModifiersBuffer.Name = "tab_ModifiersBuffer";
            tab_ModifiersBuffer.Padding = new System.Windows.Forms.Padding(3);
            tab_ModifiersBuffer.Size = new System.Drawing.Size(544, 374);
            tab_ModifiersBuffer.TabIndex = 0;
            tab_ModifiersBuffer.Text = "Modifiers";
            tab_ModifiersBuffer.UseVisualStyleBackColor = true;
            // 
            // pnl_ModifiersHide
            // 
            pnl_ModifiersHide.Controls.Add(label6);
            pnl_ModifiersHide.Controls.Add(btn_ModifiersCreate);
            pnl_ModifiersHide.Controls.Add(label5);
            pnl_ModifiersHide.Dock = System.Windows.Forms.DockStyle.Fill;
            pnl_ModifiersHide.Location = new System.Drawing.Point(3, 31);
            pnl_ModifiersHide.Name = "pnl_ModifiersHide";
            pnl_ModifiersHide.Size = new System.Drawing.Size(538, 340);
            pnl_ModifiersHide.TabIndex = 10;
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label6.Location = new System.Drawing.Point(3, 85);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(532, 15);
            label6.TabIndex = 10;
            label6.Text = "This item has no modifiers, but a new entry can be created for it.";
            label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_ModifiersCreate
            // 
            btn_ModifiersCreate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btn_ModifiersCreate.BackColor = System.Drawing.Color.White;
            btn_ModifiersCreate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifiersCreate.ClickEffectEnabled = true;
            btn_ModifiersCreate.DefaultColor = System.Drawing.Color.White;
            btn_ModifiersCreate.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifiersCreate.Location = new System.Drawing.Point(169, 122);
            btn_ModifiersCreate.Name = "btn_ModifiersCreate";
            btn_ModifiersCreate.Size = new System.Drawing.Size(200, 32);
            btn_ModifiersCreate.TabIndex = 9;
            btn_ModifiersCreate.Text = "Create Modifiers";
            btn_ModifiersCreate.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifiersCreate.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifiersCreate.Click += btn_ModifiersCreate_Click;
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label5.Location = new System.Drawing.Point(3, 172);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(532, 15);
            label5.TabIndex = 11;
            label5.Text = "Note: This feature is in beta. Using it may cause corruption.";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnl_ModifiersMenu
            // 
            pnl_ModifiersMenu.Controls.Add(btn_ModifierDelete);
            pnl_ModifiersMenu.Controls.Add(btn_ModifierAddCurve);
            pnl_ModifiersMenu.Controls.Add(btn_ModifierAddConstant);
            pnl_ModifiersMenu.Controls.Add(btn_ModifierAddCombined);
            pnl_ModifiersMenu.Dock = System.Windows.Forms.DockStyle.Top;
            pnl_ModifiersMenu.Enabled = false;
            pnl_ModifiersMenu.Location = new System.Drawing.Point(3, 3);
            pnl_ModifiersMenu.Name = "pnl_ModifiersMenu";
            pnl_ModifiersMenu.Size = new System.Drawing.Size(538, 28);
            pnl_ModifiersMenu.TabIndex = 13;
            // 
            // btn_ModifierDelete
            // 
            btn_ModifierDelete.BackColor = System.Drawing.Color.White;
            btn_ModifierDelete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifierDelete.ClickEffectEnabled = true;
            btn_ModifierDelete.DefaultColor = System.Drawing.Color.White;
            btn_ModifierDelete.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifierDelete.Location = new System.Drawing.Point(3, 3);
            btn_ModifierDelete.Name = "btn_ModifierDelete";
            btn_ModifierDelete.Size = new System.Drawing.Size(66, 20);
            btn_ModifierDelete.TabIndex = 8;
            btn_ModifierDelete.Text = "- Delete";
            btn_ModifierDelete.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifierDelete.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifierDelete.Click += btn_ModifierDelete_Click;
            // 
            // btn_ModifierAddCurve
            // 
            btn_ModifierAddCurve.BackColor = System.Drawing.Color.White;
            btn_ModifierAddCurve.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifierAddCurve.ClickEffectEnabled = true;
            btn_ModifierAddCurve.DefaultColor = System.Drawing.Color.White;
            btn_ModifierAddCurve.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifierAddCurve.Location = new System.Drawing.Point(75, 3);
            btn_ModifierAddCurve.Name = "btn_ModifierAddCurve";
            btn_ModifierAddCurve.Size = new System.Drawing.Size(81, 20);
            btn_ModifierAddCurve.TabIndex = 9;
            btn_ModifierAddCurve.Text = "+ New Curve";
            btn_ModifierAddCurve.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifierAddCurve.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifierAddCurve.Click += btn_ModifierAddCurve_Click;
            // 
            // btn_ModifierAddConstant
            // 
            btn_ModifierAddConstant.BackColor = System.Drawing.Color.White;
            btn_ModifierAddConstant.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifierAddConstant.ClickEffectEnabled = true;
            btn_ModifierAddConstant.DefaultColor = System.Drawing.Color.White;
            btn_ModifierAddConstant.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifierAddConstant.Location = new System.Drawing.Point(275, 3);
            btn_ModifierAddConstant.Name = "btn_ModifierAddConstant";
            btn_ModifierAddConstant.Size = new System.Drawing.Size(107, 20);
            btn_ModifierAddConstant.TabIndex = 6;
            btn_ModifierAddConstant.Text = "+ New Constant";
            btn_ModifierAddConstant.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifierAddConstant.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifierAddConstant.Click += btn_ModifierAddConstant_Click;
            // 
            // btn_ModifierAddCombined
            // 
            btn_ModifierAddCombined.BackColor = System.Drawing.Color.White;
            btn_ModifierAddCombined.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifierAddCombined.ClickEffectEnabled = true;
            btn_ModifierAddCombined.DefaultColor = System.Drawing.Color.White;
            btn_ModifierAddCombined.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifierAddCombined.Location = new System.Drawing.Point(162, 3);
            btn_ModifierAddCombined.Name = "btn_ModifierAddCombined";
            btn_ModifierAddCombined.Size = new System.Drawing.Size(107, 20);
            btn_ModifierAddCombined.TabIndex = 7;
            btn_ModifierAddCombined.Text = "+ New Combined";
            btn_ModifierAddCombined.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifierAddCombined.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifierAddCombined.Click += btn_ModifierAddCombined_Click;
            // 
            // tab_ForcedModifiersBuffer
            // 
            tab_ForcedModifiersBuffer.Controls.Add(pnl_ForcedModifiersHide);
            tab_ForcedModifiersBuffer.Controls.Add(lv_ForcedModifiers);
            tab_ForcedModifiersBuffer.Controls.Add(pnl_ForcedModifiersMenu);
            tab_ForcedModifiersBuffer.Location = new System.Drawing.Point(4, 24);
            tab_ForcedModifiersBuffer.Name = "tab_ForcedModifiersBuffer";
            tab_ForcedModifiersBuffer.Padding = new System.Windows.Forms.Padding(3);
            tab_ForcedModifiersBuffer.Size = new System.Drawing.Size(544, 374);
            tab_ForcedModifiersBuffer.TabIndex = 1;
            tab_ForcedModifiersBuffer.Text = "Forced Modifiers";
            tab_ForcedModifiersBuffer.UseVisualStyleBackColor = true;
            // 
            // pnl_ForcedModifiersHide
            // 
            pnl_ForcedModifiersHide.Controls.Add(label2);
            pnl_ForcedModifiersHide.Controls.Add(btn_ForcedModifiersCreate);
            pnl_ForcedModifiersHide.Controls.Add(label1);
            pnl_ForcedModifiersHide.Dock = System.Windows.Forms.DockStyle.Fill;
            pnl_ForcedModifiersHide.Location = new System.Drawing.Point(3, 31);
            pnl_ForcedModifiersHide.Name = "pnl_ForcedModifiersHide";
            pnl_ForcedModifiersHide.Size = new System.Drawing.Size(538, 340);
            pnl_ForcedModifiersHide.TabIndex = 10;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label2.Location = new System.Drawing.Point(3, 85);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(532, 15);
            label2.TabIndex = 7;
            label2.Text = "This item has no forced modifiers, but a new entry can be created for it.";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_ForcedModifiersCreate
            // 
            btn_ForcedModifiersCreate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btn_ForcedModifiersCreate.BackColor = System.Drawing.Color.White;
            btn_ForcedModifiersCreate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ForcedModifiersCreate.ClickEffectEnabled = true;
            btn_ForcedModifiersCreate.DefaultColor = System.Drawing.Color.White;
            btn_ForcedModifiersCreate.HoverColor = System.Drawing.Color.LightGray;
            btn_ForcedModifiersCreate.Location = new System.Drawing.Point(169, 122);
            btn_ForcedModifiersCreate.Name = "btn_ForcedModifiersCreate";
            btn_ForcedModifiersCreate.Size = new System.Drawing.Size(200, 32);
            btn_ForcedModifiersCreate.TabIndex = 6;
            btn_ForcedModifiersCreate.Text = "Create Forced Modifiers";
            btn_ForcedModifiersCreate.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ForcedModifiersCreate.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ForcedModifiersCreate.Click += btn_ForcedModifiersCreate_Click;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label1.Location = new System.Drawing.Point(3, 172);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(532, 15);
            label1.TabIndex = 8;
            label1.Text = "Note: This feature is in beta. Using it may cause corruption.";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lv_ForcedModifiers
            // 
            lv_ForcedModifiers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lv_ForcedModifiers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            lv_ForcedModifiers.Dock = System.Windows.Forms.DockStyle.Fill;
            lv_ForcedModifiers.FullRowSelect = true;
            lv_ForcedModifiers.Location = new System.Drawing.Point(3, 31);
            lv_ForcedModifiers.MultiSelect = false;
            lv_ForcedModifiers.Name = "lv_ForcedModifiers";
            lv_ForcedModifiers.Size = new System.Drawing.Size(538, 340);
            lv_ForcedModifiers.TabIndex = 11;
            lv_ForcedModifiers.UseCompatibleStateImageBehavior = false;
            lv_ForcedModifiers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Name = "statTypeHeader";
            columnHeader1.Text = "Type";
            columnHeader1.Width = 77;
            // 
            // columnHeader2
            // 
            columnHeader2.DisplayIndex = 2;
            columnHeader2.Name = "statModifierHeader";
            columnHeader2.Text = "Modifier";
            columnHeader2.Width = 87;
            // 
            // columnHeader3
            // 
            columnHeader3.DisplayIndex = 1;
            columnHeader3.Name = "statNameHeader";
            columnHeader3.Text = "Stat";
            columnHeader3.Width = 131;
            // 
            // columnHeader4
            // 
            columnHeader4.Name = "statValueHeader";
            columnHeader4.Text = "Value";
            // 
            // pnl_ForcedModifiersMenu
            // 
            pnl_ForcedModifiersMenu.Controls.Add(btn_ForcedModifierDelete);
            pnl_ForcedModifiersMenu.Controls.Add(btn_ForcedModifierAddCurve);
            pnl_ForcedModifiersMenu.Controls.Add(btn_ForcedModifierAddConstant);
            pnl_ForcedModifiersMenu.Controls.Add(btn_ForcedModifierAddCombined);
            pnl_ForcedModifiersMenu.Dock = System.Windows.Forms.DockStyle.Top;
            pnl_ForcedModifiersMenu.Enabled = false;
            pnl_ForcedModifiersMenu.Location = new System.Drawing.Point(3, 3);
            pnl_ForcedModifiersMenu.Name = "pnl_ForcedModifiersMenu";
            pnl_ForcedModifiersMenu.Size = new System.Drawing.Size(538, 28);
            pnl_ForcedModifiersMenu.TabIndex = 13;
            // 
            // btn_ForcedModifierDelete
            // 
            btn_ForcedModifierDelete.BackColor = System.Drawing.Color.White;
            btn_ForcedModifierDelete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ForcedModifierDelete.ClickEffectEnabled = true;
            btn_ForcedModifierDelete.DefaultColor = System.Drawing.Color.White;
            btn_ForcedModifierDelete.HoverColor = System.Drawing.Color.LightGray;
            btn_ForcedModifierDelete.Location = new System.Drawing.Point(3, 3);
            btn_ForcedModifierDelete.Name = "btn_ForcedModifierDelete";
            btn_ForcedModifierDelete.Size = new System.Drawing.Size(66, 20);
            btn_ForcedModifierDelete.TabIndex = 8;
            btn_ForcedModifierDelete.Text = "- Delete";
            btn_ForcedModifierDelete.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ForcedModifierDelete.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ForcedModifierDelete.Click += btn_ForcedModifierDelete_Click;
            // 
            // btn_ForcedModifierAddCurve
            // 
            btn_ForcedModifierAddCurve.BackColor = System.Drawing.Color.White;
            btn_ForcedModifierAddCurve.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ForcedModifierAddCurve.ClickEffectEnabled = true;
            btn_ForcedModifierAddCurve.DefaultColor = System.Drawing.Color.White;
            btn_ForcedModifierAddCurve.HoverColor = System.Drawing.Color.LightGray;
            btn_ForcedModifierAddCurve.Location = new System.Drawing.Point(75, 3);
            btn_ForcedModifierAddCurve.Name = "btn_ForcedModifierAddCurve";
            btn_ForcedModifierAddCurve.Size = new System.Drawing.Size(81, 20);
            btn_ForcedModifierAddCurve.TabIndex = 9;
            btn_ForcedModifierAddCurve.Text = "+ New Curve";
            btn_ForcedModifierAddCurve.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ForcedModifierAddCurve.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ForcedModifierAddCurve.Click += btn_ForcedModifierAddCurve_Click;
            // 
            // btn_ForcedModifierAddConstant
            // 
            btn_ForcedModifierAddConstant.BackColor = System.Drawing.Color.White;
            btn_ForcedModifierAddConstant.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ForcedModifierAddConstant.ClickEffectEnabled = true;
            btn_ForcedModifierAddConstant.DefaultColor = System.Drawing.Color.White;
            btn_ForcedModifierAddConstant.HoverColor = System.Drawing.Color.LightGray;
            btn_ForcedModifierAddConstant.Location = new System.Drawing.Point(275, 3);
            btn_ForcedModifierAddConstant.Name = "btn_ForcedModifierAddConstant";
            btn_ForcedModifierAddConstant.Size = new System.Drawing.Size(107, 20);
            btn_ForcedModifierAddConstant.TabIndex = 6;
            btn_ForcedModifierAddConstant.Text = "+ New Constant";
            btn_ForcedModifierAddConstant.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ForcedModifierAddConstant.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ForcedModifierAddConstant.Click += btn_ForcedModifierAddConstant_Click;
            // 
            // btn_ForcedModifierAddCombined
            // 
            btn_ForcedModifierAddCombined.BackColor = System.Drawing.Color.White;
            btn_ForcedModifierAddCombined.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ForcedModifierAddCombined.ClickEffectEnabled = true;
            btn_ForcedModifierAddCombined.DefaultColor = System.Drawing.Color.White;
            btn_ForcedModifierAddCombined.HoverColor = System.Drawing.Color.LightGray;
            btn_ForcedModifierAddCombined.Location = new System.Drawing.Point(162, 3);
            btn_ForcedModifierAddCombined.Name = "btn_ForcedModifierAddCombined";
            btn_ForcedModifierAddCombined.Size = new System.Drawing.Size(107, 20);
            btn_ForcedModifierAddCombined.TabIndex = 7;
            btn_ForcedModifierAddCombined.Text = "+ New Combined";
            btn_ForcedModifierAddCombined.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ForcedModifierAddCombined.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ForcedModifierAddCombined.Click += btn_ForcedModifierAddCombined_Click;
            // 
            // tab_InactiveStats
            // 
            tab_InactiveStats.Controls.Add(pnl_InactiveStatsHide);
            tab_InactiveStats.Controls.Add(lv_InactiveStats);
            tab_InactiveStats.Controls.Add(pnl_InactiveStatsMenu);
            tab_InactiveStats.Location = new System.Drawing.Point(4, 24);
            tab_InactiveStats.Name = "tab_InactiveStats";
            tab_InactiveStats.Padding = new System.Windows.Forms.Padding(3);
            tab_InactiveStats.Size = new System.Drawing.Size(544, 374);
            tab_InactiveStats.TabIndex = 2;
            tab_InactiveStats.Text = "Inactive Stats";
            tab_InactiveStats.UseVisualStyleBackColor = true;
            // 
            // pnl_InactiveStatsHide
            // 
            pnl_InactiveStatsHide.Controls.Add(label3);
            pnl_InactiveStatsHide.Controls.Add(btn_InactiveStatsCreate);
            pnl_InactiveStatsHide.Controls.Add(label4);
            pnl_InactiveStatsHide.Dock = System.Windows.Forms.DockStyle.Fill;
            pnl_InactiveStatsHide.Location = new System.Drawing.Point(3, 31);
            pnl_InactiveStatsHide.Name = "pnl_InactiveStatsHide";
            pnl_InactiveStatsHide.Size = new System.Drawing.Size(538, 340);
            pnl_InactiveStatsHide.TabIndex = 10;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label3.Location = new System.Drawing.Point(3, 85);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(532, 15);
            label3.TabIndex = 4;
            label3.Text = "This item has no inactive stats, but a new entry can be created for it.";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_InactiveStatsCreate
            // 
            btn_InactiveStatsCreate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btn_InactiveStatsCreate.BackColor = System.Drawing.Color.White;
            btn_InactiveStatsCreate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_InactiveStatsCreate.ClickEffectEnabled = true;
            btn_InactiveStatsCreate.DefaultColor = System.Drawing.Color.White;
            btn_InactiveStatsCreate.HoverColor = System.Drawing.Color.LightGray;
            btn_InactiveStatsCreate.Location = new System.Drawing.Point(169, 122);
            btn_InactiveStatsCreate.Name = "btn_InactiveStatsCreate";
            btn_InactiveStatsCreate.Size = new System.Drawing.Size(200, 32);
            btn_InactiveStatsCreate.TabIndex = 3;
            btn_InactiveStatsCreate.Text = "Create Inactive Stats";
            btn_InactiveStatsCreate.TextColor = System.Drawing.SystemColors.ControlText;
            btn_InactiveStatsCreate.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_InactiveStatsCreate.Click += btn_InactiveStatsCreate_Click;
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label4.Location = new System.Drawing.Point(3, 172);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(532, 15);
            label4.TabIndex = 5;
            label4.Text = "Note: This feature is in beta. Using it may cause corruption.";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lv_InactiveStats
            // 
            lv_InactiveStats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lv_InactiveStats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { col_StatType });
            lv_InactiveStats.Dock = System.Windows.Forms.DockStyle.Fill;
            lv_InactiveStats.FullRowSelect = true;
            lv_InactiveStats.Location = new System.Drawing.Point(3, 31);
            lv_InactiveStats.MultiSelect = false;
            lv_InactiveStats.Name = "lv_InactiveStats";
            lv_InactiveStats.Size = new System.Drawing.Size(538, 340);
            lv_InactiveStats.TabIndex = 11;
            lv_InactiveStats.UseCompatibleStateImageBehavior = false;
            lv_InactiveStats.View = System.Windows.Forms.View.Details;
            // 
            // col_StatType
            // 
            col_StatType.Text = "Stat Type";
            col_StatType.Width = 200;
            // 
            // pnl_InactiveStatsMenu
            // 
            pnl_InactiveStatsMenu.Controls.Add(cmb_StatType);
            pnl_InactiveStatsMenu.Controls.Add(btn_InactiveStatsDelete);
            pnl_InactiveStatsMenu.Controls.Add(btn_InactiveStatsAdd);
            pnl_InactiveStatsMenu.Dock = System.Windows.Forms.DockStyle.Top;
            pnl_InactiveStatsMenu.Enabled = false;
            pnl_InactiveStatsMenu.Location = new System.Drawing.Point(3, 3);
            pnl_InactiveStatsMenu.Name = "pnl_InactiveStatsMenu";
            pnl_InactiveStatsMenu.Size = new System.Drawing.Size(538, 28);
            pnl_InactiveStatsMenu.TabIndex = 14;
            // 
            // cmb_StatType
            // 
            cmb_StatType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            cmb_StatType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            cmb_StatType.FormattingEnabled = true;
            cmb_StatType.Location = new System.Drawing.Point(304, 2);
            cmb_StatType.Name = "cmb_StatType";
            cmb_StatType.Size = new System.Drawing.Size(231, 23);
            cmb_StatType.TabIndex = 19;
            // 
            // btn_InactiveStatsDelete
            // 
            btn_InactiveStatsDelete.BackColor = System.Drawing.Color.White;
            btn_InactiveStatsDelete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_InactiveStatsDelete.ClickEffectEnabled = true;
            btn_InactiveStatsDelete.DefaultColor = System.Drawing.Color.White;
            btn_InactiveStatsDelete.HoverColor = System.Drawing.Color.LightGray;
            btn_InactiveStatsDelete.Location = new System.Drawing.Point(3, 3);
            btn_InactiveStatsDelete.Name = "btn_InactiveStatsDelete";
            btn_InactiveStatsDelete.Size = new System.Drawing.Size(66, 20);
            btn_InactiveStatsDelete.TabIndex = 8;
            btn_InactiveStatsDelete.Text = "- Delete";
            btn_InactiveStatsDelete.TextColor = System.Drawing.SystemColors.ControlText;
            btn_InactiveStatsDelete.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_InactiveStatsDelete.Click += btn_InactiveStatsDelete_Click;
            // 
            // btn_InactiveStatsAdd
            // 
            btn_InactiveStatsAdd.BackColor = System.Drawing.Color.White;
            btn_InactiveStatsAdd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_InactiveStatsAdd.ClickEffectEnabled = true;
            btn_InactiveStatsAdd.DefaultColor = System.Drawing.Color.White;
            btn_InactiveStatsAdd.HoverColor = System.Drawing.Color.LightGray;
            btn_InactiveStatsAdd.Location = new System.Drawing.Point(217, 3);
            btn_InactiveStatsAdd.Name = "btn_InactiveStatsAdd";
            btn_InactiveStatsAdd.Size = new System.Drawing.Size(81, 20);
            btn_InactiveStatsAdd.TabIndex = 9;
            btn_InactiveStatsAdd.Text = "+ New Stat";
            btn_InactiveStatsAdd.TextColor = System.Drawing.SystemColors.ControlText;
            btn_InactiveStatsAdd.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_InactiveStatsAdd.Click += btn_InactiveStatsAdd_Click;
            // 
            // tab_ModifierGroup
            // 
            tab_ModifierGroup.Controls.Add(pnl_ModifierGroupHide);
            tab_ModifierGroup.Controls.Add(tv_ModifierGroups);
            tab_ModifierGroup.Controls.Add(pnl_ModifierGroupMenu);
            tab_ModifierGroup.Location = new System.Drawing.Point(4, 24);
            tab_ModifierGroup.Name = "tab_ModifierGroup";
            tab_ModifierGroup.Padding = new System.Windows.Forms.Padding(3);
            tab_ModifierGroup.Size = new System.Drawing.Size(544, 374);
            tab_ModifierGroup.TabIndex = 3;
            tab_ModifierGroup.Text = "Modifier Groups";
            tab_ModifierGroup.UseVisualStyleBackColor = true;
            // 
            // pnl_ModifierGroupHide
            // 
            pnl_ModifierGroupHide.Controls.Add(label7);
            pnl_ModifierGroupHide.Controls.Add(btn_ModifierGroupCreate);
            pnl_ModifierGroupHide.Controls.Add(label8);
            pnl_ModifierGroupHide.Dock = System.Windows.Forms.DockStyle.Fill;
            pnl_ModifierGroupHide.Location = new System.Drawing.Point(3, 59);
            pnl_ModifierGroupHide.Name = "pnl_ModifierGroupHide";
            pnl_ModifierGroupHide.Size = new System.Drawing.Size(538, 312);
            pnl_ModifierGroupHide.TabIndex = 15;
            // 
            // label7
            // 
            label7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label7.Location = new System.Drawing.Point(3, 85);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(532, 15);
            label7.TabIndex = 4;
            label7.Text = "This item has no modifier groups, but a new entry can be created for it.";
            label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_ModifierGroupCreate
            // 
            btn_ModifierGroupCreate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btn_ModifierGroupCreate.BackColor = System.Drawing.Color.White;
            btn_ModifierGroupCreate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifierGroupCreate.ClickEffectEnabled = true;
            btn_ModifierGroupCreate.DefaultColor = System.Drawing.Color.White;
            btn_ModifierGroupCreate.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifierGroupCreate.Location = new System.Drawing.Point(169, 122);
            btn_ModifierGroupCreate.Name = "btn_ModifierGroupCreate";
            btn_ModifierGroupCreate.Size = new System.Drawing.Size(200, 32);
            btn_ModifierGroupCreate.TabIndex = 3;
            btn_ModifierGroupCreate.Text = "Create Modifier Group";
            btn_ModifierGroupCreate.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifierGroupCreate.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifierGroupCreate.Click += btn_ModifierGroupCreate_Click;
            // 
            // label8
            // 
            label8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label8.Location = new System.Drawing.Point(3, 172);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(532, 15);
            label8.TabIndex = 5;
            label8.Text = "Note: This feature is in beta. Using it may cause corruption.";
            label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tv_ModifierGroups
            // 
            tv_ModifierGroups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tv_ModifierGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            tv_ModifierGroups.Location = new System.Drawing.Point(3, 59);
            tv_ModifierGroups.Name = "tv_ModifierGroups";
            tv_ModifierGroups.Size = new System.Drawing.Size(538, 312);
            tv_ModifierGroups.TabIndex = 1;
            // 
            // pnl_ModifierGroupMenu
            // 
            pnl_ModifierGroupMenu.Controls.Add(btn_ModifierGroupDelete);
            pnl_ModifierGroupMenu.Controls.Add(btn_ModifierGroupAddNode);
            pnl_ModifierGroupMenu.Controls.Add(cmb_ModifierGroupNodeType);
            pnl_ModifierGroupMenu.Controls.Add(btn_ModifierGroupAddStat);
            pnl_ModifierGroupMenu.Controls.Add(cmb_ModifierGroupStatType);
            pnl_ModifierGroupMenu.Dock = System.Windows.Forms.DockStyle.Top;
            pnl_ModifierGroupMenu.Enabled = false;
            pnl_ModifierGroupMenu.Location = new System.Drawing.Point(3, 3);
            pnl_ModifierGroupMenu.Name = "pnl_ModifierGroupMenu";
            pnl_ModifierGroupMenu.Size = new System.Drawing.Size(538, 56);
            pnl_ModifierGroupMenu.TabIndex = 14;
            // 
            // btn_ModifierGroupDelete
            // 
            btn_ModifierGroupDelete.BackColor = System.Drawing.Color.White;
            btn_ModifierGroupDelete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifierGroupDelete.ClickEffectEnabled = true;
            btn_ModifierGroupDelete.DefaultColor = System.Drawing.Color.White;
            btn_ModifierGroupDelete.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifierGroupDelete.Location = new System.Drawing.Point(3, 3);
            btn_ModifierGroupDelete.Name = "btn_ModifierGroupDelete";
            btn_ModifierGroupDelete.Size = new System.Drawing.Size(93, 20);
            btn_ModifierGroupDelete.TabIndex = 8;
            btn_ModifierGroupDelete.Text = "- Delete Node";
            btn_ModifierGroupDelete.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifierGroupDelete.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifierGroupDelete.Click += btn_ModifierGroupDelete_Click;
            // 
            // btn_ModifierGroupAddNode
            // 
            btn_ModifierGroupAddNode.BackColor = System.Drawing.Color.White;
            btn_ModifierGroupAddNode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifierGroupAddNode.ClickEffectEnabled = true;
            btn_ModifierGroupAddNode.DefaultColor = System.Drawing.Color.White;
            btn_ModifierGroupAddNode.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifierGroupAddNode.Location = new System.Drawing.Point(206, 3);
            btn_ModifierGroupAddNode.Name = "btn_ModifierGroupAddNode";
            btn_ModifierGroupAddNode.Size = new System.Drawing.Size(92, 20);
            btn_ModifierGroupAddNode.TabIndex = 9;
            btn_ModifierGroupAddNode.Text = "+ New Node";
            btn_ModifierGroupAddNode.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifierGroupAddNode.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifierGroupAddNode.Click += btn_ModifierGroupAddNode_Click;
            // 
            // btn_ModifierGroupAddStat
            // 
            btn_ModifierGroupAddStat.BackColor = System.Drawing.Color.White;
            btn_ModifierGroupAddStat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            btn_ModifierGroupAddStat.ClickEffectEnabled = true;
            btn_ModifierGroupAddStat.DefaultColor = System.Drawing.Color.White;
            btn_ModifierGroupAddStat.HoverColor = System.Drawing.Color.LightGray;
            btn_ModifierGroupAddStat.Location = new System.Drawing.Point(206, 30);
            btn_ModifierGroupAddStat.Name = "btn_ModifierGroupAddStat";
            btn_ModifierGroupAddStat.Size = new System.Drawing.Size(92, 20);
            btn_ModifierGroupAddStat.TabIndex = 10;
            btn_ModifierGroupAddStat.Text = "+ New Stat";
            btn_ModifierGroupAddStat.TextColor = System.Drawing.SystemColors.ControlText;
            btn_ModifierGroupAddStat.TextFont = new System.Drawing.Font("Segoe UI", 8.25F);
            btn_ModifierGroupAddStat.Click += btn_ModifierGroupAddStat_Click;
            // 
            // cmb_ModifierGroupNodeType
            // 
            cmb_ModifierGroupNodeType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            cmb_ModifierGroupNodeType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            cmb_ModifierGroupNodeType.FormattingEnabled = true;
            cmb_ModifierGroupNodeType.Location = new System.Drawing.Point(304, 2);
            cmb_ModifierGroupNodeType.Name = "cmb_ModifierGroupNodeType";
            cmb_ModifierGroupNodeType.Size = new System.Drawing.Size(231, 23);
            cmb_ModifierGroupNodeType.TabIndex = 21;
            // 
            // cmb_ModifierGroupStatType
            // 
            cmb_ModifierGroupStatType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            cmb_ModifierGroupStatType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            cmb_ModifierGroupStatType.FormattingEnabled = true;
            cmb_ModifierGroupStatType.Location = new System.Drawing.Point(304, 29);
            cmb_ModifierGroupStatType.Name = "cmb_ModifierGroupStatType";
            cmb_ModifierGroupStatType.Size = new System.Drawing.Size(231, 23);
            cmb_ModifierGroupStatType.TabIndex = 22;
            // 
            // StatsControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Name = "StatsControl";
            Size = new System.Drawing.Size(552, 402);
            tabControl1.ResumeLayout(false);
            tab_ModifiersBuffer.ResumeLayout(false);
            pnl_ModifiersHide.ResumeLayout(false);
            pnl_ModifiersMenu.ResumeLayout(false);
            tab_ForcedModifiersBuffer.ResumeLayout(false);
            pnl_ForcedModifiersHide.ResumeLayout(false);
            pnl_ForcedModifiersMenu.ResumeLayout(false);
            tab_InactiveStats.ResumeLayout(false);
            pnl_InactiveStatsHide.ResumeLayout(false);
            pnl_InactiveStatsMenu.ResumeLayout(false);
            tab_ModifierGroup.ResumeLayout(false);
            pnl_ModifierGroupHide.ResumeLayout(false);
            pnl_ModifierGroupMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.ListView lv_Modifiers;
        private System.Windows.Forms.ColumnHeader statTypeHeader;
        private System.Windows.Forms.ColumnHeader statModifierHeader;
        private System.Windows.Forms.ColumnHeader statNameHeader;
        private System.Windows.Forms.ColumnHeader statValueHeader;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_ModifiersBuffer;
        private System.Windows.Forms.TabPage tab_ForcedModifiersBuffer;
        private System.Windows.Forms.TabPage tab_InactiveStats;
        private System.Windows.Forms.ListView lv_ForcedModifiers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ListView lv_InactiveStats;
        private System.Windows.Forms.Panel pnl_ForcedModifiersMenu;
        private ModernButton btn_ForcedModifierDelete;
        private ModernButton btn_ForcedModifierAddCurve;
        private ModernButton btn_ForcedModifierAddConstant;
        private ModernButton btn_ForcedModifierAddCombined;
        private System.Windows.Forms.Panel pnl_ModifiersMenu;
        private ModernButton btn_ModifierDelete;
        private ModernButton btn_ModifierAddCurve;
        private ModernButton btn_ModifierAddConstant;
        private ModernButton btn_ModifierAddCombined;
        private System.Windows.Forms.Panel pnl_ModifiersHide;
        private System.Windows.Forms.Panel pnl_ForcedModifiersHide;
        private System.Windows.Forms.Panel pnl_InactiveStatsMenu;
        private ModernButton btn_InactiveStatsDelete;
        private ModernButton btn_InactiveStatsAdd;
        private System.Windows.Forms.ColumnHeader col_StatType;
        private System.Windows.Forms.Panel pnl_InactiveStatsHide;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private ModernButton btn_InactiveStatsCreate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ModernButton btn_ForcedModifiersCreate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private ModernButton btn_ModifiersCreate;
        private System.Windows.Forms.ComboBox cmb_StatType;
        private System.Windows.Forms.TabPage tab_ModifierGroup;
        private System.Windows.Forms.TreeView tv_ModifierGroups;
        private System.Windows.Forms.Panel pnl_ModifierGroupMenu;
        private ModernButton btn_ModifierGroupAddNode;
        private ModernButton btn_ModifierGroupDelete;
        private ModernButton btn_ModifierGroupAddStat;
        private System.Windows.Forms.Panel pnl_ModifierGroupHide;
        private System.Windows.Forms.Label label7;
        private ModernButton btn_ModifierGroupCreate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox cmb_ModifierGroupNodeType;
        private System.Windows.Forms.ComboBox cmb_ModifierGroupStatType;
    }
}
