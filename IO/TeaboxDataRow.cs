using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaboxDataFormat.IO
{
    public class TeaboxDataRow : TeaboxDataLine
    {
        public TeaboxDataRow() : base() { }

        new public string this[int index]
        {
            get { return base[index]; }
            set { base[index] = value; }
        }

        new public string this[string column_name]
        {
            get { return base[column_name]; }
            set { base[column_name] = value; }
        }

        new public string this[int index, string default_value]
        {
            get { return base[index, default_value]; }
        }

        new public string this[string column_name, string default_value]
        {
            get { return base[column_name, default_value]; }
        }
    }
}
