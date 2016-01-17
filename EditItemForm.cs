using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Globalization;
using System.IO;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class EditItemForm : BackableForm
    {

        private Furniture _Item;
        private int FieldOffsetY = 0;
        private int FieldHeight = 36;
        private int FieldPadding = 24;

        private Panel BoxGroup;
        private TextBox BoxName, BoxDesc, BoxDimen1, BoxDimen2, BoxDimen3, BoxPrice, BoxDiscnt,
            BoxQty, BoxReordrLv, BoxShelf, BoxPhoto;
        private ComboBox BoxCtgy, BoxSubCtgy;
        private MetroButton BtnSubmit;

        public EditItemForm(Root root, IParentForm parent, Furniture data) : base(root, parent, Root.GetMsg("edit.title"))
        {
            _Item = data;
            BoxGroup = new Panel();
            BoxGroup.AutoSize = true;
            ScrollPanel SP = new ScrollPanel();
            SP.Size = new Size(_This.ContentPanel.Width, _This.ContentPanel.Height);
            SP.AutoScroll = true;
            // create textboxes input
            BoxName = InsertField(Root.GetMsg("edit.name"), null, null, null);
            BoxDesc = InsertField(Root.GetMsg("edit.desc"), null, 160, null);
            BoxDimen1 = InsertField(Root.GetMsg("edit.dimen"), Validation.FloatField, null, 1);
            BoxDimen2 = InsertField(null, Validation.FloatField, null, 2);
            BoxDimen3 = InsertField(null, Validation.FloatField, null, 3);
            BoxPrice = InsertField(Root.GetMsg("edit.price"), (sender, e) => { Validation.NumberField(sender, e, 7); }, null, null);
            BoxDiscnt = InsertField(Root.GetMsg("edit.discount"), (sender, e) => { Validation.NumberField(sender, e, 2); }, null, null);
            BoxQty = InsertField(Root.GetMsg("edit.quantity"), (sender, e) => { Validation.NumberField(sender, e, 4); }, null, null);
            BoxReordrLv = InsertField(Root.GetMsg("edit.rl"), (sender, e) => { Validation.NumberField(sender, e, 4); }, null, null);
            BoxShelf = InsertField(Root.GetMsg("edit.sl"), Validation.ShelfLocationField, null, null);
            BoxCtgy = InsertDropDown(Root.GetMsg("edit.category"));
            BoxCtgy.SelectedIndexChanged += (sender, e) =>
            {
                int id = (int)((sender as ComboBox).SelectedItem as ComboItem).Value;
                BoxSubCtgy.Items.Clear();
                BoxSubCtgy.Items.AddRange(XQL.GetSubcategoeiesForDropDown(id));
                BoxSubCtgy.SelectedIndex = 0;
            };
            BoxSubCtgy = InsertDropDown(Root.GetMsg("edit.subcategory"));
            BoxCtgy.Items.Clear();
            BoxCtgy.Items.AddRange(XQL.GetCategoeiesForDropDown());
            BoxCtgy.SelectedIndex = 0;
            BoxPhoto = InsertField(Root.GetMsg("edit.photo"), null, null, null);
            BoxName.Text =      _Item.Name;
            BoxDesc.Text =      _Item.Description;
            string[] dimens =   _Item.Dimension.Split('x');
            BoxDimen1.Text = dimens[0];
            BoxDimen2.Text = dimens[1];
            BoxDimen3.Text = dimens[2];
            BoxPrice.Text =     _Item.Price.ToString();
            BoxDiscnt.Text =    _Item.Discount.ToString();
            BoxQty.Text =       _Item.Quantity.ToString();
            BoxReordrLv.Text =  _Item.ReorderLevel.ToString();
            BoxShelf.Text =     _Item.ShelfLocation.ToString();
            BoxCtgy.Text =      _Item.Category.ToString();
            BoxSubCtgy.Text =   _Item.SubCategory.ToString();
            BoxPhoto.Text =     _Item.Photo.AbsoluteUri;
            // add / update button
            BtnSubmit = new MetroButton();
            BtnSubmit.AutoSize = true;
            BtnSubmit.Click += HandleSubmit;
            BtnSubmit.Text = Root.GetMsg("edit.btn-update");
            BoxGroup.Controls.Add(BtnSubmit);
            BtnSubmit.Location = new Point((BoxPhoto.Left + BoxPhoto.Width) - BtnSubmit.Width, (BoxPhoto.Top + BoxPhoto.Height) + FieldPadding * 2);
            // align box group to middle
            SP.Controls.Add(BoxGroup);
            BoxGroup.Margin = new Padding((_This.ContentPanel.Width - SystemInformation.VerticalScrollBarWidth) / 2 - BoxGroup.Width / 2, 48, 0, 48);
            _This.ContentPanel.Controls.Add(SP);
        }

        private TextBox InsertField(string name, KeyPressEventHandler validation, int? height, int? wDimen)
        {
            Label lbl = new Label();
            int whichDimen = wDimen ?? 0;
            // Label
            if (whichDimen <= 1)
            {
                lbl.ForeColor = Color.White;
                lbl.Font = new Font("Segoe UI Light", 14);
                lbl.Location = new Point(0, FieldOffsetY);
                lbl.Text = name;
                lbl.Size = new Size((int)(_This.Width * .2), FieldHeight);
                BoxGroup.Controls.Add(lbl);
            }
            // Input
            int tbxW = (int)(_This.Width * .35);
            if (whichDimen >= 1) tbxW = (int)(tbxW * (1 / 3f)) - 8;
            TextBox tbx = new MetroTextBox(tbxW, height ?? FieldHeight, height != null);
            if (validation != null) tbx.KeyPress += validation;
            if (whichDimen <= 1)
            {
                tbx.Location = new Point(lbl.Left + lbl.Width + (int)(_This.Width * .05), FieldOffsetY);
                FieldOffsetY += (whichDimen <= 1) ? (height ?? FieldHeight) + FieldPadding : 0;
            }
            else
            {
                int offset = whichDimen - 1;
                tbx.Location = new Point(BoxDimen1.Left + BoxDimen1.Width * offset + 12 * offset, BoxDimen1.Top);
            }
            BoxGroup.Controls.Add(tbx);
            return tbx;
        }

        private ComboBox InsertDropDown(string name)
        {
            Label lbl = new Label();
            lbl.ForeColor = Color.White;
            lbl.Font = new Font("Segoe UI Light", 14);
            lbl.Location = new Point(0, FieldOffsetY);
            lbl.Text = name;
            lbl.Size = new Size((int)(_This.Width * .2), FieldHeight);
            BoxGroup.Controls.Add(lbl);
            // Input
            MetroComboBox cmbx = new MetroComboBox();
            cmbx.Location = new Point(lbl.Left + lbl.Width + (int)(_This.Width * .05), FieldOffsetY);
            FieldOffsetY += FieldHeight + FieldPadding;
            int tbxW = (int)(_This.Width * .35);
            cmbx.Size = new Size(tbxW, FieldHeight);
            BoxGroup.Controls.Add(cmbx);
            return cmbx;
        }

        private void HandleSubmit(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(BoxName.Text) ||
                String.IsNullOrWhiteSpace(BoxDesc.Text) ||
                String.IsNullOrWhiteSpace(BoxPrice.Text) ||
                String.IsNullOrWhiteSpace(BoxDiscnt.Text) ||
                String.IsNullOrWhiteSpace(BoxQty.Text) ||
                String.IsNullOrWhiteSpace(BoxCtgy.Text) ||
                String.IsNullOrWhiteSpace(BoxPhoto.Text))
            {
                MessageBox.Show(Root.GetMsg("edit.missing-info"), Root.GetMsg("ui.warn"));
                return;
            }
            try
            {
                if (XQL.UpdateFurniture(_Item.ID,
                    name: BoxName.Text,
                    description: BoxDesc.Text,
                    dimension: BoxDimen1.Text + "x" + BoxDimen2.Text + "x" + BoxDimen3.Text,
                    price: BoxPrice.Text,
                    discount: BoxDiscnt.Text,
                    quantity: BoxQty.Text,
                    reorderLevel: BoxReordrLv.Text,
                    shelfLocation: BoxShelf.Text,
                    categoryID: ((ComboItem)BoxCtgy.SelectedItem).Value,
                    subCategoryID: ((ComboItem)BoxSubCtgy.SelectedItem).Value,
                    photo: BoxPhoto.Text) > 0)
                    BackToManage(reloadProducts: true);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, Root.GetMsg("ui.errr"));
            }
        }
    }
}
