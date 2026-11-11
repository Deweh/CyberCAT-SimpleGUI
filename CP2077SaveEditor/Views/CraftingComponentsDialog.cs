using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views
{
    /// <summary>
    /// Edits the amount of crafting and quickhack components in V's inventory.
    /// </summary>
    public class CraftingComponentsDialog : Form
    {
        private static readonly string[] s_components =
        {
            "Items.CommonMaterial1",
            "Items.UncommonMaterial1",
            "Items.RareMaterial1",
            "Items.EpicMaterial1",
            "Items.LegendaryMaterial1",
            "Items.QuickHackUncommonMaterial1",
            "Items.QuickHackRareMaterial1",
            "Items.QuickHackEpicMaterial1",
            "Items.QuickHackLegendaryMaterial1",
        };

        private readonly SaveFileHelper _saveFile;
        private readonly SubInventory _inventory;
        private readonly Dictionary<string, NumericUpDown> _fields = new();

        public CraftingComponentsDialog(SaveFileHelper saveFile, SubInventory inventory)
        {
            _saveFile = saveFile;
            _inventory = inventory;

            Text = "Crafting Components";
            Font = new Font("Segoe UI", 8.25F);
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            StartPosition = FormStartPosition.CenterParent;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(8);

            var table = new TableLayoutPanel
            {
                ColumnCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            foreach (var id in s_components)
            {
                var field = new NumericUpDown
                {
                    Maximum = uint.MaxValue,
                    Width = 120,
                    ThousandsSeparator = true,
                    Value = FindItem(id)?.Quantity ?? 0
                };
                _fields.Add(id, field);

                table.Controls.Add(new Label
                {
                    Text = ResourceHelper.GetDisplayName(id) ?? id,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Margin = new Padding(3, 6, 12, 3)
                });
                table.Controls.Add(field);
            }

            var setAllButton = new Button { Text = "Set all to 99999", AutoSize = true };
            setAllButton.Click += (_, _) =>
            {
                foreach (var field in _fields.Values)
                {
                    field.Value = 99999;
                }
            };

            var applyButton = new Button { Text = "Apply", AutoSize = true, DialogResult = DialogResult.OK };
            applyButton.Click += (_, _) => Apply();

            table.Controls.Add(setAllButton);
            table.Controls.Add(applyButton);
            AcceptButton = applyButton;

            Controls.Add(table);
        }

        private ItemData FindItem(string id)
        {
            var hash = (ulong)(TweakDBID)id;
            return _inventory.Items.FirstOrDefault(x => (ulong)x.ItemInfo.ItemId.Id == hash);
        }

        private void Apply()
        {
            foreach (var (id, field) in _fields)
            {
                var quantity = (uint)field.Value;
                var item = FindItem(id);

                if (item != null)
                {
                    if (quantity == 0)
                    {
                        _inventory.Items.Remove(item);
                    }
                    else if (item.Quantity != quantity)
                    {
                        item.Quantity = quantity;
                    }
                }
                else if (quantity > 0)
                {
                    // Same structure the Add Item dialog uses for stackable items
                    _inventory.Items.Add(new ItemData
                    {
                        ItemInfo = new ItemInfo
                        {
                            ItemId = new gameItemID
                            {
                                Id = id,
                                RngSeed = 2,
                                UniqueCounter = _saveFile.GetNextUniqueCounter()
                            },
                            ItemStructure = ItemStructure.None
                        },
                        Quantity = quantity
                    });
                }
            }
        }
    }
}
