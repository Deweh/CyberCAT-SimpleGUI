using CP2077SaveEditor.Extensions;
using CP2077SaveEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class PlayerStatsControl : UserControl, IGameControl
    {
        private static readonly Lazy<System.Drawing.Bitmap> s_corpoImg = new(() => Properties.Resources.corpo);
        private static readonly Lazy<System.Drawing.Bitmap> s_nomadImg = new(() => Properties.Resources.nomad);
        private static readonly Lazy<System.Drawing.Bitmap> s_streetkidImg = new(() => Properties.Resources.streetkid);

        private readonly MainForm _parentForm;

        private readonly Dictionary<Enum, NumericUpDown> _attributeFields, _proficiencyFields, _devPointFields;
        private readonly Dictionary<gamedataProficiencyType, gamedataStatType> _proficiencyToStatMap;
        private bool _isLoading;
        private System.Drawing.Bitmap _activeLifepathSource;
        private System.Drawing.Bitmap _scaledLifepathCache;
        private CEnum<gamedataLifePath>? _originalLifePath;

        public PlayerStatsControl(MainForm parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;

            var toolTip = new ToolTip();
            Theme.SetToolTip(toolTip, additionalPlayerStatsButton, "Inspect and edit raw stat modifiers attached to V's entity.");

            lifePathPictureBox.Paint += LifePathPictureBox_Paint;
            lifePathPictureBox.Resize += (_, _) => lifePathPictureBox.Invalidate();

            _attributeFields = new Dictionary<Enum, NumericUpDown>
            {
                {gamedataStatType.Strength, bodyUpDown},
                {gamedataStatType.Reflexes, reflexesUpDown},
                {gamedataStatType.TechnicalAbility, technicalAbilityUpDown},
                {gamedataStatType.Intelligence, intelligenceUpDown},
                {gamedataStatType.Cool, coolUpDown}
            };

            _proficiencyFields = new Dictionary<Enum, NumericUpDown> {
                {gamedataProficiencyType.Level, levelUpDown},
                {gamedataProficiencyType.StreetCred, streetCredUpDown},
                {gamedataProficiencyType.CoolSkill, assassineUpDown},
                {gamedataProficiencyType.IntelligenceSkill, netrunnerUpDown},
                {gamedataProficiencyType.ReflexesSkill, shinobiUpDown},
                {gamedataProficiencyType.StrengthSkill, soloUpDown},
                {gamedataProficiencyType.TechnicalAbilitySkill, techieUpDown}
            };

            _proficiencyToStatMap = new Dictionary<gamedataProficiencyType, gamedataStatType>
            {
                {gamedataProficiencyType.Level, gamedataStatType.Level},
                {gamedataProficiencyType.StreetCred, gamedataStatType.StreetCred},
                {gamedataProficiencyType.CoolSkill, gamedataStatType.CoolSkill},
                {gamedataProficiencyType.IntelligenceSkill, gamedataStatType.IntelligenceSkill},
                {gamedataProficiencyType.ReflexesSkill, gamedataStatType.ReflexesSkill},
                {gamedataProficiencyType.StrengthSkill, gamedataStatType.StrengthSkill},
                {gamedataProficiencyType.TechnicalAbilitySkill, gamedataStatType.TechnicalAbilitySkill},
            };

            _devPointFields = new Dictionary<Enum, NumericUpDown>
            {
                {gamedataDevelopmentPointType.Attribute, attrPointsUpDown},
                {gamedataDevelopmentPointType.Primary, perkPointsUpDown},
                {gamedataDevelopmentPointType.Espionage, relicUpDown},
            };

            foreach (var numUpDown in _attributeFields.Values)
            {
                numUpDown.ValueChanged += PlayerStatChanged;
            }

            foreach (var numUpDown in _proficiencyFields.Values)
            {
                numUpDown.ValueChanged += PlayerStatChanged;
            }

            foreach (var numUpDown in _devPointFields.Values)
            {
                numUpDown.ValueChanged += PlayerStatChanged;
            }
        }

        public string GameControlName => "Player Stats";
        public string Icon => "";

        private void OnParentFormPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_parentForm.ActiveSaveFile))
            {
                if (_parentForm.ActiveSaveFile != null)
                {
                    this.InvokeIfRequired(Init);
                }
            }
        }

        private void Init()
        {
            _isLoading = true;
            try
            {
                var playerData = _parentForm.ActiveSaveFile.GetPlayerDevelopmentData();

                // Lifepath mods (like Fresh Start) store values besides the three vanilla ones. Those show as
                // "Modded" and are written back unchanged, instead of throwing or being replaced.
                _originalLifePath = playerData.LifePath;

                lifePathBox.Items.Clear();
                lifePathBox.Items.Add("Corporate");
                lifePathBox.Items.Add("Nomad");
                lifePathBox.Items.Add("StreetKid");

                var vanillaIndex = (gamedataLifePath)playerData.LifePath switch
                {
                    gamedataLifePath.Corporate => 0,
                    gamedataLifePath.Nomad => 1,
                    gamedataLifePath.StreetKid => 2,
                    _ => -1
                };

                if (vanillaIndex >= 0)
                {
                    lifePathBox.SelectedIndex = vanillaIndex;
                }
                else
                {
                    lifePathBox.Items.Add("Modded");
                    lifePathBox.SelectedIndex = 3;
                }
                
                foreach (var proficiency in playerData.Proficiencies)
                {
                    if (_proficiencyFields.TryGetValue(proficiency.Type, out var numUpDown))
                    {
                        if (proficiency.CurrentLevel > numUpDown.Maximum)
                        {
                            numUpDown.Maximum = proficiency.CurrentLevel;
                        }

                        numUpDown.Value = proficiency.CurrentLevel;
                    }
                }

                foreach (var attribute in playerData.Attributes)
                {
                    if (_attributeFields.TryGetValue(attribute.AttributeName, out var numUpDown))
                    {
                        if (attribute.Value > numUpDown.Maximum)
                        {
                            numUpDown.Maximum = attribute.Value;
                        }

                        numUpDown.Value = attribute.Value;
                    }
                }

                foreach (var devPoint in playerData.DevPoints)
                {
                    if (_devPointFields.TryGetValue(devPoint.Type, out var numUpDown))
                    {
                        numUpDown.SetValue(devPoint.Unspent);
                    }
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void PlayerStatChanged(object sender, EventArgs e)
        {
            if (!_parentForm.IsLoaded || _isLoading)
            {
                return;
            }

            var playerData = _parentForm.ActiveSaveFile.GetPlayerDevelopmentData();
            ChangeTracker.MarkChanged();
            var statsMap = GetPlayerStats();
            if (statsMap == null)
            {
                return;
            }

            if (statsMap.ForcedModifiersBuffer?.Data is not ModifiersBuffer modifiersBuffer)
            {
                modifiersBuffer = new ModifiersBuffer();
                statsMap.ForcedModifiersBuffer = new DataBuffer { Data = modifiersBuffer };
            }

            foreach (var proficiency in playerData.Proficiencies)
            {
                if (_proficiencyFields.TryGetValue(proficiency.Type, out var numUpDown))
                {
                    proficiency.CurrentLevel = (CInt32)numUpDown.Value;
                    if (proficiency.CurrentLevel == 60)
                    {
                        proficiency.CurrentExp = 0;
                    }

                    var statType = _proficiencyToStatMap[proficiency.Type];
                    var stat = modifiersBuffer.Entries.FirstOrDefault(x => x.StatType == statType);
                    if (stat is not gameConstantStatModifierData_Deprecated constantStat)
                    {
                        constantStat = new gameConstantStatModifierData_Deprecated
                        {
                            StatType = statType,
                            ModifierType = gameStatModifierType.Additive,
                            Value = 1
                        };
                        modifiersBuffer.Entries.Add(constantStat);
                    }

                    constantStat.Value = (CFloat)(float)numUpDown.Value;
                }
            }

            foreach (var attribute in playerData.Attributes)
            {
                if (_attributeFields.TryGetValue(attribute.AttributeName, out var numUpDown))
                {
                    attribute.Value = (CInt32)numUpDown.Value;

                    var stat = modifiersBuffer.Entries.FirstOrDefault(x => x.StatType == (gamedataStatType)attribute.AttributeName);
                    if (stat is not gameConstantStatModifierData_Deprecated constantStat)
                    {
                        constantStat = new gameConstantStatModifierData_Deprecated
                        {
                            StatType = attribute.AttributeName,
                            ModifierType = gameStatModifierType.Additive,
                            Value = 1
                        };
                        modifiersBuffer.Entries.Add(constantStat);
                    }

                    constantStat.Value = (CFloat)(float)numUpDown.Value;
                }
            }

            foreach (var devPoint in playerData.DevPoints)
            {
                if (_devPointFields.TryGetValue(devPoint.Type, out var numUpDown))
                {
                    devPoint.Unspent = (CInt32)numUpDown.Value;
                }
            }
        }

        private gameSavedStatsData GetPlayerStats()
        {
            if (!Global.StatsSystemEnabled)
            {
                MessageBox.Show("Stats system disabled.");
                return null;
            }

            var statsMap = _parentForm.ActiveSaveFile.GetStatsFromEntityId(1);
            if (statsMap == null)
            {
                if (MessageBox.Show("Player stats not found. Creating it can cause issues. Do you wish to continue?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return null;
                }

                statsMap = _parentForm.ActiveSaveFile.CreateStatDataFromEntityId(1);
                statsMap.InactiveStats.Add(gamedataStatType.Level);

                var defaultForcedModifiers = new[]
                {
                    gamedataStatType.Assault,
                    gamedataStatType.Athletics,
                    gamedataStatType.Brawling,
                    gamedataStatType.ColdBlood,
                    gamedataStatType.CombatHacking,
                    gamedataStatType.CoolSkill,
                    gamedataStatType.Crafting,
                    gamedataStatType.Demolition,
                    gamedataStatType.Engineering,
                    gamedataStatType.Espionage,
                    gamedataStatType.Gunslinger,
                    gamedataStatType.Hacking,
                    gamedataStatType.IntelligenceSkill,
                    gamedataStatType.Kenjutsu,
                    gamedataStatType.ReflexesSkill,
                    gamedataStatType.Stealth,
                    gamedataStatType.StreetCred,
                    gamedataStatType.StrengthSkill,
                    gamedataStatType.TechnicalAbilitySkill,
                    gamedataStatType.Cool,
                    gamedataStatType.Intelligence,
                    gamedataStatType.Reflexes,
                    gamedataStatType.Strength,
                    gamedataStatType.TechnicalAbility,
                    gamedataStatType.Level
                };

                var forcedModifierBuffer = new ModifiersBuffer();
                foreach (var forcedModifier in defaultForcedModifiers)
                {
                    forcedModifierBuffer.Entries.Add(new gameConstantStatModifierData_Deprecated
                    {
                        StatType = forcedModifier,
                        ModifierType = gameStatModifierType.Additive,
                        Value = 1
                    });
                }

                statsMap.ForcedModifiersBuffer = new DataBuffer()
                {
                    Data = forcedModifierBuffer
                };
            }
            
            return statsMap;
        }

        private void additionalPlayerStatsButton_Click(object sender, EventArgs e)
        {
            var statsMap = GetPlayerStats();
            if (statsMap == null)
            {
                return;
            }

            new StatsForm().Init("Player", statsMap);
        }

        private void lifePathBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                ChangeTracker.MarkChanged();
            }

            lifePathPictureBox.Image = null;
            _scaledLifepathCache?.Dispose();
            _scaledLifepathCache = null;

            switch (lifePathBox.SelectedIndex)
            {
                case 0:
                    _activeLifepathSource = s_corpoImg.Value;
                    if (!_isLoading) _parentForm.ActiveSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.Corporate;
                    break;
                case 1:
                    _activeLifepathSource = s_nomadImg.Value;
                    if (!_isLoading) _parentForm.ActiveSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.Nomad;
                    break;
                case 2:
                    _activeLifepathSource = s_streetkidImg.Value;
                    if (!_isLoading) _parentForm.ActiveSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.StreetKid;
                    break;
                case 3:
                    _activeLifepathSource = null;
                    if (!_isLoading && _originalLifePath is { } originalLifePath) _parentForm.ActiveSaveFile.GetPlayerDevelopmentData().LifePath = originalLifePath;
                    break;
            }
            lifePathPictureBox.Invalidate();
        }

        private void LifePathPictureBox_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var w = lifePathPictureBox.Width;
            var h = lifePathPictureBox.Height;
            if (w <= 10 || h <= 10)
            {
                return;
            }

            var img = _activeLifepathSource;
            if (img != null)
            {
                g.Clear(Theme.IsDark ? Theme.DarkSurface : lifePathPictureBox.BackColor);
                var scale = Math.Min((float)w / img.Width, (float)h / img.Height);
                var drawW = Math.Max(1, (int)(img.Width * scale));
                var drawH = Math.Max(1, (int)(img.Height * scale));
                var drawX = (w - drawW) / 2;
                var drawY = (h - drawH) / 2;

                if (_scaledLifepathCache == null || _scaledLifepathCache.Width != drawW || _scaledLifepathCache.Height != drawH)
                {
                    _scaledLifepathCache?.Dispose();
                    _scaledLifepathCache = new System.Drawing.Bitmap(drawW, drawH, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    using var bg = System.Drawing.Graphics.FromImage(_scaledLifepathCache);
                    bg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    bg.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    bg.DrawImage(img, new System.Drawing.Rectangle(0, 0, drawW, drawH));
                }
                g.DrawImageUnscaled(_scaledLifepathCache, drawX, drawY);
            }

            if (!Theme.IsDark)
            {
                return;
            }

            using var borderPen = new System.Drawing.Pen(Theme.NeonRed, 1f);
            using var cyanPen = new System.Drawing.Pen(Theme.Cyan, 2f);
            using var yellowPen = new System.Drawing.Pen(Theme.Accent, 2f);
            g.DrawRectangle(borderPen, 0, 0, w - 1, h - 1);

            // Corner brackets
            g.DrawLine(cyanPen, 0, 0, 18, 0);
            g.DrawLine(cyanPen, 0, 0, 0, 18);
            g.DrawLine(cyanPen, w - 18, 0, w - 1, 0);
            g.DrawLine(cyanPen, w - 1, 0, w - 1, 18);
            g.DrawLine(yellowPen, 0, h - 18, 0, h - 1);
            g.DrawLine(yellowPen, 0, h - 1, 18, h - 1);
            g.DrawLine(yellowPen, w - 18, h - 1, w - 1, h - 1);
            g.DrawLine(yellowPen, w - 1, h - 18, w - 1, h - 1);
        }
    }
}
