using Newtonsoft.Json;
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
        private Action<string> callbackFunc;
        private SaveFileHelper activeSaveFile;

        public AddFact()
        {
            InitializeComponent();
        }

        public void LoadFactDialog(Action<string> callback, object _saveFile)
        {
            callbackFunc = callback;
            activeSaveFile = (SaveFileHelper)_saveFile;
            factTypeBox.SelectedIndex = 0;
            this.ShowDialog();
        }

        private void addFactButton_Click(object sender, EventArgs e)
        {
            if (factTypeBox.SelectedIndex == 1)
            {
                if (!uint.TryParse(factEntryBox.Text, out _))
                {
                    MessageBox.Show("Hash must be a valid 32-bit unsigned integer.");
                    return;
                }
                activeSaveFile.AddFactByHash(uint.Parse(factEntryBox.Text), (uint)factValueUpDown.Value);
            } else {
                var factsList = JsonConvert.DeserializeObject<Dictionary<uint, string>>(CP2077SaveEditor.Properties.Resources.Facts);
                if (!factsList.Values.Contains(factEntryBox.Text))
                {
                    MessageBox.Show("Fact name '" + factEntryBox.Text + "' could not be found on the known facts list.");
                    return;
                }
                activeSaveFile.AddFactByName(factEntryBox.Text, (uint)factValueUpDown.Value);
            }

            callbackFunc("");
            this.Close();
        }
    }
}
