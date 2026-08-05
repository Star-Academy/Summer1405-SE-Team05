namespace CleanCode;

public record WhereCondition(string Column, ExpressionOperator Operator, object Value);