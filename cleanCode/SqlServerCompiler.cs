namespace t4;

public class SqlServerCompiler : SqlCompiler
{
    protected override string ColumnSeparator => ", ";

    protected override string WrapIdentifier(string identifier) 
        => $"[{identifier}]";

    protected override string FormatParameter(int index) 
        => $"@p{index}";

    

    protected override object TransformValue(object value)
    {
        if (value is bool b)
        {
            return b ? 1 : 0;
        }
        return base.TransformValue(value);
    }
}