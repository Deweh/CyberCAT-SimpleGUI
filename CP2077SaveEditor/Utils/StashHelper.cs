using System;
using System.Collections.Generic;
using System.Linq;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Moves items between V's inventory and the stash. No UI here, the inventory page asks and reports.
    /// </summary>
    public static class StashHelper
    {
        public const ulong PlayerInventoryId = 0x1;
        public const ulong CarStashInventoryId = 0xF4240;
        public const ulong StashInventoryId = 0xE5F556FCBB62A706;

        public static bool CanMoveToStash(ItemData item)
        {
            if (item == null)
            {
                return false;
            }

            // Never move quest items or non-unequippable items to the Stash
            if (item.Flags != 0)
            {
                return false;
            }

            // Never move [M+] items (e.g. Grenades / rechargeable gadgets with both Quantity and Extended data)
            if (item.HasQuantity() && item.HasExtendedData())
            {
                return false;
            }

            var idText = item.ItemInfo.ItemId.Id.ResolvedText ?? string.Empty;
            if (idText == "Items.money" ||
                idText.StartsWith("Keycards.", StringComparison.OrdinalIgnoreCase) ||
                idText.StartsWith("Ammo.", StringComparison.OrdinalIgnoreCase) ||
                idText.StartsWith("Items.holstered_", StringComparison.OrdinalIgnoreCase) ||
                idText.Contains("w_melee_004__fists", StringComparison.OrdinalIgnoreCase) ||
                idText.Contains("Grenade", StringComparison.OrdinalIgnoreCase) ||
                idText.Contains("FirstAidWhiff", StringComparison.OrdinalIgnoreCase) ||
                idText.Contains("BonesMcCoy", StringComparison.OrdinalIgnoreCase) ||
                idText.Contains("HealthBooster", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (ResourceHelper.ItemClasses.TryGetValue(item.ItemInfo.ItemId.Id, out var itemRecord))
            {
                // In Cyberpunk 2077, only Weapons and Clothing can be safely stored and retrieved from the Stash in-game.
                // Gadgets (Grenades), Cyberware, Quickhacks/Attachments (Part), Mods (WeaponMod), Recipes, Consumables, and General items bug out in Stash.
                return itemRecord.Type is "Weapon" or "WeaponItem" or "Clothing";
            }

            // For modded/unknown items not in ItemClasses, only allow extended ([M]) items (weapons/clothing)
            return item.IsExtendedOnly();
        }

        /// <summary>
        /// The stash of the save: the apartment stash, else the car stash, else a new car stash. Null only if the save has no inventories.
        /// </summary>
        public static SubInventory GetOrCreateStash(SaveFileHelper save, out bool created)
        {
            created = false;
            var stash = save.GetInventory(StashInventoryId) ?? save.GetInventory(CarStashInventoryId);
            if (stash != null)
            {
                return stash;
            }

            var inventoriesContainer = save.GetInventoriesContainer();
            if (inventoriesContainer?.SubInventories == null)
            {
                return null;
            }

            stash = new SubInventory { InventoryId = CarStashInventoryId };
            inventoriesContainer.SubInventories.Add(stash);
            created = true;
            return stash;
        }

        /// <summary>
        /// Moves the stashable items from V's inventory to the stash, unequipping them. Returns how many moved and how many were skipped as not stashable.
        /// </summary>
        public static (int Moved, int Skipped) MoveToStash(SaveFileHelper save, SubInventory playerInventory, SubInventory stash, IEnumerable<ItemData> items)
        {
            var moved = 0;
            var skipped = 0;
            foreach (var item in items)
            {
                if (!CanMoveToStash(item))
                {
                    skipped++;
                    continue;
                }

                if (!playerInventory.Items.Remove(item))
                {
                    continue;
                }

                foreach (var equipSlot in save.GetEquipSlotsFromID(item.ItemInfo.ItemId))
                {
                    equipSlot.ItemID = new gameItemID();
                }

                // Stackable items join an existing stack of the same item
                var stack = item.IsQuantityOnly()
                    ? stash.Items.FirstOrDefault(x => x.IsQuantityOnly() &&
                                                      x.ItemInfo.ItemId.Id == item.ItemInfo.ItemId.Id &&
                                                      x.ItemInfo.ItemId.RngSeed == item.ItemInfo.ItemId.RngSeed)
                    : null;
                AddOrStack(stash, item, stack);

                moved++;
            }

            return (moved, skipped);
        }

        /// <summary>
        /// Moves items from another container (stash etc.) back to V's inventory. Returns how many moved.
        /// </summary>
        public static int MoveToInventory(SubInventory source, SubInventory playerInventory, IEnumerable<ItemData> items)
        {
            var moved = 0;
            foreach (var item in items)
            {
                if (!source.Items.Remove(item))
                {
                    continue;
                }

                ItemData stack = null;
                if (item.IsQuantityOnly())
                {
                    stack = playerInventory.Items.FirstOrDefault(x => x.IsQuantityOnly() &&
                                                                      x.ItemInfo.ItemId.Id == item.ItemInfo.ItemId.Id &&
                                                                      x.ItemInfo.ItemId.RngSeed == item.ItemInfo.ItemId.RngSeed);
                }
                else if (item.HasQuantity() && item.HasExtendedData())
                {
                    // [M+] items (grenades...) stack by record only
                    stack = playerInventory.Items.FirstOrDefault(x => x.HasQuantity() && x.HasExtendedData() &&
                                                                      x.ItemInfo.ItemId.Id == item.ItemInfo.ItemId.Id);
                }
                AddOrStack(playerInventory, item, stack);

                moved++;
            }

            return moved;
        }

        private static void AddOrStack(SubInventory target, ItemData item, ItemData stack)
        {
            if (stack != null)
            {
                stack.Quantity += item.Quantity;
            }
            else
            {
                target.Items.Add(item);
            }
        }
    }
}
