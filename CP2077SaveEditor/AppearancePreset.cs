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
            nameof(AppearanceHelper.SuppressBodyGenderPrompt),
            nameof(AppearanceHelper.NailColor),
            nameof(AppearanceHelper.PubicHairColor)
        };

        public static byte[] Save(AppearanceHelper data)
        {
            byte[] saveBytes;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms, Encoding.ASCII))
                {
                    bw.Write(new byte[] { 0x5, 0x7 });

                    foreach (PropertyInfo prop in data.GetType().GetProperties())
                    {
                        if (!IgnoredProperties.Contains(prop.Name))
                        {
                            object value = prop.GetValue(data);
                            if (value is string)
                            {
                                var strList = (List<string>)typeof(AppearanceValueLists).GetProperty(prop.Name + "s").GetValue(null, null);
                                value = strList.IndexOf((string)value);
                            }
                            else if (value.GetType().IsEnum)
                            {
                                value = (int)value;
                            }

                            if ((int)value < 0)
                            {
                                bw.Write((byte)0xFF);
                            }
                            else
                            {
                                bw.Write(Convert.ToByte(value));
                            }
                        }
                        else
                        {
                            bw.Write((byte)0xFF);
                        }
                    }

                    saveBytes = ms.ToArray();
                }
            }

            return saveBytes;
        }

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
