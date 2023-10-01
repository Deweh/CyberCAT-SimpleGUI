using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;
using WolvenKit.RED4.Save.Classes;

namespace CP2077SaveEditor
{
    public partial class ItemDetails : Form
    {
        private Func<bool> callbackFunc1;
        private ItemData activeItem;
        private SaveFileHelper activeSaveFile;
        private bool statsOnly = false;
        private bool _autoUpdate;

        public ItemDetails()
        {
            InitializeComponent();

            modsTreeView.NodeMouseDoubleClick += modsTreeView_DoubleClick;
            modsTreeView.KeyDown += modsTreeView_KeyDown;

            statsListView.DoubleClick += statsListView_DoubleClick;
            statsListView.KeyDown += statsListView_KeyDown;

            quantityUpDown.ValueChanged += ApplyableControlChanged;
            unknownFlag1CheckBox.CheckedChanged += ApplyableControlChanged;
            questItemCheckBox.CheckedChanged += ApplyableControlChanged;

            //Shown += (object s, EventArgs e) =>
            //{
            //    var record = Form1.tdbService.GetRecord(activeItem.ItemTdbId);

            //    foreach(var member in record.GetEnumerator())
            //    {
            //        MessageBox.Show(member.propPath + ": " + member.value.ToString());
            //    }
            //};
        }

        private void IterativeBuildModTree(ItemSlotPart nodeData, TreeNode rootNode)
        {
            foreach (var childNode in nodeData.Children)
            {
                var newNode = rootNode.Nodes.Add(childNode.AttachmentSlotTdbId.ResolvedText + " :: " + childNode.ItemInfo.ItemId.Id.ResolvedText + " [" + childNode.Children.Count + "]");
                newNode.Tag = childNode;
                if (childNode.Children.Count > 0)
                {
                    IterativeBuildModTree(childNode, newNode);
                }
            }
        }

        private void IterativeDeleteModNode(ItemSlotPart targetNode, ItemSlotPart rootNode)
        {

            if (rootNode.Children.Contains(targetNode))
            {
                rootNode.Children.Remove(targetNode);
            }
            else
            {
                foreach (var childNode in rootNode.Children)
                {
                    if (childNode.Children.Count > 0)
                    {
                        IterativeDeleteModNode(targetNode, childNode);
                    }
                }
            }
        }

        public bool ReloadData()
        {
            if (!statsOnly)
            {
                if (activeItem.IsQuantityOnly())
                {
                    //SimpleItemData parsing
                    this.Text = activeItem.ItemInfo.ItemId.Id.ResolvedText + " (Simple Item)";

                    quantityUpDown.Value = activeItem.Quantity;
                }

                if (activeItem.HasExtendedData())
                {
                    //ModableItemData parsing
                    this.Text = activeItem.ItemInfo.ItemId.Id.ResolvedText + " (Modable Item)";

                    if (activeItem.HasQuantity())
                    {
                        quantityUpDown.Value = activeItem.Quantity;
                    }
                    else
                    {
                        basicInfoGroupBox.Enabled = false;
                        quantityUpDown.Value = 1;
                    }

                    quickActionsGroupBox.Enabled = true;

                    txt_LootItemId.Text = ((ulong)activeItem.ItemAdditionalInfo!.LootItemPoolId).ToString();
                    unknown1Box.Text = activeItem.ItemAdditionalInfo!.Unknown2.ToString();
                    unknown3Box.Text = activeItem.ItemAdditionalInfo!.RequiredLevel.ToString();

                    modsTreeView.Nodes.Clear();
                    var rootNode = modsTreeView.Nodes.Add(activeItem.ItemSlotPart!.AttachmentSlotTdbId.ResolvedText, activeItem.ItemSlotPart.AttachmentSlotTdbId.ResolvedText + " :: " + activeItem.ItemSlotPart.ItemInfo.ItemId.Id.ResolvedText + " [" + activeItem.ItemSlotPart.Children.Count.ToString() + "]");
                    rootNode.Tag = activeItem.ItemSlotPart;
                    IterativeBuildModTree(activeItem.ItemSlotPart, rootNode);
                }
                else
                {
                    detailsTabControl.TabPages.Remove(additionalInfoTab);
                    detailsTabControl.TabPages.Remove(modInfoTab);
                }
            }

            //Stats parsing
            if (Global.StatsSystemEnabled)
            {
                var statsData = activeSaveFile.GetItemStatData(activeItem);
                if (statsData == null)
                {
                    detailsTabControl.TabPages.Remove(statsTab);
                }
                else
                {
                    detailsTabControl.TabPages.Remove(statsPlaceholderTab);
                    statsListView.Items.Clear();
                    var listRows = new List<ListViewItem>();
                    if (statsData.ModifiersBuffer?.Data is ModifiersBuffer modBuffer)
                    {
                        foreach (gameStatModifierData_Deprecated modifier in modBuffer.Entries)
                        {
                            var row = new string[] { "Constant", modifier.ModifierType.ToString(), modifier.StatType.ToString(), "" };

                            if (modifier is gameCombinedStatModifierData_Deprecated combinedData)
                            {
                                row[0] = "Combined";
                                row[3] = combinedData.Value.ToString();
                            }
                            else if (modifier is gameConstantStatModifierData_Deprecated constantData)
                            {
                                row[3] = constantData.Value.ToString();
                            }
                            else
                            {
                                row[0] = "Curve";
                            }

                            var newItem = new ListViewItem(row);
                            newItem.Tag = modifier;
                            listRows.Add(newItem);
                        }

                        statsListView.BeginUpdate();
                        statsListView.Items.AddRange(listRows.ToArray());
                        statsListView.EndUpdate();
                    }
                    else
                    {
                        statsData.ModifiersBuffer = new DataBuffer { Data = new ModifiersBuffer() };
                    }
                }
            }
            else
            {
                detailsTabControl.TabPages.Remove(statsTab);
                detailsTabControl.TabPages.Remove(statsPlaceholderTab);
            }


            if (!statsOnly)
            {
                unknownFlag1CheckBox.Checked = activeItem.Flags.HasFlag(ItemFlag.IsNotUnequippable);
                questItemCheckBox.Checked = activeItem.Flags.HasFlag(ItemFlag.IsQuestItem);
            }
            return true;
        }

