using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaboxDataFormat.IO
{
    public class TeaboxDataFile : TeaboxDataFileBase<TeaboxDataLine>, IEnumerable<TeaboxDataLine>
    {
        IFileContainer _file;

        protected TeaboxDataFile(IFileContainer file) 
        {
            _file = file;
            ReadFile(_file);
        }
        
        public static TeaboxDataFile Open(string file_path)
        {
            return Open(new FileContainer(file_path));
        }

        public static TeaboxDataFile Open(IFileContainer file)
        {
            return new TeaboxDataFile(file);
        }
        //------
        public static TeaboxDataFile DataTableToFile(TeaboxDataTable data_table, string file_path)
        {
            return DataTableToFile(data_table, new FileContainer(file_path, true));
        }

        public static TeaboxDataFile DataTableToFile(TeaboxDataTable data_table, IFileContainer file)
        {
            var new_file = Open(file);

            if(data_table.Count > 0)
            {
                new_file._titles = TeaboxDataLine.GetTitles(data_table.First()).ToArray();

                foreach (var row in data_table)
                {
                    new_file._lines.Add(row);
                }
            }
            
            return new_file;
        }
        //------
        public TeaboxDataTable GetData()
        {
            var result = new TeaboxDataTable(_titles);

            foreach (var line in _lines)
            {
                if (TeaboxDataLine.GetLineType(line) == TeaboxDataLineType.Data)
                {
                    var datarow = new TeaboxDataRow();
                    TeaboxDataLine.SetData(datarow, TeaboxDataLine.GetData(line));
                    TeaboxDataLine.SetTitles(datarow, TeaboxDataLine.GetTitles(line));
                    result.Add(datarow);
                }
                    
            }

            return result;
        }

        public TeaboxDataLine this[int index]
        {
            get
            {
                return _lines[index];
            }
        }

        public IEnumerator<TeaboxDataLine> GetEnumerator()
        {
            return _lines.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _lines.GetEnumerator();
        }

        public void Save()
        {
            WriteFile(_file);
        }
    }
}
