using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace DHS.Misc
{
    class XQL
    {
        private static OleDbConnection DBConn;

        public static void InitConnection()
        {
            // init database
            string from = Path.Combine(Util.GetSharedDirectory(), "schemas.accdb");
            string to = Path.Combine(Util.GetTempDirectory(), "schemas.run.accdb");
            if (!File.Exists(to)) File.Copy(from, to, true);
            // init connection
            string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + to;
            DBConn = new OleDbConnection(connStr);
            DBConn.Open();
        }

        /*public static void DUMP()
        {
            DataSet dataSet = new DataSet();
            OleDbDataAdapter da1 = new OleDbDataAdapter("select * from [Invoices];", DBConn);
            da1.Fill(dataSet, "Invoices");
            OleDbDataAdapter da2 = new OleDbDataAdapter("select * from [Categories];", DBConn);
            da2.Fill(dataSet, "Categories");
            OleDbDataAdapter da3 = new OleDbDataAdapter("select * from [Subcategories];", DBConn);
            da3.Fill(dataSet, "Subcategories");
            OleDbDataAdapter da4 = new OleDbDataAdapter("select * from [Stores];", DBConn);
            da4.Fill(dataSet, "Stores");
            OleDbDataAdapter da5 = new OleDbDataAdapter("select * from [StoreFurnitures];", DBConn);
            da5.Fill(dataSet, "StoreFurnitures");
            OleDbDataAdapter da6 = new OleDbDataAdapter("select * from [Staffs];", DBConn);
            da6.Fill(dataSet, "Staffs");
            OleDbDataAdapter da7 = new OleDbDataAdapter("select * from [InvoiceFurnitures];", DBConn);
            da7.Fill(dataSet, "InvoiceFurnitures");
            OleDbDataAdapter da8 = new OleDbDataAdapter("select * from [InvoiceDeliveries];", DBConn);
            da8.Fill(dataSet, "InvoiceDeliveries");
            OleDbDataAdapter da9 = new OleDbDataAdapter("select * from [Furnitures];", DBConn);
            da9.Fill(dataSet, "Furnitures");
            OleDbDataAdapter da10 = new OleDbDataAdapter("select * from [Enquires];", DBConn);
            da10.Fill(dataSet, "Enquires");
            OleDbDataAdapter da11 = new OleDbDataAdapter("select * from [Feedbacks];", DBConn);
            da11.Fill(dataSet, "Feedbacks");
            OleDbDataAdapter da12 = new OleDbDataAdapter("select * from [PaymentRecords];", DBConn);
            da12.Fill(dataSet, "PaymentRecords");
            string cmd = "";
            foreach (DataTable table in dataSet.Tables)
            {
                String columnsCommandText = "(";
                foreach (DataColumn column in table.Columns)
                {
                    String columnName = column.ColumnName;
                    String dataTypeName = column.DataType.Name;
                    String sqlDataTypeName = dataTypeName;
                    columnsCommandText += "[" + columnName + "] " + GetSqlDataTypeName(sqlDataTypeName) + ",";
                }
                columnsCommandText = columnsCommandText.Remove(columnsCommandText.Length - 1);
                columnsCommandText += ");" + Environment.NewLine + Environment.NewLine;

                cmd += "create table " + table.TableName + columnsCommandText;
            }
            string strPath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
            strPath = Path.Combine(strPath, "creation.sql");
            File.WriteAllText(strPath, cmd);
        }*/

        public static string GetSqlDataTypeName(string sqlDataTypeName)
        {
            switch (sqlDataTypeName)
            {
                case "Int32":
                    return "number";
                case "String":
                    return "longtext";
                case "Boolean":
                    return "yesno";
                case "DateTime":
                    return "datetime";
                default:
                    return sqlDataTypeName;
            }
        }

        public static Userdata LoginUser(int id, string password)
        {
            string sql = "select * from [Staffs] where [ID] = " +
                id + " and [_Password] = '" + password + "';";
            DataTable dt = new DataTable();
            dt.Load(new OleDbCommand(sql, DBConn).ExecuteReader());
            return dt.Rows.Count != 1 ? null : new Userdata(dt.Rows[0]);
        }

        public static StoreInfo ObtainStoreInformations(int storeId)
        {
            string sql = "select * from [Stores] where [ID] = " + storeId + ";";
            DataTable dt = new DataTable();
            dt.Load(new OleDbCommand(sql, DBConn).ExecuteReader());
            return dt.Rows.Count != 1 ? null : new StoreInfo(dt.Rows[0]);
        }

        public static List<string> GetColumns()
        {
            List<string> fields = new List<string>();
            OleDbCommand cmd = new OleDbCommand("select * from [Furnitures];", DBConn);
            DataTable table = cmd.ExecuteReader(CommandBehavior.SchemaOnly).GetSchemaTable();
            foreach (DataRow row in table.Rows)
                fields.Add(row[table.Columns["ColumnName"]].ToString().ToLower());
            return fields;
        }

        public static object[] GetCategoeiesForDropDown(string labelForDefItem = "")
        {
            ComboItem def = new ComboItem(labelForDefItem, 0);
            object[] obj = new object[1] { def };
            DataTable dt = new DataTable();
            dt.Load(new OleDbCommand("select * from [Categories];", DBConn).ExecuteReader());
            if (dt.Rows.Count > 0)
            {
                obj = new object[dt.Rows.Count + 1];
                obj[0] = def;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ComboItem item = new ComboItem(dt.Rows[i]["_Name"].ToString(), Int32.Parse(dt.Rows[i]["ID"].ToString()));
                    obj[i + 1] = item;
                }
            }
            return obj;
        }

        public static object[] GetSubcategoeiesForDropDown(int id, string labelForDefItem = "")
        {
            ComboItem def = new ComboItem(labelForDefItem, 0);
            object[] obj = new object[1] { def };
            DataTable dt = new DataTable();
            dt.Load(new OleDbCommand("select * from [Subcategories] where [_CID] = " + id + ";", DBConn).ExecuteReader());
            if (dt.Rows.Count > 0)
            {
                obj = new object[dt.Rows.Count + 1];
                obj[0] = def;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ComboItem item = new ComboItem(dt.Rows[i]["_Name"].ToString(), Int32.Parse(dt.Rows[i]["ID"].ToString()));
                    obj[i + 1] = item;
                }
            }
            return obj;
        }

        public static List<Furniture> GetFurnitures(
            bool inStore,
            string where = null,
            string order = null)
        {
            List<Furniture> items = new List<Furniture>();
            DataTable dt = new DataTable();
            string sql;
            if (!inStore)
            {
                sql = "select [Furnitures].[ID]," +
                             "[Furnitures].[_Name]," +
                             "[Furnitures].[_Description]," +
                             "[Furnitures].[_Dimension]," +
                             "[Furnitures].[_Price]," +
                             "[Furnitures].[_Discount]," +
                             "[Furnitures].[_InventoryQuantity]," +
                             "0 as [_Quantity]," +
                             "0 as [_ReorderLevel]," +
                             "'' as [_ShelfLocation]," +
                             "[Categories].[_Name] as [_Category]," +
                             "[Subcategories].[_Name] as [_Subcategory]," +
                             "[Furnitures].[_Photo]," +
                             "[Furnitures].[_Date] " +
                             "from (" +
                               "[Furnitures] inner join [Categories] on [Furnitures].[_CID] = [Categories].[ID]" +
                             ") left join [Subcategories] on [Furnitures].[_SCID] = [Subcategories].[ID]" +
                             " where [Furnitures].[_CID] = [Categories].[ID] and " +
                             "[Furnitures].[_Inactive] = false and " +
                             "[Furnitures].[ID] not in (select [_FID] from [StoreFurnitures])";
            }
            else
            {
                sql = "select [Furnitures].[ID]," +
                             "[Furnitures].[_Name]," +
                             "[Furnitures].[_Description]," +
                             "[Furnitures].[_Dimension]," +
                             "[Furnitures].[_Price]," +
                             "[Furnitures].[_Discount]," +
                             "[Furnitures].[_InventoryQuantity]," +
                             "[StoreFurnitures].[_Quantity]," +
                             "[StoreFurnitures].[_ReorderLevel]," +
                             "[StoreFurnitures].[_ShelfLocation]," +
                             "[Categories].[_Name] as [_Category]," +
                             "[Subcategories].[_Name] as [_Subcategory]," +
                             "[Furnitures].[_Photo]," +
                             "[Furnitures].[_Date] " +
                             "from (" +
                               "(" +
                                 "[Furnitures] inner join [Categories] on [Furnitures].[_CID] = [Categories].[ID]" +
                               ") left join [Subcategories] on [Furnitures].[_SCID] = [Subcategories].[ID]" +
                             ") inner join [StoreFurnitures] on [StoreFurnitures].[_FID] = [Furnitures].[ID]" +
                             " where [StoreFurnitures].[_SID] = " + Root.Store.ID + " and " +
                             "[Furnitures].[_Inactive] = false and " +
                             "[Furnitures].[_CID] = [Categories].[ID]";
            }
            if (where != null) sql += " and " + where + " ";
            sql += " order by ";
            if (order != null) sql += order;
            else sql += "[Furnitures].[_Date] desc, [Categories].[_Name] asc, [Furnitures].[_Name] asc";
            sql += ";";
            dt.Load(new OleDbCommand(sql, DBConn).ExecuteReader());
            foreach (DataRow row in dt.Rows)
            {
                Furniture item = new Furniture(row);
                items.Add(item);
            }
            return items;
        }
		// furn id gen = [first c of C][first c of N][YYY = (2)015][hex of MMDD = (1231).toString(16) || (0101).toString(16) = 3 length str][2 length rand int]
        public static int ImportFurniture(string id,
            object quantity,
            object reorderLevel,
            string shelfLocation)
        {
            int iQuantity = -1;
            int iReorderLevel = -1;
            iQuantity = Int32.TryParse(quantity.ToString(), out iQuantity) ? iQuantity : -1;
            if (!Validation.ValidateQuantity(iQuantity)) throw new ArgumentException("Quantity is not valid.");
            iReorderLevel = Int32.TryParse(reorderLevel.ToString(), out iReorderLevel) ? iReorderLevel : -1;
            if (!Validation.ValidateQuantity(iReorderLevel)) throw new ArgumentException("Reorder Level is not valid.");
            if (!Validation.ValidateShelfLocation(shelfLocation)) throw new ArgumentException("Shelf Location is not valid.");

            bool update = new OleDbCommand(
                "select * from [StoreFurnitures] where [_SID] = " + Root.Store.ID + " and [_FID] = '" + id + "';",
                DBConn).ExecuteReader().HasRows;
            string sql = null;
            if (update)
            {
                sql = "update [StoreFurnitures] set [_Quantity] = " + iQuantity + ", [_ReorderLevel] = " +
                    iReorderLevel + ", [_ShelfLocation] = '" + shelfLocation + "' where [_SID] = " + Root.Store.ID +
                    " and [_FID] = '" + id + "';";
            }
            else  // check item exists
            {
                bool insert = new OleDbCommand(
                    "select * from [Furnitures] where [ID] = '" + id + "' and [_Inactive] = false;",
                    DBConn).ExecuteReader().HasRows;
                if (insert)
                {
                    sql = "insert into [StoreFurnitures] " +
                    "([_SID], [_FID], [_Quantity], [_ReorderLevel], [_ShelfLocation]) values " +
                    "(" + Root.Store.ID + ", '" + id + "', " + iQuantity + ", " + iReorderLevel + ", '" + shelfLocation + "');";
                }
            }
            if (sql != null)
            {
                OleDbCommand cmd = new OleDbCommand(sql, DBConn);
                if (cmd.ExecuteNonQuery() > 0) return update ? 2 : 1;
            }
            return 0;
        }

        // @target: staff, manager
        public static int UpdateFurniture(string id,
            string name = null,
            string description = null,
            string dimension = null,
            object price = null,
            object discount = null,
            object quantity = null,
            object reorderLevel = null,
            string shelfLocation = null,
            object categoryID = null,
            object subCategoryID = null,
            string photo = null)
        {
            double dPrice = -1.0;
            int iDiscount = -1;
            int iQuantity = -1;
            int iReorderLevel = -1;
            int iCategoryID = -1;
            int iSubcategoryID = -1;
            List<string> FurnituresSnippets = new List<string>();
            List<string> StoreFurnituresSnippets = new List<string>();
            bool updateName          = false;
            bool updateDescription   = false;
            bool updateDimension     = false;
            bool updatePrice         = false;
            bool updateDiscount      = false;
            bool updateQuantity      = false;
            bool updateReorderLevel  = false;
            bool updateShelfLocation = false;
            bool updateCategoryID    = false;
            bool updateSubcategoryID = false;
            bool updatePhoto         = false;
            // validate, check field(s) to be updated
            if (null != name)
            {
                if (Validation.ValidateName(name))
                {
                    FurnituresSnippets.Add("[_Name] = @Name");
                    updateName = true;
                }
                else throw new ArgumentException("Name is not valid.");
            }
            if (null != description)
            {
                if (Validation.ValidateDescription(description))
                {
                    FurnituresSnippets.Add("[_Description] = @Description");
                    updateDescription = true;
                }
                else throw new ArgumentException("Description is not valid.");
            }
            if (null != dimension)
            {
                if (Validation.ValidateDimension(dimension))
                {
                    FurnituresSnippets.Add("[_Dimension] = @Dimension");
                    updateDimension = true;
                }
                else throw new ArgumentException("Dimension is not valid.");
            }
            if (null != price)
                dPrice = double.TryParse(price.ToString(), out dPrice) ? dPrice : -1.0;
            if (Validation.ValidatePrice(dPrice))
            {
                FurnituresSnippets.Add("[_Price] = @Price");
                updatePrice = true;
            }
            else throw new ArgumentException("Price is not valid.");
            if (null != discount)
                iDiscount = Int32.TryParse(discount.ToString(), out iDiscount) ? iDiscount : -1;
            if (Validation.ValidateDiscount(iDiscount))
            {
                FurnituresSnippets.Add("[_Discount] = @Discount");
                updateDiscount = true;
            }
            else throw new ArgumentException("Discount is not valid.");
            if (null != quantity)
                iQuantity = Int32.TryParse(quantity.ToString(), out iQuantity) ? iQuantity : -1;
            if (Validation.ValidateQuantity(iQuantity))
            {
                StoreFurnituresSnippets.Add("[_Quantity] = @Quantity");
                updateQuantity = true;
            }
            else throw new ArgumentException("Quantity is not valid.");
            if (null != reorderLevel)
                iReorderLevel = Int32.TryParse(reorderLevel.ToString(), out iReorderLevel) ? iReorderLevel : -1;
            if (Validation.ValidateQuantity(iReorderLevel))
            {
                StoreFurnituresSnippets.Add("[_ReorderLevel] = @ReorderLevel");
                updateReorderLevel = true;
            }
            else throw new ArgumentException("Reorder Level is not valid.");
            if (null != shelfLocation)
            {
                if (Validation.ValidateShelfLocation(shelfLocation))
                {
                    StoreFurnituresSnippets.Add("[_ShelfLocation] = @ShelfLocation");
                    updateShelfLocation = true;
                }
                else throw new ArgumentException("Shelf Location is not valid.");
            }
            if (null != categoryID)
                iCategoryID = Int32.TryParse(categoryID.ToString(), out iCategoryID) ? iCategoryID : -1;
            if (Validation.ValidateCategoryID(iCategoryID))
            {
                FurnituresSnippets.Add("[_CID] = @CategoryID");
                updateCategoryID = true;
            }
            else throw new ArgumentException("Category ID is not valid.");
            if (null != subCategoryID)
                iSubcategoryID = Int32.TryParse(subCategoryID.ToString(), out iSubcategoryID) ? iSubcategoryID : -1;
            if (Validation.ValidateSubcategoryID(iSubcategoryID))
            {
                FurnituresSnippets.Add("[_SCID] = @SubcategoryID");
                updateSubcategoryID = true;
            }
            else throw new ArgumentException("Subcategory ID is not valid.");
            if (null != photo)
            {
                if (Validation.ValidatePhoto(photo))
                {
                    FurnituresSnippets.Add("[_Photo] = @Photo");
                    updatePhoto = true;
                }
                else throw new ArgumentException("Photo is not valid.");
            }
            int modified = 0;
            if (FurnituresSnippets.Count > 0)
            {
                string buildSql = string.Join(", ", FurnituresSnippets);
                OleDbCommand cmd = new OleDbCommand("update [Furnitures] set " +
                    buildSql + " where [ID] = '" + id + "'", DBConn);
                TextInfo ti = CultureInfo.CreateSpecificCulture("en-US").TextInfo;
                if (updateName)
                {
                    string v = ti.ToTitleCase(name.ToLower());
                    cmd.Parameters.AddWithValue("@Name", v);
                }
                if (updateDescription) cmd.Parameters.AddWithValue("@Description", description);
                if (updateDimension) cmd.Parameters.AddWithValue("@Dimension", dimension);
                if (updatePrice) cmd.Parameters.AddWithValue("@Price", dPrice);
                if (updateDiscount) cmd.Parameters.AddWithValue("@Discount", iDiscount);
                if (updateCategoryID) cmd.Parameters.AddWithValue("@CategoryID", iCategoryID);
                if (updateSubcategoryID) cmd.Parameters.AddWithValue("@SubcategoryID", iSubcategoryID);
                if (updatePhoto) cmd.Parameters.AddWithValue("@Photo", photo);
                modified += cmd.ExecuteNonQuery();
            }
            if (StoreFurnituresSnippets.Count > 0)
            {
                string buildSql = string.Join(", ", StoreFurnituresSnippets);
                OleDbCommand cmd = new OleDbCommand("update [StoreFurnitures] set " +
                    buildSql + " where [_SID] = " + Root.Store.ID + " and [_FID] = '" + id + "'", DBConn);
                if (updateQuantity) cmd.Parameters.AddWithValue("@Quantity", iQuantity);
                if (updateReorderLevel) cmd.Parameters.AddWithValue("@ReorderLevel", iReorderLevel);
                if (updateShelfLocation) cmd.Parameters.AddWithValue("@ShelfLocation", shelfLocation);
                modified += cmd.ExecuteNonQuery();
            }
            return modified;
        }

        public static int RemoveFurniture(string id)
        {
            OleDbCommand cmd = new OleDbCommand(
                "delete from [StoreFurnitures] where " +
                "[_SID] = " + Root.Store.ID + " and [_FID] = '" + id + "';", DBConn);
            return cmd.ExecuteNonQuery();
        }

        public static void CreateRecords(string invId, bool orderCompleted, string paymentMethod, string cardInfo)
        {
            // create invoice
            OleDbCommand cmdi = new OleDbCommand(
                "insert into [Invoices] ([ID], [_SID], [_Completed], [_Date]) values " +
                "(@InvoiceID, @StoreID, @Completed, now());", DBConn);
            cmdi.Parameters.AddWithValue("InvoiceID", invId);
            cmdi.Parameters.AddWithValue("StoreID", Root.Store.ID);
            cmdi.Parameters.AddWithValue("Completed", orderCompleted);
            cmdi.ExecuteNonQuery();
            // create payment record
            OleDbCommand cmdp = new OleDbCommand(
                "insert into [PaymentRecords] ([_IID], [_Type], [_Details]) values " +
                "(@InvoiceID, @Type, @Details);", DBConn);
            cmdp.Parameters.AddWithValue("InvoiceID", invId);
            cmdp.Parameters.AddWithValue("Type", paymentMethod);
            cmdp.Parameters.AddWithValue("Details", cardInfo);
            cmdp.ExecuteNonQuery();
        }

        public static void CreateDelivery(string invId, double deposit, string customer, string address, int phone)
        {
            // create invoice
            OleDbCommand cmdi = new OleDbCommand(
                "insert into [InvoiceDeliveries] ([_IID], [_Deposit], [_Name], [_Address], [_Phone]) values " +
                "(@InvoiceID, @Deposit, @Customer, @Address, @Phone);", DBConn);
            cmdi.Parameters.AddWithValue("InvoiceID", invId);
            cmdi.Parameters.AddWithValue("Deposit", deposit);
            cmdi.Parameters.AddWithValue("Customer", customer);
            cmdi.Parameters.AddWithValue("Address", address);
            cmdi.Parameters.AddWithValue("Phone", phone);
            cmdi.ExecuteNonQuery();
        }

        public static void CreateFurnitureInvoice(string invId, string furnId, int qty, double soldPrice)
        {
            // create invoice
            OleDbCommand cmdi = new OleDbCommand(
                "insert into [InvoiceFurnitures] ([_IID], [_FID], [_Quantity], [_SoldPrice]) values " +
                "(@InvoiceID, @Furniture, @Quantity, @SoldPrice);", DBConn);
            cmdi.Parameters.AddWithValue("InvoiceID", invId);
            cmdi.Parameters.AddWithValue("Furniture", furnId);
            cmdi.Parameters.AddWithValue("Quantity", qty);
            cmdi.Parameters.AddWithValue("SoldPrice", soldPrice);
            cmdi.ExecuteNonQuery();
        }

        public static bool InvoiceIsDelivery(string invId)
        {
            DataTable dt = new DataTable();
            dt.Load(new OleDbCommand(
                "select * from [InvoiceDeliveries] where [_IID] = '" + invId + "';", DBConn).ExecuteReader());
            return dt.Rows.Count > 0;
        }

        public static double GetInvoiceDue(string invId)
        {
            DataTable dt = new DataTable();
            dt.Load(new OleDbCommand(
                "select " +
                "sum([InvoiceFurnitures].[_SoldPrice]) - first(" +
                    "iif(isnull([InvoiceDeliveries].[_Deposit]), 99999999, [InvoiceDeliveries].[_Deposit])" +
                ") as [Due] from" +
                "([Invoices] left join [InvoiceDeliveries] on [InvoiceDeliveries].[_IID] = [Invoices].[ID]) " +
                " inner join [InvoiceFurnitures] on " +
                "[InvoiceFurnitures].[_IID] = [Invoices].[ID] where [Invoices].[ID] = '" + invId + "';"
                , DBConn).ExecuteReader());
            double due = double.TryParse(dt.Rows[0]["Due"].ToString(), out due) ? due : 0;
            return due > 0 ? due : 0;
        }
        
        public static DataRowCollection GetInvoiceFurnitures(string fid)
        {
            DataTable dt = new DataTable();
            dt.Load(new OleDbCommand(
                "select [Furnitures].[ID]," +
                "[Furnitures].[_Name] as [Name]," +
                "[InvoiceFurnitures].[_Quantity] as [Quantity]," +
                "[InvoiceFurnitures].[_SoldPrice] as [Amount]," +
                "[Amount] / [Quantity] as [Price]" +
                "from [Furnitures] inner join [InvoiceFurnitures] on [Furnitures].[ID] = [InvoiceFurnitures].[_FID]" +
                "where [InvoiceFurnitures].[_IID] = '" + fid + "';", DBConn).ExecuteReader());
            return dt.Rows;
        }

        public static void UpdateQuantity(string sql)
        {
            new OleDbCommand(sql, DBConn).ExecuteNonQuery();
        }
        
        public static DataRowCollection GetDailySalesRecords()
        {
            DataTable dt = new DataTable();
            dt.Load(new OleDbCommand(
                "select [Furnitures].[_Name], SUM([InvoiceFurnitures].[_Quantity]) as [_SoldQuantity] from (" +
                    "[InvoiceFurnitures] inner join [Invoices] on [InvoiceFurnitures].[_IID] = [Invoices].[ID]" +
                ") left join [Furnitures] on [InvoiceFurnitures].[_FID] = [Furnitures].[ID] " +
                "where [Invoices].[_SID] = " + Root.Store.ID + " and format([Invoices].[_Date], 'd/m/yyyy') = Date() "+
                "group by [Furnitures].[_Name];", DBConn).ExecuteReader());
            return dt.Rows;
        }

        public static DataRowCollection GetWeeklySalesAmounts()
        {
            DataTable dt = new DataTable();
            DateTime dt2 = DateTime.Now;
            dt.Load(new OleDbCommand(
                "select format([Invoices].[_Date], 'd/m/yyyy') as [_Day], SUM([InvoiceFurnitures].[_SoldPrice]) as [_Amount] " +
                "from [InvoiceFurnitures] inner join [Invoices] on [InvoiceFurnitures].[_IID] = [Invoices].[ID] " +
                "where [Invoices].[_SID] = " + Root.Store.ID +
                "and format([Invoices].[_Date], 'd/m/yyyy') between #" + 
                    (dt2.Day - 7) + "/" + dt2.Month + "/" + dt2.Year + "# and #" + 
                    dt2.Day + "/" + dt2.Month + "/" + dt2.Year + "# " +
                "group by format([Invoices].[_Date], 'd/m/yyyy');", DBConn).ExecuteReader());
            return dt.Rows;
        }

    }
}
