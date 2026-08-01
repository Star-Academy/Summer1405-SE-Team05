namespace ClassLibrary1;

public class Query
{
    public string? Table { get; private set; }
    public List<string> Columns { get; private set; } = new List<string>(); 
    public Dictionary<string, object> WhereConditions { get; private set; } = new Dictionary<string, object>();

    public Query From(string table)
    {
        Table = table;
        return this;
    }

    public Query Select(params string[] columns)
    {
        Columns.AddRange(columns);
        return this;
    }

    public Query Where(string column, object value)
    {
        WhereConditions[column] = value;
        

        return this;
    }
}