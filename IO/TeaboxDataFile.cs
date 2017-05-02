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

        protected TeaboxDataFile(IFileContainer file, IList<string> titles_template)
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
                    throw new Exception("Can't set titles on data with new current titles.");
                }

                var titles_line = new TeaboxDataLine();
                TeaboxDataLine.SetData(titles_line, titles_template);
                TeaboxDataLine.SetLineType(titles_line, TeaboxDataLineType.Titles);
                _lines.Insert(0, titles_line);
            }
        }

        public static TeaboxDataFile Open(string file_path)
        {
            return Open(new FileContainer(file_path));
        }

        public static TeaboxDataFile Open(IFileContainer file)
        {
            return new TeaboxDataFile(file);
        }

        // ToDo Add description to why this is useful
        public static TeaboxDataFile Open<based_on>(string file_path) where based_on : TeaboxDataLine, new()
        {
            return Open<based_on>(new FileContainer(file_path, true));
        }

        // ToDo Add description to why this is useful
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

        public static TeaboxDataFile DataTableToFile(TeaboxDataTable data_table, string file_path)
        {
            return DataTableToFile(data_table, new FileContainer(file_path, true));
        }

        public static TeaboxDataFile DataTableToFile(TeaboxDataTable data_table, IFileContainer file)
        {
            var new_file = Open(file);

            new_file._titles = data_table.Titles;

            if(new_file._titles.Length > 0)
            {
                var title_line = new TeaboxDataLine();
                TeaboxDataLine.SetLineType(title_line, TeaboxDataLineType.Titles);
                TeaboxDataLine.SetData(title_line, new_file._titles);
                new_file._lines.Add(title_line);
            }

            if (data_table.Count > 0)
            {
                foreach (var row in data_table)
                {
                    new_file._lines.Add(row);
                }
            }
            
            return new_file;
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

        // ToDo: Test what happens when key is duplicate
        // ToDo: Rename? UpdateData? UpdateAndMergeData?
        // ToDo: Replace this with a better solution
        /// <summary>
        /// ToDo
        /// </summary>
        /// <typeparam name="input_type"></typeparam>
        /// <param name="new_data"></param>
        /// <param name="key"></param>
        public void MergeData<input_type>(IEnumerable<input_type> new_data, string key) where input_type : TeaboxDataLine
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
                else

                // Copy data or add new row
                if(merge_target != null)
                {
                    TeaboxDataLine.SetData(merge_target, TeaboxDataLine.GetData(new_data_item));
                    TeaboxDataLine.SetTitles(merge_target, TeaboxDataLine.GetTitles(new_data_item));
                }
                else
                {
                    _lines.Add(new_data_item);
                    merge_target = new_data_item;
                }
            }
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
