using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class PaymentForm : BackableForm
    {
        private int offsetY = 24;

        private bool IsCheckout;
        private bool PaymentMethodCash;
        private bool IsDelivery;
        private Label Change = new Label(), CashIn = new Label();
        private MetroButton BtnPaid = new MetroButton();
        private MetroCheckBox PrintInvoice = new MetroCheckBox();

        private double FinalTotal = 0; // total amount of purchased item
        private double FinalAmount = 0; // total pay amount of purchased item (20% - 100% of item price)
        private double FinalCashIn = 0; // total amount of customer paid

        // for delivery
        private string CustomerName;
        private string CustomerAddress;
        private int CustomerPhone;

        public PaymentForm(Root root, IParentForm parent, bool isCheckout, bool payByCash, double total,
            bool delivery, string customer = null, string address = null, string phone = null) : base(root, parent, Root.GetMsg("cart.make-payment"))
        {
            FinalTotal = total;
            IsCheckout = isCheckout;
            PaymentMethodCash = payByCash;
            IsDelivery = delivery;
            CustomerName = customer;
            CustomerAddress = address;
            CustomerPhone = Int32.TryParse(phone, out CustomerPhone) ? CustomerPhone : -1;
            int panelpw = (int)(_This.ContentPanel.Width * (1 / 3.0));
            Panel container = new Panel();
            container.AutoSize = true;
            // amount
            Label hintAmount = new Label();
            hintAmount.AutoSize = true;
            hintAmount.Text = delivery ? Root.GetMsg("cart.deposit") : Root.GetMsg("cart.total");
            hintAmount.Font = new Font("Segoe UI Light", 13);
            hintAmount.ForeColor = Color.FromArgb(204, 204, 204);
            container.Controls.Add(hintAmount);
            hintAmount.Location = new Point(0, offsetY);
            bool canPay = false;
            if (delivery)
            {
                Label hintDeposit = new Label();
                hintDeposit.AutoSize = true;
                hintDeposit.Text = "(" + (total * .2).ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")) + " - " + total.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")) + ")";
                hintDeposit.Font = new Font("Segoe UI Light", 12);
                hintDeposit.ForeColor = Color.FromArgb(255, 213, 77);
                container.Controls.Add(hintDeposit);
                hintDeposit.Location = new Point(hintAmount.Width, offsetY + hintAmount.Height / 2 - hintDeposit.Height / 2);
            }
            offsetY += hintAmount.Height + 12;
            Control amount;
            if (delivery)
            {
                amount = new MetroTextBox(panelpw, 32);
                amount.KeyPress += Validation.FloatField;
                amount.KeyUp += (sender, e) =>
                {
                    double deposit = double.TryParse(amount.Text, out deposit) ? deposit : 0.0;
                    double minimum = total * .2;
                    if (deposit >= minimum && deposit <= total)
                    {
                        canPay = true;
                        /*if (PaymentMethodCash) _This.SelectNextControl((Control)sender, true, true, true, true);
                        else */if (!PaymentMethodCash) CashIn.ForeColor = Color.White;
                    }
                    else
                    {
                        canPay = false;
                        if (!BtnPaid.Disabled) BtnPaid.Disabled = true;
                        if (!PaymentMethodCash) CashIn.ForeColor = Color.Gray;
                    }
                };
            }
            else
            {
                amount = new Label();
                amount.AutoSize = true;
                amount.Text = total.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK"));
                amount.Font = new Font("Segoe UI Light", 16);
                canPay = true;
            }
            container.Controls.Add(amount);
            amount.Location = new Point(0, offsetY);
            offsetY += amount.Height + 12;
            // cash in
            Label hintCashIn = new Label();
            hintCashIn.AutoSize = true;
            hintCashIn.Text = Root.GetMsg("cart.cash-in");
            hintCashIn.Font = new Font("Segoe UI Light", 13);
            hintCashIn.ForeColor = Color.FromArgb(204, 204, 204);
            container.Controls.Add(hintCashIn);
            hintCashIn.Location = new Point(0, offsetY);
            offsetY += hintCashIn.Height + 12;
            if (PaymentMethodCash)
            {
                Change.Text = 0.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK"));
                MetroTextBox tbx = new MetroTextBox(panelpw, 32);
                tbx.KeyPress += Validation.FloatField;
                EventHandler eh = new EventHandler((sender, e) =>
                {
                    double paid = double.TryParse(tbx.Text, out paid) ? paid : 0.0;
                    // <!> MUST be used only in Deposit scenario
                    double deposit = double.TryParse(amount.Text, out deposit) ? deposit : 0.0;
                    // </!> MUST be used only in Deposit scenario
                    double ttl = delivery ? deposit : total;
                    if (canPay)
                    {
                        Change.Text = (paid - ttl > 0 ? paid - ttl : 0).ToString("C", CultureInfo.CreateSpecificCulture("zh-HK"));
                        FinalAmount = ttl;
                        FinalCashIn = paid; 
                        BtnPaid.Disabled = paid < ttl;
                    }
                });
                tbx.GotFocus += eh;
                tbx.KeyUp += new KeyEventHandler(eh);
                container.Controls.Add(tbx);
                tbx.Location = new Point(0, offsetY);
                offsetY += tbx.Height + 12;
            }
            else
            {
                Change.Text = "N/A";
                // icon
                PictureBox icon = new PictureBox();
                icon.Size = new Size(24, 24);
                icon.Image = Properties.Resources.scan;
                container.Controls.Add(icon);
                icon.Location = new Point(0, offsetY);
                // status text
                CashIn = new Label();
                CashIn.AutoSize = true;
                CashIn.Text = Root.GetMsg("cart.pay-by-card");
                CashIn.Font = new Font("Segoe UI Light", 16);
                if (delivery) CashIn.ForeColor = Color.Gray;
                container.Controls.Add(CashIn);
                CashIn.Location = new Point(0 + icon.Width + 12, offsetY + (icon.Height / 2 - CashIn.Height / 2));
                offsetY += icon.Height + 12;
                // emulate payment process
                int ticked = 0;
                bool paymentSucceed = false;
                Timer emulate = new Timer();
                emulate.Interval = 500;
                emulate.Tick += (sender, e) =>
                {
                    if (paymentSucceed)
                    {
                        icon.Image = Properties.Resources.tick;
                        CashIn.Text = Root.GetMsg("cart.payment-ok");
                        BtnPaid.Disabled = false;
                        FinalAmount = !delivery ? total : double.TryParse(amount.Text, out FinalAmount) ? FinalAmount : 0.0;
                        FinalCashIn = FinalAmount; 
                        emulate.Stop();
                    }
                    else
                    {
                        if (ticked < 4)
                            CashIn.Text = Root.GetMsg("cart.payment-card-phase-1");
                        else if (ticked < 8)
                            CashIn.Text = Root.GetMsg("cart.payment-card-phase-2");
                        else if (ticked < 16)
                            CashIn.Text = Root.GetMsg("cart.payment-card-phase-3");
                        else if (ticked < 24)
                            CashIn.Text = Root.GetMsg("cart.payment-card-phase-4");
                        else
                            paymentSucceed = true;
                        ticked++;
                    }
                };
                EventHandler eh = null;
                eh = new EventHandler((sender, e) => {
                    if (delivery && !canPay) return;
                    emulate.Enabled = true;
                    emulate.Start();
                    icon.Image = Properties.Resources.loader_small;
                    icon.Click -= eh;
                    icon.Cursor = Cursors.Default;
                    CashIn.Click -= eh;
                    CashIn.Cursor = Cursors.Default;
                });
                icon.Click += eh;
                icon.Cursor = Cursors.Hand;
                CashIn.Click += eh;
                CashIn.Cursor = Cursors.Hand;
            }
            // changes
            Label hintChange = new Label();
            hintChange.AutoSize = true;
            hintChange.Text = Root.GetMsg("cart.change");
            hintChange.Font = new Font("Segoe UI Light", 13);
            hintChange.ForeColor = Color.FromArgb(204, 204, 204);
            container.Controls.Add(hintChange);
            hintChange.Location = new Point(0, offsetY);
            offsetY += hintChange.Height + 12;
            Change.AutoSize = true;
            Change.Font = new Font("Segoe UI Light", 16);
            container.Controls.Add(Change);
            Change.Location = new Point(0, offsetY);
            offsetY += Change.Height + 12;
            // offset for buttons
            offsetY += 32;
            // mark order paid
            BtnPaid.AutoSize = true;
            BtnPaid.Text = Root.GetMsg("cart.mark-and-return");
            BtnPaid.Click += ActionPaymentCompleted;
            BtnPaid.Disabled = true;
            container.Controls.Add(BtnPaid);
            BtnPaid.Location = new Point(0, offsetY);
            offsetY += BtnPaid.Height + 12;
            // print invoice checkbox
            container.Controls.Add(PrintInvoice);
            PrintInvoice.Location = new Point(BtnPaid.Left + BtnPaid.Width + 32, BtnPaid.Top + BtnPaid.Height / 2 - PrintInvoice.Height / 2);
            Label hintPrintInvoice = new Label();
            hintPrintInvoice.AutoSize = true;
            hintPrintInvoice.Text = Root.GetMsg("cart.print-invoice");
            hintPrintInvoice.Font = new Font("Segoe UI Light", 12);
            hintPrintInvoice.ForeColor = Color.FromArgb(204, 204, 204);
            container.Controls.Add(hintPrintInvoice);
            hintPrintInvoice.Location = new Point(PrintInvoice.Left + PrintInvoice.Width + 8, BtnPaid.Top + BtnPaid.Height / 2 - hintPrintInvoice.Height / 2);

            _This.ContentPanel.Controls.Add(container);
            container.Location = new Point(_This.ContentPanel.Width / 2 - container.Width / 2, _This.ContentPanel.Height / 2 - container.Height / 2 - _This.ContentPanel.Top / 2);
        }

        private void ActionPaymentCompleted(object sender, EventArgs e)
        {
            DateTime completedAt = DateTime.Now;
            string invoiceID = Printing.GenerateInvoiceID(completedAt);
            // create invoice, payment record
            bool orderComplete = FinalCashIn >= FinalAmount && FinalAmount == FinalTotal;
            if (PaymentMethodCash)
                XQL.CreateRecords(invoiceID, orderComplete, "Cash", "");
            else
            {
                string refno = Guid.NewGuid().ToString("N");
                string last4 = new Random().Next(1000, 9999).ToString();
                string brand = Math.Round(new Random().NextDouble()) == 1 ? "MasterCard" : "Visa";
                XQL.CreateRecords(invoiceID, orderComplete, "Credit Card", "{RefNo:'" + refno + "',Last4:'" + last4 + "',Brand:'" + brand + "',Country:'HK',ExpYear:2019,ExpMonth:12,CvcPass:true}");
            }
            // create delivery record if order is delivery
            if (IsDelivery)
            {
                XQL.CreateDelivery(invoiceID, FinalAmount, CustomerName, CustomerAddress, CustomerPhone);
                Util.FileDeliveryReport(invoiceID, CustomerName, CustomerAddress, CustomerPhone, FinalTotal - FinalAmount);
            }
            // init which cart
            Dictionary<string, int> Cart = IsCheckout ? _Root.ShoppingCart : _Root.OrderCart;
            // create furniture sold invoice and update quantity
            string inIds = "[Furnitures].[ID] in ('',"; // <--- set empty id for default empty cart
            foreach (KeyValuePair<string, int> item in Cart)
                if (item.Value > 0) // quantity not zero
                    inIds += "'" + item.Key + "',";
            inIds = inIds.Remove(inIds.Length - 1) + ")";
            List<Furniture> items = XQL.GetFurnitures(inStore: true, where: inIds);
            if (IsCheckout)
            {
                string updateQtySql = "update [StoreFurnitures] set [_Quantity] = switch(";
                foreach (Furniture furn in items)
                {
                    double soldPrice = (furn.Price * Cart[furn.ID]) * ((100 - furn.Discount) / 100f);
                    XQL.CreateFurnitureInvoice(invoiceID, furn.ID, Cart[furn.ID], soldPrice);
                    updateQtySql += "[_FID] = '" + furn.ID + "', [_Quantity] - " + Cart[furn.ID] + ",";
                }
                updateQtySql += "true, [_Quantity]) where [_SID] = " + Root.Store.ID + ";";
                XQL.UpdateQuantity(updateQtySql);
            }
            else
            {
                string updateQtySql = "update [Furnitures] set [_InventoryQuantity] = switch(";
                foreach (Furniture furn in items)
                {
                    double soldPrice = (furn.Price * Cart[furn.ID]) * ((100 - furn.Discount) / 100f);
                    XQL.CreateFurnitureInvoice(invoiceID, furn.ID, Cart[furn.ID], soldPrice);
                    updateQtySql += "[ID] = '" + furn.ID + "', [_InventoryQuantity] - " + Cart[furn.ID] + ",";
                }
                updateQtySql += "true, [_InventoryQuantity]);";
                XQL.UpdateQuantity(updateQtySql);
            }
            // generate invoice and save to disk
            string invoice = Printing.GenerateInvoice(invoiceID);
            Util.SaveInvoice(invoiceID, invoice);
            // print invoice
            if (PrintInvoice.Checked) Printing.PrintPage(_Root, invoice);
            // empty cart
            Cart.Clear();
            // start new order
            BackToManage(reloadProducts: true);
        }
    }
}
