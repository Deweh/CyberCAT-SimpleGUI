using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2077SaveEditor
{
    [DefaultEvent("Click")]
    public class ModernButton : Panel
    {
        private readonly Label textLabel = new Label();
        private Color defaultColor = Utils.Theme.DarkRaised;
        private Color hoverColor = Utils.Theme.DarkHover;
        private bool clickEffectEnabled = true;
        private bool _isSelected;
        private bool _isHovered;
        private bool _isPressed;
        private bool _isAccent;

        [Browsable(true)]
        [Category("Style")]
        public new BorderStyle BorderStyle
        {
            get => BorderStyle.None;
            set => base.BorderStyle = BorderStyle.None;
        }

        [Browsable(true)]
        [Category("Style")]
        public override string Text
        {
            get => textLabel.Text;
            set
            {
                textLabel.Text = value;
                textLabel.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Style")]
        public Color DefaultColor
        {
            get => defaultColor;
            set { defaultColor = value; UpdateVisualState(); }
        }

        [Browsable(true)]
        [Category("Style")]
        public Color HoverColor
        {
            get => hoverColor;
            set { hoverColor = value; UpdateVisualState(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAccent
        {
            get => _isAccent;
            set { _isAccent = value; UpdateVisualState(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                {
                    return;
                }

                _isSelected = value;
                if (textLabel.Font != null)
                {
                    textLabel.Font = new Font(textLabel.Font, _isSelected ? FontStyle.Bold : FontStyle.Regular);
                }
                UpdateVisualState();
                textLabel.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Style")]
        public Color TextColor
        {
            get => textLabel.ForeColor;
            set { textLabel.ForeColor = value; textLabel.Invalidate(); }
        }

        [Browsable(true)]
        [Category("Style")]
        public Font TextFont
        {
            get => textLabel.Font;
            set
            {
                textLabel.Font = _isSelected && value != null ? new Font(value, FontStyle.Bold) : value;
                textLabel.Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Style")]
        public bool ClickEffectEnabled
        {
            get => clickEffectEnabled;
            set => clickEffectEnabled = value;
        }

        public ModernButton()
        {
            base.BorderStyle = BorderStyle.None;
            DoubleBuffered = true;
            Controls.Add(textLabel);
            textLabel.TextAlign = ContentAlignment.MiddleCenter;
            textLabel.AutoSize = false;
            textLabel.Dock = DockStyle.Fill;
            textLabel.MouseEnter += TextMouseEnter;
            textLabel.MouseLeave += TextMouseLeave;
            textLabel.MouseDown += TextMouseDown;
            textLabel.MouseUp += TextMouseUp;
            textLabel.Paint += TextPaint;
            EnabledChanged += (_, _) => textLabel.Invalidate();
            Resize += (_, _) => textLabel.Invalidate();
        }

        private void UpdateVisualState()
        {
            BackColor = Parent?.BackColor ?? (Utils.Theme.IsDark ? Utils.Theme.DarkGround : defaultColor);
            textLabel.BackColor = BackColor;
            textLabel.Invalidate();
        }

        private void TextPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bgClear = Parent?.BackColor ?? (Utils.Theme.IsDark ? Utils.Theme.DarkGround : Color.White);
            g.Clear(bgClear);

            var w = textLabel.Width;
            var h = textLabel.Height;
            if (w <= 4 || h <= 4)
            {
                return;
            }

            if (!Utils.Theme.IsDark)
            {
                var lightFill = _isHovered ? hoverColor : (_isSelected ? Color.Gainsboro : defaultColor);
                using var brush = new SolidBrush(lightFill);
                using var pen = new Pen(Utils.Theme.BorderColor);
                g.FillRectangle(brush, 0, 0, w - 1, h - 1);
                g.DrawRectangle(pen, 0, 0, w - 1, h - 1);
                DrawButtonContent(g, w, h, textLabel.ForeColor, textLabel.ForeColor);
                return;
            }

            var cut = Math.Min(10, Math.Max(6, h / 4));
            var rect = new Rectangle(0, 0, w - 1, h - 1);
            var pts = Utils.Theme.GetChamferPoints(rect, 0, cut);

            Color bodyColor;
            Color borderColor;
            Color barColor;
            Color fgColor;
            Color iconColor;
            int barWidth;
            float borderWidth;

            if (!Enabled)
            {
                bodyColor = Color.FromArgb(20, 8, 12);
                borderColor = Color.FromArgb(85, 22, 38);
                barColor = Color.FromArgb(85, 22, 38);
                fgColor = Color.FromArgb(115, 50, 68);
                iconColor = fgColor;
                barWidth = 3;
                borderWidth = 1f;
            }
            else if (_isPressed && clickEffectEnabled)
            {
                bodyColor = Utils.Theme.Accent;
                borderColor = Utils.Theme.Cyan;
                barColor = Utils.Theme.NeonRed;
                fgColor = Utils.Theme.OnAccent;
                iconColor = Utils.Theme.OnAccent;
                barWidth = 6;
                borderWidth = 2f;
            }
            else if (_isSelected)
            {
                bodyColor = Color.FromArgb(62, 18, 24);
                borderColor = Utils.Theme.Accent;
                barColor = Utils.Theme.Accent;
                fgColor = Utils.Theme.Accent;
                iconColor = Utils.Theme.Cyan;
                barWidth = 6;
                borderWidth = 1.8f;
            }
            else if (_isHovered)
            {
                bodyColor = Utils.Theme.DarkHover;
                borderColor = Utils.Theme.Cyan;
                barColor = Utils.Theme.Cyan;
                fgColor = Utils.Theme.Cyan;
                iconColor = Utils.Theme.Accent;
                barWidth = 5;
                borderWidth = 1.8f;
            }
            else if (_isAccent)
            {
                bodyColor = Color.FromArgb(48, 12, 22);
                borderColor = Utils.Theme.NeonRed;
                barColor = Utils.Theme.Accent;
                fgColor = Utils.Theme.Accent;
                iconColor = Utils.Theme.Accent;
                barWidth = 5;
                borderWidth = 1.4f;
            }
            else
            {
                bodyColor = Utils.Theme.DarkRaised;
                borderColor = Utils.Theme.NeonRed;
                barColor = Utils.Theme.NeonRed;
                fgColor = Utils.Theme.DarkText;
                iconColor = Utils.Theme.Cyan;
                barWidth = 3;
                borderWidth = 1f;
            }

            using (var bodyBrush = new SolidBrush(bodyColor))
            {
                g.FillPolygon(bodyBrush, pts);
            }

            if (!_isPressed && Enabled)
            {
                Utils.Theme.DrawScanlines(g, rect, 18);
            }

            // Left high-voltage Cyberpunk bar
            using (var barBrush = new SolidBrush(barColor))
            {
                g.FillRectangle(barBrush, 0, 0, barWidth, h);
            }

            // Selected / Accent inner notch on the left bar
            if ((_isSelected || _isAccent) && Enabled)
            {
                using var notchBrush = new SolidBrush(_isSelected ? Utils.Theme.NeonRed : Utils.Theme.Cyan);
                g.FillRectangle(notchBrush, barWidth, h / 2 - 4, 2, 8);
            }

            // Chamfered neon border
            using (var borderPen = new Pen(borderColor, borderWidth))
            {
                g.DrawPolygon(borderPen, pts);
            }

            // Diagonal highlight along the chamfered corner
            if (Enabled)
            {
                var chamferHighlight = _isHovered ? Utils.Theme.Accent : (_isSelected ? Utils.Theme.Cyan : Utils.Theme.Accent);
                using var chamferPen = new Pen(chamferHighlight, 2f);
                g.DrawLine(chamferPen, rect.Right, rect.Bottom - cut, rect.Right - cut, rect.Bottom);

                // Top-right corner HUD tick
                using var tickPen = new Pen(_isHovered || _isSelected ? Utils.Theme.Cyan : Utils.Theme.NeonRed, 1.5f);
                g.DrawLine(tickPen, rect.Right - 9, rect.Top + 1, rect.Right - 1, rect.Top + 1);
                g.DrawLine(tickPen, rect.Right - 1, rect.Top + 1, rect.Right - 1, rect.Top + 7);
            }

            // Micro telemetry stamp on tall primary buttons (like Load Save)
            if (_isAccent && h >= 46 && Enabled)
            {
                using var microFont = new Font(Utils.Theme.MonoFontFamily, 6.5f, FontStyle.Bold);
                var stampColor = _isPressed ? Utils.Theme.OnAccent : Utils.Theme.Cyan;
                TextRenderer.DrawText(g, "NC // R-2077", microFont, new Rectangle(barWidth + 6, h - 15, w - cut - 12, 13), stampColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            DrawButtonContent(g, w, h, fgColor, iconColor);
        }

        private void DrawButtonContent(Graphics g, int w, int h, Color fgColor, Color iconColor)
        {
            var hasIcon = _iconGlyph != null && s_iconFont.Value != null;
            if (hasIcon)
            {
                var dpi = DeviceDpi / 96f;
                var iconBounds = new Rectangle((int)(11 * dpi), 0, (int)(IconWidth * dpi), h);
                TextRenderer.DrawText(g, _iconGlyph, s_iconFont.Value, iconBounds, iconColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            var displayText = (textLabel.Text ?? string.Empty).ToUpperInvariant();
            var baseFont = textLabel.Font ?? Font;
            var style = (_isSelected || _isAccent || _isHovered) ? FontStyle.Bold : baseFont.Style;
            using var drawFont = new Font(Utils.Theme.UiFontFamily, baseFont.Size, style, baseFont.Unit);

            if (hasIcon)
            {
                var dpiScale = DeviceDpi / 96f;
                var textLeft = (int)((IconWidth + 15) * dpiScale);
                var textBounds = new Rectangle(textLeft, 0, w - textLeft - (int)(7 * dpiScale), h);
                TextRenderer.DrawText(g, displayText, drawFont, textBounds, fgColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                if (_isSelected)
                {
                    TextRenderer.DrawText(g, "▸", drawFont, new Rectangle(w - 18, 0, 14, h), Utils.Theme.Cyan,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
            else
            {
                var yAdjust = (_isAccent && h >= 46) ? -3 : 0;
                var textBounds = new Rectangle(8, yAdjust, w - 14, h);
                TextRenderer.DrawText(g, displayText, drawFont, textBounds, fgColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        private const int IconWidth = 22;

        private static readonly Lazy<Font> s_iconFont = new(() =>
        {
            var installed = FontFamily.Families.Select(x => x.Name).ToHashSet();
            var name = new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" }.FirstOrDefault(installed.Contains);
            return name == null ? null : new Font(name, 12.5F, FontStyle.Regular, GraphicsUnit.Point);
        });

        private string _iconGlyph;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string IconGlyph
        {
            get => _iconGlyph;
            set
            {
                _iconGlyph = value;
                textLabel.Invalidate();
            }
        }

        private void TextMouseEnter(object sender, EventArgs e)
        {
            _isHovered = true;
            base.OnMouseEnter(e);
            UpdateVisualState();
        }

        private void TextMouseLeave(object sender, EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            base.OnMouseLeave(e);
            UpdateVisualState();
        }

        private void TextMouseDown(object sender, EventArgs e)
        {
            _isPressed = true;
            textLabel.Invalidate();
        }

        private void TextMouseUp(object sender, EventArgs e)
        {
            _isPressed = false;
            textLabel.Invalidate();
            base.OnClick(e);
        }
    }

    public class ModernValuePicker : Panel
    {
        private readonly Font[] fonts = new Font[]
        {
            new Font(Utils.Theme.MonoFontFamily, 9.5F, FontStyle.Bold),
            new Font(Utils.Theme.MonoFontFamily, 12.5F, FontStyle.Bold),
            new Font(Utils.Theme.UiFontFamily, 9.5F, FontStyle.Bold)
        };
        private Pen pixelPen = new Pen(Color.Black, 1);
        private bool[] gStates = new bool[] { false, false, false, false, false };
        private bool[] enabledStates = new bool[] { true, true };
        private Rectangle leftRect = new Rectangle(1, 1, 1, 1);
        private Rectangle rightRect = new Rectangle(1, 1, 1, 1);

        private PickerValueType _type = PickerValueType.Integer;
        private int _index = 0;
        private int _maxInt = 99;
        private int _minInt = 0;
        private string[] _stringCollection = Array.Empty<string>();
        private string _stringValue = "00";

        public delegate void IndexChangedHandler(ModernValuePicker sender);
        public event IndexChangedHandler IndexChanged;

        public string PickerName { get; set; } = "Name";
        public string StringValue
        {
            get => _stringValue;
            set
            {
                _stringValue = value;
                Invalidate();
            }
        }
        public bool SuppressIndexChange { get; set; } = false;

        public PickerValueType PickerType
        {
            get => _type;
            set
            {
                _type = value;
                SuppressIndexChange = true;
                Index = 0;
            }
        }

        public int Index
        {
            get => _index;
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
                        _index = 0;
                    }
                    else if (value < 0)
                    {
                        _index = StringCollection.Length - 1;
                    }
                    else
                    {
                        _index = value;
                    }

                    StringValue = StringCollection.Length < 1 ? string.Empty : StringCollection[_index];
                }

                if (SuppressIndexChange)
                {
                    SuppressIndexChange = false;
                }
                else
                {
                    IndexChanged?.Invoke(this);
                }
            }
        }

        public string[] StringCollection
        {
            get => _stringCollection;
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

        public int MaxIntegerValue
        {
            get => _maxInt;
            set
            {
                _maxInt = value;
                if (PickerType == PickerValueType.Integer && Index > _maxInt)
                {
                    Index = _maxInt;
                }
            }
        }

        public int MinIntegerValue
        {
            get => _minInt;
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
            base.BorderStyle = BorderStyle.None;
            PickerName = "Name";
            StringValue = "00";
            Size = new Size(200, 100);
            DoubleBuffered = true;
            UpdatePen();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? (Utils.Theme.IsDark ? Utils.Theme.DarkGround : Color.White));

            var cardRect = new Rectangle(1, 1, Width - 3, Height - 3);
            var cut = 10;
            var cardPts = Utils.Theme.GetChamferPoints(cardRect, 0, cut);

            if (Utils.Theme.IsDark)
            {
                using (var cardFill = new SolidBrush(Utils.Theme.DarkSurface))
                {
                    g.FillPolygon(cardFill, cardPts);
                }
                Utils.Theme.DrawScanlines(g, cardRect, 14);

                // Header strip
                using (var stripBrush = new SolidBrush(Color.FromArgb(45, 255, 0, 60)))
                {
                    g.FillRectangle(stripBrush, cardRect.Left + 1, cardRect.Top + 1, cardRect.Width - 2, 24);
                }
                using (var yellowBar = new SolidBrush(Utils.Theme.Accent))
                {
                    g.FillRectangle(yellowBar, cardRect.Left, cardRect.Top, 4, 25);
                }

                var isCardActive = gStates[0] || gStates[1];
                using (var borderPen = new Pen(isCardActive ? Utils.Theme.Cyan : Utils.Theme.NeonRed, isCardActive ? 1.5f : 1f))
                {
                    g.DrawPolygon(borderPen, cardPts);
                }

                using (var yellowChamfer = new Pen(Utils.Theme.Accent, 2f))
                {
                    g.DrawLine(yellowChamfer, cardRect.Right, cardRect.Bottom - cut, cardRect.Right - cut, cardRect.Bottom);
                }
            }
            else
            {
                g.DrawRectangle(pixelPen, cardRect);
            }

            leftRect = new Rectangle(10, (Height / 5) * 3 - 18, 26, 40);
            rightRect = new Rectangle(Width - 36, (Height / 5) * 3 - 18, 26, 40);

            // Center readout terminal box
            var centerBox = new Rectangle(leftRect.Right + 6, leftRect.Top, rightRect.Left - leftRect.Right - 12, leftRect.Height);
            if (Utils.Theme.IsDark && centerBox.Width > 10)
            {
                using var terminalFill = new SolidBrush(Utils.Theme.DarkGround);
                using var terminalBorder = new Pen(Color.FromArgb(110, 0, 240, 255), 1f);
                g.FillRectangle(terminalFill, centerBox);
                g.DrawRectangle(terminalBorder, centerBox);
            }

            // Left & Right '<' '>' buttons
            DrawStepButton(g, leftRect, "<", gStates[0], gStates[2]);
            DrawStepButton(g, rightRect, ">", gStates[1], gStates[3]);

            var valueColor = Enabled ? Utils.Theme.CyanAccent : Color.Gray;
            var titleColor = Enabled ? (Utils.Theme.IsDark ? Utils.Theme.Accent : pixelPen.Color) : Color.Gray;

            if (!gStates[4])
            {
                TextRenderer.DrawText(g, (StringValue ?? string.Empty).ToUpperInvariant(), fonts[1], centerBox, valueColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            var headerRect = new Rectangle(10, 2, Width - 20, 22);
            var titleText = Utils.Theme.IsDark ? "// " + (PickerName ?? string.Empty).ToUpperInvariant() : (PickerName ?? string.Empty).ToUpperInvariant();
            TextRenderer.DrawText(g, titleText, fonts[2], headerRect, titleColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private void DrawStepButton(Graphics g, Rectangle rect, string symbol, bool hovered, bool pressed)
        {
            if (!Utils.Theme.IsDark)
            {
                if (hovered) g.FillRectangle(Brushes.LightGray, rect);
                if (pressed) g.FillRectangle(Brushes.DarkGray, rect);
                g.DrawRectangle(pixelPen, rect);
                TextRenderer.DrawText(g, symbol, fonts[0], rect, pixelPen.Color, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            var fill = !Enabled
                ? Utils.Theme.DarkSurface
                : pressed
                    ? Utils.Theme.Cyan
                    : hovered
                        ? Utils.Theme.Accent
                        : Utils.Theme.DarkRaised;

            var border = !Enabled
                ? Color.FromArgb(80, 30, 45)
                : (hovered || pressed ? Utils.Theme.Cyan : Utils.Theme.NeonRed);

            var fg = !Enabled
                ? Color.FromArgb(100, 50, 65)
                : (hovered || pressed ? Utils.Theme.OnAccent : Utils.Theme.Cyan);

            using (var fillBrush = new SolidBrush(fill))
            using (var borderPen = new Pen(border, 1.2f))
            {
                g.FillRectangle(fillBrush, rect);
                g.DrawRectangle(borderPen, rect);
            }

            TextRenderer.DrawText(g, symbol, fonts[0], rect, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (leftRect.Contains(e.Location) && !gStates[0] && enabledStates[0])
            {
                gStates[0] = true;
                Invalidate();
            }
            else if (!leftRect.Contains(e.Location) && gStates[0])
            {
                gStates[0] = false;
                Invalidate();
            }

            if (rightRect.Contains(e.Location) && !gStates[1] && enabledStates[1])
            {
                gStates[1] = true;
                Invalidate();
            }
            else if (!rightRect.Contains(e.Location) && gStates[1])
            {
                gStates[1] = false;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            gStates = new bool[] { false, false, false, false, false };
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (leftRect.Contains(e.Location) && !gStates[2] && enabledStates[0])
            {
                gStates[2] = true;
                Invalidate();
            }

            if (rightRect.Contains(e.Location) && !gStates[3] && enabledStates[1])
            {
                gStates[3] = true;
                Invalidate();
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
                Invalidate();
            }

            if (gStates[3])
            {
                if (enabledStates[1])
                {
                    Index += 1;
                }
                gStates[3] = false;
                Invalidate();
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            UpdatePen();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            UpdatePen();
        }

        private void UpdatePen()
        {
            pixelPen = new Pen(Enabled ? ForeColor : Color.Gray, 1);
            Invalidate();
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (gStates[4])
            {
                return;
            }

            var valueSize = TextRenderer.MeasureText(StringValue.ToUpper(), fonts[1]);
            var valueLoc = new Point((Width / 2) - (valueSize.Width / 2), (Height / 5) * 3 - (valueSize.Height / 2));

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
            Controls.Add(entryBox);

            entryBox.Font = fonts[1];
            entryBox.Location = valueLoc;
            entryBox.Text = StringValue;
            entryBox.Size = valueSize;
            entryBox.KeyDown += entryBox_KeyDown;
            entryBox.Select();

            enabledStates = new bool[] { false, false };
            gStates[4] = true;
            Invalidate();
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
                e.Handled = true;
                e.SuppressKeyPress = true;
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
                if (int.TryParse(castedSender.Text, out var newInd))
                {
                    Index = newInd;
                }
            }
            Controls.Remove(castedSender);

            enabledStates = new bool[] { true, true };
            gStates[4] = false;
            Invalidate();
        }
    }

    public enum PickerValueType
    {
        Integer,
        String
    }
}
