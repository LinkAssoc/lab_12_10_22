using lab_12_10_22.Enums;

namespace lab_12_10_22.Helpers
{
    public static class SizeHelper
    {
        public static long GetDirectorySize(DirectoryInfo di)
        {
            long size = 0;
            foreach (FileInfo fi in di.EnumerateFiles())
            {
                size += fi.Length;
            }
            return size;
        }

        public static string FormatSize(double size)
        {
            int i = 0;
            while (size > 1024)
            {
                size /= 1024;
                i++;
            }
            return Math.Round(size, 3) + ((FileMeasure)i).ToString();
        }
    }
}
