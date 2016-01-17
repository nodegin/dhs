using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace DHS.UI
{
    public class MetroCheckBox : CheckBox
    {
        public MetroCheckBox() : base()
        {
            this.Location = new Point(1, 1);
            this.Size = new Size(24, 24);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(77, 77, 77)), new Rectangle(Point.Empty, this.Size));
            if (this.Checked)
            {
                pe.Graphics.DrawImage(Properties.Resources.tick, new Rectangle(1, 1, this.Width - 2, this.Height - 2));
            }
        }
    }
}