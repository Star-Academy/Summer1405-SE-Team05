namespace t4;

public class PostgresCompiler : SqlCompiler
{
    protected override string ColumnSeparator => ", ";

    protected override string WrapIdentifier(string identifier) 
        => $"\"{identifier}\"";

    protected override string FormatParameter(int index) 
        => $"${index + 1}";

    
}