        public void LoadItem(ItemData item, object _saveFile, Func<bool> callback1, Random rand)
        {
            callbackFunc1 = callback1;
            activeItem = item;
            activeSaveFile = (SaveFileHelper)_saveFile;
            ReloadData();

            this.ShowDialog();
        }

        public void LoadStatsOnly(uint seed, object _saveFile, string name)
        {
            if (!Global.StatsSystemEnabled)
            {
                MessageBox.Show("Stats system disabled.");
                this.Close();
            }

            callbackFunc1 = delegate { return true; };
            var dummyItem = new ItemData();
            dummyItem.ItemInfo = new() { ItemId = new() { RngSeed = seed } };
            activeItem = dummyItem;
            activeSaveFile = (SaveFileHelper)_saveFile;
            this.Text = name;

            var newHeight = this.Height - 190;
            basicInfoGroupBox.Visible = false;
            flagsGroupBox.Visible = false;
            applyButton.Visible = false;
            detailsTabControl.TabPages.Remove(additionalInfoTab);
            detailsTabControl.TabPages.Remove(modInfoTab);

            if (name == "Player")
            {
                quickActionsGroupBox.Visible = false;
            }
            else
            {
                newHeight += 60;
            }

            this.Height = newHeight;
            statsOnly = true;
            ReloadData();

            this.CenterToParent();
            this.ShowDialog();
        }

