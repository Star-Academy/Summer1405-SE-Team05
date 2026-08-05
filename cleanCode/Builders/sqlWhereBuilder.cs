using System.Text;

namespace CleanCode;


internal sealed class SqlWhereBuilder : IWhereBuilder
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