using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;
using Point = System.Drawing.Point;

namespace CP2077SaveEditor.Views.Controls
{
    /// <summary>
    /// Shows V's cyberware slots, equips any cyberware (owned or not) and raises the capacity limit.
    /// </summary>
    public class CyberwareControl : UserControl, IGameControl
    {
        private readonly MainForm _parentForm;

        private readonly NumericUpDown _extraCapacity;
        private readonly ListView _slots;
        private readonly ComboBox _items;
        private readonly CheckBox _ownedOnly;
        private readonly Button _equipButton;
        private readonly Button _unequipButton;

        private CyberwareHelper _cyberware;

        // An entry of the item picker: an owned item or a record to create
        private record Choice(string Label, ItemData Owned, ulong RecordId)
        {
            public override string ToString() => Label;
        }

        public CyberwareControl(MainForm parentForm)
        {
            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;

            var capacityLabel = new Label { Text = "Extra cyberware capacity:", AutoSize = true, Location = new Point(3, 9) };
            _extraCapacity = new NumericUpDown { Minimum = -1000, Maximum = 100000, Width = 80, Location = new Point(150, 6) };
            var applyCapacity = new Button { Text = "Apply", AutoSize = true, Location = new Point(236, 4) };
            applyCapacity.Click += (_, _) =>
            {
                _cyberware?.SetExtraCapacity((int)_extraCapacity.Value);
                ChangeTracker.MarkChanged();
                MessageBox.Show($"Extra capacity set to {_extraCapacity.Value}.", "Cyberware");
            };
            var capacityHint = new Label
            {
                Text = "Added on top of the capacity the game computes (level, Tech skill, perks, capacity shards).",
                AutoSize = true,
                ForeColor = SystemColors.GrayText,
                Location = new Point(320, 9)
            };

            var top = new Panel { Dock = DockStyle.Top, Height = 32 };
            top.Controls.AddRange(new Control[] { capacityLabel, _extraCapacity, applyCapacity, capacityHint });

            _slots = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false,
                HideSelection = false
            };
            _slots.Columns.Add("Area", 180);
            _slots.Columns.Add("Slot", 50);
            _slots.Columns.Add("Equipped", 520);
            _slots.SelectedIndexChanged += (_, _) => FillItems();

            _items = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 420, Location = new Point(3, 6), DropDownHeight = 400 };
            _ownedOnly = new CheckBox { Text = "Owned only", AutoSize = true, Location = new Point(430, 8) };
            _ownedOnly.CheckedChanged += (_, _) => FillItems();

