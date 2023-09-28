using CP2077SaveEditor.Extensions;
using CP2077SaveEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class PlayerStatsControl : UserControl, IGameControl
    {
        private readonly Form2 _parentForm;

        private readonly Dictionary<Enum, NumericUpDown> _attributeFields, _proficiencyFields, _devPointFields;

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

            foreach (var proficiency in playerData.Proficiencies)
            {
                if (_proficiencyFields.TryGetValue(proficiency.Type, out var numUpDown))
                {
                    proficiency.CurrentLevel = (CInt32)numUpDown.Value;
                    if (proficiency.CurrentLevel == 60)
                    {
                        proficiency.CurrentExp = 0;
                    }
                }
            }

            foreach (var attribute in playerData.Attributes)
            {
                if (_attributeFields.TryGetValue(attribute.AttributeName, out var numUpDown))
                {
                    attribute.Value = (CInt32)numUpDown.Value;
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

        private void additionalPlayerStatsButton_Click(object sender, EventArgs e)
        {
            if (!Global.StatsSystemEnabled)
            {
                MessageBox.Show("Stats system disabled.");
                return;
            }
            var i = Array.FindIndex(_parentForm.ActiveSaveFile.GetStatsMap().Keys.ToArray(), x => x.EntityHash == 1);
            var details = new ItemDetails();
            details.LoadStatsOnly(_parentForm.ActiveSaveFile.GetStatsMap().Values[i].Seed, _parentForm.ActiveSaveFile, "Player");
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
