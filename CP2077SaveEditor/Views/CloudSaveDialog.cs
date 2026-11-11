using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CP2077SaveEditor.Cloud;

namespace CP2077SaveEditor.Views
{
    /// <summary>
    /// Download saves from / upload the current save to the CD PROJEKT RED cross-save cloud (PS5, Xbox, PC).
    /// Signing in happens in the browser; the access token stays in memory until the dialog is closed.
    /// </summary>
    internal sealed class CloudSaveDialog : Form
    {
        private readonly Func<Task<(byte[] Save, byte[] Metadata)>> _getCurrentSave;
        private readonly Action<string> _openSave;
        private readonly CdprCloudClient _client;
        private readonly CancellationTokenSource _cts = new();

        private readonly Button _signIn = new() { Text = "Sign in", AutoSize = true, MinimumSize = new Size(110, 34), Margin = new Padding(0, 0, 8, 0) };
        private readonly Button _refresh = new() { Text = "Refresh", AutoSize = true, MinimumSize = new Size(100, 34), Margin = new Padding(0, 0, 12, 0), Enabled = false };
        private readonly Label _info = new() { AutoSize = true, Anchor = AnchorStyles.Left, MaximumSize = new Size(560, 0) };
        private readonly ListView _slots = new()
        {
            Dock = DockStyle.Fill, View = View.Details,
            FullRowSelect = true, MultiSelect = false, HideSelection = false,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point),
        };
        private readonly Button _download = new() { Text = "Download and open", AutoSize = true, MinimumSize = new Size(190, 36), Margin = new Padding(0, 0, 8, 0), Enabled = false };
        private readonly Button _upload = new() { Text = "Upload open save to slot", AutoSize = true, MinimumSize = new Size(230, 36), Enabled = false };
        private readonly Label _note = new()
        {
            AutoSize = true, MaximumSize = new Size(700, 0), Margin = new Padding(0, 10, 0, 0),
            ForeColor = SystemColors.GrayText,
            Text = "Uses the unofficial CD PROJEKT RED cross-save API, which may change without notice. " +
                   "Uploading overwrites the cloud slot; the previous version is backed up locally first.",
        };

