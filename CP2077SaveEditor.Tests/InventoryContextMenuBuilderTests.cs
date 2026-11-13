using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.Views;
using WolvenKit.RED4.Types;
using Xunit;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Tests;

public class InventoryContextMenuBuilderTests
{
    [Fact]
    public void Build_PlayerInventorySingleUnequippedSelection_IncludesExpectedMenuItems()
    {
        var clickedItem = new ListViewItem("Item 1");
        var equipAreas = new CArray<gameSEquipArea>
        {
            new()
            {
                AreaType = gamedataEquipmentArea.Weapon,
                EquipSlots = new CArray<gameSEquipSlot> { new(), new() }
            }
        };

        using var menu = InventoryContextMenuBuilder.Build(new InventoryContextMenuRequest
        {
            IsPlayerInventory = true,
            IsSearchActive = false,
            HasClipboardItem = true,
            SelectedCount = 1,
            TotalListedCount = 10,
            ClickedItem = clickedItem,
            EquipAreas = equipAreas
        });

        var labels = menu.Items.OfType<ToolStripMenuItem>().Select(x => x.Text).ToList();
        Assert.Contains("Cut", labels);
        Assert.Contains("Copy", labels);
        Assert.Contains("Paste", labels);
        Assert.Contains("Move to Stash", labels);
        Assert.Contains("Move All to Stash", labels);
        Assert.Contains("Select All (Ctrl+A)", labels);
        Assert.Contains("Equip in Slot", labels);
        Assert.Contains("Make Tier 5++", labels);
        Assert.Contains("Set Quantity...", labels);
        Assert.Contains("Delete", labels);

        var equipMenu = menu.Items.OfType<ToolStripMenuItem>().Single(x => x.Text == "Equip in Slot");
        Assert.Equal(2, equipMenu.DropDownItems.Count);
        Assert.Equal("Weapon 1", equipMenu.DropDownItems[0].Text);
        Assert.Equal("Weapon 2", equipMenu.DropDownItems[1].Text);
    }

    [Fact]
    public void Build_StashContainerMultipleSelectionWithActiveSearch_UsesPluralAndFilteredLabels()
    {
        var clickedItem = new ListViewItem("Stash Item");

        using var menu = InventoryContextMenuBuilder.Build(new InventoryContextMenuRequest
        {
            IsPlayerInventory = false,
            IsSearchActive = true,
            HasClipboardItem = false,
            SelectedCount = 3,
            TotalListedCount = 5,
            ClickedItem = clickedItem
        });

        var labels = menu.Items.OfType<ToolStripMenuItem>().Select(x => x.Text).ToList();
        Assert.DoesNotContain("Move to Stash", labels);
        Assert.Contains("Move 3 Items to V's Inventory", labels);
        Assert.Contains("Move All Listed Items to V's Inventory", labels);
        Assert.Contains("Select All (Ctrl+A)", labels);
        Assert.Contains("Make 3 Items Tier 5++", labels);
        Assert.Contains("Set Quantity of 3 Items...", labels);
        Assert.Contains("Delete 3 Items", labels);
    }
}
