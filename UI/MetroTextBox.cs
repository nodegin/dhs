using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace DHS.UI
{
    class MetroTextBox : TextBox
    {
        public MetroTextBox(int width, int height, bool multiline = false)
        {
            this.BackColor = Color.FromArgb(77, 77, 77);
            this.BorderStyle = BorderStyle.None;
            this.MinimumSize = new Size(width, height);
            this.MaximumSize = new Size(width, height);
            this.Font = new Font("Segoe UI Light", 16);
            this.ForeColor = Color.White;
            this.Width = width;
            this.Multiline = multiline;

            if (!multiline)
            {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.Height = height;
                int heightWithBorder = this.ClientRectangle.Height;
                this.BorderStyle = BorderStyle.None;
                this.AutoSize = false;
                this.Height = heightWithBorder;
            }
            else this.Height = height;
        }
    }
}
