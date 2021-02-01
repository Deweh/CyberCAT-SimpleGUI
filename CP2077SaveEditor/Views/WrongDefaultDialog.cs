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
    public partial class WrongDefaultDialog : Form
    {
        public WrongDefaultDialog(string className, string prop, object value)
        {
            InitializeComponent();
            var n = Environment.NewLine;
            errorBox.Text = "Please report this issue at https://github.com/Deweh/CyberCAT-SimpleGUI/issues" + n + "WrongDefaultValue" + n + "Class Name: " + className + n + "Property: " + prop + n + "Value: " + value.ToString();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
