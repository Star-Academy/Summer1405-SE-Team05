namespace CleanCode;

internal sealed record WhereCondition(string Column, ExpressionOperator Operator, object Value);