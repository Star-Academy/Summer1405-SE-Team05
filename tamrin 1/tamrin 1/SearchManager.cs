namespace tamrin_1
{
    public class SearchManager
    {
        public List<string> List { get; set; } = new List<string>();
        //for saving all commands

        private int Current { get; set; } 
        //for showing where we are right now

        public void AddSearch(string command)
        {
            if (Current < List.Count - 1)
            {
                List.RemoveRange(Current + 1, List.Count - (Current + 1));
            }
            List.Add(command.Substring("search".Length));
            Current++;
        }
        public void GoBack()
        {
            if (Current > 0) Current--;
        }

        public void GoForward()
        {
            if (Current < List.Count - 1) Current++;
        }

        public void PrintCurrent()
        {
            if (Current == -1) Console.WriteLine("it command empty");
            else Console.WriteLine(List[Current]);
        }
    }
}