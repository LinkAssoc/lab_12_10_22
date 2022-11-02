using System.Text;

namespace lab_12_10_22.Managers
{
    public static class FileManager
    {
        public static void CreateFile(string path, bool force)
        {
            if (File.Exists(path) && !force)
            {
                throw new Exception("Файл с таким именем существует.");
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Create(path).Close();
        }

        public static void RemoveFile(string path, bool force)
        {
            if (!File.Exists(path))
            {
                throw new Exception("Файл с таким именем не существует.");
            }
            var fi = new FileInfo(path);
            if (fi.Length > 0 && !force)
            {
                throw new Exception("Файл не пуст.");
            }
            File.Delete(path);
        }

        public static void OutputFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("Файл с таким именем не существует.");
            }
            using StreamReader stream = new StreamReader(path);
            while (!stream.EndOfStream)
            {
                Console.WriteLine(stream.ReadLine());
            }
        }

        public static void EditFile(string path)
        {
            ConsoleKeyInfo c;
            using StreamReader sr = new StreamReader(path);
            var sb = new StringBuilder();
            sb.Append(sr.ReadToEnd());
            sr.Close();
            do
            {
                Console.Clear();
                Console.WriteLine(sb.ToString());
                c = Console.ReadKey(true);
                if (c.Key == ConsoleKey.Enter)
                {
                    sb.Append(Environment.NewLine);
                }
                else if (c.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                else if (c.Key != ConsoleKey.Escape)
                {
                    sb.Append(c.KeyChar);
                }
            }
            while (c.Key != ConsoleKey.Escape);
            using StreamWriter sw = new StreamWriter(path);
            sw.WriteLine(sb.ToString());
        }
    }
}
