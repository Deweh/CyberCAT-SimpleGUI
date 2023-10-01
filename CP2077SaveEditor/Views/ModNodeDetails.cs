using System;
using System.Windows.Forms;
using WolvenKit.RED4.Save.Classes;

namespace CP2077SaveEditor
{
    public partial class ModNodeDetails : Form
    {
        private ItemSlotPart activeNode;
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

            activeNode.ItemInfo.ItemId.Id = txt_ModName.Text;

            _autoUpdate = true;
            txt_ModId.Text = ((ulong)activeNode.ItemInfo.ItemId.Id).ToString();
            _autoUpdate = false;
        }

        private void txt_ModId_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.ItemInfo.ItemId.Id = ulong.Parse(txt_ModId.Text);

            _autoUpdate = true;
            txt_ModName.Text = activeNode.ItemInfo.ItemId.Id.GetResolvedText() ?? "";
            _autoUpdate = false;
        }

        private void txt_LootItemName_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.ItemAdditionalInfo.LootItemPoolId = txt_LootItemName.Text;

            _autoUpdate = true;
            txt_LootItemId.Text = ((ulong)activeNode.ItemAdditionalInfo.LootItemPoolId).ToString();
            _autoUpdate = false;
        }

        private void txt_LootItemId_TextChanged(object sender, EventArgs e)
        {
            if (_autoUpdate)
            {
                return;
            }

            activeNode.ItemAdditionalInfo.LootItemPoolId = ulong.Parse(txt_LootItemId.Text);

            _autoUpdate = true;
            txt_LootItemName.Text = activeNode.ItemAdditionalInfo.LootItemPoolId.GetResolvedText() ?? "";
            _autoUpdate = false;
        }

        public void LoadNode(ItemSlotPart node, Func<bool> callback, object saveFileObj)
        {
            activeNode = node;
            callbackFunc = callback;
            activeSaveFile = (SaveFileHelper)saveFileObj;

            txt_AttachmentId.Text = ((ulong)node.AttachmentSlotTdbId).ToString();
            txt_ModId.Text = ((ulong)node.ItemInfo.ItemId.Id).ToString();
            txt_LootItemId.Text = ((ulong)node.ItemAdditionalInfo.LootItemPoolId).ToString();

            unknown1Box.Text = node.Unknown2.ToString();
            unknown2Box.Text = node.ItemAdditionalInfo.Unknown2.ToString();
            unknown3Box.Text = node.ItemAdditionalInfo.RequiredLevel.ToString();
            unknown4Box.Text = node.AppearanceName;

            object resolvedStats = null;

            if (Global.StatsSystemEnabled)
            {
                resolvedStats = activeSaveFile.GetStatsFromSeed(node.ItemInfo.ItemId.RngSeed);
            }

            if (resolvedStats != null)
            {
                resolvedItemLabel.Text = "View Details";
            }
            else
            {
                resolvedItemLabel.Enabled = false;
            }

            this.Text = node.AttachmentSlotTdbId.ResolvedText + " :: " + node.ItemInfo.ItemId.Id.ResolvedText;
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
            activeNode.ItemInfo.ItemId.Id = ulong.Parse(txt_ModId.Text);
            activeNode.ItemAdditionalInfo.LootItemPoolId = ulong.Parse(txt_LootItemId.Text);
            activeNode.Unknown2 = uint.Parse(unknown1Box.Text);
            activeNode.ItemAdditionalInfo.Unknown2 = uint.Parse(unknown2Box.Text);
            activeNode.ItemAdditionalInfo.RequiredLevel = float.Parse(unknown3Box.Text);
            activeNode.AppearanceName = unknown4Box.Text;

            callbackFunc.Invoke();
            this.Close();
        }

        private void resolvedItemLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var details = new ItemDetails();
            if (activeNode.ItemInfo.ItemId.Id.ResolvedText.Length > 0)
            {
                details.LoadStatsOnly(activeNode.ItemInfo.ItemId.RngSeed, activeSaveFile, activeNode.ItemInfo.ItemId.Id.ResolvedText);
            }
            else
            {
                details.LoadStatsOnly(activeNode.ItemInfo.ItemId.RngSeed, activeSaveFile, activeNode.ItemInfo.ItemId.Id.ResolvedText);
            }

        }

        private void btn_MaxLevel_Click(object sender, EventArgs e)
        {
            unknown3Box.Text = float.MaxValue.ToString();
        }
    }
}
