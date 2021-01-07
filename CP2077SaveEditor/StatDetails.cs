using CyberCAT.Core.Classes.Mapping.StatsSystem;
using CyberCAT.Core.DumpedEnums;
using CyberCAT.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2077SaveEditor
{
    public partial class StatDetails : Form
    {
        private Func<bool> callbackFunc;
        private CyberCAT.Core.Classes.Mapping.Global.Handle<GameStatModifierData> activeStat;

        public StatDetails()
        {
            InitializeComponent();
        }

        public void LoadStat(CyberCAT.Core.Classes.Mapping.Global.Handle<GameStatModifierData> stat, Func<bool> callback)
        {
            callbackFunc = callback;
            activeStat = stat;

            if (stat.Value.GetType().Name == "GameCombinedStatModifierData")
            {
                statTabControl.TabPages.Remove(constantStatTab);
                statTabControl.TabPages.Remove(curveStatTab);
                var data = ((GameCombinedStatModifierData)stat.Value);

                combinedModifier.Text = data.ModifierType.ToString();
                combinedOperation.Text = data.Operation.ToString();
                combinedRefObject.Text = data.RefObject.ToString();
                combinedRefStatType.Text = data.RefStatType.ToString();
                combinedStatType.Text = data.StatType.ToString();
                combinedValue.Text = data.Value.ToString();
            }
            else if (stat.Value.GetType().Name == "GameConstantStatModifierData")
            {
                statTabControl.TabPages.Remove(combinedStatTab);
                statTabControl.TabPages.Remove(curveStatTab);
                var data = ((GameConstantStatModifierData)stat.Value);

                constantModifier.Text = data.ModifierType.ToString();
                constantStatType.Text = data.StatType.ToString();
                constantValue.Text = data.Value.ToString();
            }
            else if (stat.Value.GetType().Name == "GameCurveStatModifierData")
            {
                statTabControl.TabPages.Remove(constantStatTab);
                statTabControl.TabPages.Remove(combinedStatTab);
                var data = ((GameCurveStatModifierData)stat.Value);

                curveColumnName.Text = data.ColumnName.ToString();
                curveName.Text = data.CurveName.ToString();
                curveStat.Text = data.CurveStat.ToString();
                curveModifier.Text = data.ModifierType.ToString();
                curveStatType.Text = data.StatType.ToString();
            }

            this.ShowDialog();
            this.Refresh();
        }

        private void applyCloseButton_Click(object sender, EventArgs e)
        {
            if (activeStat.Value.GetType().Name == "GameCombinedStatModifierData")
            {
                var data = (GameCombinedStatModifierData)activeStat.Value;
                try
                {
                    data.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), combinedModifier.Text);
                    data.Operation = (gameCombinedStatOperation)Enum.Parse(typeof(gameCombinedStatOperation), combinedOperation.Text);
                    data.RefObject = (gameStatObjectsRelation)Enum.Parse(typeof(gameStatObjectsRelation), combinedRefObject.Text);
                    data.RefStatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), combinedRefStatType.Text);
                    data.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), combinedStatType.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid enum.");
                    return;
                }

                try
                {
                    data.Value = float.Parse(combinedValue.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Value must be a valid float.");
                    return;
                }     
            }
            else if (activeStat.Value.GetType().Name == "GameConstantStatModifierData")
            {
                var data = (GameConstantStatModifierData)activeStat.Value;
                try
                {
                    data.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), constantModifier.Text);
                    data.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), constantStatType.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid enum.");
                    return;
                }

                try
                {
                    data.Value = float.Parse(constantValue.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Value must be a valid float.");
                    return;
                }
            }
            else if (activeStat.Value.GetType().Name == "GameCurveStatModifierData")
            {
                var data = (GameCurveStatModifierData)activeStat.Value;

                data.ColumnName = curveColumnName.Text;
                data.CurveName = curveName.Text;

                try
                {
                    data.CurveStat = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), curveStat.Text);
                    data.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), curveModifier.Text);
                    data.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), curveStatType.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid enum.");
                    return;
                }
            }

            callbackFunc.Invoke();
            this.Close();
        }
    }
}
