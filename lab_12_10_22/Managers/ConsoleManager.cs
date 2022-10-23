using lab_12_10_22.Enums;
using lab_12_10_22.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace lab_12_10_22.Managers
{
    public static class ConsoleManager
    {
        public static DirectoryInfo CurrentDirectory { get; set; } = new DirectoryInfo(Environment.CurrentDirectory);

        public static void Print(string input = "")
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[{Environment.UserName}] {CurrentDirectory.FullName} > ");
            Console.ResetColor();
            Console.Write(input);
        }

        public static void PrintError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        public static bool Input()
        {
            try
            {
                string inputString = "";

                int currentIndex = 0;
                do
                {
                    ConsoleKeyInfo readKeyResult = Console.ReadKey(true);

                    if (readKeyResult.Key == ConsoleKey.Tab)
                    {
                        if (inputString.ToLower().StartsWith("cd"))
                        {
                            string path = StringHelper.FormatPath(StringHelper.RemoveFlags(StringHelper.RemoveCommand(inputString)));
                            if (path.Length == 0)
                            {
                                var di = new DirectoryInfo(CurrentDirectory.FullName);
                                var diChildren = di.GetDirectories();
                                Console.WriteLine(Environment.NewLine);
                                if (diChildren.Length > 0)
                                {
                                    Console.WriteLine("Возможные папки:");
                                    foreach (var directoryInfo in diChildren)
                                    {
                                        Console.WriteLine(directoryInfo.Name);
                                    }
                                }
                                else
                                    Console.WriteLine("Не обнаружено папок.");
                                Print(inputString);
                            } else if (path[^1] == '/')
                            {
                                var diRelative = new DirectoryInfo(Path.Combine(CurrentDirectory.FullName, path));
                                var diAbsolute = new DirectoryInfo(path);
                                var di = diRelative;
                                if (!diRelative.Exists)
                                {
                                    di = diAbsolute;
                                }
                                Console.WriteLine(Environment.NewLine);
                                var diChildren = di.GetDirectories();
                                if (diChildren.Length > 0)
                                {
                                    Console.WriteLine("Возможные папки:");
                                    foreach (var directoryInfo in diChildren)
                                    {
                                        Console.WriteLine(directoryInfo.Name);
                                    }
                                }
                                else
                                    Console.WriteLine("Не обнаружено папок.");
                                Print(inputString);
                            }
                            else
                            {
                                if (path.Contains('/'))
                                {
                                    string[] pathParts = path.Split('/');
                                    string directoryNamePart = pathParts[^1];
                                    string pathBeforeDirectory = string.Join('/', pathParts[0..(pathParts.Length - 1)]);
                                    var diRelative = new DirectoryInfo(Path.Combine(CurrentDirectory.FullName, pathBeforeDirectory));
                                    var diAbsolute = new DirectoryInfo(pathBeforeDirectory);
                                    var di = diRelative;
                                    if (!diRelative.Exists)
                                    {
                                        di = diAbsolute;
                                    }
                                    var diChildren = di.GetDirectories();
                                    List<DirectoryInfo> matches = new List<DirectoryInfo>();
                                    foreach (var directoryInfo in diChildren)
                                    {
                                        if (directoryInfo.Name.ToLower().StartsWith(directoryNamePart.ToLower()))
                                        {
                                            matches.Add(directoryInfo);
                                        }
                                    }
                                    if (matches.Count == 0)
                                    {
                                        Console.WriteLine(Environment.NewLine + "Не обнаружено совпадений." + Environment.NewLine);
                                        Print(inputString);
                                    }
                                    else if (matches.Count == 1)
                                    {
                                        string inputAddition = matches[0].Name[directoryNamePart.Length..matches[0].Name.Length];
                                        inputString += inputAddition;
                                        currentIndex += inputAddition.Length;
                                        Console.WriteLine(Environment.NewLine);
                                        Print(inputString);
                                    }
                                    else
                                    {
                                        Console.WriteLine(Environment.NewLine + "Возможные папки:");
                                        foreach (var directoryInfo in matches)
                                        {
                                            Console.WriteLine(directoryInfo.Name);
                                        }
                                        Print(inputString);
                                    }
                                }
                                else
                                {
                                    string[] pathParts = StringHelper.FormatPath(Path.Combine(CurrentDirectory.FullName, path)).Split('/');
                                    string directoryNamePart = pathParts[^1];
                                    string pathBeforeDirectory = string.Join('/', pathParts[0..(pathParts.Length - 1)]);
                                    var di = new DirectoryInfo(pathBeforeDirectory);
                                    var diEnum = di.EnumerateDirectories();
                                    List<DirectoryInfo> matches = new List<DirectoryInfo>();
                                    foreach (var directoryInfo in diEnum)
                                    {
                                        if (directoryInfo.Name.ToLower().StartsWith(directoryNamePart.ToLower()))
                                        {
                                            matches.Add(directoryInfo);
                                        }
                                    }
                                    if (matches.Count == 0)
                                    {
                                        Console.WriteLine(Environment.NewLine + "Не обнаружено совпадений." + Environment.NewLine);
                                        Print(inputString);
                                    }
                                    else if (matches.Count == 1)
                                    {
                                        string inputAddition = matches[0].Name[directoryNamePart.Length..matches[0].Name.Length];
                                        inputString += inputAddition;
                                        currentIndex += inputAddition.Length;
                                        Console.WriteLine(Environment.NewLine);
                                        Print(inputString);
                                    }
                                    else
                                    {
                                        Console.WriteLine(Environment.NewLine + "Возможные папки:");
                                        foreach (var directoryInfo in matches)
                                        {
                                            Console.WriteLine(directoryInfo.Name);
                                        }
                                        Print(inputString);
                                    }
                                }
                            }
                        }
                    }
                    else if (readKeyResult.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }
                    else if (readKeyResult.Key == ConsoleKey.Backspace)
                    {
                        if (currentIndex > 0)
                        {
                            inputString = inputString.Remove(inputString.Length - 1);
                            Console.Write(readKeyResult.KeyChar);
                            Console.Write(' ');
                            Console.Write(readKeyResult.KeyChar);
                            currentIndex--;
                        }
                    }
                    else
                    {
                        inputString += readKeyResult.KeyChar;
                        Console.Write(readKeyResult.KeyChar);
                        currentIndex++;
                    }
                }
                while (true);

                string[] parameters = inputString.Trim().Split(' ');
                string command = parameters[0].ToLower();
                string[] args = parameters[1..parameters.Length];
                var flags = StringHelper.GetFlags(args);
                if (command == "cd")
                {
                    string path = string.Join(" ", args);
                    CurrentDirectory = DirectoryManager.MoveToDirectory(CurrentDirectory, path);
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
                else if (command == "clear") Clear();
                else if (command == "mkdir")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        DirectoryManager.CreateDirectory(path, flags.ContainsKey("-rf"));
                    }
                    else
                        throw new Exception("Имя папки не введено.");
                }
                else if (command == "rmdir")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        DirectoryManager.RemoveDirectory(path, flags.ContainsKey("-rf"));
                    }
                    else
                        throw new Exception("Имя папки не введено.");
                }
                else if (command == "touch")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        FileManager.CreateFile(path, flags.ContainsKey("-rf"));
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "rm")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        FileManager.RemoveFile(path, flags.ContainsKey("-rf"));
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "cat")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        FileManager.OutputFile(path);
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "vim")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        FileManager.EditFile(path);
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "tree")
                {
                    if (args.Length > 0)
                    {
                        string path = StringHelper.RemoveFlags(string.Join(" ", args));
                        int depth = 2;
                        if (flags.ContainsKey("-depth"))
                        {
                            depth = int.Parse(flags["-depth"]);
                        }
                        DirectoryManager.PrintTree(new DirectoryInfo(path), depth);
                    }
                    else
                        throw new Exception("Имя файла не введено.");
                }
                else if (command == "exit")
                {
                    return false;
                }
                else
                {
                    throw new Exception("Неизвестная команда.");
                }
                Print();
            }
            catch (UnauthorizedAccessException)
            {
                PrintError("Нет доступа к папке или файлу.");
            }
            catch (Exception e)
            {
                PrintError(e.Message);
                Print();
            }
            return true;
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void PrintDirectoryInfo(DirectoryInfo di)
        {
            var directorySize = SizeHelper.GetDirectorySize(di);
            Console.WriteLine("{0,0}{1,40}{2,20}{3,20}", "(DIR)", di.Name, di.CreationTime, SizeHelper.FormatSize(directorySize));
        }

        public static void PrintFileInfo(FileInfo fi)
        {
            Console.WriteLine("{0,0}{1,40}{2,20}{3,20}", "(FILE)", fi.Name, fi.CreationTime, SizeHelper.FormatSize(fi.Length));
        }
    }
}
