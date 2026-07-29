using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchApp
{
    public class SearchAnalytics
    {
        public void PrintStats(List<string> list)
        {
            var result = list.GroupBy(command => command).OrderByDescending(item => item.Count());
            var count = 0;
            foreach (var item in result)
            {
                Console.WriteLine(item.Key);
                count++;
                if (count == 3) break;
            }
        }

        public void PrintUnique(List<string> list)
        {
            var result = list.GroupBy(w => w);
            foreach (var item in result)
            {
                if (item.Count() == 1) Console.WriteLine(item.Key);
            }
        }
    }
}