using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DHS.Misc
{
    class Furniture
    {
        public string   ID;
        public string   Name;
        public string   Description;
        public string   Dimension;
        public double   Price;
        public double   Discount;
        public int      Quantity;
        public int      ReorderLevel;
        public int      InventoryQuantity;
        public string   ShelfLocation;
        public string   Category;
        public string   SubCategory;
        public Uri      Photo;
        public DateTime Date;

        public Furniture(DataRow item)
        {
            this.ID = item["ID"].ToString();
            this.Name = item["_Name"].ToString();
            this.Description = item["_Description"].ToString();
            this.Dimension = item["_Dimension"].ToString();
            this.Price = double.Parse(item["_Price"].ToString());
            double discount = double.Parse(item["_Discount"].ToString());
            this.Discount = discount > 100 ? 100 : discount < 0 ? 0 : discount;
            this.Quantity = Int32.Parse(item["_Quantity"].ToString());
            this.ReorderLevel = Int32.Parse(item["_ReorderLevel"].ToString());
            int invQty = Int32.TryParse(item["_InventoryQuantity"].ToString(), out invQty) ? invQty : 0;
            this.InventoryQuantity = invQty;
            this.ShelfLocation = item["_ShelfLocation"].ToString();
            this.Category = item["_Category"].ToString();
            this.SubCategory = item["_SubCategory"].ToString();
            this.Photo = new Uri(item["_Photo"].ToString());
            this.Date = DateTime.ParseExact(item["_Date"].ToString(), "d/M/yyyy H:mm:ss", null);
        }
        
    }
}
