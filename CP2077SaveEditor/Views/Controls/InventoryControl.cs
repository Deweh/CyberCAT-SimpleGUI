using CP2077SaveEditor.Extensions;
using CP2077SaveEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Save.InventoryHelper;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class InventoryControl : UserControl, IGameControl
    {
        private readonly Form2 _parentForm;

        private Random _random = new();

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
                ((SimpleItemData)playerInventory.Items[playerInventory.Items.IndexOf(playerInventory.Items.Where(x => x.Header.ItemId.Id.ResolvedText == "Items.money").FirstOrDefault())].Data).Quantity = (uint)moneyUpDown.Value;
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
                if (item.Header.ItemStructure != 0)
                {

                }

                var name = item.Header.ItemId.Id.ResolvedText;
                if (string.IsNullOrEmpty(name) && hasEnhancedCraft)
                {
                    name = EnhancedCraftHelper.GetName(_parentForm.ActiveSaveFile, item);
                    if (name != null)
                    {

                    }
                }

                var row = new string[] { name, "", item.Header.ItemId.Id.ResolvedText, "", "1", item.Header.ItemId.Id.ResolvedText };

                if (item.Data.GetType() == typeof(SimpleItemData))
                {
                    row[4] = ((SimpleItemData)item.Data).Quantity.ToString();
                    row[1] = "[S] ";
                }
                else if (item.Data.GetType() == typeof(ModableItemWithQuantityData))
                {
                    row[4] = ((ModableItemWithQuantityData)item.Data).Quantity.ToString();
                    row[1] = "[M+] ";
                }
                else
                {
                    row[1] = "[M] ";
                }

                if (ResourceHelper.ItemClasses.TryGetValue(item.Header.ItemId.Id, out var itemData))
                {
                    row[1] += itemData.Type;

                    /*if (itemData.IsSingleInstance)
                    {
                        if (itemData.Type == "Grenade")
                        {
                            row[1] += " [M+]";
                        }
                        else
                        {
                            row[1] += " [S]";
                        }
                    }
                    else
                    {
                        row[1] += " [M]";
                    }*/
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
                if (item.Header.ItemId.Id.ResolvedText == "Items.money" && containerID == "1")
                {
                    var money = ((SimpleItemData)item.Data).Quantity;

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
                    var equippedItem = listViewRows.FirstOrDefault(x => ((ItemData)x.Tag).Header.ItemId.Id == equipInfo.Key.Id);
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

        private void inventoryListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && inventoryListView.SelectedIndices.Count > 0)
            {
                var containerId = containersListBox.SelectedItem.ToString();
                if (_inventoryNames.Values.Contains(containerId))
                {
                    containerId = _inventoryNames.FirstOrDefault(x => x.Value == containerId).Key.ToString();
                }

                var contextMenu = new ContextMenuStrip();
                if (Global.IsDebug)
                {
                    contextMenu.Items.Add("New Item").Click += (object sender, EventArgs e) =>
                    {
                        var inventory = _parentForm.ActiveSaveFile.GetInventory(1);
                        //inventory.Items.Add(inventory.Items.Last().CreateSimpleItem());
                    };
                }

                if (containerId == "1")
                {
                    var activeItem = (InventoryHelper.ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
                    var equipSlot = _parentForm.ActiveSaveFile.GetEquippedItems().FirstOrDefault(x => x.Key.Id == activeItem.Header.ItemId.Id).Key;

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
                contextMenu.Items.Add("Delete").Click += DeleteInventoryItem;
                contextMenu.Show(Cursor.Position);
            }
        }

        private void EquipInventoryItem(object sender, EventArgs e)
        {
            var slot = (gameSEquipSlot)((ToolStripItem)sender).Tag;
            var currentItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
            slot.ItemID = new gameItemID() { Id = currentItem.Header.ItemId.Id };
            RefreshInventory();
            inventoryListView.SelectedVirtualItems()[0].Selected = false;
        }

        private void UnequipInventoryItem(object sender, EventArgs e)
        {
            var equipId = (gameItemID)((ToolStripItem)sender).Tag;
            foreach (var equipSlot in _parentForm.ActiveSaveFile.GetEquipSlotsFromID(equipId))
            {
                equipSlot.ItemID = null;
            }

            RefreshInventory();
            if (inventoryListView.SelectedVirtualItems().Count > 0)
            {
                inventoryListView.SelectedVirtualItems()[0].Selected = false;
            }
        }

        private void DeleteInventoryItem(object sender = null, EventArgs e = null)
        {
            var containerId = containersListBox.SelectedItem.ToString();
            if (_inventoryNames.Values.Contains(containerId))
            {
                containerId = _inventoryNames.FirstOrDefault(x => x.Value == containerId).Key.ToString();
            }

            var activeItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
            _parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerId)).Items.Remove(activeItem);

            if (((ItemData)inventoryListView.SelectedVirtualItems()[0].Tag).Header.ItemId.Id.ResolvedText == "Items.money" && containerId == "1")
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
            itemDialog.LoadDialog(_parentForm.ActiveSaveFile.GetInventory(ulong.Parse(containerID)));

            RefreshInventory();
        }
    }
}
