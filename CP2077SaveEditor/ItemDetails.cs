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
        private Func<bool> callbackFunc2;
        private ItemData activeItem;
        private DataType itemType;
        private SaveFileHelper activeSaveFile;

        public ItemDetails()
        {
            InitializeComponent();
            this.FormClosing += ItemDetails_FormClosing;

            modsTreeView.NodeMouseDoubleClick += modsTreeView_DoubleClick;
            modsTreeView.KeyDown += modsTreeView_KeyDown;

            statsListView.DoubleClick += statsListView_DoubleClick;
            statsListView.KeyDown += statsListView_KeyDown;
        }

        enum DataType
        {
            SimpleItem,
            ModableItem
        }

        private void ItemDetails_FormClosing(object sender, EventArgs e)
        {
            callbackFunc2.Invoke();
        }

        private void IterativeBuildModTree(ItemData.ItemModData nodeData, TreeNode rootNode)
        {
            foreach (ItemData.ItemModData childNode in nodeData.Children)
            {
                var newNode = rootNode.Nodes.Add(childNode.AttachmentSlotName + " :: " + childNode.ItemName + " [" + childNode.ChildrenCount.ToString() + "]");
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
            if (activeItem.Data.GetType().FullName.EndsWith("SimpleItemData"))
            {
                //SimpleItemData parsing
                itemType = DataType.SimpleItem;
                this.Text = activeItem.ItemName + " (Simple Item)";

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
                itemType = DataType.ModableItem;
                this.Text = activeItem.ItemName + " (Modable Item)";

                basicInfoGroupBox.Enabled = false;
                quickActionsGroupBox.Enabled = true;

                var data = (ItemData.ModableItemData)activeItem.Data;
                quantityUpDown.Value = 1;
                modsBaseIdBox.Text = data.TdbId1.Raw64.ToString();

                modsTreeView.Nodes.Clear();

                var rootNode = modsTreeView.Nodes.Add(data.RootNode.AttachmentSlotName, data.RootNode.AttachmentSlotName + " :: " + data.RootNode.ItemName + " [" + data.RootNode.ChildrenCount.ToString() + "]");
                rootNode.Tag = data.RootNode;

                IterativeBuildModTree(data.RootNode, rootNode);
            }

            //Stats parsing
            if (activeSaveFile.GetItemStatData(activeItem) == null)
            {
                if (detailsTabControl.TabPages.Contains(statsTab))
                {
                    detailsTabControl.TabPages.Remove(statsTab);
                }
            } else {
                statsListView.Items.Clear();
                var listRows = new List<ListViewItem>();
                var statsData = activeSaveFile.GetItemStatData(activeItem);
                foreach (Handle<GameStatModifierData> modifier in statsData.StatModifiers)
                {
                    var row = new string[] { "Constant", modifier.Value.ModifierType.ToString(), modifier.Value.StatType.ToString(), "" };

                    if (modifier.Value.GetType().Name == "GameCombinedStatModifierData")
                    {
                        row[0] = "Combined";
                        row[3] = ((GameCombinedStatModifierData)modifier.Value).Value.ToString();
                    }
                    else if(modifier.Value.GetType().Name == "GameConstantStatModifierData")
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
            }
            
            unknownFlag1CheckBox.Checked = activeItem.Flags.Unknown2;
            questItemCheckBox.Checked = activeItem.Flags.IsQuestItem;
            return true;
        }

        public void LoadItem(ItemData item, object _saveFile, Func<bool> callback1, Func<bool> callback2)
        {
            callbackFunc1 = callback1;
            callbackFunc2 = callback2;
            activeItem = item;
            activeSaveFile = (SaveFileHelper)_saveFile;
            ReloadData();

            this.ShowDialog();
        }

        private void pasteLegendaryIdButton_Click(object sender, EventArgs e)
        {
            if (activeSaveFile.GetItemStatData(activeItem) == null)
            {
                if (MessageBox.Show("Item has no stat data. Fallback to old method?", "Notice", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    modsBaseIdBox.Text = ((ulong)88400986533).ToString();
                }
            } else {
                var foundQualityStat = false;
                foreach (Handle<GameStatModifierData> modifier in activeSaveFile.GetItemStatData(activeItem).StatModifiers)
                {
                    if (modifier.Value.GetType().Name == "GameConstantStatModifierData")
                    {
                        if (((GameConstantStatModifierData)modifier.Value).StatType == gamedataStatType.Quality)
                        {
                            ((GameConstantStatModifierData)modifier.Value).Value = 4;
                            foundQualityStat = true;
                        }
                    }
                }

                if (!foundQualityStat)
                {
                    var newModifierData = new GameConstantStatModifierData();
                    newModifierData.ModifierType = gameStatModifierType.Additive;
                    newModifierData.StatType = gamedataStatType.Quality;
                    newModifierData.Value = 4;

                    var newModifier = activeSaveFile.GetStatsContainer().CreateHandle<GameStatModifierData>(newModifierData);
                    activeSaveFile.GetItemStatData(activeItem).StatModifiers = activeSaveFile.GetItemStatData(activeItem).StatModifiers.Append(newModifier).ToArray();
                }

                ReloadData();
                MessageBox.Show("Legendary quality stat applied.");
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (itemType == DataType.SimpleItem)
            {
                ((ItemData.SimpleItemData)activeItem.Data).Quantity = (uint)quantityUpDown.Value;
            } else {
                try
                {
                    ulong.Parse(modsBaseIdBox.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("ID must be a 64-bit unsigned integer.");
                    return;
                }
                ((ItemData.ModableItemData)activeItem.Data).TdbId1.Raw64 = ulong.Parse(modsBaseIdBox.Text);
            }
            activeItem.Flags.Unknown2 = unknownFlag1CheckBox.Checked;
            activeItem.Flags.IsQuestItem = questItemCheckBox.Checked;
            callbackFunc1.Invoke();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            callbackFunc2.Invoke();
            this.Close();
        }

        private void modsTreeView_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var nodeDetails = new ModNodeDetails();
            nodeDetails.LoadNode(((ItemData.ItemModData)e.Node.Tag), ReloadData);
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

        private void RemoveStat(Handle<GameStatModifierData> statsHandle)
        {
            var modifiersList = activeSaveFile.GetItemStatData(activeItem).StatModifiers.ToList();
            modifiersList.Remove(statsHandle);
            activeSaveFile.GetItemStatData(activeItem).StatModifiers = modifiersList.ToArray();

            activeSaveFile.GetStatsContainer().RemoveHandle((int)statsHandle.Id);

            foreach (GameSavedStatsData value in activeSaveFile.GetStatsMap().Values)
            {
                if (value.StatModifiers != null)
                {
                    foreach (Handle<GameStatModifierData> modifierData in value.StatModifiers)
                    {
                        if (modifierData.Id > statsHandle.Id)
                        {
                            modifierData.SetId(modifierData.Id - 1);
                        }
                    }
                }
            }
        }

        private uint AddStat(Type statType)
        {
            var newModifierData = Activator.CreateInstance(statType);

            if (statType == typeof(GameCurveStatModifierData))
            {
                ((GameCurveStatModifierData)newModifierData).ColumnName = "<null>";
                ((GameCurveStatModifierData)newModifierData).CurveName = "<null>";

            }

            var newModifier = activeSaveFile.GetStatsContainer().CreateHandle<GameStatModifierData>((GameStatModifierData)newModifierData);
            activeSaveFile.GetItemStatData(activeItem).StatModifiers = activeSaveFile.GetItemStatData(activeItem).StatModifiers.Append(newModifier).ToArray();

            return newModifier.Id;
        }

        private void statsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && statsListView.SelectedItems.Count > 0)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                RemoveStat((Handle<GameStatModifierData>)statsListView.SelectedItems[0].Tag);
                statsListView.Items.Remove(statsListView.SelectedItems[0]);
            }
        }

        private void addConstantStatButton_Click(object sender, EventArgs e)
        {
            var newId = AddStat(typeof(GameConstantStatModifierData));
            ReloadData();

            var statDialog = new StatDetails();
            statDialog.LoadStat(activeSaveFile.GetItemStatData(activeItem).StatModifiers[Array.FindIndex(activeSaveFile.GetItemStatData(activeItem).StatModifiers, x => x.Id == newId)], ReloadData);
        }

        private void removeStatButton_Click(object sender, EventArgs e)
        {
            if (statsListView.SelectedItems.Count > 0) {
                RemoveStat((Handle<GameStatModifierData>)statsListView.SelectedItems[0].Tag);
                statsListView.Items.Remove(statsListView.SelectedItems[0]);
            }
            
        }

        private void addCombinedStatButton_Click(object sender, EventArgs e)
        {
            var newId = AddStat(typeof(GameCombinedStatModifierData));
            ReloadData();

            var statDialog = new StatDetails();
            statDialog.LoadStat(activeSaveFile.GetItemStatData(activeItem).StatModifiers[Array.FindIndex(activeSaveFile.GetItemStatData(activeItem).StatModifiers, x => x.Id == newId)], ReloadData);
        }

        private void addCurveStatButton_Click(object sender, EventArgs e)
        {
            var newId = AddStat(typeof(GameCurveStatModifierData));
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
                nodeDialog.LoadNode(newNode, ReloadData);
            }
        }
    }
}
