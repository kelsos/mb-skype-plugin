using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    internal class SquareButton : Button
    {
        private bool _mousedown;
        private bool _mouseenter;

        public SquareButton()
        {
            FontColor = Color.Black;
            ButtonColor = Color.Gray;
            _mousedown = false;
            _mouseenter = false;
        }

        [Description("Sets Gets the text color."), Category("Values"), DefaultValue(typeof (Color), "Black"),
         Browsable(true)]
        public Color FontColor { get; set; }

        [Description("Sets or gets the button color."), Category("Values"), DefaultValue(typeof (Color), "Gray"),
         Browsable(true)]
        public Color ButtonColor { get; set; }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var outerRectangle = new Rectangle(0, 0, Width, Height);
            SolidBrush solid;
            RectangleF rf;
            StringFormat sf;
            Graphics g;
            SolidBrush brush;
            if (_mousedown == false)
            {
                g = pevent.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                outerRectangle = new Rectangle(0, 0, Width, Height);
                using (solid = new SolidBrush(ControlPaint.Light(ButtonColor)))
                {
                    g.FillRectangle(solid, outerRectangle);
                }
                using (brush = new SolidBrush(FontColor))
                {
                    rf = new RectangleF(0, 0, Width - 1, Height - 1);
                    using (
                        sf =
                        new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center})
                    {
                        g.DrawString(Text, new Font("tahoma", 10.0f), brush, rf, sf);
                    }
                }
            }

            else
            {
                g = pevent.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (solid = new SolidBrush(ControlPaint.Light(ButtonColor)))
                {
                    g.FillRectangle(solid, outerRectangle);
                }
                using (brush = new SolidBrush(FontColor))
                {
                    rf = new RectangleF(0, 0, Width - 1, Height - 1);
                    using (sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        g.DrawString(Text, new Font("tahoma", 10.0f), brush, rf, sf);
                    }
                }
            }

            if (_mouseenter != true) return;

            outerRectangle = new Rectangle(0, 0, Width, Height);
            solid = new SolidBrush(ControlPaint.LightLight(ButtonColor));
            g.FillRectangle(solid, outerRectangle);
            using (brush = new SolidBrush(FontColor))
            {
                rf = new RectangleF(0, 0, Width - 1, Height - 1);
                sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
                g.DrawString(Text, new Font("tahoma", 10.0f), brush, rf, sf);
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            _mousedown = true;
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _mousedown = false;
            base.OnMouseUp(mevent);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _mouseenter = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _mouseenter = false;
            base.OnMouseLeave(e);
        }
    }
}