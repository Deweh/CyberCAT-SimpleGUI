using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class StatsControl : UserControl
    {
        private gameSavedStatsData _gameSavedStatsData;

        public StatsControl()
        {
            InitializeComponent();

            cmb_StatType.GotFocus += PopulateStatTypes;
        }

        private void PopulateStatTypes(object sender, EventArgs e)
        {
            ((ComboBox)sender).Items.AddRange(Enum.GetNames(typeof(gamedataStatType)));
        }

        public void Init(gameSavedStatsData gameSavedStatsData)
        {
            _gameSavedStatsData = gameSavedStatsData;

            ReloadData();
        }

        private bool ReloadData()
        {
            lv_Modifiers.Items.Clear();
            lv_ForcedModifiers.Items.Clear();
            lv_InactiveStats.Items.Clear();

            if (_gameSavedStatsData is { ModifiersBuffer.Data: ModifiersBuffer modifiers })
            {
                pnl_ModifiersHide.Visible = false;
                pnl_ModifiersMenu.Enabled = true;

                var listRows = new List<ListViewItem>();
                foreach (var modifier in modifiers.Entries)
                {
                    string[] data = null;

                    if (modifier is gameConstantStatModifierData_Deprecated constantStat)
                    {
                        data = new[] { "Constant", constantStat.ModifierType.ToString(), constantStat.StatType.ToString(), constantStat.Value.ToString() };
                    }

                    if (modifier is gameCombinedStatModifierData_Deprecated combinedStat)
                    {
                        data = new[] { "Combined", combinedStat.ModifierType.ToString(), combinedStat.StatType.ToString(), combinedStat.Value.ToString() };
                    }

                    if (modifier is gameCurveStatModifierData_Deprecated curveStat)
                    {
                        data = new[] { "Curve", curveStat.ModifierType.ToString(), curveStat.StatType.ToString(), "" };
                    }

                    if (modifier is gameRandomStatModifierData_Deprecated randomStat)
                    {
                        data = new[] { "Random", randomStat.ModifierType.ToString(), randomStat.StatType.ToString(), randomStat.Value.ToString() };
                    }

                    if (modifier is gameSubStatModifierData_Deprecated subStat)
                    {
                        data = new[] { "Random", subStat.ModifierType.ToString(), subStat.StatType.ToString(), "" };
                    }

                    if (data == null)
                    {
                        MessageBox.Show("Unsupported stat type found");
                        return false;
                    }

                    listRows.Add(new ListViewItem(data)
                    {
                        Tag = modifier
                    });
                }

                lv_Modifiers.BeginUpdate();
                lv_Modifiers.Items.AddRange(listRows.ToArray());
                lv_Modifiers.EndUpdate();
            }

            if (_gameSavedStatsData is { ForcedModifiersBuffer.Data: ModifiersBuffer forcedModifiers })
            {
                pnl_ForcedModifiersHide.Visible = false;
                pnl_ForcedModifiersMenu.Enabled = true;

                var listRows = new List<ListViewItem>();
                foreach (var modifier in forcedModifiers.Entries)
                {
                    string[] data = null;

                    if (modifier is gameConstantStatModifierData_Deprecated constantStat)
                    {
                        data = new[] { "Constant", constantStat.ModifierType.ToString(), constantStat.StatType.ToString(), constantStat.Value.ToString() };
                    }

                    if (modifier is gameCombinedStatModifierData_Deprecated combinedStat)
                    {
                        data = new[] { "Combined", combinedStat.ModifierType.ToString(), combinedStat.StatType.ToString(), combinedStat.Value.ToString() };
                    }

                    if (modifier is gameCurveStatModifierData_Deprecated curveStat)
                    {
                        data = new[] { "Curve", curveStat.ModifierType.ToString(), curveStat.StatType.ToString(), "" };
                    }

                    if (modifier is gameRandomStatModifierData_Deprecated randomStat)
                    {
                        data = new[] { "Random", randomStat.ModifierType.ToString(), randomStat.StatType.ToString(), randomStat.Value.ToString() };
                    }

                    if (modifier is gameSubStatModifierData_Deprecated subStat)
                    {
                        data = new[] { "Random", subStat.ModifierType.ToString(), subStat.StatType.ToString(), "" };
                    }

                    if (data == null)
                    {
                        MessageBox.Show("Unsupported stat type found");
                        return false;
                    }

                    listRows.Add(new ListViewItem(data)
                    {
                        Tag = modifier
                    });
                }

                lv_ForcedModifiers.BeginUpdate();
                lv_ForcedModifiers.Items.AddRange(listRows.ToArray());
                lv_ForcedModifiers.EndUpdate();
            }

            if (_gameSavedStatsData.InactiveStats is { })
            {
                pnl_InactiveStatsHide.Visible = false;
                pnl_InactiveStatsMenu.Enabled = true;

                var listRows = new List<ListViewItem>();
                foreach (var inactiveStat in _gameSavedStatsData.InactiveStats)
                {
                    var data = new[] { inactiveStat.ToEnumString() };

                    listRows.Add(new ListViewItem(data)
                    {
                        Tag = inactiveStat
                    });
                }

                lv_InactiveStats.BeginUpdate();
                lv_InactiveStats.Items.AddRange(listRows.ToArray());
                lv_InactiveStats.EndUpdate();
            }

            return true;
        }

        private void lv_Modifiers_DoubleClick(object sender, EventArgs e)
        {
            if (lv_Modifiers.SelectedItems.Count > 0)
            {
                var nodeDetails = new StatDetails();
                nodeDetails.LoadStat((gameStatModifierData_Deprecated)lv_Modifiers.SelectedItems[0].Tag, ReloadData);
            }
        }

        private void lv_Modifiers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lv_Modifiers.SelectedItems.Count > 0)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                ((ModifiersBuffer)_gameSavedStatsData.ModifiersBuffer.Data)!.Entries.Remove((gameStatModifierData_Deprecated)lv_Modifiers.SelectedItems[0].Tag);
                lv_Modifiers.Items.Remove(lv_Modifiers.SelectedItems[0]);
            }
        }

        private void btn_ModifierDelete_Click(object sender, EventArgs e)
        {
            if (lv_Modifiers.SelectedItems.Count > 0)
            {
                ((ModifiersBuffer)_gameSavedStatsData.ModifiersBuffer.Data)!.Entries.Remove((gameStatModifierData_Deprecated)lv_Modifiers.SelectedItems[0].Tag);
                lv_Modifiers.Items.Remove(lv_Modifiers.SelectedItems[0]);
            }
        }

        private void btn_ModifierAddCurve_Click(object sender, EventArgs e)
        {
            var newStat = new gameCurveStatModifierData_Deprecated();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newStat, () =>
            {
                ((ModifiersBuffer)_gameSavedStatsData.ModifiersBuffer.Data)!.Entries.Add(newStat);
                ReloadData();

                return true;
            });
        }

        private void btn_ModifierAddCombined_Click(object sender, EventArgs e)
        {
            var newStat = new gameCombinedStatModifierData_Deprecated();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newStat, () =>
            {
                ((ModifiersBuffer)_gameSavedStatsData.ModifiersBuffer.Data)!.Entries.Add(newStat);
                ReloadData();

                return true;
            });
        }

        private void btn_ModifierAddConstant_Click(object sender, EventArgs e)
        {
            var newStat = new gameConstantStatModifierData_Deprecated();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newStat, () =>
            {
                ((ModifiersBuffer)_gameSavedStatsData.ModifiersBuffer.Data)!.Entries.Add(newStat);
                ReloadData();

                return true;
            });
        }

        private void btn_ForcedModifierDelete_Click(object sender, EventArgs e)
        {
            if (lv_ForcedModifiers.SelectedItems.Count > 0)
            {
                ((ModifiersBuffer)_gameSavedStatsData.ForcedModifiersBuffer.Data)!.Entries.Remove((gameStatModifierData_Deprecated)lv_ForcedModifiers.SelectedItems[0].Tag);
                lv_ForcedModifiers.Items.Remove(lv_ForcedModifiers.SelectedItems[0]);
            }
        }

        private void btn_ForcedModifierAddCurve_Click(object sender, EventArgs e)
        {
            var newStat = new gameCurveStatModifierData_Deprecated();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newStat, () =>
            {
                ((ModifiersBuffer)_gameSavedStatsData.ForcedModifiersBuffer.Data)!.Entries.Add(newStat);
                ReloadData();

                return true;
            });
        }

        private void btn_ForcedModifierAddCombined_Click(object sender, EventArgs e)
        {
            var newStat = new gameCombinedStatModifierData_Deprecated();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newStat, () =>
            {
                ((ModifiersBuffer)_gameSavedStatsData.ForcedModifiersBuffer.Data)!.Entries.Add(newStat);
                ReloadData();

                return true;
            });
        }

        private void btn_ForcedModifierAddConstant_Click(object sender, EventArgs e)
        {
            var newStat = new gameConstantStatModifierData_Deprecated();

            var statDialog = new StatDetails();
            statDialog.LoadStat(newStat, () =>
            {
                ((ModifiersBuffer)_gameSavedStatsData.ForcedModifiersBuffer.Data)!.Entries.Add(newStat);
                ReloadData();

                return true;
            });
        }

        private void btn_InactiveStatsCreate_Click(object sender, EventArgs e)
        {
            _gameSavedStatsData.InactiveStats = new CArray<CEnum<Enums.gamedataStatType>>();
            ReloadData();
        }

        private void btn_ForcedModifiersCreate_Click(object sender, EventArgs e)
        {
            _gameSavedStatsData.ForcedModifiersBuffer = new DataBuffer
            {
                Buffer = new RedBuffer
                {
                    Data = new ModifiersBuffer()
                }
            };
            ReloadData();
        }

        private void btn_ModifiersCreate_Click(object sender, EventArgs e)
        {
            _gameSavedStatsData.ModifiersBuffer = new DataBuffer
            {
                Buffer = new RedBuffer
                {
                    Data = new ModifiersBuffer()
                }
            };
            ReloadData();
        }

        private void btn_InactiveStatsDelete_Click(object sender, EventArgs e)
        {
            if (lv_InactiveStats.SelectedItems.Count > 0)
            {
                _gameSavedStatsData.InactiveStats.Remove((CEnum<gamedataStatType>)lv_InactiveStats.SelectedItems[0].Tag);
                lv_InactiveStats.Items.Remove(lv_InactiveStats.SelectedItems[0]);
            }
        }

        private void btn_InactiveStatsAdd_Click(object sender, EventArgs e)
        {
            if (Enum.TryParse<gamedataStatType>(cmb_StatType.Text, out var statType))
            {
                _gameSavedStatsData.InactiveStats.Add(statType);
                ReloadData();
            }
        }
    }
}
