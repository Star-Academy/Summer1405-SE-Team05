namespace CleanCode;

public sealed class DataBaseInput
{
    public DataBaseInput(string queryString, List<object> bindings)
    {
        QueryString = queryString;
        Bindings = bindings.AsReadOnly();
    }

    public string QueryString { get; }
    public IReadOnlyList<object> Bindings { get; }
}