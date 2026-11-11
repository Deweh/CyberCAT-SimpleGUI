namespace CP2077SaveEditor.Views
{
    partial class AddItem
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
            txt_Search = new System.Windows.Forms.TextBox();
            cb_Type = new System.Windows.Forms.ComboBox();
            lb_Items = new System.Windows.Forms.ListBox();
            lbl_Count = new System.Windows.Forms.Label();
            lbl_Quantity = new System.Windows.Forms.Label();
            num_Quantity = new System.Windows.Forms.NumericUpDown();
            btn_Add = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)num_Quantity).BeginInit();
            SuspendLayout();
            //
            // txt_Search
            //
            txt_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txt_Search.Location = new System.Drawing.Point(12, 12);
            txt_Search.Name = "txt_Search";
            txt_Search.PlaceholderText = "Search (e.g. Preset_Overture)";
            txt_Search.Size = new System.Drawing.Size(290, 22);
            txt_Search.TabIndex = 0;
            txt_Search.TextChanged += txt_Search_TextChanged;
            txt_Search.KeyDown += txt_Search_KeyDown;
            //
            // cb_Type
            //
            cb_Type.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cb_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_Type.FormattingEnabled = true;
            cb_Type.Location = new System.Drawing.Point(308, 12);
            cb_Type.Name = "cb_Type";
            cb_Type.Size = new System.Drawing.Size(140, 21);
            cb_Type.TabIndex = 1;
            cb_Type.SelectedIndexChanged += cb_Type_SelectedIndexChanged;
            //
            // lb_Items
            //
            lb_Items.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lb_Items.FormattingEnabled = true;
            lb_Items.IntegralHeight = false;
            lb_Items.Location = new System.Drawing.Point(12, 40);
            lb_Items.Name = "lb_Items";
            lb_Items.Size = new System.Drawing.Size(436, 300);
            lb_Items.TabIndex = 2;
            lb_Items.SelectedIndexChanged += lb_Items_SelectedIndexChanged;
            lb_Items.DoubleClick += lb_Items_DoubleClick;
            //
            // lbl_Count
            //
            lbl_Count.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lbl_Count.AutoSize = true;
            lbl_Count.Location = new System.Drawing.Point(12, 349);
            lbl_Count.Name = "lbl_Count";
            lbl_Count.Size = new System.Drawing.Size(0, 13);
            lbl_Count.TabIndex = 3;
            //
            // lbl_Quantity
            //
            lbl_Quantity.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            lbl_Quantity.AutoSize = true;
            lbl_Quantity.Location = new System.Drawing.Point(184, 349);
            lbl_Quantity.Name = "lbl_Quantity";
            lbl_Quantity.Size = new System.Drawing.Size(54, 13);
            lbl_Quantity.TabIndex = 4;
            lbl_Quantity.Text = "Quantity:";
            //
            // num_Quantity
            //
            num_Quantity.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            num_Quantity.Enabled = false;
            num_Quantity.Location = new System.Drawing.Point(244, 347);
            num_Quantity.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            num_Quantity.Name = "num_Quantity";
            num_Quantity.Size = new System.Drawing.Size(120, 22);
            num_Quantity.TabIndex = 5;
            num_Quantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // btn_Add
            //
            btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btn_Add.Enabled = false;
            btn_Add.Location = new System.Drawing.Point(373, 346);
            btn_Add.Name = "btn_Add";
            btn_Add.Size = new System.Drawing.Size(75, 23);
            btn_Add.TabIndex = 6;
            btn_Add.Text = "Add";
            btn_Add.UseVisualStyleBackColor = true;
            btn_Add.Click += btn_Add_Click;
            //
            // AddItem
            //
            AcceptButton = btn_Add;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(460, 381);
            Controls.Add(btn_Add);
            Controls.Add(num_Quantity);
            Controls.Add(lbl_Quantity);
            Controls.Add(lbl_Count);
            Controls.Add(lb_Items);
            Controls.Add(cb_Type);
            Controls.Add(txt_Search);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            MinimumSize = new System.Drawing.Size(400, 250);
            Name = "AddItem";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Add Item";
            ((System.ComponentModel.ISupportInitialize)num_Quantity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txt_Search;
        private System.Windows.Forms.ComboBox cb_Type;
        private System.Windows.Forms.ListBox lb_Items;
        private System.Windows.Forms.Label lbl_Count;
        private System.Windows.Forms.Label lbl_Quantity;
        private System.Windows.Forms.NumericUpDown num_Quantity;
        private System.Windows.Forms.Button btn_Add;
    }
}