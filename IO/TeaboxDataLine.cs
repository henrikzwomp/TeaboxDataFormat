using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaboxDataFormat.IO
{
    public class TeaboxDataLine
    {
        protected IList<string> _data;
        protected IList<string> _titles;
        protected string _comment;
        protected TeaboxDataLineType _type;
        protected readonly Guid _unique_instance_id;

        public TeaboxDataLine()
        {
            _data = new List<string>();
            _titles = new List<string>();
            _type = TeaboxDataLineType.Data; // Default to data because that is the most common reason for creating a line.
            _comment = "";
            _unique_instance_id = Guid.NewGuid();
        }

        protected string this[int index]
        {
            get
            {
                if (index < _data.Count)
                    return _data[index];

                return "";
            }
            set
            {
                while (index >= _data.Count)
                    _data.Add("");

                _data[index] = value;
            }
        }

        protected string this[string column_name]
        {
            get
            {
                for (int i = 0; i < _titles.Count; i++)
                {
                    if (_titles[i] == column_name)
                        return this[i];
                }

                return "";
            }
            set
            {
                for (int i = 0; i < _titles.Count; i++)
                {
                    if (_titles[i] == column_name)
                    {
                        this[i] = value;
                        return;
                    }
                }

                _titles.Add(column_name);
                this[column_name] = value;
            }
        }

        protected string this[int index, string default_value]
        {
            get
            {
                if (index < _data.Count)
                    return _data[index];

                if (index == _data.Count && !string.IsNullOrEmpty(_comment))
                    return _comment;

                return default_value;
            }
        }

        protected string this[string column_name, string default_value]
        {
            get
            {
                for (int i = 0; i < _titles.Count; i++)
                {
                    if (_titles[i] == column_name)
                        return this[i, default_value];
                }

                return default_value;
            }
        }

        #region Static Help methods

        public static TeaboxDataLineType GetLineType(TeaboxDataLine line)
        {
            return line._type;
        }

        public static IList<string> GetTitles(TeaboxDataLine line)
        {
            return line._titles;
        }

        public static IList<string> GetData(TeaboxDataLine line)
        {
            return line._data;
        }

        public static string GetData(TeaboxDataLine line, int index) { return line[index]; }

        public static string GetData(TeaboxDataLine line, string column_name) { return line[column_name]; }

        public static string GetData(TeaboxDataLine line, int index, string default_value)
        { return line[index, default_value]; ; }

        public static string GetData(TeaboxDataLine line, string column_name, string default_value)
        { return line[column_name, default_value]; ; }

        public static string GetComment(TeaboxDataLine line)
        {
            return line._comment;
        }

        public static void SetTitles(TeaboxDataLine line, IList<string> titles) 
        {
            if (titles.IsReadOnly)
                line._titles = new List<string>(titles);
            else
                line._titles = titles;
        }

        public static void SetData(TeaboxDataLine line, IList<string> data)
        {
            if (data.IsReadOnly)
                line._data = new List<string>(data);
            else
                line._data = data;
        }

        public static void SetData(TeaboxDataLine line, int index, string value) { line[index] = value; }

        public static void SetData(TeaboxDataLine line, string column_name, string value) { line[column_name] = value; }

        public static void SetComment(TeaboxDataLine line, string comment)
        {
            line._comment = comment;
        }

        public static void SetComment(TeaboxDataLine line, ref string comment)
        {
            line._comment = comment;
        }

        public static void SetLineType(TeaboxDataLine line, TeaboxDataLineType type)
        {
            line._type = type;
        }

        public static void SetLineType(TeaboxDataLine line, ref TeaboxDataLineType type)
        {
            line._type = type;
        }

        // ToDo Possible to remove _unique_instance_id ?
        public static Guid GetUniqueInstanceId(TeaboxDataLine line)
        {
            return line._unique_instance_id;
        }

        // ToDo Write tests for this
        public static void SetDataFromProperties<line_type>(TeaboxDataLine line) where line_type : TeaboxDataLine
        {
            foreach (var prop in typeof(line_type).GetProperties())
            {
                var atts = prop.GetCustomAttributes(typeof(TeaboxDataAttribute), true);

                if (atts.Length == 1 && atts[0].GetType() == typeof(TeaboxDataAttribute))
                {
                    var value_object = prop.GetValue(line);

                    var value_string = "";

                    if (value_object != null)
                        value_string = value_object.ToString();

                    TeaboxDataLine.SetData(line, prop.Name, value_string);
                }
            }
        }

        // ToDo Write tests for this
        public static void SetPropertiesFromData<line_type>(TeaboxDataLine line) where line_type : TeaboxDataLine
        {
            foreach (var prop in typeof(line_type).GetProperties())
            {
                var atts = prop.GetCustomAttributes(typeof(TeaboxDataAttribute), true);

                if (atts.Length == 1 && atts[0].GetType() == typeof(TeaboxDataAttribute))
                {
                    if (prop.PropertyType == typeof(string))
                        prop.SetValue(line, TeaboxDataLine.GetData(line, prop.Name));
                    else if (prop.PropertyType == typeof(int))
                    {
                        int v = 0;
                        int.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                        prop.SetValue(line, v);
                    }
                    else if (prop.PropertyType == typeof(DateTime))
                    {
                        DateTime v = DateTime.MinValue;
                        DateTime.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                        prop.SetValue(line, v);
                    }
                    else if (prop.PropertyType == typeof(bool))
                    {
                        bool v = false;
                        bool.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                        prop.SetValue(line, v);
                    }
                    else if (prop.PropertyType == typeof(double))
                    {
                        double v = 0;
                        double.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                        prop.SetValue(line, v);
                    }
                    else if (prop.PropertyType == typeof(decimal)) // ToDo Test
                    {
                        decimal v = 0;
                        decimal.TryParse(TeaboxDataLine.GetData(line, prop.Name), out v);
                        prop.SetValue(line, v);
                    }
                    else
                        throw new Exception("Property type not supported.");
                }
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            if(obj is TeaboxDataLine)
            {
                return (_unique_instance_id == ((TeaboxDataLine)obj)._unique_instance_id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _unique_instance_id.GetHashCode();
        }
    }
}
