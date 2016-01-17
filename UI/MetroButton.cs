using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DHS.UI
{
    class MetroButton : Button
    {

        private bool _Disabled = false;
        public bool Disabled {
            get { return _Disabled; }
            set { _Disabled = value; Invalidate(); }
        }

        public MetroButton(int border = 1, int fontSize = 13) : base()
        {
            base.ForeColor = Color.White;
            base.Font = new Font("Segoe UI Light", fontSize);
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.BorderColor = Color.Gray;
            base.FlatAppearance.BorderSize = border;
            base.TabStop = false;
            base.GotFocus += (sender, e) =>
            {
                base.NotifyDefault(false);
            };
        }

        protected override void OnClick(EventArgs e)
        {
            if (Disabled) return;
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (this.Disabled)
            {
                pe.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 34, 34, 34)), pe.ClipRectangle);
            }
        }

    }
}
