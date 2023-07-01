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
            cb_Items = new System.Windows.Forms.ComboBox();
            lbl_Item = new System.Windows.Forms.Label();
            lbl_Quantity = new System.Windows.Forms.Label();
            num_Quantity = new System.Windows.Forms.NumericUpDown();
            btn_Add = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)num_Quantity).BeginInit();
            SuspendLayout();
            // 
            // cb_Items
            // 
            cb_Items.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            cb_Items.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            cb_Items.FormattingEnabled = true;
            cb_Items.Location = new System.Drawing.Point(72, 12);
            cb_Items.Name = "cb_Items";
            cb_Items.Size = new System.Drawing.Size(274, 21);
            cb_Items.TabIndex = 0;
            cb_Items.SelectedIndexChanged += cb_Items_SelectedIndexChanged;
            // 
            // lbl_Item
            // 
            lbl_Item.AutoSize = true;
            lbl_Item.Location = new System.Drawing.Point(12, 15);
            lbl_Item.Name = "lbl_Item";
            lbl_Item.Size = new System.Drawing.Size(32, 13);
            lbl_Item.TabIndex = 1;
            lbl_Item.Text = "Item:";
            // 
            // lbl_Quantity
            // 
            lbl_Quantity.AutoSize = true;
            lbl_Quantity.Location = new System.Drawing.Point(12, 41);
            lbl_Quantity.Name = "lbl_Quantity";
            lbl_Quantity.Size = new System.Drawing.Size(54, 13);
            lbl_Quantity.TabIndex = 2;
            lbl_Quantity.Text = "Quantity:";
            // 
            // num_Quantity
            // 
            num_Quantity.Enabled = false;
            num_Quantity.Location = new System.Drawing.Point(72, 39);
            num_Quantity.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            num_Quantity.Name = "num_Quantity";
            num_Quantity.Size = new System.Drawing.Size(120, 22);
            num_Quantity.TabIndex = 3;
            num_Quantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btn_Add
            // 
            btn_Add.Enabled = false;
            btn_Add.Location = new System.Drawing.Point(271, 68);
            btn_Add.Name = "btn_Add";
            btn_Add.Size = new System.Drawing.Size(75, 23);
            btn_Add.TabIndex = 4;
            btn_Add.Text = "Add";
            btn_Add.UseVisualStyleBackColor = true;
            btn_Add.Click += btn_Add_Click;
            // 
            // AddItem
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(358, 103);
            Controls.Add(btn_Add);
            Controls.Add(num_Quantity);
            Controls.Add(lbl_Quantity);
            Controls.Add(lbl_Item);
            Controls.Add(cb_Items);
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "AddItem";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Add Item";
            ((System.ComponentModel.ISupportInitialize)num_Quantity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cb_Items;
        private System.Windows.Forms.Label lbl_Item;
        private System.Windows.Forms.Label lbl_Quantity;
        private System.Windows.Forms.NumericUpDown num_Quantity;
        private System.Windows.Forms.Button btn_Add;
    }
}