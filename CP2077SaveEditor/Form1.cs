using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CyberCAT.Core.Classes;
using CyberCAT.Core.Classes.NodeRepresentations;
using CyberCAT.Core.Classes.Parsers;
using CyberCAT.Core.Classes.Interfaces;
using Newtonsoft.Json;

namespace CP2077SaveEditor
{
    public partial class Form1 : Form
    {
        private SaveFileHelper activeSaveFile;
        private ModernButton activeTabButton = new ModernButton();
        private Panel activeTabPanel = new Panel();
        private ListViewColumnSorter inventoryColumnSorter;
        private ListViewColumnSorter factsColumnSorter;
        private Dictionary<string, string> itemClasses;
        private ItemData activeItemEdit = null;

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1040, 627);
            this.CenterToScreen();
            editorPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);
            editorPanel.Controls.Add(appearancePanel);
            editorPanel.Controls.Add(inventoryPanel);
            editorPanel.Controls.Add(factsPanel);
            appearancePanel.Dock = DockStyle.Fill;
            inventoryPanel.Dock = DockStyle.Fill;
            factsPanel.Dock = DockStyle.Fill;
            appearancePanel.Visible = false;
            inventoryPanel.Visible = false;
            factsPanel.Visible = false;

            factsListView.AfterLabelEdit += factsListView_AfterLabelEdit;
            factsListView.MouseUp += factsListView_MouseUp;
            factsListView.ColumnClick += factsListView_ColumnClick;
            inventoryListView.DoubleClick += inventoryListView_DoubleClick;
            inventoryListView.ColumnClick += inventoryListView_ColumnClick;

            inventoryColumnSorter = new ListViewColumnSorter();
            factsColumnSorter = new ListViewColumnSorter();
            inventoryListView.ListViewItemSorter = inventoryColumnSorter;
            factsListView.ListViewItemSorter = factsColumnSorter;

            factsSearchBox.GotFocus += SearchBoxGotFocus;
            factsSearchBox.LostFocus += SearchBoxLostFocus;
            inventorySearchBox.GotFocus += SearchBoxGotFocus;
            inventorySearchBox.LostFocus += SearchBoxLostFocus;

            itemClasses = JsonConvert.DeserializeObject<Dictionary<string, string>>(CP2077SaveEditor.Properties.Resources.ItemClasses);
        }

        //This function & other functions related to managing tabs need to be refactored.
        private void SwapTab(ModernButton tabButton, Panel tabPanel)
        {
            if (tabButton == activeTabButton)
            {
                return;
            }

            activeTabButton.DefaultColor = Color.White;
            activeTabButton.HoverColor = Color.DarkGray;
            activeTabButton.TextColor = Color.Black;
            activeTabButton.ClickEffectEnabled = true;
            tabButton.DefaultColor = Color.DimGray;
            tabButton.HoverColor = Color.DimGray;
            tabButton.TextColor = Color.White;
            tabButton.ClickEffectEnabled = false;

            activeTabPanel.Visible = false;
            tabPanel.Visible = true;
            tabPanel.Enabled = true;

            activeTabButton = tabButton;
            activeTabPanel = tabPanel;
        }

        private void RefreshAppearanceValues()
        {
            var valueFields = new Dictionary<string, TextBox>
            {
                //Facial Features
                {"first.additional.second.eyes", eyesBox}, {"first.main.first.eyes_color", eyesColorBox }, {"first.additional.second.nose", noseBox}, {"first.additional.second.mouth", mouthBox}, {"first.additional.second.jaw", jawBox}, {"first.additional.second.ear", earsBox},
                //Hair
                {"first.main.hash.hair_color", hairStyleBox}, {"first.main.first.hair_color", hairColorBox},
                //Makeup
                {"first.main.first.makeupEyes_", eyeMakeupBox}, {"first.main.first.makeupLips_", lipMakeupBox}, {"first.main.first.makeupCheeks_", cheekMakeupBox},
                //Body
                {"third.main.first.body_color", skinColorBox}, {"third.main.first.nipples_", nipplesBox}, {"third.main.first.genitals_", genitalsBox}, {"third.additional.second.breast", breastsBox}
            };

            foreach(string searchString in valueFields.Keys)
            {
                valueFields[searchString].Text = activeSaveFile.GetAppearanceValue(searchString);
            }
        }

        private void IterateAppearanceSection(CharacterCustomizationAppearances.Section section, TreeNode rootNode)
        {
            foreach (CharacterCustomizationAppearances.AppearanceSection subSection in section.AppearanceSections)
            {
                var subSectionNode = rootNode.Nodes.Add(subSection.SectionName, subSection.SectionName);
                var mainlistNode = subSectionNode.Nodes.Add("Main List", "Main List");
                foreach (CharacterCustomizationAppearances.HashValueEntry entry in subSection.MainList)
                {
                    var entryNode = mainlistNode.Nodes.Add(entry.FirstString, entry.FirstString);
                    entryNode.Nodes.Add("First String: " + entry.FirstString);
                    entryNode.Nodes.Add("Hash: " + entry.Hash.ToString());
                    entryNode.Nodes.Add("Second String: " + entry.SecondString);
                }
                var additionallistNode = subSectionNode.Nodes.Add("Additional List", "Additional List");
                foreach (CharacterCustomizationAppearances.ValueEntry entry in subSection.AdditionalList)
                {
                    var entryNode = additionallistNode.Nodes.Add(entry.FirstString, entry.FirstString);
                    entryNode.Nodes.Add("First String: " + entry.FirstString);
                    entryNode.Nodes.Add("Second String: " + entry.SecondString);
                }
            }
        }

        public bool RefreshInventory(string search = "", int searchField = -1)
        {
            var listViewRows = new List<ListViewItem>();
            var containerID = containersListBox.SelectedItem.ToString();
            containerGroupBox.Visible = true;
            containerGroupBox.Text = containerID;
            if (containerID == "Player Inventory") { containerID = "1"; }

            ListViewItem selectItem = null;
            foreach (ItemData item in activeSaveFile.GetInventory(ulong.Parse(containerID)).Items)
            {
                var row = new string[] { item.ItemGameName, "Unknown", item.ItemName, "1", item.ItemGameDescription };

                if (item.ItemGameName.Length < 1)
                {
                    row[0] = "Unknown";
                }

                if (item.Data.GetType().FullName.EndsWith("SimpleItemData") == true)
                {
                    row[3] = ((ItemData.SimpleItemData)item.Data).Quantity.ToString();
                }

                var id = item.ItemTdbId.ToString();
                if (itemClasses.ContainsKey(id))
                {
                    row[1] = itemClasses[id];
                }

                if (search != "")
                {
                    if (searchField > -1)
                    {
                        if (!row[searchField].ToLower().Contains(search.ToLower()))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        var containsSearchString = false;
                        foreach (string rowItem in row)
                        {
                            if (rowItem.ToLower().Contains(search.ToLower()))
                            {
                                containsSearchString = true;
                                break;
                            }
                        }
                        if (!containsSearchString)
                        {
                            continue;
                        }
                    }
                }

                var newItem = new ListViewItem(row);
                newItem.Tag = item;

                if (activeItemEdit == item)
                {
                    newItem.Selected = true;
                    selectItem = newItem;
                }

                listViewRows.Add(newItem);
                if (item.ItemGameName == "Eddies" && containerID == "1")
                {
                    moneyUpDown.Value = ((ItemData.SimpleItemData)item.Data).Quantity;
                    moneyUpDown.Enabled = true;
                }
            }

            inventoryListView.BeginUpdate();
            inventoryListView.ListViewItemSorter = null;

            inventoryListView.Items.Clear();
            inventoryListView.Items.AddRange(listViewRows.ToArray());

            inventoryListView.ListViewItemSorter = inventoryColumnSorter;
            inventoryListView.Sort();
            inventoryListView.EndUpdate();

            if (selectItem != null)
            {
                inventoryListView.EnsureVisible(inventoryListView.Items.IndexOf(selectItem));
            }
            
            return true;
        }

        public bool RefreshInventory()
        {
            if (inventorySearchBox.Text != "" && inventorySearchBox.Text != "Search")
            {
                inventorySearchBox_TextChanged(null, new EventArgs());
            } else {
                RefreshInventory("", -1);
            }
            return true;
        }

        public bool ClearActiveItem()
        {
            activeItemEdit = null;
            return true;
        }

        public void RefreshFacts(string search = "")
        {
            var factsList = activeSaveFile.GetKnownFacts();
            var listViewRows = new List<ListViewItem>();

            foreach (FactsTable.FactEntry fact in factsList)
            {
                if (search != "")
                {
                    if (!fact.FactName.ToLower().Contains(search.ToLower()))
                    {
                        continue;
                    }
                }

                var newItem = new ListViewItem(new string[] { fact.Value.ToString(), fact.FactName });
                newItem.Tag = fact;

                listViewRows.Add(newItem);
            }

            factsListView.BeginUpdate();
            factsListView.ListViewItemSorter = null;

            factsListView.Items.Clear();
            factsListView.Items.AddRange(listViewRows.ToArray());

            factsListView.ListViewItemSorter = factsColumnSorter;
            factsListView.Sort();
            factsListView.EndUpdate();
        }

        private void openSaveButton_Click(object sender, EventArgs e)
        {
            var fileWindow = new OpenFileDialog();
            fileWindow.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\";
            fileWindow.Filter = "Cyberpunk 2077 Save File|*.dat";
            if (fileWindow.ShowDialog() == DialogResult.OK)
            {
                //Initialize NameResolver & FactResolver dictionaries, build parsers list & load save file
                NameResolver.UseDictionary(JsonConvert.DeserializeObject<Dictionary<ulong, NameResolver.NameStruct>>(CP2077SaveEditor.Properties.Resources.ItemNames));
                FactResolver.UseDictionary(JsonConvert.DeserializeObject<Dictionary<ulong, string>>(CP2077SaveEditor.Properties.Resources.Facts));

                var parsers = new List<INodeParser>();
                parsers.AddRange(new INodeParser[] {
                    new CharacterCustomizationAppearancesParser(), new InventoryParser(), new ItemDataParser(), new FactsDBParser(),
                    new FactsTableParser(), new GameSessionConfigParser(), new ItemDropStorageManagerParser(), new ItemDropStorageParser()
                });

                var newSave = new SaveFileHelper(parsers);
                newSave.LoadPCSaveFile(new MemoryStream(File.ReadAllBytes(fileWindow.FileName)));
                activeSaveFile = newSave;

                //Appearance parsing
                RefreshAppearanceValues();

                //Inventory parsing
                moneyUpDown.Enabled = false;
                moneyUpDown.Value = 0;

                containersListBox.Items.Clear();
                foreach (Inventory.SubInventory container in activeSaveFile.GetInventoriesContainer().SubInventories)
                {
                    var containerID = container.InventoryId.ToString(); if (containerID == "1") { containerID = "Player Inventory"; }
                    containersListBox.Items.Add(containerID);
                }

                if (containersListBox.Items.Count > 0)
                {
                    containersListBox.SelectedIndex = 0;
                }

                //Facts parsing
                RefreshFacts();
                //These 2 lines may look redundant, but they initialize the factsListView so that the render thread doesn't freeze when selecting the Quest Facts tab for the first time.
                //Since the render thread will be frozen here anyways while everything loads, it's best to do this here.
                factsPanel.Visible = true;
                factsPanel.Visible = false;

                //Update controls
                editorPanel.Enabled = true;
                optionsPanel.Enabled = true;
                filePathLabel.Text = Path.GetFileName(Path.GetDirectoryName(fileWindow.FileName));
                statusLabel.Text = "Save file loaded.";
                SwapTab(appearanceButton, appearancePanel);
            }
        }

        private void saveChangesButton_Click(object sender, EventArgs e)
        {
            var saveWindow = new SaveFileDialog();
            saveWindow.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\";
            saveWindow.Filter = "Cyberpunk 2077 Save File|*.dat";
            if (saveWindow.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveWindow.FileName) && !File.Exists(Path.GetDirectoryName(saveWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveWindow.FileName) + ".old"))
                {
                    File.Copy(saveWindow.FileName, Path.GetDirectoryName(saveWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveWindow.FileName) + ".old");
                }
                File.WriteAllBytes(saveWindow.FileName, activeSaveFile.SaveToPCSaveFile());
                statusLabel.Text = "File saved.";
            }
        }

        private void appearanceButton_Click(object sender, EventArgs e)
        {
            SwapTab(appearanceButton, appearancePanel);
        }

        private void inventoryButton_Click(object sender, EventArgs e)
        {
            SwapTab(inventoryButton, inventoryPanel);
        }

        private void moneyUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (moneyUpDown.Enabled)
            {
                ((ItemData.SimpleItemData)activeSaveFile.GetInventory(1).Items[Array.FindIndex(activeSaveFile.GetInventory(1).Items, x => x.ItemGameName == "Eddies")].Data).Quantity = (uint)moneyUpDown.Value;
            }
        }

        private void containersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (containersListBox.SelectedIndex > -1)
            {
                RefreshInventory();
            } else {
                containerGroupBox.Visible = false;
            }
        }

        private void saveAppearButton_Click(object sender, EventArgs e)
        {
            var saveWindow = new SaveFileDialog();
            saveWindow.Filter = "Cyberpunk 2077 Character Preset|*.preset";
            if (saveWindow.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveWindow.FileName, JsonConvert.SerializeObject(activeSaveFile.GetAppearanceContainer()));
                statusLabel.Text = "Appearance preset saved.";
            }
        }

        private void loadAppearButton_Click(object sender, EventArgs e)
        {
            var fileWindow = new OpenFileDialog();
            fileWindow.Filter = "Cyberpunk 2077 Character Preset|*.preset";
            if (fileWindow.ShowDialog() == DialogResult.OK)
            {
                var newValues = JsonConvert.DeserializeObject<CharacterCustomizationAppearances>(File.ReadAllText(fileWindow.FileName));
                activeSaveFile.SetAllAppearanceValues(newValues);
                RefreshAppearanceValues();
                statusLabel.Text = "Appearance preset loaded.";
            }
        }

        private void factsButton_Click(object sender, EventArgs e)
        {
            SwapTab(factsButton, factsPanel);
        }

        private void factsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                UInt32.Parse(e.Label);
            } catch (Exception) {
                e.CancelEdit = true;
                return;
            }
            ((FactsTable.FactEntry)factsListView.SelectedItems[0].Tag).Value = UInt32.Parse(e.Label);
        }

        private void inventoryListView_DoubleClick(object sender, EventArgs e)
        {
            if (inventoryListView.SelectedIndices.Count > 0)
            {
                activeItemEdit = (ItemData)inventoryListView.SelectedItems[0].Tag;
                var activeDetails = new ItemDetails();
                activeDetails.LoadItem((ItemData)inventoryListView.SelectedItems[0].Tag, RefreshInventory, ClearActiveItem);
            }
        }

        private void inventoryListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == inventoryColumnSorter.SortColumn)
            {
                if (inventoryColumnSorter.Order == SortOrder.Ascending)
                {
                    inventoryColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    inventoryColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                inventoryColumnSorter.SortColumn = e.Column;
                inventoryColumnSorter.Order = SortOrder.Ascending;
            }

            inventoryListView.Sort();
        }

        private void factsListView_MouseUp(object sender, EventArgs e)
        {
            if (factsListView.SelectedItems.Count > 0)
            {
                factsListView.SelectedItems[0].BeginEdit();
            }
        }

        private void factsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == factsColumnSorter.SortColumn)
            {
                if (factsColumnSorter.Order == SortOrder.Ascending)
                {
                    factsColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    factsColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                factsColumnSorter.SortColumn = e.Column;
                factsColumnSorter.Order = SortOrder.Ascending;
            }

            factsListView.Sort();
        }

        private void factsSearchBox_TextChanged(object sender, EventArgs e)
        {
            if (factsSearchBox.ForeColor != Color.Silver)
            {
                RefreshFacts(factsSearchBox.Text);
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

        private void inventorySearchBox_TextChanged(object sender, EventArgs e)
        {
            if (inventorySearchBox.ForeColor != Color.Silver)
            {
                var query = inventorySearchBox.Text;
                if (query.StartsWith("name:"))
                {
                    query = query.Remove(0, 5);
                    RefreshInventory(query, 0);
                }
                else if (query.StartsWith("type:"))
                {
                    query = query.Remove(0, 5);
                    RefreshInventory(query, 1);
                }
                else if (query.StartsWith("id:"))
                {
                    query = query.Remove(0, 3);
                    RefreshInventory(query, 2);
                }
                else if (query.StartsWith("quantity:"))
                {
                    query = query.Remove(0, 9);
                    RefreshInventory(query, 3);
                }
                else if (query.StartsWith("desc:"))
                {
                    query = query.Remove(0, 5);
                    RefreshInventory(query, 4);
                }
                else
                {
                    RefreshInventory(query, -1);
                }
            }
            
        }
    }
}
