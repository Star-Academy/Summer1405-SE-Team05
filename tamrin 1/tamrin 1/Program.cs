List<String> list = new List<String>();
int current = -1;
while (true)
{
    String s = Console.ReadLine().Trim().ToLower();
    
    if (s.Equals("exit"))
    {
        break;
    }else if (s.StartsWith("search"))
    {
        list.Add(s.Substring("search".Length));
        current++;
    }else if (s.StartsWith("back"))
    {
        if (current > 0) current--;
    }else if (s.StartsWith("forward"))
    {
        if(current < list.Count-1)current++;
    }else if (s.StartsWith("current"))
    {
        if (current == -1) Console.WriteLine("it s empty");
        else Console.WriteLine(list[current]);
    }else if (s.StartsWith("stats"))
    {
        var result = list.GroupBy(w => w).OrderByDescending(x => x.Count());
        int count = 0;
        foreach (var item in result)
        {
            Console.WriteLine(item.Key);
            count++;
            if(count == 3)break;
        }
    }else if (s.StartsWith("unique"))
    {
        var result = list.GroupBy(w => w);
        foreach (var item in result)
        {
            if(item.Count() == 1) Console.WriteLine(item.Key);
        }
    }
}