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
    public partial class AddFact : Form
    {
        private Action<string, int, int> callbackFunc;

        public AddFact()
        {
            InitializeComponent();
        }

        public void LoadFactDialog(Action<string, int, int> callback)
        {
            callbackFunc = callback;
            factTypeBox.SelectedIndex = 0;
            this.ShowDialog();
        }

        private void addFactButton_Click(object sender, EventArgs e)
        {
            callbackFunc(factEntryBox.Text, factTypeBox.SelectedIndex, (int)factValueUpDown.Value);
            this.Close();
        }
    }
}
