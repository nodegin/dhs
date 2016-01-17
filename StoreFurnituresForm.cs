using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading.Tasks;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class StoreFurnituresForm : BackableForm
    {

        private ScrollPanel ItemList = new ScrollPanel();

        public StoreFurnituresForm(Root root, MetroForm parent) : base(root, parent, Root.GetMsg("main.menu.store-furnitures"))
        {
            _This.InsertActionButtons(Root.GetMsg("stfu.action.reload"), Properties.Resources.reload, (sender, e) => { ReloadItems(); });
            _This.InsertActionButtons(Root.GetMsg("stfu.action.search"), Properties.Resources.search, ActionSearch);
            _This.InsertActionButtons(Root.GetMsg("stfu.action.order-cart"), Properties.Resources.receipt, ActionOrder);
            _This.InsertActionButtons(Root.GetMsg("stfu.action.shopping-cart"), Properties.Resources.cart, ActionCheckout);
            // add furnitures
            ItemList.Size = _This.ContentPanel.Size;
            _This.ContentPanel.Controls.Add(ItemList);
            ReloadItems();
        }
        private int RowSize = 80;
        private int RowMargin = 18;
        private int OffsetCounter = 0;
        // allow every class reload this form through this method
        public void ReloadItems(string whereClause = null, string orderBy = null)
        {
            List<Furniture> items = XQL.GetFurnitures(inStore: true, where: whereClause, order: orderBy);
            int scrollbarAdjust = items.Count * RowSize >= ItemList.Height ? SystemInformation.VerticalScrollBarWidth : 0;
            ItemList.Controls.Clear();
            if (items.Count < 1)
            {
                Label lbl = new Label();
                lbl.Font = new Font("Segoe UI Light", 24);
                lbl.ForeColor = Color.FromArgb(204, 204, 204);
                lbl.Text = Root.GetMsg("ui.no-result");
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
                row.Size = new Size(ItemList.Width, RowSize);
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
                // item category and name
                Label id = new Label();
                id.BackColor = Color.Transparent;
                id.AutoSize = true;
                id.MinimumSize = new Size(160, 0);
                id.Font = new Font("Segoe UI Light", 16);
                id.ForeColor = Color.FromArgb(204, 204, 204);
                id.Text = "[" + item.ID + "]";
                int middlePos = RowSize / 2 - id.Height / 2;
                id.Location = new Point(RowSize + RowMargin, middlePos - RowMargin);
                row.Controls.Add(id);
                Label name = new Label();
                name.BackColor = Color.Transparent;
                name.AutoSize = true;
                name.Text = item.Name;
                name.Font = new Font("Segoe UI Light", 16);
                name.Location = new Point(id.Width + id.Left, middlePos - RowMargin);
                row.Controls.Add(name);
                // item details
                Label cat = GetNextItemLabel(item.Category, id.Width - RowMargin);
                Label price = GetNextItemLabel(Util.CalcDiscountedPrice(item), 240, item.Discount > 0 ? Color.FromArgb(255, 213, 77) : (Color?)null);
                Label qty = GetNextItemLabel(Root.GetMsg("stfu.qty") + ": " + item.Quantity, 100);
                Label shelf = GetNextItemLabel(Root.GetMsg("stfu.shelf") + ": " + item.ShelfLocation, 120);
                Label date = GetNextItemLabel(Root.GetMsg("stfu.date") + ": " + item.Date.ToString("d/M/yyyy"), 120);
                DarkToolTip tt = new DarkToolTip();
                tt.SetToolTip(cat, String.IsNullOrEmpty(item.SubCategory) ? Root.GetMsg("ui.not-set") : item.SubCategory);
                // notify out of stock
                if (item.Quantity < 1)
                {
                    qty.Text = Root.GetMsg("stfu.qty") + ": " + item.InventoryQuantity;
                    if (item.InventoryQuantity < 1) qty.ForeColor = Color.FromArgb(204, 77, 77);
                    else qty.ForeColor = Color.FromArgb(204, 128, 34);
                }
                else if (item.Quantity <= item.ReorderLevel) qty.ForeColor = Color.FromArgb(205, 220, 57);
                else qty.ForeColor = Color.FromArgb(77, 160, 45);
                row.Controls.Add(cat);
                row.Controls.Add(price);
                row.Controls.Add(qty);
                row.Controls.Add(shelf);
                row.Controls.Add(date);
                // view details button
                MetroButton btnDetails = new MetroButton(border: 0);
                btnDetails.BackColor = Color.FromArgb(77, 77, 77);
                btnDetails.Size = new Size((int)(RowSize * 0.75), RowSize - 2);
                btnDetails.Click += (sender, e) => ActionViewDetails(sender, e, item);
                btnDetails.Location = new Point(ItemList.Width - btnDetails.Width - scrollbarAdjust, 1);
                btnDetails.BackgroundImage = Properties.Resources.arrow_right;
                btnDetails.BackgroundImageLayout = ImageLayout.Center;
                row.Controls.Add(btnDetails);
                // if not out of stock, add cart actions
                if (item.Quantity > 0)
                {
                    CartActionPanel cartActions = new CartActionPanel(this, RowSize, item, false);
                    cartActions.Location = new Point(ItemList.Width - btnDetails.Width - cartActions.Width - scrollbarAdjust, 1);
                    row.Controls.Add(cartActions);
                }
                else
                {
                    if (_Root.OrderCart.ContainsKey(item.ID) && _Root.OrderCart[item.ID] > 0)
                    {
                        CartActionPanel cartActions = new CartActionPanel(this, RowSize, item, true);
                        cartActions.Location = new Point(ItemList.Width - btnDetails.Width - cartActions.Width - scrollbarAdjust, 1);
                        row.Controls.Add(cartActions);
                    }
                    else
                    {
                        MetroButton btnOrder = new MetroButton(border: 0);
                        btnOrder.Size = new Size((int)(RowSize * 1.25), RowSize - 2);
                        if (item.InventoryQuantity > 0)
                        {
                            btnOrder.BackColor = Color.FromArgb(204, 128, 34);
                            btnOrder.BackgroundImage = Properties.Resources.receipt;
                            btnOrder.BackgroundImageLayout = ImageLayout.Center;
                            btnOrder.Click += (sender, e) =>
                            {
                                DialogResult order = MessageBox.Show(Root.GetMsg("stfu.continue-order"), Root.GetMsg("ui.info"), MessageBoxButtons.YesNo);
                                if (order == DialogResult.Yes)
                                {
                                    //ActionAddInventoryDeliveryOrder(sender, e, item);
                                    row.Controls.Remove(btnOrder);
                                    CartActionPanel cartActions = new CartActionPanel(this, RowSize, item, true);
                                    cartActions.Location = new Point(ItemList.Width - btnDetails.Width - cartActions.Width - scrollbarAdjust, 1);
                                    row.Controls.Add(cartActions);
                                }
                            };
                        }
                        else
                        {
                            btnOrder.BackColor = Color.FromArgb(45, 128, 128, 128);
                            btnOrder.FlatAppearance.MouseOverBackColor = btnOrder.BackColor;
                            btnOrder.FlatAppearance.MouseDownBackColor = Color.FromArgb(61, 128, 128, 128);
                            btnOrder.Paint += (sender, e) =>
                            {
                                Image im = Properties.Resources.receipt;
                                ColorMatrix matrix = new ColorMatrix();
                                matrix.Matrix33 = .5f;
                                ImageAttributes attributes = new ImageAttributes();
                                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                e.Graphics.DrawImage(im, new Rectangle(btnOrder.Width / 2 - im.Width / 2, btnOrder.Height / 2 - im.Height / 2, im.Width, im.Height), 0, 0, im.Width, im.Height, GraphicsUnit.Pixel, attributes);
                            };
                            btnOrder.Click += (sender, e) =>
                            {
                                MessageBox.Show(Root.GetMsg("stfu.out-of-stock"), Root.GetMsg("ui.warn"));
                            };
                        }
                        btnOrder.Location = new Point(ItemList.Width - btnDetails.Width - btnOrder.Width - scrollbarAdjust, 1);
                        row.Controls.Add(btnOrder);
                    }
                }
                ItemList.Controls.Add(row);
                OffsetCounter = 0;
            }
        }

        private Label GetNextItemLabel(string text, int thisWidth, Color? color = null)
        {
            Label lbl = new Label();
            lbl.BackColor = Color.Transparent;
            lbl.ForeColor = color ?? Color.FromArgb(204, 204, 204);
            lbl.AutoSize = true;
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI Light", 13);
            int lblMiddlePos = RowSize / 2 - lbl.Height / 2;
            OffsetCounter += RowMargin;
            lbl.Location = new Point(RowSize + OffsetCounter, lblMiddlePos + RowMargin);
            OffsetCounter += thisWidth;
            return lbl;
        }

        private void ActionSearch(object sender, EventArgs e)
        {
            new SearchStoreFurnituresForm(_Root, this).Show();
        }

        private void ActionCheckout(object sender, EventArgs e)
        {
            new CheckoutForm(_Root, this, true).Show();
        }

        private void ActionOrder(object sender, EventArgs e)
        {
            new CheckoutForm(_Root, this, false).Show();
        }
        
        private void ActionViewDetails(object sender, EventArgs e, Furniture dt)
        {
            new FurnitureDetailsForm(_Root, this, dt).Show();
        }

    }
}
