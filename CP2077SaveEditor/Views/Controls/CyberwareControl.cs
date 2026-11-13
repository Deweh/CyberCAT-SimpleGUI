using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;
using Point = System.Drawing.Point;

namespace CP2077SaveEditor.Views.Controls
{
    /// <summary>
    /// Interactive Ripperdoc / Build-Planner Cyberware screen with real Cyberware implant icons,
    /// anatomical body diagram, vertical Capacity &amp; Armor gauges, and a visual 6-column Cyberware Picker modal with T1..T5++ tier pills.
    /// </summary>
    public class CyberwareControl : UserControl, IGameControl
    {
        private sealed class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
            }
        }

        public sealed class CatalogueTierDto
        {
            public string Tier { get; set; }
            public string ItemId { get; set; }
            public int Capacity { get; set; }
        }

        public sealed class CatalogueEntryDto
        {
            public string Name { get; set; }
            public string Slot { get; set; }
            public int IconIndex { get; set; }
            public bool IsIconic { get; set; }
            public int Capacity { get; set; }
            public List<CatalogueTierDto> Tiers { get; set; } = new();
        }

        private const int AtlasCellSize = 96;
        private const int AtlasColumns = 11;

        private static readonly Lazy<Bitmap> s_bodyBitmap = new(() => Properties.Resources.cyberware_body);
        private static readonly Lazy<Bitmap> s_atlasBitmap = new(() => Properties.Resources.cyberware_atlas);
        private static readonly Lazy<Bitmap[]> s_iconBitmaps = new(() =>
        {
            var atlas = s_atlasBitmap.Value;
            if (atlas == null) return Array.Empty<Bitmap>();
            const int total = 121;
            var icons = new Bitmap[total];
            for (var i = 0; i < total; i++)
            {
                var col = i % AtlasColumns;
                var row = i / AtlasColumns;
                var srcRect = new Rectangle(col * AtlasCellSize, row * AtlasCellSize, AtlasCellSize, AtlasCellSize);
                if (srcRect.Right <= atlas.Width && srcRect.Bottom <= atlas.Height)
                {
                    var icon = new Bitmap(AtlasCellSize, AtlasCellSize, PixelFormat.Format32bppPArgb);
                    using var ig = Graphics.FromImage(icon);
                    ig.DrawImage(atlas, new Rectangle(0, 0, AtlasCellSize, AtlasCellSize), srcRect, GraphicsUnit.Pixel);
                    icons[i] = icon;
                }
            }
            return icons;
        });
        private static readonly Lazy<List<CatalogueEntryDto>> s_catalogue = new(() =>
            JsonSerializer.Deserialize<List<CatalogueEntryDto>>(Properties.Resources.CyberwareCatalogue) ?? new());

        // Maps TweakDBID (ulong) -> (CatalogueEntryDto, TierString)
        private static readonly Lazy<Dictionary<ulong, (CatalogueEntryDto Entry, string Tier)>> s_byItemId = new(() =>
        {
            var map = new Dictionary<ulong, (CatalogueEntryDto, string)>();
            foreach (var entry in s_catalogue.Value)
            {
                if (entry.Tiers == null) continue;
                foreach (var t in entry.Tiers)
                {
                    if (string.IsNullOrEmpty(t.ItemId)) continue;
                    var id = (ulong)(TweakDBID)t.ItemId;
                    map.TryAdd(id, (entry, t.Tier));
                }
            }
            return map;
        });

        // Fallback lookup by Display Name (case-insensitive) for pre-2.0 or variant item records
        private static readonly Lazy<Dictionary<string, CatalogueEntryDto>> s_byDisplayName = new(() =>
        {
            var map = new Dictionary<string, CatalogueEntryDto>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in s_catalogue.Value)
            {
                if (!string.IsNullOrEmpty(entry.Name))
                {
                    map.TryAdd(entry.Name, entry);
                }

                // Equipped items can be record variants the catalogue doesn't list (e.g. ...Legendary2PlusPlus) and their
                // names are shown in the UI language, so also index the in-game name of every tier
                foreach (var tier in entry.Tiers ?? new())
                {
                    var localized = ResourceHelper.GetDisplayName((TweakDBID)tier.ItemId);
                    if (!string.IsNullOrEmpty(localized))
                    {
                        map.TryAdd(localized, entry);
                    }
                }
            }
            return map;
        });

        private static readonly string[] s_allTiers =
        {
            "T1", "T1+", "T2", "T2+", "T3", "T3+", "T4", "T4+", "T5", "T5+", "T5++"
        };

        private record AreaGroupDef(string AreaType, string Title, string ModalTitle, bool IsRightColumn, bool AlignInner, float BodyAnchorX, float BodyAnchorY);

        private static readonly AreaGroupDef[] s_areaDefs =
        {
            // Left column (top to bottom)
            new("FrontalCortexCW",        "FRONTAL CORTEX",       "Frontal Cortex",       false, false, 0.49f, 0.08f),
            new("ArmsCW",                 "ARMS",                 "Arms",                 false, true,  0.27f, 0.36f),
            new("MusculoskeletalSystemCW","SKELETON",             "Skeleton",             false, false, 0.48f, 0.27f),
            new("NervousSystemCW",        "NERVOUS SYSTEM",       "Nervous System",       false, false, 0.50f, 0.37f),
            new("IntegumentarySystemCW",  "INTEGUMENTARY SYSTEM", "Integumentary System", false, false, 0.44f, 0.46f),

            // Right column (top to bottom)
            new("SystemReplacementCW",    "OPERATING SYSTEM",     "Operating System",     true,  false, 0.52f, 0.10f),
            new("EyesCW",                 "FACE",                 "Face",                 true,  false, 0.51f, 0.12f),
            new("HandsCW",                "HANDS",                "Hands",                true,  false, 0.76f, 0.46f),
            new("CardiovascularSystemCW", "CIRCULATORY SYSTEM",   "Circulatory System",   true,  false, 0.52f, 0.25f),
            new("LegsCW",                 "LEGS",                 "Legs",                 true,  false, 0.56f, 0.72f),
        };

        private sealed class SlotCard
        {
            public AreaGroupDef Group { get; init; }
            public gameSEquipArea Area { get; init; }
            public gameSEquipSlot Slot { get; init; }
            public int SlotIndex { get; init; }
            public Rectangle Bounds { get; set; }
            public ulong EquippedId => (ulong)(Slot?.ItemID?.Id ?? 0);
            public bool IsEmpty => EquippedId == 0;
            public string DisplayName { get; set; } = string.Empty;
            public string Quality { get; set; }
            public string TierBadge { get; set; }
            public int IconIndex { get; set; } = -1;
            public bool IsIconic { get; set; }
            public int CapacityCost { get; set; }
            public ItemData EquippedItem { get; set; }
        }

        private record Choice(string Label, ItemData Owned, ulong RecordId, string Quality)
        {
            public override string ToString() => Label;
        }

        private readonly MainForm _parentForm;
        private readonly DoubleBufferedPanel _topSummaryBar;
        private readonly DoubleBufferedPanel _canvas;
        private readonly Panel _capacityInputPanel;
        private readonly NumericUpDown _extraCapacity;
        private readonly Label _selectedSlotLabel;
        private readonly ComboBox _items;
        private readonly CheckBox _ownedOnly;
        private readonly Button _equipButton;
        private readonly Button _unequipButton;
        private Bitmap _scaledBodyBitmap;

        private CyberwareHelper _cyberware;
        private readonly List<SlotCard> _slotCards = new();
        private SlotCard _selectedCard;
        private SlotCard _hoveredCard;
        private string _lastSelectedTier = "T5++";

        // Character summary stats for top HUD bar
        private int _playerLevel = 1;
        private int _streetCred = 1;
        private int _attributeSum = 15;
        private int _perkPoints = 0;
        private int _relicPoints = 0;

        public CyberwareControl(MainForm parentForm)
        {
            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;
            DoubleBuffered = true;

            // 1. Top Summary HUD Bar
            _topSummaryBar = new DoubleBufferedPanel
            {
                Dock = DockStyle.Top,
                Height = 34
            };
            _topSummaryBar.Paint += TopSummaryBar_Paint;

            // 2. Bottom Cyberware Selector Bar (fallback dropdown for any custom/modded cyberware)
            var bottom = new DoubleBufferedPanel
            {
                Dock = DockStyle.Bottom,
                Height = 38
            };
            bottom.Paint += BottomBar_Paint;

            _selectedSlotLabel = new Label
            {
                Text = "// SELECT A SLOT",
                AutoSize = false,
                Size = new Size(195, 26),
                Location = new Point(8, 6),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(Theme.UiFontFamily, 8.75f, FontStyle.Bold)
            };

            _items = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 340,
                Location = new Point(208, 7),
                DropDownHeight = 400
            };

            _ownedOnly = new CheckBox
            {
                Text = "Owned only",
                AutoSize = true,
                Location = new Point(558, 9)
            };
            _ownedOnly.CheckedChanged += (_, _) => FillItems();

            _equipButton = new Button
            {
                Text = "Equip",
                Size = new Size(76, 26),
                Location = new Point(660, 6)
            };
            _equipButton.Click += (_, _) => EquipSelected();

            _unequipButton = new Button
            {
                Text = "Unequip",
                Size = new Size(84, 26),
                Location = new Point(742, 6)
            };
            _unequipButton.Click += (_, _) => UnequipSelected();

            bottom.Controls.AddRange(new Control[] { _selectedSlotLabel, _items, _ownedOnly, _equipButton, _unequipButton });

            // 3. Center Interactive Ripperdoc Canvas
            _canvas = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill
            };
            _canvas.Paint += Canvas_Paint;
            _canvas.Resize += (_, _) =>
            {
                LayoutSlotCards();
                _canvas.Invalidate();
            };
            _canvas.MouseMove += Canvas_MouseMove;
            _canvas.MouseLeave += (_, _) =>
            {
                if (_hoveredCard != null)
                {
                    _hoveredCard = null;
                    _canvas.Cursor = Cursors.Default;
                    _canvas.Invalidate();
                }
            };
            _canvas.MouseDown += Canvas_MouseDown;

            // Bottom-left Capacity widget inside canvas
            _capacityInputPanel = new Panel
            {
                Size = new Size(108, 92),
                Location = new Point(6, 360),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            var capTitle = new Label
            {
                Text = "Extra Capacity",
                AutoSize = false,
                Size = new Size(104, 16),
                Location = new Point(2, 2),
                Font = new Font(Theme.UiFontFamily, 8f, FontStyle.Bold)
            };

            _extraCapacity = new NumericUpDown
            {
                Minimum = -1000,
                Maximum = 100000,
                Width = 56,
                Location = new Point(2, 21)
            };

            var maxCapBtn = new Button
            {
                Text = "MAX",
                Size = new Size(44, 23),
                Location = new Point(61, 20)
            };
            maxCapBtn.Click += (_, _) =>
            {
                if (_cyberware == null) return;
                _extraCapacity.Value = 500;
                _cyberware.SetExtraCapacity(500);
                ChangeTracker.MarkChanged();
                _canvas.Invalidate();
                _topSummaryBar.Invalidate();
            };

            var applyCapBtn = new Button
            {
                Text = "APPLY",
                Size = new Size(103, 24),
                Location = new Point(2, 47)
            };
            applyCapBtn.Click += (_, _) =>
            {
                if (_cyberware == null) return;
                _cyberware.SetExtraCapacity((int)_extraCapacity.Value);
                ChangeTracker.MarkChanged();
                _canvas.Invalidate();
                _topSummaryBar.Invalidate();
            };

            var capSubHint = new Label
            {
                Text = "Bonus Humanity",
                AutoSize = false,
                Size = new Size(104, 14),
                Location = new Point(2, 74),
                ForeColor = SystemColors.GrayText,
                Font = new Font(Theme.UiFontFamily, 7f, FontStyle.Regular)
            };

            _capacityInputPanel.Controls.AddRange(new Control[] { capTitle, _extraCapacity, maxCapBtn, applyCapBtn, capSubHint });
            _canvas.Controls.Add(_capacityInputPanel);

            var toolTip = new ToolTip();
            Theme.SetToolTip(toolTip, _extraCapacity, "Additive Humanity stat modifier added on top of V's base cyberware capacity.");
            Theme.SetToolTip(toolTip, applyCapBtn, "Apply the extra cyberware capacity modifier to V.");
            Theme.SetToolTip(toolTip, maxCapBtn, "Set extra cyberware capacity to +500 immediately.");
            Theme.SetToolTip(toolTip, _equipButton, "Equip the chosen cyberware into the selected slot (creates the item if not in V's inventory).");
            Theme.SetToolTip(toolTip, _unequipButton, "Unequip the cyberware in the selected slot (the item stays in V's inventory).");

            Controls.Add(_canvas);
            Controls.Add(_topSummaryBar);
            Controls.Add(bottom);
        }

        public string GameControlName => "Cyberware";
        public string Icon => "";

        private void OnParentFormPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_parentForm.ActiveSaveFile) && _parentForm.ActiveSaveFile != null)
            {
                this.InvokeIfRequired(Init);
            }
        }

        private void Init()
        {
            _cyberware = new CyberwareHelper(_parentForm.ActiveSaveFile);
            _extraCapacity.Value = Math.Clamp(_cyberware.GetExtraCapacity(), (int)_extraCapacity.Minimum, (int)_extraCapacity.Maximum);
            ReadCharacterSummary();
            RebuildSlotCards();
        }

        private void ReadCharacterSummary()
        {
            try
            {
                var devData = _parentForm.ActiveSaveFile.GetPlayerDevelopmentData();
                if (devData?.Proficiencies != null)
                {
                    var lvl = devData.Proficiencies.FirstOrDefault(x => x.Type == gamedataProficiencyType.Level);
                    if (lvl != null) _playerLevel = lvl.CurrentLevel;

                    var sc = devData.Proficiencies.FirstOrDefault(x => x.Type == gamedataProficiencyType.StreetCred);
                    if (sc != null) _streetCred = sc.CurrentLevel;
                }

                if (devData?.DevPoints != null)
                {
                    _perkPoints = devData.DevPoints.FirstOrDefault(x => x.Type == gamedataDevelopmentPointType.Primary)?.Unspent ?? 0;
                    _relicPoints = devData.DevPoints.FirstOrDefault(x => x.Type == gamedataDevelopmentPointType.Espionage)?.Unspent ?? 0;
                }

                var playerStats = _parentForm.ActiveSaveFile.GetStatsFromEntityId(1)?.ModifiersBuffer?.Data as ModifiersBuffer;
                if (playerStats != null)
                {
                    var attrTypes = new[]
                    {
                        gamedataStatType.Strength,
                        gamedataStatType.Reflexes,
                        gamedataStatType.TechnicalAbility,
                        gamedataStatType.Intelligence,
                        gamedataStatType.Cool
                    };
                    var sum = 0;
                    foreach (var attr in attrTypes)
                    {
                        var mod = playerStats.Entries.OfType<gameConstantStatModifierData_Deprecated>()
                            .FirstOrDefault(x => x.StatType == attr && x.ModifierType == gameStatModifierType.Additive);
                        if (mod != null) sum += (int)mod.Value;
                    }
                    if (sum > 0) _attributeSum = sum;
                }
            }
            catch
            {
                // Ignore missing summary stats on unusual saves
            }
        }

        private void RebuildSlotCards()
        {
            var previousArea = _selectedCard?.Group.AreaType;
            var previousIndex = _selectedCard?.SlotIndex ?? 0;

            _slotCards.Clear();
            if (_cyberware == null)
            {
                return;
            }

            var areasByType = _cyberware.GetCyberwareAreas()
                .GroupBy(x => x.AreaType.ToString())
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var def in s_areaDefs)
            {
                if (!areasByType.TryGetValue(def.AreaType, out var area) || area.EquipSlots == null)
                {
                    continue;
                }

                for (var i = 0; i < area.EquipSlots.Count; i++)
                {
                    var slot = area.EquipSlots[i];
                    var card = new SlotCard
                    {
                        Group = def,
                        Area = area,
                        Slot = slot,
                        SlotIndex = i
                    };
                    PopulateCardMetadata(card);
                    _slotCards.Add(card);
                }
            }

            _selectedCard = _slotCards.FirstOrDefault(c => c.Group.AreaType == previousArea && c.SlotIndex == previousIndex)
                            ?? _slotCards.FirstOrDefault();

            LayoutSlotCards();
            FillItems();
            _topSummaryBar.Invalidate();
            _canvas.Invalidate();
        }

        private void PopulateCardMetadata(SlotCard card)
        {
            if (card.IsEmpty)
            {
                card.DisplayName = string.Empty;
                card.Quality = null;
                card.TierBadge = null;
                card.IconIndex = -1;
                card.IsIconic = false;
                card.CapacityCost = 0;
                card.EquippedItem = null;
                return;
            }

            card.DisplayName = GetName(card.EquippedId);
            card.EquippedItem = _cyberware.FindItem(card.Slot.ItemID);

            var quality = ResourceHelper.ItemClasses.GetValueOrDefault(card.EquippedId)?.Quality;
            if (s_byItemId.Value.TryGetValue(card.EquippedId, out var matched))
            {
                card.IconIndex = matched.Entry.IconIndex;
                card.IsIconic = matched.Entry.IsIconic;
                card.CapacityCost = matched.Entry.Capacity;
                card.TierBadge = matched.Tier;
                quality ??= TierToQualityName(matched.Tier);
            }
            else if (s_byDisplayName.Value.TryGetValue(card.DisplayName, out var byName))
            {
                card.IconIndex = byName.IconIndex;
                card.IsIconic = byName.IsIconic;
                card.CapacityCost = byName.Capacity;
                card.TierBadge = GetTierBadge(quality, card.EquippedId);
            }
            else
            {
                card.IconIndex = -1;
                card.IsIconic = false;
                card.CapacityCost = 0;
                card.TierBadge = GetTierBadge(quality, card.EquippedId);
            }

            card.Quality = quality;
        }

        private static string TierToQualityName(string tier)
        {
            if (string.IsNullOrEmpty(tier)) return null;
            if (tier.StartsWith("T5", StringComparison.OrdinalIgnoreCase)) return "Legendary";
            if (tier.StartsWith("T4", StringComparison.OrdinalIgnoreCase)) return "Epic";
            if (tier.StartsWith("T3", StringComparison.OrdinalIgnoreCase)) return "Rare";
            if (tier.StartsWith("T2", StringComparison.OrdinalIgnoreCase)) return "Uncommon";
            return "Common";
        }

        private static Color GetTierColor(string tierOrQuality)
        {
            var q = TierToQualityName(tierOrQuality) ?? tierOrQuality;
            return Theme.GetQualityColor(q) ?? Color.FromArgb(210, 220, 230);
        }

        private static string GetTierBadge(string quality, ulong id)
        {
            var resolved = ((TweakDBID)id).GetResolvedText() ?? string.Empty;
            var plus = resolved.EndsWith("PlusPlus", StringComparison.OrdinalIgnoreCase) ? "++"
                     : resolved.EndsWith("Plus", StringComparison.OrdinalIgnoreCase) ? "+" : "";

            if (string.IsNullOrEmpty(quality))
            {
                if (resolved.Contains("Legendary", StringComparison.OrdinalIgnoreCase)) quality = "Legendary";
                else if (resolved.Contains("Epic", StringComparison.OrdinalIgnoreCase)) quality = "Epic";
                else if (resolved.Contains("Rare", StringComparison.OrdinalIgnoreCase)) quality = "Rare";
                else if (resolved.Contains("Uncommon", StringComparison.OrdinalIgnoreCase)) quality = "Uncommon";
            }

            return (quality?.Replace("Plus", "")) switch
            {
                "Legendary" => "T5" + plus,
                "Epic"      => "T4" + plus,
                "Rare"      => "T3" + plus,
                "Uncommon"  => "T2" + plus,
                "Common"    => "T1" + plus,
                _           => "CW"
            };
        }

        private static void DrawAtlasIcon(Graphics g, int iconIndex, Rectangle destRect)
        {
            var icons = s_iconBitmaps.Value;
            if (iconIndex < 0 || iconIndex >= icons.Length)
            {
                return;
            }

            var icon = icons[iconIndex];
            if (icon == null)
            {
                return;
            }

            var prevInterp = g.InterpolationMode;
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.DrawImage(icon, destRect);
            g.InterpolationMode = prevInterp;
        }

        /// <summary>
        /// Draws the golden circuit-board background pattern exclusively for Iconic cyberware in Cyberpunk 2077.
        /// </summary>
        private static void DrawIconicCircuitPattern(Graphics g, Rectangle bounds, bool isIconic)
        {
            if (!isIconic)
            {
                return;
            }

            // Subtle golden Iconic gradient wash at top-right
            using (var washBrush = new LinearGradientBrush(
                bounds,
                Color.FromArgb(10, 235, 180, 30),
                Color.FromArgb(48, 235, 180, 30),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(washBrush, bounds);
            }

            using var goldPen = new Pen(Color.FromArgb(115, 240, 185, 35), 1.3f);
            using var padBrush = new SolidBrush(Color.FromArgb(150, 250, 200, 45));

            // Right-side diagonal PCB traces
            var rx = bounds.Right - 4;
            var ty = bounds.Top + 4;
            for (var i = 0; i < 4; i++)
            {
                var offset = i * 6;
                var p1 = new Point(rx - 18 - offset, ty + 2);
                var p2 = new Point(rx - 8 - offset, ty + 12);
                var p3 = new Point(rx - 8 - offset, bounds.Bottom - 10 - offset);
                var p4 = new Point(rx - 2, bounds.Bottom - 4 - offset);
                if (p1.X > bounds.Left + 6 && p3.Y > p2.Y)
                {
                    g.DrawLines(goldPen, new[] { p1, p2, p3, p4 });
                    g.FillEllipse(padBrush, p1.X - 2, p1.Y - 2, 4, 4);
                }
            }

            // Left-side subtle PCB traces (matching Gamestegy's full-width Iconic circuit backdrop)
            var lx = bounds.Left + 6;
            for (var i = 0; i < 3; i++)
            {
                var offset = i * 6;
                var p1 = new Point(lx + 16 + offset, ty + 2);
                var p2 = new Point(lx + 6 + offset, ty + 12);
                var p3 = new Point(lx + 6 + offset, bounds.Bottom - 12 - offset);
                if (p1.X < bounds.Right - 10 && p3.Y > p2.Y)
                {
                    g.DrawLines(goldPen, new[] { p1, p2, p3 });
                    g.FillEllipse(padBrush, p1.X - 2, p1.Y - 2, 4, 4);
                }
            }
        }

        private void LayoutSlotCards()
        {
            var w = _canvas.ClientSize.Width;
            var h = _canvas.ClientSize.Height;
            if (w <= 100 || h <= 100)
            {
                return;
            }

            _capacityInputPanel.Location = new Point(6, Math.Max(180, h - _capacityInputPanel.Height - 6));

            var leftGaugeWidth = 116;
            var rightGaugeWidth = 68;
            var centerWidth = Math.Clamp((int)(w * 0.25f), 165, 275);
            var sideWidth = Math.Max(180, (w - leftGaugeWidth - rightGaugeWidth - centerWidth) / 2);

            var leftColX = leftGaugeWidth + 8;
            var rightColX = w - rightGaugeWidth - sideWidth - 4;

            var cardGap = 7;
            var rowStep = Math.Max(74, (h - 12) / 5);
            var cardH = Math.Clamp(rowStep - 22, 52, 74);
            var cardW = Math.Clamp(Math.Min((sideWidth - cardGap * 2 - 8) / 3, cardH + 6), 54, 78);

            var leftDefs = s_areaDefs.Where(d => !d.IsRightColumn).ToList();
            for (var r = 0; r < leftDefs.Count; r++)
            {
                var def = leftDefs[r];
                var cards = _slotCards.Where(c => c.Group.AreaType == def.AreaType).ToList();
                var y = 22 + r * rowStep;
                for (var i = 0; i < cards.Count; i++)
                {
                    var slotIndex = def.AlignInner ? (2 - i) : i;
                    var x = leftColX + (slotIndex % 3) * (cardW + cardGap);
                    var rowExtraY = (i / 3) * (cardH + 4);
                    cards[i].Bounds = new Rectangle(x, y + rowExtraY, cardW, cardH);
                }
            }

            var rightDefs = s_areaDefs.Where(d => d.IsRightColumn).ToList();
            for (var r = 0; r < rightDefs.Count; r++)
            {
                var def = rightDefs[r];
                var cards = _slotCards.Where(c => c.Group.AreaType == def.AreaType).ToList();
                var y = 22 + r * rowStep;
                for (var i = 0; i < cards.Count; i++)
                {
                    var x = rightColX + (i % 3) * (cardW + cardGap);
                    var rowExtraY = (i / 3) * (cardH + 4);
                    cards[i].Bounds = new Rectangle(x, y + rowExtraY, cardW, cardH);
                }
            }
        }

        private Rectangle GetBodyImageBounds()
        {
            var w = _canvas.ClientSize.Width;
            var h = _canvas.ClientSize.Height;
            var leftGaugeWidth = 116;
            var rightGaugeWidth = 68;
            var centerWidth = Math.Clamp((int)(w * 0.25f), 165, 275);
            var sideWidth = Math.Max(180, (w - leftGaugeWidth - rightGaugeWidth - centerWidth) / 2);

            var centerStartX = leftGaugeWidth + sideWidth + 6;
            var centerActualW = Math.Max(130, w - leftGaugeWidth - rightGaugeWidth - sideWidth * 2 - 12);
            var availH = h - 10;

            var img = s_bodyBitmap.Value;
            if (img == null)
            {
                return new Rectangle(centerStartX, 5, centerActualW, availH);
            }

            var scale = Math.Min((float)centerActualW / img.Width, (float)availH / img.Height);
            var drawW = Math.Max(1, (int)(img.Width * scale));
            var drawH = Math.Max(1, (int)(img.Height * scale));
            var drawX = centerStartX + (centerActualW - drawW) / 2;
            var drawY = 5 + (availH - drawH) / 2;
            return new Rectangle(drawX, drawY, drawW, drawH);
        }

        private void TopSummaryBar_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bg = Theme.IsDark ? Theme.DarkGround : SystemColors.Control;
            g.Clear(bg);

            var rect = new Rectangle(4, 2, _topSummaryBar.Width - 9, _topSummaryBar.Height - 5);
            if (rect.Width <= 40)
            {
                return;
            }

            var pts = Theme.GetChamferPoints(rect, 0, 8);
            using (var fill = new SolidBrush(Theme.IsDark ? Theme.DarkSurface : Color.White))
            using (var border = new Pen(Theme.IsDark ? Theme.NeonRed : Theme.BorderColor, 1f))
            {
                g.FillPolygon(fill, pts);
                g.DrawPolygon(border, pts);
            }

            var usedCap = _slotCards.Where(c => !c.IsEmpty).Sum(c => c.CapacityCost);
            var extraCap = _cyberware?.GetExtraCapacity() ?? 0;

            var metrics = new (string Label, string Value, Color ValueColor)[]
            {
                ("LEVEL", _playerLevel.ToString(), Theme.CyanAccent),
                ("STREET CRED", _streetCred.ToString(), Theme.CyanAccent),
                ("ATTRIBUTES", $"{_attributeSum}/81", Theme.CyanAccent),
                ("PERK POINTS", _perkPoints.ToString(), Theme.AccentBar),
                ("RELIC", $"{_relicPoints}/15", Theme.CyanAccent),
                ("CAPACITY USED", usedCap.ToString(), Theme.AccentBar),
                ("EXTRA CAP", extraCap >= 0 ? $"+{extraCap}" : extraCap.ToString(), Theme.CyanAccent)
            };

            using var labelFont = new Font(Theme.UiFontFamily, 8f, FontStyle.Bold);
            using var valFont = new Font(Theme.MonoFontFamily, 10.5f, FontStyle.Bold);
            var segW = rect.Width / metrics.Length;

            for (var i = 0; i < metrics.Length; i++)
            {
                var (lbl, val, color) = metrics[i];
                var segX = rect.Left + i * segW + 8;
                var lblSize = TextRenderer.MeasureText(g, lbl, labelFont);
                TextRenderer.DrawText(g, lbl, labelFont, new Point(segX, rect.Top + (rect.Height - lblSize.Height) / 2),
                    Theme.IsDark ? Theme.DarkText : SystemColors.ControlText);

                var valSize = TextRenderer.MeasureText(g, val, valFont);
                TextRenderer.DrawText(g, val, valFont, new Point(segX + lblSize.Width + 2, rect.Top + (rect.Height - valSize.Height) / 2 - 1),
                    color);
            }
        }

        private void BottomBar_Paint(object sender, PaintEventArgs e)
        {
            if (!Theme.IsDark)
            {
                return;
            }
            using var pen = new Pen(Theme.NeonRed, 1f);
            using var yellowPen = new Pen(Theme.Accent, 2f);
            e.Graphics.DrawLine(pen, 0, 0, _canvas.Width, 0);
            e.Graphics.DrawLine(yellowPen, 8, 0, 160, 0);
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Theme.IsDark ? Theme.DarkGround : SystemColors.Control);

            var bodyBounds = GetBodyImageBounds();

            // 1. Draw the central translucent cybernetic human figure from pre-scaled PARGB cache
            var bodyImg = s_bodyBitmap.Value;
            if (bodyImg != null && bodyBounds.Width > 20 && bodyBounds.Height > 20)
            {
                if (_scaledBodyBitmap == null || _scaledBodyBitmap.Width != bodyBounds.Width || _scaledBodyBitmap.Height != bodyBounds.Height)
                {
                    _scaledBodyBitmap?.Dispose();
                    _scaledBodyBitmap = new Bitmap(bodyBounds.Width, bodyBounds.Height, PixelFormat.Format32bppPArgb);
                    using var bg = Graphics.FromImage(_scaledBodyBitmap);
                    bg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    bg.DrawImage(bodyImg, new Rectangle(0, 0, bodyBounds.Width, bodyBounds.Height));
                }
                g.DrawImageUnscaled(_scaledBodyBitmap, bodyBounds.Location);
            }

            // 2. Draw HUD anatomical callout line for the hovered or selected slot group
            var activeCard = _hoveredCard ?? _selectedCard;
            if (activeCard != null && bodyBounds.Width > 20)
            {
                DrawAnatomicalCallout(g, activeCard, bodyBounds);
            }

            // 3. Draw vertical CAPACITY gauge (left) and ARMOR/CHROME gauge (right)
            DrawLeftCapacityGauge(g);
            DrawRightChromeGauge(g);

            // 4. Draw section headers and slot cards
            DrawSlotGroupsAndCards(g);

            // 5. Instant zero-lag HUD readout pill for hovered slot card
            if (_hoveredCard != null)
            {
                var status = _hoveredCard.IsEmpty
                    ? $"{_hoveredCard.Group.Title} #{_hoveredCard.SlotIndex + 1} — EMPTY (Click to install)"
                    : $"{_hoveredCard.DisplayName} [{_hoveredCard.TierBadge}] · Cap {_hoveredCard.CapacityCost} (Click: change | Right-click: unequip)";
                using var hudFont = new Font(Theme.UiFontFamily, 8f, FontStyle.Bold);
                var sz = TextRenderer.MeasureText(g, status, hudFont);
                var pillW = sz.Width + 20;
                var pillH = 22;
                var pillRect = new Rectangle((_canvas.ClientSize.Width - pillW) / 2, _canvas.ClientSize.Height - pillH - 4, pillW, pillH);
                using var pillFill = new SolidBrush(Color.FromArgb(230, 18, 9, 14));
                using var pillBorder = new Pen(Theme.Cyan, 1f);
                g.FillRectangle(pillFill, pillRect);
                g.DrawRectangle(pillBorder, pillRect);
                TextRenderer.DrawText(g, status, hudFont, pillRect, Theme.Cyan,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private void DrawAnatomicalCallout(Graphics g, SlotCard card, Rectangle bodyBounds)
        {
            var def = card.Group;
            var targetX = bodyBounds.Left + (int)(bodyBounds.Width * def.BodyAnchorX);
            var targetY = bodyBounds.Top + (int)(bodyBounds.Height * def.BodyAnchorY);

            var cardAnchorX = def.IsRightColumn ? card.Bounds.Left : card.Bounds.Right;
            var cardAnchorY = card.Bounds.Top + card.Bounds.Height / 2;
            var elbowX = (cardAnchorX + targetX) / 2;

            var lineColor = _hoveredCard != null ? Theme.Cyan : Theme.Accent;
            using var glowPen = new Pen(Color.FromArgb(55, lineColor), 4f);
            using var linePen = new Pen(Color.FromArgb(190, lineColor), 1.3f);

            var pts = new[]
            {
                new Point(cardAnchorX, cardAnchorY),
                new Point(elbowX, cardAnchorY),
                new Point(targetX, targetY)
            };
            g.DrawLines(glowPen, pts);
            g.DrawLines(linePen, pts);

            using var fillBrush = new SolidBrush(lineColor);
            g.FillEllipse(fillBrush, targetX - 3, targetY - 3, 6, 6);
            g.DrawEllipse(linePen, targetX - 7, targetY - 7, 14, 14);
        }

        private void DrawLeftCapacityGauge(Graphics g)
        {
            var topY = 8;
            var bottomY = _capacityInputPanel.Top - 32;
            if (bottomY <= topY + 60)
            {
                return;
            }

            using var headerFont = new Font(Theme.UiFontFamily, 8.5f, FontStyle.Bold);
            TextRenderer.DrawText(g, "⬡ CAPACITY", headerFont, new Point(10, topY), Theme.NeonRed);

            var barX = 42;
            var barTop = topY + 24;
            var barBottom = bottomY - 4;
            var totalTicks = 38;
            var stepY = (float)(barBottom - barTop) / totalTicks;

            var usedCap = _slotCards.Where(c => !c.IsEmpty).Sum(c => c.CapacityCost);
            var activeTicks = Math.Clamp((int)Math.Round(usedCap / 350.0 * totalTicks), 0, totalTicks);

            using var inactiveBrush = new SolidBrush(Color.FromArgb(55, 110, 145, 165));
            using var yellowBrush = new SolidBrush(Theme.Accent);
            using var redBrush = new SolidBrush( Color.FromArgb(255, 95, 95));

            for (var i = 0; i < totalTicks; i++)
            {
                var tickFromBottom = totalTicks - 1 - i;
                var ty = (int)(barTop + i * stepY);
                var brush = tickFromBottom >= activeTicks
                    ? inactiveBrush
                    : (tickFromBottom > 12 ? redBrush : yellowBrush);
                g.FillRectangle(brush, barX, ty, 11, Math.Max(2, (int)stepY - 2));
                g.FillRectangle(brush, barX + 13, ty, 11, Math.Max(2, (int)stepY - 2));
            }

            // Yellow badge at the active level of the capacity bar
            var badgeY = Math.Clamp((int)(barBottom - activeTicks * stepY - 10), barTop, barBottom - 20);
            var badgeRect = new Rectangle(barX + 28, badgeY, 36, 20);
            using (var yellowFill = new SolidBrush(Theme.Accent))
            using (var monoFont = new Font(Theme.MonoFontFamily, 8f, FontStyle.Bold))
            {
                g.FillRectangle(yellowFill, badgeRect);
                TextRenderer.DrawText(g, usedCap.ToString(), monoFont, badgeRect, Theme.OnAccent,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void DrawRightChromeGauge(Graphics g)
        {
            var w = _canvas.ClientSize.Width;
            var h = _canvas.ClientSize.Height;
            var rightX = w - 62;
            var topY = 8;
            var bottomY = h - 38;
            if (bottomY <= topY + 60)
            {
                return;
            }

            using var headerFont = new Font(Theme.UiFontFamily, 8.5f, FontStyle.Bold);
            TextRenderer.DrawText(g, "⛨ ARMOR", headerFont, new Point(rightX - 4, topY), Theme.NeonRed);

            var barX = rightX + 18;
            var barTop = topY + 24;
            var barBottom = bottomY - 4;
            var totalTicks = 40;
            var stepY = (float)(barBottom - barTop) / totalTicks;

            var equippedCount = _slotCards.Count(c => !c.IsEmpty);
            var totalSlots = Math.Max(1, _slotCards.Count);
            var activeTicks = (int)Math.Round((double)equippedCount / totalSlots * totalTicks);

            using var inactiveBrush = new SolidBrush(Color.FromArgb(55, 110, 145, 165));
            using var activeBrush = new SolidBrush(Theme.Cyan);

            for (var i = 0; i < totalTicks; i++)
            {
                var tickFromBottom = totalTicks - 1 - i;
                var ty = (int)(barTop + i * stepY);
                var brush = tickFromBottom < activeTicks ? activeBrush : inactiveBrush;
                g.FillRectangle(brush, barX, ty, 24, Math.Max(2, (int)stepY - 2));
            }

            // Cyan badge showing estimated armor / chrome score
            var armorScore = equippedCount * 42;
            var badgeY = Math.Clamp((int)(barBottom - activeTicks * stepY - 10), barTop, barBottom - 20);
            var badgeRect = new Rectangle(barX - 36, badgeY, 32, 20);
            using (var cyanFill = new SolidBrush(Theme.Cyan))
            using (var monoFont = new Font(Theme.MonoFontFamily, 8f, FontStyle.Bold))
            {
                g.FillRectangle(cyanFill, badgeRect);
                TextRenderer.DrawText(g, armorScore.ToString(), monoFont, badgeRect, Theme.OnAccent,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void DrawSlotGroupsAndCards(Graphics g)
        {
            using var titleFont = new Font(Theme.UiFontFamily, 8.5f, FontStyle.Bold);
            using var plusFont = new Font(Theme.MonoFontFamily, 14f, FontStyle.Regular);
            using var itemFont = new Font(Theme.UiFontFamily, 7f, FontStyle.Bold);

            foreach (var def in s_areaDefs)
            {
                var groupCards = _slotCards.Where(c => c.Group.AreaType == def.AreaType).ToList();
                if (groupCards.Count == 0)
                {
                    continue;
                }

                var minX = groupCards.Min(c => c.Bounds.Left);
                var maxX = groupCards.Max(c => c.Bounds.Right);
                var topY = groupCards.Min(c => c.Bounds.Top) - 18;
                var headerRect = new Rectangle(minX, topY, maxX - minX, 16);
                var align = def.IsRightColumn ? TextFormatFlags.Left : (def.AlignInner ? TextFormatFlags.Right : TextFormatFlags.HorizontalCenter);

                var isGroupActive = (_hoveredCard?.Group == def) || (_selectedCard?.Group == def);
                var headerColor = isGroupActive ? Theme.Accent : Color.FromArgb(255, 72, 62);
                TextRenderer.DrawText(g, def.Title, titleFont, headerRect, headerColor, align | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

                foreach (var card in groupCards)
                {
                    DrawSingleSlotCard(g, card, plusFont, itemFont);
                }
            }
        }

        private void DrawSingleSlotCard(Graphics g, SlotCard card, Font plusFont, Font itemFont)
        {
            var r = card.Bounds;
            if (r.Width <= 10 || r.Height <= 10)
            {
                return;
            }

            var isSelected = ReferenceEquals(card, _selectedCard);
            var isHovered = ReferenceEquals(card, _hoveredCard);
            var cut = 7;
            var pts = Theme.GetChamferPoints(r, 0, cut);

            var fillColor = isSelected
                ? Color.FromArgb(42, 16, 22)
                : isHovered
                    ? Color.FromArgb(46, 14, 24)
                    : card.IsEmpty
                        ? Color.FromArgb(24, 9, 14)
                        : Color.FromArgb(20, 12, 18);

            using (var fillBrush = new SolidBrush(fillColor))
            {
                g.FillPolygon(fillBrush, pts);
            }

            if (!card.IsEmpty)
            {
                // Golden circuit board traces (brighter for Iconic cyberware, subtle for standard)
                DrawIconicCircuitPattern(g, r, card.IsIconic);

                // Draw the Cyberware implant icon centered inside the slot tile
                if (card.IconIndex >= 0)
                {
                    var pad = Math.Max(4, r.Height / 10);
                    var iconSize = Math.Min(r.Width - pad * 2 - 4, r.Height - pad * 2);
                    var iconRect = new Rectangle(
                        r.Left + 4 + (r.Width - 4 - iconSize) / 2,
                        r.Top + (r.Height - iconSize) / 2,
                        iconSize,
                        iconSize);
                    DrawAtlasIcon(g, card.IconIndex, iconRect);
                }
                else
                {
                    // Fallback text if custom/modded cyberware has no atlas icon
                    var textRect = new Rectangle(r.Left + 6, r.Top + 4, r.Width - 10, r.Height - 8);
                    TextRenderer.DrawText(g, card.DisplayName, itemFont, textRect, Theme.DarkText,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }

                // Left vertical rarity bar (exactly like Gamestegy / Cyberpunk 2077 2.0)
                var rarityColor = GetTierColor(card.TierBadge ?? card.Quality);
                using (var rarityBrush = new SolidBrush(rarityColor))
                {
                    g.FillRectangle(rarityBrush, r.Left + 2, r.Top + 3, 3, r.Height - 6);
                }

                // Cyan corner triangle in bottom-right chamfer
                using (var cyanBrush = new SolidBrush(Theme.Cyan))
                {
                    var tri = new[]
                    {
                        new Point(r.Right - 1, r.Bottom - cut - 5),
                        new Point(r.Right - 1, r.Bottom - cut),
                        new Point(r.Right - cut, r.Bottom - 1),
                        new Point(r.Right - cut - 5, r.Bottom - 1)
                    };
                    g.FillPolygon(cyanBrush, tri);
                }
            }
            else
            {
                var plusColor = isHovered || isSelected ? Theme.Accent : Color.FromArgb(95, 225, 235);
                TextRenderer.DrawText(g, "+", plusFont, r, plusColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            var borderColor = isSelected
                ? Theme.Accent
                : isHovered
                    ? Theme.Cyan
                    : card.IsEmpty
                        ? Color.FromArgb(115, 28, 42)
                        : Color.FromArgb(70, 95, 110);

            using (var borderPen = new Pen(borderColor, (isSelected || isHovered) ? 1.8f : 1f))
            {
                g.DrawPolygon(borderPen, pts);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var hit = _slotCards.FirstOrDefault(c => c.Bounds.Contains(e.Location));
            if (ReferenceEquals(hit, _hoveredCard))
            {
                return;
            }

            _hoveredCard = hit;
            _canvas.Cursor = hit != null ? Cursors.Hand : Cursors.Default;
            _canvas.Invalidate();
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            var hit = _slotCards.FirstOrDefault(c => c.Bounds.Contains(e.Location));
            if (hit == null)
            {
                return;
            }

            _selectedCard = hit;
            FillItems();
            _canvas.Invalidate();

            if (e.Button == MouseButtons.Right)
            {
                if (!hit.IsEmpty)
                {
                    UnequipSelected();
                }
                else
                {
                    ShowVisualPickerModal(hit);
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                ShowVisualPickerModal(hit);
            }
        }

        /// <summary>
        /// Opens the visual 6-column Cyberware Picker Modal with T1..T5++ tier pills and real implant icons,
        /// modeled directly after Gamestegy's Build Planner modal.
        /// </summary>
        private void ShowVisualPickerModal(SlotCard slotCard)
        {
            if (_cyberware == null || _parentForm.ActiveSaveFile == null)
            {
                return;
            }

            var areaType = slotCard.Area.AreaType.ToString();
            var areaEntries = s_catalogue.Value
                .Where(x => string.Equals(x.Slot, areaType, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            var activeTier = !slotCard.IsEmpty && !string.IsNullOrEmpty(slotCard.TierBadge) && s_allTiers.Contains(slotCard.TierBadge)
                ? slotCard.TierBadge
                : _lastSelectedTier;

            // Find what is already installed across all slots in this area
            var installedInArea = _slotCards
                .Where(c => c.Group.AreaType == areaType && !c.IsEmpty)
                .Select(c => c.DisplayName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            using var modal = new Form
            {
                Text = slotCard.Group.ModalTitle,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(920, 640),
                Padding = new Padding(2),
                BackColor = Theme.NeonRed,
                ForeColor = Theme.DarkText,
                KeyPreview = true
            };

            var innerW = modal.ClientSize.Width - 4;

            // Outer border & header bar
            var headerPanel = new DoubleBufferedPanel
            {
                Dock = DockStyle.Top,
                Width = innerW,
                Height = 148,
                BackColor = Color.FromArgb(18, 9, 14)
            };

            var closeBtn = new Button
            {
                Text = "✕",
                Size = new Size(36, 30),
                Location = new Point(innerW - 58, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Font = new Font(Theme.UiFontFamily, 10f, FontStyle.Bold)
            };
            closeBtn.Click += (_, _) => modal.Close();

            var searchBox = new TextBox
            {
                PlaceholderText = "Search cyberware",
                Location = new Point(22, 48),
                Size = new Size(innerW - 44, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(14, 7, 11),
                ForeColor = Theme.DarkText,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font(Theme.UiFontFamily, 10.5f)
            };

            var tierHitBoxes = new List<(string Tier, Rectangle Bounds)>();
            var gridPanel = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(18, 9, 14)
            };

            var footerPanel = new DoubleBufferedPanel
            {
                Dock = DockStyle.Bottom,
                Width = innerW,
                Height = 56,
                BackColor = Color.FromArgb(18, 9, 14)
            };

            var removeBtn = new Button
            {
                Text = "REMOVE",
                Size = new Size(104, 34),
                Location = new Point(22, 11),
                Enabled = !slotCard.IsEmpty
            };
            removeBtn.Click += (_, _) =>
            {
                CyberwareHelper.Unequip(slotCard.Slot);
                ChangeTracker.MarkChanged();
                RebuildSlotCards();
                modal.DialogResult = DialogResult.OK;
                modal.Close();
            };

            var cancelBtn = new Button
            {
                Text = "CANCEL",
                Size = new Size(104, 34),
                Location = new Point(innerW - 126, 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            cancelBtn.Click += (_, _) => modal.Close();

            footerPanel.Controls.AddRange(new Control[] { removeBtn, cancelBtn });
            footerPanel.Paint += (_, pe) =>
            {
                using var borderPen = new Pen(Color.FromArgb(55, 255, 0, 60), 1f);
                pe.Graphics.DrawLine(borderPen, 16, 0, footerPanel.Width - 16, 0);
            };

            List<CatalogueEntryDto> filteredEntries = new();
            int hoveredGridIndex = -1;

            CatalogueTierDto ResolveTier(CatalogueEntryDto entry, string targetTier)
            {
                if (entry.Tiers == null || entry.Tiers.Count == 0) return null;
                var exact = entry.Tiers.FirstOrDefault(t => string.Equals(t.Tier, targetTier, StringComparison.OrdinalIgnoreCase));
                if (exact != null) return exact;

                // Pick closest available tier
                var targetIdx = Array.IndexOf(s_allTiers, targetTier);
                return entry.Tiers
                    .OrderBy(t => Math.Abs(Array.IndexOf(s_allTiers, t.Tier) - targetIdx))
                    .First();
            }

            void UpdateFilter()
            {
                var terms = searchBox.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                filteredEntries = areaEntries
                    .Where(e => terms.All(t => e.Name.Contains(t, StringComparison.CurrentCultureIgnoreCase)))
                    .ToList();

                var cols = 6;
                var rows = (int)Math.Ceiling(filteredEntries.Count / (double)cols);
                var cardH = 168;
                var gap = 10;
                gridPanel.AutoScrollMinSize = new Size(0, rows * (cardH + gap) + 20);
                hoveredGridIndex = -1;
                gridPanel.Invalidate();
                headerPanel.Invalidate();
            }

            headerPanel.Paint += (_, pe) =>
            {
                var g = pe.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Modal title in Cyberpunk red/coral
                using var titleFont = new Font(Theme.UiFontFamily, 13f, FontStyle.Bold);
                TextRenderer.DrawText(g, slotCard.Group.ModalTitle, titleFont, new Point(20, 14), Color.FromArgb(255, 72, 62));

                // Yellow border around search box
                using var searchBorderPen = new Pen( Color.FromArgb(210, 190, 40), 1.2f);
                g.DrawRectangle(searchBorderPen, searchBox.Left - 1, searchBox.Top - 1, searchBox.Width + 1, searchBox.Height + 1);

                // Tier selector pills: T1 .. T5++
                tierHitBoxes.Clear();
                using var pillFont = new Font(Theme.UiFontFamily, 8.5f, FontStyle.Bold);
                var px = 22;
                var py = 84;
                foreach (var t in s_allTiers)
                {
                    var sz = TextRenderer.MeasureText(g, t, pillFont);
                    var pillW = Math.Max(34, sz.Width + 14);
                    var pillRect = new Rectangle(px, py, pillW, 26);
                    tierHitBoxes.Add((t, pillRect));

                    var isSelectedTier = string.Equals(t, activeTier, StringComparison.OrdinalIgnoreCase);
                    var fill = isSelectedTier ? Color.FromArgb(18, 65, 78) : Color.FromArgb(26, 22, 32);
                    var border = isSelectedTier ? Theme.Cyan : Color.FromArgb(65, 75, 95);
                    var fg = isSelectedTier ? Theme.Cyan : Color.FromArgb(210, 215, 225);

                    using (var b = new SolidBrush(fill))
                    using (var p = new Pen(border, isSelectedTier ? 1.6f : 1f))
                    {
                        g.FillRectangle(b, pillRect);
                        g.DrawRectangle(p, pillRect);
                    }
                    TextRenderer.DrawText(g, t, pillFont, pillRect, fg, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    px += pillW + 6;
                }

                // "Installed: <Name>, <Tier>" line
                using var infoFont = new Font(Theme.UiFontFamily, 9f, FontStyle.Regular);
                using var infoBoldFont = new Font(Theme.UiFontFamily, 9f, FontStyle.Bold);
                var installedText = slotCard.IsEmpty
                    ? "Installed: None (Empty Slot)"
                    : $"Installed: {slotCard.DisplayName}, {slotCard.TierBadge?.Replace("T", "Tier ")}";
                TextRenderer.DrawText(g, installedText, infoBoldFont, new Point(22, 120), Theme.DarkText);
            };

            headerPanel.MouseDown += (_, me) =>
            {
                foreach (var (tier, bounds) in tierHitBoxes)
                {
                    if (bounds.Contains(me.Location))
                    {
                        activeTier = tier;
                        _lastSelectedTier = tier;
                        headerPanel.Invalidate();
                        gridPanel.Invalidate();
                        break;
                    }
                }
            };
            headerPanel.MouseMove += (_, me) =>
            {
                headerPanel.Cursor = tierHitBoxes.Any(t => t.Bounds.Contains(me.Location)) ? Cursors.Hand : Cursors.Default;
            };

            Rectangle GetGridCardBounds(int index)
            {
                var cols = 6;
                var gap = 10;
                var leftMargin = 22;
                var availW = gridPanel.ClientSize.Width - leftMargin * 2;
                var cardW = Math.Max(120, (availW - gap * (cols - 1)) / cols);
                var cardH = 168;
                var col = index % cols;
                var row = index / cols;
                var scrollY = gridPanel.AutoScrollPosition.Y;
                return new Rectangle(
                    leftMargin + col * (cardW + gap),
                    6 + row * (cardH + gap) + scrollY,
                    cardW,
                    cardH);
            }

            gridPanel.Paint += (_, pe) =>
            {
                var g = pe.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using var nameFont = new Font(Theme.UiFontFamily, 8.75f, FontStyle.Bold);
                using var metaFont = new Font(Theme.UiFontFamily, 8f, FontStyle.Regular);
                using var checkFont = new Font(Theme.UiFontFamily, 8f, FontStyle.Bold);

                for (var i = 0; i < filteredEntries.Count; i++)
                {
                    var entry = filteredEntries[i];
                    var r = GetGridCardBounds(i);
                    if (r.Bottom < 0 || r.Top > gridPanel.ClientSize.Height || !r.IntersectsWith(pe.ClipRectangle))
                    {
                        continue;
                    }

                    var resolvedTier = ResolveTier(entry, activeTier);
                    var tierLabel = resolvedTier?.Tier ?? activeTier;
                    var capCost = resolvedTier?.Capacity ?? entry.Capacity;
                    var isCurrentSlotEquipped = !slotCard.IsEmpty && string.Equals(slotCard.DisplayName, entry.Name, StringComparison.OrdinalIgnoreCase);
                    var isOtherSlotEquipped = !isCurrentSlotEquipped && installedInArea.Contains(entry.Name);
                    var isHovered = i == hoveredGridIndex;

                    var cut = 9;
                    var pts = Theme.GetChamferPoints(r, 0, cut);

                    var bgFill = isCurrentSlotEquipped
                        ? Color.FromArgb(24, 24, 34)
                        : isHovered
                            ? Color.FromArgb(38, 16, 26)
                            : Color.FromArgb(20, 12, 18);

                    using (var fillBrush = new SolidBrush(bgFill))
                    {
                        g.FillPolygon(fillBrush, pts);
                    }

                    // Golden Iconic circuit background on top half of card
                    var topHalf = new Rectangle(r.Left + 2, r.Top + 2, r.Width - 4, 84);
                    DrawIconicCircuitPattern(g, topHalf, entry.IsIconic);

                    // Left vertical rarity bar matching resolved tier
                    var rarityColor = GetTierColor(tierLabel);
                    using (var rarityBrush = new SolidBrush(rarityColor))
                    {
                        g.FillRectangle(rarityBrush, r.Left + 3, r.Top + 4, 3, r.Height - 8);
                    }

                    // Implant icon (74x74 centered at top)
                    var iconRect = new Rectangle(r.Left + (r.Width - 72) / 2, r.Top + 8, 72, 72);
                    DrawAtlasIcon(g, entry.IconIndex, iconRect);

                    // Installed cyan checkmark badge in top-right
                    if (isCurrentSlotEquipped)
                    {
                        var checkRect = new Rectangle(r.Right - 22, r.Top + 5, 16, 16);
                        using var cyanBrush = new SolidBrush(Theme.Cyan);
                        g.FillEllipse(cyanBrush, checkRect);
                        TextRenderer.DrawText(g, "✓", checkFont, checkRect, Theme.OnAccent,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }

                    // Implant Name
                    var nameRect = new Rectangle(r.Left + 8, r.Top + 82, r.Width - 14, 42);
                    var nameColor = isOtherSlotEquipped ? Color.FromArgb(160, 165, 175) : Theme.DarkText;
                    TextRenderer.DrawText(g, entry.Name, nameFont, nameRect, nameColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                    // Capacity / Status subtitle
                    var metaRect = new Rectangle(r.Left + 8, r.Top + 126, r.Width - 14, 34);
                    var metaText = isOtherSlotEquipped
                        ? "Already installed"
                        : (tierLabel != activeTier ? $"{tierLabel} · Capacity {capCost}" : $"Capacity {capCost}");
                    TextRenderer.DrawText(g, metaText, metaFont, metaRect, Color.FromArgb(155, 165, 180),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);

                    // Card border (Cyan if currently equipped or hovered)
                    var borderColor = isCurrentSlotEquipped || isHovered
                        ? Theme.Cyan
                        : Color.FromArgb(58, 66, 82);
                    using (var borderPen = new Pen(borderColor, (isCurrentSlotEquipped || isHovered) ? 1.8f : 1f))
                    {
                        g.DrawPolygon(borderPen, pts);
                    }
                }
            };

            gridPanel.MouseMove += (_, me) =>
            {
                var hitIdx = -1;
                for (var i = 0; i < filteredEntries.Count; i++)
                {
                    if (GetGridCardBounds(i).Contains(me.Location))
                    {
                        hitIdx = i;
                        break;
                    }
                }
                if (hitIdx != hoveredGridIndex)
                {
                    var prev = hoveredGridIndex;
                    hoveredGridIndex = hitIdx;
                    gridPanel.Cursor = hitIdx >= 0 ? Cursors.Hand : Cursors.Default;
                    if (prev >= 0 && prev < filteredEntries.Count)
                    {
                        gridPanel.Invalidate(Rectangle.Inflate(GetGridCardBounds(prev), 3, 3));
                    }
                    if (hitIdx >= 0 && hitIdx < filteredEntries.Count)
                    {
                        gridPanel.Invalidate(Rectangle.Inflate(GetGridCardBounds(hitIdx), 3, 3));
                    }
                }
            };

            gridPanel.MouseDown += (_, me) =>
            {
                if (me.Button != MouseButtons.Left) return;
                for (var i = 0; i < filteredEntries.Count; i++)
                {
                    if (GetGridCardBounds(i).Contains(me.Location))
                    {
                        var chosenEntry = filteredEntries[i];
                        var resolved = ResolveTier(chosenEntry, activeTier);
                        if (resolved != null && !string.IsNullOrEmpty(resolved.ItemId))
                        {
                            var recordId = (ulong)(TweakDBID)resolved.ItemId;
                            var owned = _parentForm.ActiveSaveFile.GetInventory(1).Items
                                .FirstOrDefault(x => (ulong)x.ItemInfo.ItemId.Id == recordId);
                            var q = TierToQualityName(resolved.Tier);
                            EquipChoiceIntoCard(slotCard, new Choice(chosenEntry.Name, owned, recordId, q));
                            modal.DialogResult = DialogResult.OK;
                            modal.Close();
                        }
                        break;
                    }
                }
            };

            searchBox.TextChanged += (_, _) => UpdateFilter();
            modal.KeyDown += (_, ke) =>
            {
                if (ke.KeyCode == Keys.Escape)
                {
                    modal.Close();
                }
            };

            headerPanel.Controls.AddRange(new Control[] { closeBtn, searchBox });
            modal.Controls.Add(gridPanel);
            modal.Controls.Add(headerPanel);
            modal.Controls.Add(footerPanel);

            Theme.Apply(closeBtn);
            Theme.Apply(footerPanel);
            footerPanel.BackColor = Color.FromArgb(18, 9, 14);

            // Ensure right-aligned controls stay inside the docked width even under high-DPI scaling
            closeBtn.Location = new Point(headerPanel.ClientSize.Width - closeBtn.Width - 22, 10);
            searchBox.Width = headerPanel.ClientSize.Width - 44;
            cancelBtn.Location = new Point(footerPanel.ClientSize.Width - cancelBtn.Width - 22, 11);

            UpdateFilter();
            modal.Shown += (_, _) => searchBox.Focus();
            modal.ShowDialog(this);
        }

        private List<Choice> GetChoicesForArea(string areaName, bool ownedOnly)
        {
            var owned = _parentForm.ActiveSaveFile.GetInventory(1).Items
                .Where(x => CyberwareHelper.GetArea(x.ItemInfo.ItemId.Id) == areaName)
                .Select(x =>
                {
                    var q = ResourceHelper.ItemClasses.GetValueOrDefault((ulong)x.ItemInfo.ItemId.Id)?.Quality;
                    var badge = s_byItemId.Value.TryGetValue((ulong)x.ItemInfo.ItemId.Id, out var m)
                        ? m.Tier
                        : GetTierBadge(q, (ulong)x.ItemInfo.ItemId.Id);
                    return new Choice($"{GetName(x.ItemInfo.ItemId.Id)} [{badge}] (owned)", x, (ulong)x.ItemInfo.ItemId.Id, q);
                })
                .OrderBy(x => x.Label, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            var choices = owned.AsEnumerable();
            if (!ownedOnly)
            {
                var ownedIds = owned.Select(x => x.RecordId).ToHashSet();
                choices = choices.Concat(ResourceHelper.CyberwareAreas
                    .Where(x => x.Value == areaName && !ownedIds.Contains(x.Key))
                    .Select(x =>
                    {
                        var q = ResourceHelper.ItemClasses.GetValueOrDefault(x.Key)?.Quality;
                        var badge = s_byItemId.Value.TryGetValue(x.Key, out var m)
                            ? m.Tier
                            : GetTierBadge(q, x.Key);
                        return new Choice($"{GetName(x.Key)} [{badge}]", null, x.Key, q);
                    })
                    .OrderBy(x => x.Label, StringComparer.CurrentCultureIgnoreCase));
            }

            return choices.ToList();
        }

        private void FillItems()
        {
            _items.Items.Clear();
            _equipButton.Enabled = _unequipButton.Enabled = false;

            if (_selectedCard == null || _parentForm.ActiveSaveFile == null)
            {
                _selectedSlotLabel.Text = "// SELECT A SLOT";
                return;
            }

            _selectedSlotLabel.Text = $"// {_selectedCard.Group.Title} #{_selectedCard.SlotIndex + 1}";
            var areaName = _selectedCard.Area.AreaType.ToString();
            var choices = GetChoicesForArea(areaName, _ownedOnly.Checked);

            _items.Items.AddRange(choices.Cast<object>().ToArray());
            if (_items.Items.Count > 0)
            {
                var currentIdx = _selectedCard.IsEmpty
                    ? 0
                    : Math.Max(0, choices.FindIndex(c => c.RecordId == _selectedCard.EquippedId));
                _items.SelectedIndex = currentIdx;
            }

            _equipButton.Enabled = _items.Items.Count > 0;
            _unequipButton.Enabled = !_selectedCard.IsEmpty;
        }

        private void EquipSelected()
        {
            if (_selectedCard == null || _items.SelectedItem is not Choice choice)
            {
                return;
            }

            EquipChoiceIntoCard(_selectedCard, choice);
        }

        private void EquipChoiceIntoCard(SlotCard card, Choice choice)
        {
            var item = choice.Owned;
            if (item == null)
            {
                var record = ResourceHelper.ItemClasses.GetValueOrDefault(choice.RecordId) ?? new ItemRecord { Type = "Cyberware" };
                item = _parentForm.ActiveSaveFile.CreateItem(choice.RecordId, record, 1);
                _parentForm.ActiveSaveFile.GetInventory(1).Items.Add(item);
            }

            _cyberware.Equip(card.Slot, item);
            ChangeTracker.MarkChanged();
            RebuildSlotCards();
        }

        private void UnequipSelected()
        {
            if (_selectedCard != null)
            {
                CyberwareHelper.Unequip(_selectedCard.Slot);
                ChangeTracker.MarkChanged();
                RebuildSlotCards();
            }
        }

        private static string GetName(TweakDBID id) => ResourceHelper.GetDisplayName(id) ?? id.GetResolvedText() ?? ((ulong)id).ToString();

        private static string GetName(ulong id) => GetName((TweakDBID)id);
    }
}
