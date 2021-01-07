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
using CyberCAT.Core.Classes.Mapping.StatsSystem;
using CyberCAT.Core.DumpedEnums;
using CyberCAT.Core.Classes.Mapping.Global;

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

            statsTreeView.NodeMouseDoubleClick += statsTreeView_DoubleClick;
            statsTreeView.KeyDown += statsTreeView_KeyDown;
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
                var newChildren = rootNode.Children.ToList();
                newChildren.Remove(targetNode);
                rootNode.Children = newChildren.ToArray();
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
                statsTreeView.Nodes.Clear();
                var statsData = activeSaveFile.GetItemStatData(activeItem);
                foreach (CyberCAT.Core.Classes.Mapping.Global.Handle<GameStatModifierData> modifier in statsData.StatModifiers)
                {
                    var rootNode = statsTreeView.Nodes.Add(modifier.Value.GetType().Name + " :: " + modifier.Value.StatType.ToString());
                    rootNode.Tag = modifier;

                    if (modifier.Value.GetType().Name == "GameCombinedStatModifierData")
                    {
                        rootNode.Nodes.Add("Modifier Type: " + ((GameCombinedStatModifierData)modifier.Value).ModifierType.ToString());
                        rootNode.Nodes.Add("Operation: " + ((GameCombinedStatModifierData)modifier.Value).Operation.ToString());
                        rootNode.Nodes.Add("Ref Object: " + ((GameCombinedStatModifierData)modifier.Value).RefObject.ToString());
                        rootNode.Nodes.Add("Ref Stat Type: " + ((GameCombinedStatModifierData)modifier.Value).RefStatType.ToString());
                        rootNode.Nodes.Add("Stat Type: " + ((GameCombinedStatModifierData)modifier.Value).StatType.ToString());
                        rootNode.Nodes.Add("Value: " + ((GameCombinedStatModifierData)modifier.Value).Value.ToString());

                        rootNode.Nodes.Add("< Edit >").Tag = modifier;
                    }
                    else if(modifier.Value.GetType().Name == "GameConstantStatModifierData")
                    {
                        rootNode.Nodes.Add("Modifier Type: " + ((GameConstantStatModifierData)modifier.Value).ModifierType.ToString());
                        rootNode.Nodes.Add("Stat Type: " + ((GameConstantStatModifierData)modifier.Value).StatType.ToString());
                        rootNode.Nodes.Add("Value: " + ((GameConstantStatModifierData)modifier.Value).Value.ToString());

                        rootNode.Nodes.Add("< Edit >").Tag = modifier;
                    }
                    else if (modifier.Value.GetType().Name == "GameCurveStatModifierData")
                    {
                        rootNode.Nodes.Add("Column Name: " + ((GameCurveStatModifierData)modifier.Value).ColumnName.ToString());
                        rootNode.Nodes.Add("Curve Name: " + ((GameCurveStatModifierData)modifier.Value).CurveName.ToString());
                        rootNode.Nodes.Add("Curve Stat: " + ((GameCurveStatModifierData)modifier.Value).CurveStat.ToString());
                        rootNode.Nodes.Add("Modifier Type: " + ((GameCurveStatModifierData)modifier.Value).ModifierType.ToString());
                        rootNode.Nodes.Add("Stat Type: " + ((GameCurveStatModifierData)modifier.Value).StatType.ToString());

                        rootNode.Nodes.Add("< Edit >").Tag = modifier;
                    }
                }
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
                foreach (CyberCAT.Core.Classes.Mapping.Global.Handle<GameStatModifierData> modifier in activeSaveFile.GetItemStatData(activeItem).StatModifiers)
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

                    uint maxIdFound = 0;
                    foreach (GameSavedStatsData value in activeSaveFile.GetStatsMap().Values)
                    {
                        if (value.StatModifiers != null)
                        {
                            foreach (Handle<GameStatModifierData> modifierData in value.StatModifiers)
                            {
                                if (modifierData.Id > maxIdFound)
                                {
                                    maxIdFound = modifierData.Id;
                                }
                            }
                        }
                    }

                    var newModifier = new CyberCAT.Core.Classes.Mapping.Global.Handle<GameStatModifierData>(maxIdFound + 1);
                    newModifier.Value = newModifierData;

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

        private void statsTreeView_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                var nodeDetails = new StatDetails();
                nodeDetails.LoadStat(((CyberCAT.Core.Classes.Mapping.Global.Handle<GameStatModifierData>)e.Node.Tag), ReloadData);
            }
        }

        private void modsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && modsTreeView.SelectedNode != null) {
                MessageBox.Show("Node deletion is not currently supported by CyberCAT. Please use CPSE for the time being.", "Error");
                return;

                var data = (ItemData.ModableItemData)activeItem.Data;
                if (data.RootNode != (ItemData.ItemModData)modsTreeView.SelectedNode.Tag)
                {
                    IterativeDeleteModNode(((ItemData.ItemModData)modsTreeView.SelectedNode.Tag), data.RootNode);
                    modsTreeView.SelectedNode.Remove();
                } else {

                    data.RootNode.Children = new ItemData.ItemModData[0];
                    data.RootNode.AttachmentSlotTdbId = 0;
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
            uint removeId = statsHandle.Id;

            var modifiersList = activeSaveFile.GetItemStatData(activeItem).StatModifiers.ToList();
            modifiersList.Remove(statsHandle);
            activeSaveFile.GetItemStatData(activeItem).StatModifiers = modifiersList.ToArray();

            foreach (GameSavedStatsData value in activeSaveFile.GetStatsMap().Values)
            {
                if (value.StatModifiers != null)
                {
                    var replaceList = value.StatModifiers.ToList();

                    foreach (Handle<GameStatModifierData> modifierData in value.StatModifiers)
                    {
                        if (modifierData.Id > removeId)
                        {
                            var newHandle = new Handle<GameStatModifierData>(modifierData.Id - 1);
                            newHandle.Value = modifierData.Value;

                            replaceList[replaceList.FindIndex(x => x == modifierData)] = newHandle;
                        }
                    }

                    value.StatModifiers = replaceList.ToArray();
                }
            }
        }

        private void statsTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && statsTreeView.SelectedNode != null)
            {
                if (statsTreeView.SelectedNode.Tag != null && statsTreeView.SelectedNode.Text != "< Edit >")
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    RemoveStat((Handle<GameStatModifierData>)statsTreeView.SelectedNode.Tag);
                    statsTreeView.Nodes.Remove(statsTreeView.SelectedNode);
                }
            }
        }

        private void addStatButton_Click(object sender, EventArgs e)
        {
            var addStatDialog = new AddStat();
            addStatDialog.LoadAddDialog(addStatCallback);
        }

        public void addStatCallback(string modifierObjType)
        {
            object newModifierData;
            if (modifierObjType == "GameCombinedStatModifierData")
            {
                newModifierData = new GameCombinedStatModifierData();
            }
            else if (modifierObjType == "GameConstantStatModifierData")
            {
                newModifierData = new GameConstantStatModifierData();
            }
            else if (modifierObjType == "GameCurveStatModifierData")
            {
                newModifierData = new GameCurveStatModifierData();
                ((GameCurveStatModifierData)newModifierData).ColumnName = "<null>";
                ((GameCurveStatModifierData)newModifierData).CurveName = "<null>";

            } else {
                return;
            }

            uint maxIdFound = 0;
            foreach (GameSavedStatsData value in activeSaveFile.GetStatsMap().Values)
            {
                if (value.StatModifiers != null)
                {
                    foreach (Handle<GameStatModifierData> modifierData in value.StatModifiers)
                    {
                        if (modifierData.Id > maxIdFound)
                        {
                            maxIdFound = modifierData.Id;
                        }
                    }
                }
            }

            var newModifier = new CyberCAT.Core.Classes.Mapping.Global.Handle<GameStatModifierData>(maxIdFound + 1);
            newModifier.Value = (GameStatModifierData)newModifierData;

            activeSaveFile.GetItemStatData(activeItem).StatModifiers = activeSaveFile.GetItemStatData(activeItem).StatModifiers.Append(newModifier).ToArray();
            ReloadData();

            var newNode = statsTreeView.Nodes[statsTreeView.Nodes.Count - 1];
            newNode.Expand();
            newNode.EnsureVisible();
            statsTreeView.SelectedNode = newNode;
        }

        private void removeStatButton_Click(object sender, EventArgs e)
        {
            if (statsTreeView.SelectedNode != null) {

                if (statsTreeView.SelectedNode.Tag != null && statsTreeView.SelectedNode.Text != "< Edit >")
                {
                    RemoveStat((Handle<GameStatModifierData>)statsTreeView.SelectedNode.Tag);
                    statsTreeView.Nodes.Remove(statsTreeView.SelectedNode);
                }

            }
            
        }
    }
}
