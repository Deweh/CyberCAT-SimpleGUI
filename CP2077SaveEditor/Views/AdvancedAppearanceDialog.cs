using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor
{
    public partial class AdvancedAppearanceDialog : Form
    {
        private readonly gameuiCharacterCustomizationPresetWrapper _container;

        private Dictionary<string, gameuiCustomizationAppearance> options = new Dictionary<string, gameuiCustomizationAppearance>();
        public delegate void ApplyEvent();
        public ApplyEvent ChangesApplied;

        public AdvancedAppearanceDialog(gameuiCharacterCustomizationPresetWrapper container)
        {
            InitializeComponent();

            _container = container;

            firstBox.TextChanged += inputBox_TextChanged;
            secondBox.TextChanged += inputBox_TextChanged;
            pathBox.TextChanged += inputBox_TextChanged;
        }

        private void AdvancedAppearanceDialog_Load(object sender, EventArgs e)
        {
            foreach (var customizationGroup in _container.Preset.HeadGroups)
            {
                foreach (var mainEntry in customizationGroup.Customization)
                {
                    options.Add("face/" + customizationGroup.Name + "/" + mainEntry.Name, mainEntry);
                }
            }

            foreach (var customizationGroup in _container.Preset.ArmsGroups)
            {
                foreach (var mainEntry in customizationGroup.Customization)
                {
                    options.Add("face/" + customizationGroup.Name + "/" + mainEntry.Name, mainEntry);
                }
            }

            foreach (var customizationGroup in _container.Preset.BodyGroups)
            {
                foreach (var mainEntry in customizationGroup.Customization)
                {
                    options.Add("face/" + customizationGroup.Name + "/" + mainEntry.Name, mainEntry);
                }
            }

            optionsBox.Items.AddRange(options.Keys.ToArray());
        }

        private void optionsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optionsBox.SelectedItem != null)
            {
                var entry = options[(string)optionsBox.SelectedItem];

                firstBox.Text = entry.Definition;
                secondBox.Text = entry.Name;
                pathBox.Text = entry.Resource.DepotPath.ToString();
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

            var entry = options[(string)optionsBox.SelectedItem];

            entry.Definition = firstBox.Text;
            entry.Name = secondBox.Text;
            entry.Resource = new CResourceAsyncReference<appearanceAppearanceResource>(pathBox.Text, entry.Resource.Flags);

            ChangesApplied();
            applyButton.Enabled = false;
        }
    }
}
