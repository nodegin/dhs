using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using Transitions;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class ImportFurnitureForm : BackableForm
    {

        private int TopSelectorBarSize = 64;
        private int RowSize = 80;
        private ScrollPanel ItemList = new ScrollPanel();
        private string QueryCat = null, QuerySubCat;

        public ImportFurnitureForm(Root root, IParentForm parent) : base(root, parent, Root.GetMsg("main.menu.import-furnitures"))
        {
            _This.InsertActionButtons(Root.GetMsg("imfu.import-csv"), Properties.Resources.import_file, ActionImportFromCSV);
            ItemList.Size = new Size(_This.ContentPanel.Width, _This.ContentPanel.Height - TopSelectorBarSize);
            ItemList.Top = TopSelectorBarSize;
            _This.ContentPanel.Controls.Add(ItemList);
            InsertCategorySelector();
        }

        private void InsertCategorySelector()
        {
            int comboBoxHeight = 32;
            Panel top = new Panel();
            top.Size = new Size(_This.ContentPanel.Width, TopSelectorBarSize);
            _This.ContentPanel.Controls.Add(top);
            // category selector
            MetroComboBox selCategory = new MetroComboBox();
            selCategory.Size = new Size((_This.ContentPanel.Width / 2) - 16 - 16 / 2, comboBoxHeight);
            top.Controls.Add(selCategory);
            selCategory.Location = new Point(16, top.Height / 2 - comboBoxHeight / 2);
            // sub-category selector
            MetroComboBox selSubCategory = new MetroComboBox();
            selSubCategory.Size = new Size((_This.ContentPanel.Width / 2) - 16 - 16 / 2, comboBoxHeight);
            top.Controls.Add(selSubCategory);
            selSubCategory.Location = new Point(16 + selCategory.Width + 16, top.Height / 2 - comboBoxHeight / 2);
            // set items
            selCategory.SelectedIndexChanged += (sender, e) =>
            {
                int id = (int)((sender as ComboBox).SelectedItem as ComboItem).Value;
                selSubCategory.Items.Clear();
                selSubCategory.Items.AddRange(XQL.GetSubcategoeiesForDropDown(id, Root.GetMsg("imfu.any-subcategory")));
                selSubCategory.SelectedIndex = 0;
                if (id > 0)
                    QueryCat = "[Furnitures].[_CID] = " + id;
                else
                    QueryCat = null;
                QuerySubCat = null;
                UpdateList();
            };
            selCategory.Items.Clear();
            selCategory.Items.AddRange(XQL.GetCategoeiesForDropDown(Root.GetMsg("imfu.any-category")));
            selCategory.SelectedIndex = 0;
            selSubCategory.SelectedIndexChanged += (sender, e) =>
            {
                int id = (int)((sender as ComboBox).SelectedItem as ComboItem).Value;
                if (id > 0)
                    QuerySubCat = " and [Furnitures].[_SCID] = " + id;
                else
                    QuerySubCat = null;
                UpdateList();
            };
        }

        private void UpdateList()
        {
            List<Furniture> furnitures = XQL.GetFurnitures(
                inStore: false,
                where: QueryCat != null && QuerySubCat != null ? QueryCat + QuerySubCat : QueryCat != null ? QueryCat : null);
            int scrollbarAdjust = furnitures.Count * RowSize >= ItemList.Height ? SystemInformation.VerticalScrollBarWidth : 0; 
            ItemList.Controls.Clear();
            if (furnitures.Count < 1)
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
            else for (int i = 0; i < furnitures.Count; i++)
            {
                Furniture item = furnitures[i];
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
                pic.LoadCompleted += (sender, e) =>
                {
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
                // Import button
                Button btnImport = new Button();
                btnImport.BackColor = Color.FromArgb(204, 128, 34);
                btnImport.FlatStyle = FlatStyle.Flat;
                btnImport.FlatAppearance.BorderSize = 0;
                btnImport.Size = new Size((int)(RowSize * 0.75), RowSize - 2);
                btnImport.Click += (sender, e) => ActionImportItem(sender, e, item);
                btnImport.TabStop = false;
                btnImport.Location = new Point(ItemList.Width - btnImport.Width - scrollbarAdjust, 1);
                btnImport.BackgroundImage = Properties.Resources.import;
                btnImport.BackgroundImageLayout = ImageLayout.Center;
                
                row.Controls.Add(btnImport);
            }
        }

        private void ActionImportFromCSV(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                string msg = "";
                int inserted = 0;
                int updated = 0;
                int failed = 0;
                var reader = new StreamReader(File.OpenRead(file.FileName));
                if (file.FileName.EndsWith("csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (String.IsNullOrWhiteSpace(line)) continue;
                        var m = Regex.Match(line, "'(?<id>.+)',(?<quantity>\\d+),(?<reorderLevel>\\d+),'(?<shelf>[a-zA-Z][0-9]{2})'");
                        if (!m.Success)
                        {
                            failed++;
                            continue;
                        }
                        int ret = XQL.ImportFurniture(m.Groups["id"].Value, m.Groups["quantity"].Value, m.Groups["reorderLevel"].Value, m.Groups["shelf"].Value.ToUpper());
                        if (ret == 1) inserted++;
                        else if (ret == 2) updated++;
                        else if (ret == 0) failed++;
                    }
                    if (inserted > 0) msg += inserted + " " + Root.GetMsg("imfu.records-inserted") + "\n";
                    if (updated > 0) msg += updated + " " + Root.GetMsg("imfu.records-updated") + "\n";
                    if (failed > 0) msg += failed + " " + Root.GetMsg("imfu.records-failed") + "\n";
                    msg.TrimEnd('\n');
                }
                else msg = Root.GetMsg("imfu.invalid-csv");
                if (!String.IsNullOrEmpty(msg)) MessageBox.Show(msg);
                if (inserted > 0 || updated > 0) OnBack();
            }
        }

        private Panel DetailInputPane = new Panel();
        private int FieldOffsetY = 0;
        private int FieldHeight = 36;
        private int FieldPadding = 24;
        private int ButtonOffsetX = 0;
        private Size PaneAllFieldSize = Size.Empty;

        private void ActionImportItem(object sender, EventArgs e, Furniture dt)
        {
            _This.ContentPanel.SuspendLayout();
            DetailInputPane.AutoSize = true;
            DetailInputPane.BackColor = Color.Transparent;
            Panel DetailBackground = new Panel();
            Bitmap screenshot = new Bitmap(_This.ContentPanel.Width, _This.ContentPanel.Height, PixelFormat.Format32bppArgb);
            Graphics gfxScreenshot = Graphics.FromImage(screenshot);
            Point location = _This.ContentPanel.PointToScreen(Point.Empty);
            gfxScreenshot.CopyFromScreen(location.X, location.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            gfxScreenshot.FillRectangle(new SolidBrush(Color.FromArgb(204, _This.BackColor)), 0, 0, screenshot.Width, screenshot.Height);
            gfxScreenshot.Dispose();
            Bitmap resized = new Bitmap(screenshot, new Size((int)(screenshot.Width * .75), (int)(screenshot.Height * .75)));
            ImageTools.FastBlur(resized, 8);
            DetailBackground.BackgroundImage = resized;
            DetailBackground.BackgroundImageLayout = ImageLayout.Stretch;
            DetailBackground.Size = _This.ContentPanel.Size;
            DetailBackground.Controls.Add(DetailInputPane);
            _This.ContentPanel.Controls.Add(DetailBackground);
            TextBox BoxQty = InsertField(Root.GetMsg("edit.quantity"), (sndr, evnt) => { Validation.NumberField(sndr, evnt, 4); });
            TextBox BoxReordrLv = InsertField(Root.GetMsg("edit.rl"), (sndr, evnt) => { Validation.NumberField(sndr, evnt, 4); });
            TextBox BoxShelf = InsertField(Root.GetMsg("edit.sl"), Validation.ShelfLocationField);
            InsertButton(Root.GetMsg("imfu.import"), (sndr, evnt) =>
            {
                try
                {
                    if (XQL.ImportFurniture(dt.ID,
                        quantity: BoxQty.Text,
                        reorderLevel: BoxReordrLv.Text,
                        shelfLocation: BoxShelf.Text) == 1)
                        OnBack();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, Root.GetMsg("ui.errr"));
                }
            });
            InsertButton(Root.GetMsg("imfu.cancel"), (sndr, evnt) =>
            {
                DetailInputPane = new Panel();
                FieldOffsetY = 0;
                ButtonOffsetX = 0;
                PaneAllFieldSize = Size.Empty;
                _This.ContentPanel.Controls.Remove(DetailBackground);
                _This.Title.Text = Root.GetMsg("main.menu.import-furnitures");
            });
            DetailInputPane.Location = new Point(_This.ContentPanel.Width / 2 - DetailInputPane.Width / 2, _This.ContentPanel.Height / 2 - DetailInputPane.Height / 2);
            DetailBackground.BringToFront();
            _This.Title.Text = Root.GetMsg("imfu.import") + " " + dt.Name;
            _This.ContentPanel.ResumeLayout(false);
        }

        private TextBox InsertField(string text, KeyPressEventHandler validation)
        {
            Label lbl = new Label();
            // Label
            lbl.BackColor = Color.Transparent;
            lbl.ForeColor = Color.White;
            lbl.Font = new Font("Segoe UI Light", 16);
            lbl.Location = new Point(0, FieldOffsetY);
            lbl.Text = text;
            lbl.Size = new Size((int)(_This.Width * .2), FieldHeight);
            DetailInputPane.Controls.Add(lbl);
            // Input
            TextBox tbx = new MetroTextBox((int)(_This.Width * .4), FieldHeight, false);
            tbx.Location = new Point(lbl.Left + lbl.Width + FieldPadding, FieldOffsetY);
            tbx.ImeMode = System.Windows.Forms.ImeMode.Disable;
            if (validation != null) tbx.KeyPress += validation;
            FieldOffsetY += FieldHeight + FieldPadding;
            DetailInputPane.Controls.Add(tbx);
            return tbx;
        }

        private void InsertButton(string text, EventHandler handler)
        {
            MetroButton btn = new MetroButton();
            btn.AutoSize = true;
            btn.Click += handler;
            btn.MouseEnter += (sender, e) => { btn.BackColor = Color.FromArgb(128, 45, 45, 45); };
            btn.MouseDown += (sender, e) => { btn.BackColor = Color.FromArgb(128, 34, 34, 34); };
            btn.MouseLeave += (sender, e) => { btn.BackColor = Color.Transparent; };
            btn.Text = text;
            if (PaneAllFieldSize.IsEmpty)
                PaneAllFieldSize = DetailInputPane.Size;
            DetailInputPane.Controls.Add(btn);
            btn.Location = new Point(PaneAllFieldSize.Width - btn.Width - ButtonOffsetX - 3, PaneAllFieldSize.Height + 48);
            ButtonOffsetX = btn.Width + 48;
        }

    }
}
