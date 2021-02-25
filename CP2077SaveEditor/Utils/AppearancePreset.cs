using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace CP2077SaveEditor
{
    public static class AppearancePreset
    {
        public static readonly string[] IgnoredProperties = new[]
        {
            nameof(AppearanceHelper.MainSections),
            nameof(AppearanceHelper.SuppressBodyGenderPrompt),
            nameof(AppearanceHelper.NailColor),
            nameof(AppearanceHelper.PubicHairColor)
        };

        //public static string Save(AppearanceHelper data)
        //{
        //    var values = new uint[193];

        //    values[0] = (uint)data.SkinTone - 1;
        //    values[1] = (uint)data.SkinType - 1;

        //    for (int i = 2; i < 7; i++)
        //    {
        //        values[i] = data.SkinType == i - 1 ? 0 : uint.MaxValue;
        //    }

        //    var hairNum = data.HairStyle == "Shaved" ? 38 : uint.Parse(data.HairStyle.Substring(data.HairStyle.Length - 2)) - 1;
        //    values[7] = hairNum;

        //    for (int i = 8; i < 46; i++)
        //    {
        //        values[i] = hairNum == i - 8 ? (uint)data.HairColor - 1 : uint.MaxValue;
        //    }

        //    values[46] = (uint)data.Eyes - 1;
        //    values[47] = (uint)data.EyeColor - 1;
        //    values[48] = (uint)data.Eyebrows - 1;

        //    for (int i = 49; i < 58; i++)
        //    {
        //        values[i] = i - 48 == data.Eyebrows ? (uint)data.EyebrowColor - 1 : uint.MaxValue;
        //    }

        //    values[58] = (uint)data.Nose - 1;
        //    values[59] = (uint)data.Mouth - 1;
        //    values[60] = (uint)data.Jaw - 1;
        //    values[61] = (uint)data.Ears - 1;
        //    values[62] = (uint)data.Cyberware;

        //    for (int i = 63; i < 72; i++)
        //    {
        //        values[i] = i - 63 == data.Cyberware ? 0 : uint.MaxValue;
        //    }

        //    values[72] = (uint)data.FacialScars;
        //    values[73] = (uint)data.FacialTattoos - 1;
        //    values[74] = (uint)data.FacialTattoos - 1;

        //    for (int i = 75; i < 87; i++)
        //    {
        //        values[i] = i - 75 == data.FacialTattoos ? 0 : uint.MaxValue;
        //    }

        //    values[87] = (uint)data.Piercings;

        //    for (int i = 88; i < 103; i++)
        //    {
        //        values[i] = i - 88 == data.Piercings ? (uint)data.PiercingColor - 1 : uint.MaxValue;
        //    }

        //    values[103] = (uint)data.Teeth;
        //    values[104] = (uint)data.EyeMakeup;

        //    for (int i = 105; i < 114; i++)
        //    {
        //        values[i] = i - 105 == data.EyeMakeup ? (uint)data.EyeMakeupColor - 1 : uint.MaxValue;
        //    }

        //    values[114] = (uint)data.LipMakeup;

        //    for (int i = 115; i < 123; i++)
        //    {
        //        values[i] = i - 115 == data.LipMakeup ? (uint)data.LipMakeupColor - 1 : uint.MaxValue;
        //    }

        //    values[123] = (uint)data.CheekMakeup;

        //    for (int i = 124; i < 130; i++)
        //    {
        //        values[i] = i - 124 == data.CheekMakeup ? (uint)data.CheekMakeupColor - 1 : uint.MaxValue;
        //    }

        //    values[130] = (uint)data.Blemishes;

        //    for (int i = 131; i < 135; i++)
        //    {
        //        values[i] = i - 131 == data.Blemishes ? 0 : uint.MaxValue;
        //    }

        //    values[135] = data.Nails == "Short" ? 0 : 1;
        //    values[136] = data.Nails == "Short" ? 0 : 1;
        //    values[137] = (uint)data.NailColor - 1;
        //    values[138] = (uint)data.SkinTone - 1;
        //    values[139] = (uint)data.SkinTone - 1;
        //    values[140] = (uint)data.Chest - 1;
        //    values[141] = (uint)data.Nipples;

        //    for (int i = 142; i < 146; i++)
        //    {
        //        values[i] = i - 141 == data.Nipples ? 0 : uint.MaxValue;
        //    }

        //    values[146] = (uint)data.BodyTattoos;

        //    for (int i = 147; i < 153; i++)
        //    {
        //        values[i] = i - 147 == data.BodyTattoos ? 0 : uint.MaxValue;
        //    }

        //    values[153] = (uint)data.BodyScars;

        //    for (int i = 154; i < 159; i++)
        //    {
        //        values[i] = i - 154 == data.BodyScars ? 0 : uint.MaxValue;
        //    }

        //    values[159] = (uint)data.Genitals;

        //    for (int i = 160; i < 164; i++)
        //    {
        //        values[i] = i - 159 == data.Genitals ? 0 : uint.MaxValue;
        //    }

        //    for (int i = 165; i < 164; i++)
        //    {
        //        values[i] = i - 159 == data.Genitals ? 0 : uint.MaxValue;
        //    }
        //}

        public static void Load(byte[] data, AppearanceHelper helper)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var br = new BinaryReader(ms, Encoding.ASCII))
                {
                    if (br.ReadByte() != 0x5 || br.ReadByte() != 0x7)
                    {
                        throw new Exception("Invalid appearance preset.");
                    }

                    var props = helper.GetType().GetProperties();
                    int i = 0;
                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        if (i > (props.Count() - 1))
                        {
                            break;
                        }

                        int value = Convert.ToInt32(br.ReadByte());
                        if (value == 255 || IgnoredProperties.Contains(props[i].Name))
                        {
                            i++;
                            continue;
                        }

                        if(props[i].PropertyType == typeof(string))
                        {
                            var strList = (List<string>)typeof(AppearanceValueLists).GetProperty(props[i].Name + "s").GetValue(null, null);
                            if (value < strList.Count())
                            {
                                props[i].SetValue(helper, strList[value]);
                            }
                            else
                            {
                                throw new Exception("Invalid appearance preset: bad string index.");
                            }
                        }
                        else if (props[i].PropertyType.IsEnum)
                        {
                            if (Enum.IsDefined(props[i].PropertyType, value))
                            {
                                props[i].SetValue(helper, value);
                            }
                            else
                            {
                                throw new Exception("Invalid appearance preset: bad enum.");
                            }
                        }
                        else
                        {
                            props[i].SetValue(helper, value);
                        }
                        i++;
                    }
                }
            }
        }
    }
}
