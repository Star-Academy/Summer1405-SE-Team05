namespace t4;

public class ExpressionOperator
{
    public new static readonly ExpressionOperator Equals = new("=");
    public static readonly ExpressionOperator NotEquals = new("<>");
    public static readonly ExpressionOperator GreaterThan = new(">");
    public static readonly ExpressionOperator GreaterThanOrEqual = new(">=");
    public static readonly ExpressionOperator LessThan = new("<");
    public static readonly ExpressionOperator LessThanOrEqual = new("<=");
    public static readonly ExpressionOperator Like = new("LIKE");

    public string Symbol { get; }

    private ExpressionOperator(string symbol)
    {
        Symbol = symbol;
    }

    public override string ToString() => Symbol;
}