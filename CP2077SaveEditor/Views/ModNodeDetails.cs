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
using static WolvenKit.RED4.Save.InventoryHelper;

namespace CP2077SaveEditor
{
    public partial class ModNodeDetails : Form
    {
        private ItemModData activeNode;
        private Func<bool> callbackFunc;
        private SaveFileHelper activeSaveFile;

        public ModNodeDetails()
        {
            InitializeComponent();
        }

        private void ModNodeDetails_Load(object sender, EventArgs e)
        {

        }

        public void LoadNode(ItemModData node, Func<bool> callback, object saveFileObj)
        {
            activeNode = node;
            callbackFunc = callback;
            activeSaveFile = (SaveFileHelper)saveFileObj;

            attachmentNameLabel.Text = node.AttachmentSlotTdbId.ResolvedText;
            attachmentIdBox.Text = ((ulong)node.AttachmentSlotTdbId).ToString();
            item1NameLabel.Text = node.Header.ItemId.Id.ResolvedText;
            
            item1IdBox.Text = ((ulong)node.Header.ItemId.Id).ToString();
            unknownIDBox.Text = ((ulong)node.TdbId2).ToString();
            unknown1Box.Text = node.Unknown2.ToString();
            unknown2Box.Text = node.Unknown3.ToString();
            unknown3Box.Text = node.Unknown4.ToString();
            unknown4Box.Text = node.AppearanceName;

            object resolvedStats = null;

            if (Form1.statsSystemEnabled)
            {
                resolvedStats = activeSaveFile.GetStatsFromSeed(node.Header.ItemId.RngSeed);
            }

            if (resolvedStats != null)
            {
                resolvedItemLabel.Text = "View Details";
            } else {
                resolvedItemLabel.Enabled = false;
            }

            this.Text = node.AttachmentSlotTdbId.ResolvedText + " :: " + node.Header.ItemId.Id.ResolvedText;
            this.ShowDialog();
        }

        private void applyCloseButton_Click(object sender, EventArgs e)
        {
            try
            {
                ulong.Parse(attachmentIdBox.Text);
                ulong.Parse(item1IdBox.Text);
                ulong.Parse(unknownIDBox.Text);
                uint.Parse(unknown1Box.Text);
                uint.Parse(unknown2Box.Text);
                float.Parse(unknown3Box.Text);
            } catch(Exception) {
                MessageBox.Show("Invalid value.");
                return;
            }

            activeNode.AttachmentSlotTdbId = ulong.Parse(attachmentIdBox.Text);
            activeNode.Header.ItemId.Id = ulong.Parse(item1IdBox.Text);
            activeNode.TdbId2 = ulong.Parse(unknownIDBox.Text);
            activeNode.Unknown2 = uint.Parse(unknown1Box.Text);
            activeNode.Unknown3 = uint.Parse(unknown2Box.Text);
            activeNode.Unknown4 = float.Parse(unknown3Box.Text);
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
    }
}
