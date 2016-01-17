using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Globalization;
using Transitions;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class MainForm : MetroForm
    {

        private Root _Root;
        private int MenuCategoryHeight = 24;
        private int MenuButtonHeight = 72;
        private int OffsetY = 0;
        
        public MainForm(Root p) : base(p, Root.GetMsg("main.title"), 540, 72)
        {
            _Root = p;
            // home icon web@2x
            this.SuspendLayout();
            this.AutoSize = true;
            InsertMenu(Root.GetMsg("main.menu.store-furnitures"), Properties.Resources.menu_list, ViewFurnitures);
            if (Root.CurrentStaff.IsManager())
            {
                InsertCategory(Root.GetMsg("main.category.management"));
                InsertMenu(Root.GetMsg("main.menu.delivery-report"), Properties.Resources.menu_send, SendDeliveryReport);
                InsertMenu(Root.GetMsg("main.menu.reorder-report"), Properties.Resources.menu_reorder, SendReorderReport);
                InsertMenu(Root.GetMsg("main.menu.sales-report"), Properties.Resources.menu_chart, GenerateSalesReport);
                InsertMenu(Root.GetMsg("main.menu.import-furnitures"), Properties.Resources.menu_import, ImportFurniture);
            }
            this.ResumeLayout(false);
            AnimateShowFormTrigger();
            IsLaunched = true;
        }

        private bool IsLaunched = false;
        private void AnimateShowFormTrigger()
        {
            this.Show();
            Transition t = new Transition(new TransitionType_Deceleration(100));
            int tempY = this.Location.Y;
            this.CenterToScreen();
            int centerY = this.Location.Y;
            this.Top = tempY;
            t.add(this, "Top", centerY);
            t.add(this, "Opacity", 1.0);
            t.run();
        }

        public override void AnimateShowForm(EventHandler<Transition.Args> handler = null)
        {
            if (IsLaunched) AnimateShowFormTrigger();
        }

        private void InsertCategory(string category)
        {
            Label cat = new Label();
            cat.Font = new Font("Segoe UI Light", 13);
            cat.ForeColor = Color.FromArgb(204, 204, 204);
            cat.Text = category;
            cat.Size = new Size(this.ContentPanel.Width, MenuCategoryHeight);
            cat.Left = this.Title.Left;
            cat.Top = OffsetY;
            this.ContentPanel.Controls.Add(cat);
            OffsetY += MenuCategoryHeight + 12;
            this.ContentPanel.Height = OffsetY + 12;
        }

        private void InsertMenu(string text, Bitmap icon, EventHandler handler)
        {
            MetroButton btn = new MetroButton(fontSize: 16);
            btn.BackColor = Color.FromArgb(45, 45, 45);
            btn.FlatAppearance.BorderSize = 0;
            btn.Image = icon;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextAlign = ContentAlignment.MiddleRight;
            btn.Size = new Size(this.ContentPanel.Width - this.Title.Left * 2, MenuButtonHeight);
            btn.Left = this.Title.Left;
            btn.Click += handler;
            btn.Top = OffsetY;
            btn.Padding = new Padding(16, 0, 16, 0);
            btn.Text = text;
            this.ContentPanel.Controls.Add(btn);
            OffsetY += MenuButtonHeight + 12;
            this.ContentPanel.Height = OffsetY + 12;
        }

        private void ViewFurnitures(object sender, EventArgs e)
        {
            new StoreFurnituresForm(_Root, this).Show();
        }

        private void SendDeliveryReport(object sender, EventArgs e)
        {
            new SendDeliveryReportForm(_Root, this).Show();
        }

        private void SendReorderReport(object sender, EventArgs e)
        {
            new SendReorderReportForm(_Root, this).Show();
        }

        private void GenerateSalesReport(object sender, EventArgs e)
        {
            new GenerateSalesReportForm(_Root, this).Show();
        }
        
        private void ImportFurniture(object sender, EventArgs e)
        {
            new ImportFurnitureForm(_Root, this).Show();
        }

    }
}
