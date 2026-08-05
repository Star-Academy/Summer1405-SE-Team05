
namespace CleanCode;

internal sealed class SqlFromBuilder : IFromBuilder
{
    public string Build(string? table, IParameterIdentifier paramIdentifier)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentException("Table name cannot be null or empty.");

        return $"FROM {paramIdentifier.WrapIdentifier(table)}";
    }
}