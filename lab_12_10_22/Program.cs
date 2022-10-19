using System;

namespace lab_12_10_22
{
    internal class Program
    {
        

        static void Main(string[] args)
        {
            var manager = new Manager();
            manager.Print();
            bool toContinue = true;
            while (toContinue)
            {
                toContinue = manager.Input();
            }
        }
    }
}