using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace TeaboxDataFormat.IO
{
    public class TeaboxDataFile : TeaboxDataFileBase<TeaboxDataLine>
        , IEnumerable<TeaboxDataLine>
        , IList<TeaboxDataLine>
    {
        IFileContainer _file;

        public TeaboxDataFile(IFileContainer file) 
        {
            _file = file;
            ReadFile(_file);
        }

        /// <summary>
        /// Create a new TeaboxDataFile based on file
        /// </summary>
        /// <param name="file">File to Open.</param>
        /// <param name="titles_template">Titles to add to existing titles in file.</param>
        public TeaboxDataFile(IFileContainer file, IList<string> titles_template)
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

                var titles_line = new TeaboxDataLine();
                TeaboxDataLine.SetData(titles_line, titles_template);
                TeaboxDataLine.SetLineType(titles_line, TeaboxDataLineType.Titles);
                _lines.Insert(0, titles_line);
            }
        }
        
        public static TeaboxDataFile Open(string file_path)
        {
            return Open(new FileContainer(file_path, true));
        }

        public static TeaboxDataFile Open(IFileContainer file)
        {
            return new TeaboxDataFile(file);
        }

        /// <summary>
        /// When file is saved, title row will be based on properties marked with the TeaboxDataAttribute object.
        /// </summary>
        /// <typeparam name="based_on"></typeparam>
        /// <param name="file_path"></param>
        /// <returns></returns>
        public static TeaboxDataFile Open<based_on>(string file_path) where based_on : TeaboxDataLine, new()
        {
            return Open<based_on>(new FileContainer(file_path, true));
        }

        /// <summary>
        /// When file is saved, title row will be based on properties marked with the TeaboxDataAttribute object.
        /// </summary>
        /// <typeparam name="based_on"></typeparam>
        /// <param name="file_path"></param>
        /// <returns></returns>
        public static TeaboxDataFile Open<based_on>(IFileContainer file_path) where based_on : TeaboxDataLine, new()
        {
            var titles = new List<string>();

            foreach (var prop in typeof(based_on).GetProperties())
            {
                var atts = prop.GetCustomAttributes(typeof(TeaboxDataAttribute), true);

                if (atts.Length == 1 && atts[0].GetType() == typeof(TeaboxDataAttribute))
                {
                    titles.Add(prop.Name);
                }
            }

            return new TeaboxDataFile(file_path, titles);
        }
        
        [Obsolete("Moved to TeaboxDataTable.ToTeaboxDataFile(...)")]
        public static TeaboxDataFile DataTableToFile(TeaboxDataTable data_table, string file_path)
        {
            return data_table.ToTeaboxDataFile(file_path);
        }

        [Obsolete("Moved to TeaboxDataTable.ToTeaboxDataFile(...)")]
        public static TeaboxDataFile DataTableToFile(TeaboxDataTable data_table, IFileContainer file)
        {
            return data_table.ToTeaboxDataFile(file);
        }

        /// <summary>
        /// Get all lines with data as TeaboxDataTable object
        /// Data is referenced to back to TeaboxDataFile object.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get all lines with data in list with object of choosen type
        /// </summary>
        /// <typeparam name="output_type">Object that is child of TeaboxDataLine class</typeparam>
        /// <returns></returns>
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
                    
                    foreach (var prop in typeof(output_type).GetProperties())
                    {
                        var atts = prop.GetCustomAttributes(typeof(TeaboxDataAttribute), true);

                        if (atts.Length == 1 && atts[0].GetType() == typeof(TeaboxDataAttribute))
                        {
                            if (prop.PropertyType == typeof(string))
                                prop.SetValue(new_item, TeaboxDataLine.GetData(line, prop.Name));
                            else if (prop.PropertyType == typeof(int))
                            {
                                int v = 0;
                                int.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                                prop.SetValue(new_item, v);
                            }
                            else if (prop.PropertyType == typeof(DateTime))
                            {
                                DateTime v = DateTime.MinValue;
                                DateTime.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                                prop.SetValue(new_item, v);
                            }
                            else if (prop.PropertyType == typeof(bool))
                            {
                                bool v = false;
                                bool.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                                prop.SetValue(new_item, v);
                            }
                            else if (prop.PropertyType == typeof(double))
                            {
                                double v = 0;
                                double.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                                prop.SetValue(new_item, v);
                            }
                            else
                                throw new Exception("Property type not supported.");
                        }
                    }
                    result.Add(new_item);
                }

            }

            return result;
        }

        [Obsolete("Renamed to UpdateAndMergeData")]
        public void MergeData<input_type>(IEnumerable<input_type> new_data, string key) where input_type : TeaboxDataLine
        {
            UpdateAndMergeData<input_type>(new_data, key);
        }

        // ToDo: Test what happens when key is duplicate
        /// <summary>
        /// ToDo
        /// </summary>
        /// <typeparam name="input_type"></typeparam>
        /// <param name="new_data"></param>
        /// <param name="key"></param>
        public void UpdateAndMergeData<input_type>(IEnumerable<input_type> new_data, string key) where input_type : TeaboxDataLine
        {
            foreach(var new_data_item in new_data)
            {
                // Set data from properties 
                // ToDo test that data set to properties is used when merging (copied to data collection first)
                foreach (var prop in new_data_item.GetType().GetProperties())
                {
                    var atts = prop.GetCustomAttributes(typeof(TeaboxDataAttribute), true);

                    if (atts.Length == 1 && atts[0].GetType() == typeof(TeaboxDataAttribute))
                    {
                        TeaboxDataLine.SetData(new_data_item, prop.Name, prop.GetValue(new_data_item).ToString());
                    }
                }

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

        public TeaboxDataLine this[int index]
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
        
        public IEnumerator<TeaboxDataLine> GetEnumerator()// IEnumerator
        {
            return _lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() // IEnumerator
        {
            return _lines.GetEnumerator();
        }

        public int IndexOf(TeaboxDataLine item) // IList
        {
            return _lines.IndexOf(item);
        }

        public void Insert(int index, TeaboxDataLine item) // IList
        {
            TeaboxDataLine.SetTitles(item, _titles);
            _lines.Insert(index, item);
        }

        public void RemoveAt(int index) // IList
        {
            _lines.RemoveAt(index);
        }

        public void Add(TeaboxDataLine item) // ICollection
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

        public bool Contains(TeaboxDataLine item)
        {
            return _lines.Contains(item);
        }

        public void CopyTo(TeaboxDataLine[] array, int arrayIndex) // ICollection
        {
            _lines.CopyTo(array, arrayIndex);
        }

        public bool Remove(TeaboxDataLine item) // ICollection
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
