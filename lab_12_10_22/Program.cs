using lab_12_10_22.Managers;
using System;

namespace lab_12_10_22
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StringHelper.InitializeFlagsDictionaries();
            ConsoleManager.Print();
            bool toContinue = true;
            while (toContinue)
            {
                toContinue = ConsoleManager.Input();
            }
        }
    }
}