namespace CleanCode;

public class PostgresParameterIdentifier : IParameterIdentifier
{
    public string ColumnSeparator => ", ";

    public string WrapIdentifier(string identifier)
    {
        return $"\"{identifier}\"";
    }

    public string FormatParameter(int index)
    {
        return $"${index + 1}";
    }
    public string FormatParameterName(int index) => "";

    public string GetLogicalOperatorString(LogicalOperator logicalOp)
    {
        return logicalOp.ToString().ToUpper();
    }
}
