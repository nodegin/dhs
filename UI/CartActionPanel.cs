using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

using DHS.Misc;

namespace DHS.UI
{
    class CartActionPanel : Panel
    {
        private IParentForm Parent;
        private Furniture Item;
        private bool IsOrderCart;
        private Dictionary<string, int> Cart;
        private EventHandler Callback;

        private TextBox ItemQuantity = new TextBox();
        private Button AddItem = new Button();
        private Button SubtractItem = new Button();

        public CartActionPanel(IParentForm parent, int rowSize, Furniture data, bool isOrder, EventHandler callback = null)
        {
            Parent = parent;
            Item = data;
            IsOrderCart = isOrder;
            Cart = IsOrderCart ? parent._Root.OrderCart : parent._Root.ShoppingCart;
            Callback = callback;
            this.BackColor = Color.FromArgb(77, 77, 77);
            this.Size = new Size((int)(rowSize * 1.25), rowSize - 2);
            this.Margin = Padding.Empty;
            // label number in cart   
            ItemQuantity.BorderStyle = BorderStyle.None;
            ItemQuantity.BackColor = this.BackColor;
            ItemQuantity.Font = new Font("Segoe UI Light", 18);
            ItemQuantity.TextAlign = HorizontalAlignment.Center;
            ItemQuantity.ForeColor = Color.White;
            ItemQuantity.MaximumSize = new Size(this.Width - 32 - 1, this.Height);
            if (Cart.ContainsKey(Item.ID))
                ItemQuantity.Text = Cart[Item.ID].ToString();
            else
                ItemQuantity.Text = "0";
            bool txtChanged = false;
            Timer checkText = new Timer();
            checkText.Tick += (sender, e) =>
            {
                checkText.Stop();
                if (ItemQuantity.Text.Length < 1) return;
                int inputtedQty = Int32.Parse(ItemQuantity.Text);
                if (inputtedQty < 0 || inputtedQty > (IsOrderCart ? Item.InventoryQuantity : Item.Quantity))
                    inputtedQty = (IsOrderCart ? Item.InventoryQuantity : Item.Quantity);
                int oldQty = Cart.ContainsKey(Item.ID) ? Cart[Item.ID] : 0;
                Cart[Item.ID] = inputtedQty;
                ItemQuantity.Text = inputtedQty.ToString();
                ItemQuantity.Select(ItemQuantity.Text.Length, 0);
                if (txtChanged && null != Callback) Callback(oldQty, e);
                txtChanged = false;
            };
            checkText.Enabled = true;
            ItemQuantity.ImeMode = System.Windows.Forms.ImeMode.Disable;
            ItemQuantity.MaxLength = 4;
            ItemQuantity.TextChanged += (sender, e) => {
                checkText.Stop();
                txtChanged = true;
                checkText.Start();
            };
            ItemQuantity.KeyPress += (sender, e) => { Validation.NumberField(sender, e, 4); };
            ItemQuantity.Leave += (sender, e) => {
                if (String.IsNullOrEmpty(ItemQuantity.Text))
                {
                    Cart[Item.ID] = 0;
                    ItemQuantity.Text = "0";
                }
            };
            this.Controls.Add(ItemQuantity);
            ItemQuantity.Top = this.Height / 2 - ItemQuantity.Height / 2;
            // increase amount button
            AddItem.BackgroundImage = Properties.Resources.arrow_increase;
            AddItem.BackColor = IsOrderCart ? Color.FromArgb(204, 128, 34) : Color.FromArgb(77, 160, 45);
            AddItem.FlatStyle = FlatStyle.Flat;
            AddItem.FlatAppearance.BorderSize = 0;
            AddItem.BackgroundImageLayout = ImageLayout.Center;
            AddItem.Size = new Size(32, (int)(this.Height * .5));
            AddItem.Click += UpdateCart;
            AddItem.Left = ItemQuantity.Width + 1;
            AddItem.TabStop = false;
            this.Controls.Add(AddItem);
            // decrease amount button
            SubtractItem.BackgroundImage = Properties.Resources.arrow_decrease;
            SubtractItem.BackColor = AddItem.BackColor;
            SubtractItem.FlatStyle = AddItem.FlatStyle;
            SubtractItem.FlatAppearance.BorderSize = AddItem.FlatAppearance.BorderSize;
            SubtractItem.BackgroundImageLayout = AddItem.BackgroundImageLayout;
            SubtractItem.Size = AddItem.Size;
            SubtractItem.Top = AddItem.Height;
            SubtractItem.Left = AddItem.Left;
            SubtractItem.Click += UpdateCart;
            SubtractItem.TabStop = false;
            this.Controls.Add(SubtractItem);
        }

        private void UpdateCart(object sender, EventArgs e)
        {
            int oldQty = Cart[Item.ID];
            if (!Cart.ContainsKey(Item.ID))
                Cart.Add(Item.ID, 0);
            if (sender is Button && sender.Equals(AddItem))
            {
                if (Cart[Item.ID] < (IsOrderCart ? Item.InventoryQuantity : Item.Quantity))
                    Cart[Item.ID] += 1;
                else
                    Cart[Item.ID] = 0;
                ItemQuantity.Text = Cart[Item.ID].ToString();
            }
            else if (sender is Button && sender.Equals(SubtractItem))
            {
                if (Cart[Item.ID] > 0)
                    Cart[Item.ID] -= 1;
                else // negative set to maximum
                    Cart[Item.ID] = (IsOrderCart ? Item.InventoryQuantity : Item.Quantity);
                ItemQuantity.Text = Cart[Item.ID].ToString();
            }
            if (null != Callback) Callback(oldQty, e);
        }

    }
}
