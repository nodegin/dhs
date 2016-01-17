using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class SearchStoreFurnituresForm : BackableForm
    {

        private int FieldOffsetY = 0;
        private int FieldHeight = 36;
        private int FieldPadding = 24;

        private Panel BoxGroup;
        private TextBox BoxID, BoxName, BoxPrice, BoxQty;
        private ComboBox BoxCtgy, BoxSubCtgy, BoxOrderBy;
        private MetroButton BtnSubmit;

        public SearchStoreFurnituresForm(Root root, IParentForm parent) : base(root, parent, Root.GetMsg("srch.title"))
        {
            BoxGroup = new Panel();
            BoxGroup.AutoSize = true;
            ScrollPanel SP = new ScrollPanel();
            SP.Size = new Size(_This.ContentPanel.Width, _This.ContentPanel.Height);
            SP.AutoScroll = true;
            // create textboxes input
            BoxID = InsertField(Root.GetMsg("edit.id"), Validation.IDField);
            BoxName = InsertField(Root.GetMsg("edit.name"), null);
            BoxPrice = InsertField(Root.GetMsg("edit.price"), Validation.SearchFloatField);
            BoxQty = InsertField(Root.GetMsg("edit.quantity"), Validation.SearchFloatField);
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
            BoxOrderBy = InsertDropDown(Root.GetMsg("srch.order-by"));
            string asc = Root.GetMsg("srch.asc");
            string desc = Root.GetMsg("srch.desc");
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.id") + " " + asc, 0));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.id") + " " + desc, 1));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.name") + " " + asc, 2));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.name") + " " + desc, 3));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.price") + " " + asc, 4));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.price") + " " + desc, 5));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.quantity") + " " + asc, 6));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.quantity") + " " + desc, 7));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.sl") + " " + asc, 8));
            BoxOrderBy.Items.Add(new ComboItem(Root.GetMsg("edit.sl") + " " + desc, 9));
            BoxOrderBy.SelectedIndex = 0;
            // add / update button
            BtnSubmit = new MetroButton();
            BtnSubmit.AutoSize = true;
            BtnSubmit.Click += HandleSubmit;
            BtnSubmit.Text = Root.GetMsg("srch.search");
            BoxGroup.Controls.Add(BtnSubmit);
            BtnSubmit.Location = new Point((BoxOrderBy.Left + BoxOrderBy.Width) - BtnSubmit.Width, (BoxOrderBy.Top + BoxOrderBy.Height) + FieldPadding * 2);
            // reset button
            MetroButton BtnReset = new MetroButton();
            BtnReset.AutoSize = true;
            BtnReset.Click += HandleReset;
            BtnReset.Text = Root.GetMsg("srch.reset");
            BoxGroup.Controls.Add(BtnReset);
            BtnReset.Location = new Point(BtnSubmit.Left - BtnReset.Width - 32, BtnSubmit.Top);

            SP.Controls.Add(BoxGroup);
            BoxGroup.Margin = new Padding(_This.ContentPanel.Width / 2 - BoxGroup.Width / 2, _This.ContentPanel.Height / 2 - BoxGroup.Height / 2, 0, 0);
            _This.ContentPanel.Controls.Add(SP);
        }

        private TextBox InsertField(string name, KeyPressEventHandler validation)
        {
            Label lbl = new Label();
            // Label
            lbl.ForeColor = Color.White;
            lbl.Font = new Font("Segoe UI Light", 16);
            lbl.Location = new Point(0, FieldOffsetY);
            lbl.Text = name;
            lbl.Size = new Size((int)(_This.Width * .2), FieldHeight);
            BoxGroup.Controls.Add(lbl);
            // Input
            int tbxW = (int)(_This.Width * .4);
            TextBox tbx = new MetroTextBox(tbxW, FieldHeight, false);
            tbx.Location = new Point(lbl.Left + lbl.Width + FieldPadding, FieldOffsetY);
            tbx.ImeMode = System.Windows.Forms.ImeMode.Disable;
            if (validation != null) tbx.KeyPress += validation;
            FieldOffsetY += FieldHeight + FieldPadding;
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
            cmbx.Location = new Point(lbl.Left + lbl.Width + FieldPadding, FieldOffsetY);
            cmbx.ImeMode = System.Windows.Forms.ImeMode.Disable;
            FieldOffsetY += FieldHeight + FieldPadding;
            int tbxW = (int)(_This.Width * .4);
            cmbx.Size = new Size(tbxW, FieldHeight);
            BoxGroup.Controls.Add(cmbx);
            return cmbx;
        }

        private void HandleReset(object sender, EventArgs e)
        {
            BoxID.Text = BoxName.Text = BoxPrice.Text = BoxQty.Text = "";
            BoxCtgy.SelectedIndex = BoxSubCtgy.SelectedIndex = BoxOrderBy.SelectedIndex = 0;
        }

        private void HandleSubmit(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(BoxID.Text) && !Validation.ValidateID(BoxID.Text))
            {
                MessageBox.Show(Root.GetMsg("edit.id") + Root.GetMsg("srch.id-length-error"), Root.GetMsg("ui.errr"));
                return;
            }
            string where = "";
            bool ignoreID = String.IsNullOrEmpty(BoxID.Text);
            bool ignoreName = String.IsNullOrEmpty(BoxName.Text);
            bool ignorePrice = String.IsNullOrEmpty(BoxPrice.Text);
            bool ignoreQuantity = String.IsNullOrEmpty(BoxQty.Text);
            bool ignoreCtgy = (int)((ComboItem)BoxCtgy.SelectedItem).Value == 0;
            bool ignoreSubCtgy = ignoreCtgy || (int)((ComboItem)BoxSubCtgy.SelectedItem).Value == 0;

            if (!ignoreID)
                where += "[Furnitures].[ID] = '" + BoxID.Text + "' and ";
            if (!ignoreName)
                where += "[Furnitures].[_Name] like '%" + BoxName.Text + "%' and ";
            if (!ignorePrice)
                where += BuildNumberQuerySqlPart("[Furnitures].[_Price]", BoxPrice.Text);
            if (!ignoreQuantity)
                where += BuildNumberQuerySqlPart("[StoreFurnitures].[_Quantity]", BoxQty.Text);
            if (!ignoreCtgy)
                where += "[Furnitures].[_CID] = " + ((ComboItem)BoxCtgy.SelectedItem).Value + " and ";
            if (!ignoreSubCtgy)
                where += "[Furnitures].[_SCID] = " + ((ComboItem)BoxSubCtgy.SelectedItem).Value + " and ";

            if (String.IsNullOrEmpty(where))
                where = null;
            else if (where.IndexOf("SYNTAX_ERROR") > -1)
            {
                MessageBox.Show(Root.GetMsg("srch.search-syntax-error"), Root.GetMsg("ui.errr"));
                return;
            }
            else
                where = where.Remove(where.LastIndexOf(" and "));
            string[] orders = new string[] {
                "[Furnitures].[ID] asc",
                "[Furnitures].[ID] desc",
                "[Furnitures].[_Name] asc",
                "[Furnitures].[_Name] desc",
                "[Furnitures].[_Price] asc",
                "[Furnitures].[_Price] desc",
                "[StoreFurnitures].[_Quantity] asc",
                "[StoreFurnitures].[_Quantity] desc",
                "[StoreFurnitures].[_ShelfLocation] asc",
                "[StoreFurnitures].[_ShelfLocation] desc"
            };
            int i = (int)((ComboItem)BoxOrderBy.SelectedItem).Value;
            BackToManage(reloadProducts: true, whereClause: where, orderBy: orders[i]);
        }

        private string BuildNumberQuerySqlPart(string field, string input)
        {
            string part = "";
            var m = Regex.Match(input, @"^(?<range>\d+\-\d+)$|^(?<compare>(>|>=|<|<=)\d+)$|^(?<equals>\d+)$");
            if (m.Success)
            {
                if (!String.IsNullOrEmpty(m.Groups["range"].Value))
                {
                    string[] parts = m.Groups["range"].Value.Split('-');
                    if (Int32.Parse(parts[0]) > Int32.Parse(parts[1]))
                        part = field + " between " + parts[1] + " and " + parts[0] + " and ";
                    else
                        part = field + " between " + parts[0] + " and " + parts[1] + " and ";
                }
                else if (!String.IsNullOrEmpty(m.Groups["compare"].Value))
                {
                    part = field + " " + m.Groups["compare"].Value + " and ";
                }
                else if (!String.IsNullOrEmpty(m.Groups["equals"].Value))
                {
                    part = field + " = " + m.Groups["equals"].Value + " and ";
                }
            }
            else part = "SYNTAX_ERROR";
            return part;
        }

    }
}
