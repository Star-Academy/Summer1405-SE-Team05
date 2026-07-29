using System;

namespace SearchApp
{
    public class Program
    {
        public static void Main()
        {
            var manager = new SearchManager();
            var analytics = new SearchAnalytics();

            while (true)
            {
                var command = Console.ReadLine().Trim().ToLower();

                if (command.Equals("exit"))
                {
                    break;
                }
                else if (command.StartsWith("search"))
                {
                    manager.AddSearch(command);
                }
                else if (command.StartsWith("back"))
                {
                    manager.GoBack();
                }
                else if (command.StartsWith("forward"))
                {
                    manager.GoForward();
                }
                else if (command.StartsWith("current"))
                {
                    manager.PrintCurrent();
                }
                else if (command.StartsWith("stats"))
                {
                    analytics.PrintStats(manager.List);
                }
                else if (command.StartsWith("unique"))
                {
                    analytics.PrintUnique(manager.List);
                }
            }
        }
    }
}