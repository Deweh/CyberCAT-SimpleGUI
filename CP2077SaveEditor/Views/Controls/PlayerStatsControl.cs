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
        private readonly Form2 _parentForm;

        private readonly Dictionary<Enum, NumericUpDown> _attributeFields, _proficiencyFields, _devPointFields;
        private readonly Dictionary<gamedataProficiencyType, gamedataStatType> _proficiencyToStatMap;

        public PlayerStatsControl(Form2 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;

            //BackgroundImage = Properties.Resources.player_stats;

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
            var playerData = _parentForm.ActiveSaveFile.GetPlayerDevelopmentData();

            lifePathBox.SelectedIndex = (gamedataLifePath)playerData.LifePath switch
            {
                gamedataLifePath.Corporate => 2,
                gamedataLifePath.Nomad => 0,
                gamedataLifePath.StreetKid => 1,
                _ => throw new ArgumentOutOfRangeException()
            };

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

        private void PlayerStatChanged(object sender, EventArgs e)
        {
            if (!_parentForm.IsLoaded)
            {
                return;
            }

            var playerData = _parentForm.ActiveSaveFile.GetPlayerDevelopmentData();
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
            if (lifePathBox.SelectedIndex == 0)
            {
                lifePathPictureBox.Image = Properties.Resources.nomad;
                _parentForm.ActiveSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.Nomad;
            }
            else if (lifePathBox.SelectedIndex == 1)
            {
                lifePathPictureBox.Image = Properties.Resources.streetkid;
                _parentForm.ActiveSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.StreetKid;
            }
            else if (lifePathBox.SelectedIndex == 2)
            {
                lifePathPictureBox.Image = Properties.Resources.corpo;
                _parentForm.ActiveSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.Corporate;
            }
        }
    }
}
