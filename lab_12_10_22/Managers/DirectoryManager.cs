namespace lab_12_10_22.Managers
{
    public static class DirectoryManager
    {
        public static DirectoryInfo MoveToDirectory(DirectoryInfo currentDirectory, string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            if (path.Length == 0)
            {
                throw new Exception("Не введен путь.");
            }
            if (path == "..")
            {
                if (currentDirectory.Parent == null)
                {
                    throw new Exception("Папки не существует.");
                }
                directoryInfo = currentDirectory.Parent;
            }
            else
            {
                var directory = new DirectoryInfo(Path.Combine(currentDirectory.FullName, path));
                var directory2 = new DirectoryInfo(path);
                if (directory.Exists)
                {
                    directoryInfo = directory;
                }
                else if (directory2.Exists)
                {
                    directoryInfo = directory2;
                }
                else if (path != ".")
                {
                    throw new Exception("Такой папки не существует.");
                }
            }
            return directoryInfo;
        }

        public static void CreateDirectory(string path, bool force)
        {
            if (Directory.Exists(path) && !force)
            {
                throw new Exception("Папка с таким именем существует.");
            }
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            new DirectoryInfo(path).Create();
        }

        public static void RemoveDirectory(string path, bool force)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception("Папки с таким именем не существует.");
            }
            var di = new DirectoryInfo(path);
            if ((di.GetDirectories().Length > 0 || di.GetFiles().Length > 0) && !force)
            {
                throw new Exception("Папка не пуста.");
            }
            Directory.Delete(path, true);
        }

        public static void PrintTree(
            DirectoryInfo di,
            int depth = 10,
            int intendCount = 0)
        {
            if (intendCount == depth)
            {
                return;
            }
            foreach (var diChild in di.EnumerateDirectories())
            {
                string intend = "";
                for (int i = 0; i < intendCount * 4; i++)
                {
                    intend += '-';
                }
                Console.WriteLine(intend + (intendCount != 0 ? " " : "") + diChild.Name);
                PrintTree(diChild, depth, intendCount + 1);
            }
        }

        public static DirectoryInfo GetDirectoryInfoFromPath(string path, DirectoryInfo currentDirectory)
        {
            var diRelative = new DirectoryInfo(Path.Combine(currentDirectory.FullName, path));
            var diAbsolute = new DirectoryInfo(path);
            var di = diRelative;
            if (!diRelative.Exists)
            {
                di = diAbsolute;
            }
            return di;
        }
    }
}
