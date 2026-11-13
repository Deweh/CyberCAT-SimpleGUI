using System;
using System.Windows.Forms;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views
{
    public sealed class InventoryContextMenuRequest
    {
        public bool IsPlayerInventory { get; init; }
        public bool IsSearchActive { get; init; }
        public bool HasClipboardItem { get; init; }
        public int SelectedCount { get; init; }
        public int TotalListedCount { get; init; }
        public ListViewItem ClickedItem { get; init; }
        public gameItemID EquippedSlotId { get; init; }
        public CArray<gameSEquipArea> EquipAreas { get; init; }

        public EventHandler OnCut { get; init; }
        public EventHandler OnCopy { get; init; }
        public EventHandler OnPaste { get; init; }
        public EventHandler OnMoveSelectedToStash { get; init; }
        public EventHandler OnMoveAllToStash { get; init; }
        public EventHandler OnMoveSelectedToInventory { get; init; }
        public EventHandler OnMoveAllToInventory { get; init; }
        public EventHandler OnSelectAll { get; init; }
        public EventHandler OnUnequip { get; init; }
        public EventHandler OnEquipInSlot { get; init; }
        public EventHandler OnMakeMaxTier { get; init; }
        public EventHandler OnSetQuantity { get; init; }
        public EventHandler OnDelete { get; init; }
    }

    public static class InventoryContextMenuBuilder
    {
        public static ContextMenuStrip Build(InventoryContextMenuRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var contextMenu = new ContextMenuStrip();

            AddClipboardSection(contextMenu, request);
            AddStashTransferSection(contextMenu, request);
            AddEquipmentSection(contextMenu, request);
            AddItemActionsSection(contextMenu, request);

            return contextMenu;
        }

        private static void AddClipboardSection(ContextMenuStrip contextMenu, InventoryContextMenuRequest request)
        {
            if (request.ClickedItem != null)
            {
                contextMenu.Items.Add("Cut", null, request.OnCut).Tag = request.ClickedItem;
                contextMenu.Items.Add("Copy", null, request.OnCopy).Tag = request.ClickedItem;
            }

            if (request.HasClipboardItem)
            {
                contextMenu.Items.Add("Paste", null, request.OnPaste);
            }

            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Items.Add(new ToolStripSeparator());
            }
        }

        private static void AddStashTransferSection(ContextMenuStrip contextMenu, InventoryContextMenuRequest request)
        {
            var selectedCount = request.SelectedCount;

            if (request.IsPlayerInventory)
            {
                if (selectedCount > 0)
                {
                    var moveText = selectedCount == 1 ? "Move to Stash" : $"Move {selectedCount} Items to Stash";
                    contextMenu.Items.Add(moveText, null, request.OnMoveSelectedToStash);
                }

                if (request.TotalListedCount > 0)
                {
                    var moveAllText = request.IsSearchActive ? "Move All Listed Items to Stash" : "Move All to Stash";
                    contextMenu.Items.Add(moveAllText, null, request.OnMoveAllToStash);
                }

                if (contextMenu.Items.Count > 0)
                {
                    contextMenu.Items.Add(new ToolStripSeparator());
                }
            }

            if (request.TotalListedCount > 0)
            {
                if (!request.IsPlayerInventory)
                {
                    if (selectedCount > 0)
                    {
                        var moveBackText = selectedCount == 1 ? "Move to V's Inventory" : $"Move {selectedCount} Items to V's Inventory";
                        contextMenu.Items.Add(moveBackText, null, request.OnMoveSelectedToInventory);
                    }

                    var moveAllBackText = request.IsSearchActive ? "Move All Listed Items to V's Inventory" : "Move All to V's Inventory";
                    contextMenu.Items.Add(moveAllBackText, null, request.OnMoveAllToInventory);
                    contextMenu.Items.Add(new ToolStripSeparator());
                }

                contextMenu.Items.Add("Select All (Ctrl+A)", null, request.OnSelectAll);
            }
        }

        private static void AddEquipmentSection(ContextMenuStrip contextMenu, InventoryContextMenuRequest request)
        {
            if (!request.IsPlayerInventory || request.ClickedItem == null || request.SelectedCount != 1)
            {
                return;
            }

            if (request.EquippedSlotId != null)
            {
                var unequipItem = contextMenu.Items.Add("Unequip", null, request.OnUnequip);
                unequipItem.Tag = request.EquippedSlotId;
                return;
            }

            if (request.EquipAreas == null)
            {
                return;
            }

            var equipIn = new ToolStripMenuItem("Equip in Slot");
            foreach (var area in request.EquipAreas)
            {
                var counter = 1;
                foreach (var slot in area.EquipSlots)
                {
                    var slotItem = equipIn.DropDownItems.Add($"{area.AreaType} {counter}", null, request.OnEquipInSlot);
                    slotItem.Tag = slot;
                    counter++;
                }
            }
            contextMenu.Items.Add(equipIn);
        }

        private static void AddItemActionsSection(ContextMenuStrip contextMenu, InventoryContextMenuRequest request)
        {
            if (request.ClickedItem == null)
            {
                return;
            }

            var selectedCount = request.SelectedCount;
            contextMenu.Items.Add(selectedCount > 1 ? $"Make {selectedCount} Items Tier 5++" : "Make Tier 5++", null, request.OnMakeMaxTier);
            contextMenu.Items.Add(selectedCount > 1 ? $"Set Quantity of {selectedCount} Items..." : "Set Quantity...", null, request.OnSetQuantity);
            contextMenu.Items.Add(selectedCount > 1 ? $"Delete {selectedCount} Items" : "Delete", null, request.OnDelete);
        }
    }
}
