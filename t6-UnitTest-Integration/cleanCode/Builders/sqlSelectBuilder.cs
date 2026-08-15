namespace CleanCode;

internal sealed class SqlSelectBuilder : ISelectBuilder
{
    private readonly IParameterIdentifier _paramIdentifier;

    public SqlSelectBuilder(IParameterIdentifier paramIdentifier)
    {
        _paramIdentifier = paramIdentifier ?? throw new ArgumentNullException(nameof(paramIdentifier));
    }

    public string Build(List<string> columns)
    {
        var columons = columns.Count > 0
            ? string.Join(_paramIdentifier.ColumnSeparator, columns.Select(_paramIdentifier.WrapIdentifier))
            : "*";

        return $"SELECT {columons}";
    }
}