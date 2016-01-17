using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Threading.Tasks;
using System.Globalization;

using DHS.Misc;

namespace DHS
{
    class GenerateSalesReportForm : BackableForm
    {

        public GenerateSalesReportForm(Root root, MetroForm parent) : base(root, parent, Root.GetMsg("main.menu.sales-report"))
        {
            WebBrowser Preview = new WebBrowser();
            _This.InsertActionButtons(Root.GetMsg("gsrp.print"), Properties.Resources.print, (sender, e) =>
            {
                Printing.PrintPage(_Root, Preview.DocumentText);
            });
            Preview.Size = new Size(_This.ContentPanel.Width, _This.ContentPanel.Height);
            Preview.DocumentText = GetReportContent();
            _This.ContentPanel.Controls.Add(Preview);
        }

        private string GetReportContent()
        {
            string html = File.ReadAllText(Path.Combine(Util.GetSharedDirectory(), "SalesReport.html"));
            html = html.Replace("{{CURRENT_DATE}}", DateTime.Now.ToString("yyyy/M/d H:mm"));
            html = html.Replace("{{STORE_NAME}}", Root.Store.Name);
            // daily sales summary selects
            DataRowCollection salesRecords = XQL.GetDailySalesRecords();
            int dayTotalSoldQty = 0;
            foreach (DataRow row in salesRecords) dayTotalSoldQty += Int32.Parse(row["_SoldQuantity"].ToString());
            // generate line chart for items
            string dailySummary = "<div class='lineChart'>";
            Color baseDailySummaryLineColor = ColorTranslator.FromHtml("#f48fb1");
            if (salesRecords.Count < 1)
                dailySummary += "<div><div style='padding:8px'>NO DATA</div></div>";
            foreach (DataRow row in salesRecords)
            {
                float w = (float.Parse(row["_SoldQuantity"].ToString()) / dayTotalSoldQty);
                int R = Math.Min(baseDailySummaryLineColor.R + (int)(48 * w), 224);
                int G = Math.Min(baseDailySummaryLineColor.G + (int)(48 * w), 224);
                int B = Math.Min(baseDailySummaryLineColor.B + (int)(48 * w), 224);
                Color c = Color.FromArgb(R, G, B);
                string perc = Math.Round(w * 100, 1).ToString();
                dailySummary += "<div><div><span style='background:" + ColorTranslator.ToHtml(c) + ";width:" + perc + "%'>" +
                    perc + "%&emsp;" + row["_Name"] + "&emsp;&emsp;Sold Qty: " + row["_SoldQuantity"] + "</span></div></div>";
            }
            dailySummary += "</div>";
            html = html.Replace("<!--DAILY_SALES_SUMMARY-->", dailySummary);

            // weekly sales summary selects
            DataRowCollection salesAmounts = XQL.GetWeeklySalesAmounts();
            long weekTotalSoldAmount = 0;
            foreach (DataRow row in salesAmounts) weekTotalSoldAmount += long.Parse(row["_Amount"].ToString());
            string weeklySummary = "<div class='lineChart'>";
            Color baseWeeklySummaryLineColor = ColorTranslator.FromHtml("#a5d6a7");
            if (salesAmounts.Count < 1)
                weeklySummary += "<div><div style='padding:8px'>NO DATA</div></div>";
            foreach (DataRow row in salesAmounts)
            {
                float w = (float.Parse(row["_Amount"].ToString()) / weekTotalSoldAmount);
                int R = Math.Min(baseWeeklySummaryLineColor.R + (int)(48 * w), 224);
                int G = Math.Min(baseWeeklySummaryLineColor.G + (int)(48 * w), 224);
                int B = Math.Min(baseWeeklySummaryLineColor.B + (int)(48 * w), 224);
                Color c = Color.FromArgb(R, G, B);
                string perc = Math.Round(w * 100, 1).ToString();
                weeklySummary += "<div><div><span style='background:" + ColorTranslator.ToHtml(c) + ";width:" + perc + "%'>" +
                    perc + "%&emsp;" + row["_Day"] + "&emsp;&emsp;Sold Amount: " +
                    double.Parse(row["_Amount"].ToString()).ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")) + "</span></div></div>";
            }
            weeklySummary += "</div>";
            html = html.Replace("<!--WEEKLY_SALES_SUMMARY-->", weeklySummary);

            // todo: popular/non popular item report
            return html;
        }
    }
}
