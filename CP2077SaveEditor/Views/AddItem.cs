using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views
{
    public partial class AddItem : Form
    {
        private SubInventory _inventory;
        private SaveFileHelper _activeSaveFile;

        private List<ItemEntry> _items = new();

        // ToString is what the list shows
        private record ItemEntry(string Id, string Label, ItemRecord Record)
        {
            public override string ToString() => Label;
        }

        private TweakDBID? _selectedItemId;
        private ItemRecord _selectedItemRecord;

        public AddItem()
        {
            InitializeComponent();
        }

        public void LoadDialog(SubInventory inventory, SaveFileHelper activeSaveFile)
        {
            _inventory = inventory;
            _activeSaveFile = activeSaveFile;

            RefreshItems();
            ShowDialog();
        }

        private const string AllTypes = "All types";

        private void RefreshItems()
        {
            _items = new List<ItemEntry>();

            foreach (var (key, value) in ResourceHelper.ItemClasses)
            {
                // Items from newer patches can't be listed until WolvenKit knows their names
                var id = ((TweakDBID)key).GetResolvedText();
                if (id == null)
                {
                    continue;
                }

                var displayName = ResourceHelper.GetDisplayName(key);
                _items.Add(new ItemEntry(id, displayName == null ? id : $"{displayName}  ({id})", value));
            }

            // Items with an in-game name first, the technical ones (recipes, parts...) after
            _items = _items
                .OrderBy(x => x.Label == x.Id)
                .ThenBy(x => x.Label, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            cb_Type.Items.Clear();
            cb_Type.Items.Add(AllTypes);
            cb_Type.Items.AddRange(_items.Select(x => x.Record.Type).Distinct().Order().Cast<object>().ToArray());
            cb_Type.SelectedIndex = 0; // Triggers FilterItems
        }

        private void FilterItems()
        {
            var terms = txt_Search.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var type = cb_Type.SelectedItem as string;

            var filtered = _items
                .Where(x => type == null || type == AllTypes || x.Record.Type == type)
                .Where(x => terms.All(t => x.Label.Contains(t, StringComparison.CurrentCultureIgnoreCase)))
                .ToArray();

            lb_Items.BeginUpdate();
            lb_Items.Items.Clear();
            lb_Items.Items.AddRange(filtered.Cast<object>().ToArray());
            lb_Items.EndUpdate();

            lbl_Count.Text = $"{filtered.Length} item(s)";

            if (filtered.Length == 1)
            {
                lb_Items.SelectedIndex = 0;
            }
            else
            {
                lb_Items_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            FilterItems();
        }

        private void txt_Search_KeyDown(object sender, KeyEventArgs e)
        {
            // Lets the user search and pick an item without leaving the keyboard
            if (e.KeyCode == Keys.Down && lb_Items.Items.Count > 0)
            {
                lb_Items.Focus();
                lb_Items.SelectedIndex = Math.Max(lb_Items.SelectedIndex, 0);
                e.Handled = true;
            }
        }

        private void cb_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterItems();
        }

        private void lb_Items_DoubleClick(object sender, EventArgs e)
        {
            if (btn_Add.Enabled)
            {
                btn_Add_Click(sender, e);
            }
        }

        private void lb_Items_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedItemId = null;
            _selectedItemRecord = null;

            num_Quantity.Enabled = false;
            btn_Add.Enabled = false;

            if (lb_Items.SelectedItem is not ItemEntry entry)
            {
                return;
            }

            var itemRecord = entry.Record;
            _selectedItemId = entry.Id;
            _selectedItemRecord = itemRecord;

            if (itemRecord.IsSingleInstance || itemRecord.Type == "Grenade")
            {
                num_Quantity.Enabled = true;
            }

            btn_Add.Enabled = true;
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (_selectedItemId == null)
            {
                return;
            }

            _inventory.Items.Add(_activeSaveFile.CreateItem((TweakDBID)_selectedItemId, _selectedItemRecord, (uint)num_Quantity.Value));
            ChangeTracker.MarkChanged();

            Close();
        }
    }
}
