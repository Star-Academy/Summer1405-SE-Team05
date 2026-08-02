namespace t4;

public class SqlResult
{
    public string Sql { get; }
    public IReadOnlyList<object> Bindings { get; }

    public SqlResult(string sql, List<object> bindings)
    {
        Sql = sql;
        Bindings = bindings.AsReadOnly();
    }
}