using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DHS.Misc
{
    class Validation
    {
        // for forms
        public static void NumberField(object sender, KeyPressEventArgs e, int length)
        {
            string input = ((TextBox)sender).Text;
            if ((e.KeyChar < '0' || e.KeyChar > '9' || input.Length >= length) && e.KeyChar > 31) e.Handled = true;
        }

        public static void NonNumberField(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') && e.KeyChar > 31) e.Handled = true;
        }

        public static void FloatField(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '.' && e.KeyChar > 31 ||
                e.KeyChar == '.' && ((TextBox)sender).Text.IndexOf(".") > -1)
                e.Handled = true;
        }

        public static void SearchFloatField(object sender, KeyPressEventArgs e)
        {
            string text = ((TextBox)sender).Text;
            string temp = text + e.KeyChar;
            if (!Regex.IsMatch(e.KeyChar.ToString(), "[<=>.0-9]") && e.KeyChar > 31 ||
                Regex.IsMatch(e.KeyChar.ToString(), "[<=>]") && Regex.IsMatch(((TextBox)sender).Text, "[" + e.KeyChar + "]"))
            {
                e.Handled = true;
            }
        }

        public static void IDField(object sender, KeyPressEventArgs e)
        {
            string input = ((TextBox)sender).Text;
            if (input.Length >= 10 && e.KeyChar > 31 || !new Regex(@"[a-zA-Z0-9]").IsMatch(e.KeyChar.ToString()) && e.KeyChar > 31)
                e.Handled = true;
            else if (e.KeyChar >= 'a' && e.KeyChar <= 'z')
                e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        public static void ShelfLocationField(object sender, KeyPressEventArgs e)
        {
            string input = ((TextBox)sender).Text;
            if (input.Length >= 3 && e.KeyChar > 31 ||
                new Regex(@"[a-zA-Z]").IsMatch(input) && new Regex(@"[a-zA-Z]").IsMatch(e.KeyChar.ToString()) && e.KeyChar > 31 || // already have char
                input.Length == 0 && e.KeyChar >= '0' && e.KeyChar <= '9' || // not starts with char
                !new Regex(@"[a-zA-Z0-9]").IsMatch(e.KeyChar.ToString()) && e.KeyChar > 31) // must be a-z and 0-9
            {
                e.Handled = true;
            }
            else if (input.Length == 0 && e.KeyChar >= 'a' && e.KeyChar <= 'z')
                e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        // for sql / plain input
        public static bool ValidateID(string id)
        {
            return id.Length == 10;
        }
        public static bool ValidateName(string name)
        {
            return name.Length < 48;
        }
        public static bool ValidateDescription(string desc)
        {
            return desc.Length < 400;
        }
        public static bool ValidateDimension(string dimen)
        {
            return new Regex(@"([0-9]{1,}([.][0-9]{1,})?)x([0-9]{1,}([.][0-9]{1,})?)x([0-9]{1,}([.][0-9]{1,})?)").IsMatch(dimen);
        }
        public static bool ValidatePrice(double price)
        {
            return price > 0 && price <= 9999999;
        }
        public static bool ValidateDiscount(double discount)
        {
            return discount >= 0 && discount < 100;
        }
        public static bool ValidateQuantity(int qty)
        {
            return qty >= 0 && qty <= 9999;
        }
        public static bool ValidateShelfLocation(string loc)
        {
            return new Regex(@"[A-Z][0-9][0-9]").IsMatch(loc);
        }
        public static bool ValidateCategoryID(int id)
        {
            return id > 0;
        }
        public static bool ValidateSubcategoryID(int id)
        {
            return id >= 0;
        }
        public static bool ValidateCategory(string ctgy)
        {
            return !String.IsNullOrWhiteSpace(ctgy) && ctgy.Length < 20;
        }
        public static bool ValidateSubCategory(string subctgy)
        {
            return !String.IsNullOrWhiteSpace(subctgy) && subctgy.Length < 40;
        }
        public static bool ValidatePhoto(string photo)
        {
            return Uri.IsWellFormedUriString(photo, UriKind.Absolute);
        }
    }
}
