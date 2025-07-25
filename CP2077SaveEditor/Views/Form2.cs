﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
using CP2077SaveEditor.Utils;
using CP2077SaveEditor.Views.Controls;
using WolvenKit.Common.Services;
using WolvenKit.Core.Compression;
using WolvenKit.RED4.Save.IO;
using WolvenKit.RED4.TweakDB.Helper;
using WolvenKit.RED4.Types.Pools;

namespace CP2077SaveEditor.Views
{
    public partial class Form2 : Form, INotifyPropertyChanged
    {
        internal static TweakDBStringHelper TweakDbStringHelper;
        internal HashService HashService;

        internal bool IsLoaded;

        private SaveFileHelper _activeSaveFile;

        private bool _isSaving = false;

        public Form2()
        {
            InitializeComponent();

            CompressionSettings.Get().UseOodle = false;
            ModManager.LoadTypes();

            RegisterControl(new PlayerStatsControl(this), true);
            RegisterControl(new AppearanceControl(this));
            RegisterControl(new InventoryControl(this));
            RegisterControl(new VehiclesControl(this));
            RegisterControl(new QuestFactsControl(this));
            RegisterControl(new ExtrasControl(this));

            if (File.Exists(Environment.CurrentDirectory + "\\config.json"))
            {
                try
                {
                    var config = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(Environment.CurrentDirectory + "\\config.json"));
                    if (config.ContainsKey("EnablePSData") && config["EnablePSData"] == 0)
                    {
                        // Global.PSDataEnabled = false;
                    }

                    if (config.ContainsKey("EnableWIPFeatures") && config["EnableWIPFeatures"] == 1)
                    {
                        Global.IsDebug = true;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to read config.json", "Notice");
                }
            }
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            SetStatus("Init...");

            await Init();

            openSaveButton.Enabled = true;

            SetStatus("Idle");
        }

        internal SaveFileHelper ActiveSaveFile
        {
            get => _activeSaveFile;
            private set => SetField(ref _activeSaveFile, value);
        }

        #region Controls

        private void RegisterControl(UserControl userControl, bool isLandingPage = false)
        {
            if (userControl is not IGameControl gc)
            {
                throw new Exception();
            }

            userControl.Dock = DockStyle.Fill;

            sm_Menu.AddButton(gc.GameControlName, (_, _) => { SwitchPanel(userControl); });
            if (isLandingPage)
            {
                SwitchPanel(userControl);
            }
        }

        private void SwitchPanel(UserControl userControl)
        {
            if (userControl == null)
            {
                return;
            }

            this.InvokeIfRequired(() =>
            {
                if (pnl_Content.Controls.Count != 0)
                {
                    pnl_Content.Controls.Clear();
                }

                pnl_Content.Controls.Add(userControl);
            });
        }

        #endregion Controls

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

        private async Task Init()
        {
            if (TweakDbStringHelper == null)
            {
                var tweakDbStrStream = typeof(TweakDBService).Assembly.GetManifestResourceStream("WolvenKit.Common.Resources.tweakdbstr.kark");

                TweakDbStringHelper = new TweakDBStringHelper();
                TweakDbStringHelper.LoadFromStream(tweakDbStrStream);
            }

            if (HashService == null)
            {
                await Task.Run(() => HashService = new HashService());
            }
        }

