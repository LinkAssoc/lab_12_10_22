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
        private static DirectoryInfo CurrentDirectory { get; set; } = new DirectoryInfo(Environment.CurrentDirectory);

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{Environment.UserName}] {CurrentDirectory.FullName}");
            Console.ResetColor();
        }

        public void PrintError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
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
                        if (CurrentDirectory.Parent == null)
                        {
                            throw new Exception("Папки не существует.");
                        }
                        CurrentDirectory = CurrentDirectory.Parent;
                    }
                    else
                    {
                        var directory = new DirectoryInfo(Path.Combine(CurrentDirectory.FullName, path));
                        var directory2 = new DirectoryInfo(path);
                        if (directory.Exists)
                        {
                            CurrentDirectory = directory;
                        }
                        else if (directory2.Exists)
                        {
                            CurrentDirectory = directory2;
                        }
                        else if (path != ".")
                        {
                            throw new Exception("Такой папки не существует.");
                        }
                    }
                }
                else if (command == "ls")
                {
                    foreach (DirectoryInfo di in CurrentDirectory.EnumerateDirectories())
                    {
                        PrintDirectoryInfo(di);
                    }
                    foreach (FileInfo fi in CurrentDirectory.EnumerateFiles())
                    {
                        PrintFileInfo(fi);
                    }
                }
                else if (command == "clear")
                {
                    Clear();
                }
                else if (command == "mkdir")
                {
                    if (args.Length > 0)
                    {
                        string directoryPath = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveArguments(string.Join(" ", args)));
                        if (Directory.Exists(directoryPath) && !args.Contains("-rf"))
                        {
                            throw new Exception("Папка с таким именем существует.");
                        }
                        if (Directory.Exists(directoryPath))
                        {
                            Directory.Delete(directoryPath, true);
                        }
                        new DirectoryInfo(directoryPath).Create();
                    }
                    else
                        throw new Exception("Имя папки не введено.");
                }
                else if (command == "rmdir")
                {
                    if (args.Length > 0)
                    {
                        string directoryPath = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveArguments(string.Join(" ", args)));
                        if (!Directory.Exists(directoryPath))
                        {
                            throw new Exception("Папки с таким именем не существует.");
                        }
                        var di = new DirectoryInfo(directoryPath);
                        if ((di.GetDirectories().Length > 0 || di.GetFiles().Length > 0) && !args.Contains("-rf"))
                        {
                            throw new Exception("Папка не пуста.");
                        }
                        Directory.Delete(directoryPath, true);
                    }
                    else
                        throw new Exception("Имя папки не введено.");
                }
                else if (command == "touch")
                {
                    if (args.Length > 0)
                    {
                        string filePath = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveArguments(string.Join(" ", args)));
                        if (File.Exists(filePath) && !args.Contains("-rf"))
                        {
                            throw new Exception("Файл с таким именем существует.");
                        }
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        File.Create(filePath).Close();
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "rm")
                {
                    if (args.Length > 0)
                    {
                        string filePath = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveArguments(string.Join(" ", args)));
                        if (!File.Exists(filePath))
                        {
                            throw new Exception("Файл с таким именем не существует.");
                        }
                        var fi = new FileInfo(filePath);
                        if (fi.Length > 0 && !args.Contains("-rf"))
                        {
                            throw new Exception("Файл не пуст.");
                        }
                        File.Delete(filePath);
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "cat")
                {
                    if (args.Length > 0)
                    {
                        string filePath = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveArguments(string.Join(" ", args)));
                        if (!File.Exists(filePath))
                        {
                            throw new Exception("Файл с таким именем не существует.");
                        }
                        using StreamReader stream = new StreamReader(filePath);
                        while (!stream.EndOfStream)
                        {
                            Console.WriteLine(stream.ReadLine());               
                        }
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "vim")
                {
                    if (args.Length > 0)
                    {
                        string filePath = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveArguments(string.Join(" ", args)));
                        var fileManager = new FileManager(filePath);
                        fileManager.EditFile();
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "exit")
                {
                    return false;
                }
                Print();
                

                // здесь преобразовать в строку и вывод

            }
            catch (UnauthorizedAccessException)
            {
                PrintError("Нет доступа к папке или файлу.");
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
            return true;
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
            foreach (FileInfo fi in di.EnumerateFiles())
            {
                size += fi.Length;
            }
            return size;
        }

        enum FileMeasure
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
            return Math.Round(size, 3) + ((FileMeasure)i).ToString();
        }

        void Clear()
        {
            Console.Clear();
        }
    }
}
