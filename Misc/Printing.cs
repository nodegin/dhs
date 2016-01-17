using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace DHS.Misc
{
    class Printing
    {
        public static string GenerateInvoice(string invID)
        {
            string html = File.ReadAllText(Path.Combine(Util.GetSharedDirectory(), "Invoice.html"));
            html = html.Replace("{{INVOICE_ID}}", invID);
            html = html.Replace("{{CURRENT_DATE}}", DateTime.Now.ToString("yyyy/M/d H:mm"));
            html = html.Replace("{{STAFF_NAME}}", Root.CurrentStaff.Name);
            html = html.Replace("{{STORE_NAME}}", Root.Store.Name);
            string records = "";
            int i = 1;
            double totalAmount = 0;
            foreach (DataRow row in XQL.GetInvoiceFurnitures(invID))
            {
                double amount = double.Parse(row["Amount"].ToString());
                records += "<tr>" +
                    "<td class='index'>" + i + "</td>" +
                    "<td class='id'>" + row["ID"] + "</td>" +
                    "<td class='name'>" + row["Name"] + "</td>" +
                    "<td class='price'>" + row["Price"] + "</td>" +
                    "<td class='qty'>" + row["Quantity"] + "</td>" +
                    "<td class='amount'>" + amount.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")) + "</td>" +
                "</tr>";
                totalAmount += amount;
                i++;
            }
            html = html.Replace("{{ITEM_RECORDS}}", records);
            double due = XQL.GetInvoiceDue(invID);
            html = html.Replace("{{AMOUNT_TOTAL}}", totalAmount.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")));
            html = html.Replace("{{AMOUNT_PAID}}", (totalAmount - due).ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")));
            if (XQL.InvoiceIsDelivery(invID))
            {
                html = html.Replace("{{AMOUNT_DUE_VISIBILITY}}", "");
                html = html.Replace("{{AMOUNT_DUE}}", due.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")));
                html = html.Replace("{{DELIVERY_HINT}}", "NOTE<br>" +
                    "<sup>1</sup> Items will delivered within 5 working days once availability comfirmed<br>" +
                     "<sup>2</sup> Remain amount will be charged once items delivered to address");
            }
            else
            {
                html = html.Replace("{{AMOUNT_DUE_VISIBILITY}}", "style='display:none'");
                html = html.Replace("{{AMOUNT_DUE}}", "");
                html = html.Replace("{{DELIVERY_HINT}}", "");
            }
            return html;
        }

        public static string GenerateInvoiceID(DateTime time)
        {
            return "DH" + new string(Root.Store.Area.Split(' ').Select(s => s[0]).ToArray()).Substring(0, 2).ToUpper() +
                long.Parse(time.ToString("yyMMddHHmmss")).ToString("X");
        }

        public static void PrintPage(Form caller, string html)
        {
            WebBrowser webBrowserForPrinting = new WebBrowser();
            webBrowserForPrinting.Size = Size.Empty;
            webBrowserForPrinting.Visible = false;
            webBrowserForPrinting.DocumentCompleted += (sender, e) =>
            {
                webBrowserForPrinting.ShowPrintPreviewDialog();
                caller.Controls.Remove(webBrowserForPrinting);
            };
            webBrowserForPrinting.DocumentText = html;
            caller.Controls.Add(webBrowserForPrinting);
        }
    }
}
