using System;
using System.ComponentModel;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
using CP2077SaveEditor.Utils;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class ModsControl : UserControl, IGameControl
    {
        private readonly Form2 _parentForm;

        public ModsControl(Form2 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;
        }

        public string GameControlName => "Mods";

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
            gb_WardrobeExtra.Enabled = _parentForm.ActiveSaveFile.GetScriptableSystem<WardrobeSystemExtra>() != null;
        }

        private void btn_ClearBlacklist_Click(object sender, EventArgs e)
        {
            _parentForm.ActiveSaveFile.GetScriptableSystem<WardrobeSystemExtra>().Blacklist = null;
        }
    }
}
