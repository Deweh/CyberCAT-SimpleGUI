using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class ExtrasControl : UserControl, IGameControl
    {
        private readonly MainForm _parentForm;

        public ExtrasControl(MainForm parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;

            var toolTip = new ToolTip();
            Theme.SetToolTip(toolTip, btn_UnlockAll, "Unlocks every fast travel terminal on the map, so you can travel there without discovering it first. Points already unlocked are kept.");
            Theme.SetToolTip(toolTip, btn_MakeVulnerable, "Removes the invulnerability (god mode) override from V, e.g. after using console commands.");
            Theme.SetToolTip(toolTip, btn_ClearBlacklist, "Wardrobe Extra mod: shows the items you hid from the wardrobe again.");
        }

        public string GameControlName => "Extras";
        public string Icon => "";

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
            gb_Player.Enabled = true;
            gb_WardrobeExtra.Enabled = _parentForm.ActiveSaveFile.GetScriptableSystem<WardrobeSystemExtra>() != null;
        }

        private void btn_UnlockAll_Click(object sender, EventArgs e)
        {
            var fts = _parentForm.ActiveSaveFile.GetScriptableSystem<FastTravelSystem>();
            if (fts == null)
            {
                MessageBox.Show("No fast travel data found in this save.", "Unlock Fast Travel");
                return;
            }

            // Only add what's missing, so points the bundled list doesn't know about (newer patches, mods) are kept
            var known = fts.FastTravelNodes.Select(x => (ulong)x.Chunk.PointRecord).ToHashSet();
            var added = 0;
            foreach (var record in ResourceHelper.FastTravelRecords)
            {
                if (known.Add((ulong)(TweakDBID)record.PointRecord))
                {
                    fts.FastTravelNodes.Add(new gameFastTravelPointData
                    {
                        MarkerRef = record.MarkerRef,
                        PointRecord = record.PointRecord
                    });
                    added++;
                    ChangeTracker.MarkChanged();
                }
            }

            MessageBox.Show($"{added} fast travel point(s) unlocked.", "Unlock Fast Travel");
        }

        private void btn_MakeVulnerable_Click(object sender, EventArgs e)
        {
            var godModeSystem = _parentForm.ActiveSaveFile.GetGodModeSystem();
            if (godModeSystem != null)
            {
                var data = (gameGodModeSaveData)godModeSystem.RootChunk;
                foreach (var entityData in data.Gods)
                {
                    if (entityData.EntityId.Hash != 1)
                    {
                        continue;
                    }

                    for (var i = entityData.Data.Overrides.Count - 1; i >= 0; i--)
                    {
                        if (entityData.Data.Overrides[i].Type == Enums.gameGodModeType.Invulnerable)
                        {
                            entityData.Data.Overrides.RemoveAt(i);
                            ChangeTracker.MarkChanged();
                        }
                    }
                }
            }

            MessageBox.Show("Invulnerability removed from V.", "Make Vulnerable");
        }

        private void btn_ClearBlacklist_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear the Wardrobe Extra blacklist? Hidden items will show up in the wardrobe again.", "Clear Blacklist", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            _parentForm.ActiveSaveFile.GetScriptableSystem<WardrobeSystemExtra>().Blacklist = null;
            ChangeTracker.MarkChanged();
        }
    }
}
