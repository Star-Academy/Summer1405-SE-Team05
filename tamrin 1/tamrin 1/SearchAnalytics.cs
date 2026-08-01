namespace tamrin_1
{
    public static class SearchAnalytics
    {
        public static void PrintStats(List<string> list)
        {
            var result = list.GroupBy(command => command).OrderByDescending(item => item.Count());
            
            foreach (var item in result.Take(3))
            {
                Console.WriteLine(item.Key);
            }
        }

        public static void PrintUnique(List<string> list)
        {
            var result = list.GroupBy(w => w);
            
            foreach (var item in result
                         .Where(x => x.Count() == 1))
            {
                Console.WriteLine(item.Key);
            }
        }
    }
}