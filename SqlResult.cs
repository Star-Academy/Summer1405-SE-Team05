namespace ClassLibrary1;

public class SqlResult
{
    public string Sql { get; set; } = string.Empty;

    public List<object> Bindings { get; set; } = new List<object>();
}