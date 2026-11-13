using CP2077SaveEditor.Extensions;
using CP2077SaveEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class InventoryControl : UserControl, IGameControl
    {
        private enum Operation
        {
            None,
            Cut,
            Copy
        }

        private readonly MainForm _parentForm;

        private Random _random = new();

        private Operation _itemOperation;
        private ItemData _itemHolder;

        private const string SearchPlaceholder = "Search  (filters: name: type: id: slot: quantity: desc:)";

        private const ulong PlayerInventoryId = StashHelper.PlayerInventoryId;
        private const ulong CarStashInventoryId = StashHelper.CarStashInventoryId;
        private const ulong StashInventoryId = StashHelper.StashInventoryId;

        private readonly Dictionary<ulong, string> _inventoryNames = new()
        {
            { 0x1, "V's Inventory" },
            { 0xF4240, "Car Stash" },
            { 0x895724, "Nomad Intro Items" },
            { 0x895956, "Street Kid Intro Items" },
            { 0x8959E8, "Corpo Intro Items" },
            { 0x38E8D0C9F9A087AE, "Panam's Stash" },
            { 0x6E48C594562422DE, "Judy's Stash" },
            { 0x7901DE03D136A5AF, "V's Wardrobe" },
            { 0xE5F556FCBB62A706, "V's Stash" },
            { 0xEDAD8C9B086A615E, "River's Stash" }
        };

        public InventoryControl(MainForm parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;

            var toolTip = new ToolTip();
            Theme.SetToolTip(toolTip, btn_AddItem, "Add any weapon, clothing, cyberware, mod or consumable to the selected container.");
            Theme.SetToolTip(toolTip, btn_MoveToStash, "Move the selected items (or all listed stashable items) between V's Inventory and V's Stash.");
            Theme.SetToolTip(toolTip, btn_CraftingComponents, "Edit quantities of all item and quickhack crafting components at once.");
            Theme.SetToolTip(toolTip, clearQuestFlagsButton, "Remove Quest/NotUnequippable flags so locked items can be dropped, stashed or disassembled.");
        }

        public string GameControlName => "Inventory";
        public string Icon => "";

        private void OnParentFormPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_parentForm.ActiveSaveFile))
            {
                if (_parentForm.ActiveSaveFile != null)
                {
                    this.InvokeIfRequired(RefreshInventories);
                }
            }
        }

        private void moneyUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (moneyUpDown.Enabled)
            {
                var playerInventory = _parentForm.ActiveSaveFile.GetInventory(1);
                playerInventory.Items[playerInventory.Items.IndexOf(playerInventory.Items.FirstOrDefault(x => x.ItemInfo.ItemId.Id.ResolvedText == "Items.money"))].Quantity = (uint)moneyUpDown.Value;
                ChangeTracker.MarkChanged();
            }
        }

        private void clearQuestFlagsButton_Click(object sender, EventArgs e)
        {
            if (_parentForm.ActiveSaveFile == null)
            {
                return;
            }

            var selectedRows = inventoryListView.SelectedVirtualItems();
            if (selectedRows.Count > 0)
            {
                var prompt = $"Clear item flags from the {selectedRows.Count} selected item(s)?\r\n\r\n" +
                             "Yes = Selected items only\r\n" +
                             "No = All items across all inventories\r\n" +
                             "Cancel = Abort";

                var result = MessageBox.Show(prompt, "Clear Item Flags", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                {
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    var clearedCount = 0;
                    foreach (var row in selectedRows)
                    {
                        if (row.Tag is ItemData item)
                        {
                            item.Flags = 0;
                            ChangeTracker.MarkChanged();
                            clearedCount++;
                        }
                    }

                    MessageBox.Show($"Cleared flags on {clearedCount} selected item(s).", "Clear Item Flags", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                if (MessageBox.Show("Clear item flags (Quest Item / Not Unequippable) on ALL items across all inventories?", "Clear Item Flags", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
            }

            foreach (SubInventory inventory in _parentForm.ActiveSaveFile.GetInventoriesContainer().SubInventories)
            {
                foreach (ItemData item in inventory.Items)
                {
                    item.Flags = 0;
                    ChangeTracker.MarkChanged();
                }
            }
            MessageBox.Show("All item flags cleared.", "Clear Item Flags", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void containersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Selected indices belong to the previous container's rows; drop them before switching.
            inventoryListView.SelectedIndices.Clear();
            btn_MoveToStash.Text = GetSelectedContainerId() == PlayerInventoryId.ToString() ? "Move to Stash" : "Move to Inventory";
            btn_MoveToStash.Invalidate();
            inventoryListView.SelectedIndices.Clear();

            if (containersListBox.SelectedIndex > -1)
            {
                RefreshInventory();
            }
            else
            {
                containerGroupBox.Visible = false;
            }
        }

        private void RefreshInventories()
        {
            containersListBox.Items.Clear();
            foreach (var container in _parentForm.ActiveSaveFile.GetInventoriesContainer().SubInventories)
            {
                var containerId = container.InventoryId.ToString();
                if (_inventoryNames.Keys.Contains(container.InventoryId))
                {
                    containerId = _inventoryNames[container.InventoryId];
                }
                containersListBox.Items.Add(containerId);
            }

            if (containersListBox.Items.Count > 0)
            {
                containersListBox.SelectedIndex = 0;
            }

            RefreshInventory();
        }

        public bool RefreshInventory()
        {
            if (inventorySearchBox.Text != "" && inventorySearchBox.Text != SearchPlaceholder)
            {
                inventorySearchBox_TextChanged(null, new EventArgs());
            }
            else
            {
                RefreshInventory("", -1);
            }
            return true;
        }

        public bool RefreshInventory(string search, int searchField)
        {
            var listViewRows = new List<ListViewItem>();
            var containerID = containersListBox.SelectedItem.ToString();

            var hasEnhancedCraft = EnhancedCraftHelper.IsInstalled(_parentForm.ActiveSaveFile);

            containerGroupBox.Visible = true;
            containerGroupBox.Text = containerID;

            containerID = GetSelectedContainerId();

            foreach (var item in _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerID)).Items)
            {
                var name = ResourceHelper.GetDisplayName(item.ItemInfo.ItemId.Id) ?? item.ItemInfo.ItemId.Id.ResolvedText;
                if (string.IsNullOrEmpty(item.ItemInfo.ItemId.Id.ResolvedText) && hasEnhancedCraft)
                {
                    // Items crafted with the EnhancedCraft mod use runtime records with a custom name
                    name = EnhancedCraftHelper.GetName(_parentForm.ActiveSaveFile, item) ?? name;
                }

                var row = new string[] { name, "", item.ItemInfo.ItemId.Id.ResolvedText, "", "1", item.ItemInfo.ItemId.Id.ResolvedText };

                if (item.IsQuantityOnly())
                {
                    row[4] = item.Quantity.ToString();
                    row[1] = "[S] ";
                }
                else if (item.IsExtendedOnly())
                {
                    row[1] = "[M] ";
                }
                else
                {
                    row[4] = item.Quantity.ToString();
                    row[1] = "[M+] ";
                }

                if (ResourceHelper.ItemClasses.TryGetValue(item.ItemInfo.ItemId.Id, out var itemData))
                {
                    row[1] += itemData.Type;
                }
                else
                {
                    row[1] += "Unknown";
                }

                if (search != "")
                {
                    if (searchField > -1)
                    {
                        if (!row[searchField].ToLower().Contains(search.ToLower()))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        var containsSearchString = false;
                        foreach (string rowItem in row)
                        {
                            if (!string.IsNullOrEmpty(rowItem) && rowItem.ToLower().Contains(search.ToLower()))
                            {
                                containsSearchString = true;
                                break;
                            }
                        }
                        if (!containsSearchString)
                        {
                            continue;
                        }
                    }
                }

                var newItem = new ListViewItem(row);
                newItem.Tag = item;
                // Only the name is colored by rarity, like in the game
                newItem.UseItemStyleForSubItems = false;
                if (Theme.GetQualityColor(itemData?.Quality) is { } qualityColor)
                {
                    newItem.SubItems[0].ForeColor = qualityColor;
                }

                listViewRows.Add(newItem);
                if (item.ItemInfo.ItemId.Id.ResolvedText == "Items.money" && containerID == "1")
                {
                    var money = item.Quantity;

                    moneyUpDown.Enabled = false;
                    if (money > moneyUpDown.Maximum)
                    {
                        moneyUpDown.Maximum = money;
                    }

                    moneyUpDown.Value = money;
                    moneyUpDown.Enabled = true;
                }
            }

            if (containerID == "1")
            {
                foreach (KeyValuePair<gameItemID, string> equipInfo in _parentForm.ActiveSaveFile.GetEquippedItems().Reverse())
                {
                    var equippedItem = listViewRows.FirstOrDefault(x => ((ItemData)x.Tag).ItemInfo.ItemId.Id == equipInfo.Key.Id);
                    if (equippedItem != null)
                    {
                        equippedItem.SubItems[3].Text = equipInfo.Value;
                        foreach (ListViewItem.ListViewSubItem subItem in equippedItem.SubItems)
                        {
                            subItem.BackColor = Theme.HighlightRow;
                        }
                        listViewRows.Remove(equippedItem);
                        listViewRows.Insert(0, equippedItem);
                    }
                }
            }

            inventoryListView.SetVirtualItems(listViewRows);
            return true;
        }

        private void inventorySearchBox_TextChanged(object sender, EventArgs e)
        {
            if (inventorySearchBox.ForeColor != Color.Silver)
            {
                var query = inventorySearchBox.Text;
                if (query.StartsWith("name:"))
                {
                    query = query.Remove(0, 5);
                    RefreshInventory(query, 0);
                }
                else if (query.StartsWith("type:"))
                {
                    query = query.Remove(0, 5);
                    RefreshInventory(query, 1);
                }
                else if (query.StartsWith("id:"))
                {
                    query = query.Remove(0, 3);
                    RefreshInventory(query, 2);
                }
                else if (query.StartsWith("slot:"))
                {
                    query = query.Remove(0, 5);
                    RefreshInventory(query, 3);
                }
                else if (query.StartsWith("quantity:"))
                {
                    query = query.Remove(0, 9);
                    RefreshInventory(query, 4);
                }
                else if (query.StartsWith("desc:"))
                {
                    query = query.Remove(0, 5);
                    RefreshInventory(query, 5);
                }
                else
                {
                    RefreshInventory(query, -1);
                }
            }
        }

        private void inventoryListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var containerId = GetSelectedContainerId();
            var isPlayerInventory = containerId == PlayerInventoryId.ToString();
            var hitTest = inventoryListView.HitTest(e.Location);

            // Right-clicking a row outside the current selection makes it the only selected row,
            // so menu actions always apply to what the user sees highlighted.
            if (hitTest.Item != null && !inventoryListView.SelectedIndices.Contains(hitTest.Item.Index))
            {
                inventoryListView.SelectedIndices.Clear();
                inventoryListView.SelectedIndices.Add(hitTest.Item.Index);
            }

            var selectedCount = inventoryListView.SelectedIndices.Count;
            gameItemID equippedSlotId = null;
            CArray<gameSEquipArea> equipAreas = null;

            if (isPlayerInventory && hitTest.Item?.Tag is ItemData activeItem && selectedCount == 1)
            {
                equippedSlotId = _parentForm.ActiveSaveFile.GetEquippedItems()
                    .FirstOrDefault(x => x.Key.Id == activeItem.ItemInfo.ItemId.Id).Key;
                if (equippedSlotId == null)
                {
                    equipAreas = _parentForm.ActiveSaveFile.GetEquipAreas();
                }
            }

            var contextMenu = InventoryContextMenuBuilder.Build(new InventoryContextMenuRequest
            {
                IsPlayerInventory = isPlayerInventory,
                IsSearchActive = IsSearchActive(),
                HasClipboardItem = _itemHolder != null,
                SelectedCount = selectedCount,
                TotalListedCount = inventoryListView.VirtualListSize,
                ClickedItem = hitTest.Item,
                EquippedSlotId = equippedSlotId,
                EquipAreas = equipAreas,
                OnCut = CutItem,
                OnCopy = CopyItem,
                OnPaste = PasteItem,
                OnMoveSelectedToStash = MoveSelectedToStash,
                OnMoveAllToStash = MoveAllToStash,
                OnMoveSelectedToInventory = MoveSelectedToInventory,
                OnMoveAllToInventory = MoveAllToInventory,
                OnSelectAll = (_, _) => SelectAllInventoryItems(),
                OnUnequip = UnequipInventoryItem,
                OnEquipInSlot = EquipInventoryItem,
                OnMakeMaxTier = MakeSelectedMaxTier,
                OnSetQuantity = SetSelectedQuantity,
                OnDelete = DeleteInventoryItem
            });

            contextMenu.Show(Cursor.Position);
        }

        private void CutItem(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem { Tag: ListViewItem { Tag: ItemData selectedItemData } })
            {
                return;
            }

            var containerId = GetSelectedContainerId();

            _itemOperation = Operation.Cut;
            _itemHolder = selectedItemData;

            _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId)).Items.Remove(selectedItemData);
            ChangeTracker.MarkChanged();
            RefreshInventory();
        }

        private void CopyItem(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem { Tag: ListViewItem { Tag: ItemData selectedItemData } })
            {
                return;
            }

            _itemOperation = Operation.Copy;
            _itemHolder = selectedItemData;
        }

        private void PasteItem(object sender, EventArgs e)
        {
            if (_itemHolder == null)
            {
                return;
            }

            var containerId = GetSelectedContainerId();
            if (string.IsNullOrEmpty(containerId))
            {
                return;
            }

            if (_itemOperation == Operation.Cut)
            {
                _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId)).Items.Add(_itemHolder);
                ChangeTracker.MarkChanged();
                RefreshInventory();

                _itemOperation = Operation.Copy;
                return;
            }

            if (_itemOperation == Operation.Copy)
            {
                var clonedItem = _parentForm.ActiveSaveFile.CloneItem(_itemHolder);
                _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId)).Items.Add(clonedItem);
                ChangeTracker.MarkChanged();
                RefreshInventory();
            }
        }

        private void EquipInventoryItem(object sender, EventArgs e)
        {
            var slot = (gameSEquipSlot)((ToolStripItem)sender).Tag;
            var currentItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
            // The slot points to this exact item, like the game does, not just to its record
            var id = currentItem.ItemInfo.ItemId;
            slot.ItemID = new gameItemID
            {
                Id = id.Id,
                RngSeed = id.RngSeed,
                UniqueCounter = id.UniqueCounter,
                Flags = id.Flags
            };
            RefreshInventory();
            ChangeTracker.MarkChanged();
            inventoryListView.SelectedVirtualItems()[0].Selected = false;
        }

        private void UnequipInventoryItem(object sender, EventArgs e)
        {
            var equipId = (gameItemID)((ToolStripItem)sender).Tag;
            foreach (var equipSlot in _parentForm.ActiveSaveFile.GetEquipSlotsFromID(equipId))
            {
                equipSlot.ItemID = new gameItemID();
            }

            RefreshInventory();
            ChangeTracker.MarkChanged();
            if (inventoryListView.SelectedVirtualItems().Count > 0)
            {
                inventoryListView.SelectedVirtualItems()[0].Selected = false;
            }
        }

        private void DeleteInventoryItem(object sender = null, EventArgs e = null)
        {
            var selectedRows = inventoryListView.SelectedVirtualItems();
            if (selectedRows.Count == 0)
            {
                return;
            }

            var containerId = GetSelectedContainerId();
            var inventory = _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId));
            var virtualInfo = inventoryListView.GetVirtualInfo();

            foreach (var row in selectedRows)
            {
                if (row.Tag is not ItemData selectedItemData)
                {
                    continue;
                }

                inventory.Items.Remove(selectedItemData);
                ChangeTracker.MarkChanged();

                if (selectedItemData.ItemInfo.ItemId.Id.ResolvedText == "Items.money" && containerId == "1")
                {
                    moneyUpDown.Enabled = false;
                    moneyUpDown.Value = 0;
                }

                virtualInfo.Items.Remove(row);
            }

            inventoryListView.SelectedIndices.Clear();
            inventoryListView.VirtualListSize = virtualInfo.Items.Count;
            inventoryListView.Invalidate();
            //statusLabel.Text = "'" + activeItem.Header.ItemId.Id.ResolvedText + "' deleted.";
        }

        private string GetSelectedContainerId()
        {
            if (containersListBox.SelectedItem == null)
            {
                return string.Empty;
            }

            var containerId = containersListBox.SelectedItem.ToString();
            if (_inventoryNames.Values.Contains(containerId))
            {
                containerId = _inventoryNames.FirstOrDefault(x => x.Value == containerId).Key.ToString();
            }

            return containerId;
        }

        private bool IsSearchActive()
        {
            return !string.IsNullOrEmpty(inventorySearchBox.Text) &&
                   !(inventorySearchBox.Text == SearchPlaceholder && inventorySearchBox.ForeColor == Color.Silver);
        }

        private void SelectAllInventoryItems()
        {
            inventoryListView.BeginUpdate();
            try
            {
                inventoryListView.SelectedIndices.Clear();
                for (var i = 0; i < inventoryListView.VirtualListSize; i++)
                {
                    inventoryListView.SelectedIndices.Add(i);
                }
            }
            finally
            {
                inventoryListView.EndUpdate();
            }
        }

        private void btn_MoveToStash_Click(object sender, EventArgs e)
        {
            if (_parentForm.ActiveSaveFile == null || containersListBox.SelectedItem == null)
            {
                return;
            }

            var containerId = GetSelectedContainerId();
            if (containerId != PlayerInventoryId.ToString())
            {
                var selectedStashRows = inventoryListView.SelectedVirtualItems();
                if (selectedStashRows.Count > 0)
                {
                    MoveItemsToInventory(selectedStashRows.Select(x => x.Tag).OfType<ItemData>().ToList());
                }
                else
                {
                    MoveAllToInventory(sender, e);
                }
                return;
            }

            var selectedRows = inventoryListView.SelectedVirtualItems();
            if (selectedRows.Count > 0)
            {
                MoveItemsToStash(selectedRows.Select(x => x.Tag).OfType<ItemData>().ToList());
                return;
            }

            var allListedItems = inventoryListView.GetVirtualInfo()?.Items
                .Select(x => x.Tag)
                .OfType<ItemData>()
                .Where(StashHelper.CanMoveToStash)
                .ToList() ?? new List<ItemData>();

            if (allListedItems.Count == 0)
            {
                MessageBox.Show("No stashable items available to move to Stash.", "Move to Stash", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var prompt = IsSearchActive()
                ? $"No items are selected. Do you want to move all {allListedItems.Count} stashable listed items to Stash?"
                : $"No items are selected. Do you want to move all {allListedItems.Count} stashable items from V's Inventory to Stash?";

            if (MessageBox.Show(prompt, "Move to Stash", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MoveItemsToStash(allListedItems);
            }
        }

        private void SetSelectedQuantity(object sender, EventArgs e)
        {
            // Weapons, clothing etc. have no quantity, only stackable items do
            var items = inventoryListView.SelectedVirtualItems()
                .Select(x => x.Tag)
                .OfType<ItemData>()
                .Where(x => x.HasQuantity())
                .ToList();

            if (items.Count == 0)
            {
                MessageBox.Show("None of the selected items can be stacked.", "Set Quantity");
                return;
            }

            var quantity = PromptQuantity(items[0].Quantity);
            if (quantity == null)
            {
                return;
            }

            foreach (var item in items)
            {
                item.Quantity = quantity.Value;
                ChangeTracker.MarkChanged();
            }

            RefreshInventory();
        }

        private uint? PromptQuantity(uint initialValue)
        {
            using var dialog = new Form
            {
                Text = "Set Quantity",
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(220, 64),
                Font = Font
            };

            var input = new NumericUpDown
            {
                Maximum = uint.MaxValue,
                Minimum = 1,
                Value = Math.Max(initialValue, 1),
                ThousandsSeparator = true,
                Location = new System.Drawing.Point(10, 10),
                Width = 200
            };
            var okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(135, 36) };

            dialog.Controls.Add(input);
            dialog.Controls.Add(okButton);
            dialog.AcceptButton = okButton;

            return dialog.ShowDialog(this) == DialogResult.OK ? (uint)input.Value : null;
        }

        private void MakeSelectedMaxTier(object sender, EventArgs e)
        {
            var saveFile = _parentForm.ActiveSaveFile;
            var updated = 0;
            var alreadyMax = 0;
            var skipped = 0;

            foreach (var item in inventoryListView.SelectedVirtualItems().Select(x => x.Tag).OfType<ItemData>())
            {
                // Only items with stat data have a tier (weapons, clothing, cyberware...)
                var statsData = saveFile.GetStatsFromItemId(item.ItemInfo.ItemId);
                if (statsData == null)
                {
                    skipped++;
                    continue;
                }

                if (saveFile.SetMaxTier(statsData))
                {
                    updated++;
                    ChangeTracker.MarkChanged();
                }
                else
                {
                    alreadyMax++;
                }
            }

            var message = $"{updated} item(s) set to Tier 5++.";
            if (alreadyMax > 0)
            {
                message += $"{Environment.NewLine}{alreadyMax} item(s) were already Tier 5++.";
            }
            if (skipped > 0)
            {
                message += $"{Environment.NewLine}{skipped} item(s) were skipped because they have no stat data (upgrade them in-game once to create it).";
            }

            MessageBox.Show(message, "Tier 5++");
        }

        private void MoveSelectedToStash(object sender, EventArgs e)
        {
            var selectedItems = inventoryListView.SelectedVirtualItems()
                .Select(x => x.Tag)
                .OfType<ItemData>()
                .ToList();

            MoveItemsToStash(selectedItems);
        }

        private void MoveAllToStash(object sender, EventArgs e)
        {
            var allListedItems = inventoryListView.GetVirtualInfo()?.Items
                .Select(x => x.Tag)
                .OfType<ItemData>()
                .Where(StashHelper.CanMoveToStash)
                .ToList() ?? new List<ItemData>();

            if (allListedItems.Count == 0)
            {
                MessageBox.Show("No stashable items available to move to Stash.", "Move to Stash", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var prompt = IsSearchActive()
                ? $"Move all {allListedItems.Count} stashable listed items to Stash?"
                : $"Move all {allListedItems.Count} stashable items from V's Inventory to Stash?";

            if (MessageBox.Show(prompt, "Move to Stash", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MoveItemsToStash(allListedItems);
            }
        }

        private void MoveItemsToStash(List<ItemData> itemsToMove)
        {
            if (_parentForm.ActiveSaveFile == null || itemsToMove == null || itemsToMove.Count == 0)
            {
                return;
            }

            var playerInventory = _parentForm.ActiveSaveFile.GetInventory(PlayerInventoryId);
            var stashInventory = GetOrCreateStashInventory(out var stashDisplayName);

            if (playerInventory == null)
            {
                MessageBox.Show("Could not find V's Inventory in the active save file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (stashInventory == null)
            {
                MessageBox.Show("Could not find or create Stash in the active save file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var (movedCount, skippedCount) = StashHelper.MoveToStash(_parentForm.ActiveSaveFile, playerInventory, stashInventory, itemsToMove);

            inventoryListView.SelectedIndices.Clear();
            RefreshInventory();
            ChangeTracker.MarkChanged();

            if (movedCount == 0)
            {
                MessageBox.Show(
                    "None of the selected items can be moved to Stash (Items such as Money, Grenades/Gadgets, Cyberware, Quickhacks/Mods, Consumables, and Quest items are skipped to prevent Stash bugs).",
                    "Move to Stash",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (skippedCount > 0)
            {
                MessageBox.Show(
                    $"Moved {movedCount} item(s) to {stashDisplayName} ({skippedCount} non-stashable item(s) such as Money, Grenades, or Quest items were skipped).",
                    "Move to Stash",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Moved {movedCount} item(s) to {stashDisplayName}.", "Move to Stash", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MoveSelectedToInventory(object sender, EventArgs e)
        {
            var selectedItems = inventoryListView.SelectedVirtualItems()
                .Select(x => x.Tag)
                .OfType<ItemData>()
                .ToList();

            MoveItemsToInventory(selectedItems);
        }

        private void MoveAllToInventory(object sender, EventArgs e)
        {
            var allListedItems = inventoryListView.GetVirtualInfo()?.Items
                .Select(x => x.Tag)
                .OfType<ItemData>()
                .ToList() ?? new List<ItemData>();

            if (allListedItems.Count == 0)
            {
                MessageBox.Show("No items available to move to V's Inventory.", "Move to Inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var containerName = containersListBox.SelectedItem?.ToString() ?? "Stash";
            var prompt = IsSearchActive()
                ? $"Move all {allListedItems.Count} listed items from {containerName} to V's Inventory?"
                : $"Move all {allListedItems.Count} items from {containerName} to V's Inventory?";

            if (MessageBox.Show(prompt, "Move to Inventory", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MoveItemsToInventory(allListedItems);
            }
        }

        private void MoveItemsToInventory(List<ItemData> itemsToMove)
        {
            if (_parentForm.ActiveSaveFile == null || itemsToMove == null || itemsToMove.Count == 0)
            {
                return;
            }

            var containerId = GetSelectedContainerId();
            if (string.IsNullOrEmpty(containerId) || containerId == PlayerInventoryId.ToString())
            {
                return;
            }

            var sourceInventory = _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId));
            var playerInventory = _parentForm.ActiveSaveFile.GetInventory(PlayerInventoryId);

            if (sourceInventory == null)
            {
                MessageBox.Show("Could not find the selected container in the active save file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (playerInventory == null)
            {
                MessageBox.Show("Could not find V's Inventory in the active save file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var movedCount = StashHelper.MoveToInventory(sourceInventory, playerInventory, itemsToMove);

            inventoryListView.SelectedIndices.Clear();
            RefreshInventory();
            ChangeTracker.MarkChanged();

            if (movedCount > 0)
            {
                MessageBox.Show($"Moved {movedCount} item(s) to V's Inventory.", "Move to Inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private SubInventory GetOrCreateStashInventory(out string stashDisplayName)
        {
            var stash = StashHelper.GetOrCreateStash(_parentForm.ActiveSaveFile, out var created);
            stashDisplayName = stash != null ? _inventoryNames[stash.InventoryId] : "Stash";

            if (created && !containersListBox.Items.Contains(stashDisplayName))
            {
                containersListBox.Items.Add(stashDisplayName);
            }

            return stash;
        }

        private void inventoryListView_DoubleClick(object sender, EventArgs e)
        {
            // Open the row under the mouse, not the first selected one, which may be a different item
            var clicked = inventoryListView.HitTest(inventoryListView.PointToClient(Cursor.Position)).Item;
            if (clicked?.Tag is ItemData item)
            {
                var activeDetails = new ItemDetails();
                activeDetails.LoadItem(item, _parentForm.ActiveSaveFile, RefreshInventory, _random);
            }
        }

        private void inventoryListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                SelectAllInventoryItems();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (inventoryListView.SelectedIndices.Count > 0 && e.KeyCode == Keys.Delete)
            {
                DeleteInventoryItem();
            }
        }

        private void SearchBoxGotFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == SearchPlaceholder && ((TextBox)sender).ForeColor == Color.Silver)
            {
                ((TextBox)sender).Text = "";
                ((TextBox)sender).ForeColor = Theme.IsDark ? Theme.DarkText : SystemColors.WindowText;
            }
        }

        private void SearchBoxLostFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).ForeColor = Color.Silver;
                ((TextBox)sender).Text = SearchPlaceholder;
            }
        }

        private void btn_CraftingComponents_Click(object sender, EventArgs e)
        {
            using var dialog = new CraftingComponentsDialog(_parentForm.ActiveSaveFile, _parentForm.ActiveSaveFile.GetInventory(PlayerInventoryId));
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                RefreshInventory();
                ChangeTracker.MarkChanged();
            }
        }

        private void btn_AddItem_Click(object sender, EventArgs e)
        {
            var containerID = GetSelectedContainerId();

            var itemDialog = new AddItem();
            itemDialog.LoadDialog(_parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerID)), _parentForm.ActiveSaveFile);

            RefreshInventory();
        }
    }
}