        private void pasteLegendaryIdButton_Click(object sender, EventArgs e)
        {
            var statData = activeSaveFile.GetItemStatData(activeItem);
            if (statData == null)
            {
                if (MessageBox.Show("Item has no stat data. To fix this, please upgrade the item in-game at least once.", "Notice", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //modsBaseIdBox.Text = ((ulong)88400986533).ToString();
                }
            }
            else
            {
                activeSaveFile.SetConstantStat(gamedataStatType.Quality, 4, statData);
                activeSaveFile.SetConstantStat(gamedataStatType.RandomCurveInput, 0, statData, gameStatModifierType.Multiplier);

                ReloadData();
                MessageBox.Show("Legendary quality stat applied.");
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (!statsOnly)
            {
                if (activeItem.HasExtendedData())
                {
                    // TODO: Check this
                    // activeItem.ItemAdditionalInfo = activeItem.ItemSlotPart!.ItemAdditionalInfo;

                    activeItem.ItemAdditionalInfo!.LootItemPoolId = ulong.Parse(txt_LootItemId.Text);
                    activeItem.ItemAdditionalInfo.Unknown2 = uint.Parse(unknown1Box.Text);
                    activeItem.ItemAdditionalInfo.RequiredLevel = float.Parse(unknown3Box.Text);
                }

                if (activeItem.HasQuantity())
                {
                    activeItem.Quantity = (uint)quantityUpDown.Value;
                }

                if (unknownFlag1CheckBox.Checked)
                {
                    // For readability: this adds the flag.
                    activeItem.Flags |= ItemFlag.IsNotUnequippable;
                }
                else
                {
                    // For readability: this clears the flag.
                    activeItem.Flags &= ~ItemFlag.IsNotUnequippable;
                }

                if (questItemCheckBox.Checked)
                {
                    activeItem.Flags |= ItemFlag.IsQuestItem;
                }
                else
                {
                    activeItem.Flags &= ~ItemFlag.IsQuestItem;
                }

                callbackFunc1();
                applyButton.Enabled = false;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void modsTreeView_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var nodeDetails = new ModNodeDetails();
            nodeDetails.LoadNode(((ItemSlotPart)e.Node.Tag), ReloadData, activeSaveFile);
        }

        private void statsListView_DoubleClick(object sender, EventArgs e)
        {
            if (statsListView.SelectedItems.Count > 0)
            {
                var nodeDetails = new StatDetails();
                nodeDetails.LoadStat((gameStatModifierData_Deprecated)statsListView.SelectedItems[0].Tag, ReloadData);
            }
        }

        private void modsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && modsTreeView.SelectedNode != null)
            {

                if (activeItem.ItemSlotPart != (ItemSlotPart)modsTreeView.SelectedNode.Tag)
                {
                    IterativeDeleteModNode(((ItemSlotPart)modsTreeView.SelectedNode.Tag), activeItem.ItemSlotPart);
                    modsTreeView.SelectedNode.Remove();
                }
                else
                {

                    activeItem.ItemSlotPart.Children.Clear();
                    activeItem.ItemSlotPart.AttachmentSlotTdbId = 0;
                    activeItem.ItemSlotPart.ItemInfo.ItemId.Id = 0;
                    activeItem.ItemSlotPart.ItemAdditionalInfo.LootItemPoolId = 0;
                    activeItem.ItemSlotPart.ItemAdditionalInfo.Unknown2 = 0;
                    activeItem.ItemSlotPart.ItemAdditionalInfo.RequiredLevel = 0;
                    activeItem.ItemSlotPart.Unknown2 = 0;
                    activeItem.ItemSlotPart.AppearanceName = "";
                    ReloadData();
                }
            }
        }

        private void statsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && statsListView.SelectedItems.Count > 0)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                activeSaveFile.RemoveStat((gameStatModifierData_Deprecated)statsListView.SelectedItems[0].Tag, activeSaveFile.GetItemStatData(activeItem));
                statsListView.Items.Remove(statsListView.SelectedItems[0]);
            }
        }

        private void addConstantStatButton_Click(object sender, EventArgs e)
        {
            var newId = activeSaveFile.AddStat(typeof(gameConstantStatModifierData_Deprecated), activeSaveFile.GetItemStatData(activeItem));
            ReloadData();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newId, ReloadData);
        }

        private void removeStatButton_Click(object sender, EventArgs e)
        {
            if (statsListView.SelectedItems.Count > 0)
            {
                activeSaveFile.RemoveStat((gameStatModifierData_Deprecated)statsListView.SelectedItems[0].Tag, activeSaveFile.GetItemStatData(activeItem));
                statsListView.Items.Remove(statsListView.SelectedItems[0]);
            }

        }

        private void addCombinedStatButton_Click(object sender, EventArgs e)
        {
            var newId = activeSaveFile.AddStat(typeof(gameCombinedStatModifierData_Deprecated), activeSaveFile.GetItemStatData(activeItem));
            ReloadData();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newId, ReloadData);
        }

        private void addCurveStatButton_Click(object sender, EventArgs e)
        {
            var newId = activeSaveFile.AddStat(typeof(gameCurveStatModifierData_Deprecated), activeSaveFile.GetItemStatData(activeItem));
            ReloadData();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newId, ReloadData);
        }

        private void deleteModNodeButton_Click(object sender, EventArgs e)
        {
            if (modsTreeView.SelectedNode != null)
            {

                if (activeItem.ItemSlotPart != (ItemSlotPart)modsTreeView.SelectedNode.Tag)
                {
                    IterativeDeleteModNode(((ItemSlotPart)modsTreeView.SelectedNode.Tag), activeItem.ItemSlotPart);
                    modsTreeView.SelectedNode.Remove();
                }
                else
                {

                    activeItem.ItemSlotPart.Children.Clear();
                    activeItem.ItemSlotPart.AttachmentSlotTdbId = 0;
                    activeItem.ItemSlotPart.ItemInfo.ItemId.Id = 0;
                    activeItem.ItemSlotPart.ItemAdditionalInfo.LootItemPoolId = 0;
                    activeItem.ItemSlotPart.ItemAdditionalInfo.Unknown2 = 0;
                    activeItem.ItemSlotPart.ItemAdditionalInfo.RequiredLevel = 0;
                    activeItem.ItemSlotPart.Unknown2 = 0;
                    activeItem.ItemSlotPart.AppearanceName = "";
                    ReloadData();
                }
            }
        }

        private void newModNodeButton_Click(object sender, EventArgs e)
        {
            if (modsTreeView.SelectedNode == null)
            {
                MessageBox.Show("Must select parent node.");
            }
            else
            {
                var newNode = new ItemSlotPart
                {
                    ItemInfo = new ItemInfo
                    {
                        ItemId = new gameItemID()
                    },
                    AppearanceName = "None",
                    ItemAdditionalInfo = new ItemAdditionalInfo()
                };
                ((ItemSlotPart)modsTreeView.SelectedNode.Tag).Children.Add(newNode);
                ReloadData();
                var nodeDialog = new ModNodeDetails();
                nodeDialog.LoadNode(newNode, ReloadData, activeSaveFile);
            }
        }

        private void infuseLegendaryComponentsButton_Click(object sender, EventArgs e)
        {
            //var components = activeSaveFile.GetInventory(1).Items.Where(x => x.ItemTdbId.GameName == "Legendary Upgrade Components").FirstOrDefault();
            //if (components == null || ((ItemData.SimpleItemData)components.Data).Quantity < 5)
            //{
            //    MessageBox.Show("Insufficient Legendary Upgrade Components. (5) required.");
            //    return;
            //}

            //if (activeSaveFile.GetItemStatData(activeItem) == null)
            //{
            //    MessageBox.Show("Item has no stat data. To fix this, please upgrade the item in-game at least once.");
            //    return;
            //}

            //if (MessageBox.Show("This process will remove (5) Legendary Upgrade Components from your inventory in exchange for upgrading the selected item to legendary quality. The selected item's level will also be upgraded to your current level. Do you wish to continue?", "Infuse Legendary Components", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    var playerData = activeSaveFile.GetPlayerDevelopmentData();
            //    var level = playerData.Value.Proficiencies[Array.FindIndex(playerData.Value.Proficiencies, x => x.Type == gamedataProficiencyType.Level)].CurrentLevel;

            //    ((ItemData.SimpleItemData)components.Data).Quantity -= 5;
            //    activeSaveFile.SetConstantStat(gamedataStatType.Quality, 4, activeSaveFile.GetItemStatData(activeItem));
            //    activeSaveFile.SetConstantStat(gamedataStatType.ItemLevel, level * 10, activeSaveFile.GetItemStatData(activeItem));
            //    activeSaveFile.SetConstantStat(gamedataStatType.PowerLevel, level, activeSaveFile.GetItemStatData(activeItem));
            //    ReloadData();

            //    MessageBox.Show("Legendary components infused.");
            //}
        }

        private void ApplyableControlChanged(object sender, EventArgs e)
        {
            applyButton.Enabled = true;
        }

        private void createStatDataButton_Click(object sender, EventArgs e)
        {
            activeSaveFile.CreateStatData(activeItem.ItemInfo.ItemId);
            detailsTabControl.TabPages.Remove(statsPlaceholderTab);
            detailsTabControl.TabPages.Insert(0, statsTab);
            detailsTabControl.SelectedTab = statsTab;
            ReloadData();
        }

        private void txt_LootItemName_TextChanged(object sender, EventArgs e)
        {
            if (!activeItem.HasExtendedData() || _autoUpdate)
            {
                return;
            }

            activeItem.ItemAdditionalInfo!.LootItemPoolId = txt_LootItemName.Text;

            _autoUpdate = true;
            txt_LootItemId.Text = ((ulong)activeItem.ItemAdditionalInfo.LootItemPoolId).ToString();
            _autoUpdate = false;
        }

        private void txt_LootItemId_TextChanged(object sender, EventArgs e)
        {
            if (!activeItem.HasExtendedData() || _autoUpdate)
            {
                return;
            }

            activeItem.ItemAdditionalInfo!.LootItemPoolId = ulong.Parse(txt_LootItemId.Text);

            _autoUpdate = true;
            txt_LootItemName.Text = activeItem.ItemAdditionalInfo.LootItemPoolId.GetResolvedText() ?? "";
            _autoUpdate = false;
        }

        private void btn_MaxLevel_Click(object sender, EventArgs e)
        {
            unknown3Box.Text = float.MaxValue.ToString();
        }
    }
}
