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
using System.Text.RegularExpressions;
using System.Reflection;

namespace CP2077SaveEditor
{
    public partial class Form1 : Form
    {
        //Save File Object
        private SaveFileHelper activeSaveFile;

        //Save Info
        private bool loadingSave = false;
        private bool cancelLoad = false;
        private int saveType = 0;
        private Random globalRand = new Random();
        private WrongDefaultValueEventArgs wrongDefaultInfo = null;

        //GUI
        private ModernButton activeTabButton = new ModernButton();
        private Panel activeTabPanel = new Panel();
        private string debloatInfo = "";

        //Lookup Dictionaries
        private Dictionary<Enum, NumericUpDown> attrFields, proficFields;
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

            var tabPanels = new Panel[] { appearancePanel, inventoryPanel, factsPanel, statsPanel, vehiclesPanel };
            foreach (Panel singleTab in tabPanels)
            {
                editorPanel.Controls.Add(singleTab);
                singleTab.Dock = DockStyle.Fill;
                singleTab.Visible = false;
            }

            GenericUnknownStructParser.WrongDefaultValue += GenericUnknownStructParser_WrongDefaultValue;
            factsListView.AfterLabelEdit += factsListView_AfterLabelEdit;
            factsListView.MouseUp += factsListView_MouseUp;
            factsListView.KeyDown += factsListView_KeyDown;
            inventoryListView.MouseClick += inventoryListView_Click;
            inventoryListView.DoubleClick += inventoryListView_DoubleClick;
            inventoryListView.KeyDown += inventoryListView_KeyDown;
            vehiclesListView.DoubleClick += vehiclesListView_DoubleClick;

            factsSearchBox.GotFocus += SearchBoxGotFocus;
            factsSearchBox.LostFocus += SearchBoxLostFocus;
            inventorySearchBox.GotFocus += SearchBoxGotFocus;
            inventorySearchBox.LostFocus += SearchBoxLostFocus;

            perkPointsUpDown.ValueChanged += PlayerStatChanged;
            attrPointsUpDown.ValueChanged += PlayerStatChanged;
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

            foreach (NumericUpDown numUpDown in attrFields.Values)
            {
                numUpDown.ValueChanged += PlayerStatChanged;
            }

            foreach (NumericUpDown numUpDown in proficFields.Values)
            {
                numUpDown.ValueChanged += PlayerStatChanged;
            }

            var lastPos = 20;
            foreach (PropertyInfo property in typeof(AppearanceHelper).GetProperties())
            {
                //if (property.PropertyType.Name == "Object")
                //{
                //    MessageBox.Show(property.Name);
                //}

                if (property.PropertyType.Name != "Object" && property.PropertyType.Name != "Boolean" && property.Name != "MainSections" && property.CanWrite == true)
                {
                    var picker = new ModernValuePicker()
                    {
                        Name = property.Name,
                        PickerName = Regex.Replace(property.Name, "([a-z])([A-Z])", "$1 $2"),
                        Location = new Point(0, lastPos),
                        Tag = property
                    };

                    appearanceOptionsPanel.Controls.Add(picker);
                    picker.IndexChanged += AppearanceOptionChanged;
                    picker.MouseEnter += AppearanceOptionMouseEnter;
                    
                    lastPos += picker.Height + 20;
                }
            }

