using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace MusicBeePlugin
{
   class SquareButton : Button
   {
      bool mousedown = false;
      bool mouseenter = false;

      Color _textColor;
      Color _buttonColor;

      public SquareButton()
         : base()
      {
         _textColor = Color.Black;
         _buttonColor = Color.Gray;
      }

      [Description("Sets Gets the text color."),
       Category("Values"),
       DefaultValue(typeof(Color), "Black"),
       Browsable(true)]
      public Color FontColor
      {
         get
         {
            return _textColor;
         }
         set
         {
            _textColor = value;
         }
      }
      [Description("Sets or gets the button color."),
      Category("Values"),
      DefaultValue(typeof(Color), "Gray"),
      Browsable(true)]
      public Color ButtonColor
      {
         get
         {
            return _buttonColor;
         }
         set
         {
            _buttonColor = value;
         }
      }

      protected override void OnPaint(PaintEventArgs pevent)
      {
         if (mousedown == false)
         {
				Graphics g = pevent.Graphics;
				g.SmoothingMode = SmoothingMode.AntiAlias;
				Rectangle outerRectangle = new Rectangle(0, 0, this.Width, this.Height);
				SolidBrush solid = new SolidBrush(ControlPaint.Light(_buttonColor));
				g.FillRectangle(solid, outerRectangle);
				SolidBrush Brush = new SolidBrush(_textColor);
				RectangleF rf = new RectangleF(0, 0, this.Width - 1, this.Height - 1);
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.DrawString(this.Text, new Font("tahoma", 10.0f), Brush, rf, sf);
         }

         else
         {
				Graphics g = pevent.Graphics;
				g.SmoothingMode = SmoothingMode.AntiAlias;
				Rectangle outerRectangle = new Rectangle(0, 0, this.Width, this.Height);
				SolidBrush solid = new SolidBrush(ControlPaint.Light(_buttonColor));
				g.FillRectangle(solid, outerRectangle);
				SolidBrush Brush = new SolidBrush(_textColor);
				RectangleF rf = new RectangleF(0, 0, this.Width-1, this.Height-1);
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.DrawString(this.Text, new Font("tahoma", 10.0f), Brush, rf, sf);
         }

         if (mouseenter == true)
         {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle outerRectangle = new Rectangle(0, 0, this.Width, this.Height);
            SolidBrush solid = new SolidBrush(ControlPaint.LightLight(_buttonColor));
            g.FillRectangle(solid, outerRectangle);
            SolidBrush Brush = new SolidBrush(_textColor);
            RectangleF rf = new RectangleF(0, 0, this.Width-1, this.Height - 1);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(this.Text, new Font("tahoma", 10.0f), Brush, rf, sf);
         }
      }

      protected override void OnMouseDown(MouseEventArgs mevent)
      {
         this.mousedown = true;
         base.OnMouseDown(mevent);
      }

      protected override void OnMouseUp(MouseEventArgs mevent)
      {
         this.mousedown = false;
         base.OnMouseUp(mevent);
      }

      protected override void OnMouseEnter(EventArgs e)
      {
         this.mouseenter = true;
         base.OnMouseEnter(e);
      }

      protected override void OnMouseLeave(EventArgs e)
      {
         this.mouseenter = false;
         base.OnMouseLeave(e);
      }

   }
}
