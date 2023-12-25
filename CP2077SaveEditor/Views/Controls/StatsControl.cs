using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
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

            InitInactiveStatsMenuStatTypes();
            InitModifierGroups();

            tv_ModifierGroups.AfterSelect += ModifierGroup_AfterSelect;
        }

        private void InitInactiveStatsMenuStatTypes()
        {
            cmb_StatType.Items.AddRange(Enum.GetNames(typeof(gamedataStatType)));
        }

        private void InitModifierGroups()
        {
            foreach (var groupName in ResourceHelper.ModifierGroups.Select(x => x.Name))
            {
                cmb_ModifierGroupNodeType.Items.Add(groupName);
            }
        }

        private void ModifierGroup_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btn_ModifierGroupAddStat.Enabled = false;
            cmb_ModifierGroupStatType.Enabled = false;

            if (tv_ModifierGroups.SelectedNode.Tag is SavedModifierGroupStatTypesBuffer_Entry entry)
            {
                btn_ModifierGroupAddStat.Enabled = true;
                cmb_ModifierGroupStatType.Enabled = true;

                cmb_ModifierGroupStatType.Items.Clear();

                foreach (var statType in ResourceHelper.ModifierGroups.First(x => x.CRC == entry.ModifierGroupHash).StatTypes)
                {
                    cmb_ModifierGroupStatType.Items.Add(statType);
                }
            }
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
            tv_ModifierGroups.Nodes.Clear();

            pnl_ModifiersHide.Visible = true;
            pnl_ModifiersMenu.Enabled = false;

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

            pnl_ForcedModifiersHide.Visible = true;
            pnl_ForcedModifiersMenu.Enabled = false;

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

            pnl_InactiveStatsHide.Visible = true;
            pnl_InactiveStatsMenu.Enabled = false;

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

            pnl_ModifierGroupHide.Visible = true;
            pnl_ModifierGroupMenu.Enabled = false;

            if (_gameSavedStatsData is { SavedModifierGroupStatTypesBuffer.Data: SavedModifierGroupStatTypesBuffer savedModifierGroupStatTypesBuffer })
            {
                pnl_ModifierGroupHide.Visible = false;
                pnl_ModifierGroupMenu.Enabled = true;

                foreach (var entry in savedModifierGroupStatTypesBuffer.Entries)
                {
                    var rootNode = tv_ModifierGroups.Nodes.Add(ResourceHelper.ModifierGroups.First(x => x.CRC == entry.ModifierGroupHash).Name);
                    rootNode.Tag = entry;

                    foreach (var statType in entry.StatTypes)
                    {
                        var childNode = rootNode.Nodes.Add(statType.ToString());
                        childNode.Tag = statType;
                    }
                }

                tv_ModifierGroups.ExpandAll();
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

        private void btn_ModifierGroupCreate_Click(object sender, EventArgs e)
        {
            _gameSavedStatsData.SavedModifierGroupStatTypesBuffer = new DataBuffer
            {
                Buffer = new RedBuffer
                {
                    Data = new SavedModifierGroupStatTypesBuffer()
                }
            };
            ReloadData();
        }

        private void btn_ModifierGroupDelete_Click(object sender, EventArgs e)
        {
            if (tv_ModifierGroups.SelectedNode != null)
            {
                if (tv_ModifierGroups.SelectedNode.Tag is SavedModifierGroupStatTypesBuffer_Entry entry)
                {
                    ((SavedModifierGroupStatTypesBuffer)_gameSavedStatsData.SavedModifierGroupStatTypesBuffer.Data)!.Entries.Remove(entry);
                }

                if (tv_ModifierGroups.SelectedNode.Tag is gamedataStatType statType)
                {
                    ((SavedModifierGroupStatTypesBuffer_Entry)tv_ModifierGroups.SelectedNode.Parent.Tag).StatTypes.Remove(statType);
                }

                ReloadData();
            }
        }

        private void btn_ModifierGroupAddNode_Click(object sender, EventArgs e)
        {
            var crc = ResourceHelper.ModifierGroups.First(x => x.Name == cmb_ModifierGroupNodeType.SelectedText).CRC;
            ((SavedModifierGroupStatTypesBuffer)_gameSavedStatsData.SavedModifierGroupStatTypesBuffer.Data)!.Entries.Add(new SavedModifierGroupStatTypesBuffer_Entry { ModifierGroupHash = crc });
            ReloadData();
        }

        private void btn_ModifierGroupAddStat_Click(object sender, EventArgs e)
        {
            if (tv_ModifierGroups.SelectedNode?.Tag is SavedModifierGroupStatTypesBuffer_Entry entry)
            {
                if (Enum.TryParse<gamedataStatType>(cmb_ModifierGroupStatType.Text, out var statType))
                {
                    entry.StatTypes.Add(statType);
                    ReloadData();
                }
            }
        }
    }
}
