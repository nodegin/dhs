using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DHS.Misc
{
    class StoreInfo
    {
        public int ID;
        public string Name;
        public string Area;
        public string Address;
        public int Phone;
        public Uri Photo;

        public StoreInfo(DataRow item)
        {
            this.ID = Int32.Parse(item["ID"].ToString());
            this.Name = item["_Name"].ToString();
            this.Area = item["_Area"].ToString();
            this.Address = item["_Address"].ToString();
            this.Phone = Int32.Parse(item["_Phone"].ToString());
            this.Photo = new Uri(item["_Photo"].ToString());
        }

    }
}
