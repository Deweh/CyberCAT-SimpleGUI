namespace CP2077SaveEditor.Views.Controls
{
    partial class PlayerStatsControl
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
            gb_Progression = new System.Windows.Forms.GroupBox();
            tlp_Progression = new System.Windows.Forms.TableLayoutPanel();
            tlp_Main = new System.Windows.Forms.TableLayoutPanel();
            gb_Attributes = new System.Windows.Forms.GroupBox();
            gb_Skills = new System.Windows.Forms.GroupBox();
            gb_LifePath = new System.Windows.Forms.GroupBox();
            additionalPlayerStatsButton = new ModernButton();
            soloUpDown = new System.Windows.Forms.NumericUpDown();
            shinobiUpDown = new System.Windows.Forms.NumericUpDown();
            netrunnerUpDown = new System.Windows.Forms.NumericUpDown();
            assassineUpDown = new System.Windows.Forms.NumericUpDown();
            perkPointsUpDown = new System.Windows.Forms.NumericUpDown();
            attrPointsUpDown = new System.Windows.Forms.NumericUpDown();
            lifePathBox = new System.Windows.Forms.ComboBox();
            lifePathPictureBox = new System.Windows.Forms.PictureBox();
            coolUpDown = new System.Windows.Forms.NumericUpDown();
            intelligenceUpDown = new System.Windows.Forms.NumericUpDown();
            technicalAbilityUpDown = new System.Windows.Forms.NumericUpDown();
            bodyUpDown = new System.Windows.Forms.NumericUpDown();
            reflexesUpDown = new System.Windows.Forms.NumericUpDown();
            streetCredUpDown = new System.Windows.Forms.NumericUpDown();
            levelUpDown = new System.Windows.Forms.NumericUpDown();
            techieUpDown = new System.Windows.Forms.NumericUpDown();
            relicUpDown = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            gb_Progression.SuspendLayout();
            tlp_Progression.SuspendLayout();
            tlp_Main.SuspendLayout();
            gb_Attributes.SuspendLayout();
            gb_Skills.SuspendLayout();
            gb_LifePath.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)soloUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)shinobiUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)netrunnerUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)assassineUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)perkPointsUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)attrPointsUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lifePathPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)coolUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)intelligenceUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)technicalAbilityUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bodyUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)reflexesUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)streetCredUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)levelUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)techieUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)relicUpDown).BeginInit();
            SuspendLayout();
            // 
            // gb_Progression
            // 
            gb_Progression.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            gb_Progression.Controls.Add(tlp_Progression);
            gb_Progression.Location = new System.Drawing.Point(12, 10);
            gb_Progression.Name = "gb_Progression";
            gb_Progression.Size = new System.Drawing.Size(827, 88);
            gb_Progression.TabIndex = 0;
            gb_Progression.TabStop = false;
            gb_Progression.Text = "Character Progression & Development Points";
            // 
            // tlp_Progression
            // 
            tlp_Progression.ColumnCount = 5;
            tlp_Progression.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tlp_Progression.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tlp_Progression.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tlp_Progression.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tlp_Progression.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tlp_Progression.Dock = System.Windows.Forms.DockStyle.Fill;
            tlp_Progression.Location = new System.Drawing.Point(3, 19);
            tlp_Progression.Name = "tlp_Progression";
            tlp_Progression.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            tlp_Progression.RowCount = 2;
            tlp_Progression.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            tlp_Progression.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tlp_Progression.Size = new System.Drawing.Size(821, 66);
            tlp_Progression.TabIndex = 0;
            tlp_Progression.Controls.Add(label14, 0, 0);
            tlp_Progression.Controls.Add(levelUpDown, 0, 1);
            tlp_Progression.Controls.Add(label15, 1, 0);
            tlp_Progression.Controls.Add(streetCredUpDown, 1, 1);
            tlp_Progression.Controls.Add(label11, 2, 0);
            tlp_Progression.Controls.Add(attrPointsUpDown, 2, 1);
            tlp_Progression.Controls.Add(label12, 3, 0);
            tlp_Progression.Controls.Add(perkPointsUpDown, 3, 1);
            tlp_Progression.Controls.Add(label13, 4, 0);
            tlp_Progression.Controls.Add(relicUpDown, 4, 1);
            // 
            // tlp_Main
            // 
            tlp_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tlp_Main.ColumnCount = 3;
            tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32F));
            tlp_Main.Controls.Add(gb_Attributes, 0, 0);
            tlp_Main.Controls.Add(gb_Skills, 1, 0);
            tlp_Main.Controls.Add(gb_LifePath, 2, 0);
            tlp_Main.Location = new System.Drawing.Point(8, 104);
            tlp_Main.Name = "tlp_Main";
            tlp_Main.RowCount = 1;
            tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tlp_Main.Size = new System.Drawing.Size(835, 432);
            tlp_Main.TabIndex = 1;
            // 
            // gb_Attributes
            // 
            gb_Attributes.Controls.Add(label7);
            gb_Attributes.Controls.Add(bodyUpDown);
            gb_Attributes.Controls.Add(label6);
            gb_Attributes.Controls.Add(reflexesUpDown);
            gb_Attributes.Controls.Add(label9);
            gb_Attributes.Controls.Add(technicalAbilityUpDown);
            gb_Attributes.Controls.Add(label8);
            gb_Attributes.Controls.Add(intelligenceUpDown);
            gb_Attributes.Controls.Add(label10);
            gb_Attributes.Controls.Add(coolUpDown);
            gb_Attributes.Dock = System.Windows.Forms.DockStyle.Fill;
            gb_Attributes.Location = new System.Drawing.Point(4, 4);
            gb_Attributes.Margin = new System.Windows.Forms.Padding(4);
            gb_Attributes.Name = "gb_Attributes";
            gb_Attributes.Size = new System.Drawing.Size(275, 424);
            gb_Attributes.TabIndex = 0;
            gb_Attributes.TabStop = false;
            gb_Attributes.Text = "Core Attributes (1 - 20)";
            // 
            // gb_Skills
            // 
            gb_Skills.Controls.Add(label1);
            gb_Skills.Controls.Add(assassineUpDown);
            gb_Skills.Controls.Add(label2);
            gb_Skills.Controls.Add(netrunnerUpDown);
            gb_Skills.Controls.Add(label3);
            gb_Skills.Controls.Add(shinobiUpDown);
            gb_Skills.Controls.Add(label4);
            gb_Skills.Controls.Add(soloUpDown);
            gb_Skills.Controls.Add(label5);
            gb_Skills.Controls.Add(techieUpDown);
            gb_Skills.Dock = System.Windows.Forms.DockStyle.Fill;
            gb_Skills.Location = new System.Drawing.Point(287, 4);
            gb_Skills.Margin = new System.Windows.Forms.Padding(4);
            gb_Skills.Name = "gb_Skills";
            gb_Skills.Size = new System.Drawing.Size(275, 424);
            gb_Skills.TabIndex = 1;
            gb_Skills.TabStop = false;
            gb_Skills.Text = "Skill Progression (1 - 60)";
            // 
            // gb_LifePath
            // 
            gb_LifePath.Controls.Add(lifePathBox);
            gb_LifePath.Controls.Add(lifePathPictureBox);
            gb_LifePath.Controls.Add(additionalPlayerStatsButton);
            gb_LifePath.Dock = System.Windows.Forms.DockStyle.Fill;
            gb_LifePath.Location = new System.Drawing.Point(570, 4);
            gb_LifePath.Margin = new System.Windows.Forms.Padding(4);
            gb_LifePath.Name = "gb_LifePath";
            gb_LifePath.Size = new System.Drawing.Size(261, 424);
            gb_LifePath.TabIndex = 2;
            gb_LifePath.TabStop = false;
            gb_LifePath.Text = "Life Path & Modifiers";
            // 
            // levelUpDown
            // 
            levelUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            levelUpDown.Location = new System.Drawing.Point(11, 29);
            levelUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            levelUpDown.Name = "levelUpDown";
            levelUpDown.Size = new System.Drawing.Size(96, 27);
            levelUpDown.TabIndex = 1;
            levelUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // streetCredUpDown
            // 
            streetCredUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            streetCredUpDown.Location = new System.Drawing.Point(172, 29);
            streetCredUpDown.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            streetCredUpDown.Name = "streetCredUpDown";
            streetCredUpDown.Size = new System.Drawing.Size(96, 27);
            streetCredUpDown.TabIndex = 2;
            streetCredUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // attrPointsUpDown
            // 
            attrPointsUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            attrPointsUpDown.Location = new System.Drawing.Point(333, 29);
            attrPointsUpDown.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            attrPointsUpDown.Name = "attrPointsUpDown";
            attrPointsUpDown.Size = new System.Drawing.Size(110, 27);
            attrPointsUpDown.TabIndex = 3;
            // 
            // perkPointsUpDown
            // 
            perkPointsUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            perkPointsUpDown.Location = new System.Drawing.Point(494, 29);
            perkPointsUpDown.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            perkPointsUpDown.Name = "perkPointsUpDown";
            perkPointsUpDown.Size = new System.Drawing.Size(110, 27);
            perkPointsUpDown.TabIndex = 4;
            // 
            // relicUpDown
            // 
            relicUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            relicUpDown.Location = new System.Drawing.Point(655, 29);
            relicUpDown.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            relicUpDown.Name = "relicUpDown";
            relicUpDown.Size = new System.Drawing.Size(110, 27);
            relicUpDown.TabIndex = 5;
            // 
            // bodyUpDown
            // 
            bodyUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            bodyUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            bodyUpDown.Location = new System.Drawing.Point(185, 36);
            bodyUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            bodyUpDown.Name = "bodyUpDown";
            bodyUpDown.Size = new System.Drawing.Size(74, 27);
            bodyUpDown.TabIndex = 6;
            bodyUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // reflexesUpDown
            // 
            reflexesUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            reflexesUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            reflexesUpDown.Location = new System.Drawing.Point(185, 86);
            reflexesUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            reflexesUpDown.Name = "reflexesUpDown";
            reflexesUpDown.Size = new System.Drawing.Size(74, 27);
            reflexesUpDown.TabIndex = 7;
            reflexesUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // technicalAbilityUpDown
            // 
            technicalAbilityUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            technicalAbilityUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            technicalAbilityUpDown.Location = new System.Drawing.Point(185, 136);
            technicalAbilityUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            technicalAbilityUpDown.Name = "technicalAbilityUpDown";
            technicalAbilityUpDown.Size = new System.Drawing.Size(74, 27);
            technicalAbilityUpDown.TabIndex = 8;
            technicalAbilityUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // intelligenceUpDown
            // 
            intelligenceUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            intelligenceUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            intelligenceUpDown.Location = new System.Drawing.Point(185, 186);
            intelligenceUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            intelligenceUpDown.Name = "intelligenceUpDown";
            intelligenceUpDown.Size = new System.Drawing.Size(74, 27);
            intelligenceUpDown.TabIndex = 9;
            intelligenceUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // coolUpDown
            // 
            coolUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            coolUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            coolUpDown.Location = new System.Drawing.Point(185, 236);
            coolUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            coolUpDown.Name = "coolUpDown";
            coolUpDown.Size = new System.Drawing.Size(74, 27);
            coolUpDown.TabIndex = 10;
            coolUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // assassineUpDown
            // 
            assassineUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            assassineUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            assassineUpDown.Location = new System.Drawing.Point(185, 36);
            assassineUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            assassineUpDown.Name = "assassineUpDown";
            assassineUpDown.Size = new System.Drawing.Size(74, 27);
            assassineUpDown.TabIndex = 11;
            // 
            // netrunnerUpDown
            // 
            netrunnerUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            netrunnerUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            netrunnerUpDown.Location = new System.Drawing.Point(185, 86);
            netrunnerUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            netrunnerUpDown.Name = "netrunnerUpDown";
            netrunnerUpDown.Size = new System.Drawing.Size(74, 27);
            netrunnerUpDown.TabIndex = 12;
            // 
            // shinobiUpDown
            // 
            shinobiUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            shinobiUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            shinobiUpDown.Location = new System.Drawing.Point(185, 136);
            shinobiUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            shinobiUpDown.Name = "shinobiUpDown";
            shinobiUpDown.Size = new System.Drawing.Size(74, 27);
            shinobiUpDown.TabIndex = 13;
            // 
            // soloUpDown
            // 
            soloUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            soloUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            soloUpDown.Location = new System.Drawing.Point(185, 186);
            soloUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            soloUpDown.Name = "soloUpDown";
            soloUpDown.Size = new System.Drawing.Size(74, 27);
            soloUpDown.TabIndex = 14;
            // 
            // techieUpDown
            // 
            techieUpDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            techieUpDown.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            techieUpDown.Location = new System.Drawing.Point(185, 236);
            techieUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            techieUpDown.Name = "techieUpDown";
            techieUpDown.Size = new System.Drawing.Size(74, 27);
            techieUpDown.TabIndex = 15;
            // 
            // lifePathBox
            // 
            lifePathBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lifePathBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            lifePathBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lifePathBox.FormattingEnabled = true;
            lifePathBox.Location = new System.Drawing.Point(14, 30);
            lifePathBox.Name = "lifePathBox";
            lifePathBox.Size = new System.Drawing.Size(233, 25);
            lifePathBox.TabIndex = 16;
            lifePathBox.SelectedIndexChanged += lifePathBox_SelectedIndexChanged;
            // 
            // lifePathPictureBox
            // 
            lifePathPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lifePathPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lifePathPictureBox.Location = new System.Drawing.Point(14, 65);
            lifePathPictureBox.Name = "lifePathPictureBox";
            lifePathPictureBox.Size = new System.Drawing.Size(233, 305);
            lifePathPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            lifePathPictureBox.TabIndex = 17;
            lifePathPictureBox.TabStop = false;
            // 
            // additionalPlayerStatsButton
            // 
            additionalPlayerStatsButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            additionalPlayerStatsButton.BackColor = System.Drawing.Color.LightGray;
            additionalPlayerStatsButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            additionalPlayerStatsButton.ClickEffectEnabled = true;
            additionalPlayerStatsButton.DefaultColor = System.Drawing.Color.LightGray;
            additionalPlayerStatsButton.HoverColor = System.Drawing.Color.Silver;
            additionalPlayerStatsButton.Location = new System.Drawing.Point(14, 380);
            additionalPlayerStatsButton.Name = "additionalPlayerStatsButton";
            additionalPlayerStatsButton.Size = new System.Drawing.Size(233, 32);
            additionalPlayerStatsButton.TabIndex = 18;
            additionalPlayerStatsButton.Text = "Edit Additional Stats...";
            additionalPlayerStatsButton.TextColor = System.Drawing.SystemColors.ControlText;
            additionalPlayerStatsButton.TextFont = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            additionalPlayerStatsButton.Click += additionalPlayerStatsButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(16, 40);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(83, 19);
            label1.TabIndex = 60;
            label1.Text = "Headhunter";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(16, 90);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(74, 19);
            label2.TabIndex = 61;
            label2.Text = "Netrunner";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(16, 140);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(53, 19);
            label3.TabIndex = 62;
            label3.Text = "Shinobi";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label4.Location = new System.Drawing.Point(16, 190);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(37, 19);
            label4.TabIndex = 63;
            label4.Text = "Solo";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(16, 240);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(61, 19);
            label5.TabIndex = 64;
            label5.Text = "Engineer";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(16, 90);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(57, 19);
            label6.TabIndex = 65;
            label6.Text = "Reflexes";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label7.Location = new System.Drawing.Point(16, 40);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(38, 19);
            label7.TabIndex = 66;
            label7.Text = "Body";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label8.Location = new System.Drawing.Point(16, 190);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(83, 19);
            label8.TabIndex = 67;
            label8.Text = "Intelligence";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label9.Location = new System.Drawing.Point(16, 140);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(113, 19);
            label9.TabIndex = 68;
            label9.Text = "Technical Ability";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(16, 240);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(39, 19);
            label10.TabIndex = 69;
            label10.Text = "Cool";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label11.Location = new System.Drawing.Point(333, 4);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(106, 17);
            label11.TabIndex = 70;
            label11.Text = "Attribute Points";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label12.Location = new System.Drawing.Point(494, 4);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(77, 17);
            label12.TabIndex = 71;
            label12.Text = "Perk Points";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label13.Location = new System.Drawing.Point(655, 4);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(80, 17);
            label13.TabIndex = 72;
            label13.Text = "Relic Points";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label14.Location = new System.Drawing.Point(11, 4);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(38, 17);
            label14.TabIndex = 73;
            label14.Text = "Level";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label15.Location = new System.Drawing.Point(172, 4);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(76, 17);
            label15.TabIndex = 74;
            label15.Text = "Street Cred";
            // 
            // PlayerStatsControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            Controls.Add(tlp_Main);
            Controls.Add(gb_Progression);
            Name = "PlayerStatsControl";
            Size = new System.Drawing.Size(851, 548);
            gb_Progression.ResumeLayout(false);
            tlp_Progression.ResumeLayout(false);
            tlp_Progression.PerformLayout();
            tlp_Main.ResumeLayout(false);
            gb_Attributes.ResumeLayout(false);
            gb_Attributes.PerformLayout();
            gb_Skills.ResumeLayout(false);
            gb_Skills.PerformLayout();
            gb_LifePath.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)soloUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)shinobiUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)netrunnerUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)assassineUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)perkPointsUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)attrPointsUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)lifePathPictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)coolUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)intelligenceUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)technicalAbilityUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)bodyUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)reflexesUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)streetCredUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)levelUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)techieUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)relicUpDown).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox gb_Progression;
        private System.Windows.Forms.TableLayoutPanel tlp_Progression;
        private System.Windows.Forms.TableLayoutPanel tlp_Main;
        private System.Windows.Forms.GroupBox gb_Attributes;
        private System.Windows.Forms.GroupBox gb_Skills;
        private System.Windows.Forms.GroupBox gb_LifePath;
        private ModernButton additionalPlayerStatsButton;
        private System.Windows.Forms.NumericUpDown techieUpDown;
        private System.Windows.Forms.NumericUpDown soloUpDown;
        private System.Windows.Forms.NumericUpDown shinobiUpDown;
        private System.Windows.Forms.NumericUpDown netrunnerUpDown;
        private System.Windows.Forms.NumericUpDown assassineUpDown;
        private System.Windows.Forms.NumericUpDown perkPointsUpDown;
        private System.Windows.Forms.NumericUpDown attrPointsUpDown;
        private System.Windows.Forms.ComboBox lifePathBox;
        private System.Windows.Forms.PictureBox lifePathPictureBox;
        private System.Windows.Forms.NumericUpDown coolUpDown;
        private System.Windows.Forms.NumericUpDown intelligenceUpDown;
        private System.Windows.Forms.NumericUpDown technicalAbilityUpDown;
        private System.Windows.Forms.NumericUpDown bodyUpDown;
        private System.Windows.Forms.NumericUpDown reflexesUpDown;
        private System.Windows.Forms.NumericUpDown streetCredUpDown;
        private System.Windows.Forms.NumericUpDown levelUpDown;
        private System.Windows.Forms.NumericUpDown relicUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
    }
}
