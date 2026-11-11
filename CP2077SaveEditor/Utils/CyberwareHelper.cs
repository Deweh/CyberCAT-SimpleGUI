using System;
using System.Collections.Generic;
using System.Linq;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Equips cyberware and raises V's cyberware capacity.
    /// </summary>
    public class CyberwareHelper
    {
        // V's capacity is computed on load (21 + 3 per level + Tech skill + perks + capacity shards),
        // a saved Humanity modifier on the player is added on top of that
        private const gamedataStatType CapacityStat = gamedataStatType.Humanity;

        private readonly SaveFileHelper _saveFile;

        public CyberwareHelper(SaveFileHelper saveFile)
        {
            _saveFile = saveFile;
        }

        public IEnumerable<gameSEquipArea> GetCyberwareAreas() =>
            _saveFile.GetEquipAreas().Where(x => ResourceHelper.CyberwareAreas.ContainsValue(x.AreaType.ToString()));

        public int GetExtraCapacity()
        {
            var modifiers = GetPlayerModifiers();
            return modifiers == null ? 0 : (int)GetCapacityModifiers(modifiers).Sum(x => (float)x.Value);
        }

        public void SetExtraCapacity(int value)
        {
            var playerStats = _saveFile.GetStatsFromEntityId(1) ?? throw new InvalidOperationException("The save has no stats for V");
            playerStats.ModifiersBuffer.Data ??= new ModifiersBuffer();
            var modifiers = (ModifiersBuffer)playerStats.ModifiersBuffer.Data;

            // Keep a single modifier, so the value shown is exactly what was set
            foreach (var modifier in GetCapacityModifiers(modifiers).ToList())
            {
                modifiers.Entries.Remove(modifier);
            }

            if (value != 0)
            {
                modifiers.Entries.Add(new gameConstantStatModifierData_Deprecated
                {
                    StatType = CapacityStat,
                    ModifierType = gameStatModifierType.Additive,
                    Value = value
                });
            }
        }

        public static string GetArea(TweakDBID itemId) => ResourceHelper.CyberwareAreas.GetValueOrDefault((ulong)itemId);

        public void Equip(gameSEquipSlot slot, ItemData item)
        {
            // An item can only be in one slot
            foreach (var area in GetCyberwareAreas())
            {
                foreach (var other in area.EquipSlots.Where(x => IsSameItem(x.ItemID, item.ItemInfo.ItemId)))
                {
                    other.ItemID = new gameItemID();
                }
            }

            var id = item.ItemInfo.ItemId;
            slot.ItemID = new gameItemID
            {
                Id = id.Id,
                RngSeed = id.RngSeed,
                UniqueCounter = id.UniqueCounter,
                Flags = id.Flags
            };
        }

        public static void Unequip(gameSEquipSlot slot)
        {
            slot.ItemID = new gameItemID();
        }

        public ItemData FindItem(gameItemID id) =>
            id == null || (ulong)id.Id == 0 ? null : _saveFile.GetInventory(1).Items.FirstOrDefault(x => IsSameItem(x.ItemInfo.ItemId, id));

        private static bool IsSameItem(gameItemID a, gameItemID b) =>
            a != null && b != null && a.Id == b.Id && a.RngSeed == b.RngSeed && a.UniqueCounter == b.UniqueCounter;

        private ModifiersBuffer GetPlayerModifiers() =>
            _saveFile.GetStatsFromEntityId(1)?.ModifiersBuffer.Data as ModifiersBuffer;

        private static IEnumerable<gameConstantStatModifierData_Deprecated> GetCapacityModifiers(ModifiersBuffer modifiers) =>
            modifiers.Entries
                .OfType<gameConstantStatModifierData_Deprecated>()
                .Where(x => x.StatType == CapacityStat && x.ModifierType == gameStatModifierType.Additive);
    }
}
