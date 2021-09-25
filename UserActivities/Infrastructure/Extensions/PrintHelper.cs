using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreActivities.Extensions
{
    public static class PrintHelper
    {
        public static string PrintResult(this IList<string> results)
        {
            results = results.Select((x, index) =>
            {
                x = $"{index+1}. {x}";
                return x;
            }).ToList();

            var tupleList = new List<(string, string, string)>();
            var main = results.Count - (results.Count % 3);
            var left = results.Count % 3;

            if (main != 0)
            {
                for (int i = 0; i < main; i += 3)
                    tupleList.Add((results[i], results[i + 1], results[i + 2]));
            }

            if (left != 0 && left == 1)
                tupleList.Add((results[main], string.Empty, string.Empty));
            else if (left != 0 && left == 2)
                tupleList.Add((results[main], results[main + 1], string.Empty));
            else if (left != 0 && left == 3)
                tupleList.Add((results[main], results[main + 1], results[main + 2]));

            var result = tupleList.AsEnumerable().ToStringTable(
                    new[] { "", "", "" },
                    a => a.Item1, a => a.Item2, a => a.Item3);

            Console.WriteLine(result);
            Console.WriteLine();

            return result;
        }

        public static void PrintResult(this string result)
            => Console.WriteLine(result);

        public static void ClearScreen()
        {
            Console.WriteLine("Clear Screen? (y/n)");
            var option = Console.ReadLine();
            if (option == "y")
                Console.Clear();
        }
    }
}
