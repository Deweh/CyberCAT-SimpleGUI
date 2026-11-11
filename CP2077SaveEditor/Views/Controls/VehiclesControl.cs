using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using CP2077SaveEditor.Extensions;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class VehiclesControl : UserControl, IGameControl
    {
        private readonly MainForm _parentForm;

        private readonly List<string> _vehicles = JsonSerializer.Deserialize<List<string>>(CP2077SaveEditor.Properties.Resources.Vehicles);

        private vehicleGarageComponentPS _garagePS;

        public VehiclesControl(MainForm parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;
        }

        public string GameControlName => "Unlocked Vehicles";
        public string Icon => "";

        private void OnParentFormPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_parentForm.ActiveSaveFile))
            {
                if (_parentForm.ActiveSaveFile != null)
                {
                    this.InvokeIfRequired(Init);
                }
            }
        }

        private void Init()
        {
            var ps = _parentForm.ActiveSaveFile.GetPSDataContainer();
            _garagePS = (vehicleGarageComponentPS)ps.Entries.FirstOrDefault(x => x.Data is vehicleGarageComponentPS)?.Data;

            if (_garagePS != null)
            {
                _garagePS.UnlockedVehicleArray ??= new();
            }

            vehiclesListView.Enabled = _garagePS != null;
            pnl_Top.Enabled = _garagePS != null;

            RefreshList();
        }

        private void RefreshList()
        {
            var listItems = new List<ListViewItem>();

            if (_garagePS == null)
            {
                listItems.Add(new ListViewItem("No vehicle data found. Try unlocking at least 1 vehicle in-game first."));
            }
            else
            {
                var unlockedVehicles = _garagePS.UnlockedVehicleArray.Select(x => x.VehicleID.RecordID.ResolvedText).ToHashSet();

                foreach (var vehicle in GetVisibleVehicles())
                {
                    listItems.Add(new ListViewItem(new[] { vehicle, ResourceHelper.GetDisplayName(vehicle) ?? "" })
                    {
                        Checked = unlockedVehicles.Contains(vehicle)
                    });
                }
            }

            vehiclesListView.SetVirtualItems(listItems);
        }

        private IEnumerable<string> GetVisibleVehicles()
        {
            var search = txt_Search.Text.Trim();
            if (search == "")
            {
                return _vehicles;
            }

            return _vehicles.Where(x => x.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                        (ResourceHelper.GetDisplayName(x)?.Contains(search, StringComparison.CurrentCultureIgnoreCase) ?? false));
        }

        private void SetUnlocked(string vehicle, bool unlocked)
        {
            var list = _garagePS.UnlockedVehicleArray;
            ChangeTracker.MarkChanged();
            var existing = list.FirstOrDefault(x => x.VehicleID.RecordID.ResolvedText == vehicle);

            if (unlocked && existing == null)
            {
                list.Add(new vehicleUnlockedVehicle
                {
                    VehicleID = new vehicleGarageVehicleID { RecordID = vehicle }
                });
            }
            else if (!unlocked && existing != null)
            {
                list.Remove(existing);
            }
        }

        private void vehiclesListView_DoubleClick(object sender, EventArgs e)
        {
            if (_garagePS == null)
            {
                return;
            }

            foreach (var selectedItem in vehiclesListView.SelectedVirtualItems())
            {
                selectedItem.Checked = !selectedItem.Checked;
                SetUnlocked(selectedItem.Text, selectedItem.Checked);
            }

            vehiclesListView.Invalidate();
        }

        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            if (_garagePS != null)
            {
                RefreshList();
            }
        }

        private void btn_UnlockAll_Click(object sender, EventArgs e)
        {
            SetAllVisible(true);
        }

        private void btn_LockAll_Click(object sender, EventArgs e)
        {
            SetAllVisible(false);
        }

        // Acts on the vehicles matching the current search, so a filtered subset can be (un)locked at once
        private void SetAllVisible(bool unlocked)
        {
            var vehicles = GetVisibleVehicles().ToList();
            if (vehicles.Count == 0)
            {
                return;
            }

            var action = unlocked ? "Unlock" : "Lock";
            var scope = txt_Search.Text.Trim() == "" ? "all" : "the";
            if (MessageBox.Show($"{action} {scope} {vehicles.Count} listed vehicle(s)?", $"{action} vehicles", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            foreach (var vehicle in vehicles)
            {
                SetUnlocked(vehicle, unlocked);
            }

            RefreshList();
        }
    }
}