        public CloudSaveDialog(CdprCloudClient client, Func<Task<(byte[] Save, byte[] Metadata)>> getCurrentSave, Action<string> openSave)
        {
            _client = client;
            _getCurrentSave = getCurrentSave;
            _openSave = openSave;

            Text = "Cloud saves (CD PROJEKT RED cross-save)";
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(740, 470);
            MinimumSize = new Size(620, 400);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Padding = new Padding(14);

            _slots.Columns.Add("Slot", 230);
            _slots.Columns.Add("Size", 90);
            _slots.Columns.Add("Last modified", 220);
            _info.Text = "Not signed in.";

            // Docked layout so nothing is clipped, whatever the display scaling is
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false, Padding = new Padding(0, 0, 0, 10) };
            top.Controls.AddRange(new Control[] { _signIn, _refresh, _info });
            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(0, 10, 0, 0) };
            var buttons = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
            buttons.Controls.AddRange(new Control[] { _download, _upload });
            bottom.Controls.AddRange(new Control[] { buttons, _note });
            Controls.Add(_slots);
            Controls.Add(bottom);
            Controls.Add(top);

            Utils.Theme.Apply(this);

            _signIn.Click += async (_, _) => await SignInAsync();
            _refresh.Click += async (_, _) => await RefreshAsync();
            _download.Click += async (_, _) => await DownloadAsync();
            _upload.Click += async (_, _) => await UploadAsync();
            _slots.SelectedIndexChanged += (_, _) => UpdateButtons();
            _slots.Resize += (_, _) => FitLastColumn();
            Shown += (_, _) => FitLastColumn();
            FormClosed += (_, _) =>
            {
                _cts.Cancel(); // the client belongs to the main window and keeps the sign-in
                _cts.Dispose();
            };
            Shown += async (_, _) =>
            {
                if (_client.IsSignedIn)
                {
                    _signIn.Enabled = false;
                    await RefreshAsync();
                }
            };
        }

        private void FitLastColumn()
        {
            var rest = _slots.ClientSize.Width - _slots.Columns[0].Width - _slots.Columns[1].Width;
            if (rest > 80)
            {
                _slots.Columns[2].Width = rest;
            }
        }

        private string SelectedSlot => _slots.SelectedItems.Count > 0 ? _slots.SelectedItems[0].Text : null;

        private void UpdateButtons()
        {
            var ready = _client.IsSignedIn;
            _refresh.Enabled = ready;
            _download.Enabled = ready && SelectedSlot != null;
            _upload.Enabled = ready && SelectedSlot != null && _getCurrentSave != null;
        }

        private void SetBusy(bool busy, string text)
        {
            _info.Text = text;
            UseWaitCursor = busy;
            if (busy)
            {
                _signIn.Enabled = _refresh.Enabled = _download.Enabled = _upload.Enabled = false;
            }
            else
            {
                _signIn.Enabled = !_client.IsSignedIn;
                UpdateButtons();
            }
        }

        private async Task SignInAsync()
        {
            SetBusy(true, "Waiting for sign-in in your browser...");
            try
            {
                await _client.SignInAsync(code =>
                {
                    Invoke(() => _info.Text = $"Approve in the browser. Code: {code.UserCode}");
                    Process.Start(new ProcessStartInfo(code.VerificationUrl) { UseShellExecute = true });
                }, _cts.Token);
                await RefreshAsync();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                SetBusy(false, "Sign-in failed: " + ex.Message);
            }
        }

        private async Task RefreshAsync()
        {
            SetBusy(true, "Loading cloud saves...");
            try
            {
                var list = await _client.ListSlotsAsync(_cts.Token);
                _slots.Items.Clear();
                foreach (var s in list)
                {
                    var modified = DateTimeOffset.TryParse(s.LastModified, out var when) ? when.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") : s.LastModified;
                    _slots.Items.Add(new ListViewItem(new[] { s.Name, $"{s.Bytes / 1024.0 / 1024.0:0.0} MB", modified }));
                }

                SetBusy(false, $"Signed in. {list.Count} cloud save(s).");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                SetBusy(false, "Could not load cloud saves: " + ex.Message);
            }
        }

        private static string GetSafeSlotFolderName(string slot)
        {
            var safe = Path.GetFileName(slot ?? string.Empty);
            foreach (var invalid in Path.GetInvalidFileNameChars())
            {
                safe = safe.Replace(invalid, '_');
            }
            return string.IsNullOrWhiteSpace(safe) ? "slot" : safe;
        }

        private async Task DownloadAsync()
        {
            var slot = SelectedSlot;
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CyberCAT", "cloud-downloads", GetSafeSlotFolderName(slot));
            SetBusy(true, $"Downloading {slot}...");
            try
            {
                await _client.DownloadSlotAsync(slot, dir, _cts.Token);
                SetBusy(false, $"Downloaded {slot}.");
                _openSave(Path.Combine(dir, "sav.dat"));
                Close();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                SetBusy(false, "Download failed: " + ex.Message);
            }
        }

        private async Task UploadAsync()
        {
            var slot = SelectedSlot;
            var confirm = MessageBox.Show(this,
                $"Overwrite the cloud slot \"{slot}\" with the save that is currently open?{Environment.NewLine}{Environment.NewLine}" +
                "The current cloud version is backed up locally first. Make sure you also have your own backup of your cloud saves.",
                "Upload to cloud", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (confirm != DialogResult.OK)
            {
                return;
            }

            var backup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CyberCAT", "cloud-backups", GetSafeSlotFolderName(slot), DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            SetBusy(true, "Preparing save...");
            try
            {
                var data = await _getCurrentSave();
                SetBusy(true, $"Uploading to {slot}...");
                await _client.UploadSlotAsync(slot, data.Save, data.Metadata, backup, _cts.Token);
                SetBusy(false, $"Uploaded to {slot} and verified. Previous version backed up to {backup}");
                await RefreshAsync();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                SetBusy(false, "Upload failed: " + ex.Message);
            }
        }
    }
}
