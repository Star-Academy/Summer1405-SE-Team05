using tamrin_1;

var manager = new SearchManager();

while (true)
{
    var input = Console.ReadLine().Trim();

    var firstWord = input.Split(' ')[0];

    LanguageDictionary.Commands.TryGetValue(firstWord, out var commandType);
    switch (commandType)
    {
        case CommandType.Exit:
            return;

        case CommandType.Search:
            manager.AddSearch(input);
            break;

        case CommandType.Back:
            manager.GoBack();
            break;

        case CommandType.Forward:
            manager.GoForward();
            break;

        case CommandType.Current:
            manager.PrintCurrent();
            break;

        case CommandType.Stats:
            SearchAnalytics.PrintStats(manager.List);
            break;

        case CommandType.Unique:
            SearchAnalytics.PrintUnique(manager.List);
            break;

        default:
            Console.WriteLine("Command not recognized.");
            break;
    }
}