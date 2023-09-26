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
            SuspendLayout();
            // 
            // additionalPlayerStatsButton
            // 
            additionalPlayerStatsButton.BackColor = System.Drawing.Color.LightGray;
            additionalPlayerStatsButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            additionalPlayerStatsButton.ClickEffectEnabled = true;
            additionalPlayerStatsButton.DefaultColor = System.Drawing.Color.LightGray;
            additionalPlayerStatsButton.HoverColor = System.Drawing.Color.Silver;
            additionalPlayerStatsButton.Location = new System.Drawing.Point(366, 411);
            additionalPlayerStatsButton.Name = "additionalPlayerStatsButton";
            additionalPlayerStatsButton.Size = new System.Drawing.Size(124, 24);
            additionalPlayerStatsButton.TabIndex = 58;
            additionalPlayerStatsButton.Text = "Edit Additional Stats";
            additionalPlayerStatsButton.TextColor = System.Drawing.SystemColors.ControlText;
            additionalPlayerStatsButton.TextFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            additionalPlayerStatsButton.Click += additionalPlayerStatsButton_Click;
            // 
            // soloUpDown
            // 
            soloUpDown.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            soloUpDown.Location = new System.Drawing.Point(704, 253);
            soloUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            soloUpDown.Name = "soloUpDown";
            soloUpDown.Size = new System.Drawing.Size(31, 19);
            soloUpDown.TabIndex = 50;
            // 
            // shinobiUpDown
            // 
            shinobiUpDown.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            shinobiUpDown.Location = new System.Drawing.Point(704, 225);
            shinobiUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            shinobiUpDown.Name = "shinobiUpDown";
            shinobiUpDown.Size = new System.Drawing.Size(31, 19);
            shinobiUpDown.TabIndex = 49;
            // 
            // netrunnerUpDown
            // 
            netrunnerUpDown.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            netrunnerUpDown.Location = new System.Drawing.Point(704, 198);
            netrunnerUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            netrunnerUpDown.Name = "netrunnerUpDown";
            netrunnerUpDown.Size = new System.Drawing.Size(31, 19);
            netrunnerUpDown.TabIndex = 47;
            // 
            // assassineUpDown
            // 
            assassineUpDown.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            assassineUpDown.Location = new System.Drawing.Point(704, 169);
            assassineUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            assassineUpDown.Name = "assassineUpDown";
            assassineUpDown.Size = new System.Drawing.Size(31, 19);
            assassineUpDown.TabIndex = 46;
            // 
            // perkPointsUpDown
            // 
            perkPointsUpDown.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            perkPointsUpDown.Location = new System.Drawing.Point(44, 310);
            perkPointsUpDown.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            perkPointsUpDown.Name = "perkPointsUpDown";
            perkPointsUpDown.Size = new System.Drawing.Size(45, 25);
            perkPointsUpDown.TabIndex = 45;
            // 
            // attrPointsUpDown
            // 
            attrPointsUpDown.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            attrPointsUpDown.Location = new System.Drawing.Point(44, 266);
            attrPointsUpDown.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
            attrPointsUpDown.Name = "attrPointsUpDown";
            attrPointsUpDown.Size = new System.Drawing.Size(45, 25);
            attrPointsUpDown.TabIndex = 44;
            // 
            // lifePathBox
            // 
            lifePathBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            lifePathBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lifePathBox.FormattingEnabled = true;
            lifePathBox.Items.AddRange(new object[] { "Nomad", "Street Kid", "Corpo" });
            lifePathBox.Location = new System.Drawing.Point(366, 191);
            lifePathBox.Name = "lifePathBox";
            lifePathBox.Size = new System.Drawing.Size(124, 21);
            lifePathBox.TabIndex = 43;
            lifePathBox.SelectedIndexChanged += lifePathBox_SelectedIndexChanged;
            // 
            // lifePathPictureBox
            // 
            lifePathPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lifePathPictureBox.Location = new System.Drawing.Point(366, 217);
            lifePathPictureBox.Name = "lifePathPictureBox";
            lifePathPictureBox.Size = new System.Drawing.Size(124, 188);
            lifePathPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            lifePathPictureBox.TabIndex = 42;
            lifePathPictureBox.TabStop = false;
            // 
            // coolUpDown
            // 
            coolUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            coolUpDown.Location = new System.Drawing.Point(583, 448);
            coolUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            coolUpDown.Name = "coolUpDown";
            coolUpDown.Size = new System.Drawing.Size(45, 27);
            coolUpDown.TabIndex = 38;
            coolUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // intelligenceUpDown
            // 
            intelligenceUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            intelligenceUpDown.Location = new System.Drawing.Point(279, 448);
            intelligenceUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            intelligenceUpDown.Name = "intelligenceUpDown";
            intelligenceUpDown.Size = new System.Drawing.Size(45, 27);
            intelligenceUpDown.TabIndex = 41;
            intelligenceUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // technicalAbilityUpDown
            // 
            technicalAbilityUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            technicalAbilityUpDown.Location = new System.Drawing.Point(583, 159);
            technicalAbilityUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            technicalAbilityUpDown.Name = "technicalAbilityUpDown";
            technicalAbilityUpDown.Size = new System.Drawing.Size(45, 27);
            technicalAbilityUpDown.TabIndex = 40;
            technicalAbilityUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // bodyUpDown
            // 
            bodyUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            bodyUpDown.Location = new System.Drawing.Point(282, 159);
            bodyUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            bodyUpDown.Name = "bodyUpDown";
            bodyUpDown.Size = new System.Drawing.Size(45, 27);
            bodyUpDown.TabIndex = 39;
            bodyUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // reflexesUpDown
            // 
            reflexesUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            reflexesUpDown.Location = new System.Drawing.Point(425, 73);
            reflexesUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            reflexesUpDown.Name = "reflexesUpDown";
            reflexesUpDown.Size = new System.Drawing.Size(45, 27);
            reflexesUpDown.TabIndex = 37;
            reflexesUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // streetCredUpDown
            // 
            streetCredUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            streetCredUpDown.Location = new System.Drawing.Point(116, 25);
            streetCredUpDown.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            streetCredUpDown.Name = "streetCredUpDown";
            streetCredUpDown.Size = new System.Drawing.Size(45, 27);
            streetCredUpDown.TabIndex = 36;
            streetCredUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // levelUpDown
            // 
            levelUpDown.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            levelUpDown.Location = new System.Drawing.Point(15, 25);
            levelUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            levelUpDown.Name = "levelUpDown";
            levelUpDown.Size = new System.Drawing.Size(45, 27);
            levelUpDown.TabIndex = 35;
            levelUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // techieUpDown
            // 
            techieUpDown.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            techieUpDown.Location = new System.Drawing.Point(704, 279);
            techieUpDown.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            techieUpDown.Name = "techieUpDown";
            techieUpDown.Size = new System.Drawing.Size(31, 19);
            techieUpDown.TabIndex = 51;
            // 
            // PlayerStatsControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            Controls.Add(additionalPlayerStatsButton);
            Controls.Add(techieUpDown);
            Controls.Add(soloUpDown);
            Controls.Add(shinobiUpDown);
            Controls.Add(netrunnerUpDown);
            Controls.Add(assassineUpDown);
            Controls.Add(perkPointsUpDown);
            Controls.Add(attrPointsUpDown);
            Controls.Add(lifePathBox);
            Controls.Add(lifePathPictureBox);
            Controls.Add(coolUpDown);
            Controls.Add(intelligenceUpDown);
            Controls.Add(technicalAbilityUpDown);
            Controls.Add(bodyUpDown);
            Controls.Add(reflexesUpDown);
            Controls.Add(streetCredUpDown);
            Controls.Add(levelUpDown);
            Name = "PlayerStatsControl";
            Size = new System.Drawing.Size(851, 548);
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
            ResumeLayout(false);
        }

        #endregion

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
    }
}
