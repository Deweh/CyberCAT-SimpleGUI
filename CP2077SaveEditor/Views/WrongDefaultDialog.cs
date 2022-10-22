using System;
using System.Windows.Forms;

namespace CP2077SaveEditor
{
    public partial class WrongDefaultDialog : Form
    {
        public WrongDefaultDialog(string className, string prop, object value)
        {
            InitializeComponent();
            errorBox.Text = "Please report this issue at https://github.com/Deweh/CyberCAT-SimpleGUI/issues" + Environment.NewLine +
                            "WrongDefaultValue" + Environment.NewLine + 
                            "Class Name: " + className + Environment.NewLine + 
                            "Property: " + prop + Environment.NewLine +
                            "Value: " + value;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}