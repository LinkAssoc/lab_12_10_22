using lab_12_10_22.Enums;
using lab_12_10_22.Managers;

namespace lab_12_10_22
{
    public static class StringHelper
    {
        private static readonly IReadOnlyDictionary<string, Flag> FlagsWithArguments = 
            new Dictionary<string, Flag>() 
            { 
                { "depth", Flag.Depth } 
            };

        private static readonly IReadOnlyDictionary<string, Flag> FlagsWithoutArguments =
            new Dictionary<string, Flag>()
            {
                { "rf", Flag.Rf },
                { "f", Flag.F }
            };

        public static string RemoveFlags(string str)
        {
            string[] stringWithoutArgumentsArray = str.Split(' ');
            string stringWithoutArguments = "";
            bool toContinueNext = false;
            for (int i = 0; i < stringWithoutArgumentsArray.Length; i++)
            {
                if (toContinueNext)
                {
                    toContinueNext = false;
                    continue;
                }
                if (stringWithoutArgumentsArray[i].StartsWith("-"))
                {
                    string arg = stringWithoutArgumentsArray[i][1..stringWithoutArgumentsArray[i].Length];
                    if (FlagsWithArguments.ContainsKey(arg))
                    {
                        toContinueNext = true;
                    }
                }
                else
                {
                    stringWithoutArguments += stringWithoutArgumentsArray[i] + " ";
                }
            }
            return stringWithoutArguments.Trim();
        }

        public static string RemoveCommand(string str)
        {
            string[] input = str.Trim().Split(' ');
            return string.Join(" ", input[1..input.Length]);
        }

        public static string FormatPath(string path)
        {
            return path.Replace('\\', '/');
        }

        public static Dictionary<Flag, string> GetFlags(string input)
        {
            var dict = new Dictionary<Flag, string>();
            string[] inputArray = input.Split(' ');
            for (int i = 0; i < inputArray.Length; i++)
            {
                string arg = inputArray[i][1..inputArray[i].Length];
                if (!inputArray[i].StartsWith('-'))
                {
                    continue;
                }
                if (arg.Contains('=') && FlagsWithArguments.ContainsKey(arg.Split('=')[0]))
                {
                    string[] flag = arg.Split('=');
                    dict.Add(FlagsWithArguments[flag[0]], flag[1]);
                }
                else if (FlagsWithoutArguments.ContainsKey(arg))
                {
                    dict.Add(FlagsWithoutArguments[arg], "true");
                }
            }
            return dict;
        }

        public static Dictionary<Flag, string> GetFlags(string[] input)
        {
            var dict = new Dictionary<Flag, string>();
            for (int i = 0; i < input.Length; i++)
            {
                string arg = input[i][1..input[i].Length];
                if (!input[i].StartsWith('-'))
                {
                    continue;
                }
                if (arg.Contains('=') && FlagsWithArguments.ContainsKey(arg.Split('=')[0]))
                {
                    string[] flag = arg.Split('=');
                    dict.Add(FlagsWithArguments[flag[0]], flag[1]);
                }
                else if (FlagsWithoutArguments.ContainsKey(arg))
                {
                    dict.Add(FlagsWithoutArguments[arg], "true");
                }
            }
            return dict;
        }

        public static void CdTabFunction(
            DirectoryInfo currentDirectory,
            string path,
            ref string inputString,
            ref int currentIndex)
        {
            if (path.Length == 0)
            {
                var di = new DirectoryInfo(currentDirectory.FullName);
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
                {
                    Console.WriteLine("Не обнаружено папок.");
                }
                ConsoleManager.Print(inputString);
            }
            else if (path[^1] == '/')
            {
                var di = DirectoryManager.GetDirectoryInfoFromPath(path, currentDirectory);
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
                {
                    Console.WriteLine("Не обнаружено папок.");
                }
                ConsoleManager.Print(inputString);
            }
            else
            {
                if (path.Contains('/'))
                {
                    string[] pathParts = path.Split('/');
                    string directoryNamePart = pathParts[^1];
                    string pathBeforeDirectory = string.Join('/', pathParts[0..(pathParts.Length - 1)]);
                    var di = DirectoryManager.GetDirectoryInfoFromPath(pathBeforeDirectory, currentDirectory);
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
                        ConsoleManager.Print(inputString);
                    }
                    else if (matches.Count == 1)
                    {
                        string inputAddition = matches[0].Name[directoryNamePart.Length..matches[0].Name.Length];
                        inputString += inputAddition;
                        currentIndex += inputAddition.Length;
                        Console.WriteLine(Environment.NewLine);
                        ConsoleManager.Print(inputString);
                    }
                    else
                    {
                        Console.WriteLine(Environment.NewLine + "Возможные папки:");
                        foreach (var directoryInfo in matches)
                        {
                            Console.WriteLine(directoryInfo.Name);
                        }
                        ConsoleManager.Print(inputString);
                    }
                }
                else
                {
                    string[] pathParts = FormatPath(Path.Combine(currentDirectory.FullName, path)).Split('/');
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
                        ConsoleManager.Print(inputString);
                    }
                    else if (matches.Count == 1)
                    {
                        string inputAddition = matches[0].Name[directoryNamePart.Length..matches[0].Name.Length];
                        inputString += inputAddition;
                        currentIndex += inputAddition.Length;
                        Console.WriteLine(Environment.NewLine);
                        ConsoleManager.Print(inputString);
                    }
                    else
                    {
                        Console.WriteLine(Environment.NewLine + "Возможные папки:");
                        foreach (var directoryInfo in matches)
                        {
                            Console.WriteLine(directoryInfo.Name);
                        }
                        ConsoleManager.Print(inputString);
                    }
                }
            }
        }
    }
}
