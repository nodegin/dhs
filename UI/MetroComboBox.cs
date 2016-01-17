using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DHS.UI
{
    public partial class MetroComboBox : ComboBox
    {

        private Brush BorderBrush = new SolidBrush(Color.FromArgb(77, 77, 77));
        private Brush DropButtonBrush = new SolidBrush(Color.FromArgb(77, 77, 77));
        private Timer timer = new Timer();

        public MetroComboBox()
        {
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Font = new Font("Segoe UI Light", 13);
            this.ForeColor = Color.White;
            this.BackColor = Color.FromArgb(77, 77, 77);
            this.KeyPress += (sender, e) => { e.Handled = true; };
            // Important to fix c# NullReference bug
            this.DropDownClosed += (sender, e) => {
                if (this.Items.Count < 1) return;
                if (this.SelectedItem == null) this.SelectedIndex = 0;
            };
            // Do not modify
            this.SelectedIndexChanged += (sender, e) =>
            {
                timer.Enabled = true;
            };
            timer.Tick += (sender, e) =>
            {
                // Remove highlight and disable timer
                this.Select(0, 0);
                timer.Enabled = false;
            };
            this.Enter += (sender, e) =>
            {
                // Enable the time so that the Highlight can be removed
                timer.Enabled = true;
            };
            this.Disposed += (sender, e) =>
            {
                timer.Stop();
            };
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case 0xf:
                    Graphics g = this.CreateGraphics();
                    g.FillRectangle(BorderBrush, this.ClientRectangle);

                    //Create the path for the arrow
                    System.Drawing.Drawing2D.GraphicsPath pth = new System.Drawing.Drawing2D.GraphicsPath();
                    PointF TopLeft = new PointF(this.Width - 13, (this.Height - 5) / 2);
                    PointF TopRight = new PointF(this.Width - 6, (this.Height - 5) / 2);
                    PointF Bottom = new PointF(this.Width - 9, (this.Height + 2) / 2);
                    pth.AddLine(TopLeft, TopRight);
                    pth.AddLine(TopRight, Bottom);

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    //Draw the arrow
                    g.FillPath(new SolidBrush(Color.White), pth);

                    break;
                default:
                    break;
            }
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            this.Invalidate();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}