        private async void LoadSave(string savePath)
        {
            IsLoaded = false;
            pnl_Content.Enabled = false;
            openSaveButton.Enabled = false;
            saveChangesButton.Enabled = false;

            SetStatus("Loading save...");

            var status = EFileReadErrorCodes.NoCSav;

            try
            {
                await Task.Run(() =>
                {
                    using var fs = File.Open(savePath, FileMode.Open);
                    using var reader = new CyberpunkSaveReader(fs);

                    status = reader.ReadFile(out var save);
                    switch (status)
                    {
                        case EFileReadErrorCodes.NoError:
                            ActiveSaveFile = new SaveFileHelper { SaveFile = save };

                            var metadataPath = Path.Combine(Path.GetDirectoryName(savePath), "metadata.9.json");
                            if (File.Exists(metadataPath))
                            {
                                ActiveSaveFile.Metadata = File.ReadAllBytes(metadataPath);
                            }

                            var screenshotPath = Path.Combine(Path.GetDirectoryName(savePath), "screenshot.png");
                            if (File.Exists(screenshotPath))
                            {
                                ActiveSaveFile.ImageData = File.ReadAllBytes(screenshotPath);
                            }

                            this.InvokeIfRequired(() => { saveChangesButton.Enabled = true; });
                            break;
                        case EFileReadErrorCodes.NoCSav:
                            MessageBox.Show("Failed to parse save file: File contains invalid data");
                            return;
                        case EFileReadErrorCodes.UnsupportedVersion:
                            fs.Position = 0;
                            reader.ReadFileInfo(out var info);

                            MessageBox.Show(
                                $"Failed to parse save file: Game version {info.Value.GameVersion} is not supported!");
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });

                filePathLabel.Text = Path.GetFileName(Path.GetDirectoryName(savePath));

                GC.Collect();
            }
            catch (Exception e)
            {
                try
                {
                    var message = e.Message;
                    if (message == "CEnum \"gamedataStatType.ItemTierQuality\" could not be found!")
                    {
                        message += Environment.NewLine + "Loading of modded saves with this data is currently not supported! See \"Known Issues\" on Nexus mods";
                    }

                    await File.WriteAllTextAsync("error.txt", message + Environment.NewLine + e.StackTrace);
                    MessageBox.Show("Failed to parse save file:" + Environment.NewLine + 
                                    message + Environment.NewLine +
                                    " An error.txt file has been generated with additional information.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to parse save file: " + Environment.NewLine + 
                                    e.Message + Environment.NewLine + Environment.NewLine +
                                    "Stack Trace:" + Environment.NewLine + 
                                    e.StackTrace);
                }
            }

            SetStatus("Idle");

            openSaveButton.Enabled = true;

            if (status == EFileReadErrorCodes.NoError)
            {
                pnl_Content.Enabled = true;
                IsLoaded = true;
            }
        }

        internal void SetStatus(string message)
        {
            this.InvokeIfRequired(() => { tssl_Status.Text = message; });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private async void saveChangesButton_Click(object sender, EventArgs e)
        {
            var initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077";

            var saveWindow = new FolderBrowserDialog();
            saveWindow.InitialDirectory = initialDirectory;
            if (saveWindow.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var fileDirectory = saveWindow.SelectedPath;

            if (fileDirectory == initialDirectory)
            {
                MessageBox.Show("Saving into the root folder isn't allowed");
                return;
            }

            pnl_Content.Enabled = false;
            openSaveButton.Enabled = false;
            saveChangesButton.Enabled = false;

            var fileName = Path.Combine(fileDirectory, "sav.dat");

            if (File.Exists(fileName) && !File.Exists(Path.Combine(fileDirectory, "sav.old")))
            {
                File.Copy(fileName, Path.Combine(fileDirectory, "sav.old"));
            }

            SetStatus("Saving...");
            _isSaving = true;

            try
            {
                using var ms = new MemoryStream();

                await Task.Run(() =>
                {
                    using var writer = new CyberpunkSaveWriter(ms, Encoding.UTF8, true);
                    writer.WriteFile(ActiveSaveFile.SaveFile, true);
                });

                await using (var fs = File.Open(fileName, FileMode.Create))
                {
                    ms.Position = 0;
                    await ms.CopyToAsync(fs);
                }

                var metadataPath = Path.Combine(fileDirectory, "metadata.9.json");
                if (!File.Exists(metadataPath))
                {
                    if (ActiveSaveFile.Metadata != null)
                    {
                        await File.WriteAllBytesAsync(metadataPath, ActiveSaveFile.Metadata);
                    }
                    else
                    {
                        MessageBox.Show("Error while saving file: metadata.9.json could not be found.");
                    }
                }

                var screenshotPath = Path.Combine(fileDirectory, "screenshot.png");
                if (!File.Exists(screenshotPath))
                {
                    if (ActiveSaveFile.ImageData != null)
                    {
                        await File.WriteAllBytesAsync(screenshotPath, ActiveSaveFile.ImageData);
                    }
                    else
                    {
                        MessageBox.Show("Error while saving file: screenshot.png could not be found.");
                    }
                }
            }
            catch (Exception err)
            {
                try
                {
                    await File.WriteAllTextAsync("error.txt", err.Message + '\n' + err.TargetSite + '\n' + err.StackTrace);
                    MessageBox.Show("Failed to save changes: " + err.Message + " An error.txt file has been generated with additional information.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to save changes: " + err.Message + err.StackTrace);
                }
            }

            pnl_Content.Enabled = true;
            openSaveButton.Enabled = true;
            saveChangesButton.Enabled = true;

            _isSaving = false;
            SetStatus("File saved.");

            GC.Collect();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isSaving && MessageBox.Show("Saving in progress. Closing now will lead to corrupted saves.\r\nAre you sure you want to exit?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}