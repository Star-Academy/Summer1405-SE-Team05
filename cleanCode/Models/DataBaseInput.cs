namespace CleanCode;

public sealed class DataBaseInput
{
    public DataBaseInput(string sql, List<object> bindings)
    {
        QueryString = sql;
        Bindings = bindings.AsReadOnly();
    }

    public string QueryString { get; }
    public IReadOnlyList<object> Bindings { get; }
}