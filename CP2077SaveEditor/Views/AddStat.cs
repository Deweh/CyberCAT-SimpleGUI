using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2077SaveEditor
{
    public partial class AddStat : Form
    {
        private Action<string> callbackFunc;

        public AddStat()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            callbackFunc(modifierObjectBox.Text);
            this.Close();
        }

        public void LoadAddDialog(Action<string> callback)
        {
            callbackFunc = callback;
            modifierObjectBox.SelectedIndex = 0;
            this.ShowDialog();
        }
    }
}
