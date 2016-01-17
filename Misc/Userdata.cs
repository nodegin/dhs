using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace DHS.Misc
{
    class Userdata
    {

        public int ID;
        public string Name;
        public string Email;
        public char Gender;
        public DateTime EmploymentDate;
        public int Phone;
        public string Address;
        public int Salary;
        public int Store;
        public string Position;

        public Userdata(DataRow item)
        {
            this.ID = Int32.Parse(item["ID"].ToString());
            this.Name = item["_FirstName"].ToString() + " " + item["_LastName"].ToString();
            this.Email = item["_Email"].ToString();
            this.Gender = item["_Gender"].ToString().ToCharArray()[0];
            this.EmploymentDate = DateTime.ParseExact(item["_EmploymentDate"].ToString(), "d/M/yyyy H:mm:ss", null);
            this.Phone = Int32.Parse(item["_Phone"].ToString());
            this.Address = item["_Address"].ToString();
            this.Salary = Int32.Parse(item["_Salary"].ToString());
            this.Store = Int32.Parse(item["_SID"].ToString());
            this.Position = item["_Position"].ToString();
        }

        public bool IsManager()
        {
            return this.Position.Equals("Manager");
        }

    }
}
