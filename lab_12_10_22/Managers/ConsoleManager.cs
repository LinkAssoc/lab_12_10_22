using lab_12_10_22.Enums;
using lab_12_10_22.Helpers;

namespace lab_12_10_22.Managers
{
    public static class ConsoleManager
    {
        public static DirectoryInfo CurrentDirectory { get; set; } = new DirectoryInfo(Environment.CurrentDirectory);
        private static List<string> commandsHistory = new List<string>();
        private static int commandsIndex = -1;

        public static void Print(string input = "")
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[{Environment.UserName}] {StringHelper.FormatPath(CurrentDirectory.FullName)} > ");
            Console.ResetColor();
            Console.Write(input);
        }

        public static void PrintError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        public static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("-----HELP-----");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("cd [path] - Переход по папкам (Нажимая таб, вы используете помощник построения пути)");
            Console.WriteLine("ls - Вывод содержания текущей папки с дополнительной информацией");
            Console.WriteLine("clear - Очистить консоль");
            Console.WriteLine("mkdir [path to directory] - Создать папку");
            Console.WriteLine("mkdir [path to directory] -rf - Создать папку / пересоздать пустую папку, если уже существует с таким названием");
            Console.WriteLine("rmdir [path to directory] - Удалить папку");
            Console.WriteLine("rmdir [path to directory] -rf - Удалить папку / удалить папку с содержимым внутри, если она непустая");
            Console.WriteLine("touch [path to file] - Создать файл");
            Console.WriteLine("touch [path to file] -f - Создать пустой файл, если уже существует с таким названием");
            Console.WriteLine("rm [path to file] - Удалить файл");
            Console.WriteLine("rm [path to file] -f - Удалить файл с содержимым внутри, если он непустой");
            Console.WriteLine("cat [path to file] - Вывести содержание файла");
            Console.WriteLine("vim [path to file] - Редактировать файл. Escape - выйти из редактирования файла, изменения сохранятся");
            Console.WriteLine("tree [path to directory] - Вывести дерево");
            Console.WriteLine("tree [path to directory] -depth=value - Вывести дерево, value - глубина (если не введено, то depth = 2)");
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
                            StringHelper.CdTabFunction(CurrentDirectory, path, ref inputString, ref currentIndex);
                        }
                    }
                    else if (readKeyResult.Key == ConsoleKey.Enter)
                    {
                        commandsHistory.Add(inputString);
                        commandsIndex = commandsHistory.Count - 1;
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
                    else if (readKeyResult.Key == ConsoleKey.UpArrow
                        && commandsIndex > 0
                        && commandsHistory.Count >= commandsIndex)
                    {
                        inputString = CommandsHistoryUp(ref currentIndex);
                        Console.WriteLine();
                        Print(inputString);
                    }
                    else if (readKeyResult.Key == ConsoleKey.DownArrow
                        && commandsIndex > 0
                        && commandsHistory.Count >= commandsIndex + 2)
                    {
                        inputString = CommandsHistoryDown(ref currentIndex);
                        Console.WriteLine();
                        Print(inputString);
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
                else if (command == "clear")
                {
                    Clear();
                }
                else if (command == "mkdir")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        DirectoryManager.CreateDirectory(path, flags.ContainsKey(Flag.Rf));
                    }
                    else
                    {
                        throw new Exception("Имя папки не введено.");
                    }
                }
                else if (command == "rmdir")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        DirectoryManager.RemoveDirectory(path, flags.ContainsKey(Flag.Rf));
                    }
                    else
                    {
                        throw new Exception("Имя папки не введено.");
                    }
                }
                else if (command == "touch")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        FileManager.CreateFile(path, flags.ContainsKey(Flag.F));
                    }
                    else
                    {
                        throw new Exception("Имя файла не введено.");
                    }
                }
                else if (command == "rm")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        FileManager.RemoveFile(path, flags.ContainsKey(Flag.F));
                    }
                    else
                    {
                        throw new Exception("Имя файла не введено.");
                    }
                }
                else if (command == "cat")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        FileManager.OutputFile(path);
                    }
                    else
                    {
                        throw new Exception("Имя файла не введено.");
                    }
                }
                else if (command == "vim")
                {
                    if (args.Length > 0)
                    {
                        string path = Path.Combine(CurrentDirectory.FullName, StringHelper.RemoveFlags(string.Join(" ", args)));
                        FileManager.EditFile(path);
                    }
                    else
                    {
                        throw new Exception("Имя файла не введено.");
                    }
                }
                else if (command == "tree")
                {
                    if (args.Length > 0)
                    {
                        string path = StringHelper.RemoveFlags(string.Join(" ", args));
                        int depth = 2;
                        if (flags.ContainsKey(Flag.Depth))
                        {
                            depth = int.Parse(flags[Flag.Depth]);
                        }
                        DirectoryManager.PrintTree(new DirectoryInfo(path), depth);
                    }
                    else
                    {
                        throw new Exception("Имя файла не введено.");
                    }
                }
                else if (command == "help")
                {
                    PrintHelp();
                }
                else if (command == "exit")
                {
                    return false;
                }
                else
                {
                    throw new Exception("Неизвестная команда.");
                }
            }
            catch (UnauthorizedAccessException)
            {
                PrintError("Нет доступа к папке или файлу.");
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
            Print();
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

        public static string CommandsHistoryUp(ref int currentIndex)
        {
            commandsIndex--;
            string input = commandsHistory[commandsIndex];
            currentIndex = input.Length;
            return input;
        }

        public static string CommandsHistoryDown(ref int currentIndex)
        {
            commandsIndex++;
            string input = commandsHistory[commandsIndex];
            currentIndex = input.Length;
            return input;
        }
    }
}
