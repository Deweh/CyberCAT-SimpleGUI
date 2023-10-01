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

        private Dictionary<string, ItemRecord> _items = new();

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

            var item = new ItemData
            {
                ItemInfo = new ItemInfo
                {
                    ItemId = new gameItemID
                    {
                        Id = (TweakDBID)_selectedItemId,
                        UniqueCounter = _activeSaveFile.GetNextUniqueCounter()
                    }
                }
            };

            if (_selectedItemRecord.Type == "Grenade")
            {
                item.ItemInfo.ItemStructure = ItemStructure.Quantity | ItemStructure.Extended;
                item.ItemInfo.ItemId.RngSeed = 2;

                item.Quantity = (uint)num_Quantity.Value;
                item.ItemAdditionalInfo = new ItemAdditionalInfo();
                item.ItemSlotPart = new ItemSlotPart
                {
                    ItemInfo = new ItemInfo
                    {
                        ItemId = new gameItemID()
                    },
                    AppearanceName = "None",
                    ItemAdditionalInfo = new ItemAdditionalInfo()
                };
            }
            else
            {
                if (_selectedItemRecord.IsSingleInstance)
                {
                    item.ItemInfo.ItemStructure = ItemStructure.None; // or 2 with any seed
                    item.ItemInfo.ItemId.RngSeed = 2;

                    item.Quantity = (uint)num_Quantity.Value;
                }
                else
                {
                    item.ItemInfo.ItemStructure = ItemStructure.None; // or 1 with any seed
                    item.ItemInfo.ItemId.RngSeed = _activeSaveFile.CreateUniqueSeed();

                    item.ItemAdditionalInfo = new ItemAdditionalInfo();
                    item.ItemSlotPart = new ItemSlotPart
                    {
                        ItemInfo = new ItemInfo
                        {
                            ItemId = new gameItemID()
                        },
                        AppearanceName = "None",
                        ItemAdditionalInfo = new ItemAdditionalInfo()
                    };
                }
            }

            if (item.HasExtendedData())
            {
                _activeSaveFile.CreateStatData(item.ItemInfo.ItemId);

                if (ResourceHelper.ItemClasses.TryGetValue(item.ItemInfo.ItemId.Id, out var itemRecord))
                {
                    if (itemRecord.SlotParts is { Count: > 0 })
                    {
                        item.ItemSlotPart = CreateSlotPart(itemRecord.SlotParts[0].ItemPartPreset, itemRecord.SlotParts[0].Slot);
                        //_activeSaveFile.CreateStatData(modableItemData.RootNode);

                        if (itemRecord.SlotParts.Count > 1)
                        {
                            var childMods = new List<ItemSlotPart>();
                            for (int i = 1; i < itemRecord.SlotParts.Count; i++)
                            {
                                var subMod = CreateSlotPart(itemRecord.SlotParts[i].ItemPartPreset, itemRecord.SlotParts[i].Slot);
                                //_activeSaveFile.CreateStatData(subMod);

                                childMods.Add(subMod);
                            }

                            item.ItemSlotPart!.Children = childMods;
                        }
                    }
                }
            }

            _inventory.Items.Add(item);

            Close();

            ItemSlotPart CreateSlotPart(TweakDBID id, TweakDBID slot)
            {
                return new ItemSlotPart
                {
                    ItemInfo = new ItemInfo
                    {
                        ItemId = new gameItemID
                        {
                            Id = id,
                            RngSeed = _activeSaveFile.CreateUniqueSeed(),
                            UniqueCounter = _activeSaveFile.GetNextUniqueCounter()
                        }
                    },
                    AppearanceName = "None",
                    AttachmentSlotTdbId = slot,
                    ItemAdditionalInfo = new ItemAdditionalInfo
                    {
                        RequiredLevel = float.MaxValue
                    }
                };
            }
        }
    }
}
