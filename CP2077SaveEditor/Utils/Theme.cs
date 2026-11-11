using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Cyberpunk 2077 Night City HUD theme: deep crimson-black void surfaces, Samurai Neon Red (#FF003C) structural borders,
    /// blazing Cyberpunk Yellow (#FCEE0A) primary highlights, and Electric Netrunner Cyan (#00F0FF) telemetry/data readouts.
    /// </summary>
    public static class Theme
    {
        public static bool IsDark { get; private set; }

        // Authentic Cyberpunk 2077 pause/character menu palette (deep crimson-void + neon red + yellow + cyan)
        public static readonly Color DarkGround = Color.FromArgb(14, 5, 9);
        public static readonly Color DarkSurface = Color.FromArgb(24, 8, 14);
        public static readonly Color DarkRaised = Color.FromArgb(38, 11, 20);
        public static readonly Color DarkHover = Color.FromArgb(74, 15, 32);
        public static readonly Color NeonRed = Color.FromArgb(255, 0, 60);
        public static readonly Color DarkBorder = Color.FromArgb(195, 14, 50);
        public static readonly Color DarkText = Color.FromArgb(242, 248, 252);
        public static readonly Color DarkMuted = Color.FromArgb(255, 92, 122);

        // Cyberpunk Yellow (#FCEE0A) and Netrunner Electric Cyan (#00F0FF)
        public static readonly Color Accent = Color.FromArgb(252, 238, 10);
        public static readonly Color OnAccent = Color.FromArgb(14, 5, 9);
        public static readonly Color Cyan = Color.FromArgb(0, 240, 255);

        public static Color AccentBar => IsDark ? Accent : Color.FromArgb(204, 180, 0);
        public static Color CyanAccent => IsDark ? Cyan : Color.FromArgb(0, 122, 140);
        public static Color MutedText => IsDark ? DarkMuted : Color.FromArgb(95, 99, 104);
        public static Color BorderColor => IsDark ? NeonRed : Color.FromArgb(218, 220, 224);

        private static readonly Lazy<FontFamily> s_uiFontFamily = new(() =>
        {
            var installed = FontFamily.Families.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            foreach (var candidate in new[] { "Bahnschrift", "Segoe UI Variable Text", "Segoe UI" })
            {
                if (installed.TryGetValue(candidate, out var family))
                {
                    return family;
                }
            }
            return SystemFonts.DefaultFont.FontFamily;
        });

        private static readonly Lazy<FontFamily> s_monoFontFamily = new(() =>
        {
            var installed = FontFamily.Families.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            foreach (var candidate in new[] { "Cascadia Mono", "Consolas" })
            {
                if (installed.TryGetValue(candidate, out var family))
                {
                    return family;
                }
            }
            return FontFamily.GenericMonospace;
        });

        public static FontFamily UiFontFamily => s_uiFontFamily.Value;
        public static FontFamily MonoFontFamily => s_monoFontFamily.Value;

        /// <summary>
        /// Text color of an item rarity, like the game's (Uncommon green ... Legendary orange). Null for Common or unknown.
        /// </summary>
        public static Color? GetQualityColor(string quality)
        {
            if (quality == null)
            {
                return null;
            }

            // "EpicPlus" -> "Epic"
            return quality.Replace("Plus", "") switch
            {
                "Uncommon" => Color.FromArgb(35, 225, 110),
                "Rare" => Color.FromArgb(0, 195, 255),
                "Epic" => Color.FromArgb(200, 95, 255),
                "Legendary" => Color.FromArgb(255, 150, 15),
                _ => null
            };
        }

        // Rows that stand out a little, e.g. equipped items
        public static Color HighlightRow => IsDark ? Color.FromArgb(52, 14, 26) : Color.FromArgb(244, 246, 248);

        private static readonly ConditionalWeakTable<Form, object> s_themedForms = new();
        private static readonly ConditionalWeakTable<Control, object> s_paintedControls = new();
        private static readonly PropertyInfo s_doubleBufferedProp =
            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void EnableDoubleBuffering(Control control)
        {
            s_doubleBufferedProp?.SetValue(control, true);
        }

        /// <summary>
        /// Sets a tooltip on a control and all of its inner children (such as ModernButton's label).
        /// </summary>
        public static void SetToolTip(ToolTip toolTip, Control control, string text)
        {
            toolTip.SetToolTip(control, text);
            foreach (Control child in control.Controls)
            {
                SetToolTip(toolTip, child, text);
            }
        }

        /// <summary>
        /// Draws subtle horizontal CRT/HUD scanlines inside the given rectangle.
        /// </summary>
        public static void DrawScanlines(Graphics g, Rectangle bounds, int alpha = 14)
        {
            if (!IsDark || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            var prevSmooth = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;
            using var scanPen = new Pen(Color.FromArgb(alpha, 255, 0, 60), 1);
            for (var y = bounds.Top + 2; y < bounds.Bottom; y += 4)
            {
                g.DrawLine(scanPen, bounds.Left, y, bounds.Right, y);
            }
            g.SmoothingMode = prevSmooth;
        }

        /// <summary>
        /// Creates a polygon with 45-degree Cyberpunk chamfered corners on top-left and bottom-right.
        /// </summary>
        public static Point[] GetChamferPoints(Rectangle rect, int cutTopLeft, int cutBottomRight)
        {
            var l = rect.Left;
            var t = rect.Top;
            var r = rect.Right;
            var b = rect.Bottom;
            return new[]
            {
                new Point(l + cutTopLeft, t),
                new Point(r, t),
                new Point(r, b - cutBottomRight),
                new Point(r - cutBottomRight, b),
                new Point(l, b),
                new Point(l, t + cutTopLeft)
            };
        }

        /// <summary>
        /// Call before the first window is created. mode: 0 follows Windows, 1 light, 2 dark.
        /// </summary>
        public static void Initialize(int mode)
        {
#pragma warning disable WFO5001 // Dark mode is still marked experimental
            Application.SetColorMode(mode switch
            {
                1 => SystemColorMode.Classic,
                2 => SystemColorMode.Dark,
                _ => SystemColorMode.System
            });
            IsDark = Application.IsDarkModeEnabled;
#pragma warning restore WFO5001

            // Dialogs are created all over the code, theme every new window once
            Application.Idle += (_, _) =>
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (!s_themedForms.TryGetValue(form, out _))
                    {
                        s_themedForms.Add(form, null);
                        Apply(form);
                    }
                }
            };
        }

        /// <summary>
        /// Applies typography and Cyberpunk 2077 HUD styling to a control and its children.
        /// </summary>
        public static void Apply(Control control)
        {
            ApplyTypography(control);
            if (control is Form or UserControl or Panel or GroupBox or Button or ListView)
            {
                EnableDoubleBuffering(control);
            }

            if (!IsDark)
            {
                if (control is Button lightBtn)
                {
                    lightBtn.FlatStyle = FlatStyle.Flat;
                    lightBtn.FlatAppearance.BorderColor = BorderColor;
                }

                foreach (Control lightChild in control.Controls)
                {
                    Apply(lightChild);
                }
                return;
            }

            if (control is ModernButton button)
            {
                button.DefaultColor = DarkRaised;
                button.HoverColor = DarkHover;
                button.TextColor = DarkText;
            }
            else if (control is Button stdButton)
            {
                stdButton.FlatStyle = FlatStyle.Flat;
                stdButton.FlatAppearance.BorderSize = 0;
                stdButton.FlatAppearance.BorderColor = NeonRed;
                stdButton.FlatAppearance.MouseOverBackColor = DarkHover;
                stdButton.FlatAppearance.MouseDownBackColor = Accent;
                stdButton.BackColor = DarkRaised;
                stdButton.ForeColor = Cyan;
                AttachCyberButtonPaint(stdButton);

                // The paint uses bold capitals, wider than what AutoSize measured
                if (stdButton.AutoSize)
                {
                    using var boldFont = new Font(UiFontFamily, stdButton.Font.Size, FontStyle.Bold, stdButton.Font.Unit);
                    var textWidth = TextRenderer.MeasureText((stdButton.Text ?? string.Empty).ToUpperInvariant(), boldFont).Width;
                    stdButton.MinimumSize = new Size(textWidth + 24, stdButton.MinimumSize.Height);
                }
            }
            else
            {
                if (control is TabPage tabPage)
                {
                    tabPage.UseVisualStyleBackColor = false;
                    tabPage.BackColor = DarkGround;
                    tabPage.ForeColor = DarkText;
                }

                if (control is Form form)
                {
                    form.BackColor = DarkGround;
                    form.ForeColor = DarkText;
                    AttachCyberFormPaint(form);
                }
                else if (control is StatusStrip statusStrip)
                {
                    statusStrip.BackColor = DarkGround;
                    statusStrip.ForeColor = Accent;
                    statusStrip.SizingGrip = false;
                    AttachCyberStatusStripPaint(statusStrip);
                    foreach (ToolStripItem item in statusStrip.Items)
                    {
                        if (item is ToolStripStatusLabel label)
                        {
                            label.ForeColor = label.IsLink || label.Name == "tssl_Info" ? Cyan : Accent;
                        }
                    }
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.BackColor = DarkSurface;
                    groupBox.ForeColor = Cyan;
                    AttachCyberGroupBoxPaint(groupBox);
                }
                else if (control is Panel panel and not ModernValuePicker)
                {
                    if (panel.BorderStyle == BorderStyle.FixedSingle)
                    {
                        panel.BorderStyle = BorderStyle.None;
                        panel.BackColor = DarkGround;
                        AttachCyberPanelPaint(panel);
                    }
                    else if (IsLight(panel.BackColor))
                    {
                        panel.BackColor = DarkGround;
                    }
                }
                else if (control is UserControl uc)
                {
                    uc.BackColor = DarkGround;
                    uc.ForeColor = DarkText;
                }
                else if (control is TextBox or ListBox or TreeView or NumericUpDown or ComboBox)
                {
                    control.BackColor = DarkSurface;
                    control.ForeColor = Cyan;
                    if (control is TextBox tb && tb.BorderStyle == BorderStyle.Fixed3D)
                    {
                        tb.BorderStyle = BorderStyle.FixedSingle;
                    }
                }
                else if (control is ListView lv)
                {
                    lv.BackColor = DarkSurface;
                    lv.ForeColor = DarkText;
                }
                else if (IsLight(control.BackColor))
                {
                    control.BackColor = DarkGround;
                }

                if (control is Label lbl)
                {
                    if (lbl.ForeColor == SystemColors.GrayText)
                    {
                        lbl.ForeColor = DarkMuted;
                    }
                    else if (lbl.Name == "filePathLabel")
                    {
                        lbl.BackColor = DarkSurface;
                        lbl.ForeColor = Cyan;
                        AttachCyberBadgePaint(lbl);
                    }
                    else if (lbl.ForeColor.GetBrightness() < 0.4f)
                    {
                        lbl.ForeColor = DarkText;
                    }
                }
                else if (control is CheckBox or RadioButton)
                {
                    control.ForeColor = Accent;
                }
                else if (control is not GroupBox && control.ForeColor.GetBrightness() < 0.4f)
                {
                    control.ForeColor = DarkText;
                }
            }

            // Owner-draw ListView details headers and rows in Cyberpunk HUD style
            if (control is ListView { View: View.Details, OwnerDraw: false } detailsView)
            {
                detailsView.OwnerDraw = true;
                detailsView.GridLines = false;
                if (detailsView.BorderStyle == BorderStyle.Fixed3D)
                {
                    detailsView.BorderStyle = BorderStyle.FixedSingle;
                }
                detailsView.DrawColumnHeader += DrawDarkColumnHeader;
                detailsView.DrawSubItem += DrawDarkSubItem;

                detailsView.ClientSizeChanged += (_, _) => FillLastColumn(detailsView);
                detailsView.ColumnWidthChanged += (_, e) =>
                {
                    if (e.ColumnIndex < detailsView.Columns.Count - 1)
                    {
                        FillLastColumn(detailsView);
                    }
                };
                FillLastColumn(detailsView);
            }

            // Owner-draw ListBox selection in Cyberpunk Yellow / Crimson
            if (control is ListBox { DrawMode: DrawMode.Normal } listBox and not CheckedListBox)
            {
                listBox.DrawMode = DrawMode.OwnerDrawFixed;
                listBox.DrawItem += DrawDarkListBoxItem;
                // The fixed item height is designed at 96 DPI, make it fit the text
                listBox.ItemHeight = Math.Max(listBox.ItemHeight, TextRenderer.MeasureText("Ag", listBox.Font).Height + 4);
                listBox.GotFocus += (_, _) => listBox.Invalidate();
                listBox.LostFocus += (_, _) => listBox.Invalidate();
            }

            if (control is ListView { VirtualMode: false } listView)
            {
                foreach (ListViewItem item in listView.Items)
                {
                    if (IsLight(item.BackColor))
                    {
                        item.BackColor = DarkSurface;
                    }
                }
            }

            foreach (Control child in control.Controls)
            {
                Apply(child);
            }
        }

        private static void AttachCyberButtonPaint(Button btn)
        {
            if (s_paintedControls.TryGetValue(btn, out _))
            {
                return;
            }
            s_paintedControls.Add(btn, null);

            var isHovered = false;
            var isPressed = false;
            btn.MouseEnter += (_, _) => { isHovered = true; btn.Invalidate(); };
            btn.MouseLeave += (_, _) => { isHovered = false; isPressed = false; btn.Invalidate(); };
            btn.MouseDown += (_, _) => { isPressed = true; btn.Invalidate(); };
            btn.MouseUp += (_, _) => { isPressed = false; btn.Invalidate(); };

            btn.Paint += (_, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(btn.Parent?.BackColor ?? DarkGround);

                var rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
                var cut = Math.Min(7, Math.Max(4, btn.Height / 4));
                var pts = GetChamferPoints(rect, 0, cut);

                var fillColor = !btn.Enabled
                    ? Color.FromArgb(22, 10, 14)
                    : isPressed
                        ? Accent
                        : isHovered
                            ? DarkHover
                            : DarkRaised;

                using (var fillBrush = new SolidBrush(fillColor))
                {
                    g.FillPolygon(fillBrush, pts);
                }

                if (!isPressed && btn.Enabled)
                {
                    DrawScanlines(g, rect, 18);
                }

                // Left high-voltage bar
                var barColor = !btn.Enabled
                    ? Color.FromArgb(90, 30, 45)
                    : isPressed
                        ? NeonRed
                        : isHovered
                            ? Cyan
                            : Accent;
                using (var barBrush = new SolidBrush(barColor))
                {
                    g.FillRectangle(barBrush, 0, 0, isHovered ? 4 : 3, btn.Height);
                }

                // Chamfered neon border
                var borderColor = !btn.Enabled
                    ? Color.FromArgb(90, 30, 45)
                    : isPressed
                        ? NeonRed
                        : isHovered
                            ? Cyan
                            : NeonRed;
                using (var borderPen = new Pen(borderColor, isHovered ? 1.6f : 1f))
                {
                    g.DrawPolygon(borderPen, pts);
                }

                // Corner accent tick on top-right
                if (btn.Enabled)
                {
                    using var tickPen = new Pen(isHovered ? Accent : Cyan, 1.5f);
                    g.DrawLine(tickPen, rect.Right - 8, rect.Top + 1, rect.Right - 1, rect.Top + 1);
                    g.DrawLine(tickPen, rect.Right - 1, rect.Top + 1, rect.Right - 1, rect.Top + 6);
                }

                var textColor = !btn.Enabled
                    ? Color.FromArgb(120, 60, 75)
                    : isPressed
                        ? OnAccent
                        : isHovered
                            ? Accent
                            : Cyan;

                var textBounds = new Rectangle(6, 0, btn.Width - 10, btn.Height);
                using var boldFont = new Font(UiFontFamily, btn.Font.Size, FontStyle.Bold, btn.Font.Unit);
                TextRenderer.DrawText(g, (btn.Text ?? string.Empty).ToUpperInvariant(), boldFont, textBounds, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            };
        }

        private static void AttachCyberGroupBoxPaint(GroupBox gb)
        {
            if (s_paintedControls.TryGetValue(gb, out _))
            {
                return;
            }
            s_paintedControls.Add(gb, null);

            gb.Paint += (_, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(gb.Parent?.BackColor ?? DarkGround);

                var topOffset = 10;
                var cut = 12;
                var rect = new Rectangle(1, topOffset, gb.Width - 3, gb.Height - topOffset - 2);
                if (rect.Width <= 10 || rect.Height <= 10)
                {
                    return;
                }

                var pts = GetChamferPoints(rect, 0, cut);
                using (var surfaceBrush = new SolidBrush(DarkSurface))
                {
                    g.FillPolygon(surfaceBrush, pts);
                }

                DrawScanlines(g, rect, 12);

                // Top header bar strip inside groupbox
                using (var headerStrip = new SolidBrush(Color.FromArgb(45, 255, 0, 60)))
                {
                    g.FillRectangle(headerStrip, rect.Left + 1, rect.Top + 1, rect.Width - 2, 16);
                }

                // Neon Red structural border
                using (var redPen = new Pen(NeonRed, 1.2f))
                {
                    g.DrawPolygon(redPen, pts);
                }

                // Cyan & Yellow HUD corner brackets
                using (var cyanPen = new Pen(Cyan, 2f))
                {
                    // Top-left bracket
                    g.DrawLine(cyanPen, rect.Left, rect.Top + 18, rect.Left, rect.Top);
                    g.DrawLine(cyanPen, rect.Left, rect.Top, rect.Left + 24, rect.Top);

                    // Bottom-left bracket
                    g.DrawLine(cyanPen, rect.Left, rect.Bottom - 12, rect.Left, rect.Bottom);
                    g.DrawLine(cyanPen, rect.Left, rect.Bottom, rect.Left + 16, rect.Bottom);
                }

                using (var yellowPen = new Pen(Accent, 2.2f))
                {
                    // Diagonal chamfer highlight on bottom-right
                    g.DrawLine(yellowPen, rect.Right, rect.Bottom - cut, rect.Right - cut, rect.Bottom);
                }

                // Header badge pill
                var title = "// " + (gb.Text ?? string.Empty).ToUpperInvariant();
                using var headerFont = new Font(UiFontFamily, 9.25f, FontStyle.Bold);
                var titleSize = TextRenderer.MeasureText(g, title, headerFont);
                var badgeRect = new Rectangle(12, 1, titleSize.Width + 14, 19);
                var badgePts = GetChamferPoints(badgeRect, 0, 5);

                using (var badgeFill = new SolidBrush(DarkRaised))
                using (var badgeBorder = new Pen(NeonRed, 1f))
                using (var yellowBar = new SolidBrush(Accent))
                {
                    g.FillPolygon(badgeFill, badgePts);
                    g.FillRectangle(yellowBar, badgeRect.Left, badgeRect.Top, 4, badgeRect.Height);
                    g.DrawPolygon(badgeBorder, badgePts);
                }

                TextRenderer.DrawText(g, title, headerFont, new Rectangle(badgeRect.Left + 7, badgeRect.Top, badgeRect.Width - 8, badgeRect.Height), Cyan,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

                // Micro telemetry tag on top-right of groupbox
                if (gb.Width > badgeRect.Right + 95)
                {
                    using var microFont = new Font(MonoFontFamily, 6.75f, FontStyle.Regular);
                    var tagRect = new Rectangle(rect.Right - 82, rect.Top + 2, 76, 13);
                    TextRenderer.DrawText(g, "NC // SYS.77", microFont, tagRect, DarkMuted,
                        TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };
        }

        private static void AttachCyberPanelPaint(Panel panel)
        {
            if (s_paintedControls.TryGetValue(panel, out _))
            {
                return;
            }
            s_paintedControls.Add(panel, null);

            panel.Padding = new Padding(Math.Max(panel.Padding.Left, 3), Math.Max(panel.Padding.Top, 3), Math.Max(panel.Padding.Right, 3), Math.Max(panel.Padding.Bottom, 3));
            panel.Resize += (_, _) => panel.Invalidate();
            panel.Paint += (_, e) =>
            {
                var g = e.Graphics;
                var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                if (rect.Width <= 4 || rect.Height <= 4)
                {
                    return;
                }

                using var borderPen = new Pen(NeonRed, 1f);
                g.DrawRectangle(borderPen, rect);

                using var cyanPen = new Pen(Cyan, 2f);
                // Top-left & Top-right corner brackets
                g.DrawLine(cyanPen, 0, 0, 22, 0);
                g.DrawLine(cyanPen, 0, 0, 0, 22);
                g.DrawLine(cyanPen, rect.Right - 22, 0, rect.Right, 0);
                g.DrawLine(cyanPen, rect.Right, 0, rect.Right, 22);

                using var yellowPen = new Pen(Accent, 2f);
                // Bottom-left & Bottom-right corner brackets
                g.DrawLine(yellowPen, 0, rect.Bottom - 22, 0, rect.Bottom);
                g.DrawLine(yellowPen, 0, rect.Bottom, 22, rect.Bottom);
                g.DrawLine(yellowPen, rect.Right - 22, rect.Bottom, rect.Right, rect.Bottom);
                g.DrawLine(yellowPen, rect.Right, rect.Bottom - 22, rect.Right, rect.Bottom);
            };
        }

        private static void AttachCyberBadgePaint(Label lbl)
        {
            if (s_paintedControls.TryGetValue(lbl, out _))
            {
                return;
            }
            s_paintedControls.Add(lbl, null);

            lbl.Paint += (_, e) =>
            {
                var g = e.Graphics;
                var rect = new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1);
                using var fill = new SolidBrush(DarkSurface);
                using var border = new Pen(DarkBorder, 1f);
                using var leftTick = new SolidBrush(Accent);
                g.FillRectangle(fill, rect);
                g.FillRectangle(leftTick, 0, 0, 3, lbl.Height);
                g.DrawRectangle(border, rect);

                using var monoFont = new Font(MonoFontFamily, 7.5f, FontStyle.Bold);
                TextRenderer.DrawText(g, "// " + (lbl.Text ?? string.Empty).ToUpperInvariant(), monoFont,
                    new Rectangle(6, 0, lbl.Width - 8, lbl.Height), Cyan,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            };
        }

        private static void AttachCyberFormPaint(Form form)
        {
            if (s_paintedControls.TryGetValue(form, out _))
            {
                return;
            }
            s_paintedControls.Add(form, null);

            form.Resize += (_, _) => form.Invalidate();
            form.Paint += (_, e) =>
            {
                var g = e.Graphics;
                // Top neon yellow + red hazard rule across the window top edge
                using var redPen = new Pen(NeonRed, 2f);
                using var yellowPen = new Pen(Accent, 2f);
                using var cyanPen = new Pen(Cyan, 1f);

                // The layout is designed at 96 DPI, scale the hard-coded positions with the display
                var s = form.DeviceDpi / 96f;

                g.DrawLine(redPen, 0, 1, form.ClientSize.Width, 1);
                g.DrawLine(yellowPen, 12 * s, 1, 188 * s, 1);
                g.DrawLine(cyanPen, 196 * s, 1, 340 * s, 1);

                // Vertical HUD divider line between sidebar and content
                using var dividerPen = new Pen(Color.FromArgb(90, 255, 0, 60), 1f);
                g.DrawLine(dividerPen, 192 * s, 8 * s, 192 * s, form.ClientSize.Height - 26 * s);
            };
        }

        private static void AttachCyberStatusStripPaint(StatusStrip strip)
        {
            if (s_paintedControls.TryGetValue(strip, out _))
            {
                return;
            }
            s_paintedControls.Add(strip, null);

            strip.Paint += (_, e) =>
            {
                using var redPen = new Pen(NeonRed, 1f);
                using var yellowPen = new Pen(Accent, 2f);
                e.Graphics.DrawLine(redPen, 0, 0, strip.Width, 0);
                e.Graphics.DrawLine(yellowPen, 12, 0, 100, 0);
            };
        }

        private static void ApplyTypography(Control control)
        {
            if (control is ModernButton button)
            {
                if (button.TextFont != null && button.TextFont.FontFamily.Name != UiFontFamily.Name)
                {
                    button.TextFont = new Font(UiFontFamily, button.TextFont.Size, button.TextFont.Style, button.TextFont.Unit);
                }
                return;
            }

            if (control is NumericUpDown numUpDown)
            {
                if (numUpDown.Font.FontFamily.Name != MonoFontFamily.Name)
                {
                    numUpDown.Font = new Font(MonoFontFamily, numUpDown.Font.Size, FontStyle.Bold, numUpDown.Font.Unit);
                }
                return;
            }

            if (control.Font != null && control.Font.FontFamily.Name == "Segoe UI" && UiFontFamily.Name != "Segoe UI")
            {
                control.Font = new Font(UiFontFamily, control.Font.Size, control.Font.Style, control.Font.Unit);
            }
        }

        private static void DrawDarkColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using var background = new SolidBrush(DarkRaised);
            using var rightDivider = new Pen(DarkBorder, 1f);
            using var bottomRule = new Pen(NeonRed, 2f);
            using var yellowTick = new SolidBrush(Accent);

            e.Graphics.FillRectangle(background, e.Bounds);
            DrawScanlines(e.Graphics, e.Bounds, 15);
            e.Graphics.FillRectangle(yellowTick, e.Bounds.Left, e.Bounds.Top + 4, 2, e.Bounds.Height - 8);
            e.Graphics.DrawLine(rightDivider, e.Bounds.Right - 1, e.Bounds.Top + 2, e.Bounds.Right - 1, e.Bounds.Bottom - 3);
            e.Graphics.DrawLine(bottomRule, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            var textBounds = Rectangle.Inflate(e.Bounds, -7, 0);
            var headerText = (e.Header?.Text ?? string.Empty).ToUpperInvariant();
            // Shrink the font until it fits the header row, so it stays readable on any display scaling
            var headerSize = e.Font?.Size ?? 9f;
            var headerFont = new Font(UiFontFamily, headerSize, FontStyle.Bold);
            while (headerSize > 6f && TextRenderer.MeasureText(e.Graphics, "Ag", headerFont).Height > e.Bounds.Height - 4)
            {
                headerSize -= 0.5f;
                headerFont.Dispose();
                headerFont = new Font(UiFontFamily, headerSize, FontStyle.Bold);
            }
            using var fittedFont = headerFont;
            TextRenderer.DrawText(e.Graphics, headerText, headerFont, textBounds, Cyan,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // The bold capitals are wider than the widths the designers chose; widen the column once, after this paint
            var needed = TextRenderer.MeasureText(e.Graphics, headerText, headerFont).Width + 16;
            if (e.Header != null && e.Bounds.Width < needed && sender is ListView headerOwner && e.ColumnIndex < headerOwner.Columns.Count - 1)
            {
                var column = e.Header;
                headerOwner.BeginInvoke(() => column.Width = Math.Max(column.Width, needed));
            }
        }

        private static void DrawDarkSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var listView = (ListView)sender;

            if (!e.Item.Selected)
            {
                e.DrawDefault = true;
                return;
            }

            var focused = listView.Focused;
            var textColor = focused ? OnAccent : Cyan;
            using var background = new SolidBrush(focused ? Accent : DarkHover);
            e.Graphics.FillRectangle(background, e.Bounds);

            if (e.ColumnIndex == 0)
            {
                using var edgeBar = new SolidBrush(focused ? NeonRed : Accent);
                e.Graphics.FillRectangle(edgeBar, e.Bounds.Left, e.Bounds.Top, 4, e.Bounds.Height);
            }

            var textBounds = Rectangle.Inflate(e.Bounds, -5, 0);
            if (listView.CheckBoxes && e.ColumnIndex == 0)
            {
                var state = e.Item.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
                var glyph = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
                var glyphLocation = new Point(e.Bounds.X + 6, e.Bounds.Y + (e.Bounds.Height - glyph.Height) / 2);
                CheckBoxRenderer.DrawCheckBox(e.Graphics, glyphLocation, state);

                textBounds = Rectangle.FromLTRB(glyphLocation.X + glyph.Width + 4, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
            }

            using var rowFont = new Font(e.Item.Font ?? listView.Font, FontStyle.Bold);
            TextRenderer.DrawText(e.Graphics, e.SubItem?.Text, rowFont, textBounds, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private static void DrawDarkListBoxItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            var listBox = (ListBox)sender;
            var selected = (e.State & DrawItemState.Selected) != 0;
            var focused = listBox.Focused;

            using var background = new SolidBrush(!selected ? listBox.BackColor : focused ? Accent : DarkHover);
            e.Graphics.FillRectangle(background, e.Bounds);

            if (selected)
            {
                using var leftBar = new SolidBrush(focused ? NeonRed : Accent);
                e.Graphics.FillRectangle(leftBar, e.Bounds.Left, e.Bounds.Top, 4, e.Bounds.Height);
            }

            var textColor = selected ? (focused ? OnAccent : Cyan) : listBox.ForeColor;
            var baseFont = e.Font ?? listBox.Font;
            if (selected)
            {
                using var boldFont = new Font(baseFont, FontStyle.Bold);
                TextRenderer.DrawText(e.Graphics, listBox.GetItemText(listBox.Items[e.Index]), boldFont, Rectangle.Inflate(e.Bounds, -6, 0), textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, listBox.GetItemText(listBox.Items[e.Index]), baseFont, Rectangle.Inflate(e.Bounds, -6, 0), textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        private static void FillLastColumn(ListView listView)
        {
            if (listView.Columns.Count == 0)
            {
                return;
            }

            var last = listView.Columns[^1];
            var othersWidth = 0;
            for (var i = 0; i < listView.Columns.Count - 1; i++)
            {
                othersWidth += listView.Columns[i].Width;
            }

            var width = listView.ClientSize.Width - othersWidth - 1;
            if (width > 40 && last.Width != width)
            {
                last.Width = width;
            }
        }

        private static bool IsLight(Color color) => color.A > 0 && color.GetBrightness() > 0.85f;
    }
}
