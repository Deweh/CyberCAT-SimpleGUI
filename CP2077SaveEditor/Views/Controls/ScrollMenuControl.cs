using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class ScrollMenuControl : UserControl
    {
        private const int BaseStepHeight = 44;
        private const int BaseArrowHeight = 20;
        private const int MinStepHeight = 30;
        private bool _inLayout;

        // The layout is designed at 96 DPI, scale it with the display
        private int _step = BaseStepHeight;
        private int StepHeight => _step;
        private readonly List<ModernButton> _buttons = new();

        public ScrollMenuControl()
        {
            InitializeComponent();
        }

        public ModernButton AddButton(string text, string icon, EventHandler eventHandler)
        {
            var button = new ModernButton()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ClickEffectEnabled = true,
                DefaultColor = Color.White,
                HoverColor = Color.DarkGray,
                Location = new Point(-1, _buttons.Count * StepHeight - 1),
                Size = new Size(pnl_Menu.Width + 1, StepHeight + 1),
                Text = text,
                IconGlyph = icon,
                TextColor = Color.Black,
                TextFont = new Font("Segoe UI", 10.25F, FontStyle.Regular, GraphicsUnit.Point),
            };
            button.Click += eventHandler;
            button.MouseWheel += pnl_Menu_MouseWheel;

            _buttons.Add(button);
            pnl_Menu.Controls.Add(button);
            return button;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            if (_inLayout || pnl_Menu == null)
            {
                return;
            }

            _inLayout = true;
            try
            {
                // Fit everything on screen if possible, the items shrink down to a minimum height.
                // The scroll arrows only appear when the control is too small even for that.
                var scale = DeviceDpi / 96f;
                var arrow = (int)(BaseArrowHeight * scale);
                var baseStep = (int)(BaseStepHeight * scale);
                var minStep = (int)(MinStepHeight * scale);
                var count = Math.Max(1, _buttons.Count);
                var needsScroll = count * minStep > Height;

                btn_ScrollUp.Visible = btn_ScrollDown.Visible = needsScroll;
                if (needsScroll)
                {
                    _step = minStep;
                    btn_ScrollUp.SetBounds(0, 0, Width, arrow);
                    btn_ScrollDown.SetBounds(0, Height - arrow, Width, arrow);
                    pnl_Menu.SetBounds(0, arrow, Width, Math.Max(0, Height - 2 * arrow));
                }
                else
                {
                    _step = Math.Clamp(Height / count, minStep, baseStep);
                    pnl_Menu.SetBounds(0, 0, Width, Height);
                }

                if (_buttons.Count > 0)
                {
                    var step = StepHeight;
                    var minOffset = Math.Min(0, pnl_Menu.Height - _buttons.Count * step);
                    var offset = Math.Clamp(_buttons[0].Top + 1, minOffset, 0);
                    for (var i = 0; i < _buttons.Count; i++)
                    {
                        _buttons[i].SetBounds(-1, offset + i * step - 1, pnl_Menu.Width + 1, step + 1);
                    }
                }
            }
            finally
            {
                _inLayout = false;
            }
        }

        public void SelectButton(ModernButton selectedButton)
        {
            foreach (var button in _buttons)
            {
                button.IsSelected = button == selectedButton;
            }

            if (selectedButton != null)
            {
                while (selectedButton.Top < -1 && pnl_Menu.Controls[0].Top < -1)
                {
                    ScrollBy(StepHeight);
                }

                while (selectedButton.Bottom > pnl_Menu.Height + 1 && pnl_Menu.Controls[^1].Bottom > pnl_Menu.Height)
                {
                    ScrollBy(-StepHeight);
                }
            }
        }

        private void ScrollBy(int offset)
        {
            if (pnl_Menu.Controls.Count == 0)
            {
                return;
            }

            if (offset > 0 && pnl_Menu.Controls[0].Location.Y + offset > -1)
            {
                return;
            }

            if (offset < 0 && pnl_Menu.Controls[^1].Location.Y + StepHeight <= pnl_Menu.Height)
            {
                return;
            }

            foreach (Control control in pnl_Menu.Controls)
            {
                control.Location = new Point(control.Location.X, control.Location.Y + offset);
            }
        }

        private void btn_ScrollUp_Click(object sender, EventArgs e)
        {
            ScrollBy(StepHeight);
        }

        private void pnl_Menu_MouseWheel(object sender, MouseEventArgs eventArgs)
        {
            if (eventArgs.Delta > 0)
            {
                ScrollBy(StepHeight);
            }

            if (eventArgs.Delta < 0)
            {
                ScrollBy(-StepHeight);
            }
        }

        private void btn_ScrollDown_Click(object sender, EventArgs e)
        {
            ScrollBy(-StepHeight);
        }
    }
}
