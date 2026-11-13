namespace CP2077SaveEditor.Views.Controls
{
    partial class QuestFactsControl
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
            this.makeAllRomanceableButton = new CP2077SaveEditor.ModernButton();
            this.factsSaveButton = new CP2077SaveEditor.ModernButton();
            this.enableSecretEndingButton = new CP2077SaveEditor.ModernButton();
            this.addFactButton = new CP2077SaveEditor.ModernButton();
            this.factsSearchBox = new System.Windows.Forms.TextBox();
            this.factsListView = new System.Windows.Forms.ListView();
            this.factsValueHeader = new System.Windows.Forms.ColumnHeader();
            this.factsNameHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // enableSecretEndingButton
            // 
            this.enableSecretEndingButton.BackColor = System.Drawing.Color.White;
            this.enableSecretEndingButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.enableSecretEndingButton.ClickEffectEnabled = true;
            this.enableSecretEndingButton.DefaultColor = System.Drawing.Color.White;
            this.enableSecretEndingButton.HoverColor = System.Drawing.Color.LightGray;
            this.enableSecretEndingButton.Location = new System.Drawing.Point(6, 6);
            this.enableSecretEndingButton.Name = "enableSecretEndingButton";
            this.enableSecretEndingButton.Size = new System.Drawing.Size(160, 28);
            this.enableSecretEndingButton.TabIndex = 5;
            this.enableSecretEndingButton.Text = "Enable Secret Ending";
            this.enableSecretEndingButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.enableSecretEndingButton.TextFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.enableSecretEndingButton.Click += new System.EventHandler(this.enableSecretEndingButton_Click);
            // 
            // makeAllRomanceableButton
            // 
            this.makeAllRomanceableButton.BackColor = System.Drawing.Color.White;
            this.makeAllRomanceableButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.makeAllRomanceableButton.ClickEffectEnabled = true;
            this.makeAllRomanceableButton.DefaultColor = System.Drawing.Color.White;
            this.makeAllRomanceableButton.HoverColor = System.Drawing.Color.LightGray;
            this.makeAllRomanceableButton.Location = new System.Drawing.Point(172, 6);
            this.makeAllRomanceableButton.Name = "makeAllRomanceableButton";
            this.makeAllRomanceableButton.Size = new System.Drawing.Size(220, 28);
            this.makeAllRomanceableButton.TabIndex = 6;
            this.makeAllRomanceableButton.Text = "Make All Characters Romanceable";
            this.makeAllRomanceableButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.makeAllRomanceableButton.TextFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.makeAllRomanceableButton.Click += new System.EventHandler(this.makeAllRomanceableButton_Click);
            // 
            // addFactButton
            // 
            this.addFactButton.BackColor = System.Drawing.Color.White;
            this.addFactButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addFactButton.ClickEffectEnabled = true;
            this.addFactButton.DefaultColor = System.Drawing.Color.White;
            this.addFactButton.HoverColor = System.Drawing.Color.LightGray;
            this.addFactButton.Location = new System.Drawing.Point(398, 6);
            this.addFactButton.Name = "addFactButton";
            this.addFactButton.Size = new System.Drawing.Size(104, 28);
            this.addFactButton.TabIndex = 3;
            this.addFactButton.Text = "+ New Fact";
            this.addFactButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.addFactButton.TextFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.addFactButton.Click += new System.EventHandler(this.addFactButton_Click);
            // 
            // factsSaveButton
            // 
            this.factsSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.factsSaveButton.BackColor = System.Drawing.Color.White;
            this.factsSaveButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.factsSaveButton.ClickEffectEnabled = true;
            this.factsSaveButton.DefaultColor = System.Drawing.Color.White;
            this.factsSaveButton.HoverColor = System.Drawing.Color.LightGray;
            this.factsSaveButton.Location = new System.Drawing.Point(725, 6);
            this.factsSaveButton.Name = "factsSaveButton";
            this.factsSaveButton.Size = new System.Drawing.Size(120, 28);
            this.factsSaveButton.TabIndex = 7;
            this.factsSaveButton.Text = "Export to Text";
            this.factsSaveButton.TextColor = System.Drawing.SystemColors.ControlText;
            this.factsSaveButton.TextFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.factsSaveButton.Click += new System.EventHandler(this.factsSaveButton_Click);
            // 
            // factsSearchBox
            // 
            this.factsSearchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.factsSearchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.factsSearchBox.ForeColor = System.Drawing.Color.Silver;
            this.factsSearchBox.Location = new System.Drawing.Point(6, 40);
            this.factsSearchBox.Name = "factsSearchBox";
            this.factsSearchBox.Size = new System.Drawing.Size(839, 23);
            this.factsSearchBox.TabIndex = 2;
            this.factsSearchBox.Text = "Search";
            this.factsSearchBox.TextChanged += new System.EventHandler(this.factsSearchBox_TextChanged);
            this.factsSearchBox.GotFocus += new System.EventHandler(this.SearchBoxGotFocus);
            this.factsSearchBox.LostFocus += new System.EventHandler(this.SearchBoxLostFocus);
            // 
            // factsListView
            // 
            this.factsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.factsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.factsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.factsValueHeader,
            this.factsNameHeader});
            this.factsListView.FullRowSelect = true;
            this.factsListView.GridLines = true;
            this.factsListView.LabelEdit = true;
            this.factsListView.Location = new System.Drawing.Point(0, 69);
            this.factsListView.MultiSelect = false;
            this.factsListView.Name = "factsListView";
            this.factsListView.Size = new System.Drawing.Size(851, 479);
            this.factsListView.TabIndex = 8;
            this.factsListView.UseCompatibleStateImageBehavior = false;
            this.factsListView.View = System.Windows.Forms.View.Details;
            this.factsListView.VirtualMode = true;
            this.factsListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.factsListView_AfterLabelEdit);
            this.factsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.factsListView_KeyDown);
            this.factsListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.factsListView_MouseUp);
            // 
            // factsValueHeader
            // 
            this.factsValueHeader.Name = "factsValueHeader";
            this.factsValueHeader.Text = "Value";
            this.factsValueHeader.Width = 72;
            // 
            // factsNameHeader
            // 
            this.factsNameHeader.Name = "factsNameHeader";
            this.factsNameHeader.Text = "Name";
            this.factsNameHeader.Width = 750;
            // 
            // QuestFactsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.factsListView);
            this.Controls.Add(this.factsSearchBox);
            this.Controls.Add(this.addFactButton);
            this.Controls.Add(this.enableSecretEndingButton);
            this.Controls.Add(this.factsSaveButton);
            this.Controls.Add(this.makeAllRomanceableButton);
            this.Name = "QuestFactsControl";
            this.Size = new System.Drawing.Size(851, 548);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ModernButton makeAllRomanceableButton;
        private ModernButton factsSaveButton;
        private ModernButton enableSecretEndingButton;
        private ModernButton addFactButton;
        private System.Windows.Forms.TextBox factsSearchBox;
        private System.Windows.Forms.ListView factsListView;
        private System.Windows.Forms.ColumnHeader factsValueHeader;
        private System.Windows.Forms.ColumnHeader factsNameHeader;
    }
}
