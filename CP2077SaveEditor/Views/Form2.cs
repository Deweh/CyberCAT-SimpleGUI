using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views
{
    public partial class Form2 : Form, INotifyPropertyChanged
    {
        internal static TweakDBStringHelper TweakDbStringHelper;
        internal HashService HashService;

        internal bool IsLoaded;

        private SaveFileHelper _activeSaveFile;
        private int _saveType = 0;

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

                TweakDBIDPool.ResolveHashHandler += TweakDbStringHelper.GetString;
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
                            ActiveSaveFile = new SaveFileHelper() { SaveFile = save };
                            this.InvokeIfRequired(() => { saveChangesButton.Enabled = true; });
                            break;
                        case EFileReadErrorCodes.NoCSav:
                            MessageBox.Show("Failed to parse save file: File contains invalid data");
                            return;
                        case EFileReadErrorCodes.UnsupportedVersion:
                            fs.Position = 0;
                            reader.ReadFileInfo(out var info);

                            MessageBox.Show(
                                $"Failed to parse save file: Game version {info.Value.gameVersion} is not supported!");
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
                    await File.WriteAllTextAsync("error.txt", e.Message + Environment.NewLine + e.StackTrace);
                    MessageBox.Show("Failed to parse save file: " + e.Message +
                                    " An error.txt file has been generated with additional information.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to parse save file: " + e.Message + " \n\n Stack Trace: \n" + e.StackTrace);
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
            var saveWindow = new SaveFileDialog();
            saveWindow.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077\\";
            saveWindow.Filter = "Cyberpunk 2077 Save File|*.dat";
            if (saveWindow.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            pnl_Content.Enabled = false;
            openSaveButton.Enabled = false;
            swapSaveType.Enabled = false;
            saveChangesButton.Enabled = false;

            if (File.Exists(saveWindow.FileName) && !File.Exists(Path.GetDirectoryName(saveWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveWindow.FileName) + ".old"))
            {
                File.Copy(saveWindow.FileName, Path.GetDirectoryName(saveWindow.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveWindow.FileName) + ".old");
            }

            SetStatus("Saving...");

            try
            {
                await Task.Run(() =>
                {
                    using var fs = File.Open(saveWindow.FileName, FileMode.Create);
                    using var writer = new CyberpunkSaveWriter(fs);
                    writer.WriteFile(ActiveSaveFile.SaveFile, _saveType == 0);
                });
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
            swapSaveType.Enabled = true;
            saveChangesButton.Enabled = true;

            SetStatus("File saved.");

            GC.Collect();
        }

        private void swapSaveType_Click(object sender, EventArgs e)
        {
            if (_saveType == 0)
            {
                _saveType = 1;
                swapSaveType.Text = "Save Type: PS4";
            } else {
                _saveType = 0;
                swapSaveType.Text = "Save Type: PC";
            }
        }
    }
}