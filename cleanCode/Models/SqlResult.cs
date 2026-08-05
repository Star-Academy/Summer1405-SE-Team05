namespace CleanCode;

public class SqlInput
{
    public SqlInput(string sql, List<object> bindings)
    {
        Sql = sql;
        Bindings = bindings.AsReadOnly();
    }

    public string Sql { get; }
    public IReadOnlyList<object> Bindings { get; }
}