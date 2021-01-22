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
using CyberCAT.Core.DumpedEnums;
using Newtonsoft.Json;
using CP2077SaveEditor.Extensions;
using CyberCAT.Core.Classes.Mapping;

namespace CP2077SaveEditor
{
    public partial class Form1 : Form
    {
        //Save File Object
        private SaveFileHelper activeSaveFile = new SaveFileHelper(new INodeParser[] {});

        //Save Info
        private bool loadingSave = false;
        private int saveType = 0;

        //GUI
        private ModernButton activeTabButton = new ModernButton();
        private Panel activeTabPanel = new Panel();
        private string debloatInfo = "";

        //Lookup Dictionaries
        private Dictionary<string, CharacterCustomizationAppearances> appearanceCompareNodes;
        private Dictionary<Enum, NumericUpDown> attrFields, proficFields;
        private Dictionary<string, NumericUpDown> linearAppearanceFeatures;
        private static readonly Dictionary<string, string> itemClasses = JsonConvert.DeserializeObject<Dictionary<string, string>>(CP2077SaveEditor.Properties.Resources.ItemClasses);
        private static readonly Dictionary<ulong, string> inventoryNames = new()
        {
            { 0x1, "V's Inventory" },
            { 0xF4240, "Car Stash" },
            { 0x895724, "Nomad Intro Items" },
            { 0x895956, "Street Kid Intro Items" },
            { 0x8959E8, "Corpo Intro Items" },
            { 0x38E8D0C9F9A087AE, "Panam's Stash" },
            { 0x6E48C594562422DE, "Judy's Stash" },
            { 0x7901DE03D136A5AF, "V's Wardrobe" },
            { 0xE5F556FCBB62A706, "V's Stash" },
            { 0xEDAD8C9B086A615E, "River's Stash" }
        };

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1040, 627);
            this.CenterToScreen();
            editorPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);

            var tabPanels = new Panel[] { appearancePanel, inventoryPanel, factsPanel, statsPanel };
            foreach (Panel singleTab in tabPanels)
            {
                editorPanel.Controls.Add(singleTab);
                singleTab.Dock = DockStyle.Fill;
                singleTab.Visible = false;
            }

            factsListView.AfterLabelEdit += factsListView_AfterLabelEdit;
            factsListView.MouseUp += factsListView_MouseUp;
            factsListView.KeyDown += factsListView_KeyDown;
            inventoryListView.DoubleClick += inventoryListView_DoubleClick;
            inventoryListView.KeyDown += inventoryListView_KeyDown;

            factsSearchBox.GotFocus += SearchBoxGotFocus;
            factsSearchBox.LostFocus += SearchBoxLostFocus;
            inventorySearchBox.GotFocus += SearchBoxGotFocus;
            inventorySearchBox.LostFocus += SearchBoxLostFocus;

            levelUpDown.ValueChanged += PlayerStatChanged;
            streetCredUpDown.ValueChanged += PlayerStatChanged;

            bodyUpDown.ValueChanged += PlayerStatChanged;
            reflexesUpDown.ValueChanged += PlayerStatChanged;
            technicalAbilityUpDown.ValueChanged += PlayerStatChanged;
            intelligenceUpDown.ValueChanged += PlayerStatChanged;
            coolUpDown.ValueChanged += PlayerStatChanged;
            perkPointsUpDown.ValueChanged += PlayerStatChanged;
            attrPointsUpDown.ValueChanged += PlayerStatChanged;

            athleticsUpDown.ValueChanged += PlayerStatChanged;
            annihilationUpDown.ValueChanged += PlayerStatChanged;
            streetBrawlerUpDown.ValueChanged += PlayerStatChanged;
            assaultUpDown.ValueChanged += PlayerStatChanged;
            handgunsUpDown.ValueChanged += PlayerStatChanged;
            bladesUpDown.ValueChanged += PlayerStatChanged;
            craftingUpDown.ValueChanged += PlayerStatChanged;
            engineeringUpDown.ValueChanged += PlayerStatChanged;
            breachProtocolUpDown.ValueChanged += PlayerStatChanged;
            quickhackingUpDown.ValueChanged += PlayerStatChanged;
            stealthUpDown.ValueChanged += PlayerStatChanged;
            coldBloodUpDown.ValueChanged += PlayerStatChanged;

            debloatWorker.RunWorkerCompleted += debloatWorker_Completed;

            statsPanel.BackgroundImage = CP2077SaveEditor.Properties.Resources.player_stats;

            attrFields = new Dictionary<Enum, NumericUpDown>
            {
                {gamedataStatType.Strength, bodyUpDown},
                {gamedataStatType.Reflexes, reflexesUpDown},
                {gamedataStatType.TechnicalAbility, technicalAbilityUpDown},
                {gamedataStatType.Intelligence, intelligenceUpDown},
                {gamedataStatType.Cool, coolUpDown}
            };

            proficFields = new Dictionary<Enum, NumericUpDown> {
                {gamedataProficiencyType.Level, levelUpDown},
                {gamedataProficiencyType.StreetCred, streetCredUpDown},
                {gamedataProficiencyType.Athletics, athleticsUpDown},
                {gamedataProficiencyType.Demolition, annihilationUpDown},
                {gamedataProficiencyType.Brawling, streetBrawlerUpDown},
                {gamedataProficiencyType.Invalid, assaultUpDown},
                {gamedataProficiencyType.Gunslinger, handgunsUpDown},
                {gamedataProficiencyType.Kenjutsu, bladesUpDown},
                {gamedataProficiencyType.Crafting, craftingUpDown},
                {gamedataProficiencyType.Engineering, engineeringUpDown},
                {gamedataProficiencyType.Hacking, breachProtocolUpDown},
                {gamedataProficiencyType.CombatHacking, quickhackingUpDown},
                {gamedataProficiencyType.Stealth, stealthUpDown},
                {gamedataProficiencyType.ColdBlood, coldBloodUpDown}
            };

            linearAppearanceFeatures = new Dictionary<string, NumericUpDown>
            {
                {"first.additional.second.eyes", eyesUpDown},
                {"first.additional.second.nose", noseUpDown},
                {"first.additional.second.mouth", mouthUpDown},
                {"first.additional.second.jaw", jawUpDown},
                {"first.additional.second.ear", earsUpDown}
            };

            foreach (NumericUpDown control in linearAppearanceFeatures.Values)
            {
                control.ValueChanged += LinearAppearanceValueChanged;
            }

            hairStyleBox.Items.AddRange(activeSaveFile.Appearance.HairStyles.Keys.ToArray());
            hairColorBox.Items.AddRange(activeSaveFile.Appearance.HairColors.ToArray());
            skinColorBox.Items.AddRange(activeSaveFile.Appearance.SkinColors.ToArray());
            eyesColorBox.Items.AddRange(activeSaveFile.Appearance.EyeColors.ToArray());

