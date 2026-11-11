using System;
using System.Linq;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Reads and changes the perks of the 2.0 attribute trees (PlayerDevelopmentData.AttributesData).
    /// </summary>
    public class PerkHelper
    {
        private readonly PlayerDevelopmentData _data;

        public PerkHelper(PlayerDevelopmentData data)
        {
            _data = data;
        }

        public int GetLevel(PerkRecord perk)
        {
            if (!Enum.TryParse<gamedataNewPerkType>(perk.Type, out var type))
            {
                return 0;
            }

            return _data.AttributesData
                .SelectMany(x => x.UnlockedPerks)
                .Where(x => x.Type == type)
                .Select(x => (int)x.CurrLevel)
                .DefaultIfEmpty(0)
                .Max();
        }

        /// <summary>
        /// Sets the perk level (0 removes it). With chargePoints the difference is taken from (or given back to)
        /// the perk points, Relic points for Espionage perks, like buying it in-game would.
        /// Requirements between perks are not checked.
        /// </summary>
        public bool SetLevel(PerkRecord perk, int level, bool chargePoints)
        {
            if (!Enum.TryParse<gamedataNewPerkType>(perk.Type, out var type) ||
                !Enum.TryParse<gamedataAttributeDataType>(perk.Attribute + "AttributeData", out var attributeType))
            {
                return false;
            }

            level = Math.Clamp(level, 0, perk.MaxLevel);
            var difference = level - GetLevel(perk);
            if (difference == 0)
            {
                return false;
            }

            var attribute = _data.AttributesData.FirstOrDefault(x => x.Type == attributeType);
            if (attribute == null)
            {
                attribute = new SAttributeData { Type = attributeType };
                _data.AttributesData.Add(attribute);
            }

            var existing = attribute.UnlockedPerks.FirstOrDefault(x => x.Type == type);
            if (level == 0)
            {
                attribute.UnlockedPerks.Remove(existing);
            }
            else if (existing != null)
            {
                existing.CurrLevel = level;
            }
            else
            {
                attribute.UnlockedPerks.Add(new SNewPerk { Type = type, CurrLevel = level });
            }

            if (chargePoints)
            {
                var pointType = attributeType == gamedataAttributeDataType.EspionageAttributeData
                    ? gamedataDevelopmentPointType.Espionage
                    : gamedataDevelopmentPointType.Primary;
                var points = _data.DevPoints.FirstOrDefault(x => x.Type == pointType);
                if (points != null)
                {
                    points.Spent = Math.Max(0, points.Spent + difference);
                    points.Unspent = Math.Max(0, points.Unspent - difference);
                }
            }

            return true;
        }

        public int GetUnspentPoints(gamedataDevelopmentPointType type) =>
            _data.DevPoints.FirstOrDefault(x => x.Type == type)?.Unspent ?? 0;
    }
}
