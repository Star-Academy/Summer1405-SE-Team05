
namespace CleanCode;

internal sealed class SqlSelectBuilder : ISelectBuilder
{
    public string Build(List<string> columns, IParameterIdentifier paramIdentifier)
    {
        var cols = columns.Count > 0
            ? string.Join(paramIdentifier.ColumnSeparator, columns.Select(paramIdentifier.WrapIdentifier))
            : "*";

        return $"SELECT {cols}";
    }
}