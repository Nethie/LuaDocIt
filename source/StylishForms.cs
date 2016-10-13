using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace StylishForms
{
    class StylishForm : Form
    {
        public Color backgroundColor = Color.FromArgb(30, 30, 30);
        public string backgroundImage = null;
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
            Pen pen = new Pen(backgroundColor);
            SolidBrush brush = new SolidBrush(backgroundColor);

            e.Graphics.FillRectangle(brush, rect);
            e.Graphics.DrawRectangle(pen, rect);

            if (backgroundImage != null)
            {
                Image background = Image.FromFile(backgroundImage);
                e.Graphics.DrawImage(background, 0, 0, Size.Width - 1, Size.Height - 1);
            }
        }
    }

    class StylishTextEntry : TextBox
    {
        public StylishTextEntry()
        {
            //BackColor = Color.Transparent;
            Size = new Size(240, 40);
        }

        public Color OutlineColor = Color.FromArgb(40, 40, 40);
        public Color Background = Color.FromArgb(25, 25, 25);
        protected override void OnPaint(PaintEventArgs e)
        {
            Pen Outline = new Pen(OutlineColor, 1);
            SolidBrush Fill = new SolidBrush(Background);
            Rectangle Rect = new Rectangle(0, 0, Width - 1, Height - 1);

            e.Graphics.FillRectangle(Fill, Rect);
            e.Graphics.DrawRectangle(Outline, Rect);
        }
    }

    class StylishComboBox : ComboBox
    {
        //placeholder
    }

    class StylishButton : UserControl
    {
        public StylishButton()
        {
            //Override inherited values
            BackColor = Color.Transparent;
            Size = new Size(240, 40);
            Text = "Stylish Button";
        }

        //Register modifiers
        public Color Background = Color.Black;
        public Color SideGround = Color.Gray;
        public Color IconBackground = Color.LightGray;
        public Color TextColor = Color.Black;
        public Font TextFont = new Font("Arial", 16);
        public StringFormat TextFormat = new StringFormat();
        public int SideHeight = 4;
        public Image IconImage = null;

        public void SetIcon(string path)
        {
            IconImage = Image.FromFile(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Background
            Pen Outline = new Pen(Background);
            SolidBrush Fill = new SolidBrush(Background);
            Rectangle Rect = new Rectangle(0, 0, Width - 1, Height - 1);

            e.Graphics.FillRectangle(Fill, Rect);
            e.Graphics.DrawRectangle(Outline, Rect);

            //SideGround
            Rect.Y = Height - SideHeight;
            Rect.Height = SideHeight;
            Outline.Color = SideGround;
            Fill.Color = SideGround;

            e.Graphics.DrawRectangle(Outline, Rect);
            e.Graphics.FillRectangle(Fill, Rect);

            //Icon
            if (IconImage != null)
            {
                Rect.Y = 0;
                Rect.Width = Rect.Height = (Height - SideHeight) - 1;
                Outline.Color = Fill.Color = IconBackground;

                e.Graphics.FillRectangle(Fill, Rect);
                e.Graphics.DrawRectangle(Outline, Rect);

                e.Graphics.DrawImage(IconImage, (Height - SideHeight) / 2 - IconImage.Height / 2, (Height - SideHeight) / 2 - IconImage.Height / 2);
            }

            //Text
            Fill.Color = TextColor;
            PointF TextPos = new PointF(Width / 2, (Height / 2) - SideHeight - TextFont.SizeInPoints / 2);
            TextFormat.Alignment = (IconImage != null) ? StringAlignment.Near : StringAlignment.Center;
            TextPos.X = (IconImage != null) ? (Height - SideHeight) + (Height - SideHeight) / 2 - IconImage.Height / 2 : (Width / 2);
            e.Graphics.DrawString(Text, TextFont, Fill, TextPos, TextFormat);
        }
    }
}
