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

namespace CP2077SaveEditor
{
    public partial class ModNodeDetails : Form
    {
        private ItemData.ItemModData activeNode;
        private Func<bool> callbackFunc;
        private SaveFileHelper activeSaveFile;

        public ModNodeDetails()
        {
            InitializeComponent();
        }

        private void ModNodeDetails_Load(object sender, EventArgs e)
        {

        }

        public void LoadNode(ItemData.ItemModData node, Func<bool> callback, object saveFileObj)
        {
            activeNode = node;
            callbackFunc = callback;
            activeSaveFile = (SaveFileHelper)saveFileObj;

            attachmentNameLabel.Text = node.AttachmentSlotTdbId.Name;
            attachmentIdBox.Text = node.AttachmentSlotTdbId.Raw64.ToString();

            if (node.ItemTdbId.GameName.Length > 0)
            {
                item1NameLabel.Text = node.ItemTdbId.GameName;
            }
            else
            {
                item1NameLabel.Text = node.ItemTdbId.Name;
            }
            
            item1IdBox.Text = node.ItemTdbId.Raw64.ToString();
            unknown1Box.Text = node.Unknown2.ToString();
            unknown2Box.Text = node.Unknown3.ToString();
            unknown3Box.Text = node.Unknown4.ToString();

            object resolvedStats = null;

            if (Form1.statsSystemEnabled)
            {
                resolvedStats = activeSaveFile.GetStatsFromSeed(node.Header.Seed);
            }

            if (resolvedStats != null)
            {
                resolvedItemLabel.Text = "View Details";
            } else {
                resolvedItemLabel.Enabled = false;
            }

            this.Text = node.AttachmentSlotTdbId.Name + " :: " + node.ItemTdbId.Name;
            this.ShowDialog();
        }

        private void applyCloseButton_Click(object sender, EventArgs e)
        {
            try
            {
                ulong.Parse(attachmentIdBox.Text);
                ulong.Parse(item1IdBox.Text);
                uint.Parse(unknown1Box.Text);
                uint.Parse(unknown2Box.Text);
                float.Parse(unknown3Box.Text);
            } catch(Exception) {
                MessageBox.Show("Invalid value.");
                return;
            }

            activeNode.AttachmentSlotTdbId.Raw64 = ulong.Parse(attachmentIdBox.Text);
            activeNode.ItemTdbId.Raw64 = ulong.Parse(item1IdBox.Text);
            activeNode.Unknown2 = uint.Parse(unknown1Box.Text);
            activeNode.Unknown3 = uint.Parse(unknown2Box.Text);
            activeNode.Unknown4 = float.Parse(unknown3Box.Text);

            callbackFunc.Invoke();
            this.Close();
        }

        private void resolvedItemLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var details = new ItemDetails();
            if (activeNode.ItemTdbId.GameName.Length > 0)
            {
                details.LoadStatsOnly(activeNode.Header.Seed, activeSaveFile, activeNode.ItemTdbId.GameName);
            }
            else
            {
                details.LoadStatsOnly(activeNode.Header.Seed, activeSaveFile, activeNode.ItemTdbId.Name);
            }
            
        }
    }
}
