using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.Views
{
    public partial class MainForm : Form, INotifyPropertyChanged
    {
        internal static TweakDBStringHelper TweakDbStringHelper;
        internal HashService HashService;

        internal bool IsLoaded;

        private SaveFileHelper _activeSaveFile;

        private bool _isSaving = false;
        private bool _isCheckingChanges = false;
        private bool _closeConfirmed = false;

        private const int MaxBackups = 5;

        private string _activeSavePath;
        private SaveBrowserControl _saveBrowser;
        private UserControl _firstEditorPage;
        private UserControl _currentPage;
        private readonly Dictionary<UserControl, ModernButton> _pageButtons = new();
        private readonly string _baseTitle;
        private string _saveName;
        private bool _isReadOnly;

        // Saves from newer game versions can contain data the bundled WolvenKit doesn't know about, which would be dropped on write
        private const ulong MaxSupportedGameVersion = (ulong)gameGameVersion.Current;
        private byte[] _savedStateHash;

        // Undo: a state is recorded once the edits pause, see CaptureUndoPoint
        private readonly UndoHistory _undoHistory = new(AppConfig.UndoSteps);
        private readonly Timer _undoTimer = new() { Interval = 1500 };
        private bool _undoPending;
        private bool _isRestoring;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool IsWindowEnabled(IntPtr hWnd);

        public MainForm()
        {
            InitializeComponent();
            _baseTitle = Text;
            tssl_Info.Text = UpdateChecker.CurrentVersionText;
            ChangeTracker.Changed += (_, _) => this.InvokeIfRequired(UpdateTitle);
            ChangeTracker.Edited += (_, _) => this.InvokeIfRequired(OnEdited);
            _undoTimer.Tick += (_, _) => CaptureUndoPoint();

            // Before the pages are created, some of them read it in their constructor
            Global.IsDebug = AppConfig.EnableWipFeatures;

            CompressionSettings.Get().UseOodle = false;
            ModManager.LoadTypes();

            _saveBrowser = new SaveBrowserControl(this);
            RegisterControl(_saveBrowser, true);
            _firstEditorPage = new PlayerStatsControl(this);
            RegisterControl(_firstEditorPage);
            RegisterControl(new PerksControl(this));
            RegisterControl(new CyberwareControl(this));
            RegisterControl(new AppearanceControl(this));
            RegisterControl(new InventoryControl(this));
            RegisterControl(new VehiclesControl(this));
            RegisterControl(new QuestFactsControl(this));
            RegisterControl(new ExtrasControl(this));

            Theme.Apply(this);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            SetStatus("Init...");

            await Init();

            openSaveButton.Enabled = true;

            SetStatus("Idle");

            if (AppConfig.CheckForUpdates)
            {
                await ShowUpdateIfAvailable();
            }
        }

        private async Task ShowUpdateIfAvailable()
        {
            var newerRelease = await UpdateChecker.GetNewerReleaseAsync();
            if (newerRelease == null)
            {
                return;
            }

            tssl_Info.Text = $"{UpdateChecker.CurrentVersionText} - update available: {newerRelease}";
            tssl_Info.IsLink = true;
            tssl_Info.ToolTipText = "Open the download page";
            tssl_Info.Click += (_, _) => Process.Start(new ProcessStartInfo(UpdateChecker.ReleasesPage) { UseShellExecute = true });
        }

        // "*" after the save name while there are edits that weren't saved yet
        private void UpdateTitle()
        {
            if (_saveName == null)
            {
                return;
            }

            var name = ChangeTracker.HasChanges ? _saveName + " *" : _saveName;
            filePathLabel.Text = name;
            Text = $"{_baseTitle} - {name}";
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
            Theme.Apply(userControl);

            var button = sm_Menu.AddButton(gc.GameControlName, gc.Icon, (_, _) => { SwitchPanel(userControl); });
            _pageButtons[userControl] = button;
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
                _currentPage = userControl;

                if (_pageButtons.TryGetValue(userControl, out var button))
                {
                    sm_Menu.SelectButton(button);
                }

                // The save browser is the only page that works without a loaded save
                pnl_Content.Enabled = IsLoaded || userControl == _saveBrowser;
            });
        }

        /// <summary>
        /// Opens a save after asking about unsaved changes, used by the save browser.
        /// </summary>
        internal async void OpenSave(string savePath)
        {
            if (!await ConfirmDiscardChanges())
            {
                return;
            }

            LoadSave(savePath);
        }

        #endregion Controls

        private async void openSaveButton_Click(object sender, EventArgs e)
        {
            if (!await ConfirmDiscardChanges())
            {
                return;
            }

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
            quickSaveButton.Enabled = false;

            _activeSavePath = null;
            _savedStateHash = null;
            ChangeTracker.Reset();
            _isReadOnly = false;
            _undoHistory.Reset(null);
            _undoPending = false;
            _undoTimer.Stop();

            SetStatus("Loading save...");
            tspb_Busy.Visible = true;

            var status = EFileReadErrorCodes.NoCSav;
            ulong? gameVersion = null;

            try
            {
                await Task.Run(() =>
                {
                    using var fs = File.Open(savePath, FileMode.Open);
                    using var reader = new CyberpunkSaveReader(fs);

                    // Read the header first, so the version is known even if parsing the rest fails
                    if (reader.ReadFileInfo(out var saveInfo) == EFileReadErrorCodes.NoError)
                    {
                        gameVersion = saveInfo.Value.GameVersion;
                    }
                    fs.Position = 0;

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

                            if (gameVersion / 100 > MaxSupportedGameVersion / 100) // Compare major.minor only, so hotfixes (e.g. 2.31) count as their base patch
                            {
                                _isReadOnly = true;
                                MessageBox.Show(
                                    $"This save is from game version {FormatGameVersion(gameVersion.Value)}, but this editor only supports up to {FormatGameVersion(MaxSupportedGameVersion)}.{Environment.NewLine}{Environment.NewLine}" +
                                    "Saving it could silently drop data the editor doesn't understand (e.g. item stats), so the save was opened in read-only mode.",
                                    "Unsupported game version", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }

                            // Taken after the controls have initialized, since some of them normalize save data on load
                            var initialState = SerializeSave();
                            _savedStateHash = SHA256.HashData(initialState);
                            ChangeTracker.Reset();
                            _undoHistory.Reset(initialState);
                            _activeSavePath = savePath;

                            this.InvokeIfRequired(() =>
                            {
                                saveChangesButton.Enabled = true;
                                quickSaveButton.Enabled = true;
                            });
                            break;
                        case EFileReadErrorCodes.NoCSav:
                            MessageBox.Show("Failed to parse save file: File contains invalid data");
                            return;
                        case EFileReadErrorCodes.UnsupportedVersion:
                            MessageBox.Show(
                                $"Failed to parse save file: Game version {(gameVersion.HasValue ? FormatGameVersion(gameVersion.Value) : "unknown")} is not supported!{Environment.NewLine}" +
                                "Only saves from game version 2.0 or newer can be edited.");
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });

                _saveName = Path.GetFileName(Path.GetDirectoryName(savePath)) + (_isReadOnly ? " (read-only)" : "");
                UpdateTitle();

                GC.Collect();
            }
            catch (Exception e)
            {
                // Don't leave the editor enabled on a half-loaded save
                status = EFileReadErrorCodes.NoCSav;

                var versionInfo = gameVersion.HasValue ? FormatGameVersion(gameVersion.Value) : "unknown";
                var logFile = Logger.Error(
                    $"Failed to load save: {savePath}{Environment.NewLine}" +
                    $"Game version: {versionInfo} (editor supports up to {FormatGameVersion(MaxSupportedGameVersion)})", e);

                MessageBox.Show("Failed to parse save file:" + Environment.NewLine +
                                e.Message + GetLoadErrorHint(e, gameVersion) + Environment.NewLine + Environment.NewLine +
                                "Details were written to:" + Environment.NewLine + logFile);
            }

            SetStatus("Idle");
            tspb_Busy.Visible = false;

            openSaveButton.Enabled = true;

            if (status == EFileReadErrorCodes.NoError)
            {
                pnl_Content.Enabled = true;
                IsLoaded = true;

                if (_currentPage == _saveBrowser)
                {
                    SwitchPanel(_firstEditorPage);
                }
            }
            else if (_currentPage == _saveBrowser)
            {
                // Let the user pick another save
                pnl_Content.Enabled = true;
            }
        }

        private static string GetLoadErrorHint(Exception e, ulong? gameVersion)
        {
            if (gameVersion / 100 > MaxSupportedGameVersion / 100)
            {
                return $"{Environment.NewLine}{Environment.NewLine}This save is from game version {FormatGameVersion(gameVersion.Value)}, which is newer than this editor supports ({FormatGameVersion(MaxSupportedGameVersion)}). " +
                       "Check for an updated version of the editor.";
            }

            // WolvenKit reports unknown enums/classes like: CEnum "gamedataStatType.ItemTierQuality" could not be found!
            if (e.Message.Contains("could not be found", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Environment.NewLine}{Environment.NewLine}The save contains data the editor doesn't know about, usually added by a game mod. " +
                       "Loading saves with this data is currently not supported. See \"Known Issues\" on Nexus mods.";
            }

            return "";
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

        internal static string SavesRootDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Saved Games\\CD Projekt Red\\Cyberpunk 2077";

        private async void saveChangesButton_Click(object sender, EventArgs e)
        {
            var initialDirectory = SavesRootDirectory;

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

            await SaveToDirectory(fileDirectory);
        }

        private async void quickSaveButton_Click(object sender, EventArgs e)
        {
            await QuickSave();
        }

        private async void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                await QuickSave();
            }
            else if (e.Control && (e.KeyCode == Keys.Z || e.KeyCode == Keys.Y) && !IsTextInputFocused())
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                var redo = e.KeyCode == Keys.Y || e.Shift;
                await UndoOrRedo(redo);
            }
        }

        // Text boxes and number fields have their own Ctrl+Z
        private bool IsTextInputFocused()
        {
            Control control = this;
            while (control is ContainerControl { ActiveControl: { } active })
            {
                control = active;
            }

            return control is TextBoxBase or NumericUpDown || control?.Parent is NumericUpDown;
        }

        private void OnEdited()
        {
            if (!IsLoaded || _isRestoring || !_undoHistory.IsEnabled || _isReadOnly)
            {
                return;
            }

            // Edits in quick succession become one undo step
            _undoPending = true;
            _undoTimer.Stop();
            _undoTimer.Start();
        }

        /// <summary>
        /// Records the current state as an undo point. Serializing takes about a second, so it runs on the UI thread:
        /// nothing can change the save meanwhile, and clicks wait until it's done.
        /// </summary>
        private bool CaptureUndoPoint()
        {
            if (!_undoPending)
            {
                _undoTimer.Stop();
                return true;
            }

            // Not while a dialog or message box is open (an action may be half done) or another operation runs; the timer tries again
            if (!IsLoaded || _isSaving || _isCheckingChanges || _isRestoring || !IsWindowEnabled(Handle))
            {
                return false;
            }

            _undoTimer.Stop();
            SetStatus("Creating undo point...");
            statusStrip1.Update();
            Cursor = Cursors.WaitCursor;
            try
            {
                _undoHistory.Push(SerializeSave());
                _undoPending = false;
                SetStatus("Idle (Ctrl+Z to undo)");
            }
            catch (Exception e)
            {
                // Undo is a convenience; the save itself is untouched
                Logger.Error("Unable to create an undo point", e);
                _undoPending = false;
                SetStatus("Idle");
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            return true;
        }

        private async Task UndoOrRedo(bool redo)
        {
            if (!IsLoaded || _isSaving || _isCheckingChanges || _isRestoring || !_undoHistory.IsEnabled || _isReadOnly)
            {
                return;
            }

            // Edits not recorded yet become a step first, so they can be redone
            if (!CaptureUndoPoint())
            {
                return;
            }

            var state = redo ? _undoHistory.Redo() : _undoHistory.Undo();
            if (state == null)
            {
                SetStatus(redo ? "Nothing to redo" : "Nothing to undo");
                return;
            }

            await RestoreState(state, redo ? "Redoing..." : "Undoing...");
        }

        /// <summary>
        /// Replaces the loaded save with a recorded state. The pages reinitialize as they do when a save is loaded.
        /// </summary>
        private async Task RestoreState(byte[] state, string status)
        {
            _isRestoring = true;
            SetUiEnabled(false);
            SetStatus(status);

            var previous = ActiveSaveFile;
            try
            {
                IsLoaded = false;
                await Task.Run(() =>
                {
                    using var stream = new MemoryStream(state);
                    using var reader = new CyberpunkSaveReader(stream);
                    if (reader.ReadFile(out var save) != EFileReadErrorCodes.NoError)
                    {
                        throw new InvalidDataException("The recorded state could not be read back");
                    }

                    ActiveSaveFile = new SaveFileHelper
                    {
                        SaveFile = save,
                        Metadata = previous.Metadata,
                        ImageData = previous.ImageData
                    };
                });
                IsLoaded = true;

                // Undoing back to the saved state clears the "*"
                ChangeTracker.Reset();
                if (!SHA256.HashData(state).AsSpan().SequenceEqual(_savedStateHash))
                {
                    ChangeTracker.MarkChanged();
                }

                SetStatus(_undoHistory.CanUndo ? "Idle (Ctrl+Z to undo, Ctrl+Y to redo)" : "Idle (Ctrl+Y to redo)");
            }
            catch (Exception e)
            {
                IsLoaded = previous != null;
                var logFile = Logger.Error("Undo/redo failed", e);
                MessageBox.Show($"Undo failed, the save was left as it was.{Environment.NewLine}{Environment.NewLine}Details were written to:{Environment.NewLine}{logFile}",
                    "Undo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Idle");
            }
            finally
            {
                _isRestoring = false;
                SetUiEnabled(true);
                GC.Collect();
            }
        }

        private async Task QuickSave()
        {
            if (!quickSaveButton.Enabled || _activeSavePath == null)
            {
                return;
            }

            await SaveToDirectory(Path.GetDirectoryName(_activeSavePath));
        }

        private async Task<bool> SaveToDirectory(string fileDirectory)
        {
            SetUiEnabled(false);

            var fileName = Path.Combine(fileDirectory, "sav.dat");
            var tempFileName = fileName + ".tmp";
            var success = false;

            SetStatus("Saving...");
            _isSaving = true;

            try
            {
                var data = await Task.Run(SerializeSave);

                // Write to a temporary file first, so a failed save never touches the original
                await File.WriteAllBytesAsync(tempFileName, data);

                SetStatus("Verifying...");
                await Task.Run(() => VerifySaveFile(tempFileName));

                if (File.Exists(fileName))
                {
                    if (!File.Exists(Path.Combine(fileDirectory, "sav.old")))
                    {
                        File.Copy(fileName, Path.Combine(fileDirectory, "sav.old"));
                    }

                    CreateBackup(fileName);
                }

                File.Move(tempFileName, fileName, true);

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

                _savedStateHash = SHA256.HashData(data);
                ChangeTracker.Reset();

                // The written data is the current state, a free undo point
                if (_undoPending)
                {
                    _undoHistory.Push(data);
                    _undoPending = false;
                    _undoTimer.Stop();
                }
                _activeSavePath = fileName;
                _saveName = Path.GetFileName(fileDirectory);
                UpdateTitle();

                success = true;

                _saveBrowser.RefreshSaves();
            }
            catch (Exception err)
            {
                try
                {
                    if (File.Exists(tempFileName))
                    {
                        File.Delete(tempFileName);
                    }
                }
                catch (Exception)
                {
                    // The temp file is harmless, it gets overwritten on the next save
                }

                var logFile = Logger.Error($"Failed to save: {fileName}", err);
                MessageBox.Show("Failed to save changes: " + err.Message + Environment.NewLine + Environment.NewLine +
                                "Details were written to:" + Environment.NewLine + logFile);
            }

            SetUiEnabled(true);

            _isSaving = false;
            SetStatus(success ? "File saved." : "Save failed.");

            GC.Collect();

            return success;
        }

        private byte[] SerializeSave() => ActiveSaveFile.Serialize();

        private static void VerifySaveFile(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new CyberpunkSaveReader(fs);

            var status = reader.ReadFile(out _);
            if (status != EFileReadErrorCodes.NoError)
            {
                throw new InvalidDataException($"The written save could not be read back ({status}). The original save was not modified.");
            }
        }

        private static void CreateBackup(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName)!;

            File.Copy(fileName, Path.Combine(directory, $"sav.{DateTime.Now:yyyyMMdd-HHmmss}.bak"), true);

            // The timestamp format sorts chronologically by name
            foreach (var oldBackup in Directory.GetFiles(directory, "sav.*.bak").OrderByDescending(x => x).Skip(MaxBackups))
            {
                File.Delete(oldBackup);
            }
        }

        private void SetUiEnabled(bool enabled)
        {
            // Everything that disables the UI is a long running operation
            tspb_Busy.Visible = !enabled;

            pnl_Content.Enabled = enabled;
            openSaveButton.Enabled = enabled;
            saveChangesButton.Enabled = enabled && !_isReadOnly;
            quickSaveButton.Enabled = enabled && !_isReadOnly && _activeSavePath != null;
        }

        private static string FormatGameVersion(ulong version)
        {
            // e.g. 2310 -> "2.31", 2500 -> "2.5"
            var patch = (version % 1000 / 10).ToString("00").TrimEnd('0');
            return patch.Length == 0 ? $"{version / 1000}.0" : $"{version / 1000}.{patch}";
        }

        /// <summary>
        /// Asks the user what to do with unsaved changes. Returns false if the pending action should be cancelled.
        /// </summary>
        private async Task<bool> ConfirmDiscardChanges()
        {
            if (!IsLoaded || _savedStateHash == null)
            {
                return true;
            }

            SetUiEnabled(false);
            SetStatus("Checking for unsaved changes...");
            _isCheckingChanges = true;

            byte[] currentHash;
            try
            {
                currentHash = await Task.Run(() => SHA256.HashData(SerializeSave()));
            }
            catch (Exception)
            {
                // If the save can't be serialized we can't tell, so assume it changed
                currentHash = null;
            }

            _isCheckingChanges = false;
            SetUiEnabled(true);
            SetStatus("Idle");

            if (currentHash != null && currentHash.AsSpan().SequenceEqual(_savedStateHash))
            {
                // E.g. an edit that was undone by hand
                ChangeTracker.Reset();
                return true;
            }

            var result = MessageBox.Show("There are unsaved changes. Do you want to save them first?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                return await SaveToDirectory(Path.GetDirectoryName(_activeSavePath));
            }

            return result == DialogResult.No;
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isSaving)
            {
                if (MessageBox.Show("Saving in progress. Closing now will lead to corrupted saves.\r\nAre you sure you want to exit?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                return;
            }

            if (_closeConfirmed || e.CloseReason == CloseReason.WindowsShutDown)
            {
                return;
            }

            e.Cancel = true;

            if (_isCheckingChanges)
            {
                return;
            }

            if (await ConfirmDiscardChanges())
            {
                _closeConfirmed = true;
                Close();
            }
        }
    }
}
