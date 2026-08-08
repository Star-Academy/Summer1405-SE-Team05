using System.Text;

namespace CleanCode;

internal sealed class SqlWhereBuilder : IWhereBuilder
{
    private readonly IExpressionOperator _expressionOperator;
    private readonly IParameterIdentifier _paramIdentifier;

    public SqlWhereBuilder(
        IParameterIdentifier paramIdentifier,
        IExpressionOperator expressionOperator)
    {
        _paramIdentifier = paramIdentifier ?? throw new ArgumentNullException(nameof(paramIdentifier));
        _expressionOperator = expressionOperator ?? throw new ArgumentNullException(nameof(expressionOperator));
    }

    public string Build(List<QueryClause> clauses, List<object> bindings)
    {
        ArgumentNullException.ThrowIfNull(clauses);
        ArgumentNullException.ThrowIfNull(bindings);

        if (clauses.Count == 0) return string.Empty;

        var stringBuilder = new StringBuilder("WHERE ");

        for (var i = 0; i < clauses.Count; i++)
        {
            var clause = clauses[i];
            var condition = clause.WhereCondition;

            if (i > 0)
                stringBuilder.Append(' ')
                    .Append(_paramIdentifier.GetLogicalOperatorString(clause.LogicalOperator))
                    .Append(' ');

            var column = _paramIdentifier.WrapIdentifier(condition.Column);
            var operatorSymbol = _expressionOperator.GetSymbol(condition.Operator);
            var paramPlaceholder = _paramIdentifier.FormatParameter(bindings.Count);
            var processedValue = _paramIdentifier.TransformValue(condition.Value);

            stringBuilder.Append(column)
                .Append(' ')
                .Append(operatorSymbol)
                .Append(' ')
                .Append(paramPlaceholder);

            bindings.Add(processedValue);
        }

        return stringBuilder.ToString();
    }
}