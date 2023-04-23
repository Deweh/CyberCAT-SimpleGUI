using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor
{
    public partial class StatDetails : Form
    {
        private Func<bool> callbackFunc;
        private gameStatModifierData_Deprecated activeStat;

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

        public void LoadStat(gameStatModifierData_Deprecated stat, Func<bool> callback)
        {
            callbackFunc = callback;
            activeStat = stat;

            if (stat is gameCombinedStatModifierData_Deprecated comStat)
            {
                statTabControl.TabPages.Remove(constantStatTab);
                statTabControl.TabPages.Remove(curveStatTab);

                combinedModifier.Text = comStat.ModifierType.ToString();
                combinedOperation.Text = comStat.Operation.ToString();
                combinedRefObject.Text = comStat.RefObject.ToString();
                combinedRefStatType.Text = comStat.RefStatType.ToString();
                combinedStatType.Text = comStat.StatType.ToString();
                combinedValue.Text = comStat.Value.ToString();                
            }
            else if (stat is gameConstantStatModifierData_Deprecated constantStat)
            {
                statTabControl.TabPages.Remove(combinedStatTab);
                statTabControl.TabPages.Remove(curveStatTab);

                constantModifier.Text = constantStat.ModifierType.ToString();
                constantStatType.Text = constantStat.StatType.ToString();
                constantValue.Text = constantStat.Value.ToString();
            }
            else if (stat is gameCurveStatModifierData_Deprecated curvStat)
            {
                statTabControl.TabPages.Remove(constantStatTab);
                statTabControl.TabPages.Remove(combinedStatTab);

                curveColumnName.Text = curvStat.ColumnName.ToString();
                curveName.Text = curvStat.CurveName.ToString();
                curveStat.Text = curvStat.CurveStat.ToString();
                curveModifier.Text = curvStat.ModifierType.ToString();
                curveStatType.Text = curvStat.StatType.ToString();
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

            if (activeStat is gameCombinedStatModifierData_Deprecated comStat)
            {
                if (!CheckInput(new List<bool> {
                    Enum.TryParse<gameStatModifierType>(combinedModifier.Text, out _),
                    Enum.TryParse<gameCombinedStatOperation>(combinedOperation.Text, out _),
                    Enum.TryParse<gameStatObjectsRelation>(combinedRefObject.Text, out _),
                    Enum.TryParse<gamedataStatType>(combinedRefStatType.Text, out _),
                    Enum.TryParse<gamedataStatType>(combinedStatType.Text, out _)
                }, combinedValue.Text)) { return; }

                comStat.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), combinedModifier.Text);
                comStat.Operation = (gameCombinedStatOperation)Enum.Parse(typeof(gameCombinedStatOperation), combinedOperation.Text);
                comStat.RefObject = (gameStatObjectsRelation)Enum.Parse(typeof(gameStatObjectsRelation), combinedRefObject.Text);
                comStat.RefStatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), combinedRefStatType.Text);
                comStat.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), combinedStatType.Text);

                comStat.Value = float.Parse(combinedValue.Text);

            }
            else if (activeStat is gameConstantStatModifierData_Deprecated constantStat)
            {
                if (!CheckInput(new List<bool> {
                    Enum.TryParse<gameStatModifierType>(constantModifier.Text, out _),
                    Enum.TryParse<gamedataStatType>(constantStatType.Text, out _)
                }, constantValue.Text)) { return; }

                constantStat.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), constantModifier.Text);
                constantStat.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), constantStatType.Text);

                constantStat.Value = float.Parse(constantValue.Text);

            }
            else if (activeStat is gameCurveStatModifierData_Deprecated curvStat)
            {
                if (!CheckInput(new List<bool> {
                    Enum.TryParse<gamedataStatType>(curveStat.Text, out _),
                    Enum.TryParse<gameStatModifierType>(curveModifier.Text, out _),
                    Enum.TryParse<gamedataStatType>(curveStatType.Text, out _)
                })) { return; }

                curvStat.ColumnName = curveColumnName.Text;
                curvStat.CurveName = curveName.Text;

                curvStat.CurveStat = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), curveStat.Text);
                curvStat.ModifierType = (gameStatModifierType)Enum.Parse(typeof(gameStatModifierType), curveModifier.Text);
                curvStat.StatType = (gamedataStatType)Enum.Parse(typeof(gamedataStatType), curveStatType.Text);
            }

            callbackFunc.Invoke();
            this.Close();
        }
    }
}
