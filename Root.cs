using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using Microsoft.Win32;

using DHS.Misc;
using DHS.I18n;

namespace DHS
{
    class Root : Form
    {
        // store info
        public static StoreInfo Store = null;
        // current logged in user
        public static Userdata CurrentStaff = null;
        // shopping cart items
        public Dictionary<string, int> ShoppingCart = new Dictionary<string, int>();
        public Dictionary<string, int> OrderCart = new Dictionary<string, int>();
        // ie registry for printing
        private Dictionary<string, object> IERegistries = new Dictionary<string, object>();
        public RegistryKey IEPageSetup;
        // language
        private static Dictionary<string, string> CurrentLang = CHT.Msg;

        public Root()
        {
            this.Opacity = 0;
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.Location = new Point(this.Width * 2, this.Height * 2);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            /*  IMPORTANT LINES, DO NOT MODIFY  */
            #region Registry Modification
            IEPageSetup = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\PageSetup", true);
            IERegistries.Add("Print_Background", IEPageSetup.GetValue("Print_Background"));
            IERegistries.Add("footer", IEPageSetup.GetValue("footer"));
            IERegistries.Add("header", IEPageSetup.GetValue("header"));
            IERegistries.Add("margin_top", IEPageSetup.GetValue("margin_top"));
            IERegistries.Add("margin_right", IEPageSetup.GetValue("margin_right"));
            IERegistries.Add("margin_bottom", IEPageSetup.GetValue("margin_bottom"));
            IERegistries.Add("margin_left", IEPageSetup.GetValue("margin_left"));

            IEPageSetup.SetValue("Print_Background", "yes");
            IEPageSetup.SetValue("footer", "");
            IEPageSetup.SetValue("header", "");
            IEPageSetup.SetValue("margin_top", "0");
            IEPageSetup.SetValue("margin_right", "0");
            IEPageSetup.SetValue("margin_bottom", "0");
            IEPageSetup.SetValue("margin_left", "0");
            Application.ApplicationExit += (sender, e) =>
            {
                foreach (KeyValuePair<string, object> item in IERegistries)
                {
                    IEPageSetup.SetValue(item.Key, item.Value == null ? "" : item.Value);
                }
                IEPageSetup.Close();
            };
            #endregion
            /*  IMPORTANT LINES, DO NOT MODIFY  */
            XQL.InitConnection();
            // get the store info
            Store = XQL.ObtainStoreInformations(1);
            // set default lang
            SetLang(0xa);
            new LoginForm(this).Show();
        }

        public void NotifyLoggedIn(Userdata user)
        {
            // set current user and bring user to main form
            CurrentStaff = user;
            new MainForm(this).Show();
        }

        public void NotifyLoggedOut()
        {
            // show login form again
            CurrentStaff = null;
            ShoppingCart.Clear();
            List<Form> openForms = new List<Form>();
            foreach (Form f in Application.OpenForms) openForms.Add(f);
            foreach (Form f in openForms)
            {
                if (f.GetType() != typeof(Root))
                {
                    ((MetroForm)f).Flag_DoNotKillRoot = true;
                    f.Close();
                }
            }
            new LoginForm(this).Show();
        }

        public static void SetLang(int lang)
        {
            switch (lang)
            {
                case 0xa:
                    CurrentLang = ENG.Msg;
                    break;
                case 0xb:
                    CurrentLang = CHT.Msg;
                    break;
                case 0xc:
                    CurrentLang = CHS.Msg;
                    break;
            }
        }

        public static string GetMsg(string key)
        {
            string msg;
            if (CurrentLang.TryGetValue(key, out msg))
                return msg;
            return "#undefined#";
        }

    }
}
