using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaboxDataFormat.IO
{
    public class TeaboxDataTable : IEnumerable<TeaboxDataRow>
    {
        private string[] _titles;
        private IList<TeaboxDataRow> _rows;

        public TeaboxDataTable(params string[] titles)
        {
            _titles = titles;
            _rows = new List<TeaboxDataRow>();
        }
        
        public void Add(TeaboxDataRow new_row)
        {
            _rows.Add(new_row);
        }

        public int Count
        {
            get { return _rows.Count; }
        }

        public TeaboxDataRow this[int index]
        {
            get
            {
                return _rows[index];
            }
        }

        public IEnumerator<TeaboxDataRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }
        
        public string[] Titles { get { return _titles; } }

        
        /*public TeaboxDataFile ToTeaboxDataFile(string file_path)
        {
            return ToTeaboxDataFile(new FileContainer(file_path, true));
        }

        public TeaboxDataFile ToTeaboxDataFile(IFileContainer file)
        {
            var new_file = new TeaboxDataFile(file, this.Titles);

            if (this.Count > 0)
            {
                foreach (var row in this)
                {
                    new_file.Add(row);
                }
            }
            
            return new_file;
        }*/
    }
}
