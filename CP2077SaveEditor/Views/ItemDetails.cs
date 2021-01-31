using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyberCAT.Core.Classes;
using CyberCAT.Core.Classes.NodeRepresentations;
using CyberCAT.Core.DumpedEnums;
using CyberCAT.Core.Classes.Mapping;
using CyberCAT.Core.Classes.DumpedClasses;

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
        }

        private void IterativeBuildModTree(ItemData.ItemModData nodeData, TreeNode rootNode)
        {
            foreach (ItemData.ItemModData childNode in nodeData.Children)
            {
                var newNode = rootNode.Nodes.Add(childNode.AttachmentSlotTdbId.Name + " :: " + childNode.ItemTdbId.Name + " [" + childNode.ChildrenCount.ToString() + "]");
                newNode.Tag = childNode;
                if (childNode.ChildrenCount > 0)
                {
                    IterativeBuildModTree(childNode, newNode);
                }
            }
        }

        private void IterativeDeleteModNode(ItemData.ItemModData targetNode, ItemData.ItemModData rootNode)
        {

            if (rootNode.Children.Contains(targetNode))
            {
                rootNode.Children.Remove(targetNode);
            } else {
                foreach (ItemData.ItemModData childNode in rootNode.Children)
                {
                    if (childNode.ChildrenCount > 0)
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
                if (activeItem.Data.GetType().FullName.EndsWith("SimpleItemData"))
                {
                    //SimpleItemData parsing
                    this.Text = activeItem.ItemTdbId.Name + " (Simple Item)";

                    basicInfoGroupBox.Enabled = true;
                    quickActionsGroupBox.Enabled = false;

                    if (detailsTabControl.TabPages.Contains(modInfoTab))
                    {
                        detailsTabControl.TabPages.Remove(modInfoTab);
                    }

                    var data = (ItemData.SimpleItemData)activeItem.Data;
                    quantityUpDown.Value = data.Quantity;
                }
                else
                {
                    //ModableItemData parsing
                    this.Text = activeItem.ItemTdbId.Name + " (Modable Item)";

                    basicInfoGroupBox.Enabled = false;
                    quickActionsGroupBox.Enabled = true;

                    var data = (ItemData.ModableItemData)activeItem.Data;
                    quantityUpDown.Value = 1;
                    modsBaseIdBox.Text = data.TdbId1.Raw64.ToString();

                    modsTreeView.Nodes.Clear();

                    var rootNode = modsTreeView.Nodes.Add(data.RootNode.AttachmentSlotTdbId.Name, data.RootNode.AttachmentSlotTdbId.Name + " :: " + data.RootNode.ItemTdbId.Name + " [" + data.RootNode.ChildrenCount.ToString() + "]");
                    rootNode.Tag = data.RootNode;

                    IterativeBuildModTree(data.RootNode, rootNode);
                }
                if (activeItem.Data.GetType() == typeof(ItemData.ModableItemWithQuantityData))
                {
                    basicInfoGroupBox.Enabled = true;
                    quantityUpDown.Value = ((ItemData.ModableItemWithQuantityData)activeItem.Data).Quantity;
                }
            }

            //Stats parsing
            if (activeSaveFile.GetItemStatData(activeItem) == null)
            {
                activeSaveFile.CreateStatData(activeItem, globalRand);
            }
            statsListView.Items.Clear();
            var listRows = new List<ListViewItem>();
            var statsData = activeSaveFile.GetItemStatData(activeItem);
            if (statsData.StatModifiers != null)
            {
                foreach (Handle<GameStatModifierData> modifier in statsData.StatModifiers)
                {
                    var row = new string[] { "Constant", modifier.Value.ModifierType.ToString(), modifier.Value.StatType.ToString(), "" };

                    if (modifier.Value.GetType().Name == "GameCombinedStatModifierData")
                    {
                        row[0] = "Combined";
                        row[3] = ((GameCombinedStatModifierData)modifier.Value).Value.ToString();
                    }
                    else if (modifier.Value.GetType().Name == "GameConstantStatModifierData")
                    {
                        row[3] = ((GameConstantStatModifierData)modifier.Value).Value.ToString();
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
            } else {
                statsData.StatModifiers = new Handle<GameStatModifierData>[0];
            }
            

            if (!statsOnly)
            {
                unknownFlag1CheckBox.Checked = activeItem.Flags.IsNotUnequippable;
                questItemCheckBox.Checked = activeItem.Flags.IsQuestItem;
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
            callbackFunc1 = delegate{ return true; };
            var dummyItem = new ItemData();
            dummyItem.Header.Seed = seed;
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
                if (activeItem.Data.GetType() == typeof(ItemData.SimpleItemData))
                {
                    ((ItemData.SimpleItemData)activeItem.Data).Quantity = (uint)quantityUpDown.Value;
                }
                else
                {
                    try
                    {
                        ulong.Parse(modsBaseIdBox.Text);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("ID must be a 64-bit unsigned integer.");
                        return;
                    }
                    if (activeItem.Data.GetType() == typeof(ItemData.ModableItemData))
                    {
                        ((ItemData.ModableItemData)activeItem.Data).TdbId1.Raw64 = ulong.Parse(modsBaseIdBox.Text);
                    } else {
                        ((ItemData.ModableItemWithQuantityData)activeItem.Data).TdbId1.Raw64 = ulong.Parse(modsBaseIdBox.Text);
                        ((ItemData.ModableItemWithQuantityData)activeItem.Data).Quantity = (uint)quantityUpDown.Value;
                    }
                  
                }
                activeItem.Flags.IsNotUnequippable = unknownFlag1CheckBox.Checked;
                activeItem.Flags.IsQuestItem = questItemCheckBox.Checked;
                callbackFunc1.Invoke();
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
            nodeDetails.LoadNode(((ItemData.ItemModData)e.Node.Tag), ReloadData, activeSaveFile);
        }

        private void statsListView_DoubleClick(object sender, EventArgs e)
        {
            if (statsListView.SelectedItems.Count > 0)
            {
                var nodeDetails = new StatDetails();
                nodeDetails.LoadStat(((Handle<GameStatModifierData>)statsListView.SelectedItems[0].Tag), ReloadData);
            }
        }

        private void modsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && modsTreeView.SelectedNode != null) {

                var data = (ItemData.ModableItemData)activeItem.Data;
                if (data.RootNode != (ItemData.ItemModData)modsTreeView.SelectedNode.Tag)
                {
                    IterativeDeleteModNode(((ItemData.ItemModData)modsTreeView.SelectedNode.Tag), data.RootNode);
                    modsTreeView.SelectedNode.Remove();
                } else {

                    data.RootNode.Children.Clear();
                    data.RootNode.AttachmentSlotTdbId.Raw64 = 0;
                    data.RootNode.ItemTdbId.Raw64 = 0;
                    data.RootNode.TdbId2.Raw64 = 0;
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
                activeSaveFile.RemoveStat((Handle<GameStatModifierData>)statsListView.SelectedItems[0].Tag, activeSaveFile.GetItemStatData(activeItem));
                statsListView.Items.Remove(statsListView.SelectedItems[0]);
            }
        }

        private void addConstantStatButton_Click(object sender, EventArgs e)
        {
            var newId = activeSaveFile.AddStat(typeof(GameConstantStatModifierData), activeSaveFile.GetItemStatData(activeItem));
            ReloadData();

            var statDialog = new StatDetails();
            statDialog.LoadStat(activeSaveFile.GetItemStatData(activeItem).StatModifiers[Array.FindIndex(activeSaveFile.GetItemStatData(activeItem).StatModifiers, x => x.Id == newId)], ReloadData);
        }

        private void removeStatButton_Click(object sender, EventArgs e)
        {
            if (statsListView.SelectedItems.Count > 0) {
                activeSaveFile.RemoveStat((Handle<GameStatModifierData>)statsListView.SelectedItems[0].Tag, activeSaveFile.GetItemStatData(activeItem));
                statsListView.Items.Remove(statsListView.SelectedItems[0]);
            }
            
        }

        private void addCombinedStatButton_Click(object sender, EventArgs e)
        {
            var newId = activeSaveFile.AddStat(typeof(GameCombinedStatModifierData), activeSaveFile.GetItemStatData(activeItem));
            ReloadData();

            var statDialog = new StatDetails();
            statDialog.LoadStat(activeSaveFile.GetItemStatData(activeItem).StatModifiers[Array.FindIndex(activeSaveFile.GetItemStatData(activeItem).StatModifiers, x => x.Id == newId)], ReloadData);
        }

        private void addCurveStatButton_Click(object sender, EventArgs e)
        {
            var newId = activeSaveFile.AddStat(typeof(GameCurveStatModifierData), activeSaveFile.GetItemStatData(activeItem));
            ReloadData();

            var statDialog = new StatDetails();
            statDialog.LoadStat(activeSaveFile.GetItemStatData(activeItem).StatModifiers[Array.FindIndex(activeSaveFile.GetItemStatData(activeItem).StatModifiers, x => x.Id == newId)], ReloadData);
        }

        private void deleteModNodeButton_Click(object sender, EventArgs e)
        {
            if (modsTreeView.SelectedNode != null)
            {

                var data = (ItemData.ModableItemData)activeItem.Data;
                if (data.RootNode != (ItemData.ItemModData)modsTreeView.SelectedNode.Tag)
                {
                    IterativeDeleteModNode(((ItemData.ItemModData)modsTreeView.SelectedNode.Tag), data.RootNode);
                    modsTreeView.SelectedNode.Remove();
                }
                else
                {

                    data.RootNode.Children.Clear();
                    data.RootNode.AttachmentSlotTdbId.Raw64 = 0;
                    data.RootNode.ItemTdbId.Raw64 = 0;
                    data.RootNode.TdbId2.Raw64 = 0;
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
                var newNode = new ItemData.ItemModData();
                ((ItemData.ItemModData)modsTreeView.SelectedNode.Tag).Children.Add(newNode);
                ReloadData();
                var nodeDialog = new ModNodeDetails();
                nodeDialog.LoadNode(newNode, ReloadData, activeSaveFile);
            }
        }

        private void infuseLegendaryComponentsButton_Click(object sender, EventArgs e)
        {
            var components = activeSaveFile.GetInventory(1).Items.Where(x => x.ItemTdbId.GameName == "Legendary Upgrade Components").FirstOrDefault();
            if (components == null || ((ItemData.SimpleItemData)components.Data).Quantity < 5)
            {
                MessageBox.Show("Insufficient Legendary Upgrade Components. (5) required.");
                return;
            }

            if (activeSaveFile.GetItemStatData(activeItem) == null)
            {
                MessageBox.Show("Item has no stat data. To fix this, please upgrade the item in-game at least once.");
                return;
            }

            if (MessageBox.Show("This process will remove (5) Legendary Upgrade Components from your inventory in exchange for upgrading the selected item to legendary quality. The selected item's level will also be upgraded to your current level. Do you wish to continue?", "Infuse Legendary Components", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var playerData = activeSaveFile.GetPlayerDevelopmentData();
                var level = playerData.Value.Proficiencies[Array.FindIndex(playerData.Value.Proficiencies, x => x.Type == gamedataProficiencyType.Level)].CurrentLevel;

                ((ItemData.SimpleItemData)components.Data).Quantity -= 5;
                activeSaveFile.SetConstantStat(gamedataStatType.Quality, 4, activeSaveFile.GetItemStatData(activeItem));
                activeSaveFile.SetConstantStat(gamedataStatType.ItemLevel, level * 10, activeSaveFile.GetItemStatData(activeItem));
                activeSaveFile.SetConstantStat(gamedataStatType.PowerLevel, level, activeSaveFile.GetItemStatData(activeItem));
                ReloadData();

                MessageBox.Show("Legendary components infused.");
            }
        }

        private void ApplyableControlChanged(object sender, EventArgs e)
        {
            applyButton.Enabled = true;
        }
    }
}
