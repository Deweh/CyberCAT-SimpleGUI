using System;
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
            Close();
        }

        public void LoadAddDialog(Action<string> callback)
        {
            callbackFunc = callback;
            modifierObjectBox.SelectedIndex = 0;
            ShowDialog();
        }
    }
}