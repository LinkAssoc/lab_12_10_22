using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_12_10_22
{
    public class FileManager
    {
        public string FilePath { get; set; }

        public FileManager(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("Файл с таким именем не существует.");
            }
            FilePath = filePath;
        }

        public bool EditFile()
        {
            ConsoleKeyInfo c;
            using StreamReader sr = new StreamReader(FilePath);
            string lines = "";
            lines += sr.ReadToEnd();
            sr.Close();
            do
            {
                Console.Clear();
                Console.WriteLine(lines);
                c = Console.ReadKey(true);
                if (c.Key == ConsoleKey.Enter)
                {
                    lines += Environment.NewLine;
                }
                else if (c.Key == ConsoleKey.Backspace && lines.Length > 0)
                {
                    lines = lines.Remove(lines.Length - 1, 1);
                }
                else if (c.Key != ConsoleKey.Escape)
                {
                    lines += c.KeyChar;
                }
            }
            while (c.Key != ConsoleKey.Escape);
            using StreamWriter sw = new StreamWriter(FilePath);
            sw.WriteLine(lines);
            return true;
        }
    }
}
