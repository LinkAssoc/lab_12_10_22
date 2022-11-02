using lab_12_10_22.Managers;

namespace lab_12_10_22
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleManager.Print();
            bool toContinue = true;
            while (toContinue)
            {
                toContinue = ConsoleManager.Input();
            }
        }
    }
}