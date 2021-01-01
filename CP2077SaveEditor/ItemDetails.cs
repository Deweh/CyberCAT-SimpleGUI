using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyberCAT.Core.Classes.NodeRepresentations;

namespace CP2077SaveEditor
{
    public partial class ItemDetails : Form
    {
        private Func<string> callbackFunc;
        private ItemData activeItem;
        private DataType itemType;

        public ItemDetails()
        {
            InitializeComponent();
        }

        enum DataType
        {
            SimpleItem,
            ModableItem
        }

        private void ItemDetails_Load(object sender, EventArgs e)
        {
            modsTreeView.NodeMouseDoubleClick += modsTreeView_DoubleClick;
            modsTreeView.KeyDown += modsTreeView_KeyDown;
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
            var newChildren = new List<ItemData.ItemModData>();
            var foundTarget = false;

            foreach (ItemData.ItemModData childNode in rootNode.Children)
            {
                if (childNode == targetNode)
                {
                    foundTarget = true;
                } else {
                    newChildren.Add(childNode);
                }

                if (childNode.ChildrenCount > 0 && !foundTarget)
                {
                    IterativeDeleteModNode(targetNode, childNode);
                }
            }

            if (foundTarget)
            {
                rootNode.Children = newChildren.ToArray();
            }
        }

        public bool ReloadData()
        {
            if (activeItem.Data.GetType().FullName.EndsWith("SimpleItemData"))
            {
                itemType = DataType.SimpleItem;
                this.Text += " (Simple Item)";
                basicInfoGroupBox.Enabled = true;
                quickActionsGroupBox.Enabled = false;
                modInfoGroupBox.Enabled = false;
                var data = (ItemData.SimpleItemData)activeItem.Data;
                quantityUpDown.Value = data.Quantity;
            }
            else
            {
                itemType = DataType.ModableItem;
                this.Text += " (Modable Item)";
                basicInfoGroupBox.Enabled = false;
                quickActionsGroupBox.Enabled = true;
                modInfoGroupBox.Enabled = true;
                var data = (ItemData.ModableItemData)activeItem.Data;
                quantityUpDown.Value = 1;
                modsBaseIdBox.Text = data.TdbId1.ToString();

                modsTreeView.Nodes.Clear();
                var rootNode = modsTreeView.Nodes.Add(data.RootNode.AttachmentSlotName, data.RootNode.AttachmentSlotName + " :: " + data.RootNode.ItemName + " [" + data.RootNode.ChildrenCount.ToString() + "]");
                rootNode.Tag = data.RootNode;
                IterativeBuildModTree(data.RootNode, rootNode);
            }
            unknownFlag1CheckBox.Checked = activeItem.Flags.Unknown2;
            questItemCheckBox.Checked = activeItem.Flags.IsQuestItem;
            return true;
        }

        public void LoadItem(ItemData item, Func<string> callback)
        {
            callbackFunc = callback;
            this.Text = item.ItemName;
            activeItem = item;
            ReloadData();

            this.ShowDialog();
        }

        private void pasteLegendaryIdButton_Click(object sender, EventArgs e)
        {
            modsBaseIdBox.Text = ((ulong)88400986533).ToString();
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
                ((ItemData.ModableItemData)activeItem.Data).TdbId1 = ulong.Parse(modsBaseIdBox.Text);
            }
            activeItem.Flags.Unknown2 = unknownFlag1CheckBox.Checked;
            activeItem.Flags.IsQuestItem = questItemCheckBox.Checked;
            callbackFunc.Invoke();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void modsTreeView_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var nodeDetails = new ModNodeDetails();
            nodeDetails.LoadNode(((ItemData.ItemModData)e.Node.Tag), ReloadData);
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
    }
}
