using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaboxDataFormat.IO
{
    public abstract class TeaboxDataFileReader
    {
        public const string CommentIdentifier = "//";
        public const string DataDelimiter = "\t";
        public const string TitleRowIdentifier = "!";

        protected static void ParseLine(string line, bool no_comments, 
            out string comment, out TeaboxDataLineType type, out string[] data)
        {
            comment = "";
            type = TeaboxDataLineType.Other;
            data = new string[0];

            if (!no_comments)
            {
                var comment_index = line.IndexOf(CommentIdentifier);

                if(comment_index > -1)
                {
                    if (comment_index == 0) // Starts with "//"
                    {
                        comment = line.Substring(line.IndexOf(CommentIdentifier) + CommentIdentifier.Length);
                        return;
                    }

                    while(comment_index > 0)
                    {
                        var pre_char = line.Substring(comment_index - 1, 1);
                        if (pre_char == " " || pre_char == "\t")
                        {
                            comment = line.Substring(comment_index + CommentIdentifier.Length);
                            line = line.Substring(0, comment_index);
                            break;
                        }

                        if (line.Length <= comment_index + 2)
                            break;

                        comment_index = line.IndexOf(CommentIdentifier, comment_index+2);
                    }
                }
            }

            line = line.TrimEnd();

            if (line.StartsWith(TitleRowIdentifier))
            {
                type = TeaboxDataLineType.Titles;
                line = line.Substring(TitleRowIdentifier.Length);
            }

            if (line != string.Empty)
                data = new string[1] { line };

            if (line.Contains(DataDelimiter))
                data = line.Split(DataDelimiter.ToCharArray());

            for (int i = 0; i < data.Length; i++)
                data[i] = data[i].Trim();

            if (type != TeaboxDataLineType.Titles && data.Length > 0)
                type = TeaboxDataLineType.Data;
        }
    }
}
