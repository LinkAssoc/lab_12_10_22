using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace lab_12_10_22
{
    public enum Command
    {

    }

    public class Manager
    {
        private static DirectoryInfo Directory { get; set; } = new DirectoryInfo(Environment.CurrentDirectory);

        public void Print()
        {
            Console.WriteLine(Directory.FullName);
        }

        /*
        Возможная реализация вызова функций
        // TODO: Реализовать поиск функции по toLower строке
        public string Input()
        {
            string[] parameters = Console.ReadLine().Trim().Split(' ');
            string command = parameters[0];
            string[] args = parameters[1..parameters.Length];
            Type thisType = GetType();
            MethodInfo method = thisType.GetMethod(command);
            method.Invoke(this, args);
            return command;
        }
        */
        public bool Input()
        {
            try
            {
                string[] parameters = Console.ReadLine().Trim().Split(' ');
                string command = parameters[0].ToLower();
                string[] args = parameters[1..parameters.Length];
                if (command == "cd")
                {
                    string path = string.Join(" ", args);
                    if (path.Length == 0)
                        throw new Exception("Не введен путь.");
                    if (path == "..")
                    {
                        if (Directory.Parent == null)
                        {
                            throw new Exception("Папки не существует.");
                        }
                        Directory = Directory.Parent;
                    }
                    else
                    {
                        var directory = new DirectoryInfo(Path.Combine(Directory.FullName, path));
                        var directory2 = new DirectoryInfo(path);
                        if (directory.Exists)
                        {
                            Directory = directory;
                        }
                        else if (directory2.Exists)
                        {
                            Directory = directory2;
                        }
                        else if (path != ".")
                        {
                            throw new Exception("Такой папки не существует.");
                        }
                    }
                }
                else if (command == "ls")
                {
                    foreach (DirectoryInfo di in Directory.EnumerateDirectories())
                    {
                        PrintDirectoryInfo(di);
                    }
                    foreach (FileInfo fi in Directory.EnumerateFiles())
                    {
                        PrintFileInfo(fi);
                    }
                }
                else if (command == "exit")
                {
                    return false;
                }
                Print();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return true;
            }
        }

        void PrintDirectoryInfo(DirectoryInfo di)
        {
            var directorySize = GetDirectorySize(di);
            Console.WriteLine("{0,0}{1,40}{2,20}{3,20}", "(DIR)", di.Name, di.CreationTime, FormatSize(directorySize));
        }

        void PrintFileInfo(FileInfo fi)
        {
            Console.WriteLine("{0,0}{1,40}{2,20}{3,20}", "(FILE)", fi.Name, fi.CreationTime, FormatSize(fi.Length));
        }

        long GetDirectorySize(DirectoryInfo di)
        {
            long size = 0;
            var currentDirectory = di;
            foreach (FileInfo fi in currentDirectory.EnumerateFiles())
            {
                size += fi.Length;
            }
            foreach (DirectoryInfo subDi in currentDirectory.EnumerateDirectories())
            {
                foreach (FileInfo fi in subDi.EnumerateFiles())
                {
                    size += fi.Length;
                }
            }
            return size;
        }

        enum Measure
        {
            B,
            KB,
            MB,
            GB,
            TB
        }

        string FormatSize(double size)
        {
            int i = 0;
            while (size > 1024)
            {
                size /= 1024;
                i++;
            }
            return Math.Round(size, 3) + ((Measure)i).ToString();
        }
    }
}
