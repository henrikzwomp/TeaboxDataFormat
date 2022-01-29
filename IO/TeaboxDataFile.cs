using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using TeaboxDataFormat.IO;

namespace TeaboxDataFormat.IO
{
    public class TeaboxDataFile : TeaboxDataFile<TeaboxDataLine>
    {
        private TeaboxDataFile(IFileContainer file) : base(file)
        {

        }
    }

    public class TeaboxDataFile<line_type> : TeaboxDataFileBase<line_type>
        , IEnumerable<line_type>
        , IList<line_type> where line_type : TeaboxDataLine, new()
    {
        IFileContainer _file;

        protected TeaboxDataFile(IFileContainer file) 
        {
            _file = file;
            ReadFile(_file);
        }

        /// <summary>
        /// Create a new TeaboxDataFile based on file
        /// </summary>
        /// <param name="file">File to Open.</param>
        /// <param name="titles_template">Titles to add to existing titles in file.</param>
        private TeaboxDataFile(IFileContainer file, IList<string> titles_template)
        {
            _file = file;
            ReadFile(_file);

            if(_lines.Any(x => TeaboxDataLine.GetLineType(x) == TeaboxDataLineType.Titles)) // ToDo Test that titles will be updated
            {
                var new_titles = new List<string>(_titles);

                foreach(var title in titles_template)
                {
                    if(!_titles.Contains(title))
                    {
                        new_titles.Add(title);
                    }
                }

                _titles = new_titles.ToArray();

                var titles_line = _lines.First(x => TeaboxDataLine.GetLineType(x) == TeaboxDataLineType.Titles);
                TeaboxDataLine.SetData(titles_line, _titles);
            }
            else
            {
                if(_lines.Any(x => TeaboxDataLine.GetLineType(x) == TeaboxDataLineType.Data)) // ToDo Test
                {
                    throw new Exception("Can't set titles on data with untitled data.");
                }

                var titles_line = new line_type();
                TeaboxDataLine.SetData(titles_line, titles_template);
                TeaboxDataLine.SetLineType(titles_line, TeaboxDataLineType.Titles);
                _lines.Insert(0, titles_line);
            }
        }
        
        public static TeaboxDataFile<TeaboxDataLine> Open(string file_path)
        {
            return Open(new FileContainer(file_path, true));
        }

        public static TeaboxDataFile<TeaboxDataLine> Open(IFileContainer file)
        {
            return new TeaboxDataFile<TeaboxDataLine>(file);
        }

        /// <summary>
        /// When file is saved, title row will be based on properties marked with the TeaboxDataAttribute object.
        /// </summary>
        /// <typeparam name="based_on"></typeparam>
        /// <param name="file_path"></param>
        /// <returns></returns>
        public static TeaboxDataFile<based_on> Open<based_on>(string file_path) where based_on : TeaboxDataLine, new()
        {
            return Open<based_on>(new FileContainer(file_path, true));
        }

        /// <summary>
        /// When file is saved, title row will be based on properties marked with the TeaboxDataAttribute object.
        /// </summary>
        /// <typeparam name="based_on"></typeparam>
        /// <param name="file_path"></param>
        /// <returns></returns>
        public static TeaboxDataFile<based_on> Open<based_on>(IFileContainer file_path) where based_on : TeaboxDataLine, new()
        {
            var titles = new List<string>();

            foreach (var prop in typeof(based_on).GetProperties())
            {
                //var atts = prop.GetCustomAttributes(typeof(TeaboxDataAttribute), true);

                //if (atts.Length == 1 && atts[0].GetType() == typeof(TeaboxDataAttribute))
                //{
                    titles.Add(prop.Name);
                //}
            }
            return new TeaboxDataFile<based_on>(file_path, titles);
        }

        /// <summary>
        /// Get all lines with data as TeaboxDataTable object
        /// Data is referenced to back to TeaboxDataFile object.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Renamed to GetDataTable")]
        public TeaboxDataTable GetData()
        {
            return GetDataTable();
        }

        /// <summary>
        /// Get all lines with data as TeaboxDataTable object
        /// Data is referenced to back to TeaboxDataFile object.
        /// </summary>
        /// <returns></returns>
        public TeaboxDataTable GetDataTable()
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

        /// <summary>
        /// Get all lines with data in list with object of choosen type (data not referenced to file class)
        /// </summary>
        /// <typeparam name="output_type">Object that is child of TeaboxDataLine class</typeparam>
        /// <returns></returns>
        [Obsolete("This will probably be removed in the future.")]
        public IList<output_type> GetDataAs<output_type>() where output_type : TeaboxDataLine, new()
        {
            var result = new List<output_type>();

            foreach (var line in _lines)
            {
                if (TeaboxDataLine.GetLineType(line) == TeaboxDataLineType.Data)
                {
                    var new_item = new output_type();
                    TeaboxDataLine.SetData(new_item, TeaboxDataLine.GetData(line));
                    TeaboxDataLine.SetTitles(new_item, TeaboxDataLine.GetTitles(line));
                    TeaboxDataLine.SetPropertiesFromData<output_type>(new_item);
                    result.Add(new_item);
                }
            }

            return result;
        }

        // <summary>
        /// Get all lines with data in list with object of choosen type
        /// </summary>
        /// <typeparam name="output_type">Object that is child of TeaboxDataLine class</typeparam>
        /// <returns></returns>
        public IList<line_type> GetDataLines()
        {
            var result = new List<line_type>();

            foreach (var line in _lines)
            {
                if (TeaboxDataLine.GetLineType(line) == TeaboxDataLineType.Data)
                {
                    result.Add(line);
                }
            }

            return result;
        }

        // ToDo: Test what happens when key is duplicate
        /// <summary>
        /// ToDo
        /// </summary>
        /// <param name="new_data"></param>
        /// <param name="key"></param>
        public void UpdateAndMergeData(IEnumerable<line_type> new_data, string key)
        {
            foreach(var new_data_item in new_data)
            {

                // Set data from properties 
                // ToDo test that data set to properties is used when merging (copied to data collection first)
                TeaboxDataLine.SetDataFromProperties<line_type>(new_data_item);

                // Look for same row in target
                TeaboxDataLine merge_target = null;

                if(_lines.Any(x =>
                    TeaboxDataLine.GetLineType(x) == TeaboxDataLineType.Data &&
                    TeaboxDataLine.GetData(x, key) == TeaboxDataLine.GetData(new_data_item, key)))
                {
                    merge_target = _lines
                        .First(x =>
                        TeaboxDataLine.GetLineType(x) == TeaboxDataLineType.Data &&
                        TeaboxDataLine.GetData(x, key) == TeaboxDataLine.GetData(new_data_item, key));
                }

                // Copy data or add new row
                if(merge_target != null)
                {
                    TeaboxDataLine.SetData(merge_target, TeaboxDataLine.GetData(new_data_item));
                    TeaboxDataLine.SetTitles(merge_target, TeaboxDataLine.GetTitles(new_data_item));
                    TeaboxDataLine.SetPropertiesFromData<line_type>(merge_target);
                }
                else
                {
                    _lines.Add(new_data_item);
                }
            }
        }

        public void Save()
        {
            WriteFile(_file);
        }

        #region IEnumerator, IList and ICollection methods

        public line_type this[int index]
        {
            get
            {
                return _lines[index];
            }
            set
            {
                _lines[index] = value;
            }
        }
        
        public IEnumerator<line_type> GetEnumerator()// IEnumerator
        {
            return _lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() // IEnumerator
        {
            return _lines.GetEnumerator();
        }

        public int IndexOf(line_type item) // IList
        {
            return _lines.IndexOf(item);
        }

        public void Insert(int index, line_type item) // IList
        {
            TeaboxDataLine.SetTitles(item, _titles);
            _lines.Insert(index, item);
        }

        public void RemoveAt(int index) // IList
        {
            _lines.RemoveAt(index);
        }

        public void Add(line_type item) // ICollection
        {
            TeaboxDataLine.SetTitles(item, _titles);
            _lines.Add(item);
        }

        /// <summary>
        /// Warning: Will clear all lines including set titles.
        /// </summary>
        public void Clear() // ICollection
        {
            _titles = new string[0];
            _lines.Clear();
        }

        public bool Contains(line_type item)
        {
            return _lines.Contains(item);
        }

        public void CopyTo(line_type[] array, int arrayIndex) // ICollection
        {
            _lines.CopyTo(array, arrayIndex);
        }

        public bool Remove(line_type item) // ICollection
        {
            return _lines.Remove(item);
        }
        
        public int Count // ICollection
        {
            get
            {
                return _lines.Count;
            }
        }

        public bool IsReadOnly // ICollection
        {
            get
            {
                return _lines.IsReadOnly;
            }
        }

        #endregion
    }
}
