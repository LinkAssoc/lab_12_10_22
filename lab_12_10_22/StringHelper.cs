using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_12_10_22
{
    public static class StringHelper
    {
        public static string RemoveArguments(string str)
        {
            string[] stringWithoutArgumentsArray = str.Split(' ');
            string stringWithoutArguments = "";
            for (int i = 0; i < stringWithoutArgumentsArray.Length; i++)
            {
                if (!stringWithoutArgumentsArray[i].StartsWith("-"))
                {
                    stringWithoutArguments += stringWithoutArgumentsArray[i] + " ";
                }
            }
            return stringWithoutArguments.Trim();
        }
    }
}
