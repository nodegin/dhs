using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transitions;

namespace DHS
{
    interface IParentForm
    {
        Root _Root { get; }
        MetroForm _This { get; }
        void AnimateHideForm(EventHandler<Transition.Args> handler = null);
        void AnimateShowForm(EventHandler<Transition.Args> handler = null);
    }
}
