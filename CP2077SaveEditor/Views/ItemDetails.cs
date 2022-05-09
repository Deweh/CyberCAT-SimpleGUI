using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;
using static WolvenKit.RED4.Save.InventoryHelper;

namespace CP2077SaveEditor
{
    public partial class ItemDetails : Form
    {
        private Func<bool> callbackFunc1;
        private ItemData activeItem;
        private SaveFileHelper activeSaveFile;
        private bool statsOnly = false;
        private Random globalRand;
        
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
            modsBaseIdBox.TextChanged += ApplyableControlChanged;

            //Shown += (object s, EventArgs e) =>
            //{
            //    var record = Form1.tdbService.GetRecord(activeItem.ItemTdbId);

            //    foreach(var member in record.GetEnumerator())
            //    {
            //        MessageBox.Show(member.propPath + ": " + member.value.ToString());
            //    }
            //};
        }

        private void IterativeBuildModTree(ItemModData nodeData, TreeNode rootNode)
        {
            foreach (ItemModData childNode in nodeData.Children)
            {
                var newNode = rootNode.Nodes.Add(childNode.AttachmentSlotTdbId.ResolvedText + " :: " + childNode.ItemTdbId.ResolvedText + " [" + childNode.Children.Count + "]");
                newNode.Tag = childNode;
                if (childNode.Children.Count > 0)
                {
                    IterativeBuildModTree(childNode, newNode);
                }
            }
        }

