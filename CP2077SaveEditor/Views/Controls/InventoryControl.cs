using CP2077SaveEditor.Extensions;
using CP2077SaveEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
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

        private readonly Form2 _parentForm;

        private Random _random = new();

        private Operation _itemOperation;
        private ItemData _itemHolder;

        private string debloatInfo = "";
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

        public InventoryControl(Form2 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;
        }

        public string GameControlName => "Inventory";

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
            }
        }

        private void clearQuestFlagsButton_Click(object sender, EventArgs e)
        {
            foreach (SubInventory inventory in _parentForm.ActiveSaveFile.GetInventoriesContainer().SubInventories)
            {
                foreach (ItemData item in inventory.Items)
                {
                    item.Flags = 0;
                }
            }
            MessageBox.Show("All item flags cleared.");
        }

        /// <summary>
        /// Checks if an item is safe to consider for duplicate removal
        /// </summary>
        private bool IsSafeToRemoveDuplicates(ItemData item)
        {
            var itemId = item.ItemInfo.ItemId.Id.ResolvedText;

            if (string.IsNullOrEmpty(itemId))
                return false;

            // Currently only clothing items are considered safe (for wardrobe cleanup)
            return itemId.StartsWith("Items.Formal") ||
                   itemId.StartsWith("Items.Casual") ||
                   itemId.StartsWith("Items.Boots") ||
                   itemId.StartsWith("Items.Jacket") ||
                   itemId.StartsWith("Items.Pants") ||
                   itemId.StartsWith("Items.Shirt") ||
                   itemId.StartsWith("Items.Shoes") ||
                   itemId.StartsWith("Items.Skirt") ||
                   itemId.StartsWith("Items.Dress") ||
                   itemId.StartsWith("Items.Hat") ||
                   itemId.StartsWith("Items.Glasses") ||
                   itemId.StartsWith("Items.Mask") ||
                   itemId.StartsWith("Items.Vest") ||
                   itemId.StartsWith("Items.Shorts") ||
                   itemId.StartsWith("Items.Sweater") ||
                   itemId.StartsWith("Items.Tank") ||
                   itemId.StartsWith("Items.Top") ||
                   itemId.StartsWith("Items.Underwear") ||
                   itemId.StartsWith("Items.Bra") ||
                   itemId.StartsWith("Items.Panties");
        }

        /// <summary>
        /// Items are considered duplicates if they have the same base item, quality level, and mods.
        /// </summary>
        private string GetItemUniqueKey(ItemData item)
        {
            var key = item.ItemInfo.ItemId.Id.ResolvedText;

            // Add quality information if available (this is what determines legendary vs common)
            if (item.ItemAdditionalInfo != null)
            {
                key += $"|LootPool:{item.ItemAdditionalInfo.LootItemPoolId}";
                key += $"|Level:{item.ItemAdditionalInfo.RequiredLevel}";
            }

            // Add item structure information (affects how item behaves)
            key += $"|Structure:{item.ItemInfo.ItemStructure}";

            // Add flags (quest items, etc. - these matter for functionality)
            key += $"|Flags:{item.Flags}";

            // Add quantity for stackable items (different quantities are different items)
            key += $"|Qty:{item.Quantity}";

            return key;
        }

        /// <summary>
        /// Keeps the first occurrence of each unique item and removes all subsequent duplicates.
        /// </summary>
        private void RemoveDuplicates(object sender, EventArgs e)
        {
            // Get the currently selected inventory
            var currentContainerId = containersListBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(currentContainerId))
            {
                MessageBox.Show("Please select an inventory first.", "No Inventory Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Convert display name back to ID if needed
            if (_inventoryNames.Values.Contains(currentContainerId))
            {
                currentContainerId = _inventoryNames.FirstOrDefault(x => x.Value == currentContainerId).Key.ToString();
            }

            // Get the current inventory name for display
            var currentInventoryName = _inventoryNames.ContainsKey(ulong.Parse(currentContainerId))
                ? _inventoryNames[ulong.Parse(currentContainerId)]
                : $"Inventory {currentContainerId}";

            // Display warning message including current limitations
            var dialogMessage = $@"Choose duplicate removal scope:

            Yes - Remove duplicates from ALL inventories
            No - Remove duplicates from {currentInventoryName} only
            Cancel - Abort

            WARNING: This is a dangerous operation! For safety, this currently
            only removes clothing duplicates (for wardrobe cleanup).";

            var result = MessageBox.Show(dialogMessage, "Remove Duplicates", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                return;
            }

            var removeFromAll = result == DialogResult.Yes;
            var totalRemoved = 0;
            var duplicatesFound = new Dictionary<string, int>();
            var inventoriesProcessed = new List<string>();

            var inventoriesToProcess = removeFromAll
                ? _parentForm.ActiveSaveFile.GetInventoriesContainer().SubInventories
                : new List<SubInventory> {
                    _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(currentContainerId))
                  }.Where(x => x != null);

            foreach (SubInventory inventory in inventoriesToProcess)
            {
                if (inventory == null) continue;

                var inventoryName = _inventoryNames.ContainsKey(inventory.InventoryId)
                    ? _inventoryNames[inventory.InventoryId]
                    : $"Inventory {inventory.InventoryId:X}";

                var itemsToRemove = new List<ItemData>();
                var seenItems = new HashSet<string>();

                foreach (ItemData item in inventory.Items)
                {
                    var itemId = item.ItemInfo.ItemId.Id.ResolvedText;

                    // Only process items that are safe to remove duplicates from
                    if (!IsSafeToRemoveDuplicates(item))
                    {
                        continue;
                    }

                    var itemUniqueKey = GetItemUniqueKey(item);

                    if (seenItems.Contains(itemUniqueKey))
                    {
                        // This is a duplicate, mark for removal
                        itemsToRemove.Add(item);
                        totalRemoved++;

                        if (duplicatesFound.ContainsKey(itemId))
                        {
                            duplicatesFound[itemId]++;
                        }
                        else
                        {
                            duplicatesFound[itemId] = 2; // First duplicate found
                        }
                    }
                    else
                    {
                        seenItems.Add(itemUniqueKey);
                    }
                }

                // Remove the duplicate items
                foreach (var itemToRemove in itemsToRemove)
                {
                    inventory.Items.Remove(itemToRemove);
                }

                if (itemsToRemove.Count > 0)
                {
                    inventoriesProcessed.Add($"{inventoryName}: {itemsToRemove.Count} duplicates removed");
                }
            }

            // Show results
            var scope = removeFromAll ? "all inventories" : $"{currentInventoryName.ToLower()} only";
            var message = $"Duplicate removal complete!\n\nScope: {scope}\nTotal duplicates removed: {totalRemoved}";

            if (duplicatesFound.Count > 0)
            {
                message += $"\n\nItems with duplicates found: {duplicatesFound.Count}";
                if (duplicatesFound.Count <= 15) // Show details for reasonable numbers
                {
                    message += "\n\nDuplicate items:";
                    foreach (var kvp in duplicatesFound.OrderByDescending(x => x.Value))
                    {
                        message += $"\n• {kvp.Key} ({kvp.Value} copies)";
                    }
                }
                else
                {
                    message += $"\n\nTop 10 most duplicated items:";
                    foreach (var kvp in duplicatesFound.OrderByDescending(x => x.Value).Take(10))
                    {
                        message += $"\n• {kvp.Key} ({kvp.Value} copies)";
                    }
                }
            }

            if (inventoriesProcessed.Count > 0)
            {
                message += "\n\nInventories processed:";
                foreach (var inv in inventoriesProcessed)
                {
                    message += $"\n• {inv}";
                }
            }

            MessageBox.Show(message, "Duplicate Removal Results", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Refresh the inventory display
            RefreshInventory();
        }

        private void debloatButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This process will remove redundant data from your save. Just in case, it's recommended that you back up your save before continuing. Continue?", "Notice", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            debloatWorker.RunWorkerAsync();
            debloatTimer.Start();

            //editorPanel.Enabled = false;
            //optionsPanel.Enabled = false;
            //openSaveButton.Enabled = false;
            //swapSaveType.Enabled = false;
        }

        private void debloatWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //var entryCounter = 1;
            //foreach (CyberCAT.Core.Classes.DumpedClasses.GameSavedStatsData stats in activeSaveFile.GetStatsMap().Values)
            //{
            //    if (stats.StatModifiers != null)
            //    {
            //        var craftedModifiers = stats.StatModifiers.Where(x => x.Value.StatType == gamedataStatType.IsItemCrafted);

            //        if (craftedModifiers != null && craftedModifiers.Count() > 100)
            //        {
            //            debloatInfo = "DE-BLOAT IN PROGRESS :: (1/2) :: Entry: " + entryCounter.ToString() + "/" + activeSaveFile.GetStatsMap().Values.Length.ToString();

            //            var ids = new HashSet<uint>();
            //            foreach (Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData> modifierData in craftedModifiers)
            //            {
            //                ids.Add(modifierData.Id);
            //            }

            //            activeSaveFile.GetStatsContainer().RemoveHandles(ids);
            //            stats.StatModifiers = new Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData>[] { };
            //        }
            //    }
            //    entryCounter++;
            //}

            //entryCounter = 0;
            //uint handleInd = 1;
            //foreach (CyberCAT.Core.Classes.DumpedClasses.GameSavedStatsData value in activeSaveFile.GetStatsMap().Values)
            //{
            //    if (value.StatModifiers != null && value.StatModifiers.Count() > 0)
            //    {
            //        var handleCounter = 0;
            //        foreach (Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData> modifierData in value.StatModifiers)
            //        {
            //            modifierData.Id = handleInd;
            //            debloatInfo = "DE-BLOAT IN PROGRESS :: (2/2) :: Entry: " + entryCounter.ToString() + "/" + activeSaveFile.GetStatsMap().Values.Length.ToString() + " -- Handle: " + handleCounter.ToString() + "/" + value.StatModifiers.Count().ToString();
            //            handleInd++; handleCounter++;
            //        }
            //    }
            //    entryCounter++;
            //}
        }

        private void debloatWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            debloatTimer.Stop();
            //editorPanel.Enabled = true;
            //optionsPanel.Enabled = true;
            //openSaveButton.Enabled = true;
            //swapSaveType.Enabled = true;
            //
            //statusLabel.Text = "De-bloat complete.";
            MessageBox.Show("De-bloat complete.");
        }

        private void debloatTimer_Tick(object sender, EventArgs e)
        {
            //statusLabel.Text = debloatInfo;
        }

        private void containersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            if (inventorySearchBox.Text != "" && inventorySearchBox.Text != "Search")
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

            if (_inventoryNames.Values.Contains(containerID))
            {
                containerID = _inventoryNames.FirstOrDefault(x => x.Value == containerID).Key.ToString();
            }

            foreach (var item in _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerID)).Items)
            {
                var name = item.ItemInfo.ItemId.Id.ResolvedText;
                if (string.IsNullOrEmpty(name) && hasEnhancedCraft)
                {
                    name = EnhancedCraftHelper.GetName(_parentForm.ActiveSaveFile, item);
                    if (name != null)
                    {

                    }
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

                listViewRows.Add(newItem);
                if (item.ItemInfo.ItemId.Id.ResolvedText == "Items.money" && containerID == "1")
                {
                    var money = item.Quantity;

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
                        equippedItem.BackColor = Color.FromArgb(248, 248, 248);
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
                else if (query.StartsWith("quantity:"))
                {
                    query = query.Remove(0, 9);
                    RefreshInventory(query, 3);
                }
                else if (query.StartsWith("desc:"))
                {
                    query = query.Remove(0, 5);
                    RefreshInventory(query, 4);
                }
                else
                {
                    RefreshInventory(query, -1);
                }
            }
        }

        private void inventoryListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var containerId = containersListBox.SelectedItem.ToString();
                if (_inventoryNames.Values.Contains(containerId))
                {
                    containerId = _inventoryNames.FirstOrDefault(x => x.Value == containerId).Key.ToString();
                }

                var contextMenu = new ContextMenuStrip();

                var hitTest = inventoryListView.HitTest(e.Location);
                //if (hitTest.Item != null)
                //{
                //    contextMenu.Items.Add("Cut", null, CutItem).Tag = hitTest.Item;
                //    contextMenu.Items.Add("Copy", null, CopyItem).Tag = hitTest.Item;
                //}
                //
                //if (_itemHolder != null)
                //{
                //    contextMenu.Items.Add("Paste").Click += PasteItem;
                //}

                if (containerId == "1" && hitTest.Item != null)
                {
                    var activeItem = (ItemData)hitTest.Item.Tag;
                    var equipSlot = _parentForm.ActiveSaveFile.GetEquippedItems().FirstOrDefault(x => x.Key.Id == activeItem.ItemInfo.ItemId.Id).Key;

                    if (equipSlot != null)
                    {
                        var unequipItem = contextMenu.Items.Add("Unequip");
                        unequipItem.Tag = equipSlot;
                        unequipItem.Click += UnequipInventoryItem;
                    }
                    else
                    {
                        var equipIn = new ToolStripMenuItem("Equip in Slot");
                        foreach (var area in _parentForm.ActiveSaveFile.GetEquipAreas())
                        {
                            int counter = 1;
                            foreach (var slot in area.EquipSlots)
                            {
                                var slotItem = equipIn.DropDownItems.Add(area.AreaType.ToString() + " " + counter.ToString());
                                slotItem.Tag = slot;
                                slotItem.Click += EquipInventoryItem;
                                counter++;
                            }
                        }
                        contextMenu.Items.Add(equipIn);
                    }
                }

                if (hitTest.Item != null)
                {
                    contextMenu.Items.Add("Delete", null, DeleteInventoryItem).Tag = hitTest.Item;
                }

                var removeDuplicatesItem = contextMenu.Items.Add("Remove Duplicates");
                removeDuplicatesItem.Click += RemoveDuplicates;

                contextMenu.Show(Cursor.Position);
            }
        }

        private void CutItem(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem { Tag: ListViewItem { Tag: ItemData selectedItemData } selectedItem })
            {
                return;
            }

            var containerId = containersListBox.SelectedItem.ToString();
            if (_inventoryNames.Values.Contains(containerId))
            {
                containerId = _inventoryNames.FirstOrDefault(x => x.Value == containerId).Key.ToString();
            }

            _itemOperation = Operation.Cut;
            _itemHolder = selectedItemData;

            _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId)).Items.Remove(selectedItemData);
            RefreshInventory();
        }

        private void CopyItem(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem { Tag: ListViewItem { Tag: ItemData selectedItemData } selectedItem })
            {
                return;
            }

            _itemOperation = Operation.Cut;
            _itemHolder = selectedItemData;
        }

        private void PasteItem(object sender, EventArgs e)
        {
            if (_itemHolder == null)
            {
                return;
            }

            var containerId = containersListBox.SelectedItem.ToString();
            if (_inventoryNames.Values.Contains(containerId))
            {
                containerId = _inventoryNames.FirstOrDefault(x => x.Value == containerId).Key.ToString();
            }

            if (_itemOperation == Operation.Cut)
            {
                _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId)).Items.Add(_itemHolder);
                RefreshInventory();

                _itemOperation = Operation.Copy;
                return;
            }

            if (_itemOperation == Operation.Copy)
            {

            }
        }

        private void EquipInventoryItem(object sender, EventArgs e)
        {
            var slot = (gameSEquipSlot)((ToolStripItem)sender).Tag;
            var currentItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
            slot.ItemID = new gameItemID() { Id = currentItem.ItemInfo.ItemId.Id };
            RefreshInventory();
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
            if (inventoryListView.SelectedVirtualItems().Count > 0)
            {
                inventoryListView.SelectedVirtualItems()[0].Selected = false;
            }
        }

        private void DeleteInventoryItem(object sender = null, EventArgs e = null)
        {
            if (sender is not ToolStripMenuItem { Tag: ListViewItem { Tag: ItemData selectedItemData } selectedItem })
            {
                return;
            }

            var containerId = containersListBox.SelectedItem.ToString();
            if (_inventoryNames.Values.Contains(containerId))
            {
                containerId = _inventoryNames.FirstOrDefault(x => x.Value == containerId).Key.ToString();
            }

            _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId)).Items.Remove(selectedItemData);

            if (selectedItemData.ItemInfo.ItemId.Id.ResolvedText == "Items.money" && containerId == "1")
            {
                moneyUpDown.Enabled = false;
                moneyUpDown.Value = 0;
            }

            inventoryListView.RemoveVirtualItem(inventoryListView.SelectedVirtualItems()[0]);
            //statusLabel.Text = "'" + activeItem.Header.ItemId.Id.ResolvedText + "' deleted.";
        }

        private void inventoryListView_DoubleClick(object sender, EventArgs e)
        {
            if (inventoryListView.SelectedIndices.Count > 0)
            {
                var activeDetails = new ItemDetails();
                activeDetails.LoadItem((ItemData)inventoryListView.SelectedVirtualItems()[0].Tag, _parentForm.ActiveSaveFile, RefreshInventory, _random);
            }
        }

        private void inventoryListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (inventoryListView.SelectedIndices.Count > 0 && e.KeyCode == Keys.Delete)
            {
                DeleteInventoryItem();
            }
        }

        private void SearchBoxGotFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "Search" && ((TextBox)sender).ForeColor == Color.Silver)
            {
                ((TextBox)sender).Text = "";
                ((TextBox)sender).ForeColor = Color.Black;
            }
        }

        private void SearchBoxLostFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).ForeColor = Color.Silver;
                ((TextBox)sender).Text = "Search";
            }
        }

        private void btn_AddItem_Click(object sender, EventArgs e)
        {
            var containerID = containersListBox.SelectedItem.ToString();
            if (_inventoryNames.Values.Contains(containerID))
            {
                containerID = _inventoryNames.FirstOrDefault(x => x.Value == containerID).Key.ToString();
            }

            var itemDialog = new AddItem();
            itemDialog.LoadDialog(_parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerID)), _parentForm.ActiveSaveFile);

            RefreshInventory();
        }
    }
}
