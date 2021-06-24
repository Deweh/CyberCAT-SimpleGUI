using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CP2077SaveEditor.Extensions;
using static CyberCAT.Core.Classes.NodeRepresentations.CharacterCustomizationAppearances;

namespace CP2077SaveEditor
{
    public partial class AdvancedAppearanceDialog : Form
    {
        private Dictionary<string, HashValueEntry> options = new Dictionary<string, HashValueEntry>();
        private Form1 parent;

        public AdvancedAppearanceDialog()
        {
            InitializeComponent();
        }

        private void AdvancedAppearanceDialog_Load(object sender, EventArgs e)
        {
            parent = this.Owner as Form1;
            var container = Form1.activeSaveFile.GetAppearanceContainer();

            foreach (var section in container.FirstSection.AppearanceSections)
            {
                foreach (var mainEntry in section.MainList)
                {
                    options.Add("face/" + section.SectionName + "/" + mainEntry.SecondString, mainEntry);
                }
            }

            foreach (var section in container.SecondSection.AppearanceSections)
            {
                foreach (var mainEntry in section.MainList)
                {
                    options.Add("hands/" + section.SectionName + "/" + mainEntry.SecondString, mainEntry);
                }
            }

            foreach (var section in container.ThirdSection.AppearanceSections)
            {
                foreach (var mainEntry in section.MainList)
                {
                    options.Add("body/" + section.SectionName + "/" + mainEntry.SecondString, mainEntry);
                }
            }

            optionsBox.Items.AddRange(options.Keys.ToArray());
        }

        private void optionsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optionsBox.SelectedItem != null)
            {
                HashValueEntry entry = options[(string)optionsBox.SelectedItem];

                firstBox.Text = entry.FirstString;
                secondBox.Text = entry.SecondString;
                pathBox.Text = entry.GetPath();
            }
        }
    }
}
