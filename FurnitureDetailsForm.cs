using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Globalization;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class FurnitureDetailsForm : BackableForm
    {
        private Furniture _Item;

        public FurnitureDetailsForm(Root root, IParentForm parent, Furniture item) : base(root, parent, item.Name)
        {
            _Item = item;
            if (Root.CurrentStaff.IsManager())
            {
                _This.InsertActionButtons(Root.GetMsg("edit.title"), Properties.Resources.edit, EditDetails);
                _This.InsertActionButtons(Root.GetMsg("edit.delete-item"), Properties.Resources.delete, DeleteItem);
            }
            int borderSpacing = 12;
            Font fieldFont = new Font("Segoe UI Light", 15);
            // left side description pane
            Panel leftSide = new Panel();
            leftSide.AutoSize = true;
            leftSide.Width = (int)(_This.ContentPanel.Width * .5);
            _This.ContentPanel.Controls.Add(leftSide);
            PictureBox pic = new PictureBox();
            pic.Image = Properties.Resources.loader_medium;
            pic.ImageLocation = _Item.Photo.AbsoluteUri;
            pic.Size = new Size(200, 200);
            pic.Paint += (sender, e) => {
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, pic.Width, pic.Height), Color.FromArgb(64, 128, 128, 128), ButtonBorderStyle.Solid);
            };
            pic.SizeMode = PictureBoxSizeMode.CenterImage;
            pic.LoadCompleted += (sender, e) => {
                pic.SizeMode = PictureBoxSizeMode.Zoom;
            };
            leftSide.Controls.Add(pic);
            pic.Location = new Point(leftSide.Width / 2 - pic.Width / 2, borderSpacing * 2);
            // description
            Label desc = new Label();
            desc.AutoSize = true;
            desc.MaximumSize = new Size(leftSide.Width, _This.ContentPanel.Height - _This.ContentPanel.Top - pic.Height - pic.Top * 2);
            desc.Font = fieldFont;
            desc.ForeColor = Color.FromArgb(204, 204, 204);
            desc.Padding = new Padding(borderSpacing * 2, borderSpacing * 4, borderSpacing * 2, borderSpacing * 2);
            desc.Text = _Item.Description;
            leftSide.Controls.Add(desc);
            desc.Location = new Point(leftSide.Width / 2 - desc.Width / 2, _This.ContentPanel.Top + pic.Height - pic.Top * 2);
            leftSide.Top = _This.ContentPanel.Height / 2 - leftSide.Height / 2;
            // right side details pane
            Panel rightSide = new Panel();
            rightSide.Left = leftSide.Width;
            rightSide.Size = new Size(_This.ContentPanel.Width - rightSide.Left, _This.ContentPanel.Height);
            // textboxes for _Item k
            Label kes = new Label();
            kes.TextAlign = ContentAlignment.MiddleRight;
            kes.Size = new Size((int)(rightSide.Width * .4) - borderSpacing, rightSide.Height);
            kes.Font = fieldFont;
            kes.Text = Root.GetMsg("edit.id") + "\n\n" +
                Root.GetMsg("edit.dimen") + "\n\n" +
                Root.GetMsg("edit.price") + "\n\n" +
                Root.GetMsg("edit.discount") + "\n\n" +
                Root.GetMsg("edit.quantity") + "\n\n" +
                Root.GetMsg("edit.rl") + "\n\n" +
                Root.GetMsg("edit.sl") + "\n\n" +
                Root.GetMsg("edit.category") + "\n\n" +
                Root.GetMsg("edit.subcategory") + "\n\n" +
                Root.GetMsg("edit.date");
            rightSide.Controls.Add(kes);
            // textboxes for _Item v
            Label ves = new Label();
            ves.TextAlign = ContentAlignment.MiddleLeft;
            ves.ForeColor = Color.FromArgb(204, 204, 204);
            ves.Font = fieldFont;
            ves.Size = new Size((int)(rightSide.Width * .6) - borderSpacing, rightSide.Height);
            ves.Text = _Item.ID + "\n\n" +
                _Item.Dimension.Replace("x", " × ") + "\n\n" +
                _Item.Price.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")) + "\n\n" +
                _Item.Discount + "%" + "\n\n" +
                _Item.Quantity + "\n\n" +
                _Item.ReorderLevel + "\n\n" +
                _Item.ShelfLocation + "\n\n" +
                _Item.Category + "\n\n" +
                (String.IsNullOrEmpty(_Item.SubCategory) ? Root.GetMsg("ui.not-set") : _Item.SubCategory) + "\n\n" +
                _Item.Date.ToString("yyyy-MM-dd HH:mm");
            ves.Left = kes.Width + borderSpacing * 2;
            rightSide.Controls.Add(ves);
            _This.ContentPanel.Controls.Add(rightSide);
        }

        private void EditDetails(object sender, EventArgs e)
        {
            new EditItemForm(_Root, this, _Item).Show();
        }

        private MetroButton CreateControl(string text, Color btnColor, EventHandler handler)
        {
            MetroButton btn = new MetroButton();
            btn.ForeColor = btnColor;
            btn.Text = text;
            btn.Click += handler;
            return btn;
        }

        private void DeleteItem(object sender, EventArgs e)
        {
            DialogResult delete = MessageBox.Show(Root.GetMsg("edit.confirm-delete"), Root.GetMsg("ui.warn"), MessageBoxButtons.YesNo);
            if(delete == DialogResult.Yes)
            {
                if (XQL.RemoveFurniture(_Item.ID) > 0)
                    BackToManage(reloadProducts: true);
            }
        }

    }
}
