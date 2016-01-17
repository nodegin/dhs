using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

using DHS.Misc;

namespace DHS
{
    class SendReorderReportForm : BackableForm
    {

        private bool NilRecord = true;
        private string UpdateQtySql;

        public SendReorderReportForm(Root root, MetroForm parent) : base(root, parent, Root.GetMsg("main.menu.reorder-report"))
        {
            _This.InsertActionButtons(Root.GetMsg("snrp.send"), Properties.Resources.send, (sender, e) =>
            {
                if (!NilRecord)
                {
                    XQL.UpdateQuantity(UpdateQtySql);
                    MessageBox.Show(Root.GetMsg("snrp.reorder-report-sent"), Root.GetMsg("ui.info"));
                    OnBack();
                }
                else MessageBox.Show(Root.GetMsg("snrp.empty-reorder"), Root.GetMsg("ui.warn"));
            });
            WebBrowser Preview = new WebBrowser();
            Preview.Size = new Size(_This.ContentPanel.Width, _This.ContentPanel.Height);
            Preview.DocumentText = GetReportContent();
            _This.ContentPanel.Controls.Add(Preview);
        }

        private string GetReportContent()
        {
            List<Furniture> reorders = XQL.GetFurnitures(inStore: true, where: "[StoreFurnitures].[_Quantity] <= [StoreFurnitures].[_ReorderLevel]");
            if (reorders.Count > 0)
            {
                NilRecord = false;
                string html = File.ReadAllText(Path.Combine(Util.GetSharedDirectory(), "ReorderReport.html"));
                html = html.Replace("{{CURRENT_DATE}}", DateTime.Now.ToString("yyyy/M/d H:mm"));
                html = html.Replace("{{STORE_NAME}}", Root.Store.Name);
                string items = "";
                UpdateQtySql = "update [StoreFurnitures] set [_Quantity] = ([_Quantity] + [_ReorderLevel] * 3) where [_FID] in(";
                foreach (Furniture furn in reorders)
                {
                    UpdateQtySql += "'" + furn.ID + "',";
                    items += "<tr>" +
                        "<td class='id'>" + furn.ID + "</td>" +
                        "<td class='name'>" + furn.Name + "</td>" +
                        "<td class='qty rtx'>" + furn.Quantity + "</td>" +
                        "<td class='reorder rtx'>" + furn.ReorderLevel + "</td>" +
                        "<td class='diff rtx'>" + (furn.Quantity - furn.ReorderLevel) + "</td>" +
                        "<td class='replenish rtx'>" + (furn.ReorderLevel * 3) + "</td>" +
                    "</tr>";
                }
                UpdateQtySql = UpdateQtySql.TrimEnd(',') + ") and [_SID] = " + Root.Store.ID + ";";
                html = html.Replace("{{ITEM_RECORDS}}", items);

                return html;
            }
            return "<html><head><style>html{background:" + ColorTranslator.ToHtml(_This.BackColor) + ";color:#fff;font:20px 'Segoe UI Light', sans-serif;padding:16px 24px}</style></head>" +
                "<body>" + Root.GetMsg("snrp.empty-reorder") + "</body></html>";
        }
    }
}
