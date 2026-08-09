namespace CleanCode;

public interface IParameterIdentifier
{
    string ColumnSeparator { get; }

    string WrapIdentifier(string identifier);
    string FormatParameter(int index);
    string GetLogicalOperatorString(LogicalOperator logicalOp);
    string FormatParameterName(int index);

    object TransformValue(object value)
    {
        return value;
    }
}