namespace CleanCode;

internal sealed class PostgresParameterIdentifier : IParameterIdentifier
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

    public string FormatParameterName(int index)
    {
        return "";
    }

    public string GetLogicalOperatorString(LogicalOperator logicalOperator)
    {
        return logicalOperator.ToString().ToUpper();
    }
}