using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.CR2W.JSON;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class ExtrasControl : UserControl, IGameControl
    {
        private readonly Form2 _parentForm;

        public ExtrasControl(Form2 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;
        }

        public string GameControlName => "Extras";

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
            gb_FastTravel.Enabled = true;
            gb_WardrobeExtra.Enabled = _parentForm.ActiveSaveFile.GetScriptableSystem<WardrobeSystemExtra>() != null;
        }

        private void btn_UnlockAll_Click(object sender, EventArgs e)
        {
            var fts = _parentForm.ActiveSaveFile.GetScriptableSystem<FastTravelSystem>();
            if (fts != null)
            {
                fts.FastTravelNodes.Clear();
                foreach (var record in ResourceHelper.FastTravelRecords)
                {
                    fts.FastTravelNodes.Add(new gameFastTravelPointData
                    {
                        MarkerRef = record.MarkerRef,
                        PointRecord = record.PointRecord
                    });
                }
            }
        }

        private void btn_ClearBlacklist_Click(object sender, EventArgs e)
        {
            _parentForm.ActiveSaveFile.GetScriptableSystem<WardrobeSystemExtra>().Blacklist = null;
        }
    }
}
