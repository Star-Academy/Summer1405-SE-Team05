namespace t4;

using System.Text;

public abstract class SqlCompiler
{
    protected abstract string WrapIdentifier(string identifier);
    protected abstract string FormatParameter(int index);

    protected virtual object TransformValue(object value) => value;
    protected abstract string ColumnSeparator { get; }

    protected virtual string GetLogicalOperatorString(LogicalOperator logicalOp) => logicalOp switch
    {
        LogicalOperator.And => "AND",
        LogicalOperator.Or => "OR",
        _ => throw new ArgumentOutOfRangeException(nameof(logicalOp), logicalOp, null)
    };

    public virtual SqlResult Compile(Query query)
    {
        var bindings = new List<object>();
        var sb = new StringBuilder();

        sb.Append(CompileSelect(query));
        sb.Append(" ");
        sb.Append(CompileFrom(query));

        if (query.Clauses.Count > 0)
        {
            sb.Append(" ");
            sb.Append(CompileWhere(query, bindings));
        }

        return new SqlResult(sb.ToString(), bindings);
    }

    private string CompileSelect(Query query)
    {
        var cols = query.Columns.Count > 0
            ? string.Join(ColumnSeparator, query.Columns.Select(WrapIdentifier))
            : "*";

        return $"SELECT {cols}";
    }

    private string CompileFrom(Query query)
    {
        return $"FROM {WrapIdentifier(query.Table)}";
    }

    private string CompileWhere(Query query, List<object> bindings)
    {
        var sb = new StringBuilder("WHERE ");

        for (int i = 0; i < query.Clauses.Count; i++)
        {
            var clause = query.Clauses[i];
            var cond = clause.Condition;

            if (i > 0)
            {
                sb.Append($" {GetLogicalOperatorString(clause.LogicalOp)} ");
            }

            string paramPlaceholder = FormatParameter(i);
            object processedValue = TransformValue(cond.Value);

            sb.Append($"{WrapIdentifier(cond.Column)} {cond.Operator.Symbol} {paramPlaceholder}");
            bindings.Add(processedValue);
        }

        return sb.ToString();
    }
}