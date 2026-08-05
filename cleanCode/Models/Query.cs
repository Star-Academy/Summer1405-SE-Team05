namespace CleanCode;

public class Query
{
    public string? Table { get; private set; }
    public List<string> Columns { get; } = new();
    public List<QueryClause> Clauses { get; } = new();

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

    public Query Where(string column, ExpressionOperator op, object value)
    {
        var condition = new WhereCondition(column, op, value);
        Clauses.Add(new QueryClause(condition, LogicalOperator.And));
        return this;
    }

    public Query Where(string column, object value)
    {
        return Where(column, ExpressionOperator.Equals, value);
    }

    public Query OrWhere(string column, ExpressionOperator op, object value)
    {
        var condition = new WhereCondition(column, op, value);
        Clauses.Add(new QueryClause(condition, LogicalOperator.Or));
        return this;
    }

    public Query OrWhere(string column, object value)
    {
        return OrWhere(column, ExpressionOperator.Equals, value);
    }
}