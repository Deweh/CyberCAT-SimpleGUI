﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.CR2W.JSON;
using WolvenKit.RED4.Save;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class AppearanceControl : UserControl, IGameControl
    {
        private readonly Form2 _parentForm;

        private List<string> wipFields = new()
        {
            "Beard",
            "BeardStyle"
        };

        public AppearanceControl(Form2 parentForm)
        {
            InitializeComponent();

            _parentForm = parentForm;
            _parentForm.PropertyChanged += OnParentFormPropertyChanged;

            //appearanceOptionsPanel.Enabled = Global.IsDebug;
            //appearancePreviewBox.Enabled = Global.IsDebug;
            //advancedAppearanceButton.Enabled = Global.IsDebug;

            var lastPos = 0;
            foreach (PropertyInfo property in typeof(AppearanceHelper2).GetProperties())
            {
                if (!Global.IsDebug)
                {
                    var attr = property.GetCustomAttribute<BrowsableAttribute>();
                    if (attr is { Browsable: false })
                    {
                        continue;
                    }
                }

                if (property.PropertyType.Name != "Object" && property.PropertyType.Name != "Boolean" && property.Name != "MainSections" && property.CanWrite == true)
                {
                    var picker = new ModernValuePicker()
                    {
                        Name = property.Name,
                        PickerName = Regex.Replace(property.Name, "([a-z])([A-Z])", "$1 $2"),
                        Location = new System.Drawing.Point(0, lastPos),
                        Tag = property
                    };

                    appearanceOptionsPanel.Controls.Add(picker);
                    picker.IndexChanged += AppearanceOptionChanged;
                    picker.MouseEnter += AppearanceOptionMouseEnter;
                    
                    lastPos += picker.Height + 20;
                }
            }
        }

        public string GameControlName => "Appearance";

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
            RefreshAppearanceValues();
            SetAppearanceImage("VoiceTone", ((int)_parentForm.ActiveSaveFile.Appearance.VoiceTone).ToString("00"));
        }

        private void AppearanceOptionMouseEnter(object sender, EventArgs e)
        {
            if (((ModernValuePicker)sender).Enabled == true)
            {
                SetAppearanceImage(((ModernValuePicker)sender).Name, ((ModernValuePicker)sender).Index.ToString("00"));
            }
        }

        private void AppearanceOptionChanged(ModernValuePicker sender)
        {
            var value = ((PropertyInfo)sender.Tag).PropertyType;
            if (value == typeof(int))
            {
                SetAppearanceValue(sender, sender.Index);
            }
            else if(value == typeof(string))
            {
                SetAppearanceValue(sender, sender.StringValue);
            }
            else if (value.IsEnum)
            {
                SetAppearanceValue(sender, Enum.Parse(value, sender.StringValue));
            }
            RefreshAppearanceValues();
            SetAppearanceImage(sender.Name, sender.Index.ToString("00"));
        }

        private void advancedAppearanceButton_Click(object sender, EventArgs e)
        {
            var advancedDialog = new AdvancedAppearanceDialog(_parentForm.ActiveSaveFile.GetAppearanceContainer());
            advancedDialog.ChangesApplied += RefreshAppearanceValues;
            advancedDialog.ShowDialog();
        }

        private void saveAppearButton_Click(object sender, EventArgs e)
        {
            var saveWindow = new SaveFileDialog();
            saveWindow.Filter = "Cyberpunk 2077 Character Preset|*.v2preset";
            if (saveWindow.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveWindow.FileName, RedJsonSerializer.Serialize(new PresetDto { Data = _parentForm.ActiveSaveFile.GetAppearanceContainer() } ));
                _parentForm.SetStatus("Appearance preset saved.");
            }
        }

        private void loadAppearButton_Click(object sender, EventArgs e)
        {
            var fileWindow = new OpenFileDialog();
            fileWindow.Filter = "Cyberpunk 2077 Character Preset|*.v2preset|Cyberpunk 2077 Character Preset|*.preset";
            if (fileWindow.ShowDialog() == DialogResult.OK)
            {
                var ext = Path.GetExtension(fileWindow.FileName);
                var text = File.ReadAllText(fileWindow.FileName);

                gameuiCharacterCustomizationPresetWrapper newValues;
                if (ext == ".preset")
                {
                    if (text.StartsWith('L'))
                    {
                        MessageBox.Show("Loading of \"Appearance Change Unlocker\" presets is not supported!", "Error", MessageBoxButtons.OK);
                        return;
                    }

                    newValues = LegacyPresetHelper.Convert(text);
                }
                else
                {
                    if (JsonNode.Parse(text) is not JsonObject jsonObject)
                    {
                        return;
                    }

                    if (((IDictionary<string, JsonNode>)jsonObject).TryGetValue("JsonVersion", out var value))
                    {
                        newValues = RedJsonSerializer.Deserialize<PresetDto>(text, new RedJsonSerializerOptions { JsonVersion = value.GetValue<string>() }).Data;
                    }
                    else
                    {
                        try
                        {
                            newValues = RedJsonSerializer.Deserialize<gameuiCharacterCustomizationPresetWrapper>(text, new RedJsonSerializerOptions {JsonVersion = "0.0.6"});
                        }
                        catch (Exception)
                        {
                            try
                            {
                                newValues = RedJsonSerializer.Deserialize<gameuiCharacterCustomizationPresetWrapper>(text, new RedJsonSerializerOptions { JsonVersion = "0.0.7" });
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Unknown error while reading preset!", "Error", MessageBoxButtons.OK);
                                return;
                            }
                        }
                    }
                }

                if ((bool)newValues.Preset.IsMale != (bool)_parentForm.ActiveSaveFile.GetAppearanceContainer().Preset.IsMale)
                {
                    _parentForm.ActiveSaveFile.Appearance.SuppressBodyGenderPrompt = true;
                    _parentForm.ActiveSaveFile.Appearance.BodyGender = newValues.Preset.IsMale ? AppearanceGender.Male : AppearanceGender.Female;
                }
                _parentForm.ActiveSaveFile.SetAppearanceContainer(newValues);
                RefreshAppearanceValues();
                _parentForm.SetStatus("Appearance preset loaded.");
            }
        }

        private void SetAppearanceImage(string name, string value)
        {
            /*var oldImg = appearancePreviewBox.Image;
            var imgExists = true;
            var path = Directory.GetCurrentDirectory() + "\\previews\\" + name + "\\" + _parentForm.ActiveSaveFile.Appearance.BodyGender.ToString() + value + ".jpg";
            if (!File.Exists(path)) {
                path = Directory.GetCurrentDirectory() + "\\previews\\" + name + "\\" + value + ".jpg";
                if (!File.Exists(path))
                {
                    imgExists = false;
                }
            }

            if (imgExists)
            {
                try
                {
                    appearancePreviewBox.Image = Image.FromFile(path);
                }
                catch (Exception)
                {
                    appearancePreviewBox.Image = null;
                }
            }
            else
            {
                appearancePreviewBox.Image = null;
            }
            oldImg?.Dispose();*/
        }

        public void RefreshAppearanceValues()
        {
            foreach (ModernValuePicker picker in appearanceOptionsPanel.Controls)
            {
                try
                {
                    var value = GetAppearanceValue(picker);
                    if (value is int intValue)
                    {
                        if (intValue < 0)
                        {
                            picker.StringValue = string.Empty;
                            picker.Enabled = false;
                            continue;
                        }

                        picker.SuppressIndexChange = true;
                        picker.Index = intValue;
                    }
                    else if (value is string strValue)
                    {
                        if (picker.StringCollection.Length < 1)
                        {
                            picker.PickerType = PickerValueType.String;
                            picker.StringCollection = ((List<string>)typeof(AppearanceValueLists).GetProperty(picker.Name + "s").GetValue(null, null)).ToArray();
                        }
                        picker.SuppressIndexChange = true;

                        if (picker.StringCollection.Contains(strValue))
                        {
                            picker.Index = Array.IndexOf(picker.StringCollection, strValue);
                        }
                        else
                        {
                            picker.Index = 0;
                            picker.StringValue = strValue;
                        }
                    }
                    else if (value is Enum)
                    {
                        if (picker.StringCollection.Length < 1)
                        {
                            picker.PickerType = PickerValueType.String;
                            picker.StringCollection = Enum.GetNames(value.GetType());
                        }

                        picker.SuppressIndexChange = true;
                        picker.Index = (int)value;
                    }
                    picker.Enabled = true;
                }
                catch(Exception)
                {
                    picker.StringValue = string.Empty;
                    picker.Enabled = false;
                }
            }
        }

        private object GetAppearanceValue(ModernValuePicker picker)
        {
            return ((PropertyInfo)picker.Tag).GetValue(_parentForm.ActiveSaveFile.Appearance);
        }

        private void SetAppearanceValue(ModernValuePicker picker, object value)
        {
            ((PropertyInfo)picker.Tag).SetValue(_parentForm.ActiveSaveFile.Appearance, value);
        }
    }
}
