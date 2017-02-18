using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaboxDataFormat.IO
{
    public abstract class TeaboxDataFileBase<item_type> : TeaboxDataFileReader where item_type : TeaboxDataLine, new()
    {
        protected IList<item_type> _lines; // ToDo: Change to IEnumerable ?
        protected string[] _titles;

        protected void ReadFile(IFileContainer file)
        {
            var raw_lines = file.ReadAllLines();

            _lines = new List<item_type>();

            if (_titles == null)
                _titles = new string[0]; // If set in constructor. Don't overwrite it here.

            foreach (var raw_line in raw_lines)
            {
                string comment;
                TeaboxDataLineType type;
                string[] data;

                ParseLine(raw_line, out comment, out type, out data);

                if (type == TeaboxDataLineType.Titles && _titles.Length == 0)
                {
                    _titles = new string[data.Length];
                    Array.Copy(data, _titles, data.Length);
                }

                var new_item = new item_type();
                TeaboxDataLine.SetData(new_item, data);
                TeaboxDataLine.SetTitles(new_item, _titles);
                TeaboxDataLine.SetLineType(new_item, ref type);
                TeaboxDataLine.SetComment(new_item, ref comment);
                _lines.Add(new_item);
            }
        }

        protected void WriteFile(IFileContainer file)
        {
            //if (_titles == null)
            //    _titles = new string[0]; // If set in constructor. Don't overwrite it here.

            var raw_lines = new List<string>();

            foreach (var line in _lines)
            {
                string comment = TeaboxDataLine.GetComment(line);
                var type = TeaboxDataLine.GetLineType(line);

                string raw_line = "";

                if (type == TeaboxDataLineType.Titles)
                    raw_line += (TitleRowIdentifier +
                        string.Join(DataDelimiter, TeaboxDataLine.GetData(line)));
                else if (type == TeaboxDataLineType.Data)
                {
                    string line_data = "";

                    if(_titles == null || _titles.Length == 0)
                    {
                        var parts = TeaboxDataLine.GetData(line);

                        for (int i = 0; i < parts.Count; i++)
                        {
                            if (i > 0)
                                line_data += DataDelimiter;
                            line_data += parts[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _titles.Length; i++)
                        {
                            if (i > 0)
                                line_data += DataDelimiter;
                            line_data += TeaboxDataLine.GetData(line, _titles[i]);
                        }
                    }
                    

                    raw_line += line_data;
                }

                if (!string.IsNullOrEmpty(comment))
                {
                    if (raw_line != "")
                        raw_line += " ";

                    raw_line += (CommentIdentifier + comment);
                }

                raw_lines.Add(raw_line);
            }

            file.WriteAllLines(raw_lines);
        }

    }
}
