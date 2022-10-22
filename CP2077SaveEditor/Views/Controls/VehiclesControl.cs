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
        private readonly Form2 _parentForm;

        public VehiclesControl(Form2 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;
        }

        public string GameControlName => "Unlocked Vehicles";

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

        private void vehiclesListView_DoubleClick(object sender, EventArgs e)
        {
            var ps = _parentForm.ActiveSaveFile.GetPSDataContainer();
            var vehiclePS = (vehicleGarageComponentPS)ps.Entries.Where(x => x.Data is vehicleGarageComponentPS).FirstOrDefault().Data;
            var unlockedVehicles = vehiclePS.UnlockedVehicleArray.Select(x => x.VehicleID.RecordID.ResolvedText);

            foreach (var selectedItem in vehiclesListView.SelectedVirtualItems())
            {
                selectedItem.Checked = !selectedItem.Checked;

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
                        var list = vehiclePS.UnlockedVehicleArray;
                        for(int i = 0; i < list.Count; i++)
                        {
                            var unlocked = list[i];

                            if (unlocked.VehicleID.RecordID.ResolvedText == selectedItem.Text)
                            {
                                list.Remove(unlocked);
                                break;
                            }
                        }
                        foreach (var unlocked in vehiclePS.UnlockedVehicleArray)
                        {
                            
                        }
                    }
                }
            }

            vehiclesListView.Invalidate();
        }

        private void Init()
        {
            var listItems = new List<ListViewItem>();

            var ps = _parentForm.ActiveSaveFile.GetPSDataContainer();
            var vehiclePS = (vehicleGarageComponentPS)ps.Entries.FirstOrDefault(x => x.Data is vehicleGarageComponentPS)?.Data;
            var unlockedVehicles = new List<string>();

            var vehicles = JsonSerializer.Deserialize<List<string>>(CP2077SaveEditor.Properties.Resources.Vehicles);

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

            vehiclesListView.SetVirtualItems(listItems);
        }
    }
}
