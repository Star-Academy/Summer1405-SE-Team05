namespace CleanCode;

public sealed record WhereCondition(string Column, ExpressionOperatorType Operator, object Value);