using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;

namespace CP2077SaveEditor.Views.Controls
{
    /// <summary>
    /// Lists the saves in the game's save folder with their screenshot and metadata, double click opens one.
    /// </summary>
    public class SaveBrowserControl : UserControl, IGameControl
    {
        private static readonly Size s_thumbnailSize = new(160, 90);

        private readonly MainForm _parentForm;

        private readonly TextBox _search;
        private readonly CheckBox _showAutoSaves;
        private readonly ListView _list;
        private readonly ImageList _thumbnails;
        private readonly Label _emptyLabel;

        private List<SaveEntry> _saves = new();
        private int _refreshVersion;

        private record SaveEntry(string Folder, string SavPath, DateTime Modified, string Name, string Details, string Version, bool IsAutoSave);

        public SaveBrowserControl(MainForm parentForm)
        {
            _parentForm = parentForm;

            _search = new TextBox { PlaceholderText = "Search saves", Width = 240, Location = new Point(10, 8), BorderStyle = BorderStyle.FixedSingle };
            _search.TextChanged += (_, _) => FillList();

            _showAutoSaves = new CheckBox { Text = "Show autosaves", AutoSize = true, Checked = true, Location = new Point(262, 9) };
            _showAutoSaves.CheckedChanged += (_, _) => FillList();

            var refreshButton = new Button { Text = "Refresh", Size = new Size(92, 26), Location = new Point(392, 5) };
            refreshButton.Click += (_, _) => RefreshSaves();

            var top = new Panel { Dock = DockStyle.Top, Height = 38 };
            top.Paint += (_, e) =>
            {
                if (!Theme.IsDark)
                {
                    return;
                }
                using var bottomPen = new Pen(Theme.NeonRed, 1f);
                using var yellowPen = new Pen(Theme.Accent, 2f);
                e.Graphics.DrawLine(bottomPen, 0, top.Height - 1, top.Width, top.Height - 1);
                e.Graphics.DrawLine(yellowPen, 10, top.Height - 1, 140, top.Height - 1);

                if (top.Width > 650)
                {
                    using var hudFont = new Font(Theme.MonoFontFamily, 8f, FontStyle.Bold);
                    TextRenderer.DrawText(e.Graphics, "// NIGHT CITY DATABANK :: DOUBLE-CLICK TO JACK IN", hudFont,
                        new Rectangle(refreshButton.Right + 12, 0, top.Width - refreshButton.Right - 22, top.Height - 2), Theme.Cyan,
                        TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
            };
            top.Resize += (_, _) => top.Invalidate();

            // Position the row from the real control sizes, fixed pixel positions overlap on scaled displays
            top.Layout += (_, _) =>
            {
                var s = top.DeviceDpi / 96f;
                var gap = (int)(12 * s);
                _search.Location = new Point((int)(10 * s), (top.Height - _search.Height) / 2);
                _showAutoSaves.Location = new Point(_search.Right + gap, (top.Height - _showAutoSaves.Height) / 2);
                refreshButton.Size = new Size((int)(92 * s), (int)(26 * s));
                refreshButton.Location = new Point(_showAutoSaves.Right + gap, (top.Height - refreshButton.Height) / 2);
            };
            top.Controls.AddRange(new Control[] { _search, _showAutoSaves, refreshButton });

            _thumbnails = new ImageList { ImageSize = s_thumbnailSize, ColorDepth = ColorDepth.Depth32Bit };

            _list = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Tile,
                TileSize = new Size(400, 100),
                LargeImageList = _thumbnails,
                MultiSelect = false,
                BorderStyle = BorderStyle.None
            };
            // Tile view shows the item text plus one line per extra column
            _list.Columns.Add("Name");
            _list.Columns.Add("Details");
            _list.Columns.Add("Version");
            _list.ItemActivate += (_, _) => OpenSelected();

            _emptyLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            Controls.Add(_list);
            Controls.Add(_emptyLabel);
            Controls.Add(top);

            RefreshSaves();
        }

        public string GameControlName => "Saves";
        public string Icon => "";

        public void RefreshSaves()
        {
            _saves = ReadSaves();
            FillList();
            LoadThumbnailsAsync();
        }

        private static List<SaveEntry> ReadSaves()
        {
            var saves = new List<SaveEntry>();
            if (!Directory.Exists(MainForm.SavesRootDirectory))
            {
                return saves;
            }

            foreach (var folder in Directory.GetDirectories(MainForm.SavesRootDirectory))
            {
                var savPath = Path.Combine(folder, "sav.dat");
                if (!File.Exists(savPath))
                {
                    continue;
                }

                var folderName = Path.GetFileName(folder);
                saves.Add(ReadMetadata(folder, savPath) ?? new SaveEntry(folder, savPath, File.GetLastWriteTime(savPath), folderName, "", "",
                    folderName.StartsWith("AutoSave", StringComparison.OrdinalIgnoreCase)));
            }

            return saves.OrderByDescending(x => x.Modified).ToList();
        }

        private static SaveEntry ReadMetadata(string folder, string savPath)
        {
            try
            {
                var metadataPath = Path.Combine(folder, "metadata.9.json");
                if (!File.Exists(metadataPath))
                {
                    return null;
                }

                using var document = JsonDocument.Parse(File.ReadAllText(metadataPath));
                var metadata = document.RootElement.GetProperty("Data").GetProperty("metadata");

                string GetString(string name) => metadata.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : "";
                double GetNumber(string name) => metadata.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetDouble() : 0;
                bool GetBool(string name) => metadata.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.True;

                var playTime = TimeSpan.FromSeconds(GetNumber("playthroughTime"));
                var details = $"Level {GetNumber("level"):0} · {GetString("lifePath")} · {GetString("bodyGender")} · {(int)playTime.TotalHours}h{playTime.Minutes:00}m played";
                var version = $"Patch {GetString("buildPatch")} · {File.GetLastWriteTime(savPath).ToString("g", CultureInfo.CurrentCulture)}" +
                              (GetBool("isModded") ? " · modded" : "");

                return new SaveEntry(folder, savPath, File.GetLastWriteTime(savPath), Path.GetFileName(folder), details, version,
                    Path.GetFileName(folder).StartsWith("AutoSave", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                // A broken metadata file shouldn't hide the save
                Logger.Error($"Failed to read the metadata of {folder}", e);
                return null;
            }
        }

        private void FillList()
        {
            var terms = _search.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var visible = _saves
                .Where(x => _showAutoSaves.Checked || !x.IsAutoSave)
                .Where(x => terms.All(t => (x.Name + " " + x.Details).Contains(t, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var save in visible)
            {
                _list.Items.Add(new ListViewItem(new[] { save.Name, save.Details, save.Version })
                {
                    Tag = save,
                    ImageKey = save.Folder
                });
            }
            _list.EndUpdate();

            _emptyLabel.Text = _saves.Count == 0
                ? $"No saves found in{Environment.NewLine}{MainForm.SavesRootDirectory}{Environment.NewLine}{Environment.NewLine}Use \"Load Save\" to open one from somewhere else."
                : "No saves match the search.";
            _emptyLabel.Visible = visible.Count == 0;
            _list.Visible = visible.Count > 0;
        }

        private async void LoadThumbnailsAsync()
        {
            // A newer refresh makes this one obsolete
            var version = ++_refreshVersion;
            _thumbnails.Images.Clear();

            foreach (var save in _saves)
            {
                var thumbnail = await Task.Run(() => CreateThumbnail(Path.Combine(save.Folder, "screenshot.png")));
                if (version != _refreshVersion || IsDisposed)
                {
                    thumbnail?.Dispose();
                    return;
                }

                if (thumbnail != null)
                {
                    _thumbnails.Images.Add(save.Folder, thumbnail);
                }
            }

            _list.Invalidate();
        }

        private static Bitmap CreateThumbnail(string screenshotPath)
        {
            try
            {
                if (!File.Exists(screenshotPath))
                {
                    return null;
                }

                using var image = Image.FromFile(screenshotPath);
                var w = s_thumbnailSize.Width;
                var h = s_thumbnailSize.Height;
                var thumbnail = new Bitmap(w, h);
                using var graphics = Graphics.FromImage(thumbnail);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, w, h);

                if (Theme.IsDark)
                {
                    using var borderPen = new Pen(Theme.NeonRed, 1f);
                    using var cyanPen = new Pen(Theme.Cyan, 2f);
                    using var yellowBrush = new SolidBrush(Theme.Accent);
                    graphics.DrawRectangle(borderPen, 0, 0, w - 1, h - 1);
                    graphics.FillRectangle(yellowBrush, 0, 0, 3, h);
                    graphics.DrawLine(cyanPen, w - 12, 0, w - 1, 0);
                    graphics.DrawLine(cyanPen, w - 1, 0, w - 1, 12);
                    graphics.DrawLine(cyanPen, 0, h - 1, 12, h - 1);
                }
                return thumbnail;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void OpenSelected()
        {
            if (_list.SelectedItems.Count == 1 && _list.SelectedItems[0].Tag is SaveEntry save)
            {
                _parentForm.OpenSave(save.SavPath);
            }
        }
    }
}
