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

                combinedModifier.Text = ((GameCombinedStatModifierData)stat.Value).ModifierType.ToString();
                combinedOperation.Text = ((GameCombinedStatModifierData)stat.Value).Operation.ToString();
                combinedRefObject.Text = ((GameCombinedStatModifierData)stat.Value).RefObject.ToString();
                combinedRefStatType.Text = ((GameCombinedStatModifierData)stat.Value).RefStatType.ToString();
                combinedStatType.Text = ((GameCombinedStatModifierData)stat.Value).StatType.ToString();
                combinedValue.Text = ((GameCombinedStatModifierData)stat.Value).Value.ToString();
            }
            else if (stat.Value.GetType().Name == "GameConstantStatModifierData")
            {
                statTabControl.TabPages.Remove(combinedStatTab);
                statTabControl.TabPages.Remove(curveStatTab);

                constantModifier.Text = ((GameConstantStatModifierData)stat.Value).ModifierType.ToString();
                constantStatType.Text = ((GameConstantStatModifierData)stat.Value).StatType.ToString();
                constantValue.Text = ((GameConstantStatModifierData)stat.Value).Value.ToString();
            }
            else if (stat.Value.GetType().Name == "GameCurveStatModifierData")
            {
                statTabControl.TabPages.Remove(constantStatTab);
                statTabControl.TabPages.Remove(combinedStatTab);

                curveColumnName.Text = ((GameCurveStatModifierData)stat.Value).ColumnName.ToString();
                curveName.Text = ((GameCurveStatModifierData)stat.Value).CurveName.ToString();
                curveStat.Text = ((GameCurveStatModifierData)stat.Value).CurveStat.ToString();
                curveModifier.Text = ((GameCurveStatModifierData)stat.Value).ModifierType.ToString();
                curveStatType.Text = ((GameCurveStatModifierData)stat.Value).StatType.ToString();
            }

            this.ShowDialog();
            this.Refresh();
        }

        private void applyCloseButton_Click(object sender, EventArgs e)
        {
            if (activeStat.Value.GetType().Name == "GameCombinedStatModifierData")
            {
                var data = (GameCombinedStatModifierData)activeStat.Value;

                data.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), combinedModifier.Text);
                data.Operation = (gameCombinedStatOperation)Enum.Parse(typeof(gameCombinedStatOperation), combinedOperation.Text);
                data.RefObject = (gameStatObjectsRelation)Enum.Parse(typeof(gameStatObjectsRelation), combinedRefObject.Text);
                data.RefStatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), combinedRefStatType.Text);
                data.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), combinedStatType.Text);
                data.Value = float.Parse(combinedValue.Text);
            }
            else if (activeStat.Value.GetType().Name == "GameConstantStatModifierData")
            {
                var data = (GameConstantStatModifierData)activeStat.Value;

                data.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), constantModifier.Text);
                data.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), constantStatType.Text);
                data.Value = float.Parse(constantValue.Text);
            }
            else if (activeStat.Value.GetType().Name == "GameCurveStatModifierData")
            {
                var data = (GameCurveStatModifierData)activeStat.Value;

                data.ColumnName = curveColumnName.Text;
                data.CurveName = curveName.Text;
                data.CurveStat = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), curveStat.Text);
                data.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), curveModifier.Text);
                data.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), curveStatType.Text);
            }

            callbackFunc.Invoke();
            this.Close();
        }
    }
}