        private void IterativeDeleteModNode(ItemModData targetNode, ItemModData rootNode)
        {

            if (rootNode.Children.Contains(targetNode))
            {
                rootNode.Children.Remove(targetNode);
            } else {
                foreach (ItemModData childNode in rootNode.Children)
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
                if (activeItem.Data is SimpleItemData simpleData)
                {
                    //SimpleItemData parsing
                    this.Text = activeItem.ItemTdbId.ResolvedText + " (Simple Item)";

                    if (detailsTabControl.TabPages.Contains(modInfoTab))
                    {
                        detailsTabControl.TabPages.Remove(modInfoTab);
                    }

                    quantityUpDown.Value = simpleData.Quantity;
                }

                if (activeItem.Data is ModableItemData modableData)
                {
                    //ModableItemData parsing
                    this.Text = activeItem.ItemTdbId.ResolvedText + " (Modable Item)";

                    if (activeItem.Data is ModableItemWithQuantityData modableQuantityData)
                    {
                        quantityUpDown.Value = modableQuantityData.Quantity;
                    } else {
                        basicInfoGroupBox.Enabled = false;
                        quantityUpDown.Value = 1;
                    }

                    quickActionsGroupBox.Enabled = true;
                    modsBaseIdBox.Text = ((ulong)modableData.TdbId1).ToString();

                    modsTreeView.Nodes.Clear();
                    var rootNode = modsTreeView.Nodes.Add(modableData.RootNode.AttachmentSlotTdbId.ResolvedText, modableData.RootNode.AttachmentSlotTdbId.ResolvedText + " :: " + modableData.RootNode.ItemTdbId.ResolvedText + " [" + modableData.RootNode.Children.Count.ToString() + "]");
                    rootNode.Tag = modableData.RootNode;
                    IterativeBuildModTree(modableData.RootNode, rootNode);
                }
            }

            //Stats parsing
            if (Form1.statsSystemEnabled)
            {
                if (activeSaveFile.GetItemStatData(activeItem) == null)
                {
                    detailsTabControl.TabPages.Remove(statsTab);
                }
                else
                {
                    detailsTabControl.TabPages.Remove(statsPlaceholderTab);
                    statsListView.Items.Clear();
                    var listRows = new List<ListViewItem>();
                    var statsData = activeSaveFile.GetItemStatData(activeItem);
                    if (statsData.StatModifiers != null)
                    {
                        foreach (CHandle<gameStatModifierData_Deprecated> modifier in statsData.StatModifiers)
                        {
                            var row = new string[] { "Constant", modifier.Chunk.ModifierType.ToString(), modifier.Chunk.StatType.ToString(), "" };

                            if (modifier.Chunk is gameCombinedStatModifierData_Deprecated combinedData)
                            {
                                row[0] = "Combined";
                                row[3] = combinedData.Value.ToString();
                            }
                            else if (modifier.Chunk is gameConstantStatModifierData_Deprecated constantData)
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
                        statsData.StatModifiers = new();
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
            globalRand = rand;
            ReloadData();

            this.ShowDialog();
        }

        public void LoadStatsOnly(uint seed, object _saveFile, string name)
        {
            if (!Form1.statsSystemEnabled)
            {
                MessageBox.Show("Stats system disabled.");
                this.Close();
            }

            callbackFunc1 = delegate{ return true; };
            var dummyItem = new ItemData();
            dummyItem.Header = new() { Seed = seed };
            activeItem = dummyItem;
            activeSaveFile = (SaveFileHelper)_saveFile;
            this.Text = name;

            var newHeight = this.Height - 190;
            basicInfoGroupBox.Visible = false;
            flagsGroupBox.Visible = false;
            applyButton.Visible = false;
            detailsTabControl.TabPages.Remove(modInfoTab);

            if (name == "Player")
            {
                quickActionsGroupBox.Visible = false;
            } else {
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
                if (MessageBox.Show("Item has no stat data. To fix this, please upgrade the item in-game at least once. Fallback to old method?", "Notice", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    modsBaseIdBox.Text = ((ulong)88400986533).ToString();
                }
            } else {
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
                if (activeItem.Data is ModableItemData modableData)
                {
                    if (!ulong.TryParse(modsBaseIdBox.Text, out ulong id))
                    {
                        MessageBox.Show("Special ID must be a 64-bit unsigned integer.");
                        return;
                    }
                    modableData.TdbId1 = id;
                }

                if (activeItem.Data is SimpleItemData || activeItem.Data is ModableItemWithQuantityData)
                {
                    ((dynamic)activeItem.Data).Quantity = (uint)quantityUpDown.Value;
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
            nodeDetails.LoadNode(((ItemModData)e.Node.Tag), ReloadData, activeSaveFile);
        }

        private void statsListView_DoubleClick(object sender, EventArgs e)
        {
            if (statsListView.SelectedItems.Count > 0)
            {
                var nodeDetails = new StatDetails();
                nodeDetails.LoadStat(((CHandle<gameStatModifierData_Deprecated>)statsListView.SelectedItems[0].Tag), ReloadData);
            }
        }

        private void modsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && modsTreeView.SelectedNode != null) {

                var data = (ModableItemData)activeItem.Data;
                if (data.RootNode != (ItemModData)modsTreeView.SelectedNode.Tag)
                {
                    IterativeDeleteModNode(((ItemModData)modsTreeView.SelectedNode.Tag), data.RootNode);
                    modsTreeView.SelectedNode.Remove();
                } else {

                    data.RootNode.Children.Clear();
                    data.RootNode.AttachmentSlotTdbId = 0;
                    data.RootNode.ItemTdbId = 0;
                    data.RootNode.TdbId2 = 0;
                    data.RootNode.Unknown2 = 0;
                    data.RootNode.Unknown3 = 0;
                    data.RootNode.Unknown4 = 0;
                    data.RootNode.UnknownString = "";
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
                activeSaveFile.RemoveStat((CHandle<gameStatModifierData_Deprecated>)statsListView.SelectedItems[0].Tag, activeSaveFile.GetItemStatData(activeItem));
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
            if (statsListView.SelectedItems.Count > 0) {
                activeSaveFile.RemoveStat((CHandle<gameStatModifierData_Deprecated>)statsListView.SelectedItems[0].Tag, activeSaveFile.GetItemStatData(activeItem));
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

                var data = (ModableItemData)activeItem.Data;
                if (data.RootNode != (ItemModData)modsTreeView.SelectedNode.Tag)
                {
                    IterativeDeleteModNode(((ItemModData)modsTreeView.SelectedNode.Tag), data.RootNode);
                    modsTreeView.SelectedNode.Remove();
                }
                else
                {

                    data.RootNode.Children.Clear();
                    data.RootNode.AttachmentSlotTdbId = 0;
                    data.RootNode.ItemTdbId = 0;
                    data.RootNode.TdbId2 = 0;
                    data.RootNode.Unknown2 = 0;
                    data.RootNode.Unknown3 = 0;
                    data.RootNode.Unknown4 = 0;
                    data.RootNode.UnknownString = "";
                    ReloadData();
                }
            }
        }

        private void newModNodeButton_Click(object sender, EventArgs e)
        {
            if (modsTreeView.SelectedNode == null)
            {
                MessageBox.Show("Must select parent node.");
            } else {
                var newNode = new ItemModData();
                ((ItemModData)modsTreeView.SelectedNode.Tag).Children.Add(newNode);
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
            activeSaveFile.CreateStatData(activeItem, globalRand);
            detailsTabControl.TabPages.Remove(statsPlaceholderTab);
            detailsTabControl.TabPages.Insert(0, statsTab);
            detailsTabControl.SelectedTab = statsTab;
            ReloadData();
        }
    }
}
