using lab_12_10_22.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_12_10_22
{
    public static class StringHelper
    {
        public static Dictionary<string, Flag> FlagsWithArguments = new Dictionary<string, Flag>();

        public static Dictionary<string, Flag> FlagsWithoutArguments = new Dictionary<string, Flag>();

        public static void InitializeFlagsDictionaries()
        {
            FlagsWithArguments.Add("depth", Flag.depth);
            FlagsWithoutArguments.Add("rf", Flag.rf);
        }

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

        public static Dictionary<string, string> GetFlags(string input)
        {
            var dict = new Dictionary<string, string>();
            string[] inputArray = input.Split(' ');
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (inputArray[i].StartsWith("-"))
                {
                    string arg = inputArray[i][1..inputArray[i].Length];
                    if (FlagsWithArguments.ContainsKey(arg))
                    {
                        dict.Add(inputArray[i], inputArray[i + 1]);
                    }
                    else
                    {
                        dict.Add(inputArray[i], "true");
                    }
                }
            }
            return dict;
        }

        public static Dictionary<string, string> GetFlags(string[] input)
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < input.Length; i++)
            {
                string arg = input[i][1..input[i].Length];
                if (FlagsWithArguments.ContainsKey(arg))
                {
                    dict.Add(input[i], input[i + 1]);
                }
                else
                {
                    dict.Add(input[i], "true");
                }
            }
            return dict;
        }
    }
}
