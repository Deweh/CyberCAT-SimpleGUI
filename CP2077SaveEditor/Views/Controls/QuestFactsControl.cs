using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CP2077SaveEditor.Extensions;
using CP2077SaveEditor.Utils;
using Newtonsoft.Json;
using WolvenKit.RED4.Save;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class QuestFactsControl : UserControl, IGameControl
    {
        private readonly Form2 _parentForm;

        public QuestFactsControl(Form2 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;
        }

        public string GameControlName => "Quest Facts";

        private void OnParentFormPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_parentForm.ActiveSaveFile))
            {
                if (_parentForm.ActiveSaveFile != null)
                {
                    this.InvokeIfRequired(RefreshFacts);
                }
            }
        }

        private void factsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (!uint.TryParse(e.Label, out var value))
            {
                e.CancelEdit = true;
            }

            ((FactsTable.FactEntry)factsListView.SelectedVirtualItems()[0].Tag).Value = value;
        }

        private void factsListView_MouseUp(object sender, EventArgs e)
        {
            if (factsListView.SelectedIndices.Count > 0)
            {
                factsListView.SelectedVirtualItems()[0].BeginEdit();
            }
        }

        private void factsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (factsListView.SelectedIndices.Count > 0 && e.KeyCode == Keys.Delete)
            {
                ((FactsTable)_parentForm.ActiveSaveFile.GetFactsContainer().Children[0].Value).FactEntries.Remove((FactsTable.FactEntry)factsListView.SelectedVirtualItems()[0].Tag);
                _parentForm.SetStatus("'" + factsListView.SelectedVirtualItems()[0].SubItems[1].Text + "' deleted.");
                factsListView.RemoveVirtualItem(factsListView.SelectedVirtualItems()[0]);
            }
        }

        private void SearchBoxGotFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "Search" && ((TextBox)sender).ForeColor == Color.Silver)
            {
                ((TextBox)sender).Text = "";
                ((TextBox)sender).ForeColor = Color.Black;
            }
        }

        private void SearchBoxLostFocus(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).ForeColor = Color.Silver;
                ((TextBox)sender).Text = "Search";
            }
        }

        private void RefreshFacts()
        {
            RefreshFacts("");
        }

        private void RefreshFacts(string search)
        {
            var factsList = _parentForm.ActiveSaveFile.GetKnownFacts();
            var listViewRows = new List<ListViewItem>();

            foreach (FactsTable.FactEntry fact in factsList)
            {
                if (search != "")
                {
                    if (!_parentForm.ActiveSaveFile.KnownFacts[fact.FactName].ToLower().Contains(search.ToLower()))
                    {
                        continue;
                    }
                }

                var newItem = new ListViewItem(new string[] { fact.Value.ToString(), _parentForm.ActiveSaveFile.KnownFacts[fact.FactName] });
                newItem.Tag = fact;

                listViewRows.Add(newItem);
            }

            factsListView.SetVirtualItems(listViewRows);
        }

        private void factsSearchBox_TextChanged(object sender, EventArgs e)
        {
            if (factsSearchBox.ForeColor != Color.Silver)
            {
                RefreshFacts(factsSearchBox.Text);
            }
        }

        private void addFactButton_Click(object sender, EventArgs e)
        {
            var factDialog = new AddFact();
            factDialog.LoadFactDialog(RefreshFacts, _parentForm.ActiveSaveFile);
        }

        private void factsSaveButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Note: This option is not meant to overwrite your save file. If you are only trying to save changes to your quest facts, you should press 'Save Changes' instead.");

            var saveWindow = new SaveFileDialog();
            saveWindow.Filter = "Text File|*.txt";
            if (saveWindow.ShowDialog() == DialogResult.OK)
            {
                var facts = _parentForm.ActiveSaveFile.GetKnownFacts();
                var final = new List<KeyValuePair<string, uint>>(facts.Count);

                foreach (var singleFact in facts)
                {
                    final.Add(new KeyValuePair<string, uint>(_parentForm.ActiveSaveFile.KnownFacts[singleFact.FactName], singleFact.Value));
                }

                File.WriteAllText(saveWindow.FileName, JsonConvert.SerializeObject(final, Formatting.Indented));
            }
        }

        private void enableSecretEndingButton_Click(object sender, EventArgs e)
        {
            _parentForm.ActiveSaveFile.SetFactByName("sq032_johnny_friend", 1);
            RefreshFacts();
            MessageBox.Show("Secret ending enabled.");
        }

        private void makeAllRomanceableButton_Click(object sender, EventArgs e)
        {
            _parentForm.ActiveSaveFile.SetFactByName("judy_romanceable", 1);
            _parentForm.ActiveSaveFile.SetFactByName("river_romanceable", 1);
            _parentForm.ActiveSaveFile.SetFactByName("panam_romanceable", 1);
            _parentForm.ActiveSaveFile.SetFactByName("kerry_romanceable", 1);
            RefreshFacts();
            MessageBox.Show("All characters are now romanceable.");
        }
    }
}
