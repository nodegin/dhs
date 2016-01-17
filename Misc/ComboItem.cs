using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHS.Misc
{
    class ComboItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public ComboItem(string text, object value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }

    }
}
