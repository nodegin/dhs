using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class SendDeliveryReportForm : BackableForm
    {

        private string SelectedReport = null;
        private WebBrowser Preview = new WebBrowser();
        private ScrollPanel RecordList = new ScrollPanel();

        public SendDeliveryReportForm(Root root, MetroForm parent) : base(root, parent, Root.GetMsg("main.menu.delivery-report"))
        {
            _This.InsertActionButtons(Root.GetMsg("snrp.send"), Properties.Resources.send, (sender, e) =>
            {
                if (null != SelectedReport)
                {
                    File.Delete(SelectedReport);
                    Reset();
                }
                else MessageBox.Show(Root.GetMsg("snrp.choose-report"), Root.GetMsg("ui.warn"));
            });
            RecordList.Size = new Size(360, _This.ContentPanel.Height);
            _This.ContentPanel.Controls.Add(RecordList);
            Preview.Size = new Size(_This.ContentPanel.Width - 361, _This.ContentPanel.Height);
            Preview.Left = 361;
            _This.ContentPanel.Controls.Add(Preview);

            Reset();
        }

        private void Reset()
        {
            SelectedReport = null;
            Preview.DocumentText = "<html><head><style>html{background:" + ColorTranslator.ToHtml(_This.BackColor) + "}</style></head><body></body></html>";
            RefreshList();
        }

        private void RefreshList()
        {
            RecordList.Controls.Clear();
            foreach (string f in Directory.GetFiles(Util.GetDeliveryReportsDirectory()))
            {
                Label record = new Label();
                record.Size = new Size(RecordList.Width, 48);
                record.Font = new Font("Segoe UI Light", 14);
                record.Padding = new Padding(12, 6, 12, 6);
                record.Text = Path.GetFileNameWithoutExtension(f);
                record.TextAlign = ContentAlignment.MiddleLeft;
                record.Image = Properties.Resources.arrow_right;
                record.ImageAlign = ContentAlignment.MiddleRight;
                record.BackColor = Color.FromArgb(45, 45, 45);
                record.Margin = Padding.Empty;
                record.Cursor = Cursors.Hand;
                record.Click += (sender, e) =>
                {
                    if (SelectedReport != f || SelectedReport == null)
                    {
                        Preview.Url = new Uri(f);
                        SelectedReport = f;
                    }
                    else Reset();
                };
                record.MouseDown += (sender, e) =>
                {
                    record.BackColor = Color.FromArgb(77, 77, 77);
                };
                record.MouseUp += (sender, e) =>
                {
                    record.BackColor = Color.FromArgb(45, 45, 45);
                };
                RecordList.Controls.Add(record);
            }
        }

    }
}
