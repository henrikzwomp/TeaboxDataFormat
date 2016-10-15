using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TeaboxDataFormat.IO
{
    public interface IFileContainer
    {
        IList<string> ReadAllLines();
        void WriteAllLines(IList<string> lines);
    }

    public class FileContainer : IFileContainer
    {
        string _file_path;
        bool _create_if_missing = false;

        public FileContainer(string file_path)
        {
            _file_path = file_path;
        }

        public FileContainer(string file_path, bool create_if_missing)
        {
            _file_path = file_path;
            _create_if_missing = create_if_missing;
        }

        public IList<string> ReadAllLines()
        {
            if (!File.Exists(_file_path))
            {
                if (_create_if_missing)
                    return new List<string>();

               throw new Exception("Can't find file: " + _file_path);
            }
                

            return new List<string>(File.ReadAllLines(_file_path));
        }

        public void WriteAllLines(IList<string> lines)
        {
            File.WriteAllLines(_file_path, lines);
        }
    }
}
