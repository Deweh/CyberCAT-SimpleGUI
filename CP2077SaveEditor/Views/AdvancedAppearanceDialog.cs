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
using static WolvenKit.RED4.Save.CharacterCustomizationAppearances;

namespace CP2077SaveEditor
{
    public partial class AdvancedAppearanceDialog : Form
    {
        private Dictionary<string, HashValueEntry> options = new Dictionary<string, HashValueEntry>();
        public delegate void ApplyEvent();
        public ApplyEvent ChangesApplied;

        public AdvancedAppearanceDialog()
        {
            InitializeComponent();

            firstBox.TextChanged += inputBox_TextChanged;
            secondBox.TextChanged += inputBox_TextChanged;
            pathBox.TextChanged += inputBox_TextChanged;
        }

        private void AdvancedAppearanceDialog_Load(object sender, EventArgs e)
        {
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
                pathBox.Text = entry.Hash.DepotPath.ToString();
            }
            else
            {
                firstBox.Text = string.Empty;
                secondBox.Text = string.Empty;
                pathBox.Text = string.Empty;
            }

            applyButton.Enabled = false;
        }

        private void inputBox_TextChanged(object sender, EventArgs e)
        {
            applyButton.Enabled = true;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (optionsBox.SelectedItem is null)
            {
                MessageBox.Show("Error: No appearance entry selected.");
                return;
            }

            HashValueEntry entry = options[(string)optionsBox.SelectedItem];

            entry.FirstString = firstBox.Text;
            entry.SecondString = secondBox.Text;
            entry.Hash.DepotPath = pathBox.Text;

            ChangesApplied();
            applyButton.Enabled = false;
        }
    }
}
