using System;
using System.Windows.Forms;
using static WolvenKit.RED4.Save.InventoryHelper;

namespace CP2077SaveEditor
{
    public partial class ModNodeDetails : Form
    {
        private ItemModData activeNode;
        private Func<bool> callbackFunc;
        private SaveFileHelper activeSaveFile;

        private bool _autoUpdate;

        public ModNodeDetails()
        {
            InitializeComponent();
        }

        private void txt_AttachmentName_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.AttachmentSlotTdbId = txt_AttachmentName.Text;

            _autoUpdate = true;
            txt_AttachmentId.Text = ((ulong)activeNode.AttachmentSlotTdbId).ToString();
            _autoUpdate = false;
        }

        private void txt_AttachmentId_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.AttachmentSlotTdbId = ulong.Parse(txt_AttachmentId.Text);

            _autoUpdate = true;
            txt_AttachmentName.Text = activeNode.AttachmentSlotTdbId.GetResolvedText() ?? "";
            _autoUpdate = false;
        }

        private void txt_ModName_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.Header.ItemId.Id = txt_ModName.Text;

            _autoUpdate = true;
            txt_ModId.Text = ((ulong)activeNode.Header.ItemId.Id).ToString();
            _autoUpdate = false;
        }

        private void txt_ModId_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.Header.ItemId.Id = ulong.Parse(txt_ModId.Text);

            _autoUpdate = true;
            txt_ModName.Text = activeNode.Header.ItemId.Id.GetResolvedText() ?? "";
            _autoUpdate = false;
        }

        private void txt_LootItemName_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.ModHeaderThing.LootItemId = txt_LootItemName.Text;

            _autoUpdate = true;
            txt_LootItemId.Text = ((ulong)activeNode.ModHeaderThing.LootItemId).ToString();
            _autoUpdate = false;
        }

        private void txt_LootItemId_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.ModHeaderThing.LootItemId = ulong.Parse(txt_LootItemId.Text);

            _autoUpdate = true;
            txt_LootItemName.Text = activeNode.ModHeaderThing.LootItemId.GetResolvedText() ?? "";
            _autoUpdate = false;
        }

        public void LoadNode(ItemModData node, Func<bool> callback, object saveFileObj)
        {
            activeNode = node;
            callbackFunc = callback;
            activeSaveFile = (SaveFileHelper)saveFileObj;

            txt_AttachmentId.Text = ((ulong)node.AttachmentSlotTdbId).ToString();
            txt_ModId.Text = ((ulong)node.Header.ItemId.Id).ToString();
            txt_LootItemId.Text = ((ulong)node.ModHeaderThing.LootItemId).ToString();

            unknown1Box.Text = node.Unknown2.ToString();
            unknown2Box.Text = node.ModHeaderThing.Unknown2.ToString();
            unknown3Box.Text = node.ModHeaderThing.RequiredLevel.ToString();
            unknown4Box.Text = node.AppearanceName;

            object resolvedStats = null;

            if (Global.StatsSystemEnabled)
            {
                resolvedStats = activeSaveFile.GetStatsFromSeed(node.Header.ItemId.RngSeed);
            }

            if (resolvedStats != null)
            {
                resolvedItemLabel.Text = "View Details";
            }
            else
            {
                resolvedItemLabel.Enabled = false;
            }

            this.Text = node.AttachmentSlotTdbId.ResolvedText + " :: " + node.Header.ItemId.Id.ResolvedText;
            this.ShowDialog();
        }

        private void applyCloseButton_Click(object sender, EventArgs e)
        {
            try
            {
                ulong.Parse(txt_AttachmentId.Text);
                ulong.Parse(txt_ModId.Text);
                ulong.Parse(txt_LootItemId.Text);
                uint.Parse(unknown1Box.Text);
                uint.Parse(unknown2Box.Text);
                float.Parse(unknown3Box.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid value.");
                return;
            }

            activeNode.AttachmentSlotTdbId = ulong.Parse(txt_AttachmentId.Text);
            activeNode.Header.ItemId.Id = ulong.Parse(txt_ModId.Text);
            activeNode.ModHeaderThing.LootItemId = ulong.Parse(txt_LootItemId.Text);
            activeNode.Unknown2 = uint.Parse(unknown1Box.Text);
            activeNode.ModHeaderThing.Unknown2 = uint.Parse(unknown2Box.Text);
            activeNode.ModHeaderThing.RequiredLevel = float.Parse(unknown3Box.Text);
            activeNode.AppearanceName = unknown4Box.Text;

            callbackFunc.Invoke();
            this.Close();
        }

        private void resolvedItemLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var details = new ItemDetails();
            if (activeNode.Header.ItemId.Id.ResolvedText.Length > 0)
            {
                details.LoadStatsOnly(activeNode.Header.ItemId.RngSeed, activeSaveFile, activeNode.Header.ItemId.Id.ResolvedText);
            }
            else
            {
                details.LoadStatsOnly(activeNode.Header.ItemId.RngSeed, activeSaveFile, activeNode.Header.ItemId.Id.ResolvedText);
            }

        }

        private void btn_MaxLevel_Click(object sender, EventArgs e)
        {
            unknown3Box.Text = float.MaxValue.ToString();
        }
    }
}