            _equipButton = new Button { Text = "Equip", AutoSize = true, Location = new Point(530, 4) };
            _equipButton.Click += (_, _) => EquipSelected();
            _unequipButton = new Button { Text = "Unequip", AutoSize = true, Location = new Point(610, 4) };
            _unequipButton.Click += (_, _) => UnequipSelected();

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 34 };
            bottom.Controls.AddRange(new Control[] { _items, _ownedOnly, _equipButton, _unequipButton });

            Controls.Add(_slots);
            Controls.Add(top);
            Controls.Add(bottom);
        }

        public string GameControlName => "Cyberware";
        public string Icon => "";

        private void OnParentFormPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_parentForm.ActiveSaveFile) && _parentForm.ActiveSaveFile != null)
            {
                this.InvokeIfRequired(Init);
            }
        }

        private void Init()
        {
            _cyberware = new CyberwareHelper(_parentForm.ActiveSaveFile);
            _extraCapacity.Value = Math.Clamp(_cyberware.GetExtraCapacity(), (int)_extraCapacity.Minimum, (int)_extraCapacity.Maximum);
            FillSlots();
        }

        private void FillSlots()
        {
            var selected = _slots.SelectedIndices.Count > 0 ? _slots.SelectedIndices[0] : -1;

            _slots.BeginUpdate();
            _slots.Items.Clear();
            foreach (var area in _cyberware.GetCyberwareAreas())
            {
                for (var i = 0; i < area.EquipSlots.Count; i++)
                {
                    var slot = area.EquipSlots[i];
                    var equipped = (ulong)(slot.ItemID?.Id ?? 0) == 0 ? "(empty)" : GetName(slot.ItemID.Id);
                    _slots.Items.Add(new ListViewItem(new[] { area.AreaType.ToString().Replace("CW", ""), (i + 1).ToString(), equipped })
                    {
                        Tag = (area, slot),
                        ForeColor = equipped == "(empty)" ? SystemColors.GrayText : SystemColors.ControlText
                    });
                }
            }
            _slots.EndUpdate();

            if (selected >= 0 && selected < _slots.Items.Count)
            {
                _slots.Items[selected].Selected = true;
            }

            FillItems();
        }

        private void FillItems()
        {
            _items.Items.Clear();
            _equipButton.Enabled = _unequipButton.Enabled = false;

            if (_slots.SelectedItems.Count == 0 || _slots.SelectedItems[0].Tag is not (gameSEquipArea area, gameSEquipSlot slot))
            {
                return;
            }

            var areaName = area.AreaType.ToString();

            // Owned items of this area first, then everything else the game has for it
            var owned = _parentForm.ActiveSaveFile.GetInventory(1).Items
                .Where(x => CyberwareHelper.GetArea(x.ItemInfo.ItemId.Id) == areaName)
                .Select(x => new Choice($"{GetName(x.ItemInfo.ItemId.Id)}  (owned)", x, (ulong)x.ItemInfo.ItemId.Id))
                .OrderBy(x => x.Label, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            var choices = owned.AsEnumerable();
            if (!_ownedOnly.Checked)
            {
                var ownedIds = owned.Select(x => x.RecordId).ToHashSet();
                choices = choices.Concat(ResourceHelper.CyberwareAreas
                    .Where(x => x.Value == areaName && !ownedIds.Contains(x.Key))
                    .Select(x => new Choice(GetName(x.Key), null, x.Key))
                    .OrderBy(x => x.Label, StringComparer.CurrentCultureIgnoreCase));
            }

            _items.Items.AddRange(choices.Cast<object>().ToArray());
            if (_items.Items.Count > 0)
            {
                _items.SelectedIndex = 0;
            }

            _equipButton.Enabled = _items.Items.Count > 0;
            _unequipButton.Enabled = (ulong)(slot.ItemID?.Id ?? 0) != 0;
        }

        private void EquipSelected()
        {
            if (_slots.SelectedItems.Count == 0 || _slots.SelectedItems[0].Tag is not (gameSEquipArea, gameSEquipSlot slot) ||
                _items.SelectedItem is not Choice choice)
            {
                return;
            }

            var item = choice.Owned;
            if (item == null)
            {
                // Not in the inventory yet, create it like the Add Item dialog does
                var record = ResourceHelper.ItemClasses.GetValueOrDefault(choice.RecordId) ?? new ItemRecord { Type = "Cyberware" };
                item = _parentForm.ActiveSaveFile.CreateItem(choice.RecordId, record, 1);
                _parentForm.ActiveSaveFile.GetInventory(1).Items.Add(item);
            }

            _cyberware.Equip(slot, item);
            ChangeTracker.MarkChanged();
            FillSlots();
        }

        private void UnequipSelected()
        {
            if (_slots.SelectedItems.Count > 0 && _slots.SelectedItems[0].Tag is (gameSEquipArea, gameSEquipSlot slot))
            {
                CyberwareHelper.Unequip(slot);
                ChangeTracker.MarkChanged();
                FillSlots();
            }
        }

        private static string GetName(TweakDBID id) => ResourceHelper.GetDisplayName(id) ?? id.GetResolvedText() ?? ((ulong)id).ToString();

        private static string GetName(ulong id) => GetName((TweakDBID)id);
    }
}
