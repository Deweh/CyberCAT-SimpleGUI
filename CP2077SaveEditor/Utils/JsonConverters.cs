using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Utils
{
    public static class JsonConverters
    {
        public class AppearanceResourceConverter : JsonConverter<CResourceReference<appearanceAppearanceResource>>
        {
            public override void WriteJson(JsonWriter writer, CResourceReference<appearanceAppearanceResource> value, JsonSerializer serializer)
            {
                writer.WriteValue((ulong)value.DepotPath);
            }

            public override CResourceReference<appearanceAppearanceResource> ReadJson(JsonReader reader, Type objectType, CResourceReference<appearanceAppearanceResource> existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                object value = reader.Value;
                ulong final;

                if (value is BigInteger bigVal)
                {
                    final = (ulong)bigVal;
                }
                else
                {
                    final = Convert.ToUInt64(value);
                }

                return new CResourceReference<appearanceAppearanceResource>(final);
            }
        }
    }
}
