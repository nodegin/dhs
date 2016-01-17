using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Reflection;
using System.Text.RegularExpressions;

using DHS.Misc;
using DHS.UI;

namespace DHS
{
    class LoginForm : MetroForm
    {

        private Root _Root;
        private Label LabelLogin, LabelPassword;
        private TextBox InputLogin, InputPassword;
        private MetroButton ButtonLogin;
        private MetroComboBox DropLang;

        public LoginForm(Root p) : base(p, Root.GetMsg("login.title"), 540, 320)
        {
            _Root = p;
            // label - staff id
            LabelLogin = new Label();
            LabelLogin.AutoSize = true;
            LabelLogin.Font = new Font("Segoe UI Light", 14);
            LabelLogin.AutoSize = true;
            int lblX = base.Title.Left * 3;
            int lblHeight = LabelLogin.Height;
            int panelAdjust = (base.Size.Height - base.ContentPanel.Height) / 2;
            int lblInFormMiddle = (base.ContentPanel.Height / 2 - lblHeight / 2) - panelAdjust;
            int yOffsetLogin = lblInFormMiddle - lblHeight - 8;
            LabelLogin.Location = new Point(lblX, yOffsetLogin);
            LabelLogin.Text = Root.GetMsg("login.staffid");
            this.ContentPanel.Controls.Add(LabelLogin);
            // label - password
            LabelPassword = new Label();
            LabelPassword.AutoSize = true;
            LabelPassword.Font = new Font("Segoe UI Light", 14);
            LabelPassword.AutoSize = true;
            int yOffsetPassword = lblInFormMiddle + lblHeight + 8;
            LabelPassword.Location = new Point(lblX, yOffsetPassword);
            LabelPassword.Text = Root.GetMsg("login.passwd");
            this.ContentPanel.Controls.Add(LabelPassword);
            // textbox - staff id
            InputLogin = new MetroTextBox(256, 32);
            InputLogin.BackColor = Color.FromArgb(77, 77, 77);
            InputLogin.BorderStyle = BorderStyle.None;
            InputLogin.Font = new Font("Segoe UI Light", 16);
            InputLogin.ForeColor = Color.White;
            int txtX = base.Size.Width - lblX - InputLogin.Width;
            InputLogin.Location = new Point(txtX, yOffsetLogin - 8);
            InputLogin.TabStop = false;
            InputLogin.KeyDown += DetectHotKey;
            InputLogin.KeyPress += ValidateCharacters;
            this.ContentPanel.Controls.Add(InputLogin);
            // textbox - password
            InputPassword = new MetroTextBox(256, 32);
            InputPassword.BackColor = Color.FromArgb(77, 77, 77);
            InputPassword.BorderStyle = BorderStyle.None;
            InputPassword.Font = new Font("Segoe UI Light", 16);
            InputPassword.ForeColor = Color.White;
            InputPassword.Location = new Point(txtX, yOffsetPassword - 8);
            InputPassword.TabStop = false;
            InputPassword.KeyDown += DetectHotKey;
            InputPassword.KeyPress += ValidateCharacters;
            InputPassword.PasswordChar = '*';
            this.ContentPanel.Controls.Add(InputPassword);
            InputLogin.Text = "1";
            InputPassword.Text = "1234";
            // button login
            ButtonLogin = new MetroButton(fontSize: 12);
            ButtonLogin.Text = Root.GetMsg("login.title");
            ButtonLogin.AutoSize = true;
            ButtonLogin.Click += HandleLogin;
            ButtonLogin.TabStop = false;
            this.ContentPanel.Controls.Add(ButtonLogin);
            int btnX = base.Size.Width - lblX - ButtonLogin.Width;
            int btnY = base.Size.Height - base.ContentPanel.Top - base.Title.Top - ButtonLogin.Height + 1/* border size */;
            int minus = base.Size.Height - (base.ContentPanel.Top + yOffsetPassword + InputPassword.Height);
            btnY = (btnY - minus / 2) + ButtonLogin.Height - 1 * 2;
            ButtonLogin.Location = new Point(btnX, btnY);
            // lang chooser
            DropLang = new MetroComboBox();
            DropLang.Size = new Size(128, ButtonLogin.Height);
            DropLang.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.ContentPanel.Controls.Add(DropLang);
            DropLang.Location = new Point(InputPassword.Left, btnY + 1 /* border */);
            // add langs
            DropLang.SelectedIndexChanged += (sender, e) =>
            {
                int lang = (int)((sender as ComboBox).SelectedItem as ComboItem).Value;
                Root.SetLang(lang);
                UpdateFormLang();
            };
            DropLang.Items.Add(new ComboItem("English", 0xa));
            DropLang.Items.Add(new ComboItem("繁體中文", 0xb));
            DropLang.Items.Add(new ComboItem("简体中文", 0xc));
            DropLang.SelectedIndex = 0;
            // add clock
            Label currTime = new Label();
            currTime.AutoSize = true;
            currTime.Font = new Font("Segoe UI Light", 11);
            currTime.ForeColor = Color.FromArgb(204, 204, 204);
            currTime.Location = new Point(lblX, btnY + ButtonLogin.Height / 4);
            this.ContentPanel.Controls.Add(currTime);
            Timer tm = new Timer();
            tm.Tick += (sender, e) => {
                currTime.Text = DateTime.Now.ToString("yyyy/M/d H:mm:ss");
            };
            tm.Start();
        }

        private void DetectHotKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) ButtonLogin.PerformClick();
        }

        private void ValidateCharacters(object sender, KeyPressEventArgs e)
        {
            string pattern = @"[\x00-\x7F]";
            string input = ((TextBox)sender).Text;
            if (!Regex.IsMatch(e.KeyChar.ToString(), pattern))
            {
                e.Handled = true;
            }
        }

        private void HandleLogin(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(InputLogin.Text) ||
                String.IsNullOrWhiteSpace(InputPassword.Text))
            {
                MessageBox.Show("Please enter both field of Staff ID and Password", "Warning");
                return;
            }
            try
            {
                int id = int.TryParse(InputLogin.Text, out id) ? id : 0;
                Userdata user = XQL.LoginUser(id, InputPassword.Text);
                if (null == user)
                {
                    MessageBox.Show("Incorrect Staff ID or Password", "Error");
                    return;
                }
                _Root.NotifyLoggedIn(user);
                this.Flag_DoNotKillRoot = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect database\n"+ex.Message, "Error");
            }
        }

        private void UpdateFormLang(){
            _This.Title.Text = Root.GetMsg("login.title");
            LabelLogin.Text = Root.GetMsg("login.staffid");
            LabelPassword.Text = Root.GetMsg("login.passwd");
            ButtonLogin.Text = Root.GetMsg("login.title");
        }

    }
}
