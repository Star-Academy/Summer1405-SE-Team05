namespace CleanCode;

public class SqlServerParameterIdentifier : IParameterIdentifier
{
    public string ColumnSeparator => ", ";

    public string WrapIdentifier(string identifier)
    {
        return $"[{identifier}]";
    }

    public string FormatParameter(int index)
    {
        return $"@p{index}";
    }
    public string FormatParameterName(int index) => $"@p{index}";

    public string GetLogicalOperatorString(LogicalOperator logicalOp)
    {
        return logicalOp.ToString().ToUpper();
    }
}
