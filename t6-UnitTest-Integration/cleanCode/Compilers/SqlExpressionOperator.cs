namespace CleanCode;

public class SqlExpressionOperator : IExpressionOperator
{
    public string GetSymbol(ExpressionOperatorType operatorType)
    {
        return operatorType switch
        {
            ExpressionOperatorType.Equals => "=",
            ExpressionOperatorType.NotEquals => "<>",
            ExpressionOperatorType.GreaterThan => ">",
            ExpressionOperatorType.GreaterThanOrEqual => ">=",
            ExpressionOperatorType.LessThan => "<",
            ExpressionOperatorType.LessThanOrEqual => "<=",
            ExpressionOperatorType.Like => "LIKE",
            _ => throw new ArgumentOutOfRangeException(nameof(operatorType), $"Unsupported operator: {operatorType}")
        };
    }
}