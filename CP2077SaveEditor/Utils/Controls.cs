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
        //Painting
        private Font[] fonts = new Font[] { new Font("Segoe UI", (float)8.25), new Font("Segoe UI", 13), new Font("Segoe UI", 13, FontStyle.Bold) };
        private Pen pixelPen = new Pen(Color.Black, 1);
        private bool[] gStates = new bool[] { false, false, false, false, false };
        private bool[] enabledStates = new bool[] { true, true };
        private Rectangle leftRect = new Rectangle(1,1,1,1);
        private Rectangle rightRect = new Rectangle(1,1,1,1);

        //Other
        private PickerValueType _type = PickerValueType.Integer;
        private int _index = 0;
        private int _maxInt = 99;
        private int _minInt = 0;
        private string[] _stringCollection = new string[] { };
        private string _stringValue = "00";

        public delegate void IndexChangedHandler(ModernValuePicker sender);
        public event IndexChangedHandler IndexChanged;

        public string PickerName { get; set; } = "Name";
        public string StringValue
        {
            get
            {
                return _stringValue;
            }
            set
            {
                _stringValue = value;
                this.Invalidate(new Rectangle(leftRect.Right, leftRect.Top, rightRect.Left - leftRect.Right, 50));
            }
        }
        public bool SuppressIndexChange { get; set; } = false;

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

                if (SuppressIndexChange == true)
                {
                    SuppressIndexChange = false;
                } else {
                    if (IndexChanged != null) { IndexChanged(this); }
                }
            }
        }

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

        public ModernValuePicker()
        {
            PickerName = "Name";
            StringValue = "00";
            this.Size = new Size(200, 100);
            this.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this, true, null);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            leftRect = new Rectangle(10, (this.Height / 5) * 3 - 25, 20, 50);
            rightRect = new Rectangle(this.Width - 30, (this.Height / 5) * 3 - 25, 20, 50);
            var leftSize = TextRenderer.MeasureText("<", fonts[0]);
            var rightSize = TextRenderer.MeasureText(">", fonts[0]);
            var valueSize = TextRenderer.MeasureText(StringValue.ToUpper(), fonts[1]);
            var nameSize = TextRenderer.MeasureText(PickerName.ToUpper(), fonts[2]);

            if (gStates[0]) g.FillRectangle(Brushes.LightGray, leftRect);
            if (gStates[1]) g.FillRectangle(Brushes.LightGray, rightRect);
            if (gStates[2]) g.FillRectangle(Brushes.DarkGray, leftRect);
            if (gStates[3]) g.FillRectangle(Brushes.DarkGray, rightRect);

            g.DrawRectangle(pixelPen, leftRect);
            g.DrawRectangle(pixelPen, rightRect);
            TextRenderer.DrawText(g, "<", fonts[0], new Point(leftRect.X + ((leftRect.Width / 2) - (leftSize.Width / 2)), leftRect.Y + ((leftRect.Height / 2) - (leftSize.Height / 2))), pixelPen.Color);
            TextRenderer.DrawText(g, ">", fonts[0], new Point(rightRect.X + ((rightRect.Width / 2) - (rightSize.Width / 2)), rightRect.Y + ((rightRect.Height / 2) - (rightSize.Height / 2))), pixelPen.Color);
            if (!gStates[4]) TextRenderer.DrawText(g, StringValue.ToUpper(), fonts[1], new Point((this.Width / 2) - (valueSize.Width / 2), (this.Height / 5) * 3 - (valueSize.Height / 2)), pixelPen.Color);
            TextRenderer.DrawText(g, PickerName.ToUpper(), fonts[2], new Point((this.Width / 2) - (nameSize.Width / 2), (this.Height / 5) - (nameSize.Height / 2)), pixelPen.Color);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (leftRect.Contains(e.Location) && !gStates[0] && enabledStates[0])
            {
                gStates[0] = true;
                this.Invalidate(leftRect);
            }
            else if (!leftRect.Contains(e.Location) && gStates[0])
            {
                gStates[0] = false;
                this.Invalidate(leftRect);
            }

            if (rightRect.Contains(e.Location) && !gStates[1] && enabledStates[1])
            {
                gStates[1] = true;
                this.Invalidate(rightRect);
            }
            else if (!rightRect.Contains(e.Location) && gStates[1])
            {
                gStates[1] = false;
                this.Invalidate(rightRect);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            gStates = new bool[] { false, false, false, false, false };
            
            this.Invalidate(leftRect);
            this.Invalidate(rightRect);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (leftRect.Contains(e.Location) && !gStates[2] && enabledStates[0])
            {
                gStates[2] = true;
                this.Invalidate(leftRect);
            }

            if (rightRect.Contains(e.Location) && !gStates[3] && enabledStates[1])
            {
                gStates[3] = true;
                this.Invalidate(rightRect);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (gStates[2])
            {
                if (enabledStates[0])
                {
                    Index -= 1;
                }
                gStates[2] = false;
                this.Invalidate(leftRect);
            }

            if (gStates[3])
            {
                if (enabledStates[1])
                {
                    Index += 1;
                }
                gStates[3] = false;
                this.Invalidate(rightRect);
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (this.Enabled)
            {
                pixelPen = new Pen(Color.Black, 1);
            } else {
                pixelPen = new Pen(Color.Gray, 1);
            }
            this.Invalidate();
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (gStates[4])
            {
                return;
            }

            var valueSize = TextRenderer.MeasureText(StringValue.ToUpper(), fonts[1]);
            var valueLoc = new Point((this.Width / 2) - (valueSize.Width / 2), (this.Height / 5) * 3 - (valueSize.Height / 2));

            if (!new Rectangle(valueLoc, valueSize).Contains(PointToClient(Cursor.Position)))
            {
                return;
            }

            Control entryBox;
            if (PickerType == PickerValueType.Integer)
            {
                entryBox = new TextBox();
            }
            else
            {
                var newCombo = new ComboBox();
                newCombo.Items.AddRange(StringCollection);
                newCombo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                newCombo.AutoCompleteSource = AutoCompleteSource.ListItems;
                newCombo.SelectedIndexChanged += entryBox_IndexChanged;
                entryBox = newCombo;
            }
            this.Controls.Add(entryBox);

            entryBox.Font = fonts[1];
            entryBox.Location = valueLoc;
            entryBox.Text = StringValue;
            entryBox.Size = valueSize;
            entryBox.KeyDown += entryBox_KeyDown;
            entryBox.Select();

            enabledStates = new bool[] { false, false };
            gStates[4] = true;
            this.Invalidate();
        }

        private void entryBox_IndexChanged(object sender, EventArgs e)
        {
            var castedSender = (ComboBox)sender;
            if (castedSender.Text != StringValue)
            {
                entryBox_ApplyValue(sender);
            }
        }

        private void entryBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; e.SuppressKeyPress = true;
                entryBox_ApplyValue(sender);
            }
        }

        private void entryBox_ApplyValue(object sender)
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
            }
            else
            {
                castedSender = (TextBox)sender;
                int newInd;
                if (int.TryParse(castedSender.Text, out newInd) == true)
                {
                    Index = newInd;
                }
            }
            this.Controls.Remove(castedSender);

            enabledStates = new bool[] { true, true };
            gStates[4] = false;
            this.Invalidate();
        }
    }

    public enum PickerValueType
    {
        Integer,
        String
    }
}
