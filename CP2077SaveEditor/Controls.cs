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
            textLabel.Click += TextClick;
            textLabel.MouseDown += TextMouseDown;
            textLabel.MouseUp += TextMouseUp;
        }

        private void TextClick(object sender, EventArgs e)
        {
            base.OnClick(e);
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
        }
    }
}
