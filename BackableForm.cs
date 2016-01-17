using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Transitions;

using DHS.Misc;

namespace DHS
{
    class BackableForm : IParentForm
    {
        protected IParentForm _Parent;
        private Root _RefRoot;
        public Root _Root { get { return _RefRoot; } }
        private MetroForm _RefThis;
        public MetroForm _This { get { return _RefThis; } } 
        protected bool OverrideBackReloadDHS = false;

        public BackableForm(Root root, IParentForm parent, string name)
        {
            _Parent = parent;
            _RefRoot = root;
            _RefThis = new MetroForm(_Root, name, 1136, 720, OnBack);
            _This.Tag = this;
            if (_Parent.GetType().BaseType == typeof(MetroForm))
            {
                _Parent.AnimateHideForm((sn, ev) =>
                {
                    ((MetroForm)_Parent).Hide(((MetroForm)_Parent));
                });
            }
            else
            {
                IParentForm parentAtTaskBar = _Parent;
                while (((BackableForm)parentAtTaskBar)._Parent.GetType() != typeof(MainForm))
                {
                    parentAtTaskBar = ((BackableForm)parentAtTaskBar)._Parent;
                }
                _Parent.AnimateHideForm((sn, ev) =>
                {
                    ((BackableForm) parentAtTaskBar)._This.Hide(((BackableForm) parentAtTaskBar)._This);
                });
            }
        }
        
        public void Show()
        {
            // hide parent and show this
            _This.AnimateShowForm();
        }

        // for parent == BackableForm
        public void AnimateHideForm(EventHandler<Transition.Args> handler = null)
        {
            _This.AnimateHideForm((sn, ev) =>
            {
                _This.Hide(_This);
            });
        }

        // for parent == BackableForm
        public void AnimateShowForm(EventHandler<Transition.Args> handler = null)
        {
            _This.AnimateShowForm();
        }

        // private method handle back to parent
        protected void OnBack(object sender = null, EventArgs args = null)
        {
            if (OverrideBackReloadDHS) BackToManage(reloadProducts: true);
            else
            {
                _This.Flag_DoNotKillRoot = true;
                _This.AnimateHideForm((sn, ev) =>
                {
                    _This.Close(_This);
                });
                _Parent.AnimateShowForm();
            }
        }

        // back to Manage form
        protected void BackToManage(bool reloadProducts = false, string whereClause = null, string orderBy = null)
        {
            StoreFurnituresForm manage = null;
            // close all parent backable levels
            List<Form> openForms = new List<Form>();
            foreach (Form f in Application.OpenForms) openForms.Add(f);
            foreach (Form f in openForms)
            {
                Type ft = f.GetType();
                if (f.Tag is StoreFurnituresForm)
                {
                    manage = (StoreFurnituresForm)f.Tag;
                }
                else if (ft != typeof(MainForm) && ft != typeof(Root))
                {
                    ((MetroForm)f).Flag_DoNotKillRoot = true;
                    f.Close();
                }
            }
            // open DHSForm
            if (reloadProducts) manage.ReloadItems(whereClause, orderBy);
            ((MetroForm)manage._This).AnimateShowForm();
        }

    }
}
