using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Extensions
{
    /*public class JsonResolver : ITweakDbResolver
    {
        private static Dictionary<ulong, NameStruct> _items = new Dictionary<ulong, NameStruct>();
        private static Dictionary<string, ulong> _nameToHash = new Dictionary<string, ulong>();

        public string GetName(TweakDBID tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            if (_items.ContainsKey(tdbid))
            {
                return _items[tdbid].Name;
            }

            return $"Unknown_{tdbid}";
        }

        public string GetGameName(TweakDBID tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            if (_items.ContainsKey(tdbid))
            {
                return _items[tdbid].GameName;
            }

            return $"";
        }

        public string GetGameDescription(TweakDBID tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            if (_items.ContainsKey(tdbid))
            {
                return _items[tdbid].GameDescription;
            }

            return $"";
        }

        public ulong GetHash(string itemName)
        {
            return _nameToHash.ContainsKey(itemName) ? _nameToHash[itemName] : 0;
        }

        public JsonResolver(Dictionary<ulong, NameStruct> dictionary)
        {
            _items = dictionary;
            _nameToHash = dictionary.ToDictionary(pair => pair.Value.Name, pair => pair.Key);
        }

        public struct NameStruct
        {
            public string Name { get; set; }
            public string GameName { get; set; }
            public string GameDescription { get; set; }
        }
    }*/

    /*public class BinaryResolver : ITweakDbResolver
    {
        private TweakDbParser parser = new TweakDbParser();
        private byte[] decompressedData;
        public Dictionary<ulong, TdbIdInfo> TdbIdIndex;

        public string GetName(TweakDBID tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            return TdbIdIndex.Keys.Contains(tdbid) ? TdbIdIndex[tdbid].Name : $"Unknown_{tdbid}";
        }

        public string GetGameName(TweakDBID tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            if (TdbIdIndex.Keys.Contains(tdbid) && TdbIdIndex[tdbid].InfoOffset != 0)
            {
                using (var ms = new MemoryStream(decompressedData))
                {
                    using (var br = new BinaryReader(ms, Encoding.UTF8))
                    {
                        br.BaseStream.Seek(TdbIdIndex[tdbid].InfoOffset, SeekOrigin.Begin);
                        return br.ReadString();
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetGameDescription(TweakDBID tdbid)
        {
            if (tdbid == null)
            {
                return "<null>";
            }

            if (TdbIdIndex.Keys.Contains(tdbid) && TdbIdIndex[tdbid].InfoOffset != 0)
            {
                using (var ms = new MemoryStream(decompressedData))
                {
                    using (var br = new BinaryReader(ms, Encoding.UTF8))
                    {
                        br.BaseStream.Seek(TdbIdIndex[tdbid].InfoOffset, SeekOrigin.Begin);
                        br.ReadString();
                        return br.ReadString();
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public ulong GetHash(string itemName)
        {
            return TdbIdIndex.Where(x => x.Value.Name == itemName).Select(x => x.Key).FirstOrDefault();
        }

        public BinaryResolver(byte[] data)
        {
            // SaveFile.ReportProgress(new SaveProgressChangedEventArgs(0, 0, "Item Database"));

            decompressedData = Decompress(data);

            using (var ms = new MemoryStream(decompressedData))
            {
                using (var br = new BinaryReader(ms, Encoding.UTF8))
                {
                    // Magic
                    if (br.ReadByte() != 0x5 || br.ReadByte() != 0x8) throw new Exception();

                    var totalItemsCount = br.ReadUInt32();
                    var infoItemsCount = br.ReadUInt32();
                    var infoIndex = new List<ulong>();

                    TdbIdIndex = new Dictionary<ulong, TdbIdInfo>((int)totalItemsCount);

                    for (uint i = 0; i < totalItemsCount; i++)
                    {
                        var name = br.ReadString();
                        var tdbid = parser.GetTweakDBID(name);
                        TdbIdIndex.Add(tdbid, new TdbIdInfo() { Name = name, InfoOffset = 0 });
                        if (i < infoItemsCount) infoIndex.Add(tdbid);

                        // SaveFile.ReportProgress(new SaveProgressChangedEventArgs((int)i, (int)totalItemsCount));
                    }

                    for (uint i = 0; i < infoItemsCount; i++)
                    {
                        TdbIdIndex[infoIndex[(int)i]].InfoOffset = br.BaseStream.Position;
                        br.BaseStream.Seek(br.Read7BitEncodedInt(), SeekOrigin.Current);
                        br.BaseStream.Seek(br.Read7BitEncodedInt(), SeekOrigin.Current);
                    }
                }
            }
        }

        private static byte[] Decompress(byte[] input)
        {
            using (var source = new MemoryStream(input))
            {
                byte[] lengthBytes = new byte[4];
                source.Read(lengthBytes, 0, 4);

                var length = BitConverter.ToInt32(lengthBytes, 0);
                using (var decompressionStream = new GZipStream(source,
                    CompressionMode.Decompress))
                {
                    var result = new byte[length];
                    decompressionStream.Read(result, 0, length);
                    return result;
                }
            }
        }

        public class TdbIdInfo
        {
            public string Name { get; set; }
            public long InfoOffset { get; set; }
        }
    }

    public static class BinaryDatabaseWriter
    {
        public static byte[] Write(Dictionary<ulong, JsonResolver.NameStruct> sourceJson, string tweakStrPath = "")
        {
            var filteredItems = sourceJson.Where(x => !string.IsNullOrEmpty(x.Value.GameName) || !string.IsNullOrEmpty(x.Value.GameDescription));
            Dictionary<TweakDBID, string> allStrings;
            if (string.IsNullOrEmpty(tweakStrPath))
            {
                allStrings = new Dictionary<TweakDBID, string>();
                foreach (var item in sourceJson)
                {
                    allStrings.Add(item.Key, item.Value.Name);
                }
            }
            else
            {
                allStrings = new TweakDbParser().GetStrings(tweakStrPath, 2);
            }
            
            foreach (var item in filteredItems)
            {
                allStrings.Remove(item.Key);
            }

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms, Encoding.UTF8))
                {
                    bw.Write(new byte[] { 0x5, 0x8 });
                    bw.Write((uint)(allStrings.Count + filteredItems.Count()));
                    bw.Write((uint)filteredItems.Count());

                    foreach (var item in filteredItems)
                    {
                        bw.Write(item.Value.Name);
                    }

                    foreach (var item in allStrings)
                    {
                        bw.Write(item.Value);
                    }

                    foreach (var item in filteredItems)
                    {
                        bw.Write(item.Value.GameName);
                        bw.Write(item.Value.GameDescription);
                    }

                    return Compress(ms.ToArray());
                }
            }
        }

        private static byte[] Compress(byte[] input)
        {
            using (var result = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(input.Length);
                result.Write(lengthBytes, 0, 4);

                using (var compressionStream = new GZipStream(result,
                    CompressionMode.Compress))
                {
                    compressionStream.Write(input, 0, input.Length);
                    compressionStream.Flush();

                }
                return result.ToArray();
            }
        }
    }*/
}
