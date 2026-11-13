using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Views.Controls
{
    /// <summary>
    /// Edits the perks of the 2.0 attribute trees.
    /// </summary>
    public class PerksControl : UserControl, IGameControl
    {
        private static readonly (string Attribute, string Label)[] s_attributes =
        {
            ("Body", "Body"),
            ("Reflexes", "Reflexes"),
            ("TechnicalAbility", "Technical Ability"),
            ("Intelligence", "Intelligence"),
            ("Cool", "Cool"),
            ("Espionage", "Relic (Phantom Liberty)"),
        };

        private static readonly string[] s_tierOrder = { "Novice", "Adept", "Expert", "Master" };

        private readonly MainForm _parentForm;

        private readonly ComboBox _attribute;
        private readonly CheckBox _usePoints;
        private readonly Label _points;
        private readonly ListView _list;
        private readonly NumericUpDown _level;

        private PerkHelper _perks;

        public PerksControl(MainForm parentForm)
        {
            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;

            _attribute = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200, Location = new Point(3, 5) };
            _attribute.Items.AddRange(s_attributes.Select(x => (object)x.Label).ToArray());
            _attribute.SelectedIndexChanged += (_, _) => FillList();

            _usePoints = new CheckBox { Text = "Use perk points", Checked = true, AutoSize = true, Location = new Point(215, 7) };
            var toolTip = new ToolTip();
            toolTip.SetToolTip(_usePoints, "Takes (or gives back) perk points like buying the perk in-game would. Uncheck to unlock perks for free.");

            _points = new Label { AutoSize = true, Location = new Point(340, 8) };

            var top = new Panel { Dock = DockStyle.Top, Height = 32 };
            top.Controls.AddRange(new Control[] { _attribute, _usePoints, _points });

            _list = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false
            };
            _list.Columns.Add("Perk", 380);
            _list.Columns.Add("Tier", 140);
            _list.Columns.Add("Level", 80);
            _list.DoubleClick += (_, _) => ToggleSelected();
            _list.SelectedIndexChanged += (_, _) => UpdateLevelInput();

            var levelLabel = new Label { Text = "Level:", AutoSize = true, Margin = new Padding(3, 9, 3, 0) };
            _level = new NumericUpDown { Width = 60, Margin = new Padding(3, 6, 6, 0), Maximum = 3 };

            var setButton = new Button { Text = "Set level", AutoSize = true };
            setButton.Click += (_, _) => SetLevel(_list.SelectedItems.Cast<ListViewItem>(), _ => (int)_level.Value);

            var maxAllButton = new Button { Text = "Max all", AutoSize = true };
            maxAllButton.Click += (_, _) => SetLevel(_list.Items.Cast<ListViewItem>(), perk => perk.MaxLevel);

            var removeAllButton = new Button { Text = "Remove all", AutoSize = true };
            removeAllButton.Click += (_, _) =>
            {
                if (MessageBox.Show($"Remove every perk of {_attribute.Text}?", "Perks", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SetLevel(_list.Items.Cast<ListViewItem>(), _ => 0);
                }
            };

            Theme.SetToolTip(toolTip, setButton, "Set the selected perks to the chosen level.");
            Theme.SetToolTip(toolTip, maxAllButton, "Unlock every perk in the current attribute tree at its maximum level.");
            Theme.SetToolTip(toolTip, removeAllButton, "Reset all perks in the current attribute tree to level 0.");

            var hint = new Label
            {
                Text = "Double click toggles a perk. Requirements between perks are not checked.",
                AutoSize = true,
                ForeColor = SystemColors.GrayText,
                Margin = new Padding(12, 9, 3, 0)
            };

            // Flows, so the buttons can grow with the theme's font without overlapping
            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 34, WrapContents = false, Padding = new Padding(0, 1, 0, 0) };
            bottom.Controls.AddRange(new Control[] { levelLabel, _level, setButton, maxAllButton, removeAllButton, hint });

            Controls.Add(_list);
            Controls.Add(top);
            Controls.Add(bottom);
        }

        public string GameControlName => "Perks";
        public string Icon => "";

        private void OnParentFormPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_parentForm.ActiveSaveFile) && _parentForm.ActiveSaveFile != null)
            {
                this.InvokeIfRequired(Init);
            }
        }

        private void Init()
        {
            _perks = new PerkHelper(_parentForm.ActiveSaveFile.GetPlayerDevelopmentData());

            if (_attribute.SelectedIndex == -1)
            {
                _attribute.SelectedIndex = 0; // Triggers FillList
            }
            else
            {
                FillList();
            }
        }

        private void FillList()
        {
            if (_perks == null || _attribute.SelectedIndex == -1)
            {
                return;
            }

            var attribute = s_attributes[_attribute.SelectedIndex].Attribute;

            _list.BeginUpdate();
            _list.Items.Clear();
            var perks = ResourceHelper.Perks
                .Where(x => x.Attribute == attribute)
                .OrderBy(x => Array.IndexOf(s_tierOrder, x.Tier) is var index and >= 0 ? index : s_tierOrder.Length)
                .ThenBy(ResourceHelper.GetPerkName, StringComparer.CurrentCultureIgnoreCase);

            foreach (var perk in perks)
            {
                var level = _perks.GetLevel(perk);
                _list.Items.Add(new ListViewItem(new[] { ResourceHelper.GetPerkName(perk), perk.Tier, $"{level} / {perk.MaxLevel}" })
                {
                    Tag = perk,
                    ForeColor = level > 0 ? SystemColors.ControlText : SystemColors.GrayText
                });
            }
            _list.EndUpdate();

            UpdatePoints();
        }

        private void UpdatePoints()
        {
            _points.Text = $"Unspent perk points: {_perks.GetUnspentPoints(gamedataDevelopmentPointType.Primary)}   " +
                           $"Relic points: {_perks.GetUnspentPoints(gamedataDevelopmentPointType.Espionage)}";
        }

        private void UpdateLevelInput()
        {
            if (_list.SelectedItems.Count > 0 && _list.SelectedItems[0].Tag is PerkRecord perk)
            {
                _level.Maximum = perk.MaxLevel;
                _level.Value = _perks.GetLevel(perk);
            }
        }

        private void ToggleSelected()
        {
            SetLevel(_list.SelectedItems.Cast<ListViewItem>(), perk => _perks.GetLevel(perk) < perk.MaxLevel ? perk.MaxLevel : 0);
        }

        private void SetLevel(IEnumerable<ListViewItem> rows, Func<PerkRecord, int> level)
        {
            if (_perks == null)
            {
                return;
            }

            foreach (var perk in rows.Select(x => x.Tag).OfType<PerkRecord>().ToList())
            {
                _perks.SetLevel(perk, level(perk), _usePoints.Checked);
                ChangeTracker.MarkChanged();
            }

            var selected = _list.SelectedIndices.Cast<int>().ToList();
            FillList();
            foreach (var index in selected)
            {
                _list.Items[index].Selected = true;
            }
        }
    }
}
