using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace CP2077SaveEditor.Views
{
    public partial class AddItem : Form
    {
        private InventoryHelper.SubInventory _inventory;

        private Dictionary<string, ItemRecord> _items = new();

        private TweakDBID? _selectedItemId;
        private ItemRecord _selectedItemRecord;

        public AddItem()
        {
            InitializeComponent();
        }

        public void LoadDialog(InventoryHelper.SubInventory inventory)
        {
            _inventory = inventory;

            RefreshItems();
            ShowDialog();
        }

        private void RefreshItems()
        {
            _items = new Dictionary<string, ItemRecord>();

            foreach (var (key, value) in ResourceHelper.ItemClasses)
            {
                _items.Add(((TweakDBID)key).GetResolvedText()!, value);
            }

            _items = _items.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            cb_Items.Items.Clear();
            cb_Items.Items.AddRange(_items.Keys.Cast<object>().ToArray());
        }

        private void cb_Items_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedItemId = null;
            _selectedItemRecord = null;

            num_Quantity.Enabled = false;
            btn_Add.Enabled = false;

            if (cb_Items.SelectedItem is not string itemStr || !_items.TryGetValue(itemStr, out var itemRecord))
            {
                return;
            }

            _selectedItemId = itemStr;
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

            var item = new InventoryHelper.ItemData
            {
                Header = new InventoryHelper.gameItemIdWrapper
                {
                    ItemId = new gameItemID
                    {
                        Id = (TweakDBID)_selectedItemId
                    }
                }
            };

            if (_selectedItemRecord.Type == "Grenade")
            {
                item.Header.ItemStructure = (Enums.gamedataItemStructure)3;
                item.Header.ItemId.RngSeed = 2;

                item.Data = new InventoryHelper.ModableItemWithQuantityData
                {
                    Quantity = (uint)num_Quantity.Value,
                    ModHeaderThing = new InventoryHelper.ModHeaderThing(),
                    RootNode = new InventoryHelper.ItemModData
                    {
                        AppearanceName = "None",
                        Header = new InventoryHelper.gameItemIdWrapper
                        {
                            ItemId = new gameItemID()
                        },
                        ModHeaderThing = new InventoryHelper.ModHeaderThing()
                    }
                };
            }
            else
            {
                if (_selectedItemRecord.IsSingleInstance)
                {
                    item.Header.ItemStructure = (Enums.gamedataItemStructure)0; // or 2 with any seed
                    item.Header.ItemId.RngSeed = 2;

                    item.Data = new InventoryHelper.SimpleItemData
                    {
                        Quantity = (uint)num_Quantity.Value
                    };
                }
                else
                {
                    var randBytes = new byte[4];
                    new Random().NextBytes(randBytes);
                    var newSeed = BitConverter.ToUInt32(randBytes);

                    item.Header.ItemStructure = (Enums.gamedataItemStructure)0; // or 1 with any seed
                    item.Header.ItemId.RngSeed = newSeed;

                    item.Data = new InventoryHelper.ModableItemData
                    {
                        ModHeaderThing = new InventoryHelper.ModHeaderThing(),
                        RootNode = new InventoryHelper.ItemModData
                        {
                            AppearanceName = "None",
                            Header = new InventoryHelper.gameItemIdWrapper
                            {
                                ItemId = new gameItemID()
                            },
                            ModHeaderThing = new InventoryHelper.ModHeaderThing()
                        }
                    };
                }
            }

            _inventory.Items.Add(item);

            Close();
        }
    }
}
