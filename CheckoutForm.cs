using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms.VisualStyles;
using System.Data.OleDb;
using System.Globalization;
using Transitions;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class CheckoutForm : BackableForm
    {
        private bool IsCheckout;
        private Dictionary<string, int> Cart;

        private int RowSize = 80;
        private int PanePadding = 24;
        private ScrollPanel ItemList = new ScrollPanel();
        private Panel InfoPane = new Panel();
        private Label FieldSubtotal = new Label();
        private Label FieldDiscounted = new Label();
        private MetroCheckBox FieldDelivery = new MetroCheckBox();
        /* \-> */ private MetroTextBox FieldCustomerName;
        /* \-> */ private MetroTextBox FieldAddress;
        /* \-> */ private MetroTextBox FieldPhoneNum;
        private Label FieldTotal = new Label();
        private MetroButton PayByCash = new MetroButton(fontSize: 12);
        private MetroButton PayByCreditCard = new MetroButton(fontSize: 12);

        private double ValSubtotal = 0;
        private double ValDiscounted = 0;
        private double ValTotal = 0;

        public CheckoutForm(Root root, IParentForm parent, bool isCheckout) : base(root, parent, isCheckout ? Root.GetMsg("cart.checkout") : Root.GetMsg("cart.order"))
        {
            IsCheckout = isCheckout;
            Cart = IsCheckout ? _Root.ShoppingCart : _Root.OrderCart;
            _This.InsertActionButtons(Root.GetMsg("cart.action.clear-all"), Properties.Resources.clear_all, (sender, e) =>
            {
                Cart.Clear();
                UpdateList();
            });
            ItemList.Size = new Size((int)Math.Round(_This.ContentPanel.Width * .65) + PanePadding, _This.ContentPanel.Height);
            ItemList.Paint += (sender, e) => {
                int strip = (int)(InfoPane.Height * .1);
                Pen borderPen = new Pen(new SolidBrush(Color.FromArgb(77, 77, 77)));
                int off = ItemList.Width - 1;
                e.Graphics.DrawLine(borderPen, new Point(off, strip), new Point(off, ItemList.Height - strip - 1));
            };
            _This.ContentPanel.Controls.Add(ItemList);
            InfoPane.AutoSize = true;
            InfoPane.MinimumSize = new Size((int)Math.Round(_This.ContentPanel.Width * .35), 0);
            InfoPane.Left = ItemList.Width + PanePadding / 2;
            _This.ContentPanel.Controls.Add(InfoPane);
            InitialInfoPaneComponents();

            // must reload parent list avoid any inconsistency
            OverrideBackReloadDHS = true;
            UpdateList();
        }

        private int FieldsOffsetY = 0;
        private int FieldHeaderPadding = 8;
        private int FieldsPadding = 12;
        private int FieldMaxWidth = 0;
        private void InitialInfoPaneComponents()
        {
            FieldMaxWidth = InfoPane.Width - PanePadding * 2;
            // subtotal
            InsertFieldHeader(Root.GetMsg("cart.subtotal"), InfoPane);
            InsertField(FieldSubtotal, InfoPane, typeof(Label));
            // discount
            InsertFieldHeader(Root.GetMsg("cart.discounted"), InfoPane);
            InsertField(FieldDiscounted, InfoPane, typeof(Label));
            // total
            InsertFieldHeader(Root.GetMsg("cart.total"), InfoPane);
            InsertField(FieldTotal, InfoPane, typeof(Label));
            // panel for contain optional delivery inputs
            Panel dPane = new Panel();
            dPane.Visible = false;
            dPane.AutoSize = true;
            // is delivery item (for checkout only)
            if (IsCheckout)
            {
                InsertFieldHeader(Root.GetMsg("cart.delivery"), InfoPane);
                InsertField(FieldDelivery, InfoPane, typeof(MetroCheckBox));
            }
            // customer name
            FieldCustomerName = new MetroTextBox(FieldMaxWidth - 6 * 2, 32);
            InsertFieldHeader(Root.GetMsg("cart.cust-name"), dPane);
            InsertField(FieldCustomerName, dPane, typeof(MetroTextBox));
            FieldCustomerName.KeyPress += Validation.NonNumberField;
            // address
            FieldAddress = new MetroTextBox(FieldMaxWidth - 6 * 2, 64);
            InsertFieldHeader(Root.GetMsg("cart.cust-addr"), dPane);
            InsertField(FieldAddress, dPane, typeof(MetroTextBox));
            FieldAddress.Multiline = true;
            // phone
            FieldPhoneNum = new MetroTextBox(FieldMaxWidth - 6 * 2, 32);
            InsertFieldHeader(Root.GetMsg("cart.cust-phone"), dPane);
            InsertField(FieldPhoneNum, dPane, typeof(MetroTextBox));
            FieldPhoneNum.KeyPress += (sender, e) => { Validation.NumberField(sender, e, 8); };
            // insert optional delivery inputs panel
            dPane.Top = FieldsOffsetY;
            InfoPane.Controls.Add(dPane);
            FieldsOffsetY += dPane.Height + FieldsPadding;
            Size dPaneSavedSize = new Size(dPane.Size.Width, dPane.Size.Height);
            dPane.AutoSize = false;
            if (IsCheckout) dPane.Size = new Size(dPaneSavedSize.Width, 0);
            dPane.Visible = true;
            // padding for buttons
            FieldsOffsetY += 12;
            // PayByCash
            PayByCash.AutoSize = true;
            int btnWidth = (int)(FieldMaxWidth * .4);
            PayByCash.MinimumSize = new Size(btnWidth, 48);
            PayByCash.Padding = new Padding(32, 0, 0, 0);
            PayByCash.Left = FieldMaxWidth / 2 - btnWidth - FieldsPadding;
            PayByCash.Paint += (sender, e) =>
            {
                Image im = Properties.Resources.money;
                SizeF size = e.Graphics.MeasureString(PayByCash.Text, PayByCash.Font);
                e.Graphics.DrawImage(im, (btnWidth - size.Width) / 2 - PayByCash.Padding.Left / 2, PayByCash.Height / 2 - im.Height / 2);
            };
            PayByCash.Click += (sender, e) => { DoPayment(true); };
            PayByCash.Top = FieldsOffsetY - (IsCheckout ? dPaneSavedSize.Height : 0);
            PayByCash.Text = Root.GetMsg("cart.cash");
            InfoPane.Controls.Add(PayByCash);
            // PayByCreditCard
            PayByCreditCard.MinimumSize = new Size(btnWidth, 48);
            PayByCreditCard.Padding = PayByCash.Padding;
            PayByCreditCard.Left = FieldMaxWidth / 2 + FieldsPadding;
            PayByCreditCard.Paint += (sender, e) =>
            {
                Image im = Properties.Resources.credit_card;
                SizeF size = e.Graphics.MeasureString(PayByCreditCard.Text, PayByCreditCard.Font);
                e.Graphics.DrawImage(im, (btnWidth - size.Width) / 2 - PayByCreditCard.Padding.Left / 2, PayByCreditCard.Height / 2 - im.Height / 2);
            };
            PayByCreditCard.Click += (sender, e) => { DoPayment(false); };
            PayByCreditCard.Top = FieldsOffsetY - (IsCheckout ? dPaneSavedSize.Height : 0);
            PayByCreditCard.Text = Root.GetMsg("cart.card");
            InfoPane.Controls.Add(PayByCreditCard);
            int savedPaymentButtonOffsetY = FieldsOffsetY;
            FieldsOffsetY += PayByCreditCard.Height + FieldsPadding;

            // animate for slide down
            InfoPane.Top = _This.ContentPanel.Height / 2 - InfoPane.Height / 2;
            int savedInfoPaneY = InfoPane.Top;
            int savedInfoPaneHeight = InfoPane.Height;
            InfoPane.AutoSize = false;
            InfoPane.Height = savedInfoPaneHeight + (IsCheckout ? 0 : dPaneSavedSize.Height);
            if (IsCheckout)
            {
                FieldDelivery.CheckedChanged += (sender, e) =>
                {
                    if (FieldDelivery.Checked)
                    {
                        Transition t = new Transition(new TransitionType_EaseInEaseOut(250));
                        t.add(dPane, "Height", dPaneSavedSize.Height);
                        t.add(PayByCash, "Top", savedPaymentButtonOffsetY);
                        t.add(PayByCreditCard, "Top", savedPaymentButtonOffsetY);
                        t.add(InfoPane, "Height", savedInfoPaneHeight + dPaneSavedSize.Height);
                        t.add(InfoPane, "Top", savedInfoPaneY - dPaneSavedSize.Height / 2);
                        t.run();
                    }
                    else
                    {
                        dPane.AutoSize = false;
                        Transition t = new Transition(new TransitionType_EaseInEaseOut(250));
                        t.add(dPane, "Height", 0);
                        t.add(PayByCash, "Top", savedPaymentButtonOffsetY - dPaneSavedSize.Height);
                        t.add(PayByCreditCard, "Top", savedPaymentButtonOffsetY - dPaneSavedSize.Height);
                        t.add(InfoPane, "Height", savedInfoPaneHeight);
                        t.add(InfoPane, "Top", savedInfoPaneY);
                        t.run();
                    }
                };
            }
            else dPane.Size = dPaneSavedSize;
        }

        private int OffsetYForSlidingPane = 0;
        private void InsertFieldHeader(string name, Control holder)
        {
            Label lbl = new Label();
            lbl.BackColor = Color.Transparent;
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI Light", 12);
            lbl.ForeColor = Color.FromArgb(204, 204, 204);
            lbl.Text = name;
            lbl.Left = holder.Padding.Left;
            if (holder.Equals(InfoPane))
            {
                lbl.Top = FieldsOffsetY;
                holder.Controls.Add(lbl);
                FieldsOffsetY += lbl.Height + FieldHeaderPadding;
            }
            else
            {
                lbl.Top = OffsetYForSlidingPane;
                holder.Controls.Add(lbl);
                OffsetYForSlidingPane += lbl.Height + FieldHeaderPadding;
            }
        }
        private void InsertField(Control ctrl, Control holder, Type t)
        {
            bool isChild = !holder.Equals(InfoPane);
            if (t == typeof(Label))
            {
                ctrl.Font = new Font("Segoe UI Light", 15);
                ctrl.BackColor = Color.Transparent;
                ctrl.Left = holder.Padding.Left;
                ctrl.AutoSize = true;
                ctrl.MaximumSize = new Size(FieldMaxWidth, 0);
            }
            else if (t == typeof(MetroCheckBox))
            {
                ctrl.Left = holder.Padding.Left + 6;
            }
            else if (t == typeof(MetroTextBox))
            {
                ctrl.AutoSize = false;
                ctrl.Left = holder.Padding.Left + 6;
            }
            ctrl.Top = isChild ? OffsetYForSlidingPane : FieldsOffsetY;
            holder.Controls.Add(ctrl);
            if (isChild) OffsetYForSlidingPane += ctrl.Height + FieldsPadding;
            else FieldsOffsetY += ctrl.Height + FieldsPadding;
        }

        private void UpdateList()
        {
            // reset values
            ValSubtotal = 0;
            ValDiscounted = 0;
            ValTotal = 0;
            ItemList.Controls.Clear();
            string inIds = "[Furnitures].[ID] in ('',"; // <--- set empty id for default empty cart
            foreach (KeyValuePair<string, int> item in Cart)
                if (item.Value > 0) // quantity not zero
                    inIds += "'" + item.Key + "',";
            inIds = inIds.Remove(inIds.Length - 1) + ")";

            List<Furniture> items = XQL.GetFurnitures(inStore: true, where: inIds, order: "[Furnitures].[_Name] asc");
            int scrollbarAdjust = items.Count * RowSize >= ItemList.Height ? SystemInformation.VerticalScrollBarWidth : 0;
            if (items.Count < 1)
            {
                Label lbl = new Label();
                lbl.Font = new Font("Segoe UI Light", 24);
                lbl.ForeColor = Color.FromArgb(204, 204, 204);
                lbl.Text = IsCheckout ? Root.GetMsg("cart.shopping-cart-empty") : Root.GetMsg("cart.order-cart-empty");
                lbl.AutoSize = true;
                ItemList.Controls.Add(lbl);
                lbl.Margin = new Padding(
                    ItemList.Width / 2 - lbl.Width / 2,
                    ItemList.Height / 2 - lbl.Height / 2,
                    0, 0);
            }
            else for (int i = 0; i < items.Count; i++)
            {
                Furniture item = items[i];
                // create holder for item
                int alpha = (i % 2 == 0) ? 16 : 8;
                Panel row = new Panel();
                row.BackColor = Color.FromArgb(alpha, 255, 255, 255);
                row.Size = new Size(ItemList.Width - PanePadding, RowSize);
                row.Margin = Padding.Empty;
                row.Top = i * RowSize;
                // item photo
                PictureBox pic = new PictureBox();
                pic.Image = Properties.Resources.loader_small;
                pic.ImageLocation = item.Photo.AbsoluteUri;
                pic.Size = new Size(RowSize, RowSize);
                pic.SizeMode = PictureBoxSizeMode.CenterImage;
                pic.LoadCompleted += (sender, e) => {
                    pic.SizeMode = PictureBoxSizeMode.Zoom;
                };
                row.Controls.Add(pic);
                ItemList.Controls.Add(row);
                // name
                Label name = new Label();
                name.BackColor = Color.Transparent;
                name.AutoSize = true;
                name.Text = item.Name;
                name.Font = new Font("Segoe UI Light", 16);
                row.Controls.Add(name);
                name.Location = new Point(RowSize + 16, RowSize / 2 - name.Height / 2);
                // view details button
                Button btnDelete = new Button();
                btnDelete.BackColor = Color.FromArgb(204, 77, 77);
                btnDelete.FlatStyle = FlatStyle.Flat;
                btnDelete.FlatAppearance.BorderSize = 0;
                btnDelete.Size = new Size((int)(RowSize * 0.75), RowSize - 2);
                btnDelete.Click += (sender, e) => {
                    Cart.Remove(item.ID);
                    UpdateList();
                };
                btnDelete.TabStop = false;
                btnDelete.Location = new Point(row.Width - btnDelete.Width - scrollbarAdjust, 1);
                btnDelete.BackgroundImage = Properties.Resources.delete;
                btnDelete.BackgroundImageLayout = ImageLayout.Center;
                row.Controls.Add(btnDelete);
                // cart actions
                CartActionPanel cartActions = new CartActionPanel(this, RowSize, item, !IsCheckout, (oldQty, e) => {
                    // old: UpdateList();
                    // update changed item only
                    int diff = Cart[item.ID] - (int)oldQty;
                    double subtotalDiff = item.Price * diff;
                    ValSubtotal += subtotalDiff;
                    double discountedDiff = item.Price * diff * ((100 - item.Discount) / 100f);
                    ValDiscounted += subtotalDiff - discountedDiff;
                    ValTotal += discountedDiff;
                    UpdatePrice();
                });
                cartActions.Location = new Point(row.Width - btnDelete.Width - cartActions.Width - scrollbarAdjust, 1);
                row.Controls.Add(cartActions);
                ItemList.Controls.Add(row);

                // update price
                double subtotal = item.Price * Cart[item.ID];
                ValSubtotal += subtotal;
                double discounted = item.Price * Cart[item.ID] * ((100 - item.Discount) / 100f);
                ValDiscounted += subtotal - discounted;
                ValTotal += discounted;
            }
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            FieldSubtotal.Text = ValSubtotal.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK"));
            FieldDiscounted.Text = ValDiscounted.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK"));
            FieldTotal.Text = ValTotal.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK"));
        }

        private void DoPayment(bool byCash)
        {
            if (ValTotal < 1) MessageBox.Show(IsCheckout ? Root.GetMsg("cart.shopping-cart-empty") : Root.GetMsg("cart.order-cart-empty"), Root.GetMsg("ui.errr"));
            else if (FieldDelivery.Checked && (String.IsNullOrWhiteSpace(FieldCustomerName.Text) || String.IsNullOrWhiteSpace(FieldAddress.Text) || String.IsNullOrWhiteSpace(FieldPhoneNum.Text))) MessageBox.Show(Root.GetMsg("cart.cust-info-empty"), Root.GetMsg("ui.errr"));
            else new PaymentForm(_Root, this, IsCheckout, byCash, ValTotal, IsCheckout ? FieldDelivery.Checked : true, FieldCustomerName.Text, FieldAddress.Text, FieldPhoneNum.Text).Show();
        }

    }
}
