using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace DHS.UI
{
    class DarkToolTip : ToolTip
    {

        private Font font = new Font("Segoe UI Light", 12);

        public DarkToolTip()
        {
            this.OwnerDraw = true;
            this.BackColor = Color.FromArgb(45, 45, 45);
            this.Draw += (sender, e) =>
            {
                e.DrawBackground();
                e.DrawBorder();
                e.Graphics.DrawString(e.ToolTipText, font, Brushes.White, new PointF(8, 5));
            };
            this.Popup += (sender, e) =>
            {
                Size tooltipSize = TextRenderer.MeasureText(this.GetToolTip(e.AssociatedControl), font);
                e.ToolTipSize = new Size(tooltipSize.Width + 12, tooltipSize.Height + 12);
            };
        }

    }
}
