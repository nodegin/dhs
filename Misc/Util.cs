using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace DHS.Misc
{
    class Util
    {

        public static string GetProjectDirectory()
        {
            string project = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            DirectoryInfo currPath = new DirectoryInfo(Assembly.GetExecutingAssembly().Location);
            while (!currPath.Name.Equals(project)) currPath = currPath.Parent;
            return currPath.FullName;
        }

        public static string GetSharedDirectory()
        {
            string binDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(binDir, "DHS_Files");
        }

        public static string GetTempDirectory()
        {
            return Path.Combine(Util.GetSharedDirectory(), "_Temp");
        }

        public static string GetDeliveryReportsDirectory()
        {
            string reportDir = Path.Combine(Util.GetSharedDirectory(), "DeliveryReports");
            Directory.CreateDirectory(reportDir); // create if not exist
            return reportDir;
        }
        
        public static void SaveInvoice(string invID, string content)
        {
            string invDir = Path.Combine(Util.GetSharedDirectory(), "Invoices");
            Directory.CreateDirectory(invDir); // create if not exist
            File.WriteAllText(Path.Combine(invDir, invID + ".html"), content, Encoding.UTF8);
        }

        public static void FileDeliveryReport(string invID, string customer, string address, int phone, double remain)
        {
            string template = Path.Combine(Util.GetSharedDirectory(), "DeliveryReport.html");
            string todayReport = Path.Combine(Util.GetDeliveryReportsDirectory(), DateTime.Now.ToString("yyyyMMdd") + "-DeliveryReport.html");
            
            if (!File.Exists(todayReport))
            {
                string tpl = File.ReadAllText(template);
                tpl = tpl.Replace("{{CURRENT_DATE}}", DateTime.Now.ToString("yyyy/M/d"));
                tpl = tpl.Replace("{{STORE_NAME}}", Root.Store.Name);
                File.WriteAllText(todayReport, tpl, Encoding.UTF8);
            }
            string fContent = File.ReadAllText(todayReport);
            fContent = fContent.Insert(fContent.IndexOf("<!--INJECT-POINT-->"), "<li>" +
                    "Invoice<span class='tab'>" + invID + "</span><br>" +
                    "Destination: <span class='tab'>" + address + "</span><br>" +
                    "Contact: <span class='tab'>" + customer + " (Phone: " + phone + ")</span><br>" +
                    "Remain: <span class='tab'>" + remain.ToString("C", CultureInfo.CreateSpecificCulture("zh-HK")) + "</span>" + 
                "</li>");
            File.WriteAllText(todayReport, fContent, Encoding.UTF8);
        }
        
        public static string CalcDiscountedPrice(Furniture item)
        {
            bool hasDiscount = item.Discount > 0;
            double discount = (100 - item.Discount) / 100f;
            double amount = hasDiscount ? item.Price * discount : item.Price;
            string percentOff = hasDiscount ? " (-" + item.Discount + "%)" : "";
            return amount
                .ToString("C", CultureInfo.CreateSpecificCulture("zh-HK"))
                + percentOff;
        }

        public static double CalcPrimaryPrice(Furniture item)
        {
            // display price (* discount?) > retail price (* discount?)

            return 0.0;
        }
    }
}