#if DEBUG
            appearanceCompareValuesBox.Visible = true;
#endif
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
            //Non-Editable Values

            var valueFields = new Dictionary<string, TextBox>
            {
                //Makeup
                {"first.main.first.makeupEyes_", eyeMakeupBox}, {"first.main.first.makeupLips_", lipMakeupBox}, {"first.main.first.makeupCheeks_", cheekMakeupBox},
                //Body
                {"third.main.first.nipples_", nipplesBox}, {"third.main.first.genitals_", genitalsBox}, {"third.additional.second.breast", breastsBox}
            };

            foreach(string searchString in valueFields.Keys)
            {
                valueFields[searchString].Text = activeSaveFile.GetAppearanceValue(searchString);
            }

            //Facial Features

            foreach (string searchString in linearAppearanceFeatures.Keys)
            {
                var result = activeSaveFile.GetAppearanceValue(searchString);

                if (result == "default")
                {
                    linearAppearanceFeatures[searchString].Value = 1;
                }
                else
                {
                    result = result.Substring(1, 2);
                    linearAppearanceFeatures[searchString].Value = int.Parse(result) + 1;
                }
            }

            //Hair Style & Color

            var hairColorEntry = activeSaveFile.GetAppearanceValue("first.main.first.hair_color");

            if (hairColorEntry != "default")
            {
                hairColorBox.Text = hairColorEntry;
                hairColorBox.Enabled = true;

                var hairHash = ulong.Parse(activeSaveFile.GetAppearanceValue("first.main.hash.hair_color"));
                foreach (string styleName in activeSaveFile.Appearance.HairStyles.Keys)
                {
                    hairColorBox.Text = activeSaveFile.GetAppearanceValue("first.main.first.hair_color");

                    if (activeSaveFile.Appearance.HairStyles[styleName] == hairHash)
                    {
                        hairStyleBox.Text = styleName;
                    }
                }
            }
            else
            {
                hairStyleBox.Text = "Shaved";
                hairColorBox.Items.Add("None");
                hairColorBox.Text = "None";
                hairColorBox.Enabled = false;
            }

            //Concated Colors

            skinColorBox.Text = activeSaveFile.GetAppearanceValue("third.main.first.body_color").Split("__", StringSplitOptions.None).Last();

            var eyeColor = activeSaveFile.GetAppearanceValue("first.main.first.eyes_color").Split("__", StringSplitOptions.None).Last();
            if (!eyesColorBox.Items.Contains(eyeColor))
            {
                eyesColorBox.Items.Add(eyeColor);
            }
            eyesColorBox.Text = eyeColor;
        }

        private void LinearAppearanceValueChanged(object sender, EventArgs e)
        {
            var i = 1;
            foreach (string searchString in linearAppearanceFeatures.Keys)
            {
                if (linearAppearanceFeatures[searchString] == sender)
                {
                    var stringParts = searchString.Split('.');
                    activeSaveFile.SetLinearAppearanceValue(searchString.Split('.')[3], i, (int)((NumericUpDown)sender).Value);
                }
                i++;
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

            if (inventoryNames.Values.Contains(containerID))
            {
                containerID = inventoryNames.Where(x => x.Value == containerID).FirstOrDefault().Key.ToString();
            }

            foreach (ItemData item in activeSaveFile.GetInventory(ulong.Parse(containerID)).Items)
            {
                var row = new string[] { (item.ItemGameName.Length > 1) ? item.ItemGameName : "Unknown", "", item.ItemName, "", "1", item.ItemGameDescription };

                if (item.Data.GetType() == typeof(ItemData.SimpleItemData))
                {
                    row[4] = ((ItemData.SimpleItemData)item.Data).Quantity.ToString();
                    row[1] = "[S] ";
                }
                else if(item.Data.GetType() == typeof(ItemData.ModableItemWithQuantityData))
                {
                    row[4] = ((ItemData.ModableItemWithQuantityData)item.Data).Quantity.ToString();
                    row[1] = "[M+] ";
                } else {
                    row[1] = "[M] ";
                }

                var id = item.ItemTdbId.ToString();
                if (itemClasses.ContainsKey(id))
                {
                    row[1] += itemClasses[id];
                } else {
                    row[1] += "Unknown";
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

                listViewRows.Add(newItem);
                if (item.ItemGameName == "Eddies" && containerID == "1")
                {
                    moneyUpDown.Value = ((ItemData.SimpleItemData)item.Data).Quantity;
                    moneyUpDown.Enabled = true;
                }
            }

            if (containerID == "1")
            {
                foreach (KeyValuePair<CyberCAT.Core.Classes.DumpedClasses.GameItemID, string> equipInfo in activeSaveFile.GetEquippedItems().Reverse())
                {
                    var equippedItem = listViewRows.Where(x => ((ItemData)x.Tag).ItemTdbId.Id == equipInfo.Key.Id.Id).FirstOrDefault();

                    if (equippedItem != null)
                    {
                        equippedItem.SubItems[3].Text = equipInfo.Value;
                        equippedItem.BackColor = Color.FromArgb(248, 248, 248);
                        listViewRows.Remove(equippedItem);
                        listViewRows.Insert(0, equippedItem);
                    }
                }
            }

            inventoryListView.SetVirtualItems(listViewRows);
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

            factsListView.SetVirtualItems(listViewRows);
        }

        private void openSaveButton_Click(object sender, EventArgs e)
        {
            var fileWindow = new OpenFileDialog();
            fileWindow.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\";
            fileWindow.Filter = "Cyberpunk 2077 Save File|*.dat";
            if (fileWindow.ShowDialog() == DialogResult.OK)
            {
                loadingSave = true;
                statusLabel.Text = "Loading save...";
                this.Refresh();
                //Initialize NameResolver & FactResolver dictionaries, build parsers list & load save file
                NameResolver.UseDictionary(JsonConvert.DeserializeObject<Dictionary<ulong, NameResolver.NameStruct>>(CP2077SaveEditor.Properties.Resources.ItemNames));
                FactResolver.UseDictionary(JsonConvert.DeserializeObject<Dictionary<ulong, string>>(CP2077SaveEditor.Properties.Resources.Facts));

                var parsers = new List<INodeParser>();
                parsers.AddRange(new INodeParser[] {
                    new CharacterCustomizationAppearancesParser(), new InventoryParser(), new ItemDataParser(), new FactsDBParser(),
                    new FactsTableParser(), new GameSessionConfigParser(), new ItemDropStorageManagerParser(), new ItemDropStorageParser(),
                    new StatsSystemParser(), new ScriptableSystemsContainerParser()
                });

                var newSave = new SaveFileHelper(parsers);
                if (saveType == 0)
                {
                    newSave.LoadPCSaveFile(new MemoryStream(File.ReadAllBytes(fileWindow.FileName)));
                } else {
                    newSave.LoadPS4SaveFile(new MemoryStream(File.ReadAllBytes(fileWindow.FileName)));
                }
                
                activeSaveFile = newSave;

                //Appearance parsing
                RefreshAppearanceValues();

                //Inventory parsing
                moneyUpDown.Enabled = false;
                moneyUpDown.Value = 0;

                containersListBox.Items.Clear();
                foreach (Inventory.SubInventory container in activeSaveFile.GetInventoriesContainer().SubInventories)
                {
                    var containerID = container.InventoryId.ToString();
                    if (inventoryNames.Keys.Contains(container.InventoryId))
                    {
                        containerID = inventoryNames[container.InventoryId];
                    }
                    containersListBox.Items.Add(containerID);
                }

                if (containersListBox.Items.Count > 0)
                {
                    containersListBox.SelectedIndex = 0;
                }

                //Facts parsing
                RefreshFacts();

                //Player stats parsing
                var playerData = activeSaveFile.GetPlayerDevelopmentData();
                foreach (gamedataProficiencyType proficType in proficFields.Keys)
                {
                    CyberCAT.Core.Classes.DumpedClasses.SProficiency profic;
                    if (proficType == gamedataProficiencyType.Invalid)
                    {
                        profic = playerData.Value.Proficiencies[Array.FindIndex(playerData.Value.Proficiencies, x => x.Type == null)];
                    } else {
                        profic = playerData.Value.Proficiencies[Array.FindIndex(playerData.Value.Proficiencies, x => x.Type == proficType)];
                    }
                    if (profic.CurrentLevel > (int)proficFields[proficType].Maximum)
                    {
                        proficFields[proficType].Maximum = profic.CurrentLevel;
                    }
                    
                    proficFields[proficType].Value = profic.CurrentLevel;
                }

                foreach (gamedataStatType attrType in attrFields.Keys)
                {
                    CyberCAT.Core.Classes.DumpedClasses.SAttribute attr = playerData.Value.Attributes[Array.FindIndex(playerData.Value.Attributes, x => x.AttributeName == attrType)];
                    if (attr.Value > (int)attrFields[attrType].Maximum)
                    {
                        attrFields[attrType].Maximum = attr.Value;
                    }

                    attrFields[attrType].Value = attr.Value;
                }

                switch (activeSaveFile.GetPlayerDevelopmentData().Value.LifePath)
                {
                    case gamedataLifePath.Nomad:
                        lifePathBox.SelectedIndex = 0;
                        break;
                    case gamedataLifePath.StreetKid:
                        lifePathBox.SelectedIndex = 1;
                        break;
                    default:
                        lifePathBox.SelectedIndex = 2;
                        break;
                }

                var points = activeSaveFile.GetPlayerDevelopmentData().Value.DevPoints;
                perkPointsUpDown.Value = points[Array.FindIndex(points, x => x.Type == gamedataDevelopmentPointType.Primary)].Unspent;
                attrPointsUpDown.Value = points[Array.FindIndex(points, x => x.Type == null)].Unspent;

                //Update controls
                loadingSave = false;
                editorPanel.Enabled = true;
                optionsPanel.Enabled = true;
                filePathLabel.Text = Path.GetFileName(Path.GetDirectoryName(fileWindow.FileName));
                statusLabel.Text = "Save file loaded.";
                SwapTab(statsButton, statsPanel);
            }
        }

        private void saveChangesButton_Click(object sender, EventArgs e)
        {
            var saveWindow = new SaveFileDialog();
            saveWindow.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\";
            saveWindow.Filter = "Cyberpunk 2077 Save File|*.dat";
            if (saveWindow.ShowDialog() == DialogResult.OK)
            {
                statusLabel.Text = "Saving changes...";
                this.Refresh();
                if (File.Exists(saveWindow.FileName) && !File.Exists(Path.GetDirectoryName(saveWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveWindow.FileName) + ".old"))
                {
                    File.Copy(saveWindow.FileName, Path.GetDirectoryName(saveWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveWindow.FileName) + ".old");
                }
                byte[] saveBytes;
                if (saveType == 0)
                {
                    saveBytes = activeSaveFile.SaveToPCSaveFile();
                } else {
                    saveBytes = activeSaveFile.SaveToPS4SaveFile();
                }
                

                var parsers = new List<INodeParser>();
                parsers.AddRange(new INodeParser[] {
                    new CharacterCustomizationAppearancesParser(), new InventoryParser(), new ItemDataParser(), new FactsDBParser(),
                    new FactsTableParser(), new GameSessionConfigParser(), new ItemDropStorageManagerParser(), new ItemDropStorageParser(),
                    new StatsSystemParser(), new ScriptableSystemsContainerParser()
                });

                var testFile = new SaveFileHelper(parsers);
                try
                {
                    if (saveType == 0)
                    {
                        testFile.LoadPCSaveFile(new MemoryStream(saveBytes));
                    }
                    else
                    {
                        testFile.LoadPS4SaveFile(new MemoryStream(saveBytes));
                    }
                }
                catch (Exception ex)
                {
                    statusLabel.Text = "Save cancelled.";
                    File.WriteAllText("error.txt", ex.Message + '\n' + ex.TargetSite + '\n' + ex.StackTrace);
                    MessageBox.Show("Corruption has been detected in the edited save file. No data has been written. Please report this issue on github.com/Deweh/CyberCAT-SimpleGUI with the generated error.txt file.");
                    return;
                }

                File.WriteAllBytes(saveWindow.FileName, saveBytes);
                activeSaveFile = testFile;
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
                var playerInventory = activeSaveFile.GetInventory(1);
                ((ItemData.SimpleItemData)playerInventory.Items[ playerInventory.Items.IndexOf( playerInventory.Items.Where(x => x.ItemGameName == "Eddies").FirstOrDefault() )].Data).Quantity = (uint)moneyUpDown.Value;
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
            ((FactsTable.FactEntry)factsListView.SelectedVirtualItems()[0].Tag).Value = UInt32.Parse(e.Label);
        }

        private void inventoryListView_DoubleClick(object sender, EventArgs e)
        {
            if (inventoryListView.SelectedIndices.Count > 0)
            {
                var activeDetails = new ItemDetails();
                activeDetails.LoadItem((ItemData)inventoryListView.SelectedVirtualItems()[0].Tag, activeSaveFile, RefreshInventory);
            }
        }

        private void inventoryListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (inventoryListView.SelectedIndices.Count > 0 && e.KeyCode == Keys.Delete)
            {
                var containerID = containersListBox.SelectedItem.ToString();
                if (inventoryNames.Values.Contains(containerID))
                {
                    containerID = inventoryNames.Where(x => x.Value == containerID).FirstOrDefault().Key.ToString();
                }

                var activeItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
                activeSaveFile.GetInventory(ulong.Parse(containerID)).Items.Remove(activeItem);

                if (((ItemData)inventoryListView.SelectedVirtualItems()[0].Tag).ItemGameName == "Eddies" && containerID == "1")
                {
                    moneyUpDown.Enabled = false;
                    moneyUpDown.Value = 0;
                }

                inventoryListView.RemoveVirtualItem(inventoryListView.SelectedVirtualItems()[0]);
                statusLabel.Text = "'" + (string.IsNullOrEmpty(activeItem.ItemGameName) ? activeItem.ItemName : activeItem.ItemGameName) + "' deleted.";
            }
        }

        private void factsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (factsListView.SelectedIndices.Count > 0 && e.KeyCode == Keys.Delete)
            {
                ((FactsTable)activeSaveFile.GetFactsContainer().Children[0].Value).FactEntries.Remove((FactsTable.FactEntry)factsListView.SelectedVirtualItems()[0].Tag);
                statusLabel.Text = "'" + factsListView.SelectedVirtualItems()[0].SubItems[1].Text + "' deleted.";
                factsListView.RemoveVirtualItem(factsListView.SelectedVirtualItems()[0]);
            }
        }

        private void factsListView_MouseUp(object sender, EventArgs e)
        {
            if (factsListView.SelectedIndices.Count > 0)
            {
                factsListView.SelectedVirtualItems()[0].BeginEdit();
            }
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

        private void addFactButton_Click(object sender, EventArgs e)
        {
            var factDialog = new AddFact();
            factDialog.LoadFactDialog(RefreshFacts, activeSaveFile);
        }

        private void factsSaveButton_Click(object sender, EventArgs e)
        {
            var saveWindow = new SaveFileDialog();
            saveWindow.Filter = "JSON File|*.json";
            if (saveWindow.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveWindow.FileName, JsonConvert.SerializeObject(activeSaveFile.GetKnownFacts(), Formatting.Indented));
            }
        }

        private void statsButton_Click(object sender, EventArgs e)
        {
            SwapTab(statsButton, statsPanel);
        }

        private void lifePathBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lifePathBox.SelectedIndex == 0)
            {
                lifePathPictureBox.Image = CP2077SaveEditor.Properties.Resources.nomad;
                activeSaveFile.GetPlayerDevelopmentData().Value.LifePath = gamedataLifePath.Nomad;
            }
            else if (lifePathBox.SelectedIndex == 1)
            {
                lifePathPictureBox.Image = CP2077SaveEditor.Properties.Resources.streetkid;
                activeSaveFile.GetPlayerDevelopmentData().Value.LifePath = gamedataLifePath.StreetKid;
            }
            else if (lifePathBox.SelectedIndex == 2)
            {
                lifePathPictureBox.Image = CP2077SaveEditor.Properties.Resources.corpo;
                activeSaveFile.GetPlayerDevelopmentData().Value.LifePath = gamedataLifePath.Corporate;
            }
        }

        private void clearQuestFlagsButton_Click(object sender, EventArgs e)
        {
            foreach (Inventory.SubInventory inventory in activeSaveFile.GetInventoriesContainer().SubInventories)
            {
                foreach (ItemData item in inventory.Items)
                {
                    item.Flags.Raw = 0;
                }
            }
            MessageBox.Show("All item flags cleared.");
        }

        private void additionalPlayerStatsButton_Click(object sender, EventArgs e)
        {
            var i = Array.FindIndex(activeSaveFile.GetStatsMap().Keys, x => x.EntityHash == 1);
            var details = new ItemDetails();
            details.LoadStatsOnly(activeSaveFile.GetStatsMap().Values[i].Seed, activeSaveFile, "Player");
        }

        private void enableSecretEndingButton_Click(object sender, EventArgs e)
        {
            activeSaveFile.SetFactByName("sq032_johnny_friend", 1);
            RefreshFacts();
            MessageBox.Show("Secret ending enabled.");
        }

        private void makeAllRomanceableButton_Click(object sender, EventArgs e)
        {
            activeSaveFile.SetFactByName("judy_romanceable", 1);
            activeSaveFile.SetFactByName("river_romanceable", 1);
            activeSaveFile.SetFactByName("panam_romanceable", 1);
            activeSaveFile.SetFactByName("kerry_romanceable", 1);
            RefreshFacts();
            MessageBox.Show("All characters are now romanceable.");
        }

        private void appearanceCompareLoadButton_Click(object sender, EventArgs e)
        {
            var fileWindow = new OpenFileDialog();
            fileWindow.Filter = "Cyberpunk 2077 Save File|*.dat";
            if (fileWindow.ShowDialog() == DialogResult.OK)
            {
                appearanceCompareListBox.Items.Clear();
                appearanceCompareNodes = new Dictionary<string, CharacterCustomizationAppearances>();

                foreach (string fileName in Directory.GetFiles(Path.GetDirectoryName(fileWindow.FileName)))
                {
                    if (fileName.EndsWith(".dat"))
                    {
                        var tempSave = new SaveFileHelper(new List<INodeParser> { new CharacterCustomizationAppearancesParser() });
                        tempSave.LoadPCSaveFile(new MemoryStream(File.ReadAllBytes(fileName)));
                        appearanceCompareNodes.Add(Path.GetFileNameWithoutExtension(fileName), tempSave.GetAppearanceContainer());
                        appearanceCompareListBox.Items.Add(fileName);
                    }
                }
            }
        }

        private void appearanceCompareSaveButton_Click(object sender, EventArgs e)
        {
            var values = new List<AppearanceConstructor.ValueRepresentation>();

            foreach (string appearanceName in appearanceCompareNodes.Keys)
            {
                var singleValue = new AppearanceConstructor.ValueRepresentation(appearanceCompareNameBox.Text, appearanceName);
                var currentAppearance = appearanceCompareNodes[appearanceName];

                var mainSections = new[] { currentAppearance.FirstSection, currentAppearance.SecondSection, currentAppearance.ThirdSection };
                var sectionName = "First";

                foreach (CharacterCustomizationAppearances.Section mainSection in mainSections)
                {
                    foreach (CharacterCustomizationAppearances.AppearanceSection section in currentAppearance.FirstSection.AppearanceSections)
                    {
                        var appearanceSectionName = section.SectionName;
                        var baseSection = activeSaveFile.GetAppearanceContainer().FirstSection.AppearanceSections.Where(x => x.SectionName == section.SectionName).FirstOrDefault();

                        foreach (CharacterCustomizationAppearances.HashValueEntry mainEntry in section.MainList)
                        {
                            var baseMainEntry = baseSection.MainList.Where(x => activeSaveFile.Appearance.CompareMainListAppearanceEntries(x.SecondString, mainEntry.SecondString)).FirstOrDefault();

                            if (baseMainEntry != null)
                            {
                                if (baseMainEntry.FirstString != mainEntry.FirstString || baseMainEntry.Hash != mainEntry.Hash || baseMainEntry.SecondString != mainEntry.SecondString)
                                {
                                    singleValue.Entries.Add(new AppearanceConstructor.EntryPair(new AppearanceConstructor.EntryLocation(sectionName, appearanceSectionName), mainEntry));
                                }
                            }
                            else
                            {
                                singleValue.Entries.Add(new AppearanceConstructor.EntryPair(new AppearanceConstructor.EntryLocation(sectionName, appearanceSectionName), mainEntry));
                            }
                        }

                        foreach (CharacterCustomizationAppearances.ValueEntry additionalEntry in section.AdditionalList)
                        {
                            var baseAdditionalEntry = baseSection.AdditionalList.Where(x => x.FirstString == additionalEntry.FirstString).FirstOrDefault();

                            if (baseAdditionalEntry != null)
                            {
                                if (baseAdditionalEntry.SecondString != additionalEntry.SecondString)
                                {
                                    singleValue.Entries.Add(new AppearanceConstructor.EntryPair(new AppearanceConstructor.EntryLocation(sectionName, appearanceSectionName), additionalEntry));
                                }
                            }
                            else
                            {
                                singleValue.Entries.Add(new AppearanceConstructor.EntryPair(new AppearanceConstructor.EntryLocation(sectionName, appearanceSectionName), additionalEntry));
                            }
                        }
                    }

                    if (sectionName == "First")
                    {
                        sectionName = "Second";
                    } else {
                        sectionName = "Third";
                    }
                }

                values.Add(singleValue);
            }

            var fileWindow = new SaveFileDialog();
            fileWindow.Filter = "JSON File|*.json";
            if (fileWindow.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(fileWindow.FileName, JsonConvert.SerializeObject(values, Formatting.Indented));
            }
        }

        private void swapSaveType_Click(object sender, EventArgs e)
        {
            if (saveType == 0)
            {
                saveType = 1;
                swapSaveType.Text = "Save Type: PS4";
            } else {
                saveType = 0;
                swapSaveType.Text = "Save Type: PC";
            }
        }

        private void hairStyleBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loadingSave)
            {
                if (hairColorBox.Enabled == false && hairStyleBox.Text != "Shaved")
                {
                    activeSaveFile.Appearance.CreateHairEntry(hairStyleBox.Text);
                    RefreshAppearanceValues();

                    hairColorBox.Items.Remove("None");
                    return;
                }

                if (hairColorBox.Enabled == true && hairStyleBox.Text == "Shaved")
                {
                    activeSaveFile.Appearance.DeleteHairEntry();

                    RefreshAppearanceValues();
                    return;
                }

                activeSaveFile.Appearance.SetHairStyle(hairStyleBox.Text);
            }
        }

        private void hairColorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            activeSaveFile.Appearance.SetHairColor(hairColorBox.Text);
        }

        private void skinColorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            activeSaveFile.Appearance.SetSkinColor(skinColorBox.Text);
        }

        private void eyesColorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            activeSaveFile.Appearance.SetEyeColor(eyesColorBox.Text);
        }

        private void debloatButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This process will attempt to remove redundant data from your save & is potentially destructive. It's highly recommended that you back up your save before continuing. This process can take 10+ minutes for heavily bloated saves and cannot be cancelled once started. Continue?", "WARNING", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            debloatWorker.RunWorkerAsync();
            debloatTimer.Start();

            editorPanel.Enabled = false;
            optionsPanel.Enabled = false;
            openSaveButton.Enabled = false;
            swapSaveType.Enabled = false;
            
        }

        private void debloatTimer_Tick(object sender, EventArgs e)
        {
            statusLabel.Text = debloatInfo;
        }

        private void debloatWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var x = 1;
            foreach (CyberCAT.Core.Classes.DumpedClasses.GameSavedStatsData stats in activeSaveFile.GetStatsMap().Values)
            {
                if (stats.StatModifiers != null)
                {
                    if (stats.StatModifiers.Count() > 100)
                    {
                        for (int i = 0; i <= stats.StatModifiers.Count() - 1; i++)
                        {
                            activeSaveFile.GetStatsContainer().RemoveHandle((int)stats.StatModifiers[i].Id);
                            debloatInfo = "DE-BLOAT IN PROGRESS :: (1/2) :: Entry: " + x.ToString() + "/" + activeSaveFile.GetStatsMap().Values.Length + " -- Handle: " + i.ToString() + "/" + stats.StatModifiers.Count().ToString();
                        }

                        stats.StatModifiers = new Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData>[] { };
                    }
                }
                x++;
            }

            uint y = 0;
            x = 0;
            foreach (CyberCAT.Core.Classes.DumpedClasses.GameSavedStatsData value in activeSaveFile.GetStatsMap().Values)
            {
                if (value.StatModifiers != null && value.StatModifiers.Count() > 0)
                {
                    if (y < 1)
                    {
                        y = value.StatModifiers[0].Id;
                    }
                    var z = 0;
                    foreach (Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData> modifierData in value.StatModifiers)
                    {
                        modifierData.SetId(y);
                        debloatInfo = "DE-BLOAT IN PROGRESS :: (2/2) :: Entry: " + x.ToString() + "/" + activeSaveFile.GetStatsMap().Values.Length + " -- Handle: " + z.ToString() + "/" + value.StatModifiers.Count().ToString();
                        y++;
                        z++;
                    }
                }
                x++;
            }
        }

        private void debloatWorker_Completed(object sender, EventArgs e)
        {
            debloatTimer.Stop();
            editorPanel.Enabled = true;
            optionsPanel.Enabled = true;
            openSaveButton.Enabled = true;
            swapSaveType.Enabled = true;

            statusLabel.Text = "De-bloat complete.";
            MessageBox.Show("De-bloat complete.");
        }

        private void PlayerStatChanged(object sender, EventArgs e)
        {
            if (!loadingSave)
            {
                var playerData = activeSaveFile.GetPlayerDevelopmentData();
                foreach (gamedataProficiencyType proficType in proficFields.Keys)
                {
                    if (proficType == gamedataProficiencyType.Invalid)
                    {
                        playerData.Value.Proficiencies[Array.FindIndex(playerData.Value.Proficiencies, x => x.Type == null)].CurrentLevel = (int)proficFields[proficType].Value;
                    }
                    else
                    {
                         playerData.Value.Proficiencies[Array.FindIndex(playerData.Value.Proficiencies, x => x.Type == proficType)].CurrentLevel = (int)proficFields[proficType].Value;
                    }
                }

                foreach (gamedataStatType attrType in attrFields.Keys)
                {
                     playerData.Value.Attributes[Array.FindIndex(playerData.Value.Attributes, x => x.AttributeName == attrType)].Value = (int)attrFields[attrType].Value;
                }

                var points = activeSaveFile.GetPlayerDevelopmentData().Value.DevPoints;

                points[Array.FindIndex(points, x => x.Type == CyberCAT.Core.DumpedEnums.gamedataDevelopmentPointType.Primary)].Unspent = (int)perkPointsUpDown.Value;
                points[Array.FindIndex(points, x => x.Type == null)].Unspent = (int)attrPointsUpDown.Value;
            }
        }
    }
}
