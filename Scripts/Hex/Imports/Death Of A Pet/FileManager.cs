using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class TextFile
    {
        private string filename = "";
        FileInfo info;

        public TextFile(string fname)
        {
            filename = fname;
            info = new FileInfo(filename);
        }

        public void Create()
        {
            File.Create(filename);
        }

        public string Read()
        {
            string s = "";
            if (info.Exists)
            {
                s = File.ReadAllText(filename);
            }
            return s;
        }

        public void Write(string text)
        {
            Create();
            File.WriteAllText(filename, text);
        }

        public bool Exists()
        {
            bool b = info.Exists;
            return b;
        }
    }
}
