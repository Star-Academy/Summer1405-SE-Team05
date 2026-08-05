namespace CleanCode;

using System.Text;

// ساخت بخش SELECT
public class SqlSelectBuilder : ISelectBuilder
{
    public string Build(List<string> columns, IParameterIdentifier paramIdentifier)
    {
        var cols = columns.Count > 0
            ? string.Join(paramIdentifier.ColumnSeparator, columns.Select(paramIdentifier.WrapIdentifier))
            : "*";

        return $"SELECT {cols}";
    }
}

// ساخت بخش FROM
public class SqlFromBuilder : IFromBuilder
{
    public string Build(string? table, IParameterIdentifier paramIdentifier)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentException("Table name cannot be null or empty.");

        return $"FROM {paramIdentifier.WrapIdentifier(table)}";
    }
}

// ساخت بخش WHERE
public class SqlWhereBuilder : IWhereBuilder
{
    public string Build(List<QueryClause> clauses, IParameterIdentifier paramIdentifier, List<object> bindings)
    {
        if (clauses.Count == 0) return string.Empty;

        var sb = new StringBuilder("WHERE ");

        for (var i = 0; i < clauses.Count; i++)
        {
            var clause = clauses[i];
            var cond = clause.Condition;

            if (i > 0)
                sb.Append($" {paramIdentifier.GetLogicalOperatorString(clause.LogicalOp)} ");

            var column = paramIdentifier.WrapIdentifier(cond.Column);
            var paramPlaceholder = paramIdentifier.FormatParameter(i);
            var processedValue = paramIdentifier.TransformValue(cond.Value);

            sb.Append($"{column} {cond.Operator.Symbol} {paramPlaceholder}");
            bindings.Add(processedValue);
        }

        return sb.ToString();
    }
}
