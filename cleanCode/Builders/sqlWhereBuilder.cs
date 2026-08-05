using System.Text;

namespace CleanCode;

internal sealed class SqlWhereBuilder : IWhereBuilder
{
    private readonly IParameterIdentifier _paramIdentifier;

    public SqlWhereBuilder(IParameterIdentifier paramIdentifier)
    {
        _paramIdentifier = paramIdentifier ?? throw new ArgumentNullException(nameof(paramIdentifier));
    }
    public string Build(List<QueryClause> clauses, List<object> bindings)
    {
        if (clauses.Count == 0) return string.Empty;

        var stringBuilder = new StringBuilder("WHERE ");

        for (var i = 0; i < clauses.Count; i++)
        {
            var clause = clauses[i];
            var condition = clause.WhereCondition;

            if (i > 0)
                stringBuilder.Append($" {_paramIdentifier.GetLogicalOperatorString(clause.LogicalOperator)} ");

            var column = _paramIdentifier.WrapIdentifier(condition.Column);
            var paramPlaceholder = _paramIdentifier.FormatParameter(i);
            var processedValue = _paramIdentifier.TransformValue(condition.Value);

            stringBuilder.Append($"{column} {condition.Operator.Symbol} {paramPlaceholder}");
            bindings.Add(processedValue);
        }

        return stringBuilder.ToString();
    }
}