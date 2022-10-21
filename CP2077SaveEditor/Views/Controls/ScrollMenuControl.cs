using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2077SaveEditor.Views.Controls
{
    public partial class ScrollMenuControl : UserControl
    {
        private readonly List<ModernButton> _buttons = new();

        public ScrollMenuControl()
        {
            InitializeComponent();
        }

        public void AddButton(string text, EventHandler eventHandler)
        {
            var button = new ModernButton()
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ClickEffectEnabled = true,
                DefaultColor = Color.White,
                HoverColor = Color.DarkGray,
                Location = new Point(-1, _buttons.Count * 60 - 1),
                Size = new Size(pnl_Menu.Width + 1, 61),
                Text = text,
                TextColor = Color.Black,
                TextFont = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point),
            };
            button.Click += eventHandler;

            _buttons.Add(button);
            pnl_Menu.Controls.Add(button);
        }

        private void Scroll(int offset)
        {
            if (pnl_Menu.Controls[0].Location.Y + offset == 59)
            {
                return;
            }

            if (pnl_Menu.Controls[^1].Location.Y + offset == -61)
            {
                return;
            }

            if (offset < 0 && pnl_Menu.Controls[^1].Location.Y + 60 < pnl_Menu.Height)
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
            Scroll(60);
        }

        private void pnl_Menu_MouseWheel(object sender, MouseEventArgs eventArgs)
        {
            if (eventArgs.Delta > 0)
            {
                Scroll(60);
            }

            if (eventArgs.Delta < 0)
            {
                Scroll(-60);
            }
        }

        private void btn_ScrollDown_Click(object sender, EventArgs e)
        {
            Scroll(-60);
        }
    }
}
