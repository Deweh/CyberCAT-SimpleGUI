using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CP2077SaveEditor
{
    [DefaultEvent("Click")]
    public class ModernButton : Panel
    {
        private Label textLabel = new Label();
        private Color defaultColor = Color.White;
        private Color hoverColor = Color.LightGray;
        private Boolean clickEffectEnabled = true;

        [Browsable(true)]
        [Category("Style")]
        public override string Text
        {
            get { return textLabel.Text; }
            set { textLabel.Text = value; }
        }

        [Browsable(true)]
        [Category("Style")]
        public Color DefaultColor
        {
            get { return defaultColor; }
            set { defaultColor = value; this.BackColor = value; }
        }

        [Browsable(true)]
        [Category("Style")]
        public Color HoverColor
        {
            get { return hoverColor; }
            set { hoverColor = value; }
        }

        [Browsable(true)]
        [Category("Style")]
        public Color TextColor
        {
            get { return textLabel.ForeColor; }
            set { textLabel.ForeColor = value; }
        }

        [Browsable(true)]
        [Category("Style")]
        public Font TextFont
        {
            get { return textLabel.Font; }
            set { textLabel.Font = value; }
        }

        [Browsable(true)]
        [Category("Style")]
        public Boolean ClickEffectEnabled
        {
            get { return clickEffectEnabled; }
            set { clickEffectEnabled = value; }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.Controls.Add(textLabel);
            this.BorderStyle = BorderStyle.FixedSingle;
            textLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            textLabel.AutoSize = false;
            textLabel.Dock = DockStyle.Fill;
            textLabel.MouseEnter += TextMouseEnter;
            textLabel.MouseLeave += TextMouseLeave;
            textLabel.MouseDown += TextMouseDown;
            textLabel.MouseUp += TextMouseUp;
        }

        private void TextMouseEnter(object sender, EventArgs e)
        {
            base.OnMouseEnter(e);
            this.BackColor = HoverColor;
        }

        private void TextMouseLeave(object sender, EventArgs e)
        {
            base.OnMouseLeave(e);
            this.BackColor = DefaultColor;
        }

        private void TextMouseDown(object sender, EventArgs e)
        {
            if (clickEffectEnabled)
            {
                this.BorderStyle = BorderStyle.Fixed3D;
            }
        }

        private void TextMouseUp(object sender, EventArgs e)
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            base.OnClick(e);
        }
    }

    public class ModernValuePicker : Panel
    {
        private ModernButton leftButton = new ModernButton();
        private ModernButton rightButton = new ModernButton();
        private Label valueLabel = new Label();
        private Label nameLabel = new Label();
        private PickerValueType _type = PickerValueType.Integer;
        private int _index = 0;
        private int _maxInt = 99;
        private int _minInt = 0;
        private string[] _stringCollection = new string[] { };
        private string _stringValue = string.Empty;

        public delegate void IndexChangedHandler(ModernValuePicker sender);
        public event IndexChangedHandler IndexChanged;

        public string PickerName
        {
            get {
                return nameLabel.Text;
            }
            set {
                nameLabel.Text = value.ToUpper();
                UpdateChildControls();
            }
        }

        public PickerValueType PickerType
        {
            get {
                return _type;
            }
            set
            {
                _type = value;
                SuppressIndexChange = true;
                Index = 0;
            }
        }

        public string StringValue
        {
            get
            {
                return _stringValue;
            }
            set
            {
                _stringValue = value;
                valueLabel.Text = _stringValue.ToUpper();
                UpdateChildControls();
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (PickerType == PickerValueType.Integer)
                {
                    if (value > MaxIntegerValue)
                    {
                        _index = MaxIntegerValue;
                    }
                    else if (value < MinIntegerValue)
                    {
                        _index = MinIntegerValue;
                    }
                    else
                    {
                        _index = value;
                    }
                    StringValue = _index.ToString("00");
                }
                else
                {
                    if (value > StringCollection.Length - 1)
                    {
                        _index = StringCollection.Length - 1;
                    }
                    else if (value < 0)
                    {
                        _index = 0;
                    }
                    else
                    {
                        _index = value;
                    }

                    if (StringCollection.Length < 1)
                    {
                        StringValue = string.Empty;
                    } else {
                        StringValue = StringCollection[_index];
                    }
                }

                UpdateChildControls();

                if (SuppressIndexChange == true)
                {
                    SuppressIndexChange = false;
                } else {
                    if (IndexChanged != null) { IndexChanged(this); }
                }
            }
        }

        public bool SuppressIndexChange { get; set; } = false;

        public string[] StringCollection
        {
            get
            {
                return _stringCollection;
            }
            set
            {
                _stringCollection = value;
                if (PickerType == PickerValueType.String)
                {
                    SuppressIndexChange = true;
                    Index = 0;
                }
            }
        }

        public int MaxIntegerValue {
            get
            {
                return _maxInt;
            }
            set
            {
                _maxInt = value;
                if (PickerType == PickerValueType.Integer && Index > _maxInt)
                {
                    Index = _maxInt;
                }
            }
        }

        public int MinIntegerValue {
            get
            {
                return _minInt;
            }
            set
            {
                _minInt = value;
                if (PickerType == PickerValueType.Integer && Index < _minInt)
                {
                    Index = _minInt;
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.Controls.AddRange(new Control[] { leftButton, rightButton, valueLabel, nameLabel });
            this.BorderStyle = BorderStyle.None;

            leftButton.Text = "<";
            rightButton.Text = ">";
            leftButton.Size = new Size(20, 50);
            rightButton.Size = new Size(20, 50);
            nameLabel.AutoSize = true;
            valueLabel.AutoSize = true;
            nameLabel.Font = new Font(nameLabel.Font.FontFamily, 13, FontStyle.Bold);
            valueLabel.Font = new Font(valueLabel.Font.FontFamily, 13);

            leftButton.Click += leftButton_Click;
            rightButton.Click += rightButton_Click;
            nameLabel.TextChanged += UpdateChildControls;
            valueLabel.TextChanged += UpdateChildControls;
            this.SizeChanged += UpdateChildControls;

            valueLabel.Click += valueLabel_DoubleClick;
            UpdateChildControls();
        }

        public ModernValuePicker()
        {
            PickerName = "Name";
            valueLabel.Text = "00";
            this.Size = new Size(200, 100);
        }

        private void UpdateChildControls(object sender = null, EventArgs e = null)
        {
            leftButton.Location = new Point(10, (this.Height / 5) * 3 - (leftButton.Height / 2));
            rightButton.Location = new Point(this.Width - 30, (this.Height / 5) * 3 - (rightButton.Height / 2));
            nameLabel.Location = new Point((this.Width / 2) - (nameLabel.Width / 2), (this.Height / 5) - (nameLabel.Height / 2));
            valueLabel.Location = new Point((this.Width / 2) - (valueLabel.Width / 2), (this.Height / 5) * 3 - (valueLabel.Height / 2));
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            Index = Index - 1;
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            Index = Index + 1;
        }

        private void valueLabel_DoubleClick(object sender, EventArgs e)
        {
            Control entryBox;
            if (PickerType == PickerValueType.Integer)
            {
                entryBox = new TextBox();
            } else {
                var newCombo = new ComboBox();
                newCombo.Items.AddRange(StringCollection);
                newCombo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                newCombo.AutoCompleteSource = AutoCompleteSource.ListItems;
                newCombo.SelectedIndexChanged += entryBox_IndexChanged;
                entryBox = newCombo;
            }
            this.Controls.Add(entryBox);

            entryBox.Font = valueLabel.Font;
            entryBox.Location = valueLabel.Location;
            entryBox.Text = StringValue;
            entryBox.Size = valueLabel.Size;
            entryBox.KeyDown += entryBox_KeyDown;
            entryBox.Select();

            leftButton.Enabled = false;
            rightButton.Enabled = false;

            valueLabel.Visible = false;
        }

        private void entryBox_IndexChanged(object sender, EventArgs e)
        {
            var castedSender = (ComboBox)sender;
            if (castedSender.Text != StringValue)
            {
                entryBox_ApplyValue(sender, null);
            }
        }

        private void entryBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; e.SuppressKeyPress = true;
                entryBox_ApplyValue(sender, null);
            }
        }

        private void entryBox_ApplyValue(object sender, EventArgs e)
        {
            Control castedSender;
            if (sender is ComboBox)
            {
                castedSender = (ComboBox)sender;
                var newInd = Array.FindIndex(StringCollection, x => x.ToLower() == castedSender.Text.ToLower());
                if (newInd > -1)
                {
                    Index = newInd;
                }
            } else {
                castedSender = (TextBox)sender;
                int newInd;
                if (int.TryParse(castedSender.Text, out newInd) == true)
                {
                    Index = newInd;
                }
            }

            this.Controls.Remove(castedSender);

            leftButton.Enabled = true;
            rightButton.Enabled = true;

            valueLabel.Visible = true;
        }
    }

    public enum PickerValueType
    {
        Integer,
        String
    }
}
