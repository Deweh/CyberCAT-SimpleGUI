using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Since patch 2.5 the stats system stores <see cref="gameSavedStatsDataOptimized"/> entries instead of
    /// <see cref="gameSavedStatsData"/>. Their record id and inactive stats are indices into shared tables, and their
    /// modifiers live in slices of one shared binary buffer, deduplicated between entries.
    ///
    /// This exposes those entries as regular <see cref="gameSavedStatsData"/> objects (the format the rest of the
    /// editor works with) and writes changes back in <see cref="Flush"/>. Slices can be shared, so a changed entry
    /// gets a slice of its own instead of editing one in place.
    /// </summary>
    public class OptimizedStatsHelper
    {
        // gameBufferSlice.Combined: the low 30 bits are the slice size, the top 2 bits what it contains
        private const uint SizeMask = 0x3FFFFFFF;
        private const int KindShift = 30;

        private const uint ItemModifiersKind = 0;
        private const uint EntityModifiersKind = 1;
        private const uint ModifierGroupsKind = 2;

        // Lets the UI know which stats came from here, without passing the save around
        private static readonly ConditionalWeakTable<gameSavedStatsData, object> s_handedOut = new();

        private readonly gameStatsStateMapStructure _map;

        private readonly Dictionary<gameSavedStatsDataOptimized, gameSavedStatsData> _materialized = new(ReferenceEqualityComparer.Instance);
        private readonly HashSet<gameSavedStatsDataOptimized> _unparsed = new(ReferenceEqualityComparer.Instance);
        private Dictionary<(gameStatIDType, ulong), gameSavedStatsDataOptimized> _index;

        public OptimizedStatsHelper(gameStatsStateMapStructure map)
        {
            _map = map;
        }

        public static bool IsOptimized(gameStatsStateMapStructure map) => map.ValuesOptimized.Count > 0;

        /// <summary>
        /// True for stats backed by the optimized format, which has no forced modifiers.
        /// </summary>
        public static bool IsOptimizedData(gameSavedStatsData data) => data != null && s_handedOut.TryGetValue(data, out _);

        public bool ContainsSeed(uint seed) =>
            _map.ValuesOptimized.Any(x => x.Seed == seed) || _materialized.Values.Any(x => x.Seed == seed);

        public gameSavedStatsData Get(gameStatsObjectID id)
        {
            if (_index == null)
            {
                _index = new();
                foreach (var entry in _map.ValuesOptimized)
                {
                    _index.TryAdd(((gameStatIDType)entry.IdType, (ulong)entry.EntityHash), entry);
                }
            }

            if (!_index.TryGetValue(((gameStatIDType)id.IdType, (ulong)id.EntityHash), out var optimized))
            {
                return null;
            }

            if (!_materialized.TryGetValue(optimized, out var data))
            {
                data = Materialize(optimized);
                _materialized.Add(optimized, data);
                s_handedOut.AddOrUpdate(data, null);
            }

            return data;
        }

        public void Add(gameStatsObjectID id, gameSavedStatsData data)
        {
            var optimized = new gameSavedStatsDataOptimized
            {
                IdType = id.IdType,
                EntityHash = id.EntityHash
            };

            _map.ValuesOptimized.Add(optimized);
            _index?.TryAdd(((gameStatIDType)id.IdType, (ulong)id.EntityHash), optimized);
            _materialized.Add(optimized, data);
            s_handedOut.AddOrUpdate(data, null);
        }

        private gameSavedStatsData Materialize(gameSavedStatsDataOptimized optimized)
        {
            var modifiers = new ModifiersBuffer();
            var groups = new SavedModifierGroupStatTypesBuffer();

            var data = new gameSavedStatsData
            {
                RecordID = _map.SharedRecordId[(ushort)optimized.RecordIDIndex],
                Seed = optimized.Seed,
                // Same instance, so edits to it need no write back
                StatModifiers = optimized.StatModifiers,
                ModifiersBuffer = new DataBuffer { Data = modifiers },
                SavedModifierGroupStatTypesBuffer = new DataBuffer { Data = groups }
            };

            foreach (var index in optimized.InactiveStatsIndices)
            {
                data.InactiveStats.Add(_map.SharedInactiveStats[(ushort)index]);
            }

            foreach (var index in optimized.BufferSliceIndices)
            {
                var (kind, bytes) = GetSlice((ushort)index);
                var buffer = RedBuffer.CreateBuffer(0, bytes);

                if (kind == ModifierGroupsKind)
                {
                    using var reader = new SavedModifierGroupStatTypesBufferReader(new MemoryStream(bytes));
                    reader.ReadBuffer(buffer);

                    var parsed = (SavedModifierGroupStatTypesBuffer)buffer.Data!;
                    groups.Entries.AddRange(parsed.Entries);
                    if (!parsed.IsParsed)
                    {
                        _unparsed.Add(optimized);
                    }
                }
                else
                {
                    using var reader = new ModifiersBufferReader(new MemoryStream(bytes));
                    reader.ReadBuffer(buffer);

                    var parsed = (ModifiersBuffer)buffer.Data!;
                    modifiers.Entries.AddRange(parsed.Entries);
                    if (!parsed.IsParsed)
                    {
                        _unparsed.Add(optimized);
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Writes the stats handed out by <see cref="Get"/> and <see cref="Add"/> back into the optimized structure.
        /// Unchanged entries are left untouched, so calling this repeatedly is safe.
        /// </summary>
        public void Flush()
        {
            if (_materialized.Count == 0)
            {
                return;
            }

            var writer = new SliceWriter(_map);

            foreach (var (optimized, data) in _materialized)
            {
                // Rewriting a slice we couldn't fully read would lose the parts we didn't understand
                if (_unparsed.Contains(optimized))
                {
                    continue;
                }

                var recordIndex = GetOrAddIndex(_map.SharedRecordId, data.RecordID);
                if (optimized.RecordIDIndex != recordIndex)
                {
                    optimized.RecordIDIndex = recordIndex;
                }

                if (optimized.Seed != data.Seed)
                {
                    optimized.Seed = data.Seed;
                }

                if (!ReferenceEquals(optimized.StatModifiers, data.StatModifiers))
                {
                    optimized.StatModifiers = data.StatModifiers ?? new();
                }

                var inactiveIndices = data.InactiveStats
                    .Select(x => GetOrAddIndex(_map.SharedInactiveStats, x))
                    .ToList();
                SetIfChanged(optimized.InactiveStatsIndices, inactiveIndices, x => optimized.InactiveStatsIndices = x);

                var sliceIndices = new List<ushort>();
                var currentSlices = optimized.BufferSliceIndices.Select(x => (ushort)x).ToList();

                if (data.ModifiersBuffer?.Data is ModifiersBuffer { Entries.Count: > 0 } modifiers)
                {
                    var kind = currentSlices
                        .Select(x => (uint)_map.SharedBufferSlices[x].Combined >> KindShift)
                        .Where(x => x != ModifierGroupsKind)
                        .DefaultIfEmpty(optimized.IdType == gameStatIDType.EntityID ? EntityModifiersKind : ItemModifiersKind)
                        .First();

                    using var ms = new MemoryStream();
                    using (var modifiersWriter = new ModifiersBufferWriter(ms))
                    {
                        modifiersWriter.WriteBuffer(modifiers);
                    }

                    sliceIndices.Add(writer.GetSlice(kind, ms.ToArray(), currentSlices));
                }

                if (data.SavedModifierGroupStatTypesBuffer?.Data is SavedModifierGroupStatTypesBuffer { Entries.Count: > 0 } groups)
                {
                    using var ms = new MemoryStream();
                    using (var groupsWriter = new SavedModifierGroupStatTypesBufferWriter(ms))
                    {
                        groupsWriter.WriteBuffer(groups);
                    }

                    sliceIndices.Add(writer.GetSlice(ModifierGroupsKind, ms.ToArray(), currentSlices));
                }

                SetIfChanged(optimized.BufferSliceIndices, sliceIndices, x => optimized.BufferSliceIndices = x);
            }

            writer.Commit();
        }

        private (uint Kind, byte[] Bytes) GetSlice(ushort index)
        {
            var slice = _map.SharedBufferSlices[index];
            var combined = (uint)slice.Combined;
            var bytes = _map.SharedDataBuffer.Buffer.GetBytes().AsSpan((int)(uint)slice.Offset, (int)(combined & SizeMask)).ToArray();

            return (combined >> KindShift, bytes);
        }

        private static ushort GetOrAddIndex<T>(CArray<T> list, T value) where T : IRedType
        {
            var index = list.IndexOf(value);
            if (index == -1)
            {
                list.Add(value);
                index = list.Count - 1;
            }

            return (ushort)index;
        }

        private static void SetIfChanged(CArray<CUInt16> current, List<ushort> wanted, Action<CArray<CUInt16>> setter)
        {
            if (current.Select(x => (ushort)x).SequenceEqual(wanted))
            {
                return;
            }

            var array = new CArray<CUInt16>();
            foreach (var value in wanted)
            {
                array.Add(value);
            }

            setter(array);
        }

        /// <summary>
        /// Finds or appends slices in the shared buffer.
        /// </summary>
        private class SliceWriter
        {
            private readonly gameStatsStateMapStructure _map;

            private byte[] _bytes;
            private int _usedEnd;
            private bool _changed;
            private Dictionary<(uint, string), ushort> _existing;

            public SliceWriter(gameStatsStateMapStructure map)
            {
                _map = map;
                _map.SharedDataBuffer ??= new DataBuffer();
                _bytes = _map.SharedDataBuffer.Buffer?.GetBytes() ?? Array.Empty<byte>();
                _usedEnd = _map.SharedBufferSlices.Count == 0
                    ? 0
                    : _map.SharedBufferSlices.Max(x => (int)(uint)x.Offset + (int)((uint)x.Combined & SizeMask));
            }

            /// <summary>
            /// Returns the index of a slice with this content, preferring one the entry already uses.
            /// </summary>
            public ushort GetSlice(uint kind, byte[] bytes, List<ushort> preferred)
            {
                foreach (var index in preferred)
                {
                    if (Matches(index, kind, bytes))
                    {
                        return index;
                    }
                }

                // Same deduplication the game does, keeps repeated saves from growing the buffer
                if (_existing == null)
                {
                    _existing = new();
                    for (var i = 0; i < _map.SharedBufferSlices.Count; i++)
                    {
                        var slice = _map.SharedBufferSlices[i];
                        var sliceKind = (uint)slice.Combined >> KindShift;
                        _existing.TryAdd((sliceKind, Convert.ToBase64String(SliceBytes(slice))), (ushort)i);
                    }
                }

                var key = (kind, Convert.ToBase64String(bytes));
                if (_existing.TryGetValue(key, out var existingIndex))
                {
                    return existingIndex;
                }

                if (_usedEnd + bytes.Length > _bytes.Length)
                {
                    // The game allocates the buffer in powers of two
                    var newSize = (int)BitOperations.RoundUpToPowerOf2((uint)(_usedEnd + bytes.Length));
                    Array.Resize(ref _bytes, newSize);
                }

                Buffer.BlockCopy(bytes, 0, _bytes, _usedEnd, bytes.Length);

                _map.SharedBufferSlices.Add(new gameBufferSlice
                {
                    Offset = (uint)_usedEnd,
                    Combined = (uint)bytes.Length | kind << KindShift
                });

                _usedEnd += bytes.Length;
                _changed = true;

                var newIndex = (ushort)(_map.SharedBufferSlices.Count - 1);
                _existing.Add(key, newIndex);

                return newIndex;
            }

            public void Commit()
            {
                if (_changed)
                {
                    _map.SharedDataBuffer.Buffer = RedBuffer.CreateBuffer(0, _bytes);
                }
            }

            private bool Matches(ushort index, uint kind, byte[] bytes)
            {
                var slice = _map.SharedBufferSlices[index];
                return (uint)slice.Combined >> KindShift == kind && SliceBytes(slice).AsSpan().SequenceEqual(bytes);
            }

            private byte[] SliceBytes(gameBufferSlice slice) =>
                _bytes.AsSpan((int)(uint)slice.Offset, (int)((uint)slice.Combined & SizeMask)).ToArray();
        }
    }
}
