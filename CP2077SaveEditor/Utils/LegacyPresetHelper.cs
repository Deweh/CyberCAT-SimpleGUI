using Newtonsoft.Json.Linq;
using SharpDX;
using System;
using System.Text.Json.Nodes;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Utils;

public class LegacyPresetHelper
{
    public static gameuiCharacterCustomizationPresetWrapper Convert(string json)
    {
        var node = JsonNode.Parse(json);

        var result = new gameuiCharacterCustomizationPresetWrapper();

        result.DataExists = node["DataExists"].GetValue<bool>();
        result.Unknown1 = node["Unknown1"].GetValue<uint>();

        var bytes = System.Convert.FromBase64String(node["UnknownFirstBytes"].GetValue<string>());
        result.Preset.Version = BitConverter.ToUInt32(bytes, 0);
        result.Preset.IsMale = bytes[4] == 1;
        result.IsBrainGenderMale = bytes[5] == 1;

        result.Preset.HeadGroups = ConvertGroupArray(node["FirstSection"]["AppearanceSections"].AsArray());
        result.Preset.ArmsGroups = ConvertGroupArray(node["SecondSection"]["AppearanceSections"].AsArray());
        result.Preset.BodyGroups = ConvertGroupArray(node["ThirdSection"]["AppearanceSections"].AsArray());

        foreach (var tripleNode in node["StringTriples"].AsArray())
        {
            result.Preset.PerspectiveInfo.Add(new gameuiPerspectiveInfo
            {
                Name = tripleNode["FirstString"].GetValue<string>(),
                Fpp = tripleNode["SecondString"].GetValue<string>(),
                Tpp = tripleNode["ThirdString"].GetValue<string>()
            });
        }

        foreach (var stringNode in node["Strings"].AsArray())
        {
            result.Preset.Tags.Tags.Add(stringNode.GetValue<string>());
        }

        return result;
    }

    private static CArray<gameuiCustomizationGroup> ConvertGroupArray(JsonArray array)
    {
        var result = new CArray<gameuiCustomizationGroup>();

        foreach (var sectionNode in array)
        {
            var group = new gameuiCustomizationGroup();

            group.Name = sectionNode["SectionName"].GetValue<string>();
            foreach (var mainNode in sectionNode["MainList"].AsArray())
            {
                var appearance = new gameuiCustomizationAppearance();

                appearance.Resource = new CResourceAsyncReference<appearanceAppearanceResource>(mainNode["Hash"].GetValue<ulong>(), InternalEnums.EImportFlags.Soft);
                appearance.Definition = mainNode["FirstString"].GetValue<string>();
                appearance.Name = mainNode["SecondString"].GetValue<string>();

                var bytes = System.Convert.FromBase64String(mainNode["TrailingBytes"].GetValue<string>());
                appearance.CensorFlag = (Enums.CensorshipFlags)BitConverter.ToUInt32(bytes, 0);
                appearance.CensorFlagAction = (Enums.gameuiCharacterCustomizationActionType)BitConverter.ToUInt32(bytes, 4);

                group.Customization.Add(appearance);
            }
            foreach (var mainNode in sectionNode["AdditionalList"].AsArray())
            {
                var morph = new gameuiCustomizationMorph();

                morph.RegionName = mainNode["FirstString"].GetValue<string>();
                morph.TargetName = mainNode["SecondString"].GetValue<string>();

                var bytes = System.Convert.FromBase64String(mainNode["TrailingBytes"].GetValue<string>());
                morph.CensorFlag = (Enums.CensorshipFlags)BitConverter.ToUInt32(bytes, 0);
                morph.CensorFlagAction = (Enums.gameuiCharacterCustomizationActionType)BitConverter.ToUInt32(bytes, 4);

                group.Morphs.Add(morph);
            }

            result.Add(group);
        }

        return result;
    }
}