            if (File.Exists(Environment.CurrentDirectory + "\\config.json"))
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(Environment.CurrentDirectory + "\\config.json"));
                    if (config["SaveButtonPosition"] == 1)
                    {
                        saveChangesButton.Location = new Point(saveChangesButton.Location.X, factsButton.Location.Y + factsButton.Height - 1);
                        saveChangesButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    }
                    else if(config["SaveButtonPosition"] > 1)
                    {
                        saveChangesButton.Location = new Point(saveChangesButton.Location.X, saveChangesButton.Location.Y - config["SaveButtonPosition"]);
                    }
                }
                catch (Exception)
                { }
            }
            
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
            foreach (ModernValuePicker picker in appearanceOptionsPanel.Controls)
            {
                try
                {
                    var value = GetAppearanceValue(picker);
                    if (value is int intValue)
                    {
                        if (intValue < 0)
                        {
                            picker.StringValue = string.Empty;
                            picker.Enabled = false;
                            continue;
                        }

                        picker.SuppressIndexChange = true;
                        picker.Index = intValue;
                    }
                    else if (value is string strValue)
                    {
                        if (picker.StringCollection.Length < 1)
                        {
                            picker.PickerType = PickerValueType.String;
                            picker.StringCollection = ((List<string>)typeof(AppearanceValueLists).GetProperty(picker.Name + "s").GetValue(null, null)).ToArray();
                        }
                        picker.SuppressIndexChange = true;

                        if (picker.StringCollection.Contains(strValue))
                        {
                            picker.Index = Array.IndexOf(picker.StringCollection, strValue);
                        }
                        else
                        {
                            picker.Index = 0;
                            picker.StringValue = strValue;
                        }
                    }
                    else if (value is Enum)
                    {
                        if (picker.StringCollection.Length < 1)
                        {
                            picker.PickerType = PickerValueType.String;
                            picker.StringCollection = Enum.GetNames(value.GetType());
                        }

                        picker.SuppressIndexChange = true;
                        picker.Index = (int)value;
                    }
                    picker.Enabled = true;
                }
                catch(Exception)
                {
                    picker.StringValue = string.Empty;
                    picker.Enabled = false;
                }
            }
        }

        private object GetAppearanceValue(ModernValuePicker picker)
        {
            return ((PropertyInfo)picker.Tag).GetValue(activeSaveFile.Appearance);
        }

        private void SetAppearanceValue(ModernValuePicker picker, object value)
        {
            ((PropertyInfo)picker.Tag).SetValue(activeSaveFile.Appearance, value);
        }

        private void AppearanceOptionChanged(ModernValuePicker sender)
        {
            var value = ((PropertyInfo)sender.Tag).PropertyType;
            if (value == typeof(int))
            {
                SetAppearanceValue(sender, sender.Index);
            }
            else if(value == typeof(string))
            {
                SetAppearanceValue(sender, sender.StringValue);
            }
            else if (value.IsEnum)
            {
                SetAppearanceValue(sender, Enum.Parse(value, sender.StringValue));
            }
            RefreshAppearanceValues();
            SetAppearanceImage(sender.Name, sender.Index.ToString("00"));
        }

        private void AppearanceOptionMouseEnter(object sender, EventArgs e)
        {
            if (((ModernValuePicker)sender).Enabled == true)
            {
                SetAppearanceImage(((ModernValuePicker)sender).Name, ((ModernValuePicker)sender).Index.ToString("00"));
            }
        }

        private void SetAppearanceImage(string name, string value)
        {
            var path = Environment.CurrentDirectory + "\\previews\\" + name + "\\" + activeSaveFile.Appearance.BodyGender.ToString() + value + ".jpg";
            if (!File.Exists(path)) {
                path = Environment.CurrentDirectory + "\\previews\\" + name + "\\" + value + ".jpg";
                if (!File.Exists(path))
                {
                    appearancePreviewBox.Image = new Bitmap(1, 1);
                    return;
                }
            }

            try
            {
                Image oldImg = appearancePreviewBox.Image;
                appearancePreviewBox.Image = Image.FromFile(path);
                if (oldImg != null) oldImg.Dispose();
            }
            catch (Exception)
            {
                appearancePreviewBox.Image = new Bitmap(1, 1);
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
                var row = new string[] { item.ItemTdbId.GameNameFallback, "", item.ItemTdbId.Name, "", "1", item.ItemTdbId.GameDescription };

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
                if (item.ItemTdbId.GameName == "Eddies" && containerID == "1")
                {
                    moneyUpDown.Value = ((ItemData.SimpleItemData)item.Data).Quantity;
                    moneyUpDown.Enabled = true;
                }
            }

            if (containerID == "1")
            {
                foreach (KeyValuePair<CyberCAT.Core.Classes.DumpedClasses.GameItemID, string> equipInfo in activeSaveFile.GetEquippedItems().Reverse())
                {
                    var equippedItem = listViewRows.Where(x => ((ItemData)x.Tag).ItemTdbId.Raw64 == equipInfo.Key.Id.Raw64).FirstOrDefault();

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
                var itemNames = JsonConvert.DeserializeObject<Dictionary<ulong, JsonResolver.NameStruct>>(CP2077SaveEditor.Properties.Resources.ItemNames);
                NameResolver.TweakDbResolver = new JsonResolver(itemNames);
                FactResolver.UseDictionary(JsonConvert.DeserializeObject<Dictionary<ulong, string>>(CP2077SaveEditor.Properties.Resources.Facts));

                var parsers = new List<INodeParser>();
                parsers.AddRange(new INodeParser[] {
                    new CharacterCustomizationAppearancesParser(), new InventoryParser(), new ItemDataParser(), new FactsDBParser(),
                    new FactsTableParser(), new GameSessionConfigParser(), new ItemDropStorageManagerParser(), new ItemDropStorageParser(),
                    new StatsSystemParser(), new ScriptableSystemsContainerParser(), new PSDataParser()
                });

                wrongDefaultInfo = null;

                var newSave = new SaveFileHelper(parsers);
                newSave.Load(new MemoryStream(File.ReadAllBytes(fileWindow.FileName)));

                if (wrongDefaultInfo != null)
                {
                    if (new WrongDefaultDialog(wrongDefaultInfo.ClassName, wrongDefaultInfo.PropertyName, wrongDefaultInfo.Value).ShowDialog() != DialogResult.OK)
                    {
                        cancelLoad = true;
                    }
                }

                if (cancelLoad)
                {
                    cancelLoad = false;
                    statusLabel.Text = "Load cancelled.";
                    return;
                }
                
                activeSaveFile = newSave;

                //Appearance parsing
                RefreshAppearanceValues();
                SetAppearanceImage("VoiceTone", ((int)activeSaveFile.Appearance.VoiceTone).ToString("00"));

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
                perkPointsUpDown.SetValue(points[Array.FindIndex(points, x => x.Type == gamedataDevelopmentPointType.Primary)].Unspent);
                attrPointsUpDown.SetValue(points[Array.FindIndex(points, x => x.Type == null)].Unspent);

                //PSData parsing
                var ps = activeSaveFile.GetPSDataContainer();
                var vehiclePS = (CyberCAT.Core.Classes.DumpedClasses.VehicleGarageComponentPS)ps.ClassList.Where(x => x is CyberCAT.Core.Classes.DumpedClasses.VehicleGarageComponentPS).FirstOrDefault();
                var unlockedVehicles = new List<string>();

                var vehicles = itemNames.Values.Where(x => x.Name.StartsWith("Vehicle.") && x.Name.EndsWith("_player"));
                var listItems = new List<ListViewItem>();

                if (vehiclePS != null)
                {
                    vehiclesListView.Enabled = true;
                    unlockedVehicles = vehiclePS.UnlockedVehicleArray.Select(x => x.VehicleID.RecordID.Name).ToList();

                    foreach (var info in vehicles)
                    {
                        var newItem = new ListViewItem(info.Name);
                        newItem.Checked = true;
                        newItem.Checked = unlockedVehicles.Contains(info.Name);
                        listItems.Add(newItem);
                    }
                }
                else
                {
                    vehiclesListView.Enabled = false;
                    listItems.Add(new ListViewItem("No vehicle data found. Try unlocking at least 1 vehicle in-game first."));
                }

                vehiclesListView.SetVirtualItems(listItems);

                //Update controls
                loadingSave = false;
                editorPanel.Enabled = true;
                optionsPanel.Enabled = true;
                filePathLabel.Text = Path.GetFileName(Path.GetDirectoryName(fileWindow.FileName));
                statusLabel.Text = "Save file loaded.";
                SwapTab(statsButton, statsPanel);
            }
        }

        private void GenericUnknownStructParser_WrongDefaultValue(object sender, WrongDefaultValueEventArgs e)
        {
            e.Ignore = true;
            wrongDefaultInfo = e;
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
                    saveBytes = activeSaveFile.Save();
                } else {
                    saveBytes = activeSaveFile.Save(false);
                }
                

                var parsers = new List<INodeParser>();
                parsers.AddRange(new INodeParser[] {
                    new CharacterCustomizationAppearancesParser(), new InventoryParser(), new ItemDataParser(), new FactsDBParser(),
                    new FactsTableParser(), new GameSessionConfigParser(), new ItemDropStorageManagerParser(), new ItemDropStorageParser(),
                    new StatsSystemParser(), new ScriptableSystemsContainerParser(), new PSDataParser()
                });

                var testFile = new SaveFileHelper(parsers);
                try
                {
                    if (saveType == 0)
                    {
                        testFile.Load(new MemoryStream(saveBytes));
                    }
                    else
                    {
                        testFile.Load(new MemoryStream(saveBytes));
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
                ((ItemData.SimpleItemData)playerInventory.Items[ playerInventory.Items.IndexOf( playerInventory.Items.Where(x => x.ItemTdbId.GameName == "Eddies").FirstOrDefault() )].Data).Quantity = (uint)moneyUpDown.Value;
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
                if (newValues.UnknownFirstBytes[4] != activeSaveFile.GetAppearanceContainer().UnknownFirstBytes[4])
                {
                    activeSaveFile.Appearance.SuppressBodyGenderPrompt = true;
                    activeSaveFile.Appearance.BodyGender = (AppearanceGender)newValues.UnknownFirstBytes[4];
                }
                activeSaveFile.Appearance.SetAllValues(newValues);
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

        private void inventoryListView_Click (object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && inventoryListView.SelectedIndices.Count > 0)
            {
                var containerID = containersListBox.SelectedItem.ToString();
                if (inventoryNames.Values.Contains(containerID))
                {
                    containerID = inventoryNames.Where(x => x.Value == containerID).FirstOrDefault().Key.ToString();
                }

                var contextMenu = new ContextMenuStrip();
                if (containerID == "1")
                {
                    var activeItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
                    var equipSlot = activeSaveFile.GetEquippedItems().Where(x => x.Key.Id.Raw64 == activeItem.ItemTdbId.Raw64).FirstOrDefault().Key;

                    if (equipSlot != null)
                    {
                        var unequipItem = contextMenu.Items.Add("Unequip");
                        unequipItem.Tag = equipSlot;
                        unequipItem.Click += UnequipInventoryItem;
                    }
                    else
                    {
                        var equipIn = new ToolStripMenuItem("Equip in Slot");
                        foreach (var area in activeSaveFile.GetEquipAreas())
                        {
                            int counter = 1;
                            foreach (var slot in area.EquipSlots)
                            {
                                var slotItem = equipIn.DropDownItems.Add(area.AreaType.ToString() + " " + counter.ToString());
                                slotItem.Tag = slot;
                                slotItem.Click += EquipInventoryItem;
                                counter++;
                            }
                        }
                        contextMenu.Items.Add(equipIn);
                    }
                }
                contextMenu.Items.Add("Delete").Click += DeleteInventoryItem;
                contextMenu.Show(Cursor.Position);
            }
        }

        private void inventoryListView_DoubleClick(object sender, EventArgs e)
        {
            if (inventoryListView.SelectedIndices.Count > 0)
            {
                var activeDetails = new ItemDetails();
                activeDetails.LoadItem((ItemData)inventoryListView.SelectedVirtualItems()[0].Tag, activeSaveFile, RefreshInventory, globalRand);
            }
        }

        private void inventoryListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (inventoryListView.SelectedIndices.Count > 0 && e.KeyCode == Keys.Delete)
            {
                DeleteInventoryItem();
            }
        }

        private void EquipInventoryItem(object sender, EventArgs e)
        {
            var slot = (CyberCAT.Core.Classes.DumpedClasses.GameSEquipSlot)((ToolStripItem)sender).Tag;
            var currentItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
            slot.ItemID = new CyberCAT.Core.Classes.DumpedClasses.GameItemID() { Id = new TweakDbId() { Raw64 = currentItem.ItemTdbId.Raw64 } };
            RefreshInventory();
            inventoryListView.SelectedVirtualItems()[0].Selected = false;
        }

        private void UnequipInventoryItem(object sender, EventArgs e)
        {
            var equipId = (CyberCAT.Core.Classes.DumpedClasses.GameItemID)((ToolStripItem)sender).Tag;
            foreach (var equipSlot in activeSaveFile.GetEquipSlotsFromID(equipId))
            {
                equipSlot.ItemID = null;
            }

            RefreshInventory();
            if (inventoryListView.SelectedVirtualItems().Count > 0)
            {
                inventoryListView.SelectedVirtualItems()[0].Selected = false;
            }
        }

        private void DeleteInventoryItem(object sender = null, EventArgs e = null)
        {
            var containerID = containersListBox.SelectedItem.ToString();
            if (inventoryNames.Values.Contains(containerID))
            {
                containerID = inventoryNames.Where(x => x.Value == containerID).FirstOrDefault().Key.ToString();
            }

            var activeItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
            activeSaveFile.GetInventory(ulong.Parse(containerID)).Items.Remove(activeItem);

            if (((ItemData)inventoryListView.SelectedVirtualItems()[0].Tag).ItemTdbId.GameName == "Eddies" && containerID == "1")
            {
                moneyUpDown.Enabled = false;
                moneyUpDown.Value = 0;
            }

            inventoryListView.RemoveVirtualItem(inventoryListView.SelectedVirtualItems()[0]);
            statusLabel.Text = "'" + (string.IsNullOrEmpty(activeItem.ItemTdbId.GameName) ? activeItem.ItemTdbId.Name : activeItem.ItemTdbId.GameName) + "' deleted.";
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
            MessageBox.Show("Note: This option is not meant to overwrite your save file. If you are only trying to save changes to your quest facts, you should press 'Save Changes' instead.");

            var saveWindow = new SaveFileDialog();
            saveWindow.Filter = "Text File|*.txt";
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

        private void debloatButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This process will remove redundant data from your save. Just in case, it's recommended that you back up your save before continuing. Continue?", "Notice", MessageBoxButtons.YesNo) != DialogResult.Yes)
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
            var entryCounter = 1;
            foreach (CyberCAT.Core.Classes.DumpedClasses.GameSavedStatsData stats in activeSaveFile.GetStatsMap().Values)
            {
                if (stats.StatModifiers != null)
                {
                    var craftedModifiers = stats.StatModifiers.Where(x => x.Value.StatType == gamedataStatType.IsItemCrafted);

                    if (craftedModifiers != null && craftedModifiers.Count() > 100)
                    {
                        debloatInfo = "DE-BLOAT IN PROGRESS :: (1/2) :: Entry: " + entryCounter.ToString() + "/" + activeSaveFile.GetStatsMap().Values.Length.ToString();

                        var ids = new HashSet<uint>();
                        foreach (Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData> modifierData in craftedModifiers)
                        {
                            ids.Add(modifierData.Id);
                        }

                        activeSaveFile.GetStatsContainer().RemoveHandles(ids);
                        stats.StatModifiers = new Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData>[] { };
                    }
                }
                entryCounter++;
            }

            entryCounter = 0;
            uint handleInd = 1;
            foreach (CyberCAT.Core.Classes.DumpedClasses.GameSavedStatsData value in activeSaveFile.GetStatsMap().Values)
            {
                if (value.StatModifiers != null && value.StatModifiers.Count() > 0)
                {
                    var handleCounter = 0;
                    foreach (Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData> modifierData in value.StatModifiers)
                    {
                        modifierData.Id = handleInd;
                        debloatInfo = "DE-BLOAT IN PROGRESS :: (2/2) :: Entry: " + entryCounter.ToString() + "/" + activeSaveFile.GetStatsMap().Values.Length.ToString() + " -- Handle: " + handleCounter.ToString() + "/" + value.StatModifiers.Count().ToString();
                        handleInd++; handleCounter++;
                    }
                }
                entryCounter++;
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

        private void vehiclesButton_Click(object sender, EventArgs e)
        {
            SwapTab(vehiclesButton, vehiclesPanel);
        }

        private void vehiclesListView_DoubleClick(object sender, EventArgs e)
        {
            var ps = activeSaveFile.GetPSDataContainer();
            var vehiclePS = (CyberCAT.Core.Classes.DumpedClasses.VehicleGarageComponentPS)ps.ClassList.Where(x => x is CyberCAT.Core.Classes.DumpedClasses.VehicleGarageComponentPS).FirstOrDefault();
            var unlockedVehicles = vehiclePS.UnlockedVehicleArray.Select(x => x.VehicleID.RecordID.Name);

            foreach (var selectedItem in vehiclesListView.GetVirtualInfo().Items)
            {
                if (selectedItem.Checked)
                {
                    if (!unlockedVehicles.Contains(selectedItem.Text))
                    {
                        vehiclePS.UnlockedVehicleArray = vehiclePS.UnlockedVehicleArray.Append(new CyberCAT.Core.Classes.DumpedClasses.VehicleUnlockedVehicle()
                        {
                            VehicleID = new CyberCAT.Core.Classes.DumpedClasses.VehicleGarageVehicleID() { RecordID = TweakDbId.FromName(selectedItem.Text) }
                        }).ToArray();
                    }
                }
                else
                {
                    if (unlockedVehicles.Contains(selectedItem.Text))
                    {
                        var list = vehiclePS.UnlockedVehicleArray.ToList();
                        foreach (var unlocked in vehiclePS.UnlockedVehicleArray)
                        {
                            if (unlocked.VehicleID.RecordID.Name == selectedItem.Text)
                            {
                                list.Remove(unlocked);
                            }
                        }
                        vehiclePS.UnlockedVehicleArray = list.ToArray();
                    }
                }
            }

            vehiclesListView.Invalidate();
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
