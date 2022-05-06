using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Save.IO;
using static WolvenKit.RED4.Types.Enums;
using static WolvenKit.RED4.Save.InventoryHelper;
using Newtonsoft.Json;
using CP2077SaveEditor.Extensions;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor
{
    public partial class Form1 : Form
    {
        //Save File Object
        public static SaveFileHelper activeSaveFile;

        //Services
        public static WolvenKit.Common.Services.TweakDBService tdbService;
        public static WolvenKit.Common.Services.HashService hashService;

        //Save Info
        private bool loadingSave = false;
        private bool cancelLoad = false;
        private int saveType = 0;
        private Random globalRand = new Random();
        public static bool psDataEnabled = true;
        public static bool statsSystemEnabled = true;
        public static bool wipEnabled = false;

        //GUI
        private ModernButton activeTabButton = new ModernButton();
        private Panel activeTabPanel = new Panel();
        private string debloatInfo = "";
        private int currentProgress = 0;
        private int maxProgress = 0;
        private string currentNode = string.Empty;

        //Lookup Dictionaries
        private Dictionary<Enum, NumericUpDown> attrFields, proficFields;
        private static BinaryResolver itemResolver;
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
        private List<string> wipFields = new()
        {
            "Beard",
            "BeardStyle"
        };

        //Etc
        public static string appLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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

            //GenericUnknownStructParser.WrongDefaultValue += GenericUnknownStructParser_WrongDefaultValue;
            //SaveFile.ProgressChanged += SaveFile_ProgressChanged;
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
                {gamedataProficiencyType.Assault, assaultUpDown},
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

            if (File.Exists(Environment.CurrentDirectory + "\\config.json"))
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(Environment.CurrentDirectory + "\\config.json"));
                    if (config.ContainsKey("SaveButtonPosition") && config["SaveButtonPosition"] == 1)
                    {
                        saveChangesButton.Location = new System.Drawing.Point(saveChangesButton.Location.X, factsButton.Location.Y + factsButton.Height - 1);
                        saveChangesButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    }
                    else if (config.ContainsKey("SaveButtonPosition") && config["SaveButtonPosition"] > 1)
                    {
                        saveChangesButton.Location = new System.Drawing.Point(saveChangesButton.Location.X, saveChangesButton.Location.Y - config["SaveButtonPosition"]);
                    }

                    if (config.ContainsKey("EnablePSData") && config["EnablePSData"] == 0)
                    {
                        //psDataEnabled = false;
                    }

                    if (config.ContainsKey("EnableWIPFeatures") && config["EnableWIPFeatures"] == 1)
                    {
                        wipEnabled = true;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to read config.json", "Notice");
                }
            }

            var lastPos = 0;
            foreach (PropertyInfo property in typeof(AppearanceHelper).GetProperties())
            {
                //if (property.PropertyType.Name == "Object")
                //{
                //    MessageBox.Show(property.Name);
                //}

                if (!wipEnabled && wipFields.Contains(property.Name))
                {
                    continue;
                }

                if (property.PropertyType.Name != "Object" && property.PropertyType.Name != "Boolean" && property.Name != "MainSections" && property.CanWrite == true)
                {
                    var picker = new ModernValuePicker()
                    {
                        Name = property.Name,
                        PickerName = Regex.Replace(property.Name, "([a-z])([A-Z])", "$1 $2"),
                        Location = new System.Drawing.Point(0, lastPos),
                        Tag = property
                    };

                    appearanceOptionsPanel.Controls.Add(picker);
                    picker.IndexChanged += AppearanceOptionChanged;
                    picker.MouseEnter += AppearanceOptionMouseEnter;
                    
                    lastPos += picker.Height + 20;
                }
            }

            var args = Environment.GetCommandLineArgs();
            foreach (var singleArg in args)
            {
                if (File.Exists(singleArg))
                {
                    if (Path.GetExtension(singleArg) == ".preset")
                    {
                        //InstantApplyPreset(singleArg);
                    }
                    else if (Path.GetExtension(singleArg) == ".dat")
                    {
                        LoadSave(singleArg);
                    }
                }
            }

        }

        //private void InstantApplyPreset(string presetPath)
        //{
        //    var fileWindow = new OpenFileDialog()
        //    {
        //        Title = "Choose Save File To Apply Preset To",
        //        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\",
        //        Filter = "Cyberpunk 2077 Save File|*.dat"
        //    };

        //    if (fileWindow.ShowDialog() == DialogResult.OK && MessageBox.Show(new Form { TopMost = true }, "Are you sure you wish to apply " + Path.GetFileName(presetPath) + " to " + fileWindow.FileName + "?", "Apply Preset", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //    {
        //        var tempSave = new SaveFileHelper(new INodeParser[] { new CharacterCustomizationAppearancesParser() });

        //        try
        //        {
        //            tempSave.Load(new MemoryStream(File.ReadAllBytes(fileWindow.FileName)));
        //        }
        //        catch(Exception)
        //        {
        //            MessageBox.Show(new Form { TopMost = true }, "Unable to apply preset: failed to parse save file.", "Error");
        //            Close();
        //        }

        //        var newValues = JsonConvert.DeserializeObject<CharacterCustomizationAppearances>(File.ReadAllText(presetPath));

        //        if (newValues.UnknownFirstBytes.Length > 6)
        //        {
        //            newValues.UnknownFirstBytes = newValues.UnknownFirstBytes.Skip(newValues.UnknownFirstBytes.Length - 6).ToArray();
        //        }

        //        if (newValues.UnknownFirstBytes[4] != tempSave.GetAppearanceContainer().UnknownFirstBytes[4])
        //        {
        //            tempSave.Appearance.SuppressBodyGenderPrompt = true;
        //            tempSave.Appearance.BodyGender = (AppearanceGender)newValues.UnknownFirstBytes[4];
        //        }
        //        tempSave.Appearance.SetAllValues(newValues);

        //        byte[] newSave = null;
        //        try
        //        {
        //            newSave = tempSave.Save();
        //        }
        //        catch (Exception)
        //        {
        //            MessageBox.Show(new Form { TopMost = true }, "Unable to apply preset: failed to rebuild save file.", "Error");
        //            Close();
        //        }

        //        if (!File.Exists(Path.GetDirectoryName(fileWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(fileWindow.FileName) + ".old"))
        //        {
        //            File.Copy(fileWindow.FileName, Path.GetDirectoryName(fileWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(fileWindow.FileName) + ".old");
        //        }

        //        File.WriteAllBytes(fileWindow.FileName, newSave);
        //        MessageBox.Show(new Form { TopMost = true }, Path.GetFileName(presetPath) + " successfully applied to " + fileWindow.FileName, "Success");
        //    }
        //    Close();
        //}

        //private void SaveFile_ProgressChanged(object sender, SaveProgressChangedEventArgs e)
        //{
        //    if (e.NodeName != string.Empty)
        //    {
        //        currentProgress = 0;
        //        maxProgress = 0;
        //        currentNode = e.NodeName;
        //    }
        //    else if (e.Maximum > 0)
        //    {
        //        Interlocked.Exchange(ref currentProgress, e.CurrentProgress);
        //        Interlocked.Exchange(ref maxProgress, e.Maximum);
        //    }
        //}

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

        public void RefreshAppearanceValues()
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
            var oldImg = appearancePreviewBox.Image;
            var imgExists = true;
            var path = appLocation + "\\previews\\" + name + "\\" + activeSaveFile.Appearance.BodyGender.ToString() + value + ".jpg";
            if (!File.Exists(path)) {
                path = appLocation + "\\previews\\" + name + "\\" + value + ".jpg";
                if (!File.Exists(path))
                {
                    imgExists = false;
                }
            }

            if (imgExists)
            {
                try
                {
                    appearancePreviewBox.Image = Image.FromFile(path);
                }
                catch (Exception)
                {
                    appearancePreviewBox.Image = null;
                }
            }
            else
            {
                appearancePreviewBox.Image = null;
            }
            oldImg?.Dispose();

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
                var row = new string[] { item.ItemTdbId.ResolvedText, "", item.ItemTdbId.ResolvedText, "", "1", item.ItemTdbId.ResolvedText };

                if (item.Data.GetType() == typeof(SimpleItemData))
                {
                    row[4] = ((SimpleItemData)item.Data).Quantity.ToString();
                    row[1] = "[S] ";
                }
                else if(item.Data.GetType() == typeof(ModableItemWithQuantityData))
                {
                    row[4] = ((ModableItemWithQuantityData)item.Data).Quantity.ToString();
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
                if (item.ItemTdbId.ResolvedText == "Items.money" && containerID == "1")
                {
                    var money = ((SimpleItemData)item.Data).Quantity;

                    if(money > moneyUpDown.Maximum)
                    {
                        moneyUpDown.Maximum = money;
                    }

                    moneyUpDown.Value = money;
                    moneyUpDown.Enabled = true;
                }
            }

            if (containerID == "1")
            {
                foreach (KeyValuePair<gameItemID, string> equipInfo in activeSaveFile.GetEquippedItems().Reverse())
                {
                    var equippedItem = listViewRows.Where(x => ((ItemData)x.Tag).ItemTdbId == equipInfo.Key.Id).FirstOrDefault();

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
                    if (!fact.FactName.ToString().ToLower().Contains(search.ToLower()))
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

        private async void LoadSave(string savePath)
        {
            //File.WriteAllBytes("items.bin", BinaryDatabaseWriter.Write(JsonConvert.DeserializeObject<Dictionary<ulong, JsonResolver.NameStruct>>(File.ReadAllText("names.json")), "tweakdb.str"));
            //if (File.Exists(Path.Combine(appLocation, "items.bin")))
            //{
            //    itemResolver = new BinaryResolver(File.ReadAllBytes(Path.Combine(appLocation, "items.bin")));
            //}
            //else
            //{
            //    itemResolver = new BinaryResolver(CP2077SaveEditor.Properties.Resources.ItemNames);
            //}
            //NameResolver.TweakDbResolver = itemResolver;
            //FactResolver.UseDictionary(JsonConvert.DeserializeObject<Dictionary<ulong, string>>(CP2077SaveEditor.Properties.Resources.Facts));

            loadingSave = true;
            editorPanel.Enabled = false;
            optionsPanel.Enabled = false;
            loadTimer.Start();

            if (tdbService == null)
            {
                await Task.Run(() => tdbService = new());
            }

            if (hashService == null)
            {
                await Task.Run(() => hashService = new());
            }

            CyberpunkSaveFile bufferFile = null;
            Exception error = null;

            try
            {
                await Task.Run(() =>
                {
                    using var ms = new MemoryStream(File.ReadAllBytes(savePath));
                    var reader = new CyberpunkSaveReader(ms);

                    if (reader.ReadFile(out var save) == EFileReadErrorCodes.NoError)
                    {
                        bufferFile = save;
                    }

                    reader = null;
                });
            }
            catch (Exception e)
            {
                error = e;
            }

            loadTimer.Stop();
            currentProgress = 0;
            maxProgress = 0;
            currentNode = string.Empty;

            if (cancelLoad)
            {
                cancelLoad = false;
                statusLabel.Text = "Load cancelled.";
                return;
            }

            if (error != null)
            {
                statusLabel.Text = "Load cancelled.";

                try
                {
                    File.WriteAllText(Path.Combine(appLocation, "error.txt"), error.Message + Environment.NewLine + error.StackTrace);
                    MessageBox.Show("Failed to parse save file: " + error.Message + " An error.txt file has been generated with additional information.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to parse save file: " + error.Message + " \n\n Stack Trace: \n" + error.StackTrace);
                }

                return;
            }

            activeSaveFile = new SaveFileHelper() { SaveFile = bufferFile };

            //Appearance parsing
            RefreshAppearanceValues();
            SetAppearanceImage("VoiceTone", ((int)activeSaveFile.Appearance.VoiceTone).ToString("00"));

            //Inventory parsing
            moneyUpDown.Enabled = false;
            moneyUpDown.Value = 0;

            containersListBox.Items.Clear();
            foreach (SubInventory container in activeSaveFile.GetInventoriesContainer().SubInventories)
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
                SProficiency profic;
                profic = playerData.Proficiencies[Array.FindIndex(playerData.Proficiencies.ToArray(), x => x.Type == proficType)];
                if (profic.CurrentLevel > (int)proficFields[proficType].Maximum)
                {
                    proficFields[proficType].Maximum = profic.CurrentLevel;
                }

                proficFields[proficType].Value = profic.CurrentLevel;
            }

            foreach (gamedataStatType attrType in attrFields.Keys)
            {
                SAttribute attr = playerData.Attributes[Array.FindIndex(playerData.Attributes.ToArray(), x => x.AttributeName == attrType)];
                if (attr.Value > (int)attrFields[attrType].Maximum)
                {
                    attrFields[attrType].Maximum = attr.Value;
                }

                attrFields[attrType].Value = attr.Value;
            }

            var lp = activeSaveFile.GetPlayerDevelopmentData().LifePath;

            if (lp == gamedataLifePath.Nomad)
            {
                lifePathBox.SelectedIndex = 0;
            }
            else if (lp == gamedataLifePath.StreetKid)
            {
                lifePathBox.SelectedIndex = 1;
            }
            else
            {
                lifePathBox.SelectedIndex = 2;
            }

            var points = activeSaveFile.GetPlayerDevelopmentData().DevPoints.ToArray();
            perkPointsUpDown.SetValue(points[Array.FindIndex(points, x => x.Type == gamedataDevelopmentPointType.Primary)].Unspent);
            attrPointsUpDown.SetValue(points[Array.FindIndex(points, x => x.Type == gamedataDevelopmentPointType.Secondary)].Unspent);

            //PSData parsing

            var listItems = new List<ListViewItem>();

            if (!psDataEnabled)
            {
                vehiclesListView.Enabled = false;
                listItems.Add(new ListViewItem("PSData disabled."));
            }
            else
            {
                var ps = activeSaveFile.GetPSDataContainer();
                var vehiclePS = (vehicleGarageComponentPS)ps.Entries.Where(x => x.Data is vehicleGarageComponentPS).FirstOrDefault().Data;
                var unlockedVehicles = new List<string>();

                var vehicles = tdbService.GetRecords().Where(x => x.ResolvedText.StartsWith("Vehicle.") && x.ResolvedText.EndsWith("_player")).Select(x => x.ResolvedText);

                if (vehiclePS != null)
                {
                    vehiclesListView.Enabled = true;
                    if (vehiclePS.UnlockedVehicleArray == null)
                    {
                        vehiclePS.UnlockedVehicleArray = new();
                    }
                    else
                    {
                        unlockedVehicles = vehiclePS.UnlockedVehicleArray.Select(x => x.VehicleID.RecordID.ResolvedText).ToList();
                    }

                    foreach (var info in vehicles)
                    {
                        var newItem = new ListViewItem(info);
                        newItem.Checked = true;
                        newItem.Checked = unlockedVehicles.Contains(info);
                        listItems.Add(newItem);
                    }
                }
                else
                {
                    vehiclesListView.Enabled = false;
                    listItems.Add(new ListViewItem("No vehicle data found. Try unlocking at least 1 vehicle in-game first."));
                }
            }

            vehiclesListView.SetVirtualItems(listItems);

            //Update controls
            loadingSave = false;
            editorPanel.Enabled = true;
            optionsPanel.Enabled = true;
            filePathLabel.Text = Path.GetFileName(Path.GetDirectoryName(savePath));
            statusLabel.Text = "Save file loaded.";
            SwapTab(statsButton, statsPanel);
        }

        private void openSaveButton_Click(object sender, EventArgs e)
        {
            var fileWindow = new OpenFileDialog();
            fileWindow.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\";
            fileWindow.Filter = "Cyberpunk 2077 Save File|*.dat";
            if (fileWindow.ShowDialog() == DialogResult.OK)
            {
                LoadSave(fileWindow.FileName);
            }
        }

        //private void GenericUnknownStructParser_WrongDefaultValue(object sender, WrongDefaultValueEventArgs e)
        //{
        //    e.Ignore = true;
        //    //wrongDefaultInfo = e;
        //}

        private void saveChangesButton_Click(object sender, EventArgs e)
        {
            //var saveWindow = new SaveFileDialog();
            //saveWindow.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\";
            //saveWindow.Filter = "Cyberpunk 2077 Save File|*.dat";
            //if (saveWindow.ShowDialog() == DialogResult.OK)
            //{
            //    if (File.Exists(saveWindow.FileName) && !File.Exists(Path.GetDirectoryName(saveWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveWindow.FileName) + ".old"))
            //    {
            //        File.Copy(saveWindow.FileName, Path.GetDirectoryName(saveWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveWindow.FileName) + ".old");
            //    }

            //    editorPanel.Enabled = false;
            //    optionsPanel.Enabled = false;
            //    loadTimer.Start();
            //    var worker = new BackgroundWorker();
            //    worker.DoWork += (object sender, DoWorkEventArgs e) =>
            //    {
            //        e.Result = activeSaveFile.Save(saveType == 0);
            //    };
            //    worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            //    {
            //        byte[] saveBytes = (byte[])e.Result;
            //        loadTimer.Stop();
            //        currentProgress = 0;
            //        maxProgress = 0;
            //        currentNode = string.Empty;

            //        var parsers = new List<INodeParser>();
            //        parsers.AddRange(new INodeParser[] {
            //            new CharacterCustomizationAppearancesParser(), new InventoryParser(), new ItemDataParser(), new FactsDBParser(),
            //            new FactsTableParser(), new GameSessionConfigParser(), new ItemDropStorageManagerParser(), new ItemDropStorageParser(),
            //            new ScriptableSystemsContainerParser()
            //        });

            //        if (psDataEnabled)
            //        {
            //            parsers.Add(new PSDataParser());
            //        }

            //        if (statsSystemEnabled)
            //        {
            //            parsers.Add(new StatsSystemParser());
            //        }

            //        var testFile = new SaveFileHelper(parsers);
            //        try
            //        {
            //            if (saveType == 0)
            //            {
            //                testFile.Load(new MemoryStream(saveBytes));
            //            }
            //            else
            //            {
            //                testFile.Load(new MemoryStream(saveBytes));
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            statusLabel.Text = "Save cancelled.";
            //            try
            //            {
            //                File.WriteAllText(Path.Combine(appLocation, "error.txt"), ex.Message + '\n' + ex.TargetSite + '\n' + ex.StackTrace);
            //                MessageBox.Show("Corruption has been detected in the edited save file. No data has been written. Please report this issue on github.com/Deweh/CyberCAT-SimpleGUI with the generated error.txt file.");
            //            }
            //            catch (Exception)
            //            {
            //                MessageBox.Show("Corruption has been detected in the edited save file. No data has been written. Please report this issue on github.com/Deweh/CyberCAT-SimpleGUI. \n\n" + ex.Message + "\n" + ex.StackTrace);
            //            }

            //            return;
            //        }
            //        finally
            //        {
            //            editorPanel.Enabled = true;
            //            optionsPanel.Enabled = true;
            //        }

            //        File.WriteAllBytes(saveWindow.FileName, saveBytes);
            //        activeSaveFile = testFile;
            //        statusLabel.Text = "File saved.";
            //    };

            //    worker.RunWorkerAsync();
            //}
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
                ((SimpleItemData)playerInventory.Items[ playerInventory.Items.IndexOf( playerInventory.Items.Where(x => x.ItemTdbId.ResolvedText == "Items.money").FirstOrDefault() )].Data).Quantity = (uint)moneyUpDown.Value;
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
            //var fileWindow = new OpenFileDialog();
            //fileWindow.Filter = "Cyberpunk 2077 Character Preset|*.preset";
            //if (fileWindow.ShowDialog() == DialogResult.OK)
            //{
            //    var newValues = JsonConvert.DeserializeObject<CharacterCustomizationAppearances>(File.ReadAllText(fileWindow.FileName));

            //    if (newValues.UnknownFirstBytes.Length > 6)
            //    {
            //        newValues.UnknownFirstBytes = newValues.UnknownFirstBytes.Skip(newValues.UnknownFirstBytes.Length - 6).ToArray();
            //    }

            //    if (newValues.UnknownFirstBytes[4] != activeSaveFile.GetAppearanceContainer().UnknownFirstBytes[4])
            //    {
            //        activeSaveFile.Appearance.SuppressBodyGenderPrompt = true;
            //        activeSaveFile.Appearance.BodyGender = (AppearanceGender)newValues.UnknownFirstBytes[4];
            //    }
            //    activeSaveFile.Appearance.SetAllValues(newValues);
            //    RefreshAppearanceValues();
            //    statusLabel.Text = "Appearance preset loaded.";
            //}
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
                if (wipEnabled)
                {
                    contextMenu.Items.Add("New Item").Click += (object sender, EventArgs e) =>
                    {
                        var inventory = activeSaveFile.GetInventory(1);
                        //inventory.Items.Add(inventory.Items.Last().CreateSimpleItem());
                    };
                }

                if (containerID == "1")
                {
                    var activeItem = (InventoryHelper.ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
                    var equipSlot = activeSaveFile.GetEquippedItems().Where(x => x.Key.Id == activeItem.ItemTdbId).FirstOrDefault().Key;

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
            var slot = (gameSEquipSlot)((ToolStripItem)sender).Tag;
            var currentItem = (ItemData)inventoryListView.SelectedVirtualItems()[0].Tag;
            slot.ItemID = new gameItemID() { Id = currentItem.ItemTdbId };
            RefreshInventory();
            inventoryListView.SelectedVirtualItems()[0].Selected = false;
        }

        private void UnequipInventoryItem(object sender, EventArgs e)
        {
            var equipId = (gameItemID)((ToolStripItem)sender).Tag;
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

            if (((ItemData)inventoryListView.SelectedVirtualItems()[0].Tag).ItemTdbId.ResolvedText == "Items.money" && containerID == "1")
            {
                moneyUpDown.Enabled = false;
                moneyUpDown.Value = 0;
            }

            inventoryListView.RemoveVirtualItem(inventoryListView.SelectedVirtualItems()[0]);
            statusLabel.Text = "'" + activeItem.ItemTdbId.ResolvedText + "' deleted.";
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
                activeSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.Nomad;
            }
            else if (lifePathBox.SelectedIndex == 1)
            {
                lifePathPictureBox.Image = CP2077SaveEditor.Properties.Resources.streetkid;
                activeSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.StreetKid;
            }
            else if (lifePathBox.SelectedIndex == 2)
            {
                lifePathPictureBox.Image = CP2077SaveEditor.Properties.Resources.corpo;
                activeSaveFile.GetPlayerDevelopmentData().LifePath = gamedataLifePath.Corporate;
            }
        }

        private void clearQuestFlagsButton_Click(object sender, EventArgs e)
        {
            foreach (SubInventory inventory in activeSaveFile.GetInventoriesContainer().SubInventories)
            {
                foreach (ItemData item in inventory.Items)
                {
                    item.Flags = 0;
                }
            }
            MessageBox.Show("All item flags cleared.");
        }

        private void additionalPlayerStatsButton_Click(object sender, EventArgs e)
        {
            if (!statsSystemEnabled)
            {
                MessageBox.Show("Stats system disabled.");
                return;
            }
            var i = Array.FindIndex(activeSaveFile.GetStatsMap().Keys.ToArray(), x => x.EntityHash == 1);
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
            //var entryCounter = 1;
            //foreach (CyberCAT.Core.Classes.DumpedClasses.GameSavedStatsData stats in activeSaveFile.GetStatsMap().Values)
            //{
            //    if (stats.StatModifiers != null)
            //    {
            //        var craftedModifiers = stats.StatModifiers.Where(x => x.Value.StatType == gamedataStatType.IsItemCrafted);

            //        if (craftedModifiers != null && craftedModifiers.Count() > 100)
            //        {
            //            debloatInfo = "DE-BLOAT IN PROGRESS :: (1/2) :: Entry: " + entryCounter.ToString() + "/" + activeSaveFile.GetStatsMap().Values.Length.ToString();

            //            var ids = new HashSet<uint>();
            //            foreach (Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData> modifierData in craftedModifiers)
            //            {
            //                ids.Add(modifierData.Id);
            //            }

            //            activeSaveFile.GetStatsContainer().RemoveHandles(ids);
            //            stats.StatModifiers = new Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData>[] { };
            //        }
            //    }
            //    entryCounter++;
            //}

            //entryCounter = 0;
            //uint handleInd = 1;
            //foreach (CyberCAT.Core.Classes.DumpedClasses.GameSavedStatsData value in activeSaveFile.GetStatsMap().Values)
            //{
            //    if (value.StatModifiers != null && value.StatModifiers.Count() > 0)
            //    {
            //        var handleCounter = 0;
            //        foreach (Handle<CyberCAT.Core.Classes.DumpedClasses.GameStatModifierData> modifierData in value.StatModifiers)
            //        {
            //            modifierData.Id = handleInd;
            //            debloatInfo = "DE-BLOAT IN PROGRESS :: (2/2) :: Entry: " + entryCounter.ToString() + "/" + activeSaveFile.GetStatsMap().Values.Length.ToString() + " -- Handle: " + handleCounter.ToString() + "/" + value.StatModifiers.Count().ToString();
            //            handleInd++; handleCounter++;
            //        }
            //    }
            //    entryCounter++;
            //}
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
            var vehiclePS = (vehicleGarageComponentPS)ps.Entries.Where(x => x.Data is vehicleGarageComponentPS).FirstOrDefault().Data;
            var unlockedVehicles = vehiclePS.UnlockedVehicleArray.Select(x => x.VehicleID.RecordID.ResolvedText);

            foreach (var selectedItem in vehiclesListView.GetVirtualInfo().Items)
            {
                if (selectedItem.Checked)
                {
                    if (!unlockedVehicles.Contains(selectedItem.Text))
                    {
                        vehiclePS.UnlockedVehicleArray.Add(new vehicleUnlockedVehicle()
                        {
                            VehicleID = new vehicleGarageVehicleID() { RecordID = selectedItem.Text }
                        });
                    }
                }
                else
                {
                    if (unlockedVehicles.Contains(selectedItem.Text))
                    {
                        var list = vehiclePS.UnlockedVehicleArray.ToList();
                        foreach (var unlocked in vehiclePS.UnlockedVehicleArray)
                        {
                            if (unlocked.VehicleID.RecordID.ResolvedText == selectedItem.Text)
                            {
                                list.Remove(unlocked);
                            }
                        }
                        vehiclePS.UnlockedVehicleArray.Clear();
                        foreach (var itm in list)
                        {
                            vehiclePS.UnlockedVehicleArray.Add(itm);
                        }
                    }
                }
            }

            vehiclesListView.Invalidate();
        }

        private void loadTimer_Tick(object sender, EventArgs e)
        {
            var status = string.Empty;

            if (loadingSave)
            {
                status += "Parsing";
            }
            else
            {
                status += "Rebuilding";
            }

            if (currentNode != string.Empty)
            {
                status += " " + currentNode;
                if (maxProgress > 0 && currentProgress < maxProgress)
                {
                    status += " (" + Math.Round((Decimal.Divide(currentProgress, maxProgress) * 100), 0).ToString() + "%)";
                }
            }

            status += "...";
            statusLabel.Text = status;
        }

        private void PlayerStatChanged(object sender, EventArgs e)
        {
            if (!loadingSave)
            {
                var playerData = activeSaveFile.GetPlayerDevelopmentData();
                foreach (gamedataProficiencyType proficType in proficFields.Keys)
                {
                    playerData.Proficiencies[Array.FindIndex(playerData.Proficiencies.ToArray(), x => x.Type == proficType)].CurrentLevel = (int)proficFields[proficType].Value;
                }

                foreach (gamedataStatType attrType in attrFields.Keys)
                {
                     playerData.Attributes[Array.FindIndex(playerData.Attributes.ToArray(), x => x.AttributeName == attrType)].Value = (int)attrFields[attrType].Value;
                }

                var points = activeSaveFile.GetPlayerDevelopmentData().DevPoints;

                points[Array.FindIndex(points.ToArray(), x => x.Type == gamedataDevelopmentPointType.Primary)].Unspent = (int)perkPointsUpDown.Value;
                points[Array.FindIndex(points.ToArray(), x => x.Type == gamedataDevelopmentPointType.Secondary)].Unspent = (int)attrPointsUpDown.Value;
            }
        }

        private void advancedAppearanceButton_Click(object sender, EventArgs e)
        {
            var advancedDialog = new AdvancedAppearanceDialog();
            advancedDialog.ChangesApplied += RefreshAppearanceValues;
            advancedDialog.ShowDialog();
        }
    }
}
