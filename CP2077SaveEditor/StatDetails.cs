using CyberCAT.Core.Classes.Mapping;
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
using CyberCAT.Core.Classes.DumpedClasses;

namespace CP2077SaveEditor
{
    public partial class StatDetails : Form
    {
        private Func<bool> callbackFunc;
        private Handle<GameStatModifierData> activeStat;

        public StatDetails()
        {
            InitializeComponent();
            constantStatType.GotFocus += PopulateStatTypes;
            combinedStatType.GotFocus += PopulateStatTypes;
            combinedRefStatType.GotFocus += PopulateStatTypes;
            curveStat.GotFocus += PopulateStatTypes;
            curveStatType.GotFocus += PopulateStatTypes;
        }

        private void PopulateStatTypes(object sender, EventArgs e)
        {
            var statTypes = Enum.GetNames(typeof(gamedataStatType));
            ((ComboBox)sender).Items.AddRange(statTypes);
        }

        public void LoadStat(Handle<GameStatModifierData> stat, Func<bool> callback)
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
        }

        private void applyCloseButton_Click(object sender, EventArgs e)
        {
            bool CheckInput(List<bool> results, string value = null)
            {
                if (results.Any(x => x == false))
                {
                    MessageBox.Show("Invalid enum. Must choose an option from the drop down list.");
                    return false;
                }

                if (value != null)
                {
                    if (!float.TryParse(value, out _))
                    {
                        MessageBox.Show("Value must be a valid float.");
                        return false;
                    }
                }
                return true;
            }

            if (activeStat.Value.GetType().Name == "GameCombinedStatModifierData")
            {
                if (!CheckInput(new List<bool> {
                    Enum.TryParse<gameStatModifierType>(combinedModifier.Text, out _),
                    Enum.TryParse<gameCombinedStatOperation>(combinedOperation.Text, out _),
                    Enum.TryParse<gameStatObjectsRelation>(combinedRefObject.Text, out _),
                    Enum.TryParse<gamedataStatType>(combinedRefStatType.Text, out _),
                    Enum.TryParse<gamedataStatType>(combinedStatType.Text, out _)
                }, combinedValue.Text)) { return; }

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
                if (!CheckInput(new List<bool> {
                    Enum.TryParse<gameStatModifierType>(constantModifier.Text, out _),
                    Enum.TryParse<gamedataStatType>(constantStatType.Text, out _)
                }, constantValue.Text)) { return; }

                var data = (GameConstantStatModifierData)activeStat.Value;

                data.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), constantModifier.Text);
                data.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), constantStatType.Text);

                data.Value = float.Parse(constantValue.Text);

            }
            else if (activeStat.Value.GetType().Name == "GameCurveStatModifierData")
            {
                if (!CheckInput(new List<bool> {
                    Enum.TryParse<gamedataStatType>(curveStat.Text, out _),
                    Enum.TryParse<gameStatModifierType>(curveModifier.Text, out _),
                    Enum.TryParse<gamedataStatType>(curveStatType.Text, out _)
                }, constantValue.Text)) { return; }

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
