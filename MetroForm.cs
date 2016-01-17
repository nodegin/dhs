using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transitions;

using DHS.UI;

namespace DHS
{
    class MetroForm : Form, IParentForm
    {

        ////////////////////////////////////////////////////////
        //                                                    //
        //  _|_|_|    _|_|_|_|  _|_|_|_|  _|_|_|_|  _|    _|  //
        //  _|    _|    _|_|    _|    _|    _|_|    _|    _|  //
        //  _|    _|    _|_|    _|_|_|      _|_|      _|_|    //
        //  _|    _|    _|_|    _|  _|      _|_|      _|_|    //
        //  _|_|_|    _|_|_|_|  _|    _|    _|_|      _|_|    //
        //                                                    //
        ////////////////////////////////////////////////////////

        private Root _RRoot;
        public Root _Root { get { return _RRoot; } }
        private MetroForm _RThis;
        public MetroForm _This { get { return _RThis; } } 
        public bool Flag_DoNotKillRoot = false;
        private bool Flag_IsSecondary = false;
        // elements
        public Label Title;
        public Panel ContentPanel = new Panel();
        private int ActionButtonsOffsetNegX = 0;

        public MetroForm(Root p, string name, int width, int height, EventHandler onBack = null)
        {
            _RRoot = p;
            _RThis = this;
            Flag_IsSecondary = null != onBack;
            this.SuspendLayout();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Icon = Properties.Resources.logo;
            this.Size = new Size(width, height);
            this.BackColor = Color.FromArgb(34, 34, 34);
            this.ForeColor = Color.White;
            this.CenterToScreen();
            this.Activated += OnFormResumed;
            this.FormClosed += OnFormClosed;
            this.DoubleBuffered = true;
            ContentPanel.Visible = false;
            // title
            Title = new Label();
            Title.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Title.Font = new Font("Segoe UI Light", 20);
            Title.Text = name;
            this.Text = name;
            Title.AutoSize = true;
            if (Flag_IsSecondary)
            {
                MetroButton back = new MetroButton(border: 0);
                back.Size = new Size(32, 32);
                Title.Location = new Point(back.Width + 16 * 2, 16);
                back.BackgroundImage = Properties.Resources.form_back;
                back.BackgroundImageLayout = ImageLayout.Center;
                DarkToolTip tt = new DarkToolTip();
                tt.SetToolTip(back, Root.GetMsg("ui.back"));
                back.Location = new Point(16, 16 + Title.Top - Title.Height / 2);
                back.Click += onBack;
                this.Controls.Add(back);
            }
            else Title.Location = new Point(16, 16);
            this.Controls.Add(Title);
            // close button
            InsertActionButtons(Root.GetMsg("ui.close"), Properties.Resources.form_close, (sender, e) =>
            {
                AnimateHideForm(CloseFormAction);
            });
            // minimize button
            InsertActionButtons(Root.GetMsg("ui.minimize"), Properties.Resources.form_minimize, (sender, e) =>
            {
                AnimateHideForm(MinimizeFormAction);
            });
            if (Root.CurrentStaff != null)
            {
                InsertCurrentUser(Root.CurrentStaff.Name);
                InsertActionButtons(Root.GetMsg("ui.logout"), Properties.Resources.logout, (sender, e) =>
                {
                    _Root.NotifyLoggedOut();
                });
            }
            // avoid overlap painted border
            ContentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ContentPanel.Left = 1;
            ContentPanel.Top = Title.Height + Title.Top * 2;
            ContentPanel.Size = new Size(width - 2, height - ContentPanel.Top - 1);
            this.Controls.Add(ContentPanel);

            // hide immediately prepare for animation
            // form next will call Activated => FormResumed
            this.Opacity = 0;
            this.CenterToScreen();
            this.Top += 80;
            this.ResumeLayout(false);
        }

        // mat. icon: 1x web, 18dp (sys btn)
        // mat. icon: 1x web, 24dp (act btn)

        public void InsertActionButtons(string desc, Bitmap icon, EventHandler handler)
        {
            MetroButton btn = new MetroButton(border: 0);
            btn.Size = new Size(28, 28);
            btn.BackgroundImage = icon;
            btn.BackgroundImageLayout = ImageLayout.Center;
            btn.Click += handler;
            DarkToolTip tt = new DarkToolTip();
            tt.SetToolTip(btn, desc);
            this.Controls.Add(btn);
            btn.Location = new Point(this.Width - btn.Width - ActionButtonsOffsetNegX - 12, 12);
            ActionButtonsOffsetNegX += btn.Width + 12;
        }

        private void InsertCurrentUser(string name)
        {
            Label lbl = new Label();
            lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbl.MinimumSize = new Size(0, 28);
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI Light", 12);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Text = name;
            DarkToolTip tt = new DarkToolTip();
            lbl.MouseHover += (sender, e) => {
                tt.SetToolTip(lbl, DateTime.Now.ToString("d/M/yyyy H:mm:ss"));
            };
            this.Controls.Add(lbl);
            lbl.Location = new Point(this.Width - lbl.Width - ActionButtonsOffsetNegX - 12, 12);
            ActionButtonsOffsetNegX += lbl.Width + 12;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, this.Width, this.Height), Color.FromArgb(64, 128, 128, 128), ButtonBorderStyle.Solid);
        }

        protected void OnFormResumed(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            AnimateShowForm();
            ContentPanel.Visible = true;
        }

        private void OnFormClosed(object sender, EventArgs args)
        {
            if (!Flag_DoNotKillRoot) _Root.Close();
        }

        // make calls thread safe from backable form
        delegate void ThreadCallback();
        public void Hide(Form form)
        {
            if (form.InvokeRequired) form.Invoke(new ThreadCallback(Hide), null);
            else form.Hide();
        }
        public void Show(Form form)
        {
            if (form.InvokeRequired) form.Invoke(new ThreadCallback(Show), null);
            else form.Show();
        }
        public void Close(Form form)
        {
            if (form.InvokeRequired) form.Invoke(new ThreadCallback(Close), null);
            else form.Close();
        }

        public void AnimateHideForm(EventHandler<Transition.Args> handler = null)
        {
            Transition t = new Transition(new TransitionType_EaseInEaseOut(100));
            t.add(this, "Top", this.Location.Y + 48);
            t.add(this, "Opacity", 0.0);
            if (handler != null) t.TransitionCompletedEvent += handler;
            t.run();
        }

        public virtual void AnimateShowForm(EventHandler<Transition.Args> handler = null)
        {
            this.Show();
            Transition t = new Transition(new TransitionType_EaseInEaseOut(100));
            int tempY = this.Location.Y;
            this.CenterToScreen();
            int centerY = this.Location.Y;
            this.Top = tempY;
            t.add(this, "Top", centerY);
            t.add(this, "Opacity", 1.0);
            if (handler != null) t.TransitionCompletedEvent += handler;
            t.run();
        }

        public void CloseFormAction(object sender, EventArgs args)
        {
            this.Close();
            _Root.Close();
        }

        public void MinimizeFormAction(object sender, EventArgs args)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
