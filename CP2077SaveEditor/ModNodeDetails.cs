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
    public partial class ModNodeDetails : Form
    {
        private ItemData.ItemModData activeNode;
        private Func<bool> callbackFunc;

        public ModNodeDetails()
        {
            InitializeComponent();
        }

        private void ModNodeDetails_Load(object sender, EventArgs e)
        {

        }

        public void LoadNode(ItemData.ItemModData node, Func<bool> callback)
        {
            activeNode = node;
            callbackFunc = callback;

            attachmentNameLabel.Text = node.AttachmentSlotName;
            attachmentIdBox.Text = node.AttachmentSlotTdbId.ToString();
            item1NameLabel.Text = node.ItemName;
            item1IdBox.Text = node.ItemTdbId.ToString();
            unknown1Box.Text = node.Unknown2.ToString();
            unknown2Box.Text = node.Unknown3.ToString();
            unknown3Box.Text = node.Unknown4.ToString();

            this.Text = node.AttachmentSlotName + " :: " + node.ItemName;
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
                uint.Parse(unknown3Box.Text);
            } catch(Exception) {
                MessageBox.Show("Invalid value.");
                return;
            }

            activeNode.AttachmentSlotTdbId = ulong.Parse(attachmentIdBox.Text);
            activeNode.ItemTdbId = ulong.Parse(item1IdBox.Text);
            activeNode.Unknown2 = uint.Parse(unknown1Box.Text);
            activeNode.Unknown3 = uint.Parse(unknown2Box.Text);
            activeNode.Unknown4 = uint.Parse(unknown3Box.Text);

            callbackFunc.Invoke();
            this.Close();
        }
    }
}
